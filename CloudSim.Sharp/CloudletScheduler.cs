using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public abstract class CloudletScheduler
    {
        public abstract double UpdateVmProcessing(double currentTime, List<double> mipsShare);
        public abstract List<Double> GetCurrentRequestedMips();
        public abstract double GetTotalUtilizationOfCpu(double time);
        public abstract double GetCurrentRequestedUtilizationOfRam();
        public abstract double GetCurrentRequestedUtilizationOfBw();
    }
}
