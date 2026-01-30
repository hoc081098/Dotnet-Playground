using System.Text;
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

        // 2. Declare a consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            // 2.1. Copy the body to a new array to make it safe to use outside this event,
            // and then parse it to an OrderPlaced instance
            var body = eventArgs.Body.ToArray(); // bodyCopy is now safe to use elsewhere
            var orderPlaced = JsonSerializer.Deserialize<OrderPlaced>(body)!;

            Console.WriteLine($"[<<<] Received OrderPlaced: {orderPlaced}");
            await Task.Delay(5_000, stoppingToken); // Simulate processing time

            // 2.2. Acknowledge the message as processed
            await ((AsyncEventingBasicConsumer)sender).Channel
                .BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

            Console.WriteLine($"[<<<] Acknowledged deliveryTag={eventArgs.DeliveryTag}");
        };

        // 3. Start consuming messages from the queue
        // this consumer tag identifies the subscription when it has to be cancelled
        var consumerTag = await channel.BasicConsumeAsync(
            queue: "orders",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[<<<] Consumer tagged: {consumerTag}");

        // 4. Wait until stopping is requested
        // If we do not wait here, the channel and connection will be disposed immediately -> cannot acknowledge messages
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}