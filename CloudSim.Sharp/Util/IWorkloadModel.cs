using System.Collections.Generic;

namespace CloudSim.Sharp.Util
{
    public interface IWorkloadModel
    {
        List<Cloudlet> GenerateWorkload();
    }
}
