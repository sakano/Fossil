using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fossil
{
    internal class Lexer
    {
        public Lexer(TextReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            this.reader = reader;
        }


        public Token Read()
        {
            Contract.Ensures(Contract.Result<Token>() != null);
            Token token = getAt(0);
            if (token == null) {
                return new EOFToken(lineNumber);
            }
            tokens.RemoveFirst();
            return token;
        }

        public Token Peek(int index)
        {
            Contract.Requires(index >= 0);
            Token token = getAt(index);
            if (token == null) {
                return new EOFToken(lineNumber);
            }
            return token;
        }

        public int LineNumber { get { return lineNumber; } }

        private readonly TextReader reader;
        private readonly LinkedList<Token> tokens = new LinkedList<Token>();

        private int lineNumber = 0;
        private bool hasReadToEnd = false;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.reader != null);
            Contract.Invariant(this.lineNumber >= 0);
        }

        private readonly Regex regex = new Regex(
            @"\s*" +
            @"(?<comment>//.*)|" +
            @"(?<integer>[0-9]+)|" +
            @"(?<identifier>[_a-zA-Z][_a-zA-Z0-9]*)|" +
            @"(?<operator>;|\(|\)|{|}|=|\+|-|\*|/)|" +
            "\"(?<string>.+?)\"",
            RegexOptions.Compiled);

        private readonly Dictionary<string, OperatorType> operatorTypes = new Dictionary<string, OperatorType> {
            { ";", OperatorType.Semicolon },
            { "(", OperatorType.LeftParenthesis },
            { ")", OperatorType.RightParenthesis },
            { "{", OperatorType.LeftBrace },
            { "}", OperatorType.RightBrace },
            { "=", OperatorType.Assignment },
            { "+", OperatorType.Addition },
            { "-", OperatorType.Subtraction },
            { "*", OperatorType.Multiplication },
            { "/", OperatorType.Division },
        };

        private Token getAt(int index)
        {
            Contract.Requires(index >= 0);
            while (!hasReadToEnd && tokens.Count <= index) {
                readLine();
            }

            if (tokens.Count > index) {
                Contract.Assume(Enumerable.Count(tokens) > index);
                return tokens.ElementAt(index);
            }
            return null;
        }

        private void readLine()
        {
            Contract.Requires(!hasReadToEnd);
            // 次の行のテキストを得る
            ++lineNumber;
            string line = reader.ReadLine();
            if (line == null) {
                hasReadToEnd = true;
                return;
            }

            // トークンの種類ごとに判別してtokensに追加
            foreach (Match match in regex.Matches(line)) {
                Contract.Assume(match != null);
                Contract.Assume(match.Groups["integer"] != null);
                Contract.Assume(match.Groups["identifier"] != null);
                Contract.Assume(match.Groups["operator"] != null);
                Contract.Assume(match.Groups["string"] != null);
                Token token = null;
                if (match.Groups["integer"].Success) {
                    token = new IntegerToken(lineNumber, Convert.ToInt32(match.Groups["integer"].Value, 10));
                } else if (match.Groups["identifier"].Success) {
                    Contract.Assume(match.Groups["identifier"].Value.Length != 0);
                    token = new IdentifierToken(lineNumber, match.Groups["identifier"].Value);
                } else if (match.Groups["operator"].Success) {
                    string key = match.Groups["operator"].Value;
                    Contract.Assume(operatorTypes.ContainsKey(key));
                    token = new OperatorToken(lineNumber, operatorTypes[key]);
                } else if (match.Groups["string"].Success) {
                    token = new StringToken(lineNumber, match.Groups["string"].Value);
                } else {
                    throw new NotImplementedException();
                }
                Contract.Assert(token != null);
                tokens.AddLast(token);
            }
        }
    }
}
