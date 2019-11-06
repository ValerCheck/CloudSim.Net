using CloudSim.Sharp.Core.Interfaces;

namespace CloudSim.Sharp.Core
{
    public abstract class Resource : IResourceCapacity
    {
        public static Resource NULL = new ResourceNull();

        public bool IsSubClassOf<T>() => typeof(T).IsAssignableFrom(GetType());
    
        public abstract long Capacity { get; }

        public abstract long AvailableResource { get; }

        public abstract long AllocatedResource { get; }

        /**
         * Checks if there the capacity required for the given resource is available (free)
         * at this resource. This method is commonly used to check if there is a specific
         * amount of resource free at a physical resource (this Resource instance)
         * that is required by a virtualized resource (the given Resource).
         *
         * @param resource the resource to check if its capacity is available at the current resource
         * @return true if the capacity required by the given Resource is free; false otherwise
         * @see #isAmountAvailable(long)
         */
        public virtual bool IsAmountAvailable(Resource resource) => IsAmountAvailable(resource.Capacity);
        public virtual bool IsAmountAvailable(double amountToCheck) => IsAmountAvailable((long)amountToCheck);
        public abstract bool IsAmountAvailable(long amountToCheck);
        public virtual bool IsFull() => AvailableResource <= 0;

        /**
         * Gets the current percentage of resource utilization in scale from 0 to 1.
         * It is the percentage of the total resource capacity that is currently allocated.
         * @return current resource utilization (allocation) percentage in scale from 0 to 1
         */
        public virtual double PercentUtilization => Capacity > 0 ? AllocatedResource / (double)Capacity : 0.0;
    }
}
