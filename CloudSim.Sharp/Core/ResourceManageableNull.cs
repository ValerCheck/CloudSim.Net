namespace CloudSim.Sharp.Core
{
    public class ResourceManageableNull : ResourceManageable
    {
        public override long Capacity => 0;
        public override long AvailableResource => 0;
        public override long AllocatedResource => 0;
        public override bool IsFull() => false;
        public override bool AddCapacity(long capacityToAdd) => false;
        public override bool AllocateResource(long amountToAllocate) => false;
        public override long DeallocateAllResources() => 0;
        public override bool DeallocateAndRemoveResource(long amountToDeallocate) => false;
        public override bool DeallocateResource(long amountToDeaalocate) => false;
        public override bool IsAmountAvailable(long amountToCheck) => false;
        public override bool IsResourceAmountBeingUsed(long amountToCheck) => false;
        public override bool IsSuitable(long newTotalAllocatedResource) => false;
        public override bool RemoveCapacity(long capacityToRemove) => false;
        public override bool SetAllocatedResource(long newTotalAllocatedResource) => false;
        public override bool SetCapacity(long newCapacity) => false;
        public override bool SumCapacity(long amountToSum) => false;
    }
}
