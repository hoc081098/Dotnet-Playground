namespace CsharpPlayground.AsyncProgramming;

public readonly record struct Unit
{
    public static readonly Unit Value = new();
}

public static class TaskExtensions
{
    // ReSharper disable once InconsistentNaming
    public static async Task<TResult> Select<TSource, TResult>(
        this Task<TSource> sourceTask,
        Func<TSource, TResult> selector,
        bool continueOnCapturedContext = false) =>
        selector(await sourceTask.ConfigureAwait(continueOnCapturedContext: continueOnCapturedContext));

    // ReSharper disable once InconsistentNaming
    public static async Task<TResult> SelectMany<TSource, TResult>(
        this Task<TSource> sourceTask,
        Func<TSource, Task<TResult>> selector,
        bool continueOnCapturedContext = false
    )
    {
        var source = await sourceTask.ConfigureAwait(continueOnCapturedContext: continueOnCapturedContext);
        return await selector(source).ConfigureAwait(continueOnCapturedContext: continueOnCapturedContext);
    }

    // ReSharper disable once InconsistentNaming
    public static async Task<TResult> SelectMany<TSource, TCollection, TResult>(
        this Task<TSource> sourceTask,
        Func<TSource, Task<TCollection>> collectionSelector,
        Func<TSource, TCollection, TResult> resultSelector,
        bool continueOnCapturedContext = false)
    {
        var source = await sourceTask.ConfigureAwait(continueOnCapturedContext: continueOnCapturedContext);
        var collection = await collectionSelector(source)
            .ConfigureAwait(continueOnCapturedContext: continueOnCapturedContext);
        return resultSelector(source, collection);
    }

    // ReSharper disable once InconsistentNaming
    public static Task<Unit> ToUnitTask(this Task task)
    {
        return task.IsCompletedSuccessfully
            ? Task.FromResult(Unit.Value)
            : AwaitSlow(task);

        static async Task<Unit> AwaitSlow(Task t)
        {
            await t.ConfigureAwait(false);
            return Unit.Value;
        }
    }

    public static void Run()
    {
        Task.Delay(100)
            .ToUnitTask()
            .Select(_ => "Result from non-generic Task")
            .ContinueWith(PrintTaskResult)
            .Wait();

        Task.FromException<int>(new InvalidOperationException("Simulated failure"))
            .Select(_ => "Result from non-generic Task")
            .ContinueWith(PrintTaskResult)
            .Wait();

        Task.Run(() => 42)
            .SelectMany(v => Task.Delay(100).ToUnitTask().Select(_ => v + 1))
            .ContinueWith(PrintTaskResult)
            .Wait();

        Task.FromException<int>(new InvalidOperationException("Outer error"))
            .SelectMany(v => Task.Delay(100).ToUnitTask().Select(_ => v + 1))
            .ContinueWith(PrintTaskResult)
            .Wait();

        Task.Run(() => 42)
            .SelectMany(_ => Task.FromException<int>(new InvalidOperationException("Inner error")))
            .ContinueWith(PrintTaskResult)
            .Wait();
    }

    public static void RunQuerySyntax()
    {
        var result = from i in Task.FromResult(42)
            from j in Task.Delay(2000).ToUnitTask().Select(_ => i + 1)
            select new { i, j };

        result.ContinueWith(PrintTaskResult).Wait();
    }

    private static void PrintTaskResult<T>(Task<T> t)
    {
        switch (t)
        {
            case { IsCanceled: true }:
                Console.WriteLine("Task was canceled.");
                break;
            case { IsFaulted: true, Exception: { } ex }:
                Console.WriteLine("Task faulted with exception: " + ex.Message);
                break;
            default:
                Console.WriteLine($"Status is {t.Status}, value is {t.Result.AsPrintable()}");
                break;
        }
    }
}