using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public interface IStorage : IStorageInfo, IStorageFileWorker
    {        
        bool IsFull();
        
        bool ReserveSpace(int fileSize);

        bool HasPotentialAvailableSpace(int fileSize);
        
    }
}
