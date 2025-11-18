using System.Runtime.CompilerServices;

namespace CsharpPlayground;

public static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PrintSeparator() =>
        // Similar to Kotlin's "-".repeat(30)
        Console.WriteLine(new string('-', 30));
}

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsPrintable(this object? value)
        =>
            value?.ToString() ?? "null";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsPrintable<T>(this T value)
        where T : struct
        =>
            value.ToString() ?? "null";
}