using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorWaitPulse
{

    class Program
    {
        public static Boat SyncBoat;
        public static Random Generator;

        static void Main(string[] args)
        {
            SyncBoat = new Boat();
            Generator = new Random();

            var routingCrewThread = new Thread(RoutingCrew);
            routingCrewThread.IsBackground = true;
            routingCrewThread.Start();

            var loadingCrewThread = new Thread(LoadingCrew);
            loadingCrewThread.IsBackground = true;
            loadingCrewThread.Start();

            Console.ReadLine();
        }

        public static void RoutingCrew()
        {
            try
            {
                Monitor.Enter(SyncBoat);
                Console.WriteLine("entered lock by routing crew", SyncBoat.BoatId);
                for (int shippedBoats = 0; shippedBoats < 10; shippedBoats++)
                {
                    while (SyncBoat.LoadingStatus != LoadingStatus.Loaded)
                    {
                        Console.WriteLine("routing crew waits for boat status change");
                        Monitor.Wait(SyncBoat, 2000);
                    }

                    Console.WriteLine("letting boat {0} with {1} packages to leave the port", SyncBoat.BoatId, SyncBoat.PackagesLoaded);
                    // simulate boat leaving the port and new boat landing
                    Thread.Sleep(500);
                    SyncBoat.Reset();

                    Console.WriteLine("letting boat {0} packages to enter the port", SyncBoat.BoatId);

                    //Monitor.Pulse(SyncBoat);
                    //Console.WriteLine("routing crew pulsed", SyncBoat.BoatId);
                }
            }
            finally
            {
                Monitor.Exit(SyncBoat);
            }
        }

        public static void LoadingCrew()
        {
            try
            {
                Monitor.Enter(SyncBoat);
                Console.WriteLine("entered lock by loading crew", SyncBoat.BoatId);
                while (true) { 
                    while (SyncBoat.LoadingStatus != LoadingStatus.ReadyForLoad)
                    {
                        Console.WriteLine("loading crew waits for boat status change");
                        Monitor.Wait(SyncBoat, 2000);
                    }

                    Console.WriteLine("start loading boat {0}", SyncBoat.BoatId);

                    var loadingTime = 0;
                    while (loadingTime < 200)
                    {
                        var packageLoadTime = Generator.Next(1, 10);
                        Thread.Sleep(packageLoadTime);

                        loadingTime += packageLoadTime;
                        SyncBoat.PackagesLoaded++;
                    }
                    Console.WriteLine("total {0} packages are loaded in boat {1}", SyncBoat.PackagesLoaded, SyncBoat.BoatId);
                    SyncBoat.LoadingStatus = LoadingStatus.Loaded;

                    //Monitor.Pulse(SyncBoat);
                    //Console.WriteLine("loading crew pulsed", SyncBoat.BoatId);
                }
            }
            finally
            {
                Monitor.Exit(SyncBoat);
            }
        }
    }
}
