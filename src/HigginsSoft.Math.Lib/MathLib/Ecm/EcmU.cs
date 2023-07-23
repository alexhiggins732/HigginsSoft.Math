/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Some routines inspired by the Stanford Bit Twiddling Hacks by Sean Eron Anderson:
// http://graphics.stanford.edu/~seander/bithacks.html

#define TARGET_64BIT
// a comparitively tiny amount of p-1 work can find
// ~15 - 30% of factors before any ecm is performed.
#define DO_UPM1
//gmp-ecm has do upm1 defined, but in practice 30 semi-primes from 2^32-2^62 takes 77ms without and 9 seconds with.
// see: EcmTests.FactorEcm2p32To2p62
#undef DO_UPM1
#define _MSC_VER

using System;
using uint64_t = System.UInt64;
using uint32_t = System.UInt32;
using uint8_t = System.Int32;
using MathGmp.Native;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;


namespace HigginsSoft.Math.Lib
{
    //Large portions of this code from GMP_ECM
    public partial class EcmU
    {
        public Factorization FactorEcm(ulong n, int arbitrary = 1)

        {
            var state = 0UL;
            return FactorEcm(n, arbitrary, ref state);
        }

        public Factorization FactorEcm(ulong n, int arbitrary, ref ulong state)
        {
            Factorization result = new();
            var p = Factor(n, arbitrary, ref state);
            if (p > 1 && p < n)
            {
                result.Add(n / p, 1);
                result.Add(p, 1);
            }
            else
            {
                result.Add(n, 1);
            }
            return result;
        }

        const double INV_2_POW_32 = 1.0 / (double)((uint64_t)(1) << 32);


        public unsafe static ulong Factor(ulong n, int arbitrary)
        {
            var state = 0ul;
            return Factor(n, arbitrary, ref state);
        }

        public unsafe static ulong Factor(ulong n, int arbitrary, ref ulong randomstate)
        {
            if (randomstate == 0)
            {
                var rnd = new Random();
                var stateBytes = new byte[8];
                rnd.NextBytes(stateBytes);

                fixed (byte* ptr = stateBytes)
                {
                    randomstate = *(ulong*)ptr;
                }
            }

            var result = getfactor_uecm(n, arbitrary, ref randomstate);
            return result;
        }

        public static ulong Factor(ulong n, int B1, int curves)
        {

            ulong state = 0;
            return Factor(n, B1, 25 * B1, curves, ref state);
        }

        public static ulong Factor(ulong n, int B1, int B2, int curves, ref ulong state)
        {
            ulong f64 = 0;
            return Factor(n, ref f64, B1, B2, curves, ref state);
        }

        public static ulong Factor(ulong n, ref ulong f64, int B1, int B2, int curves, ref ulong state)
        {
            var result = microecm(n, ref f64,  B1, B2, curves, ref state);
            return (ulong)result;
        }

        struct uecm_pt
        {
            public uint64_t X;
            public uint64_t Z;
        };
        //todo see, investigate use of avx2 vector256


        // getfactor_uecm() returns 1 if unable to find a factor of q64,
        // Otherwise it returns a factor of q64.
        // 
        // if the input is known to have no small factors, set is_arbitrary=0, 
        // otherwise, set is_arbitrary=1 and a few curves targetting small factors
        // will be run prior to the standard sequence of curves for the input size.
        //  
        // Prior to your first call of getfactor_uecm(), set *pran = 0  (or set it to
        // some other arbitrary value); after that, don't change *pran.
        // FYI: *pran is used within this file by a random number generator, and it
        // holds the current value of a pseudo random sequence.  Your first assigment
        // to *pran seeds the sequence, and after seeding it you don't want to
        // change *pran, since that would restart the sequence.
        static uint64_t getfactor_uecm(uint64_t q64, int is_arbitrary, ref uint64_t pran)
        {
            if (q64 % 2 == 0)
                return 2;
            int bits = uecm_get_bits(q64);
            return uecm_dispatch(q64, bits, is_arbitrary, ref pran);
        }

        static int uecm_get_bits(ulong q64)
            => MathLib.BitLength(q64);

        static uint64_t getfactor_upm1(uint64_t q64, int b1)
            => Pm1.getfactor_upm1(q64, b1);

        static ulong uecm_dispatch(ulong n, int targetBits, int arbitrary, ref ulong ploc_lcg)
        {
            int B1, curves;
            uint64_t f64 = 1;
#if MICRO_ECM_VERBOSE_PRINTF
                //uecm_stage2_pair(70, 180, 120, 120, 30, primes);
                //uecm_stage2_pair(180, 25 * 70, 150, 120, 30, primes);

                //uecm_stage2_pair(47, 150, 90, 120, 30, primes);
                //uecm_stage2_pair(150, 25 * 47, 150, 120, 30, primes);

                //uecm_stage2_pair(30, 150, 90, 120, 30, primes);
                //uecm_stage2_pair(150, 25 * 30, 150, 120, 30, primes);
                // 
                // for b1=125: Need Pa = 150, Pd = 120, Pad = 30
                // for b1=165: Need Pa = 180, Pd = 120, Pad = 60
                // for b1=205: need Pa = 210, Pd = 120, Pad = 90
                //uecm_stage2_pair(125, 50 * 125, 150, 120, 30, primes);

                //exit(0);
#endif
            if (arbitrary == 1)
            {
                // try fast attempts to find possible small factors.
                {
                    B1 = 47;
                    curves = 1;
                    microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
                    if (f64 > 1)
                        return f64;
                }
                {
                    B1 = 70;
                    curves = 1;
                    microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
                    if (f64 > 1)
                        return f64;
                }
                if (targetBits > 58)
                {
                    B1 = 125;
                    curves = 1;
                    microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
                    if (f64 > 1)
                        return f64;
                }
            }

            if (targetBits <= 40)
            {
                B1 = 27;
                curves = 32;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }
            else if (targetBits <= 44)
            {
                B1 = 47;
                curves = 32;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }
            else if (targetBits <= 48)
            {
                // multi-thread issue here...
                //f64 = LehmanFactor(n, 0, 0, 0);
                B1 = 70;
                curves = 32;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }
            else if (targetBits <= 52)
            {
                //gmp-ecm has DO_UPM1 defined with a note it picks up about 1/3rd of factors.
                // however, 30 semiprimes from 2^32 t to 2^62 takes 9 seconds with and 77ms without.
                // not worth wall time, period, especially if it is only getting 33%.
# if DO_UPM1
                f64 = getfactor_upm1(n, 100);
                if (f64 > 1)
                    return f64;
#endif
                B1 = 85;
                curves = 32;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }
            else if (targetBits <= 58)
            {
# if DO_UPM1
                f64 = getfactor_upm1(n, 333);
                if (f64 > 1)
                    return f64;
#endif
                B1 = 125;
                curves = 32;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }
            else if (targetBits <= 62)
            {
# if DO_UPM1
                f64 = getfactor_upm1(n, 333);
                if (f64 > 1)
                    return f64;
#endif
                B1 = 165;
                curves = 42;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }
            else if (targetBits <= 64)
            {
#if DO_UPM1
                f64 = getfactor_upm1(n, 333);
                if (f64 > 1)
                    return f64;
#endif
                B1 = 205;
                curves = 42;
                microecm(n, ref f64, B1, 25 * B1, curves, ref ploc_lcg);
            }

            return f64;
        }

