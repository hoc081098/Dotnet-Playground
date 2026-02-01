using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebAppPlayground;

public sealed record OrderPlaced(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAtUtc);

public class DemoRabbitMqBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory();

        // Connections are meant to be long-lived.
        // The underlying protocol is designed and optimized for long running connections.
        // That means that opening a new connection per operation, e.g. a message published, is unnecessary
        // and strongly discouraged as it will introduce a lot of network roundtrips and overhead.
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        // Channels are also meant to be long-lived but since many recoverable protocol errors will result in channel closure,
        // channel lifespan could be shorter than that of its connection.
        // Closing and opening new channels per operation is usually unnecessary but can be appropriate.
        // When in doubt, consider reusing channels first.
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // 1. Declare a queue: ensure it exists (create it if not already existing)
        var queueDeclareOk = await channel.QueueDeclareAsync(
            queue: "orders", // Name of the queue
            durable: true, // Save to disk so the queue is not lost on broker restart
            autoDelete: false, // Do not delete the queue when the last consumer disconnects
            exclusive: false, // Can be used by other connections
            arguments: null,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[>>>] Queue declared: {queueDeclareOk.QueueName}");

        // 2. Publish messages to the queue
        for (var i = 0; i < 10; i++)
        {
            // 2.1. Create the message, then serialize it to UTF-8 bytes (JSON)
            var orderPlaced = new OrderPlaced(
                OrderId: Guid.CreateVersion7(),
                TotalAmount: 99.99m + i,
                CreatedAtUtc: DateTimeOffset.UtcNow);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(orderPlaced);

            // 2.2. Publish the message to the "orders" queue
            await channel.BasicPublishAsync(
                // Default exchange: it routes to queue with the name of the routing key (EQUALITY)
                exchange: string.Empty,
                routingKey: "orders",
                // The message must route to a queue (fail if it can't)
                mandatory: true,
                basicProperties: new BasicProperties
                {
                    // Message will be persisted to disk
                    Persistent = true
                },
                body: bytes,
                cancellationToken: stoppingToken);

            Console.WriteLine($"[>>>] Sent OrderPlaced message: {orderPlaced}");
            await Task.Delay(10_000, stoppingToken);
        }
    }
}

public class DemoRabbitMqConsumerBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory();

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // 1. Declare a queue: ensure it exists (create it if not already existing)
        var queueDeclareOk = await channel.QueueDeclareAsync(
            queue: "orders", // Name of the queue
            durable: true, // Save to disk so the queue is not lost on broker restart
            autoDelete: false, // Do not delete the queue when the last consumer disconnects
            exclusive: false, // Can be used by other connections
            arguments: null,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[<<<] Queue declared: {queueDeclareOk.QueueName}");

        // 2. Set prefetch count to 1 to process one message at a time
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        // 3. Declare a consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (sender, eventArgs) =>
            HandleMessageAsync((AsyncEventingBasicConsumer)sender, eventArgs, stoppingToken);

        // 4. Start consuming messages from the queue
        // this consumer tag identifies the subscription when it has to be cancelled
        var consumerTag = await channel.BasicConsumeAsync(
            queue: "orders",
            // false <=> manual acknowledgements.
            // We only acknowledge messages when processed successfully.
            // If processing fails, we could use `BasicNack` to requeue or route to a dead-letter queue
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[<<<] Consumer tagged: {consumerTag}");

        // 5. Wait until stopping is requested
        // If we do not wait here, the channel and connection will be disposed immediately -> cannot acknowledge messages
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static async Task HandleMessageAsync(
        AsyncEventingBasicConsumer sender,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        try
        {
            // 3.1. Copy the body to a new array to make it safe to use outside this event,
            // and then parse it to an OrderPlaced instance
            var body = eventArgs.Body.ToArray(); // bodyCopy is now safe to use elsewhere
            var orderPlaced = JsonSerializer.Deserialize<OrderPlaced>(body)!;

            Console.WriteLine($"[<<<] Received OrderPlaced: {orderPlaced}");
            await Task.Delay(5_000, cancellationToken); // Simulate processing time

            // 3.2. Acknowledge the message as processed
            await sender.Channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            Console.WriteLine($"[<<<] Acknowledged deliveryTag={eventArgs.DeliveryTag}");
        }
        catch (OperationCanceledException ex)
        {
            // Ignore cancellation exceptions
            throw;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            Console.WriteLine($"[<<<] Rejecting RabbitMQ message due to invalid format: {ex.Message}");

            // 3.3. Reject the message (nack) without requeuing - it's an invalid/poison message
            await sender.Channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                // INVALID/POISON MESSAGE - do not requeue
                requeue: false,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // 3.4. On any other error, nack the message with requeue = true
            Console.WriteLine($"[<<<] Requeuing RabbitMQ message due to processing error: {ex.Message}");

            await sender.Channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                // Requeue the message for another processing attempt
                requeue: true,
                cancellationToken: cancellationToken);
        }
    }
}