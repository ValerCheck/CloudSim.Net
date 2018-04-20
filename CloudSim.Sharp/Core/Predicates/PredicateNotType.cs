using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core.Predicates
{
    public class PredicateNotType : Predicate
    {
        private int[] _tags;

        public PredicateNotType(int tag)
        {
            _tags = new int[] { tag };
        }

        public PredicateNotType(int[] tags)
        {
            _tags = (int[])tags.Clone();
        }

        public override bool Match(SimEvent simEvent)
        {
            int tag = simEvent.Tag;
            foreach(var t in _tags)
            {
                if (tag == t) return false;
            }
            return true;
        }
    }
}
