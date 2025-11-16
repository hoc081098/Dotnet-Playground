using System.Numerics;
using Microsoft.CSharp.RuntimeBinder;

namespace CsharpPlayground.OOP;

/// <summary>
/// Demonstrates interfaces and generics in C#.
/// 
/// Similar to Kotlin/Java interfaces and generics, but with some differences:
/// - Default interface methods (C# 8.0+)
/// - Generic constraints (where clause)
/// - Covariance and contravariance (in/out keywords - applies to interfaces and delegates)
/// - Multiple interface implementation
/// </summary>
public class InterfacesAndGenericsPlayground
{
    public static void Run()
    {
        Console.WriteLine("=== Interfaces ===");

        // Interface implementation
        IAnimal dog = new Dog("Buddy");
        IAnimal cat = new Cat("Whiskers");

        dog.MakeSound();
        cat.MakeSound();

        dog.Eat("meat");
        cat.Eat("fish");

        // Default interface method (C# 8.0+)
        dog.Sleep(); // Uses default implementation
        cat.Sleep();

        // Multiple interface implementation
        ISwimmable swimmableDog = new Dog("Max");
        swimmableDog.Swim();

        Utils.PrintSeparator();
        Console.WriteLine("=== Generics ===");

        // Generic class usage
        var intBox = new Box<int>(42);
        Console.WriteLine($"Int box: {intBox.GetValue()}");
        var stringBox = new Box<string>("Hello");
        Console.WriteLine($"String box: {stringBox.GetValue()}");

        // Generic method
        var maxInt = MaxOf(1, 2);
        var maxString = MaxOf("apple", "banana");
        Console.WriteLine($"Max int: {maxInt}");
        Console.WriteLine($"Max string: {maxString}");

        // Generic constraint
        var intProcessor = new NumberProcessor<int>();
        var sum = intProcessor.Sum(10, 20);
        Console.WriteLine($"Sum: {sum}");

        // Error: The type 'string' must be a non-nullable value type in order to use it as parameter 'T'
        // var stringProcessor = new NumberProcessor<string>();

        // Error: The type 'CsharpPlayground.OOP.DemoStruct' must be convertible to 'System.Numerics.INumber<CsharpPlayground.OOP.DemoStruct>'
        // in order to use it as parameter 'T' in the generic class 'CsharpPlayground.OOP.NumberProcessor<T>'
        // var demoStructProcessor = new NumberProcessor<DemoStruct>();

        new DemoParameterlessConstructor<DemoStructParameterless>().DoSomething();
        new DemoParameterlessConstructor<DemoClassParameterless>().DoSomething();
        new DemoParameterlessConstructor<DemoStruct>().DoSomething();

        // Error: 'string' must be a non-abstract type with a public parameterless constructor
        // in order to use it as parameter 'T' in the generic class 'CsharpPlayground.OOP.DemoParameterlessConstructor<T>'
        // new DemoParameterlessConstructor<string>().DoSomething();

        // Error: 'CsharpPlayground.OOP.Dog' must be a non-abstract type with a public parameterless constructor
        // in order to use it as parameter 'T' in the generic class 'CsharpPlayground.OOP.DemoParameterlessConstructor<T>'
        // new DemoParameterlessConstructor<Dog>();

        var sd1 = SumViaDynamic(1, 2);
        Console.WriteLine($"SumViaDynamic(1, 2): {sd1}");

        try
        {
            var sd2 = SumViaDynamic<List<int>>([1, 2, 3], [4, 5, 6]);
            Console.WriteLine($"SumViaDynamic(List<int>, List<int>): [{string.Join(", ", sd2)}]");
        }
        catch (RuntimeBinderException ex)
        {
            Console.WriteLine($"SumViaDynamic(List<int>, List<int>): {ex}");
        }

        var employeeRepo = new Repository<Employee>();
        var employee1 = new Employee(1, "Alice");
        var employee2 = new Employee(2, "Bob");
        employeeRepo.Add(employee1);
        employeeRepo.Add(employee2);
        Console.WriteLine($"Repository count: {employeeRepo.Count}");
        var foundEmployee = employeeRepo.GetById(1);
        Console.WriteLine($"Found: {foundEmployee?.Name}");
        var notFoundEmployee = employeeRepo.GetById(3);
        Console.WriteLine($"Not found: {notFoundEmployee?.Name ?? "null"}");
    }

