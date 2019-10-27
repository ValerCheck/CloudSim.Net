using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Vm
{
    public interface UtilizationHistory
    {
        /**
         * An attribute that implements the Null Object Design Pattern for {@link UtilizationHistory}
         * objects.
         */
        UtilizationHistory NULL = new UtilizationHistoryNull();

        /**
         * Gets the utilization Median Absolute Deviation (MAD) in MIPS.
         *
         * @return
         */
        double UtilizationMad { get; }

        /**
         * Gets the utilization mean in MIPS.
         *
         * @return
         */
        double UtilizationMean { get; }

        /**
         * Gets the utilization variance in MIPS.
         *
         * @return the utilization variance in MIPS
         */
        double UtilizationVariance { get; }

        /**
         * Adds a CPU utilization percentage history value related to the current simulation time,
         * to the beginning of the History List.
         * <b>The value is added only if the utilization history {@link #isEnabled()}.</b>
         *
         * @param time the current simulation time
         */
        void addUtilizationHistory(double time);

        /**
         * Gets a <b>read-only</b> CPU utilization percentage history map
         * where each key is the time the utilization was collected and
         * each value is the utilization percentage (between [0 and 1]).
         * There will be at least one entry for each time multiple of the {@link Datacenter#getSchedulingInterval()}.
         * <b>This way, it's required to set a Datacenter scheduling interval with the desired value.</b>
         *
         * @return
         */
        SortedDictionary<Double, Double> History { get; }

        /**
         * Computes the amount of power the VM is using, relative to the total Host's power consumption
         * (in watt-sec).
         *
         * @param time the time to get the VM power consumption
         * @return the relative VM power consumption in watt-sec
         */
        double powerConsumption(double time);

        /**
         * Computes the relative percentage of the CPU the VM is using from the Host's total MIPS Capacity.
         * If the capacity is 1000 MIPS and the VM is using 250 MIPS, it's equivalent to 25%
         * of the Host's capacity.
         *
         * <p>This method uses the historical data to compute the relative CPU utilization,
         * allowing it to be called after the simulation finishes.
         * It's different from the {@link Vm#getHostCpuUtilization(double)}
         * that can be called only when the simulation is running.</p>
         *
         * @param time the time to get the relative VM CPU utilization
         * @return the relative VM CPU usage percent (from 0 to 1)
         */
        double getHostCpuUtilization(double time);

        /**
         * Checks if the object is enabled to add data to the history.
         * @return
         */
        bool IsEnabled { get; }

        /**
         * Enables the history so that utilization data can be added to it.
         */
        void enable();

        /**
         * Disables the history to avoid utilization data to be added to it.
         * That allows to reduce memory usage since no utilization
         * data will be collected.
         */
        void disable();

        int MaxHistoryEntries { get; set; }

        Vm Vm { get; }
    }

}
