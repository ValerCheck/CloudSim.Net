using System;
using System.IO;
using System.Text;

namespace CloudSim.Sharp.Network
{
    public class GraphReaderBrite : IGraphReader
    {
        private const int PARSE_NOTHING = 0;
        private const int PARSE_NODES = 1;
        private const int PARSE_EDGES = 2;

        private int _state = PARSE_NOTHING;

        private TopologicalGraph _graph = null;

        public TopologicalGraph ReadGraphFile(string fileName)
        {
            _graph = new TopologicalGraph();

            if (!System.IO.File.Exists(fileName)) throw new FileNotFoundException();

            using (var streamReader = new StreamReader(System.IO.File.OpenRead(fileName)))
            {
                string newLine = Environment.NewLine;
                string nextLine = null;
                var sb = new StringBuilder();

                while (!streamReader.EndOfStream)
                {
                    nextLine = streamReader.ReadLine();

                    sb.Append(nextLine)
                        .Append(newLine);

                    if (_state == PARSE_NOTHING)
                    {
                        if (nextLine.Contains("Nodes:"))
                        {
                            _state = PARSE_NODES;
                        }
                    }
                    else if (_state == PARSE_NODES)
                    {
                        ParseNodeString(nextLine);
                    }
                    else if (_state == PARSE_EDGES)
                    {
                        ParseEdgesString(nextLine);
                    }
                }
            }

            return _graph;
        }

        private void ParseNodeString(string nodeLine)
        {
            var values = nodeLine.Split(' ');

            int parameters = 3;

            if (nodeLine.Contains("Edges:"))
            {
                _state = PARSE_EDGES;
                return;
            }

            if (values.Length == 0) return;

            int nodeId = 0;
            string nodeLabel = "";
            int xPos = 0;
            int yPos = 0;
            int actualParam = 0;

            for (int i = 0; i < values.Length && i < parameters; i++)
            {
                string token = values[i];
                switch (i)
                {
                    case 0:
                        int.TryParse(token, out nodeId);
                        nodeLabel = nodeId.ToString();
                        break;
                    case 1:
                        int.TryParse(token, out xPos);
                        break;
                    case 2:
                        int.TryParse(token, out yPos);
                        break;
                }
            }

            TopologicalNode topoNode = new TopologicalNode(nodeId, nodeLabel, xPos, yPos);
            _graph.AddNode(topoNode);
        }

        private void ParseEdgesString(string nodeLine)
        {
            var values = nodeLine.Split(' ');

            int parameters = 6;

            if (values.Length == 0) return;

            int fromNode = 0;
            int toNode = 0;
            float linkDelay = 0;
            int linkBandwidth = 0;

            for (int i = 0; i < values.Length && i < parameters; i++)
            {
                string token = values[i];
                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        int.TryParse(token, out fromNode);
                        break;
                    case 2:
                        int.TryParse(token, out toNode);
                        break;
                    case 3:
                        break;
                    case 4:
                        float.TryParse(token, out linkDelay);
                        break;
                    case 5:
                        float tempBw;
                        if (float.TryParse(token, out tempBw))
                        {
                            linkBandwidth = Convert.ToInt32(tempBw);
                        }
                        break;
                }
            }

            _graph.AddLink(new TopologicalLink(fromNode, toNode, linkDelay, linkBandwidth));
        }
    }
}
