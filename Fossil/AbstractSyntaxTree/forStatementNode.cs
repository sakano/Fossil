using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class forStatementNode : INode
    {
        public forStatementNode(INode initializer, INode condition, INode iterator, INode body)
        {
            Contract.Requires<ArgumentNullException>(initializer != null);
            Contract.Requires<ArgumentNullException>(condition != null);
            Contract.Requires<ArgumentNullException>(iterator != null);
            Contract.Requires<ArgumentNullException>(body != null);
            this.initializerNode = initializer;
            this.conditionNode = condition;
            this.iteratorNode = iterator;
            this.bodyNode = body;
        }

        public Variant Eval(Environment env)
        {
            Environment newEnv = new Environment(env);
            Variant result = null;
            initializerNode.Eval(newEnv);
            while ((bool)conditionNode.Eval(newEnv)) {
                result = bodyNode.Eval(newEnv);
                iteratorNode.Eval(newEnv);
            }
            return result ?? new Variant();
        }

        private readonly INode initializerNode;
        private readonly INode conditionNode;
        private readonly INode iteratorNode;
        private readonly INode bodyNode;
    }
}
