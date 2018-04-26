using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp.Network
{
    public class FloydWarshallFloat
    {
        private int _numVertices;

        // Matrices used in dynamic programming
        private float[][] _dk, _dkMinusOne;

        // The predecessor matrix. Matrix used by dynamic programming.
        private int[][] _pk, _pkMinusOne;

        public void Initialize(int numVertices)
        {
            _numVertices = numVertices;

            _dk = new float[numVertices][];
            _dkMinusOne = new float[numVertices][];
            _pk = new int[numVertices][];
            _pkMinusOne = new int[numVertices][];

            for (int i = 0; i < numVertices; i++)
            {
                _dk[i] = new float[numVertices];
                _dkMinusOne[i] = new float[numVertices];
                _pk[i] = new int[numVertices];
                _pkMinusOne[i] = new int[numVertices];
            }
        }

        public float[][] AllPairsShortestPaths(float[][] adjMatrix)
        {
            for (int i = 0; i < _numVertices; i++)
            {
                for (int j = 0; j < _numVertices; j++)
                {
                    if (adjMatrix[i][j] != 0)
                    {
                        _dkMinusOne[i][j] = adjMatrix[i][j];
                        _pkMinusOne[i][j] = i;
                    }
                    else
                    {
                        _dkMinusOne[i][j] = float.MaxValue;
                        _pkMinusOne[i][j] = -1;
                    }
                }
            }

            for (int k = 0; k < _numVertices; k++)
            {
                for (int i = 0; i < _numVertices; i++)
                {
                    for (int j = 0; j < _numVertices; j++)
                    {
                        if (i != j)
                        {
                            if (_dkMinusOne[i][j] <= _dkMinusOne[i][k] + _dkMinusOne[k][j])
                            {
                                _dk[i][j] = _dkMinusOne[i][j];
                                _pk[i][j] = _pkMinusOne[i][j];
                            }
                            else
                            {
                                _dk[i][j] = _dkMinusOne[i][k] + _dkMinusOne[k][j];
                                _pk[i][j] = _pkMinusOne[k][j];
                            }
                        }
                        else
                        {
                            _pk[i][j] = -1;
                        }
                    }
                }

                for (int i = 0; i < _numVertices; i++)
                {
                    for (int j = 0; j < _numVertices; j++)
                    {
                        _dkMinusOne[i][j] = _dk[i][j];
                        _pkMinusOne[i][j] = _pk[i][j];
                    }
                }
            }

            return _dk;
        }

        public int[][] PK
        {
            get { return _pk; }
        }
    }
}