    // Generic method with constraint
    private static T MaxOf<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) > 0 ? a : b;


    // Using `dynamic` here forces the C# Dynamic Language Runtime (DLR) to resolve `+` at runtime.
    // Key points:
    // - The operator `+` is NOT resolved at compile-time because T is unknown.
    // - At runtime, the DLR performs overload resolution based on *runtime types*
    //   (similar to compiler rules, but using dynamic binding).
    // - The DLR generates and caches a CallSite for the dynamic rule (fast on subsequent calls).
    // - If the runtime type does not support operator `+`, a RuntimeBinderException is thrown.
    // - This is NOT reflection-based invocation; however, the DLR may use reflection metadata
    //   internally to locate operator overloads.
    // - Performance is slower than using generic constraints (e.g., INumber<T>), but flexible
    //   because it works with any type that overloads operator `+`.
    private static T SumViaDynamic<T>(T a, T b) where T : notnull
    {
        dynamic da = a;
        dynamic db = b;
        return da + db;
    }
}

// Interface definition - similar to Kotlin/Java
public interface IAnimal
{
    string Name { get; }
    void MakeSound();
    void Eat(string food);

    // Default interface method (C# 8.0+)
    void Sleep()
    {
        Console.WriteLine($"{Name} is sleeping...");
    }
}

// Another interface
public interface ISwimmable
{
    void Swim();
}

// Class implementing multiple interfaces
//  - C# uses ':' for interface implementation.
//  - No `override` keyword needed for interface methods. It is only used for virtual/abstract class methods.
public record Dog(string Name) : IAnimal, ISwimmable
{
    public void MakeSound() => Console.WriteLine($"Dog[{Name}] says: Woof!");
    public void Eat(string food) => Console.WriteLine($"Dog[{Name}] is eating {food}.");
    public void Swim() => Console.WriteLine($"Dog[{Name}] is swimming!");

    // Override default interface method
    public void Sleep() => Console.WriteLine($"{Name} (dog) is sleeping in the doghouse");
}

public record Cat(string Name) : IAnimal
{
    public void MakeSound()
    {
        Console.WriteLine($"Cat[{Name}] says: Meow!");
    }

    public void Eat(string food)
    {
        Console.WriteLine($"Cat[{Name}] is eating {food}");
    }

    // Uses default Sleep() implementation
}

// ---------------------------------------------------------------------------

// Generic class - similar to Kotlin/Java, but it cannot use `in`/`out` keywords.
public class Box<T>
{
    private readonly T _value;

    public Box(T value)
    {
        _value = value;
    }

    public T GetValue() => _value;
}

// Generic class with constraint
// where T : struct → T must be a value type (int, bool, custom struct)
// where T : class → T must be a reference type
// where T : new() → T must have a parameterless constructor
// where T : IComparable<T> → T must implement IComparable<T>
public class NumberProcessor<T> where T : struct, INumber<T>
{
    public T Sum(T a, T b) => a + b;
    public T Max(T a, T b) => a > b ? a : b;
}

public class DemoParameterlessConstructor<T> where T : new()
{
    public void DoSomething()
    {
        var t = new T();
        Console.WriteLine("Created t: " + t);
    }
}

public struct DemoStruct
{
    public int Number { get; init; }
}

public struct DemoStructParameterless;

public class DemoClassParameterless;

// ---------------------------------------------------------------------------

// Entity interface for repository
public interface IEntity
{
    int Id { get; }
}

// Generic repository pattern with multiple constraints
// T : class, IEntity => T is a non-nullable reference type (no `?`) implementing IEntity
public class Repository<T> where T : class, IEntity
{
    private readonly List<T> _items = [];

    public void Add(T item)
    {
        _items.Add(item);
        Console.WriteLine($"Added {item.GetType().Name} with Id {item.Id}");
    }

    public T? GetById(int id) => _items.FirstOrDefault(item => item.Id == id);

    public int Count => _items.Count;
}

// Example entity
public record Employee(int Id, string Name) : IEntity;