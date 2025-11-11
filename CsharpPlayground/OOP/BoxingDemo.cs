namespace CsharpPlayground.OOP;

public static class BoxingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== BOXING / UNBOXING DEMO ===");

        int x = 42; // Value type nằm trên stack
        object boxed = x; // ✅ Boxing: copy x lên heap
        Console.WriteLine($"Boxed: {boxed} (Type: {boxed.GetType().Name})");

        int y = (int)boxed; // ✅ Unboxing: copy ngược về stack
        Console.WriteLine($"Unboxed: {y}");

        // So sánh reference: khác địa chỉ
        Console.WriteLine($"ReferenceEquals(x, boxed)? {ReferenceEquals(x, boxed)}"); // False

        // Boxing xảy ra khi cast sang interface
        IComparable comparable = x; // ✅ Boxing nữa
        Console.WriteLine($"Boxed via interface: {comparable.GetType().Name}");

        // Boxing xảy ra cả khi truyền vào object parameter
        PrintObject(x); // ✅ boxing
        PrintGeneric(x); // ✅ boxing (vì generic không bị ràng buộc)

        // Nhưng không boxing khi generic bị ràng buộc struct
        PrintGenericStruct(x); // ✅ no boxing nhờ JIT dùng constrained.
    }

    static void PrintObject(object obj)
    {
        obj!.Equals(obj);
        Console.WriteLine($"PrintObject: {obj} (boxed)");
    }

    static void PrintGeneric<T>(T value)
    {
        value!.Equals(value);
        Console.WriteLine($"PrintGeneric: {value} (may be boxed if T is value type)");
    }

    static void PrintGenericStruct<T>(T value) where T : struct
    {
        value.Equals(value);
        Console.WriteLine($"PrintGenericStruct: {value} (no boxing)");
    }
}