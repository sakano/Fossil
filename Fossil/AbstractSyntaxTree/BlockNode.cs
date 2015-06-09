﻿using System;
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
            Environment newEnv = new Environment(env);
            return EvalWithoutNewEnvironment(newEnv);
        }

        public Variant EvalWithoutNewEnvironment(Environment env)
        {
            Contract.Requires<ArgumentNullException>(env != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            Variant result = null;
            foreach (var node in nodes) {
                Contract.Assume(node != null);
                result = node.Eval(env);
            }
            return result ?? new Variant();
        }

        private readonly IEnumerable<INode> nodes;
    }
}
