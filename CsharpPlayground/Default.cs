// ReSharper disable PreferConcreteValueOverDefault

using CsharpPlayground.OOP;

namespace CsharpPlayground;


public static class Default
{
    private static class SomeStaticClass;

    private record class SomeClass
    {
        public int Value { get; set; }
    }

    private record struct SomeStruct()
    {
        public int Value { get; set; } = 99;
    }

    private record AnotherClass
    {
        public SomeStruct SomeStruct { get; set; }
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

        Utils.PrintSeparator();

        // IL_021f: ldloca.s     someStruct
        // IL_0221: call         instance void SomeStruct::.ctor()      <-> new SomeStruct()
        // new SomeStruct() == zero-initialization + constructor call
        var newSomeStruct = new SomeStruct();

        // IL_0226: ldloca.s     defaultOfSomeStruct
        // IL_0228: initobj      SomeStruct                             <-> default(SomeStruct) <-> zero-initialization
        var defaultOfSomeStruct = default(SomeStruct);
        Console.WriteLine("new SomeStruct(): " + newSomeStruct.ToString());
        Console.WriteLine("default(SomeStruct): " + defaultOfSomeStruct.ToString());
        Console.WriteLine("new AnotherClass().SomeStruct is default(SomeStruct): " + new AnotherClass().SomeStruct);
    }
}