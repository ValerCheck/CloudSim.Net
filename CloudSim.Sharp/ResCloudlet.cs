using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class ResCloudlet
    {

        private readonly Cloudlet _cloudlet;

	    private double _arrivalTime;

        private double _finishedTime;

        private long _cloudletFinishedSoFar;
        
        private double _startExecTime;
        
        private double _totalCompletionTime;
        
        private int _machineId;
        
        private int _peId;
        
        private int[] _machineArrayId = null;
        
        private int[] _peArrayId = null;
        
        private int _index;
        
        private const int NOT_FOUND = -1;
        
        private readonly long _startTime;
        
        private readonly int _duration;
        
        private readonly int _reservId;
        
        private int _pesNumber;

        public ResCloudlet(Cloudlet cloudlet)
        {
            _cloudlet = cloudlet;
            _startTime = 0;
            _reservId = NOT_FOUND;
            _duration = 0;

            Init();
        }

        public ResCloudlet(Cloudlet cloudlet, long startTime, int duration, int reservID)
        {
            _cloudlet = cloudlet;
            _startTime = startTime;
            _reservId = reservID;
            _duration = duration;

            Init();
        }

        public long GetStartTime()
        {
            return _startTime;
        }
        
        public int GetDurationTime()
        {
            return _duration;
        }
        
        public int GetNumberOfPes()
        {
            return _pesNumber;
        }

        public int GetReservationID()
        {
            return _reservId;
        }
        
        public bool HasReserved()
        {
            if (_reservId == NOT_FOUND)
            {
                return false;
            }

            return true;
        }

        private void Init()
        {
            _pesNumber = _cloudlet.GetNumberOfPes();
            
            if (_pesNumber > 1)
            {
                _machineArrayId = new int[_pesNumber];
                _peArrayId = new int[_pesNumber];
            }

            _arrivalTime = Core.CloudSim.Clock;
            _cloudlet.SetSubmissionTime(_arrivalTime);

            _finishedTime = NOT_FOUND; 
            _machineId = NOT_FOUND;
            _peId = NOT_FOUND;
            _index = 0;
            _totalCompletionTime = 0.0;
            _startExecTime = 0.0;

            _cloudletFinishedSoFar = _cloudlet.GetCloudletFinishedSoFar() * Consts.MILLION;
        }

        public int GetCloudletId()
        {
            return _cloudlet.GetCloudletId();
        }

        public int GetUserId()
        {
            return _cloudlet.GetUserId();
        }

        public long GetCloudletLength()
        {
            return _cloudlet.GetCloudletLength();
        }

        public long GetCloudletTotalLength()
        {
            return _cloudlet.GetCloudletTotalLength();
        }
        
        public int GetCloudletClassType()
        {
            return _cloudlet.GetClassType();
        }

        public bool SetCloudletStatus(int status)
        {
            int prevStatus = _cloudlet.GetCloudletStatus();
            
            if (prevStatus == status)
            {
                return false;
            }

            bool success = true;
            try
            {
                double clock = Core.CloudSim.Clock; 
                
                _cloudlet.SetCloudletStatus(status);
                
                if (prevStatus == Cloudlet.INEXEC)
                {
                    if (status == Cloudlet.CANCELED || status == Cloudlet.PAUSED || status == Cloudlet.SUCCESS)
                    {
                        _totalCompletionTime += (clock - _startExecTime);
                        _index = 0;
                        return true;
                    }
                }

                if (prevStatus == Cloudlet.RESUMED && status == Cloudlet.SUCCESS)
                {
                    _totalCompletionTime += (clock - _startExecTime);
                    return true;
                }

                if (status == Cloudlet.INEXEC || (prevStatus == Cloudlet.PAUSED && status == Cloudlet.RESUMED))
                {
                    _startExecTime = clock;
                    _cloudlet.SetExecStartTime(_startExecTime);
                }

            }
            catch
            {
                success = false;
            }

            return success;
        }
        
        public double GetExecStartTime()
        {
            return _cloudlet.GetExecStartTime();
        }
        
        public void SetExecParam(double wallClockTime, double actualCPUTime)
        {
            _cloudlet.SetExecParam(wallClockTime, actualCPUTime);
        }

        public void SetMachineAndPeId(int machineId, int peId)
        {
            _machineId = machineId;
            _peId = peId;

            if (_peArrayId != null && _pesNumber > 1)
            {
                _machineArrayId[_index] = machineId;
                _peArrayId[_index] = peId;
                _index++;
            }
        }
        
        public int GetMachineId()
        {
            return _machineId;
        }
        
        public int GetPeId()
        {
            return _peId;
        }

        public int[] GetPeIdList()
        {
            return _peArrayId;
        }

        public int[] GetMachineIdList()
        {
            return _machineArrayId;
        }
        
        public long GetRemainingCloudletLength()
        {
            long length = _cloudlet.GetCloudletTotalLength() * Consts.MILLION - _cloudletFinishedSoFar;
            
            if (length < 0)
            {
                return 0;
            }

            return (long)Math.Floor((decimal)(length / Consts.MILLION));
        }
        
        public void FinalizeCloudlet()
        {
            double wallClockTime = Core.CloudSim.Clock - _arrivalTime;
            _cloudlet.SetExecParam(wallClockTime, _totalCompletionTime);

            long finished = 0;
            
            if (_cloudlet.GetCloudletStatus() == Cloudlet.SUCCESS)
            {
                finished = _cloudlet.GetCloudletLength();
            }
            else
            {
                finished = _cloudletFinishedSoFar / Consts.MILLION;
            }

            _cloudlet.SetCloudletFinishedSoFar(finished);
        }
        
        public void UpdateCloudletFinishedSoFar(long miLength)
        {
            _cloudletFinishedSoFar += miLength;
        }
        
        public double GetCloudletArrivalTime()
        {
            return _arrivalTime;
        }
        
        public void SetFinishTime(double time)
        {
            if (time < 0.0)
            {
                return;
            }

            _finishedTime = time;
        }
        
        public double GetClouddletFinishTime()
        {
            return _finishedTime;
        }

        public Cloudlet GetCloudlet()
        {
            return _cloudlet;
        }
        
        public int GetCloudletStatus()
        {
            return _cloudlet.GetCloudletStatus();
        }
        
        public String GetUid()
        {
            return GetUserId() + "-" + GetCloudletId();
        }
    }
}
