/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {
        public static long BigMul(int a, int b)
        {
            return ((long)a) * b;
        }

        /// <summary>Produces the full product of two unsigned 64-bit numbers.</summary>
        /// <param name="a">The first number to multiply.</param>
        /// <param name="b">The second number to multiply.</param>
        /// <param name="low">The low 64-bit of the product of the specied numbers.</param>
        /// <returns>The high 64-bit of the product of the specied numbers.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong BigMul(ulong a, ulong b, out ulong low)
        {
            if (Bmi2.X64.IsSupported)
            {
                ulong tmp;
                ulong high = Bmi2.X64.MultiplyNoFlags(a, b, &tmp);
                low = tmp;
                return high;
            }
            else if (ArmBase.Arm64.IsSupported)
            {
                low = a * b;
                return ArmBase.Arm64.MultiplyHigh(a, b);
            }

            return SoftwareFallback(a, b, out low);

            static ulong SoftwareFallback(ulong a, ulong b, out ulong low)
            {
                // Adaptation of algorithm for multiplication
                // of 32-bit unsigned integers described
                // in Hacker's Delight by Henry S. Warren, Jr. (ISBN 0-201-91465-4), Chapter 8
                // Basically, it's an optimized version of FOIL method applied to
                // low and high dwords of each operand

                // Use 32-bit uints to optimize the fallback for 32-bit platforms.
                uint al = (uint)a;
                uint ah = (uint)(a >> 32);
                uint bl = (uint)b;
                uint bh = (uint)(b >> 32);

                ulong mull = ((ulong)al) * bl;
                ulong t = ((ulong)ah) * bl + (mull >> 32);
                ulong tl = ((ulong)al) * bh + (uint)t;

                low = tl << 32 | (uint)mull;

                return ((ulong)ah) * bh + (t >> 32) + (tl >> 32);
            }
        }

        /// <summary>Produces the full product of two 64-bit numbers.</summary>
        /// <param name="a">The first number to multiply.</param>
        /// <param name="b">The second number to multiply.</param>
        /// <param name="low">The low 64-bit of the product of the specied numbers.</param>
        /// <returns>The high 64-bit of the product of the specied numbers.</returns>
        public static long BigMul(long a, long b, out long low)
        {
            if (ArmBase.Arm64.IsSupported)
            {
                low = a * b;
                return ArmBase.Arm64.MultiplyHigh(a, b);
            }

            ulong high = BigMul((ulong)a, (ulong)b, out ulong ulow);
            low = (long)ulow;
            return (long)high - ((a >> 63) & b) - ((b >> 63) & a);
        }
    }
}