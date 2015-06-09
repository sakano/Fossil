using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal enum BinaryOperatorType
    {
        Addition = OperatorType.Addition,
        Subtraction = OperatorType.Subtraction,
        Multiplication = OperatorType.Multiplication,
        Division = OperatorType.Division,
    }

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

        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            switch (token.Value) {
                case OperatorType.Addition:
                    return leftNode.eval() + rightNode.eval();
                case OperatorType.Subtraction:
                    return leftNode.eval() - rightNode.eval();
                case OperatorType.Multiplication:
                    return leftNode.eval() * rightNode.eval();
                case OperatorType.Division:
                    return leftNode.eval() / rightNode.eval();
                default:
                    throw new NotImplementedException();
            }
        }

        private readonly OperatorToken token;
        private readonly INode leftNode;
        private readonly INode rightNode;
    }
}
