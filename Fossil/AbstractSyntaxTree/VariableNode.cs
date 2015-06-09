using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class VariableNode : INode
    {
        public VariableNode(IdentifierToken identifierToken)
        {
            Contract.Requires<ArgumentNullException>(identifierToken != null);
            this.identifierToken = identifierToken;
        }

        public Variant Eval(Environment env)
        {
            Contract.Assume(identifierToken.Value.Length != 0);
            Variant v = env.Get(identifierToken.Value);
            if (v == null) {
                throw new RuntimeException(identifierToken.LineNumber, "undefined variable");
            }
            return v;
        }

        public Variant Assign(Environment env, Variant variant)
        {
            Contract.Requires<ArgumentNullException>(env != null);
            Contract.Requires<ArgumentNullException>(variant != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            Contract.Assume(identifierToken.Value.Length != 0);
            env.Assign(identifierToken.Value, variant);
            return variant;
        }

        private readonly IdentifierToken identifierToken;
    }
}
