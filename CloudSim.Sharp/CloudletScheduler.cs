using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{

    public abstract class CloudletScheduler
    {
        private double _previousTime;
        
        private List<Double> _currentMipsShare;
        
        protected List<ResCloudlet> cloudletWaitingList;

        protected List<ResCloudlet> cloudletExecList;
        
        protected List<ResCloudlet> cloudletPausedList;

        protected List<ResCloudlet> cloudletFinishedList;

        protected List<ResCloudlet> cloudletFailedList;

        public CloudletScheduler()
        {
            SetPreviousTime(0.0);
            cloudletWaitingList = new List<ResCloudlet>();
            cloudletExecList = new List<ResCloudlet>();
            cloudletPausedList = new List<ResCloudlet>();
            cloudletFinishedList = new List<ResCloudlet>();
            cloudletFailedList = new List<ResCloudlet>();
        }

        public abstract double UpdateVmProcessing(double currentTime, List<Double> mipsShare);

        public abstract double CloudletSubmit(Cloudlet gl, double fileTransferTime);

        public abstract double CloudletSubmit(Cloudlet gl);
        
        public abstract Cloudlet CloudletCancel(int clId);
        
        public abstract bool CloudletPause(int clId);
        
        public abstract double CloudletResume(int clId);

        public abstract void CloudletFinish(ResCloudlet rcl);

        public abstract int GetCloudletStatus(int clId);

        public abstract bool IsFinishedCloudlets();

        public abstract Cloudlet GetNextFinishedCloudlet();
        
        public abstract int RunningCloudlets();
        
        public abstract Cloudlet MigrateCloudlet();

        public abstract double GetTotalUtilizationOfCpu(double time);
        
        public abstract List<Double> GetCurrentRequestedMips();
        
        public abstract double GetTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, List<Double> mipsShare);
        
        public abstract double GetTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time);
        
        public abstract double GetTotalCurrentAllocatedMipsForCloudlet(ResCloudlet rcl, double time);
        
        public abstract double GetCurrentRequestedUtilizationOfRam();

        public abstract double GetCurrentRequestedUtilizationOfBw();
        
        public double GetPreviousTime()
        {
            return _previousTime;
        }
        
        protected void SetPreviousTime(double previousTime)
        {
            _previousTime = previousTime;
        }

        protected void SetCurrentMipsShare(List<Double> currentMipsShare)
        {
            _currentMipsShare = currentMipsShare;
        }
        
        public List<Double> GetCurrentMipsShare()
        {
            return _currentMipsShare;
        }

        public List<ResCloudlet> GetCloudletWaitingList()
        {
            return cloudletWaitingList;
        }

        protected void SetCloudletWaitingList(List<ResCloudlet> cloudletWaitingList)
        {
            this.cloudletWaitingList = cloudletWaitingList;
        }

        public List<ResCloudlet> GetCloudletExecList()
        {
            return cloudletExecList;
        }
        
        protected void SetCloudletExecList(List<ResCloudlet> cloudletExecList)
        {
            this.cloudletExecList = cloudletExecList;
        }


        public List<ResCloudlet> GetCloudletPausedList()
        {
            return cloudletPausedList;
        }
        
        protected void SetCloudletPausedList(List<ResCloudlet> cloudletPausedList)
        {
            this.cloudletPausedList = cloudletPausedList;
        }

        public List<ResCloudlet> GetCloudletFinishedList()
        {
            return cloudletFinishedList;
        }
        
        protected void SetCloudletFinishedList(List<ResCloudlet> cloudletFinishedList)
        {
            this.cloudletFinishedList = cloudletFinishedList;
        }

        public List<ResCloudlet> GetCloudletFailedList()
        {
            return cloudletFailedList;
        }

        protected void SetCloudletFailedList(List<ResCloudlet> cloudletFailedList)
        {
            this.cloudletFailedList = cloudletFailedList;
        }
        
    }
}
