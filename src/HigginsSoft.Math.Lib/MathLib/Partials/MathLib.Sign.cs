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

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(decimal value)
        {
            if (value < 0) return -1;
            return 1;
        }

        public static int Sign(double value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else if (value == 0)
            {
                return 0;
            }

            throw new ArithmeticException(SR.Arithmetic_NaN);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(short value)
        {
            return Sign((int)value);
        }

        public static int Sign(int value)
        {
            return unchecked(value >> 31 | (int)((uint)-value >> 31));
        }

        public static int Sign(long value)
        {
            return unchecked((int)(value >> 63 | (long)((ulong)-value >> 63)));
        }

        public static int Sign(nint value)
        {
#if TARGET_64BIT
            return unchecked((int)(value >> 63 | (long)((ulong)-value >> 63)));
#else
            return unchecked((int)(value >> 31) | (int)((uint)-value >> 31));
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(sbyte value)
        {
            return Sign((int)value);
        }

        public static int Sign(float value)
        {
            if (value < 0)
            {
                return -1;
            }
            else if (value > 0)
            {
                return 1;
            }
            else if (value == 0)
            {
                return 0;
            }

            throw new ArithmeticException(SR.Arithmetic_NaN);
        }
    }

}