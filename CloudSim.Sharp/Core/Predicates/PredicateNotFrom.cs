using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core.Predicates
{
    public class PredicateNotFrom : Predicate
    {
        private int[] ids;

        public PredicateNotFrom(int sourceId)
        {
            ids = new int[] { sourceId };
        }

        public PredicateNotFrom(int[] sourceIds)
        {
            ids = (int[])sourceIds.Clone();
        }

        public override bool Match(SimEvent simEvent)
        {
            int src = simEvent.Source;
            foreach(var id in ids)
            {
                if (src == id) return false;
            }
            return true;
        }
    }
}
