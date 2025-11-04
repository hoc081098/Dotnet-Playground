namespace CsharpPlayground.Advanced;

/// <summary>
/// Demonstrates interfaces and generics in C#.
/// Similar to Kotlin/Java interfaces and generics, but with some differences:
/// - Default interface methods (C# 8.0+)
/// - Generic constraints (where clause)
/// - Covariance and contravariance
/// - Multiple interface implementation
/// </summary>
public static class InterfacesAndGenericsPlayground
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

        Console.WriteLine("\n=== Generics ===");

        // Generic class usage
        var intBox = new Box<int>(42);
        Console.WriteLine($"Int box: {intBox.GetValue()}");

        var stringBox = new Box<string>("Hello");
        Console.WriteLine($"String box: {stringBox.GetValue()}");

        // Generic method
        int maxInt = GetMax(10, 20);
        string maxString = GetMax("apple", "zebra");
        Console.WriteLine($"Max int: {maxInt}");
        Console.WriteLine($"Max string: {maxString}");

        // Generic constraints
        var numberProcessor = new NumberProcessor<int>();
        Console.WriteLine($"Sum: {numberProcessor.Sum(5, 10)}");

        var employeeRepo = new Repository<Employee>();
        var employee1 = new Employee(1, "Alice");
        var employee2 = new Employee(2, "Bob");
        employeeRepo.Add(employee1);
        employeeRepo.Add(employee2);
        Console.WriteLine($"Repository count: {employeeRepo.Count}");
        var foundEmployee = employeeRepo.GetById(1);
        Console.WriteLine($"Found: {foundEmployee?.Name}");

        // Covariance (out) - can assign derived type to base type
        Console.WriteLine("\n=== Covariance (out) ===");
        IProducer<Dog> dogProducer = new AnimalProducer<Dog>(new Dog("Producer Dog"));
        IProducer<IAnimal> animalProducer = dogProducer; // Covariant assignment
        IAnimal producedAnimal = animalProducer.Produce();
        producedAnimal.MakeSound();

        // Contravariance (in) - can assign base type to derived type
        Console.WriteLine("\n=== Contravariance (in) ===");
        IConsumer<IAnimal> animalConsumer = new AnimalConsumer<IAnimal>();
        IConsumer<Dog> dogConsumer = animalConsumer; // Contravariant assignment
        dogConsumer.Consume(new Dog("Consumer Dog"));
    }

    // Generic method with constraint
    private static T GetMax<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b) > 0 ? a : b;
    }
}

// Interface definition - similar to Kotlin/Java
public interface IAnimal
{
    string Name { get; }
    void MakeSound();
    void Eat(string food);

    // Default interface method (C# 8.0+)
    // Similar to Kotlin's default method in interface
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
public class Dog : IAnimal, ISwimmable
{
    public string Name { get; }

    public Dog(string name)
    {
        Name = name;
    }

    public void MakeSound()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }

    public void Eat(string food)
    {
        Console.WriteLine($"{Name} is eating {food}");
    }

    public void Swim()
    {
        Console.WriteLine($"{Name} is swimming!");
    }

    // Override default interface method
    public void Sleep()
    {
        Console.WriteLine($"{Name} (dog) is sleeping in the doghouse");
    }
}

public class Cat : IAnimal
{
    public string Name { get; }

    public Cat(string name)
    {
        Name = name;
    }

    public void MakeSound()
    {
        Console.WriteLine($"{Name} says: Meow!");
    }

    public void Eat(string food)
    {
        Console.WriteLine($"{Name} is eating {food}");
    }

    // Uses default Sleep() implementation
}

// Generic class - similar to Kotlin/Java
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
public class NumberProcessor<T> where T : struct, IComparable<T>
{
    public T Sum(T a, T b)
    {
        // Note: In real code, you'd use more sophisticated numeric handling
        dynamic da = a;
        dynamic db = b;
        return da + db;
    }
}

// Generic repository pattern with multiple constraints
public class Repository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
        Console.WriteLine($"Added {typeof(T).Name} with Id {item.Id}");
    }

    public T? GetById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public int Count => _items.Count;
}

// Entity interface for repository
public interface IEntity
{
    int Id { get; }
}

// Example entity
public class Employee : IEntity
{
    public int Id { get; }
    public string Name { get; }

    public Employee(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

// Covariance (out keyword) - can only be used as return type
// Similar to Kotlin's out T
public interface IProducer<out T>
{
    T Produce();
}

public class AnimalProducer<T> : IProducer<T> where T : IAnimal
{
    private readonly T _animal;

    public AnimalProducer(T animal)
    {
        _animal = animal;
    }

    public T Produce()
    {
        Console.WriteLine($"Producing {_animal.Name}");
        return _animal;
    }
}

// Contravariance (in keyword) - can only be used as parameter type
// Similar to Kotlin's in T
public interface IConsumer<in T>
{
    void Consume(T item);
}

public class AnimalConsumer<T> : IConsumer<T> where T : IAnimal
{
    public void Consume(T animal)
    {
        Console.WriteLine($"Consuming {animal.Name}");
        animal.MakeSound();
    }
}
