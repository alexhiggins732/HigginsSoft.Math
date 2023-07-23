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
        public static int ILogB(double d) => System.Math.ILogB(d);

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ILogB(int value)
            => value > 0 ? BitLength(value) - 1 : value < 0 ? BitLength(-value) - 1 : int.MinValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ILogB(GmpInt value)
            => value != 0 ? value.BitLength - 1 : int.MinValue;

        public static int ILogB(BigInteger value)
            => value != 0 ? (int)value.GetBitLength() - 1 : int.MinValue;



        public static int ILogB(GmpFloat d)
            => d != 0 ? ((GmpInt)d).BitLength - 1 : int.MinValue;




        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static int ILogB(uint value)
            => System.Numerics.BitOperations.Log2(value);

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ILogB(long value)
            => value > 0 ? BitOperations.Log2((ulong)value) : value < 0 ? BitOperations.Log2((ulong)-(value)) : int.MinValue;

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ILogB(ulong value)
            => System.Numerics.BitOperations.Log2(value);
    }
}