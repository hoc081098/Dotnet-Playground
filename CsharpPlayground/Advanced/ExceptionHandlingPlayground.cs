namespace CsharpPlayground.Advanced;

/// <summary>
/// Demonstrates exception handling patterns in C#.
/// Similar to Kotlin/Java try-catch, but with some differences:
/// - finally block for cleanup
/// - using statement for automatic resource disposal (IDisposable)
/// - Exception filters (when clause)
/// - Custom exception types
/// </summary>
public static class ExceptionHandlingPlayground
{
    public static void Run()
    {
        Console.WriteLine("=== Basic Exception Handling ===");

        // Basic try-catch - similar to Kotlin/Java
        try
        {
            int result = Divide(10, 0);
            Console.WriteLine($"Result: {result}");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Caught DivideByZeroException: {ex.Message}");
        }

        // Multiple catch blocks
        try
        {
            ProcessData(null!);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Caught ArgumentNullException: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Caught InvalidOperationException: {ex.Message}");
        }
        catch (Exception ex) // Catch all other exceptions
        {
            Console.WriteLine($"Caught general exception: {ex.Message}");
        }

        // Try-catch-finally - finally always executes
        Console.WriteLine("\n=== Try-Catch-Finally ===");
        try
        {
            Console.WriteLine("In try block");
            throw new Exception("Test exception");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"In catch block: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("In finally block - always executes");
        }

        // Exception filter (when clause) - C# specific feature
        Console.WriteLine("\n=== Exception Filters ===");
        try
        {
            throw new ArgumentException("Invalid value", "paramName");
        }
        catch (ArgumentException ex) when (ex.ParamName == "paramName")
        {
            Console.WriteLine($"Caught ArgumentException with specific param: {ex.ParamName}");
        }

        // Custom exceptions
        Console.WriteLine("\n=== Custom Exceptions ===");
        try
        {
            ValidateAge(-5);
        }
        catch (InvalidAgeException ex)
        {
            Console.WriteLine($"Caught custom exception: {ex.Message}");
            Console.WriteLine($"  Age provided: {ex.Age}");
        }

        // Re-throwing exceptions
        Console.WriteLine("\n=== Re-throwing Exceptions ===");
        try
        {
            ProcessWithRetry();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Final catch: {ex.Message}");
        }

        // Using statement for automatic resource disposal
        Console.WriteLine("\n=== Using Statement (IDisposable) ===");
        // Traditional using statement
        using (var resource = new DisposableResource("Resource1"))
        {
            resource.DoWork();
        } // Dispose() is called automatically here

        // Using declaration (C# 8.0+)
        using var resource2 = new DisposableResource("Resource2");
        resource2.DoWork();
        // Dispose() is called at the end of the scope

        // Exception in async code
        Console.WriteLine("\n=== Async Exception Handling ===");
        try
        {
            // Using GetAwaiter().GetResult() for demonstration (better than Wait() for avoiding deadlocks)
            AsyncMethodWithException().GetAwaiter().GetResult();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Caught InvalidOperationException: {ex.Message}");
        }
    }

    private static int Divide(int a, int b)
    {
        return a / b; // Throws DivideByZeroException if b is 0
    }

    private static void ProcessData(string? data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (data.Length == 0)
            throw new InvalidOperationException("Data cannot be empty");

        Console.WriteLine($"Processing: {data}");
    }

    private static void ValidateAge(int age)
    {
        if (age < 0)
            throw new InvalidAgeException($"Age cannot be negative: {age}", age);

        if (age > 150)
            throw new InvalidAgeException($"Age is unrealistic: {age}", age);
    }

    private static void ProcessWithRetry()
    {
        try
        {
            // Simulate an error
            throw new InvalidOperationException("Initial error");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Caught and re-throwing: {ex.Message}");
            throw; // Re-throw the same exception (preserves stack trace)
            // throw ex; // This would reset the stack trace - avoid this
        }
    }

    private static async Task AsyncMethodWithException()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Async operation failed");
    }
}

// Custom exception class
// Kotlin equivalent: class InvalidAgeException(message: String, val age: Int) : Exception(message)
public class InvalidAgeException : Exception
{
    public int Age { get; }

    public InvalidAgeException(string message, int age) : base(message)
    {
        Age = age;
    }

    public InvalidAgeException(string message, int age, Exception innerException)
        : base(message, innerException)
    {
        Age = age;
    }
}

// IDisposable pattern for resource management
// Similar to Kotlin's use() function or Java's try-with-resources
public class DisposableResource : IDisposable
{
    private readonly string _name;
    private bool _disposed = false;

    public DisposableResource(string name)
    {
        _name = name;
        Console.WriteLine($"  [{_name}] Resource created");
    }

    public void DoWork()
    {
        if (_disposed)
            throw new ObjectDisposedException(_name);

        Console.WriteLine($"  [{_name}] Doing work...");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Console.WriteLine($"  [{_name}] Resource disposed");
        _disposed = true;

        // In real scenarios, clean up unmanaged resources here
        GC.SuppressFinalize(this);
    }

    ~DisposableResource()
    {
        // Finalizer (destructor) - only if needed for unmanaged resources
        Dispose();
    }
}
