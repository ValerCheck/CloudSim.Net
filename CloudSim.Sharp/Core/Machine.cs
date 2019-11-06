using CloudSim.Sharp.Core.Interfaces;
using System;

namespace CloudSim.Sharp.Core
{
    public abstract class Machine : Resourceful, IChangeableId
    {
        public virtual long Id { get; set; }

        public static Machine NULL = new MachineNull();

        public abstract Resource Bw { get; }
        public abstract Resource Ram { get; }
        public abstract Resource Storage { get; }
        public abstract long NumberOfPes { get; }
        public abstract double Mips { get; }
        public abstract double TotalMipsCapacity { get; }
        public abstract ISimulation Simulation { get; }


        /**
         * Checks if the Machine has been idle for a given amount of time (in seconds).
         * @param time the time interval to check if the Machine has been idle (in seconds).
         *             If time is zero, it will be checked if the Machine is currently idle.
         *             If it's negative, even if the Machine is idle, it's considered
         *             that it isn't idle enough. This is useful if you don't want to perform
         *             any operation when the machine becomes idle (for instance,
         *             if idle machines might be shut down and a negative value is given,
         *             they won't).
         * @return true if the Machine has been idle as long as the given time,
         *         false if it's active of isn't idle long enough
         */
        public virtual bool IsIdleEnough(double time) => time < 0 ? false : IdleInterval >= time;

        /**
         * Gets the interval interval the Machine has been idle.
         * @return the idle time interval (in seconds) or 0 if the Machine is not idle
         */
        public double IdleInterval => Simulation.Clock - LastBusyTime;

        /**
         * Gets the last time the Machine was running some process.
         * @return the last busy time (in seconds)
         */
        public abstract double LastBusyTime { get; }

        /**
         * Checks if the Machine is currently idle.
         * @return true if the Machine currently idle, false otherwise
         */
        public virtual bool IsIdle() => IdleInterval > 0;

        /**
         * Validates a capacity for a machine resource.
         * @param capacity the capacity to check
         */
        public static void ValidateCapacity(double capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException(nameof(capacity),"Capacity must be greater than zero");
            }
        }
    }
}
