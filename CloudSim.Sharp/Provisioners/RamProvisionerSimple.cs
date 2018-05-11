using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Provisioners
{
    public class RamProvisionerSimple : RamProvisioner
    {
        public RamProvisionerSimple(int ram) : base(ram)
        {
        }

        public override bool AllocateRamForVm(Vm vm, int ram)
        {
            throw new NotImplementedException();
        }

        public override void DeallocateRamForVm(Vm vm)
        {
            throw new NotImplementedException();
        }

        public override int GetAllocatedRamForVm(Vm vm)
        {
            throw new NotImplementedException();
        }

        public override bool IsSuitableForVm(Vm vm, int ram)
        {
            throw new NotImplementedException();
        }
    }
}
