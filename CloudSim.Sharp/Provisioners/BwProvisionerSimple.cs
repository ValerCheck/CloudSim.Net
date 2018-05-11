using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Provisioners
{
    public class BwProvisionerSimple : BwProvisioner
    {
        public BwProvisionerSimple(long bw) : base(bw)
        {
        }

        public override bool AllocateBwForVm(Vm vm, long bw)
        {
            throw new NotImplementedException();
        }

        public override void DeallocateBwForVm(Vm vm)
        {
            throw new NotImplementedException();
        }

        public override long GetAllocatedBwForVm(Vm vm)
        {
            throw new NotImplementedException();
        }

        public override bool IsSuitableForVm(Vm vm, long bw)
        {
            throw new NotImplementedException();
        }
    }
}
