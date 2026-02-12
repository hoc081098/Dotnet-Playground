using System.Text.Json;
using RabbitMQ.Client;
using WebAppShared.RabbitMq.Shared;

namespace WebAppPlayground.RabbitMq.TopicWithDlx;

public class TopicWithDlxRabbitMqPublisherBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory();
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // 1. Declare a topic exchange.
        // This ensures that the exchange exists (creates it if not already existing).
        await channel.ExchangeDeclareAsync(
            exchange: TopicWithDlxConfig.ExchangeName,
            type: ExchangeType.Topic,
            durable: true, // durable exchange
            autoDelete: false, // don’t delete when the last consumer disconnects
            cancellationToken: stoppingToken);

        // 2. Publish messages to the exchange
        for (var i = 0; i < 10; i++)
        {
            // 2.1. Create the message, then serialize it to UTF-8 bytes (JSON)
            var orderPlaced = new OrderPlaced(
                OrderId: Guid.CreateVersion7(),
                TotalAmount: 99.99m + i,
                CreatedAtUtc: DateTimeOffset.UtcNow);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(orderPlaced);

            // 2.2. Publish the message to the "orders_exchange" exchange
            // with routing key "orders.placed"
            const string routingKey = "orders.placed";
            await channel.BasicPublishAsync(
                exchange: TopicWithDlxConfig.ExchangeName,
                routingKey: routingKey,
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