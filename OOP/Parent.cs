namespace CsharpPlayground.OOP;

public class Parent
{
    private string _privateName = "hello";
    internal string _internalName = "hello";

    protected string ProtectedName = "PP";
    public string PublicName = "hello";

    public required Guid Id { get; init; }
}

public class Child : Parent;

public static class ParentChildPlayground
{
    public static void Run()
    {
        var child = new Child
        {
            Id = Guid.NewGuid(),
        };
        // Console.WriteLine(child._privateName);
        Console.WriteLine(child._internalName);
        // Console.WriteLine(child.ProtectedName);
        Console.WriteLine(child.PublicName);
        Console.WriteLine(child.Id);
    }
}