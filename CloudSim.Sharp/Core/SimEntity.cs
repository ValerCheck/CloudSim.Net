using CloudSim.Sharp.Core.Interfaces;
using CloudSim.Sharp.Core.Predicates;
using System;

namespace CloudSim.Sharp.Core
{
    public abstract class SimEntity : ISimEntity
    {
        private static Lazy<ISimEntity> _nullEntity = 
            new Lazy<ISimEntity>(() => new SimEntityNullBase());

        public static ISimEntity NULL => _nullEntity.Value;

        public virtual long Id { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual State State { get; protected set; }
        public abstract bool IsStarted { get; }
        public abstract bool IsAlive { get; }
        public abstract bool IsFinished { get; }
        public abstract ISimulation Simulation { get; protected set; }
        public abstract void ProcessEvent(SimEvent evt);
        public abstract bool Schedule(SimEvent evt);
        public abstract bool Schedule(int tag);
        public abstract bool Schedule(double delay, int tag, object data);
        public abstract bool Schedule(double delay, int tag);
        public abstract bool Schedule(ISimEntity dest, double delay, int tag, object data);
        public abstract bool Schedule(ISimEntity dest, double delay, int tag);
        public abstract bool Schedule(SimEntity dest, double delay, int tag, object data);
        public abstract bool Schedule(int tag, object data);
        public abstract void Run();
        public abstract void Start();
        public abstract void ShutdownEntity();
        public abstract ISimEntity SetName(string name);
        public abstract int CompareTo(ISimEntity other);
        public virtual object Clone() => this.MemberwiseClone();
    }
}
