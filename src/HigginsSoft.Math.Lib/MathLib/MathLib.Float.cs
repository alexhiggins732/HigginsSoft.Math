/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace HigginsSoft.Math.Lib
{

    public static partial class MathLib
    {
        public const double E = 2.7182818284590452354;

        public const double PI = 3.14159265358979323846;

        public const double Tau = 6.283185307179586476925;

        private const int maxRoundingDigits = 15;

        private const double doubleRoundLimit = 1e16d;

        // This table is required for the Round function which can specify the number of digits to round to
        private static readonly double[] roundPower10Double = new double[] {
          1E0, 1E1, 1E2, 1E3, 1E4, 1E5, 1E6, 1E7, 1E8,
          1E9, 1E10, 1E11, 1E12, 1E13, 1E14, 1E15
        };

        private const double SCALEB_C1 = 8.98846567431158E+307; // 0x1p1023

        private const double SCALEB_C2 = 2.2250738585072014E-308; // 0x1p-1022

        private const double SCALEB_C3 = 9007199254740992; // 0x1p53





        [DoesNotReturn]
        [StackTraceHidden]
        private static void ThrowAbsOverflow()
        {
            throw new OverflowException();
        }


        public static double BitDecrement(double x)
        {
            long bits = BitConverter.DoubleToInt64Bits(x);

            if (((bits >> 32) & 0x7FF00000) >= 0x7FF00000)
            {
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns double.MaxValue
                return (bits == 0x7FF00000_00000000) ? double.MaxValue : x;
            }

            if (bits == 0x00000000_00000000)
            {
                // +0.0 returns -double.Epsilon
                return -double.Epsilon;
            }

            // Negative values need to be incremented
            // Positive values need to be decremented

            bits += ((bits < 0) ? +1 : -1);
            return BitConverter.Int64BitsToDouble(bits);
        }

        public static double BitIncrement(double x)
        {
            long bits = BitConverter.DoubleToInt64Bits(x);

            if (((bits >> 32) & 0x7FF00000) >= 0x7FF00000)
            {
                // NaN returns NaN
                // -Infinity returns double.MinValue
                // +Infinity returns +Infinity
                return (bits == unchecked((long)(0xFFF00000_00000000))) ? double.MinValue : x;
            }

            if (bits == unchecked((long)(0x80000000_00000000)))
            {
                // -0.0 returns double.Epsilon
                return double.Epsilon;
            }

            // Negative values need to be decremented
            // Positive values need to be incremented

            bits += ((bits < 0) ? -1 : +1);
            return BitConverter.Int64BitsToDouble(bits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CopySign(double x, double y)
        {
            if (false && (Sse2.IsSupported || AdvSimd.IsSupported))
            {
                //return VectorMath.ConditionalSelectBitwise(Vector128.CreateScalarUnsafe(-0.0), Vector128.CreateScalarUnsafe(y), Vector128.CreateScalarUnsafe(x)).ToScalar();
            }
            else
            {
                return SoftwareFallback(x, y);
            }
            return SoftwareFallback(x, y);
            static double SoftwareFallback(double x, double y)
            {
                const long signMask = 1L << 63;

                // This method is required to work for all inputs,
                // including NaN, so we operate on the raw bits.
                long xbits = BitConverter.DoubleToInt64Bits(x);
                long ybits = BitConverter.DoubleToInt64Bits(y);

                // Remove the sign from x, and remove everything but the sign from y
                xbits &= ~signMask;
                ybits &= signMask;

                // Simply OR them to get the correct sign
                return BitConverter.Int64BitsToDouble(xbits | ybits);
            }
        }





        public static double NegativeZero = -0.0;
        public static double IEEERemainder(double x, double y)
        {
            if (double.IsNaN(x))
            {
                return x; // IEEE 754-2008: NaN payload must be preserved
            }

            if (double.IsNaN(y))
            {
                return y; // IEEE 754-2008: NaN payload must be preserved
            }

            double regularMod = x % y;

            if (double.IsNaN(regularMod))
            {
                return double.NaN;
            }

            if ((regularMod == 0) && double.IsNegative(x))
            {
                return NegativeZero;
            }

            double alternativeResult = (regularMod - (Abs(y) * Sign(x)));

            if (Abs(alternativeResult) == Abs(regularMod))
            {
                double divisionResult = x / y;
                double roundedResult = Round(divisionResult);

                if (Abs(roundedResult) > Abs(divisionResult))
                {
                    return alternativeResult;
                }
                else
                {
                    return regularMod;
                }
            }

            if (Abs(alternativeResult) < Abs(regularMod))
            {
                return alternativeResult;
            }
            else
            {
                return regularMod;
            }
        }



        //public static double Log(double value) => System.Math.Log(value);


        public static double MaxMagnitude(double x, double y)
        {
            // This matches the IEEE 754:2019 `maximumMagnitude` function
            //
            // It propagates NaN inputs back to the caller and
            // otherwise returns the input with a greater magnitude.
            // It treats +0 as greater than -0 as per the specification.

            double ax = Abs(x);
            double ay = Abs(y);

            if ((ax > ay) || double.IsNaN(ax))
            {
                return x;
            }

            if (ax == ay)
            {
                return double.IsNegative(x) ? y : x;
            }

            return y;
        }


        public static double MinMagnitude(double x, double y)
        {
            // This matches the IEEE 754:2019 `minimumMagnitude` function
            //
            // It propagates NaN inputs back to the caller and
            // otherwise returns the input with a lesser magnitude.
            // It treats -0 as lesser than +0 as per the specification.

            double ax = Abs(x);
            double ay = Abs(y);

            if ((ax < ay) || double.IsNaN(ax))
            {
                return x;
            }

            if (ax == ay)
            {
                return double.IsNegative(x) ? x : y;
            }

            return y;
        }

        /// <summary>Returns an estimate of the reciprocal of a specified number.</summary>
        /// <param name="d">The number whose reciprocal is to be estimated.</param>
        /// <returns>An estimate of the reciprocal of <paramref name="d" />.</returns>
        /// <remarks>
        ///    <para>On ARM64 hardware this may use the <c>FRECPE</c> instruction which performs a single Newton-Raphson iteration.</para>
        ///    <para>On hardware without specialized support, this may just return <c>1.0 / d</c>.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReciprocalEstimate(double d)
        {
            // x86 doesn't provide an estimate instruction for double-precision reciprocal

            if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.Arm64.ReciprocalEstimateScalar(Vector64.CreateScalar(d)).ToScalar();
            }
            else
            {
                return 1.0 / d;
            }
        }

        /// <summary>Returns an estimate of the reciprocal square root of a specified number.</summary>
        /// <param name="d">The number whose reciprocal square root is to be estimated.</param>
        /// <returns>An estimate of the reciprocal square root <paramref name="d" />.</returns>
        /// <remarks>
        ///    <para>On ARM64 hardware this may use the <c>FRSQRTE</c> instruction which performs a single Newton-Raphson iteration.</para>
        ///    <para>On hardware without specialized support, this may just return <c>1.0 / Sqrt(d)</c>.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReciprocalSqrtEstimate(double d)
        {
            // x86 doesn't provide an estimate instruction for double-precision reciprocal square root

            if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.Arm64.ReciprocalSquareRootEstimateScalar(Vector64.CreateScalar(d)).ToScalar();
            }
            else
            {
                return 1.0 / Sqrt(d);
            }
        }




        internal class SR
        {
            public const string Arithmetic_NaN = "Value is not a number";

            public const string ArgumentOutOfRange_RoundingDigits = "Argument out of range";
            public const string Argument_InvalidEnumValue = "Invalid enum argument: {0} {1}";
            public static string Format(string format, params object[] args)
            {
                return string.Format(format, args);
            }
        }

        public unsafe static double ModF(double d, double* f)
        {
            *f = d;
            return d;
        }

        [DoesNotReturn]
        private static void ThrowMinMaxException<T>(T min, T max)
        {
            throw new ArgumentException($"Value must be between {min} and {max}");
        }

        public static double ScaleB(double x, int n)
        {
            // Implementation based on https://git.musl-libc.org/cgit/musl/tree/src/math/scalbln.c
            //
            // Performs the calculation x * 2^n efficiently. It constructs a double from 2^n by building
            // the correct biased exponent. If n is greater than the maximum exponent (1023) or less than
            // the minimum exponent (-1022), adjust x and n to compute correct result.

            double y = x;
            if (n > 1023)
            {
                y *= SCALEB_C1;
                n -= 1023;
                if (n > 1023)
                {
                    y *= SCALEB_C1;
                    n -= 1023;
                    if (n > 1023)
                    {
                        n = 1023;
                    }
                }
            }
            else if (n < -1022)
            {
                y *= SCALEB_C2 * SCALEB_C3;
                n += 1022 - 53;
                if (n < -1022)
                {
                    y *= SCALEB_C2 * SCALEB_C3;
                    n += 1022 - 53;
                    if (n < -1022)
                    {
                        n = -1022;
                    }
                }
            }

            double u = BitConverter.Int64BitsToDouble(((long)(0x3ff + n) << 52));
            return y * u;
        }
    }
}
