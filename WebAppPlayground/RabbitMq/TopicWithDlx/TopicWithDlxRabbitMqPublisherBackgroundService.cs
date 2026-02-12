using System.Text;
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

        Console.WriteLine($"[>>>] Exchange declared: {TopicWithDlxConfig.ExchangeName}");

        // 2. Publish messages to the exchange
        for (var i = 0; i < 10; i++)
        {
            var task = (i % 3) switch
            {
                0 => PublishOrderPlacedAsync(channel, i, stoppingToken),
                1 => PublishOtherMessageAsync(channel, i, stoppingToken),
                _ => PublishPoisonOrderPlacedAsync(channel, stoppingToken),
            };
            await task;
            await Task.Delay(10_000, stoppingToken);
        }
    }

    private static async Task PublishOrderPlacedAsync(IChannel channel,
        int i,
        CancellationToken stoppingToken)
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
    }

    private async Task PublishOtherMessageAsync(IChannel channel, int i, CancellationToken stoppingToken)
    {
        // 2.1. Create a different type of message, then serialize it to UTF-8 bytes (JSON)
        var otherMessage = new
        {
            MessageId = Guid.CreateVersion7(),
            Content = $"This is other message number {i}",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(otherMessage);

        // 2.2. Publish the message to the "orders_exchange" exchange
        // with routing key "orders.other"

        const string routingKey = "orders.other";
        await channel.BasicPublishAsync(
            exchange: TopicWithDlxConfig.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: bytes,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[>>>] Sent Other message: {otherMessage}");
    }

    private static async Task PublishPoisonOrderPlacedAsync(
        IChannel channel,
        CancellationToken stoppingToken)
    {
        const string invalidOrderPlacedJson = "123-invalid-json"; // Invalid JSON to simulate a poison message
        var bytes = Encoding.UTF8.GetBytes(invalidOrderPlacedJson);

        const string routingKey = "orders.placed";
        await channel.BasicPublishAsync(
            exchange: TopicWithDlxConfig.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: bytes,
            cancellationToken: stoppingToken);

        Console.WriteLine("[>>>] Sent poison OrderPlaced payload to trigger DLQ.");
    }
}