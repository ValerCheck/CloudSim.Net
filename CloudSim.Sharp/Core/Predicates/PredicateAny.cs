using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core.Predicates
{
    public class PredicateAny : Predicate
    {
        public override bool Match(SimEvent simEvent)
        {
            return true;
        }
    }
}
