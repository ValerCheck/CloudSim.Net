using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class ResCloudlet { }

    class CloudletSchedulerTimeShared : CloudletScheduler
    {

        protected int currentCPUs;

        public CloudletSchedulerTimeShared() : base()
        {
            currentCPUs = 0;
        }

        public override double UpdateVmProcessing(double currentTime, List<Double> mipsShare)
        {
            SetCurrentMipsShare(mipsShare);
            double timeSpam = currentTime - GetPreviousTime();

            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                rcl.UpdateCloudletFinishedSoFar((long)(GetCapacity(mipsShare) 
                    * timeSpam * rcl.GetNumberOfPes() * Consts.MILLION));
            }

            if (GetCloudletExecList().Count == 0)
            {
                SetPreviousTime(currentTime);
                return 0.0;
            }

            double nextEvent = Double.MaxValue;
            List<ResCloudlet> toRemove = new List<ResCloudlet>();
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                long remainingLength = rcl.GetRemainingCloudletLength();
                if (remainingLength == 0)
                {
                    toRemove.Add(rcl);
                    CloudletFinish(rcl);
                    continue;
                }
            }
            toRemove.ForEach(x => GetCloudletExecList().Remove(x));
            
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                double estimatedFinishTime = currentTime
                        + (rcl.GetRemainingCloudletLength() / (GetCapacity(mipsShare) * rcl.GetNumberOfPes()));
                if (estimatedFinishTime - currentTime < Core.CloudSim.GetMinTimeBetweenEvents())
                {
                    estimatedFinishTime = currentTime + Core.CloudSim.GetMinTimeBetweenEvents();
                }

                if (estimatedFinishTime < nextEvent)
                {
                    nextEvent = estimatedFinishTime;
                }
            }

            SetPreviousTime(currentTime);
            return nextEvent;
        }

        protected double GetCapacity(List<Double> mipsShare)
        {
            double capacity = 0.0;
            int cpus = 0;
            foreach (Double mips in mipsShare)
            {
                capacity += mips;
                if (mips > 0.0)
                {
                    cpus++;
                }
            }
            currentCPUs = cpus;

            int pesInUse = 0;
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                pesInUse += rcl.GetNumberOfPes();
            }

            if (pesInUse > currentCPUs)
            {
                capacity /= pesInUse;
            }
            else
            {
                capacity /= currentCPUs;
            }
            return capacity;
        }


        public override Cloudlet CloudletCancel(int cloudletId)
        {
            bool found = false;
            int position = 0;

            
            found = false;
            foreach (ResCloudlet rcl in GetCloudletFinishedList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    found = true;
                    break;
                }
                position++;
            }

            if (found)
            {
                ResCloudlet rcl = GetCloudletFinishedList()[position];
                GetCloudletFinishedList().Remove(rcl);
                return rcl.GetCloudlet();
            }

            position = 0;
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    found = true;
                    break;
                }
                position++;
            }

            if (found)
            {
                ResCloudlet rcl = GetCloudletExecList()[position];
                GetCloudletExecList().Remove(rcl);
                if (rcl.GetRemainingCloudletLength() == 0)
                {
                    CloudletFinish(rcl);
                }
                else
                {
                    rcl.SetCloudletStatus(Cloudlet.CANCELED);
                }
                return rcl.GetCloudlet();
            }
            
            found = false;
            position = 0;
            foreach (ResCloudlet rcl in GetCloudletPausedList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    found = true;
                    rcl.SetCloudletStatus(Cloudlet.CANCELED);
                    break;
                }
                position++;
            }

            if (found)
            {
                ResCloudlet rcl = GetCloudletPausedList()[position];
                GetCloudletPausedList().Remove(rcl);
                return rcl.GetCloudlet();
            }

            return null;
        }

        public override bool CloudletPause(int cloudletId)
        {
            bool found = false;
            int position = 0;

            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    found = true;
                    break;
                }
                position++;
            }

            if (found)
            {
                ResCloudlet rcl = GetCloudletExecList()[position];
                GetCloudletExecList().Remove(rcl);
                if (rcl.GetRemainingCloudletLength() == 0)
                {
                    CloudletFinish(rcl);
                }
                else
                {
                    rcl.SetCloudletStatus(Cloudlet.PAUSED);
                    GetCloudletPausedList().Add(rcl);
                }
                return true;
            }
            return false;
        }

       
        public override void CloudletFinish(ResCloudlet rcl)
        {
            rcl.SetCloudletStatus(Cloudlet.SUCCESS);
            rcl.FinalizeCloudlet();
            GetCloudletFinishedList().Add(rcl);
        }

        
        public override double CloudletResume(int cloudletId)
        {
            bool found = false;
            int position = 0;
            
            foreach (ResCloudlet rcl in GetCloudletPausedList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    found = true;
                    break;
                }
                position++;
            }

            if (found)
            {
                ResCloudlet rgl = GetCloudletPausedList()[position];
                GetCloudletPausedList().Remove(rgl);
                rgl.SetCloudletStatus(Cloudlet.INEXEC);
                GetCloudletExecList().Add(rgl);
                
                double remainingLength = rgl.GetRemainingCloudletLength();
                double estimatedFinishTime = Core.CloudSim.Clock
                        + (remainingLength / (GetCapacity(GetCurrentMipsShare()) * rgl.GetNumberOfPes()));

                return estimatedFinishTime;
            }

            return 0.0;
        }

        public override double CloudletSubmit(Cloudlet cloudlet, double fileTransferTime)
        {
            ResCloudlet rcl = new ResCloudlet(cloudlet);
            rcl.SetCloudletStatus(Cloudlet.INEXEC);
            for (int i = 0; i < cloudlet.GetNumberOfPes(); i++)
            {
                rcl.SetMachineAndPeId(0, i);
            }

            GetCloudletExecList().Add(rcl);
            
            double extraSize = GetCapacity(GetCurrentMipsShare()) * fileTransferTime;
            long length = (long)(cloudlet.GetCloudletLength() + extraSize);
            cloudlet.SetCloudletLength(length);

            return cloudlet.GetCloudletLength() / GetCapacity(GetCurrentMipsShare());
        }

        public override double CloudletSubmit(Cloudlet cloudlet)
        {
            return CloudletSubmit(cloudlet, 0.0);
        }

        
        public override int GetCloudletStatus(int cloudletId)
        {
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    return rcl.GetCloudletStatus();
                }
            }
            foreach (ResCloudlet rcl in GetCloudletPausedList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    return rcl.GetCloudletStatus();
                }
            }
            return -1;
        }

        public override double GetTotalUtilizationOfCpu(double time)
        {
            double totalUtilization = 0;
            foreach (ResCloudlet gl in GetCloudletExecList())
            {
                totalUtilization += gl.GetCloudlet().GetUtilizationOfCpu(time);
            }
            return totalUtilization;
        }

        public override bool IsFinishedCloudlets()
        {
            return GetCloudletFinishedList().Count > 0;
        }

        public override Cloudlet GetNextFinishedCloudlet()
        {
            if (GetCloudletFinishedList().Count > 0)
            {
                ResCloudlet rcl = GetCloudletFinishedList()[0];
                GetCloudletFinishedList().RemoveAt(0);
                return rcl.GetCloudlet();
            }
            return null;
        }

        public override int RunningCloudlets()
        {
            return GetCloudletExecList().Count;
        }

        public override Cloudlet MigrateCloudlet()
        {
            ResCloudlet rgl = GetCloudletExecList()[0];
            GetCloudletExecList().RemoveAt(0);
            rgl.FinalizeCloudlet();
            return rgl.GetCloudlet();
        }

        public override List<Double> GetCurrentRequestedMips()
        {
            List<Double> mipsShare = new List<Double>();
            return mipsShare;
        }

        public override double GetTotalCurrentAvailableMipsForCloudlet
            (ResCloudlet rcl, List<Double> mipsShare)
        {
            
            return GetCapacity(GetCurrentMipsShare());
        }

        public override double GetTotalCurrentAllocatedMipsForCloudlet
            (ResCloudlet rcl, double time)
        {
            return 0.0;
        }

   
        public override double GetTotalCurrentRequestedMipsForCloudlet
            (ResCloudlet rcl, double time)
        {
            return 0.0;
        }

  
        public override double GetCurrentRequestedUtilizationOfRam()
        {
            double ram = 0;
            foreach (ResCloudlet cloudlet in cloudletExecList)
            {
                ram += cloudlet.GetCloudlet().GetUtilizationOfRam(Core.CloudSim.Clock);
            }
            return ram;
        }

 
        public override double GetCurrentRequestedUtilizationOfBw()
        {
            double bw = 0;
            foreach (ResCloudlet cloudlet in cloudletExecList)
            {
                bw += cloudlet.GetCloudlet().GetUtilizationOfBw(Core.CloudSim.Clock);
            }
            return bw;
        }

    }
}
