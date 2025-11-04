// ReSharper disable PreferConcreteValueOverDefault

using CsharpPlayground.OOP;

namespace CsharpPlayground;

internal static class StringExtensions
{
    internal static string AsPrintable(this object? value) =>
        value?.ToString() ?? "null";
}

public static class Default
{
    private static class SomeStaticClass;

    private record class SomeClass
    {
        public int Value { get; set; }
    }

    private record struct SomeStruct
    {
        public int Value { get; set; }
    }

    private enum SomeEnum
    {
        None = 0, // default(SomeEnum) == 0
        First = 1,
        Second = 2,
    }

    public static void Run()
    {
        Console.WriteLine("Default of bool: " + default(bool).AsPrintable());
        Console.WriteLine("Default of int: " + default(int).AsPrintable());
        Console.WriteLine("Default of char: " + default(char).AsPrintable());
        Console.WriteLine("Default of float: " + default(float).AsPrintable());
        Console.WriteLine("Default of double: " + default(double).AsPrintable());
        Console.WriteLine("Default of decimal: " + default(decimal).AsPrintable());
        Console.WriteLine("Default of string: " + default(string).AsPrintable());
        Console.WriteLine("Default of object: " + default(object).AsPrintable());
        Console.WriteLine("Default of DateTime: " + default(DateTime).AsPrintable());
        Console.WriteLine("Default of DateTimeOffset: " + default(DateTimeOffset).AsPrintable());
        Console.WriteLine("Default of Guid: " + default(Guid).AsPrintable());
        Console.WriteLine("Default of int?: " + default(int?).AsPrintable());
        Console.WriteLine("Default of Nullable<int>: " + default(Nullable<int>).AsPrintable());
        Console.WriteLine("Default of SomeStaticClass: " + default(SomeStaticClass).AsPrintable());
        Console.WriteLine("Default of SomeClass: " + default(SomeClass).AsPrintable());
        Console.WriteLine("Default of SomeStruct: " + default(SomeStruct).AsPrintable());
        Console.WriteLine("Default of SomeEnum: " + default(SomeEnum).AsPrintable());
        Console.WriteLine("Default of SomeEnum (actual): " +
                          EnumExtensions.FindByValueOrThrow<SomeEnum>((int)default(SomeEnum)).AsPrintable());
    }
}