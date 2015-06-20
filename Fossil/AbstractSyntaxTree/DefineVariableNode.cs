using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class DefineVariableNode : INode
    {
        public DefineVariableNode(IdentifierToken varName, INode initializer)
        {
            Contract.Requires<ArgumentNullException>(varName != null);
            Contract.Requires<ArgumentNullException>(initializer != null);
            this.varName = varName;
            this.initializer = initializer;
        }

        public Variant Eval(Environment env)
        {
            var variable = initializer.Eval(env);
            if (!env.AssignNew(varName.Value, variable)) {
                throw new RuntimeException(varName.LineNumber, "variable already defined");
            }
            return variable;
        }

        private readonly IdentifierToken varName;
        private readonly INode initializer;
    }
}