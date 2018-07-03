using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSim.Sharp.Core;
using CloudSim.Sharp.Core.Predicates;

namespace CloudSim.Sharp
{
    public class Cloudlet
    {
        private readonly int _cloudletId;

        private int userId;

        private long cloudletLength;

        private readonly long cloudletFileSize;

        private readonly long cloudletOutputSize;

        private int numberOfPes;

        private int status;

        private double execStartTime;

        private double finishTime;

        private int reservationId = -1;

        private readonly bool record;

        private String newline;

        private StringBuilder history;

        private readonly List<Resource> resList;

        private int index;

        private int classType;

        private int netToS;

        private string num;

        public const int CREATED = 0;

        public const int READY = 1;

        public const int QUEUED = 2;

        public const int INEXEC = 3;

        public const int SUCCESS = 4;

        public const int FAILED = 5;

        public const int CANCELED = 6;

        public const int PAUSED = 7;

        public const int RESUMED = 8;

        public const int FAILED_RESOURCE_UNAVAILABLE = 9;

        protected int _vmId;

        protected double costPerBw;

        protected double accumulatedBwCost;

        private UtilizationModel utilizationModelCpu;

        private UtilizationModel utilizationModelRam;

        private UtilizationModel utilizationModelBw;

        private List<String> requiredFiles = null;

        public Cloudlet(
                 int cloudletId,
                 long cloudletLength,
                 int pesNumber,
                 long cloudletFileSize,
                 long cloudletOutputSize,
                 UtilizationModel utilizationModelCpu,
                 UtilizationModel utilizationModelRam,
                 UtilizationModel utilizationModelBw)
            : this(
                    cloudletId,
                    cloudletLength,
                    pesNumber,
                    cloudletFileSize,
                    cloudletOutputSize,
                    utilizationModelCpu,
                    utilizationModelRam,
                    utilizationModelBw,
                    false)
        {
            _vmId = -1;
            accumulatedBwCost = 0;
            costPerBw = 0;
            requiredFiles = new List<String>();
        }

        public Cloudlet(
                int cloudletId,
                long cloudletLength,
                int pesNumber,
                long cloudletFileSize,
                long cloudletOutputSize,
                UtilizationModel utilizationModelCpu,
                UtilizationModel utilizationModelRam,
                UtilizationModel utilizationModelBw,
                bool record,
                List<String> fileList) :
            this(
                    cloudletId,
                    cloudletLength,
                    pesNumber,
                    cloudletFileSize,
                    cloudletOutputSize,
                    utilizationModelCpu,
                    utilizationModelRam,
                    utilizationModelBw,
                    record)
        {
            _vmId = -1;
            accumulatedBwCost = 0.0;
            costPerBw = 0.0;

            requiredFiles = fileList;
        }

        public Cloudlet(
                int cloudletId,
                long cloudletLength,
                int pesNumber,
                long cloudletFileSize,
                long cloudletOutputSize,
                UtilizationModel utilizationModelCpu,
                UtilizationModel utilizationModelRam,
                UtilizationModel utilizationModelBw,
                List<String> fileList) :
            this(
                    cloudletId,
                    cloudletLength,
                    pesNumber,
                    cloudletFileSize,
                    cloudletOutputSize,
                    utilizationModelCpu,
                    utilizationModelRam,
                    utilizationModelBw,
                    false)
        {
            _vmId = -1;
            accumulatedBwCost = 0.0;
            costPerBw = 0.0;

            requiredFiles = fileList;
        }

        public Cloudlet(
                 int cloudletId,
                 long cloudletLength,
                 int pesNumber,
                 long cloudletFileSize,
                 long cloudletOutputSize,
                 UtilizationModel utilizationModelCpu,
                 UtilizationModel utilizationModelRam,
                 UtilizationModel utilizationModelBw,
                 bool record)
        {
            userId = -1;
            status = CREATED;
            _cloudletId = cloudletId;
            numberOfPes = pesNumber;
            execStartTime = 0.0;
            finishTime = -1.0;
            classType = 0;
            netToS = 0;

            this.cloudletLength = Math.Max(1, cloudletLength);
            this.cloudletFileSize = Math.Max(1, cloudletFileSize);
            this.cloudletOutputSize = Math.Max(1, cloudletOutputSize);

            resList = new List<Resource>(2);
            index = -1;
            this.record = record;

            _vmId = -1;
            accumulatedBwCost = 0.0;
            costPerBw = 0.0;

            requiredFiles = new List<String>();

            SetUtilizationModelCpu(utilizationModelCpu);
            SetUtilizationModelRam(utilizationModelRam);
            SetUtilizationModelBw(utilizationModelBw);
        }

