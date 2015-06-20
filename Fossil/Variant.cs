using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    internal enum VariantType
    {
        Void,
        Boolean,
        Integer,
        String,
        Function,
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

        public Variant(ICallableObject value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            this.type = VariantType.Function;
            this.funcValue = value;
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
                case VariantType.Function:
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
                case VariantType.Function:
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
                case VariantType.Function:
                    Contract.Assume(variant.funcValue != null);
                    return variant.funcValue.Name;
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
                case VariantType.Function:
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
                case VariantType.Function:
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

        public static Variant operator %(Variant lhs, Variant rhs)
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
            return (Variant)(lhsValue % rhsValue);
        }

        public static Variant operator <(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Requires(lhs.Type == VariantType.Integer);
            Contract.Requires(rhs.Type == VariantType.Integer);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs < (int)rhs);
        }


        public static Variant operator >(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Requires(lhs.Type == VariantType.Integer);
            Contract.Requires(rhs.Type == VariantType.Integer);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs > (int)rhs);
        }

        public static Variant operator <=(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Requires(lhs.Type == VariantType.Integer);
            Contract.Requires(rhs.Type == VariantType.Integer);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs <= (int)rhs);
        }

        public static Variant operator >=(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Requires(lhs.Type == VariantType.Integer);
            Contract.Requires(rhs.Type == VariantType.Integer);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs >= (int)rhs);
        }

        public bool Equals(Variant rhs)
        {
            if (rhs == null) { return false; }
            if (type != rhs.type) { return false; }
            switch (type) {
                case VariantType.String:
                    return stringValue.Equals(rhs.stringValue);
                case VariantType.Function:
                    return (funcValue == rhs.funcValue);
                case VariantType.Integer:
                    return (intValue == rhs.intValue);
                case VariantType.Boolean:
                    return (intValue == rhs.intValue);
                case VariantType.Void:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        public Variant CallFunction(Environment env, List<INode> parameters)
        {
            Contract.Requires<ArgumentNullException>(env != null);
            Contract.Requires<ArgumentNullException>(parameters != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            Contract.Assume(type == VariantType.Function);
            Contract.Assert(funcValue != null);
            return funcValue.Call(env, parameters);
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
                case VariantType.Function:
                    return funcValue.ToString();
                default:
                    throw new NotImplementedException();
            }
        }

        private readonly int intValue;
        private readonly string stringValue = null;
        private readonly ICallableObject funcValue = null;
        
        private readonly VariantType type;
        public VariantType Type { get { return this.type; } }

        private const int boolTrue = 1;
        private const int boolFalse = 0;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(stringValue != null || type != VariantType.String);
            Contract.Invariant(funcValue != null || type != VariantType.Function);
        }
    }
}
