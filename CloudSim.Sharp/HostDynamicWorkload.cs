using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSim.Sharp.Lists;
using CloudSim.Sharp.Provisioners;

namespace CloudSim.Sharp
{
    public class HostDynamicWorkload : Host
    {
        private double _utilizationMips;
        private double _previousUtilizationMips;
        private readonly List<HostStateHistoryEntry> _stateHistory = new List<HostStateHistoryEntry>();
        private const double Delta = 10E-5;

        public HostDynamicWorkload(
            int id,
            RamProvisioner ramProvisioner,
            BwProvisioner bwProvisioner,
            long storage,
            List<Pe> peList,
            VmScheduler vmScheduler) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
        {
            UtilizationMips = 0;
            PreviousUtilizationMips = 0;
        }

        public double UpdateVmsProcessing(double currentTime)
        {
            double smallerTime = base.UpdateVmsProcessing(currentTime);
            PreviousUtilizationMips = UtilizationMips;
            UtilizationMips = 0;
            double hostTotalRequestedMips = 0;

            foreach (Vm vm in VmList)
            {
                VmScheduler.DeallocatePesForVm(vm);
            }

            foreach (Vm vm in VmList)
            {
                VmScheduler.AllocatePesForVm(vm, vm.GetCurrentRequestedMips());
            }

            foreach (Vm vm in VmList)
            {
                double totalRequestedMips = vm.GetCurrentRequestedTotalMips();
                double totalAllocatedMips = VmScheduler.GetTotalAllocatedMipsForVm(vm);

                if (!Log.IsDisabled)
                {
                    Log.FormatLine(
                        $"{0:0.##}: [Host #{Id}] Total allocated MIPS for VM #{vm.Id} (Host #{vm.Host.Id}) " +
                        $"is {1:0.##}, was requested {2:0.##} out of total {3:0.##} ({4:0.##}%)",
                            Core.CloudSim.Clock,
                            totalAllocatedMips,
                            totalRequestedMips,
                            vm.Mips,
                            totalRequestedMips / vm.Mips * 100);

                    List<Pe> pes = VmScheduler.GetPesAllocatedForVM(vm);
                    StringBuilder pesString = new StringBuilder();
                    foreach (Pe pe in pes)
                    {
                        pesString.Append(string.Format($" PE #{pe.Id}: {0:0.##}", pe.PeProvisioner
                                .GetTotalAllocatedMipsForVm(vm)));
                    }
                    Log.FormatLine(
                            $"{0:0.##}: [Host #{Id}] MIPS for VM #{vm.Id} by PEs (" +
                            $"{GetNumberOfPes()} * {VmScheduler.PeCapacity}).{pesString}",
                            Core.CloudSim.Clock);
                }

                if (VmsMigratingIn.Contains(vm))
                {
                    Log.FormatLine($"{0:0.##}: [Host #{Id}] VM #{vm.Id}" +
                                   $" is being migrated to Host #{Id}", Core.CloudSim.Clock);
                }
                else
                {
                    if (totalAllocatedMips + 0.1 < totalRequestedMips)
                    {
                        Log.FormatLine($"{0:0.##}: [Host #{Id}] Under allocated MIPS for VM #{vm.Id}: " +
                                       $"{1:0.##}", Core.CloudSim.Clock, totalRequestedMips - totalAllocatedMips);
                    }

                    vm.AddStateHistoryEntry(
                            currentTime,
                            totalAllocatedMips,
                            totalRequestedMips,
                            (vm.InMigration && !VmsMigratingIn.Contains(vm)));

                    if (vm.InMigration)
                    {
                        Log.FormatLine(
                                $"{0:0.##}: [Host #{Id}] VM #{vm.Id} is in migration",
                                Core.CloudSim.Clock);
                        totalAllocatedMips /= 0.9; // performance degradation due to migration - 10%
                    }
                }

                UtilizationMips = UtilizationMips + totalAllocatedMips;
                hostTotalRequestedMips += totalRequestedMips;
            }

            AddStateHistoryEntry(
                    currentTime,
                    UtilizationMips,
                    hostTotalRequestedMips,
                    (UtilizationMips > 0));

            return smallerTime;
        }

        public List<Vm> GetCompletedVms()
        {
            List<Vm> vmsToRemove = new List<Vm>();
            foreach (Vm vm in VmList)
            {
                if (vm.InMigration)
                {
                    continue;
                }
                if (Math.Abs(vm.GetCurrentRequestedTotalMips()) < Delta)
                {
                    vmsToRemove.Add(vm);
                }
            }
            return vmsToRemove;
        }

        public double GetMaxUtilization()
        {
            return PeList.GetMaxUtilization();
        }
        
        public double GetMaxUtilizationAmongVmsPes(Vm vm)
        {
            return PeList.GetMaxUtilizationAmongVmsPes(vm);
        }
        
        public double GetUtilizationOfRam()
        {
            return RamProvisioner.UsedRam;
        }
        
        public double GetUtilizationOfBw()
        {
            return BwProvisioner.UsedBw;
        }

        public double GetUtilizationOfCpu()
        {
            double utilization = UtilizationMips/GetTotalMips();
            if (utilization > 1 && utilization < 1.01)
            {
                utilization = 1;
            }
            return utilization;
        }

        public double GetPreviousUtilizationOfCpu()
        {
            double utilization = PreviousUtilizationMips/GetTotalMips();
            if (utilization > 1 && utilization < 1.01)
            {
                utilization = 1;
            }
            return utilization;
        }

        public double GetUtilizationOfCpuMips()
        {
            return UtilizationMips;
        }

        public double UtilizationMips
        {
            get { return _utilizationMips; }
            protected set { _utilizationMips = value; }
        }

        public double PreviousUtilizationMips
        {
            get { return _previousUtilizationMips;}
            protected set { _previousUtilizationMips = value; }
        }

        public List<HostStateHistoryEntry> StateHistory => _stateHistory;

        public void AddStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isActive)
        {
            HostStateHistoryEntry newState = new HostStateHistoryEntry(
                time, 
                allocatedMips, 
                requestedMips, 
                isActive);

            if (StateHistory.Count != 0)
            {
                var previousState = StateHistory.Last();
                if (Math.Abs(previousState.Time - time) < Delta)
                {
                    StateHistory[StateHistory.Count - 1] = newState;
                    return;
                }
            }
            StateHistory.Add(newState);
        }
    }
}
