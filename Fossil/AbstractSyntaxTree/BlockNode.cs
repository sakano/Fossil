using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class BlockNode : INode
    {
        public BlockNode(IEnumerable<INode> nodes)
        {
            Contract.Requires(nodes != null);
            this.nodes = nodes;
        }

        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            Variant result = new Variant();

            foreach (var node in nodes) {
                Contract.Assume(node != null);
                result = node.eval();
            }
            return result;
        }

        private readonly IEnumerable<INode> nodes;
    }
}
