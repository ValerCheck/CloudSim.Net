using System;
using System.Text;

namespace CloudSim.Sharp.Core
{
    public class SimEvent : ICloneable, IComparable<SimEvent>
    {
        // Internal event type.
        private int _etype;

        // The time that this event was scheduled, at which it should occur.
        private double _time;

        // Time that the event was removed from the queue to start service.
        private double _endWaitingTime;

        // Id of entity who scheduled the event.
        private int _sourceEntity;

        // Id of entity that the event will be sent to.
        private int _destEntity;

        // The user defined type of the event.
        private int _tag;

        /** 
         * Any data the event is carrying. 
         * @todo I would be used generics to define the type of the event data.
         * But this modification would incur several changes in the simulator core
         * that has to be assessed first.
         **/
        private object _data;

        /**
         * An attribute to help CloudSim to identify the order of received events
         * when multiple events are generated at the same time.
         * If two events have the same {@link #time}, to know
         * what event is greater than other (i.e. that happens after other),
         * the {@link #compareTo(org.cloudbus.cloudsim.core.SimEvent)}
         * makes use of this field.
         */
        private long _serial = -1;

        // Internal event types

        public const int ENULL = 0;

        public const int SEND = 1;

        public const int HOLD_DONE = 2;

        public const int CREATE = 3;

        public SimEvent()
        {
            _etype = ENULL;
            _time = -1L;
            _endWaitingTime = -1.0;
            _sourceEntity = -1;
            _destEntity = -1;
            _tag = -1;
            _data = null;
        }

        public SimEvent(int etype, double time, int src, int dest, int tag, object edata)
        {
            _etype = etype;
            _time = time;
            _sourceEntity = src;
            _destEntity = dest;
            _tag = tag;
            _data = edata;
        }

        public SimEvent(int etype, double time, int src)
        {
            _etype = etype;
            _time = time;
            _sourceEntity = src;
            _destEntity = -1;
            _tag = -1;
            _data = null;
        }

        public int CompareTo(SimEvent ev)
        {
            if (ev == null) return 1;
            else if (_time < ev._time) return -1;
            else if (_time > ev._time) return 1;
            else if (_serial < ev._serial) return -1;
            else if (this == ev) return 0;
            else return 1;
        }

        public int Destination
        {
            get { return _destEntity; }
            set { _destEntity = value; }
        }
        
        public int Source
        {
            get { return _sourceEntity; }
            set { _sourceEntity = value; }
        }

        public double EventTime
        {
            get { return _time; }
        }

        public double EndWaitingTime
        {
            get { return _endWaitingTime; }
        }

        public int InternalType
        {
            get { return _etype; }
        }

        public int Type
        {
            get { return _tag; }
        }

        public int ScheduledBy
        {
            get { return _sourceEntity; }
        }

        public void SetSerial(long serial)
        {
            _serial = serial;
        }

        public int Tag
        {
            get { return _tag; }
        }

        public object Data
        {
            get { return _data; }
        }

        public object Clone()
        {
            return new SimEvent(_etype, _time, _sourceEntity, _destEntity, _tag, _data);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"Event tag = {_tag} ")
                .Append($"source = {CloudSim.GetEntity(_sourceEntity).Name} ")
                .Append($"destination = {CloudSim.GetEntity(_destEntity).Name}");

            return sb.ToString();
        }

    }
}
