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
            //routingCrewThread.IsBackground = true;
            routingCrewThread.Start();

            Thread.Sleep(100);

            var loadingCrewThread = new Thread(LoadingCrew);
            //loadingCrewThread.IsBackground = true;
            loadingCrewThread.Start();

        }

        public static void RoutingCrew()
        {
            try
            {
                Monitor.Enter(SyncBoat);
                //Console.WriteLine("entered lock by routing crew", SyncBoat.BoatId);
                for (int shippedBoats = 0; shippedBoats < 10; shippedBoats++)
                {
                    while (SyncBoat.LoadingStatus != LoadingStatus.Loaded)
                    {
                        Log("routing crew: waits until boat is loaded");
                        Monitor.Wait(SyncBoat, 10000);
                    }

                    Log("routing crew: letting boat {0} with {1} packages to leave the port", SyncBoat.BoatId, SyncBoat.PackagesLoaded);
                    // simulate boat leaving the port
                    Thread.Sleep(1000);


                    SyncBoat.NewBoatIsAtTheDoor();

                    Log("routing crew: letting boat {0} to enter the port", SyncBoat.BoatId);
                    // simulate new boat arrival
                    Thread.Sleep(1000);

                    Monitor.PulseAll(SyncBoat);
                    Console.WriteLine("Press enter to confirm boat arrived to port");
                    Console.ReadLine();
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
                //Console.WriteLine("entered lock by loading crew", SyncBoat.BoatId);
                while (true) { 
                    while (SyncBoat.LoadingStatus != LoadingStatus.ReadyForLoad)
                    {
                        Log("loading crew: waits until new boat arrives");
                        Monitor.Wait(SyncBoat, 10000);
                    }

                    Log("loading crew: start loading boat {0}", SyncBoat.BoatId);
                    
                    // simulate boat being loaded for some time
                    // Thread.Sleep(1000);

                    var loadingTime = 0;
                    while (loadingTime < 2000)
                    {
                        var packageLoadTime = Generator.Next(10, 100);
                        Thread.Sleep(packageLoadTime);

                        loadingTime += packageLoadTime;
                        SyncBoat.PackagesLoaded++;
                    }
                    Log("loading crew: total {0} packages are loaded in boat {1}", SyncBoat.PackagesLoaded, SyncBoat.BoatId);
                    SyncBoat.LoadingStatus = LoadingStatus.Loaded;

                    Monitor.PulseAll(SyncBoat);
                    Console.WriteLine("Press enter to confirm boat loading is over");
                    Console.ReadLine();
                }
            }
            finally
            {
                Monitor.Exit(SyncBoat);
            }
        }

        private static void Log(string template, params object[] args) {
            Console.WriteLine(String.Format("{0}:{1}: ", DateTime.Now.Minute, DateTime.Now.Second) + String.Format(template, args));
        }
    }
}
