using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Network
{
    public class TopologicalLink
    {
        private int _srcNodeId = 0;
        private int _destNodeId = 0;
        private float _linkDelay = 0;
        private float _linkBw = 0;

        public TopologicalLink(int srcNode, int destNode, float delay, float bw)
        {
            _linkDelay = delay;
            _srcNodeId = srcNode;
            _destNodeId = destNode;
            _linkBw = bw;
        }

        public int SrcNodeId
        {
            get { return _srcNodeId; }
        }

        public int DestNodeId
        {
            get { return _destNodeId; }
        }

        public float LinkDelay
        {
            get { return _linkDelay; }
        }

        public float LinkBw
        {
            get { return _linkBw; }
        }
    }
}
