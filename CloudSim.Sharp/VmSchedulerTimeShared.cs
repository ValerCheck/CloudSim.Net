using CloudSim.Sharp.Provisioners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class VmSchedulerTimeShared : VmScheduler
    {

        public Dictionary<String, List<Double>> MipsMapRequested { get; set; }

        private int _pesInUse;

        public VmSchedulerTimeShared(List<Pe> pelist) : base(pelist)
        {
            MipsMapRequested = new Dictionary<String, List<Double>>();
        }

        public override bool AllocatePesForVm(Vm vm, List<Double> mipsShareRequested)
        {
            if (vm.InMigration)
            {
                if (!VmsMigratingIn.Contains(vm.Uid)
                    && !VmsMigratingOut.Contains(vm.Uid))
                {
                    VmsMigratingOut.Add(vm.Uid);
                }
            }
            else
            {
                if (VmsMigratingOut.Contains(vm.Uid))
                {
                    VmsMigratingOut.Remove(vm.Uid);
                }
            }
            bool result = AllocatePesForVm(vm.Uid, mipsShareRequested);
            UpdatePeProvisioning();
            return result;
        }


        protected virtual bool AllocatePesForVm(String vmUid, List<Double> mipsShareRequested)
        {
            double totalRequestedMips = 0;
            double peMips = PeCapacity;
            foreach (Double mips in mipsShareRequested)
            {

                if (mips > peMips)
                {
                    return false;
                }
                totalRequestedMips += mips;
            }

            if (AvailableMips < totalRequestedMips)
            {
                return false;
            }

            MipsMapRequested.Add(vmUid, mipsShareRequested);

            PesInUse = PesInUse + mipsShareRequested.Count;

            if (VmsMigratingIn.Contains(vmUid))
            {
                totalRequestedMips *= 0.1;
            }

            List<double> mipsShareAllocated = new List<double>();
            foreach (double mipsRequested in mipsShareRequested)
            {
                double newMipsReq = mipsRequested;
                if (VmsMigratingOut.Contains(vmUid))
                {
                    newMipsReq *= 0.9;
                }
                else if (VmsMigratingIn.Contains(vmUid))
                {
                    newMipsReq *= 0.1;
                }
                mipsShareAllocated.Add(newMipsReq);
            }

            MipsMap.Add(vmUid, mipsShareAllocated);
            AvailableMips = AvailableMips - totalRequestedMips;

            return true;
        }

        protected void UpdatePeProvisioning()
        {
            // TODO: implement method
            PeMap.Clear();

            foreach (Pe p in PeList)
            {
                p.PeProvisioner.DeallocateMipsForAllVms();
            }
            int index = 0;
            int max = PeList.Count;
            
            PeProvisioner peProvisioner = PeList[index].PeProvisioner;

            double availableMips = peProvisioner.AvailableMips;

            foreach (var entry in MipsMap)
            {
                String vmUid = entry.Key;
                PeMap.Add(vmUid, new List<Pe>());

                foreach (double mips in entry.Value)
                {
                    Double newMips = mips;
                    while (newMips >= 0.1)
                    {
                        
                        if (availableMips >= mips)
                        {
                            peProvisioner.AllocateMipsForVm(vmUid, mips);
                            PeMap[vmUid].Add(PeList[index]);
                            availableMips -= mips;
                            break;
                        }
                        else
                        {
                            peProvisioner.AllocateMipsForVm(vmUid, availableMips);
                            PeMap[vmUid].Add(PeList[index]);
                            newMips -= availableMips;

                            if (mips <= 0.1)
                            {
                                break;
                            }

                            if (index == max - 1)
                            {
                                Log.WriteConcatLine("There is no enough MIPS (",
                                    mips, ") to accommodate VM ", vmUid);
                            }

                            index++;

                            peProvisioner = PeList[index].PeProvisioner; 
                            availableMips = peProvisioner.AvailableMips;
                        }
                    }
                }
            }
        }

        public override void DeallocatePesForVm(Vm vm)
        {
            MipsMapRequested.Remove(vm.Uid);
            PesInUse = 0;
            MipsMap.Clear();

            AvailableMips = PeList.Select(x => x.Mips).Sum();

            foreach (Pe pe in PeList)
            {
                pe.PeProvisioner.DeallocateMipsForVm(vm);
            }

            foreach (var entry in MipsMapRequested)
            {
                AllocatePesForVm(entry.Key, entry.Value);
            }

            UpdatePeProvisioning();
        }

        public new void DeallocatePesForAllVms()
        {
            base.DeallocatePesForAllVms();
            MipsMapRequested.Clear();
            PesInUse = 0;
        }

        public new double GetMaxAvailableMips()
        {
            return AvailableMips;
        }

        protected int PesInUse { get; set; }

    }
}
