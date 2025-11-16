using System.Runtime.CompilerServices;

namespace CsharpPlayground;

public static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PrintSeparator() =>
        // Similar to Kotlin's "-".repeat(30)
        Console.WriteLine(new string('-', 30));
}