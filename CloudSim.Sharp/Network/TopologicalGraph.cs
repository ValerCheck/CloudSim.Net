using System.Collections.Generic;
using System.Text;

namespace CloudSim.Sharp.Network
{
    public class TopologicalGraph
    {
        private ICollection<TopologicalLink> _linkList = null;

        private ICollection<TopologicalNode> _nodeList = null;

        public TopologicalGraph()
        {
            _linkList = new LinkedList<TopologicalLink>();
            _nodeList = new LinkedList<TopologicalNode>();
        }

        public void AddLink(TopologicalLink edge)
        {
            _linkList.Add(edge);
        }

        public void AddNode(TopologicalNode node)
        {
            _nodeList.Add(node);
        }

        public int NumberOfNodes
        {
            get { return _nodeList.Count; }
        }

        public int NumberOfLinks
        {
            get { return _linkList.Count; }
        }

        public IEnumerator<TopologicalLink> GetLinkEnumerator()
        {
            return _linkList.GetEnumerator();
        }

        public IEnumerator<TopologicalNode> GetNodeEnumerator()
        {
            return _nodeList.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Topological-node-information: ");

            foreach(var node in _nodeList)
            {
                sb.AppendLine($"{node.NodeId} | x is: {node.CoordinateX} y is: {node.CoordinateY}");
            }

            sb.AppendLine("\n\n node-link-information:");

            foreach(var link in _linkList)
            {
                sb.AppendLine($"from: {link.SrcNodeId} to: {link.DestNodeId} delay: {link.LinkDelay}");
            }
            return sb.ToString();
        }
    }
}
