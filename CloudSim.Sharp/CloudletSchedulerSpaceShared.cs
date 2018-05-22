using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class CloudletSchedulerSpaceShared : CloudletScheduler
    {
        protected int currentCpus;
        
        protected int usedPes;
        
        public CloudletSchedulerSpaceShared()
        {
            usedPes = 0;
            currentCpus = 0;
        }

        public override double UpdateVmProcessing(double currentTime, List<Double> mipsShare)
        {
            SetCurrentMipsShare(mipsShare);
            double timeSpam = currentTime - GetPreviousTime(); 
            double capacity = 0.0;
            int cpus = 0;

            foreach (Double mips in mipsShare)
            { 
                capacity += mips;
                if (mips > 0)
                {
                    cpus++;
                }
            }
            currentCpus = cpus;
            capacity /= cpus;
            
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                rcl.UpdateCloudletFinishedSoFar(
                                    (long)(capacity * timeSpam 
                                    * rcl.GetNumberOfPes() * Consts.MILLION));
            }

            if (GetCloudletExecList().Count == 0 
                && GetCloudletWaitingList().Count == 0)
            {
                SetPreviousTime(currentTime);
                return 0.0;
            }
            
            int finished = 0;
            List<ResCloudlet> toRemove = new List<ResCloudlet>();

            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                if (rcl.GetRemainingCloudletLength() == 0)
                {
                    toRemove.Add(rcl);
                    CloudletFinish(rcl);
                    finished++;
                }
            }

            toRemove.ForEach(x => GetCloudletExecList().Remove(x));
            
            if (GetCloudletWaitingList().Count != 0)
            {
                for (int i = 0; i < finished; i++)
                {
                    toRemove.Clear();
                    foreach (ResCloudlet rcl in GetCloudletWaitingList())
                    {
                        if ((currentCpus - usedPes) >= rcl.GetNumberOfPes())
                        {
                            rcl.SetCloudletStatus(Cloudlet.INEXEC);
                            for (int k = 0; k < rcl.GetNumberOfPes(); k++)
                            {
                                rcl.SetMachineAndPeId(0, i);
                            }
                            GetCloudletExecList().Add(rcl);
                            usedPes += rcl.GetNumberOfPes();
                            toRemove.Add(rcl);
                            break;
                        }
                    }

                    toRemove.ForEach(x => GetCloudletExecList().Remove(x));

                }
            }
            
            double nextEvent = Double.MaxValue;
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                double remainingLength = rcl.GetRemainingCloudletLength();
                double estimatedFinishTime = currentTime + (remainingLength / (capacity * rcl.GetNumberOfPes()));
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

        public override Cloudlet CloudletCancel(int cloudletId)
        {

            foreach (ResCloudlet rcl in GetCloudletFinishedList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    GetCloudletFinishedList().Remove(rcl);
                    return rcl.GetCloudlet();
                }
            }
            
            foreach (ResCloudlet rcl in GetCloudletExecList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
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
            }

            foreach (ResCloudlet rcl in GetCloudletPausedList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    GetCloudletPausedList().Remove(rcl);
                    return rcl.GetCloudlet();
                }
            }
            
            foreach (ResCloudlet rcl in GetCloudletWaitingList())
            {
                if (rcl.GetCloudletId() == cloudletId)
                {
                    rcl.SetCloudletStatus(Cloudlet.CANCELED);
                    GetCloudletWaitingList().Remove(rcl);
                    return rcl.GetCloudlet();
                }
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
                
                ResCloudlet rgl = GetCloudletExecList()[position];
                GetCloudletExecList().Remove(rgl);
                if (rgl.GetRemainingCloudletLength() == 0)
                {
                    CloudletFinish(rgl);
                }
                else
                {
                    rgl.SetCloudletStatus(Cloudlet.PAUSED);
                    GetCloudletPausedList().Add(rgl);
                }
                return true;

            }

            position = 0;
            found = false;

            foreach (ResCloudlet rcl in GetCloudletWaitingList())
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

                ResCloudlet rgl = GetCloudletWaitingList()[position];
                GetCloudletWaitingList().Remove(rgl);

                if (rgl.GetRemainingCloudletLength() == 0)
                {
                    CloudletFinish(rgl);
                }
                else
                {
                    rgl.SetCloudletStatus(Cloudlet.PAUSED);
                    GetCloudletPausedList().Add(rgl);
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
            usedPes -= rcl.GetNumberOfPes();
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
                ResCloudlet rcl = GetCloudletPausedList()[position];
                GetCloudletPausedList().Remove(rcl);
                
                if ((currentCpus - usedPes) >= rcl.GetNumberOfPes())
                {
                    rcl.SetCloudletStatus(Cloudlet.INEXEC);
                    for (int i = 0; i < rcl.GetNumberOfPes(); i++)
                    {
                        rcl.SetMachineAndPeId(0, i);
                    }

                    long size = rcl.GetRemainingCloudletLength();
                    size *= rcl.GetNumberOfPes();
                    rcl.GetCloudlet().SetCloudletLength(size);

                    GetCloudletExecList().Add(rcl);
                    usedPes += rcl.GetNumberOfPes();
                    
                    double capacity = 0.0;
                    int cpus = 0;
                    foreach (Double mips in GetCurrentMipsShare())
                    {
                        capacity += mips;
                        if (mips > 0)
                        {
                            cpus++;
                        }
                    }
                    currentCpus = cpus;
                    capacity /= cpus;

                    long remainingLength = rcl.GetRemainingCloudletLength();
                    double estimatedFinishTime = Core.CloudSim.Clock
                            + (remainingLength / (capacity * rcl.GetNumberOfPes()));

                    return estimatedFinishTime;
                }
                else
                {
                    rcl.SetCloudletStatus(Cloudlet.QUEUED);

                    long size = rcl.GetRemainingCloudletLength();
                    size *= rcl.GetNumberOfPes();
                    rcl.GetCloudlet().SetCloudletLength(size);

                    GetCloudletWaitingList().Add(rcl);
                    return 0.0;
                }

            }
            
            return 0.0;

        }

        public override double CloudletSubmit(Cloudlet cloudlet, double fileTransferTime)
        {
            
            if ((currentCpus - usedPes) >= cloudlet.GetNumberOfPes())
            {
                ResCloudlet rcl = new ResCloudlet(cloudlet);
                rcl.SetCloudletStatus(Cloudlet.INEXEC);
                for (int i = 0; i < cloudlet.GetNumberOfPes(); i++)
                {
                    rcl.SetMachineAndPeId(0, i);
                }

                GetCloudletExecList().Add(rcl);
                usedPes += cloudlet.GetNumberOfPes();
            }
            else
            {
                ResCloudlet rcl = new ResCloudlet(cloudlet);
                rcl.SetCloudletStatus(Cloudlet.QUEUED);
                GetCloudletWaitingList().Add(rcl);
                return 0.0;
            }
            
            double capacity = 0.0;
            int cpus = 0;
            foreach (Double mips in GetCurrentMipsShare())
            {
                capacity += mips;
                if (mips > 0)
                {
                    cpus++;
                }
            }

            currentCpus = cpus;
            capacity /= cpus;
            
            double extraSize = capacity * fileTransferTime;
            long length = cloudlet.GetCloudletLength();
            length += (long)extraSize;
            cloudlet.SetCloudletLength(length);
            return cloudlet.GetCloudletLength() / capacity;
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

            foreach (ResCloudlet rcl in GetCloudletWaitingList())
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
                Cloudlet cloudlet = GetCloudletFinishedList()[0].GetCloudlet();
                GetCloudletFinishedList().RemoveAt(0);
                return cloudlet;
            }
            return null;
        }

        public override int RunningCloudlets()
        {
            return GetCloudletExecList().Count;
        }

      
        public override Cloudlet MigrateCloudlet()
        {
            ResCloudlet rcl = GetCloudletExecList()[0];
            GetCloudletExecList().RemoveAt(0);

            rcl.FinalizeCloudlet();
            Cloudlet cl = rcl.GetCloudlet();
            usedPes -= cl.GetNumberOfPes();
            return cl;
        }

        public override List<Double> GetCurrentRequestedMips()
        {
            List<Double> mipsShare = new List<Double>();
            if (GetCurrentMipsShare() != null)
            {
                foreach (Double mips in GetCurrentMipsShare())
                {
                    mipsShare.Add(mips);
                }
            }
            return mipsShare;
        }

        public override double GetTotalCurrentAvailableMipsForCloudlet
            (ResCloudlet rcl, List<Double> mipsShare)
        {
         
            double capacity = 0.0;
            int cpus = 0;
            foreach (Double mips in mipsShare)
            { 
                capacity += mips;
                if (mips > 0)
                {
                    cpus++;
                }
            }
            currentCpus = cpus;
            capacity /= cpus; 
            return capacity;
        }

        public override double GetTotalCurrentAllocatedMipsForCloudlet
            (ResCloudlet rcl, double time)
        {
            //TODO: the method isn't in fact implemented
 
            return 0.0;
        }

        public override double GetTotalCurrentRequestedMipsForCloudlet
            (ResCloudlet rcl, double time)
        {
            //TODO: the method isn't in fact implemented
           
            return 0.0;
        }

        public override double GetCurrentRequestedUtilizationOfRam()
        {
            //TODO: the method isn't in fact implemented

            return 0.0;
        }

        public override double GetCurrentRequestedUtilizationOfBw()
        {
            //TODO: the method isn't in fact implemented

            return 0.0;
        }

    }
}

