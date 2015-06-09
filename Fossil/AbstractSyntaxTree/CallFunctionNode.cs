using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class CallFunctionNode : INode
    {
        public CallFunctionNode(INode funcName, List<INode> parameters)
        {
            Contract.Requires<ArgumentNullException>(funcName != null);
            Contract.Requires<ArgumentNullException>(parameters != null);
            this.funcName = funcName;
            this.parameters = parameters;
        }

        public Variant Eval(Environment env)
        {
            Variant variant = funcName.Eval(env);
            return variant.CallFunction(env, parameters);
        }

        private readonly INode funcName;
        private readonly List<INode> parameters;
    }
}