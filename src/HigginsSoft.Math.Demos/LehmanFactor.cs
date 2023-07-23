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
using System.Runtime.CompilerServices;
using int64_t = System.Int64;
using uint64_t = System.UInt64;
using uint64 = System.UInt64;
using real = System.Double;
using System.Text;
using HigginsSoft.Math.Lib;
using System.Numerics;

namespace HigginsSoft.Math.Demos
{
    /****
    Code by Warren D. Smith, Dec 2011, to implement Lehman integer-factorization
    algorithm.  Rigorous O(N^(1/3)) step factoring
    (or prime-proving) algorithm invented by RS Lehman 1974.

    Ported from Yafu by bburrow@gmail.com which is public domain.
    *******/
    public class Lehman
    {

        public Lehman()
        {
            init_lehman();
        }
        static double[] sqr_tab = new double[1024];

        void make_sqr_tab()
        {
            int i;
            for (i = 0; i < 1024; i++)
                sqr_tab[i] = MathLib.Sqrt((double)i);

            return;
        }


        //the 6542 primes up to 65536=2^16, then sentinel 65535 at end
        static int[] prime = new int[6543];
        void MakePrimeTable()
        {
            int i, j, k;
            prime[0] = 2;
            prime[1] = 3;
            prime[2] = 5;
            k = 3;
            for (i = 7; i < 65536; i += 2)
            {
        
                for (j = 0; prime[j] * (uint)prime[j] <= i; j++)
                {
                    if (i % prime[j] == 0)
                        goto next;
                }
                prime[k] = i;
                k++;
            next:;
            }

            prime[k] = 65535; //sentinel

        }
        void init_lehman()
        {
            MakeIssq();
            MakePrimeTable();
            make_sqr_tab();

            return;
        }
        static int[] issq1024 = new int[1024];
        static int[] issq4199 = new int[4199];



        void MakeIssq()
        {
            int i;

            for (i = 0; i < 1024; i++) { issq1024[(i * i) % 1024] = 1; }

            for (i = 0; i < 3465; i++) { issq4199[(i * i) % 3465] |= 2; }
            for (i = 0; i < 4199; i++) { issq4199[(i * i) % 4199] |= 1; }

        }

        /**
         * Trial division factor algorithm replacing division by multiplications.
         *
         * Instead of dividing N by consecutive primes, we store the reciprocals of those primes, too,
         * and multiply N by those reciprocals. Only if such a result is near to an integer we need
         * to do a division.
         *
         * Assuming that we want to identify "near integers" with a precision of 2^-d.
         * Then the approach works for primes p if bitLength(p) >= bitLength(N) - 53 + d.
         *
         * @authors Thilo Harich + Tilman Neumann
         */

        /*

        Ported to C and released 7/31/19
        Ben Buhrow

                Ported to C# by Alexander Higgins

        */

        //this could explain error: 18 bits in double including sign.
        //x2 9223372036854775808 =	0100 0011 111 00000000000000000000000000000000000000000000000000000
        const int DISCRIMINATOR_BITS = 10; // experimental result
        static double DISCRIMINATOR => 1.0 / (1 << DISCRIMINATOR_BITS);
        const int FACTORLIMIT = (1 << 21) + 1000;
        const int MAXPRIMES = 155678;

        static int[] primes = new int[MAXPRIMES];
        static double[] reciprocals = new double[MAXPRIMES];


        void TDiv63InverseSetup(uint64_t[] PRIMES)
        {
            for (var i = 0; i < PRIMES.Length; i++)
            {
                primes[i] = (int)PRIMES[i];
                reciprocals[i] = 1.0 / primes[i];
            }
        }

        int tdiv_inverse(int64_t N, int pLimit)
        {
            int i = 0;
            int lbits = MathLib.LeadingZeroCount(N);
            int Nbits = 64 - lbits;

            int pMinBits = Nbits - 53 + DISCRIMINATOR_BITS;
            if (pMinBits > 0)
            {
                // for the smallest primes we must do standard trial division
                int pMin = 1 << pMinBits;
                //printf("standard trial division to limit %d on Nbits = %d, leading bits %d, top limit %d\n",
                //	pMin, Nbits, lbits, pLimit);

                for (; primes[i] < pMin; i++)
                {
                    if (N % primes[i] == 0)
                    {
                        return primes[i];
                    }
                }
            }

            // Now the primes are big enough to apply trial division by inverses
            for (; primes[i] <= pLimit; i++)
            {

                long nDivPrime = (int64_t)(N * reciprocals[i] + DISCRIMINATOR);
                //if (N == 346425669865991LL && primes[i] == 70163)
                //	printf("nDivPrime = %ld, test = %ld\n", nDivPrime, nDivPrime * primes[i]);

                if (nDivPrime * (int64_t)primes[i] == N)
                {
                    // nDivPrime is very near to an integer
                    if (N % primes[i] == 0)
                    {
                        //printf("Found factor %d\n", primes[i]);
                        return primes[i];
                    }
                }
            }

            // nothing found up to pLimit
            return 0;
        }

