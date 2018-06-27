using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public interface IPacket
    {
        string ToString();
        long Size { get; set; }
        int GetDestId();
        int GetId();
        int GetSrcId();
        int NetServiceType { get; set; }
        int Last { get; set; }
        int GetTag();
    }
}
