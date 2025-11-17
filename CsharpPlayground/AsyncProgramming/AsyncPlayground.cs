namespace CsharpPlayground.AsyncProgramming;

/// <summary>
/// Demonstrates async/await patterns in C#, similar to Kotlin coroutines and Java CompletableFuture.
/// 
/// Key differences from Kotlin:
/// - C# uses Task and Task&lt;T&gt; (similar to Kotlin's Deferred and Java's CompletableFuture)
/// - async/await keywords (similar to Kotlin's suspend functions)
/// - Task.Run for background work (similar to Kotlin's launch/async with Dispatchers.IO)
/// - ConfigureAwait(false) for library code (no direct Kotlin equivalent, related to context switching)
/// - Console apps have no SynchronizationContext, similar to ASP.NET Core. Continuations after `await`
///   resume on ThreadPool threads.
/// </summary>
public static class AsyncPlayground
{
    public static void Run() => RunAsync().GetAwaiter().GetResult();

    public static async Task RunAsync()
    {
        Console.WriteLine("=== Async/Await Basics ===");

        Utils.PrintSeparator();
        Console.WriteLine("SynchronizationContext.Current is null: " + (SynchronizationContext.Current is null));
        await DemoContinueWithAsync();
        Utils.PrintSeparator();

        // Basic async/await - similar to Kotlin's suspend function
        var data = await GetDataAsync();
        Console.WriteLine($"GetDataAsync: {data}");

        Utils.PrintSeparator();

        // Multiple async operations in sequence
        var data1 = await GetData1Async();
        var data2 = await GetData2Async(data1);
        Console.WriteLine($"GetData1Async: {data1}, GetData2Async: {data2}");

        Utils.PrintSeparator();

        // Task.WhenAll: Parallel execution - similar to Kotlin's async/await/awaitAll.
        var task1 = FetchData1Async();
        var task2 = FetchData1Async(); // Running in parallel - don't use await here
        var results = await Task.WhenAll(task1, task2);
        Console.WriteLine($"Parallel: task1={task1.Result}, task2={task2.Result}, results={results[0]}, {results[1]}");

        Utils.PrintSeparator();

        // Task.WhenAny: first completed task - similar to Kotlin's select expression or Observable.race in ReactiveX.
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var fastTask = DelayAndReturnAsync(100, "Fast", cancellationToken);
        var slowTask = DelayAndReturnAsync(2000, "Slow", cancellationToken);
        PrintStatus(fastTask, slowTask, 1);

        var firstCompletedTask = await Task.WhenAny(fastTask, slowTask);
        PrintStatus(fastTask, slowTask, 2);
        await cancellationTokenSource.CancelAsync();
        var fastResult = await firstCompletedTask;

        PrintStatus(fastTask, slowTask, 3);
        Console.WriteLine($"First completed: {fastResult}");
        Console.WriteLine($"IsFast: {ReferenceEquals(fastTask, firstCompletedTask)}, " +
                          $"IsSlow: {ReferenceEquals(slowTask, firstCompletedTask)}");

        Utils.PrintSeparator();

        // Exception handling in async code
        try
        {
            await ThrowExceptionAsync();
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine($"Caught exception from async method: {exception.Message}");
        }

        Utils.PrintSeparator();

        try
        {
            // Timeout with CancellationToken - similar to Kotlin's withTimeout
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));
            await LongRunningOperationAsync(timeoutCts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled due to timeout");
        }

        Utils.PrintSeparator();

        try
        {
            // Linked CancellationTokens - similar to Kotlin's Job and withTimeout combined
            using var manualCts = new CancellationTokenSource();
            var manualCancellationToken = manualCts.Token;

            // Manual cancellation occurs before timeout
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                manualCancellationToken,
                new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token
            );

            // ReSharper disable once MethodSupportsCancellation
            var disposeTask = Task.Run(() =>
            {
                Thread.Sleep(250);
                CancelSilently(manualCts);
            });

