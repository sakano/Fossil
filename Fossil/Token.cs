﻿using System;
using System.Diagnostics.Contracts;

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

    internal abstract class Token
    {
        private readonly int lineNumber;
        public int LineNumber
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return lineNumber;
            }
        }

        public abstract TokenType Type { get; }

        protected Token(int lineNumber)
        {
            Contract.Requires<ArgumentOutOfRangeException>(lineNumber >= 0);
            this.lineNumber = lineNumber;
        }

        [ContractInvariantMethod]
        private void ObjectInvaliant()
        {
            Contract.Invariant(this.lineNumber >= 0);
            Contract.Invariant(this.LineNumber >= 0);
        }
    }

    internal class EOFToken : Token
    {
        public EOFToken(int lineNumber)
            : base(lineNumber)
        {
            Contract.Requires<ArgumentOutOfRangeException>(lineNumber >= 0);
        }

        public override TokenType Type { get { return TokenType.EOF; } }
    }

    internal class IntegerToken : Token
    {
        public IntegerToken(int lineNumber, int value)
            : base(lineNumber)
        {
            Contract.Requires<ArgumentOutOfRangeException>(lineNumber >= 0);
            this.value = value;
        }

        public override TokenType Type { get { return TokenType.Integer; } }

        private readonly int value;
        public int Value { get { return value; } }
    }

    internal class StringToken : Token
    {
        public StringToken(int lineNumber, string value)
            : base(lineNumber)
        {
            Contract.Requires<ArgumentOutOfRangeException>(lineNumber >= 0);
            Contract.Requires<ArgumentNullException>(value != null);
            this.value = value;
        }

        public override TokenType Type { get { return TokenType.String; } }

        private readonly string value;
        public string Value
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return value;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.value != null);
        }
    }

    internal class IdentifierToken : Token
    {
        public IdentifierToken(int lineNumber, string value)
            : base(lineNumber)
        {
            Contract.Requires<ArgumentOutOfRangeException>(lineNumber >= 0);
            Contract.Requires(value != null);
            Contract.Requires(value.Length != 0);
            this.value = value;
        }

        public override TokenType Type { get { return TokenType.Identifier; } }

        private readonly string value;
        public string Value
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                Contract.Ensures(Contract.Result<string>().Length != 0);
                return value;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.value != null);
            Contract.Invariant(this.value.Length != 0);
        }
    }

    internal class OperatorToken : Token
    {
        public OperatorToken(int lineNumber, OperatorType value)
            : base(lineNumber)
        {
            Contract.Requires<ArgumentOutOfRangeException>(lineNumber >= 0);
            this.value = value;
        }

        public override TokenType Type { get { return TokenType.Operator; } }

        private readonly OperatorType value;
        public OperatorType Value { get { return value; } }
    }
}
