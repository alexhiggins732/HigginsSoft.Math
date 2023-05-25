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

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {
        public static long PowerMod(
            long @base, 
            int exponent,
            long modulus
            )
        {
            var result = 1L;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * @base) % modulus;

                exponent = exponent >> 1;
                @base = (@base * @base) % modulus;
            }
            return result;
        }

        public static int PowerMod(
            int @base, 
            int exponent,
            int modulus
            )
        {
            var result = 1;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (int)(((long)result * @base) % modulus);

                exponent = exponent >> 1;
                @base = (int)(((long)@base * @base) % modulus);
            }
            return result;
        }

        public static uint PowerMod(
            uint @base,
            int exponent,
            uint modulus
            )
        {
            var result = 1u;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (uint)(((ulong)result * @base) % modulus);

                exponent = exponent >> 1;
                @base = (uint)(((ulong)@base * @base) % modulus);
            }
            return result;
        }

        public static ulong PowerMod(
            ulong @base,
            int exponent,
            ulong modulus
            )
        {
            var result = 1ul;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * @base) % modulus;

                exponent = exponent >> 1;
                @base = (@base * @base) % modulus;
            }
            return result;
        }

        public static BigInteger PowerMod(
           BigInteger @base,
           int exponent,
           BigInteger modulus
           )
            => BigInteger.ModPow(@base, exponent, modulus);

        public static GmpInt PowerMod(
           GmpInt @base,
           int exponent,
           GmpInt modulus
           )
            => @base.PowerMod(exponent, modulus);


    }
}