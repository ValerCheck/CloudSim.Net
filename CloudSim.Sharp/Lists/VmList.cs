using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Lists
{
    public static class VmList
    {
        public static Vm GetById(this List<Vm> vmList, int id)
        {
            return vmList.FirstOrDefault(vm => vm.Id == id);
        }

        public static Vm GetByIdAndUserId(this List<Vm> vmList, int id, int userId)
        {
            return vmList.FirstOrDefault(vm => vm.Id == id && vm.UserId == userId);
        }
    }
}
