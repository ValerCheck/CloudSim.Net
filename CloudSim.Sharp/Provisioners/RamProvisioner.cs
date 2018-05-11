using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Provisioners
{
    public abstract class RamProvisioner
    {
        private int _ram;
        private int _availableRam;

        public RamProvisioner(int ram)
        {
            _ram = ram;
            AvailableRam = ram;
        }

        public abstract bool AllocateRamForVm(Vm vm, int ram);

        public abstract int GetAllocatedRamForVm(Vm vm);

        public abstract void DeallocateRamForVm(Vm vm);

        public void DeallocateRamForAllVms()
        {
            AvailableRam = Ram;
        }

        public abstract bool IsSuitableForVm(Vm vm, int ram);

        public int UsedRam
        {
            get { return _ram - _availableRam; }
        }

        public int Ram
        {
            get { return _ram; }
            protected set { _ram = value; }
        }

        public int AvailableRam
        {
            get { return _availableRam; }
            protected set { _availableRam = value; }
        }
    }
}
