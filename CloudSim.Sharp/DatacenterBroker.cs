using System.Collections.Generic;
using CloudSim.Sharp.Core;
using CloudSim.Sharp.Lists;

namespace CloudSim.Sharp
{
    public class DatacenterBroker : SimEntity
    {
        protected List<Vm> _vmList;
        protected List<Vm> _vmsCreatedList;
        protected List<Cloudlet> _cloudletList;
        protected List<Cloudlet> _cloudletSubmittedList;
        protected List<Cloudlet> _cloudletReceivedList;
        protected int _cloudletsSubmitted;
        protected int _vmsRequested;
        protected int _vmsAcks;
        protected int _vmsDestroyed;
        protected List<int> _datacenterIdsList;
        protected List<int> _datacenterRequestedIdsList;
        protected Dictionary<int, int> _vmsToDatacenterMap;
        protected Dictionary<int, DatacenterCharacteristics> _datacenterCharacteristicsList;      

        public DatacenterBroker(string name) : base(name)
        {
            VmList = new List<Vm>();
            VmsCreatedList = new List<Vm>();
            CloudletList = new List<Cloudlet>();
            CloudletSubmittedList = new List<Cloudlet>();
            CloudletReceivedList = new List<Cloudlet>();

            _cloudletsSubmitted = 0;
            VmsRequested = 0;
            VmsAcks = 0;
            VmsDestroyed  = 0;

            DatacenterIdsList = new List<int>();
            DatacenterRequestedIdsList = new List<int>();
            VmsToDatacentersMap  = new Dictionary<int, int>();
            DatacenterCharacteristicsList = new Dictionary<int, DatacenterCharacteristics>();
        }

        public void SubmitVmList(List<Vm> list)
        {
            VmList.AddRange(list);
        }

        public void SubmitCloudletList(List<Cloudlet> list)
        {
            CloudletList.AddRange(list);
        }

        public void BindCloudletToVm(int cloudletId, int vmId)
        {
            CloudletList.GetById(cloudletId).VmId  = vmId;
        }

        public override void ProcessEvent(SimEvent e)
        {
            switch (e.Tag)
            {
                // Resource characteristics request
                case CloudSimTags.RESOURCE_CHARACTERISTICS_REQUEST:
                    ProcessResourceCharacteristicsRequest(e);
                    break;
                // Resource characteristics answer
                case CloudSimTags.RESOURCE_CHARACTERISTICS:
                    ProcessResourceCharacteristics(e);
                    break;
                // VM Creation answer
                case CloudSimTags.VM_CREATE_ACK:
                    ProcessVmCreate(e);
                    break;
                // A finished cloudlet returned
                case CloudSimTags.CLOUDLET_RETURN:
                    ProcessCloudletReturn(e);
                    break;
                // if the simulation finishes
                case CloudSimTags.END_OF_SIMULATION:
                    ShutdownEntity();
                    break;
                // other unknown tags are processed by this method
                default:
                    ProcessOtherEvent(e);
                    break;
            }
        }

        protected void ProcessOtherEvent(SimEvent e)
        {
            if (e == null)
            {
                Log.WriteLine($"{Name}.processOtherEvent(): Error - an event is null.");
                return;
            }

            Log.WriteLine($"{Name}.processOtherEvent(): Error - event unknown by this DatacenterBroker.");
        }

        protected void ProcessResourceCharacteristics(SimEvent ev)
        {
            DatacenterCharacteristics characteristics = (DatacenterCharacteristics)ev.Data;
            DatacenterCharacteristicsList.Add(characteristics.Id, characteristics);

            if (DatacenterCharacteristicsList.Count == DatacenterIdsList.Count)
            {
                DatacenterRequestedIdsList = new List<int>();
                CreateVmsInDatacenter(DatacenterIdsList[0]);
            }
        }

        protected void ProcessResourceCharacteristicsRequest(SimEvent e)
        {
            DatacenterIdsList = Core.CloudSim.CloudResourceList;
            DatacenterCharacteristicsList = new Dictionary<int, DatacenterCharacteristics>();

            Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: Cloud Resource List received with " +
                          $"{DatacenterIdsList.Count} resource(s)");

            foreach (int datacenterId in DatacenterIdsList)
            {
                SendNow(datacenterId, CloudSimTags.RESOURCE_CHARACTERISTICS, Id);
            }
        }

        protected void ProcessVmCreate(SimEvent e)
        {
            int[] data = (int[]) e.Data;
            int datacenterId = data[0];
            int vmId = data[1];
            int result = data[2];

            if (result == CloudSimTags.TRUE)
            {
                VmsToDatacentersMap.Add(vmId, datacenterId);
                VmsCreatedList.Add(VmList.GetById(vmId));
                Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: VM #{vmId} has been created" +
                              $" in Datacenter #{datacenterId}, Host #{VmList.GetById(vmId).Host.Id}");
            }
            else
            {
                Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: Creation of VM #{vmId} failed in Datacenter #{datacenterId}");
            }

            IncrementVmsAck();

