using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Core
{
    public interface IChangeableId : IIdentifiable
    {
        new long Id { get; set; }
    }
}
