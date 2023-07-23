/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System;
using uint64_t = System.UInt64;
using uint32_t = System.UInt32;
using System.Runtime.CompilerServices;
using MathGmp.Native;
using static MathGmp.Native.gmp_lib;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using intv = System.Numerics.Vector<int>;
namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {
        static Random rand = new Random();

        public const int MAX_INT_SQUARE_ROOT = 46340;
        public const int MAX_INT_SQUARE = 2_147_395_600;
        public const int MAX_UINT_SQUARE_ROOT = 65535;
        public const uint MAX_UINT_SQUARE = 4_294_836_225;
        public const int MAX_LONG_SQUARE_ROOT = 3_037_000;
        public const long MAX_LONG_SQUARE = 9_223_369_000_000;
        public const uint MAX_ULONG_SQUARE_ROOT = 4_294_967_295;
        public const ulong MAX_ULONG_SQUARE = 18_446_744_065_119_617_025;

        public class RhoPoly
        {
            public int Poly { get; set; }
            public RhoPoly(int poly)
            {
                if (poly > 0)
                    this.Poly = poly;
                else
                {
                    poly= rand.Next();
                }
            }

        }

        public static int PollardRho31(int n, RhoPoly? rhoPoly = null, int maxAttempts = 20)
        {

            // guard against negative numbers
            if (n < 4) return n;
            if ((n & 1) == 0) return 2;

            if (n > MAX_INT_SQUARE_ROOT)
            {
                return (int)PollardRho32((uint)n, maxAttempts);
            }


            int x;
            int y;
            int c;
            int abs;
            int gcd = 1;
            int xlimit = n - 2 <= MAX_INT_SQUARE_ROOT ? n - 2 : MAX_INT_SQUARE_ROOT - 2;
            int climit = n - 1 <= MAX_INT_SQUARE_ROOT ? n - 1 : MAX_INT_SQUARE_ROOT - 1;
            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                x = rand.Next(0, xlimit) + 2;
                y = x;
                c = rhoPoly != null ? rhoPoly.Poly : rand.Next(0, climit) + 1;
                while (gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = x > y ? x - y : y - x;
                    gcd = MathUtil.Gcd(abs, n);
                }
                if (gcd == n)
                    gcd = 1;
            }
            if (gcd == 1)
            {
                gcd = n;
            }
            return gcd;
        }




        static Vector<int> PollardRho31AvxFallBack(Vector<int> n, int maxAttempts = 20)
        {
            var result = vect_32_result;
            for (var i = 0; i < vect_32_Size; i++)
            {
                var j = n[i];
                if (j < 4)
                    result[i] = j;
                else if ((j & 1) == 0)
                    result[i] = j;
                else if (j > MAX_INT_SQUARE_ROOT)
                    result[i] = (int)PollardRho32((uint)j, maxAttempts);
                else
                    result[i] = PollardRho31(i, new RhoPoly(0), maxAttempts); ;
            }
            return new Vector<int>(result);
        }

        static Vector<int> vect_32_1 = new Vector<int>(1);
        static Vector<int> vect_32_2 = new Vector<int>(2);
        static Vector<int> vect_32_4 = new Vector<int>(4);
        static Vector<int> vect_32_MAX_INT_SQUARE_ROOT = new Vector<int>(MAX_INT_SQUARE_ROOT);
        static Vector<int> vect_32_MAX_INT_SQUARE_ROOT_MINUS_1 = new Vector<int>(MAX_INT_SQUARE_ROOT - 1);
        static Vector<int> vect_32_MAX_INT_SQUARE_ROOT_MINUS_2 = new Vector<int>(MAX_INT_SQUARE_ROOT - 2);

        static Vector256<float> vect_32_0_AsFloat = Vector256.Create(0f);
        static Vector256<float> vect_32_1_AsFloat = Vector256.Create(1f);
        static Vector256<float> vect_32_2_AsFloat = Vector256.Create(2f);
        static Vector256<float> vect_32_4_AsFloat = Vector256.Create(4f);
        static Vector256<float> vect_32_MAX_INT_SQUARE_ROOT_AsFloat = Vector256.Create((float)MAX_INT_SQUARE_ROOT);
        static Vector256<float> vect_32_MAX_INT_SQUARE_ROOT_MINUS_1_AsFloat = Vector256.Create((float)MAX_INT_SQUARE_ROOT - 1);
        static Vector256<float> vect_32_MAX_INT_SQUARE_ROOT_MINUS_2_AsFloat = Vector256.Create((float)MAX_INT_SQUARE_ROOT - 2);
        static Vector256<float> err = Vector256.Create(0.0001f);
        static int vect_32_Size = Vector<int>.Count;

        static int[] vect_32_result = new int[vect_32_Size];
        public unsafe static int PollardRho31Avx(int value, int maxAttempts = 20)
        {
            //guard against negative numbers
            if (value < 4) return value;
            if ((value & 1) == 0) return 2;
            if (value > MAX_INT_SQUARE_ROOT)
                return (int)PollardRho32((uint)value, maxAttempts);

            var n = new Vector<int>(value);
            var result = PollardRho31Avx(n, maxAttempts, true);
            return result;
        }

        public static Vector256<float> ConvertToFloat(Vector256<int> v)
        {
            var low = Avx2.ConvertToVector128Single(v.GetLower());
            var high = Avx2.ConvertToVector128Single(v.GetUpper());
            return Vector256.Create(low, high);
        }

        public static Vector256<int> ConvertToFloat(Vector256<float> v)
        {
            return Avx2.ConvertToVector256Int32WithTruncation(v);

        }

        static intv muladdmod(intv a, intv b, intv add, intv mod)
        {
            var product = a * b + add;
            var floored = product / mod;
            var s = floored * mod;
            var rem = product - s;
            return rem;
        }

        static int BinaryMod(int a, int b)
        {
            while (a >= b)
            {
                var b1 = b;
                var t = b1 << 1;
                while (t > 0 && t < a)
                {
                    b1 = t;
                    t = b1 << 1;
                }
                a -= b1;
            }
            return a;
        }



        public unsafe static int PollardRho31Avx(Vector<int> n, int maxAttempts = 20, bool prechecked = false)
        {
            var value = n[0];

            var nfloat = ConvertToFloat(n.AsVector256());
            var recipN = Avx.Reciprocal(nfloat);

            //Avx.ReciprocalScalar

            var found = 1;
            if (!prechecked)
            {
                var n_256 = n.AsVector256();
                var nF = ConvertToFloat(n_256);

                //if (n<4) return n;
                var comp = Avx.CompareLessThan(nF, vect_32_4_AsFloat);
                var mask = Avx.MoveMask(comp);
                if (mask > 0)
                    return CheckGcd(n, PollardRho31AvxFallBack(n, maxAttempts));


                //if ((n & 1) == 0) return 2;

                var c1 = n & vect_32_1;
                var isEven = c1 != vect_32_1;
                if (isEven)
                    return 2;


                comp = Avx.CompareGreaterThan(nF, vect_32_MAX_INT_SQUARE_ROOT_AsFloat);
                mask = Avx.MoveMask(comp);
                if (mask > 0) //todo: call PollardRho32Avx with precheked=true since n>4 & not even.
                    return CheckGcd(n, PollardRho31AvxFallBack(n, maxAttempts));
            }


            intv x = new(), y = new(), c = new(), abs = new();//, gcd = new(1);


            //var xlimit = Avx.Min(ConvertToFloat((n - vect_32_2).AsVector256()), vect_32_MAX_INT_SQUARE_ROOT_MINUS_2_AsFloat);
            //var climit = Avx.Min(ConvertToFloat((n - vect_32_1).AsVector256()), vect_32_MAX_INT_SQUARE_ROOT_MINUS_2_AsFloat);
            var xlimit = Vector.Max(n - vect_32_2, vect_32_MAX_INT_SQUARE_ROOT);
            var climit = Vector.Min(n - vect_32_1, vect_32_MAX_INT_SQUARE_ROOT);

            Vector<int> NextRand(Vector<int> max, int add)
            {
                for (var i = 0; i < vect_32_Size; i++)
                {
                    vect_32_result[i] = 1 + rand.Next(0, max[i]);
                }
                return new Vector<int>(vect_32_result);
            }




            //gcd == 1 -> gcd == vect_32_1 appears to do element by element loop scan not using avxn.
            //for (var i = 0; gcd == vect_32_1 && i < maxAttempts; i++)
            int i = 0;
            for (; found == 1 && i < maxAttempts; i += vect_32_Size)
            {

                //x = rand.Next(0, xlimit) + 2;
                x = NextRand(xlimit, 2);
                //y = x;
                y = x;
                //c = rand.Next(0, climit) + 1;
                c = NextRand(climit, 1);
                while (found == 1)
                {
                    //x = (x * x + c) % n;
                    //x = muladdmod(x, x, c, n);

                    //var xtemp = muladdmod(x, x, c, n);

                    //var product = x * x + c;
                    //var floored = product / n;
                    //var s = floored * n;
                    //var rem = product - s;
                    //if (xtemp != rem)
                    //{
                    //    string bp = "";
                    //}
                    //var x2 = product - ((product / n) * n);
                    //if (x2 != xtemp)
                    //{
                    //    string bp = "";
                    //}



                    var product = x * x + c;
                    x = product - ((product / n) * n);
                    //x = BinaryMod(product, n);

                    /*
                    x = x * x;
                    x = x - c;
                    Vect.Mod(x, n);*/



                    //y = (y * y + c) % n;
                    //var ytmp = muladdmod(y, y, c, n);
                    //product = (y * y + c);
                    //y = product - ((product / n) * n);

                    //if (ytmp != y)
                    //{
                    //    string bp = "";
                    //}
                    ////y = (y * y + c) % n;
                    //ytmp = muladdmod(y, y, c, n);
                    //product = (y * y + c);
                    //y = product - ((product / n) * n);

                    //if (ytmp != y)
                    //{
                    //    string bp = "";
                    //}

                    product = (y * y + c);
                    y = product - ((product / n) * n);
                    product = (y * y + c);
                    y = product - ((product / n) * n);

                    abs = Vector.Abs(Vector.Max(x, y) - Vector.Min(x, y));
                    //gcd = Gcd(n, abs);
                    for (var k = 0; (found == 1 || found == value) && k < vect_32_Size; k++)
                    {
                        var a = abs[k];
                        if (a == 0) //edge case of (x*x+c)%n=x, gives factorization of gcd(n, x)
                        {
                            a = c[i];
                        }
                        var b = MathUtil.Gcd(value, a);
                        if (b > 1)
                        {
                            if (b < value)
                                return b;
                            found = b;
                        }
                    }
                    //abs = x > y ? x - y : y - x;
                    //gcd = MathUtil.Gcd(abs, n);
                }

                found = 1;
            }
            if (found == 1)
            {
                //var y256 = y.AsVector256();
                //var x256 = x.AsVector256(); 
                //var abs256 = abs.AsVector256();
                //var c256 = c.AsVector256();
                found = value;
            }

            return found;
        }



        private static int CheckGcd(Vector<int> n, Vector<int> gcd)
        {
            var n256 = n.AsVector256();
            var gcd256 = gcd.AsVector256();
            var nF = ConvertToFloat(n256);
            var gcdF = ConvertToFloat(gcd256);

            var compLt = Avx.CompareLessThan(gcdF, nF);
            var maskLt = Avx.MoveMask(compLt);
            var compGt = Avx.CompareGreaterThan(gcdF, vect_32_1_AsFloat);
            var maskGt = Avx.MoveMask(compGt);
            var mask = maskLt & maskGt;

            if (mask == 0)
                return n[0];
            else
            {
                for (var i = 0; i < vect_32_Size; i++)
                {
                    if ((mask & (1 << i)) > 0)
                    {
                        return (int)gcd[i];
                    }
                }
            }
            return n[0];
        }

        public static uint PollardRho32(uint n, int maxAttempts = 20)
        {
            if (n > MAX_UINT_SQUARE_ROOT)
                return (uint)PollardRho63(n, maxAttempts);
            else if (n <= MAX_INT_SQUARE_ROOT)
                return (uint)PollardRho31((int)n, new RhoPoly(0), maxAttempts);

            if (n < 4) return n;
            if ((n & 1) == 0) return 2;


            uint x;
            uint y;
            uint c;
            uint abs;
            uint gcd = 1;

            int xlimit = n - 2 <= (uint)MAX_INT_SQUARE_ROOT ? (int)n - 2 : MAX_INT_SQUARE_ROOT - 2;
            int climit = n - 1 <= (uint)MAX_INT_SQUARE_ROOT ? (int)n - 1 : MAX_INT_SQUARE_ROOT - 1;

            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                //TODO: need rand for 32, 63,64
                x = (uint)rand.Next(0, xlimit) + 2;
                y = x;
                c = (uint)rand.Next(0, climit) + 1;
                while (gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = x > y ? x - y : y - x;
                    gcd = MathUtil.Gcd(abs, n);
                }
                if (gcd == n)
                    gcd = 1;
            }
            if (gcd == 1)
            {
                gcd = n;
            }
            return gcd;
        }


        public static long PollardRho63(long n, int maxAttempts = 20)
        {
            // guard against negative numbers.
            if (n < 4) return n;
            if ((n & 1) == 0) return 2;
            if (n > MAX_LONG_SQUARE_ROOT)
                return (long)PollardRho64((ulong)n, maxAttempts);
            else if (n <= MAX_UINT_SQUARE_ROOT)
                return (long)PollardRho32((uint)n, maxAttempts);

            long x;
            long y;
            int c;
            long abs;
            long gcd = 1;

            int xlimit = n - 2 <= (long)MAX_INT_SQUARE_ROOT ? (int)n - 2 : MAX_INT_SQUARE_ROOT - 2;
            int climit = n - 1 <= (long)MAX_INT_SQUARE_ROOT ? (int)n - 1 : MAX_INT_SQUARE_ROOT - 1;

            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                x = rand.Next(0, xlimit) + 2;
                y = x;
                c = rand.Next(0, climit) + 1;
                while (gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = x > y ? x - y : y - x;
                    gcd = MathUtil.Gcd(abs, n);
                }
                if (gcd == n)
                    gcd = 1;
            }
            if (gcd == 1)
            {
                gcd = n;
            }
            return gcd;
        }

        public static ulong PollardRho64(ulong n, int maxAttempts = 20)
        {
            if (n > MAX_ULONG_SQUARE_ROOT)
                return (ulong)PollardRhoZ(n, maxAttempts);
            else if (n <= MAX_LONG_SQUARE_ROOT)
                return (ulong)PollardRho63((long)n, maxAttempts);

            if (n < 4) return n;
            if ((n & 1) == 0) return 2;


            ulong x;
            ulong y;
            ulong c;
            ulong abs;
            ulong gcd = 1;


            int xlimit = n - 2 <= (ulong)MAX_INT_SQUARE_ROOT ? (int)n - 2 : MAX_INT_SQUARE_ROOT - 2;
            int climit = n - 1 <= (ulong)MAX_INT_SQUARE_ROOT ? (int)n - 1 : MAX_INT_SQUARE_ROOT - 1;

            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                //TODO: need rand for 32, 63,64
                x = (ulong)rand.Next(0, xlimit) + 2;
                y = x;
                c = (ulong)rand.Next(0, climit) + 1;
                while (gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = x > y ? x - y : y - x;
                    gcd = MathUtil.Gcd(abs, n);
                }
                if (gcd == n)
                    gcd = 1;
            }
            if (gcd == 1)
            {
                gcd = n;
            }
            return gcd;
        }


        public static class BigRandom
        {
            private static Random random = new Random();

            public static BigInteger Next(BigInteger minValue, BigInteger maxValue)
            {
                BigInteger range = maxValue - minValue;

                // Generate a random number within the range
                byte[] bytes = range.ToByteArray();
                BigInteger result;

                do
                {
                    random.NextBytes(bytes);
                    bytes[bytes.Length - 1] &= 0x7F; // Ensure positive value
                    result = new BigInteger(bytes);
                } while (result >= range);

                // Add the minimum value to the result
                return result + minValue;
            }
        }

        public static BigInteger PollardRho(BigInteger n, int maxAttempts = 20)
        {
            if (n <= MAX_ULONG_SQUARE_ROOT)
                return (ulong)PollardRho64((ulong)n, maxAttempts);

            if (n < 4) return n;
            if ((n & 1) == 0) return 2;


            BigInteger x;
            BigInteger y;
            BigInteger c;
            BigInteger abs;
            BigInteger gcd = 1;
            var bits = MathLib.Log2(n);


            BigInteger xlimit = n - 2;
            BigInteger climit = n - 1;

            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                //TODO: need rand for 32, 63,64
                x = BigRandom.Next(0, xlimit) + 2;
                y = x;
                c = BigRandom.Next(0, climit) + 1;
                var gcdCount = 0;
                while (gcdCount< maxAttempts && gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = x > y ? x - y : y - x;
                    gcd = MathUtil.Gcd(abs, n);
                    if (gcd == n)
                        break;
                    gcdCount++;
                }
                if (gcd == n)
                    gcd = 1;

            }
            if (gcd == 1)
            {
                gcd = n;
            }
            return gcd;
        }

        public static GmpInt PollardRhoZOld(GmpInt n, int maxAttempts = 20)
        {

            if (n == 1) return 1;
            if ((n & 1) == 0) return 2;

            GmpInt x;
            GmpInt y;
            int c, b;
            GmpInt abs;
            GmpInt gcd = 1;

            int xRandLimit = int.MaxValue - 2;
            if (n - 2 < int.MaxValue) xRandLimit = (int)(n - 2);
            var cRandLimit = int.MaxValue - 1;
            if (n - 1 < int.MaxValue) cRandLimit = (int)(n - 1);

            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                b = rand.Next(0, xRandLimit) + 2;
                x = b;
                y = x;
                c = rand.Next(0, cRandLimit) + 1;
                while (gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = MathLib.Abs(x - y);
                    gcd = MathUtil.Gcd(abs, n);
                }
                if (gcd == n) gcd = 1;
            }
            if (gcd == 1)
            {
                gcd = n;
            }
            return gcd;
        }

        // same as PollardRhoZ, just using low level library to limit allocations.
        public static GmpInt PollardRhoZ(GmpInt n, int maxAttempts = 20)
        {

            if (n.IsOne) return 1;
            else if (n < 4)
                return n;
            else if (n <= MAX_ULONG_SQUARE_ROOT)
                return (ulong)PollardRho64((ulong)n, maxAttempts);
            if (n.IsEven) return 2;


            // We can use gmpint directly with low-level api.
            // This way constructing, initializing and freeing is handled automatically.
            int c = 0, b = 0;


            //GmpInt x;
            //GmpInt y;
            //int c, b;
            //GmpInt abs;
            //GmpInt gcd = 1;

            // just init and set all upfront so we don't need to track what needs to be disposed.
            mpz_t x = new(); mpz_init_set_si(x, 0);
            mpz_t y = new(); mpz_init_set_si(y, 0);
            mpz_t abs = new(); mpz_init_set_si(abs, 0);
            mpz_t gcd = new(); mpz_init_set_si(gcd, 1);
            mpz_t gmp_n = n.Data;


            int xRandLimit = int.MaxValue - 2;
            if (n - 2 < int.MaxValue) xRandLimit = (int)(n - 2);
            var cRandLimit = int.MaxValue - 1;
            if (n - 1 < int.MaxValue) cRandLimit = (int)(n - 1);

            //for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            for (var i = 0; mpz_cmp_si(gcd, 1) == 0 && i < maxAttempts; i++)
            {

                b = rand.Next(0, xRandLimit) + 2;
                //x = b;
                mpz_set_si(x, b);

                //y = x;
                mpz_set(y, x);

                c = rand.Next(0, cRandLimit) + 1;
                //while (gcd == 1)
                while (mpz_cmp_si(gcd, 1) == 0)
                {
                    //x = (x * x + c) % n;
                    mpz_mul(x, x, x);
                    mpz_add_ui(x, x, (uint)c);
                    mpz_tdiv_r(x, x, gmp_n);



                    //y = (y * y + c) % n;
                    mpz_mul(y, y, y);
                    mpz_add_ui(y, y, (uint)c);
                    mpz_tdiv_r(y, y, gmp_n);

                    //y = (y * y + c) % n;
                    mpz_mul(y, y, y);
                    mpz_add_ui(y, y, (uint)c);
                    mpz_tdiv_r(y, y, gmp_n);

                    //abs = MathLib.Abs(x - y);
                    mpz_sub(abs, x, y);
                    if (mpz_sgn(abs) < 0)
                        mpz_add(abs, abs, gmp_n);

                    //gcd = MathUtil.Gcd(abs, n);
                    mpz_gcd(gcd, abs, gmp_n);
                }
                if (mpz_cmp(gcd, gmp_n) == 0)
                    mpz_set_si(gcd, 1);
            }

            //if (gcd == 1)
            if (mpz_cmp_si(gcd, 1) == 0)
            {
                //gcd = n;
                mpz_set(gcd, gmp_n);
            }

            mpz_clear(x);
            mpz_clear(y);
            mpz_clear(abs);
            return (GmpInt)gcd;
        }

        public static GmpInt PollardRhoC(GmpInt n, int c, int maxAttempts = 20)
        {
            var res = mbrent(n.Data, (uint)c, out mpz_t gmp_f, maxAttempts);
            return new GmpInt(gmp_f);
        }

        static int mbrent(mpz_t gmp_n, uint c, out mpz_t gmp_f, int iterations = 20)
        {
            /*
            run pollard's rho algorithm on n with Brent's modification, 
            returning the first factor found in f, or else 0 for failure.
            use f(x) = x^2 + c
            see, for example, bressoud's book.
            */

            mpz_t x = new(), y = new(), q = new(), g = new(), ys = new(), t1 = new();
            gmp_f = new();

            uint32_t i = 0, k, r, m;
            int it;
            int imax = iterations;

            // initialize local args
            mpz_init(x);
            mpz_init(y);
            mpz_init(q);
            mpz_init(g);
            mpz_init(ys);
            mpz_init(t1);
            mpz_init(gmp_f);
            // starting state of algorithm.  
            r = 1;
            m = 256;
            i = 0;
            it = 0;

            mpz_set_ui(q, 1);
            mpz_set_ui(y, 0);
            mpz_set_ui(g, 1);

            do
            {
                mpz_set(x, y);
                for (i = 0; i <= r; i++)
                {
                    mpz_mul(t1, y, y);      //y = (y*y + c) mod n
                    mpz_add_ui(t1, t1, c);
                    mpz_tdiv_r(y, t1, gmp_n);
                }

                k = 0;
                do
                {
                    mpz_set(ys, y);
                    for (i = 1; i <= MIN(m, r - k); i++)
                    {
                        mpz_mul(t1, y, y); //y=(y*y + c)%n
                        mpz_add_ui(t1, t1, c);
                        mpz_tdiv_r(y, t1, gmp_n);

                        mpz_sub(t1, x, y); //q = q*abs(x-y) mod n
                        if (mpz_sgn(t1) < 0)
                            mpz_add(t1, t1, gmp_n);
                        mpz_mul(q, t1, q);
                        mpz_tdiv_r(q, q, gmp_n);
                    }
                    mpz_gcd(g, q, gmp_n);
                    k += m;
                    it++;

                    // abort after the specified number of gcd's
                    if (it > imax)
                    {
                        mpz_set(gmp_f, gmp_n);
                        goto free;
                    }

                } while (k < r && (mpz_get_ui(g) == 1));
                r *= 2;
            } while (mpz_get_ui(g) == 1);

            if (mpz_cmp(g, gmp_n) == 0)
            {
                // back track
                do
                {
                    mpz_mul(t1, ys, ys); //ys = (ys*ys + c) mod n
                    mpz_add_ui(t1, t1, c);
                    mpz_tdiv_r(ys, t1, gmp_n);

                    mpz_sub(t1, ys, x);
                    if (mpz_sgn(t1) < 0)
                        mpz_add(t1, t1, gmp_n);
                    mpz_gcd(g, t1, gmp_n);
                } while ((mpz_size(g) == 1) && (mpz_get_ui(g) == 1));

                if (mpz_cmp(g, gmp_n) == 0)
                {
                    mpz_set(gmp_f, gmp_n);
                    goto free;
                }
                else
                {
                    mpz_set(gmp_f, g);
                    goto free;
                }
            }
            else
            {
                mpz_set(gmp_f, g);
                goto free;
            }


        free:
            //if (fobj->VFLAG >= 0)
            //	printf("\n");

            mpz_clear(x);
            mpz_clear(y);
            mpz_clear(q);
            mpz_clear(g);
            mpz_clear(ys);
            mpz_clear(t1);

            return it;
        }



        static uint _trail_zcnt64(ulong n) => (uint)MathLib.TrailingZeroCount(n);

        static uint MIN(uint a, uint b) => a < b ? a : b;

