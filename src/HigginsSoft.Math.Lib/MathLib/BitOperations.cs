/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Some routines inspired by the Stanford Bit Twiddling Hacks by Sean Eron Anderson:
// http://graphics.stanford.edu/~seander/bithacks.html

#define TARGET_64BIT

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Numerics;

namespace HigginsSoft.Math.Lib
{

    public static partial class MathLib
    {
        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(int value) => (value & (value - 1)) == 0 && value > 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(uint value) => (value & (value - 1)) == 0 && value != 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(long value) => (value & (value - 1)) == 0 && value > 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(ulong value) => (value & (value - 1)) == 0 && value != 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsPow2(nint value) => (value & (value - 1)) == 0 && value > 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsPow2(nuint value) => (value & (value - 1)) == 0 && value != 0;

        /// <summary>Round the given integral value up to a power of 2.</summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RoundUpToPowerOf2(uint value)
            => System.Numerics.BitOperations.RoundUpToPowerOf2(value);


        /// <summary>
        /// Round the given integral value up to a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundUpToPowerOf2(ulong value)
            => System.Numerics.BitOperations.RoundUpToPowerOf2(value);
        //{
        //    if (Lzcnt.X64.IsSupported || ArmBase.Arm64.IsSupported)
        //    {
        //        int shift = 64 - LeadingZeroCount(value - 1);
        //        return (1ul ^ (ulong)(shift >> 6)) << shift;
        //    }

        //    // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
        //    --value;
        //    value |= value >> 1;
        //    value |= value >> 2;
        //    value |= value >> 4;
        //    value |= value >> 8;
        //    value |= value >> 16;
        //    value |= value >> 32;
        //    return value + 1;
        //}


        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(int value)
            => System.Numerics.BitOperations.LeadingZeroCount((uint)value);

        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(uint value)
            => System.Numerics.BitOperations.LeadingZeroCount(value);

        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(long value)
            => System.Numerics.BitOperations.LeadingZeroCount((ulong)value);

        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(ulong value)
            => System.Numerics.BitOperations.LeadingZeroCount(value);


        


        /// <summary>Returns the integer (ceiling) log of the specified value, base 2.</summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Log2Ceiling(uint value)
        {
            int result = ILogB(value);
            if (PopCount(value) != 1)
            {
                result++;
            }
            return result;
        }

        /// <summary>Returns the integer (ceiling) log of the specified value, base 2.</summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Log2Ceiling(ulong value)
        {
            int result = ILogB(value);
            if (PopCount(value) != 1)
            {
                result++;
            }
            return result;
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(uint value)
            => System.Numerics.BitOperations.PopCount(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(int value)
            => System.Numerics.BitOperations.PopCount((uint)value);

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// </summary>
        /// <param name="value">The value.</param>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(ulong value)
            => System.Numerics.BitOperations.PopCount(value);


        /// <summary>
        /// Count the number of trailing zero bits in an integer value.
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(int value)
            => TrailingZeroCount((uint)value);

        /// <summary>
        /// Count the number of trailing zero bits in an integer value.
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(uint value)
            => System.Numerics.BitOperations.TrailingZeroCount(value);

        /// <summary>
        /// Count the number of trailing zero bits in a mask.
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(long value)
            => TrailingZeroCount((ulong)value);

        /// <summary>
        /// Count the number of trailing zero bits in a mask.
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(ulong value)
            => System.Numerics.BitOperations.TrailingZeroCount(value);


        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset)
            => (value << offset) | (value >> (32 - offset));

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int offset)
            => (value << offset) | (value >> (64 - offset));

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, int offset)
            => (value >> offset) | (value << (32 - offset));

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateRight(ulong value, int offset)
            => (value >> offset) | (value << (64 - offset));


#if NO_NUMERICS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(int value)
        {
            return 31 - BitLength(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(uint value)
        {
            var bitLen = BitLength(value);
            return 32 - bitLen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(long value)
        {
            var bitLen = BitLength(value);
            return 63 - bitLen;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(ulong value)
        {
            var bitLen = BitLength(value);
            return 64 - bitLen;
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


#else

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitLength(int value)
            => 32 - LeadingZeroCount(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitLength(uint value)
            => 32 - LeadingZeroCount(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitLength(long value)
            => 64 - LeadingZeroCount(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitLength(ulong value)
            => 64 - LeadingZeroCount(value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitLength(BigInteger value)
            => (int)value.GetBitLength();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitLength(GmpInt value)
            => value.BitLength;

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

#endif
    }
}