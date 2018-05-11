namespace CloudSim.Sharp
{
    public class VmStateHistoryEntry
    {
        private double _time;
        private double _allocatedMips;
        private double _requestedMips;
        private bool _isInMigration;

        public VmStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isInMigration)
        {
            Time = time;
            AllocatedMips = allocatedMips;
            RequestedMips = requestedMips;
            IsInMigration = isInMigration;
        }

        public double Time
        {
            get { return _time; }
            protected set { _time = value; }
        }

        public double AllocatedMips
        {
            get { return _allocatedMips; }
            protected set { _allocatedMips = value; }
        }

        public double RequestedMips
        {
            get { return _requestedMips; }
            protected set { _requestedMips = value; }
        }

        public bool IsInMigration
        {
            get { return _isInMigration; }
            protected set { _isInMigration = value; }
        }
    }
}
