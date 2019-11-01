using CloudSim.Sharp.Core;
using System;

namespace CloudSim.Sharp
{
    public class Datacenter : SimEntity
    {
        public Datacenter(string name) : base(name)
        {
        }

        public override void ProcessEvent(SimEvent e)
        {
            throw new NotImplementedException();
        }

        public override void ShutdownEntity()
        {
            throw new NotImplementedException();
        }

        public override void StartEntity()
        {
            throw new NotImplementedException();
        }
    }
}
