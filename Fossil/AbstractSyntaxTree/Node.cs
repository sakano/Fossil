using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fossil
{
    internal interface INode
    {
        Object eval();
    }

    internal class IntegerNode : INode
    {
        public IntegerNode(IntegerToken token) {
            this.token = token;
        }

        public Object eval() {
            return token.Value;
        }

        private readonly IntegerToken token;
    }
}
