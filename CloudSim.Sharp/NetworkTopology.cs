using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CloudSim.Sharp.Network;

namespace CloudSim.Sharp
{
    public class NetworkTopology
    {
        public static int NextIdx = 0;

        private static bool _networkEnabled = false;

        public static DelayMatrixFloat DelayMatrix = null;

        public static double[][] BwMatrix = null;

        public static TopologicalGraph Graph = null;

        public static Dictionary<int, int> Map = null;

        public static bool IsNetworkEnabled
        {
            get { return _networkEnabled; }
        }


        public static void BuildNetworkTopology(string filename)
        {
            Log.WriteConcatLine("Topology file: ", filename);

            GraphReaderBrite reader = new GraphReaderBrite();

            try
            {
                Graph = reader.ReadGraphFile(filename);
                Map = new Dictionary<int, int>();
                GenerateMatrices();
            }
            catch (IOException e)
            {
                Log.WriteLine("Problem in process BRITE file. Network simulation is disabled.");
                Log.WriteLine($"Error: {e.Message}");
            }
        }

        private static void GenerateMatrices()
        {
            DelayMatrix = new DelayMatrixFloat(Graph, false);

            BwMatrix = CreateBwMatrix(Graph, false);

            _networkEnabled = true;
        }

        public static void AddLink(int srcId, int destId, double bw, double lat)
        {
            if (Graph == null)
            {
                Graph = new TopologicalGraph();
            }

            if (Map == null)
            {
                Map = new Dictionary<int, int>();
            }

            if (!Map.ContainsKey(srcId))
            {
                Graph.AddNode(new TopologicalNode(NextIdx));
                Map.Add(srcId, NextIdx);
                NextIdx++;
            }

            if (!Map.ContainsKey(destId))
            {
                Graph.AddNode(new TopologicalNode(NextIdx));
                Map.Add(destId, NextIdx);
                NextIdx++;
            }

            Graph.AddLink(new TopologicalLink(Map[srcId], Map[destId], (float)lat, (float)bw));
            GenerateMatrices();
        }

        private static double[][] CreateBwMatrix(TopologicalGraph graph, bool directed)
        {
            int nodes = Graph.NumberOfNodes;

            double[][] mtx = new double[nodes][];

            for (var i = 0; i < nodes; i++)
                mtx[i] = new double[nodes];

            var enumerator = graph.GetLinkEnumerator();
            do
            {
                TopologicalLink edge = enumerator.Current;

                if (edge != null)
                {
                    mtx[edge.SrcNodeId][edge.DestNodeId] = edge.LinkBw;

                    if (!directed)
                    {
                        mtx[edge.DestNodeId][edge.SrcNodeId] = edge.LinkBw;
                    }
                }

            } while (enumerator.MoveNext());

            return mtx;
        }

        public static void MapNode(int cloudSimEntityId, int briteId)
        {
            if (_networkEnabled)
            {
                try
                {
                    if (!Map.ContainsKey(cloudSimEntityId))
                    {
                        if (!Map.ContainsValue(briteId))
                        {
                            Map.Add(cloudSimEntityId, briteId);
                        }
                        else
                        {
                            Log.WriteLine($"Error in network mapping. BRITE node {briteId} already in use.");
                        }
                    } 
                    else
                    {
                        Log.WriteLine($"Error in network mapping. CloudSim entity {cloudSimEntityId} already mapped.");
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLine($"Error in network mapping. CloudSim node {cloudSimEntityId} not mapped to BRITE node {briteId}");
                }

            }
        }

        public static void UnmapNode(int cloudSimEntityId)
        {
            if (_networkEnabled)
            {
                try
                {
                    Map.Remove(cloudSimEntityId);
                }
                catch(Exception e)
                {
                    Log.WriteLine($"Error in network unmapping. CloudSim node: {cloudSimEntityId}");
                }
            }
        }

        public static double GetDelay(int srcId, int destId)
        {
            if (_networkEnabled)
            {
                try
                {
                    double delay = DelayMatrix.GetDelay(Map[srcId], Map[destId]);

                    return delay;
                }
                catch(Exception e)
                {
                    //
                }
            }
            return 0;
        }
    }
}
