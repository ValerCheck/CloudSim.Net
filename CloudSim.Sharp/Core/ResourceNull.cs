namespace CloudSim.Sharp.Core
{
    public class ResourceNull : Resource
    {
        public override long Capacity => 0;

        public override long AvailableResource => 0;

        public override long AllocatedResource => 0;

        public override bool IsAmountAvailable(long amountToCheck) => false;

        public override bool IsFull() => false;
    }
}