        public class Resource
        {

            public double submissionTime = 0.0;

            public double wallClockTime = 0.0;

            public double actualCPUTime = 0.0;

            public double costPerSec = 0.0;

            public long finishedSoFar = 0;

            public int resourceId = -1;

            public String resourceName = null;

        }

        public bool SetReservationId(int resId)
        {
            if (resId <= 0)
            {
                return false;
            }
            reservationId = resId;
            return true;
        }

        public int GetReservationId()
        {
            return reservationId;
        }

        public bool HasReserved()
        {
            if (reservationId == -1)
            {
                return false;
            }
            return true;
        }

        public bool SetCloudletLength(long cloudletLength)
        {
            if (cloudletLength <= 0)
            {
                return false;
            }

            this.cloudletLength = cloudletLength;
            return true;
        }

        public bool SetNetServiceLevel(int netServiceLevel)
        {
            bool success = false;
            if (netServiceLevel > 0)
            {
                netToS = netServiceLevel;
                success = true;
            }

            return success;
        }

        public int GetNetServiceLevel()
        {
            return netToS;
        }

        public double GetWaitingTime()
        {
            if (index == -1)
            {
                return 0;
            }

            double subTime = resList[index].submissionTime;
            return execStartTime - subTime;
        }

        public bool SetClassType(int classType)
        {
            bool success = false;
            if (classType > 0)
            {
                this.classType = classType;
                success = true;
            }

            return success;
        }

        public int GetClassType()
        {
            return classType;
        }

        public bool SetNumberOfPes(int numberOfPes)
        {
            if (numberOfPes > 0)
            {
                this.numberOfPes = numberOfPes;
                return true;
            }
            return false;
        }

        public int GetNumberOfPes()
        {
            return numberOfPes;
        }

        public String GetCloudletHistory()
        {
            String msg = null;
            if (history == null)
            {
                msg = "No history is recorded for Cloudlet #" + _cloudletId;
            }
            else
            {
                msg = history.ToString();
            }

            return msg;
        }

        public long GetCloudletFinishedSoFar()
        {
            if (index == -1)
            {
                return cloudletLength;
            }

            long finish = resList[index].finishedSoFar;
            if (finish > cloudletLength)
            {
                return cloudletLength;
            }

            return finish;
        }

        public bool IsFinished()
        {
            if (index == -1)
            {
                return false;
            }

            bool completed = false;

            long finish = resList[index].finishedSoFar;
            long result = cloudletLength - finish;
            if (result <= 0.0)
            {
                completed = true;
            }
            return completed;
        }

        public void SetCloudletFinishedSoFar(long length)
        {

            if (length < 0.0 || index < 0)
            {
                return;
            }

            Resource res = resList[index];
            res.finishedSoFar = length;

            if (record)
            {
                Write("Sets the length's finished so far to " + length);
            }
        }

        public void SetUserId(int id)
        {
            userId = id;
            if (record)
            {
                Write("Assigns the Cloudlet to "
                    + Core.CloudSim.GetEntityName(id) + " (ID #" + id + ")");
            }
        }

        public int GetUserId()
        {
            return userId;
        }

        public int GetResourceId()
        {
            if (index == -1)
            {
                return -1;
            }
            return resList[index].resourceId;
        }

        public long GetCloudletFileSize()
        {
            return cloudletFileSize;
        }

        public long GetCloudletOutputSize()
        {
            return cloudletOutputSize;
        }

        public void SetResourceParameter(int resourceID, double cost)
        {
            Resource res = new Resource
            {
                resourceId = resourceID,
                costPerSec = cost,
                resourceName = Core.CloudSim.GetEntityName(resourceID)
            };

            resList.Add(res);

            if (index == -1 && record)
            {
                Write("Allocates this Cloudlet to "
                    + res.resourceName
                    + " (ID #" + resourceID
                    + ") with cost = $" + cost + "/sec");
            }
            else if (record)
            {
                int id = resList[index].resourceId;
                String name = resList[index].resourceName;
                Write("Moves Cloudlet from "
                    + name + " (ID #" + id + ") to "
                    + res.resourceName + " (ID #"
                    + resourceID + ") with cost = $" + cost + "/sec");
            }

            index++;
        }

        public void SetSubmissionTime(double clockTime)
        {
            if (clockTime < 0.0 || index < 0)
            {
                return;
            }

            Resource res = resList[index];
            res.submissionTime = clockTime;

            if (record)
            {
                Write("Sets the submission time to " + clockTime.ToString(num));
            }
        }

