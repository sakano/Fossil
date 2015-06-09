using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class BooleanNode : INode
    {
        public BooleanNode(bool b)
        {
            this.b = b;
        }

        public Variant Eval(Environment env)
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)b;
        }

        private readonly bool b;
    }
}
