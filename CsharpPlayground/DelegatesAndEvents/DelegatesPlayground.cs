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

        Console.WriteLine(new string('-', 30));

        // Func<T, TResult> - similar to Kotlin's (T) -> Result
        Func<int> funcWithoutParams = () => Random.Shared.Next();
        Func<int, int> squareFunc = x => x * x;
        var squareFuncInferred = (int x) => x * x; // Type inferred by compiler:  Func<int,int>
        Func<int, int, int> sumFunc = (a, b) => a + b;
        Console.WriteLine($"funcWithoutParams(): {funcWithoutParams()}");
        Console.WriteLine($"squareFunc(5): {squareFunc(5)}");
        Console.WriteLine($"sumFunc(3, 4): {sumFunc(3, 4)}");

        Console.WriteLine(new string('-', 30));

        // Predicate<T> - similar to Kotlin's (T) -> Boolean
        Predicate<int> isEvenPredicate = x => x % 2 == 0;
        // But you can also use Func<int, bool> for the same purpose
        Func<int, bool> isEvenFunc = x => x % 2 == 0;
        var isEvenFuncInferred = (int x) => x % 2 == 0; // Always inferred as Func<int,bool>
        Console.WriteLine($"isEvenPredicate(4): {isEvenPredicate(4)}, {isEvenFunc(4)}");
        Console.WriteLine($"isEvenPredicate(5): {isEvenPredicate(5)}, {isEvenFunc(5)}");
        var eventNumbers = FilterList([1, 2, 3, 4, 5], isEvenPredicate);
        Console.WriteLine($"Even numbers: {string.Join(", ", eventNumbers)}");

        Console.WriteLine(new string('-', 30));

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

        Console.WriteLine(new string('-', 30));

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

        Console.WriteLine(new string('-', 30));

        // Demo multicast delegate for logging
        // Under the hood, the Delegate.Combine creates a new delegate that chains the invocations (like a linked list).
        // This is immutable; adding/removing handlers creates new delegate instances, not mutating existing ones.
        LogHandler logHandler = ConsoleLog;
        logHandler += FileLog;
        logHandler("This is a log message.");

        Console.WriteLine("✅ Delegates Playground finished successfully.");
    }

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
}