using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Lists
{
    public static class ResCloudletList
    {
        public static ResCloudlet GetByIdAndUserId(this List<ResCloudlet> list, int cloudletId, int userId)
        {
            return list.FirstOrDefault(rcl => rcl.GetCloudletId() == cloudletId && rcl.GetUserId() == userId);
        }

        public static int IndexOf(this List<ResCloudlet> list, int cloudletId, int userId)
        {
            int i = 0;
            foreach (ResCloudlet rcl in list)
            {
                if (rcl.GetCloudletId() == cloudletId && rcl.GetUserId() == userId)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public static bool Move(this List<ResCloudlet> listFrom, List<ResCloudlet> listTo, ResCloudlet cloudlet)
        {
            if (listFrom.Remove(cloudlet))
            {
                listTo.Add(cloudlet);
                return true;
            }
            return false;
        }

        public static int GetPositionById(this List<ResCloudlet> list, int id)
        {
            int i = 0;
            foreach (ResCloudlet cloudlet in list)
            {
                if (cloudlet.GetCloudletId() == id)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}
