using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeSystem.Exceptions
{
    /// <summary>
    /// The master node exception.
    /// </summary>
    class NodeException : Exception
    {
        public NodeException() { }
        public NodeException(string msg) : base(msg) { }
        public NodeException(string msg, Exception exception) : base(msg, exception) { }
    }
}
