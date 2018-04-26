using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Network
{
    public interface IGraphReader
    {
        TopologicalGraph ReadGraphFile(string fileName);
    }
}
