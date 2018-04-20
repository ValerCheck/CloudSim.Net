using System.Collections;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public class FutureQueue : IEnumerable<SimEvent>
    {
        private SortedSet<SimEvent> _sortedSet = new SortedSet<SimEvent>();

        private long _serial = 0;

        public void AddEvent(SimEvent newEvent)
        {
            newEvent.SetSerial(_serial++);
            _sortedSet.Add(newEvent);
        }

        public void AddEventFirst(SimEvent newEvent)
        {
            newEvent.SetSerial(0);
            _sortedSet.Add(newEvent);
        }
        
        public int Size()
        {
            return _sortedSet.Count;
        }

        public bool Remove(SimEvent ev)
        {
            return _sortedSet.Remove(ev);
        }

        public bool RemoveAll(ICollection<SimEvent> events)
        {
            bool result = false;

            foreach (var ev in events)
                result |= Remove(ev);
            
            return result;
        }

        public void Clear()
        {
            _sortedSet.Clear();
        }

        public IEnumerator<SimEvent> GetEnumerator()
        {
            return ((IEnumerable<SimEvent>)_sortedSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<SimEvent>)_sortedSet).GetEnumerator();
        }
    }
}
