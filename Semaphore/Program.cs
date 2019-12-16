using System;
using System.Threading;
using System.Threading.Tasks;

public class SemaphoreExample
{
    private static SemaphoreSlim semaphore;
    private static int completedThreads = 0;

    public static void Main()
    {
        // Create the semaphore.
        semaphore = new SemaphoreSlim(0, 3);
        WriteSemaphoreStatus();

        // Create and start five numbered tasks.
        for (int i = 0; i < 5; i++)
        {
            var currentTaskId = i;
            var thread = new Thread(() => {
                // Each task begins by requesting the semaphore.
                Console.WriteLine("Task {0} begins and waits for the semaphore.", currentTaskId);
                semaphore.Wait();

                Console.WriteLine("Task {0} enters the semaphore.", currentTaskId);

                // this simulate some work
                Thread.Sleep(100);

                Console.WriteLine("Task {0} releases the semaphore; previous count: {1}.",
                                  currentTaskId, semaphore.Release());

                Interlocked.Add(ref completedThreads, 1);
            });
            thread.Start();
        }

        Thread.Sleep(1000);
        // now all threads are started, let's set semaphore count to 3 so 3 threads will be able to step into protected code
        semaphore.Release(3);

        WriteSemaphoreStatus();

        while (completedThreads < 5) {
            Thread.Sleep(100);
        }

        Console.WriteLine("Main thread exits.");
        Console.ReadLine();
    }

    private static void WriteSemaphoreStatus()
    {
        Console.WriteLine("{0} tasks can enter the semaphore.",
                          semaphore.CurrentCount);
    }
}