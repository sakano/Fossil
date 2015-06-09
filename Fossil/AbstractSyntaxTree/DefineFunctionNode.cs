using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class DefineFunctionNode : INode
    {
        public DefineFunctionNode(IdentifierNode funcName, List<IdentifierNode> argNames, BlockNode body)
        {
            Contract.Requires<ArgumentNullException>(funcName != null);
            Contract.Requires<ArgumentNullException>(argNames != null);
            Contract.Requires<ArgumentNullException>(body != null);
            this.funcName = funcName;
            this.argNames = argNames;
            this.body = body;
        }

        public Variant Eval(Environment env)
        {
            var variant = new Variant(new FunctionObject(funcName, argNames, body));
            env.Assign(funcName.Name, variant);
            return variant;
        }

        private readonly IdentifierNode funcName;
        private readonly List<IdentifierNode> argNames;
        private readonly BlockNode body;
    }
}