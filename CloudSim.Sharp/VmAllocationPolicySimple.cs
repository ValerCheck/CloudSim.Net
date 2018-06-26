using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class VmAllocationPolicySimple : VmAllocationPolicy
    {

        private Dictionary<String, Host> VmTable { get; set; }

        private Dictionary<String, int> UsedPes { get; set; }

        private List<int> FreePes { get; set; }

        public VmAllocationPolicySimple(List<Host> list) : base(list)
        {

            FreePes = new List<int>();

            foreach (Host host in HostList)
            {
                FreePes.Add(host.GetNumberOfPes());
            }

            VmTable = new Dictionary<String, Host>();
            UsedPes = new Dictionary<String, int>();
        }


        public override bool AllocateHostForVm(Vm vm)
        {
            int requiredPes = vm.NumberOfPes;
            bool result = false;
            int tries = 0;
            List<int> freePesTmp = new List<int>();

            foreach (int freePes in FreePes)
            {
                freePesTmp.Add(freePes);
            }

            if (!VmTable.ContainsKey(vm.Uid))
            {

                do
                {
                    int moreFree = int.MinValue;
                    int idx = -1;

                    for (int i = 0; i < freePesTmp.Count; i++)
                    {
                        if (freePesTmp[i] > moreFree)
                        {
                            moreFree = freePesTmp[i];
                            idx = i;
                        }

                    }

                    Host host = HostList[idx];
                    result = host.VmCreate(vm);

                    if (result)
                    {
                        VmTable.Add(vm.Uid, host);
                        UsedPes.Add(vm.Uid, requiredPes);
                        FreePes[idx] = FreePes[idx] - requiredPes;
                        result = true;
                        break;
                    }
                    else
                    {
                        freePesTmp[idx] = int.MinValue;
                    }

                    tries++;

                } while (!result && tries < FreePes.Count);

            }

            return result;
        }

        public override void DeallocateHostForVm(Vm vm)
        {
            Host host = VmTable[vm.Uid];
            VmTable.Remove(vm.Uid);
            int idx = HostList.IndexOf(host);
            int pes = UsedPes[vm.Uid];
            UsedPes.Remove(vm.Uid);

            if (host != null)
            {
                host.VmDestroy(vm);
                FreePes[idx] = FreePes[idx] + pes;
            }
        }

        public override Host GetHost(Vm vm)
        {
            return VmTable[vm.Uid];
        }

        public override Host GetHost(int vmId, int userId)
        {
            return VmTable[Vm.GetUid(userId, vmId)];
        }

        public override List<Dictionary<String, Object>>
                OptimizeAllocation(List<Vm> vmList)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public override bool AllocateHostForVm(Vm vm, Host host)
        {
            if (host.VmCreate(vm))
            {
                VmTable.Add(vm.Uid, host);

                int requiredPes = vm.NumberOfPes;
                int idx = HostList.IndexOf(host);
                UsedPes.Add(vm.Uid, requiredPes);
                FreePes[idx] = FreePes[idx] - requiredPes;

                Log.FormatLine(
                        "%.2f: VM #" + vm.Id + " has been allocated to the host #"
                        + host.Id,
                        CloudSim.Sharp.Core.CloudSim.Clock);

                return true;
            }

            return false;
        }
    }
}
