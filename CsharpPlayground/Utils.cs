using System.Runtime.CompilerServices;

namespace CsharpPlayground;

public static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PrintSeparator() =>
        Console.WriteLine(new string('-', 30));
}