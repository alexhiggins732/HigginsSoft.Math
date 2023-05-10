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
using System.Numerics;


namespace HigginsSoft.Math.Lib
{
    public partial class MathUtil
    {
        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// and set s and t such that a * s + b * t = g
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, GmpInt b)
        {
            GmpInt result = new();
            Gcd(result, a.Data, b.Data);
            return result;
        }

        /// <summary>
        /// Set <paramref name="result"/> to the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Gcd(GmpInt result, GmpInt a, GmpInt b)
        {
            //Gcd(result.Data, a.Data, b.Data);
            gmp_lib.mpz_gcd(result.Data, a, b);
          
        }

        ///// <summary>
        ///// Set <paramref name="result"/> to the the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        ///// </summary>
        ///// <param name="result"></param>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        //public static void Gcd(mpz_t result, mpz_t a, mpz_t b)
        //{
        //    gmp_lib.mpz_gcd(result, a, b);
        //}

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, uint b)
        {
            var result = new GmpInt();
            Gcd(result, a.Data, b);
            return result;
        }

        /// <summary>
        /// Set <paramref name="result"/> to the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Gcd(GmpInt result, GmpInt a, uint b)
        {
            //Gcd(result.Data, a.Data, b);
            gmp_lib.mpz_gcd_ui(result.Data, a, b);
        }


        ///// <summary>
        ///// Set <paramref name="result"/> to the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        ///// </summary>
        ///// <param name="result"></param>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        //public static void Gcd(mpz_t result, mpz_t a, uint b)
        //{
        //    gmp_lib.mpz_gcd_ui(result, a, b);
        //}


        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static int Gcd(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static uint Gcd(uint a, uint b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static long Gcd(long a, long b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static ulong Gcd(ulong a, ulong b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Gcd(float a, float b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static double Gcd(double a, double b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static decimal Gcd(decimal a, decimal b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }


        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static BigInteger Gcd(BigInteger a, BigInteger b)
        {
            if (b == 0)
            {
                return a;
            }
            return Gcd(b, a % b);
        }

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, int b)
            => Gcd(a, (GmpInt)b);


        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, long b)
            => Gcd(a, (GmpInt)b);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, ulong b)
            => Gcd(a, (GmpInt)b);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, float b)
            => Gcd(a, (GmpInt)b);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, double b)
            => Gcd(a, (GmpInt)b);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, decimal b)
            => Gcd(a, (GmpInt)b);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(GmpInt a, BigInteger b)
            => Gcd(a, (GmpInt)b);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(int a, GmpInt b)
            => Gcd((GmpInt)b, a);


        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(long a, GmpInt b)
            => Gcd((GmpInt)b, a);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(ulong a, GmpInt b)
            => Gcd((GmpInt)b, a);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(float a, GmpInt b)
            => Gcd((GmpInt)b, a);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(double a, GmpInt b)
            => Gcd((GmpInt)b, a);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(decimal a, GmpInt b)
            => Gcd((GmpInt)b, a);

        /// <summary>
        /// Returns the greatest common divisor of <paramref name="a"/> and <paramref name="b"/> , 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GmpInt Gcd(BigInteger a, GmpInt b)
            => Gcd((GmpInt)b, a);



    }
}
