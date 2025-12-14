namespace CsharpPlayground.Patterns;

public static class Patterns
{
    private sealed record Point(int X, int Y);

    private sealed class Circle
    {
        public required Point Center { get; init; }
        public required double Radius { get; init; }

        // Deconstruct method required for positional patterns
        public void Deconstruct(out Point center, out double radius)
        {
            center = Center;
            radius = Radius;
        }
    }

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

        // 3. ======================== property pattern ========================
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

        var circle1 = new Circle
        {
            Center = new Point(0, 0),
            Radius = 10.0,
        };
        var circle1Desc = circle1 switch
        {
            { Center: { X: 0, Y: 0 }, Radius: 10 } => "Circle is centered at origin with radius 10",
            { Center: { X: var x, Y: > 0 }, Radius: > 10 } =>
                $"Circle is centered at ({x}, y>0) with radius greater than 10",
            _ => "Some other circle"
        };
        Console.WriteLine($"circle1: Center={circle1.Center}, Radius={circle1.Radius} and {circle1Desc}");

        // 3. ======================== positional pattern ========================

        // 3.1. Positional pattern with Records
        // Under the hood, the `Deconstruct` method is called: point1.Deconstruct(out X, out Y)
        var point1DescPositional = point1 switch
        {
            (0, 0) => "Point is at the origin",
            (> 0, _) => "Point is in the right half-plane",
            _ => "Point is in the left half-plane",
        };
        var point2DescPositional = point2 switch
        {
            (0, 0) => "Point is at the origin",
            (_, > 0) => "Point is in the upper half-plane",
            _ => "Point is in the lower half-plane",
        };
        Console.WriteLine($"Point1: {point1} and {point1DescPositional}");
        Console.WriteLine($"Point2: {point2} and {point2DescPositional}");

        // 3.2. Positional pattern with Tuples
        // Under the hood, the `Item1`, `Item2`, ... properties are accessed.
        var myTuple1 = (10, 20); // ValueTuple<int, int>
        var myTuple1Desc = myTuple1 switch
        {
            (0, 0) => "Both elements are zero",
            (> 0, _) => "First element is positive",
            _ => "First element is non-positive",
        };

        // ValueTuple<int, int, int, int, int, int, int, ValueTuple<int, int>>
        var myTuple2 = (1, 2, 3, 4, 5, 6, 7, 8, 9);
        var myTuple2Desc = myTuple2 switch
        {
            (0, 0, 0, 0, 0, 0, 0, 0, 0) => "All elements are zero",
            (> 0, _, _, _, _, _, _, _, _) => "First element is positive",
            _ => "First element is non-positive",
        };

        Console.WriteLine($"myTuple1: {myTuple1} and {myTuple1Desc}");
        Console.WriteLine($"myTuple2: {myTuple2} and {myTuple2Desc}");

        // 3.3. Positional pattern with Deconstruct method in class
        var circle2 = new Circle
        {
            Center = new Point(0, 0),
            Radius = 10.0
        };
        var circle2Desc = circle1 switch
        {
            ((0, 0), 10) => "Circle is centered at origin with radius 10",
            ((var x, > 0), > 10) => $"Circle is centered at ({x}, y>0) with radius greater than 10",
            _ => "Some other circle",
        };
        Console.WriteLine($"circle2: Center={circle2.Center}, Radius={circle2.Radius} and {circle2Desc}");
    }
}