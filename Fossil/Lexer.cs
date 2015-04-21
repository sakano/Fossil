using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Fossil
{
    internal class Lexer
    {
        public Lexer(TextReader reader) {
            this.reader = reader;
        }

        public IEnumerable<string> Lines {
            get {
                return lines;
            }
        }


        public Token Read() {
            Token token = getAt(0);
            if (token == null) {
                return new EOFToken();
            }
            tokens.RemoveFirst();
            return token;
        }

        public Token Peek(int index) {
            Token token = getAt(index);
            if (token == null) {
                return new EOFToken();
            }
            return token;
        }

        private readonly TextReader reader;
        private readonly List<string> lines = new List<string>();
        private readonly LinkedList<Token> tokens = new LinkedList<Token>();

        private int lineNumber = -1;
        private bool hasReadToEnd = false;

        private readonly Regex regex = new Regex(
            @"\s*" +
            @"(?<comment>//.*)|" +
            @"(?<integer>[0-9]+)|" +
            @"(?<identifier>[_a-zA-Z][_a-zA-Z0-9]*)|" +
            @"(?<operator>\(|\)|\+|-|\*|/)|" +
            "\"(?<string>.+?)\"",
            RegexOptions.Compiled);
        
        private readonly Dictionary<string, OperatorType> operatorTypes = new Dictionary<string, OperatorType> {
            { "(", OperatorType.LeftParenthesis },
            { ")", OperatorType.RightParenthesis },
            { "+", OperatorType.Addition },
            { "-", OperatorType.Subtraction },
            { "*", OperatorType.Multiplication },
            { "/", OperatorType.Division },
        };
        
        private Token getAt(int index) {
            while (!hasReadToEnd && tokens.Count <= index) {
                readLine();
            }

            if (tokens.Count > index) {
                return tokens.ElementAt(index);
            }
            return null;
        }

        private void readLine() {
            // 次の行のテキストを得る
            ++lineNumber;
            string line = reader.ReadLine();
            if (line == null) {
                hasReadToEnd = true;
                return;
            }

            lines.Add(line);

            // トークンの種類ごとに判別してtokensに追加
            foreach (Match match in regex.Matches(line)) {
                Token token = null;
                if (match.Groups["integer"].Success) {
                    token = new IntegerToken(lineNumber, int.Parse(match.Groups["integer"].Value));
                } else if (match.Groups["identifier"].Success) {
                    token = new IdentifierToken(lineNumber, match.Groups["identifier"].Value);
                } else if (match.Groups["operator"].Success) {
                    string key = match.Groups["operator"].Value;
                    Debug.Assert(operatorTypes.ContainsKey(key));
                    token = new OperatorToken(lineNumber, operatorTypes[key]);
                } else if (match.Groups["string"].Success) {
                    token = new StringToken(lineNumber, match.Groups["string"].Value);
                }
                if (token != null) {
                    tokens.AddLast(token);
                }
            }
        }
    }
}
