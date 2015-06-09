using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class VoidNode : INode
    {
        public VoidNode()
        {
        }

        public Variant Eval(Environment env)
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return new Variant();
        }
    }
}
