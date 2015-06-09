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

        public Variant Eval(Environment env)
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            Variant result = new Variant();
            Environment newEnv = new Environment(env);
            foreach (var node in nodes) {
                Contract.Assume(node != null);
                result = node.Eval(newEnv);
            }
            return result;
        }

        private readonly IEnumerable<INode> nodes;
    }
}
