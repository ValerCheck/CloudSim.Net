using CloudSim.Sharp.Core.Interfaces;
using CloudSim.Sharp.Core.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public abstract class SimEntity : ISimEntity
    {
        public static ISimEntity NULL = new SimEntityNullBase();

        public long Id { get; private set; }

        public virtual string Name { get; private set; }

        public virtual State State { get; set; }

        public abstract bool IsStarted { get; }

        public abstract bool IsAlive { get; }

        public abstract bool IsFinished { get; }

        public abstract ISimulation Simulation { get; set; }

        public abstract void ProcessEvent(SimEvent evt);

        public abstract bool Schedule(SimEvent evt);
        public abstract bool Schedule(int tag);
        public abstract bool Schedule(double delay, int tag, object data);
        public abstract bool Schedule(double delay, int tag);
        public abstract bool Schedule(ISimEntity dest, double delay, int tag, object data);
        public abstract bool Schedule(ISimEntity dest, double delay, int tag);
        public abstract bool Schedule(int tag, object data);

        public abstract void Run();

        public abstract void Start();
        public abstract void ShutdownEntity();
        public abstract ISimEntity SetName(string name);
        public abstract int CompareTo(SimEntity other);
        public abstract object Clone();
    }
}
