using System.Diagnostics.CodeAnalysis;

namespace CsharpPlayground.DelegatesAndEvents;

/// <summary>
/// Demonstrates delegates, events, and Func/Action types in C#.
/// These are similar to:
/// - Kotlin's function types and lambdas: (Int, String) -> Boolean
/// - Java's functional interfaces: Function, Consumer, Predicate
/// Key concepts:
/// - Delegate: type-safe function pointer
/// - Event: publisher-subscriber pattern based on delegates
/// - Action&lt;T&gt;: delegate with no return value (void)
/// - Func&lt;T, TResult&gt;: delegate with return value
/// - Predicate&lt;T&gt;: delegate that returns bool (Func&lt;T, bool&gt;)
/// </summary>
[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public static class DelegatesPlayground
{
    delegate int MathOperation(int a, int b);

    delegate void LogHandler(string message);

    public static void Run()
    {
        Console.WriteLine("=== Built-in Delegates: Action, Func, Predicate, Comparison ===");

        // Action<T> - similar to Kotlin's (T) -> Unit
        Action actionWithoutParams = () => Console.WriteLine($"Hello from actionWithoutParams!");
        Action<string> actionWithParams = name => Console.WriteLine($"Hello, {name} from actionWithParams!");
        Action<string, int> actionWithTwoParams = (name, age) =>
            Console.WriteLine($"Hello, {name}. You are {age} years old from actionWithTwoParams!");
        actionWithoutParams(); // Equivalent to actionWithoutParams.Invoke()
        actionWithParams("Alice"); // Equivalent to actionWithParams.Invoke("Alice")
        actionWithTwoParams("Bob", 30); // Equivalent to actionWithTwoParams.Invoke("Bob", 30)

        Utils.PrintSeparator();

        // Func<T, TResult> - similar to Kotlin's (T) -> Result
        Func<int> funcWithoutParams = () => Random.Shared.Next();
        Func<int, int> squareFunc = x => x * x;
        var squareFuncInferred = (int x) => x * x; // Type inferred by compiler: Func<int, int>
        Func<int, int, int> sumFunc = (a, b) => a + b;
        Console.WriteLine($"funcWithoutParams(): {funcWithoutParams()}");
        Console.WriteLine($"squareFunc(5): {squareFunc(5)}");
        Console.WriteLine($"sumFunc(3, 4): {sumFunc(3, 4)}");

        Utils.PrintSeparator();

        // Predicate<T> - similar to Kotlin's (T) -> Boolean
        Predicate<int> isEvenPredicate = x => x % 2 == 0;
        // Func<int, bool> can be used for the same purpose
        Func<int, bool> isEvenFunc = x => x % 2 == 0;
        var isEvenFuncInferred = (int x) => x % 2 == 0; // Always inferred as Func<int, bool>
        Console.WriteLine($"isEvenPredicate(4): {isEvenPredicate(4)}, {isEvenFunc(4)}");
        Console.WriteLine($"isEvenPredicate(5): {isEvenPredicate(5)}, {isEvenFunc(5)}");
        var evenNumbers = FilterList([1, 2, 3, 4, 5], isEvenPredicate);
        Console.WriteLine($"Even numbers: {string.Join(", ", evenNumbers)}");

        Utils.PrintSeparator();

        // Comparison<T> - similar to Kotlin's Comparator<T>
        Comparison<int> comparison = (a, b) => b.CompareTo(a); // Descending order comparison
        List<int> numbers = [5, 2, 8, 1, 4];
        numbers.Sort(comparison); // Sort in place using descending order
        Console.WriteLine($"Sorted numbers (descending): {string.Join(", ", numbers)}");

        // Comparer<T> is a class, while Comparison<T> is a delegate (function pointer)
        // Typically, Comparison<T> is used inline (via lambda) for sorting
        List<int> numbers2 = [5, 2, 8, 1, 4];
        var comparer = Comparer<int>.Create(comparison);
        numbers2.Sort(comparer); // Sort in descending order using Comparer<T>
        Console.WriteLine($"Sorted numbers using Comparer (descending): {string.Join(", ", numbers2)}");

        Utils.PrintSeparator();

        Console.WriteLine("=== Custom Delegate ===");
        // Custom delegates are:
        // - Reference types
        // - Multicast delegates (can chain multiple methods)
        // - Support covariance/contravariance
        // - Compiled to IL as subclasses of MulticastDelegate
        MathOperation sumOp = (a, b) =>
        {
            Console.WriteLine("Invoke sumOp");
            return a + b;
        };
        MathOperation multiplyOp = (a, b) =>
        {
            Console.WriteLine("Invoke multiplyOp");
            return a * b;
        };
        MathOperation subtractOp = (a, b) =>
        {
            Console.WriteLine("Invoke subtractOp");
            return a - b;
        };
        MathOperation multicastOps = sumOp + multiplyOp + subtractOp;
        Console.WriteLine($"sumOp(3, 4): {sumOp(3, 4)}");
        Console.WriteLine($"multiplyOp(3, 4): {multiplyOp(3, 4)}");
        Console.WriteLine($"subtractOp(3, 4): {subtractOp(3, 4)}");
        Console.WriteLine(
            $"multicastOps.GetInvocationList: {string.Join(", ", multicastOps.GetInvocationList() as IEnumerable<Delegate>)}");
        Console.WriteLine(
            $"multicastOps(3, 4): {multicastOps(3, 4)}"); // Print -1 because it invokes all of 3 delegates, but returns the last result (subtractOp)

        Utils.PrintSeparator();

        // Multicast delegate demonstration for logging
        // Delegate.Combine creates a new delegate that chains the invocations (like a linked list)
        // Delegates are immutable; adding/removing handlers creates new instances rather than mutating existing ones
        LogHandler logHandler = ConsoleLog;
        logHandler += FileLog;
        logHandler("This is a log message.");

        Utils.PrintSeparator();

        Console.WriteLine("=== Higher-Order Functions with Delegates ===");
        List<int> nums = [1, 2, 3, 4, 5];
        var sum1 = ReduceInts(nums, sumOp, 0);
        var sum2 = ReduceInts(nums, (acc, e) => acc + e, 0);
        var product1 = ReduceInts(nums, multiplyOp, 1);
        var product2 = ReduceInts(nums, (acc, e) => acc * e, 1);
        Console.WriteLine($"Sum using sumOp: {sum1}");
        Console.WriteLine($"Sum using lambda: {sum2}");
        Console.WriteLine($"Product using multiplyOp: {product1}");
        Console.WriteLine($"Product using lambda: {product2}");

        Utils.PrintSeparator();

        Console.WriteLine("=== Method Group Conversion to Delegate ===");
        // Method group conversion behavior:

        // Static method to delegate - returns same cached instance
        // ReferenceEquals for static method delegates is implementation-defined by C# spec.
        // Modern .NET runtimes usually cache static method delegates for performance.
        // Compiler caches: __SumMethod ?? (__SumMethod = new Func<int, int, int>((object) null, __methodptr(SumMethod)))
        Func<int, int, int> methodGroupSum1 = SumMethod;
        // Same cached instance
        Func<int, int, int> methodGroupSum2 = SumMethod;
        Console.WriteLine(
            $"static method: Compare by reference: {ReferenceEquals(methodGroupSum1, methodGroupSum2)}"); // True
        Console.WriteLine($"static method: Compare by equality: {methodGroupSum1.Equals(methodGroupSum2)}"); // True
        Console.WriteLine($"static method: Compare by equality: {methodGroupSum1 == methodGroupSum2}"); // True

        // Same target + instance method to delegate - creates new instances each time
        // However, they are equal in terms of method and target (Equals and == return true)
        var target = new TargetClass();
        // new Func<int, int, int>((object) target, __methodptr(InstanceMethod))
        Func<int, int, int> instanceMethodDelegate1 = target.InstanceMethod;
        // new Func<int, int, int>((object) target, __methodptr(InstanceMethod))
        Func<int, int, int> instanceMethodDelegate2 = target.InstanceMethod;
        Console.WriteLine(
            $"instance method: Compare by reference: {ReferenceEquals(instanceMethodDelegate1, instanceMethodDelegate2)}"); // False
        Console.WriteLine(
            $"instance method: Compare by equality: {instanceMethodDelegate1.Equals(instanceMethodDelegate2)}"); // True
        Console.WriteLine(
            $"instance method: Compare by equality: {instanceMethodDelegate1 == instanceMethodDelegate2}"); // True

        // Different targets + instance method to delegate - different delegate instances (always false)
        // new Func<int, int, int>((object) new DelegatesPlayground.TargetClass(), __methodptr(InstanceMethod))
        Func<int, int, int> instanceMethodDelegate3 = new TargetClass().InstanceMethod;
        // new Func<int, int, int>((object) new DelegatesPlayground.TargetClass(), __methodptr(InstanceMethod))
        Func<int, int, int> instanceMethodDelegate4 = new TargetClass().InstanceMethod;
        Console.WriteLine(
            $"different instance method: Compare by reference: {ReferenceEquals(instanceMethodDelegate3, instanceMethodDelegate4)}"); // False
        Console.WriteLine(
            $"different instance method: Compare by equality: {instanceMethodDelegate3.Equals(instanceMethodDelegate4)}"); // False
        Console.WriteLine(
            $"different instance method: Compare by equality: {instanceMethodDelegate3 == instanceMethodDelegate4}"); // False

        Utils.PrintSeparator();

        Console.WriteLine("=== Capture and Closure in Delegates ===");

        // When a lambda captures a local variable, the compiler generates a closure class
        // to hold the captured variables. The delegate instance references this closure.
        // [CompilerGenerated]
        // private sealed class <>c__DisplayClass2_0
        // {
        //   public int capturedVariable;
        //   [...]
        //   internal int <Run>b__16(int x)
        //   {
        //     return x + this.capturedVariable;
        //   }
        // }

        // var cDisplayClass20 = new DelegatesPlayground.<>c__DisplayClass2_0();
        var capturedVariable = 10; // cDisplayClass20.capturedVariable = 10;

        // Func<int, int> addCaptured = new Func<int, int>((object) cDisplayClass20, __methodptr(<Run>b__16));
        Func<int, int> addCaptured = x => x + capturedVariable; // Captures capturedVariable

        Console.WriteLine($"addCaptured(5): {addCaptured(5)}"); // 15
        capturedVariable = 0; // cDisplayClass20.capturedVariable = 0;
        Console.WriteLine($"addCaptured(5) after changing capturedVariable : {addCaptured(5)}"); // 5

        // Demo lambdas that doesn't capture any variables
        Func<int, int>
            noCaptureLambda1 = x => x * x; // compiler usually caches → likely singleton delegate per call site
        Func<int, int>
            noCaptureLambda2 =
                static x => x * x; // Static lambda always cached → guaranteed singleton delegate per call site
        noCaptureLambda1(5);
        noCaptureLambda2(5);

        Utils.PrintSeparator();
        Console.WriteLine("=== Delegate Variance ===");

        // Func<out TResult> is covariant in TResult
        Func<Dog> dogFactory = () => new Dog("Buddy");
        Func<IAnimal> animalFactory = dogFactory; // Covariance

        // Action<in T> is contravariant in T
        Action<IAnimal> consumeAnimal = a => Console.WriteLine(a.Name);
        Action<Dog> consumeDog = consumeAnimal; // Contravariance
        consumeDog(dogFactory());
        consumeAnimal(animalFactory());
        Console.WriteLine("variance OK");

        Console.WriteLine("✅ Delegates Playground finished successfully.");
    }

    private interface IAnimal
    {
        string Name { get; }
    };

    private record Dog(string Name) : IAnimal;

    private record TargetClass
    {
        public int InstanceMethod(int x, int y) => x + y;
        public static int StaticMethod(int x, int y) => x + y;
    }

    private static int SumMethod(int arg1, int arg2) => arg1 + arg2;

    private static void ConsoleLog(string message)
    {
        Console.WriteLine($"[Console] {message}");
    }

    private static void FileLog(string message)
    {
        Console.WriteLine($"[File] {message}");
    }

    private static List<int> FilterList(List<int> numbers, Predicate<int> predicate)
    {
        // Note: In production code, use LINQ's Where method for filtering
        var result = new List<int>();
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var num in numbers)
        {
            if (predicate(num))
            {
                result.Add(num);
            }
        }

        return result;
    }

    // Higher-order function example
    private static int ReduceInts(List<int> numbers, MathOperation operation, int initialValue)
    {
        // Note: In production code, use LINQ's Aggregate method for reduction
        var result = initialValue;
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var num in numbers)
        {
            result = operation(result, num);
        }

        return result;
    }
}