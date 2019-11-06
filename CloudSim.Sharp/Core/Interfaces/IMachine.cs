using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CloudSim.Sharp.Cloudlet;

namespace CloudSim.Sharp.Core.Interfaces
{
    public interface IMachine
    {
        Resource getBw();

        Resource getRam();

        Resource getStorage();

        long getNumberOfPes();

        double getMips();

        double getTotalMipsCapacity();

        Simulation getSimulation();


    /**
     * Checks if the Machine has been idle for a given amount of time (in seconds).
     * @param time the time interval to check if the Machine has been idle (in seconds).
     *             If time is zero, it will be checked if the Machine is currently idle.
     *             If it's negative, even if the Machine is idle, it's considered
     *             that it isn't idle enough. This is useful if you don't want to perform
     *             any operation when the machine becomes idle (for instance,
     *             if idle machines might be shut down and a negative value is given,
     *             they won't).
     * @return true if the Machine has been idle as long as the given time,
     *         false if it's active of isn't idle long enough
     */
    default boolean isIdleEnough(final double time)
        {
            if (time < 0)
            {
                return false;
            }

            return getIdleInterval() >= time;
        }

    /**
     * Gets the interval interval the Machine has been idle.
     * @return the idle time interval (in seconds) or 0 if the Machine is not idle
     */
    default double getIdleInterval()
        {
            return getSimulation().clock() - getLastBusyTime();
        }

        /**
         * Gets the last time the Machine was running some process.
         * @return the last busy time (in seconds)
         */
        double getLastBusyTime();

    /**
     * Checks if the Machine is currently idle.
     * @return true if the Machine currently idle, false otherwise
     */
    default boolean isIdle()
        {
            return getIdleInterval() > 0;
        }

        /**
         * Validates a capacity for a machine resource.
         * @param capacity the capacity to check
         */
        static void validateCapacity(final double capacity)
        {
            if (capacity <= 0)
            {
                throw new IllegalArgumentException("Capacity must be greater than zero");
            }
        }
    }
}
