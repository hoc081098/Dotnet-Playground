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

        // Action - similar to Kotlin's (T) -> Unit
        Action actionWithoutParams = () => Console.WriteLine($"Hello from actionWithoutParams!");
        Action<string> actionWithParams = name => Console.WriteLine($"Hello, {name} from actionWithParams!");
        Action<string, int> actionWithTwoParams = (name, age) =>
            Console.WriteLine($"Hello, {name}. You are {age} years old from actionWithTwoParams!");
        actionWithoutParams(); // === actionWithoutParams.Invoke()
        actionWithParams("Alice"); // === actionWithParams.Invoke("Alice")
        actionWithTwoParams("Bob", 30); // === actionWithTwoParams.Invoke("Bob", 30)

        Utils.PrintSeparator();

        // Func<T, TResult> - similar to Kotlin's (T) -> Result
        Func<int> funcWithoutParams = () => Random.Shared.Next();
        Func<int, int> squareFunc = x => x * x;
        var squareFuncInferred = (int x) => x * x; // Type inferred by compiler:  Func<int,int>
        Func<int, int, int> sumFunc = (a, b) => a + b;
        Console.WriteLine($"funcWithoutParams(): {funcWithoutParams()}");
        Console.WriteLine($"squareFunc(5): {squareFunc(5)}");
        Console.WriteLine($"sumFunc(3, 4): {sumFunc(3, 4)}");

        Utils.PrintSeparator();

        // Predicate<T> - similar to Kotlin's (T) -> Boolean
        Predicate<int> isEvenPredicate = x => x % 2 == 0;
        // But you can also use Func<int, bool> for the same purpose
        Func<int, bool> isEvenFunc = x => x % 2 == 0;
        var isEvenFuncInferred = (int x) => x % 2 == 0; // Always inferred as Func<int,bool>
        Console.WriteLine($"isEvenPredicate(4): {isEvenPredicate(4)}, {isEvenFunc(4)}");
        Console.WriteLine($"isEvenPredicate(5): {isEvenPredicate(5)}, {isEvenFunc(5)}");
        var eventNumbers = FilterList([1, 2, 3, 4, 5], isEvenPredicate);
        Console.WriteLine($"Even numbers: {string.Join(", ", eventNumbers)}");

        Utils.PrintSeparator();

        // Comparison<T> - similar to Kotlin's Comparator<T>
        Comparison<int> comparison = (a, b) => b.CompareTo(a); // Compare in descending order
        List<int> numbers = [5, 2, 8, 1, 4];
        numbers.Sort(comparison); // Sort in descending order (sort in place)
        Console.WriteLine($"Sorted numbers (descending): {string.Join(", ", numbers)}");

        // Comparer<T> is a class while Comparison<T> is a delegate (function pointer)
        // We usually use Comparison<T> inline (via lambda) for sorting.
        List<int> numbers2 = [5, 2, 8, 1, 4];
        var comparer = Comparer<int>.Create(comparison);
        numbers2.Sort(comparer); // Sort in descending order using Comparer<T>
        Console.WriteLine($"Sorted numbers using Comparer (descending): {string.Join(", ", numbers2)}");

        Utils.PrintSeparator();

        Console.WriteLine("=== Custom Delegate ===");
        // Reference type
        // Multicast delegate
        // Có variance
        // Có Invoke
        // IL tạo subclass của MulticastDelegate
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
            $"multicastOps(3, 4): {multicastOps(3, 4)}"); // Invokes all 3, returns last result (subtractOp)

        Utils.PrintSeparator();

        // Demo multicast delegate for logging
        // Under the hood, the Delegate.Combine creates a new delegate that chains the invocations (like a linked list).
        // This is immutable; adding/removing handlers creates new delegate instances, not mutating existing ones.
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
        // Method group conversion

        // Static method to delegate -> return same delegate instance because compiler caches it
        // Cache internally = __SumMethod ?? (__SumMethod = new Func<int, int, int>((object) null, __methodptr(SumMethod)));
        Func<int, int, int> methodGroupSum1 = SumMethod;
        // Same cached instance = __SumMethod ?? (__SumMethod = new Func<int, int, int>((object) null, __methodptr(SumMethod)));
        Func<int, int, int> methodGroupSum2 = SumMethod;
        Console.WriteLine(
            $"static method: Compare by reference: {ReferenceEquals(methodGroupSum1, methodGroupSum2)}"); // True
        Console.WriteLine($"static method: Compare by equality: {methodGroupSum1.Equals(methodGroupSum2)}"); // True
        Console.WriteLine($"static method: Compare by equality: {methodGroupSum1 == methodGroupSum2}"); // True

        // Once target + instance method to delegate -> return new delegate instance each time
        // but they are equal in terms of method and target (Equals and == return true)
        var target = new TargetClass();
        // new Func<int, int, int>((object) target, __methodptr(InstanceMethod));
        Func<int, int, int> instanceMethodDelegate1 = target.InstanceMethod;
        // new Func<int, int, int>((object) target, __methodptr(InstanceMethod));
        Func<int, int, int> instanceMethodDelegate2 = target.InstanceMethod;
        Console.WriteLine(
            $"instance method: Compare by reference: {ReferenceEquals(instanceMethodDelegate1, instanceMethodDelegate2)}"); // False
        Console.WriteLine(
            $"instance method: Compare by equality: {instanceMethodDelegate1.Equals(instanceMethodDelegate2)}"); // True
        Console.WriteLine(
            $"instance method: Compare by equality: {instanceMethodDelegate1 == instanceMethodDelegate2}"); // True

        // Two different target + instance method to delegate -> different delegate instances -> always false
        // = new Func<int, int, int>((object) new DelegatesPlayground.TargetClass(), __methodptr(InstanceMethod));
        Func<int, int, int> instanceMethodDelegate3 = new TargetClass().InstanceMethod;
        // = new Func<int, int, int>((object) new DelegatesPlayground.TargetClass(), __methodptr(InstanceMethod));
        Func<int, int, int> instanceMethodDelegate4 = new TargetClass().InstanceMethod;
        Console.WriteLine(
            $"different instance method: Compare by reference: {ReferenceEquals(instanceMethodDelegate3, instanceMethodDelegate4)}"); // False
        Console.WriteLine(
            $"different instance method: Compare by equality: {instanceMethodDelegate3.Equals(instanceMethodDelegate4)}"); // False
        Console.WriteLine(
            $"different instance method: Compare by equality: {instanceMethodDelegate3 == instanceMethodDelegate4}"); // False

        Console.WriteLine("✅ Delegates Playground finished successfully.");
    }

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
        // In reality, you would use LINQ's Where method for this.
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
    private static int ReduceInts(List<int> numbers, MathOperation operation, int intialValue)
    {
        // In reality, you would use LINQ's Aggregate method for this.
        var result = intialValue;
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var num in numbers)
        {
            result = operation(result, num);
        }

        return result;
    }
}