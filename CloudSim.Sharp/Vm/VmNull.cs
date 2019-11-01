using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Vm
{
    public class VmNull : Vm
    {
        public int Id { get { return -1; } set { } }
         public double getSubmissionDelay()
         {
            return 0;
         }
         public void setSubmissionDelay(double submissionDelay) {/**/}
         public void addStateHistoryEntry(VmStateHistoryEntry entry) {/**/}
         public Resource getBw()
        {
            return Resource.NULL;
        }
         public CloudletScheduler getCloudletScheduler() { return CloudletScheduler.NULL; }
         public long getFreePesNumber() { return 0; }
         public long getExpectedFreePesNumber() { return 0; }
         public long getCurrentRequestedBw()
        {
            return 0;
        }
         public double getCurrentRequestedMaxMips()
        {
            return 0.0;
        }
         public List<Double> getCurrentRequestedMips()
        {
            return Collections.emptyList();
        }
         public long getCurrentRequestedRam()
        {
            return 0;
        }
         public double getCurrentRequestedTotalMips()
        {
            return 0.0;
        }
         public Host getHost()
        {
            return Host.NULL;
        }
         public double getMips()
        {
            return 0;
        }
         public long getNumberOfPes()
        {
            return 0;
        }
         public Vm addOnHostAllocationListener(EventListener<VmHostEventInfo> listener)
        {
            return this;
        }
         public Vm addOnHostDeallocationListener(EventListener<VmHostEventInfo> listener) { return this; }
         public Vm addOnCreationFailureListener(EventListener<VmDatacenterEventInfo> listener)
        {
            return this;
        }
         public Vm addOnUpdateProcessingListener(EventListener<VmHostEventInfo> listener)
        {
            return this;
        }
         public void notifyOnHostAllocationListeners() {/**/}
         public void notifyOnHostDeallocationListeners(Host deallocatedHost) {/**/}
         public void notifyOnCreationFailureListeners(Datacenter failedDatacenter) {/**/}
         public boolean removeOnUpdateProcessingListener(EventListener<VmHostEventInfo> listener)
        {
            return false;
        }
         public boolean removeOnHostAllocationListener(EventListener<VmHostEventInfo> listener)
        {
            return false;
        }
         public boolean removeOnHostDeallocationListener(EventListener<VmHostEventInfo> listener)
        {
            return false;
        }
         public boolean removeOnCreationFailureListener(EventListener<VmDatacenterEventInfo> listener) { return false; }
         public Resource getRam()
        {
            return Resource.NULL;
        }
         public Resource getStorage()
        {
            return Resource.NULL;
        }
         public List<VmStateHistoryEntry> getStateHistory()
        {
            return Collections.emptyList();
        }
         public double getHostCpuUtilization(double time) { return 0; }
         public double getCpuPercentUtilization(double time)
        {
            return 0.0;
        }
         public double getCpuPercentUtilization()
        {
            return 0;
        }
         public double getExpectedHostCpuUtilization(double vmCpuUtilizationPercent) { return 0; }
         public double getHostRamUtilization() { return 0; }
         public double getHostBwUtilization() { return 0; }
         public double getTotalCpuMipsUtilization() { return 0; }
         public double getTotalCpuMipsUtilization(double time)
        {
            return 0.0;
        }
         public String getUid()
        {
            return "";
        }
         public DatacenterBroker getBroker()
        {
            return DatacenterBroker.NULL;
        }
         public void setBroker(DatacenterBroker broker) {/**/}
         public double getStartTime() { return 0; }
         public Vm setStartTime(double startTime) { return this; }
         public double getStopTime() { return 0; }
         public double getTotalExecutionTime() { return 0; }
         public Vm setStopTime(double stopTime) { return this; }
         public double getLastBusyTime() { return 0; }
         public double getIdleInterval() { return 0; }
         public boolean isIdle() { return false; }
         public boolean isIdleEnough(double time) { return false; }
         public UtilizationHistory getUtilizationHistory() { return UtilizationHistory.NULL; }
         public String getVmm()
        {
            return "";
        }
         public boolean isCreated()
        {
            return false;
        }
         public boolean isSuitableForCloudlet(Cloudlet cloudlet) { return false; }
         public boolean isInMigration()
        {
            return false;
        }
         public void setCreated(boolean created) {/**/}
        public Vm setBw(long bwCapacity)
        {
            return this;
        }
        public void setHost(Host host) {/**/}
        public void setInMigration(boolean migrating) {/**/}
        public Vm setRam(long ramCapacity)
        {
            return this;
        }
         public Vm setSize(long size)
        {
            return this;
        }
         public double updateProcessing(double currentTime, List<Double> mipsShare) { return 0.0; }
         public double updateProcessing(List<Double> mipsShare) { return 0; }
         public Vm setCloudletScheduler(CloudletScheduler cloudletScheduler)
        {
            return this;
        }
         public int compareTo(Vm vm) { return 0; }
         public double getTotalMipsCapacity()
        {
            return 0.0;
        }
         public void allocateResource(Class<? extends ResourceManageable> clazz, long amount) {/**/}
         public void deallocateResource(Class<? extends ResourceManageable> clazz) {/**/}
         public void setFailed(boolean failed) {/**/}
         public boolean isFailed()
        {
            return true;
        }
         public boolean isWorking() { return false; }
         public Simulation getSimulation()
        {
            return Simulation.NULL;
        }
         public void setLastTriedDatacenter(Datacenter lastTriedDatacenter) {/**/}
         public Datacenter getLastTriedDatacenter() { return Datacenter.NULL; }
         public List<ResourceManageable> getResources() { return Collections.emptyList(); }
         public String toString() { return "Vm.NULL"; }
         public HorizontalVmScaling getHorizontalScaling() { return HorizontalVmScaling.NULL; }
         public Vm setHorizontalScaling(HorizontalVmScaling scaling) throws IllegalArgumentException { return this; }
         public Vm setRamVerticalScaling(VerticalVmScaling scaling) throws IllegalArgumentException { return this; }
         public Vm setBwVerticalScaling(VerticalVmScaling scaling) throws IllegalArgumentException { return this; }
         public Vm setPeVerticalScaling(VerticalVmScaling scaling) throws IllegalArgumentException { return this; }
         public VerticalVmScaling getRamVerticalScaling() { return VerticalVmScaling.NULL; }
         public VerticalVmScaling getBwVerticalScaling() { return VerticalVmScaling.NULL; }
         public VerticalVmScaling getPeVerticalScaling() { return VerticalVmScaling.NULL; }
         public Processor getProcessor() { return Processor.NULL; }
         public String getDescription() { return ""; }
         public Vm setDescription(String description) { return this; }
         public VmGroup getGroup() { return null; }
         public double getTimeZone() { return Integer.MAX_VALUE; }
         public Vm setTimeZone(double timeZone) { return this; }
    }
}
