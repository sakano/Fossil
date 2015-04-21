namespace Fossil
{
    internal enum TokenType
    {
        EOF,
        Integer,
        String,
        Identifier,
        Operator,
    }

    internal enum OperatorType
    {
        LeftParenthesis,
        RightParenthesis,
        Addition,
        Subtraction,
        Multiplication,
        Division,
    }

    internal abstract class Token
    {
        public int LineNumber {
            get {
                return lineNumber;
            }
        }

        public abstract TokenType Type {
            get;
        }

        protected Token(int lineNumber) {
            this.lineNumber = lineNumber;
        }

        private readonly int lineNumber;
    }

    internal class EOFToken : Token
    {
        public EOFToken()
            : base(-1) {
        }

        public override TokenType Type {
            get {
                return TokenType.EOF;
            }
        }
    }

    internal class IntegerToken : Token
    {
        public IntegerToken(int lineNumber, int value)
            : base(lineNumber) {
            this.value = value;
        }

        public override TokenType Type {
            get {
                return TokenType.Integer;
            }
        }

        public int Value {
            get {
                return value;
            }
        }

        private int value;
    }

    internal class StringToken : Token
    {
        public StringToken(int lineNumber, string value)
            : base(lineNumber) {
            this.value = value;
        }

        public override TokenType Type {
            get {
                return TokenType.String;
            }
        }

        public string Value {
            get {
                return value;
            }
        }

        private string value;
    }

    internal class IdentifierToken : StringToken
    {
        public IdentifierToken(int lineNumber, string value)
            : base(lineNumber, value) {
        }

        public override TokenType Type {
            get {
                return TokenType.Identifier;
            }
        }
    }

    internal class OperatorToken : Token
    {
        public OperatorToken(int lineNumber, OperatorType value)
            : base(lineNumber) {
            this.value = value;
        }

        public override TokenType Type {
            get {
                return TokenType.Operator;
            }
        }

        public OperatorType Value {
            get {
                return value;
            }
        }

        private OperatorType value;
    }
}
