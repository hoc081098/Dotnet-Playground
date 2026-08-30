using System.Runtime.CompilerServices;

namespace CsharpPlayground.csharp15;

public static class UnionPlayground
{
    public sealed record Error(string Code, string Message);

    public union Result<T>(T, Error);

    [Union]
    public class Result2<T>
    {
        public Result2(T value) => Value = value;

        public Result2(Error error) => Value = error;

        public object? Value { get; init; }
    }

    public static void Run()
    {
        Result<int> successResult = 42;
        Result<int> errorResult = new Error("User.NotFound", "Not Found");

        Console.WriteLine(successResult switch
        {
            int value => $"Success: {value}",
            Error error => $"Error: {error}"
        });

        Console.WriteLine(errorResult switch
        {
            int value => $"Success: {value}",
            Error error => $"Error: {error}"
        });

        Console.WriteLine("---");

        Result2<int> successResult2 = 42;
        Result2<int> errorResult2 = new Error("User.NotFound", "Not Found");

        Console.WriteLine(successResult2 switch
        {
            int value => $"Success: {value}",
            Error error => $"Error: {error}"
        });

        Console.WriteLine(errorResult2 switch
        {
            int value => $"Success: {value}",
            Error error => $"Error: {error}"
        });
    }
}