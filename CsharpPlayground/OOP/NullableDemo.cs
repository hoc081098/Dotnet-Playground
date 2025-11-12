namespace CsharpPlayground.OOP;

public class NullableDemo
{
    public static void Run()
    {
        // int? === Nullable<int>

        // Under the hood (use constructor)
        // - `new Nullable<int>();`
        // - OR `new int?();`
        Nullable<int> n = null;

        // Under the hood (use implicit operator)
        // - `new Nullable<int>(2)`;
        // - OR `new int?(2)`;
        n = 2;

        // Under the hood: `n.GetValueOrDefault(100);`
        var nonNull1 = n ?? 100;
        // Under the hood: `n.HasValue ? n.GetValueOrDefault() : ComputeIntValue();`
        var nonNull2 = n ?? ComputeIntValue();
        // Under the hood: `n.Value;` (use explicit operator)
        // throws an InvalidOperationException if n.HasValue == false (i.e., n is null).
        var nonNull3 = (int)n!;

        Console.WriteLine($"n = {n}, nonNull1 = {nonNull1}, nonNull2 = {nonNull2}, nonNull3 = {nonNull3}");
    }

    private static int ComputeIntValue() => Random.Shared.Next();
}