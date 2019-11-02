using CloudSim.Sharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public class CloudInformationService : CloudSimEntity
    {
        private static Logger LOGGER = LoggerFactory.GetLogger(nameof(CloudInformationService));

        private ISet<Datacenter> _datacenterList;
        private ISet<CloudInformationService> _cisList;

        public CloudInformationService(Simulation simulation) : base(simulation)
        {
            _datacenterList = new SortedSet<Datacenter>();
            _cisList = new SortedSet<CloudInformationService>();
        }

        protected override void StartEntity() { }

        public override void ProcessEvent(SimEvent e)
        {
            int id = -1;
            switch (e.Tag)
            {
                case CloudSimTags.REGISTER_REGIONAL_GIS:
                    _cisList.Add((CloudInformationService) e.Data);
                    break;

                case CloudSimTags.REQUEST_REGIONAL_GIS:
                    id = (int)e.Data;
                    Send(id, 0, e.Tag, _cisList);
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
