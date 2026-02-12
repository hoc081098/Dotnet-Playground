using RabbitMQ.Client;

namespace WebAppPlayground.RabbitMq.TopicWithDlx;

// orders-exchange (topic)
//    ↓
// orders-consumer-1 (queue)
//    x-dead-letter-exchange = orders-deadletter-exchange
// 
//
//
// orders-deadletter-exchange (fanout)
//    ↓
// orders-deadletter-queue
public static class TopicWithDlxConfig
{
    public const string ExchangeName = "orders-exchange";
    public const string QueueName = "orders-consumer-1";
    public const string Binding = "orders.*";

    public const string DeadletterExchange = "orders-deadletter-exchange";
    public const string DeadletterQueue = "orders-deadletter-queue";

    public static async Task SetupForDlxAsync(this IChannel channel, CancellationToken cancellationToken = default)
    {
        // 1. Declare the dead-letter exchange
        await channel.ExchangeDeclareAsync(
            exchange: DeadletterExchange,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // 2. Declare the dead-letter queue
        await channel.QueueDeclareAsync(
            queue: DeadletterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // 3. Bind the dead-letter queue to the dead-letter exchange
        await channel.QueueBindAsync(
            queue: DeadletterQueue,
            exchange: DeadletterExchange,
            routingKey: "",
            cancellationToken: cancellationToken); // Routing key can be empty for fanout 
    }
}