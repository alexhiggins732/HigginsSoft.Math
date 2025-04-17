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
using System.IO.Pipelines;
using System.Net;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HigginsSoft.Math.Lib
{

    public static partial class MathLib
    {


        public class TonelliShanksPy
        {
            public static bool IsQuadraticResidue(BigInteger n, BigInteger p)
            {
                //TODO Perform benchmarking of Legendre symbol vs ModPow
                return MathLib.LegendreSymbol(n, p) == 1;
                //return BigInteger.ModPow(n, (p - 1) / 2, p) == 1;
            }

            public static (BigInteger Root1, BigInteger Root2, BigInteger Offset1, BigInteger Offset2)
                    GetRootsFromKnownOffset(BigInteger p, BigInteger n, BigInteger knownOffset, BigInteger? start = null)
            {

                BigInteger startVal = start ?? n.Sqrt();
                // Recover first root
                var r1 = (startVal + knownOffset) % p;

                // Compute second root
                var r2 = (p - r1) % p;

                // Compute second offset
                var k2 = (startVal - r2 + (p - 1)) / p;
                var offset2 = r2 + p * k2 - startVal;

                var offset1 = knownOffset;

                //var ordered = new[] { offset1, offset2 }.OrderBy(x => x).ToArray();
                //var delta = BigInteger.Abs(ordered[1] - ordered[0]);
                if (offset1 < offset2)
                {
                    return (r1, r2, offset1, offset2);
                }
                else
                {
                    return (r1, r2, offset2, offset1);
                }

            }

            public static Dictionary<BigInteger, (BigInteger Offset1, BigInteger Offset2)> GetFactorBaseOffsets(BigInteger n, List<(BigInteger prime, BigInteger offset)> primes)
            {
                var start = n.Sqrt();
                var result = new Dictionary<BigInteger, (BigInteger Offset1, BigInteger Offset2)>(primes.Count);
                for (var i = 0; i < primes.Count; i++)
                {
                    var offsets = GetRootsFromKnownOffset(primes[i].prime, n, primes[i].offset, start);
                    result.Add(primes[i].prime, (offsets.Offset1, offsets.Offset2));
                }
                return result;

            }


            public static Dictionary<BigInteger, (BigInteger, BigInteger)> GetFactorBaseOffsets(BigInteger n, List<BigInteger> primes)
            {
                var start = n.Sqrt();
                var result = new Dictionary<BigInteger, (BigInteger, BigInteger)>(primes.Count);
                for (var i = 0; i < primes.Count; i++)
                {
                    var offsets = GetFactorBaseOffsets(n, primes[i], start, false);
                    result.Add(primes[i], offsets);
                }
                return result;

            }

            public static Dictionary<int, List<int>> GetFactorBaseOffsets(BigInteger n, List<int>? primes = null)
            {

                if (primes == null)
                {
                    primes = Primes.IntFactorPrimes.ToList();
                }

                // var factorBasePrimes = primes.Where(p => MathLib.LegendreSymbol(n, p) == 1).ToList();

                var factorBase = new Dictionary<int, List<int>>();

                var start = n.Sqrt();
                foreach (var factor in primes)
                {
                    if (MathLib.LegendreSymbol(n, factor) == 1)
                    {
                        var offsets = new List<int>();
                        var roots = new List<BigInteger>();
                        if (factor == 2)
                        {
                            roots.Add(n % factor);
                        }
                        else
                        {
                            // sqrt_N
                            var root = TonelliShanksAlgo(n, factor);
                            if (root == 0) continue;
                            roots.Add(root);
                            roots.Add(factor - root);
                        }
                        foreach (var r in roots)
                        {
                            var k = (start - r + (factor - 1)) / factor;
                            int x = (int)((r + factor * k) - start);
                            offsets.Add(x);
                        }
                        offsets.Sort();
                        factorBase.Add(factor, offsets);
                    }
                }

                return factorBase;
            }

            public static Dictionary<long, List<long>> GetFactorBaseOffsetsLong(BigInteger n, List<long>? primes = null)
            {

                if (primes == null)
                {
                    primes = Primes.IntFactorPrimes.Select(x => (long)x).ToList();
                }

                // var factorBasePrimes = primes.Where(p => MathLib.LegendreSymbol(n, p) == 1).ToList();

                var factorBase = new Dictionary<long, List<long>>();

                var start = n.Sqrt();
                foreach (var factor in primes)
                {
                    if (MathLib.LegendreSymbol(n, factor) == 1)
                    {
                        var roots = new List<BigInteger>();
                        if (factor == 2)
                        {
                            roots.Add(n % factor);
                        }
                        else
                        {
                            // sqrt_N
                            var root = TonelliShanksAlgo(n, factor);
                            if (root == 0) continue;
                            roots.Add(root);
                            roots.Add(factor - root);
                        }


                        var offsets = new List<long>();
                        foreach (var r in roots)
                        {
                            var k = (start - r + (factor - 1)) / factor;
                            int x = (int)((r + factor * k) - start);
                            offsets.Add(x);
                        }
                        offsets.Sort();
                        factorBase.Add(factor, offsets);
                    }
                }

                return factorBase;
            }

            /// <summary>
            /// Get the offsets for the solutions of sqrt(n) mod p
            /// </summary>
            /// <param name="n"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static (BigInteger, BigInteger) GetFactorBaseOffsets(BigInteger n, BigInteger p, BigInteger? startVal = null, bool quadResCheck = true)
            {
                if (quadResCheck)
                    if (MathLib.LegendreSymbol(n, p) != 1)
                        return (-1, -1);

                BigInteger start = startVal ?? n.Sqrt();


                //BigInteger x = 0;
                //BigInteger k = 0;



                //var roots = new List<BigInteger>();
                //var result = roots;

                var offsets = new List<BigInteger>();
                var roots = new List<BigInteger>();
                if (p == 2)
                {
                    roots.Add(n % p);
                }
                else
                {
                    // sqrt_N
                    var root = TonelliShanksAlgo(n, p);
                    if (root == -1)
                        return (0, 0);
                    //if (root == 0) continue;
                    roots.Add(root);
                    roots.Add(p - root);
                }
                foreach (var r in roots)
                {
                    var k = (start - r + (p - 1)) / p;
                    var x = ((r + p * k) - start);
                    offsets.Add(x);
                }
                offsets.Sort();

                //k = (start - root0 + (p - 1)) / p;
                //x = ((root0 + p * k) - start);
                //root0 = x;

                //if (p != 2)
                //{
                //    root1 = p - root0;
                //    k = (start - root1 + (p - 1)) / p;
                //    x = ((root1 + p * k) - start);
                //    root1 = x;
                //    if (root0 < root1)
                //        return (root0, root1);
                //    else
                //    {
                //        return (root1, root0);
                //    }
                //}
                //else
                //{
                //    return (root0, root0);
                //}


                if (offsets.Count == 1)
                {
                    return (offsets[0], offsets[0]);
                }
                else
                {
                    offsets.Sort();
                    return (offsets[0], offsets[1]);
                }
            }


            /// <summary>
            /// Get the offsets for the solutions of sqrt(n) mod p
            /// </summary>
            /// <param name="n"></param>
            /// <param name="p"></param>
            /// <param name="offset">Offset of the sieve</param>
            /// <param name="shanksRoot">Root of the sieve</param>
            /// <param name="quadResCheck">If true, check if n is a quadratic residue mod p</param>
            /// <returns></returns>
            public static (BigInteger, BigInteger) GetFactorBaseOffsets(BigInteger n, BigInteger p, BigInteger startVal, BigInteger shanksRoot, bool quadResCheck = true)
            {
                if (quadResCheck)
                    if (MathLib.LegendreSymbol(n, p) != 1)
                        return (-1, -1);

                BigInteger start = startVal;

                var offsets = new List<BigInteger>();
                var roots = new List<BigInteger>();
                if (p == 2)
                {
                    roots.Add(n % p);
                }
                else
                {
                    // sqrt_N
                    var root = shanksRoot;
                    //if (root == 0) continue;
                    roots.Add(root);
                    roots.Add(p - root);
                }
                foreach (var r in roots)
                {
                    var k = (start - r + (p - 1)) / p;
                    var x = ((r + p * k) - start);
                    offsets.Add(x);
                }
                offsets.Sort();

                if (offsets.Count == 1)
                {
                    return (offsets[0], offsets[0]);
                }
                else
                {
                    offsets.Sort();
                    return (offsets[0], offsets[1]);
                }
            }


            public static List<BigInteger> GetSmooth(BigInteger n, int sieving_array_size, List<int> factorBase, int max_num)
            {
                //def gen_smooth(factor_base, max_num):
                //    ret = set({})
                //    startpoint = int(math.sqrt(N)) - sieving_array_size // 2
                //    endpoint = startpoint + sieving_array_size

                //    print(" = initializing an array for quadratic sieving")
                //    sieve = [x * x - N for x in range(startpoint, endpoint)]

                //    #print(sieve)

                List<BigInteger> ret = new();
                //var startpoint = n.Sqrt() - sieving_array_size / 2;
                // var endpoint = startpoint + sieving_array_size;
                var startpoint = n.Sqrt() + 1;
                var endpoint = startpoint + sieving_array_size;
                // Initialize sieve: (x + startPoint)^2 - n
                var sieve = new BigInteger[sieving_array_size];

                for (int i = 0; i < sieving_array_size; i++)
                {
                    var x = startpoint + i;
                    var xs = x * x;
                    sieve[i] = x * x - n;
                }

                Console.WriteLine("Initialized sieve of size " + startpoint);

                // TODO: Continue with marking positions divisible by factor base primes

                foreach (var factor in factorBase)
                {


                    //            if factor == 2:
                    //    # solving x*x=N % 2
                    //    if N % 2 == 0:
                    //            R_all = [0]
                    //    else:
                    //        R_all = [1]
                    //else:
                    //    # sqrt_N
                    //    R = tonelli_shanks_algo(N, factor)
                    //    assert R != 0
                    //    R_all = [R, factor - R]

                    var R_all = new List<BigInteger>();
                    if (factor == 2)
                    {
                        // solving x*x=N % 2
                        if (n % 2 == 0)
                        {
                            R_all.Add(0);
                        }
                        else
                        {
                            R_all.Add(1);
                        }
                    }
                    else
                    {
                        // sqrt_N
                        var root = TonelliShanksAlgo(n, factor);
                        if (root == 0) continue;
                        R_all.Add(root);
                        R_all.Add(factor - root);
                    }


                    // R + faktor * k > startpoint
                    // faktor * k > startpoint - R
                    // k > ( startpoint - R ) // factor
                    // R + faktor * k < endpoint
                    foreach (var r in R_all)
                    {
                        var k_from = (startpoint - r + (factor - 1)) / factor;
                        var k_to = (k_from + (endpoint - (r + factor * k_from) +
                                (factor - 1)) / factor);
                        for (var k = k_from; k < k_to; k++)
                        {
                            int x = (int)((r + factor * k) - startpoint);
                            //if sieve[x]==0:
                            //  continue
                            var val = x + startpoint;
                            if (sieve[x] % factor != 0) continue;
                            //print("s_before[x]=%d, factor=%d"%(sieve[x],factor))
                            sieve[x] /= factor;
                            while (sieve[x] % factor == 0)
                            {
                                sieve[x] /= factor;
                            }
                            if (sieve[x] == 1 && x != 0)
                            {
                                ret.Add(val);
                                var number = val * val - n;
                                if (!IsSmooth(number, factorBase)) continue;
                                Console.WriteLine(" + founded %d" + ret.Count);
                                sieve[x] = 0;
                                if (ret.Count > max_num)
                                    return ret.ToList();
                            }
                        }
                    }

                }

                return ret;
                //    for factor in factor_base:
                //        # tonelli shanks algo doesn't work with factor 2
                //        if factor == 2:
                //            # solving x*x=N % 2
                //            if N % 2 == 0:
                //                    R_all = [0]
                //            else:
                //                R_all = [1]
                //        else:
                //            # sqrt_N
                //            R = tonelli_shanks_algo(N, factor)
                //            assert R != 0
                //            R_all = [R, factor - R]

                //        print(" = factor %d, R_all=%s" % (factor, R_all))

                //        # R + faktor * k > startpoint
                //        # faktor * k > startpoint - R
                //        # k > ( startpoint - R ) // factor

                //        # R + faktor * k < endpoint

                //        for R in R_all:
                //            k_from = (startpoint - R + (factor - 1)) // factor
                //            k_to = (k_from + (endpoint - (R + factor * k_from) +
                //                    (factor - 1)) // factor)

                //            for k in range(k_from, k_to):
                //                x = (R + factor * k) - startpoint
                //                #if sieve[x]==0:
                //                #  continue
                //                val = x + startpoint

                //                assert sieve[x] % factor == 0

                //                #print("s_before[x]=%d, factor=%d"%(sieve[x],factor))
                //                sieve[x] //= factor
                //                while(sieve[x] % factor == 0):
                //                    sieve[x] //= factor

                //                if sieve[x] == 1 and x != 0:
                //                    ret.add(val)
                //                    number = val * val - N
                //                    assert is_smooth(number, factor_base)
                //                    print(" + founded %d" % (len(ret)))
                //                    sieve[x] = 0
                //                    if len(ret) > max_num:
                //                        return list(ret)
                //    return list(ret)
                // 
            }

            /*
             def is_smooth(n, factor_base):
     if n == 0:
         return False

     for factor in factor_base:
         while n % factor == 0:
             n = n / factor
     return n == 1

             */
            public static bool IsSmooth(BigInteger n, List<int> factorBase)
            {
                if (n == 0)
                    return false;
                foreach (var factor in factorBase)
                {
                    while (n % factor == 0)
                    {
                        n /= factor;
                    }
                }
                return n == 1;
            }


            public static BigInteger TonelliShanksAlgo(BigInteger n, BigInteger p)
            {

                //def tonelli_shanks_algo(n, p):
                //    """ algo for solving a congruence x * x =* n by module p """

                //    assert p % 2 == 1

                //    if n ** ((p - 1) // 2) % p != 1:
                //        return 0



                if (p == 2)
                {
                    return n % p;
                }

                BigInteger pm1 = p - 1;
                if (BigInteger.ModPow(n, pm1 / 2, p) != 1)
                {
                    return 0;
                }

                BigInteger k;
                BigInteger Q;
                //BigInteger calc = 0;
                if (p % 4 == 3)
                {
                    k = (p / 4);
                    Q = BigInteger.ModPow(n, k + 1, p) % p;
                    return Q;
                }
                else if (p % 8 == 5)
                {
                    k = (p / 8);
                    Q = BigInteger.ModPow(n, 2 * k + 1, p);
                    if (Q == 1)
                    {
                        Q = BigInteger.ModPow(n, k + 1, p);
                        return Q;
                    }
                    if (Q == p - 1)
                    {
                        Q = BigInteger.ModPow(4 * n, k + 1, p);
                        Q = (Q * (p + 1) / 2) % p;
                        return Q;
                    }
                }

                //    Q = p - 1
                //    S = 0
                //    while Q % 2 == 0:
                //        Q //= 2
                //        S += 1


                Q = pm1;
                var S = 0;
                while (Q % 2 == 0)
                {
                    Q /= 2;
                    S += 1;
                }

                //    #print(" = Q=%d S=%d" % (Q,S))

                //    # find z such as Legendre symbol (z/p) == -1
                //    for z in range(2, 100):
                //        euler_crit = z ** ((p - 1) // 2) % p
                //        if euler_crit == p - 1:
                //            break
                //    else:
                //        print(" - z not founded")
                //        return 0

                var z = 2;
                pm1 >>= 1; // pm1 / 2
                while (BigInteger.ModPow(z, pm1, p) != p - 1)
                {
                    z++;
                    if (z > 100)
                    {
                        //throw new ArithmeticException("Failed to find z");
                        return -1;
                    }
                }

                //    c = z ** Q % p
                //    #print(" = z=%d c=%d" % (z,c))

                var c = BigInteger.ModPow(z, Q, p);


                //    R = n ** ((Q + 1) // 2) % p
                //    t = n ** Q % p
                //    M = S


                var R = BigInteger.ModPow(n, (Q + 1) / 2, p);
                var t = BigInteger.ModPow(n, Q, p);
                //var M = S;


                //    while t != 1:
                //        for i in range(1, M):
                //            if t ** (2 ** i) % p == 1:
                //                break
                //        else:
                //            print(" - i not founded")
                //        #print( " = i=%d" % (i))
                //        b = c ** (2 ** (M - i - 1)) % p
                //        R = (R * b) % p
                //        t = (t * b * b) % p
                //        c = (b * b) % p
                //        M = i
                //    return R

                while (t != 1)
                {
                    int i = 1;
                    BigInteger temp = BigInteger.ModPow(t, BigInteger.Pow(2, i), p);
                    while (temp != 1)
                    {
                        temp = BigInteger.ModPow(temp, 2, p);
                        i++;
                        if (i == S)
                            //throw new ArithmeticException("Failed to converge");
                            return -1;
                    }
                    BigInteger b = BigInteger.ModPow(c, BigInteger.Pow(2, S - i - 1), p);
                    R = (R * b) % p;
                    t = (t * b * b) % p;
                    c = (b * b) % p;
                    S = i;
                }
                //if (R != calc)
                //{
                //    throw new ArithmeticException("invalid mod 4 calculation");
                //}
                return R;


            }


            /*
             def factorize(N):
    factor_base = get_factor_base(N, primes)
    print("factor base: %s" % factor_base)

    print(" = generating smooth array")
    U = gen_smooth(factor_base, len(factor_base) + 20)*/
            public static void Factorize(BigInteger n, List<int>? primes = null)
            {
                if (primes == null)
                {
                    primes = Primes.IntFactorPrimes.ToList();
                }
                var factorBase = primes.Where(p => MathLib.LegendreSymbol(n, p) == 1).ToList();

                Console.WriteLine("factor base: " + factorBase.Count);
                Console.WriteLine(" = generating smooth array");
                var U = GetSmooth(n, 100000, factorBase, (int)factorBase.Count + 20);
                Console.WriteLine(" = smooth array: " + U.Count);
            }
        }
        public static class HenselLifting
        {
            /// <summary>
            /// Lift a square root mod p to mod p^k using Hensel's Lemma after solving the quadratic residue equation x^2 ≡ n (mod p) with an offset.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="p"></param>
            /// <param name="k"></param>
            /// <param name="rootModP"></param>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            public static BigInteger LiftSqrt(BigInteger a, BigInteger p, int k, BigInteger rootModP)
            {
                if (k <= 1)
                    return rootModP;

                // Starting values
                BigInteger x = rootModP;
                BigInteger pk = p;

                for (int i = 2; i <= k; i++)
                {
                    pk *= p;

                    // Compute correction term: (a - x^2) / p^i-1 mod p
                    BigInteger r = (a - BigInteger.ModPow(x, 2, pk)) % pk;
                    if (r < 0) r += pk;

                    // Derivative: 2x mod p
                    BigInteger d = (2 * x) % p;
                    if (d == 0)
                        throw new InvalidOperationException("Hensel lifting failed: derivative is 0 mod p.");

                    // Solve for correction modulo p
                    BigInteger invD = ModInverse(d, p);
                    BigInteger e = (r / (pk / p)) * invD % p;

                    // Lift x
                    x = x + e * (pk / p);
                    x %= pk;
                }

                return x;
            }

            /// <summary>
            /// Modular inverse using Extended Euclidean Algorithm
            /// </summary>
            /// <param name="a"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            public static BigInteger ModInverse(BigInteger a, BigInteger m)
            {
                BigInteger m0 = m, x0 = 0, x1 = 1;
                if (m == 1) return 0;
                while (a > 1)
                {
                    BigInteger q = a / m;
                    (a, m) = (m, a % m);
                    (x0, x1) = (x1 - q * x0, x0);
                }

                return x1 < 0 ? x1 + m0 : x1;
            }
        }
        public class TonelliShanks2
        {
            /// <summary>
            /// Computes the modular exponentiation of a base raised to an exponent modulo a modulus.
            /// </summary>
            /// <param name="baseVal"></param>
            /// <param name="exp"></param>
            /// <param name="mod"></param>
            /// <returns></returns>
            public static BigInteger ModPow(BigInteger baseVal, BigInteger exp, BigInteger mod)
                => BigInteger.ModPow(baseVal, exp, mod);

            /// <summary>
            /// Computes the modular square root of a number modulo a prime using Tonelli-Shanks algorithm.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            /// <exception cref="ArithmeticException"></exception>
            public static BigInteger ModSqrt(BigInteger a, BigInteger p)
            {
                if (p == 2) return a % p;
                if (ModPow(a, (p - 1) / 2, p) != 1)
                {
                    return -1;
                    //throw new ArgumentException("No square root exists");
                }


                if (p % 4 == 3)
                    return ModPow(a, (p + 1) / 4, p);

                // Tonelli-Shanks algorithm
                BigInteger q = p - 1;
                int s = 0;
                while ((q & 1) == 0)
                {
                    q >>= 1;
                    s++;
                }

                BigInteger z = 2;
                while (ModPow(z, (p - 1) / 2, p) != p - 1)
                    z++;

                BigInteger c = ModPow(z, q, p);
                BigInteger x = ModPow(a, (q + 1) / 2, p);
                BigInteger t = ModPow(a, q, p);
                int m = s;

                while (t != 1)
                {
                    int i = 1;
                    BigInteger temp = ModPow(t, 2, p);
                    while (temp != 1)
                    {
                        temp = ModPow(temp, 2, p);
                        i++;
                        if (i == m) throw new ArithmeticException("Failed to converge");
                    }

                    BigInteger b = ModPow(c, BigInteger.One << (m - i - 1), p);
                    x = (x * b) % p;
                    t = (t * b * b) % p;
                    c = (b * b) % p;
                    m = i;
                }

                return x;
            }

            /// <summary>
            /// Solves the quadratic residue equation x^2 ≡ n (mod p) with an offset.
            /// </summary>
            /// <param name="n"></param>
            /// <param name="offset"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static (BigInteger a, BigInteger b) SolveResidueOffsets(BigInteger n, BigInteger offset, BigInteger p)
            {
                BigInteger r = (offset * offset) % n;
                BigInteger root = ModSqrt(r, p);
                if (root == -1)
                {
                    return (-1, -1);
                }
                BigInteger other = p - root;

                BigInteger a = (root - offset % p + p) % p;
                BigInteger b = (other - offset % p + p) % p;

                return (root, other);
            }
        }

        public class Quadratic
        {
            public static bool IsQuadratic(long q, long n)
            {
                return Legendre(n, (q - 1) / 2, q) == 1;
            }
            public static int Legendre(long a, long q, long n)
            {
                long x = q;
                long result = 1;

                a = a % n;

                if (x == 0)
                    return (int)result;

                while (x != 0)
                {
                    if (x % 2 == 0)
                    {
                        a = Power(a, 2) % n;
                        x /= 2;
                    }
                    else
                    {
                        x--;
                        result = (result * a) % n;
                    }
                }

                return (int)result;
            }
            public static int Legendre1(long a, long q, long l, long n)
            {
                long x = Power(q, l);
                long z = 1;

                a = a % n;

                if (x == 0)
                    return (int)z;

                while (x != 0)
                {
                    if (x % 2 == 0)
                    {
                        a = Power(a, 2) % n;
                        x /= 2;
                    }
                    else
                    {
                        x--;
                        z = (z * a) % n;
                    }
                }

                return (int)z;
            }
            public static long Power(long a, long b)
            {
                return (int)MathLib.Pow((double)a, (double)b);
            }
        }
        public class TonelliShanks
        {
            public class Solution1
            {
                //https://maxwellmlin.com/assets/pdf/sieve-2024.pdf
                public static BigInteger TonelliShanks(BigInteger n, BigInteger p)
                {
                    if (p == 2)
                    {
                        return -1;
                    }
                    if (BigInteger.ModPow(n, (p - 1) / 2, p) != 1)
                    {
                        return -1;
                    }

                    //find p-1 = 2^s * q
                    BigInteger q = p - 1;
                    BigInteger s = 0;
                    while ((q & 1) == 0)
                    {
                        s++;
                        q >>= 1;
                    }
                    //find z such that z^(p-1)/2 = -1
                    BigInteger z = 2;
                    while (BigInteger.ModPow(z, (p - 1) / 2, p) != p - 1)
                    {
                        z++;
                    }
                    //find c = z^q mod p
                    BigInteger c = BigInteger.ModPow(z, q, p);
                    //find r = n^(q+1)/2 mod p
                    BigInteger r = BigInteger.ModPow(n, (q + 1) / 2, p);
                    //find t = n^q mod p
                    BigInteger t = BigInteger.ModPow(n, q, p);
                    //find m = s
                    BigInteger m = s;
                    //while t != 1
                    while (t != 1)
                    {
                        if (t == 0)
                        {
                            return -1;
                        }
                        else if (t == 1)
                        {
                            return r;
                        }

                        BigInteger temp = t;
                        for (var i = 1; i < m; i++)
                        {
                            temp = BigInteger.ModPow(temp, 2, p);
                            if (temp == 1)
                            {
                                //find b = c^(2^(m-i-1)) mod p
                                BigInteger b = BigInteger.ModPow(c, BigInteger.Pow(2, (int)m - i - 1), p);
                                //update r = r * b mod p
                                r = (r * b) % p;
                                //update c = b^2 mod p
                                c = (b * b) % p;
                                //update t = t * c mod p
                                t = (t * c) % p;
                                //update m = i
                                m = i;
                            }

                        }

                    }
                    return r;

                }


            }
            public class Solution
            {
                private readonly BigInteger root1, root2;
                private readonly bool exists;

                public Solution(BigInteger root1, BigInteger root2, bool exists)
                {
                    this.root1 = root1;
                    this.root2 = root2;
                    this.exists = exists;
                }

                public BigInteger Root1()
                {
                    return root1;
                }

                public BigInteger Root2()
                {
                    return root2;
                }

                public bool Exists()
                {
                    return exists;
                }
            }


            /// <summary>
            /// Gets the square roots for the equation x2 ≡ n(mod p)
            ///     where n is an integer which is a quadratic (mod p), p
            /// </summary>
            /// <param name="n"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static Solution GetSolutions(BigInteger n, BigInteger p)
            {
                if (BigInteger.ModPow(n, (p - 1) / 2, p) != 1)
                {
                    return new Solution(0, 0, false);
                }

                BigInteger q = p - 1;
                BigInteger ss = 0;
                while ((q & 1) == 0)
                {
                    ss = ss + 1;
                    q = q >> 1;
                }

                if (ss == 1)
                {
                    BigInteger r1 = BigInteger.ModPow(n, (p + 1) / 4, p);
                    return new Solution(r1, p - r1, true);
                }

                BigInteger z = 2;
                while (BigInteger.ModPow(z, (p - 1) / 2, p) != p - 1)
                {
                    z = z + 1;
                }
                BigInteger c = BigInteger.ModPow(z, q, p);
                BigInteger r = BigInteger.ModPow(n, (q + 1) / 2, p);
                BigInteger t = BigInteger.ModPow(n, q, p);
                BigInteger m = ss;

                while (true)
                {
                    if (t == 1)
                    {
                        return new Solution(r, p - r, true);
                    }
                    BigInteger i = 0;
                    BigInteger zz = t;
                    while (zz != 1 && i < (m - 1))
                    {
                        zz = zz * zz % p;
                        i = i + 1;
                    }
                    BigInteger b = c;
                    BigInteger e = m - i - 1;
                    while (e > 0)
                    {
                        b = b * b % p;
                        e = e - 1;
                    }
                    r = r * b % p;
                    c = b * b % p;
                    t = t * c % p;
                    m = i;
                }
            }
            class Program
            {


                static void run()
                {
                    List<Tuple<long, long>> pairs = new List<Tuple<long, long>>() {
                new Tuple<long, long>(10, 13),
                new Tuple<long, long>(56, 101),
                new Tuple<long, long>(1030, 10009),
                new Tuple<long, long>(1032, 10009),
                new Tuple<long, long>(44402, 100049),
                new Tuple<long, long>(665820697, 1000000009),
                new Tuple<long, long>(881398088036, 1000000000039),
            };

                    foreach (var pair in pairs)
                    {
                        Solution sol = GetSolutions(pair.Item1, pair.Item2);
                        Console.WriteLine("n = {0}", pair.Item1);
                        Console.WriteLine("p = {0}", pair.Item2);
                        if (sol.Exists())
                        {
                            Console.WriteLine("root1 = {0}", sol.Root1());
                            Console.WriteLine("root2 = {0}", sol.Root2());
                        }
                        else
                        {
                            Console.WriteLine("No solution exists");
                        }
                        Console.WriteLine();
                    }

                    BigInteger bn = BigInteger.Parse("41660815127637347468140745042827704103445750172002");
                    BigInteger bp = BigInteger.Pow(10, 50) + 577;
                    Solution bsol = GetSolutions(bn, bp);
                    Console.WriteLine("n = {0}", bn);
                    Console.WriteLine("p = {0}", bp);
                    if (bsol.Exists())
                    {
                        Console.WriteLine("root1 = {0}", bsol.Root1());
                        Console.WriteLine("root2 = {0}", bsol.Root2());
                    }
                    else
                    {
                        Console.WriteLine("No solution exists");
                    }
                }
            }

        }
    }
}