        public double GetSubmissionTime()
        {
            if (index == -1)
            {
                return 0.0;
            }
            return resList[index].submissionTime;
        }

        public void SetExecStartTime(double clockTime)
        {
            execStartTime = clockTime;
            if (record)
            {
                Write("Sets the execution start time to " + clockTime.ToString(num));
            }
        }

        public double GetExecStartTime()
        {
            return execStartTime;
        }

        public void SetExecParam(double wallTime, double actualTime)
        {
            if (wallTime < 0.0 || actualTime < 0.0 || index < 0)
            {
                return;
            }

            Resource res = resList[index];
            res.wallClockTime = wallTime;
            res.actualCPUTime = actualTime;

            if (record)
            {
                Write("Sets the wall clock time to " + wallTime.ToString(num) + " and the actual CPU time to "
                        + actualTime.ToString(num));
            }
        }

        public void SetCloudletStatus(int newStatus)
        {

            if (status == newStatus)
            {
                return;
            }

            if (newStatus < CREATED || newStatus > FAILED_RESOURCE_UNAVAILABLE)
            {
                throw new Exception(
                        "Cloudlet.setCloudletStatus() : Error - Invalid integer range for Cloudlet status.");

            }

            if (newStatus == SUCCESS)
            {
                finishTime = Core.CloudSim.Clock;
            }

            if (record)
            {
                Write("Sets Cloudlet status from "
                    + GetCloudletStatusString() + " to "
                    + GetStatusString(newStatus));
            }

            status = newStatus;
        }


        public int GetCloudletStatus()
        {
            return status;
        }

        public String GetCloudletStatusString()
        {
            return GetStatusString(status);
        }

        public static String GetStatusString(int status)
        {
            String statusString = null;
            switch (status)
            {
                case CREATED:
                    statusString = "Created";
                    break;

                case READY:
                    statusString = "Ready";
                    break;

                case INEXEC:
                    statusString = "InExec";
                    break;

                case SUCCESS:
                    statusString = "Success";
                    break;

                case QUEUED:
                    statusString = "Queued";
                    break;

                case FAILED:
                    statusString = "Failed";
                    break;

                case CANCELED:
                    statusString = "Canceled";
                    break;

                case PAUSED:
                    statusString = "Paused";
                    break;

                case RESUMED:
                    statusString = "Resumed";
                    break;

                case FAILED_RESOURCE_UNAVAILABLE:
                    statusString = "Failed_resource_unavailable";
                    break;

                default:
                    break;
            }

            return statusString;
        }

        public long GetCloudletLength()
        {
            return cloudletLength;
        }

        public long GetCloudletTotalLength()
        {
            return GetCloudletLength() * GetNumberOfPes();
        }

        public double GetCostPerSec()
        {
            if (index == -1)
            {
                return 0.0;
            }
            return resList[index].costPerSec;
        }

        public double GetWallClockTime()
        {
            if (index == -1)
            {
                return 0.0;
            }
            return resList[index].wallClockTime;
        }

        public String[] GetAllResourceName()
        {
            int size = resList.Count;
            String[] data = null;

            if (size > 0)
            {
                data = new String[size];
                for (int i = 0; i < size; i++)
                {
                    data[i] = resList[i].resourceName;
                }
            }

            return data;
        }

        public int[] GetAllResourceId()
        {
            int size = resList.Count;
            int[] data = null;

            if (size > 0)
            {
                data = new int[size];
                for (int i = 0; i < size; i++)
                {
                    data[i] = resList[i].resourceId;
                }
            }

            return data;
        }

        public double GetActualCPUTime(int resId)
        {
            Resource resource = GetResourceById(resId);
            if (resource != null)
            {
                return resource.actualCPUTime;
            }
            return 0.0;
        }

        public double GetCostPerSec(int resId)
        {
            Resource resource = GetResourceById(resId);
            if (resource != null)
            {
                return resource.costPerSec;
            }
            return 0.0;
        }

        public long GetCloudletFinishedSoFar(int resId)
        {
            Resource resource = GetResourceById(resId);
            if (resource != null)
            {
                return resource.finishedSoFar;
            }
            return 0;
        }

        public double GetSubmissionTime(int resId)
        {
            Resource resource = GetResourceById(resId);
            if (resource != null)
            {
                return resource.submissionTime;
            }
            return 0.0;
        }

        public double GetWallClockTime(int resId)
        {
            Resource resource = GetResourceById(resId);
            if (resource != null)
            {
                return resource.wallClockTime;
            }
            return 0.0;
        }

