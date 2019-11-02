using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core.Interfaces
{
    public interface ISimulation
    {
        Predicate<SimEvent> ANY_EVT(SimEvent ev);

        bool IsTerminationTimeSet { get; }

        /**
         * Aborts the simulation without finishing the processing
         * of entities in the {@link #getEntityList() entities list}, <b>which may give
         * unexpected results</b>.
         * <p><b>Use this method just if you want to abandon the simulation an usually ignore the results.</b></p>
         */
        void Abort();

        /**
         * Adds a new entity to the simulation. Each {@link CloudSimEntity} object
         * register itself when it is instantiated.
         *
         * @param e The new entity
         */
        void AddEntity(CloudSimEntity e);

        /**
         * Cancels the first event from the future event queue that matches a given predicate
         * and was sent by a given entity, then removes it from the queue.
         *
         * @param src Id of entity that scheduled the event
         * @param p   the event selection predicate
         * @return the removed event or {@link SimEvent#NULL} if not found
         */
        SimEvent Cancel(SimEntity src, Predicate<SimEvent> p);

        /**
         * Cancels all events from the future event queue that matches a given predicate
         * and were sent by a given entity, then removes those ones from the queue.
         *
         * @param src Id of entity that scheduled the event
         * @param p   the event selection predicate
         * @return true if at least one event has been cancelled; false otherwise
         */
        bool CancelAll(SimEntity src, Predicate<SimEvent> p);

        /**
         * Gets the current simulation time in seconds.
         *
         * @return
         * @see #isRunning()
         */
        double Clock { get; }

        string ClockStr { get; }

        /**
         * Gets the current simulation time in minutes.
         *
         * @return
         * @see #isRunning()
         */
        double ClockInMinutes { get; }

        double ClockInHours { get; }

        /**
         * Find first deferred event matching a predicate.
         *
         * @param dest Id of entity that the event has to be sent to
         * @param p    the event selection predicate
         * @return the first matched event or {@link SimEvent#NULL} if not found
         */
        SimEvent FindFirstDeferred(SimEntity dest, Predicate<SimEvent> p);

        /**
         * Gets the {@link CloudInformationService}.
         *
         * @return the Entity
         */
        CloudInformationService CloudInfoService { get; }

        /**
         * Returns a <b>read-only</b> list of entities created for the simulation.
         *
         * @return
         */
        List<SimEntity> EntityList { get; }

        /**
         * Returns the minimum time between events (in seconds).
         * Events within shorter periods after the last event are discarded.
         *
         * @return the minimum time between events (in seconds).
         */
        double MinTimeBetweenEvents { get; }

        /**
         * Get the current number of entities in the simulation.
         *
         * @return The number of entities
         */
        int NumEntities { get; }

        /**
         * Removes a listener from the onEventProcessingListener List.
         *
         * @param listener the listener to remove
         * @return true if the listener was found and removed, false otherwise
         */
        bool RemoveOnEventProcessingListener(EventHandler<SimEvent> listener);

        /**
         * Adds an {@link EventListener} object that will be notified when the simulation is paused.
         * When this Listener is notified, it will receive an {@link EventInfo} informing
         * the time the pause occurred.
         *
         * <p>This object is just information about the event
         * that happened. In fact, it isn't generated an actual {@link SimEvent} for a pause event
         * because there is not need for that.</p>
         *
         * @param listener the event listener to add
         * @return
         */
        ISimulation AddOnSimulationPauseListener(EventHandler<EventArgs> listener);

        ISimulation AddOnSimulationStartListener(EventHandler<EventArgs> listener);

        /**
         * Removes a listener from the onSimulationPausedListener List.
         *
         * @param listener the listener to remove
         * @return true if the listener was found and removed, false otherwise
         */
        bool RemoveOnSimulationPauseListener(EventHandler<EventArgs> listener);

        /**
         * Adds a {@link EventListener} object that will be notified when any event
         * is processed by CloudSim. When this Listener is notified, it will receive
         * the {@link SimEvent} that was processed.
         *
         * @param listener the event listener to add
         * @return
         */
        ISimulation AddOnEventProcessingListener(EventHandler<SimEvent> listener);

        /**
         * Adds a {@link EventListener} object that will be notified every time when the
         * simulation clock advances. Notifications are sent in a second interval to avoid notification flood.
         * Thus, if the clock changes, for instance, from 1.0, to 1.1, 2.0, 2.1, 2.2, 2.5 and then 3.2,
         * notifications will just be sent for the times 1, 2 and 3 that represent the integer
         * part of the simulation time.
         *
         * @param listener the event listener to add
         * @return
         */
        ISimulation AddOnClockTickListener(EventHandler<EventArgs> listener);

        /**
         * Removes a listener from the onClockTickListener List.
         *
         * @param listener the listener to remove
         * @return true if the listener was found and removed, false otherwise
         */
        bool RemoveOnClockTickListener(EventHandler<EventArgs> listener);

        /**
         * Pauses an entity for some time.
         * @param src   id of entity to be paused
         * @param delay the time period for which the entity will be inactive
         */
        void PauseEntity(SimEntity src, double delay);

        /**
         * Checks if the simulation is paused.
         *
         * @return
         */
        bool IsPaused { get; }

        /**
         * Requests the simulation to be paused as soon as possible.
         *
         * @return true if the simulation was paused, false if it was already paused or has finished
         */
        bool Pause();

        /**
         * Requests the simulation to be paused at a given time.
         * The method schedules the pause request and then returns immediately.
         *
         * @param time the time at which the simulation has to be paused
         * @return true if pause request was successfully received (the given time
         * is greater than or equal to the current simulation time), false otherwise.
         */
        bool Pause(double time);

        /**
         * This method is called if one wants to resume the simulation that has
         * previously been paused.
         *
         * @return true if the simulation has been restarted or false if it wasn't paused.
         */
        bool Resume();

        /**
         * Check if the simulation is still running.
         * Even if the simulation {@link #isPaused() is paused},
         * the method returns true to indicate that the simulation is
         * in fact active yet.
         * <p>
         * This method should be used by
         * entities to check if they should continue executing.
         *
         * @return
         */
        bool IsRunning { get; }

        /**
         * Selects the first deferred event that matches a given predicate
         * and removes it from the queue.
         *
         * @param dest entity that the event has to be sent to
         * @param p    the event selection predicate
         * @return the removed event or {@link SimEvent#NULL} if not found
         */
        SimEvent Select(SimEntity dest, Predicate<SimEvent> p);

        /**
         * Sends an event where all data required is defined inside the event instance.
         * @param evt the event to send
         */
        void Send(SimEvent evt);

        /**
         * Sends an event from one entity to another.
         * @param src  entity that scheduled the event
         * @param dest  entity that the event will be sent to
         * @param delay How many seconds after the current simulation time the event should be sent
         * @param tag   the {@link SimEvent#getTag() tag} that classifies the event
         * @param data  the {@link SimEvent#getData() data} to be sent inside the event
         */
        void Send(SimEntity src, SimEntity dest, double delay, int tag, object data);

        /**
         * Sends an event where all data required is defined inside the event instance,
         * adding it to the beginning of the queue in order to give priority to it.
         * @param evt the event to send
         */
        void SendFirst(SimEvent evt);

        /**
         * Sends an event from one entity to another, adding it to the beginning of the queue in order to give priority to it.
         * @param src  entity that scheduled the event
         * @param dest  entity that the event will be sent to
         * @param delay How many seconds after the current simulation time the event should be sent
         * @param tag   the {@link SimEvent#getTag() tag} that classifies the event
         * @param data  the {@link SimEvent#getData() data} to be sent inside the event
         */
        void SendFirst(SimEntity src, SimEntity dest, double delay, int tag, object data);

        /**
         * Sends an event from one entity to another without delaying
         * the message.
         * @param src  entity that scheduled the event
         * @param dest entity that the event will be sent to
         * @param tag  the {@link SimEvent#getTag() tag} that classifies the event
         * @param data the {@link SimEvent#getData() data} to be sent inside the event
         */
        void SendNow(SimEntity src, SimEntity dest, int tag, object data);

        /**
         * Runs the simulation for a specific period of time and then immediately returns.
         * In order to complete the whole simulation you need to invoke this method multiple times
         *
         * <b>Note:</b> Should be used only in the <b>synchronous</b> mode (after starting the simulation
         * with {@link #startSync()}).
         *
         * @param interval The interval for which the simulation should be run (in seconds)
         * @return Clock at the end of simulation interval (in seconds)
         */
        double RunFor(double interval);

        /**
         * Starts simulation execution and <b>waits for
         * all entities to finish</b>, i.e. until all entities threads reach
         * non-RUNNABLE state or there are no more events in the future event queue.
         * <p>
         * <b>Note</b>: This method should be called only after all the entities
         * have been setup and added. The method blocks until the simulation is ended.
         * </p>
         *
         * @return the last clock time
         * @throws UnsupportedOperationException When the simulation has already run once.
         * If you paused the simulation and wants to resume it,
         * you must use {@link #resume()} instead of calling the current method.
         *
         * @see #startSync()
         */
        double Start();

        /**
         * Starts simulation execution in synchronous mode, retuning immediately. You need
         * to call {@link #runFor(double)} method subsequently to actually process simulation steps.
         *
         * <b>Note</b>: This method should be called only after all the entities
         * have been setup and added. The method returns immediately after preparing the
         * internal state of the simulation.
         * </p>
         *
         * @throws UnsupportedOperationException When the simulation has already run once.
         * If you paused the simulation and wants to resume it,
         * you must use {@link #resume()} instead of calling the current method.
         *
         * @see #runFor(double)
         */
        void StartSync();

        bool IsTimeToTerminateSimulationUnderRequest();

        /**
         * Forces the termination of the simulation before it ends.
         *
         * @return true if the simulation was running and the termination request was accepted,
         * false if the simulation was not started yet
         */
        bool Terminate();

        /**
         * Schedules the termination of the simulation for a given time (in seconds).
         *
         * <p>If a termination time is set, the simulation stays running even
         * if there is no event to process.
         * It keeps waiting for new dynamic events, such as the creation
         * of Cloudlets and VMs at runtime.
         * If no event happens, the clock is increased to simulate time passing.
         * The clock increment is defined according to: (i) the lower {@link Datacenter#getSchedulingInterval()}
         * between existing Datacenters;  or (ii) {@link #getMinTimeBetweenEvents()} in case
         * no {@link Datacenter} has its schedulingInterval set.</p>
         *
         * @param time the time at which the simulation has to be terminated (in seconds)
         * @return true if the time given is greater than the current simulation time, false otherwise
         */
        bool TerminateAt(double time);

        /**
         * Sets the state of an entity to {@link SimEntity.State#WAITING},
         * making it to wait for events that satisfy a given predicate.
         * Only such events will be passed to the entity.
         * This is done to avoid unnecessary context switch.
         *
         * @param src entity that scheduled the event
         * @param p   the event selection predicate
         */
        void Wait(CloudSimEntity src, Predicate<SimEvent> p);

        /**
         * Gets the network topology used for Network simulations.
         *
         * @return
         */
#warning to be done
        #region network
        //NetworkTopology GetNetworkTopology();

        /**
         * Sets the network topology used for Network simulations.
         *
         * @param networkTopology the network topology to set
         */
        //void setNetworkTopology(NetworkTopology networkTopology);

        #endregion
        
        /**
         * Gets the number of events in the future queue
         * which match a given predicate.
         * @param predicate the predicate to filter the list of future events.
         * @return the number of future events which match the predicate
         */
        long GetNumberOfFutureEvents(Predicate<SimEvent> predicate);

        /**
         * Gets or sets the last time (in seconds) some Cloudlet was processed in the simulation.
         */
        double LastCloudletProcessingUpdate { get; set; }
    }
}
