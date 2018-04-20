namespace CloudSim.Sharp.Core.Predicates
{
    public class PredicateType : Predicate
    {
        private int[] _tags;

        public PredicateType(int tag)
        {
            _tags = new int[] { tag };
        }

        public PredicateType(int[] tags)
        {
            _tags = (int[])tags.Clone();
        }

        public override bool Match(SimEvent simEvent)
        {
            var evTag = simEvent.Tag;
            foreach(var tag in _tags)
            {
                if (tag == evTag) return true;
            }
            return false;
        }
    }
}
