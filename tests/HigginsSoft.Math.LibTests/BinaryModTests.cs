#define SKIP_LONG_TESTS

using MathGmp.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static HigginsSoft.Math.Lib.MathLib;
using intv = System.Numerics.Vector<int>;
namespace HigginsSoft.Math.Lib.Tests
{
    public static class VectorExensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> ToFloat(this Vector256<int> value)
            => Avx.ConvertToVector256Single(value);
    }

    public struct Vect256X
    {
        public const int Size = 256 / 8;
        private readonly ulong _00;
        private readonly ulong _01;
        private readonly ulong _02;
        private readonly ulong _03;

        public Vect256X(uint v)
        {
            _03 = _02 = _01 = _00 = ((ulong)v) << 32 | v;
        }

        internal static Vect256X Create(uint v)
        {
            return new Vect256X(v);
        }
        public Vect256X<TOut> As<TIn, TOut>()
            where TIn : struct
            where TOut : struct
        {
            //TODO: if system type the return system typ
            return new Vect256X<TOut>(_00, _01, _02, _03);
        }

        public unsafe static Vector<T> CreateVector<T>(Vect256X<T> vect256X)
            where T : struct
        {
            throw new NotImplementedException();
        }


    }
    [StructLayout(LayoutKind.Sequential, Size = Vect256X.Size)]
    public struct Vect256X<T>
        where T : struct
    {
        internal readonly ulong _00;
        internal readonly ulong _01;
        internal readonly ulong _02;
        internal readonly ulong _03;
        public unsafe Vect256X(ulong* ptr)
        {
            _00 = (ulong)ptr + 64 * 0;
            _01 = (ulong)ptr + 62 * 1;
            _02 = (ulong)ptr + 64 * 2;
            _03 = (ulong)ptr + 64 * 3;
        }
        public Vect256X(ulong _00, ulong _01, ulong _02, ulong _03)
        {
            this._00 = _00;
            this._01 = _01;
            this._02 = _02;
            this._03 = _03;
        }
        public static int Count => Vect256X.Size / Unsafe.SizeOf<T>();


        public static Vect256X<T> Zero => default;

        public static Vect256X<T> AllBitsSet => Vect256X.Create(0xFFFFFFFF).As<uint, T>();

        public Vector<T> AsVector() =>
            Vect256X.CreateVector<T>(this);


    }
    public class BinaryModTest
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BinaryMod(int a, int b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1;
            }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BinaryModV2(int a, int b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1;
            }

            var b1 = b;
            while (a >= b)
            {
                var t = b1 << 1;
                while (t > 0 && t < a)
                {
                    b1 = t;
                    t = b1 << 1;
                }
                a -= b1;
                if (a > b)
                {
                    t = b1;
                    while (t > 0 && t > a)
                    {
                        b1 = t;
                        t = t >> 1;
                    }
                }
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BinaryModLz(int a, int b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1;
            }

            int lena, lenb, b1, shift;
            while (a >= b)
            {
                lena = 32 - (int)Lzcnt.LeadingZeroCount((uint)a);
                lenb = 32 - (int)Lzcnt.LeadingZeroCount((uint)b);
                shift = lena - lenb;
                b1 = b << shift;
                if (b1 > a)
                    b1 >>= 1;
                a -= b1;
            }
            return a;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static GmpInt BinaryModGmp(GmpInt a, GmpInt b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1;
            }

            int lena, lenb, shift;
            GmpInt b1;
            while (a >= b)
            {
                lena = a.BitLength;
                lenb = b.BitLength;
                shift = lena - lenb;
                b1 = b << shift;
                if (b1 > a)
                    b1 >>= 1;
                a -= b1;
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static GmpInt BinaryModMpz(GmpInt a, GmpInt b)
        {
            int t = gmp_lib.mpz_cmp(a.Data, b.Data);
            if (t < 1)
            {
                if (t == 0) return 0;
                return a;
            }

            //  if (b < 3)
            t = gmp_lib.mpz_cmp_si(b.Data, 3);
            if (t < 0)
            {
                // if (b < 2) return 0;
                t = gmp_lib.mpz_cmp_si(b.Data, 2);
                if (t < 0) return 0;
                return a & 1;

            }


            int shift;
            //GmpInt b1;
            mpz_t b1 = new();
            gmp_lib.mpz_init(b1);
            //while (a >= b)
            do
            {



                //lena = a.BitLength;
                t = (int)gmp_lib.mpz_sizeinbase(a, 2);
                gmp_lib.mpz_set(b1, b.Data);
                shift = (int)gmp_lib.mpz_sizeinbase(b1, 2);
                shift = t - shift;
                //b1 = b << shift;
                // can we calculate the correct shift, so we don't have to shift twice?
                if (true)
                {
                    if (shift > 1)
                    {
                        shift -= 1;
                        gmp_lib.mpz_mul_2exp(b1, b1, (uint)shift);
                    }
                }
                else
                {

                    gmp_lib.mpz_mul_2exp(b1, b1, (uint)shift);
                    //if (b1 > a) b1 >>= 1;

                    t = gmp_lib.mpz_cmp(b1, a.Data);
                    if (t > 0)
                        gmp_lib.mpz_tdiv_q_2exp(b1, b1, 1);
                }
                //a -= b1;
                gmp_lib.mpz_sub(a.Data, a.Data, b1);

                t = gmp_lib.mpz_cmp(a.Data, b.Data);
            } while (t > -1);
            gmp_lib.mpz_clear(b1);
            return a;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint BinaryModLzU(uint a, uint b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1u;
            }

            uint lena, lenb, b1;
            uint shift;
            while (a >= b)
            {
                lena = 32u - Lzcnt.LeadingZeroCount(a);
                lenb = 32u - Lzcnt.LeadingZeroCount(b);
                shift = lena - lenb;
                b1 = b << (int)shift;
                if (b1 > a)
                    b1 >>= 1;
                a -= b1;
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint BinaryModLzU_V3(uint a, uint b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1u;
            }

            uint b1;
            uint t;
            while (a >= b)
            {
                b1 = Lzcnt.LeadingZeroCount(a);
                t = Lzcnt.LeadingZeroCount(b);
                b1 -= t;
                b1 = 32u - b1;
                b1 = b << (int)b1;
                if (b1 > a)
                    b1 >>= 1;
                a -= b1;
            }
            return a;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint BinaryModLzU_V2(uint a, uint b)
        {
            if (a == b) return 0;
            if (b < 3)
            {
                if (b < 2) return 0;
                return a & 1u;
            }
            uint t;
            if (a >= b)
            {
                t = b << (int)((32u - Lzcnt.LeadingZeroCount(a)) - (32u - Lzcnt.LeadingZeroCount(b)));
                while (a >= b)
                {
                    while (t > a)
                    {
                        t >>= 1;
                    }
                    a -= t;
                }
            }

            return a;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (int , uint Count) BinaryModCount(int a, int b)
        {
            if (a == b) return (0, 0);
            if (b == 0) return (a, 0);
            uint count = 0;
            while (a >= b)
            {
                var b1 = b;
                var t = b1 << 1;
                while (t > 0 && t < a)
                {
                    count++;
                    b1 = t;
                    t = t << 1;
                }
                a -= b1;
                count++;
            }
            return (a, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (int , uint Count) BinaryModCountV2(int a, int b)
        {
            if (a == b) return (0, 0);
            if (b == 0) return (a, 0);
            uint count = 0;
            var b1 = b;
            while (a >= b)
            {
                var t = b1 << 1;
                while (t > 0 && t < a)
                {
                    count++;
                    b1 = t;
                    t = t << 1;
                }
                a -= b1;
                if (a > b)
                {
                    t = b1;
                    while (t > 0 && t > a)
                    {
                        count++;
                        b1 = t;
                        t = t >> 1;
                    }
                }
                count++;
            }
            return (a, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (uint , uint Count) BinaryModLzUCount(uint a, uint b)
        {
            if (a == b) return (0, 0);
            if (b < 3)
            {
                if (b < 2) return (0, 0);
                return (a & 1u, 0);
            }

            uint lena, lenb, b1;
            uint shift, count = 0;
            while (a >= b)
            {
                count++;
                lena = 32u - Lzcnt.LeadingZeroCount(a);
                lenb = 32u - Lzcnt.LeadingZeroCount(b);
                shift = lena - lenb;
                b1 = b << (int)shift;
                if (b1 > a)
                    b1 >>= 1;
                a -= b1;
            }
            return (a, count);
        }


        static int[] result = new int[Vector128<int>.Count];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static intv BinaryModAvx(intv a, intv b)
        {
            var a256 = a.AsVector256();
            var b256 = b.AsVector256();
            var a256F = Avx.ConvertToVector256Single(a256);
            var b256F = Avx.ConvertToVector256Single(b256);

            var comp = Avx.CompareGreaterThanOrEqual(a256F, b256F);
            var mask = Avx.MoveMask(comp);

            //     while (a >= b)
            while (mask > 0)
            {
                //var b1 = b;
                var b1 = b256;

                // var t = b1 << 1;
                var t = Avx2.ShiftLeftLogical(a256, 1);
                var gtZero = Avx.CompareGreaterThan(t.ToFloat(), Vector256<float>.Zero);
                var ltA = Avx.CompareGreaterThan(t.ToFloat(), a256F);
                var gtMask = Avx.MoveMask(gtZero);
                var ltMask = Avx.MoveMask(ltA);
                var tGtZeroAndTLessThanA = gtMask & ltMask;

                //while (t > 0 && t < a)
                while (tGtZeroAndTLessThanA > 0)
                {
                    //  b1 = t;
                    b1 = t;

                    // t = b1 << 1;
                    t = Avx2.ShiftLeftLogical(a256, 1);

                    // t > 0 && t < a
                    gtZero = Avx.CompareGreaterThan(t.ToFloat(), Vector256<float>.Zero);
                    ltA = Avx.CompareGreaterThan(t.ToFloat(), a256F);
                    gtMask = Avx.MoveMask(gtZero);
                    ltMask = Avx.MoveMask(ltA);

                    //todo: move elements to results;
                    tGtZeroAndTLessThanA = gtMask & ltMask;
                }
                //a -= b1;
                a256 = Avx2.Subtract(a256, b1);
            }

            //return a;
            return a;



        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestClass]
        public class BinaryModTests
        {
            const int BinaryModTestCountPower = 16;
            const int BinaryModStart = 1 << 28;
            const int BinaryModTestCount = 1 << BinaryModTestCountPower;
            const ulong BinaryModTotalTests = (1ul << BinaryModTestCountPower) * (1ul << BinaryModTestCountPower);
            const int BinaryModLimit = BinaryModStart + BinaryModTestCount;

            [TestMethod]
            public void BinaryModT_Mpz_GmpVsBinMod_Rsa1024_Test()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    uint p = (uint)primes[i];



                    var expected = rsa % p;
                    sw.Stop();
                    var expectedElapsed = sw.Elapsed;

                    sw = Stopwatch.StartNew();

                    int shiftCount = 0;
                    int subCount = 0;
                    int opCount = 0;

                    var lenA = rsa.BitCount;
                    var lenB = 32u - Lzcnt.LeadingZeroCount(p);
                    var shift = (int)(lenA - lenB);
                    var GmpDivisor = (GmpInt)p;
                    GmpDivisor <<= shift;

                    if (GmpDivisor > rsa)
                    {
                        shiftCount++;
                        GmpDivisor >>= 1;
                    }

                    var diff = rsa - GmpDivisor;
                    subCount++;
                    opCount++;
                    if (bool.Parse(bool.FalseString))
                    {
                        var diffSize = diff.BitCount;
                        Console.WriteLine($"Single op reduced N from {lenA} bits to {diffSize} bits");
                    }


                    var rsaRes = diff;

                    while (rsaRes >= p)
                    {
                        while (GmpDivisor > rsaRes)
                        {
                            GmpDivisor >>= 1;
                            shiftCount++;
                        }
                        rsaRes -= GmpDivisor;
                        subCount++;
                        opCount++;

                        //Console.WriteLine($"Op {opCount} reduced N from {lenA} bits to {rsa.BitCount} bits");
                    }

                    sw.Stop();
                    var actualElapsed = sw.Elapsed;
                    Console.WriteLine($"Performed {opCount} operations mod {lenA} using {shiftCount} shifts and {subCount} subractions in {sw.Elapsed}");
                    var actual = rsaRes;
                    Console.WriteLine($"Expected = {expected} in {expectedElapsed}");
                    Console.WriteLine($"Actual = {actual} in {actualElapsed}");
                }
            }




            [TestMethod]
            public void BinaryModT_Mpz_Rsa2014_GmpMod_Test_2p16()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                int count = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    var a = primes[i];
                    var res = rsa % a;
                    if (res > -1)
                        count++;
                }

                sw.Stop();
                Console.WriteLine($"{nameof(GmpInt)} % performed {count.ToString("N0")} mod operations in {sw.Elapsed}");
            }

            [TestMethod]
            public void BinaryModT_Mpz_Rsa2014_MpzBinMod_Test_2p16()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                int count = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    var a = primes[i];
                    var res = BinaryModMpz(rsa, a);
                    if (res > -1)
                        count++;

                    if (false)
                    {
                        var expected = rsa % a;
                        if (expected != res)
                        {
                            string message = $"{nameof(BinaryModMpz)} failed for {nameof(rsa)} mod {a}";
                            Assert.AreEqual(expected, res, message);
                        }
                    }
                }

                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModMpz)} performed {count.ToString("N0")} mod operations in {sw.Elapsed}");
            }


            [TestMethod]
            public void BinaryModT_Mpz_Rsa2014_BinMod_Test_2p16()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                int count = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    var a = primes[i];
                    var res = BinaryModGmp(rsa, a);
                    if (res > -1)
                        count++;

                    if (false)
                    {
                        var expected = rsa % a;
                        if (expected != res)
                        {
                            string message = $"{nameof(BinaryModGmp)} failed for {nameof(rsa)} mod {a}";
                            Assert.AreEqual(expected, res, message);
                        }
                    }

                }

                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModT_Mpz_Rsa2014_BinMod_Test_2p16)} performed {count.ToString("N0")} mod operations in {sw.Elapsed}");
            }

            [TestMethod]
            public void BinaryModT_Mpz_Rsa1024_PlusRoot_GmpMod_Test_2p16()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var root = MathLib.Sqrt(rsa);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                int count = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    var a = primes[i] + root;
                    var res = rsa % a;
                    if (res > -1)
                        count++;
                }

                sw.Stop();
                Console.WriteLine($"{nameof(GmpInt)} % performed {count.ToString("N0")} mod operations in {sw.Elapsed}");
            }

            [TestMethod]
            public void BinaryModT_Mpz_Rsa1024_PlusRoot_MpzMod_Test_2p16()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var root = MathLib.Sqrt(rsa);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                int count = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    var a = primes[i] + root;
                    var res = BinaryModMpz(rsa, a);
                    if (res > -1)
                        count++;

                    if (false) // set to true to run test,false to time. If false opmization will remove without need for pragma.
                    {
                        var expected = rsa % a;
                        if (expected != res)
                        {
                            string message = $"{nameof(BinaryModMpz)} failed for {nameof(rsa)} mod {a}";
                            Assert.AreEqual(expected, res, message);
                        }
                    }

                }

                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModMpz)} performed {count.ToString("N0")} mod operations in {sw.Elapsed}");
            }



            [TestMethod]
            public void BinaryModT_Mpz_Rsa1024_PlusRoot_BinMod_Test_2p16()
            {
                int rsaBits = 1024;
                var rsa = MathLib.Rsa(rsaBits);
                var root = MathLib.Sqrt(rsa);
                var primes = new PrimeGeneratorUnsafe(BinaryModTestCount).ToList();
                int count = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < primes.Count; i++)
                {
                    var a = primes[i] + root;
                    var res = BinaryModGmp(rsa, a);
                    if (res > -1)
                        count++;
                }

                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModGmp)} performed {count.ToString("N0")} mod operations in {sw.Elapsed}");
            }



            [TestMethod]
            public void RunBinaryModCountTests()
            {
                Dictionary<uint, uint> counts = new();
                int worstA = 0, worstB = 0;
                uint worstCount = 0;
                var sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModStart; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModStart; b++)
                    {
                        var res = BinaryModCount(a, b);
                        if (res.Count > worstCount)
                        {
                            worstCount = res.Count;
                            worstA = a;
                            worstB = b;
                        }
                        if (!counts.ContainsKey(res.Count))
                            counts.Add(res.Count, 1);
                        else counts[res.Count]++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"$Worst: {worstA} % {worstB} => Count: {worstCount} in {sw.Elapsed}");
                var ordered = counts.OrderBy(x => x.Key);
                foreach (var kvp in ordered)
                {
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
                }
            }

            [TestMethod]
            public void RunBinaryModCountV2Tests()
            {
                Dictionary<uint, uint> counts = new();
                int worstA = 0, worstB = 0;
                uint worstCount = 0;
                var sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModStart; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModStart; b++)
                    {
                        var res = BinaryModCountV2(a, b);
                        if (res.Count > worstCount)
                        {
                            worstCount = res.Count;
                            worstA = a;
                            worstB = b;
                        }
                        if (!counts.ContainsKey(res.Count))
                            counts.Add(res.Count, 1);
                        else counts[res.Count]++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"$Worst: {worstA} % {worstB} => Count: {worstCount} in {sw.Elapsed}");
                var ordered = counts.OrderBy(x => x.Key);
                foreach (var kvp in ordered)
                {
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
                }
            }

            [TestMethod]
            public void RunBinaryModCountLZTests()
            {
                Dictionary<uint, uint> counts = new();
                uint worstA = 0, worstB = 0;
                uint worstCount = 0;
                var sw = Stopwatch.StartNew();
                for (uint a = BinaryModStart; a < BinaryModStart; a++)
                {
                    for (uint b = BinaryModStart; b < BinaryModStart; b++)
                    {
                        var res = BinaryModLzUCount(a, b);
                        if (res.Count > worstCount)
                        {
                            worstCount = res.Count;
                            worstA = a;
                            worstB = b;
                        }
                        if (!counts.ContainsKey(res.Count))
                            counts.Add(res.Count, 1);
                        else counts[res.Count]++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"$Worst: {worstA} % {worstB} => Count: {worstCount} in {sw.Elapsed}");
                var ordered = counts.OrderBy(x => x.Key);
                foreach (var kvp in ordered)
                {
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
                }
            }

            [TestMethod]
            public void RunBinaryModTests()
            {
                var b = 15;
                var a = 5;
                var expected = 0;
                var actual = BinaryMod(b, a);
                Assert.AreEqual(expected, actual);

                a = 4;
                expected = b % a;
                actual = BinaryMod(b, a);
                Assert.AreEqual(expected, actual);

                var limit = 1 << 16;
                for (var i = 2; i < limit; i++)
                {
                    b = i + 1;
                    a = i;
                    expected = b % a;
                    actual = BinaryMod(b, a);
                    Assert.AreEqual(expected, actual);
                }

                var primes = Primes.IntFactorPrimes;
                for (var i = 0; i < primes.Length - 2; i++)
                {
                    var p0 = primes[i];
                    var p1 = primes[i + 1];
                    var p2 = primes[i + 2];
                    b = p2 * p1;
                    a = p1 * p0;
                    expected = b % a;
                    actual = BinaryMod(b, a);
                    if (expected != actual)
                    {
                        string message = $"Binary Mod Failed for {b} % {a} = {expected}";
                        Assert.AreEqual(expected, actual, message);
                    }

                }

                b = int.MaxValue;
                a = (int)MathLib.MAX_INT_SQUARE_ROOT;
                expected = b % a;
                actual = BinaryMod(b, a);
                Assert.AreEqual(expected, actual);

            }

            [TestMethod]
            public void TimeMod_1000_BinaryMod_Tests()
            {
                var sw = Stopwatch.StartNew();
                var tests = 1000;
                for (var t = 0; t < tests; t++)
                {

                    var b = 15;
                    var a = 5;

                    var actual = BinaryMod(b, a);
                    Assert.IsTrue(actual < a);

                    b = 4;

                    actual = BinaryMod(a, b);
                    Assert.IsTrue(actual < a);

                    var limit = 1 << 16;
                    for (var i = 2; i < limit; i++)
                    {
                        b = i + 1;
                        a = i;

                        actual = BinaryMod(b, a);
                        Assert.IsTrue(actual < a);
                    }

                    var primes = Primes.IntFactorPrimes;
                    for (var i = 0; i < primes.Length - 2; i++)
                    {
                        var p0 = primes[i];
                        var p1 = primes[i + 1];
                        var p2 = primes[i + 2];
                        b = p2 * p1;
                        a = p1 * p0;

                        actual = BinaryMod(b, a);
                        Assert.IsTrue(actual < a);

                    }

                    b = int.MaxValue;
                    a = (int)MathLib.MAX_INT_SQUARE_ROOT;

                    actual = BinaryMod(b, a);
                    Assert.IsTrue(actual < a);
                }
                sw.Stop();
                Console.WriteLine($"{nameof(TimeMod_1000_BinaryMod_Tests)} completed in {sw.Elapsed}");
            }


            [TestMethod]
            public void TimeMod_BinaryMod_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryMod(a, b);
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryMod)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }

            [TestMethod]
            public void TimeMod_BinaryMod_V2_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryModV2(a, b);
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModV2)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }



            [TestMethod]
            public void TimeMod_BinaryMod_Lz_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();

                for (var a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryModLz(a, b);
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModLz)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }

            [TestMethod]
            public void TimeMod_BinaryMod_LzV2_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (uint a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (uint b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryModLzU_V2(a, b);
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModLzU_V2)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }

            [TestMethod]
            public void TimeMod_BinaryMod_LzV3_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (uint a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (uint b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryModLzU_V3(a, b);
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModLzU_V3)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }


            [TestMethod]
            public void TimeMod_BinaryMod_LzU_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (uint a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (uint b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryModLzU(a, b);
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModLzU)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }


            [TestMethod]
            public void TimeMod_SystemMod_ToLimitTests()
            {
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = a % b;
                        if (res > -1) //prevent JIT optimization from removing the binary mod call
                            count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"System Mod completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }

            [TestMethod]
            public void TimeMod_Versions_ToLimitTests()
            {

                const int limit = 1 << 16;
                uint count = 0;
                var sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryMod(a, b);
                        count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryMod)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                var elapsedMs = sw.ElapsedMilliseconds;
                var opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");

                count = 0;
                sw = Stopwatch.StartNew();
                for (var a = BinaryModStart; a < BinaryModLimit; a++)
                {
                    for (var b = BinaryModStart; b < BinaryModLimit; b++)
                    {
                        var res = BinaryModV2(a, b);
                        count++;
                    }
                }
                sw.Stop();
                Console.WriteLine($"{nameof(BinaryModV2)} completed {BinaryModTotalTests.ToString("N0")} tests from {BinaryModStart.ToString("N0")} to {BinaryModLimit.ToString("N0")} in {sw.Elapsed}");
                elapsedMs = sw.ElapsedMilliseconds;
                opRate = (double)BinaryModTotalTests / elapsedMs / 1000;
                Console.WriteLine($"{nameof(BinaryModLzU)} OpRate {opRate.ToString("N4")} ops/ms");
            }


            [TestMethod]
            public void TimeMod_1000_SystemMod_Tests()
            {
                var sw = Stopwatch.StartNew();
                var tests = 1000;
                for (var t = 0; t < tests; t++)
                {
                    var b = 15;
                    var a = 5;

                    var actual = b % a;
                    Assert.IsTrue(actual < a);

                    b = 4;

                    actual = b % a;
                    Assert.IsTrue(actual < a);

                    var limit = 1 << 16;
                    for (var i = 2; i < limit; i++)
                    {
                        b = i + 1;
                        a = i;

                        actual = b % a;
                        Assert.IsTrue(actual < a);
                    }

                    var primes = Primes.IntFactorPrimes;
                    for (var i = 0; i < primes.Length - 2; i++)
                    {
                        var p0 = primes[i];
                        var p1 = primes[i + 1];
                        var p2 = primes[i + 2];
                        b = p2 * p1;
                        a = p1 * p0;

                        actual = b % a;
                        Assert.IsTrue(actual < a);

                    }

                    b = int.MaxValue;
                    a = (int)MathLib.MAX_INT_SQUARE_ROOT;

                    actual = b % a;
                    Assert.IsTrue(actual < a);

                }
                sw.Stop();
                Console.WriteLine($"{nameof(TimeMod_1000_SystemMod_Tests)} completed in {sw.Elapsed}");
            }
        }


        [TestClass]
        public class BinaryModAvxTests
        {




            [TestMethod]
            public void RunBinaryModTests()
            {
                var b = 15;
                var a = 5;
                var expected = 0;
                var actual = BinaryMod(b, a);
                Assert.AreEqual(expected, actual);

                a = 4;
                expected = b % a;
                actual = BinaryMod(b, a);
                Assert.AreEqual(expected, actual);

                var limit = 1 << 16;
                for (var i = 2; i < limit; i++)
                {
                    b = i + 1;
                    a = i;
                    expected = b % a;
                    actual = BinaryMod(b, a);
                    Assert.AreEqual(expected, actual);
                }

                var primes = Primes.IntFactorPrimes;
                for (var i = 0; i < primes.Length - 2; i++)
                {
                    var p0 = primes[i];
                    var p1 = primes[i + 1];
                    var p2 = primes[i + 2];
                    b = p2 * p1;
                    a = p1 * p0;
                    expected = b % a;
                    actual = BinaryMod(b, a);
                    if (expected != actual)
                    {
                        string message = $"Binary Mod Failed for {b} % {a} = {expected}";
                        Assert.AreEqual(expected, actual, message);
                    }

                }

                b = int.MaxValue;
                a = (int)MathLib.MAX_INT_SQUARE_ROOT;
                expected = b % a;
                actual = BinaryMod(b, a);
                Assert.AreEqual(expected, actual);

            }

        }
    }
}