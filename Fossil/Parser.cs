using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    internal class Parser
    {
        public Parser(TextReader reader) {
            lexer = new Lexer(reader);
        }

        public INode read() {
            if (lexer.Peek(0).Type == TokenType.EOF) {
                return null;
            }
            return expression();
        }

        public IEnumerable<string> Lines {
            get {
                return lexer.Lines;
            }
        }

        private readonly Lexer lexer;

        private INode expression() {
            INode leftNode = term();
            while (checkNextOperator(OperatorType.Addition) || checkNextOperator(OperatorType.Subtraction)) {
                var operatorToken = (OperatorToken)lexer.Read();
                INode rightNode = term();
                leftNode = new BinaryOperatorNode(operatorToken, leftNode, rightNode);
            }
            return leftNode;
        }

        private INode term() {
            INode leftNode = factor();
            while (checkNextOperator(OperatorType.Multiplication) || checkNextOperator(OperatorType.Division)) {
                var operatorToken = (OperatorToken)lexer.Read();
                var rightNode = factor();
                leftNode = new BinaryOperatorNode(operatorToken, leftNode, rightNode);
            }
            return leftNode;
        }

        private INode factor() {
            if (checkNextOperator(OperatorType.LeftParenthesis)) {
                lexer.Read();
                INode node = expression();
                Debug.Assert(checkNextOperator(OperatorType.RightParenthesis));
                lexer.Read();
                return node;
            } else {
                var token = lexer.Read();
                Debug.Assert(token.Type == TokenType.Integer, token.ToString());
                return new IntegerNode((IntegerToken)token);
            }
        }

        private bool checkNextOperator(OperatorType type) {
            Token token = lexer.Peek(0);
            if (token.Type != TokenType.Operator) {
                return false;
            }

            OperatorToken operatorToken = (OperatorToken)token;
            return operatorToken.Value == type;
        }
    }
}
