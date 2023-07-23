/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {
        public static double LOG_2 = MathLib.Log(2.0);
        public static double LOG_10 = MathLib.Log(10.0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(double d) => System.Math.Log2(Abs(d));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(int d) => System.Math.Log2((double)Abs(d));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(uint d) => System.Math.Log2((double)d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(long d) => System.Math.Log2((double)Abs(d));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(ulong d) => System.Math.Log2((double)d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(int d) => Log((double)d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(uint d) => Log((double)d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(long d) => Log((double)d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(ulong d) => Log((double)d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(GmpInt d) => BigMath.LogBig(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(BigInteger d) => BigMath.LogBig(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(GmpInt d)
            => (d.Sign > -1 ? Log(d) : Log(-d)) / LOG_2;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(BigInteger d)
            => (d.Sign > -1 ? Log(d) : Log(-d)) / LOG_2;

        // ported from Java: https://stackoverflow.com/questions/6827516/logarithm-for-biginteger
        /**
 * Provides some mathematical operations on {@code BigDecimal} and {@code BigInteger}.
 * Static methods.
 */
        public class BigMath
        {

            public static readonly double LOG_2 = MathLib.Log(2.0);
            public static readonly double LOG_10 = MathLib.Log(10.0);

            // numbers greater than 10^MAX_DIGITS_10 or e^MAX_DIGITS_E are considered unsafe ('too big') for floating point operations
            const int MAX_DIGITS_10 = 294;
            const int MAX_DIGITS_2 = 977; // ~ MAX_DIGITS_10 * LN(10)/LN(2)
            const int MAX_DIGITS_E = 677; // ~ MAX_DIGITS_10 * LN(10)

            /**
             * Computes the natural logarithm of a {@link BigInteger} 
             * <p>
             * Works for really big integers (practically unlimited), even when the argument 
             * falls outside the {@code double} range
             * <p>
             * 
             * 
             * @param val Argument
             * @return Natural logarithm, as in {@link java.lang.Math#log(double)}<br>
             * {@code Nan} if argument is negative, {@code NEGATIVE_INFINITY} if zero.
             */
            public static double LogBig(BigInteger val)
            {
                if (val.Sign < 1)
                    return val.Sign < 0 ? Double.NaN : Double.NegativeInfinity;
                int blex = (int)val.GetBitLength() - MAX_DIGITS_2; // any value in 60..1023 works here
                if (blex > 0)
                    val = val >> blex;// val.shiftRight(blex);
                double res = MathLib.Log(((double)val));
                return blex > 0 ? res + blex * LOG_2 : res;
            }

            public static double LogBig(GmpInt val)
            {
                if (val.Sign < 1)
                    return val.Sign < 0 ? Double.NaN : Double.NegativeInfinity;
                int blex = (int)val.BitLength - MAX_DIGITS_2; // any value in 60..1023 works here
                if (blex > 0)
                    val = val >> blex;// val.shiftRight(blex);
                double res = MathLib.Log(((double)val));
                return blex > 0 ? res + blex * LOG_2 : res;
            }

            /**
             * Computes the natural logarithm of a {@link BigDecimal} 
             * <p>
             * Works for really big (or really small) arguments, even outside the double range.
             * 
             * @param val Argument
             * @return Natural logarithm, as in {@link java.lang.Math#log(double)}<br>
             * {@code Nan} if argument is negative, {@code NEGATIVE_INFINITY} if zero.
             */
            //public static double logBigDecimal(GmpFloat val)
            //{
            //    if (val.Sign < 1)
            //        return val.Sign < 0 ? Double.NaN : Double.NegativeInfinity;
            //    int digits = val.Precision - val.Scale;
            //    if (digits < MAX_DIGITS_10 && digits > -MAX_DIGITS_10)
            //        return MathLib.Log((double)val);
            //    else
            //        return LogBigInteger((GmpInt)val) - val.scale() * LOG_10;
            //}

            /**
             * Computes the exponential function, returning a {@link BigDecimal} (precision ~ 16).
             * <p>
             * Works for very big and very small exponents, even when the result 
             * falls outside the double range.
             *
             * @param exponent Any finite value (infinite or {@code Nan} throws {@code IllegalArgumentException})    
             * @return The value of {@code e} (base of the natural logarithms) raised to the given exponent, 
             * as in {@link java.lang.Math#exp(double)}
             */
            //public static BigDecimal expBig(double exponent)
            //{
            //    if (!Double.isFinite(exponent))
            //        throw new IllegalArgumentException("Infinite not accepted: " + exponent);
            //    // e^b = e^(b2+c) = e^b2 2^t with e^c = 2^t 
            //    double bc = MAX_DIGITS_E;
            //    if (exponent < bc && exponent > -bc)
            //        return new BigDecimal(Math.exp(exponent), MathContext.DECIMAL64);
            //    boolean neg = false;
            //    if (exponent < 0)
            //    {
            //        neg = true;
            //        exponent = -exponent;
            //    }
            //    double b2 = bc;
            //    double c = exponent - bc;
            //    int t = (int)Math.ceil(c / LOG_10);
            //    c = t * LOG_10;
            //    b2 = exponent - c;
            //    if (neg)
            //    {
            //        b2 = -b2;
            //        t = -t;
            //    }
            //    return new BigDecimal(Math.exp(b2), MathContext.DECIMAL64).movePointRight(t);
            //}

            /**
             * Same as {@link java.lang.Math#pow(double,double)} but returns a {@link BigDecimal} (precision ~ 16).
             * <p>
             * Works even for outputs that fall outside the {@code double} range.
             * <br>
             * The only limitation is that {@code b * log(a)} cannot exceed the {@code double} range. 
             * 
             * @param a Base. Should be non-negative 
             * @param b Exponent. Should be finite (and non-negative if base is zero)
             * @return Returns the value of the first argument raised to the power of the second argument.
             */
            //public static BigDecimal powBig(double a, double b)
            //{
            //    if (!(Double.isFinite(a) && Double.isFinite(b)))
            //        throw new IllegalArgumentException(
            //                Double.isFinite(b) ? "base not finite: a=" + a : "exponent not finite: b=" + b);
            //    if (b == 0)
            //        return BigDecimal.ONE;
            //    else if (b == 1)
            //        return BigDecimal.valueOf(a);
            //    if (a <= 0)
            //    {
            //        if (a == 0)
            //        {
            //            if (b >= 0)
            //                return BigDecimal.ZERO;
            //            else
            //                throw new IllegalArgumentException("0**negative = infinite b=" + b);
            //        }
            //        else
            //            throw new IllegalArgumentException("negative base a=" + a);
            //    }
            //    double x = b * Math.log(a);
            //    if (Math.abs(x) < MAX_DIGITS_E)
            //        return BigDecimal.valueOf(Math.pow(a, b));
            //    else
            //        return expBig(x);
            //}

        }
    }
}