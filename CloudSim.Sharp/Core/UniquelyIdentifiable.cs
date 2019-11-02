using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public abstract class UniquelyIdentifiable : IIdentifiable
    {
        /**
         * Generates an Unique Identifier (UID).
         *
         * @param brokerId the id of the {@link DatacenterBroker} (user)
         * @param id the object id
         * @return the generated UID
         */
        static string GetUid(long brokerId, long id) => $"{brokerId}-{id}";
        public abstract long Id { get; }
        /**
         * Gets the Unique Identifier (UID) for the VM, that is compounded by the id
         * of a {@link DatacenterBroker} (representing the User)
         * and the object id.
         *
         * @return
         */
        public abstract string GetUid();
    }
}
