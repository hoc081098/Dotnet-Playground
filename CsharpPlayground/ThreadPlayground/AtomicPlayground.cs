namespace CsharpPlayground.ThreadPlayground;

public class AtomicReference<T>(T value)
    where T : class
{
    private T _value = value;

    public T Value
    {
        get => Volatile.Read(ref _value);
        set => Volatile.Write(ref _value, value);
    }

    public T GetAndSet(T newValue) => Interlocked.Exchange(ref _value, newValue);

    public bool CompareAndSet(T expectedValue, T newValue)
    {
        var old = Interlocked.CompareExchange(ref _value, newValue, expectedValue);
        return ReferenceEquals(old, expectedValue);
    }

    public T CompareAndExchange(T expectedValue, T newValue)
        => Interlocked.CompareExchange(ref _value, newValue, expectedValue);
}

internal record DemoData(int Id, string Name);

public static class AtomicPlayground
{
    public static void Run()
    {
        var atomicRef = new AtomicReference<DemoData>(new DemoData(0, "Initial"));
        var newValue = new DemoData(2, "Two");

        Task.Run(() =>
        {
            Thread.Sleep(10);
            var old = atomicRef.GetAndSet(newValue);
            Console.WriteLine($"[2] GetAndSet: Old={old}, New={atomicRef.Value}");
        });

        Task.Run(() =>
        {
            var old = atomicRef.GetAndSet(new DemoData(1, "One"));
            Console.WriteLine($"[1] GetAndSet: Old={old}, New={atomicRef.Value}");
        });

        Task.Run(() =>
        {
            Thread.Sleep(200);
            if (atomicRef.CompareAndSet(newValue, new DemoData(3, "Three")))
            {
                Console.WriteLine($"[3] CompareAndSet: value={atomicRef.Value}");
                atomicRef.Value = newValue;
                Console.WriteLine($"[4] CompareAndSet: value={atomicRef.Value}");
            }
        }).Wait();
    }
}