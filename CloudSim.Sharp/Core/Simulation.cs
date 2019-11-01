using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public abstract class Simulation : ISimulation
    {
        public static ISimulation NULL = new SimulationNull();
        public abstract bool IsTerminationTimeSet { get; }
        public abstract double Clock { get; }
        public abstract string ClockStr { get; }
        public abstract double ClockInMinutes { get; }
        public abstract double ClockInHours { get; }

        public abstract CloudInformationService CloudInfoService { get; }

        public abstract List<SimEntity> EntityList { get; }
        public abstract double MinTimeBetweenEvents { get; }
        public abstract int NumEntities { get; }
        public abstract bool IsPaused { get; }
        public abstract bool IsRunning { get; }
        public abstract double LastCloudletProcessingUpdate { get; set; }

        /**
* Defines IDs for a list of {@link ChangeableId} entities that don't
* have one already assigned. Such entities can be a {@link Cloudlet},
* {@link Vm} or any object that implements {@link ChangeableId}.
*
* @param <T> the type of entities to define an ID
* @param list list of objects to define an ID
* @return the last entity that had an id set
*/
        public static T SetIdForEntitiesWithoutOne<T>(List<T> list) where T : IChangeableId
        {
            return SetIdForEntitiesWithoutOne(list, default);
        }

        /**
         * Defines IDs for a list of {@link ChangeableId} entities that don't
         * have one already assigned. Such entities can be a {@link Cloudlet},
         * {@link Vm} or any object that implements {@link ChangeableId}.
         *
         * @param <T> the type of entities to define an ID
         * @param list list of objects to define an ID
         * @param lastEntity the last created Entity which its ID will be used
         *        as the base for the next IDs
         * @return the last entity that had an id set
         */
        public static T SetIdForEntitiesWithoutOne<T>(List<T> list, T lastEntity) where T : IChangeableId
        {
            if (list?.Count == 0)
            {
                return lastEntity;
            }

            long id = lastEntity == null ? list[list.Count - 1].Id : lastEntity.Id;
            //if the ID is a negative number lower than -1, it's set as -1 to start the first ID as 0
            id = Math.Max(id, -1);
            T entity = lastEntity;
            for (int i = 0; i < list.Count; i++)
            {
                entity = list[i];
                if (entity.Id < 0)
                    entity.Id = ++id;

                if (entity is VmGroup vmGroup)
                {
                    entity = (T)SetIdForEntitiesWithoutOne(vmGroup.VmList, entity);
                    id = entity.Id;
                }
            }
            return entity;
        }

        public abstract void Abort();
        public abstract void AddEntity(CloudSimEntity e);

        public virtual ISimulation AddOnClockTickListener(EventHandler<EventArgs> listener) => this;

        public virtual ISimulation AddOnEventProcessingListener(EventHandler<SimEvent> listener) => this;

        public virtual ISimulation AddOnSimulationPauseListener(EventHandler<EventArgs> listener) => this;

        public virtual ISimulation AddOnSimulationStartListener(EventHandler<EventArgs> listener) => this;

        public Predicate<SimEvent> ANY_EVT(SimEvent ev) => e => true;

        public abstract SimEvent Cancel(SimEntity src, Predicate<SimEvent> p);
        public abstract bool CancelAll(SimEntity src, Predicate<SimEvent> p);
        public abstract SimEvent FindFirstDeferred(SimEntity dest, Predicate<SimEvent> p);
        public abstract long GetNumberOfFutureEvents(Predicate<SimEvent> predicate);
        public abstract bool IsTimeToTerminateSimulationUnderRequest();
        public abstract bool Pause();
        public abstract bool Pause(double time);
        public abstract void PauseEntity(SimEntity src, double delay);
        public abstract bool RemoveOnClockTickListener(EventHandler<EventArgs> listener);
        public abstract bool RemoveOnEventProcessingListener(EventHandler<SimEvent> listener);
        public abstract bool RemoveOnSimulationPauseListener(EventHandler<EventArgs> listener);
        public abstract bool Resume();
        public abstract double RunFor(double interval);
        public abstract SimEvent Select(SimEntity dest, Predicate<SimEvent> p);
        public abstract void Send(SimEvent evt);
        public abstract void Send(SimEntity src, SimEntity dest, double delay, int tag, object data);
        public abstract void SendFirst(SimEvent evt);
        public abstract void SendFirst(SimEntity src, SimEntity dest, double delay, int tag, object data);
        public abstract void SendNow(SimEntity src, SimEntity dest, int tag, object data);
        public abstract double Start();
        public abstract void StartSync();
        public abstract bool Terminate();
        public abstract bool TerminateAt(double time);
        public abstract void Wait(CloudSimEntity src, Predicate<SimEvent> p);
    }
}
