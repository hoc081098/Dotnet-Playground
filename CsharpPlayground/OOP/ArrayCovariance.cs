namespace CsharpPlayground.OOP;

public static class ArrayCovariance
{
    public static void Run()
    {
        // The problem with array covariance: it can lead to runtime exceptions.
        // In C#, arrays are covariant, meaning you can assign an array of a more derived type (e.g., string[])
        // to an array of a less derived type (e.g., object[]).
        // However, this can lead to runtime exceptions if you try to store an incompatible type into the array.
        // Under the hood, the CLR performs a runtime check when you assign a value to an array element.
        // If the value is not compatible with the actual array type, it throws an ArrayTypeMismatchException.

        string[] ints = ["1", "2", "3", "4"];
        Console.WriteLine($"ints.GetType(): {ints.GetType()}"); // System.String[]

        // ReSharper disable once CoVariantArrayConversion
        object[] objects = ints; // ✅ Array covariance: string[] can be assigned to object[]
        Console.WriteLine($"objects.GetType(): {objects.GetType()}"); // System.String[]
        try
        {
            objects[0] = true; // ❌ Runtime ArrayTypeMismatchException: cannot store bool in string[]
        }
        catch (ArrayTypeMismatchException e)
        {
            Console.WriteLine("Caught expected ArrayTypeMismatchException: " + e);
        }
        
        List<string> stringList = ["a", "b", "c"];
        // List<object> objectList = stringList; // ❌ Compile-time error: List<T> is invariant, cannot assign List<string> to List<object>
    }
}