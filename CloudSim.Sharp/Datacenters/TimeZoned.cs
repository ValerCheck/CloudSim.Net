using System;
using System.Collections.Generic;

namespace CloudSim.Sharp.Datacenters
{
    public abstract class TimeZoned
    {
        /**
         * Gets or sets the time zone offset, a value between  [-12 and 12],
         * in which the object is physically located.
         *
         * @return the time zone offset
         */
        double TimeZone { get; set; }

        public virtual double ValidateTimeZone(double timeZone)
        {
            if (timeZone < -12 || timeZone > 13)
                throw new ArgumentException("Timezone offset must be between [-12 and 12].");
            
            return timeZone;
        }

        /**
         * Selects the {@link Datacenter} closest to a given {@link Vm}, based on their timezone.
         * It considers the Datacenter list is already sorted by timezone.
         *
         * @param vm to Vm to try place into the closest Datacenter
         * @param datacenters the list of available Datacenters, sorted by timezone
         * @return the first selected Datacenter
         */
        public static Datacenter ClosestDatacenter(Vm.Vm vm, List<Datacenter> datacenters)
        {
            if (datacenters?.Count == 0)
                throw new ArgumentException("The list of Datacenters is empty.");

            if (datacenters.Count == 1)
                return datacenters[0];

            /* Since the datacenter list is expected to be sorted,
             * if the VM timezone is negative or zero, start looking from the beginning of the list.
             * If it's positive, start looking from the end. */
            var it = datacenters.GetEnumerator();

            Datacenter currentDc = Datacenter.NULL, previousDc = currentDc;

            do
            {
                currentDc = it.Current;
                /*Since the Datacenter list is expected to be sorted, after finding the first DC with a
                distance larger than the previous one, the previous is the closest one.*/
                if (GetDistance(vm, currentDc) > GetDistance(vm, previousDc))
                    return previousDc;

                previousDc = currentDc;
            } while (it.MoveNext());

            return currentDc;
        }

        /**
         * Computes the distance between two TimeZoned objects,
         * considering their timezone offset values.
         *
         * @param o1 the first object
         * @param o2 the second object
         * @return a positive integer value representing the distance between the objects
         */
        public static double GetDistance(TimeZoned o1, TimeZoned o2) => Math.Abs(o2.TimeZone - o1.TimeZone);

        public static string Format(double timeZone)
        {
            double decimals = timeZone - (int)timeZone;
            string formatted = decimals == 0 ?
                                        string.Format("GMT%+.0f", timeZone) :
                                        string.Format("GMT%+d:%2.0f", (int)timeZone, TimeSpan.FromHours(decimals).TotalMinutes);
            return string.Format("%-8s", formatted);
        }

    }
}
