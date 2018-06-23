using System.Collections.Generic;

namespace CloudSim.Sharp.Provisioners
{
    public abstract class PeProvisioner
    {
        private double _mips;
        private double _availableMips;

        public double Mips
        {
            get { return _mips; }
            set { _mips = value; }
        }

        public double AvailableMips
        {
            get { return _mips; }
            protected set { _mips = value; }
        }

        public abstract bool AllocateMipsForVm(Vm vm, double mips);

        public abstract bool AllocateMipsForVm(string vmUid, double mips);

        public abstract bool AllocateMipsForVm(Vm vm, List<double> mips);

        public abstract List<double> GetAllocatedMipsForVm(Vm vm);

        public abstract double GetTotalAllocatedMipsForVm(Vm vm);

        public abstract double GetAllocatedMipsForVmByVirtualPeId(Vm vm, int peId);

        public abstract void DeallocateMipsForVm(Vm vm);

        public virtual void DeallocateMipsForAllVms()
        {
            AvailableMips = Mips;
        }

        public PeProvisioner(double mips)
        {
            Mips = mips;
            AvailableMips = mips;
        }

        public double GetTotalAllocatedMips()
        {
            double totalAllocatedMips = Mips - AvailableMips;
            if (totalAllocatedMips > 0) return totalAllocatedMips;
            return 0;
        }

        public double GetUtilization()
        {
            return GetTotalAllocatedMips() / Mips;
        }
    }
}
