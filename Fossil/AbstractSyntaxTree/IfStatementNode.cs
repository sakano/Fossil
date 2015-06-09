using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class IfStatementNode : INode
    {
        public IfStatementNode(INode conditionNode, INode thenNode, INode elseNode)
        {
            Contract.Requires(conditionNode != null);
            Contract.Requires(thenNode != null);
            this.conditionNode = conditionNode;
            this.thenNode = thenNode;
            this.elseNode = elseNode;
        }

        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            if ((bool)conditionNode.eval()) {
                return thenNode.eval();
            } else if (elseNode == null) {
                return new Variant();
            } else {
                return elseNode.eval();
            }
        }

        private readonly INode conditionNode;
        private readonly INode thenNode;
        private readonly INode elseNode;
    }
}
