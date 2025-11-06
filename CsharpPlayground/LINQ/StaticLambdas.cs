namespace CsharpPlayground.LINQ;

internal sealed class MyClass
{
    private static int _staticField = 20;
    private int _instanceField = 10;

    public void DoSomething()
    {
        var localVariable = 5;

        // Non-static lambda (captures localVariable and _instanceField implicitly)
        Action nonStaticLambda = () =>
            Console.WriteLine(
                $"this: {this}, Local: {localVariable}, Instance: {_instanceField}, Static: {_staticField}");

        // Static lambda (cannot capture this or localVariable or _instanceField)
        // It only accesses static members.
        // This would cause a compile-time error if uncommented:
        // Action staticLambdaError = static () => 
        //     Console.WriteLine(
        //         $"this: {this}, Local: {localVariable}, Instance: {_instanceField}, Static: {_staticField}");

        // Correct static lambda (no captures)
        Action staticLambda = static () => Console.WriteLine("This is a static lambda! Static Field: " + _staticField);

        nonStaticLambda();
        staticLambda();
    }
}

public static class StaticLambdas
{
    public static void Run()
    {
        var myClass = new MyClass();
        myClass.DoSomething();
    }
}