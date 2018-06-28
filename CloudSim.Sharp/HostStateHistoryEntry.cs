namespace CloudSim.Sharp
{
    public class HostStateHistoryEntry
    {
        private double _time;
        private double _allocatedMips;
        private double _requestedMips;
        private bool _isActive;

        public HostStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isActive)
        {
            Time = time;
            AllocatedMips = allocatedMips;
            RequestedMips = requestedMips;
            IsActive = isActive;
        }

        public double Time
        {
            get { return _time; }
            protected set { _time = value; }
        }

        public double AllocatedMips
        {
            get { return _allocatedMips;}
            protected set { _allocatedMips = value; }
        }

        public double RequestedMips
        {
            get { return _requestedMips;}
            protected set { _requestedMips = value; }
        }

        public bool IsActive
        {
            get { return _isActive;}
            set { _isActive = value; }
        }

    }
}
