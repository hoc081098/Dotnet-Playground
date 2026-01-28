namespace CsharpPlayground.AsyncProgramming;

public class SemaphoreSlimDemo
{
    public static void Run()
    {
        Gate();
        Basic();
    }

    private static void Gate()
    {
        // initialCount = 0, maxCount = 2
        // ⇒ ban đầu không ai được đi qua
        // ⇒ chỉ khi có .Release() thì mới mở cổng
        // Nó đúng nghĩa là gate / latch / signal, không phải semaphore “giới hạn concurrency” thuần nữa.
        var gate = new SemaphoreSlim(initialCount: 0, maxCount: 3);

        // count = 0
        // Mọi Wait() / WaitAsync() → block / await
        // Ví dụ mental model
        // •	Consumer: WaitAsync() → chờ
        // •	Producer / Controller: Release() → cho phép tiếp tục

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
                // WaitAsync(timeout) để tránh deadlock vô hạn -> return false nếu không lấy được
                // WaitAsync() -> return Task that never completes nếu không lấy được
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