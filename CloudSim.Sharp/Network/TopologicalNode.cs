namespace CloudSim.Sharp.Network
{
    public class TopologicalNode
    {
        private int _nodeId = 0;
        private string _nodeName = null;
        private int _worldX = 0;
        private int _worldY = 0;

        public TopologicalNode(int nodeId)
        {
            _nodeId = nodeId;
            _nodeName = nodeId.ToString();
        }

        public TopologicalNode(int nodeId, int x, int y)
        {
            _nodeId = nodeId;
            _nodeName = nodeId.ToString();
            _worldX = x;
            _worldY = y;
        }

        public TopologicalNode(int nodeId, string name, int x, int y)
        {
            _nodeId = nodeId;
            _nodeName = name;
            _worldX = x;
            _worldY = y;
        }

        public int NodeId
        {
            get { return _nodeId; }
        }

        public string NodeLabel
        {
            get { return _nodeName; }
        }

        public int CoordinateX
        {
            get { return _worldX; }
        }

        public int CoordinateY
        {
            get { return _worldY; }
        }
    }
}
