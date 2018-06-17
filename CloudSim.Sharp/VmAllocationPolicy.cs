using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public abstract class VmAllocationPolicy
    {

        private List<Host> HostList { get; set; }

        public VmAllocationPolicy(List<Host> list)
        {
            HostList = list;
        }
        
        public abstract bool AllocateHostForVm(Vm vm);
        
        public abstract bool AllocateHostForVm(Vm vm, Host host);
        
        public abstract List<Dictionary<String, Object>> OptimizeAllocation
            (List<Vm> vmList);
        
        public abstract void DeallocateHostForVm(Vm vm);
        
        public abstract Host GetHost(Vm vm);
        
        public abstract Host GetHost(int vmId, int userId);

    }
}
