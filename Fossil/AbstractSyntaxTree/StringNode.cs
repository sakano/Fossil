using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class StringNode : INode
    {
        public StringNode(StringToken token)
        {
            Contract.Requires<ArgumentNullException>(token != null);
            this.token = token;
        }

        public Variant Eval(Environment env)
        {
            return (Variant)token.Value;
        }

        private readonly StringToken token;
    }
}
