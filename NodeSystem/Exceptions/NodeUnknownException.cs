using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeSystem.Exceptions
{
    class NodeUnknownException : NodeException
    {
        public NodeUnknownException() { }
        public NodeUnknownException(string msg) : base(msg) { }
        public NodeUnknownException(string msg, Exception exception) : base(msg, exception) { }
    }
}
