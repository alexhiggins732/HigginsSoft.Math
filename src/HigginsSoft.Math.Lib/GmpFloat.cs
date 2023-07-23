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
using System.Collections;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace HigginsSoft.Math.Lib
{
    public class GmpFloat : IDisposable
    {

        public mpf_t Data { get; } = new();

        private static readonly GmpFloat s_bnOneInt = new GmpFloat(1);
        private static readonly GmpFloat s_bnZeroInt = new GmpFloat(0);
        private static readonly GmpFloat s_bnMinusOneInt = new GmpFloat(-1);

        #region constructors

        public GmpFloat()
        {
            gmp_lib.mpf_init_set_d(Data, 0.0);
        }

        public GmpFloat(mpz_t value)
            : this()
        {
            gmp_lib.mpf_set_z(Data, value);
        }

        public GmpFloat(GmpInt value)
            : this(value.Data)
        {

        }

        public GmpFloat(mpf_t value)
            : this()
        {
            gmp_lib.mpf_set(Data, value);
        }

        public GmpFloat(GmpFloat value)
            : this(value.Data)
        {

        }

        public GmpFloat(int value)
        {
            if (value > -1)
            {
                gmp_lib.mpf_init_set_si(Data, value);
            }
            else
            {
                //linux bug fix. gmp_lib initializes negative values a uint even when call set_si.
                gmp_lib.mpf_init_set_ui(Data, (uint)-value);
                gmp_lib.mpf_neg(Data, Data);
            }
        }

        public GmpFloat(uint value)
        {
            gmp_lib.mpf_init_set_ui(Data, value);
        }

        public GmpFloat(long value)
            : this(value.ToString())
        {
        }

        public GmpFloat(long value, uint precision)
            : this(value.ToString(), precision, 10)
        {
        }

        public GmpFloat(ulong value)
            : this(value.ToString())
        {
        }

        public GmpFloat(double value)
        {
            gmp_lib.mpf_init_set_d(Data, value);
        }


        public GmpFloat(decimal value)
            : this(value.ToString().Split('.')[0])
        {

        }

        public GmpFloat(BigInteger value)
            : this(value.ToString())
        {

        }

        public GmpFloat(string value, int @base = 10)
        {
            char_ptr ptr = new char_ptr(value);
            gmp_lib.mpf_init_set_str(Data, ptr, @base);
            gmp_lib.free(ptr);
        }

        public GmpFloat(string value, uint precision, int @base = 10)
        {
            gmp_lib.mpf_init2(Data, precision);
            char_ptr ptr = new char_ptr(value);
            gmp_lib.mpf_set_str(Data, ptr, @base);
            gmp_lib.free(ptr);
        }

        #endregion

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToExponentialString() => Data.ToString();

        public string ToString(int @base)
        {
            if (Data.Pointer == IntPtr.Zero) return "uninitialized";
            if (IsZero) return "0";

            mp_exp_t exp = 0;
            char_ptr s = gmp_lib.mpf_get_str(char_ptr.Zero, ref exp, @base, 0, Data);
            var stringValue = s.ToString();
            if (exp > 1)
            {
                stringValue= stringValue.PadRight(exp, '0');
            }
            if (stringValue.IndexOf('.') == -1)
            {
                //stringValue = $"0.{stringValue.PadRight(exp, '0')}";
            }
            else
            {
                //.PadRight(exp, '0');
            }
            return stringValue;
        }

        #region public properties



        public static GmpFloat Zero
        {
            get { return s_bnZeroInt.Clone(); }
        }

        public static GmpFloat One
        {
            get { return s_bnOneInt.Clone(); }
        }

        public static GmpFloat MinusOne
        {
            get { return s_bnMinusOneInt.Clone(); }
        }

        public bool IsZero => Sign == 0;
        public bool IsOne => Sign == 1 && gmp_lib.mpf_cmp_si(Data, 1) == 0;
        public int Sign => gmp_lib.mpf_sgn(Data);


        #endregion

        #region public methods

        public GmpFloat Clone()
        {
            return new GmpFloat(this);
        }


        public int CompareTo(GmpFloat other)
            => gmp_lib.mpf_cmp(this.Data, other.Data);

        public int CompareTo(int other)
        {
            if (other > -1)
            {
                return gmp_lib.mpf_cmp_si(this.Data, other);
            }
            else
            {
                // linux bug. Negative ints are converted to uint.
                return gmp_lib.mpf_cmp(this.Data, ((GmpFloat)other).Data);
            }
        }
        public int CompareTo(uint other)
            => gmp_lib.mpf_cmp_ui(this.Data, other);

        public int CompareTo(long other)
           => CompareTo((GmpFloat)other);

        public int CompareTo(ulong other)
           => CompareTo((GmpFloat)other);

        public int CompareTo(float other)
            => gmp_lib.mpf_cmp_d(this.Data, other);

        public int CompareTo(double other)
            => gmp_lib.mpf_cmp_d(this.Data, other);

        public int CompareTo(decimal other)
            => CompareTo((GmpFloat)other);

        public int CompareTo(BigInteger other)
            => CompareTo((GmpFloat)other);

        public int CompareTo(GmpInt other)
            => CompareTo((GmpFloat)other);


        #endregion

        #region public instance methods

        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is GmpFloat other))
                return false;
            return Equals(other);
        }

        public bool Equals(GmpFloat other)
        {

            if (Sign != other.Sign) return false;
            return this == other;
        }

        public bool Equals(int other) => this == other;

        public bool Equals(uint other) => this == other;

        public bool Equals(long other) => this.Equals((GmpFloat)other);

        public bool Equals(ulong other) => this.Equals((GmpFloat)other);

        public bool Equals(float other) => this.Equals((GmpFloat)other);

        public bool Equals(double other) => this == other;

        public bool Equals(BigInteger other) => this.Equals((GmpFloat)other);

        public bool Equals(GmpInt other) => this.Equals((GmpFloat)other);


        public Byte[] Bytes()
        {
            var hex = ToString(16);
            var bytes = hex.HexToByteArray().Reverse().ToArray();
            return bytes;
        }


        public ulong[] RawData()
        {
            var result = ToString(16).HexToUlongArray();
            return result;
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
        public static void SetDefaultPrecision(uint value)
        {
            gmp_lib.mpf_set_default_prec(value);
        }


        #region public static operators

        public static implicit operator GmpFloat(string value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(Byte value)
        {
            return new GmpFloat(value);
        }
        public static implicit operator GmpFloat(SByte value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(Int16 value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(UInt16 value)
        {
            return new GmpFloat(value);
        }


        public static implicit operator GmpFloat(int value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(uint value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(long value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(ulong value)
        {
            return new GmpFloat(value);
        }

        public static explicit operator GmpFloat(float value)
        {
            return new GmpFloat(value);
        }

        public static explicit operator GmpFloat(double value)
        {
            return new GmpFloat(value);
        }

        public static explicit operator GmpFloat(decimal value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(BigInteger value)
        {
            return new GmpFloat(value);
        }

        public static explicit operator GmpFloat(GmpInt value)
        {
            return new GmpFloat(value);
        }

        public static explicit operator GmpFloat(mpz_t value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator GmpFloat(mpf_t value)
        {
            return new GmpFloat(value);
        }

        public static implicit operator string(GmpFloat value)
        {
            return value.ToString();
        }

        public static explicit operator byte(GmpFloat value)
        {
            return checked((byte)((int)value));
        }

        public static explicit operator sbyte(GmpFloat value)
        {
            return checked((sbyte)((int)value));
        }

        public static explicit operator short(GmpFloat value)
        {
            return checked((short)((int)value));
        }

        public static explicit operator ushort(GmpFloat value)
        {
            return checked((ushort)((int)value));
        }

        public static explicit operator int(GmpFloat value)
        {
            // handle linux bug
            if (value.Sign < 0)
                if (value < (int)short.MinValue)
                {
                    var a = +value;
                    var result = gmp_lib.mpf_get_ui(a.Data);
                    return (int)result;
                }
            return gmp_lib.mpf_get_si(value.Data);
        }


        public static explicit operator uint(GmpFloat value)
        {
            return gmp_lib.mpf_get_ui(value.Data);
        }

        public static explicit operator long(GmpFloat value)
        {
            if (value.IsZero) return 0L;
            return (long)((BigInteger)value);
        }

        public static explicit operator ulong(GmpFloat value)
        {
            if (value.IsZero) return 0UL;
            return (ulong)((BigInteger)value);
        }

        public static explicit operator GmpInt(GmpFloat value)
        {
            return new GmpInt(value);
        }

        public static explicit operator BigInteger(GmpFloat value)
        {
            if (value.IsZero) return BigInteger.Zero;
            var base10 = value.ToString().Replace(" ", "");
            var b = BigInteger.Parse(base10);
            return b;
        }

        public static explicit operator float(GmpFloat value)
        {
            return (float)(double)value;
        }

        public static explicit operator double(GmpFloat value)
        {
            return gmp_lib.mpf_get_d(value.Data);
        }

        public static explicit operator decimal(GmpFloat value)
        {
            if (value.IsZero) return 0M;
            return (decimal)(BigInteger)value;
        }

        public static explicit operator mpf_t(GmpFloat value)
        {
            return value.Clone().Data;
        }

        public static explicit operator mpz_t(GmpFloat value)
        {
            return ((GmpInt)value).Clone().Data;
        }




        public static GmpFloat operator &(GmpFloat left, GmpFloat right)
        {

            if (left.IsZero || right.IsZero)
            {
                return GmpFloat.Zero.Clone();
            }

            var result = (GmpInt)left & (GmpInt)right;
            return (GmpFloat)result;
        }

        public static GmpFloat operator |(GmpFloat left, GmpFloat right)
        {
            if (left.IsZero)
                return right.Clone();
            if (right.IsZero)
                return left.Clone();

            var result = (GmpInt)left | (GmpInt)right;
            return (GmpFloat)result;
        }


        public static GmpFloat operator ^(GmpFloat left, GmpFloat right)
        {
            var result = (GmpInt)left ^ (GmpInt)right;
            return (GmpFloat)result;
        }


        public static GmpFloat operator <<(GmpFloat value, int shift)
        {

            if (shift == 0) return value.Clone();
            //else if (shift == Int32.MinValue) return ((value >> Int32.MaxValue) >> 1);
            else if (shift < 0) return value >> -shift;

            GmpFloat result = new GmpFloat();
            gmp_lib.mpf_mul_2exp(result.Data, value.Data, (uint)shift);
            return result;
        }

        public static GmpFloat operator >>(GmpFloat value, int shift)
        {
            if (shift == 0) return value.Clone();
            //else if (shift == Int32.MinValue) return ((value << Int32.MaxValue) << 1);
            else if (shift < 0) return value << -shift;

            GmpFloat result = new GmpFloat();
            gmp_lib.mpf_div_2exp(result.Data, value.Data, (uint)shift);
            return result;

        }


        public static GmpFloat operator ~(GmpFloat value)
        {
            return (GmpFloat)~((GmpInt)value);
        }


        public static GmpFloat operator -(GmpFloat value)
        {
            GmpFloat result = new();
            gmp_lib.mpf_neg(result.Data, value.Data);
            return result;
        }

        public static GmpFloat operator +(GmpFloat value)
        {
            GmpFloat result = new();
            gmp_lib.mpf_abs(result.Data, value.Data);
            return result;
        }


        public static GmpFloat operator ++(GmpFloat value)
        {
            var result = new GmpFloat();
            gmp_lib.mpf_add_ui(result.Data, value.Data, 1);
            return result;
        }

        public static GmpFloat operator --(GmpFloat value)
        {
            var result = new GmpFloat();
            gmp_lib.mpf_sub_ui(result.Data, value.Data, 1);
            return result;
        }


        public static GmpFloat operator +(GmpFloat left, GmpFloat right)
        {
            if (right.IsZero) return left.Clone();
            if (left.IsZero) return right.Clone();

            var result = new GmpFloat();
            gmp_lib.mpf_add(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpFloat operator +(GmpFloat left, uint right)
        {
            if (right == 0) return left.Clone();
            if (left.IsZero) return right;

            var result = new GmpFloat();
            gmp_lib.mpf_add_ui(result.Data, left.Data, right);
            return result;
        }

        public static GmpFloat operator +(uint right, GmpFloat left)
            => left + right;


        public static GmpFloat operator -(GmpFloat left, GmpFloat right)
        {
            if (right.IsZero) return left.Clone();
            if (left.IsZero) return -right;

            var result = new GmpFloat();
            gmp_lib.mpf_sub(result.Data, left.Data, right.Data);
            return result;
        }

        public static GmpFloat operator -(GmpFloat left, uint right)
        {
            if (right == 0) return left.Clone();
            var result = new GmpFloat();
            gmp_lib.mpf_sub_ui(result.Data, left.Data, right);
            return result;
        }

        public static GmpFloat operator -(uint left, GmpFloat right)
        {
            if (right.IsZero) return left;

            var result = new GmpFloat();
            gmp_lib.mpf_ui_sub(result.Data, left, right.Data);
            return result;
        }


        public static GmpFloat operator *(GmpFloat left, GmpFloat right)
        {
            if (right.IsZero) return Zero.Clone();
            if (left.IsZero) return Zero.Clone();

            var result = new GmpFloat();
            gmp_lib.mpf_mul(result.Data, left.Data, right.Data);
            string resultStr = result.ToString();
            return result;
        }

        public static GmpFloat operator *(GmpFloat left, uint right)
        {
            if (right == 0) return Zero.Clone();
            if (left.IsZero) return Zero.Clone();

            var result = new GmpFloat();
            gmp_lib.mpf_mul_ui(result.Data, left.Data, right);
            return result;
        }

        public static GmpFloat operator *(uint left, GmpFloat right)
            => right * left;


        public static GmpFloat operator /(GmpFloat dividend, uint divisor)
        {
            if (divisor == 1) return dividend.Clone();

            var result = new GmpFloat();
            gmp_lib.mpf_div_ui(result.Data, dividend.Data, divisor);
            return result;
        }

        public static GmpFloat operator /(uint dividend, GmpFloat divisor)
            => (GmpFloat)dividend / divisor;

        public static GmpFloat operator /(GmpFloat dividend, GmpInt divisor)
            => dividend / (GmpFloat)divisor;

        public static GmpFloat operator /(GmpInt dividend, GmpFloat divisor)
            => (GmpFloat)dividend / divisor;

        public static GmpFloat operator /(GmpFloat dividend, GmpFloat divisor)
        {
            if (divisor.IsOne) return dividend.Clone();
            var result = new GmpFloat();
            gmp_lib.mpf_div(result.Data, dividend.Data, divisor.Data);
            return result;
        }

        // Modulus operate does not exist
        public static GmpFloat operator %(GmpFloat dividend, GmpFloat divisor)
        {
            var result = GmpFloat.Zero.Clone();
            return result;
        }


        #region CompareOperators
        public static bool operator <(GmpFloat left, GmpFloat right)
        {
            return gmp_lib.mpf_cmp(left.Data, right.Data) < 0;
        }
        public static bool operator <=(GmpFloat left, GmpFloat right)
        {
            return gmp_lib.mpf_cmp(left.Data, right.Data) <= 0;
        }
        public static bool operator >(GmpFloat left, GmpFloat right)
        {
            return gmp_lib.mpf_cmp(left.Data, right.Data) > 0;
        }
        public static bool operator >=(GmpFloat left, GmpFloat right)
        {
            return gmp_lib.mpf_cmp(left.Data, right.Data) >= 0;
        }
        public static bool operator ==(GmpFloat left, GmpFloat right)
        {
            return gmp_lib.mpf_cmp(left.Data, right.Data) == 0;
        }
        public static bool operator !=(GmpFloat left, GmpFloat right)
        {
            return gmp_lib.mpf_cmp(left.Data, right.Data) != 0;
        }


        public static bool operator <(GmpFloat left, Int32 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, Int32 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, Int32 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, Int32 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, Int32 right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpFloat left, Int32 right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(Int32 left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(Int32 left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(Int32 left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(Int32 left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(Int32 left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(Int32 left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }



        public static bool operator <(GmpFloat left, UInt32 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, UInt32 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, UInt32 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, UInt32 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, UInt32 right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpFloat left, UInt32 right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(UInt32 left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(UInt32 left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(UInt32 left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(UInt32 left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(UInt32 left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(UInt32 left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }




        public static bool operator <(GmpFloat left, Int64 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, Int64 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, Int64 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, Int64 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, Int64 right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpFloat left, Int64 right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(Int64 left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(Int64 left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(Int64 left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(Int64 left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(Int64 left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(Int64 left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }


        public static bool operator <(GmpFloat left, UInt64 right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, UInt64 right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, UInt64 right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, UInt64 right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, UInt64 right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpFloat left, UInt64 right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(UInt64 left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(UInt64 left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(UInt64 left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(UInt64 left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(UInt64 left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(UInt64 left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }

        public static bool operator <(GmpFloat left, double right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, double right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, double right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, double right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, double right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpFloat left, double right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(double left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(double left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(double left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(double left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(double left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(double left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }

        public static bool operator <(GmpFloat left, decimal right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, decimal right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, decimal right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, decimal right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, decimal right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpFloat left, decimal right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(decimal left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(decimal left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(decimal left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(decimal left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(decimal left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(decimal left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }

        public static bool operator <(GmpFloat left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, BigInteger right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(GmpFloat left, BigInteger right)
        {
            return left.CompareTo(right) != 0;
        }


        public static bool operator <(BigInteger left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(BigInteger left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(BigInteger left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(BigInteger left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(BigInteger left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }

        public static bool operator !=(BigInteger left, GmpFloat right)
        {
            return right.CompareTo(left) != 0;
        }



        public static bool operator <(GmpFloat left, GmpInt right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(GmpFloat left, GmpInt right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(GmpFloat left, GmpInt right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(GmpFloat left, GmpInt right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(GmpFloat left, GmpInt right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(GmpFloat left, GmpInt right)
        {
            return left.CompareTo(right) != 0;
        }



        public static bool operator <(GmpInt left, GmpFloat right)
        {
            return right.CompareTo(left) > 0;
        }
        public static bool operator <=(GmpInt left, GmpFloat right)
        {
            return right.CompareTo(left) >= 0;
        }
        public static bool operator >(GmpInt left, GmpFloat right)
        {
            return right.CompareTo(left) < 0;
        }
        public static bool operator >=(GmpInt left, GmpFloat right)
        {
            return right.CompareTo(left) <= 0;
        }
        public static bool operator ==(GmpInt left, GmpFloat right)
        {
            return right.CompareTo(left) == 0;
        }
        public static bool operator !=(GmpInt left, GmpFloat right)
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
                    //    gmp_lib.mpf_clear(Data);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~GmpFloat()
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

        internal GmpFloat Abs()
        {
            GmpFloat z = new();
            gmp_lib.mpf_abs(z.Data, this.Data);
            return z;
        }

        public static GmpFloat Parse(string value, int @base)
        {
            return new GmpFloat(value, 2);
        }



        #endregion
    }
}