using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Provisioners
{
    public class PeProvisionerSimple : PeProvisioner
    {
        private Dictionary<string, List<double>> _peTable;

        public PeProvisionerSimple(double availableMips) : base(availableMips)
        {

        }

        public override bool AllocateMipsForVm(string vmUid, double mips)
        {
            
        }

        public override bool AllocateMipsForVm(Vm vm, double mips)
        {
            throw new NotImplementedException();
        }

        public override void DeallocateMipsForVm(Vm vm)
        {
            if (PeTable.ContainsKey(vm.Uid))
            {

            }
        }

        public override List<double> GetAllocatedMipsForVm(Vm vm)
        {
            throw new NotImplementedException();
        }

        public override double GetAllocatedMipsForVmByVirtualPeId(Vm vm, int peId)
        {
            throw new NotImplementedException();
        }

        public override double GetTotalAllocatedMipsForVm(Vm vm)
        {
            throw new NotImplementedException();
        }

        protected Dictionary<string, List<double>> PeTable
        {
            get { return _peTable; }
            set { _peTable = value; }
        }
    }
}
