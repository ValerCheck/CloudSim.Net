using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public interface IStorageInfo
    {
        String Name { get; }
        double Capacity { get; }
        double CurrentSize { get; }
        double MaxTransferRate { get; }
        double AvailableSpace { get; }
        int NumStoredFile { get; }
    }
}
