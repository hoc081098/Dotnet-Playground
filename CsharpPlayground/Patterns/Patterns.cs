using System.Runtime.CompilerServices;

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

        // 4. ======================== positional pattern ========================

        // 4.1. Positional pattern with Records
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

        // 4.2. Positional pattern with Tuples
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

        // 4.3. Positional pattern with Deconstruct method in class
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

        // 5. ======================== logical pattern ========================
        var point3 = new Point(15, 25);
        var point3Desc = point3 switch
        {
            (> 0, > 0) and (< 20, < 30) => "Point is in the first quadrant within (0,0) and (20,30)",
            (< 0, < 0) or (> 100, > 100) => "Point is either in the third quadrant or beyond (100,100)",
            not (0, 0) => "Point is not at the origin",
            _ => "Some other point",
        };
        Console.WriteLine($"Point3: {point3} and {point3Desc}");
        var anInt = 42;
        if (anInt is > 0 and < 50)
        {
            Console.WriteLine("anInt is a positive number less than 50");
        }

        if (anInt is 0 or 8 or 10)
        {
            Console.WriteLine("anInt must be 0 or 8 or 10");
        }

        if (anInt is not < 0)
        {
            Console.WriteLine("anInt is not negative");
        }

        if (GetObject1() is null)
        {
            Console.WriteLine("Got a null object");
        }

        if (GetObject1() is not null)
        {
            Console.WriteLine("Got a non-null object");
        }

        // 5. ======================== list pattern ========================
        var msg1 = new[] { 1, 2 } switch
        {
            [1, 2, .. var rest] => $"starts with 1,2 and {rest.Length} more",
            [0] => "just zero",
            [] => "empty",
            _ => "something else"
        };
        var msg2 = new[] { 1, 2, 3 } switch
        {
            [1, 2, .. var rest] => $"starts with 1,2 and {rest.Length} more",
            [0] => "just zero",
            [] => "empty",
            _ => "something else"
        };
        var msg3 = new[] { 0 } switch
        {
            [1, 2, .. var rest] => $"starts with 1,2 and {rest.Length} more",
            [0] => "just zero",
            [] => "empty",
            _ => "something else"
        };
        var msg4 = Array.Empty<int>() switch
        {
            [1, 2, .. var rest] => $"starts with 1,2 and {rest.Length} more",
            [0] => "just zero",
            [] => "empty",
            _ => "something else"
        };
        var msg5 = new[] { 7, 8, 9 } switch
        {
            [1, 2, .. var rest] => $"starts with 1,2 and {rest.Length} more",
            [0] => "just zero",
            [] => "empty",
            _ => "something else"
        };
        Console.WriteLine("msg:" + msg1);
        Console.WriteLine("msg:" + msg2);
        Console.WriteLine("msg:" + msg3);
        Console.WriteLine("msg:" + msg4);
        Console.WriteLine("msg:" + msg5);

        var range = new Range(start: new Index(2, false), end: new Index(0, true));
        var offsetAndLength = range.GetOffsetAndLength(10);
        var (offset, length) = offsetAndLength;
        (int first, int second) anotherTuple = (first: 10, second: 10);
        
        anotherTuple = offsetAndLength;
        // => (int first, int second) is the same as (int Offset, int Length)
        // => Why? Because C# matches tuple element names by position, not by name.
        // Tuple element names in C# are just "labels", not "type identifiers"
    }
}