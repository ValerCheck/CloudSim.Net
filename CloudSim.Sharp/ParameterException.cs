using System;

namespace CloudSim.Sharp
{
    public class ParameterException : Exception
    {
        private static readonly long _serialVersionUID = 1;

        private readonly string _message;

        public ParameterException()
        {
            _message = null;
        }

        public ParameterException(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}
