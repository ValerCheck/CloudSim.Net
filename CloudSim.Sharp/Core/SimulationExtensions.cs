using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public static class SimulationExtensions
    {
        public static bool ANY_EVT(this ISimulation simulation, SimEvent ev) => true;
    }
}
