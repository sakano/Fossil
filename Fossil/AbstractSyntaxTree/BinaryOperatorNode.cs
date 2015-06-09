using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class BinaryOperatorNode : INode
    {
        public BinaryOperatorNode(OperatorToken token, INode leftNode, INode rightNode)
        {
            Contract.Requires<ArgumentNullException>(token != null);
            Contract.Requires<ArgumentNullException>(leftNode != null);
            Contract.Requires<ArgumentNullException>(rightNode != null);
            this.token = token;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
        }

        public Variant Eval(Environment env)
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            switch (token.Value) {
                case OperatorType.Assignment:
                    VariableNode variableNode = leftNode as VariableNode;
                    if (variableNode == null) { throw new RuntimeException(token.LineNumber, "invalid lvalue"); }
                    return variableNode.Assign(env, rightNode.Eval(env));
                case OperatorType.Addition:
                    return leftNode.Eval(env) + rightNode.Eval(env);
                case OperatorType.Subtraction: {
                        Variant lhs = leftNode.Eval(env);
                        if (lhs.Type != VariantType.Integer) { throw new RuntimeException(token.LineNumber, "cannot perform subtraction on this value"); }
                        Variant rhs = rightNode.Eval(env);
                        if (rhs.Type != VariantType.Integer) { throw new RuntimeException(token.LineNumber, "cannot perform subtraction on this value"); }
                        return lhs - rhs;
                    }
                case OperatorType.Multiplication:
                    return leftNode.Eval(env) * rightNode.Eval(env);
                case OperatorType.Division: {
                        Variant lhs = leftNode.Eval(env);
                        if (lhs.Type != VariantType.Integer) { throw new RuntimeException(token.LineNumber, "cannot perform division on this value"); }
                        Variant rhs = rightNode.Eval(env);
                        if (rhs.Type != VariantType.Integer) { throw new RuntimeException(token.LineNumber, "cannot perform division on this value"); }
                        return lhs / rhs;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private readonly OperatorToken token;
        private readonly INode leftNode;
        private readonly INode rightNode;
    }
}
