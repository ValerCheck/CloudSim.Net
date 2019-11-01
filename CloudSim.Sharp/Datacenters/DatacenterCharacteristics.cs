using CloudSim.Sharp.Lists;
using System;
using System.Collections.Generic;

namespace CloudSim.Sharp
{
    public class DatacenterCharacteristics
    {
        public const int TIME_SHARED = 0;
        public const int SPACE_SHARED = 1;
        public const int OTHER_POLICY_SAME_RATING = 2;
        public const int OTHER_POLICY_DIFFERENT_RATING = 3;
        public const int ADVANCE_RESERVATION = 4;

        private int _id;
        private string _architecture;
        private string _os;
        private List<Host> _hostList;
        private double _timeZone;
        private double _costPerSecond;
        private int _allocationPolicy;
               
        private string _vmm;
        private double _costPerMem;
        private double _costPerStorage;
        private double _costPerBw;

        public DatacenterCharacteristics(
            string architecture,
            string os,
            string vmm,
            List<Host> hostList,
            double timeZone,
            double costPerSec,
            double costPerMem,
            double costPerStorage,
            double costPerBw)
        {
            Id = -1;
            Architecture = architecture;
            Os = os;
            HostList = hostList;
            CostPerSecond = costPerSec;
            TimeZone = 0.0;
            Vmm = vmm;
            CostPerMem = costPerMem;
            CostPerStorage = costPerStorage;
            CostPerBw = costPerBw;
        }

        public string GetResourceName()
        {
            return Core.CloudSim.GetEntityName(Id);
        }

        public Host GetHostWithFreePe()
        {
            return HostList.GetHostWithFreePe();
        }

        public Host GetHostWIthFreePe(int peNumber)
        {
            return HostList.GetHostWithFreePe(peNumber);
        }

        public int GetMipsOfOnePe()
        {
            if (HostList.Count == 0) return -1;

            return PeList.GetMips(HostList[0].PeList, 0);
        }

        public int GetMipsOfOnePe(int id, int peId)
        {
            if (HostList.Count == 0) return -1;
            return PeList.GetMips(HostList.GetById(id).PeList, peId);
        }

        public int GetMips()
        {
            int mips = 0;
            switch (AllocationPolicy)
            {
                case TIME_SHARED:
                case OTHER_POLICY_SAME_RATING:
                    mips = GetMipsOfOnePe() * HostList.GetNumberOfPes();
                    break;

                case SPACE_SHARED:
                case OTHER_POLICY_DIFFERENT_RATING:
                    foreach (Host host in HostList)
                    {
                        mips += host.GetTotalMips();
                    }
                    break;

                default:
                    break;
            }
            return mips;
        }

        public double GetCpuTime(double cloudletLength, double load)
        {
            double cpuTime = 0.0;

            switch (AllocationPolicy)
            {
                case TIME_SHARED:
                    cpuTime = cloudletLength / (GetMipsOfOnePe() * (1.0 - load));
                    break;

                default:
                    break;
            }

            return cpuTime;
        }

        public int GetNumberOfPes()
        {
            return HostList.GetNumberOfPes();
        }

        public int GetNumberOfFreePes()
        {
            return HostList.GetNumberOfFreePes();
        }

        public int GetNumberOfBusyPes()
        {
            return HostList.GetNumberOfBusyPes();
        }

        public bool SetPeStatus(int status, int hostId, int peId)
        {
            return HostList.SetPeStatus(status, hostId, peId);
        }

        public double GetCostPerMi()
        {
            return CostPerSecond / GetMipsOfOnePe();
        }

        public int GetNumberOfHosts()
        {
            return HostList.Count;
        }

        public int GetNumberOfFailedHosts()
        {
            int numberOfFailedHosts = 0;
            foreach (Host host in HostList)
            {
                if (host.IsFailed)
                {
                    numberOfFailedHosts++;
                }
            }
            return numberOfFailedHosts;
        }

        public bool IsWorkging()
        {
            bool result = false;
            if (GetNumberOfFailedHosts() == 0)
            {
                result = true;
            }

            return result;
        }

        public double CostPerMem
        {
            get { return _costPerMem; }
            set { _costPerMem = value; }
        }

        public double CostPerStorage
        {
            get { return _costPerStorage; }
            set { _costPerStorage = value; }
        }

        public double CostPerBw
        {
            get { return _costPerBw; }
            set { _costPerBw = value; }
        }

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        protected string Architecture
        {
            get { return _architecture; }
            set { _architecture = value; }
        }

        protected string Os
        {
            get { return _os; }
            set { _os = value; }
        }

        public List<Host> HostList
        {
            get { return _hostList; }
            protected set { _hostList = value; }
        }

        protected double TimeZone
        {
            get { return _timeZone; }
            set { _timeZone = value; }
        }

        public double CostPerSecond
        {
            get { return _costPerSecond; }
            protected set { _costPerSecond = value; }
        }

        protected int AllocationPolicy
        {
            get { return _allocationPolicy; }
            set { _allocationPolicy = value; }
        }

        public string Vmm
        {
            get { return _vmm; }
            protected set { _vmm = value; }
        }
    }
}
