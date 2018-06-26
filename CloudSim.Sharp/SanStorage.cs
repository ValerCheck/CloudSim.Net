using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class SanStorage : HarddriveStorage, IStorage
    {

        private readonly double _bandwidth;

        private readonly double _networkLatency;

        public SanStorage(double capacity, double bandwidth,
            double networkLatency) : base(capacity)
        {

            _bandwidth = bandwidth;
            _networkLatency = networkLatency;
        }

        public SanStorage(String name, double capacity, double bandwidth,
            double networkLatency) : base(name, capacity)
        {

            _bandwidth = bandwidth;
            _networkLatency = networkLatency;
        }

        public override double AddReservedFile(File file)
        {
            double time = base.AddReservedFile(file);
            time += _networkLatency;
            time += file.Size * _bandwidth;

            return time;
        }

        public override double MaxTransferRate
        {
            get
            {
                double diskRate = base.MaxTransferRate;

                if (diskRate < _bandwidth)
                {
                    return diskRate;
                }
                return _bandwidth;
            }
        }

        public override double AddFile(File file)
        {
            double time = base.AddFile(file);

            time += _networkLatency;
            time += file.Size * _bandwidth;

            return time;
        }

        public override double AddFile(List<File> list)
        {
            double result = 0.0;
            if (list == null || list.Count == 0)
            {
                Log.WriteConcatLine(Name, ".addFile(): Warning - list is empty.");
                return result;
            }
            list.ForEach(x => result += AddFile(x));

            return result;
        }

        public override double DeleteFile(String fileName, File file)
        {
            return DeleteFile(file);
        }

        public override double DeleteFile(File file)
        {
            double time = base.DeleteFile(file);

            time += _networkLatency;
            time += file.Size * _bandwidth;

            return time;
        }

    }
}
