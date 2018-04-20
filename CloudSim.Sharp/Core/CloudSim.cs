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

        public static bool Running
        {
            get { return _running; }
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

    }
}
