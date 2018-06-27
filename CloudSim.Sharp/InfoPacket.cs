using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSim.Sharp.Core;

namespace CloudSim.Sharp
{
    public class InfoPacket : IPacket
    {
        private readonly string _name;
        private long _size;
        private readonly int _packetId;
        private readonly int _srcId;
        private int _destId;
        private int _last;
        private int _tag;
        private int _hopsNumber;
        private long _pingSize;
        private int _netServiceType;
        private double _bandwidth;
        private List<int> _entities;
        private List<double> _entryTimes;
        private List<double> _exitTimes;
        private List<double> _baudRates;
        private string _numFormat;

        public InfoPacket(string name, int packetID, long size, int srcID, int destID, int netServiceType)
        {
            _name = name;
            _packetId = packetID;
            _srcId = srcID;
            _destId = destID;
            _size = size;
            _netServiceType = netServiceType;

            Initialize();
        }

        private void Initialize()
        {
            _last = _srcId;
            _tag = CloudSimTags.INFOPKT_SUBMIT;
            _bandwidth = -1;
            _hopsNumber = 0;
            _pingSize = _size;

            if (_name != null)
            {
                _entities = new List<int>();
                _entryTimes = new List<double>();
                _exitTimes = new List<double>();
                _baudRates = new List<double>();
                _numFormat = "#0.000#";
            }
        }

        public int GetId()
        {
            return _packetId;
        }

        public long OriginalPingSize
        {
            get { return _size; }
            set { _size = value; }
        }

        public override string ToString()
        {
            if (_name == null)
            {
                return "Empty InfoPacket that contains no ping information.";
            }

            int SIZE = 1000;
            var sb = new StringBuilder();
            sb.AppendLine($"Ping information for {_name}")
              .AppendLine("Entity Name\tEntry Time\tExit Time\t Bandwidth")
              .AppendLine("----------------------------------------------------------");

            string tab = "    ";
            for (var i = 0; i < _entities.Count; i++)
            {
                int resId = _entities[i];
                sb.Append($"{Core.CloudSim.GetEntityName(resId)}\t\t");

                string entry = GetData(_entryTimes, i);
                string exit = GetData(_exitTimes, i);
                string bw = GetData(_baudRates, i);

                sb.AppendLine($"{entry}{tab}{tab}{exit}{tab}{tab}{bw}");
            }

            sb.Append($"\nRound Trip Time : {GetTotalResponseTime().ToString(_numFormat)} seconds")
                .Append("\nNumber of Hops  : " + GetNumHop())
                .Append($"\nBottleneck Bandwidth : {_bandwidth} bits/s");
            return sb.ToString();
        }

        public long Size
        {
            get { return _size; }
            set
            {
                if (value < 0) throw new ArgumentException("Size of info packet cannot be negative.");
                _size = value;
            }
        }

        private string GetData(List<double> v, int index)
        {
            string result;
            try
            {
                double obj = v[index];
                result = obj.ToString(_numFormat);
            }
            catch (Exception)
            {
                result = "    N/A";
            }

            return result;
        }

        public double BaudRate
        {
            get { return _bandwidth; }
        }

        public void AddHop(int id)
        {
            if (_entities == null) return;
            _hopsNumber++;
            _entities.Add(id);
        }

        public void AddEntryTime(double time)
        {
            if (_entryTimes == null) return;
            if (time < 0) time = 0.0;
            _entryTimes.Add(time);
        }

        public void AddBaudRate(double baudRate)
        {
            _baudRates?.Add(baudRate);

            if (_bandwidth < 0 || baudRate < _bandwidth)
            {
                _bandwidth = baudRate;
            }
        }

        public double[] GetDetailBaudRate()
        {
            return _baudRates?.ToArray();
        }

        public int[] GetDetailedHops()
        {
            return _entities?.ToArray();
        }

        public double[] GetDetailedExitTimes()
        {
            return _exitTimes?.ToArray();
        }

        public int GetDestId()
        {
            return _destId;
        }
        
        public int GetSrcId()
        {
            return _srcId;
        }

        public int GetNumHop()
        {
            int PAIR = 2;
            return ((_hopsNumber - PAIR) + 1)/PAIR;
        }

        public double GetTotalResponseTime()
        {
            if (_exitTimes == null || _entryTimes == null) return 0;

            double time = 0;
            try
            {
                double startTime = _exitTimes.First();
                double receiveTime = _entryTimes.Last();
                time = receiveTime - startTime;
            }
            catch (Exception)
            {
                time = 0;
            }

            return time;
        }

        public int Last
        {
            get { return _last; }
            set { _last = value; }
        }

        public int NetServiceType
        {
            get { return _netServiceType; }
            set { _netServiceType = value; }
        }
        
        public int GetTag()
        {
            return _tag;
        }

        public bool SetTag(int tag)
        {
            bool result = false;
            switch (tag)
            {
                case CloudSimTags.INFOPKT_SUBMIT:
                case CloudSimTags.INFOPKT_RETURN:
                    _tag = tag;
                    result = true;
                    break;
            }
            return result;
        }

        public void SetDestId(int id)
        {
            _destId = id;
        }
    }
}
