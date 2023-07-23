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
using HigginsSoft.Math.Lib;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static BenchmarkDotNet.Engines.EngineEventSource;
using static HigginsSoft.Math.Demos.Qs16;
using static HigginsSoft.Math.Lib.MathLib;

namespace HigginsSoft.Math.Demos
{
    public partial class Qs16
    {

        public bool GcdCheck(int n, int p, int q, Factorization result)
        {
            bool ret = false;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd == 1 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = gcd > 1 && gcd != n)
            {
                result = Naive((short)gcd);
                result.Add(Naive((short)(n / gcd)));

            }
            return ret;
        }

        public bool GcdCheck(int n, int p, int q, FactorizationInt result)
        {
            bool ret = false;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd < 2 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = (gcd > 1 && gcd != n))
            {
                result.Add(FactorizationInt.FactorTrialDivide(gcd));
                result.Add(FactorizationInt.FactorTrialDivide(n / gcd));

            }
            return ret;
        }

        public bool GcdCheck(int n, GmpInt p, GmpInt q, FactorizationInt result)
        {
            bool ret = false;
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

        #region prime consts
        const int _primes_1 = 2; const int _primes_square_1 = _primes_1 * _primes_1;
        const int _primes_2 = 3; const int _primes_square_2 = _primes_2 * _primes_2;
        const int _primes_3 = 5; const int _primes_square_3 = _primes_3 * _primes_3;
        const int _primes_4 = 7; const int _primes_square_4 = _primes_4 * _primes_4;
        const int _primes_5 = 11; const int _primes_square_5 = _primes_5 * _primes_5;
        const int _primes_6 = 13; const int _primes_square_6 = _primes_6 * _primes_6;
        const int _primes_7 = 17; const int _primes_square_7 = _primes_7 * _primes_7;
        const int _primes_8 = 19; const int _primes_square_8 = _primes_8 * _primes_8;
        const int _primes_9 = 23; const int _primes_square_9 = _primes_9 * _primes_9;
        const int _primes_10 = 29; const int _primes_square_10 = _primes_10 * _primes_10;
        const int _primes_11 = 31; const int _primes_square_11 = _primes_11 * _primes_11;
        const int _primes_12 = 37; const int _primes_square_12 = _primes_12 * _primes_12;
        const int _primes_13 = 41; const int _primes_square_13 = _primes_13 * _primes_13;
        const int _primes_14 = 43; const int _primes_square_14 = _primes_14 * _primes_14;
        const int _primes_15 = 47; const int _primes_square_15 = _primes_15 * _primes_15;
        const int _primes_16 = 53; const int _primes_square_16 = _primes_16 * _primes_16;
        const int _primes_17 = 59; const int _primes_square_17 = _primes_17 * _primes_17;
        const int _primes_18 = 61; const int _primes_square_18 = _primes_18 * _primes_18;
        const int _primes_19 = 67; const int _primes_square_19 = _primes_19 * _primes_19;
        const int _primes_20 = 71; const int _primes_square_20 = _primes_20 * _primes_20;
        const int _primes_21 = 73; const int _primes_square_21 = _primes_21 * _primes_21;
        const int _primes_22 = 79; const int _primes_square_22 = _primes_22 * _primes_22;
        const int _primes_23 = 83; const int _primes_square_23 = _primes_23 * _primes_23;
        const int _primes_24 = 89; const int _primes_square_24 = _primes_24 * _primes_24;
        const int _primes_25 = 97; const int _primes_square_25 = _primes_25 * _primes_25;
        const int _primes_26 = 101; const int _primes_square_26 = _primes_26 * _primes_26;
        const int _primes_27 = 103; const int _primes_square_27 = _primes_27 * _primes_27;
        const int _primes_28 = 107; const int _primes_square_28 = _primes_28 * _primes_28;
        const int _primes_29 = 109; const int _primes_square_29 = _primes_29 * _primes_29;
        const int _primes_30 = 113; const int _primes_square_30 = _primes_30 * _primes_30;
        const int _primes_31 = 127; const int _primes_square_31 = _primes_31 * _primes_31;
        const int _primes_32 = 131; const int _primes_square_32 = _primes_32 * _primes_32;
        #endregion


        // if changing set _precheck_tdiv_limit 
        public const int factor_base_prime_count = 8;
        const int _precheck_tdiv_limit = _primes_square_8;
        const int _max_factor_base_prime = _primes_8;
        const int tdiv_prime_count = factor_base_prime_count >> 1;

        public const int t_div_pow_limit = 16;
        const int trial_div_limit = 1 << t_div_pow_limit;
        public const int min_extra_s = 2;
        private int[] MicroFactorBase = Primes.IntFactorPrimes.Take(factor_base_prime_count).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool tdiv(int value, FactorizationInt f, out int factor)
        {
            if (value < 2)
                throw new Exception($"Invalid : {value}");
            var primes = MicroFactorBase;
            factor = value;
            int count;
            for (var j = 0; factor > 1 && j < factor_base_prime_count; j++)
            {
                count = 0;
                var p = primes[j];
                if (factor == p)
                {
                    count = 1;
                    factor = 0;
                }
                else
                {
                    while (factor > 1 && factor % p == 0)
                    {
                        factor = factor / p;
                        count++;
                    }
                }

                if (count > 0)
                {
                    f.Add(p, count);
                }
            }
            return factor < 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool tdiv(int value, FactorizationInt f, out int factor, int[] primes)
        {
            if (value < 2)
                throw new Exception($"Invalid : {value}");
            //var primes = MicroFactorBase;
            factor = value;
            int count;
            for (var j = 0; factor > 1 && j < primes.Length; j++)
            {
                count = 0;
                var p = primes[j];
                if (factor == p)
                {
                    count = 1;
                    factor = 0;
                }
                else
                {
                    while (factor > 1 && factor % p == 0)
                    {
                        factor = factor / p;
                        count++;
                    }
                }

                if (count > 0)
                {
                    f.Add(p, count);
                }
            }
            return factor < 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool PreCheck(int n, out int root, FactorizationInt result)
        {
            bool ret = false;
            root = 1;
            if (n < 2) { }
            else if (n < _precheck_tdiv_limit)
            {
                bool res = tdiv(n, result, out int factor);
                if (!res)
                {
                    result.Add(factor, 1);
                }
            }
            else if (MathLib.IsPerfectSquare(n, out root))
            {
                if (MathLib.IsPrime(root))
                {
                    result.Add(root, 2);
                }
                else
                {
                    //var f = NaiveWithFactorBase((short)root);
                    var f = FactorizationInt.FactorTrialDivide(root);
                    result.Add(f);
                    result.Add(f);
                }
            }

            else { ret = true; }
            return !ret;
        }


        public class PopComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {

                if (x == y) return 0;
                var popx = MathLib.PopCount(x);
                var popy = MathLib.PopCount(y);
                if (popx > popy) return 1; // higer popcount is higher
                if (popx < popy) return -1;// lower popcount is lower

                // see how has hights bit set.
                var xor = x ^ y;
                var xand = x & xor;
                var yand = y & xor;
                return xand > yand ? -1 : (xand < yand) ? 1 : 0;

            }
        }
        public long opCount = 0;
        static List<int> primeFactors = new();
        //var primeFactors = new Dictionary<int, int>();
        //var d = new Dictionary<int, FactorizationInt>();
        static List<FactorizationInt> d = new();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FactorizationInt NaiveWithFactorBase(int value)
        {
            opCount++;
            FactorizationInt result = new();
            int n = value;
            var primes = MicroFactorBase;

            int factor = 0;

            if (!PreCheck(n, out int root, result))
            {
                //not sure if we want this or we should just clear the factorbase.
                // or add factors to the factor base.
                if (result.Factors.Count > 0)
                {
                    n /= result.GetProduct();
                    //result.Add(FactorizationInt.FactorTrialDivide(n));
                    //return result;
                }


                if (Primes.TrialDivide(n, out factor))
                {
                    result.Add(factor, 1);
                    return result;
                }

                var start = root + 1;
                var end = n - 1 - start;
                //sieving root to root+root fails for 28. passes up to 600
                //sieving root to root+2*root passes wtih 10+primes, fails for 3 with 9.

                // sieving one or two roots is probably way to much for larger primes.
                // but good option for small with small factor base.
                if (end > start + (root << 1)) end = start + (root << 1);

                // this can seive forever if not enough smooth or too small of factorbase.
                end = n - root - 1;

                var sieverange = end - start;
                // todo run check inline
                if (sieverange < 1)
                {
                    bool tdivresult = tdiv(n, result, out factor);
                    if (!tdivresult)
                        result.Add(factor, 1);
                }
                else
                {
                    //List<int> primeFactors = new();
                    //var primeFactors = new Dictionary<int, int>();
                    //var d = new Dictionary<int, FactorizationInt>();
                    //var d = new List<FactorizationInt>();
                    int factorBaseSolve = start + _max_factor_base_prime;
                    //once we solved the factorbase dynamic use them for trial division
                    //var tdivprimes = new int[] { };
                    primeFactors.Clear();
                    d.Clear();
                    int sq;
                    int res;
                    for (var i = start; i <= end; i++)
                    {
                        sq = i * i;
                        res = sq % n; // with larger sieve ranges
                        if (res == 0)
                        {
                            result = FactorizationInt.FactorTrialDivide(i);
                            result.Add(FactorizationInt.FactorTrialDivide(n / i));

                            break;
                        }
                        else if (res == 1)
                        {
                            if (GcdCheck(n, i + 1, i - 1, result))
                            {
                                break;
                            }
                        }
                        else if (res > trial_div_limit)
                        {
                            //  original bug - recurisive siqs i
                            // fix: recusrice siq res makes runtime insane.
                            //f = NaiveWithFactorBase(res);

                            // continue cuts runtime by 66%
                            continue;
                        }

                        //optimzation: don't create new class in each loop.
                        FactorizationInt f = new();
                        if (!tdiv(res, f, out factor))
                            continue;


                        if (f.IsPerfectSquare())
                        {
                            root = (int)f.GetProduct();
                            root = (int)MathLib.Sqrt(root);
                            if (GcdCheck(n, i + root, i - root, result))
                            {
                                break;
                            }
                        }

                        //this is no longer needed never hit due to tdiv smooth check
                        //if (f.Factors.Any(x => x.P > 131))
                        //    continue;
                        if (!checkFactorBase || i < factorBaseSolve)
                            f.Factors.ForEach(x =>
                            {
                                //primeFactors[x.P] = 1;
                                if (!primeFactors.Contains(x.P))
                                    primeFactors.Add(x.P);
                            });



                        //d[i] = f;
                        f.Offset = i;
                        d.Add(f);

                        if (d.Count > primeFactors.Count + min_extra_s)
                        {
                            if (i < factorBaseSolve && checkFactorBase)
                            {
                                f.Factors.ForEach(x =>
                                {
                                    //primeFactors[x.P] = 1;
                                    if (!primeFactors.Contains(x.P))
                                        primeFactors.Add(x.P);
                                });
                                if (d.Count <= primeFactors.Count + min_extra_s)
                                    continue;
                            }
                            if (n == 51103)
                            {
                                string bp = "";
                            }
                            if (TrySolve(n, d, primeFactors, result))
                            {
                                break;
                            }
                        }
                    }

                    // with prime checking n and a sufficient sieve size this call should be reduant.
                    //  but leaving it in the event of bad parameters.
                    // qs failed, likely due to to small a number or inability to solve the congruences
                    if (result.Factors.Count == 0)
                    {
                        if (n == 51103)
                        {
                            string bp = "";
                        }
                        //if (d.Count < primeFactors.Count || !TrySolve(n, d, primeFactors, result))
                        //{
                        //    result.Add(n, 1);
                        //}
                        //bool ret = tdiv(n, result);
                        //result = FactorizationInt.FactorTrialDivide(n);

                    }
                }
            }
            return result;
        }

        static bool IsQuadRes(int p, int n)
        {
            return MathLib.Quadratic.IsQuadratic(p, n);
        }
        const bool checkFactorBase = true;
        static PopComparer comp = new PopComparer();
        private bool TrySolve(int n, List<FactorizationInt> d, List<int> factorBase, FactorizationInt result)
        {
            var solved = false;
            // assure the factorbase has all primes.

            if (checkFactorBase)
            {
                foreach (var kvp in d)
                    foreach (var x in kvp.Factors)
                        if (!factorBase.Contains(x.P))
                            factorBase.Add(x.P);
            }

#if LOG
            var factLog = Path.GetFullPath($@"facts\facts-qr-{n}.log");
            var factLogSolved = Path.GetFullPath(@$"facts\facts-qr-{n}-solved.log");
            var factLogSolvedDirect = Path.GetFullPath(@$"facts\facts-qr-{n}-solved-direct.log");

            bool checkQauds = bool.Parse(bool.TrueString);
            if (checkQauds)
            {
                File.Delete(factLog);
                using var sw = new StreamWriter(factLog, false);
                sw.WriteLine($"n={n}");
                foreach (var kvp in d)
                {
                    var fs = kvp.FactorizationString();
                    sw.WriteLine($"i_{kvp.Offset}_sq = {(long)kvp.Offset * kvp.Offset}");
                    sw.WriteLine($"i_{kvp.Offset}_res={kvp.GetProduct()}");
                    sw.WriteLine($"i_{kvp.Offset}_res_fact={kvp.EquationString()}");
                }
                sw.Close();
                List<int> quadBase = new();
                List<int> nonQuadBase = new();
                var tests = Primes.IntFactorPrimes.Take(30).ToDictionary(x => x,
                    q => MathLib.PowerMod(n, (q - 1) / 2, q));
                var tests2 = Primes.IntFactorPrimes.Take(30).ToDictionary(x => x,
                   q => MathLib.PowerMod(15347, (q - 1) / 2, q));
                // test[p]==0 divides n;
                foreach (var p in factorBase)
                {
                    var isres = MathLib.Quadratic.IsQuadratic(p, n);
                    var a1 = MathLib.PowerMod(n, (p - 1) / 2, n);


                    if (!isres)
                    {

                        nonQuadBase.Add(p);
                    }
                    else
                    {
                        quadBase.Add(p);
                    }
                }
                // This is the naive algorithm without a precomputed factorbase.
                if (nonQuadBase.Count > 0)
                {
                    string bp = $"Factorizations count {nonQuadBase.Count} primes that are not quard s of {n}";
                }
            }
#endif
            // todo:
            if (factorBase.Count > 32)
            {
                throw new Exception("Naive QS can only handle factor base of 31 primes.");
            }


            // dictionary of indexes to avoid looking up each primes index in the factor base.
            var idxDict = Enumerable.Range(0, factorBase.Count)
                .ToDictionary(i => factorBase[i], i => i);

            var factorBaseLength = factorBase.Count;

            //covert each factor into a bit mask gf(2)
            var masks = d.ToDictionary(x => x.Offset,
                x =>
                {
                    var mask = 0;
                    x.Factors.ForEach(p =>
                    {
                        var gf2 = p.Power & 1;
                        var bitIndex = idxDict[(int)p.P];
                        var factorMask = gf2 << bitIndex;
                        mask |= factorMask;

                    });
                    return mask;
                }
            );
#if LOG
            void PrintMasks(IEnumerable<KeyValuePair<int, int>> masks)
            {

                var arr = masks.ToList();
                var sb = new StringBuilder();
                sb.AppendLine($"n= {n}\n");

                var keys = masks.Select(x => x.Key).ToList();
                var keyPad = keys.Max().ToString().Length;
                for (var i = 0; i < arr.Count; i++)
                {
                    var key = arr[i].Key;
                    var value = arr[i].Value;
                    var bitString = Convert.ToString(value, 2).PadLeft(factorBaseLength, '0');
                    var columns = string.Join("\t", bitString.ToCharArray());
                    sb.AppendLine($"{key.ToString().PadLeft(keyPad, '0')}\t{bitString}\t{columns}\t{value}");
                }
                var maskString = sb.ToString();
                Console.WriteLine(maskString);
            }

            bool doLog = bool.Parse(bool.FalseString);
            if (doLog)
            {
                Console.WriteLine("-----------------");
                Console.WriteLine("unsorted matrix");
                Console.WriteLine("-----------------");
                PrintMasks(masks);

                var sortedMasks = masks.OrderByDescending(x => x.Value);

                Console.WriteLine("-----------------");
                Console.WriteLine("sorted matrix");
                Console.WriteLine("-----------------");
                PrintMasks(sortedMasks);


                Console.WriteLine("Checking exact pairs");
            }

            //step 1: remove relations without dependencies.
#endif


            var factors = masks.OrderByDescending(x => comp).ToList();

            for (var column = factorBaseLength - 1; column >= 0; column--)
            {
                var mask = 1 << column;
                for (var i = 0; i < factors.Count; i++)
                {
                    var fact = factors[i];
                    if ((fact.Value & mask) > 0)
                    {
                        bool remove = true;
                        for (var j = 0; remove && j < factors.Count; j++)
                        {
                            if (j == i) continue;
                            var otherFact = factors[j];
                            if ((otherFact.Value & mask) > 0)
                            {
                                remove = false;
                            }
                        }
                        if (remove)
                        {
                            //no linear depencies found, remove the factor
                            factors.RemoveAt(i);
                            i--;

                        }
                    }
                }
            }

            // can't solve unless we have at least 2 linear depencies
            // since we are checking for squares in main loop.
            if (factors.Count < 2)
            {
                return false;
            }


            bool checkPairs = bool.Parse(bool.TrueString);
            if (checkPairs)
            {
                var pairs = masks.ToLookup(x => x.Value);



                //try perfect squares - note the main algoritm is already checking for perfect squares.
                // so this is redundant, but leaving it here as a quick optimization in case.
                var perfectSquares = pairs[0];
                if (perfectSquares != null && perfectSquares.Count() > 0)
                {
                    var perfectCount = perfectSquares.Count();

                    var first = perfectSquares.First();
#if LOG
                    if (doLog)
                    {
                        Console.WriteLine($"Found {perfectCount} perfect solutions with mask {first} ");
                    }
#endif
                    foreach (var perfectSquare in perfectSquares)
                    {
                        var left = perfectSquare.Key;
                        var res = left * left % n;
                        if (MathLib.IsPerfectSquare(res, out int root))
                        {
                            var product = left * left;

                            if (GcdCheck(n, left + root, left - root, result))
                            {
                                return true;
                            }
                        }
                    }
                }


                // try pairs (relationships with same exact )
                foreach (var pair in pairs)
                {
                    var pairCount = pair.Count();
                    if (pairCount < 2) continue;

#if LOG
                    if (doLog)
                    {
                        Console.WriteLine($"Checking {pairCount} solutions with mask {pair.Key} ");
                    }
#endif

                    var candidates = pair.ToList();
                    for (var i = 0; i < candidates.Count - 1; i++)
                    {
                        var left = candidates[i];
                        //var leftFact = d[left.Key];
                        for (var j = i + 1; j < candidates.Count; j++)
                        {
                            var right = candidates[j];
                            //var rightFact = d[right.Key];

                            var res1 = ((GmpInt)left.Key * left.Key) % n;
                            var res2 = ((GmpInt)right.Key * right.Key) % n;
                            var res = res1 * res2;

                            if (MathLib.IsPerfectSquare(res, out GmpInt root))
                            {
                                var product = (GmpInt)left.Key * right.Key;
                                if (GcdCheck(n, product + root, product - root, result))
                                {
#if LOG
                                    File.Delete(factLogSolvedDirect);
                                    File.Move(factLog, factLogSolvedDirect);
                                    using var sw = new StreamWriter(factLogSolvedDirect, true);
                                    var pplus = product + root;
                                    var pminus = product - root;
                                    var gcdplus = MathUtil.Gcd(n, pplus);
                                    var gcdminus = MathUtil.Gcd(n, pminus);
                                    sw.WriteLine($"direct dependency={left.Key}*{left.Key}*{right.Key}*{right.Key}");
                                    sw.WriteLine($"product={product}");
                                    sw.WriteLine($"res={res}");
                                    sw.WriteLine($"resroot={root}");
                                    sw.WriteLine($"product_plus_res={pplus}");
                                    sw.WriteLine($"product_minus_res={pminus}");
                                    sw.WriteLine($"Gcd({n}, {pplus}) = {gcdplus}");
                                    sw.WriteLine($"Gcd({n}, {pminus}) = {gcdminus}");
                                    sw.Close();
#endif
                                    var factorLeft = d.First(x => x.Offset == candidates[i].Key);
                                    var factorRight = d.First(x => x.Offset == candidates[j].Key);
                                    var leftPrimes = factorLeft.Factors.Select(x => x.P);
                                    var rightPrimes = factorRight.Factors.Select(x => x.P);
                                    var solPrimes = leftPrimes.Concat(rightPrimes).Distinct().ToList();
                                    var qrDic = solPrimes.ToDictionary(x => x, x => IsQuadratic(n, x));
                                    if (qrDic.Any(x => x.Value == false))
                                    {
                                        var b = new System.Text.StringBuilder();
                                        b.AppendLine($"N={n}");
                                        MathLib.IsPerfectSquare(n, out int nSqrt);

                                        b.AppendLine($"NSqrt={nSqrt}");
                                        b.AppendLine();
                                        b.AppendLine($"Relations:");
                                        b.AppendLine($"    {candidates[i].Key}^2 % {n} = {factorLeft} ");
                                        b.AppendLine($"    {candidates[j].Key}^2 % {n} = {factorRight} ");
                                        b.AppendLine();
                                        b.AppendLine($"Solution: ");
                                        b.AppendLine($"    product: {candidates[i].Key} * {candidates[j].Key} = {product}");
                                        b.AppendLine($"    root: {factorLeft.GetProduct()} * {factorRight.GetProduct()} = {res} = {root}^2");
                                        b.AppendLine($"    gcd: gcd({n}, {product}-{root}, {product} + {root}) = {result.ToString()} ");

                                        b.AppendLine();
                                        b.AppendLine($"IsQuadratic: ");

                                        foreach (var r in qrDic.OrderBy(x=> x.Key))
                                        {
                                            b.AppendLine($"    {r.Key}: {r.Value}");

                                        }
                                        string bp = b.ToString();
                                        Console.WriteLine("Quadratic check failed.");
                                        Console.WriteLine(bp);
                                    }


                                    var solstring = "";
                                    return true;
                                }
                            }
                        }
                    }


                }
            }
            //solve the multiple depencies dependencies.

            bool matrixFallBack = bool.Parse(bool.FalseString);
            if (matrixFallBack)
            {


                // fall back to heavy matrix search
                var m = new GfMatrix32(factors);

                var solutions = m.GetSolutions(false);
                foreach (var solution in solutions)
                {
                    var solutionRows = solution.GetSourceRows();
                    var product = 1;
                    //todo check for overlow.
                    foreach (var row in solutionRows)
                    {
                        product *= row.Value;
                        if (product > n)
                            product %= n;
                        if (product < -1) // overflow
                            break;
                    }
                    if (product > 0)
                    {
                        var res = product % n;
                        if (GcdCheck(n, product + res, product - res, result))
                            return true;
                    }
                }
            }

            bool useInline = bool.Parse(bool.TrueString);
            if (useInline)
            {
#if LOG
                var chain = new List<int>();
                int steps = 0;
#endif
                for (int i = 0; solved == false && i < factors.Count; i++)
                {
                    var fact = factors[i];
#if LOG
                    steps++;
                    chain.Add(fact.Key);
                    //
                    var solutions = recurseLog(i + 1, fact.Value, () => fact.Key, () => (fact.Key * fact.Key) % n, chain);
                    int failCount = 0;
#else
                    var solutions = recurse(i + 1, fact.Value, () => fact.Key, () => (fact.Key * fact.Key) % n);
#endif

                    //orderby depth should theoretically be faster because it checks fewest dependencies first
                    // however it requires enumerating all solutions first.
                    // that then calls the product function.
                    foreach (var solution in solutions)
                    {
#if LOG
                        failCount++;
#endif
                        var product = solution.Item1();
                        var res = solution.Item2();
                        if (GcdCheck(n, product + res, product - res, result))
                        {
#if LOG
                            if (checkQauds)
                            {
                                File.Move(factLog, factLogSolved, true);
                                using var sw = new StreamWriter(factLogSolved, true);
                                var pplus = product + res;
                                var pminus = product - res;
                                var gcdplus = MathUtil.Gcd(n, pplus);
                                var gcdminus = MathUtil.Gcd(n, pminus);
                                sw.WriteLine($"product={product}");
                                sw.WriteLine($"res={res}");
                                sw.WriteLine($"product_plus_res={pplus}");
                                sw.WriteLine($"product_minus_res={pminus}");
                                sw.WriteLine($"Gcd({n}, {pplus}) = {gcdplus}");
                                sw.WriteLine($"Gcd({n}, {pminus}) = {gcdminus}");
                                var f = Factorization.FactorTrialDivide(res);
                                var factEquation = f.EquationString();

                                var sol = string.Join(" * ", solution.Item3);
                                sw.WriteLine($"sol = {sol}");
                                sw.WriteLine($"res = {res}");
                                sw.WriteLine($"resFact = {factEquation}");
                                sw.WriteLine($"Solved in Solution #{failCount} after {steps} steps using {chain.Count} of {factors.Count} factorizations.");

                                var factDict = d.ToDictionary(x => x.Offset, x => x);
                                foreach (var offset in solution.Item3)
                                {
                                    var offsetFact = factDict[offset];
                                    var offsetFactString = offsetFact.FactorizationString();
                                    sw.WriteLine($"{offset} = {offsetFactString}");
                                }

                                sw.Close();

                            }
#endif


                            solved = true;
                            break;
                        }
#if LOG
                        chain.Remove(fact.Key);
                        string bp = $"Attempt {failCount}";
#endif
                    }
#if LOG

                    chain.Remove(fact.Key);

#endif
                }

#if LOG
                IEnumerable<(Func<GmpInt>, Func<GmpInt>, List<int>)> recurseLog(
                    int start,
                    int mask,
                    Func<GmpInt> getProduct,
                    Func<GmpInt> getRes,
                    List<int> chain)
                {
                    steps++;
                    /* depth first search can be done in one loop*/
                    for (int i = start; i < factors.Count; i++)
                    {
                        int nextMask;
                        var f = factors[i];
                        chain.Add(f.Key);
                        if ((mask & f.Value) > 0)
                        {
                            if ((nextMask = (mask ^ f.Value)) == 0)
                                yield return (() => getProduct() * f.Key,
                                    () => getRes() * (f.Key * f.Key % n), chain);
                            else
                                foreach (
                                   var result in recurseLog(
                                       i + 1,
                                       nextMask,
                                       () => getProduct() * f.Key,
                                       () => getRes() * (f.Key * f.Key % n),
                                       chain
                                    )
                                )
                                    yield return result;
                        }

                        chain.Remove(f.Key);
                    }

                }

#else
                IEnumerable<(Func<GmpInt>, Func<GmpInt>)> recurse(int start, int mask, Func<GmpInt> getProduct, Func<GmpInt> getRes)
                {
                    /* depth first search can be done in one loop*/

                    for (int i = start; i < factors.Count; i++)
                    {
                        int nextMask;
                        var f = factors[i];
                        if ((mask & f.Value) > 0)
                        {
                            if ((nextMask = (mask ^ f.Value)) == 0)
                                yield return (() => getProduct() * f.Key,
                                    () => getRes() * (f.Key * f.Key % n));
                            else
                                foreach (
                                   var result in recurse(
                                       i + 1,
                                       nextMask,
                                       () => getProduct() * f.Key,
                                       () => getRes() * (f.Key * f.Key % n)
                                    )
                                )
                                    yield return result;
                        }
                    }
                }
#endif


            }
#if LOG
            if (doLog)
            {
                PrintMasks(factors);
            }
            File.Delete(factLog);
#endif
            return solved;

        }


        public Factorization Naive(short n)
        {
            Factorization result = new();
            if (n < 2) return result;
            if (n < 256)
            {
                bool isPrime = Primes.TrialDivide(n, out int factor);
                if (isPrime)
                {
                    result.Add(n, 1);
                }
                else
                {
                    result = Naive((short)factor);
                    result.Add(Naive((short)(n / factor)));
                }
            }
            else if (MathLib.IsPrime(n))
            {
                result.Add(n, 1);

            }
            else if (MathLib.IsPerfectSquare(n, out int root))
            {
                if (MathLib.IsPrime(root))
                {
                    result.Add(root, 2);
                }
                else
                {
                    result = Naive((short)root);
                    result.Add(result);
                }
            }
            else
            {
                var start = root + 1;
                var end = n - 1 - start;
                var sieverange = end - start;
                if (sieverange < 0)
                {
                    result = Factorization.FactorTrialDivide(n);
                }
                else
                {
                    //var gen = new PrimeGeneratorUnsafe(root);

                    var d = new Dictionary<int, Factorization>();
                    var primeFactors = new List<int>();
                    for (var i = start; i <= end; i++)
                    {
                        var sq = i * i;
                        var res = sq % n;
                        if (res == 0)
                        {
                            result = Naive((short)i);
                            result.Add(Naive((short)(n / i)));

                            break;
                        }
                        if (res == 1)
                        {
                            if (GcdCheck(n, i + 1, i - 1, result))
                            {
                                break;
                            }
                        }
                        Factorization f = (res > 256) ?
                            Naive((short)res) : Factorization.FactorTrialDivide(res);

                        if (f.IsPerfectSquare())
                        {
                            root = (int)f.GetProduct();
                            if (GcdCheck(n, i + root, i - root, result))
                            {
                                break;
                            }
                        }


                        d[i] = f;
                        var factors = f.Factors.Select(x => (int)x.P).Distinct();
                        primeFactors.AddRange(factors.Except(primeFactors));

                        if (d.Count > primeFactors.Count + 10)
                        {
                            if (TrySolveFact(n, d, result))
                            {
                                break;
                            }
                        }
                    }

                    // qs failed, likely due to to small a number or inability to solve the congruences
                    if (result.Factors.Count == 0)
                    {
                        result = Factorization.FactorTrialDivide(n);
                    }
                }
            }
            return result;
        }

        private bool TrySolveFact(int n, Dictionary<int, Factorization> d, Factorization result)
        {
            var solved = false;

            // This is the naive algorithm without a precomputed factorbase.
            //  Build it from the unique list of primes among all factors.
            var factorBase = d.SelectMany(x => x.Value.Factors.Select(x => (int)x.P).Distinct())
                .Distinct().ToList();

            factorBase.Sort();

            // todo:
            if (factorBase.Count > 31)
            {
                throw new Exception("Naive QS can only handle factor base of 31 primes.");
            }


            // dictionary of indexes to avoid looking up each primes index in the factor base.
            var idxDict = Enumerable.Range(0, factorBase.Count)
                .ToDictionary(i => factorBase[i], i => i);

            var factorBaseLength = factorBase.Count;

            //covert each factor into a bit mask gf(2)
            var masks = d.ToDictionary(x => x.Key,
                x =>
                {
                    var mask = 0;
                    x.Value.Factors.ForEach(p =>
                    {
                        var gf2 = p.Power & 1;
                        var bitIndex = idxDict[(int)p.P];
                        var factorMask = gf2 << bitIndex;
                        mask |= factorMask;

                    });
                    return mask;
                }
            );

            void PrintMasks(IEnumerable<KeyValuePair<int, int>> masks)
            {

                var arr = masks.ToList();
                var sb = new StringBuilder();
                var keys = masks.Select(x => x.Key).ToList();
                var keyPad = keys.Max().ToString().Length;
                for (var i = 0; i < arr.Count; i++)
                {
                    var key = arr[i].Key;
                    var value = arr[i].Value;
                    var bitString = Convert.ToString(value, 2).PadLeft(factorBaseLength, '0');
                    sb.AppendLine($"{key.ToString().PadLeft(keyPad, '0')} {bitString} {value}");
                }
                var maskString = sb.ToString();
                Console.WriteLine(maskString);
            }

            bool doLog = bool.Parse(bool.FalseString);
            if (doLog)
            {
                Console.WriteLine("-----------------");
                Console.WriteLine("unsorted matrix");
                Console.WriteLine("-----------------");
                PrintMasks(masks);

                var sortedMasks = masks.OrderByDescending(x => x.Value);

                Console.WriteLine("-----------------");
                Console.WriteLine("sorted matrix");
                Console.WriteLine("-----------------");
                PrintMasks(sortedMasks);


                Console.WriteLine("Checking exact pairs");
            }




            var pairs = masks.ToLookup(x => x.Value);

            var perfectSquares = pairs[0];
            if (perfectSquares != null && perfectSquares.Count() > 0)
            {
                var perfectCount = perfectSquares.Count();

                var first = perfectSquares.First();
                if (doLog)
                {
                    Console.WriteLine($"Found {perfectCount} perfect solutions with mask {first} ");
                }
                foreach (var perfectSquare in perfectSquares)
                {
                    var left = perfectSquare.Key;
                    var res = left * left % n;
                    if (MathLib.IsPerfectSquare(res, out int root))
                    {
                        var product = left * left;

                        if (GcdCheck(n, left + root, left - root, result))
                        {
                            break;
                        }
                    }
                }
            }



            foreach (var pair in pairs)
            {
                var pairCount = pair.Count();
                if (pairCount < 2) continue;
                if (doLog)
                {
                    Console.WriteLine($"Checking {pairCount} solutions with mask {pair.Key} ");
                }
                var candidates = pair.ToList();
                for (var i = 0; i < candidates.Count - 1; i++)
                {
                    var left = candidates[i];
                    for (var j = i + 1; j < candidates.Count; j++)
                    {
                        var right = candidates[j];
                        var product = left.Key * right.Key;

                        var res1 = (left.Key * left.Key) % n;
                        var res2 = (right.Key * right.Key) % n;
                        var res = res1 * res2;

                        if (MathLib.IsPerfectSquare(res, out int root))
                        {
                            if (GcdCheck(n, product + root, product - root, result))
                            {
                                return true;
                            }
                            //var gcd = MathUtil.Gcd(product + root, n);
                            //gcd = MathLib.Abs(gcd);
                            //if (gcd == 1 || gcd == n)
                            //{
                            //    gcd = MathUtil.Gcd(product - root, n);
                            //}
                            //gcd = MathLib.Abs(gcd);
                            //if (gcd != 1 && gcd != n)
                            //{
                            //    if (doLog)
                            //    {
                            //        Console.WriteLine($"Solved {n}= {product} +/- {root}");
                            //    }
                            //    var a = Naive((short)gcd);
                            //    result.Add(a);
                            //    var b = n / gcd;
                            //    var bFactors = Naive((short)b);
                            //    result.Add(bFactors);
                            //    return true;
                            //}
                        }
                    }
                }


            }

            //solve the dependencies.
            //naive idea: map=> factorBase[n] => DepencyCount, and work from min dependencies.
            //  either solving the solution, are eliminating the depency.

            // need to take the factorizations over mod 2.

            return solved;

        }


        private bool SolveWithMatrixClass(int n, Dictionary<int, FactorizationInt> d, List<int> factorBase, FactorizationInt result)
        {
            var idxDict = Enumerable.Range(0, factorBase.Count)
               .ToDictionary(i => factorBase[i], i => i);

            var factorBaseLength = factorBase.Count;

            //covert each factor into a bit mask gf(2)
            var masks = d.ToDictionary(x => x.Key,
                x =>
                {
                    var mask = 0;
                    x.Value.Factors.ForEach(p =>
                    {
                        var gf2 = p.Power & 1;
                        var bitIndex = idxDict[(int)p.P];
                        var factorMask = gf2 << bitIndex;
                        mask |= factorMask;

                    });
                    return mask;
                }
            );

            bool ret = false;
            var m = new GfMatrix32(masks);

            var solutions = m.GetSolutions();
            foreach (var solution in solutions)
            {
                var solutionRows = solution.GetSourceRows();
                var product = 1;
                //todo check for overlow.
                foreach (var row in solutionRows)
                {
                    product *= row.Value;
                    if (product > n)
                        product %= n;
                    if (product < -1) // overflow
                        break;
                }
                if (product > 0)
                {
                    var res = product % n;
                    if (GcdCheck(n, product + res, product - res, result))
                        return true;
                }
            }

            return ret;
        }

    }
    public class GfMatrix32
    {
        public List<GfMatrix32Row> Rows;
        public int BitLength;

        static PopComparer cmp = new PopComparer();


        void init(IEnumerable<KeyValuePair<int, int>> masks, int? bitLength = null, bool reduced = false)
        {
            if (!masks.Any())
            {
                throw new ArgumentException("Masks can not be empty", nameof(masks));
            }
            if (bitLength == null)
            {
                var max = masks.Max(x => x.Value);
                bitLength = MathLib.BitLength(max);
            }
            var orderMasks = reduced ? masks : masks.OrderByDescending(x => x.Value);
            Rows = orderMasks.Select(x => new GfMatrix32Row(this, x.Key, x.Value)).ToList();
            BitLength = bitLength.Value;
        }

        private GfMatrix32(IEnumerable<KeyValuePair<int, int>> masks, int bitLength)
        {
            init(masks, bitLength);
        }
        public GfMatrix32(IEnumerable<KeyValuePair<int, int>> masks)
        {
            init(masks);
        }
        private GfMatrix32(IEnumerable<KeyValuePair<int, int>> masks, int bitLength, bool reduced)
        {
            init(masks, bitLength, reduced);
        }

        public GfMatrix32(IEnumerable<KeyValuePair<int, int>> masks, bool reduced)
        {
            init(masks, reduced: reduced);
        }

        public GfMatrix32(List<GfMatrix32Row> rows, int bitLength)
        {
            Rows = rows;
            BitLength = bitLength;
        }
        public GfMatrix32(List<GfMatrix32Row> rows)
        {
            var max = rows.Max(x => x.Value);
            BitLength = MathLib.BitLength(max);
            Rows = rows;

        }


        /// <summary>
        /// Reduces the matrix by removing any rows that don't have a linear dependency.
        /// </summary>
        /// <returns></returns>
        public GfMatrix32 Reduce()
        {
            var masks = Rows.OrderByDescending(x => x.Value).ToList();

            int max = 0;
            for (var column = BitLength - 1; column >= 0; column--)
            {
                var mask = 1 << column;
                for (var i = 0; i < masks.Count; i++)
                {
                    var fact = masks[i];
                    if (fact.Mask > max) max = fact.Mask;
                    if ((fact.Mask & mask) > 0)
                    {

                        bool remove = true;
                        for (var j = 0; remove && j < masks.Count; j++)
                        {
                            if (j == i) continue;
                            var otherFact = masks[j];
                            if ((otherFact.Mask & mask) > 0)
                            {
                                remove = false;
                            }
                        }
                        if (remove)
                        {
                            //no linear depencies found, remove the factor
                            max = 0;
                            masks.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }



            var result = new GfMatrix32(masks);
            return result;
        }

        public IEnumerable<GfMatrix32SolutionRow> GetSolutions(bool reduce = true)
        {
            if (reduce)
            {
                foreach (var row in Reduce().GetSolutions(false))
                    yield return row;
            }
            else
                foreach (var row in GenerateSolutions())
                    yield return row;
        }
        IEnumerable<GfMatrix32SolutionRow> GenerateSolutions
         ()
        {
            var rows = this.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                var left = rows[i];

                if (left.Mask == 0)
                {
                    if (left is GfMatrix32SolutionRow sol)
                        yield return sol;
                    else
                        yield return new GfMatrix32SolutionRow(this, left, null);
                }

                else
                {
                    foreach (var solution in GenerateSolutionsRecursive(left, i + 1))
                        yield return solution;
                }
            }
        }

        IEnumerable<GfMatrix32SolutionRow> GenerateSolutionsRows
          ()
        {
            var rows = this.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                var left = rows[i];

                if (left.Mask == 0)
                {
                    if (left is GfMatrix32SolutionRow sol)
                        yield return sol;
                    else
                        yield return new GfMatrix32SolutionRow(this, left, null);
                }

                else
                {
                    foreach (var solution in GenerateSolutionsRecursive(left, i + 1))
                        yield return solution;
                }
            }
        }

        IEnumerable<GfMatrix32SolutionRow> GenerateSolutionsRecursive
          (GfMatrix32Row left, int startIndex)
        {

            var rows = this.Rows;
            var mask = left.Mask;
            for (int i = startIndex; i < rows.Count; i++)
            {
                var right = rows[i];

                if ((mask & right.Mask) > 0)
                //if (left.IsDependant(right))
                {

                    var sol = new GfMatrix32SolutionRow(this, left, right);
                    if (sol.Mask == 0)
                    {
                        yield return sol;
                    }
                    // TODO: May need to call recursive function even if was solution.
                    else
                    {

                        var results = GenerateSolutionsRecursive(sol, i + 1);
                        foreach (var result in results)
                            yield return result;

                    }
                }

            }
        }



#if MATRIX_USE_LISTS
        IEnumerable<GfMatrix32SolutionRow> GenerateSolutions
            (List<GfMatrix32Row> rows)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                var left = rows[i];

                if (left.Mask == 0)
                {
                    if (left is GfMatrix32SolutionRow sol)
                        yield return sol;
                    else
                        yield return new GfMatrix32SolutionRow(this, left, null);
                }

                else
                {

                    var currentSolution = new List<GfMatrix32Row> { rows[i] };
                    foreach (var solution in GenerateSolutionsRecursive(left, rows, i + 1, currentSolution))
                        yield return solution;
                }
            }
        }


        IEnumerable<GfMatrix32SolutionRow> GenerateSolutionsRecursive
            (GfMatrix32Row left,
            List<GfMatrix32Row> rows, int startIndex, List<GfMatrix32Row> currentSolution)
        {


            for (int i = startIndex; i < rows.Count; i++)
            {
                var right = rows[i];
                if (left.IsDependant(right))
                {

                    var sol = new GfMatrix32SolutionRow(this, left, right);
                    if (sol.Mask == 0)
                    {
                        yield return sol;
                    }
                    // TODO: May need to call recursive function even if was solution.
                    else
                    {
                        currentSolution.Add(right);
                        var results = GenerateSolutionsRecursive(sol, rows, i + 1, currentSolution);
                        foreach (var result in results)
                            yield return result;
                        currentSolution.RemoveAt(currentSolution.Count - 1);
                    }
                }
                //currentSolution.Add(rows[i]);
                //GenerateSolutionsRecursive(rows, i + 1, currentSolution);
                //currentSolution.RemoveAt(currentSolution.Count - 1);
            }
        }

#endif


    }

    public class GfMatrix32Row
    {
        public int Mask;
        public int Value;
        public GfMatrix32 Matrix;


        public GfMatrix32Row(GfMatrix32 gfMatrix32, int value, int mask)
        {
            this.Matrix = gfMatrix32;
            this.Mask = mask;
            this.Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsDependant(GfMatrix32Row otherRow)
        {
            var mask = this.Mask;
            var value = mask & otherRow.Mask;
            return value > 0;
        }
    }


    public class GfMatrix32SolutionRow
        : GfMatrix32Row
    {

        public GfMatrix32Row Left;
        public GfMatrix32Row Right;

        public GfMatrix32SolutionRow(GfMatrix32 gfMatrix32,
           GfMatrix32Row left,
            GfMatrix32Row right) : base(gfMatrix32, -1, left.Mask ^ right.Mask)
        {
            Left = left;
            Right = right;
        }

        public IEnumerable<GfMatrix32Row> GetSourceRows()
        {
            var result = new List<GfMatrix32Row>();
            if (Left is GfMatrix32SolutionRow left)
            {
                foreach (var row in left.GetSourceRows())
                    yield return row;
            }
            else
            {
                yield return Left;
            }
            if (Right is not null)
            {
                if (Right is GfMatrix32SolutionRow right)
                {
                    foreach (var row in right.GetSourceRows())
                        yield return row;
                }
                else
                {
                    yield return Right;
                }
            }
        }
    }
}