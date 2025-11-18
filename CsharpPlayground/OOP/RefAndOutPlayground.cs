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

        Utils.PrintSeparator();

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

        Utils.PrintSeparator();

        // Demo out with custom method
        Write(out var meaningOfLife);
        Console.WriteLine("meaningOfLife=" + meaningOfLife);

        Utils.PrintSeparator();

        var myStruct = new MyStruct { X = 10, Y = 20 };
        Read(myStruct);

        Utils.PrintSeparator();

        var arr = new[] { 1, 2, 3, 4, 5 };
        Console.WriteLine("First element before modification: " + arr[0]);
        ref var r = ref First(arr); // r is a reference to arr[0], similar to pointer in C/C++
        r = 99; // arr[0] = 99
        Console.WriteLine("First element after modification: " + arr[0]);

        Utils.PrintSeparator();

        Span<int> intSpan = stackalloc int[10];
        intSpan[0] = 99;
        intSpan[1] = 100;
        Console.WriteLine("Before intSpan[0]: " + intSpan[0]);
        Console.WriteLine("Before intSpan[1]: " + intSpan[1]);

        var wrapper = new MySpanWrapper<int>(intSpan);
        // Write directly into stackalloc memory
        wrapper.GetElement(0) = -99;

        Console.WriteLine("After intSpan[0]: " + intSpan[0]);
        Console.WriteLine("After intSpan[1]: " + intSpan[1]);

        Utils.PrintSeparator();
        var wrapper2 = new MySpanWrapper<int>(intSpan);

        // Compile error: cannot be boxed because ref struct cannot be assigned to variables of type object
        // var boxedWrapper2 = (object?)wrapper2; 

        // Compile error: Cannot convert source type 'CsharpPlayground.OOP.RefAndOutPlayground.MySpanWrapper<int>' to target type 'CsharpPlayground.OOP.RefAndOutPlayground.IMySpanWrapper<int>'
        // IMySpanWrapper<int> interfaceWrapper = wrapper2;

        Action action = () =>
        {
            // Compile error: Cannot use local variable 'wrapper2' of byref-like type 'MySpanWrapper<int>' inside lambda expression
            // wrapper2.GetElement(1) = -100;
        };

        UseMySpanWrapperSync(wrapper2);
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
        Console.WriteLine("Read: x=" + myStruct.X);
        Console.WriteLine("Read: y=" + myStruct.Y);
        // myStruct.X++; // 'in' parameter 'myStruct' is a read-only reference. Cannot modify struct member when accessed struct is not classified as a variable
    }

    // return ref
    private static ref int First(int[] arr) => ref arr[0];

    private interface IMySpanWrapper<T>
    {
        ref T GetElement(int index);
    }

    // ref struct = a value type with stack-only lifetime restrictions.
    // These types are used to safely represent stack-backed memory (Span<T>, ReadOnlySpan<T>, etc.).
    //
    // Restrictions (enforced by the compiler to prevent memory escape):
    //   - Cannot be boxed (cannot convert to object, dynamic, or any interface type)
    //   - Cannot be captured by lambdas, local functions, async methods, or iterators
    //   - Cannot be a field of a class (only allowed as fields inside another ref struct)
    //   - Cannot be used as a type argument for generic classes or methods
    //   - Cannot be stored in arrays
    //   - Cannot be static
    //   - Cannot outlive the current stack frame
    //
    // Reason:
    //   ref structs often contain stack-backed memory pointers. Allowing them to escape to the heap
    //   (e.g., via boxing, async state machines, interface dispatch, or captured variables)
    //   would lead to undefined behavior and potential memory corruption.
    //
    // Returning `ref T` from a ref struct member exposes a direct reference to the underlying buffer,
    // similar to returning &array[index] in C++.
    //
    // Example usage:
    //   ref var item = ref wrapper.GetElement(0);
    //   item = 42; // writes directly into the underlying Span<T> memory
    private readonly ref struct MySpanWrapper<T>(Span<T> span) : IMySpanWrapper<T>
    {
        // Copying a Span<T> only copies the pointer + length descriptor, not the data.
        private readonly Span<T> _span = span;

        // Returning ref T exposes the actual element reference inside the Span<T>.
        public ref T GetElement(int index) => ref _span[index];
    }

    // private class WrapperClasss
    // {
    //     // Compile error: Field cannot be of byref-like type 'System.Span<int>' unless it is an instance member of a 'ref' struct
    //     private readonly Span<int> _span;
    // }

    private static void UseMySpanWrapperSync(MySpanWrapper<int> spanWrapper)
    {
        // OK to use MySpanWrapper<T> in synchronous method
        var first = spanWrapper.GetElement(0);
        Console.WriteLine("UseMySpanWrapperSync: first=" + first);
    }

    // Compile error: Parameters of type 'MySpanWrapper<int>' cannot be declared in async methods
    // private static async Task UseMySpanWrapperAsync(MySpanWrapper<int> spanWrapper)
    // {
    //     
    // }
}