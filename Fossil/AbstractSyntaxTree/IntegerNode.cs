using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class IntegerNode : INode
    {
        public IntegerNode(IntegerToken token)
        {
            Contract.Requires<ArgumentNullException>(token != null);
            this.token = token;
        }

        public Variant Eval(Environment env)
        {
            return (Variant)token.Value;
        }

        private readonly IntegerToken token;
    }
}
