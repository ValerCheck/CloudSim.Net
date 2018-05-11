using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Provisioners
{
    public abstract class BwProvisioner
    {
        private long _bw;
        private long _availableBw;

        public BwProvisioner(long bw)
        {
            Bw = bw;
            AvailableBw = bw;
        }

        public abstract bool AllocateBwForVm(Vm vm, long bw);

        public abstract long GetAllocatedBwForVm(Vm vm);

        public abstract void DeallocateBwForVm(Vm vm);

        public void DeallocateBwForAllVms()
        {
            AvailableBw = Bw;
        }

        public abstract bool IsSuitableForVm(Vm vm, long bw);

        public long Bw
        {
            get { return _bw; }
            protected set { _bw = value; }
        }

        public long UsedBw
        {
            get { return _bw - _availableBw; }
        }

        public long AvailableBw
        {
            get { return _availableBw; }
            protected set { _availableBw = value; }
        }
    }
}