            if (VmsCreatedList.Count == VmList.Count - VmsDestroyed)
            {
                SubmitCloudlets();
            }
            else
            {
                if (VmsRequested == VmsAcks)
                {
                    foreach (int nextDatacenterId in DatacenterIdsList)
                    {
                        if (!DatacenterRequestedIdsList.Contains(nextDatacenterId))
                        {
                            CreateVmsInDatacenter(nextDatacenterId);
                            return;
                        }
                    }

                    if (VmsCreatedList.Count > 0)
                    {
                        SubmitCloudlets();
                    }
                    else
                    {
                        Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: none of the required VMs could be created. Aborting");
                        FinishExecution();
                    }
                }
            }
        }

        protected void ProcessCloudletReturn(SimEvent e)
        {
            Cloudlet cloudlet = (Cloudlet) e.Data;
            CloudletReceivedList.Add(cloudlet);
            Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: Cloudlet {cloudlet.CloudletId} received");
            _cloudletsSubmitted--;
            if (CloudletList.Count == 0 && _cloudletsSubmitted == 0)
            {
                Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: All Cloudlets executed. Finishing...");
                ClearDatacenters();
                FinishExecution();
            }
            else
            {
                if (CloudletList.Count > 0 && _cloudletsSubmitted == 0)
                {
                    ClearDatacenters();
                    CreateVmsInDatacenter(0);
                }
            }
        }

        protected void CreateVmsInDatacenter(int datacenterId)
        {
            int requestedVms = 0;
            string datacenterName = Core.CloudSim.GetEntityName(datacenterId);
            foreach (Vm vm in VmList)
            {
                if (!VmsToDatacentersMap.ContainsKey(datacenterId))
                {
                    Log.WriteLine($"{Core.CloudSim.Clock}: {Name}: Trying to Create VM #{vm.Id} in {datacenterName}");
                    SendNow(datacenterId, CloudSimTags.VM_CREATE_ACK, vm);
                    requestedVms++;
                }
            }

            DatacenterRequestedIdsList.Add(datacenterId);
            VmsRequested = requestedVms;
            VmsAcks = 0;
        }

        protected void SubmitCloudlets()
        {
            int vmIndex = 0;
            List<Cloudlet> successfullySubmitted = new List<Cloudlet>();
            foreach (Cloudlet cloudlet in CloudletList)
            {
                Vm vm;
                if (cloudlet.VmId == -1)
                {
                    vm = VmsCreatedList[vmIndex];
                }
                else
                {
                    vm = VmsCreatedList.GetById(cloudlet.VmId);
                    if (vm == null)
                    {
                        if (!Log.IsDisabled)
                        {
                            Log.Write($"{Core.CloudSim.Clock}: {Name}: Postponing execution of " +
                                      $"cloudlet {cloudlet.CloudletId}: bount VM not available");
                        }
                        continue;
                    }
                }

                if (!Log.IsDisabled)
                {
                    Log.Write($"{Core.CloudSim.Clock}: {Name}: Sending cloudlet {cloudlet.CloudletId} to VM #{vm.Id}");
                }

                cloudlet.VmId = vm.Id;
                SendNow(VmsToDatacentersMap[vm.Id], CloudSimTags.CLOUDLET_SUBMIT, cloudlet);
                _cloudletsSubmitted++;
                vmIndex = (vmIndex + 1) % VmsCreatedList.Count;
                CloudletSubmittedList.Add(cloudlet);
                successfullySubmitted.Add(cloudlet);
            }

            successfullySubmitted.ForEach(cloudlet => CloudletList.Remove(cloudlet));
        }

        protected void ClearDatacenters()
        {
            foreach (Vm vm in VmsCreatedList)
            {
                Log.Write($"{Core.CloudSim.Clock}: {Name}: Destroying VM #{vm.Id}");
                SendNow(VmsToDatacentersMap[vm.Id], CloudSimTags.VM_DESTROY, vm);
            }

            VmsCreatedList.Clear();
        }

        protected void FinishExecution()
        {
            SendNow(Id, CloudSimTags.END_OF_SIMULATION);
        }

        public override void ShutdownEntity()
        {
            Log.WriteConcatLine(Name, " is shutting down...");
        }

        public override void StartEntity()
        {
            Log.WriteConcatLine(Name, " is starting...");
            Schedule(Id, 0, CloudSimTags.RESOURCE_CHARACTERISTICS_REQUEST);
        }

        public List<Vm> VmList
        {
            get { return _vmList; }
            protected set { _vmList = value; }
        } 

        public List<Cloudlet> CloudletList
        {
            get { return _cloudletList;}
            protected set { _cloudletList = value; }
        } 

        public List<Cloudlet> CloudletSubmittedList
        {
            get { return _cloudletSubmittedList;}
            protected set { _cloudletSubmittedList = value; }
        } 

        public List<Cloudlet> CloudletReceivedList
        {
            get { return _cloudletReceivedList;}
            protected set { _cloudletReceivedList = value; }
        } 

        public List<Vm> VmsCreatedList
        {
            get { return _vmsCreatedList;}
            protected set { _vmsCreatedList = value; }
        } 

        protected int VmsRequested
        {
            get { return _vmsRequested; }
            set { _vmsRequested = value; }
        }

        protected int VmsAcks
        {
            get { return _vmsAcks;}
            set { _vmsAcks = value; }
        }

        protected void IncrementVmsAck()
        {
            _vmsAcks++;
        }

        protected int VmsDestroyed
        {
            get { return _vmsDestroyed;}
            set { _vmsDestroyed = value; }
        }

        protected List<int> DatacenterIdsList
        {
            get { return _datacenterIdsList; }
            set { _datacenterIdsList = value; }
        }

        protected Dictionary<int, int> VmsToDatacentersMap
        {
            get { return _vmsToDatacenterMap;}
            set { _vmsToDatacenterMap = value; }
        } 

        protected Dictionary<int, DatacenterCharacteristics> DatacenterCharacteristicsList
        {
            get { return _datacenterCharacteristicsList;}
            set { _datacenterCharacteristicsList = value; }
        } 

        protected List<int> DatacenterRequestedIdsList
        {
            get { return _datacenterRequestedIdsList; }
            set { _datacenterRequestedIdsList = value; }
        } 
    }
}