        public String GetResourceName(int resId)
        {
            Resource resource = GetResourceById(resId);
            if (resource != null)
            {
                return resource.resourceName;
            }
            return null;
        }

        public Resource GetResourceById(int resourceId)
        {
            Resource result = null;
            foreach (Resource resource in resList)
            {
                if (resource.resourceId == resourceId)
                {
                    result = resource;
                }
            }
            return result;
        }

        public double GetFinishTime()
        {
            return finishTime;
        }

        protected void Write(String str)
        {
            if (record)
            {
                return;
            }

            if (num == null || history == null)
            {
                newline = Environment.NewLine;
                num = "#0.00#";
                history = new StringBuilder(1000);
                history.Append("Time below denotes the simulation time.");
                history.Append(newline);
                history.Append("Time (sec)       Description Cloudlet #" + _cloudletId);
                history.Append(newline);
                history.Append("------------------------------------------");
                history.Append(newline);
                history.Append(Core.CloudSim.Clock.ToString(num));
                history.Append("   Creates Cloudlet ID #" + _cloudletId);
                history.Append(newline);
            }

            history.Append(Core.CloudSim.Clock.ToString(num));
            history.Append("   " + str + newline);
        }

        public int GetStatus()
        {
            return status;
        }

        public int CloudletId => _cloudletId;
        
        public int VmId
        {
            get { return _vmId; }
            set { _vmId = value; }
        }
        
        public double GetActualCPUTime()
        {
            return GetFinishTime() - GetExecStartTime();
        }

        public void SetResourceParameter(int resourceID, double costPerCPU, double costPerBw)
        {
            SetResourceParameter(resourceID, costPerCPU);
            this.costPerBw = costPerBw;
            accumulatedBwCost = costPerBw * GetCloudletFileSize();
        }

        public double GetProcessingCost()
        {
            double cost = 0;
            cost += accumulatedBwCost;
            cost += costPerBw * GetCloudletOutputSize();
            return cost;
        }

        public List<String> GetRequiredFiles()
        {
            return requiredFiles;
        }

        protected void SetRequiredFiles(List<String> requiredFiles)
        {
            this.requiredFiles = requiredFiles;
        }

        public bool AddRequiredFile(String fileName)
        {

            if (GetRequiredFiles() == null)
            {
                SetRequiredFiles(new List<String>());
            }

            bool result = false;
            for (int i = 0; i < GetRequiredFiles().Count; i++)
            {
                String temp = GetRequiredFiles()[i];
                if (temp.Equals(fileName))
                {
                    result = true;
                    break;
                }
            }

            if (!result)
            {
                GetRequiredFiles().Add(fileName);
            }

            return result;
        }

        public bool DeleteRequiredFile(String filename)
        {
            bool result = false;
            if (GetRequiredFiles() == null)
            {
                return result;
            }

            for (int i = 0; i < GetRequiredFiles().Count; i++)
            {
                String temp = GetRequiredFiles()[i];

                if (temp.Equals(filename))
                {
                    GetRequiredFiles().Remove(filename);
                    result = true;

                    break;
                }
            }

            return result;
        }

        public bool RequiresFiles()
        {
            bool result = false;
            if (GetRequiredFiles() != null && GetRequiredFiles().Count > 0)
            {
                result = true;
            }

            return result;
        }

        public UtilizationModel GetUtilizationModelCpu()
        {
            return utilizationModelCpu;
        }

        public void SetUtilizationModelCpu(UtilizationModel utilizationModelCpu)
        {
            this.utilizationModelCpu = utilizationModelCpu;
        }

        public UtilizationModel GetUtilizationModelRam()
        {
            return utilizationModelRam;
        }

        public void SetUtilizationModelRam(UtilizationModel utilizationModelRam)
        {
            this.utilizationModelRam = utilizationModelRam;
        }

        public UtilizationModel GetUtilizationModelBw()
        {
            return utilizationModelBw;
        }

        public void SetUtilizationModelBw(UtilizationModel utilizationModelBw)
        {
            this.utilizationModelBw = utilizationModelBw;
        }

        public double GetUtilizationOfCpu(double time)
        {
            return GetUtilizationModelCpu().GetUtilization(time);
        }

        public double GetUtilizationOfRam(double time)
        {
            return GetUtilizationModelRam().GetUtilization(time);
        }

        public double GetUtilizationOfBw(double time)
        {
            return GetUtilizationModelBw().GetUtilization(time);
        }

    }
    
}
