using CloudSim.Sharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public abstract class CloudSimEntity : SimEntity
    {
        private static Logger LOGGER = LoggerFactory.GetLogger(nameof(CloudSimEntity));

        Simulation _simulation;
        bool _started;
        State _state;

        /**
         * Creates a new entity.
         *
         * @param _simulation The CloudSim instance that represents the _simulation the Entity is related to
         * @throws IllegalArgumentException when the entity name is invalid
         */
        public CloudSimEntity(_simulation _simulation)
        {
            set_simulation(_simulation);
            setId(-1);
            state = State.RUNNABLE;
            __simulation.addEntity(this);
            _started = false;
        }


        /**
         * {@inheritDoc}.
         * It performs general initialization tasks that are common for every entity
         * and executes the specific entity startup code by calling {@link #startEntity()}.
         *
         * @see #startEntity()
         */
        public void start()
        {
            startEntity();
            this.setStarted(true);
        }

        public void shutdownEntity()
        {
            setState(State.FINISHED);
        }

        /**
         * Defines the logic to be performed by the entity when the _simulation starts.
         */
        protected abstract void startEntity();

        
        public bool schedule( SimEntity dest,  double delay,  int tag,  object data)
        {
            return schedule(new CloudSimEvent(delay, this, dest, tag, data));
        }

        
    public bool schedule( double delay,  int tag,  object data)
        {
            return schedule(this, delay, tag, data);
        }

        
    public bool schedule( double delay,  int tag)
        {
            return schedule(this, delay, tag, null);
        }

        
    public bool schedule( SimEntity dest,  double delay,  int tag)
        {
            return schedule(dest, delay, tag, null);
        }

        
    public bool schedule( int tag,  object data)
        {
            return schedule(this, 0, tag, data);
        }

        
    public bool schedule( SimEvent evt)
        {
            if (!canSendEvent(evt))
            {
                return false;
            }
            _simulation.send(evt);
            return true;
        }

        private bool canSendEvent( SimEvent evt)
        {
            /**
             * If the _simulation has finished and an  {@link CloudSimTags#END_OF__simulation}
             * message is sent, it has to be processed to enable entities to shutdown.
             */
            if (!_simulation.isRunning() && evt.getTag() != CloudSimTags.END_OF__simulation)
            {
                LOGGER.warn(
                    "{}: {}: Cannot send events before _simulation starts or after it finishes. Trying to send message {} to {}",
                    get_simulation().clockStr(), this, evt.getTag(), evt.getDestination());
                return false;
            }

            return true;
        }

        /**
         * Sends an event to another entity with no delay.
         *
         * @param dest the destination entity
         * @param tag  An user-defined number representing the type of event.
         * @param data The data to be sent with the event.
         */
        public void scheduleNow( SimEntity dest,  int tag,  object data)
        {
            schedule(dest, 0, tag, data);
        }

        /**
         * Sends an event to another entity with <b>no</b> attached data and no delay.
         *
         * @param dest the destination entity
         * @param tag  An user-defined number representing the type of event.
         */
        public void scheduleNow( SimEntity dest,  int tag)
        {
            schedule(dest, 0, tag, null);
        }

        /**
         * Sends a high priority event to another entity with no delay.
         *
         * @param dest the destination entity
         * @param tag  An user-defined number representing the type of event.
         * @param data The data to be sent with the event.
         */
        public void scheduleFirstNow( SimEntity dest,  int tag,  object data)
        {
            scheduleFirst(dest, 0, tag, data);
        }

        /**
         * Sends a high priority event to another entity with <b>no</b> attached data and no delay.
         * @param dest the destination entity
         * @param tag  An user-defined number representing the type of event.
         */
        public void scheduleFirstNow( SimEntity dest,  int tag)
        {
            scheduleFirst(dest, 0, tag, null);
        }

        /**
         * Sends a high priority event to another entity and with <b>no</b> attached data.
         *
         * @param dest  the destination entity
         * @param delay How many seconds after the current _simulation time the event should be sent
         * @param tag   An user-defined number representing the type of event.
         */
        public void scheduleFirst( SimEntity dest,  double delay,  int tag)
        {
            scheduleFirst(dest, delay, tag, null);
        }

        /**
         * Sends a high priority event to another entity.
         *
         * @param dest  the destination entity
         * @param delay How many seconds after the current _simulation time the event should be sent
         * @param tag   An user-defined number representing the type of event.
         * @param data  The data to be sent with the event.
         */
        public void scheduleFirst( SimEntity dest,  double delay,  int tag,  object data)
        {
             CloudSimEvent evt = new CloudSimEvent(delay, this, dest, tag, data);
            if (!canSendEvent(evt))
            {
                return;
            }

            _simulation.sendFirst(evt);
        }

        /**
         * Sets the entity to be inactive for a time period.
         *
         * @param delay the time period for which the entity will be inactive
         */
        public void pause( double delay)
        {
            if (delay < 0)
            {
                throw new ArgumentException("Negative delay supplied.");
            }

            if (!_simulation.isRunning())
            {
                return;
            }

            _simulation.pauseEntity(this, delay);
        }

        /**
         * Extracts the first event matching a predicate waiting in the entity's
         * deferred queue.
         *
         * @param predicate The event selection predicate
         * @return the _simulation event; or {@link SimEvent#NULL} if not found or the _simulation is not running
         */
        public SimEvent selectEvent( Predicate<SimEvent> predicate)
        {
            if (!_simulation.isRunning())
            {
                return SimEvent.NULL;
            }

            return _simulation.select(this, predicate);
        }

        /**
         * Cancels the first event from the future event queue that matches a given predicate
         * and that was submitted by this entity, then removes it from the queue.
         *
         * @param predicate the event selection predicate
         * @return the removed event or {@link SimEvent#NULL} if not found
         */
        public SimEvent cancelEvent( Predicate<SimEvent> predicate)
        {
            return _simulation.isRunning() ? _simulation.cancel(this, predicate) : SimEvent.NULL;
        }

        /**
         * Gets the first event matching a predicate from the deferred queue, or if
         * none match, wait for a matching event to arrive.
         *
         * @param predicate The predicate to match
         * @return the _simulation event; or {@link SimEvent#NULL} if not found or the _simulation is not running
         */
        public SimEvent getNextEvent( Predicate<SimEvent> predicate)
        {
            if (!_simulation.isRunning())
            {
                return SimEvent.NULL;
            }

            return selectEvent(predicate);
        }

        /**
         * Gets the first event waiting in the entity's deferred queue, or if there
         * are none, wait for an event to arrive.
         *
         * @return the _simulation event; or {@link SimEvent#NULL} if not found or the _simulation is not running
         */
        public SimEvent getNextEvent()
        {
            return getNextEvent(_simulation.ANY_EVT);
        }

        /**
         * Waits for an event matching a specific predicate. This method does not
         * check the entity's deferred queue.
         *
         * @param predicate The predicate to match
         */
        public void waitForEvent( Predicate<SimEvent> predicate)
        {
            if (!_simulation.isRunning())
            {
                return;
            }

            _simulation.wait(this, predicate);
            state = State.WAITING;
        }

        
    public void run()
        {
            run(Double.MAX_VALUE);
        }

        public void run( double until)
        {
            SimEvent evt = buffer == null ? getNextEvent(e->e.getTime() <= until) : buffer;

            while (evt != SimEvent.NULL)
            {
                processEvent(evt);
                if (state != State.RUNNABLE)
                {
                    break;
                }

                evt = getNextEvent(e->e.getTime() <= until);
            }

            buffer = null;
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
        
    protected  object clone() throws CloneNotSupportedException
        {
             CloudSimEntity copy = (CloudSimEntity) super.clone();
        copy.setName(name);
        copy.set_simulation(_simulation);
        copy.setEventBuffer(null);
        return copy;
    }

    
    public _simulation get_simulation()
    {
        return _simulation;
    }

    
    public  SimEntity set_simulation( _simulation _simulation)
    {
        this._simulation = objects.requireNonNull(_simulation);
        return this;
    }

    
    public SimEntity setName( String name) throws IllegalArgumentException
    {
        if (StringUtils.isBlank(name)) {
            throw new IllegalArgumentException("Entity names cannot be empty.");
        }

        if (name.contains(" ")) {
            throw new IllegalArgumentException("Entity names cannot contain spaces.");
        }

        this.name = name;
        return this;
    }

    
    public State getState()
    {
        return state;
    }

    /**
     * Sets the entity state.
     *
     * @param state the new state
     */
    
    public SimEntity setState( State state)
    {
        this.state = state;
        return this;
    }

    /**
     * Sets the entity id and defines its name based on such ID.
     *
     * @param id the new id
     */
    protected  void setId( int id)
    {
        this.id = id;
        setAutomaticName();
    }

    /**
     * Sets an automatic generated name for the entity.
     */
    private void setAutomaticName()
    {
         long id = this.id >= 0 ? this.id : this._simulation.getNumEntities();
        this.name = String.format("%s%d", getClass().getSimpleName(), id);
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
    protected void send( SimEntity dest, double delay,  int cloudSimTag,  object data)
    {
        objects.requireNonNull(dest);
        if (dest.getId() < 0)
        {
            LOGGER.error("{}.send(): invalid entity id {} for {}", getName(), dest.getId(), dest);
            return;
        }

        // if delay is negative, then it doesn't make sense. So resets to 0.0
        if (delay < 0)
        {
            delay = 0;
        }

        if (Double.isInfinite(delay))
        {
            throw new IllegalArgumentException("The specified delay is infinite value");
        }

        // only considers network delay when sending messages between different entities
        if (dest.getId() != getId())
        {
            delay += getNetworkDelay(getId(), dest.getId());
        }

        schedule(dest, delay, cloudSimTag, data);
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
    protected void send( SimEntity dest,  double delay,  int cloudSimTag)
    {
        send(dest, delay, cloudSimTag, null);
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
    protected void sendNow( SimEntity dest,  int cloudSimTag,  object data)
    {
        send(dest, 0, cloudSimTag, data);
    }

    /**
     * Sends an event/message to another entity, with a tag representing the
     * event type.
     *
     * @param dest    the destination entity
     * @param cloudSimTag an user-defined number representing the type of an event/message
     */
    protected void sendNow( SimEntity dest,  int cloudSimTag)
    {
        send(dest, 0, cloudSimTag, null);
    }

    /**
     * Gets the network delay associated to the sent of a message from a given
     * source to a given destination.
     *
     * @param src source of the message
     * @param dst destination of the message
     * @return delay to send a message from src to dst
     */
    private double getNetworkDelay( long src,  long dst)
    {
        return get_simulation().getNetworkTopology().getDelay(src, dst);
    }

    
    public bool isStarted()
    {
        return started;
    }

    
    public bool isAlive()
    {
        return state != State.FINISHED;
    }

    
    public bool isFinished()
    {
        return state == State.FINISHED;
    }

    /**
     * Defines if the entity has already started or not.
     *
     * @param started the start state to set
     */
    protected void setStarted( bool started)
    {
        this.started = started;
    }

    
    public int compareTo( SimEntity entity)
    {
        return Long.compare(this.getId(), entity.getId());
    }

    
    public bool equals( object object)
    {
        if (this == object) return true;
        if (object == null || getClass() != object.getClass()) return false;

         CloudSimEntity that = (CloudSimEntity)object;

        if (id != that.id) return false;
        return _simulation.equals(that._simulation);
    }

    public override int GetHashCode()
    {
        int result = _simulation.hashCode();
        result = 31 * result + Long.hashCode(id);
        return result;
    }
}
