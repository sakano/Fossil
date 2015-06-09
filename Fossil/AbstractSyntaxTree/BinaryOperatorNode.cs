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

        private void checkVariantIsIntegers(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            if (lhs.Type != VariantType.Integer || rhs.Type != VariantType.Integer) {
                throw new RuntimeException(token.LineNumber, "cannot calculate with this value");
            }
        }

        public Variant Eval(Environment env)
        {
            switch (token.Value) {
                case OperatorType.Assignment:
                    IdentifierNode variableNode = leftNode as IdentifierNode;
                    if (variableNode == null) { throw new RuntimeException(token.LineNumber, "invalid lvalue"); }
                    return variableNode.Assign(env, rightNode.Eval(env));
                case OperatorType.Addition:
                    return leftNode.Eval(env) + rightNode.Eval(env);
                case OperatorType.Subtraction: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs - rhs;
                    }
                case OperatorType.Multiplication:
                    return leftNode.Eval(env) * rightNode.Eval(env);
                case OperatorType.Division: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs / rhs;
                    }
                case OperatorType.Modulus: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs % rhs;
                    }
                case OperatorType.Equality:
                    return (Variant)leftNode.Eval(env).Equals(rightNode.Eval(env));
                case OperatorType.Inequality:
                    return (Variant)!leftNode.Eval(env).Equals(rightNode.Eval(env));
                case OperatorType.LogicalAnd:
                    return (Variant)((bool)leftNode.Eval(env) && (bool)rightNode.Eval(env));
                case OperatorType.LogicalOr:
                    return (Variant)((bool)leftNode.Eval(env) || (bool)rightNode.Eval(env));
                case OperatorType.GreaterThan: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs < rhs;
                    }
                case OperatorType.LessThan: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs > rhs;
                    }
                case OperatorType.GreaterThanOrEqual: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs <= rhs;
                    }
                case OperatorType.LessThanOrEqual: {
                        Variant lhs = leftNode.Eval(env), rhs = rightNode.Eval(env);
                        checkVariantIsIntegers(lhs, rhs);
                        return lhs >= rhs;
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
