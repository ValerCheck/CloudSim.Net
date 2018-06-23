using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSim.Sharp.Provisioners
{
    public class PeProvisionerSimple : PeProvisioner
    {
        private Dictionary<string, List<double>> _peTable;

        public PeProvisionerSimple(double availableMips) : base(availableMips)
        {
            PeTable = new Dictionary<string, List<double>>();
        }

        public override bool AllocateMipsForVm(Vm vm, double mips)
        {
            return AllocateMipsForVm(vm.Uid, mips);
        }

        public override bool AllocateMipsForVm(string vmUid, double mips)
        {
            if (AvailableMips < mips) return false;

            var allocatedMips = PeTable.ContainsKey(vmUid) ?
                PeTable[vmUid] :
                new List<double>();

            allocatedMips.Add(mips);

            AvailableMips -= mips;
            PeTable[vmUid] = allocatedMips;

            return true;
        }

        public override bool AllocateMipsForVm(Vm vm, List<double> mips)
        {
            double totalMipsToAllocate = mips.Sum();

            if (AvailableMips + GetTotalAllocatedMipsForVm(vm) < totalMipsToAllocate)
            {
                return false;
            }

            AvailableMips = AvailableMips + GetTotalAllocatedMipsForVm(vm) - totalMipsToAllocate;

            PeTable[vm.Uid] = mips;

            return true;
        }

        public override void DeallocateMipsForAllVms()
        {
            base.DeallocateMipsForAllVms();
            PeTable.Clear();
        }

        public override double GetAllocatedMipsForVmByVirtualPeId(Vm vm, int peId)
        {
            if (PeTable.ContainsKey(vm.Uid))
            {
                try
                {
                    return PeTable[vm.Uid][peId];
                }
                catch (Exception e)
                {

                }
            }
            return 0;
        }

        public override List<double> GetAllocatedMipsForVm(Vm vm)
        {
            if (!PeTable.ContainsKey(vm.Uid)) return null;
            return PeTable[vm.Uid];    
        }

        public override double GetTotalAllocatedMipsForVm(Vm vm)
        {
            if (!PeTable.ContainsKey(vm.Uid)) return 0;
            return PeTable[vm.Uid].Sum();
        }

        public override void DeallocateMipsForVm(Vm vm)
        {
            if (PeTable.ContainsKey(vm.Uid))
            {
                foreach (double mips in PeTable[vm.Uid])
                {
                    AvailableMips += mips;
                }
                PeTable.Remove(vm.Uid);
            }
        }
        
        protected Dictionary<string, List<double>> PeTable
        {
            get { return _peTable; }
            set { _peTable = value; }
        }
    }
}
