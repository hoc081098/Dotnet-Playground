namespace CsharpPlayground.AsyncProgramming;

public class SemaphoreSlimDemo
{
    public static void Run()
    {
        var semaphore = new SemaphoreSlim(initialCount: 2, maxCount: 2);

        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            var curI = i;
            var task = Task.Run(async () =>
            {
                var acquired = await semaphore.WaitAsync(timeout: TimeSpan.FromSeconds(30));
                if (acquired)
                {
                    try
                    {
                        Console.WriteLine($"Index {curI} acquired the semaphore.");
                        await Task.Delay(2000); // Simulate some work
                        Console.WriteLine($"Index {curI} releasing the semaphore.");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            });
            tasks.Add(task);
        }

        Task.WaitAll(tasks);
    }
}