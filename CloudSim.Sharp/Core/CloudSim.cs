using CloudSim.Sharp.Core.Predicates;
using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Core
{
    public class CloudSim
    {
        private static int NOT_FOUND = -1;

        private static bool _running;

        protected static FutureQueue _futureQueue;
        protected static DeferredQueue _deferedQueue;

        private static double _clock;

        private static List<SimEntity> _entities;

        private static IDictionary<string, SimEntity> _entitiesByName;

        private static IDictionary<int, Predicate> _waitPredicates;
        private static bool _abruptTerminate = false;

        public static bool Running
        {
            get { return _running; }
        }

        public static double Clock
        {
            get { return _clock; }
        }

        public static void Initialize()
        {
            Log.WriteLine("Initialising...");
            _entities = new List<SimEntity>();
            _entitiesByName = new Dictionary<string, SimEntity>();
            _futureQueue = new FutureQueue();
            _deferedQueue = new DeferredQueue();
            _waitPredicates = new Dictionary<int, Predicate>();
            _clock = 0;
            _running = false;
        }

        public static PredicateAny SIM_ANY = new PredicateAny();

        public static PredicateNone SIM_NONE = new PredicateNone();

        public static SimEntity GetEntity(int id)
        {
            return _entities.Find(e => e.Id == id);
        }

        public static SimEntity GetEntity(string name)
        {
            if (!_entitiesByName.ContainsKey(name))
                return null;

            return _entitiesByName[name];
        }

        public static int GetEntityId(string name)
        {
            if (!_entitiesByName.ContainsKey(name))
                return NOT_FOUND;

            return _entitiesByName[name].Id;
        }

        public static string GetEntityName(int? entityId)
        {
            if (entityId.HasValue)
                return GetEntity(entityId.Value)?.Name;

            return null;
        }

        public static ICollection<SimEntity> GetEntityList()
        {
            LinkedList<SimEntity> list = new LinkedList<SimEntity>(_entities);
            return list;
        }

        public static void AddEntity(SimEntity e)
        {
            SimEvent evt;
            if (_running)
            {
                evt = new SimEvent(SimEvent.CREATE, _clock, src:1, dest:0, tag:0, edata:e);
                _futureQueue.AddEvent(evt);
            }
            if (e.Id == -1)
            {
                int id = _entities.Count;
                e.Id = id;
                _entities.Add(e);
                _entitiesByName.Add(e.Name, e);
            }
        }

        public static void AddEntityDynamically(SimEntity e)
        {
            if (e == null)
            {
                throw new ArgumentException("Adding null entity.");
            }
            else
            {
                WriteMessage($"Adding: {e.Name}");
            }
            e.StartEntity();
        }

        private static void ValidateDelay(double delay)
        {
            if (delay < 0)
            {
                throw new ArgumentException("Send delay can't be negative.");
            }
        }

        public static void Send(int src, int dest, double delay, int tag, object data)
        {
            ValidateDelay(delay);
            SimEvent e = new SimEvent(SimEvent.SEND, _clock + delay, src, dest, tag, data);
            _futureQueue.AddEvent(e);
        }

        public static void SendFirst(int src, int dest, double delay, int tag, object data)
        {
            ValidateDelay(delay);
            SimEvent e = new SimEvent(SimEvent.SEND, _clock + delay, src, dest, tag, data);
            _futureQueue.AddEventFirst(e);
        }

        public static void Wait(int src, Predicate p)
        {
            var entity = _entities.Find(e => e.Id == src);

            entity.State = SimEntity.WAITING;
            
            if (p != SIM_ANY)
            {
                _waitPredicates.Add(src, p);
            }
        }

        public static void Pause(int src, double delay)
        {
            SimEvent ev = new SimEvent(SimEvent.HOLD_DONE, _clock + delay, src);
            _futureQueue.AddEvent(ev);
            _entities.Find(e => e.Id == src).State = SimEntity.HOLDING;
        }

        public static SimEvent Cancel(int src, Predicate p)
        {
            SimEvent ev = null;
            IEnumerator<SimEvent> enumerator = _futureQueue.GetEnumerator();
            do
            {
                ev = enumerator.Current;
                if (ev.Source == src && p.Match(ev))
                {
                    _futureQueue.Remove(ev);
                    break;
                }
            } while (enumerator.MoveNext());

            return ev;
        }

        public static bool CancelAll(int src, Predicate p)
        {
            SimEvent ev = null;
            int prevSize = _futureQueue.Size();
            IEnumerator<SimEvent> enumerator = _futureQueue.GetEnumerator();
            do
            {
                ev = enumerator.Current;
                if (ev.Source == src && p.Match(ev))
                {
                    _futureQueue.Remove(ev);
                }
            } while (enumerator.MoveNext());

            // In original CloudSim code on Java
            // the comparison has such representation
            // Not sure whether it is correct,
            // because in case if at least one event
            // will be removed, then prevSize will
            // always be greater than actual size
            return prevSize < _futureQueue.Size();
        }

        private static void ProcessEvent(SimEvent e)
        {
            int dest, src;
            SimEntity destEnt;

            if (e.EventTime < _clock)
            {
                throw new ArgumentException("Past event detected.");
            }

            _clock = e.EventTime;

            switch (e.InternalType)
            {
                case SimEvent.ENULL:
                    throw new ArgumentException("Event has a null type.");

                case SimEvent.CREATE:
                    SimEntity newEntity = (SimEntity)e.Data;
                    AddEntityDynamically(newEntity);
                    break;

                case SimEvent.SEND:
                    dest = e.Destination;
                    if (dest < 0)
                    {
                        throw new ArgumentException("Attempt to send to null entity detected.");
                    }
                    else
                    {
                        int tag = e.Tag;
                        destEnt = _entities.Find(ent => ent.Id == dest);
                        if (destEnt.State == SimEntity.WAITING)
                        {
                            Predicate p;
                            if (_waitPredicates.TryGetValue(dest, out p))
                            {
                                if (p == null || tag == 9999 || p.Match(e))
                                {
                                    destEnt.EventBuffer = (SimEvent)e.Clone();
                                    destEnt.State = SimEntity.RUNNABLE;
                                    _waitPredicates.Remove(dest);
                                } 
                                else
                                {
                                    _deferedQueue.AddEvent(e);
                                }
                            }
                        } 
                        else
                        {
                            _deferedQueue.AddEvent(e);
                        }
                    }
                    break;
                case SimEvent.HOLD_DONE:
                    src = e.Source;
                    if (src < 0)
                    {
                        throw new ArgumentException("Null entity holding.");
                    }
                    else
                    {
                        _entities.Find(ent => ent.Id == src).State = SimEntity.RUNNABLE;
                    }
                    break;
            }
        }

        public static int Waiting(int d, Predicate p)
        {
            int count = 0;
            SimEvent ev;
            IEnumerator<SimEvent> enumerator = _deferedQueue.GetEnumerator();

            do
            {
                var current = enumerator.Current;
                if (current != null)
                {
                    ev = current;
                    if (ev.Destination == d && p.Match(ev))
                    {
                        count++;
                    }
                }
            } while (enumerator.MoveNext());
            return count;
        }

        public static SimEvent Select(int src, Predicate p)
        {
            SimEvent ev = null;
            IEnumerator<SimEvent> enumerator = _deferedQueue.GetEnumerator();
            do
            {
                var current = enumerator.Current;
                if (current != null)
                {
                    ev = current;
                    if (current.Destination == src && p.Match(ev))
                    {
                        _deferedQueue.Remove(ev);
                        break;
                    }
                }
            } while (enumerator.MoveNext());
            return ev;
        }

        public static void AbruptlyTerminate()
        {
            _abruptTerminate = true;
        }

        private static void WriteMessage(string message)
        {
            Log.WriteLine(message);
        }
    }
}
