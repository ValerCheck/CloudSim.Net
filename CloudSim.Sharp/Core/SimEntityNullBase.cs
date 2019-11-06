using CloudSim.Sharp.Core.Interfaces;

namespace CloudSim.Sharp.Core
{
    public class SimEntityNullBase : ISimEntity
    {
        public virtual State State { get => State.FINISHED; }
        public virtual bool IsStarted => false;
        public virtual bool IsAlive => false;
        public virtual bool IsFinished => false;
        public virtual ISimulation Simulation { get => Core.Simulation.NULL; set { } }
        public virtual string Name => string.Empty;
        public virtual long Id => -1;
        public virtual object Clone() => new SimEntityNullBase();
        public virtual int CompareTo(ISimEntity other) => other is SimEntityNullBase ? 0 : -1;
        public virtual void ProcessEvent(SimEvent evt) { }
        public virtual void Run() { }
        public virtual void Start() { }
        public virtual void ShutdownEntity() { }
        public virtual bool Schedule(SimEvent evt) => false;
        public virtual bool Schedule(int tag) => false;
        public virtual bool Schedule(double delay, int tag, object data) => false;
        public virtual bool Schedule(double delay, int tag) => false;
        public virtual bool Schedule(ISimEntity dest, double delay, int tag, object data) => false;
        public virtual bool Schedule(ISimEntity dest, double delay, int tag) => false;
        public virtual bool Schedule(int tag, object data) => false;
        public virtual ISimEntity SetName(string name) => this;
    }
}
