using CloudSim.Sharp.Lists;
using CloudSim.Sharp.Provisioners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class Host
    {
        private int _id;
        private long _storage;
        private RamProvisioner _ramProvisioner;
        private BwProvisioner _bwProvisioner;
        private VmScheduler _vmScheduler;
        private readonly List<Vm> _vmList = new List<Vm>();
        private List<Pe> _peList;
        private bool _failed;
        private readonly List<Vm> _vmsMigratingIn = new List<Vm>();
        private Datacenter _datacenter;

        public Host(
            int id,
            RamProvisioner ramProvisioner,
            BwProvisioner bwProvisioner,
            long storage,
            List<Pe> peList,
            VmScheduler vmScheduler)
        {

        }

        public double UpdateVmsProcessing(double currentTime)
        {
            double smallerTime = double.MaxValue;

            foreach (Vm vm in VmList)
            {
                double time = vm.UpdateVmProcessing(
                    currentTime, VmScheduler.GetAllocatedMipsForVm(vm));

                if (time > 0 && time < smallerTime)
                {
                    smallerTime = time;
                }
            }
            return smallerTime;
        }

        public void AddMigratingInVm(Vm vm)
        {
            vm.InMigration = true;

            if (!VmsMigratingIn.Contains(vm))
            {
                if (Storage < vm.Size)
                {
                    Log.WriteConcatLine("[VmScheduler.addMigratingInVm] Allocation of VM #", vm.Id, " to Host #",
                            Id, " failed by storage");
                    Environment.Exit(0);
                }

                if (!RamProvisioner.AllocateRamForVm(vm, vm.GetCurrentRequestedRam()))
                {
                    Log.WriteConcatLine("[VmScheduler.addMigratingInVm] Allocation of VM #", vm.Id, " to Host #",
                            Id, " failed by RAM");
                    Environment.Exit(0);
                }

                if (!BwProvisioner.AllocateBwForVm(vm, vm.GetCurrentRequestedBw()))
                {
                    Log.WriteLine("[VmScheduler.addMigratingInVm] Allocation of VM #" + vm.Id + " to Host #"
                            + Id + " failed by BW");
                    Environment.Exit(0);
                }

                VmScheduler.VmsMigratingIn.Add(vm.Uid);
                if (!VmScheduler.AllocatePesForVm(vm, vm.GetCurrentRequestedMips()))
                {
                    Log.WriteLine("[VmScheduler.addMigratingInVm] Allocation of VM #" + vm.Id + " to Host #"
                            + Id + " failed by MIPS");
                    Environment.Exit(0);
                }

                Storage = Storage - vm.Size;

                VmsMigratingIn.Add(vm);
                VmList.Add(vm);
                UpdateVmsProcessing(Core.CloudSim.Clock);
                vm.Host.UpdateVmsProcessing(Core.CloudSim.Clock);
            }
        }

        public void RemoveMigratingInVm(Vm vm)
        {
            VmDeallocate(vm);
            VmsMigratingIn.Remove(vm);
            VmList.Remove(vm);
            VmScheduler.VmsMigratingIn.Remove(vm.Uid);
            vm.InMigration = false;
        }

        public void ReallocateMigratingInVms()
        {
            foreach (Vm vm in VmsMigratingIn)
            {
                if (!VmList.Contains(vm))
                {
                    VmList.Add(vm);
                }
                if (!VmScheduler.VmsMigratingIn.Contains(vm.Uid))
                {
                    VmScheduler.VmsMigratingIn.Add(vm.Uid);
                }
                RamProvisioner.AllocateRamForVm(vm, vm.GetCurrentRequestedRam());
                BwProvisioner.AllocateBwForVm(vm, vm.GetCurrentRequestedBw());
                VmScheduler.AllocatePesForVm(vm, vm.GetCurrentRequestedMips());
                Storage = Storage - vm.Size;
            }
        }

        public bool IsSuitableForVm(Vm vm)
        {
            return (VmScheduler.PeCapacity >= vm.GetCurrentRequestedMaxMips()
                && VmScheduler.AvailableMips >= vm.GetCurrentRequestedTotalMips()
                && RamProvisioner.IsSuitableForVm(vm, vm.GetCurrentRequestedRam())
                && BwProvisioner.IsSuitableForVm(vm, vm.GetCurrentRequestedBw()));
        }

        public bool VmCreate(Vm vm)
        {
            if (Storage < vm.Size)
            {
                Log.WriteConcatLine(
                    "[VmScheduler.vmCreate] Allocation of VM #", vm.Id, 
                    " to Host #", Id, " failed by storage");
                return false;
            }

            if (!RamProvisioner.AllocateRamForVm(vm, vm.GetCurrentRequestedRam()))
            {
                Log.WriteConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id,
                    " failed by RAM");
                return false;
            }

            if (!BwProvisioner.AllocateBwForVm(vm, vm.GetCurrentRequestedBw()))
            {
                Log.WriteConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id,
                    " failed by BW");
                RamProvisioner.DeallocateRamForVm(vm);
                return false;
            }

            if (!VmScheduler.AllocatePesForVm(vm, vm.GetCurrentRequestedMips()))
            {
                Log.WriteConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id,
                        " failed by MIPS");
                RamProvisioner.DeallocateRamForVm(vm);
                BwProvisioner.DeallocateBwForVm(vm);
                return false;
            }

            Storage = Storage - vm.Size;
            VmList.Add(vm);
            vm.Host = this;
            return true;
        }

        public void VmDestroy(Vm vm)
        {
            if (vm != null)
            {
                VmDeallocate(vm);
                VmList.Remove(vm);
                vm.Host = null;
            }
        }

        public void VmDestroyAll()
        {
            VmDeallocateAll();
            foreach(Vm vm in VmList)
            {
                vm.Host = null;
                Storage += vm.Size;
            }
            VmList.Clear();
        }

        protected void VmDeallocate(Vm vm)
        {
            RamProvisioner.DeallocateRamForVm(vm);
            BwProvisioner.DeallocateBwForVm(vm);
            VmScheduler.DeallocatePesForVm(vm);
            Storage += vm.Size;
        }

        protected void VmDeallocateAll()
        {
            RamProvisioner.DeallocateRamForAllVms();
            BwProvisioner.DeallocateBwForAllVms();
            VmScheduler.DeallocatePesForAllVms();
        }

        public Vm GetVm(int vmId, int userId)
        {
            foreach (Vm vm in VmList)
            {
                if (vm.Id == vmId && vm.UserId == userId)
                {
                    return vm;
                }
            }
            return null;
        }

        public int GetNumberOfPes()
        {
            return PeList.Count;
        }

        public int GetNumberOfFreePes()
        {
            return PeList.GetNumberOfFreePes();
        }

        public int GetTotalMips()
        {
            return PeList.GetTotalMips();
        }

        public bool AllocatePesForVm(Vm vm, List<double> mipsShare)
        {
            return VmScheduler.AllocatePesForVm(vm, mipsShare);
        }

        public void DeallocatePesForVm(Vm vm)
        {
            VmScheduler.DeallocatePesForVm(vm);
        }

        public List<double> GetAllocatedMipsForVm(Vm vm)
        {
            return VmScheduler.GetAllocatedMipsForVm(vm);
        }

        public double GetTotalAllocatedMipsForVm(Vm vm)
        {
            return VmScheduler.GetTotalAllocatedMipsForVm(vm);
        }

        public double GetMaxAvailableMips()
        {
            return VmScheduler.GetMaxAvailableMips();
        }

        public double AvailableMips
        {
            get { return VmScheduler.AvailableMips; }
        }

        public long Bw
        {
            get { return BwProvisioner.Bw; }
        }

        public int Ram
        {
            get { return RamProvisioner.Ram; }
        }

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public RamProvisioner RamProvisioner
        {
            get { return _ramProvisioner; }
            protected set { _ramProvisioner = value; }
        }

        public BwProvisioner BwProvisioner
        {
            get { return _bwProvisioner; }
            protected set { _bwProvisioner = value; }
        }

        public VmScheduler VmScheduler
        {
            get { return _vmScheduler; }
            protected set { _vmScheduler = value; }
        }

        public List<Pe> PeList
        {
            get { return _peList; }
            protected set { _peList = value; }
        }

        public List<Vm> VmList
        {
            get { return _vmList; }
        }        

        public long Storage
        {
            get { return _storage; }
            protected set { _storage = value; }
        }

        public bool IsFailed
        {
            get { return _failed; }
        }

        public bool SetFailed(bool failed)
        {
            _failed = failed;
            PeList.SetStatusFailed(failed);
            return true;
        }

        public bool SetPeStatus(int peId, int status)
        {
            return PeList.SetPeStatus(peId, status);
        }

        public List<Vm> VmsMigratingIn
        {
            get { return _vmsMigratingIn; }
        }

        public Datacenter Datacenter
        {
            get { return _datacenter; }
            set { _datacenter = value; }
        }
    }
}
