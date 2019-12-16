using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorWaitPulse
{
    public enum LoadingStatus {
        ReadyForLoad,
        Loaded,
    }

    public class Boat
    {
        public LoadingStatus LoadingStatus { get; set; }

        public int PackagesLoaded { get; set; }

        public int BoatId { get; set; }

        public void Reset() {
            LoadingStatus = LoadingStatus.ReadyForLoad;
            PackagesLoaded = 0;
            BoatId++;
        }
    }
}
