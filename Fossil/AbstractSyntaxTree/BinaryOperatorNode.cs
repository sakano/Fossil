using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
        public BinaryOperatorNode(OperatorToken token, INode leftNode, INode rightNode) {
            this.token = token;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
        }

        public Object eval() {
            switch (token.Value) {
                case OperatorType.Addition:
                    return (int)leftNode.eval() + (int)rightNode.eval();
                case OperatorType.Subtraction:
                    return (int)leftNode.eval() - (int)rightNode.eval();
                case OperatorType.Multiplication:
                    return (int)leftNode.eval() * (int)rightNode.eval();
                case OperatorType.Division:
                    return (int)leftNode.eval() / (int)rightNode.eval();
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        private readonly OperatorToken token;
        private readonly INode leftNode;
        private readonly INode rightNode;
    }
}
