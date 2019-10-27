using System.Collections.Generic;

namespace CloudSim.Sharp.Vm
{
    public class Vm
    {
        private int _id;
        private int _userId;
        private string _uid;
        private long _size;
        private double _mips;
        private int _numberOfPes;
        private int _ram;
        private long _bw;
        private string _vmm;
        private CloudletScheduler _cloudletScheduler;
        private Host _host;
        private bool _inMigration;
        private long _currentAllocatedSize;
        private int _currentAllocatedRam;
        private long _currentAllocatedBw;
        private List<double> _currentAllocatedMips;
        private bool _beingInstantiated;

        private readonly List<VmStateHistoryEntry> _stateHistory = new List<VmStateHistoryEntry>();

        public Vm(
            int id,
            int userId,
            double mips,
            int numberOfPes,
            int ram,
            long bw,
            long size,
            string vmm,
            CloudletScheduler cloudletScheduler)
        {
            Id = id;
            UserId = userId;
            Uid = GetUid(userId, id);
            Mips = mips;
            NumberOfPes = numberOfPes;
            Ram = ram;
            Bw = bw;
            Size = size;
            Vmm = vmm;
            CloudletScheduler = cloudletScheduler;

            InMigration = false;
            BeingInstantiated = true;

            CurrentAllocatedBw = 0;
            CurrentAllocatedMips = null;
            CurrentAllocatedRam = 0;
            CurrentAllocatedSize = 0;
        }

        public double UpdateVmProcessing(double currentTime, List<double> mipsShare)
        {
            if (mipsShare != null)
            {
                return CloudletScheduler.UpdateVmProcessing(currentTime, mipsShare);
            }
            return 0;
        }

        public List<double> GetCurrentRequestedMips()
        {
            List<double> currentRequestedMips = CloudletScheduler.GetCurrentRequestedMips();
            if (BeingInstantiated)
            {
                currentRequestedMips = new List<double>();
                for (int i = 0; i < NumberOfPes; i++)
                {
                    currentRequestedMips.Add(Mips);
                }
            }
            return currentRequestedMips;
        }

        public double GetCurrentRequestedTotalMips()
        {
            double totalRequestedMips = 0;
            foreach (double mips in GetCurrentRequestedMips())
            {
                totalRequestedMips += mips;
            }
            return totalRequestedMips;
        }

        public double GetCurrentRequestedMaxMips()
        {
            double maxMips = 0;
            foreach (double mips in GetCurrentRequestedMips())
            {
                if (mips > maxMips)
                    maxMips = mips;
            }
            return maxMips;
        }

        public long GetCurrentRequestedBw()
        {
            if (BeingInstantiated)
            {
                return Bw;
            }
            return (long)(CloudletScheduler.GetCurrentRequestedUtilizationOfBw() * Bw);
        }

        public int GetCurrentRequestedRam()
        {
            if (BeingInstantiated)
            {
                return Ram;
            }
            return (int)(CloudletScheduler.GetCurrentRequestedUtilizationOfRam() * Ram);
        }

        public double GetTotalUtilizationOfCpu(double time)
        {
            return CloudletScheduler.GetTotalUtilizationOfCpu(time);
        }

        public double GetTotalUtilizationOfCpuMips(double time)
        {
            return GetTotalUtilizationOfCpu(time) * Mips;
        }

        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public static string GetUid(int userId, int vmId)
        {
            return $"{userId}-{vmId}";
        }

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public int UserId
        {
            get { return _userId; }
            protected set { _userId = value; }
        }

        public double Mips
        {
            get { return _mips; }
            protected set { _mips = value; }
        }

        public int NumberOfPes
        {
            get { return _numberOfPes; }
            protected set { _numberOfPes = value; }
        }

        public int Ram
        {
            get { return _ram; }
            set { _ram = value; }
        }

        public long Bw
        {
            get { return _bw; }
            set { _bw = value; }
        }

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public string Vmm
        {
            get { return _vmm; }
            protected set { _vmm = value; }
        }

        public Host Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public CloudletScheduler CloudletScheduler
        {
            get { return _cloudletScheduler; }
            set { _cloudletScheduler = value; }
        }

        public bool InMigration
        {
            get { return _inMigration; }
            set { _inMigration = value; }
        }

        public long CurrentAllocatedSize
        {
            get { return _currentAllocatedSize; }
            protected set { _currentAllocatedSize = value; }
        }

        public int CurrentAllocatedRam
        {
            get { return _currentAllocatedRam; }
            set { _currentAllocatedRam = value; }
        }

        public long CurrentAllocatedBw
        {
            get { return _currentAllocatedBw; }
            set { _currentAllocatedBw = value; }
        }

        public List<double> CurrentAllocatedMips
        {
            get { return _currentAllocatedMips; }
            set { _currentAllocatedMips = value; }
        }

        public bool BeingInstantiated
        {
            get { return _beingInstantiated; }
            set { _beingInstantiated = value; }
        }

        public List<VmStateHistoryEntry> StateHistory
        {
            get { return _stateHistory; }
        }

        public void AddStateHistoryEntry(
            double time,
            double allocatedMips,
            double requestedMips,
            bool isInMigration)
        {
            VmStateHistoryEntry newState = new VmStateHistoryEntry(
                time,
                allocatedMips,
                requestedMips,
                isInMigration);

            if (StateHistory.Count != 0)
            {
                VmStateHistoryEntry previousState = StateHistory[StateHistory.Count - 1];
                if (previousState.Time == time)
                {
                    StateHistory[StateHistory.Count - 1] = newState;
                    return;
                }
            }
            StateHistory.Add(newState);
        }
    }
}
