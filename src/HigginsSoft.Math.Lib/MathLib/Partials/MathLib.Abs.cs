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
        public static short Abs(short value)
        {
            if (value < 0)
            {
                value = (short)-value;
                if (value < 0)
                {
                    ThrowAbsOverflow();
                }
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Abs(sbyte value)
        {
            if (value < 0)
            {
                value = (sbyte)-value;
                if (value < 0)
                {
                    ThrowAbsOverflow();
                }
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Abs(decimal value)
        {
            if (value < 0)
                return -value;
            return value;
        }


    }
}
