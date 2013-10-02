using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeSystem.Exceptions
{
    class NodeSocketInvalid : NodeException
    {
        public NodeSocketInvalid() { }
        public NodeSocketInvalid(string msg) : base(msg) { }
        public NodeSocketInvalid(string msg, Exception exception) : base(msg, exception) { }
    }
}
