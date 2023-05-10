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
    public partial class MathUtil
    {

        public static T Lcm<T>(T a, T b)
        {
            var x = Ops<T>.MultiplyT(a, b);
            var y = Ops<T>.GcdT(a, b);
            var result = Ops<T>.DivideT(x, y);
            return result;
        }
        // Helper method to calculate the least common multiple of two numbers
        public static int Lcm(int a, int b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static uint Lcm(uint a, uint b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static long Lcm(long a, long b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static ulong Lcm(ulong a, ulong b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static float Lcm(float a, float b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static double Lcm(double a, double b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static decimal Lcm(decimal a, decimal b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static BigInteger Lcm(BigInteger a, BigInteger b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, GmpInt b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, int b)
        {
            return a * b / Gcd(a, b);
        }


        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, uint b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, long b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, ulong b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, float b)
        {
            return a * (GmpInt)b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, double b)
        {
            return a * (GmpInt)b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, decimal b)
        {
            return a * (GmpInt)b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(GmpInt a, BigInteger b)
        {
            return a * b / Gcd(a, b);
        }


        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(int a, GmpInt b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(uint a, GmpInt b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(long a, GmpInt b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(ulong a, GmpInt b)
        {
            return a * b / Gcd(a, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(float a, GmpInt b)
        {
            var aGmp = (GmpInt)a;
            return aGmp * b / Gcd(aGmp, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(double a, GmpInt b)
        {
            var aGmp = (GmpInt)a;
            return aGmp * b / Gcd(aGmp, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(decimal a, GmpInt b)
        {
            var aGmp = (GmpInt)a;
            return aGmp * b / Gcd(aGmp, b);
        }

        // Helper method to calculate the least common multiple of two numbers
        public static GmpInt Lcm(BigInteger a, GmpInt b)
        {
            var aGmp = (GmpInt)a;
            return aGmp * b / Gcd(aGmp, b);
        }
    }
}
