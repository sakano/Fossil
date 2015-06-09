using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    internal class Parser
    {
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
         * ifStatement : "if" "(" expression ")" ( block | simple ) [ "else" ( block | statement )]
         * simple      : ";" | expression ";"
         * block       : "{" { simple } "}"
         * expression  : term | expression ("+" | "-") term
         * term        : factor | term ("*" | "/") factor
         * factor      : void | true | false | NUMBER | "(" expression ")"
         */

        private INode statement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Peek(0);
            if (token.Type == TokenType.Identifier) {
                var identifierToken = (IdentifierToken)token;
                if (identifierToken.Value != "if") { throw new SyntaxException(lexer.LineNumber); }
                return ifStatementNode();
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
            INode leftNode = term();
            while (checkNextOperator(OperatorType.Addition) || checkNextOperator(OperatorType.Subtraction)) {
                var operatorToken = (OperatorToken)lexer.Read();
                INode rightNode = term();
                leftNode = new BinaryOperatorNode(operatorToken, leftNode, rightNode);
            }
            return leftNode;
        }

        private INode term()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            INode leftNode = factor();
            while (checkNextOperator(OperatorType.Multiplication) || checkNextOperator(OperatorType.Division)) {
                var operatorToken = (OperatorToken)lexer.Read();
                var rightNode = factor();
                leftNode = new BinaryOperatorNode(operatorToken, leftNode, rightNode);
            }
            return leftNode;
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
                    throw new SyntaxException(lexer.LineNumber);
                }
            } else if (token.Type == TokenType.Integer) {
                return new IntegerNode((IntegerToken)token);
            } else if (token.Type == TokenType.Operator) {
                var operatorToken = (OperatorToken)token;
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
