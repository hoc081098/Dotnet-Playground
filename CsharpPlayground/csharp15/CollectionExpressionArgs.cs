namespace CsharpPlayground.csharp15;

public static class CollectionExpressionArgs
{
    public static void Run()
    {
        List<int> values = [1, 2, 3, 4, 5];

        // Pass `values.Count` to `List(int capacity)` ctor.
        List<int> copiedValues = [with(capacity: values.Count), .. values];

        // Pass `StringComparer.OrdinalIgnoreCase` to `HashSet(IEqualityComparer<T>? comparer)` ctor.
        HashSet<string> names =
        [
            with(comparer: StringComparer.OrdinalIgnoreCase),
            "Hoc",
            "HOC",
            "HoC",
            "CSharp"
        ];
        
        Console.WriteLine($"copiedValues: {string.Join(", ", copiedValues)}");
        Console.WriteLine($"names: {string.Join(", ", names)}");
    }
}