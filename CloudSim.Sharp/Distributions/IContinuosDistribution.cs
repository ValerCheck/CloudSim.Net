using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Distributions
{
    public interface IContinuosDistribution
    {
        double Sample();
    }
}
