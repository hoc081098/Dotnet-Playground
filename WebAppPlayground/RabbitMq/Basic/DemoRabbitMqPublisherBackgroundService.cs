using System.Text.Json;
using RabbitMQ.Client;
using WebAppShared.RabbitMq.Shared;

namespace WebAppPlayground.RabbitMq.Basic;

public class DemoRabbitMqPublisherBackgroundService : BackgroundService
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