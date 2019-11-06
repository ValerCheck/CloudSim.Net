using CloudSim.Sharp.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public class MachineNull : Machine
    {
        Lazy<List<ResourceManageable>> _resources = new Lazy<List<ResourceManageable>>(() => new List<ResourceManageable>());
        public override Resource Bw => Resource.NULL;

        public override Resource Ram => Resource.NULL;

        public override Resource Storage => Resource.NULL;

        public override long NumberOfPes => 0;

        public override double Mips => 0;

        public override double TotalMipsCapacity => 0;

        public override ISimulation Simulation => Core.Simulation.NULL;

        public override double LastBusyTime => 0;

        public override bool IsIdle() => true;
        public override long Id { get => 0; set { } }

        public override List<ResourceManageable> Resources => _resources.Value;
    }
}
