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

        public void SetEventBuffer(SimEvent e)
        {
            _eventBuffer = e;
        }

        public object Clone()
        {
            SimEntity copy = (SimEntity) MemberwiseClone();
            copy.Name = Name;
            copy.SetEventBuffer(null);
            return copy;
        }
    }
}
