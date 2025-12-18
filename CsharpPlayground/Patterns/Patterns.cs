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

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    private static string Describe(Direction d) => d switch
    {
        Direction.Up => "↑",
        Direction.Down => "↓",
        Direction.Left => "←",
        Direction.Right => "→"
    };

    private static object? GetObject1() => "Hello world";
    private static object? GetObject2() => 1998;

    private static Point GetPoint1() => new Point(10, 20);
    private static Point GetPoint2() => new Point(-10, -20);

    public static void Run()
    {
        // Null check pattern
        if (GetObject1() is null)
        {
            Console.WriteLine("Got a null object");
        }

        // 1. ======================== type pattern ========================
        // [part 1] C# type patterns with 'is' (C# 7.0+).
        // In C#, the is operator is used with type patterns to check if the runtime type of an expression
        // is compatible with a given type and, optionally, to declare a new variable of that type.
        // This feature, introduced in C# 7.0 and enhanced in later versions, provides a more concise way
        // to perform type checking and conditional variable assignment.
        DemoTypePatterns();

        // 2. ======================== constant / relational pattern ========================
        // [part 2] C# constant and relational patterns
        // In C#, constant and relational patterns are features of pattern matching used to test if an input value
        // matches a specific constant value or falls within a certain range or comparison condition. 
        //
        // # Constant Patterns
        // A constant pattern tests an input expression against a constant value.
        // This is a simple form of pattern matching and essentially behaves like an equality check (==). 
        // You can use constant patterns with the is operator, in switch statements, and switch expressions. 
        //
        // # Relational Patterns
        // Relational patterns allow you to compare an input expression to a constant value using relational operators,
        // such as <, >, <=, and >=. This feature was introduced in C# 9.0.
        DemoConstantAndRelationPatterns();

        // 3. ======================== property pattern ========================
        // [part 3] C# property patterns
        // C# property patterns are a pattern matching feature that allows you to match an object based on the values of its properties or fields.
        // Introduced in C# 8.0 and enhanced in subsequent versions, they provide a concise, readable way to inspect complex data structures and simplify nested conditional logic. 
        //
        // A property pattern tests whether an expression is non-null and whether its specified properties or fields match nested patterns.
        // This eliminates the need for explicit null checks and multiple, nested if statements, transforming imperative code into a more declarative style.
        // Property patterns can be used in is expressions, switch statements, and switch expressions. 
        // You define property patterns using curly braces {} after the type or variable name, followed by the properties you want to check.
        //
        // C# 10 introduced extended property patterns, which allow you to reference nested properties directly using a dot notation, making patterns even cleaner.
        var point1 = GetPoint1();
        var point2 = GetPoint2();
        var circle = new Circle { Center = new Point(X: 2, Y: 1), Radius = 20.0 };
        DemoPropertyPatterns(point1, point2, circle);

        // 4. ======================== positional pattern ========================
        // Positional patterns were introduced in C# 8.0 as a pattern matching enhancement
        // that allows deconstructing an object to match against its components.
        //
        // The `Deconstruct` method is central to how positional patterns work.
        // When a positional pattern is used in your code, the compiler translates it into a call to the corresponding `Deconstruct` method.
        // This method takes out parameters to unpack the object's properties into separate variables, which are then used for the pattern matching comparison.
        //
        // For example, when you have a line of code like if (point1 is (var X, var Y)),
        // the compiler effectively generates the underlying call to the `Deconstruct` method: `point1.Deconstruct(out X, out Y)`.
        // This mechanism means that any class or struct you define can support positional patterns simply by providing a suitable,
        // accessible Deconstruct method.

        // 4.1. Positional pattern with Records
        DemoPositionalPatternsWithRecords(point1, point2);

        // 4.2. Positional pattern with Tuples
        DemoPositionalPatternsWithValueTuples();

        // 4.3. Positional pattern with user-defined Deconstruct method
        DemoPositionalPatternsWithUserdefinedDeconstruct();

        // 5. ======================== logical pattern ========================
        DemoLogicalPatterns();

        // 6. ======================== list pattern ========================
        // 6. List patterns
        DemoListPatterns();

        var range = new Range(start: new Index(2, false), end: new Index(0, true));
        var offsetAndLength = range.GetOffsetAndLength(10);
        var (offset, length) = offsetAndLength;
        (int first, int second) anotherTuple = (first: 10, second: 10);

        anotherTuple = offsetAndLength;
        // => (int first, int second) is the same as (int Offset, int Length)
        // => Why? Because C# matches tuple element names by position, not by name.
        // Tuple element names in C# are just "labels", not "type identifiers"

        DemoVarPattern();
        DemoSwitchExpressionWithEnums();
    }

    private static void DemoTypePatterns()
    {
        if (GetObject1() is string s)
            Console.WriteLine($"It's a string: {s}");
        else
            Console.WriteLine("Not a string");

        if (GetObject2() is int n)
            Console.WriteLine($"It's an int {n}");
        else
            Console.WriteLine("Not an int greater than 50");

        switch (GetObject1())
        {
            case string s1:
                Console.WriteLine($"[switch] It's a string: {s1}");
                break;
            case int n1:
                Console.WriteLine($"[switch] It's an int: {n1}");
                break;
        }

        var des = GetObject1() switch
        {
            string s2 => $"[switch expression] It's a string: {s2}",
            int n2 => $"[switch expression] It's an int: {n2}",
            _ => "[switch expression] Unknown type"
        };
        Console.WriteLine(des);
    }

    private static void DemoConstantAndRelationPatterns()
    {
        var randomNumber = Random.Shared.Next(-100, 101); // [-100, 100]
        var randomDesc = randomNumber switch
        {
            0 => "Zero",
            < 0 => "Negative number",
            <= 50 => "Positive number less than or equal to 50",
            _ => "Positive number greater than 50",
        };
        Console.WriteLine($"Random number {randomNumber} is described as: {randomDesc}");

        var oneOrTwoOrThree = Random.Shared.Next(1, 4); // [1, 3]

        if (oneOrTwoOrThree is 1)
            Console.WriteLine("It's one");
        else if (oneOrTwoOrThree is 2)
            Console.WriteLine("It's two");
        else if (oneOrTwoOrThree is 3)
            Console.WriteLine("It's three");

        switch (oneOrTwoOrThree)
        {
            case 1:
                Console.WriteLine("It's one");
                break;
            case 2:
                Console.WriteLine("It's two");
                break;
            case 3:
                Console.WriteLine("It's three");
                break;
        }
    }

    private static void DemoPropertyPatterns(Point point1, Point point2, Circle circle)
    {
        var point1Desc = point1 switch
        {
            { X: 0, Y: 0 } => "Point is at the origin",
            { X: > 0 } => "Point is in the right half-plane",
            { X: < 0 } => "Point is in the left half-plane",
            _ => "Point lies on the Y axis"
        };
        var point2Desc = point2 switch
        {
            { X: 0, Y: 0 } => "Point is at the origin",
            { Y: > 0 } => "Point is in the upper half-plane",
            { Y: < 0 } => "Point is in the lower half-plane",
            _ => "Point lies on the X axis"
        };
        Console.WriteLine($"Point1: {point1} => {point1Desc}");
        Console.WriteLine($"Point2: {point2} => {point2Desc}");

        var circleDesc = circle switch
        {
            { Center: { X: 0, Y: 0 }, Radius: 10 } =>
                "Circle is centered at origin with radius 10",
            { Center: { X: var x, Y: > 0 and var y }, Radius: > 10 } when x > y =>
                $"Circle is centered at ({x}, {y}) with radius greater than 10 and x > y > 0",
            _ => "Some other circle"
        };
        Console.WriteLine($"circle: Center={circle.Center}, Radius={circle.Radius} => {circleDesc}");

        // C# 10 introduced extended property patterns, which allow you to reference nested properties directly using a dot notation,
        // making patterns even cleaner.
        var extendedPropertyPattern = circle switch
        {
            { Center.X: > 0 } => "Circle.Center.X is positive",
            { Center.X: < 0 } => "Circle.Center.X is negative",
            _ => "Circle.Center.X is zero"
        };
        Console.WriteLine(extendedPropertyPattern);

        // Using _ to ignore certain properties
        var ignoreSomeProperties = point1 switch
        {
            { X: _, Y: > 0 } => "point1.Y is positive",
            { X: _, Y: < 0 } => "point1.Y is negative",
            _ => "point1.Y is zero"
        };
    }

    private static void DemoPositionalPatternsWithRecords(Point point1, Point point2)
    {
        // 4.1. Positional pattern with Records
        // Under the hood, the `Deconstruct` method is called: `point1.Deconstruct(out X, out Y)`.
        //
        // Synthesized `Deconstruct` Method: When you define a record with positional parameters (e.g., record Point(int X, int Y)),
        // the compiler synthesizes a public void `Deconstruct` method.

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
    }

    private static void DemoPositionalPatternsWithValueTuples()
    {
        // 4.2. Positional pattern with Tuples
        // Under the hood, the `Item1`, `Item2`, ... properties are accessed.
        //
        // In C#, a positional pattern for a tuple accesses the underlying Item1, Item2, etc., properties automatically.
        // This pattern allows for deconstructing the tuple based on its position.

        var myTuple1 = (10, 20); // ValueTuple<int, int> myTuple1;
        var myTuple1Desc = myTuple1 switch
        {
            (0, 0) => "Both elements are zero",
            (> 0, _) => "First element is positive",
            _ => "First element is non-positive",
        };

        // ValueTuple<int, int, int, int, int, int, int, ValueTuple<int, int>> myTuple2;
        var myTuple2 = (1, 2, 3, 4, 5, 6, 7, 8, 9);
        var myTuple2Desc = myTuple2 switch
        {
            (0, 0, 0, 0, 0, 0, 0, 0, 0) => "All elements are zero",
            (> 0, _, _, _, _, _, _, _, _) => "First element is positive",
            _ => "First element is non-positive",
        };
        Console.WriteLine($"myTuple1: {myTuple1} and {myTuple1Desc}");
        Console.WriteLine($"myTuple2: {myTuple2} and {myTuple2Desc}");
    }

    private static void DemoPositionalPatternsWithUserdefinedDeconstruct()
    {
        // 4.3. Positional pattern with user-defined Deconstruct method
        // Under the hood, the `Deconstruct` method is called: `circle2.Deconstruct(out Center, out Radius)`.
        // Make sure the Deconstruct method is defined in the class with correct accessibility and signature:
        // public void Deconstruct(out Point center, out double radius)

        var circle2 = new Circle
        {
            Center = new Point(0, 0),
            Radius = 10.0
        };
        var circle2Desc = circle2 switch
        {
            ((0, 0), 10) => "Circle is centered at origin with radius 10",
            ((var x, > 0), > 10) => $"Circle is centered at ({x}, y>0) with radius greater than 10",
            _ => "Some other circle",
        };
        Console.WriteLine($"circle2: Center={circle2.Center}, Radius={circle2.Radius} and {circle2Desc}");
    }

    private static void DemoLogicalPatterns()
    {
        // 5. Logical pattern: and, or, not
        // C# offers logical patterns (and, or, and not) to combine multiple patterns within
        // is expressions and switch statements/expressions, allowing for more complex and readable conditional logic. 
        // These patterns were introduced in C# 9.0 to complement existing pattern matching features. 

        var point3 = new Point(15, 25);
        var point3Desc = point3 switch
        {
            (> 0, > 0) and (< 20, < 30) => "Point is in the first quadrant within (0,0) and (20,30)",
            (< 0, < 0) or (> 100, > 100) => "Point is either in the third quadrant or beyond (100,100)",
            not (0, 0) => "Point is not at the origin",
            _ => "Some other point",
        };
        Console.WriteLine($"Point3: {point3} and {point3Desc}");

        var anInt = Random.Shared.Next(-100, 101); // [-100, 100]
        // Using 'and' pattern to check if value is within a range
        if (anInt is > 0 and < 50) Console.WriteLine("anInt is a positive number less than 50");

        // Using 'or' pattern to check for multiple specific values
        if (anInt is 0 or 8 or 10) Console.WriteLine("anInt must be 0 or 8 or 10");

        // Using 'not' pattern to check for non-negative values
        if (anInt is not < 0) Console.WriteLine("anInt is not negative");
        if (anInt is not (3 or 4)) Console.WriteLine("anInt is neither 3 nor 4");
        if (GetObject1() is not null) Console.WriteLine("Got a non-null object");
    }

    private static void DemoListPatterns()
    {
        // 6. List patterns
        // C# list patterns, introduced in C# 11, enable concise and readable pattern matching
        // against the sequence and elements of arrays, Span<T>, and ReadOnlySpan<T> collections.
        // They replace complex for loops or if-else statements for structural checks.
        //
        // - Discard: `_` matches any single element without checking its value.
        // - Range (Slice): `..` matches a sequence of zero or more elements. It can only appear once in a pattern.
        // - Var: `var x` captures the matched element(s) into a new variable `x`.

        var msg1 = GetListDescription([1, 2]);
        var msg2 = GetListDescription([1, 2, 3]);
        var msg3 = GetListDescription([0]);
        var msg4 = GetListDescription([5, 9]);
        var msg5 = GetListDescription([]);
        var msg6 = GetListDescription([7, 8, 9]);

        foreach (var m in new[] { msg1, msg2, msg3, msg4, msg5, msg6 })
            Console.WriteLine(m);
        return;

        static string GetListDescription(List<int> ints) =>
            ints switch
            {
                [1, 2, .. var rest] => $"starts with 1,2 and {rest.Count} more",
                [0] => "just zero",
                [5, var x] => $"starts with 5 and then {x}",
                [] => "empty",
                _ => "something else"
            };
    }

    private static void DemoVarPattern()
    {
        object obj = 42;

        var desc = obj switch
        {
            // Var pattern: always matches any value and captures it into a new variable
            var x => $"var pattern: {x}"
        };
        Console.WriteLine(desc);

        var desc2 = obj switch
        {
            // The code below also uses var pattern for demonstration,
            // but it's not the best practice since it always matches any value.
            // <=> var x and int
            // <=> int x
            // Better pattern: int x
            var x when x is int => $"It's an int: {x}",
            var x => $"Some other object: {x}"
        };
        Console.WriteLine(desc2);
    }

    private static void DemoSwitchExpressionWithEnums()
    {
        var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        foreach (var dir in directions)
            Console.WriteLine(Describe(dir));

        try
        {
            Describe((Direction)100);
        }
        catch (System.Runtime.CompilerServices.SwitchExpressionException ex)
        {
            Console.WriteLine($"Caught exception: {ex.Message}");
        }
    }
}