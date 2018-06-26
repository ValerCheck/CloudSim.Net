using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class VmSchedulerTimeSharedOverSubscription : VmSchedulerTimeShared
    {

        public VmSchedulerTimeSharedOverSubscription(List<Pe> pelist)
            : base(pelist) { }

        protected override bool AllocatePesForVm
                (String vmUid, List<Double> mipsShareRequested)
        {
            double totalRequestedMips = 0;

            List<Double> mipsShareRequestedCapped = new List<Double>();
            double peMips = PeCapacity;
            foreach (Double mips in mipsShareRequested)
            {
                if (mips > peMips)
                {
                    mipsShareRequestedCapped.Add(peMips);
                    totalRequestedMips += peMips;
                }
                else
                {
                    mipsShareRequestedCapped.Add(mips);
                    totalRequestedMips += mips;
                }
            }

            MipsMapRequested.Add(vmUid, mipsShareRequested);
            PesInUse += mipsShareRequested.Capacity;

            if (VmsMigratingIn.Contains(vmUid))
            {
                totalRequestedMips *= 0.1;
            }

            if (AvailableMips >= totalRequestedMips)
            {
                List<Double> mipsShareAllocated = new List<Double>();
                for (int i = 0; i < mipsShareRequestedCapped.Count; i++)
                {
                    if (VmsMigratingOut.Contains(vmUid))
                    {
                        mipsShareRequestedCapped[i] *= 0.9;
                    }
                    else if (VmsMigratingIn.Contains(vmUid))
                    {
                        mipsShareRequestedCapped[i] *= 0.1;
                    }
                    mipsShareAllocated.Add(mipsShareRequestedCapped[i]);
                }

                MipsMap.Add(vmUid, mipsShareAllocated);
                AvailableMips -= totalRequestedMips;
            }
            else
            {
                RedistributeMipsDueToOverSubscription();
            }

            return true;
        }

        protected void RedistributeMipsDueToOverSubscription()
        {
            double totalRequiredMipsByAllVms = 0;

            Dictionary<String, List<Double>> mipsMapCapped 
                = new Dictionary<String, List<Double>>();
            foreach (var entry in MipsMapRequested)
            {

                double requiredMipsByThisVm = 0.0;
                String vmId = entry.Key;
                List<Double> mipsShareRequested = entry.Value;
                List<Double> mipsShareRequestedCapped = new List<Double>();
                double peMips = PeCapacity;
                foreach (Double mips in mipsShareRequested)
                {
                    if (mips > peMips)
                    {
                        mipsShareRequestedCapped.Add(peMips);
                        requiredMipsByThisVm += peMips;
                    }
                    else
                    {
                        mipsShareRequestedCapped.Add(mips);
                        requiredMipsByThisVm += mips;
                    }
                }

                mipsMapCapped.Add(vmId, mipsShareRequestedCapped);

                if (VmsMigratingIn.Contains(entry.Key))
                {
                    requiredMipsByThisVm *= 0.1;
                }
                totalRequiredMipsByAllVms += requiredMipsByThisVm;
            }

            double totalAvailableMips = PeList.Select(x => x.Mips).Sum();
            double scalingFactor = totalAvailableMips / totalRequiredMipsByAllVms;
            
            MipsMap.Clear();
            
            foreach (var entry in mipsMapCapped)
            {
                String vmUid = entry.Key;
                List<Double> requestedMips = entry.Value;

                List<Double> updatedMipsAllocation = new List<Double>();
                for (int i = 0; i < requestedMips.Count; i++)
                {
                    if (VmsMigratingOut.Contains(vmUid))
                    {
                        requestedMips[i] *= scalingFactor;
                        requestedMips[i] *= 0.9;
                    }
                    else if (VmsMigratingIn.Contains(vmUid))
                    {
                        requestedMips[i] *= 0.1;
                        requestedMips[i] *= scalingFactor;
                    }
                    else
                    {
                        requestedMips[i] *= scalingFactor;
                    }

                    updatedMipsAllocation.Add(Math.Floor(requestedMips[i]));
                }
                
                MipsMap.Add(vmUid, updatedMipsAllocation);

            }

            AvailableMips = 0;
        }

    }
}
