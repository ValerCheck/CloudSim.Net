using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public class CloudSimShutdown : SimEntity
    {
        private int _numUser;

        public CloudSimShutdown(string name, int numUser) : base(name)
        {
            _numUser = numUser;
        }

        public override void ProcessEvent(SimEvent e)
        {
            _numUser--;
            if (_numUser == 0 || e.Tag == CloudSimTags.ABRUPT_END_OF_SIMULATION)
            {
                CloudSim.AbruptlyTerminate();
            }
        }

        public override void ShutdownEntity()
        {
            // do nothing
        }

        public override void StartEntity()
        {
            // do nothing
        }
    }
}
