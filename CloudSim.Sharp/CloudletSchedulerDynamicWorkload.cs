using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    class CloudletSchedulerDynamicWorkload : CloudletSchedulerTimeShared
    {

        private double _mips;

        private int _numberOfPes;

        private double _totalMips;

        private Dictionary<String, Double> _underAllocatedMips;

        private double _cachePreviousTime;

        private List<Double> _cacheCurrentRequestedMips;

        public CloudletSchedulerDynamicWorkload(double mips, int numberOfPes) : base()
        {
            SetMips(mips);
            SetNumberOfPes(numberOfPes);

            SetTotalMips(GetNumberOfPes() * GetMips());
            SetUnderAllocatedMips(new Dictionary<String, Double>());
            SetCachePreviousTime(-1);
        }

 
        public override double UpdateVmProcessing(double currentTime, List<Double> mipsShare)
        {
            SetCurrentMipsShare(mipsShare);

            double timeSpan = currentTime - GetPreviousTime();
            double nextEvent = Double.MaxValue;
            List<ResCloudlet> cloudletsToFinish = new List<ResCloudlet>();

            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                rcl.UpdateCloudletFinishedSoFar((long)(timeSpan
                        * GetTotalCurrentAllocatedMipsForCloudlet(rcl, 
                        GetPreviousTime()) * Consts.MILLION));

                if (rcl.GetRemainingCloudletLength() == 0)
                { 
                    cloudletsToFinish.Add(rcl);
                    continue;
                }
                else
                {
                    double estimatedFinishTime = GetEstimatedFinishTime(rcl, currentTime);
                    if (estimatedFinishTime - currentTime < Core.CloudSim.GetMinTimeBetweenEvents())
                    {
                        estimatedFinishTime = currentTime + Core.CloudSim.GetMinTimeBetweenEvents();
                    }
                    if (estimatedFinishTime < nextEvent)
                    {
                        nextEvent = estimatedFinishTime;
                    }
                }
            }

            foreach (ResCloudlet rgl in cloudletsToFinish)
            {
                GetCloudletExecList().Remove(rgl);
                CloudletFinish(rgl);
            }

            SetPreviousTime(currentTime);

            if (GetCloudletExecList().Count == 0)
            {
                return 0;
            }

            return nextEvent;
        }

      
        public override double CloudletSubmit(Cloudlet cl)
        {
            return CloudletSubmit(cl, 0);
        }

     
        public override double CloudletSubmit(Cloudlet cl, double fileTransferTime)
        {
            ResCloudlet rcl = new ResCloudlet(cl);
            rcl.SetCloudletStatus(Cloudlet.INEXEC);

            for (int i = 0; i < cl.GetNumberOfPes(); i++)
            {
                rcl.SetMachineAndPeId(0, i);
            }

            GetCloudletExecList().Add(rcl);
            return GetEstimatedFinishTime(rcl, GetPreviousTime());
        }

      
        public override void CloudletFinish(ResCloudlet rcl)
        {
            rcl.SetCloudletStatus(Cloudlet.SUCCESS);
            rcl.FinalizeCloudlet();
            GetCloudletFinishedList().Add(rcl);
        }

     
        public override double GetTotalUtilizationOfCpu(double time)
        {
            double totalUtilization = 0;
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                totalUtilization += rcl.GetCloudlet().GetUtilizationOfCpu(time);
            }
            return totalUtilization;
        }

        public override List<Double> GetCurrentRequestedMips()
        {
            if (GetCachePreviousTime() == GetPreviousTime())
            {
                return GetCacheCurrentRequestedMips();
            }

            List<Double> currentMips = new List<Double>();
            double totalMips = GetTotalUtilizationOfCpu(GetPreviousTime()) 
                * GetTotalMips();
            double mipsForPe = totalMips / GetNumberOfPes();

            for (int i = 0; i < GetNumberOfPes(); i++)
            {
                currentMips.Add(mipsForPe);
            }

            SetCachePreviousTime(GetPreviousTime());
            SetCacheCurrentRequestedMips(currentMips);

            return currentMips;
        }

        public override double GetTotalCurrentRequestedMipsForCloudlet
            (ResCloudlet rcl, double time)
        {
            return rcl.GetCloudlet().GetUtilizationOfCpu(time) * GetTotalMips();
        }

      
        public override double GetTotalCurrentAvailableMipsForCloudlet
            (ResCloudlet rcl, List<Double> mipsShare)
        {
            double totalCurrentMips = 0.0;
            if (mipsShare != null)
            {
                int neededPEs = rcl.GetNumberOfPes();
                foreach (double mips in mipsShare)
                {
                    totalCurrentMips += mips;
                    neededPEs--;
                    if (neededPEs <= 0)
                    {
                        break;
                    }
                }
            }
            return totalCurrentMips;
        }

        public override double GetTotalCurrentAllocatedMipsForCloudlet
            (ResCloudlet rcl, double time)
        {
            double totalCurrentRequestedMips 
                = GetTotalCurrentRequestedMipsForCloudlet(rcl, time);
            double totalCurrentAvailableMips 
                = GetTotalCurrentAvailableMipsForCloudlet(rcl,
                GetCurrentMipsShare());

            if (totalCurrentRequestedMips > totalCurrentAvailableMips)
            {
                return totalCurrentAvailableMips;
            }

            return totalCurrentRequestedMips;
        }

        public void UpdateUnderAllocatedMipsForCloudlet
            (ResCloudlet rcl, double mips)
        {
            if (GetUnderAllocatedMips().ContainsKey(rcl.GetUid()))
            {
                mips += GetUnderAllocatedMips()[rcl.GetUid()];
            }
            GetUnderAllocatedMips().Add(rcl.GetUid(), mips);
        }

        public double GetEstimatedFinishTime(ResCloudlet rcl, double time)
        {
            return time
                    + ((rcl.GetRemainingCloudletLength()) 
                    / GetTotalCurrentAllocatedMipsForCloudlet(rcl, time));
        }

        public int GetTotalCurrentMips()
        {
            int totalCurrentMips = 0;
            foreach (double mips in GetCurrentMipsShare())
            {
                totalCurrentMips += Convert.ToInt32(mips);
            }
            return totalCurrentMips;
        }

        public void SetTotalMips(double mips)
        {
            _totalMips = mips;
        }

        public double GetTotalMips()
        {
            return _totalMips;
        }

        public void SetNumberOfPes(int pesNumber)
        {
            _numberOfPes = pesNumber;
        }

        public int GetNumberOfPes()
        {
            return _numberOfPes;
        }

        public void SetMips(double mips)
        {
            _mips = mips;
        }
        
        public double GetMips()
        {
            return _mips;
        }
        
        public void SetUnderAllocatedMips(Dictionary<String, Double> underAllocatedMips)
        {
            _underAllocatedMips = underAllocatedMips;
        }

        public Dictionary<String, Double> GetUnderAllocatedMips()
        {
            return _underAllocatedMips;
        }

        protected double GetCachePreviousTime()
        {
            return _cachePreviousTime;
        }
        
        protected void SetCachePreviousTime(double cachePreviousTime)
        {
            _cachePreviousTime = cachePreviousTime;
        }

        protected List<Double> GetCacheCurrentRequestedMips()
        {
            return _cacheCurrentRequestedMips;
        }

        protected void SetCacheCurrentRequestedMips(List<Double> cacheCurrentRequestedMips)
        {
            _cacheCurrentRequestedMips = cacheCurrentRequestedMips;
        }

    }
}
