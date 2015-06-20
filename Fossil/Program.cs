using System;
using System.IO;
using System.Linq;
using System.Diagnostics.Contracts;
using Fossil.AbstractSyntaxTree;
using System.Collections.Generic;

namespace Fossil
{
    class Program
    {
        static void Main()
        {
            string filename = "testScript.txt";
            TestLexer(filename);
            TestParser(filename);
            Console.ReadLine();
        }

        static void TestLexer(string filename)
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

        static void TestParser(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename));
            Environment globalEnvironment = new Environment(null);
            globalEnvironment.AssignNew("output", new Variant(new NativeFunctionObject("output", Output)));
            
            using (StreamReader reader = new StreamReader(filename)) {
                Parser parser = new Parser(reader);
                for (INode node = parser.Read(); node != null; node = parser.Read()) {
                    node.Eval(globalEnvironment);
                }
            }
        }

        static Variant Output(List<Variant> args)
        {
            Contract.Requires<ArgumentNullException>(args != null);
            System.Console.WriteLine(args.Select(arg => arg.ToString()).Aggregate((arg1, arg2) => arg1 + ", " + arg2));
            return new Variant();
        }
    }
}
