namespace CsharpPlayground.Patterns;

public static class Patterns
{
    private sealed record Point(int X, int Y);

    private static object? GetObject1() => "Hello world";
    private static object? GetObject2() => 1998;

    private static Point GetPoint1() => new Point(10, 20);
    private static Point GetPoint2() => new Point(-10, -20);

    public static void Run()
    {
        // 1. ======================== type pattern ========================
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

        // 2. ======================== constant / relational pattern ========================
        var random = Random.Shared.Next(-100, 101); // [-100, 100]
        var randomDesc = random switch
        {
            0 => "Zero",
            < 0 => "Negative number",
            <= 50 => "Positive number less than or equal to 50",
            _ => "Positive number greater than 50",
        };
        Console.WriteLine($"Random number {random} is described as: {randomDesc}");

        // 2. ======================== property pattern ========================
        var point1 = GetPoint1();
        var point1Desc = point1 switch
        {
            { X: 0, Y: 0 } => "Point is at the origin",
            { X: > 0 } => "Point is in the right half-plane",
            _ => "Point is in the left half-plane",
        };
        
        var point2 = GetPoint2();
        var point2Desc = point2 switch
        {
            { X: 0, Y: 0 } => "Point is at the origin",
            { Y: > 0 } => "Point is in the upper half-plane",
            _ => "Point is in the lower half-plane",
        };
        
        Console.WriteLine($"Point1: {point1} and {point1Desc}");
        Console.WriteLine($"Point2: {point2} and {point2Desc}");
    }
}