        const int SQRTBOUND = ((1 << 21) + 1);
        const double ROUND_UP_DOUBLE = 0.9999999665;
        static double[] sqRoot = new double[SQRTBOUND];
        static double[] sqrtInv = new double[SQRTBOUND];
        static bool initialized = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long gcd64(long a, long b) => MathUtil.Gcd(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong gcd64(ulong a, ulong b) => MathUtil.Gcd(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double sqrt(long test) => MathLib.Sqrt(test);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double sqrt(double test) => MathLib.Sqrt(test);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double pow(long n, double v) => MathLib.Pow(n, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double cbrt(double n) => MathLib.Cbrt(n);

        int64_t lehmanOdd(int kBegin, int kLimit, double sqrt4N, int64_t N, int64_t fourN)
        {
            int k;
            for (k = kBegin; k <= kLimit; k += 6)
            {
                int64_t a = (int64_t)(sqrt4N * sqRoot[k] + ROUND_UP_DOUBLE);
                // make a == (k+N) (mod 4)
                int64_t kPlusN = k + N;
                if ((kPlusN & 3) == 0)
                {
                    a += ((kPlusN - a) & 7);
                }
                else
                {
                    a += ((kPlusN - a) & 3);
                }
                int64_t test = a * a - k * fourN;

                int64_t b = (int64_t)sqrt(test);
                if (b * b == test)
                {
                    return gcd64(a + b, N);
                }

            }
            return -1;
        }

        int64_t lehmanEven(int kBegin, int kEnd, double sqrt4N, int64_t N, int64_t fourN)
        {
            int k;
            for (k = kBegin; k <= kEnd; k += 6)
            {
                // k even -> a must be odd
                int64_t a = (int64_t)(sqrt4N * sqRoot[k] + ROUND_UP_DOUBLE) | (int64_t)1;
                int64_t test = a * a - k * fourN;

                int64_t b = (int64_t)sqrt(test);
                if (b * b == test)
                {
                    return gcd64(a + b, N);
                }

            }
            return -1;
        }

        /**
         * Fast implementation of Lehman's factor algorithm.
         * Works flawlessly for N up to 60 bit.<br><br>
         *
         * It is quite surprising that the exact sqrt test of <code>test = a^2 - 4kN</code> works for N >= 45 bit.
         * At that size, both a^2 and 4kN start to overflow Long.MAX_VALUE.
         * But the error - comparing correct results vs. long results - is just the same for both a^2 and 4kN
         * (and a multiple of 2^64).
         *  Thus <code>test</code> is correct and <code>b</code> is correct, too. <code>a</code> is correct anyway.
         *
         * @authors Tilman Neumann + Thilo Harich
         */

        /*

        Ported to C and released 7/31/19
        Ben Buhrow

        Ported to C# 
        Alexander Higgins

        */

        public int64_t LehmanFactor(uint64_t uN, double Tune, bool DoTrialFirst, double CutFrac)
        {
            int i;
            int k;
            int j;
            int64_t N = (int64_t)uN;
            int64_t fourN;
            double sqrt4N;
            int64_t factor;
            double sixthRootTerm;

            if (!initialized)
            {

                uint64_t[] PRIMES;
                uint64_t NUM_P;

                // Precompute sqrts for all possible k. 2^21 entries are enough for N~2^63.
                int kMax = 1 << 21;
                for (i = 1; i < SQRTBOUND; i++)
                {
                    double sqrtI = sqrt((double)i);
                    sqRoot[i] = sqrtI;
                    sqrtInv[i] = 1.0 / sqrtI;
                }

                //PRIMES = soe_wrapper(sdata, 0, FACTORLIMIT, 0, &NUM_P, 0, 0);

                PRIMES = new PrimeGeneratorUnsafe(FACTORLIMIT).Select(x=> (ulong)x).ToArray();
                //PRIMES = Primes.UintFactorPrimes.Select(x=> (ulong)x).ToArray();
                NUM_P = (ulong) PRIMES.Length;
                TDiv63InverseSetup(PRIMES);

                //printf("prime list generated\n");
                //printf("PMAX = %lu, NUM_P = %lu\n", P_MAX, NUM_P);

                initialized = true;
                //printf("sqrt tables generated\n");
                //printf("check of sqrt(-1): %f, %d\n", sqrt(-1.), (int)sqrt(-1));

                MakeIssq();
                //printf("issqr tables generated\n");
            }

            int cb_rt = (int)cbrt(N);

            // do trial division before Lehman loop, up to the cube root
            if (DoTrialFirst)
            {
                //i = 0;
                //while (PRIMES[i] < cbrt)
                //{
                //	if ((N % PRIMES[i]) == 0)
                //		return PRIMES[i];
                //	i++;
                //}
                if ((factor = tdiv_inverse(N, cb_rt)) > 1) return factor;
            }


            fourN = N << 2;
            sqrt4N = sqrt(fourN);

            // kLimit must be 0 mod 6, since we also want to search above of it
            int kLimit = ((cb_rt + 6) / 6) * 6;
            // For kTwoA = kLimit / 64 the range for a is at most 2. We make it 0 mod 6, too.
            int kTwoA = (((cb_rt >> 6) + 6) / 6) * 6;

            // We are investigating solutions of a^2 - sqrt(k*n) = y^2 in three k-ranges:
            // * The "small range" is 1 <= k < kTwoA, where we may have more than two 'a'-solutions per k.
            //   Thus, an inner 'a'-loop is required.
            // * The "middle range" is kTwoA <= k < kLimit, where we have at most two possible 'a' values per k.
            // * The "high range" is kLimit <= k < 2*kLimit. This range is not required for the correctness
            //   of the algorithm, but investigating it for some k==0 (mod 6) improves performance.

            // We start with the middle range cases k == 0 (mod 6) and k == 3 (mod 6),
            // which have the highest chance to find a factor.
            if ((factor = lehmanEven(kTwoA, kLimit, sqrt4N, N, fourN)) > 1) return factor;
            if ((factor = lehmanOdd(kTwoA + 3, kLimit, sqrt4N, N, fourN)) > 1) return factor;

            // Now investigate the small range
            sixthRootTerm = 0.25 * pow(N, 1 / 6.0); // double precision is required for stability
            for (k = 1; k < kTwoA; k++)
            {
                int64_t a;
                int64_t fourkN = k * fourN;
                double sqrt4kN = sqrt4N * sqRoot[k];
                // only use long values
                int64_t aStart = (int64_t)(sqrt4kN + ROUND_UP_DOUBLE); // much faster than ceil() !
                int64_t aLimit = (int64_t)(sqrt4kN + sixthRootTerm * sqrtInv[k]);
                int64_t aStep;
                if ((k & 1) == 0)
                {
                    // k even -> make sure aLimit is odd
                    aLimit |= 1L;
                    aStep = 2;
                }
                else
                {
                    int64_t kPlusN = k + N;
                    if ((kPlusN & 3) == 0)
                    {
                        aStep = 8;
                        aLimit += ((kPlusN - aLimit) & 7);
                    }
                    else
                    {
                        aStep = 4;
                        aLimit += ((kPlusN - aLimit) & 3);
                    }
                }

                for (a = aLimit; a >= aStart; a -= aStep)
                {
                    int64_t test = a * a - fourkN;
                    int64_t b = (int64_t)sqrt(test);
                    if (b * b == test)
                    {
                        return (int64_t)gcd64(a + b, N);
                    }


                }
            }

            // k == 0 (mod 6) has the highest chance to find a factor; checking it in the high range boosts performance
            if ((factor = lehmanEven(kLimit, kLimit << 1, sqrt4N, N, fourN)) > 1) 
                return factor;

            // Complete middle range
            if ((factor = lehmanOdd(kTwoA + 1, kLimit, sqrt4N, N, fourN)) > 1) 
                return factor;
            if ((factor = lehmanEven(kTwoA + 2, kLimit, sqrt4N, N, fourN)) > 1)
                return factor;
            if ((factor = lehmanEven(kTwoA + 4, kLimit, sqrt4N, N, fourN)) > 1)
                return factor;
            if ((factor = lehmanOdd(kTwoA + 5, kLimit, sqrt4N, N, fourN)) > 1)
                return factor;

            // do trial division after Lehman loop ?
            int rt2 = (int)sqrt(N);

            // do trial division after Lehman loop
            if (!DoTrialFirst)
            {
                //i = 0;
                //while (PRIMES[i] < cbrt)
                //{
                //	if ((N % PRIMES[i]) == 0)
                //		return PRIMES[i];
                //	i++;
                //}
                if ((factor = tdiv_inverse(N, cb_rt)) > 1)
                    return factor;
            }

            // If sqrt(4kN) is very near to an exact integer then the fast ceil() in the 'aStart'-computation
            // may have failed. Then we need a "correction loop":
            for (k = kTwoA + 1; k <= kLimit; k++)
            {
                int64_t a = (int64_t)(sqrt4N * sqRoot[k] + ROUND_UP_DOUBLE) - 1;
                int64_t test = a * a - k * fourN;
                int64_t b = (int64_t)sqrt(test);
                if (b * b == test)
                {
                    return gcd64(a + b, N);
                }
                //{
                //	/* Step 1, reduce to 18% of inputs */
                //	int64 m = test & 127;
                //	if ((m * 0x8bc40d7d) & (m * 0xa1e2f5d1) & 0x14020a)  continue;
                //	/* Step 2, reduce to 7% of inputs (mod 99 reduces to 4% but slower) */
                //	//m = test % 240; if ((m * 0xfa445556) & (m * 0x8021feb1) & 0x614aaa0f) continue;
                //	/* m = n % 99; if ((m*0x5411171d) & (m*0xe41dd1c7) & 0x80028a80) return 0; */
                //	/* Step 3, do the square root instead of any more rejections */
                //	const int64 b = (int64)sqrt(test);
                //	if (b*b == test) {
                //		return gcd64(a + b, N);
                //	}
                //}
            }

            return 0; // fail
        }


        public long LehmanFactor(uint64 N, double Tune, double HartOLF, bool DoTrial, double CutFrac)
        {
            uint b, p, k, r, B, U, Bred, Bred2, inc, FirstCut, ip = 0;
            uint64 a, c, kN, kN4, B2, N480, UU;
            double Tune2, Tune3, x;
            if ((N & 1) == 0) return (2); //N is even
            if (Tune < 0.1)
            {
                Console.WriteLine("Sorry, Lehman only implemented for Tune>=0.1");
                return (-1);
            }

            B = (uint)cbrt(N);
            FirstCut = (uint)(CutFrac * B);
            if (FirstCut < 84) { FirstCut = 84; } //assures prime N will not activate "wrong" Lehman return
            if (FirstCut > 65535) { FirstCut = 65535; }

            if (DoTrial)
            {
                for (ip = 1; ; ip++)
                { //trial division                              
                    p = (uint)prime[ip];
                    if (p >= FirstCut) break;
                    if (N % p == 0) return (p);
                }
            }

            if (N >= 8796393022207UL)
            {
                Console.WriteLine("\nWarning: Sorry, Lehman only implemented for N<8796393022207");
                //return (-1);
            }
            Tune2 = Tune * Tune;
            Tune3 = Tune2 * Tune;
            Bred = (uint)(B / Tune3);

            if (HartOLF > 0.0)
            { // Hart's "OLF" algorithm is tried to get more speed on average...?
                //assert(gcd64(480ull, N) == 1);
                N480 = N * 480;
                UU = (ulong)(18446744073709551615ul / N480);
                Bred2 = (uint)(B * HartOLF);
                if (UU > Bred2) UU = Bred2;
                UU *= N480;
                for (kN = N480; kN < UU; kN += N480)
                {
                    a = (uint64)(sqrt((double)kN) + 0.999999999999999);
                    c = (uint64)a * (uint64)a - kN;
                    //assert(c >= 0);
                    if (0 != issq1024[c & 1023])
                    {
                        if (0 != (issq4199[c % 3465] & 2))
                        {
                            if (0 != (issq4199[c % 4199] & 1))
                            {
                                b = (uint)sqrt(c + 0.9);
                                if (b * b == c)
                                {
                                    if (a >= b)
                                    {
                                        B2 = gcd64(a - b, N);
                                        if (1 < B2 && B2 < N) 
                                            return (long)(B2); 
                                    }
                                    B2 = gcd64(a + b, N);
                                    if (1 < B2 && B2 < N) 
                                        return (long)(B2); 
                                }
                            }
                        }
                    }
                }
            }

            B2 = B * B;
            kN = 0;

            //Lehman suggested (to get more average speed) trying highly-divisible k first. However,
            //my experiments on trying to to that have usually slowed things down versus this simple loop:
            for (k = 1; k <= Bred; k++)
            {
                //if ((k & 1) == 1) { inc = 4; r = (uint)((k + N) % 4); } else { inc = 2; r = 1; }
                if ((k & 1) == 1) { inc = 4; r = (uint)((k + N) >> 2); } else { inc = 2; r = 1; }
                kN += N;
                //assert(kN == k * N);
                if (kN >= 1152921504606846976ul)
                {
                    //Console.WriteLine("Sorry, overflow, N={n} is too large\n");
                    //return (-2);
                }
                //Actually , even if overflow occurs here, one could still use approximate
                //arithmetic to compute kN4's most-signif 64 bits only, then still exactly compute x...
                //With appropriate code alterations is should be possible to extent the range... but
                //I have not tried that idea for trying to "cheat" to gain more precision.
                kN4 = kN * 4;
                x = sqrt((real)kN);
                a = (uint64)x;
                if ((uint64)a * (uint64)a == kN)
                {
                    B2 = gcd64((uint64)a, N);
                    //assert(B2 > 1);
                    //assert(B2 < N);
                    return (long)B2;
                }
                x *= 2;
                a = (uint64)(x + 0.9999999665); //very carefully chosen.
                                                //Let me repeat that: a = x+0.9999999665.  Really.
                b = (uint)(a % inc); b = ((uint)a + (inc + r - b) % inc);   //b is a but adjusted upward to make b%inc=r.
                                                                            //assert(b % inc == r);
                                                                            //assert(b >= a);
                                                                            //assert(b <= a + 4);
                c = (uint64)b * (uint64)b - kN4;  //this is the precision bottleneck.
                                                  //At this point, I used to do a test:
                                                  //if( c+kN4 != (uint64)b*(uint64)b ) //overflow-caused failure: exit!
                                                  //	printf("Sorry3, unrepairable overflow, N=%llu is too large\n", N);
                                                  //  return(0);
                                                  //However, I've now reconsidered.  I claim C language computes c mod 2^64 correctly.
                                                  //If overflow happens, this is irrelevant because c is way smaller than 2^64, kN4, and b*b.
                                                  //Hence c should be correct, despite the overflow. Hence I am removing this error-exit.

                U = (uint)(x + B2 / (2 * x));
                //old code was  U = sqrt((real)(B2+kN4+0.99));   and was 4% slower.

                //Below loop is: for(all integers a with 0<=a*a-kN4<=B*B and with a%inc==r)
                for (a = b; a <= U; c += inc * (a + a + inc), a += inc)
                {
                    //again, even though this assert can fail due to overflow, that overflow should not matter:
                    //assert( c == (uint64)a*(uint64)a-kN4 );   
                    /** Programming trick:    naive code:     c = a*a-kN4;
                   In the inner loop c is bounded between 0 and T^2*N^(2/3)
                   and can be updated additively by adding inc*(anew+aold) to it
                   when we update a to anew=a+inc. This saves a multiplication in
                   the inner loop and/or allows us to reduce precision. **/
                    if (0 != issq1024[c & 1023])
                    {
                        if (0 != (issq4199[c % 3465] & 2))
                        {
                            if (0 != (issq4199[c % 4199] & 1))
                            {
                                b = (uint)sqrt(c + 0.9);
                                if (b * b == c)
                                { //square found
                                    B2 = gcd64((uint64)(a + b), N);
                                    //assert(B2 > 1);
                                    if (B2 >= N)
                                    {
                                        Console.WriteLine($"theorem failure: B2={B2} N={N}\n", B2, N);
                                    }
                                    return (long)(B2);
                                }
                            }
                        }
                    }
                }

            }
            //square-finding has failed so resume missing part of trial division: 
            if (DoTrial)
            {
                if (B > 65535) B = 65535;
                for (; ; ip++)
                {
                    p = (uint)prime[ip];
                    if (p >= B) break;
                    if (N % p == 0) return (p);
                }
            }

            return (long)(N); //N is prime
        }



    }
}
