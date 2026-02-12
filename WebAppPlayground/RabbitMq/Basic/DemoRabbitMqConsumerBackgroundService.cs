using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebAppShared.RabbitMq.Shared;

namespace WebAppPlayground.RabbitMq.Basic;

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
            var orderPlaced = (OrderPlaced)JsonSerializer.Deserialize(
                body,
                OrderPlacedReferences.GetOrderPlacedType())!;

            Console.WriteLine($"[<<<] Received OrderPlaced: {orderPlaced}");
            await Task.Delay(5_000, cancellationToken); // Simulate processing time

            // 3.2. Acknowledge the message as processed
            await sender.Channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            Console.WriteLine($"[<<<] Acknowledged deliveryTag={eventArgs.DeliveryTag}");
        }
        catch (OperationCanceledException)
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