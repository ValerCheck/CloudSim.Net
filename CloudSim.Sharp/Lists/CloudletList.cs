using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Lists
{
    public static class CloudletList
    {
        public static T GetById<T>(this List<T> cloudletList, int id) where T : Cloudlet
        {
            return cloudletList.FirstOrDefault(cloudlet => cloudlet.CloudletId == id);
        }

        public static int GetPositionById<T>(List<T> cloudletList, int id) where T : Cloudlet
        {
            int i = 0;
            foreach (T cloudlet in cloudletList)
            {
                if (cloudlet.CloudletId == id)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public static void Sort<T>(List<T> cloudletList) where T : Cloudlet
        {
            cloudletList.Sort(delegate(T a, T b)
            {
                double cla = a.GetCloudletTotalLength();
                double clb = b.GetCloudletTotalLength();
                return cla.CompareTo(clb);
            });
	    }
    }
}
