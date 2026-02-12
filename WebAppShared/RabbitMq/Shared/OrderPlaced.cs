using System.Reflection;

namespace WebAppShared.RabbitMq.Shared;

public sealed record OrderPlaced(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAtUtc);

public static class OrderPlacedReferences
{
    private static readonly Assembly Assembly = typeof(OrderPlacedReferences).Assembly;
    private static readonly string TypeFullName = typeof(OrderPlaced).FullName!;

    public static Type GetOrderPlacedType() => Assembly.GetType(TypeFullName)!;
}