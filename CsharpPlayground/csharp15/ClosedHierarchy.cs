namespace CsharpPlayground.csharp15;

public class ClosedHierarchy
{
    public closed record GateState;

    public sealed record Closed : GateState;

    public sealed record Open(float Percent) : GateState;

    static string Describe(GateState state) => state switch
    {
        Closed => "closed",
        Open(var percent) => $"{percent}% open",
        // No warning: every direct descendant of 'GateState' is handled.
    };
    
    public static void Run()
    {
        Console.WriteLine(Describe(new Closed()));
        Console.WriteLine(Describe(new Open(75)));
    }
}