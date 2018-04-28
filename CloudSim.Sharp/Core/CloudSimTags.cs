using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public class CloudSimTags
    {
        private const int BASE = 0;
        private const int NETBASE = 100;
        public const int TRUE = 1;
        public const int FALSE = 0;
        public const int DEFAULT_BAUD_RATE = 9600;
        public const double SCHEDULE_NOW = 0;
        public const int END_OF_SIMULATION = -1;
        public const int ABRUPT_END_OF_SIMULATION = -1;
        public const int INSIGNIFICANT = BASE + 0;
        public const int EXPERIMENT = BASE + 1;
        public const int REGISTER_RESOURCE = BASE + 2;
        public const int REGISTER_RESOURCE_AR = BASE + 3;
        public const int RESOURCE_LIST = BASE + 4;
        public const int RESOURCE_AR_LIST = BASE + 5;
        public const int RESOURCE_CHARACTERISTICS = BASE + 6;
        public const int RESOURCE_DYNAMICS = BASE + 7;
        public const int RESOURCE_NUM_PE = BASE + 8;
        public const int RESOURCE_NUM_FREE_PE = BASE + 9;
        public const int RECORD_STATISTICS = BASE + 10;
        public const int RETURN_STAT_LIST = BASE + 11;
        public const int RETURN_ACC_STATISTICS_BY_CATEGORY = BASE + 12;
        public const int REGISTER_REGIONAL_GIS = BASE + 13;
        public const int REQUEST_REGIONAL_GIS = BASE + 14;
        public const int RESOURCE_CHARACTERISTICS_REQUEST = BASE + 15;
        public const int INFOPKT_SUBMIT = NETBASE + 5;
        public const int INFOPKT_RETURN = NETBASE + 6;
        public const int CLOUDLET_SUBMIT = BASE + 21;
        public const int CLOUDLET_SUBMIT_ACK = BASE + 22;
        public const int CLOUDLET_CANCEL = BASE + 23;
        public const int CLOUDLET_STATUS = BASE + 24;
        public const int CLOUDLET_PAUSE = BASE + 25;
        public const int CLOUDLET_PAUSE_ACK = BASE + 26;
        public const int CLOUDLET_RESUME = BASE + 27;
        public const int CLOUDLET_RESUME_ACK = BASE + 28;
        public const int CLOUDLET_MOVE = BASE + 29;
        public const int CLOUDLET_MOVE_ACK = BASE + 30;
        public const int VM_CREATE = BASE + 31;
        public const int VM_CREATE_ACK = BASE + 32;
        public const int VM_DESTROY = BASE + 33;
        public const int VM_DESTORY_ACK = BASE + 34;
        public const int VM_MIGRATE = BASE + 35;
        public const int VM_MIGRATE_ACK = BASE + 36;
        public const int VM_DATA_ADD = BASE + 37;
        public const int VM_DATA_ADD_ACK = BASE + 38;
        public const int VM_DATA_DEL = BASE + 39;
        public const int VM_DATA_DEL_ACK = BASE + 40;
        public const int VM_DATACENTER_EVENT = BASE + 41;
        public const int VM_BROKER_EVENT = BASE + 42;
        public const int Network_Event_UP = BASE + 43;
        public const int Network_Event_send = BASE + 44;
        public const int RESOURCE_Register = BASE + 45;
        public const int Network_Event_DOWN = BASE + 46;
        public const int Network_Event_Host = BASE + 47;
        public const int NextCycle = BASE + 48;

        private CloudSimTags()
        {
            throw new NotSupportedException("CloudSimTags cannot be instantiated");
        }
    }
}
