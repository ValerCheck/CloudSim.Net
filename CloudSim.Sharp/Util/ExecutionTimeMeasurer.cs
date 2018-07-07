using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Util
{
    public class ExecutionTimeMeasurer
    {
        private static readonly Dictionary<string, long> _executionTimes = new Dictionary<string, long>();

        private static long GetCurrentMillis()
        {
            // Looks like fair C# alternative of Java System.currentTimeMillis
            return DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
        }

        public static void Start(string name)
        {
            ExecutionTimes.Add(name, GetCurrentMillis());
        }

        public static double End(string name)
        {
            double time = (GetCurrentMillis() - ExecutionTimes[name]/1000);
            ExecutionTimes.Remove(name);
            return time;
        }

        public static Dictionary<string, long> ExecutionTimes => _executionTimes;
    }
}
