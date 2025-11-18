namespace CsharpPlayground.OOP;

public static class RefAndOutPlayground
{
    public static void Run()
    {
        // Demo ref
        var number = 5;
        Console.WriteLine("Before incrementing: " + number);
        Increment(ref number);
        Console.WriteLine($"Incremented number: {number}");

        // Demo out with Dictionary.TryGetValue
        var dict = new Dictionary<string, int>()
        {
            ["0"] = 100,
            ["1"] = 200
        };
        if (dict.TryGetValue("1", out var value))
        {
            Console.WriteLine("Has key '1' with value: " + value);
        }
        else
        {
            Console.WriteLine("No value found associated with key '1'");
        }

        if (dict.TryGetValue("2", out var value2))
        {
            Console.WriteLine("Has key '2' with value: " + value2);
        }
        else
        {
            Console.WriteLine("No value found associated with key '2'");
        }

        // Demo out with custom method
        Write(out var meaningOfLife);
        Console.WriteLine("meaningOfLife=" + meaningOfLife);

        var myStruct = new MyStruct { X = 10, Y = 20 };
        Read(myStruct);
    }

    private record struct MyStruct
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    private static void Increment(ref int value)
    {
        // ref = pass by reference (read/write)
        value++;
    }

    private static void Write(out int value)
    {
        // out parameter MUST be assigned before method returns
        // Parameter 'value' must be assigned upon exit
        value = 42; // Meaning of life :)))
    }

    private static void Read(in MyStruct myStruct)
    {
        // in = readonly reference
        Console.WriteLine(myStruct.X);
        Console.WriteLine(myStruct.Y);
        // myStruct.X++; // 'in' parameter 'myStruct' is a read-only reference. Cannot modify struct member when accessed struct is not classified as a variable
    }
}