/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/
#define LOG
#undef LOG


using BenchmarkDotNet.Disassemblers;
using HigginsSoft.Math.Lib;
using MathGmp.Native;
using Microsoft.Diagnostics.Tracing.StackSources;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static HigginsSoft.Math.Demos.Qs16;
using static HigginsSoft.Math.Lib.MathLib.Vect;

namespace HigginsSoft.Math.Demos
{
    public ref struct Qs16Avx
    {
        public Qs16Avx()
        {

        }
        //static readonly int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19 };
        static (int i, int sq, int res, int mask)[] factorizations32 = new (int i, int sq, int res, int mask)[20];
        static (long i, long sq, long res, long mask)[] factorizationsLong = new (long i, long sq, long res, long mask)[20];

        static (long i, long sq, long res, BitArray mask)[] factorizations = new (long i, long sq, long res, BitArray mask)[20];

        public static FactStruct DefaultFactStruct => new FactStruct(new[] { 2, 3, 5, 7, 11, 13, 17, 19 });
        static AvxDivRemData32_8_Divisors divData = AvxDivRemData32.CreateSmallPrimesData();

        static int primecount64 = 8;
        static AvxDivRemData64 div64Data = AvxDivRemData64.Create(Primes.IntFactorPrimes.Take(primecount64).ToArray());


        public static int PrimeCount64
        {
            get
            {
                return PrimeCount64;
            }
            set
            {
                if (primecount64!=value)
                {
                    primecount64 = value;
                    div64Data = AvxDivRemData64.Create(Primes.IntFactorPrimes.Take(value).ToArray());
                    factorizations = new (long i, long sq, long res, BitArray mask)[value << 1];
                }   
            }
        }
        public int PrimeCount { get => primecount64; set => PrimeCount64 = value; }

        SquareGeneratorAvx gen = new SquareGeneratorAvx();
        SquareGeneratorAvx64 gen64 = new SquareGeneratorAvx64();




        public FactStruct FactorUnchecked(int value, FactStruct result)
        {


            Array.Clear(factorizations32);
            result.Clear();
            int nt = value;
            if (MathLib.IsPerfectSquare(value, out int root))
            {

                FactStruct.FactorTrialDivide(root, result);
                result.Add(result);
                return result;
            }



            var gen = new SquareGeneratorAvx(value);
            bool done = !gen.MoveNext();
            int i = root + 1;
            const int vecSize = 8;
            var nvec32 = new Vector<int>(value);
            //var factorizations = new (int i, int sq, int res, int mask)[20];
            int factorizationsCount = 0;

            int factorBaseMask = 0;
            const int additionalRels = 2;
            while (!done)
            {
                //read vector of 8 squares of i*i
                var vec256 = gen.Current;
                var vec32 = vec256.AsVector<int>();

                //TODO: if vec32.Any(x=> x>n) need to perform mod instead of subtract
                // var res= i*i-n;
                var s = vec32 - nvec32;

                for (var j = 0; j < vecSize; j++, i++)
                {
                    var res = s[j];
                    if (res > nt)
                        res = res % nt;
                    if (res == 0)
                    {
                        if (GcdCheck(value, i + res, i - res, result))
                        {
                            return result;
                        }
                    }
                    else if (res == 1)
                    {
                        if (GcdCheck(value, i + 1, i - 1, result))
                        {
                            return result;
                        }
                    }

                    if (IsBSmooth(res, divData, out int factorMask, out int mask))
                    {
                        //if (mask == 0)
                        //{
                        //    if (GcdCheck(value, i + res, i - res, result))
                        //    {
                        //        return result;
                        //    }
                        //}

                        factorBaseMask = factorBaseMask | factorMask;     
                        factorizations32[factorizationsCount++] = new(i, vec32[j], res, mask);
                        if (factorizationsCount >= factorizations32.Length)
                        {
                            break;
                        }
                    }
                }
                var factorBaseCount = MathLib.PopCount(factorBaseMask);

                bool doneFactors = factorizationsCount > factorBaseCount + min_extra_s;

                bool noMoreSquares = !gen.MoveNext();
                done = doneFactors || noMoreSquares;
            }

            /*
            var l = Enumerable.Range(root + 1, 1000)
                .Select(i => new { i, sq = i * i })
                .Select(x => new { x.i, x.sq, res = x.sq % value })
                .Select(x =>
                {
                    var fmask = 0;
                    return new
                    {
                        x.i,
                        x.sq,
                        x.res,
                        bsmooth = (nt = x.res) + (fmask = 0) == 0 || 1 <= primes.Aggregate(1, (m, p) =>
                        {
                            while (nt > 1 && nt % p == 0)
                            {
                                nt /= p;
                                fmask = fmask ^ m;
                            }
                            m <<= 1;
                            return m;
                        }) && nt < 2,
                        mask = fmask,
                    };
                })
                .Where(x => x != null && x.bsmooth)
                //.OrderBy(x => x.mask)
                .ToList();

            */

            /*
            // TODO: port matrix solver to avx
            List<(int i, int mask, int res)> l;
            IEnumerable<(Func<GmpInt>, Func<GmpInt>)>
                recurse(int start, int mask, Func<GmpInt> getProduct, Func<GmpInt> getRes)
            {
                // depth first search can be done in one loo
                if (mask == 0)
                {
                    var product = getProduct();
                    var res = getRes();
                    if (MathLib.IsPerfectSquare(res, out GmpInt fact)
                        || MathLib.IsPerfectSquare(product, out fact)
                        || (fact = MathUtil.Gcd(value, product)) > 1 && fact != value)
                        yield return (() => product,
                                      () => fact);
                }
                else
                    for (int i = start; i < l.Count; i++)
                    {
                        int nextMask;
                        var f = l[i];
                        if ((mask & f.mask) > 0)
                        {
                            if ((nextMask = (mask ^ f.mask)) == 0)
                                yield return (() => getProduct() * f.i,
                                    () => getRes() * f.res);
                            else
                                foreach (var result in recurse(
                                       i + 1,
                                       nextMask,
                                       () => getProduct() * f.i,
                                       () => getRes() * f.res)
                                    )
                                    yield return result;
                        }
                    }
            }
            */

            /*

            var cmp = new PopComparer();
            var tmp = l.ToList();
            l = tmp.Take(20).OrderBy(x => x.mask, cmp).ToList();

            var f = l.Find(x =>
            {
                var solved = false;
                var solutions = recurse(i + 1, x.mask, () => x.i, () => x.res);
                foreach (var solution in solutions)
                {
                    var product = solution.Item1();
                    var res = solution.Item2();
                    if (GcdCheck(value, product + res, product - res, result))
                    {
                        solved = true;
                        break;
                    }
                }
                i++;
                return solved;
            });
            if (result.GetProduct() != value)
            {
                string bp = "";
            }*/
            return result;
        }


        public FactorizationInt FactorIntUnchecked(int value)
        {

            FactorizationInt result = new();
            Array.Clear(factorizations32);

            int nt = value;
            if (MathLib.IsPerfectSquare(value, out int root))
            {

                var f = FactorizationInt.FactorTrialDivide(root);
                result.Add(result);
                result.Add(f);
                return result;
            }


            gen.MoveTo(value);

            bool done = !gen.MoveNext();
            int i = root + 1;
            const int vecSize = 8;

            var nvec32 = new Vector<int>(value);
            //var factorizations = new (int i, int sq, int res, int mask)[20];
            int factorizationsCount = 0;

            int factorBaseMask = 0;
            const int additionalRels = 2;
            while (!done)
            {
                //read vector of 8 squares of i*i
                var vec256 = gen.Current;
                var vec32 = vec256.AsVector<int>();

                //TODO: if vec32.Any(x=> x>n) need to perform mod instead of subtract
                // var res= i*i-n;
                var s = vec32 - nvec32;

                for (var j = 0; j < vecSize; j++, i++)
                {
                    var res = s[j];
                    if (res > nt)
                        res = res % nt;
                    if (res == 0)
                    {
                        if (GcdCheck(value, i + res, i - res, result))
                        {
                            return result;
                        }
                    }
                    else if (res == 1)
                    {
                        if (GcdCheck(value, i + 1, i - 1, result))
                        {
                            return result;
                        }
                    }

                    if (IsBSmooth(res, divData, out int factorMask, out int mask))
                    {
                        //if (mask == 0)
                        //{
                        //    if (GcdCheck(value, i + res, i - res, result))
                        //    {
                        //        return result;
                        //    }
                        //}
                   
                        factorBaseMask = factorBaseMask | factorMask;
                        factorizations32[factorizationsCount++] = new(i, vec32[j], res, mask);
                        if (factorizationsCount >= factorizations32.Length)
                            break;
                    }
                }
                var factorBaseCount = MathLib.PopCount(factorBaseMask);


                bool doneFactors = factorizationsCount > factorBaseCount + min_extra_s;

                bool noMoreSquares = !gen.MoveNext();
                done = doneFactors || noMoreSquares;
                //done = factorBaseCount <= (factCount + additionalRels) || !gen.MoveNext();
            }


            return result;
        }

        public FactorizationInt64 FactorIntUnchecked(long value, TimeSpan? maxTimeout = null)
        {
            if (maxTimeout == null)
                maxTimeout = TimeSpan.MaxValue;
            FactorizationInt64 result = new();
            Array.Clear(factorizations32);


            long n = value;
            if (MathLib.IsPerfectSquare(value, out uint root))
            {

                var f = FactorizationInt64.FactorTrialDivide(root);
                result.Add(result);
                result.Add(f);
                return result;
            }

            var gen = gen64;
            gen.MoveTo(value);

            bool done = !gen.MoveNext();
            long i = root + 1;
            const int vecSize = 4;
            var nvec32 = new Vector<long>(value);
            //var factorizations = new (int i, int sq, int res, int mask)[20];
            int factorizationsCount = 0;

            int factorBaseSize = div64Data.d_divisors.Length;
            var fMask = new BitArray(factorBaseSize);
            var gf2mask = new BitArray(factorBaseSize);
            var Mask = new BitArray(factorBaseSize);
            const int additionalRels = 2;
            var sw = Stopwatch.StartNew();
            int factorBaseCount = 0;
            int count = 0;
            while (!done)
            {
                if (sw.Elapsed > maxTimeout)
                {
                    Console.WriteLine($" ==> Timeout for {n} with {factorizationsCount} factorizations and {factorBaseCount} primes from {count} s in {sw.Elapsed}.");
                    return result;
                }
                //read vector of 8 squares of i*i
                var vec256 = gen.Current;
                var vec32 = vec256.AsVector<long>();

                //TODO: if vec32.Any(x=> x>n) need to perform mod instead of subtract
                // var res= i*i-n;
                var s = vec32 - nvec32;

                for (var j = 0; j < vecSize; j++, i++)
                {
                    count++;
                    var res = s[j];
                    if (res > n)
                        res = res % n;
                    if (res == 0)
                    {
                        if (GcdCheck(value, i + res, i - res, result))
                        {
                            return result;
                        }
                    }
                    else if (res == 1)
                    {
                        if (GcdCheck(value, i + 1, i - 1, result))
                        {
                            return result;
                        }
                    }


                    //TODO: quick prime check: compute 2^(-1) mod .

                    if (IsBSmooth(res, div64Data, Mask, gf2mask))
                    {
                        //if (mask == 0)
                        //{
                        //    if (GcdCheck(value, i + res, i - res, result))
                        //    {
                        //        return result;
                        //    }
                        //}
                        bool verifyFactors = bool.Parse(bool.FalseString);

                        if (verifyFactors)
                        {
                            var intf = FactorizationInt.FactorTrialDivide(res);
                            for (var k = 0; k < intf.Factors.Count; k++)
                            {
                                int p = intf.Factors[k].P;
                                var idx = Array.IndexOf(div64Data.Divisors, p);
                                if (idx == -1)
                                {
                                    Console.WriteLine($" -> Factorization contains prime {p} outside of factorbase");
                                }
                            }
                        }

                        fMask = fMask.Or(Mask);
                        factorizations[factorizationsCount++] = new(i, vec32[j], res, new BitArray(gf2mask));
                    }
                }
                //factorBaseCount = MathLib.PopCount((ulong)factorBaseMask);
                factorBaseCount = fMask.Count;

                bool doneFactors = factorizationsCount > factorBaseCount + min_extra_s;

                bool noMoreSquares = !gen.MoveNext();
                done = doneFactors || noMoreSquares;
                //done = factorBaseCount <= (factCount + additionalRels) || !gen.MoveNext();
                if (done)
                {
                    if (noMoreSquares)
                    {
                        Console.WriteLine($" -> Sieving failed due to no more squares in {sw.Elapsed}");
                    }
                    else
                    {
                        Console.WriteLine($" -> Sieving completed with {factorizationsCount} factorizations using {factorBaseCount} primes from {count} s in {sw.Elapsed}");
                    }
                    break;
                }

            }


            return result;
        }


        static bool GcdCheck(int n, int p, int q, FactStruct result)
        {
            bool ret;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd < 2 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = (gcd > 1 && gcd != n))
            {
                FactStruct.FactorTrialDivide(gcd, result);
                FactStruct.FactorTrialDivide(n / gcd, result);
            }
            return ret;
        }


        static bool GcdCheck(int n, int p, int q, FactorizationInt result)
        {
            bool ret;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd < 2 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = (gcd > 1 && gcd != n))
            {
                result.Add(FactorizationInt.FactorTrialDivide((int)gcd));
                result.Add(FactorizationInt.FactorTrialDivide((int)(n / gcd)));

            }
            return ret;
        }

        static bool GcdCheck(long n, long p, long q, FactorizationInt64 result)
        {
            bool ret;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd < 2 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = (gcd > 1 && gcd != n))
            {
                result.Add(FactorizationInt64.FactorTrialDivide(gcd));
                result.Add(FactorizationInt64.FactorTrialDivide(n / gcd));

            }
            return ret;
        }


        static bool GcdCheck(int n, GmpInt p, GmpInt q, FactorizationInt result)
        {
            bool ret;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd < 2 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = (gcd > 1 && gcd != n))
            {
                result.Add(FactorizationInt.FactorTrialDivide((int)gcd));
                result.Add(FactorizationInt.FactorTrialDivide((int)(n / gcd)));

            }
            return ret;
        }

    }
}