            await LongRunningOperationAsync(linkedCts.Token);
            await disposeTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled due to manual cancellation");
        }

        try
        {
            // Linked CancellationTokens - similar to Kotlin's Job and withTimeout combined
            using var manualCts = new CancellationTokenSource();
            var manualCancellationToken = manualCts.Token;

            // Timeout occurs before manual cancellation
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                manualCancellationToken,
                new CancellationTokenSource(TimeSpan.FromMilliseconds(250)).Token
            );

            // ReSharper disable once MethodSupportsCancellation
            var disposeTask = Task.Run(() =>
            {
                Thread.Sleep(500);
                CancelSilently(manualCts);
            });

            await LongRunningOperationAsync(linkedCts.Token);
            await disposeTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled due to timeout");
        }

        Utils.PrintSeparator();

        // Task.Run for CPU-bound work - similar to Kotlin's withContext(Dispatchers.Default)
        Console.WriteLine("Before: current thread is " + Environment.CurrentManagedThreadId);
        var expensiveComputationTask = Task.Run(() => ExpensiveComputation(iterations: 5_000_000));
        var computeResult = await expensiveComputationTask;
        Console.WriteLine($"Compute result: {computeResult}");

        Utils.PrintSeparator();

        // ValueTask for performance-critical scenarios
        Console.WriteLine($"Cached result 1: {await GetCachedDataAsync(useCache: true)}");
        Console.WriteLine($"Cached result 2: {await GetCachedDataAsync(useCache: true)}");
        Console.WriteLine($"Cached result 3: {await GetCachedDataAsync(useCache: true)}");
        Console.WriteLine($"Cached result 4: {await GetCachedDataAsync(useCache: true)}");

        Console.WriteLine($"Get result 5: {await GetCachedDataAsync(useCache: false)}");
        Console.WriteLine($"Get result 6: {await GetCachedDataAsync(useCache: false)}");
        await Task.Delay(200);

        // Cannot use `await` multiple times on the same ValueTask instance.
        // If you want to do that, convert it to Task first via .AsTask() (call .AsTask only once).
        var nonCachedTask = GetCachedDataAsync(useCache: false).AsTask();
        Console.WriteLine($"Get result 7: {await nonCachedTask}");
        Console.WriteLine($"Get result 8: {await nonCachedTask}");
        Console.WriteLine($"Get result 9: {await nonCachedTask}");

        Utils.PrintSeparator();

        // .GetAwaiter().GetResult(), .Result and .Wait(): blocking calls to get result synchronously.

        Console.WriteLine("GetAwaiter().GetResult(): " + GetDataAsync().GetAwaiter().GetResult());
        Console.WriteLine("Task.Result: " + GetDataAsync().Result);
        GetDataAsync().Wait(); // .Wait() is similar to .Result but it does not return the result

        // The difference is that .GetAwaiter().GetResult() unwraps AggregateException and throws the original exception,
        // while .Result and .Wait() wrap exceptions in AggregateException.
        try
        {
            AnotherThrowExceptionAsync().GetAwaiter().GetResult();
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine(".GetAwaiter().GetResult() caught InvalidOperationException");
        }

        try
        {
            _ = AnotherThrowExceptionAsync().Result;
        }
        catch (AggregateException ae) when (ae.InnerException is InvalidOperationException)
        {
            Console.WriteLine(
                ".Result caught AggregateException with InnerException of type InvalidOperationException");
        }

        try
        {
            AnotherThrowExceptionAsync().Wait();
        }
        catch (AggregateException ae) when (ae.InnerException is InvalidOperationException)
        {
            Console.WriteLine(
                ".Wait() caught AggregateException with InnerException of type InvalidOperationException");
        }
    }

    // Demo: Correct vs Misuse of ContinueWith
    private static async Task DemoContinueWithAsync()
    {
        Console.WriteLine("=== ContinueWith Demo ===");

        // GOOD USAGE: Explicit scheduler + handling errors
        var goodTask = Task.Run<int>(() =>
        {
            Console.WriteLine("GoodTask thread = " + Environment.CurrentManagedThreadId);
            Thread.Sleep(200);
            throw new InvalidOperationException("Error in goodTask");
            return 123;
        });

        await goodTask.ContinueWith(
            continuationAction: t =>
            {
                Console.WriteLine("GOOD: Continuation thread = " + Environment.CurrentManagedThreadId);
                if (t.IsFaulted)
                {
                    Console.WriteLine("GOOD: Faulted: " + t.Exception);
                }
                else
                {
                    Console.WriteLine("GOOD: Result = " + t.Result);
                }
            },
            cancellationToken: CancellationToken.None,
            continuationOptions: TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously,
            scheduler: TaskScheduler.Default
        );

        Utils.PrintSeparator();

        // GOOD USAGE: Explicit scheduler + handling errors
        var goodTask2 = Task.Run<int>(() =>
        {
            Console.WriteLine("GoodTask2 thread = " + Environment.CurrentManagedThreadId);
            Thread.Sleep(200);
            return 123;
        });

        await goodTask2.ContinueWith(
            continuationAction: t =>
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine("GOOD2: Faulted: " + t.Exception);
                }
                else
                {
                    Console.WriteLine("GOOD2: Result = " + t.Result);
                    Console.WriteLine("GOOD2: Continuation thread = " + Environment.CurrentManagedThreadId);
                }
            },
            cancellationToken: CancellationToken.None,
            continuationOptions: TaskContinuationOptions.NotOnCanceled,
            scheduler: TaskScheduler.Default
        );

        Utils.PrintSeparator();

        // MISUSE #1: UI / sync context violation (simulated — prints wrong thread)
        var badTask1 = Task.Run(() =>
        {
            Thread.Sleep(100);
            return "Hello";
        });

        await badTask1.ContinueWith(t =>
        {
            // Dev THINKS it's back on UI thread → but it is TaskScheduler.Current where ContinueWith is called.
            Console.WriteLine("BAD1: Running on thread " + Environment.CurrentManagedThreadId);
            Console.WriteLine("BAD1: IsFaulted = " + t.IsFaulted);
        });

        // MISUSE #2: Swallowing exceptions unintentionally
        var badTask2 = Task.Run(() => throw new InvalidOperationException("Simulated error in badTask2"));

        await badTask2.ContinueWith(t =>
        {
            // Wrong: not checking t.IsFaulted → exception silently swallowed
            Console.WriteLine("BAD2: This should NOT be considered successful");
            Console.WriteLine("BAD2: Status is " + t.Status);
        });
    }

    // Note:
    // In modern C#, prefer async/await over Task.ContinueWith for composing asynchronous operations.
    // - ContinueWith is low-level, easy to misuse, and does not respect SynchronizationContext by default.
    // - async/await gives clearer control flow and simpler exception handling.
    // Here we model "do something after delay" with an async method instead of ContinueWith.
    private static async Task<string> DelayAndReturnAsync(int delayMs, string label, CancellationToken ct = default)
    {
        await Task.Delay(delayMs, ct);
        return label;
    }

    private static void CancelSilently(CancellationTokenSource manualCts)
    {
        try
        {
            manualCts.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // This 'T:System.Threading.CancellationTokenSource' has been disposed.
        }
        catch (AggregateException)
        {
            // An aggregate exception containing all the exceptions thrown by the registered callbacks on the associated 'T:System.Threading.CancellationToken'.
        }
    }

    private static void PrintStatus(Task<string> fastTask, Task<string> slowTask, int tag) =>
        Console.WriteLine($"[{tag}] fastTask status: {fastTask.Status}, slowTask status: {slowTask.Status}");

    // Basic async method - similar to Kotlin's suspend fun
    private static async Task<int> GetDataAsync()
    {
        // Simulate asynchronous work
        await Task.Delay(1000);
        return 42;
    }

    private static async Task<int> GetData1Async()
    {
        // Simulate asynchronous work
        await Task.Delay(1000);
        return 200;
    }

    private static async Task<string> GetData2Async(int input)
    {
        // Simulate asynchronous work
        await Task.Delay(1000);
        return "Processed " + input;
    }

    private static async Task<int> FetchData1Async()
    {
        Console.WriteLine("Fetching data 1: starting...");
        await Task.Delay(2000);
        Console.WriteLine("Fetching data 1: completed.");
        return 42;
    }

    private static async Task ThrowExceptionAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Simulated error");
    }

    private static async Task<string> AnotherThrowExceptionAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Simulated error");
    }

    private static async Task LongRunningOperationAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"cancellationToken == none: {cancellationToken == CancellationToken.None}");

        for (var i = 0; i < 10; i++)
        {
            Console.WriteLine("LongRunningOperationAsync: processing step " + i);
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(100, cancellationToken);
        }
    }

    // CPU-bound synchronous operation
    private static long ExpensiveComputation(int iterations)
    {
        Console.WriteLine($"Task.Run thread ID: {Environment.CurrentManagedThreadId}");
        Console.WriteLine($"Is background thread: {Thread.CurrentThread.IsBackground}");
        Console.WriteLine($"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}");

        long sum = 0;
        for (var i = 0; i < iterations; i++)
        {
            sum += i;
        }

        return sum;
    }

    // ValueTask for optimized async operations (avoids allocation when result is already available)
    private static ValueTask<string> GetCachedDataAsync(bool useCache)
    {
        return useCache
            ? new ValueTask<string>("Cache result")
            : new ValueTask<string>(FetchFromDatabaseAsync());
    }

    private static async Task<string> FetchFromDatabaseAsync()
    {
        Console.WriteLine("Fetching database result...");
        await Task.Delay(100);
        return "Data from database " + DateTimeOffset.Now;
    }
}