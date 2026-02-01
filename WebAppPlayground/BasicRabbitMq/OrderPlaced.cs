namespace WebAppPlayground.BasicRabbitMq;

public sealed record OrderPlaced(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAtUtc);