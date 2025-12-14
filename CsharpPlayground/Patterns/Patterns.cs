namespace CsharpPlayground.Patterns;

public static class Patterns
{
    private static object? GetObject1() => "Hello world";
    private static object? GetObject2() => 1998;

    public static void Run()
    {
        // ======================== type pattern ========================
        if (GetObject1() is string s)
        {
            Console.WriteLine($"It's a string: {s}");
        }
        else
        {
            Console.WriteLine("Not a string");
        }

        if (GetObject2() is int n)
        {
            Console.WriteLine($"It's an int {n}");
        }
        else
        {
            Console.WriteLine("Not an int greater than 50");
        }

        // ======================== constant / relational pattern ========================
        var random = Random.Shared.Next(-100, 101); // [-100, 100]
        var randomDesc = random switch
        {
            0 => "Zero",
            < 0 => "Negative number",
            <= 50 => "Positive number less than or equal to 50",
            _ => "Positive number greater than 50",
        };
        Console.WriteLine($"Random number {random} is described as: {randomDesc}");
    }
}