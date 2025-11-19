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

public static class LockPlayground
{
    // When the type of the expression is precisely System.Threading.Lock,
    // the lock statement compiles to using(x.EnterScope()).
    // Otherwise, it uses Monitor.
    private static readonly Lock _lock = new();

    public static void Run()
    {
        var i = 0;

        // using lock statement
        // - with System.Threading.Lock _lock <-> using (_lock.EnterScope)
        // - with object _lock <-> System.Threading.Monitor.Enter(_lock, ref lockTaken)
        //                          -> finally: if (lockTaken) Monitor.Exit(_lock)
        var t1 = Task.Run(() =>
        {
            lock (_lock)
            {
                i++;
            }
        });
        var t2 = Task.Run(() =>
        {
            lock (_lock)
            {
                i++;
            }
        });

        // using lock.Enter and lock.Exit
        var t3 = Task.Run(() =>
        {
            _lock.Enter();
            i++;
            _lock.Exit();
        });

        // using `using (lock.EnterScope())`
        var t4 = Task.Run(() =>
        {
            using (_lock.EnterScope())
            {
                i++;
            }
        });

        lock (_lock)
        {
            i++;
        }

        // blocking until all task are completed.
        Task.WaitAll(t1, t2, t3, t4);
        Console.WriteLine(i); // should be 5
    }
}