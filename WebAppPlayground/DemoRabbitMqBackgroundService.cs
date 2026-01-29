using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

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
        
        Console.WriteLine($"[x] Queue declared: {queueDeclareOk.QueueName}");

        // 2. Publish messages to the queue
        for (var i = 0; i < 10; i++)
        {
            // 2.1. Create the message, then serialize it to JSON, then encode it as UTF-8 bytes
            var orderPlaced = new OrderPlaced(
                OrderId: Guid.CreateVersion7(),
                TotalAmount: 99.99m + i,
                CreatedAtUtc: DateTimeOffset.UtcNow);
            var message = JsonSerializer.Serialize(orderPlaced);
            var bytes = Encoding.UTF8.GetBytes(message);

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

            Console.WriteLine($"[x] Sent OrderPlaced message: {message}");
        }
    }
}