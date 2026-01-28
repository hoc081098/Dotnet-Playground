namespace CsharpPlayground.AsyncProgramming;

public class SemaphoreSlimDemo
{
    public static void Run()
    {
        // ```
        // Wait():
        //   if currentCount > 0:
        //     currentCount--
        //     proceed
        //   else:
        //     enqueue waiter
        //     wait
        // 
        // Release():
        //   if waiter exists:
        //     wake one waiter
        //   else if currentCount < maxCount:
        //     currentCount++
        //   else:
        //     throw
        // ```
        Gate();
        Basic();
    }

    private static void Gate()
    {
        // initialCount = 0, maxCount = 3
        // ⇒ Initially, no one can pass through
        // ⇒ Only when .Release() is called, the gate opens
        // This is essentially a gate / latch / signal, not a pure "concurrency limiting" semaphore.
        var gate = new SemaphoreSlim(initialCount: 0, maxCount: 3);

        // count = 0
        // All Wait() / WaitAsync() calls → block / await
        // Mental model example:
        // • Consumer: WaitAsync() → waits
        // • Producer / Controller: Release() → allows continuation

        List<Task> tasks =
        [
            Task.Run(async () =>
            {
                Console.WriteLine("[1] start, waiting...");
                await gate.WaitAsync(); // Wait until allowed to proceed
                Console.WriteLine("[1] end");
            }),

            Task.Run(async () =>
            {
                Console.WriteLine("[2] start, waiting...");
                await gate.WaitAsync(); // Wait until allowed to proceed
                Console.WriteLine("[2] end");
            }),

            Task.Run(async () =>
            {
                Console.WriteLine("[3] start, waiting...");
                await gate.WaitAsync(); // Wait until allowed to proceed
                Console.WriteLine("[3] end");
            }),
        ];

        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Open gate for 1 task
            Console.WriteLine("Releasing gate for 1 task.");
            gate.Release();

            await Task.Delay(TimeSpan.FromSeconds(2));

            // Open gate for 1 task
            Console.WriteLine("Releasing gate for 1 task.");
            gate.Release();

            await Task.Delay(TimeSpan.FromSeconds(3));

            // Open gate for 1 task
            Console.WriteLine("Releasing gate for 1 task.");
            gate.Release();
        });

        Task.WaitAll(tasks);
    }

    private static void Basic()
    {
        var semaphore = new SemaphoreSlim(initialCount: 2, maxCount: 2);

        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            var curI = i;
            var task = Task.Run(async () =>
            {
                var acquired = await semaphore.WaitAsync(timeout: TimeSpan.FromSeconds(30));
                // WaitAsync(timeout) to avoid infinite deadlock -> returns false if unable to acquire
                // WaitAsync() -> returns a Task that never completes if unable to acquire
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