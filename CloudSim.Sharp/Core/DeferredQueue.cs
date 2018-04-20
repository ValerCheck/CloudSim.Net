using System;
using System.Collections;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public class DeferredQueue
    {
        private ICollection<SimEvent> _list = new LinkedList<SimEvent>();

        private double _maxTime = -1;

        public void AddEvent(SimEvent newEvent)
        {
            double eventTime = newEvent.EventTime;

            if (eventTime >= _maxTime)
            {
                _list.Add(newEvent);
                _maxTime = eventTime;
                return;
            }

            var enumerator = _list.GetEnumerator();
            SimEvent current = null;
            var listAsLinked = (_list as LinkedList<SimEvent>);

            do
            {
                current = enumerator.Current;
                if (current.EventTime > eventTime)
                {
                    if (listAsLinked != null)
                    {
                        var currentNode = 
                            listAsLinked.Find(current);

                        listAsLinked.AddBefore(currentNode, newEvent);
                        return;
                    }
                }
            }
            while (enumerator.MoveNext());

            _list.Add(newEvent);
        }

        public bool Remove(SimEvent e)
        {
            return _list.Remove(e);
        }

        public IEnumerator<SimEvent> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Size
        {
            get { return _list.Count; }
        }

        public void Clear()
        {
            _list.Clear();
        }
    }
}
