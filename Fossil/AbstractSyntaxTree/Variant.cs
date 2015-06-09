using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal enum VariantType
    {
        Void,
        Boolean,
        Integer,
    }

    internal class Variant
    {
        public Variant()
        {
            this.type = VariantType.Void;
            this.value = 0;
        }

        public Variant(bool value)
        {
            this.type = VariantType.Boolean;
            this.value = value ? boolTrue : boolFalse;
        }

        public Variant(int value)
        {
            this.type = VariantType.Integer;
            this.value = value;
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

        public static explicit operator int(Variant variant)
        {
            Contract.Requires(variant != null);
            return variant.value;
        }

        public static explicit operator bool(Variant variant)
        {
            Contract.Requires(variant != null);
            return variant.value != boolFalse;
        }

        public static Variant operator +(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs + (int)rhs);
        }

        public static Variant operator -(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs - (int)rhs);
        }

        public static Variant operator *(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return (Variant)((int)lhs * (int)rhs);
        }

        public static Variant operator /(Variant lhs, Variant rhs)
        {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
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
            return String.Format("[{0}]{1}", type.ToString(), value);
        }

        private readonly int value;
        private readonly VariantType type;

        private const int boolTrue = 1;
        private const int boolFalse = 0;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
        }
    }
}
