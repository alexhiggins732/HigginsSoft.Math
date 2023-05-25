/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using MathGmp.Native;
using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace HigginsSoft.Math.Lib
{
    public partial class GmpInt : IDisposable,
        ICloneable,
        IComparable,
        IComparable<GmpInt>,
        IEquatable<GmpInt>
    {

        public readonly mpz_t Data = new();



        private static readonly GmpInt s_one = new GmpInt(1);
        private static readonly GmpInt s_zero = new GmpInt(0);
        private static readonly GmpInt s_one_neg = new GmpInt(-1);

        private static readonly GmpInt s_two = new GmpInt(2);
        private static readonly GmpInt s_three = new GmpInt(3);
        private static readonly GmpInt s_five = new GmpInt(5);
        private static readonly GmpInt s_ten = new GmpInt(10);

        private static readonly GmpInt s_two_neg = new GmpInt(-2);
        private static readonly GmpInt s_three_neg = new GmpInt(-3);
        private static readonly GmpInt s_five_neg = new GmpInt(-5);
        private static readonly GmpInt s_ten_neg = new GmpInt(-10);

        #region constructors

        /// Initializes a new GmpInt to 0.
        public GmpInt()
        {
            gmp_lib.mpz_init_set_si(Data, 0);
        }

        public GmpInt(mpz_t value)
        {
            gmp_lib.mpz_init_set(Data, value);
        }

        public GmpInt(mpf_t value)
            : this()
        {
            gmp_lib.mpz_set_f(Data, value);
        }


        public GmpInt(GmpInt value)
            : this(value.Data)
        {

        }

        public GmpInt(GmpFloat value)
            : this(value.Data)
        {

        }

        public GmpInt(int value)
        {
            if (value > -1)
            {
                gmp_lib.mpz_init_set_si(Data, value);
            }
            else
            {
                //linux bug fix. gmp_lib initializes negative values a uint even when call set_si.
                gmp_lib.mpz_init_set_ui(Data, (uint)-value);
                gmp_lib.mpz_neg(Data, Data);
            }
        }

        public GmpInt(uint value)
        {
            gmp_lib.mpz_init_set_ui(Data, value);
        }

        public GmpInt(long value)
            : this(value.ToString())
        {
        }

        public GmpInt(ulong value)
            : this(value.ToString())
        {
        }

        public GmpInt(double value)
        {
            gmp_lib.mpz_init_set_d(Data, value);
        }


        public GmpInt(string value, int @base = 10)
        {
            char_ptr ptr = new char_ptr(value);
            gmp_lib.mpz_init_set_str(Data, ptr, @base);
            gmp_lib.free(ptr);
        }

        public GmpInt(BigInteger value)
            : this(value.ToString())
        {

        }

        public GmpInt(decimal value)
            : this(value.ToString().Split('.')[0])
        {

        }

        #endregion

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int @base = 10)
        {
            if (Data.Pointer == IntPtr.Zero) return "uninitialized";
            if (IsZero) return "0";
            char_ptr s = gmp_lib.mpz_get_str(char_ptr.Zero, @base, Data);
            var value = s.ToString().Replace(" ", "").TrimStart('0');
            return value;
        }

        #region public properties

        public ulong BitCount => gmp_lib.mpz_sizeinbase(Data, 2);
        public ulong DigitCount => gmp_lib.mpz_sizeinbase(Data, 10);

        public static GmpInt Zero
        {
            get { return s_zero.Clone(); }
        }

        public static GmpInt One
        {
            get { return s_one.Clone(); }
        }

        public static GmpInt Two
        {
            get { return s_two.Clone(); }
        }

        public static GmpInt Three
        {
            get { return s_three.Clone(); }
        }

        public static GmpInt Five
        {
            get { return s_five.Clone(); }
        }

        public static GmpInt Ten
        {
            get { return s_ten.Clone(); }
        }


        public static GmpInt MinusOne
        {
            get { return s_one_neg.Clone(); }
        }

        public static GmpInt MinusTwo
        {
            get { return s_two_neg.Clone(); }
        }

        public static GmpInt MinusThree
        {
            get { return s_three_neg.Clone(); }
        }

        public static GmpInt MinusFive
        {
            get { return s_five_neg.Clone(); }
        }

        public static GmpInt MinusTen
        {
            get { return s_ten_neg.Clone(); }
        }

        public bool IsPowerOfTwo
        {
            get
            {
                if (PopCount != 1) return false;
                var test = this & (this - 1);
                return test.IsZero;
            }
        }

        public uint PopCount => gmp_lib.mpz_popcount(Data).Value;
        public bool IsZero => Sign == 0;

        public bool IsOne => Sign == 1 && gmp_lib.mpz_cmp_si(Data, 1) == 0;

        public bool IsEven { get => IsZero || gmp_lib.mpz_even_p(Data) > 0; }
        public bool IsOdd { get => !IsZero && gmp_lib.mpz_odd_p(Data) > 0; }

        public int Sign => gmp_lib.mpz_sgn(Data);

        #endregion

        #region public methods

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public GmpInt Clone()
        {
            return new GmpInt(this);
        }

        public int CompareTo(object? other)
        {
            if (other == null)
                return 1;
            if (other is GmpIntConvertible convertible)
                return CompareTo(convertible);
            throw new ArgumentException("Object must be a GmpInt or GmpIntConvertible", nameof(other));
        }

        public int CompareTo(GmpInt? other)
            => other is null ? 1 : gmp_lib.mpz_cmp(this.Data, other.Data);

        public int CompareTo(int other)
        {
            if (other > -1)
            {
                return gmp_lib.mpz_cmp_si(this.Data, other);
            }
            else
            {
                //Linux bug, signed ints are converted to uint.
                return gmp_lib.mpz_cmp(this.Data, (GmpInt)other);
            }
        }


        public int CompareTo(uint other)
            => gmp_lib.mpz_cmp_ui(this.Data, other);

        public int CompareTo(long other)
           => CompareTo((GmpInt)other);

        public int CompareTo(ulong other)
           => CompareTo((GmpInt)other);

        public int CompareTo(decimal other)
            => CompareTo((GmpInt)other);

        public int CompareTo(double other)
            => gmp_lib.mpz_cmp_d(this.Data, other);

        public int CompareTo(BigInteger other)
            => CompareTo((GmpInt)other);

        #endregion

        #region public instance methods

        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is GmpInt other))
                return false;
            return Equals(other);
        }

        public bool Equals(GmpInt? other)
        {
            if (other is null) return false;
            if (Sign != other.Sign) return false;
            if (BitCount != other.BitCount) return false;
            return this == other;

        }




        public bool Equals(int other) => this == other;

        public bool Equals(uint other) => this == other;

        public bool Equals(double other) => this == other;

        public bool Equals(long other) => this.Equals((GmpInt)other);

        public bool Equals(ulong other) => this.Equals((GmpInt)other);


        public Byte[] Bytes()
        {
            byte[] result = new byte[BitCount / 8 + 1];
            void_ptr data = gmp_lib.allocate((size_t)result.Length);
            size_t countp = 0;
            gmp_lib.mpz_export(data, ref countp, -1, 1, 1, 0, Data);
            Marshal.Copy(data.ToIntPtr(), result, 0, result.Length);

            return result;

        }


        public ulong[] RawData()
        {
            var data = gmp_lib.mpz_limbs_read(Data);
            return data.Select(x => (ulong)x).ToArray();
        }

        public override int GetHashCode()
        {

            int hashCode = HashCodeHelper.CreateFromByteArray(Bytes()).GetHashCode();
            return hashCode;
        }

        internal static class HashCodeHelper
        {
            /// <summary>
            /// Combines two hash codes, useful for combining hash codes of individual vector elements
            /// </summary>
            internal static int CombineHashCodes(int h1, int h2)
            {
                return (((h1 << 5) + h1) ^ h2);
            }

            public static int CreateFromByteArray(byte[] bytes)
            {
                int hashCode = 17;
                for (int i = 0; i < bytes.Length; i++)
                {
                    hashCode = HashCodeHelper.CombineHashCodes(hashCode, bytes[i].GetHashCode());
                }
                return hashCode;
            }
        }

        #endregion

        #region static methods

        #region public static operators

        public static implicit operator GmpInt(string value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(Byte value)
        {
            return new GmpInt(value);
        }
        public static implicit operator GmpInt(SByte value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(Int16 value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(UInt16 value)
        {
            return new GmpInt(value);
        }


        public static implicit operator GmpInt(int value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(uint value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(long value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(ulong value)
        {
            return new GmpInt(value);
        }

        public static explicit operator GmpInt(float value)
        {
            return new GmpInt(value);
        }

        public static explicit operator GmpInt(double value)
        {
            return new GmpInt(value);
        }

        public static explicit operator GmpInt(decimal value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(BigInteger value)
        {
            return new GmpInt(value);
        }

        public static implicit operator GmpInt(mpz_t value)
        {
            return new GmpInt(value);
        }

        public static implicit operator string(GmpInt value)
        {
            return value.ToString();
        }

        public static explicit operator byte(GmpInt value)
        {
            return checked((byte)((int)value));
        }

        public static explicit operator sbyte(GmpInt value)
        {
            return checked((sbyte)((int)value));
        }

        public static explicit operator short(GmpInt value)
        {
            return checked((short)((int)value));
        }

        public static explicit operator ushort(GmpInt value)
        {
            return checked((ushort)((int)value));
        }

        public static explicit operator int(GmpInt value)
        {

            if (value.Sign < 0)
            {
                var a = +value;
                var result = gmp_lib.mpz_get_ui(a.Data);
                return -(int)result;
            }
            return gmp_lib.mpz_get_si(value.Data);
        }


        public static explicit operator uint(GmpInt value)
        {
            return gmp_lib.mpz_get_ui(value.Data);
        }

        public static explicit operator long(GmpInt value)
        {
            if (value.IsZero) return 0L;
            return (long)((BigInteger)value);
        }

        public static explicit operator ulong(GmpInt value)
        {
            if (value.IsZero) return 0UL;
            return (ulong)((BigInteger)value);
        }



        public static explicit operator BigInteger(GmpInt value)
        {
            if (value.IsZero) return BigInteger.Zero;
            var base10 = value.ToString().Replace(" ", "");
            var b = BigInteger.Parse(base10);
            return b;
        }

        public static explicit operator float(GmpInt value)
        {
            return (float)(double)value;
        }

        public static explicit operator double(GmpInt value)
        {
            return gmp_lib.mpz_get_d(value.Data);
        }

        public static explicit operator decimal(GmpInt value)
        {
            if (value.IsZero) return 0M;
            return (decimal)(BigInteger)value;
        }

        public static implicit operator mpz_t(GmpInt value)
        {
            return value.Clone().Data;
        }

        public static GmpInt operator &(GmpInt left, GmpInt right)
        {

            if (left.IsZero || right.IsZero)
            {
                return GmpInt.Zero.Clone();
            }

            var result = new GmpInt();
            gmp_lib.mpz_and(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpInt operator |(GmpInt left, GmpInt right)
        {
            if (left.IsZero)
                return right.Clone();
            if (right.IsZero)
                return left.Clone();

            var result = new GmpInt();
            gmp_lib.mpz_ior(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpInt operator ^(GmpInt left, GmpInt right)
        {
            var result = new GmpInt();
            gmp_lib.mpz_xor(result.Data, left.Data, right.Data);
            return result;
        }


        public static GmpInt operator <<(GmpInt value, int shift)
        {

            if (shift == 0) return value.Clone();
            //else if (shift == Int32.MinValue) return ((value >> Int32.MaxValue) >> 1);
            else if (shift < 0) return value >> -shift;

            GmpInt result = new GmpInt();
            gmp_lib.mpz_mul_2exp(result.Data, value.Data, (uint)shift);
            return result;
        }

        public static GmpInt operator >>(GmpInt value, int shift)
        {
            if (shift == 0) return value.Clone();
            //else if (shift == Int32.MinValue) return ((value << Int32.MaxValue) << 1);
            else if (shift < 0) return value << -shift;

            GmpInt result = new GmpInt();
            gmp_lib.mpz_tdiv_q_2exp(result.Data, value.Data, (uint)shift);
            return result;

        }


        public static GmpInt operator ~(GmpInt value)
        {
            GmpInt result = new();
            gmp_lib.mpz_com(result.Data, value.Data);
            return result;
        }

        public static GmpInt operator -(GmpInt value)
        {
            GmpInt result = new();
            gmp_lib.mpz_neg(result.Data, value.Data);
            return result;
        }

        public static GmpInt operator +(GmpInt value)
        {
            GmpInt result = new();
            gmp_lib.mpz_abs(result.Data, value.Data);
            return result;
        }


        public static GmpInt operator ++(GmpInt value)
        {
            return value + GmpInt.One;
        }

        public static GmpInt operator --(GmpInt value)
        {
            return value - GmpInt.One;
        }


        public static GmpInt operator +(GmpInt left, GmpInt right)
        {
            if (right.IsZero) return left.Clone();
            if (left.IsZero) return right.Clone();

            var result = new GmpInt();
            gmp_lib.mpz_add(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpInt operator +(GmpInt left, uint right)
        {
            if (right == 0) return left.Clone();
            if (left.IsZero) return right;

            var result = new GmpInt();
            gmp_lib.mpz_add_ui(result.Data, left.Data, right);
            return result;
        }

        public static GmpInt operator +(uint left, GmpInt right)
            => right + left;

        public static GmpInt operator -(GmpInt left, GmpInt right)
        {
            if (right.IsZero) return left.Clone();
            if (left.IsZero) return -right;

            var result = new GmpInt();
            gmp_lib.mpz_sub(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpInt operator -(GmpInt left, uint right)
        {
            if (right == 0) return left.Clone();
            if (left.IsZero) return -right;
            var result = new GmpInt();
            gmp_lib.mpz_sub_ui(result.Data, left.Data, right);
            return result;
        }

        public static GmpInt operator -(uint left, GmpInt right)
        {
            if (right.IsZero) return left;
            if (left == 0) return -right.Clone();
            return -(right - left);
        }

        public static GmpInt operator *(GmpInt left, GmpInt right)
        {
            if (right.IsZero) return Zero.Clone();
            if (left.IsZero) return Zero.Clone();

            var result = new GmpInt();
            gmp_lib.mpz_mul(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpInt operator *(GmpInt left, int right)
        {
            if (right == 0) return Zero.Clone();
            if (left.IsZero) return Zero.Clone();

            var result = new GmpInt();
            gmp_lib.mpz_mul_si(result.Data, left.Data, right);
            return result;
        }

        public static GmpInt operator *(int left, GmpInt right)
            => right * left;

        public static GmpInt operator *(GmpInt left, uint right)
        {
            if (right == 0) return Zero.Clone();
            if (left.IsZero) return Zero.Clone();

            var result = new GmpInt();
            gmp_lib.mpz_mul_ui(result.Data, left.Data, right);
            return result;
        }

        public static GmpInt operator *(uint left, GmpInt right)
            => right * left;


        public static GmpInt operator /(GmpInt dividend, GmpInt divisor)
        {
            var result = new GmpInt();
            gmp_lib.mpz_divexact(result.Data, dividend.Data, divisor.Data);
            return result;
        }


        public static GmpInt operator /(GmpInt dividend, int divisor)
        {
            if (divisor < 0) return dividend / (GmpInt)divisor;
            return dividend / (uint)divisor;
        }

        public static GmpInt operator /(GmpInt dividend, uint divisor)
        {
            var result = new GmpInt();
            gmp_lib.mpz_divexact_ui(result.Data, dividend.Data, divisor);
            return result;
        }

        public static GmpInt operator %(GmpInt dividend, GmpInt divisor)
        {
            var result = new GmpInt();
            gmp_lib.mpz_mod(result.Data, dividend.Data, divisor.Data);
            return result;
        }

        public static GmpInt operator %(GmpInt dividend, int divisor)
        {
            if (divisor < 0) return dividend % (GmpInt)divisor;
            return dividend % (uint)divisor;
        }

        public static GmpInt operator %(GmpInt dividend, uint divisor)
        {
            var result = new GmpInt();
            gmp_lib.mpz_mod_ui(result.Data, dividend.Data, divisor);
            return result;
        }


        #region CompareOperators
        public static bool operator <(GmpInt left, GmpInt right)
        {
            return gmp_lib.mpz_cmp(left.Data, right.Data) < 0;
        }
        public static bool operator <=(GmpInt left, GmpInt right)
        {
            return gmp_lib.mpz_cmp(left.Data, right.Data) <= 0;
        }
        public static bool operator >(GmpInt left, GmpInt right)
        {
            return gmp_lib.mpz_cmp(left.Data, right.Data) > 0;
        }
        public static bool operator >=(GmpInt left, GmpInt right)
        {
            return gmp_lib.mpz_cmp(left.Data, right.Data) >= 0;
        }
        public static bool operator ==(GmpInt left, GmpInt right)
        {
            return gmp_lib.mpz_cmp(left.Data, right.Data) == 0;
        }
        public static bool operator !=(GmpInt left, GmpInt right)
        {
            return gmp_lib.mpz_cmp(left.Data, right.Data) != 0;
        }


        public static bool operator <(GmpInt left, Int32 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, Int32 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, Int32 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, Int32 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, Int32 right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpInt left, Int32 right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(Int32 left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(Int32 left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(Int32 left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(Int32 left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(Int32 left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(Int32 left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }


        public static bool operator <(GmpInt left, UInt32 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, UInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, UInt32 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, UInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, UInt32 right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpInt left, UInt32 right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(UInt32 left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(UInt32 left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(UInt32 left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(UInt32 left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(UInt32 left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(UInt32 left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }


        public static bool operator <(GmpInt left, Int64 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, Int64 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, Int64 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, Int64 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, Int64 right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpInt left, Int64 right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(Int64 left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(Int64 left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(Int64 left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(Int64 left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(Int64 left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(Int64 left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }


        public static bool operator <(GmpInt left, UInt64 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, UInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, UInt64 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, UInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, UInt64 right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpInt left, UInt64 right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(UInt64 left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(UInt64 left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(UInt64 left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(UInt64 left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(UInt64 left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(UInt64 left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }

        public static bool operator <(GmpInt left, double right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, double right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, double right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, double right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, double right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpInt left, double right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(double left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(double left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(double left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(double left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(double left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(double left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }

        public static bool operator <(GmpInt left, decimal right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, decimal right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, decimal right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, decimal right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, decimal right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpInt left, decimal right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(decimal left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(decimal left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(decimal left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(decimal left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(decimal left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(decimal left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }

        public static bool operator <(GmpInt left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpInt left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpInt left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpInt left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpInt left, BigInteger right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpInt left, BigInteger right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(BigInteger left, GmpInt right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(BigInteger left, GmpInt right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(BigInteger left, GmpInt right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(BigInteger left, GmpInt right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(BigInteger left, GmpInt right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(BigInteger left, GmpInt right)
        {
            return right.CompareTo(left) != 0;
        }

        #endregion

        #endregion


        #endregion

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    //if (Data.Pointer != IntPtr.Zero)
                    //    gmp_lib.mpz_clear(Data);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~GmpInt()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}