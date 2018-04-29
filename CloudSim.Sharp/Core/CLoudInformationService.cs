using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public class CLoudInformationService : SimEntity
    {
        private readonly ICollection<int> _resList;

        // Advanced Reservatin
        private readonly ICollection<int> _arList;

        private readonly ICollection<int> _gisList;

        public ICollection<int> ResList
        {
            get { return _resList; }
        }

        public ICollection<int> AdvReservList
        {
            get { return _arList; }
        }

        public CLoudInformationService(string name) : base(name)
        {
            _resList = new LinkedList<int>();
            _arList = new LinkedList<int>();
            _gisList = new LinkedList<int>();
        }

        public bool ResourceSupportAR(int? id)
        {
            if (id == null) return false;
            return ResourceSupportAR(id.Value);
        }

        public bool ResourceSupportAR(int id)
        {
            bool flag = false;
            if (id < 0) flag = false;
            else flag = CheckResource(_arList, id);

            return flag;
        }

        public bool ResourceExist(int id)
        {
            bool flag = false;
            if (id < 0) flag = false;
            else flag = CheckResource(_resList, id);

            return flag;
        }

        public bool ResourceExist(int? id)
        {
            if (id == null) return false;
            return ResourceExist(id.Value);
        }
        
        private bool CheckResource(ICollection<int> list, int id)
        {
            bool flag = false;
            if (list == null || id < 0) return flag;

            int? obj = null;
            IEnumerator<int> en = list.GetEnumerator();

            do
            {
                obj = en.Current;
                if (obj != null && obj.Value == id)
                {
                    flag = true;
                    break;
                }
            } while (en.MoveNext());

            return flag;
        }
                
        public override void ShutdownEntity()
        {
            NotifyAllEntity();
        }

        private void NotifyAllEntity()
        {
            Log.WriteConcatLine(Name, ": Notify all CloudSim entities fro shutting down.");

            SignalShutdown(_resList);
            SignalShutdown(_gisList);

            _resList.Clear();
            _gisList.Clear();
        }

        public void SignalShutdown(ICollection<int> list)
        {
            if (list == null) return;

            IEnumerator<int> en = list.GetEnumerator();
            int? obj = null;
            int id = 0;

            do
            {
                obj = en.Current;
                id = obj.Value;
                Send(id, 0, CloudSimTags.END_OF_SIMULATION);
            } while (en.MoveNext());
        }

        protected void ProcessEndSimulation()
        {
            // this should be overriden by the child class
        }

        protected void ProcessOtherEvent(SimEvent e)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;
            var className = GetType().Name;
            if (e == null)
            {                
                Log.WriteConcatLine(className, ".", methodName , "(): Unable to handle a request since the event is null.");
                return;
            }

            Log.WriteLine($"{className}.{methodName}(): Unable to handle a request from {CloudSim.GetEntityName(e.Source)} with event tag = {e.Tag}");
        }

        public override void StartEntity()
        {
            // the method has no effect at the current class
        }

        public override void ProcessEvent(SimEvent e)
        {
            int id = -1;
            switch (e.Tag)
            {
                case CloudSimTags.REGISTER_REGIONAL_GIS:
                    _gisList.Add((int)e.Data);
                    break;
                case CloudSimTags.REQUEST_REGIONAL_GIS:
                    id = (int)e.Data;
                    Send(id, 0, e.Tag, _gisList);
                    break;
                case CloudSimTags.REGISTER_RESOURCE:
                    _resList.Add((int)e.Data);
                    break;
                case CloudSimTags.REGISTER_RESOURCE_AR:
                    _resList.Add((int)e.Data);
                    _arList.Add((int)e.Data);
                    break;
                case CloudSimTags.RESOURCE_LIST:
                    id = (int)e.Data;
                    Send(id, 0, e.Tag, _resList);
                    break;
                case CloudSimTags.RESOURCE_AR_LIST:
                    id = (int)e.Data;
                    Send(id, 0, e.Tag, _arList);
                    break;
                default:
                    ProcessOtherEvent(e);
                    break;
            }
        }
    }
}
