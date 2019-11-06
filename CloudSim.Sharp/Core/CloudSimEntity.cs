using CloudSim.Sharp.Core.Interfaces;
using CloudSim.Sharp.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public abstract class CloudSimEntity : SimEntity
    {
        private static ILogger LOGGER = LoggerFactory.GetLogger(nameof(CloudSimEntity));

        ISimulation _simulation;
        bool _started;
        State _state;
        string _name;
        private SimEvent _buffer;
        long _id;

        public override long Id
        {
            get => _id;
            protected set
            {
                if (_id != value)
                {
                    long id = _id >= 0 ? _id : Simulation.NumEntities;
                    _name = $"{GetType().Name}{id}";
                }
            }
        }

        public SimEvent EventBuffer
        {
            get => _buffer;
            protected set => _buffer = value;
        }

        public override ISimulation Simulation
        {
            get => _simulation;
            protected set { _simulation = value; }
        }

        public override string Name
        {
            get => _name;
            protected set => _name = value;
        }

        public bool Started
        {
            get => _started;
            protected set => _started = value;
        }

        public CloudSimEntity(Simulation simulation)
        {
            Simulation = simulation;
            Id = -1;
            State = State.RUNNABLE;
            Simulation.AddEntity(this);
            Started = false;
        }


        /**
         * {@inheritDoc}.
         * It performs general initialization tasks that are common for every entity
         * and executes the specific entity startup code by calling {@link #startEntity()}.
         *
         * @see #startEntity()
         */
        public override void Start()
        {
            StartEntity();
            Started = true;
        }

        public override void ShutdownEntity() => State = State.FINISHED;

        protected abstract void StartEntity();


        public override bool Schedule(SimEntity dest, double delay, int tag, object data)
        {
            return Schedule(new CloudSimEvent(delay, this, dest, tag, data));
        }


        public override bool Schedule(double delay, int tag, object data)
        {
            return Schedule(this, delay, tag, data);
        }


        public override bool Schedule(double delay, int tag)
        {
            return Schedule(this, delay, tag, null);
        }


        public bool Schedule(SimEntity dest, double delay, int tag)
        {
            return Schedule(dest, delay, tag, null);
        }


        public override bool Schedule(int tag, object data)
        {
            return Schedule(this, 0, tag, data);
        }


        public override bool Schedule(SimEvent evt)
        {
            if (!CanSendEvent(evt))
            {
                return false;
            }
            Simulation.Send(evt);
            return true;
        }

        private bool CanSendEvent(SimEvent evt)
        {
            /**
             * If the _simulation has finished and an  {@link CloudSimTags#END_OF__simulation}
             * message is sent, it has to be processed to enable entities to shutdown.
             */
            if (!Simulation.IsRunning && evt.Tag != CloudSimTags.END_OF_SIMULATION)
            {
                LOGGER.Warn(
                    "{0}: {1}: Cannot send events before _simulation starts or after it finishes. Trying to send message {2} to {3}",
                    Simulation.ClockStr, this, evt.Tag, evt.Destination);
                return false;
            }

            return true;
        }

        public void ScheduleNow(SimEntity dest, int tag, object data)
        {
            Schedule(dest, 0, tag, data);
        }

        public void ScheduleNow(SimEntity dest, int tag)
        {
            Schedule(dest, 0, tag, null);
        }

        /**
         * Sends a high priority event to another entity with no delay.
         *
         * @param dest the destination entity
         * @param tag  An user-defined number representing the type of event.
         * @param data The data to be sent with the event.
         */
        public void ScheduleFirstNow(SimEntity dest, int tag, object data)
        {
            ScheduleFirst(dest, 0, tag, data);
        }

        /**
         * Sends a high priority event to another entity with <b>no</b> attached data and no delay.
         * @param dest the destination entity
         * @param tag  An user-defined number representing the type of event.
         */
        public void ScheduleFirstNow(SimEntity dest, int tag)
        {
            ScheduleFirst(dest, 0, tag, null);
        }

        /**
         * Sends a high priority event to another entity and with <b>no</b> attached data.
         *
         * @param dest  the destination entity
         * @param delay How many seconds after the current _simulation time the event should be sent
         * @param tag   An user-defined number representing the type of event.
         */
        public void scheduleFirst(SimEntity dest, double delay, int tag)
        {
            ScheduleFirst(dest, delay, tag, null);
        }

        /**
         * Sends a high priority event to another entity.
         *
         * @param dest  the destination entity
         * @param delay How many seconds after the current _simulation time the event should be sent
         * @param tag   An user-defined number representing the type of event.
         * @param data  The data to be sent with the event.
         */
        public void ScheduleFirst(SimEntity dest, double delay, int tag, object data)
        {
            CloudSimEvent evt = new CloudSimEvent(delay, this, dest, tag, data);
            if (!CanSendEvent(evt))
            {
                return;
            }

            Simulation.SendFirst(evt);
        }

        /**
         * Sets the entity to be inactive for a time period.
         *
         * @param delay the time period for which the entity will be inactive
         */
        public void Pause(double delay)
        {
            if (delay < 0)
            {
                throw new ArgumentException("Negative delay supplied.");
            }

            if (!Simulation.IsRunning)
            {
                return;
            }

            Simulation.PauseEntity(this, delay);
        }

        /**
         * Extracts the first event matching a predicate waiting in the entity's
         * deferred queue.
         *
         * @param predicate The event selection predicate
         * @return the _simulation event; or {@link SimEvent#NULL} if not found or the _simulation is not running
         */
        public SimEvent SelectEvent(Predicate<SimEvent> predicate)
        {
            if (!Simulation.IsRunning)
            {
                return SimEvent.NULL;
            }

            return Simulation.Select(this, predicate);
        }

        /**
         * Cancels the first event from the future event queue that matches a given predicate
         * and that was submitted by this entity, then removes it from the queue.
         *
         * @param predicate the event selection predicate
         * @return the removed event or {@link SimEvent#NULL} if not found
         */
        public SimEvent cancelEvent(Predicate<SimEvent> predicate)
        {
            return Simulation.IsRunning ? Simulation.Cancel(this, predicate) : SimEvent.NULL;
        }

        /**
         * Gets the first event matching a predicate from the deferred queue, or if
         * none match, wait for a matching event to arrive.
         *
         * @param predicate The predicate to match
         * @return the _simulation event; or {@link SimEvent#NULL} if not found or the _simulation is not running
         */
        public SimEvent GetNextEvent(Predicate<SimEvent> predicate)
        {
            if (!Simulation.IsRunning)
            {
                return SimEvent.NULL;
            }

            return SelectEvent(predicate);
        }

        /**
         * Gets the first event waiting in the entity's deferred queue, or if there
         * are none, wait for an event to arrive.
         *
         * @return the _simulation event; or {@link SimEvent#NULL} if not found or the _simulation is not running
         */
        public SimEvent GetNextEvent()
        {
            return GetNextEvent(Simulation.ANY_EVT);
        }

        /**
         * Waits for an event matching a specific predicate. This method does not
         * check the entity's deferred queue.
         *
         * @param predicate The predicate to match
         */
        public void waitForEvent(Predicate<SimEvent> predicate)
        {
            if (!Simulation.IsRunning)
            {
                return;
            }

            Simulation.Wait(this, predicate);
            State = State.WAITING;
        }


        public override void Run()
        {
            Run(double.MaxValue);
        }

        public void Run(double until)
        {
            SimEvent evt = _buffer == null ? GetNextEvent(e => e.Time <= until) : _buffer;

            while (evt != SimEvent.NULL)
            {
                ProcessEvent(evt);
                if (State != State.RUNNABLE)
                {
                    break;
                }

                evt = GetNextEvent(e => e.Time <= until);
            }

            _buffer = null;
        }

        /**
         * Gets a clone of the entity. This is used when independent replications
         * have been specified as an output analysis method. Clones or backups of
         * the entities are made in the beginning of the _simulation in order to
         * reset the entities for each subsequent replication. This method should
         * not be called by the user.
         *
         * @return A clone of the entity
         * @throws CloneNotSupportedException when the entity doesn't support cloning
         */

        public override object Clone()
        {
            CloudSimEntity copy = (CloudSimEntity)base.Clone();
            copy.Name = _name;
            copy.Simulation = _simulation;
            copy.EventBuffer = null;
            return copy;
        }
        
        // --------------- EVENT / MESSAGE SEND WITH NETWORK DELAY METHODS ------------------

        /**
         * Sends an event/message to another entity by <b>delaying</b> the
         * _simulation time from the current time, with a tag representing the event
         * type.
         *
         * @param dest the destination entity
         * @param delay       How many seconds after the current _simulation time the event should be sent.
         *                    If delay is a negative number, then it will be changed to 0
         * @param cloudSimTag an user-defined number representing the type of an event/message
         * @param data        A reference to data to be sent with the event
         */
        protected void Send(SimEntity dest, double delay, int cloudSimTag, object data)
        {
            dest.RequireNonNull();
            if (dest.Id < 0)
            {
                LOGGER.Error("{0}.send(): invalid entity id {1} for {2}", Name, dest.Id, dest);
                return;
            }

            // if delay is negative, then it doesn't make sense. So resets to 0.0
            if (delay < 0)
            {
                delay = 0;
            }

            if (double.IsInfinity(delay))
            {
                throw new ArgumentException("The specified delay is infinite value");
            }

#warning TO_BE_DONE
            // only considers network delay when sending messages between different entities
            //if (dest.Id != Id)
            //{
            //    delay += getNetworkDelay(getId(), dest.getId());
            //}

            Schedule(dest, delay, cloudSimTag, data);
        }

        /**
         * Sends an event/message to another entity by <b>delaying</b> the
         * _simulation time from the current time, with a tag representing the event
         * type.
         *
         * @param dest    the destination entity
         * @param delay       How many seconds after the current _simulation time the event should be sent.
         *                    If delay is a negative number, then it will be changed to 0
         * @param cloudSimTag an user-defined number representing the type of an
         *                    event/message
         */
        protected void Send(SimEntity dest, double delay, int cloudSimTag)
        {
            Send(dest, delay, cloudSimTag, null);
        }

        /**
         * Sends an event/message to another entity, with a tag representing the
         * event type.
         *
         * @param dest    the destination entity
         * @param cloudSimTag an user-defined number representing the type of an
         *                    event/message
         * @param data        A reference to data to be sent with the event
         */
        protected void SendNow(SimEntity dest, int cloudSimTag, object data)
        {
            Send(dest, 0, cloudSimTag, data);
        }

        /**
         * Sends an event/message to another entity, with a tag representing the
         * event type.
         *
         * @param dest    the destination entity
         * @param cloudSimTag an user-defined number representing the type of an event/message
         */
        protected void SendNow(SimEntity dest, int cloudSimTag)
        {
            Send(dest, 0, cloudSimTag, null);
        }

#warning TO_BE_DONE
        /**
         * Gets the network delay associated to the sent of a message from a given
         * source to a given destination.
         *
         * @param src source of the message
         * @param dst destination of the message
         * @return delay to send a message from src to dst
         */

        //private double getNetworkDelay(long src, long dst)
        //{
        //    return Simulation.getNetworkTopology().getDelay(src, dst);
        //}


        public override bool IsStarted => Started;

        public override bool IsAlive => State != State.FINISHED;

        public override bool IsFinished => State == State.FINISHED;

        public int CompareTo(SimEntity entity)
        {
            return Id.CompareTo(entity.Id);
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            CloudSimEntity that = (CloudSimEntity)obj;

            if (Id != that.Id) return false;
            return Simulation.Equals(that.Simulation);
        }

        public override int GetHashCode()
        {
            int result = Simulation.GetHashCode();
            result = 31 * result + Id.GetHashCode();
            return result;
        }
    }
}
