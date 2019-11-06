using System.Collections.Generic;
using System.Linq;

namespace CloudSim.Sharp.Core
{
    public abstract class Resourceful
    {
        public ResourceManageable GetResource<T>() =>
                Resources
                .Where(resource => resource.IsSubClassOf<T>())
                .ToList()
                .FirstOrDefault(ResourceManageable.NULL);

        public abstract List<ResourceManageable> Resources { get; }
    }
}
