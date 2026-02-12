using RabbitMQ.Client;

namespace WebAppPlayground.RabbitMq.TopicWithDlx;

public static class TopicWithDlxConfig
{
    public const string ExchangeName = "orders-exchange";
    public const string QueueName = "orders-consumer-1";
    public const string Binding = "orders.*";

    public const string DeadletterExchange = "orders-deadletter-exchange";
    public const string DeadletterQueue = "orders-deadletter-queue";

    public static async Task SetupForDlxAsync(this IChannel channel)
    {
        // 1. Declare the dead-letter exchange
        await channel.ExchangeDeclareAsync(
            exchange: DeadletterExchange,
            type: ExchangeType.Fanout);

        // 2. Declare the dead-letter queue
        await channel.QueueDeclareAsync(
            queue: DeadletterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // 3. Bind the dead-letter queue to the dead-letter exchange
        await channel.QueueBindAsync(
            queue: DeadletterQueue,
            exchange: DeadletterExchange,
            routingKey: ""); // Routing key can be empty for fanout 
    }
}