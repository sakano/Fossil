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

        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)token.Value;
        }

        private readonly IntegerToken token;
    }
}
