using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Vm
{
    public class UtilizationHistoryNull : UtilizationHistory
    {
        public double UtilizationMad { get { return 0; } }
        public double UtilizationMean { get { return 0; } }
        public double getUtilizationVariance() { return 0; }
        public void addUtilizationHistory(double time) {/**/}
        public SortedDictionary<Double, Double> History { get { return new SortedDictionary<double, double>(); } }
        public double getHostCpuUtilization(double time) { return 0; }
        public double powerConsumption(double time) { return 0; }
        public bool IsEnabled => false;
        public void enable() {/**/}
        public void disable() {/**/}
        public int MaxHistoryEntries => 0;
        public Vm Vm => Vm.NULL;
    }
}
