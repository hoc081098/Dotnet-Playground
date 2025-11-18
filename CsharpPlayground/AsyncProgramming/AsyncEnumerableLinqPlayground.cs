namespace CsharpPlayground.AsyncProgramming;

public static class AsyncEnumerableLinqPlayground
{
    public static void Run()
    {
        // Select, Where, ... from System.Linq.AsyncEnumerable
        Task.Run(async () =>
        {
            await foreach (var e in ProcessAsync())
                Console.WriteLine("[3] Result: {0}", e);
        }).Wait();
    }

    private static IAsyncEnumerable<string> ProcessAsync() =>
        Enumerable.Range(start: 1, count: 20)
            .ToAsyncEnumerable()
            .Where(FilterAsync)
            .Select(MapAsync);

    private static async ValueTask<bool> FilterAsync(int element, int index, CancellationToken cancellationToken)
    {
        Console.WriteLine("[1] FilterAsync: element={0}, index={1}", element, index);
        await Task.Delay(500, cancellationToken); // Simulate async work
        return element % 2 == 0;
    }

    private static async ValueTask<string> MapAsync(int item, CancellationToken cancellationToken)
    {
        Console.WriteLine("[2] MapAsync: item={0}", item);
        await Task.Delay(1000, cancellationToken); // Simulate async work
        return "got " + item;
    }
}