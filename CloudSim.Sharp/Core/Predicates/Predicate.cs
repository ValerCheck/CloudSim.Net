using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core.Predicates
{
    public abstract class Predicate
    {
        public abstract bool Match(SimEvent simEvent);
    }
}
