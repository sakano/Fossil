using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class VoidNode : INode
    {
        public VoidNode()
        {
        }

        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return new Variant();
        }
    }
}
