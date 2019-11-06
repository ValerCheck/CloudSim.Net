using Microsoft.Extensions.Logging;
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

        public static void Warn(this ILogger logger, string message, params object[] args)
        {
            logger.LogWarning(new EventId(0, "warn"), message, args);
        }

        public static void Error(this ILogger logger, string message, params object[] args)
        {
            logger.LogError(message, args);
        }

        public static void RequireNonNull(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("This object cannot be null");
        }
    }
}
