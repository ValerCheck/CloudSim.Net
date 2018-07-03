using CloudSim.Sharp.Core.Predicates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace CloudSim.Sharp.Core
{
    public class CloudSim
    {
        private static readonly string CLOUDSIM_VERSION_STRING = "3.0";
        private static int _cisId = -1;
        private static int _shutdownId = -1;
        private static CLoudInformationService _cis = null;
        private static int NOT_FOUND = -1;
        private static bool _traceFlag = false;
        private static DateTime _dateTime;
        private static double _terminateAt = -1;
        private static double _minTimeBetweenEvents = 0.1;

        private static void InitCommonVariable(bool traceFlag, int numUser)
        {
            Initialize();
            _traceFlag = traceFlag;

            CloudSimShutdown shutdown = new CloudSimShutdown(nameof(CloudSimShutdown), numUser);
            _shutdownId = shutdown.Id;
        }

        public static void Init(int numUser, bool traceFlag)
        {
            try
            {
                InitCommonVariable(traceFlag, numUser);
                _cis = new CLoudInformationService(nameof(CLoudInformationService));
                _cisId = _cis.Id;
            }
            catch (ArgumentException s)
            {
                Log.WriteLine("CloudSim.init(): The simulation has been terminated due to an unexpected error");
                Log.WriteLine(s.Message);
            }
            catch (Exception e)
            {
                Log.WriteLine("CloudSim.init(): The simulation has been terminated due to an unexpected error");
                Log.WriteLine(e.Message);
            }
        }

        public static void Init(int numUser, Calendar cal, bool traceFlag, double periodBetweenEvents)
        {
            if (periodBetweenEvents <= 0)
            {
                throw new ArgumentException($"The minimal time between events should be positive, but is: {periodBetweenEvents}");
            }

            Init(numUser, traceFlag);
            _minTimeBetweenEvents = periodBetweenEvents;
        }

        public static double StartSimulation()
        {
            Log.WriteLine($"Starting CloudSim version {CLOUDSIM_VERSION_STRING}");
		    try
            {
                double clock = Run();

                // reset all static variables
                _cisId = -1;
                _shutdownId = -1;
                _cis = null;
                _traceFlag = false;

                return clock;
            }
            catch (ArgumentException e) {
                Log.WriteLine(e.StackTrace);
                throw new NullReferenceException("CloudSim.startCloudSimulation() :"
                        + " Error - you haven't initialized CloudSim.");
            }
        }

        private static void ValidateDelay(double delay)
        {
            if (delay < 0)
            {
                throw new ArgumentException("Send delay can't be negative.");
            }
        }

        public static void AbruptlyTerminate()
        {
            _abruptTerminate = true;
        }
        
        public static DateTime GetSimulationDateTime()
        {
            return _dateTime;
        }



        public static void StopSimulation()
        {
		    try {
                RunStop();
            } catch (ArgumentException e) {
                throw new NullReferenceException("CloudSim.stopCloudSimulation() : "
                        + "Error - can't stop Cloud Simulation.");
            }
        }
        
        public static bool TerminateSimulation()
        {
            _running = false;
            WriteMessage("Simulation: Reached termination time.");
            return true;
        }

        public static bool TerminateSimulation(double time)
        {
            if (time <= _clock) return false;
            _terminateAt = time;
            return true;
        }

        public static double GetMinTimeBetweenEvents()
        {
            return _minTimeBetweenEvents;
        }

        public static int CloudInfoServiceEntityId => _cisId;

        public static List<int> CloudResourceList
        {
            get
            {
                return (List<int>) _cis?.ResList;
            }
        } 
        
        protected static FutureQueue _futureQueue;
        protected static DeferredQueue _deferredQueue;
        private static double _clock;
        private static List<SimEntity> _entities;
        private static bool _running;
        private static IDictionary<string, SimEntity> _entitiesByName;
        private static IDictionary<int, Predicate> _waitPredicates;
        private static bool _paused = false;
        private static long _pauseAt = -1;
        private static bool _abruptTerminate = false;

        public static void Initialize()
        {
            Log.WriteLine("Initialising...");
            _entities = new List<SimEntity>();
            _entitiesByName = new Dictionary<string, SimEntity>();
            _futureQueue = new FutureQueue();
            _deferredQueue = new DeferredQueue();
            _waitPredicates = new Dictionary<int, Predicate>();
            _clock = 0;
            _running = false;
            _dateTime = new DateTime();
        }

        public static PredicateAny SIM_ANY = new PredicateAny();
        public static PredicateNone SIM_NONE = new PredicateNone();

        public static double Clock
        {
            get { return _clock; }
            private set { _clock = value; }
        }

        public static int GetNumEntities()
        {
            return _entities.Count;
        }

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
                evt = new SimEvent(SimEvent.CREATE, _clock, src: 1, dest: 0, tag: 0, edata: e);
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

        /*
         * TODO: Review this method
         */
        public static bool RunClockTick()
        {
            SimEntity ent;
            bool queue_empty;

            int entities_size = _entities.Count;

            for (int i = 0; i < entities_size; i++)
            {
                ent = _entities[i];
                if (ent.State == SimEntity.RUNNABLE) ent.Run();
            }

            if (_futureQueue.Size() > 0)
            {
                List<SimEvent> toRemove = new List<SimEvent>();
                IEnumerator<SimEvent> fit = _futureQueue.GetEnumerator();
                queue_empty = false;
                SimEvent first = fit.Current;
                ProcessEvent(first);
                _futureQueue.Remove(first);

                fit = _futureQueue.GetEnumerator();

                bool tryMore = fit.Current != null;
                do
                {
                    SimEvent next = fit.Current;
                    if (next.EventTime == first.EventTime)
                    {
                        ProcessEvent(next);
                        toRemove.Add(next);
                        tryMore = fit.MoveNext();
                    }
                    else
                    {
                        tryMore = false;
                    }
                } while (tryMore);

                _futureQueue.RemoveAll(toRemove);
            }
            else
            {
                queue_empty = true;
                _running = false;
                WriteMessage("Simulation: No more future events");
            }

            return queue_empty;
        }

        public static void RunStop()
        {
            WriteMessage("Simulation completed.");
        }

        public static void Hold(int src, long delay)
        {
            SimEvent e = new SimEvent(SimEvent.HOLD_DONE, _clock + delay, src);
            _futureQueue.AddEvent(e);
            _entities[src].State = SimEntity.HOLDING;
        }

        public static void Pause(int src, double delay)
        {
            SimEvent ev = new SimEvent(SimEvent.HOLD_DONE, _clock + delay, src);
            _futureQueue.AddEvent(ev);
            _entities.Find(e => e.Id == src).State = SimEntity.HOLDING;
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

        public static int Waiting(int d, Predicate p)
        {
            int count = 0;
            SimEvent ev;
            IEnumerator<SimEvent> enumerator = _deferredQueue.GetEnumerator();

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
            IEnumerator<SimEvent> enumerator = _deferredQueue.GetEnumerator();
            do
            {
                var current = enumerator.Current;
                if (current != null)
                {
                    ev = current;
                    if (current.Destination == src && p.Match(ev))
                    {
                        _deferredQueue.Remove(ev);
                        break;
                    }
                }
            } while (enumerator.MoveNext());
            return ev;
        }

        public static SimEvent FindFirstDeferred(int src, Predicate p)
        {
            SimEvent ev = null;
            IEnumerator<SimEvent> iter = _deferredQueue.GetEnumerator();
            do
            {
                ev = iter.Current;
                if (ev.Destination == src && p.Match(ev)) break;
            } while (iter.MoveNext());
            return ev;
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
                                    _deferredQueue.AddEvent(e);
                                }
                            }
                        }
                        else
                        {
                            _deferredQueue.AddEvent(e);
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

        public static void RunStart()
        {
            _running = true;
            foreach (SimEntity ent in _entities)
            {
                ent.StartEntity();
            }

            WriteMessage("Entities started.");
        }
        public static bool Running
        {
            get { return _running; }
        }

        public static bool PauseSimulation()
        {
            _paused = true;
            return _paused;
        }

        public static bool PauseSimulation(long time)
        {
            if (time <= _clock)
            {
                return false;
            }
            else
            {
                _pauseAt = time;
            }
            return true;
        }

        public static bool ResumeSimulation()
        {
            _paused = false;

            if (_pauseAt <= _clock)
            {
                _pauseAt = -1;
            }

            return _paused;
        }

        public static double Run()
        {
            if (!Running)
            {
                RunStart();
            }
            while (true)
            {
                if (RunClockTick() || _abruptTerminate)
                {
                    break;
                }

                // this block allows termination of simulation at a specific time
                if (_terminateAt > 0.0 && Clock >= _terminateAt)
                {
                    TerminateSimulation();
                    Clock = _terminateAt;
                    break;
                }

                if (_pauseAt != -1
                        && ((_futureQueue.Size() > 0 && _clock <= _pauseAt 
                        && _pauseAt <= _futureQueue.GetEnumerator().Current.EventTime) 
                        || _futureQueue.Size() == 0 && _pauseAt <= _clock))
                {
                    PauseSimulation();
                    _clock = _pauseAt;
                }

                while (_paused)
                {
                    try
                    {
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine(e.StackTrace);
                    }
                }
            }

            double clock = Clock;

            FinishSimulation();
            RunStop();

            return clock;
        }

        public static void FinishSimulation()
        {
            if (!_abruptTerminate)
            {
                foreach (SimEntity ent in _entities)
                {
                    if (ent.State != SimEntity.FINISHED)
                    {
                        ent.Run();
                    }
                }
            }

            foreach (SimEntity ent in _entities)
            {
                ent.ShutdownEntity();
            }

            _entities = null;
            _entitiesByName = null;
            _futureQueue = null;
            _deferredQueue = null;
            _clock = 0L;
            _running = false;

            _waitPredicates = null;
            _paused = false;
            _pauseAt = -1;
            _abruptTerminate = false;
        }

        public static void AbruptallyTerminate()
        {
            _abruptTerminate = true;
        }

        private static void WriteMessage(string message)
        {
            Log.WriteLine(message);
        }

        public static bool IsPaused()
        {
            return _paused;
        }
    }
}
