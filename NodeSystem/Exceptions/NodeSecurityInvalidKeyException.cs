using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeSystem.Exceptions
{
    class NodeSecurityInvalidKeyException : NodeException
    {
        public NodeSecurityInvalidKeyException() { }
        public NodeSecurityInvalidKeyException(string msg) : base(msg) { }
        public NodeSecurityInvalidKeyException(string msg, Exception exception) : base(msg, exception) { }
    }
}
