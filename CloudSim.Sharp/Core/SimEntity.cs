using CloudSim.Sharp.Core.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public abstract class SimEntity : ICloneable
    {
        public static int RUNNABLE = 0;
        public static int WAITING = 1;
        public static int HOLDING = 2;
        public static int FINISHED = 3;

        private string _name;
        private int _id;
        private SimEvent _eventBuffer;
        private int _state;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public int State
        {
            get { return _state; }
            set { _state = value; }
        }

        public SimEvent EventBuffer
        {
            get { return _eventBuffer; }
            set { _eventBuffer = value; }
        }

        public SimEntity(string name)
        {
            if (name.IndexOf(" ") != -1)
            {
                throw new ArgumentException("Entity names can't contain spaces.");
            }
            _name = name;
            _id = -1;
            _state = RUNNABLE;
            CloudSim.AddEntity(this);
        }

        #region Scheduling methods

        public void Schedule(int dest, double delay, int tag, object data = null)
        {
            if (!CloudSim.Running) return;
            CloudSim.Send(Id, dest, delay, tag, data);
        }
        
        public void Schedule(string dest, double delay, int tag, object data = null)
        {
            Schedule(CloudSim.GetEntityId(dest), delay, tag, data);
        }
        
        public void ScheduleNow(string dest, int tag, object data = null)
        {
            Schedule(dest, 0, tag, data);
        }
        
        public void ScheduleNow(int dest, int tag, object data = null)
        {
            Schedule(dest, 0, tag, data);
        }
        
        public void ScheduleFirst(int dest, double delay, int tag, object data = null)
        {
            if (!CloudSim.Running) return;
            CloudSim.SendFirst(Id, dest, delay, tag, data);
        }

        public void ScheduleFirst(string dest, double delay, int tag, object data = null)
        {
            ScheduleFirst(CloudSim.GetEntityId(dest), delay, tag, data);
        }

        public void ScheduleFirstNow(int dest, int tag, object data = null)
        {
            ScheduleFirst(dest, 0, tag, data);
        }

        public void ScheduleFirstNow(string dest, int tag, object data = null)
        {
            ScheduleFirst(dest, 0, tag, data);
        }

        #endregion

        public void Pause(double delay)
        {
            if (delay < 0)
            {
                throw new ArgumentException("Negative delay supplied.");
            }
            if (!CloudSim.Running) return;
            CloudSim.Pause(Id, delay);
        }
        
        public int NumEventsWaiting(Predicate p)
        {
            return CloudSim.Waiting(Id, p);
        }

        public int NumEventsWaiting()
        {
            return CloudSim.Waiting(Id, CloudSim.SIM_ANY);
        }

        public SimEvent SelectEvent(Predicate p)
        {
            if (!CloudSim.Running) return null;

            return CloudSim.Select(Id, p);
        }

        public SimEvent CancelEvent(Predicate p)
        {
            if (!CloudSim.Running) return null;

            return CloudSim.Cancel(Id, p);
        }

        public SimEvent GetNextEvent(Predicate p)
        {
            if (!CloudSim.Running) return null;

            if (NumEventsWaiting(p) > 0)
            {
                return SelectEvent(p);
            }

            return null;
        }

        public void WaitForEvent(Predicate p)
        {
            if (!CloudSim.Running) return;
            CloudSim.Wait(Id, p);
            State = WAITING;
        }

        public SimEvent GetNextEvent()
        {
            return GetNextEvent(CloudSim.SIM_ANY);
        }

        public virtual void Run()
        {
            SimEvent ev = _eventBuffer != null ?
                _eventBuffer : GetNextEvent();

            while (ev != null)
            {
                ProcessEvent(ev);
                if (State != RUNNABLE) break;
                ev = GetNextEvent();
            }

            _eventBuffer = null;
        }

        public abstract void StartEntity();

        public abstract void ProcessEvent(SimEvent e);

        public abstract void ShutdownEntity();

        #region Send Methods

        public void Send(int entityId, double delay, int cloudSimTag, object data = null)
        {
            if (entityId < 0)
            {
                Log.WriteConcatLine(Name, ".send(): Error - invalid entity id", entityId);
                return;
            }

            if (delay < 0) delay = 0;

            if (double.IsInfinity(delay))
            {
                throw new ArgumentException("The specified delay is infinite value");
            }

            int srcId = Id;

            if (entityId != srcId)
            {
                delay += GetNetworkDelay(srcId, entityId);
            }

            Schedule(entityId, delay, cloudSimTag, data);
        }

        public void Send(string entityName, double delay, int cloudSimTag, object data = null)
        {
            Send(CloudSim.GetEntityId(entityName), delay, cloudSimTag, data);
        }

        public void SendNow(int entityId, double delay, int cloudSimTag, object data = null)
        {
            Send(entityId, 0, cloudSimTag, data);
        }

        public void SendNow(string entityName, double delay, int cloudSimTag, object data = null)
        {
            Send(CloudSim.GetEntityId(entityName), delay, cloudSimTag, data);
        }

        #endregion

        private double GetNetworkDelay(int src, int dst)
        {
            if (NetworkTopology.IsNetworkEnabled)
            {
                return NetworkTopology.GetDelay(src, dst);
            }
            return 0;
        }

        public object Clone()
        {
            SimEntity copy = (SimEntity) MemberwiseClone();
            copy.Name = Name;
            copy.EventBuffer = null;
            return copy;
        }
    }
}
