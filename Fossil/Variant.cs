using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Fossil
{
    internal enum VariantType
    {
        Void,
        Boolean,
        Integer,
        String,
    }

    internal class Variant
    {
        public Variant()
        {
            this.type = VariantType.Void;
            this.intValue = 0;
        }

        public Variant(bool value)
        {
            this.type = VariantType.Boolean;
            this.intValue = value ? boolTrue : boolFalse;
        }

        public Variant(int value)
        {
            this.type = VariantType.Integer;
            this.intValue = value;
        }

        public Variant(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            this.type = VariantType.String;
            this.intValue = 0;
            this.stringValue = value;
        }

        public static explicit operator Variant(int value)
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return new Variant(value);
        }

        public static explicit operator Variant(bool value)
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return new Variant(value);
        }

        public static explicit operator Variant(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return new Variant(value);
        }

        public static explicit operator int(Variant variant)
        {
            Contract.Requires(variant != null);
            switch (variant.type) {
                case VariantType.Integer:
                    return variant.intValue;
                case VariantType.Boolean:
                    return variant.intValue == boolTrue ? 1 : 0;
                case VariantType.String:
                case VariantType.Void:
                    return 0;
                default:
                    throw new NotImplementedException();
            }
        }

        public static explicit operator bool(Variant variant)
        {
            Contract.Requires(variant != null);
            switch (variant.type) {
                case VariantType.Boolean:
                    return variant.intValue == boolTrue;
                case VariantType.String:
                case VariantType.Void:
                case VariantType.Integer:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        public static explicit operator string(Variant variant)
        {
            Contract.Requires(variant != null);
            Contract.Ensures(Contract.Result<string>() != null);
            switch (variant.type) {
                case VariantType.String:
                    Contract.Assume(variant.stringValue != null);
                    return variant.stringValue;
                case VariantType.Boolean:
                    Contract.Assume(variant.intValue == boolTrue || variant.intValue == boolFalse);
                    return variant.intValue == boolTrue ? "true" : "false";
                case VariantType.Void:
                    return "void";
                case VariantType.Integer:
                    return variant.intValue.ToString();
                default:
                    throw new NotImplementedException();
            }
        }

        public static Variant operator +(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            switch (lhs.type) {
                case VariantType.String:
                    return (Variant)((string)lhs + (string)rhs);
                case VariantType.Integer:
                case VariantType.Boolean:
                case VariantType.Void:
                    return (Variant)((int)lhs + (int)rhs);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Variant operator -(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Requires(lhs.Type == VariantType.Integer);
            Contract.Requires(rhs.Type == VariantType.Integer);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs - (int)rhs);
        }

        public static Variant operator *(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            switch (lhs.type) {
                case VariantType.String:
                    int count = (int)rhs;
                    if (count == 0) { return new Variant(""); }
                    if (count < 0) { throw new RuntimeException(-1, ""); }
                    var str = string.Concat(Enumerable.Repeat((string)lhs, count));
                    Contract.Assume(str != null);
                    return (Variant)str;
                case VariantType.Boolean:
                case VariantType.Void:
                case VariantType.Integer:
                    return (Variant)((int)lhs * (int)rhs);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Variant operator /(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Requires(lhs.Type == VariantType.Integer);
            Contract.Requires(rhs.Type == VariantType.Integer);
            Contract.Ensures(Contract.Result<Variant>() != null);
            int rhsValue = (int)rhs;
            if (rhsValue == 0) {
                throw new DivideByZeroException();
            }
            int lhsValue = (int)lhs;
            if (lhsValue == Int32.MinValue && rhsValue == -1) {
                throw new OverflowException();
            }
            return (Variant)(lhsValue / rhsValue);
        }

        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            switch (type) {
                case VariantType.String:
                    return "[string]" + stringValue;
                case VariantType.Boolean:
                    return "[bool]" + (intValue == boolTrue ? "true" : "false");
                case VariantType.Void:
                    return "[void]";
                case VariantType.Integer:
                    return "[int]" + intValue;
                default:
                    throw new NotImplementedException();
            }
        }

        private readonly int intValue;
        private readonly string stringValue = null;
        private readonly VariantType type;
        public VariantType Type { get { return this.type; } }

        private const int boolTrue = 1;
        private const int boolFalse = 0;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(stringValue != null || type != VariantType.String);
        }
    }
}
