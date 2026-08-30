namespace CsharpPlayground.csharp15;

public sealed record Person(string Name, int Age);

public static class PersonExtensions
{
    extension(Person person)
    {
        public object? this[string propName] => propName switch
        {
            nameof(Person.Name) => person.Name,
            nameof(Person.Age) => person.Age,
            _ => throw new ArgumentOutOfRangeException(nameof(propName),
                propName,
                "Unknown property"),
        };
    }
}

public class ExtensionIndexers
{
    public static void Run()
    {
        var person = new Person("Alice", 30);
        Console.WriteLine($"Name: {person["Name"]}, Age: {person["Age"]}");
    }
}