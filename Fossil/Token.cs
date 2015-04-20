﻿namespace Fossil
{
    internal enum TokenType
    {
        EOF,
        Number,
        String,
        Identifier,
        Operator,
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

    internal class NumberToken : Token
    {
        public NumberToken(int lineNumber, int value)
            : base(lineNumber) {
            this.value = value;
        }

        public override TokenType Type {
            get {
                return TokenType.Number;
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

    internal class OperatorToken : StringToken
    {
        public OperatorToken(int lineNumber, string value)
            : base(lineNumber, value) {
        }

        public override TokenType Type {
            get {
                return TokenType.Operator;
            }
        }
    }
}