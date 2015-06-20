using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class whileStatementNode : INode
    {
        public whileStatementNode(INode conditionNode, INode bodyNode)
        {
            Contract.Requires<ArgumentNullException>(conditionNode != null);
            Contract.Requires<ArgumentNullException>(bodyNode != null);
            this.conditionNode = conditionNode;
            this.bodyNode = bodyNode;
        }

        public Variant Eval(Environment env)
        {
            Variant result = null;
            while ((bool)conditionNode.Eval(env)) {
                result = bodyNode.Eval(env);
            }
            return result ?? new Variant();
        }

        private readonly INode conditionNode;
        private readonly INode bodyNode;
    }
}
