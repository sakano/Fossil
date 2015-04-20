using System.IO;

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
                        case TokenType.Number: {
                                var t = (NumberToken)token;
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

            System.Console.WriteLine("== End of file ==");
        }
    }
}
