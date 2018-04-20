namespace CloudSim.Sharp.Core.Predicates
{
    public class PredicateNone : Predicate
    {
        public override bool Match(SimEvent e)
        {
            return false;        
        }
    }
}
