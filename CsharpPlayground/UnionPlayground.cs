using System.Runtime.CompilerServices;

namespace CsharpPlayground;

public static class UnionPlayground
{
    public sealed record Error(string Code, string Message);

    public union Result<T>(T, Error);

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
    }

    // [Union]
    // public class Result<T>
    // {
    //     public Result(T value)
    //     {
    //         Value = value;
    //     }
    //
    //     public Result(Error error)
    //     {
    //         Value = error;
    //     }
    //
    //     public object? Value { get; init; }
    // }
}