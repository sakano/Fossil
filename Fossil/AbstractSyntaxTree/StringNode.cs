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
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)token.Value;
        }

        private readonly StringToken token;
    }
}
