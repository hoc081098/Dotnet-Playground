using JetBrains.Annotations;

namespace WebAppShared.RabbitMq.Shared;

[UsedImplicitly]
public sealed record OrderPlaced(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAtUtc);

public static class OrderPlacedReferences
{
    public static Type GetOrderPlacedType() => typeof(OrderPlaced);
}