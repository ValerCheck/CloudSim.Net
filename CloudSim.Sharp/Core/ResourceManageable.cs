using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public abstract class ResourceManageable : Resource
    {
        public new static ResourceManageable NULL = new ResourceManageableNull();

        public abstract bool SetCapacity(long newCapacity);
        public abstract bool SumCapacity(long amountToSum);
        public abstract bool AddCapacity(long capacityToAdd);
        public abstract bool RemoveCapacity(long capacityToRemove);
        public abstract bool AllocateResource(long amountToAllocate);
        public virtual bool AllocateResource(Resource resource) => AllocateResource(resource.Capacity);
        public abstract bool SetAllocatedResource(long newTotalAllocatedResource);
        public abstract bool DeallocateResource(long amountToDeaalocate);
        public abstract bool DeallocateAndRemoveResource(long amountToDeallocate);
        public abstract long DeallocateAllResources();
        public abstract bool IsResourceAmountBeingUsed(long amountToCheck);
        public abstract bool IsSuitable(long newTotalAllocatedResource);

        public virtual bool SetAllocatedResource(double newTotalAllocatedResource) => SetAllocatedResource((long)newTotalAllocatedResource);
        public virtual bool DeallocateResource(Resource resource) => DeallocateResource(resource.Capacity);

    }
}
