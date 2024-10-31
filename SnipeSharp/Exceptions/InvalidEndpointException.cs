using System;
using System.Runtime.Serialization;

namespace SnipeSharp.Exceptions
{
    class InvalidEndpointException : Exception
    {
        public InvalidEndpointException()
        {
        }

        public InvalidEndpointException(string message) : base(message)
        {
        }

        public InvalidEndpointException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
