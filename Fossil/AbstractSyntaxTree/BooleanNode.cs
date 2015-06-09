using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class BooleanNode : INode
    {
        public BooleanNode(bool b)
        {
            this.b = b;
        }

        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)b;
        }

        private readonly bool b;
    }
}
