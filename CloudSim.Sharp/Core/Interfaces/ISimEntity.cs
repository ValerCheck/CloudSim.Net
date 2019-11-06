using System;

namespace CloudSim.Sharp.Core.Interfaces
{
    public enum State { RUNNABLE, WAITING, HOLDING, FINISHED };

    public interface ISimEntity : ICloneable, INameable, IRunnable, IComparable<ISimEntity>
    {
        State State { get; }
        bool IsStarted { get; }
        bool IsAlive { get; }
        bool IsFinished { get; }
        ISimulation Simulation { get; set; }
        void ProcessEvent(SimEvent evt);
        bool Schedule(SimEvent evt);
        bool Schedule(int tag);
        bool Schedule(double delay, int tag, object data);
        bool Schedule(double delay, int tag);
        bool Schedule(ISimEntity dest, double delay, int tag, object data);
        bool Schedule(ISimEntity dest, double delay, int tag);
        bool Schedule(int tag, object data);
        void Start();
        void ShutdownEntity();
        ISimEntity SetName(string name);
    }
}
