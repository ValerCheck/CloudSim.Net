using CloudSim.Sharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Datacenters
{
    public class DatacenterNull : Datacenter, IComparable<SimEntity>, SimEntityNullBase
    {
        private static DatacenterStorage STORAGE = new DatacenterStorage();

        public int CompareTo(SimEntity entity) { return 0; }
        public List<Host> getHostList()
        {
            return Collections.emptyList();
        }
        public VmAllocationPolicy getVmAllocationPolicy()
        {
            return VmAllocationPolicy.NULL;
        }
        public void requestVmMigration(Vm sourceVm, Host targetHost) {/**/}
        public Host getHost(int index) => Host.NULL;
        public long getActiveHostsNumber() => 0;
        public long size() => 0;
        public Host getHostById(long id) { return Host.NULL; }
        public Datacenter AddHostList<T>(List<T> hostList) where T:Host => this;
        public Datacenter RemoveHost<T>(T host) where T:Host => this;
        public Datacenter AddHost(Host host) => this;
        public double getSchedulingInterval() { return 0; }
        public Datacenter setSchedulingInterval(double schedulingInterval) { return this; }
        public DatacenterCharacteristics getCharacteristics() { return DatacenterCharacteristics.NULL; }
        public DatacenterStorage getDatacenterStorage() { return STORAGE; }
        public void setDatacenterStorage(DatacenterStorage datacenterStorage) {/**/}
        public double getBandwidthPercentForMigration() { return 0; }
        public void setBandwidthPercentForMigration(double bandwidthPercentForMigration) {/**/}
        public double getPower() { return 0; }
        public Datacenter addOnHostAvailableListener(EventListener<HostEventInfo> listener) { return this; }
        public bool IsMigrationsEnabled() { return false; }
        public Datacenter enableMigrations() { return this; }
        public Datacenter disableMigrations() { return this; }
        public override string ToString() => "Datacenter.NULL";
        public double getTimeZone() => int.MaxValue;
        public TimeZoned setTimeZone(double timeZone) => this; }
    }
}
