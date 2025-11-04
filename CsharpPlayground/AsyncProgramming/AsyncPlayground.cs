namespace CsharpPlayground.AsyncProgramming;

/// <summary>
/// Demonstrates async/await patterns in C#, similar to Kotlin coroutines and Java CompletableFuture.
/// Key differences from Kotlin:
/// - C# uses Task and Task&lt;T&gt; (similar to Kotlin's Deferred and Java's CompletableFuture)
/// - async/await keywords (similar to Kotlin's suspend functions)
/// - Task.Run for background work (similar to Kotlin's launch/async with Dispatchers.IO)
/// - ConfigureAwait(false) for library code (no direct Kotlin equivalent, related to context switching)
/// </summary>
public static class AsyncPlayground
{
    public static async Task Run()
    {
        Console.WriteLine("=== Async/Await Basics ===");

        // Basic async/await - similar to Kotlin's suspend function
        var result = await GetDataAsync();
        Console.WriteLine($"Result: {result}");

        // Multiple async operations in sequence
        var data1 = await FetchData1Async();
        var data2 = await FetchData2Async(data1);
        Console.WriteLine($"Sequential: data1={data1}, data2={data2}");

        // Parallel execution - similar to Kotlin's async/await
        var task1 = FetchData1Async();
        var task2 = FetchData1Async(); // Running in parallel
        await Task.WhenAll(task1, task2);
        Console.WriteLine($"Parallel: task1={task1.Result}, task2={task2.Result}");

        // Task.WhenAny - first completed task
        var fastTask = Task.Delay(100).ContinueWith(_ => "Fast");
        var slowTask = Task.Delay(500).ContinueWith(_ => "Slow");
        var firstCompleted = await Task.WhenAny(fastTask, slowTask);
        Console.WriteLine($"First completed: {await firstCompleted}");

        // Exception handling in async code
        try
        {
            await ThrowExceptionAsync();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Caught exception: {ex.Message}");
        }

        // Timeout with CancellationToken
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        try
        {
            await LongRunningOperationAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled due to timeout");
        }

        // Task.Run for CPU-bound work - similar to Kotlin's withContext(Dispatchers.Default)
        var computeResult = await Task.Run(() => ComputeExpensiveOperation(1000000));
        Console.WriteLine($"Compute result: {computeResult}");

        // ValueTask for performance-critical scenarios
        var cachedResult = await GetCachedDataAsync(true);
        Console.WriteLine($"Cached result: {cachedResult}");
    }

    // Basic async method - similar to Kotlin's suspend fun
    private static async Task<string> GetDataAsync()
    {
        await Task.Delay(100); // Simulate I/O operation
        return "Hello from async";
    }

    private static async Task<int> FetchData1Async()
    {
        await Task.Delay(50);
        return 42;
    }

    private static async Task<string> FetchData2Async(int input)
    {
        await Task.Delay(50);
        return $"Processed {input}";
    }

    private static async Task ThrowExceptionAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Simulated error");
    }

    private static async Task LongRunningOperationAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < 10; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(50, cancellationToken);
        }
    }

    // CPU-bound synchronous operation
    private static long ComputeExpensiveOperation(int iterations)
    {
        long sum = 0;
        for (int i = 0; i < iterations; i++)
        {
            sum += i;
        }
        return sum;
    }

    // ValueTask for optimized async operations (avoids allocation when result is already available)
    private static ValueTask<string> GetCachedDataAsync(bool useCache)
    {
        if (useCache)
        {
            // Return synchronously without Task allocation
            return new ValueTask<string>("Cached data");
        }

        // Return async Task
        return new ValueTask<string>(FetchFromDatabaseAsync());
    }

    private static async Task<string> FetchFromDatabaseAsync()
    {
        await Task.Delay(100);
        return "Data from database";
    }
}
