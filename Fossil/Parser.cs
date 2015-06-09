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
            { OperatorType.Multiplication,       new Operator(3,  true) },
            { OperatorType.Modulus,              new Operator(3,  true) },
            { OperatorType.Division,             new Operator(3,  true) },
            { OperatorType.Addition,             new Operator(2,  true) },
            { OperatorType.Subtraction,          new Operator(2,  true) },
            { OperatorType.GreaterThan,          new Operator(1,  true) },
            { OperatorType.LessThan,             new Operator(1,  true) },
            { OperatorType.GreaterThanOrEqual,   new Operator(1,  true) },
            { OperatorType.LessThanOrEqual,      new Operator(1,  true) },
            { OperatorType.Equality,             new Operator(0,  true) },
            { OperatorType.Inequality,           new Operator(0,  true) },
            { OperatorType.LogicalAnd,           new Operator(-1, true) },
            { OperatorType.LogicalOr,            new Operator(-2, true) },
            { OperatorType.Assignment,           new Operator(-3, false) },
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
         * statement     : ifStatement | funcStatement | block | simple
         * ifStatement   : "if" "(" expression ")" ( block | simple ) [ "else" ( block | statement ) ]
         * funcStatement : "function" IDENTIFIER "(" [ arguments ] ")" block
         * arguments     : IDENTIFIER { "," IDENTIFIER }
         * simple        : ";" | expression ";"
         * block         : "{" { statement } "}"
         * expression    : literal { binary_op literal }
         * literal       : void | true | false | STRING | ["-"] NUMBER | factor
         * factor        : ( IDENTIFIER  | "(" expression ")" ) [ funcCall ]
         * funcCall      : "(" [parameters] ")"
         * parameters    : expression { "," expression }
         * binary_op     : "=" | "+" | "-" | "*" | "/" | "=="
         */

        private INode statement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Peek(0);
            if (token.Type == TokenType.Operator) {
                OperatorToken operatorToken = (OperatorToken)token;
                if (operatorToken.Value != OperatorType.LeftBrace) { throw new SyntaxException(lexer.LineNumber); }
                return block();
            } else if (token.Type == TokenType.Identifier) {
                var identifierToken = (IdentifierToken)token;
                if (identifierToken.Value == "if") { return ifStatementNode(); }
                if (identifierToken.Value == "function") { return funcStatement(); }
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
                elseNode = statement();
            }

            return new IfStatementNode(conditionNode, thenNode, elseNode);
        }

        private INode funcStatement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            var token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "function");

            token = lexer.Read();
            if (token.Type != TokenType.Identifier) { throw new SyntaxException(lexer.LineNumber); }
            var nameToken = (IdentifierToken)token;

            if (!checkNextOperator(OperatorType.LeftParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            List<IdentifierNode> argTokens;
            if (checkNextOperator(OperatorType.RightParenthesis)) {
                lexer.Read();
                argTokens = new List<IdentifierNode>();
            } else {
                argTokens = arguments();
                if (!checkNextOperator(OperatorType.RightParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
                lexer.Read();
            }

            var blockNode = block();
            return new DefineFunctionNode(new IdentifierNode(nameToken), argTokens, blockNode);
        }

        private List<IdentifierNode> arguments()
        {
            Contract.Ensures(Contract.Result<List<IdentifierNode>>() != null);
            var result = new List<IdentifierNode>();
            while(true) {
                var argToken = lexer.Read();
                if (argToken.Type != TokenType.Identifier) { throw new SyntaxException(lexer.LineNumber); }
                result.Add(new IdentifierNode((IdentifierToken)argToken));
                
                if (!checkNextOperator(OperatorType.Comma)) { break; }
                lexer.Read();
            }
            return result;
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

        private BlockNode block()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            if (!checkNextOperator(OperatorType.LeftBrace)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            var nodes = new List<INode>();
            while (!checkNextOperator(OperatorType.RightBrace)) {
                nodes.Add(statement());
            }
            lexer.Read();

            return new BlockNode(nodes);
        }

        private INode expression()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            INode rightNode = literal();
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
            INode rightNode = literal();
            Operator nextOperator;
            while ((nextOperator = peekNextBinaryOperator()) != null && nextOperator.Comparer(precedence)) {
                rightNode = expressionShift(rightNode, nextOperator.Precedence);
            }
            return new BinaryOperatorNode(operatorToken, leftNode, rightNode);
        }
        private INode literal()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Peek(0);
            if (token.Type == TokenType.Identifier) {
                var identifierToken = (IdentifierToken)token;
                if (identifierToken.Value == "void") {
                    lexer.Read();
                    return new VoidNode();
                } else if (identifierToken.Value == "true") {
                    lexer.Read();
                    return new BooleanNode(true);
                } else if (identifierToken.Value == "false") {
                    lexer.Read();
                    return new BooleanNode(false);
                }
            } else if (token.Type == TokenType.String) {
                lexer.Read();
                return new StringNode((StringToken)token);
            } else if (token.Type == TokenType.Integer) {
                lexer.Read();
                return new IntegerNode((IntegerToken)token);
            } else if (token.Type == TokenType.Operator) {
                var operatorToken = (OperatorToken)token;
                if (operatorToken.Value == OperatorType.Subtraction) {
                    lexer.Read();
                    Token nextToken = lexer.Read();
                    if (nextToken.Type != TokenType.Integer) { throw new SyntaxException(lexer.LineNumber); }
                    var integerToken = (IntegerToken)nextToken;
                    if (integerToken.Value == Int32.MinValue) { throw new SyntaxException(lexer.LineNumber); }
                    return new IntegerNode(new IntegerToken(integerToken.LineNumber, -integerToken.Value));
                }
            }
            return factor();
        }

        private INode factor()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Read();
            INode node;
            if (token.Type == TokenType.Identifier) {
                node = new IdentifierNode((IdentifierToken)token);

                if (!checkNextOperator(OperatorType.LeftParenthesis)) { return node; }
            } else if (token.Type == TokenType.Operator) {
                var operatorToken = (OperatorToken)token;
                if (operatorToken.Value != OperatorType.LeftParenthesis) { throw new SyntaxException(lexer.LineNumber); }

                node = expression();

                if (!checkNextOperator(OperatorType.RightParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
                lexer.Read();

                if (!checkNextOperator(OperatorType.LeftParenthesis)) { return node; }
            } else {
                throw new SyntaxException(lexer.LineNumber);
            }
            List<INode> parameters = funcCall();
            return new CallFunctionNode(node, parameters);
        }

        private List<INode> funcCall()
        {
            Contract.Ensures(Contract.Result<List<INode>>() != null);
            Contract.Assume(checkNextOperator(OperatorType.LeftParenthesis));
            lexer.Read();

            List<INode> result = new List<INode>();
            if (checkNextOperator(OperatorType.RightParenthesis)) {
                lexer.Read();
                return result;
            }

            result.Add(expression());

            while (checkNextOperator(OperatorType.Comma)) {
                lexer.Read();
                result.Add(expression());
            }

            if (!checkNextOperator(OperatorType.RightParenthesis)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();

            return result;
        }

        private bool checkNextOperator(OperatorType type)
        {
            Token token = lexer.Peek(0);
            return token.Type == TokenType.Operator && ((OperatorToken)token).Value == type;
        }
    }
}
