using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Network
{
    public class DelayMatrixFloat
    {
        private float[][] _delayMatrix = null;

        public int _totalNodeNum = 0;

        public DelayMatrixFloat() { }

        public DelayMatrixFloat(TopologicalGraph graph, bool directed)
        {
            CreateDelayMatrix(graph, directed);

            CalculateShortestPath();
        }

        public float GetDelay(int srcId, int destId)
        {
            if (srcId > _totalNodeNum || destId > _totalNodeNum)
            {
                throw new IndexOutOfRangeException("srcId or destId is higher than highest stored node-ID!");
            }

            return _delayMatrix[srcId][destId];
        }

        private void CreateDelayMatrix(TopologicalGraph graph, bool directed)
        {
            _totalNodeNum = graph.NumberOfNodes;

            _delayMatrix = new float[_totalNodeNum][];
            for (var i = 0; i < _totalNodeNum; i++)
            {
                _delayMatrix[i] = new float[_totalNodeNum];
            }

            for (int row = 0; row < _totalNodeNum; ++row)
            {
                for (int col = 0; col < _totalNodeNum; ++col)
                {
                    _delayMatrix[row][col] = float.MaxValue;
                }
            }

            IEnumerator<TopologicalLink> itr = graph.GetLinkEnumerator();

            TopologicalLink edge;

            do
            {
                edge = itr.Current;
                if (edge != null)
                {
                    _delayMatrix[edge.SrcNodeId][edge.DestNodeId] = edge.LinkDelay;

                    if (!directed)
                    {
                        // according to aproximity of symmetry to all communication-paths
                        _delayMatrix[edge.DestNodeId][edge.SrcNodeId] = edge.LinkDelay;
                    }
                }
            } while (itr.MoveNext());
        }

        private void CalculateShortestPath()
        {
            FloydWarshallFloat floyd = new FloydWarshallFloat();
            floyd.Initialize(_totalNodeNum);
            _delayMatrix = floyd.AllPairsShortestPaths(_delayMatrix);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("just a simple printout of the distance-aware-topology-class")
                .AppendLine("delay-matrix is:");

            for (int column = 0; column < _totalNodeNum; ++column)
            {
                sb.Append($"\t {column}");
            }

            for (int row = 0; row < _totalNodeNum; ++row)
            {
                sb.Append($"\n {row}");

                for (int col = 0; col < _totalNodeNum; ++col)
                {
                    if (_delayMatrix[row][col] == float.MaxValue)
                    {
                        sb.Append("\t -");
                    }
                    else
                    {
                        sb.Append($"\t {_delayMatrix[row][col]}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
