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
         * statement      : varStatement | ifStatement | forStatement | whileStatement | funcStatement | block | simple
         * varStatement   : varExpression ";"
         * varExpression  : "var" IDENTIFIER [ "=" expression  ]
         * ifStatement    : "if" "(" expression ")" statement [ "else" statement ]
         * forStatement   : "for" "(" [ varExpression | expression ] ";" [ expression ] ";" [expression] ")" statement
         * whileStatement : "while" "(" expression ")" statement
         * funcStatement  : "function" IDENTIFIER "(" [ arguments ] ")" block
         * arguments      : IDENTIFIER { "," IDENTIFIER }
         * simple         : ";" | expression ";"
         * block          : "{" { statement } "}"
         * expression     : literal { binary_op literal }
         * literal        : void | true | false | STRING | ["+"|"-"] NUMBER | factor
         * factor         : ( IDENTIFIER  | "(" expression ")" ) [ funcCall ]
         * funcCall       : "(" [parameters] ")"
         * parameters     : expression { "," expression }
         * binary_op      :  "==" | "!=" | "&&" | "||" | "<=" | ">=" | "=" | "+" | "-" | "*" | "/" | "%" | "<" | ">"
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
                if (identifierToken.Value == "var") { return varStatement(); }
                if (identifierToken.Value == "if") { return ifStatement(); }
                if (identifierToken.Value == "for") { return forStatement(); }
                if (identifierToken.Value == "while") { return whileStatement(); }
                if (identifierToken.Value == "function") { return funcStatement(); }
            }
            return simple();
        }
        
        private INode varStatement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            var node = varExpression();
            readOperator(OperatorType.Semicolon);
            return node;
        }

        private INode varExpression()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "var");

            Token varToken = lexer.Read();
            if (varToken.Type != TokenType.Identifier) { throw new SyntaxException(lexer.LineNumber); }

            INode initializer;
            if (isOperator(OperatorType.Assignment)) {
                lexer.Read();
                initializer = expression();
            } else {
                initializer = new VoidNode();
            }
            
            return new DefineVariableNode((IdentifierToken)varToken, initializer);
        }
        
        private INode ifStatement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "if");

            readOperator(OperatorType.LeftParenthesis);

            INode conditionNode = expression();

            readOperator(OperatorType.RightParenthesis);

            INode thenNode = statement();

            INode elseNode = null;
            token = lexer.Peek(0);
            if (token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "else") {
                lexer.Read();
                elseNode = statement();
            }

            return new IfStatementNode(conditionNode, thenNode, elseNode);
        }

        private INode forStatement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "for");
            readOperator(OperatorType.LeftParenthesis);

            INode initializerNode = null;
            if (!isOperator(OperatorType.Semicolon)) {
                Token nextToken = lexer.Peek(0);
                if (nextToken.Type == TokenType.Identifier && ((IdentifierToken)nextToken).Value == "var") {
                    initializerNode = varExpression();
                } else {
                    initializerNode = expression();
                }
            }
            readOperator(OperatorType.Semicolon);

            INode conditionNode = null;
            if (!isOperator(OperatorType.Semicolon)) {
                conditionNode = expression();
            }
            readOperator(OperatorType.Semicolon);

            INode iteratorNode = null;
            if (!isOperator(OperatorType.RightParenthesis)) {
                iteratorNode = expression();
            }
            readOperator(OperatorType.RightParenthesis);

            INode bodyNode = statement();
            
            return new forStatementNode(initializerNode, conditionNode, iteratorNode, bodyNode);
        }
        
        private INode whileStatement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            Token token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "while");
            readOperator(OperatorType.LeftParenthesis);
            var conditionNode = expression();
            readOperator(OperatorType.RightParenthesis);
            var bodyNode = statement();
            return new whileStatementNode(conditionNode, bodyNode);
        }


        private INode funcStatement()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            var token = lexer.Read();
            Contract.Assume(token.Type == TokenType.Identifier && ((IdentifierToken)token).Value == "function");

            token = lexer.Read();
            if (token.Type != TokenType.Identifier) { throw new SyntaxException(lexer.LineNumber); }
            var nameToken = (IdentifierToken)token;

            readOperator(OperatorType.LeftParenthesis);

            List<IdentifierNode> argTokens;
            if (isOperator(OperatorType.RightParenthesis)) {
                lexer.Read();
                argTokens = new List<IdentifierNode>();
            } else {
                argTokens = arguments();
                readOperator(OperatorType.RightParenthesis);
            }

            var blockNode = block();
            return new DefineFunctionNode(nameToken, argTokens, blockNode);
        }

        private List<IdentifierNode> arguments()
        {
            Contract.Ensures(Contract.Result<List<IdentifierNode>>() != null);
            var result = new List<IdentifierNode>();
            while(true) {
                var argToken = lexer.Read();
                if (argToken.Type != TokenType.Identifier) { throw new SyntaxException(lexer.LineNumber); }
                result.Add(new IdentifierNode((IdentifierToken)argToken));
                
                if (!isOperator(OperatorType.Comma)) { break; }
                lexer.Read();
            }
            return result;
        }

        private INode simple()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            if (isOperator(OperatorType.Semicolon)) {
                lexer.Read();
                return new VoidNode();
            }

            INode node = expression();

            readOperator(OperatorType.Semicolon);

            return node;
        }

        private BlockNode block()
        {
            Contract.Ensures(Contract.Result<INode>() != null);
            readOperator(OperatorType.LeftBrace);

            var nodes = new List<INode>();
            while (!isOperator(OperatorType.RightBrace)) {
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
                if (operatorToken.Value == OperatorType.Addition) {
                    lexer.Read();
                    Token nextToken = lexer.Read();
                    if (nextToken.Type != TokenType.Integer) { throw new SyntaxException(lexer.LineNumber); }
                    var integerToken = (IntegerToken)nextToken;
                    return new IntegerNode(new IntegerToken(integerToken.LineNumber, integerToken.Value));
                }
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

                if (!isOperator(OperatorType.LeftParenthesis)) { return node; }
            } else if (token.Type == TokenType.Operator) {
                var operatorToken = (OperatorToken)token;
                if (operatorToken.Value != OperatorType.LeftParenthesis) { throw new SyntaxException(lexer.LineNumber); }

                node = expression();

                readOperator(OperatorType.RightParenthesis);

                if (!isOperator(OperatorType.LeftParenthesis)) { return node; }
            } else {
                throw new SyntaxException(lexer.LineNumber);
            }
            List<INode> parameters = funcCall();
            return new CallFunctionNode(node, parameters);
        }

        private List<INode> funcCall()
        {
            Contract.Ensures(Contract.Result<List<INode>>() != null);
            readOperator(OperatorType.LeftParenthesis);

            List<INode> result = new List<INode>();
            if (isOperator(OperatorType.RightParenthesis)) {
                lexer.Read();
                return result;
            }

            result.Add(expression());

            while (isOperator(OperatorType.Comma)) {
                lexer.Read();
                result.Add(expression());
            }

            readOperator(OperatorType.RightParenthesis);

            return result;
        }

        private bool isOperator(OperatorType type)
        {
            Token token = lexer.Peek(0);
            return token.Type == TokenType.Operator && ((OperatorToken)token).Value == type;
        }

        private void readOperator(OperatorType type)
        {
            if (!isOperator(type)) { throw new SyntaxException(lexer.LineNumber); }
            lexer.Read();
        }
    }
}
