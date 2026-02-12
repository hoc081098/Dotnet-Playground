namespace WebAppPlayground.RabbitMq.TopicWithDlx;

public static class TopicWithDlxConfig
{
    public const string ExchangeName = "orders-exchange";
    public const string QueueName = "orders-consumer-1";
    public const string Binding = "orders.*";
}