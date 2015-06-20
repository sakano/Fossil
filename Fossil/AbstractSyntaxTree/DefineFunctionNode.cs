using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class DefineFunctionNode : INode
    {
        public DefineFunctionNode(IdentifierToken funcName, List<IdentifierNode> argNames, BlockNode body)
        {
            Contract.Requires<ArgumentNullException>(funcName != null);
            Contract.Requires<ArgumentNullException>(argNames != null);
            Contract.Requires<ArgumentNullException>(body != null);
            this.funcName = funcName;
            this.argNames = argNames;
            if (argNames.Count() != argNames.Distinct().Count()) {
                throw new SyntaxException(funcName.LineNumber);
            }
            this.body = body;
        }

        public Variant Eval(Environment env)
        {
            var variant = new Variant(new FunctionObject(funcName, argNames, body));
            if (!env.AssignNew(funcName.Value, variant)) {
                throw new RuntimeException(funcName.LineNumber, "variable already defined");
            }
            return variant;
        }

        private readonly IdentifierToken funcName;
        private readonly List<IdentifierNode> argNames;
        private readonly BlockNode body;
    }
}