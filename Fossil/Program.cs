using System;
using System.IO;
using System.Diagnostics.Contracts;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    class Program
    {
        static void Main()
        {
            string filename = "testScript.txt";
            testLexer(filename);
            testParser(filename);
            Console.ReadLine();
        }

        static void testLexer(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename));
            using (StreamReader reader = new StreamReader(filename)) {
                Lexer lexer = new Lexer(reader);
                for (Token token = lexer.Read(); token.Type != TokenType.EOF; token = lexer.Read()) {
                    switch (token.Type) {
                        case TokenType.Identifier: {
                                var t = (IdentifierToken)token;
                                System.Console.WriteLine("Line:{0}, Type:{1}, Value:{2}", t.LineNumber, t.Type, t.Value);
                                break;
                            }
                        case TokenType.Integer: {
                                var t = (IntegerToken)token;
                                System.Console.WriteLine("Line:{0}, Type:{1}, Value:{2}", t.LineNumber, t.Type, t.Value);
                                break;
                            }
                        case TokenType.Operator: {
                                var t = (OperatorToken)token;
                                System.Console.WriteLine("Line:{0}, Type:{1}, Value:{2}", t.LineNumber, t.Type, t.Value);
                                break;
                            }
                        case TokenType.String: {
                                var t = (StringToken)token;
                                System.Console.WriteLine("Line:{0}, Type:{1}, Value:{2}", t.LineNumber, t.Type, t.Value);
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        static void testParser(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename));
            Environment globalEnvironment = new Environment(null);
            using (StreamReader reader = new StreamReader(filename)) {
                Parser parser = new Parser(reader);
                for (INode node = parser.Read(); node != null; node = parser.Read()) {
                    System.Console.WriteLine(node.Eval(globalEnvironment));
                }
            }
        }
    }
}