        static int microecm(uint64_t n, ref uint64_t f, int B1, int B2,
                     int curves, ref uint64_t ploc_lcg)
        {
            //attempt to factor n with the elliptic curve method
            //following brent and montgomery's papers, and CP's book
            int curve;
            int found = 0;
            int result;
            uecm_pt P = new();
            uint64_t tmp1 = 0;

            uint64_t rho = (uint64_t)0 - uecm_multiplicative_inverse(n);

            uint32_t stg1_max = (uint32_t)B1;
            //    uint32_t stg2_max = B2;

            //    uint64_t unityval = u64div(1, n);
            // Let R = 2^64.  We can see R%n ≡ (R-n)%n  (mod n)
            uint64_t unityval = ((uint64_t)0 - n) % n;   // unityval == R  (mod n)

            uint64_t two = uecm_addmod(unityval, unityval, n);
            uint64_t four = uecm_addmod(two, two, n);
            uint64_t five = uecm_addmod(unityval, four, n);
            uint64_t eight = uecm_addmod(four, four, n);
            uint64_t sixteen = uecm_addmod(eight, eight, n);
            uint64_t two_8 = uecm_sqrredc(sixteen, n, rho);   // R*2^8         (mod n)
            uint64_t two_16 = uecm_sqrredc(two_8, n, rho);    // R*2^16        (mod n)
            uint64_t two_32 = uecm_sqrredc(two_16, n, rho);   // R*2^32        (mod n)
            uint64_t Rsqr = uecm_sqrredc(two_32, n, rho);     // R*2^64 == R*R  (mod n)

            uint64_t s = 0;

            int use_prebuilt_curves = 1;
            uint32_t[] smallsigma = { 11, 61, 56, 81, 83, 7, 30, 51 };
            uint64_t[] Z = new[] { 85184ul, 14526784ul, 11239424ul, 34012224ul, 36594368ul, 21952ul, 1728000ul, 8489664ul };
            uint64_t[] X = new[] { 1560896UL, 51312965696UL, 30693697091UL, 281784327616UL,
                326229015104UL, 85184UL, 716917375UL, 17495004736UL };
            uint64_t[] negt1 = new[] { 146313216UL, 476803160866816UL, 236251574395731UL, 4838810084806656UL,
                          5902145938870272UL, 655360UL, 1305683671875UL, 109380272541696UL };
            uint64_t[] d = new[] { 1098870784UL, 200325818077184UL, 110006210374144UL, 1460769954361344UL,
                1732928528232448UL, 38162432UL, 1376481360000UL, 57103695458304UL };

            uint64_t likely_gcd = 1;

            f = 1ul;
            for (curve = 0; (uint32_t)curve < curves; curve++)
            {
# if MICRO_ECM_VERBOSE_PRINTF
                stg1Add = 0;
                stg1Doub = 0;
                printf("commencing curve %d of %u\n", curve, curves);
#endif


                if ((curve < 8) && (use_prebuilt_curves > 0))
                {
                    //ubuild(ref P, rho, &work, goodsigma[curve]); // sigma);
                    //sigma = smallsigma[curve];
                    // lookup point
                    P.X = X[curve];
                    P.Z = Z[curve];
                    // some computation left to do for S parameter for this 'n'

                    uint64_t num, uvc, u3vc;

                    uint64_t dem = d[curve];
                    // jeff note:  uecm_modinv_64(dem, n) appears to require dem < n,
                    // so I now take the remainder to achieve dem < n.

                    // This is a faster way to compute dem = dem % n, even if the CPU
                    // has extremely fast division (as present in many new CPUs).
                    dem = uecm_mulredc(dem, unityval, n, rho);

                    num = uecm_mulredc(negt1[curve], Rsqr, n, rho);     // to Monty rep.
                                                                        // The mulredc postcondition guarantees  num < n.
                    num = n - num;

                    dem = uecm_modinv_64(dem, n, ref likely_gcd);

                    dem = uecm_mulredc(dem, Rsqr, n, rho);              // to Monty rep.
                    s = uecm_mulredc(num, dem, n, rho);

                    P.X = uecm_mulredc(P.X, Rsqr, n, rho);              // to Monty rep.
                    P.Z = uecm_mulredc(P.Z, Rsqr, n, rho);              // to Monty rep.
                }
                else
                {
                    likely_gcd = uecm_build(ref P, rho, n, ref ploc_lcg, ref s, five, Rsqr);
                }

                if (likely_gcd > 1)
                {
                    // If the gcd gave us a factor, we're done.  If not, since gcd != 1
                    // the inverse calculated in uecm_build would have bogus, and so this
                    // curve is probably set up for failure (hence we continue).
                    if (likely_gcd == n || n % likely_gcd != 0)
                        continue;
                    f = likely_gcd;
                    break;
                }


# if MICRO_ECM_VERBOSE_PRINTF
                {
                    printf("curve parameters:\n");
                    printf("\tn = %" PRIu64 "\n", n);
                    printf("\trho = %" PRIu64 "\n", rho);
                    printf("\tx = %" PRIx64 "\n", P.X);
                    printf("\tz = %" PRIx64 "\n", P.Z);
                    printf("\tb = %" PRIx64 "\n", s);
                }
#endif
                uecm_stage1(rho, n, ref P, (uint64_t)stg1_max, s);
                result = uecm_check_factor(P.Z, n, ref tmp1);

# if MICRO_ECM_VERBOSE_PRINTF
                {
                    printf("after stage1: P = %" PRIx64 ", %" PRIx64 "\n", P.X, P.Z);
                }
#endif
                if (result == 1)
                {
# if MICRO_ECM_VERBOSE_PRINTF
                    printf("\nfound factor %" PRIx64 " in stage 1\n", tmp1);
#endif
                    f = tmp1;
                    break;
                }

                if (B2 > B1)
                {
                    uint64_t stg2acc = uecm_stage2(ref P, rho, n, stg1_max, s, unityval);

# if MICRO_ECM_VERBOSE_PRINTF
                    {
                        printf("after stage2: A = %" PRIx64 "\n", stg2acc);
                    }
                    uint32_t paired = 0;
                    uint64_t numprimes = 0;
                    printf("performed %d pair-multiplies for %" PRIu64 " primes in stage 2\n",
                        paired, numprimes);
                    printf("performed %u point-additions and %u point-doubles in stage 2\n",
                        ptadds + stg1Add, stg1Doub);
#endif
                    result = uecm_check_factor(stg2acc, n, ref tmp1);

                    if (result == 1)
                    {
# if MICRO_ECM_VERBOSE_PRINTF
                        printf("\nfound factor %" PRIx64 " in stage 2\n", tmp1);
#endif
                        f = tmp1;
                        break;
                    }
                }
            }

            return curve;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        private static ulong uecm_mulredc(ulong x, ulong y, ulong n, ulong nhat)
        {
            return uecm_mulredc_alt(x, y, n, 0 - nhat);
        }


        // for this algorithm, see https://jeffhurchalla.com/2022/04/25/a-faster-multiplicative-inverse-mod-a-power-of-2/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_multiplicative_inverse(uint64_t a)
        {
            //    assert(a%2 == 1);  // the inverse (mod 2<<64) only exists for odd values
            uint64_t x0 = (3 * a) ^ 2;
            uint64_t y = 1 - a * x0;
            uint64_t x1 = x0 * (1 + y);
            y *= y;
            uint64_t x2 = x1 * (1 + y);
            y *= y;
            uint64_t x3 = x2 * (1 + y);
            y *= y;
            uint64_t x4 = x3 * (1 + y);
            return x4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_addmod(uint64_t x, uint64_t y, uint64_t n)
        {
#if UNDEFINED
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
        static uint64_t uecm_sqrredc(uint64_t x, uint64_t n, uint64_t nhat)
        {
            return uecm_mulredc_alt(x, x, n, 0 - nhat);
        }

        /* --- The following two functions are written by Jeff Hurchalla, Copyright 2022 --- */

        // for this algorithm, see https://jeffhurchalla.com/2022/04/28/montgomery-redc-using-the-positive-inverse-mod-r/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_mulredc_alt(uint64_t x, uint64_t y, uint64_t N, uint64_t invN)
        {
#if _MSC_VER
            uint64_t T_hi = 0;
            uint64_t T_lo = _umul128(x, y, ref T_hi);
            uint64_t m = T_lo * invN;
            uint64_t mN_hi = _umulh(m, N);
#else
            __uint128_t prod = (__uint128_t)x * y;
            uint64_t T_hi = (uint64_t)(prod >> 64);
            uint64_t T_lo = (uint64_t)(prod);
            uint64_t m = T_lo * invN;
            __uint128_t mN = (__uint128_t)m * N;
            uint64_t mN_hi = (uint64_t)(mN >> 64);
#endif
            uint64_t tmp = T_hi + N;

            tmp = tmp - mN_hi;
            uint64_t result = T_hi - mN_hi;
            result = (T_hi < mN_hi) ? tmp : result;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong _umulh(ulong m, ulong n)
        {
            ulong mN_hi = System.Math.BigMul(m, n, out ulong mN_lo);
            return mN_hi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong _umul128(ulong x, ulong y, ref ulong hi)
            => System.Math.BigMul(x, y, out hi);


        // jeff: "likely_gcd" is probably always the correct gcd, but I didn't add this
        // parameter by using any proof; it's conceivable it might be wrong sometimes.
        // See comments within the function.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_modinv_64(uint64_t a, uint64_t p, ref uint64_t plikely_gcd)
        {
            /* thanks to the folks at www.mersenneforum.org */

            uint64_t ps1, ps2, parity, dividend, divisor, rem, q, t;

            q = 1;
            rem = a;
            dividend = p;
            divisor = a;
            ps1 = 1;
            ps2 = 0;
            parity = 0;

            while (divisor > 1)
            {
                rem = dividend - divisor;
                t = rem - divisor;
                if (rem >= divisor)
                {
                    q += ps1; rem = t; t -= divisor;
                    if (rem >= divisor)
                    {
                        q += ps1; rem = t; t -= divisor;
                        if (rem >= divisor)
                        {
                            q += ps1; rem = t; t -= divisor;
                            if (rem >= divisor)
                            {
                                q += ps1; rem = t; t -= divisor;
                                if (rem >= divisor)
                                {
                                    q += ps1; rem = t; t -= divisor;
                                    if (rem >= divisor)
                                    {
                                        q += ps1; rem = t; t -= divisor;
                                        if (rem >= divisor)
                                        {
                                            q += ps1; rem = t; t -= divisor;
                                            if (rem >= divisor)
                                            {
                                                q += ps1; rem = t;
                                                if (rem >= divisor)
                                                {
                                                    q = dividend / divisor;
                                                    rem = dividend % divisor;
                                                    q *= ps1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                q += ps2;
                parity = ~parity;
                dividend = divisor;
                divisor = rem;
                ps2 = ps1;
                ps1 = q;
            }

            // jeff: added "likely_gcd".  this function seems to be a variant on the
            // extended euclidean algorithm, and thus the gcd likely equals the dividend
            // as calculated below.  However I'm doing this by analogy and educated
            // guess, not by proof.  It appears to work in all tests, so I suspect it is
            // correct, but it's possible this could be wrong in some cases.
            if (divisor == 1)
                dividend = divisor;
            plikely_gcd = dividend;


            if (parity == 0)
                return ps1;
            else
                return p - ps1;
        }


        static uint64_t uecm_build(ref uecm_pt P, uint64_t rho, uint64_t n, ref uint64_t ploc_lcg, ref uint64_t ps, uint64_t five, uint64_t Rsqr)
        {
            uint64_t t1, t2, t3, t4;
            uint64_t u, v;

            uint32_t sigma = uecm_lcg_rand_32B(7, unchecked((uint32_t)(-1)), ref ploc_lcg);

            u = uecm_mulredc((uint64_t)sigma, Rsqr, n, rho);  // to_monty(sigma)

            //printf("sigma = %" PRIu64 ", u = %" PRIu64 ", n = %" PRIu64 "\n", sigma, u, n);

            v = uecm_addmod(u, u, n);
            v = uecm_addmod(v, v, n);            // 4*sigma

            //printf("v = %" PRIu64 "\n", v);

            u = uecm_sqrredc(u, n, rho);
            t1 = five;

            //printf("monty(5) = %" PRIu64 "\n", t1);

            u = uecm_submod(u, t1, n);           // sigma^2 - 5

            //printf("u = %" PRIu64 "\n", u);

            t1 = uecm_mulredc(u, u, n, rho);
            uint64_t tmpx = uecm_mulredc(t1, u, n, rho);  // u^3

            uint64_t v2 = uecm_addmod(v, v, n);             // 2*v
            uint64_t v4 = uecm_addmod(v2, v2, n);           // 4*v
            uint64_t v8 = uecm_addmod(v4, v4, n);           // 8*v
            uint64_t v16 = uecm_addmod(v8, v8, n);          // 16*v
            uint64_t t5 = uecm_mulredc(v16, tmpx, n, rho);    // 16*u^3*v

            t1 = uecm_mulredc(v, v, n, rho);
            uint64_t tmpz = uecm_mulredc(t1, v, n, rho);  // v^3

            //compute parameter A
            t1 = uecm_submod(v, u, n);           // (v - u)
            t2 = uecm_sqrredc(t1, n, rho);
            t4 = uecm_mulredc(t2, t1, n, rho);   // (v - u)^3

            t1 = uecm_addmod(u, u, n);           // 2u
            t2 = uecm_addmod(u, v, n);           // u + v
            t3 = uecm_addmod(t1, t2, n);         // 3u + v

            t1 = uecm_mulredc(t3, t4, n, rho);   // a = (v-u)^3 * (3u + v)

            // u holds the denom (jeff note: isn't it t5 that has the denom?)
            // t1 holds the numer
            // accomplish the division by multiplying by the modular inverse
            t2 = 1;
            t5 = uecm_mulredc(t5, t2, n, rho);   // take t5 out of monty rep

            uint64_t likely_gcd = 1;
            t3 = uecm_modinv_64(t5, n, ref likely_gcd);

            t3 = uecm_mulredc(t3, Rsqr, n, rho); // to_monty(t3)
            ps = uecm_mulredc(t3, t1, n, rho);

            P.X = tmpx;
            P.Z = tmpz;

            return likely_gcd;
        }

        static uint32_t uecm_lcg_rand_32B(uint32_t lower, uint32_t upper, ref uint64_t ploc_lcg)
        {
            ploc_lcg = 6364136223846793005UL * (ploc_lcg) + 1442695040888963407UL;
            return lower + (uint32_t)(
                (double)(upper - lower) * (double)((ploc_lcg) >> 32) * INV_2_POW_32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_submod(uint64_t a, uint64_t b, uint64_t n)
        {
            uint64_t r0 = 0;
            if (_subborrow_u64(0, a, b, ref r0))
                r0 += n;
            return r0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool _subborrow_u64(int borrow, ulong x, ulong y, ref ulong result)
        {

            ulong temp = x - y;
            result = temp - (ulong)borrow;
            return temp < x;

        }

        static int uecm_check_factor(uint64_t Z, uint64_t n, ref uint64_t f)
        {
            int status = 0;
            f = uecm_bingcd64(n, Z);

            if (f > 1)
            {
                if (f == n)
                {
                    f = 0;
                    status = 0;
                }
                else
                {
                    status = 1;
                }
            }
            return status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int _trail_zcnt64(ulong value)
            => MathLib.TrailingZeroCount(value);

        static uint64_t uecm_bingcd64(uint64_t u, uint64_t v)
        {
#if true
            if (u == 0)
            {
                return v;
            }
            if (v != 0)
            {
                int count = 0;
                uint64_t u0 = u;
                uint64_t v0 = v;

                int j = _trail_zcnt64(v);
                v = (uint64_t)(v >> j);
              
                while (true)
                {
                    count++;
                    uint64_t tmp = u;
                    uint64_t sub1 = (uint64_t)(v - tmp);
                    uint64_t sub2 = (uint64_t)(tmp - v);
                    if (tmp == v)
                        break;
                    u = (tmp >= v) ? v : tmp;
                    v = (tmp >= v) ? sub2 : sub1;
                    // For the line below, the standard way to write this algorithm
                    // would have been to use _trail_zcnt64(v)  (instead of
                    // _trail_zcnt64(sub1)).  However, as pointed out by
                    // https://gmplib.org/manual/Binary-GCD, "in twos complement the
                    // number of low zero bits on u-v is the same as v-u, so counting or
                    // testing can begin on u-v without waiting for abs(u-v) to be
                    // determined."  Hence we are able to use sub1 for the argument.
                    // By removing the dependency on abs(u-v), the CPU can execute
                    // _trail_zcnt64() at the same time as abs(u-v).
                    j = _trail_zcnt64(sub1);
                    v = (uint64_t)(v >> j);
                }
                if (count > 225)
                {
                    var ul = MathLib.ILogB(u0);
                    var vl = MathLib.ILogB(v0);
                    var diff = ul - vl;

                }
            }
            return u;
#else
            // For reference, or if in the future we need to allow an even u,
            // this version allows u to be even or odd.
            if (u == 0)
            {
                return v;
            }
            if (v != 0)
            {
                int i = _trail_zcnt64(u);
                int j = _trail_zcnt64(v);
                u = (uint64_t)(u >> i);
                v = (uint64_t)(v >> j);
                int k = (i < j) ? i : j;
                while (1)
                {
                    uint64_t tmp = u;
                    uint64_t sub1 = (uint64_t)(v - tmp);
                    uint64_t sub2 = (uint64_t)(tmp - v);
                    if (tmp == v)
                        break;
                    u = (tmp >= v) ? v : tmp;
                    v = (tmp >= v) ? sub2 : sub1;
                    // For the line below, the standard way to write this algorithm
                    // would have been to use _trail_zcnt64(v)  (instead of
                    // _trail_zcnt64(sub1)).  However, as pointed out by
                    // https://gmplib.org/manual/Binary-GCD, "in twos complement the
                    // number of low zero bits on u-v is the same as v-u, so counting or
                    // testing can begin on u-v without waiting for abs(u-v) to be
                    // determined."  Hence we are able to use sub1 for the argument.
                    // By removing the dependency on abs(u-v), the CPU can execute
                    // _trail_zcnt64() at the same time as abs(u-v).
                    j = _trail_zcnt64(sub1);
                    v = (uint64_t)(v >> j);
                }
                u = (uint64_t)(u << k);
            }
            return u;
#endif
        }


        static void uecm_stage1(uint64_t rho, uint64_t n, ref uecm_pt P, uint64_t stg1, uint64_t s)
        {
            uint64_t q;

            // handle the only even case
            q = 2;
            while (q < stg1 * 4)  // jeff: multiplying by 4 improves perf ~1%
            {
                uint64_t diff1 = uecm_submod(P.X, P.Z, n);
                uint64_t sum1 = uecm_addmod(P.X, P.Z, n);
                uecm_udup(s, rho, n, sum1, diff1, ref P);
                q *= 2;
            }

            if (stg1 == 27)
            {
                uecm_uprac(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 11, 0.580178728295464130, s);
                uecm_uprac(rho, n, ref P, 13, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 17, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 19, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 23, 0.522786351415446049, s);
            }
            else if (stg1 == 47)
            {
                // jeff: improved perf slightly by using one more uprac for 3,
                // and removing uprac for 47.
                uecm_uprac(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 11, 0.580178728295464130, s);
                uecm_uprac(rho, n, ref P, 13, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 17, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 19, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 23, 0.522786351415446049, s);
                uecm_uprac(rho, n, ref P, 29, 0.548409048446403258, s);
                uecm_uprac(rho, n, ref P, 31, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 37, 0.580178728295464130, s);
                uecm_uprac(rho, n, ref P, 41, 0.548409048446403258, s);
                uecm_uprac(rho, n, ref P, 43, 0.618033988749894903, s);
                //        uecm_uprac(rho, n, ref P, 47, 0.548409048446403258, s);
            }
            else if (stg1 == 59)
            {   // jeff: probably stg1 of 59 would benefit from similar changes
                // as stg1 of 47 above, but I didn't bother. Stg1 of 59 seems to
                // always perform worse than stg1 of 47, so there doesn't seem
                // to be any reason to ever use stg1 of 59.
                uecm_uprac(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 11, 0.580178728295464130, s);
                uecm_uprac(rho, n, ref P, 13, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 17, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 19, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 23, 0.522786351415446049, s);
                uecm_uprac(rho, n, ref P, 29, 0.548409048446403258, s);
                uecm_uprac(rho, n, ref P, 31, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 1961, 0.552936068843375, s);   // 37 * 53
                uecm_uprac(rho, n, ref P, 41, 0.548409048446403258, s);
                uecm_uprac(rho, n, ref P, 43, 0.618033988749894903, s);
                uecm_uprac(rho, n, ref P, 47, 0.548409048446403258, s);
                uecm_uprac(rho, n, ref P, 59, 0.548409048446403258, s);
            }
            else if (stg1 == 70)
            {
                // call prac with best ratio found in deep search.
                // some composites are cheaper than their
                // constituent primes.
                uecm_uprac70(rho, n, ref P, s);
            }
            else // if (stg1 >= 85)
            {
                uecm_uprac85(rho, n, ref P, s);

                if (stg1 == 85)
                {
                    uecm_uprac(rho, n, ref P, 61, 0.522786351415446049, s);
                }
                else
                {
                    uecm_uprac(rho, n, ref P, 5, 0.618033988749894903, s);
                    uecm_uprac(rho, n, ref P, 11, 0.580178728295464130, s);
                    //            uecm_uprac(rho, n, ref P, 61, 0.522786351415446049, s);
                    uecm_uprac(rho, n, ref P, 89, 0.618033988749894903, s);
                    uecm_uprac(rho, n, ref P, 97, 0.723606797749978936, s);
                    uecm_uprac(rho, n, ref P, 101, 0.556250337855490828, s);
                    uecm_uprac(rho, n, ref P, 107, 0.580178728295464130, s);
                    uecm_uprac(rho, n, ref P, 109, 0.548409048446403258, s);
                    uecm_uprac(rho, n, ref P, 113, 0.618033988749894903, s);

                    if (stg1 == 125)
                    {
                        // jeff: moved 61 to here
                        uecm_uprac(rho, n, ref P, 61, 0.522786351415446049, s);
                        uecm_uprac(rho, n, ref P, 103, 0.632839806088706269, s);
                    }
                    else
                    {
                        uecm_uprac(rho, n, ref P, 7747, 0.552188778811121, s); // 61 x 127
                        uecm_uprac(rho, n, ref P, 131, 0.618033988749894903, s);
                        uecm_uprac(rho, n, ref P, 14111, 0.632839806088706, s);  // 103 x 137
                        uecm_uprac(rho, n, ref P, 20989, 0.620181980807415, s);  // 139 x 151
                        uecm_uprac(rho, n, ref P, 157, 0.640157392785047019, s);
                        uecm_uprac(rho, n, ref P, 163, 0.551390822543526449, s);

                        if (stg1 == 165)
                        {
                            uecm_uprac(rho, n, ref P, 149, 0.580178728295464130, s);
                        }
                        else
                        {
                            uecm_uprac(rho, n, ref P, 13, 0.618033988749894903, s);
                            uecm_uprac(rho, n, ref P, 167, 0.580178728295464130, s);
                            uecm_uprac(rho, n, ref P, 173, 0.612429949509495031, s);
                            uecm_uprac(rho, n, ref P, 179, 0.618033988749894903, s);
                            uecm_uprac(rho, n, ref P, 181, 0.551390822543526449, s);
                            uecm_uprac(rho, n, ref P, 191, 0.618033988749894903, s);
                            uecm_uprac(rho, n, ref P, 193, 0.618033988749894903, s);
                            uecm_uprac(rho, n, ref P, 29353, 0.580178728295464, s);  // 149 x 197
                            uecm_uprac(rho, n, ref P, 199, 0.551390822543526449, s);
                        }
                    }
                }
            }
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void uecm_udup(uint64_t s, uint64_t rho, uint64_t n,
            uint64_t insum, uint64_t indiff, ref uecm_pt P)
        {
            uint64_t tt1 = uecm_sqrredc(indiff, n, rho);          // U=(x1 - z1)^2
            uint64_t tt2 = uecm_sqrredc(insum, n, rho);           // V=(x1 + z1)^2
            P.X = uecm_mulredc(tt1, tt2, n, rho);         // x=U*V

            uint64_t tt3 = uecm_submod(tt2, tt1, n);          // w = V-U
            tt2 = uecm_mulredc(tt3, s, n, rho);      // w = (A+2)/4 * w
            tt2 = uecm_addmod(tt2, tt1, n);          // w = w + U
            P.Z = uecm_mulredc(tt2, tt3, n, rho);         // Z = w*(V-U)

# if MICRO_ECM_VERBOSE_PRINTF
            stg1Doub++;
#endif
            return;
        }

        static void uecm_uprac(uint64_t rho, uint64_t n, ref uecm_pt P, uint64_t c, double v, uint64_t s)
        {
            uint64_t d, e, r;
            int i;
            uint64_t s1, s2, d1, d2;
            uint64_t swp;

            // we require c != 0
            int shift = _trail_zcnt64(c);
            c = c >> shift;

            d = c;
            r = (uint64_t)((double)d * v + 0.5);

            d = c - r;
            e = 2 * r - c;

            uecm_pt pt1, pt2, pt3;

            // the first one is always a doubling
            // point1 is [1]P
            pt1.X = pt2.X = pt3.X = P.X;
            pt1.Z = pt2.Z = pt3.Z = P.Z;

            d1 = uecm_submod(pt1.X, pt1.Z, n);
            s1 = uecm_addmod(pt1.X, pt1.Z, n);

            // point2 is [2]P
            uecm_udup(s, rho, n, s1, d1, ref pt1);

            while (d != e)
            {
                if (d < e)
                {
                    r = d;
                    d = e;
                    e = r;
                    swp = pt1.X;
                    pt1.X = pt2.X;
                    pt2.X = swp;
                    swp = pt1.Z;
                    pt1.Z = pt2.Z;
                    pt2.Z = swp;
                }
                if (d - e <= e / 4 && ((d + e) % 3) == 0)
                {
                    d = (2 * d - e) / 3;
                    e = (e - d) / 2;

                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt1, pt2, pt3, ref pt4); // T = A + B (C)
                    uecm_pt pt5 = new();
                    uecm_uadd(rho, n, pt4, pt1, pt2, ref pt5); // T2 = T + A (B)
                    uecm_uadd(rho, n, pt2, pt4, pt1, ref pt2); // B = B + T (A)

                    swp = pt1.X;
                    pt1.X = pt5.X;
                    pt5.X = swp;
                    swp = pt1.Z;
                    pt1.Z = pt5.Z;
                    pt5.Z = swp;
                }
                else if (d - e <= e / 4 && (d - e) % 6 == 0)
                {
                    d = (d - e) / 2;

                    d1 = uecm_submod(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod(pt1.X, pt1.Z, n);

                    uecm_uadd(rho, n, pt1, pt2, pt3, ref pt2);        // B = A + B (C)
                    uecm_udup(s, rho, n, s1, d1, ref pt1);        // A = 2A
                }
                else if ((d + 3) / 4 <= e)
                {
                    d -= e;

                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

                    swp = pt2.X;
                    pt2.X = pt4.X;
                    pt4.X = pt3.X;
                    pt3.X = swp;
                    swp = pt2.Z;
                    pt2.Z = pt4.Z;
                    pt4.Z = pt3.Z;
                    pt3.Z = swp;
                }
                else if ((d + e) % 2 == 0)
                {
                    d = (d - e) / 2;

                    d2 = uecm_submod(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod(pt1.X, pt1.Z, n);

                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt2);        // B = B + A (C)
                    uecm_udup(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (d % 2 == 0)
                {
                    d /= 2;

                    d2 = uecm_submod(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod(pt1.X, pt1.Z, n);

                    uecm_uadd(rho, n, pt3, pt1, pt2, ref pt3);        // C = C + A (B)
                    uecm_udup(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (d % 3 == 0)
                {
                    d = d / 3 - e;

                    d1 = uecm_submod(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod(pt1.X, pt1.Z, n);

                    uecm_pt pt4 = new();
                    uecm_udup(s, rho, n, s1, d1, ref pt4);        // T = 2A
                    uecm_pt pt5 = new();
                    uecm_uadd(rho, n, pt1, pt2, pt3, ref pt5);        // T2 = A + B (C)
                    uecm_uadd(rho, n, pt4, pt1, pt1, ref pt1);        // A = T + A (A)
                    uecm_uadd(rho, n, pt4, pt5, pt3, ref pt4);        // T = T + T2 (C)

                    swp = pt3.X;
                    pt3.X = pt2.X;
                    pt2.X = pt4.X;
                    pt4.X = swp;
                    swp = pt3.Z;
                    pt3.Z = pt2.Z;
                    pt2.Z = pt4.Z;
                    pt4.Z = swp;

                }
                else if ((d + e) % 3 == 0)
                {
                    d = (d - 2 * e) / 3;

                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt1, pt2, pt3, ref pt4);        // T = A + B (C)


                    d2 = uecm_submod(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod(pt1.X, pt1.Z, n);
                    uecm_uadd(rho, n, pt4, pt1, pt2, ref pt2);        // B = T + A (B)
                    uecm_udup(s, rho, n, s2, d2, ref pt4);        // T = 2A
                    uecm_uadd(rho, n, pt1, pt4, pt1, ref pt1);        // A = A + T (A) = 3A
                }
                else if ((d - e) % 3 == 0)
                {
                    d = (d - e) / 3;

                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt1, pt2, pt3, ref pt4);        // T = A + B (C)

                    d2 = uecm_submod(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod(pt1.X, pt1.Z, n);
                    uecm_uadd(rho, n, pt3, pt1, pt2, ref pt3);        // C = C + A (B)

                    swp = pt2.X;
                    pt2.X = pt4.X;
                    pt4.X = swp;
                    swp = pt2.Z;
                    pt2.Z = pt4.Z;
                    pt4.Z = swp;

                    uecm_udup(s, rho, n, s2, d2, ref pt4);        // T = 2A
                    uecm_uadd(rho, n, pt1, pt4, pt1, ref pt1);        // A = A + T (A) = 3A
                }
                else
                {
                    e /= 2;

                    d2 = uecm_submod(pt2.X, pt2.Z, n);
                    s2 = uecm_addmod(pt2.X, pt2.Z, n);

                    uecm_uadd(rho, n, pt3, pt2, pt1, ref pt3);        // C = C + B (A)
                    uecm_udup(s, rho, n, s2, d2, ref pt2);        // B = 2B
                }
            }
            uecm_uadd(rho, n, pt1, pt2, pt3, ref P);     // A = A + B (C)

            for (i = 0; i < shift; i++)
            {
                d1 = uecm_submod(P.X, P.Z, n);
                s1 = uecm_addmod(P.X, P.Z, n);
                uecm_udup(s, rho, n, s1, d1, ref P);     // P = 2P
            }
            return;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void uecm_uadd(uint64_t rho, uint64_t n, uecm_pt P1, uecm_pt P2,
            uecm_pt Pin, ref uecm_pt Pout)
        {
            // compute:
            //x+ = z- * [(x1-z1)(x2+z2) + (x1+z1)(x2-z2)]^2
            //z+ = x- * [(x1-z1)(x2+z2) - (x1+z1)(x2-z2)]^2
            // where:
            //x- = original x
            //z- = original z
            // given the sums and differences of the original points
            uint64_t diff1 = uecm_submod(P1.X, P1.Z, n);
            uint64_t sum1 = uecm_addmod(P1.X, P1.Z, n);
            uint64_t diff2 = uecm_submod(P2.X, P2.Z, n);
            uint64_t sum2 = uecm_addmod(P2.X, P2.Z, n);

            uint64_t tt1 = uecm_mulredc(diff1, sum2, n, rho); //U
            uint64_t tt2 = uecm_mulredc(sum1, diff2, n, rho); //V

            uint64_t tt3 = uecm_addmod(tt1, tt2, n);
            uint64_t tt4 = uecm_submod(tt1, tt2, n);
            tt1 = uecm_sqrredc(tt3, n, rho);   //(U + V)^2
            tt2 = uecm_sqrredc(tt4, n, rho);   //(U - V)^2

            uint64_t tmpx = uecm_mulredc(tt1, Pin.Z, n, rho);     //Z * (U + V)^2
            uint64_t tmpz = uecm_mulredc(tt2, Pin.X, n, rho);     //x * (U - V)^2
            Pout.X = tmpx;
            Pout.Z = tmpz;

# if MICRO_ECM_VERBOSE_PRINTF
            stg1Add++;
#endif
            return;
        }


        static void uecm_uprac70(uint64_t rho, uint64_t n, ref uecm_pt P, uint64_t s)
        {
            uint64_t s1, s2, d1, d2;
            uint64_t swp;
            int i;
            int[] steps = new[] {
        0,6,0,6,0,6,0,4,6,0,4,6,0,4,4,6,
        0,4,4,6,0,5,4,6,0,3,3,4,6,0,3,5,
        4,6,0,3,4,3,4,6,0,5,5,4,6,0,5,3,
        3,4,6,0,3,3,4,3,4,6,0,5,3,3,3,3,
        3,3,3,3,4,3,3,4,6,0,5,4,3,3,4,6,
        0,3,4,3,5,4,6,0,5,3,3,3,4,6,0,5,
        4,3,5,4,6,0,5,5,3,3,4,6,0,4,3,3,
        3,5,4,6 };

            uecm_pt pt1 = new(), pt2 = new(), pt3 = new();
            for (i = 0; i < 116; i++)
            {
                if (steps[i] == 0)
                {
                    pt1.X = pt2.X = pt3.X = P.X;
                    pt1.Z = pt2.Z = pt3.Z = P.Z;

                    d1 = uecm_submod(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod(pt1.X, pt1.Z, n);
                    uecm_udup(s, rho, n, s1, d1, ref pt1);
                }
                else if (steps[i] == 3)
                {
                    // integrate step 4 followed by swap(1,2)
                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

                    swp = pt1.X;
                    pt1.X = pt4.X;
                    pt4.X = pt3.X;
                    pt3.X = pt2.X;
                    pt2.X = swp;
                    swp = pt1.Z;
                    pt1.Z = pt4.Z;
                    pt4.Z = pt3.Z;
                    pt3.Z = pt2.Z;
                    pt2.Z = swp;
                }
                else if (steps[i] == 4)
                {
                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

                    swp = pt2.X;
                    pt2.X = pt4.X;
                    pt4.X = pt3.X;
                    pt3.X = swp;
                    swp = pt2.Z;
                    pt2.Z = pt4.Z;
                    pt4.Z = pt3.Z;
                    pt3.Z = swp;
                }
                else if (steps[i] == 5)
                {
                    d2 = uecm_submod(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod(pt1.X, pt1.Z, n);

                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt2);        // B = B + A (C)
                    uecm_udup(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (steps[i] == 6)
                {
                    uecm_uadd(rho, n, pt1, pt2, pt3, ref P);     // A = A + B (C)
                }
            }
            return;
        }

        static void uecm_uprac85(uint64_t rho, uint64_t n, ref uecm_pt P, uint64_t s)
        {
            uint64_t s1, s2, d1, d2;
            uint64_t swp;
            int i;
            int[] steps = new[] {
                    0,6,0,6,0,6,0,6,0,4,
                    6,0,4,6,0,4,4,6,0,4,
                    4,6,0,5,4,6,0,3,3,4,
                    6,0,3,5,4,6,0,3,4,3,
                    4,6,0,5,5,4,6,0,5,3,
                    3,4,6,0,3,3,4,3,4,6,
                    0,4,3,4,3,5,3,3,3,3,
                    3,3,3,3,4,6,0,3,3,3,
                    3,3,3,3,3,3,4,3,4,3,
                    4,6,0,3,4,3,5,4,6,0,
                    5,3,3,3,4,6,0,5,4,3,
                    5,4,6,0,4,3,3,3,5,4,
                    6,0,4,3,5,3,3,4,6,0,
                    3,3,3,3,5,4,6,0,3,3,
                    3,4,3,3,4,6 };

            uecm_pt pt1 = new(), pt2 = new(), pt3 = new();
            for (i = 0; i < 146; i++)
            {
                if (steps[i] == 0)
                {
                    pt1.X = pt2.X = pt3.X = P.X;
                    pt1.Z = pt2.Z = pt3.Z = P.Z;

                    d1 = uecm_submod(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod(pt1.X, pt1.Z, n);
                    uecm_udup(s, rho, n, s1, d1, ref pt1);
                }
                else if (steps[i] == 3)
                {
                    // integrate step 4 followed by swap(1,2)
                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

                    swp = pt1.X;
                    pt1.X = pt4.X;
                    pt4.X = pt3.X;
                    pt3.X = pt2.X;
                    pt2.X = swp;
                    swp = pt1.Z;
                    pt1.Z = pt4.Z;
                    pt4.Z = pt3.Z;
                    pt3.Z = pt2.Z;
                    pt2.Z = swp;
                }
                else if (steps[i] == 4)
                {
                    uecm_pt pt4 = new();
                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

                    swp = pt2.X;
                    pt2.X = pt4.X;
                    pt4.X = pt3.X;
                    pt3.X = swp;
                    swp = pt2.Z;
                    pt2.Z = pt4.Z;
                    pt4.Z = pt3.Z;
                    pt3.Z = swp;
                }
                else if (steps[i] == 5)
                {
                    d2 = uecm_submod(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod(pt1.X, pt1.Z, n);

                    uecm_uadd(rho, n, pt2, pt1, pt3, ref pt2);        // B = B + A (C)
                    uecm_udup(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (steps[i] == 6)
                {
                    uecm_uadd(rho, n, pt1, pt2, pt3, ref P);     // A = A + B (C)
                }
            }
            return;
        }


        static unsafe uint64_t uecm_stage2(ref uecm_pt P, uint64_t rho, uint64_t n, uint32_t stg1_max, uint64_t s, uint64_t unityval)
        {
            int b;
            int i, j, k;
            uecm_pt Pa1 = new();
            ref uecm_pt Pa = ref Pa1;
            uecm_pt[] Pb = new uecm_pt[18];
            uecm_pt Pd1 = new();
            ref uecm_pt Pd = ref Pd1;
            int[] barray = null!;
            int numb = 0;

# if MICRO_ECM_VERBOSE_PRINTF
            ptadds = 0;
            stg1Doub = 0;
            stg1Add = 0;
#endif

            // this function has been written for MICRO_ECM_PARAM_D of 60, so you
            // probably don't want to change it.
            const int MICRO_ECM_PARAM_D = 60;

            //stage 2 init
            //Q = P = result of stage 1
            //compute [d]Q for 0 < d <= MICRO_ECM_PARAM_D

            uint64_t[] Pbprod = new uint64_t[18];

            // [1]Q
            Pb[1] = P;

            Pbprod[1] = uecm_mulredc(Pb[1].X, Pb[1].Z, n, rho);

            // [2]Q
            uint64_t diff1 = uecm_submod(P.X, P.Z, n);
            uint64_t sum1 = uecm_addmod(P.X, P.Z, n);
            uecm_udup(s, rho, n, sum1, diff1, ref Pb[2]);
            //    Pbprod[2] = uecm_mulredc(Pb[2].X, Pb[2].Z, n, rho);    // never used

            /*
            Let D = MICRO_ECM_PARAM_D.

            D is small in tinyecm, so it is straightforward to just enumerate the needed
            points.  We can do it efficiently with two progressions mod 6.
            Pb[0] = scratch
            Pb[1] = [1]Q;
            Pb[2] = [2]Q;
            Pb[3] = [7]Q;   prog2
            Pb[4] = [11]Q;  prog1
            Pb[5] = [13]Q;  prog2
            Pb[6] = [17]Q;  prog1
            Pb[7] = [19]Q;  prog2
            Pb[8] = [23]Q;  prog1
            Pb[9] = [29]Q;  prog1
            Pb[10] = [30 == D]Q;   // shouldn't this be [31]Q?
            Pb[11] = [31]Q; prog2   // shouldn't this be [37]Q?
            Pb[12] = [37]Q; prog2   // shouldn't this be [41]Q?
            Pb[13] = [41]Q; prog1   // [43]Q?
            Pb[14] = [43]Q; prog2   // [47]Q?
            Pb[15] = [47]Q; prog1   // [49]Q?
            Pb[16] = [49]Q; prog2   // [53]Q?
            Pb[17] = [53]Q; prog1   // [59]Q?
            // Pb[18] = [59]Q; prog1   // [60]Q?   note: we can remove this line I believe.  Pb[18] is never set, and never used.  Therefore I changed the definition of Pb above to have only 18 elements.

            two progressions with total of 17 adds to get 15 values of Pb.
            6 + 5(1) -> 11 + 6(5) -> 17 + 6(11) -> 23 + 6(17) -> 29 + 6(23) -> 35 + 6(29) -> 41 + 6(35) -> 47 + 6(41) -> 53 + 6(47) -> 59
            6 + 1(5) -> 7  + 6(1) -> 13 + 6(7)  -> 19 + 6(13) -> 25 + 6(19) -> 31 + 6(25) -> 37 + 6(31) -> 43 + 6(37) -> 49

            we also need [2D]Q = [60]Q
            to get [60]Q we just need one more add:
            compute [60]Q from [31]Q + [29]Q([2]Q), all of which we
            have after the above progressions are computed.

            we also need [A]Q = [((B1 + D) / (2D) * 2 D]Q
            which is equal to the following for various common B1:
            B1      [x]Q
            65      [120]Q
            85      [120]Q
            125     [180]Q      note: according to the A[Q] formula above, wouldn't this be [120]Q?  ( I suspect maybe the formula is supposed to be [((B1 + D) / D) * D]Q )
            165     [180]Q      note: same as above.
            205     [240]Q

            and we need [x-D]Q as well, for the above [x]Q.
            So far we are getting [x]Q and [x-2D]Q each from prac(x,Q).
            There is a better way using progressions of [2D]Q
            [120]Q = 2*[60]Q
            [180]Q = [120]Q + [60]Q([60]Q)
            [240]Q = 2*[120]Q
            [300]Q = [240]Q + [60]Q([180]Q)
            ...
            etc.

            */

            uecm_pt pt5 = new(), pt6 = new();

            // Calculate all Pb: the following is specialized for MICRO_ECM_PARAM_D=60
            // [2]Q + [1]Q([1]Q) = [3]Q
            uecm_uadd(rho, n, Pb[1], Pb[2], Pb[1], ref Pb[3]);        // <-- temporary

            // 2*[3]Q = [6]Q
            diff1 = uecm_submod(Pb[3].X, Pb[3].Z, n);
            sum1 = uecm_addmod(Pb[3].X, Pb[3].Z, n);
            uecm_udup(s, rho, n, sum1, diff1, ref pt6);   // pt6 = [6]Q

            // [3]Q + [2]Q([1]Q) = [5]Q
            uecm_uadd(rho, n, Pb[3], Pb[2], Pb[1], ref pt5);    // <-- pt5 = [5]Q
            Pb[3] = pt5;

            // [6]Q + [5]Q([1]Q) = [11]Q
            uecm_uadd(rho, n, pt6, pt5, Pb[1], ref Pb[4]);    // <-- [11]Q

            i = 3;
            k = 4;
            j = 5;
            while ((j + 12) < MICRO_ECM_PARAM_D)
            {
                // [j+6]Q + [6]Q([j]Q) = [j+12]Q
                uecm_uadd(rho, n, pt6, Pb[k], Pb[i], ref Pb[map[j + 12]]);
                i = k;
                k = (int)map[j + 12];
                j += 6;
            }

            // [6]Q + [1]Q([5]Q) = [7]Q
            uecm_uadd(rho, n, pt6, Pb[1], pt5, ref Pb[3]);    // <-- [7]Q
            i = 1;
            k = 3;
            j = 1;
            while ((j + 12) < MICRO_ECM_PARAM_D)
            {
                // [j+6]Q + [6]Q([j]Q) = [j+12]Q
                uecm_uadd(rho, n, pt6, Pb[k], Pb[i], ref Pb[map[j + 12]]);
                i = k;
                k = (int) map[j + 12];
                j += 6;
            }

            // Pd = [2w]Q
            // [31]Q + [29]Q([2]Q) = [60]Q
            uecm_uadd(rho, n, Pb[9], Pb[10], Pb[2], ref Pd);   // <-- [60]Q

# if MICRO_ECM_VERBOSE_PRINTF
            ptadds++;
#endif

            // temporary - make [4]Q
            uecm_pt pt4 = new();
            diff1 = uecm_submod(Pb[2].X, Pb[2].Z, n);
            sum1 = uecm_addmod(Pb[2].X, Pb[2].Z, n);
            uecm_udup(s, rho, n, sum1, diff1, ref pt4);   // pt4 = [4]Q


            // make all of the Pbprod's
            for (i = 3; i < 18; i++)
            {
                Pbprod[i] = uecm_mulredc(Pb[i].X, Pb[i].Z, n, rho);
            }


            //initialize info needed for giant step
            uecm_pt Pad = new();

            // Pd = [w]Q
            // [17]Q + [13]Q([4]Q) = [30]Q
            uecm_uadd(rho, n, Pb[map[17]], Pb[map[13]], pt4, ref Pad);    // <-- [30]Q

            // [60]Q + [30]Q([30]Q) = [90]Q
            // C CODE:  uecm_uadd(rho, n,  *Pd, Pad, Pad, Pa);
            uecm_uadd(rho, n, Pd, Pad, Pad, ref Pa);

            uecm_pt pt90 = Pa;   // set pt90 = [90]Q
            uecm_pt pt60 = Pd;   // set pt60 = [60]Q

            // [90]Q + [30]Q([60]Q) = [120]Q
            // C:  uecm_uadd(rho, n, *Pa, Pad, *Pd, Pd);
            uecm_uadd(rho, n, Pa, Pad, Pd, ref Pd);

            // [120]Q + [30]Q([90]Q) = [150]Q
            // C:  uecm_uadd(rho, n, *Pd, Pad, *Pa, Pa);
            uecm_uadd(rho, n, Pd, Pad, Pa, ref Pa);


            //initialize accumulator
            uint64_t acc = unityval;

            // adjustment of Pa and Pad for particular B1.
            // Currently we have Pa=150, Pd=120, Pad=30

            if (stg1_max < 70)
            {
                // first process the appropriate b's with A=90
                int[] steps27 = { 59, 53, 49, 47, 43, 37, 31, 29, 23, 19, 17, 11, 7, 1, 13, 41 };
                int[] steps47 = { 43, 37, 31, 29, 23, 19, 17, 11, 7, 1, 13, 41, 47, 49, 59 };
                int[] steps;
                int numsteps;
                if (stg1_max == 27)
                {
                    steps = steps27;
                    numsteps = 16;
                }
                else // if (stg1_max == 47)
                {
                    steps = steps47;
                    numsteps = 15;
                }

                uint64_t pt90prod = uecm_mulredc(pt90.X, pt90.Z, n, rho);

                for (i = 0; i < numsteps; i++)
                {
                    b = steps[i];
                    // accumulate the cross product  (zimmerman syntax).
                    // page 342 in Cref P
                    uint64_t tt1 = uecm_submod(pt90.X, Pb[map[b]].X, n);
                    uint64_t tt2 = uecm_addmod(pt90.Z, Pb[map[b]].Z, n);
                    uint64_t tt3 = uecm_mulredc(tt1, tt2, n, rho);
                    tt1 = uecm_addmod(tt3, Pbprod[map[b]], n);
                    tt2 = uecm_submod(tt1, pt90prod, n);

                    uint64_t tmp = uecm_mulredc(acc, tt2, n, rho);
                    if (tmp == 0)
                        break;
                    acc = tmp;
                }
            }
            else if (stg1_max == 70)
            {
                // first process these b's with A=120
                int[] steps = { 49, 47, 41, 37, 31, 23, 19, 17, 13, 11, 7, 29, 43, 53, 59 };
                // we currently have Pd=120

                uint64_t pdprod = uecm_mulredc(Pd.X, Pd.Z, n, rho);

                for (i = 0; i < 15; i++)
                {
                    b = steps[i];
                    // accumulate the cross product  (zimmerman syntax).
                    // page 342 in Cref P
                    uint64_t tt1 = uecm_submod(Pd.X, Pb[map[b]].X, n);
                    uint64_t tt2 = uecm_addmod(Pd.Z, Pb[map[b]].Z, n);
                    uint64_t tt3 = uecm_mulredc(tt1, tt2, n, rho);
                    tt1 = uecm_addmod(tt3, Pbprod[map[b]], n);
                    tt2 = uecm_submod(tt1, pdprod, n);

                    uint64_t tmp = uecm_mulredc(acc, tt2, n, rho);
                    if (tmp == 0)
                        break;
                    acc = tmp;
                }
            }
            else if (stg1_max == 165)
            {
                // Currently we have Pa=150, Pd=120, Pad=30,  and pt60=60, pt90=90
                // Need Pa = 180, Pd = 120, Pad = 60
                // either of these should be fine
#if false
        // [150]Q + [30]Q([120]Q) = [180]Q
        uecm_uadd(rho, n, *Pa, Pad, *Pd, Pa);
#else
                diff1 = uecm_submod(pt90.X, pt90.Z, n);
                sum1 = uecm_addmod(pt90.X, pt90.Z, n);
                uecm_udup(s, rho, n, sum1, diff1, ref Pa);
#endif
                Pad = pt60;
                // have pa = 180, pd = 120, pad = 60
            }
            else if (stg1_max == 205)
            {
                // Currently we have Pa=150, Pd=120, Pad=30,  and pt60=60, pt90=90
                // need Pa = 210, Pd = 120, Pad = 90

                // [120]Q + [90]Q([30]Q) = [210]Q
                uecm_uadd(rho, n, Pd, pt90, Pad, ref Pa);

                Pad = pt90;
            }

            //initialize Paprod
            uint64_t Paprod = uecm_mulredc(Pa.X, Pa.Z, n, rho);

            if (stg1_max == 27)
            {
                barray = b1_27;
                numb = numb1_27;
            }
            else if (stg1_max == 47)
            {
                barray = b1_47;
                numb = numb1_47;
            }
            else if (stg1_max <= 70)
            {
                barray = b1_70;
                numb = numb1_70;
            }
            else if (stg1_max == 85)
            {
                barray = b1_85;
                numb = numb1_85;
            }
            else if (stg1_max == 125)
            {
                barray = b1_125_x50;
                numb = numb1_125_x50;
            }
            else if (stg1_max == 165)
            {
                barray = b1_165_x50;
                numb = numb1_165_x50;
            }
            else // if (stg1_max == 205)
            {
                barray = b1_205_x50;
                numb = numb1_205_x50;
            }

            for (i = 0; i < numb; i++)
            {
                if (barray[i] == 0)
                {
                    //giant step - use the addition formula for ECM
                    uecm_pt point = Pa;

                    //Pa + Pd
                    uecm_uadd(rho, n, Pa, Pd, Pad, ref Pa);

                    //Pad holds the previous Pa
                    Pad = point;

                    //and Paprod
                    Paprod = uecm_mulredc(Pa.X, Pa.Z, n, rho);

                    i++;
                }

                //we accumulate XrZd - XdZr = (Xr - Xd) * (Zr + Zd) + XdZd - XrZr
                //in CP notation, Pa -> (Xr,Zr), Pb -> (Xd,Zd)

                b = barray[i];
                // accumulate the cross product  (zimmerman syntax).
                // page 342 in Cref P
                uint64_t tt1 = uecm_submod(Pa.X, Pb[map[b]].X, n);
                uint64_t tt2 = uecm_addmod(Pa.Z, Pb[map[b]].Z, n);
                uint64_t tt3 = uecm_mulredc(tt1, tt2, n, rho);
                tt1 = uecm_addmod(tt3, Pbprod[map[b]], n);
                tt2 = uecm_submod(tt1, Paprod, n);

                uint64_t tmp = uecm_mulredc(acc, tt2, n, rho);
                if (tmp == 0)
                    break;
                acc = tmp;
            }

            return acc;
        }


        static readonly uint32_t[] map = {
            0, 1, 2, 0, 0, 0, 0, 3, 0, 0,
            0, 4, 0, 5, 0, 0, 0, 6, 0, 7,
            0, 0, 0, 8, 0, 0, 0, 0, 0, 9,
            0, 10, 0, 0, 0, 0, 0, 11, 0, 0,
            0, 12, 0, 13, 0, 0, 0, 14, 0, 15,
            0, 0, 0, 16, 0, 0, 0, 0, 0, 17 };

        const int numb1_27 = 76;
        static int[] b1_27 = {
            1,7,13,17,23,29,31,41,43,47,49,0,59,47,43,41,
            37,31,29,19,13,7,1,11,23,0,59,53,43,41,37,31,
            23,17,11,7,1,19,29,49,0,53,49,47,43,31,23,19,
            11,7,1,13,37,59,0,59,53,43,37,31,29,23,17,13,
            11,1,47,0,59,49,41,31,23,17,11,7 };

        const int numb1_47 = 121;
        static int[] b1_47 = {
            1,7,13,17,23,29,31,41,43,47,49,0,59,47,43,41,
            37,31,29,19,13,7,1,11,23,0,59,53,43,41,37,31,
            23,17,11,7,1,19,29,49,0,53,49,47,43,31,23,19,
            11,7,1,13,37,59,0,59,53,43,37,31,29,23,17,13,
            11,1,47,0,59,49,41,31,23,17,11,7,1,19,37,47,
            0,59,49,47,43,41,31,17,13,11,7,37,0,53,49,43,
            37,23,19,13,7,1,29,31,41,59,0,59,49,47,41,23,
            19,17,13,7,1,43,53,0,59 };

        const int numb1_70 = 175;
        static int[] b1_70 = {
            31,41,43,47,49,0,59,47,43,41,37,31,29,19,13,7,
            1,11,23,0,59,53,43,41,37,31,23,17,11,7,1,19,
            29,49,0,53,49,47,43,31,23,19,11,7,1,13,37,59,
            0,59,53,43,37,31,29,23,17,13,11,1,47,0,59,49,
            41,31,23,17,11,7,1,19,37,47,0,59,49,47,43,41,
            31,17,13,11,7,37,0,53,49,43,37,23,19,13,7,1,
            29,31,41,59,0,59,49,47,41,23,19,17,13,7,1,43,
            53,0,59,49,43,37,29,17,13,7,1,19,47,53,0,59,
            53,49,47,43,31,29,23,11,17,0,47,43,41,37,31,23,
            19,17,11,1,13,29,53,0,59,47,41,37,31,23,19,11,
            7,17,29,0,53,47,43,41,17,13,11,1,23,31,37 };

        const int numb1_85 = 225;
        static int[] b1_85 = {
            1,53,49,47,43,41,37,23,19,13,11,1,7,17,29,31,0,59,47,43,41,37,31,29,19,13,7,1,11,23,0,59,53,43,41,37,
            31,23,17,11,7,1,19,29,49,0,53,49,47,43,31,23,19,11,7,1,13,37,59,0,59,53,43,37,31,29,23,17,13,11,1,47,
            0,59,49,41,31,23,17,11,7,1,19,37,47,0,59,49,47,43,41,31,17,13,11,7,37,0,53,49,43,37,23,19,13,7,1,29,
            31,41,59,0,59,49,47,41,23,19,17,13,7,1,43,53,0,59,49,43,37,29,17,13,7,1,19,47,53,0,59,53,49,47,43,31,
            29,23,11,17,0,47,43,41,37,31,23,19,17,11,1,13,29,53,0,59,47,41,37,31,23,19,11,7,17,29,0,53,47,43,41,
            17,13,11,1,23,31,37,49,0,53,47,43,41,29,19,7,1,17,31,37,49,59,0,49,43,37,19,17,1,23,29,47,53,0,59,53,
            43,41,31,17,7,1,11,13,19,29 };

        const int numb1_125 = 319;
        static int[] b1_125 = {
            23,19,13,11,1,7,17,29,31,0,59,47,43,41,37,31,29,19,13,7,1,11,23,0,59,53,43,41,37,31,23,17,11,7,1,19,
            29,49,0,53,49,47,43,31,23,19,11,7,1,13,37,59,0,59,53,43,37,31,29,23,17,13,11,1,47,0,59,49,41,31,23,
            17,11,7,1,19,37,47,0,59,49,47,43,41,31,17,13,11,7,37,0,53,49,43,37,23,19,13,7,1,29,31,41,59,0,59,49,
            47,41,23,19,17,13,7,1,43,53,0,59,49,43,37,29,17,13,7,1,19,47,53,0,59,53,49,47,43,31,29,23,11,17,0,47,
            43,41,37,31,23,19,17,11,1,13,29,53,0,59,47,41,37,31,23,19,11,7,17,29,0,53,47,43,41,17,13,11,1,23,31,
            37,49,0,53,47,43,41,29,19,7,1,17,31,37,49,59,0,49,43,37,19,17,1,23,29,47,53,0,59,53,43,41,31,17,7,1,
            11,13,19,29,0,59,53,49,47,37,29,11,13,17,23,31,0,59,43,41,37,29,23,17,13,1,31,47,0,59,53,49,47,41,37,
            31,19,13,7,11,17,29,43,0,47,29,19,11,7,1,41,43,59,0,53,49,37,23,13,11,7,1,17,19,29,41,43,59,0,59,49,
            41,37,23,13,1,7,11,29,43,47,53,0,59,53,49,31,23,13,7,1,17,29,43,47,0,59,31,29,19,11,7,37,49,53 };

        const int numb1_165 = 435;
        static int[] b1_165 = {
            13,7,1,11,17,19,31,43,47,49,53,59,0,59,49,43,
            37,31,29,23,19,17,7,11,13,47,53,0,53,47,41,37,
            31,23,19,11,1,13,29,43,59,0,53,49,41,37,31,19,
            17,1,7,23,29,47,59,0,59,53,47,43,41,29,19,17,
            13,7,1,23,31,49,0,53,47,41,37,29,23,19,11,7,
            17,31,43,49,59,0,47,43,41,37,23,19,17,13,7,11,
            29,53,0,53,49,43,37,29,23,11,7,1,13,19,31,41,
            0,53,49,47,43,37,31,23,17,11,13,41,0,59,47,43,
            37,31,29,23,11,1,17,19,41,0,59,53,19,13,7,1,
            29,43,47,49,0,53,49,47,41,29,19,17,13,11,7,1,
            23,31,43,59,0,53,49,41,37,23,19,13,11,7,1,17,
            43,47,0,47,43,41,31,19,17,7,1,13,37,49,0,59,
            49,37,29,13,1,7,11,17,19,41,47,53,0,49,47,31,
            29,7,1,13,17,19,23,37,59,0,47,37,31,19,17,13,
            11,1,29,41,43,53,0,59,41,17,13,7,1,19,23,31,
            47,49,53,0,59,53,47,43,31,29,7,1,11,17,37,41,
            49,0,49,43,37,23,19,13,1,7,17,0,59,49,41,37,
            31,29,23,1,11,13,53,0,53,43,41,37,29,23,17,13,
            11,7,1,19,31,49,0,53,43,31,29,23,19,17,1,13,
            37,41,59,0,53,43,37,31,23,13,1,17,29,59,0,59,
            49,41,37,23,19,11,1,7,29,0,59,43,17,13,11,1,
            7,23,29,37,41,49,0,49,47,43,41,29,1,7,13,19,
            23,31,59,0,59,49,47,31,29,13,7,37,41,43,0,49,
            41,29,23,13,11,7,1,17,19,31,43,53,0,53,47,43,
            37,29,23,17,1,11,13,31,41,49,59,0,53,47,41,19,
            13,11,1,17,23,43,0,53,49,47,37,23,19,11,7,17,
            29,31,43,0,53,31,19,17,13,7,1,29,37,59,0,49,
            47,41,29 };

        const int numb1_205 = 511;
        static int[] b1_205 = {
            1,23,41,0,59,53,49,47,37,23,19,17,13,1,7,29,43,0,53,49,41,31,29,19,17,11,7,1,13,37,59,0,49,47,29,23,
            13,7,1,17,31,37,43,0,59,49,47,43,37,31,29,17,13,7,1,11,19,53,0,59,53,49,41,37,23,13,1,11,17,19,29,43,
            47,0,53,49,47,43,23,19,11,1,7,17,37,41,0,59,53,41,37,31,29,19,17,11,1,13,43,47,0,53,47,41,19,17,7,1,
            11,23,31,43,59,0,59,53,41,31,13,11,7,1,17,29,37,0,49,43,37,29,11,1,13,17,19,23,41,0,59,49,47,43,41,37,
            31,19,7,1,13,23,29,53,0,53,49,43,41,37,31,29,23,13,7,17,19,47,59,0,49,47,37,29,23,17,11,7,13,19,31,41,
            53,0,59,43,29,23,19,17,13,11,1,41,0,59,37,31,23,17,13,11,7,1,19,29,43,53,0,49,47,43,41,31,19,17,1,7,11,
            13,23,0,47,43,37,29,13,11,7,1,17,19,23,31,59,0,59,37,31,29,23,19,13,1,7,11,41,47,53,0,53,49,43,31,23,
            17,13,41,59,0,59,53,31,19,17,1,7,11,23,37,47,49,0,59,53,47,43,41,37,31,23,19,17,11,1,0,59,53,49,47,31,
            17,13,7,1,11,29,37,0,53,43,31,17,13,7,1,29,41,49,0,53,49,41,29,23,11,7,1,19,31,47,0,47,43,41,29,23,19,
            7,1,11,49,0,59,31,29,23,17,11,7,1,13,41,43,0,59,43,37,17,1,7,11,13,19,41,49,0,59,53,43,41,37,31,29,23,
            13,11,1,47,0,59,53,47,31,19,17,13,1,7,11,29,37,43,49,0,49,43,41,31,17,13,7,11,23,37,53,0,53,49,41,23,
            19,13,11,7,1,17,37,59,0,49,47,43,37,31,29,23,1,7,41,0,59,43,41,37,31,17,13,11,7,47,49,0,59,49,47,37,31,
            29,19,17,7,1,0,53,47,37,19,13,1,11,31,41,0,49,47,37,23,17,13,11,7,19,31,53,0,59,53,47,29,13,11,7,1,23,
            41,0,49,47,41,37,19,11,13,17,23,29,31,43,0,59,29,19,13,1,41,43,47,53,0,59,53,43,41,37,23,17,11,7,1,13,
            29,49 };

        const int numb1_125_x50 = 645;
        static int[] b1_125_x50 = {
            23,19,13,11,1,7,17,29,31,41,43,47,49,0,59,47,
            43,41,37,31,29,19,13,7,1,11,23,0,59,53,43,41,
            37,31,23,17,11,7,1,19,29,49,0,53,49,47,43,31,
            23,19,11,7,1,13,37,59,0,59,53,43,37,31,29,23,
            17,13,11,1,47,0,59,49,41,31,23,17,11,7,1,19,
            37,47,0,59,49,47,43,41,31,17,13,11,7,37,0,53,
            49,43,37,23,19,13,7,1,29,31,41,59,0,59,49,47,
            41,23,19,17,13,7,1,43,53,0,59,49,43,37,29,17,
            13,7,1,19,47,53,0,59,53,49,47,43,31,29,23,11,
            17,0,47,43,41,37,31,23,19,17,11,1,13,29,53,0,
            59,47,41,37,31,23,19,11,7,17,29,0,53,47,43,41,
            17,13,11,1,23,31,37,49,0,53,47,43,41,29,19,7,
            1,17,31,37,49,59,0,49,43,37,19,17,1,23,29,47,
            53,0,59,53,43,41,31,17,7,1,11,13,19,29,0,59,
            53,49,47,37,29,11,13,17,23,31,0,59,43,41,37,29,
            23,17,13,1,31,47,0,59,53,49,47,41,37,31,19,13,
            7,11,17,29,43,0,47,29,19,11,7,1,41,43,59,0,
            53,49,37,23,13,11,7,1,17,19,29,41,43,59,0,59,
            49,41,37,23,13,1,7,11,29,43,47,53,0,59,53,49,
            31,23,13,7,1,17,29,43,47,0,59,31,29,19,11,7,
            37,49,53,0,41,31,29,13,17,19,37,53,59,0,53,49,
            41,19,17,13,11,1,29,31,37,43,59,0,59,47,43,31,
            29,19,17,1,23,0,53,49,47,43,41,19,11,1,7,17,
            23,29,31,37,0,59,49,47,37,23,17,13,7,1,29,41,
            43,0,59,53,49,41,31,23,17,11,19,29,43,47,0,49,
            47,37,23,19,17,7,11,41,53,59,0,59,47,43,23,1,
            11,13,17,29,31,37,0,59,53,37,31,19,17,11,1,23,
            29,43,47,49,0,53,29,19,13,11,1,23,31,41,43,59,
            0,53,23,13,11,1,7,41,47,59,0,49,47,29,23,19,
            13,7,11,37,43,53,0,43,41,29,23,7,1,13,31,47,
            49,53,59,0,59,53,47,37,31,19,7,11,13,23,41,49,
            0,47,43,41,37,31,29,17,13,1,59,0,47,41,31,19,
            17,13,7,1,23,37,43,49,53,59,0,59,49,47,31,19,
            11,7,17,29,37,43,0,43,37,23,19,11,1,7,41,47,
            0,49,37,31,29,13,7,1,23,41,0,49,43,37,31,23,
            17,13,11,1,7,19,41,47,53,0,49,47,43,31,29,23,
            19,7,13,41,0,47,31,29,23,19,17,13,11,1,41,0,
            53,49,47,41,11,7,1,17,23,31,37,59,0,59,53,49,
            43,41,31,29,13,7,17,0,49,43,23,19,1,7,13,17,
            37,59,0,59,49,37,29,19,17,7,1,13,23,47,53,0,
            59,53,49,41,23 };

        const int numb1_165_x50 = 830;
        static int[] b1_165_x50 = {
            13,7,1,11,17,19,31,43,47,49,53,59,0,59,49,43,
            37,31,29,23,19,17,7,11,13,47,53,0,53,47,41,37,
            31,23,19,11,1,13,29,43,59,0,53,49,41,37,31,19,
            17,1,7,23,29,47,59,0,59,53,47,43,41,29,19,17,
            13,7,1,23,31,49,0,53,47,41,37,29,23,19,11,7,
            17,31,43,49,59,0,47,43,41,37,23,19,17,13,7,11,
            29,53,0,53,49,43,37,29,23,11,7,1,13,19,31,41,
            0,53,49,47,43,37,31,23,17,11,13,41,0,59,47,43,
            37,31,29,23,11,1,17,19,41,0,59,53,19,13,7,1,
            29,43,47,49,0,53,49,47,41,29,19,17,13,11,7,1,
            23,31,43,59,0,53,49,41,37,23,19,13,11,7,1,17,
            43,47,0,47,43,41,31,19,17,7,1,13,37,49,0,59,
            49,37,29,13,1,7,11,17,19,41,47,53,0,49,47,31,
            29,7,1,13,17,19,23,37,59,0,47,37,31,19,17,13,
            11,1,29,41,43,53,0,59,41,17,13,7,1,19,23,31,
            47,49,53,0,59,53,47,43,31,29,7,1,11,17,37,41,
            49,0,49,43,37,23,19,13,1,7,17,0,59,49,41,37,
            31,29,23,1,11,13,53,0,53,43,41,37,29,23,17,13,
            11,7,1,19,31,49,0,53,43,31,29,23,19,17,1,13,
            37,41,59,0,53,43,37,31,23,13,1,17,29,59,0,59,
            49,41,37,23,19,11,1,7,29,0,59,43,17,13,11,1,
            7,23,29,37,41,49,0,49,47,43,41,29,1,7,13,19,
            23,31,59,0,59,49,47,31,29,13,7,37,41,43,0,49,
            41,29,23,13,11,7,1,17,19,31,43,53,0,53,47,43,
            37,29,23,17,1,11,13,31,41,49,59,0,53,47,41,19,
            13,11,1,17,23,43,0,53,49,47,37,23,19,11,7,17,
            29,31,43,0,53,31,19,17,13,7,1,29,37,59,0,49,
            47,41,29,13,11,7,1,17,19,37,0,59,49,43,41,31,
            29,19,17,7,1,11,13,23,37,0,53,43,41,31,23,17,
            7,11,29,0,59,53,49,43,37,19,17,7,13,23,47,0,
            59,53,37,29,23,17,1,19,31,43,0,49,37,19,17,11,
            7,43,47,53,59,0,59,47,43,29,1,11,17,49,0,49,
            47,43,37,29,23,13,11,7,19,31,41,59,0,49,41,23,
            19,13,1,7,47,53,0,53,49,41,31,23,11,7,13,17,
            59,0,59,43,37,31,17,7,11,41,47,53,0,53,47,43,
            41,29,23,19,17,11,59,0,59,53,49,23,17,11,7,1,
            43,0,59,53,49,47,43,41,31,17,11,7,1,37,0,41,
            37,29,19,13,7,1,23,31,47,49,59,0,59,43,37,17,
            13,1,41,47,0,53,49,31,23,17,13,7,19,29,41,0,
            59,49,47,37,29,17,7,19,23,31,41,0,53,43,37,31,
            29,23,13,1,11,17,59,0,59,53,47,41,31,23,1,7,
            29,49,0,59,49,19,11,7,13,23,29,31,37,41,0,53,
            41,23,7,1,13,19,29,31,43,49,59,0,47,43,19,17,
            1,11,13,23,49,53,0,59,43,37,31,29,17,1,7,11,
            47,49,0,59,53,49,43,37,29,23,19,7,1,0,37,31,
            19,13,11,47,53,0,53,49,47,41,31,23,17,13,7,37,
            0,59,49,47,31,29,11,13,37,53,0,49,43,41,23,19,
            13,11,1,7,17,29,37,47,59,0,59,47,43,37,31,29,
            17,13,1,19,23,49,53,0,59,53,49,41,37,23,17,13,
            1,19,0,43,37,31,19,7,13,17,23,41,47,59,0,53,
            47,43,31,29,17,13,37,59,0,47,41,31,19,13,11,7,
            1,17,23,0,59,53,49,41,29,11,1,13,17,23 };

        const int numb1_205_x50 = 1013;
        static int[] b1_205_x50 = {
            1,13,17,19,23,29,31,41,47,53,59,0,59,53,49,47,
            37,23,19,17,13,1,7,29,43,0,53,49,41,31,29,19,
            17,11,7,1,13,37,59,0,49,47,29,23,13,7,1,17,
            31,37,43,0,59,49,47,43,37,31,29,17,13,7,1,11,
            19,53,0,59,53,49,41,37,23,13,1,11,17,19,29,43,
            47,0,53,49,47,43,23,19,11,1,7,17,37,41,0,59,
            53,41,37,31,29,19,17,11,1,13,43,47,0,53,47,41,
            19,17,7,1,11,23,31,43,59,0,59,53,41,31,13,11,
            7,1,17,29,37,0,49,43,37,29,11,1,13,17,19,23,
            41,0,59,49,47,43,41,37,31,19,7,1,13,23,29,53,
            0,53,49,43,41,37,31,29,23,13,7,17,19,47,59,0,
            49,47,37,29,23,17,11,7,13,19,31,41,53,0,59,43,
            29,23,19,17,13,11,1,41,0,59,37,31,23,17,13,11,
            7,1,19,29,43,53,0,49,47,43,41,31,19,17,1,7,
            11,13,23,0,47,43,37,29,13,11,7,1,17,19,23,31,
            59,0,59,37,31,29,23,19,13,1,7,11,41,47,53,0,
            53,49,43,31,23,17,13,41,59,0,59,53,31,19,17,1,
            7,11,23,37,47,49,0,59,53,47,43,41,37,31,23,19,
            17,11,1,0,59,53,49,47,31,17,13,7,1,11,29,37,
            0,53,43,31,17,13,7,1,29,41,49,0,53,49,41,29,
            23,11,7,1,19,31,47,0,47,43,41,29,23,19,7,1,
            11,49,0,59,31,29,23,17,11,7,1,13,41,43,0,59,
            43,37,17,1,7,11,13,19,41,49,0,59,53,43,41,37,
            31,29,23,13,11,1,47,0,59,53,47,31,19,17,13,1,
            7,11,29,37,43,49,0,49,43,41,31,17,13,7,11,23,
            37,53,0,53,49,41,23,19,13,11,7,1,17,37,59,0,
            49,47,43,37,31,29,23,1,7,41,0,59,43,41,37,31,
            17,13,11,7,47,49,0,59,49,47,37,31,29,19,17,7,
            1,0,53,47,37,19,13,1,11,31,41,0,49,47,37,23,
            17,13,11,7,19,31,53,0,59,53,47,29,13,11,7,1,
            23,41,0,49,47,41,37,19,11,13,17,23,29,31,43,0,
            59,29,19,13,1,41,43,47,53,0,59,53,43,41,37,23,
            17,11,7,1,13,29,49,0,53,49,43,31,29,23,17,11,
            37,41,59,0,53,41,23,19,17,13,11,29,31,47,59,0,
            47,37,23,19,11,17,29,43,49,0,59,53,49,47,41,19,
            13,11,7,17,29,31,37,0,53,47,41,37,29,19,13,31,
            43,49,59,0,47,41,37,29,19,13,7,11,49,53,0,59,
            49,43,37,29,23,11,7,1,17,19,31,47,53,0,47,43,
            31,17,11,37,41,59,0,53,47,43,37,23,17,11,1,31,
            41,0,59,47,37,13,11,7,1,19,53,0,59,53,43,31,
            29,19,13,7,1,23,37,49,0,53,29,23,1,19,31,41,
            0,49,41,23,19,17,7,1,11,29,37,0,53,37,31,29,
            17,11,1,13,19,43,47,0,49,47,31,29,19,17,7,13,
            23,53,59,0,59,47,31,23,19,13,17,29,37,41,53,0,
            59,53,49,37,31,23,11,7,19,29,0,49,43,41,19,11,
            7,17,23,37,59,0,53,47,43,37,7,17,19,31,41,59,
            0,59,41,17,1,7,23,47,49,0,53,49,43,41,31,23,
            13,7,1,11,17,19,29,47,59,0,59,47,43,29,11,7,
            1,19,23,31,37,41,49,53,0,53,47,43,29,17,13,11,
            19,23,59,0,49,37,23,17,13,11,7,29,43,47,59,0,
            59,47,17,1,7,29,43,49,0,49,43,41,37,29,19,13,
            7,17,31,0,59,41,31,29,19,17,13,7,23,37,43,47,
            0,59,53,41,17,7,1,19,49,0,59,47,43,29,23,11,
            31,37,49,53,0,47,37,29,13,11,1,17,19,31,53,59,
            0,53,49,41,37,31,23,17,11,1,7,0,47,43,31,29,
            19,13,11,1,17,37,0,47,41,37,29,19,7,1,31,43,
            59,0,49,47,41,31,23,1,13,19,37,43,0,59,53,49,
            37,29,23,11,7,1,17,31,47,0,53,49,47,37,19,11,
            7,13,41,0,59,53,47,37,31,29,19,17,13,11,23,41,
            0,59,49,37,31,23,19,17,43,53,0,59,47,41,29,13,
            11,1,7,31,43,49,53,0,43,41,29,23,19,7,1,47,
            49,0,59,47,43,29,23,7,1,11,19,37,0,43,41,13,
            11,17,19,29,49,53,0,59,37,31,29,19,11,7,1,23,
            41,53,0,47,43 };

    }
    
}