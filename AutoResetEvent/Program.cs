using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoResetEvent
{
    using System;
    using System.Threading;

    class Example
    {
        private static AutoResetEvent event_1 = new AutoResetEvent(false);
        private static AutoResetEvent event_2 = new AutoResetEvent(false);
        private static int finishedThreads = 0;

        static void Main()
        {
            Console.WriteLine("Press Enter to create three threads and start them.\r\n");
            Console.ReadLine();

            for (int i = 1; i < 4; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }
            Thread.Sleep(250);

            while (finishedThreads < 3)
            {
                Console.WriteLine("Enter 1 to turn WaitHandle 1 into signalled state. Enter 2 to turn WaitHandle 2 into signalled state.");
                int eventNumber = 0;
                var eventNumberEntered = int.TryParse(Console.ReadLine(), out eventNumber);

                if (eventNumber == 1)
                {
                    event_1.Set();
                    Thread.Sleep(250);
                }
                else if (eventNumber == 2)
                {
                    event_2.Set();
                    Thread.Sleep(250);
                }
                else {
                    Console.WriteLine("incorrect event number.");
                }                
            }

            Console.WriteLine("job's done");
            Console.ReadLine();
        }

        static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine("{0} waits on AutoResetEvent #1.", name);
            event_1.WaitOne();
            Console.WriteLine("{0} is released from AutoResetEvent #1.", name);

            Console.WriteLine("{0} waits on AutoResetEvent #2.", name);
            event_2.WaitOne();
            Console.WriteLine("{0} is released from AutoResetEvent #2.", name);

            Console.WriteLine("{0} ends.", name);
            finishedThreads++;
        }
    }

}
