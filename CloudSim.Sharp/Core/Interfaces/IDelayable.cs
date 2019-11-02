namespace CloudSim.Sharp.Core.Interfaces
{
    public interface IDelayable
    {
        /**
         * Sets the time (in seconds) that a {@link DatacenterBroker} will wait
         * to request the creation of the object.
         * This is a relative time from the current simulation time.
         */
        double SubmissionDelay { get; set; }
    }
}
