using System.Collections.Generic;

namespace CloudSim.Sharp.Lists
{
    public static class PeList
    {
        public static Pe GetById(this List<Pe> peList, int id)
        {
            foreach (Pe pe in peList)
            {
                if (pe.Id == id) return pe;
            }
            return null;
        }

        public static int GetMips(this List<Pe> peList, int id)
        {
            Pe pe = GetById(peList, id);
            if (pe != null)
                return pe.Mips;
            return -1;
        }

        public static int GetTotalMips(this List<Pe> peList)
        {
            int totalMips = 0;
            
            foreach (Pe pe in peList)
                totalMips += pe.Mips;

            return totalMips;
        }

        public static double GetMaxUtilization(this List<Pe> peList)
        {
            double maxUtilization = 0;
            foreach (Pe pe in peList)
            {
                double utilization = pe.PeProvisioner.GetUtilization();
                if (utilization > maxUtilization)
                    maxUtilization = utilization;
            }
            return maxUtilization;
        }

        public static double GetMaxUtilizationAmongVmsPes(this List<Pe> peList, Vm vm)
        {
            double maxUtilization = 0;
            foreach (Pe pe in peList)
            {
                if (pe.PeProvisioner.GetAllocatedMipsForVm(vm) == null) continue;
                double utilization = pe.PeProvisioner.GetUtilization();
                if (utilization > maxUtilization)
                    maxUtilization = utilization;
            }

            return maxUtilization;
        }

        public static Pe GetFreePe(this List<Pe> peList)
        {
            foreach (Pe pe in peList)
            {
                if (pe.Status == Pe.FREE)
                    return pe;
            }
            return null;
        }

        public static int GetNumberOfFreePes(this List<Pe> peList)
        {
            int cnt = 0;
            foreach (Pe pe in peList)
            {
                if (pe.Status == Pe.FREE)
                    cnt++;
            }
            return cnt;
        }

        public static bool SetPeStatus(this List<Pe> peList, int id, int status)
        {
            Pe pe = GetById(peList, id);
            if (pe != null)
            {
                pe.Status = status;
                return true;
            }
            return false;
        }

        public static int GetNumberOfBusyPes(this List<Pe> peList)
        {
            int cnt = 0;
            foreach (Pe pe in peList)
            {
                if (pe.Status == Pe.BUSY)
                    cnt++;
            }
            return cnt;
        }

        public static void SetStatusFailed(
            this List<Pe> peList,
            string resName,
            int hostId,
            bool failed)
        {
            string status = null;

            if (failed) status = "FAILED";
            else status = "WORKING";

            Log.WriteConcatLine(resName, "- Machine: ", hostId, " is ", status);
            SetStatusFailed(peList, failed);
        }

        public static void SetStatusFailed(this List<Pe> peList, bool failed)
        {
            foreach (Pe pe in peList)
            {
                if (failed) pe.Status = Pe.FAILED;
                else pe.Status = Pe.FREE;
            }
        }
    }
}
