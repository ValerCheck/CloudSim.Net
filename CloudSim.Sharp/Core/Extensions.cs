using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public static class Extensions
    {
        public static T FirstOrDefault<T>(this List<T> self, T defaultValue)
        {
            if (self.Count == 0) return defaultValue;
            return self[0];
        }
    }
}
