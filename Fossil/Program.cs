using System.IO;
using System.Linq;

namespace Fossil
{
    class Program
    {
        static void Main(string[] args) {
            using (StreamReader reader = new StreamReader("testScript.txt")) {
                Lexer lexer = new Lexer(reader);

                Token token = lexer.Read();
                while (token.Type != TokenType.EOF) {
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
                    }
                    token = lexer.Read();
                }
                

            }
            using (StreamReader reader = new StreamReader("testScript.txt")) {
                Parser parser = new Parser(reader);

                INode node = parser.read();
                while (node != null) {
                    System.Console.WriteLine((int)node.eval());
                    node = parser.read();
                }
            }

            System.Console.WriteLine("== End of file ==");
        }
    }
}
