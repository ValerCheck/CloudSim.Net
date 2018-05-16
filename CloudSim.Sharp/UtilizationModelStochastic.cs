using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CloudSim.Sharp
{
    public class UtilizationModelStochastic : UtilizationModel
    {
        private Random _randomGenerator;

        private Dictionary<double, double> _history;

        public UtilizationModelStochastic()
        {
            History = new Dictionary<double, double>();
            RandomGenerator = new Random();
        }

        public UtilizationModelStochastic(long seed)
        {
            History = new Dictionary<double, double>();
            RandomGenerator = new Random((int)seed);
        }

        public double GetUtilization(double time)
        {
            if (History.ContainsKey(time))
            {
                return History[time];
            }

            double utilization = RandomGenerator.NextDouble();
            History.Add(time, utilization);
            return utilization;
        }

        protected Dictionary<double, double> History
        {
            get { return _history; }
            set { _history = value; }
        }

        public void SaveHistory(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fs, History);
                fs.Close();
            };            
        }

        public void LoadHistory(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fs, History);
                fs.Close();
            };
        }

        public Random RandomGenerator
        {
            get { return _randomGenerator; }
            set { _randomGenerator = value; }
        }
    }
}
