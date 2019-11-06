using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core.Logging
{
    public class LoggerFactory
    {
        public static ILogger GetLogger(string targetName)
        {
            return new Logger();
        }
    }
}
