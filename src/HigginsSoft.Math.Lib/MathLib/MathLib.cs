/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using System.Numerics;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{

    public static partial class MathLib
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(double d) => System.Math.Abs(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float d) => System.Math.Abs(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int d) => System.Math.Abs(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Abs(long d) => System.Math.Abs(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GmpInt Abs(GmpInt d) => d.Abs();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger Abs(BigInteger d) => BigInteger.Abs(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GmpFloat Abs(GmpFloat d) => d.Abs();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Acos(double d) => System.Math.Acos(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Acosh(double d) => System.Math.Acosh(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Asin(double d) => System.Math.Asin(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Asinh(double d) => System.Math.Asinh(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Atan(double d) => System.Math.Atan(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Atanh(double d) => System.Math.Atanh(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Atan2(double y, double x) => System.Math.Atan2(x, y);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cbrt(double d) => System.Math.Cbrt(d);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Ceiling(double d) => System.Math.Ceiling(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cos(double d) => System.Math.Cos(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cosh(double d) => System.Math.Cosh(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Exp(double d) => System.Math.Exp(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Floor(double d) => System.Math.Floor(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double FusedMultiplyAdd(double x, double y, double z)
            => System.Math.FusedMultiplyAdd(x, y, z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ILogB(double d) => System.Math.ILogB(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log(double d) => System.Math.Log(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(double d) => System.Math.Log2(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log10(double d) => System.Math.Log10(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow(double x, double y) => System.Math.Pow(x, y);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sin(double d) => System.Math.Sin(d);

        public static unsafe (double Sin, double Cos) SinCos(double d)
            => System.Math.SinCos(d);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sinh(double d) => System.Math.Sinh(d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tan(double d) => System.Math.Tan(d);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tanh(double d) => System.Math.Tanh(d);


        /// <summary>
        /// Determines whether the specified integer is a power of two
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int value)
            => 0 == (value & (value - 1));

        /// <summary>
        /// Determines whether the specified integer is a power of two, and calculates the exponent if it is.
        /// </summary>
        /// <param name="value">The integer to check.</param>
        /// <param name="exponent">If the integer is a power of two, returns the exponent of the power of two. Otherwise, returns 0.</param>
        /// <returns><c>true</c> if the specified integer is a power of two; otherwise, <c>false</c>.</returns>
        public static bool IsPowerOfTwo(int value, out int exponent)
        {
            exponent = 0;
            var result = value > 0;
            if (result && value > 1)
            {
                while (value > 0 && (value & 1) == 0)
                {
                    exponent++;
                    value >>= 1;
                }
                result = value == 1;
            }
            if (!result)
                exponent = 0;
            return result;
        }

        public static int BitLength(int value)
        {
            int bits = 0;
            if (value < 0)
            {
                bits = 1;
                value = ~value;
            }
            while (value > 0)
            {
                value >>= 1;
                bits++;
            }
            return bits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(int value)
        {

            int count = 0;
            while (value != 0)
            {
                count += value & 1;
                value >>= 1;
            }
            return count;

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Min(uint a, uint b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(long a, long b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Min(ulong a, ulong b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(double a, double b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Min(decimal a, decimal b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GmpInt Min(GmpInt a, GmpInt b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GmpFloat Min(GmpFloat a, GmpFloat b) => a < b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Max(uint a, uint b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Max(long a, long b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Max(ulong a, ulong b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Max(double a, double b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Max(decimal a, decimal b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GmpInt Max(GmpInt a, GmpInt b) => a > b ? a : b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GmpFloat Max(GmpFloat a, GmpFloat b) => a > b ? a : b;
    }
}