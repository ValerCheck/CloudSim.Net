using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Lists
{
    public static class HostList
    {
        public static Host GetById(this List<Host> hostList, int id)
        {
            foreach (Host host in hostList)
            {
                if (host.Id == id)
                {
                    return host;
                }
            }
            return null;
        }

        public static int GetNumberOfPes(this List<Host> hostList)
        {
            int numberOfPes = 0;
            foreach (Host host in hostList)
            {
                numberOfPes += host.PeList.Count;
            }
            return numberOfPes;
        }

        public static int GetNumberOfFreePes(this List<Host> hostList)
        {
            int numberOfFreePes = 0;
            foreach (Host host in hostList)
            {
                numberOfFreePes += PeList.GetNumberOfFreePes(host.PeList);
            }
            return numberOfFreePes;
        }

        public static int GetNumberOfBusyPes(this List<Host> hostList)
        {
            int numberOfBusyPes = 0;
            foreach (Host host in hostList)
            {
                numberOfBusyPes += PeList.GetNumberOfBusyPes(host.PeList);
            }
            return numberOfBusyPes;
        }

        public static Host GetHostWithFreePe(this List<Host> hostList)
        {
            return GetHostWithFreePe(hostList, 1);
        }

        public static Host GetHostWithFreePe(this List<Host> hostList, int pesNumber)
        {
            foreach (Host host in hostList)
            {
                if (PeList.GetNumberOfFreePes(host.PeList) >= pesNumber)
                {
                    return host;
                }
            }
            return null;
        }

        public static bool SetPeStatus(this List<Host> hostList, int status, int hostId, int peId)
        {
            Host host = GetById(hostList, hostId);
            if (host == null) return false;
            return host.SetPeStatus(peId, status);
        }
    }
}
