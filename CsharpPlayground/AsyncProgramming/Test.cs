using System.Collections.Concurrent;

namespace CsharpPlayground.AsyncProgramming;

// Link: https://stackoverflow.com/a/43571653
// Posted by N73k
// Retrieved 2025-11-17, License - CC BY-SA 3.0

sealed class SingleThreadSynchronizationContext : SynchronizationContext
{
    readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> _queue = new();

    public override void Post(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);

        if (!_queue.IsAddingCompleted)
        {
            _queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
        }
    }

    public void RunOnCurrentThread()
    {
        foreach (var workItem in _queue.GetConsumingEnumerable())
        {
            workItem.Key(workItem.Value);
        }
    }

    public void Complete()
    {
        _queue.CompleteAdding();
    }
}

internal static class SingleThreadSimulator
{
    public static Task Run(Func<Task> asyncMethod)
    {
        return Task.Run<Task>(async () =>
        {
            ArgumentNullException.ThrowIfNull(asyncMethod);

            // Save off the old context so we can restore it when we're done
            var previousContext = SynchronizationContext.Current;

            try
            {
                var context = new SingleThreadSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);

                // Invoke the function and alert the context when it's complete
                var task = asyncMethod() ?? throw new InvalidOperationException("No task provided.");

                _ = task.ContinueWith(_ => context.Complete(), TaskScheduler.Default);

                // Start working through the queue
                context.RunOnCurrentThread();

                await task;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }).Unwrap();
    }
}

public static class Test
{
    public static void StartHere()
    {
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

        LogCurrentSyncContext("1.1"); // Context #1
        var t = FuncFAsync();
        LogCurrentSyncContext("1.2"); // Context #1, why not Context #2?
        t.Wait();
        LogCurrentSyncContext("1.3"); // Context #1
    }


    private static async Task FuncFAsync()
    {
        await SingleThreadSimulator.Run(async () =>
        {
            LogCurrentSyncContext("2.1"); // Context #2
            await Task.Delay(7000);
            LogCurrentSyncContext("2.2"); // Context #2
            await Task.Delay(7000);
            LogCurrentSyncContext("2.3"); // Context #2
        });

        LogCurrentSyncContext("2.4"); // Context #1
    }


    // Just show the current Sync Context. Pass in some kind of marker so we know where, in the code, the logging is happening
    private static void LogCurrentSyncContext(object marker)
    {
        var sc = SynchronizationContext.Current;
        var name = sc is null ? "null" : sc.GetType().Name + sc.GetHashCode();
        Console.WriteLine(marker + " Thread: " + Environment.CurrentManagedThreadId + ", SyncContext: " + name);
    }
}