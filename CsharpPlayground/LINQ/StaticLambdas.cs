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

        // - Allocates a stack-only buffer of 5 `int` using `stackalloc`, wrapped as a `Span<int>`.
        //   Lifetime is the current scope.
        // - Initializes the first two elements to 0 and 1.
        // - Declares a static lambda `Func<Span<int>, int>` that returns `s[0] + s[1]`.
        //   Being `static`, it cannot capture outer variables, so no closure is created, and it’s safe with ref-structs like `Span<T>`.
        // - Invokes the lambda with the span and prints the result `1`.
        // Notes:
        // - `Span<T>` is stack-only and cannot be captured or boxed; passing it as a parameter to the lambda is fine.
        // - The delegate instance is allocated, but there’s no extra closure allocation because the lambda is `static`.
        Span<int> span = stackalloc int[5];
        span[0] = 0;
        span[1] = 1;
        Func<Span<int>, int> sum = static s => s[0] + s[1];
        Console.WriteLine(sum(span));
    }
}