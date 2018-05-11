using System.Collections.Generic;
using System.Linq;

namespace CloudSim.Sharp
{
    public abstract class VmScheduler
    {
        private List<Pe> _peList;

        private Dictionary<string, List<Pe>> _peMap;

        private Dictionary<string, List<double>> _mipsMap;

        private double _availableMips;

        private List<string> _vmsMigratingIn;

        private List<string> _vmsMigratingOut;

        public VmScheduler(List<Pe> peList)
        {
            PeList = peList;
            PeMap = new Dictionary<string, List<Pe>>();
            MipsMap = new Dictionary<string, List<double>>();
            AvailableMips = PeList.GetTotalMips();
            VmsMigratingIn = new List<string>();
            VmsMigratingOut = new List<string>();
        }

        public abstract bool AllocatePesForVm(Vm vm, List<double> mipsShare);

        public abstract void DeallocatePesForVm(Vm vm);
                
        public void DeallocatePesForAllVms()
        {
            MipsMap.Clear();
            AvailableMips = PeList.GetTotalMips();
            foreach (Pe pe in PeList)
            {
                pe.PeProvisioner.DeallocateMipsForAllVms();
            }
        }

        public List<Pe> GetPesAllocatedForVM(Vm vm)
        {
            return PeMap[vm.Uid];
        }

        public List<double> GetAllocatedMipsForVm(Vm vm)
        {
            return MipsMap[vm.Uid];
        }

        public double GetTotalAllocatedMipsForVm(Vm vm)
        {
            double allocated = 0;
            List<double> mipsMap = GetAllocatedMipsForVm(vm);

            if (mipsMap != null)
            {
                allocated = mipsMap.Sum();
            }
            return allocated;
        }

        public double GetMaxAvailableMips()
        {
            if (PeList == null || PeList.Count == 0)
            {
                Log.WriteLine("Pe list is empty");
                return 0;
            }

            double max = 0.0;

            foreach (Pe pe in PeList)
            {
                double tmp = pe.PeProvisioner.AvailableMips;
                if (tmp > max) max = tmp;
            }

            return max;
        }

        public double GetPeCapacity()
        {
            if (PeList == null || PeList.Count == 0)
            {
                Log.WriteLine("Pe list is empty");
                return 0;
            }
            return PeList[0].Mips;
        }

        public List<Pe> PeList
        {
            get { return _peList; }
            protected set { _peList = value; }
        }

        protected Dictionary<string, List<double>> MipsMap
        {
            get { return _mipsMap; }
            set { _mipsMap = value; }
        }

        public double AvailableMips
        {
            get { return _availableMips; }
            protected set { _availableMips = value; }
        }

        public List<string> VmsMigratingOut
        {
            get { return _vmsMigratingOut; }
            set { _vmsMigratingOut = value; }
        }

        public List<string> VmsMigratingIn
        {
            get { return _vmsMigratingIn; }
            protected set { _vmsMigratingIn = value; }
        }

        public Dictionary<string, List<Pe>> PeMap
        {
            get { return _peMap; }
            protected set { _peMap = value; }
        }
    }
}
