using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    internal class Parser
    {
        private class Operator
        {
            public readonly int Precedence;
            public readonly bool LeftAssociative;
            public Operator(int precedence, bool LeftAssociative)
            {
                this.Precedence = precedence;
                this.LeftAssociative = LeftAssociative;
            }
            public bool Comparer(int precedence)
            {
                return this.LeftAssociative
                    ? precedence < Precedence
                    : precedence <= Precedence;
            }
        }
        private readonly Dictionary<OperatorType, Operator> binaryOperators = new Dictionary<OperatorType, Operator>() {
            { OperatorType.Assignment,     new Operator(1, false) },
            { OperatorType.Addition,       new Operator(2, true) },
            { OperatorType.Subtraction,    new Operator(2, true) },
            { OperatorType.Multiplication, new Operator(3, true) },
            { OperatorType.Division,       new Operator(3, true) },
        };
        private Operator peekNextBinaryOperator()
        {
            Token token = lexer.Peek(0);
            if (token.Type != TokenType.Operator) { return null; }
            var key = ((OperatorToken)token).Value;
            if (binaryOperators.ContainsKey(key)) {
                return binaryOperators[key];
            } else {
                return null;
            }
        }

        public Parser(TextReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            lexer = new Lexer(reader);
        }

        public INode Read()
        {
            if (lexer.Peek(0).Type == TokenType.EOF) {
                return null;
            }
            return statement();
        }
        private readonly Lexer lexer;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.lexer != null);
        }

        /*
         * statement   : ifStatement | simple
         * ifStatement : "if" "(" expression ")" ( block | simple ) [ "else" ( block | statement ) ]
         * simple      : ";" | expression ";"
         * block       : "{" { simple } "}"
         * expression  : factor { binary_op factor }
         * factor      : void | true | false | ["-"] NUMBER | IDENTIFIER | STRING | "(" expression ")"
         * binary_op   : "=" | "+" | "-" | "*" | "/"
         */

        private INode statement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Peek(0);
            if (token.Type == TokenType.Identifier) {
                var identifierToken = (IdentifierToken)token;
                if (identifierToken.Value == "if") { return ifStatementNode(); }
            }
            return simple();
        }

        private INode ifStatementNode()
        {
            Contract.Ensures(Contract.Result<INode>() != null);

            Token token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "if");

            if (!checkNextOperator(OperatorType.LeftParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            INode conditionNode = expression();

            if (!checkNextOperator(OperatorType.RightParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            INode thenNode = null;
            token = lexer.Peek(0);
            if (token.Type == TokenType.Operator && ((OperatorToken)token).Value == OperatorType.LeftBrace) {
                thenNode = block();
            } else {
                thenNode = simple();
            }
            Contract.Assert(thenNode != null);

            INode elseNode = null;
            token = lexer.Peek(0);
            if (token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "else") {
                lexer.Read();
                token = lexer.Peek(0);
                if (token.Type == TokenType.Operator && ((OperatorToken)token).Value == OperatorType.LeftBrace) {
                    elseNode = block();
                } else {
                    elseNode = statement();
                }
            }

            return new IfStatementNode(conditionNode, thenNode, elseNode);
        }

        private INode simple()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            if (checkNextOperator(OperatorType.Semicolon)) {
                lexer.Read();
                return new VoidNode();
            }

            INode node = expression();

            if (!checkNextOperator(OperatorType.Semicolon)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            return node;
        }

        private INode block()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            if (!checkNextOperator(OperatorType.LeftBrace)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            var nodes = new List<INode>();
            while (!checkNextOperator(OperatorType.RightBrace)) {
                nodes.Add(simple());
            }
            lexer.Read();

            return new BlockNode(nodes);
        }

        private INode expression()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            INode rightNode = factor();
            Operator nextOperator;
            while ((nextOperator = peekNextBinaryOperator()) != null) {
                rightNode = expressionShift(rightNode, nextOperator.Precedence);
            }
            return rightNode;
        }

        private INode expressionShift(INode leftNode, int precedence)
        {
            Contract.Requires(leftNode != null);
            Contract.Ensures(Contract.Result<INode>() != null);
            var operatorToken = (OperatorToken)lexer.Read();
            INode rightNode = factor();
            Operator nextOperator;
            while ((nextOperator = peekNextBinaryOperator()) != null && nextOperator.Comparer(precedence)) {
                rightNode = expressionShift(rightNode, nextOperator.Precedence);
            }
            return new BinaryOperatorNode(operatorToken, leftNode, rightNode);
        }

        private INode factor()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Read();
            if (token.Type == TokenType.Identifier) {
                var identifierToken = (IdentifierToken)token;
                if (identifierToken.Value == "void") {
                    return new VoidNode();
                } else if (identifierToken.Value == "true") {
                    return new BooleanNode(true);
                } else if (identifierToken.Value == "false") {
                    return new BooleanNode(false);
                } else {
                    return new VariableNode(identifierToken);
                }
            } else if (token.Type == TokenType.String) {
                return new StringNode((StringToken)token);
            } else if (token.Type == TokenType.Integer) {
                return new IntegerNode((IntegerToken)token);
            } else if (token.Type == TokenType.Operator) {
                var operatorToken = (OperatorToken)token;
                if (operatorToken.Value == OperatorType.Subtraction) {
                    Token nextToken = lexer.Read();
                    if (nextToken.Type != TokenType.Integer) { throw new SyntaxException(lexer.LineNumber); }
                    var integerToken = (IntegerToken)nextToken;
                    if (integerToken.Value == Int32.MinValue) { throw new SyntaxException(lexer.LineNumber); }
                    Contract.Assume(integerToken.LineNumber >= 0);
                    return new IntegerNode(new IntegerToken(integerToken.LineNumber, -integerToken.Value));
                }

                if (operatorToken.Value != OperatorType.LeftParenthesis) { throw new SyntaxException(lexer.LineNumber); }

                var node = expression();

                if (!checkNextOperator(OperatorType.RightParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
                lexer.Read();

                return node;
            }
            throw new SyntaxException(lexer.LineNumber);
        }

        private bool checkNextOperator(OperatorType type)
        {
            Token token = lexer.Peek(0);
            return token.Type == TokenType.Operator && ((OperatorToken)token).Value == type;
        }
    }
}
