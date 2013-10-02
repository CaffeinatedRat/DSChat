using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeSystem.Exceptions
{
    class NodePortInUseException : NodeException
    {
        public NodePortInUseException() { }
        public NodePortInUseException(string msg) : base(msg) { }
        public NodePortInUseException(string msg, Exception exception) : base(msg, exception) { }
    }
}
