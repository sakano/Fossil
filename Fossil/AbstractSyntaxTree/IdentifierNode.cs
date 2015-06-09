using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class IdentifierNode : INode
    {
        public IdentifierNode(IdentifierToken identifierToken)
        {
            Contract.Requires<ArgumentNullException>(identifierToken != null);
            this.identifierToken = identifierToken;
        }

        public Variant Eval(Environment env)
        {
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
            env.Assign(identifierToken.Value, variant);
            return variant;
        }

        public string Name {
            get{
                Contract.Ensures(Contract.Result<string>() != null);
                Contract.Ensures(Contract.Result<string>().Length != 0);
                return identifierToken.Value;
            }
        }

        private readonly IdentifierToken identifierToken;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.identifierToken != null);
        }
    }
}
