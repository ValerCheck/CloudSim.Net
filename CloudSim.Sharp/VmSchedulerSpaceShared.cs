using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class VmSchedulerSpaceShared : VmScheduler
    {

        public Dictionary<String, List<Pe>> PeAllocationMap
        { get; set; }
        
        public List<Pe> FreePes { get; set; }
        
        public VmSchedulerSpaceShared(List<Pe> pelist) : base(pelist)
        {
            PeAllocationMap = new Dictionary<String, List<Pe>>();
            FreePes = new List<Pe>();
            FreePes.AddRange(pelist);
        }

        
        public override bool AllocatePesForVm(Vm vm, List<Double> mipsShare)
        {
            
            if (FreePes.Count < mipsShare.Count)
                return false;

            List<Pe> selectedPes = new List<Pe>();
            double totalMips = 0;

            for (int i = 0; i < mipsShare.Count; i++)
            {
                if (mipsShare[i] <= FreePes[i].Mips)
                {
                    selectedPes.Add(FreePes[i]);
                    totalMips += mipsShare[i];
                }
            }

            if (mipsShare.Count > selectedPes.Count)
                return false;

            selectedPes.ForEach(x => FreePes.Remove(x));

            PeAllocationMap.Add(vm.Uid, selectedPes);
            MipsMap.Add(vm.Uid, mipsShare);
            AvailableMips = AvailableMips - totalMips;
            return true;
        }

        public override void DeallocatePesForVm(Vm vm)
        {
            PeAllocationMap
                .Where(x => x.Key == vm.Uid)
                .Select(y => y.Value).ToList()
                .ForEach(x => FreePes.AddRange(x));

            PeAllocationMap.Remove(vm.Uid);

            double totalMips = 0;

            foreach (double mips in MipsMap[vm.Uid])
            {
                totalMips += mips;
            }

            AvailableMips = AvailableMips + totalMips;

            MipsMap.Remove(vm.Uid);
        }

    }
}