#if HAVEMONTY
        //from yafu:
        public static uint64_t spbrent(uint64_t N, uint64_t c, int imax)
        {


            /*
            run pollard's rho algorithm on n with Brent's modification,
            returning the first factor found in f, or else 0 for failure.
            use f(x) = x^2 + c
            see, for example, bressoud's book.
            */
            uint64_t x, y, q, g, ys, t1, f = 0, nhat;
            uint32_t i = 0, k, r, m;
            int it;

            // start out checking gcd fairly often
            r = 1;

            // under 48 bits, don't defer gcd quite as long
            i = (uint)_trail_zcnt64(N);
            if (i > 20)
                m = 32;
            else if (i > 16)
                m = 160;
            else if (i > 3)
                m = 256;
            else
                m = 384;

            it = 0;
            q = 1;
            g = 1;

            x = (((N + 2) & 4) << 1) + N; // here x*a==1 mod 2**4
            x *= 2 - N * x;               // here x*a==1 mod 2**8
            x *= 2 - N * x;               // here x*a==1 mod 2**16
            x *= 2 - N * x;               // here x*a==1 mod 2**32         
            x *= 2 - N * x;               // here x*a==1 mod 2**64
            nhat = (uint64_t)0 - x;

            // Montgomery representation of c
            c = u64div(c, N);
            y = c;

            do
            {
                x = y;
                for (i = 0; i <= r; i++)
                {
                    y = mulredc63(y, y + c, N, nhat);
                }

                k = 0;
                do
                {
                    ys = y;
                    for (i = 1; i <= MIN(m, r - k); i++)
                    {
                        y = mulredc63(y, y + c, N, nhat);
                        t1 = x > y ? y - x + N : y - x;
                        q = mulredc63(q, t1, N, nhat);
                    }

                    g = bingcd64(N, q);
                    k += m;
                    it++;

                    if (it > imax)
                    {
                        f = 0;
                        goto done;
                    }

                } while ((k < r) && (g == 1));
                r *= 2;
            } while (g == 1);

            if (g == N)
            {
                //back track
                do
                {
                    ys = mulredc63(ys, ys + c, N, nhat);
                    t1 = x > ys ? ys - x + N : ys - x;
                    g = bingcd64(N, t1);
                } while (g == 1);

                if (g == N)
                {
                    f = 0;
                }
                else
                {
                    f = g;
                }
            }
            else
            {
                f = g;
            }

        done:

            return f;
        }

        public static uint64_t spbrent64(uint64_t N, int imax)
        {


            /*
            run pollard's rho algorithm on n with Brent's modification,
            returning the first factor found in f, or else 0 for failure.
            use f(x) = x^2 + c
            see, for example, bressoud's book.
            */
            uint64_t x, y, q, g, ys, t1, f = 0, nhat;
            uint64_t c = 1;
            uint32_t i = 0, k, r, m;
            int it;

            // start out checking gcd fairly often
            r = 1;

            // under 48 bits, don't defer gcd quite as long
            i = _trail_zcnt64(N);
            if (i > 20)
                m = 32;
            else if (i > 16)
                m = 160;
            else if (i > 3)
                m = 256;
            else
                m = 384;

            it = 0;
            q = 1;
            g = 1;

            x = (((N + 2) & 4) << 1) + N; // here x*a==1 mod 2**4
            x *= 2 - N * x;               // here x*a==1 mod 2**8
            x *= 2 - N * x;               // here x*a==1 mod 2**16
            x *= 2 - N * x;               // here x*a==1 mod 2**32         
            x *= 2 - N * x;               // here x*a==1 mod 2**64
            nhat = (uint64_t)0 - x;

            // Montgomery representation of c
            c = u64div(c, N);
            y = c;

            do
            {
                x = y;
                for (i = 0; i <= r; i++)
                {
                    y = mulredc(y, y + c, N, nhat);
                }

                k = 0;
                do
                {
                    ys = y;
                    for (i = 1; i <= MIN(m, r - k); i++)
                    {
                        y = mulredc(y, y + c, N, nhat);
                        t1 = x > y ? y - x + N : y - x;
                        q = mulredc(q, t1, N, nhat);
                    }

                    g = bingcd64(N, q);
                    k += m;
                    it++;

                    if (it > imax)
                    {
                        f = 0;
                        goto done;
                    }

                } while ((k < r) && (g == 1));
                r *= 2;
            } while (g == 1);

            if (g == N)
            {
                //back track
                do
                {
                    ys = mulredc(ys, ys + c, N, nhat);
                    t1 = x > ys ? ys - x + N : ys - x;
                    g = bingcd64(N, t1);
                } while (g == 1);

                if (g == N)
                {
                    f = 0;
                }
                else
                {
                    f = g;
                }
            }
            else
            {
                f = g;
            }

        done:

            return f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t u64div(uint64_t c, uint64_t n)
        {
            uint64_t r;
            //mpz_t a;
            //mpz_init(a);
            //mpz_set_ui(a, c);
            //mpz_mul_2exp(a, a, 64);
            //r = mpz_tdiv_ui(a, n);
            //mpz_clear(a);
            // first available in Visual Studio 2019
            _udiv128(c, 0, n, &r);

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t mulredc(uint64_t x, uint64_t y, uint64_t n, uint64_t nhat)
        {
            uint64_t th, tl, u, ah, al;
            tl = _umul128(x, y, &th);
            u = tl * nhat;
            al = _umul128(u, n, &ah);
            tl = _addcarry_u64(0, al, tl, &al);
            th = _addcarry_u64((uint8_t)tl, th, ah, &x);
            if (th || (x >= n)) x -= n;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t mulredc63(uint64_t x, uint64_t y, uint64_t n, uint64_t nhat)
        {
            uint64_t th, tl, u, ah, al;
            tl = _umul128(x, y, &th);
            u = tl * nhat;
            al = _umul128(u, n, &ah);
            tl = _addcarry_u64(0, al, tl, &al);
            th = _addcarry_u64((uint8_t)tl, th, ah, &x);
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t sqrredc(uint64_t x, uint64_t n, uint64_t nhat)
        {
            uint64_t th, tl, u, ah, al;
            tl = _umul128(x, x, &th);
            u = tl * nhat;
            al = _umul128(u, n, &ah);
            tl = _addcarry_u64(0, al, tl, &al);
            th = _addcarry_u64((uint8_t)tl, th, ah, &x);
            if (th || (x >= n)) x -= n;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        staticuint64_t sqrredc63(uint64_t x, uint64_t n, uint64_t nhat)
        {
            uint64_t th, tl, u, ah, al;
            tl = _umul128(x, x, &th);
            u = tl * nhat;
            al = _umul128(u, n, &ah);
            tl = _addcarry_u64(0, al, tl, &al);
            th = _addcarry_u64((uint8_t)tl, th, ah, &x);
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t submod(uint64_t a, uint64_t b, uint64_t n)
        {
            uint64_t r0;
            if (_subborrow_u64(0, a, b, &r0))
                r0 += n;
            return r0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint32_t submod32(uint32_t a, uint32_t b, uint32_t n)
        {
            uint32_t r0;
            if (_subborrow_u32(0, a, b, &r0))
                r0 += n;
            return r0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t addmod(uint64_t x, uint64_t y, uint64_t n)
        {
#if add_carry_64
    uint64_t r;
    uint64_t tmp = x - n;
    uint8_t c = _addcarry_u64(0, tmp, y, &r);
    return (c) ? r : x + y;
#else
            // FYI: The clause above often compiles with a branch in MSVC.
            // The statement below often compiles without a branch (uses cmov) in MSVC.
            return (x >= n - y) ? x - (n - y) : x + y;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint32_t addmod32(uint32_t x, uint32_t y, uint32_t n)
        {
            // FYI: The clause above often compiles with a branch in MSVC.
            // The statement below often compiles without a branch (uses cmov) in MSVC.
            return (x >= n - y) ? x - (n - y) : x + y;
        }



        // good to 60 bit inputs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t sqrredc60(uint64_t x, uint64_t n, uint64_t nhat)
        {
            uint64_t th, tl, u, ah, al;
            uint8_t c;
            tl = _umul128(x, x, &th);
            u = tl * nhat;
            al = _umul128(u, n, &ah);
            c = _addcarry_u64(0, al, tl, &al);
            _addcarry_u64(c, th, ah, &x);
            return x;
        }


        // good to 60 bit inputs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t mulredc60(uint64_t x, uint64_t y, uint64_t n, uint64_t nhat)
        {
            uint64_t th, tl, u, ah, al;
            uint8_t c;
            tl = _umul128(x, y, &th);
            u = tl * nhat;
            al = _umul128(u, n, &ah);
            c = _addcarry_u64(0, al, tl, &al);
            _addcarry_u64(c, th, ah, &x);
            return x;
        }


        // this works if inputs are 62 bits or less
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static addmod60(ulong x, ulong y, ulong n) =>  ((x) + (y))
#endif

    }

}