namespace CsharpPlayground.DelegatesAndEvents;

/// <summary>
/// Demonstrates delegates, events, and Func/Action types in C#.
/// These are similar to:
/// - Kotlin's function types and lambdas: (Int, String) -> Boolean
/// - Java's functional interfaces: Function, Consumer, Predicate
/// Key concepts:
/// - Delegate: type-safe function pointer
/// - Event: publisher-subscriber pattern based on delegates
/// - Func&lt;T, TResult&gt;: delegate with return value
/// - Action&lt;T&gt;: delegate with no return value (void)
/// - Predicate&lt;T&gt;: delegate that returns bool (Func&lt;T, bool&gt;)
/// </summary>
public static class DelegatesAndEventsPlayground
{
    // Custom delegate definition (similar to defining a function type)
    public delegate int MathOperation(int a, int b);
    public delegate void LogHandler(string message);

    // Func and Action (built-in generic delegates)
    // Func<int, int, int> is equivalent to: int Operation(int a, int b)
    // Action<string> is equivalent to: void DoSomething(string value)

    public static void Run()
    {
        Console.WriteLine("=== Delegates Basics ===");

        // Using custom delegate
        MathOperation add = Add;
        MathOperation subtract = Subtract;
        Console.WriteLine($"10 + 5 = {add(10, 5)}");
        Console.WriteLine($"10 - 5 = {subtract(10, 5)}");

        // Using lambda expression with delegate
        MathOperation multiply = (a, b) => a * b;
        Console.WriteLine($"10 * 5 = {multiply(10, 5)}");

        // Multicast delegates - combining multiple methods
        LogHandler logger = ConsoleLog;
        logger += FileLog; // Add another method
        logger("This is a test message"); // Calls both methods

        Console.WriteLine("\n=== Func and Action ===");

        // Func<T, TResult> - similar to Kotlin's (T) -> Result
        Func<int, int, int> addFunc = (a, b) => a + b;
        Func<string, int> getLength = s => s.Length;
        Func<int, bool> isEven = n => n % 2 == 0;

        Console.WriteLine($"addFunc(7, 3) = {addFunc(7, 3)}");
        Console.WriteLine($"getLength('Hello') = {getLength("Hello")}");
        Console.WriteLine($"isEven(4) = {isEven(4)}");

        // Action<T> - similar to Kotlin's (T) -> Unit
        Action<string> print = msg => Console.WriteLine($"Message: {msg}");
        Action<int, int> printSum = (a, b) => Console.WriteLine($"{a} + {b} = {a + b}");

        print("Hello from Action");
        printSum(5, 10);

        // Higher-order functions (functions that take or return functions)
        var doubled = ApplyOperation([1, 2, 3, 4, 5], x => x * 2);
        Console.WriteLine($"\nDoubled: [{string.Join(", ", doubled)}]");

        var filtered = FilterList([1, 2, 3, 4, 5, 6], x => x % 2 == 0);
        Console.WriteLine($"Even numbers: [{string.Join(", ", filtered)}]");

        // Passing method as parameter
        ProcessNumbers([1, 2, 3], n => Console.WriteLine($"  Processing: {n}"));

        Console.WriteLine("\n=== Events ===");

        // Events demo
        var publisher = new EventPublisher();
        var subscriber1 = new EventSubscriber("Subscriber1");
        var subscriber2 = new EventSubscriber("Subscriber2");

        // Subscribe to event
        publisher.DataReceived += subscriber1.OnDataReceived;
        publisher.DataReceived += subscriber2.OnDataReceived;

        // Trigger event
        publisher.ProcessData("First message");
        publisher.ProcessData("Second message");

        // Unsubscribe
        publisher.DataReceived -= subscriber1.OnDataReceived;
        Console.WriteLine("\nAfter unsubscribing Subscriber1:");
        publisher.ProcessData("Third message");

        Console.WriteLine("\n=== Real-world example: Button click simulation ===");
        var button = new Button("Submit");
        button.Clicked += (sender, args) => Console.WriteLine($"Button '{args.ButtonName}' was clicked at {args.ClickTime:HH:mm:ss}");
        button.Clicked += (sender, args) => Console.WriteLine($"  Validating form...");
        button.Clicked += (sender, args) => Console.WriteLine($"  Submitting data...");
        
        button.Click(); // Simulate click
    }

    // Helper methods for delegates
    private static int Add(int a, int b) => a + b;
    private static int Subtract(int a, int b) => a - b;

    private static void ConsoleLog(string message)
    {
        Console.WriteLine($"[Console] {message}");
    }

    private static void FileLog(string message)
    {
        Console.WriteLine($"[File] {message}");
    }

    // Higher-order function example
    private static List<int> ApplyOperation(List<int> numbers, Func<int, int> operation)
    {
        var result = new List<int>();
        foreach (var num in numbers)
        {
            result.Add(operation(num));
        }
        return result;
    }

    private static List<int> FilterList(List<int> numbers, Predicate<int> predicate)
    {
        var result = new List<int>();
        foreach (var num in numbers)
        {
            if (predicate(num))
            {
                result.Add(num);
            }
        }
        return result;
    }

    private static void ProcessNumbers(List<int> numbers, Action<int> processor)
    {
        Console.WriteLine("Processing numbers:");
        foreach (var num in numbers)
        {
            processor(num);
        }
    }
}

// Event publisher class
public class EventPublisher
{
    // Define an event based on EventHandler delegate
    public event EventHandler<DataEventArgs>? DataReceived;

    public void ProcessData(string data)
    {
        Console.WriteLine($"\nPublisher: Processing data '{data}'");
        
        // Raise the event
        OnDataReceived(new DataEventArgs(data));
    }

    protected virtual void OnDataReceived(DataEventArgs e)
    {
        // Invoke all subscribed handlers
        DataReceived?.Invoke(this, e);
    }
}

// Custom EventArgs class
public class DataEventArgs : EventArgs
{
    public string Data { get; }
    public DateTime Timestamp { get; }

    public DataEventArgs(string data)
    {
        Data = data;
        Timestamp = DateTime.Now;
    }
}

// Event subscriber class
public class EventSubscriber
{
    private readonly string _name;

    public EventSubscriber(string name)
    {
        _name = name;
    }

    public void OnDataReceived(object? sender, DataEventArgs e)
    {
        Console.WriteLine($"  {_name} received: '{e.Data}' at {e.Timestamp:HH:mm:ss.fff}");
    }
}

// Real-world button example
public class Button
{
    public string Name { get; }

    public Button(string name)
    {
        Name = name;
    }

    // Event declaration
    public event EventHandler<ButtonClickEventArgs>? Clicked;

    public void Click()
    {
        Console.WriteLine($"\nButton '{Name}' is being clicked...");
        OnClicked(new ButtonClickEventArgs(Name, DateTime.Now));
    }

    protected virtual void OnClicked(ButtonClickEventArgs e)
    {
        Clicked?.Invoke(this, e);
    }
}

public class ButtonClickEventArgs : EventArgs
{
    public string ButtonName { get; }
    public DateTime ClickTime { get; }

    public ButtonClickEventArgs(string buttonName, DateTime clickTime)
    {
        ButtonName = buttonName;
        ClickTime = clickTime;
    }
}
