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
using System.Xml.Linq;


namespace HigginsSoft.Math.Lib
{
    public class GcdExtResult<TData>
    {
        public TData A;
        public TData B;
        public TData S;
        public TData T;
        public TData Gcd;

        public GcdExtResult(TData result, TData a, TData b, TData s, TData t)
        {
            this.Gcd = result;
            this.A = a;
            this.B = b;
            this.S = s;
            this.T = t;
        }
    }

    public partial class MathUtil
    {
        /// <summary>
        /// Return the extended euclidian greatest common divisor of <paramref name="a"/> and <paramref name="a"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GcdExtResult<GmpInt> GcdExt(GmpInt a, GmpInt b)
            => GcdExt(new(), a, b);


        /// <summary>
        /// Return the extended euclidian greatest common divisor of <paramref name="a"/> and <paramref name="a"/> , 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GcdExtResult<GmpInt> GcdExt(GmpInt result, GmpInt a, GmpInt b)
            => GcdExt(result, a, b, new(), new());




        /// <summary>
        /// Set <paramref name="result"/> to the the greatest common divisor of <paramref name="a"/> and <paramref name="a"/> , 
        /// and set s and t such that a * s + b * t = g
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static GcdExtResult<GmpInt> GcdExt(GmpInt result, GmpInt a, GmpInt b, GmpInt s, GmpInt t)
        {
            GcdExt(result.Data, a.Data, b.Data, s.Data, t.Data);
            return new GcdExtResult<GmpInt>(result, a, b, s, t);
        }



        /// <summary>
        /// Set <paramref name="result"/> to the the greatest common divisor of <paramref name="a"/> and <paramref name="a"/> , 
        /// and set s and t such that a * s + b * t = g
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void GcdExt(mpz_t result, mpz_t a, mpz_t b, mpz_t s, mpz_t t)
        {
            gmp_lib.mpz_gcdext(result, s, t, a, b);
        }



        public static GcdExtResult<int> GcdExtInt(int a, int b)
        {
            int r0 = a;
            int r1 = b;
            int s0 = 1;
            int s1 = 0;
            int t0 = 0;
            int t1 = 1;

            while (r1 != 0)
            {
                int q = r0 / r1;

                int temp = r0;
                r0 = r1;
                r1 = temp - q * r1;

                temp = s0;
                s0 = s1;
                s1 = temp - q * s1;

                temp = t0;
                t0 = t1;
                t1 = temp - q * t1;
            }
            return new GcdExtResult<int>(r0, a, b, s0, t0);

        }


        public static GcdExtResult<T> GcdExt<T>(T a, T b)
        {
            var op = OpFactory.GetOpFactory<T>();
            T r0 = a;
            T r1 = b;
            T s0 = op.ConvertFromInt(1);
            T s1 = op.ConvertFromInt(0);
            T t0 = op.ConvertFromInt(0);
            T t1 = op.ConvertFromInt(1);
            T zero = op.ConvertFromInt(0);
            T q = op.ConvertFromInt(0);
            T temp = op.ConvertFromInt(0);


            while (op.NotEqualT(r1, zero))
            {
                q = op.DivideT(r0, r1);

                temp = r0;
                r0 = r1;
                r1 = op.SubtractT(temp, op.MultiplyT(q, r1));

                temp = s0;
                s0 = s1;
                s1 = op.SubtractT(temp, op.MultiplyT(q, s1));

                temp = t0;
                t0 = t1;
                t1 = op.SubtractT(temp, op.MultiplyT(q, t1));
            }
            return new GcdExtResult<T>(r0, a, b, s0, t0);

        }

        public static bool IsProbablePrime(GmpIntConvertible convertible)
        {
            return convertible.Value.IsProbablyPrimeRabinMiller(20);
        }


        public static int GetPreviousPrime(int value)
        {
            var result = GetPreviousPrime((GmpIntConvertible)value);
            return (int)result;
        }
        public static uint GetPreviousPrime(uint value)
        {
            var result = GetPreviousPrime((GmpIntConvertible)value);
            return (uint)result;
        }
        public static long GetPreviousPrime(long value)
        {
            var result = GetPreviousPrime((GmpIntConvertible)value);
            return (int)result;
        }
        public static ulong GetPreviousPrime(ulong value)
        {
            var result = GetPreviousPrime((GmpIntConvertible)value);
            return (uint)result;
        }
        public static GmpInt GetPreviousPrime(GmpIntConvertible value)
        {
            var result = GetPreviousPrime(value.Value);
            return result;
        }

        public static GmpInt GetPreviousPrime(GmpInt value)
        {
            
            GmpInt z = value;
            if (z.IsEven)
            {
                z -= 1;
            }

            //TODO: only test candidates that +/-1 mod 6
            var test = gmp_lib.mpz_probab_prime_p(z, 20);
            while (test == 0)
            {
                z -= 2;
                test = gmp_lib.mpz_probab_prime_p(z, 20);
            }
            return z;

        }

        public static int GetNextPrime(int value)
        {
            var result = GetNextPrime((GmpIntConvertible)value);
            return (int)result;
        }
        public static uint GetNextPrime(uint value)
        {
            var result = GetNextPrime((GmpIntConvertible)value);
            return (uint)result;
        }
        public static long GetNextPrime(long value)
        {
            var result = GetNextPrime((GmpIntConvertible)value);
            return (int)result;
        }
        public static ulong GetNextPrime(ulong value)
        {
            var result = GetNextPrime((GmpIntConvertible)value);
            return (uint)result;
        }
        public static GmpInt GetNextPrime(GmpIntConvertible value)
        {
            var result = GetNextPrime(value.Value);
            return result;
        }

        public static GmpInt GetNextPrime(GmpInt value)
        {

            GmpInt z = value;
            if (z.IsEven)
            {
                z += 1;
            }

            //TODO: only test candidates that +/-1 mod 6
            var test = gmp_lib.mpz_probab_prime_p(z, 20);
            while (test == 0)
            {
                z += 2;
                test = gmp_lib.mpz_probab_prime_p(z, 20);
            }
            return z;

        }

    }
    public class GmpIntConvertible
    {
        public GmpInt Value { get; }
        public GmpIntConvertible(GmpInt i) { Value = i.Clone(); }

        public static implicit operator GmpIntConvertible(int i) => new GmpIntConvertible(i);
        public static implicit operator GmpIntConvertible(uint i) => new GmpIntConvertible(i);
        public static implicit operator GmpIntConvertible(long i) => new GmpIntConvertible(i);
        public static implicit operator GmpIntConvertible(ulong i) => new GmpIntConvertible(i);
        public static implicit operator GmpIntConvertible(float i) => new GmpIntConvertible((GmpInt)i);
        public static implicit operator GmpIntConvertible(double i) => new GmpIntConvertible((GmpInt)i);
        public static implicit operator GmpIntConvertible(decimal i) => new GmpIntConvertible((GmpInt)i);
        public static implicit operator GmpIntConvertible(BigInteger i) => new GmpIntConvertible(i);
        public static explicit operator GmpIntConvertible(GmpInt i) => new GmpIntConvertible(i);
        public static explicit operator GmpIntConvertible(GmpFloat i) => new GmpIntConvertible((GmpInt)i);

        public static implicit operator GmpInt(GmpIntConvertible i) => i.Value.Clone();
    }
}
