#define DO_UPM1
#undef DO_UPM1
#define _MSC_VER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using uint64_t = System.UInt64;
using int64_t = System.Int64;
using uint32_t = System.Int32;
using uint8_t = System.Byte;
using MathGmp.Native;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    //major portions of this code ported from GMP-ECM
    public class Pm1
    {


        public Factorization FactorPm1(ulong n, int b1 = 0)
        {
            Factorization result = new();
            var p = Factor(n, b1);
            if (p == n || p < 2)
            {
                result.Add(n, 1);
            }
            else
            {
                result.Add(n / p, 1);
                result.Add(p, 1);
            }
            return result;
        }

        public unsafe static ulong Factor(ulong n, int b1 = 0)
        {

            var result = getfactor_upm1(n, b1);
            return result;
        }

        public static Factorization MicroPM1(uint64_t n, uint32_t B1)
        {
            Factorization result = new();
            var p = micropm1(n, B1, B1 * 25);
            if (p == n || p < 2)
            {
                result.Add(n, 1);
            }
            else
            {
                result.Add(n / p, 1);
                result.Add(p, 1);
            }
            return result;
        }


        // getfactor_upm1() returns 1 if unable to find a factor of q64,
        // Otherwise it returns a factor of q64.
        // 
        // if the input is known to have no small factors, set is_arbitrary=0, 
        // otherwise, set is_arbitrary=1 and a few curves targetting small factors
        // will be run prior to the standard sequence of curves for the input size.
        //  
        // Prior to your first call of getfactor_upm1(), set *pran = 0  (or set it to
        // some other arbitrary value); after that, don't change *pran.
        // FYI: *pran is used within this file by a random number generator, and it
        // holds the current value of a pseudo random sequence.  Your first assigment
        // to *pran seeds the sequence, and after seeding it you don't want to
        // change *pran, since that would restart the sequence.
        public static uint64_t getfactor_upm1(uint64_t q64, uint32_t b1)
        {
            uint64_t result = q64;
            if (q64 % 2 == 0)
                result = 2;
            else if (b1 > 0)
            {
                result = upm1_dispatch(q64, 0, b1);
            }
            else
            {
                int bits = upm1_get_bits(q64);
                result = upm1_dispatch(q64, bits, 0);
            }
            if (result < 2) result = q64;
            return result;
        }



        static uint64_t upm1_dispatch(uint64_t n, int targetBits, uint32_t b1)
        {
            int B1 = 333;

            //upm1_generate_window_plan(33);
            //upm1_generate_window_plan(100);
            //upm1_generate_window_plan(333);
            //upm1_generate_window_plan(666);
            //upm1_generate_window_plan(1000);
            //exit(0);

            if (b1 > 0)
            {
                return micropm1(n, b1, 25 * b1);
            }
            else
            {
                if (targetBits < 50) B1 = 100;
                else B1 = 333;

                return micropm1(n, B1, 25 * B1);
            }
        }


        static uint64_t micropm1(uint64_t n, uint32_t B1, uint32_t B2)
        {
            //attempt to factor n with the elliptic curve method
            //following brent and montgomery's papers, and CP's book
            int result;
            uint64_t stg1_res, f = 0, q;
            uint64_t tmp1 = 0;
            uint64_t rho = (uint64_t)0 - upm1_multiplicative_inverse(n);

            // Let R = 2^64.  We can see R%n ≡ (R-n)%n  (mod n)
            uint64_t unityval = ((uint64_t)0 - n) % n;   // unityval == R  (mod n)

            stg1_res = upm1_stage1(rho, n, unityval, B1);
            q = upm1_mulredc(1, stg1_res, n, rho);
            result = upm1_check_factor(q - 1, n, ref tmp1);

            if (result == 1)
            {
                f = tmp1;
            }
            else if (B2 > B1)
            {
                uint64_t stg2acc;
                if (B1 <= 33)
                    stg2acc = upm1_stage2(stg1_res, rho, n, B1, unityval);
                else
                    stg2acc = upm1_stage2_pair(stg1_res, rho, n, B1, unityval);

                q = upm1_mulredc(1, stg2acc, n, rho);
                result = upm1_check_factor(q, n, ref tmp1);

                if (result == 1)
                {
                    f = tmp1;
                }
            }

            return f;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int upm1_get_bits(ulong q64)
            => MathLib.BitLength(q64);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong _umulh(ulong m, ulong n)
        {
            ulong mN_hi = System.Math.BigMul(m, n, out ulong mN_lo);
            return mN_hi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong _umul128(ulong x, ulong y, ref ulong hi)
            => System.Math.BigMul(x, y, out hi);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        static int _trail_zcnt64(ulong value)
            => MathLib.TrailingZeroCount(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t upm1_mulredc(uint64_t x, uint64_t y, uint64_t n, uint64_t nhat)
        {
            return upm1_mulredc_alt(x, y, n, 0 - nhat);
        }

        //not original code return int64_t;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t upm1_submod(uint64_t a, uint64_t b, uint64_t n)
        {

            if (_subborrow_u64(0, a, b, out uint64_t r0))
                r0 += n;
            return r0;
        }

        //__MACHINEX64(unsigned char __cdecl _subborrow_u64(unsigned char, unsigned __int64, unsigned __int64, unsigned __int64 *))

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool _subborrow_u64(int borrow, ulong x, ulong y, out ulong result)
        {

            ulong temp = x - y;
            result = temp - (ulong)borrow;
            return temp < x;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t upm1_addmod(uint64_t x, uint64_t y, uint64_t n)
        {
#if false
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
        static uint64_t upm1_sqrredc(uint64_t x, uint64_t n, uint64_t nhat)
        {
            return upm1_mulredc_alt(x, x, n, 0 - nhat);
        }


        static int upm1_check_factor(uint64_t Z, uint64_t n, ref uint64_t f)
        {
            int status = 0;
            f = upm1_bingcd64(n, Z);

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

        static uint64_t upm1_expRL(uint64_t P, uint64_t n, uint64_t rho, uint64_t one, uint32_t m)
        {
            uint64_t s = P;
            P = one;

            while (m > 0)
            {
                if ((m & 1) == 1)
                    P = upm1_mulredc(P, s, n, rho);
                s = upm1_sqrredc(s, n, rho);
                m >>= 1;
            }
            return P;
        }

        /* --- The following two functions are written by Jeff Hurchalla, Copyright 2022 --- */

        // for this algorithm, see https://jeffhurchalla.com/2022/04/28/montgomery-redc-using-the-positive-inverse-mod-r/
        static uint64_t upm1_mulredc_alt(uint64_t x, uint64_t y, uint64_t N, uint64_t invN)
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
#if MICRO_PM1_ALT_MULREDC_USE_INLINE_ASM_X86 // && !defined(_MSC_VER)
    __asm__ (
        "subq %[mN_hi], %[tmp] \n\t"    /* tmp = T_hi + N - mN_hi */
        "subq %[mN_hi], %[T_hi] \n\t"   /* T_hi = T_hi - mN_hi */
        "cmovaeq %[T_hi], %[tmp] \n\t"  /* tmp = (T_hi >= mN_hi) ? T_hi : tmp */
        : [tmp]"+&r"(tmp), [T_hi]"+&r"(T_hi)
        : [mN_hi]"r"(mN_hi)
        : "cc");
    uint64_t result = tmp;
#else
            tmp = tmp - mN_hi;
            uint64_t result = T_hi - mN_hi;
            result = (T_hi < mN_hi) ? tmp : result;
#endif
            return result;
        }

        // for this algorithm, see https://jeffhurchalla.com/2022/04/25/a-faster-multiplicative-inverse-mod-a-power-of-2/
        static uint64_t upm1_multiplicative_inverse(uint64_t a)
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


        static uint64_t upm1_bingcd64(uint64_t u, uint64_t v)
            => upm1_bingcd64_mod(u, v);
        static uint64_t upm1_bingcd64_mod(uint64_t u, uint64_t v)
        {
            if (u == 0) return v;
            return MathUtil.Gcd(u, v);
        }
        //issue if v is small and u is larg this will never complete.

        static uint64_t upm1_bingcd64_subtract(uint64_t u, uint64_t v)
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
                    var vl = MathLib.Log(v0);
                    var diff = MathLib.Abs(ul - vl);
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

        /* --- end Hurchalla functions --- */

        static uint64_t upm1_stage1(uint64_t rho, uint64_t n, uint64_t one, int stg1)
        {
            int i;
            uint64_t P = one;
            uint64_t[] g = new uint64_t[16];

            g[0] = one;
            for (i = 1; i < 16; i++)
                g[i] = upm1_addmod(g[i - 1], g[i - 1], n);

            switch (stg1)
            {
                case 33:
                    for (i = 0; i < 12; i++)
                    {
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        if (ewin33[i] > 0) P = upm1_mulredc(P, g[ewin33[i]], n, rho);
                    }
                    break;
                case 100:
                    for (i = 0; i < 34; i++)
                    {
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        if (ewin100[i] > 0) P = upm1_mulredc(P, g[ewin100[i]], n, rho);
                    }
                    break;
                case 333:
                    for (i = 0; i < 119; i++)
                    {
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        if (ewin333[i] > 0) P = upm1_mulredc(P, g[ewin333[i]], n, rho);
                    }
                    break;
                case 666:
                    for (i = 0; i < 243; i++)
                    {
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        if (ewin666[i] > 0) P = upm1_mulredc(P, g[ewin666[i]], n, rho);
                    }
                    break;
                case 1000:
                    for (i = 0; i < 360; i++)
                    {
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        P = upm1_sqrredc(P, n, rho);
                        if (ewin1000[i] > 0) P = upm1_mulredc(P, g[ewin1000[i]], n, rho);
                    }
                    break;
            }
            return P;
        }

        static uint64_t upm1_stage2(uint64_t P, uint64_t rho, uint64_t n, uint32_t b1, uint64_t unityval)
        {
            int w = 60;
            uint64_t[] d = new uint64_t[32];
            uint64_t six, five, pw, pgiant;
            int i, j;
            uint64_t acc;
            uint32_t b2 = 25 * b1;

            d[1] = P;
            d[2] = upm1_sqrredc(P, n, rho);
            six = upm1_mulredc(P, d[2], n, rho);
            six = upm1_sqrredc(six, n, rho);          // P^6

            // 1, 7, 13, 19, 25, 31, 37, 43, 49
            // unnecessary powers will be mapped to scratch d[0].
            j = 1;
            while ((j + 6) < 50)
            {
                d[upm1_map[j + 6]] = upm1_mulredc(d[upm1_map[j]], six, n, rho);
                j += 6;
            }

            // 11, 17, 23, 29, 35, 41, 47, 53, 59
            // unnecessary powers will be mapped to scratch d[0].
            five = upm1_sqrredc(d[2], n, rho);
            five = upm1_mulredc(five, d[1], n, rho);
            d[upm1_map[11]] = upm1_mulredc(five, six, n, rho);
            j = 11;
            while ((j + 6) < 60)
            {
                d[upm1_map[j + 6]] = upm1_mulredc(d[upm1_map[j]], six, n, rho);
                j += 6;
            }

#if false
    b1 = 33;
    b2 = 25 * b1;
    // baby-steps, giant-steps
    printf("/* baby-steps, giant-steps for b1 = %u, b2 = %u, w = %u\n", b1, b2, w);
    i = 1;
    uint32_t p = primes[i];
    while (p < b1) p = primes[i++];
    uint32_t q = w;
    while (q < b1) q += w;
    printf("q0 = %u\n", q);
    printf("b1_20_steps[] = {\n");
    j = 0;
    while (p < b2)
    {
        if (p < q)
        {
            printf("%u, ", q - p);
            j++;
        }
        else
        {
            q += w;
            printf("0,\n");
            printf("%u, ", q - p);
            j += 2;
        }
        p = primes[i++];
    }
    printf("};\n %d entries */ \n", j);

    exit(1);
#endif

            pw = upm1_mulredc(d[upm1_map[59]], P, n, rho);      // assumes w=60
            pgiant = pw;
            i = w;
            while (i < b1)
            {
                pgiant = upm1_mulredc(pgiant, pw, n, rho);
                i += w;
            }

            acc = unityval;

            switch (b1)
            {
                case 33:

                    for (i = 0; i < num_b1_33_steps; i++)
                    {
                        if (b1_33_steps[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, pw, n, rho);
                            i++;
                        }
                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_33_steps[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;
                case 100:

                    for (i = 0; i < num_b1_100_steps; i++)
                    {
                        if (b1_100_steps[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, pw, n, rho);
                            i++;
                        }
                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_100_steps[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;
                case 333:

                    for (i = 0; i < num_b1_333_steps; i++)
                    {
                        if (b1_333_steps[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, pw, n, rho);
                            i++;
                        }
                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_333_steps[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;
            }

            return acc;
        }

        static uint64_t upm1_stage2_pair(uint64_t P, uint64_t rho, uint64_t n, uint32_t b1, uint64_t unityval)
        {
            int w = 60;
            uint64_t[] d = new uint64_t[32];
            uint64_t six, x12, xmid, x24, x25, x36, x60, x72, five, pw, pgiant;
            int i, j;
            uint64_t acc;
            uint32_t b2 = 25 * b1;

            // we accumulate f(vw) - f(u) where f(n) = n^2, so that
            // we can pair together primes vw+/-u
            // see: P. L. Montgomery, "Speeding the Pollard and Elliptic Curve Methods
            // of Factorization," Mathematics of Computation, Vol 48, No. 177, 1987
            // 
            // b^(n+h)^2 = b^(n^2 + 2h(n) + h^2) = (b^(n^2))*(b^(2hn))*(b^(h^2))

            // u=1, u^2=1, b^u^2 = P
            d[1] = P;
            d[2] = upm1_sqrredc(P, n, rho);
            six = upm1_mulredc(P, d[2], n, rho);
            six = upm1_sqrredc(six, n, rho);                // P^6
            x12 = upm1_sqrredc(six, n, rho);                // P^12
            x24 = upm1_sqrredc(x12, n, rho);                // P^24
            x36 = upm1_mulredc(x24, x12, n, rho);           // P^36
            x60 = upm1_mulredc(x24, x36, n, rho);           // P^60
            x72 = upm1_sqrredc(x36, n, rho);                // P^72
            five = upm1_sqrredc(d[2], n, rho);
            five = upm1_mulredc(five, d[1], n, rho);        // P^5
            x25 = upm1_mulredc(x24, d[1], n, rho);          // P^25


            // P^7^2 = P^(1+6)^2 = P^(1^2) * P^(12*1) * P^36
            // P^13^2 = P^(7+6)^2 = P^(7^2) * P^(12*7) * P^36
            // P^19^2 = P^(13+6)^2 = P^(13^2) * P^(12*13) * P^36
            // ...

            // 1, 7, 13, 19, 25, 31, 37, 43, 49
            // unnecessary powers will be mapped to scratch d[0].
            j = 1;
            xmid = x12;
            while ((j + 6) < 50)
            {
                d[upm1_map[j + 6]] = upm1_mulredc(d[upm1_map[j]], x36, n, rho);
                d[upm1_map[j + 6]] = upm1_mulredc(d[upm1_map[j + 6]], xmid, n, rho);
                xmid = upm1_mulredc(xmid, x72, n, rho);
                j += 6;
            }

            // P^11^2 = P^(5+6)^2 = P^(5^2) * P^(12*5) * P^36
            // P^17^2 = P^(11+6)^2 = P^(11^2) * P^(12*11) * P^36
            // P^23^2 = P^(17+6)^2 = P^(17^2) * P^(12*17) * P^36
            // ...

            // 11, 17, 23, 29, 35, 41, 47, 53, 59
            // unnecessary powers will be mapped to scratch d[0].
            d[upm1_map[11]] = upm1_mulredc(x25, x36, n, rho);
            d[upm1_map[11]] = upm1_mulredc(d[upm1_map[11]], x60, n, rho);
            xmid = upm1_mulredc(x60, x72, n, rho);
            j = 11;
            while ((j + 6) < 60)
            {
                d[upm1_map[j + 6]] = upm1_mulredc(d[upm1_map[j]], x36, n, rho);
                d[upm1_map[j + 6]] = upm1_mulredc(d[upm1_map[j + 6]], xmid, n, rho);
                xmid = upm1_mulredc(xmid, x72, n, rho);
                j += 6;
            }

            // P^(2w)^2, assumes w=60
            // P^(120^2) = P^(14440)
            pw = upm1_expRL(P, n, rho, unityval, 120 * 120);
            uint64_t x14400 = pw;
            uint64_t x240x120 = upm1_sqrredc(pw, n, rho);
            xmid = x240x120;
            // P^(240^2) = P^(120+120)^2 = P^(120^2) * P^(240*120) * P^(14400)
            // P^(360^2) = P^(240+120)^2 = P^(240^2) * P^(240*240) * P^(14400)
            // P^(480^2) = P^(360+120)^2 = P^(360^2) * P^(240*360) * P^(14400)
            // ...

#if false
            // baby-steps, giant-steps
            int bi;
            int b1s[4] = { 100, 333, 666, 1000 };
            for (bi = 0; bi < 4; bi++)
            {
                b1 = b1s[bi];
                b2 = b1 * 25;
                printf("/* baby-steps, giant-steps pairing for b1 = %u, b2 = %u, w = %d\n",
                    b1, b2, w);
                i = 1;
                uint32_t p = primes[i];
                while (p < b1) p = primes[i++];
                uint32_t q = 2 * w;
                while (q < b1) q += 2 * w;
                printf("q0 = %u\n", q);
                printf("b1_%d_pairs[] = {\n", b1);
                uint8_t pairs[60];
                for (j = 0; j < w; j++) pairs[j] = 0;
                j = 0;
                while (p < b2)
                {
                    if (p < q)
                    {
                        printf("%u, ", q - p);
                        pairs[q - p] = 1;
                        j++;
                        p = primes[i++];
                    }
                    else
                    {
                        // now on the other side of current giant step.
                        // pair anything not currently paired.
                        while ((p < b2) && ((p - q) < w))
                        {
                            if (pairs[p - q] == 0)
                            {
                                printf("%u, ", (p - q));
                                j++;
                            }
                            p = primes[i++];
                        }
                        if (p >= b2) break;

                        // next giant step, clear the pairs.
                        int k;
                        for (k = 0; k < w; k++) pairs[k] = 0;

                        q += (2 * w);
                        printf("0,\n");
                        j++;
                    }
                }
                printf("};\n %d entries */ \n", j);
            }


            exit(1);
#endif

            pgiant = pw;
            i = 2 * w;
            while (i < b1)
            {
                pgiant = upm1_mulredc(pgiant, x14400, n, rho);
                pgiant = upm1_mulredc(pgiant, xmid, n, rho);
                xmid = upm1_mulredc(xmid, x240x120, n, rho);
                i += 2 * w;
            }

            acc = unityval;
            switch (b1)
            {
                case 100:

                    for (i = 0; i < num_b1_100_pairs; i++)
                    {
                        if (b1_100_pairs[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, x14400, n, rho);
                            pgiant = upm1_mulredc(pgiant, xmid, n, rho);
                            xmid = upm1_mulredc(xmid, x240x120, n, rho);
                            i++;
                        }

                        // if we happen to pick up all factors of the input with this
                        // prime pair, then the modular reduction will become 0.
                        // testing for this is simple and allows us to avoid gcd == n.
                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_100_pairs[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;
                case 333:

                    for (i = 0; i < num_b1_333_pairs; i++)
                    {
                        if (b1_333_pairs[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, x14400, n, rho);
                            pgiant = upm1_mulredc(pgiant, xmid, n, rho);
                            xmid = upm1_mulredc(xmid, x240x120, n, rho);
                            i++;
                        }

                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_333_pairs[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;

                case 666:

                    for (i = 0; i < num_b1_666_pairs; i++)
                    {
                        if (b1_666_pairs[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, x14400, n, rho);
                            pgiant = upm1_mulredc(pgiant, xmid, n, rho);
                            xmid = upm1_mulredc(xmid, x240x120, n, rho);
                            i++;
                        }

                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_666_pairs[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;

                case 1000:

                    for (i = 0; i < num_b1_1000_pairs; i++)
                    {
                        if (b1_1000_pairs[i] == 0)
                        {
                            pgiant = upm1_mulredc(pgiant, x14400, n, rho);
                            pgiant = upm1_mulredc(pgiant, xmid, n, rho);
                            xmid = upm1_mulredc(xmid, x240x120, n, rho);
                            i++;
                        }

                        uint64_t tmp = upm1_mulredc(acc,
                            upm1_submod(pgiant, d[upm1_map[b1_1000_pairs[i]]], n), n, rho);
                        if (tmp == 0) break;
                        else acc = tmp;
                    }
                    break;
            }

            return acc;
        }




        static int[] ewin33 = { 8, 3, 5, 5, 9, 2, 7, 9, 7, 10, 10, 0 };

        static int[] ewin100 = { // 170 muls
            12, 12, 14, 3, 12, 7, 13, 6, 12, 0, 15, 4, 1, 8, 7, 3, 0, 14, 13, 6,
            5, 6, 15, 13, 0, 11, 0, 14, 13, 3, 8, 8, 12, 0 };

        static int[] ewin333 = { // 595 muls
            2, 5, 1, 6, 12, 5, 1, 5, 0, 15, 3, 13, 2, 4, 2, 0, 4, 13, 11, 9, 4, 5,
            4, 13, 7, 15, 0, 11, 10, 7, 5, 4, 7, 0, 14, 11, 12, 10, 12, 4, 11, 2,
            5, 2, 10, 10, 7, 3, 14, 11, 8, 0, 15, 2, 2, 3, 10, 11, 6, 9, 2, 8, 15,
            4, 12, 13, 14, 13, 0, 7, 3, 3, 12, 3, 9, 8, 4, 6, 15, 0, 3, 9, 11, 14,
            5, 7, 4, 4, 11, 14, 8, 6, 11, 14, 0, 12, 13, 4, 12, 12, 11, 6, 13, 0,
            10, 8, 13, 6, 13, 15, 2, 5, 14, 13, 11, 11, 15, 0, 0 };

        static int[] ewin666 = { // 1215 muls
            6, 15, 12, 1, 15, 8, 2, 2, 13, 7, 9, 2, 13, 0, 6, 10, 14, 1, 4, 14, 0,
            10, 9, 6, 0, 9, 11, 0, 11, 2, 5, 5, 12, 14, 9, 11, 7, 11, 7, 9, 2, 1, 1,
            2, 1, 9, 14, 3, 3, 1, 0, 2, 12, 15, 0, 10, 11, 14, 10, 9, 11, 0, 13, 6,
            5, 10, 7, 14, 7, 4, 5, 3, 11, 15, 0, 9, 1, 2, 12, 13, 0, 9, 6, 10, 14, 0,
            11, 8, 7, 4, 15, 10, 10, 6, 8, 11, 9, 8, 1, 2, 7, 4, 0, 9, 6, 7, 11, 6,
            14, 0, 1, 12, 8, 11, 13, 5, 4, 0, 13, 14, 2, 0, 8, 12, 2, 8, 3, 10, 14, 11,
            0, 15, 7, 9, 1, 7, 10, 3, 8, 11, 9, 8, 10, 9, 6, 0, 1, 13, 2, 8, 14, 0, 8,
            13, 0, 0, 6, 13, 5, 15, 4, 6, 3, 0, 8, 15, 3, 14, 12, 11, 5, 6, 14, 1, 4,
            6, 9, 14, 1, 0, 3, 15, 6, 8, 0, 0, 8, 4, 10, 0, 1, 9, 9, 15, 14, 14, 8,
            12, 14, 14, 13, 8, 6, 7, 6, 11, 9, 6, 7, 7, 6, 8, 15, 1, 15, 2, 13, 9, 5,
            7, 10, 10, 11, 11, 2, 15, 4, 3, 10, 6, 9, 2, 14, 8, 14, 5, 6, 2, 15, 12,
            10, 0, 0 };

        static int[] ewin1000 = { // 1800 muls
            3, 11, 15, 13, 0, 1, 12, 9, 9, 13, 3, 2, 15, 9, 11, 15, 2, 13, 7, 4, 7, 8,
            10, 1, 3, 7, 10, 11, 3, 9, 12, 4, 14, 5, 7, 0, 14, 9, 6, 0, 0, 12, 1, 10,
            3, 8, 6, 1, 4, 3, 12, 15, 5, 11, 14, 14, 5, 1, 4, 2, 9, 2, 0, 7, 7, 14, 2,
            12, 10, 3, 4, 0, 10, 9, 2, 2, 3, 2, 4, 2, 3, 12, 5, 11, 7, 11, 0, 14, 2,
            4, 2, 7, 11, 3, 5, 1, 6, 10, 0, 2, 1, 11, 0, 10, 11, 4, 10, 0, 1, 7, 14, 1,
            14, 7, 2, 5, 0, 6, 12, 0, 7, 15, 1, 12, 15, 15, 6, 1, 5, 13, 0, 12, 4, 8,
            12, 8, 13, 14, 5, 12, 2, 5, 15, 3, 8, 6, 12, 11, 14, 1, 14, 2, 8, 1, 3, 13,
            2, 1, 14, 10, 15, 14, 9, 15, 5, 1, 2, 14, 11, 11, 9, 8, 7, 10, 0, 11, 6,
            11, 3, 3, 11, 12, 15, 11, 7, 12, 0, 14, 11, 4, 7, 3, 10, 15, 13, 4, 6, 12,
            14, 11, 14, 13, 8, 8, 5, 9, 2, 15, 4, 1, 11, 7, 6, 11, 8, 6, 2, 1, 12, 10,
            12, 11, 3, 15, 1, 9, 1, 14, 5, 15, 4, 12, 3, 8, 10, 9, 7, 4, 15, 11, 9, 1,
            1, 13, 7, 9, 1, 13, 6, 9, 0, 11, 14, 15, 12, 9, 3, 15, 8, 14, 15, 10, 7,
            11, 15, 11, 15, 15, 14, 5, 15, 0, 1, 10, 2, 13, 15, 5, 3, 10, 14, 13, 11,
            7, 12, 4, 14, 1, 5, 14, 8, 10, 14, 3, 13, 12, 13, 4, 2, 9, 13, 2, 2, 14,
            11, 3, 7, 5, 9, 1, 7, 10, 5, 15, 14, 2, 4, 2, 4, 4, 4, 0, 5, 11, 0, 10, 5,
            1, 4, 13, 0, 13, 10, 5, 9, 6, 2, 7, 0, 10, 10, 9, 8, 14, 12, 8, 6, 0, 8, 5,
            14, 8, 9, 4, 12, 1, 7, 10, 0, 0 };

        static uint32_t[] upm1_map = {
            0, 1, 2, 0, 0, 0, 0, 3, 0, 0,
            0, 4, 0, 5, 0, 0, 0, 6, 0, 7,
            0, 0, 0, 8, 0, 0, 0, 0, 0, 9,
            0, 10, 0, 0, 0, 0, 0, 11, 0, 0,
            0, 12, 0, 13, 0, 0, 0, 14, 0, 15,
            0, 0, 0, 16, 0, 0, 0, 0, 0, 17 };



        /* baby-steps, giant-steps for b1 = 33, b2 = 825, w = 60
        q0 = 60 */
        const int num_b1_33_steps = 145;
        static uint8_t[] b1_33_steps = {
            23, 19, 17, 13, 7, 1, 0,
            59, 53, 49, 47, 41, 37, 31, 23, 19, 17, 13, 11, 7, 0,
            53, 49, 43, 41, 31, 29, 23, 17, 13, 7, 1, 0,
            59, 49, 47, 43, 41, 29, 17, 13, 11, 7, 1, 0,
            59, 49, 43, 37, 31, 29, 23, 19, 17, 7, 0,
            53, 49, 47, 43, 29, 23, 13, 11, 7, 1, 0,
            53, 47, 41, 37, 31, 23, 19, 11, 1, 0,
            59, 49, 47, 41, 37, 31, 23, 19, 17, 13, 1, 0,
            53, 49, 41, 37, 31, 19, 17, 0,
            59, 53, 43, 37, 31, 29, 23, 13, 7, 1, 0,
            59, 53, 47, 43, 41, 29, 19, 17, 13, 7, 1, 0,
            59, 47, 43, 37, 29, 19, 11, 1, 0,
            53, 47, 41, 37, 29, 23, 19, 11, 7, 0,
            53, 43, 31, 29, 19, 17};

        /* baby-steps, giant-steps for b1 = 100, w = 60
        q0 = 120 */
        const int num_b1_100_steps = 382;
        static uint8_t[] b1_100_steps = {
            19, 17, 13, 11, 7, 0,
            53, 49, 43, 41, 31, 29, 23, 17, 13, 7, 1, 0,
            59, 49, 47, 43, 41, 29, 17, 13, 11, 7, 1, 0,
            59, 49, 43, 37, 31, 29, 23, 19, 17, 7, 0,
            53, 49, 47, 43, 29, 23, 13, 11, 7, 1, 0,
            53, 47, 41, 37, 31, 23, 19, 11, 1, 0,
            59, 49, 47, 41, 37, 31, 23, 19, 17, 13, 1, 0,
            53, 49, 41, 37, 31, 19, 17, 0,
            59, 53, 43, 37, 31, 29, 23, 13, 7, 1, 0,
            59, 53, 47, 43, 41, 29, 19, 17, 13, 7, 1, 0,
            59, 47, 43, 37, 29, 19, 11, 1, 0,
            53, 47, 41, 37, 29, 23, 19, 11, 7, 0,
            53, 43, 31, 29, 19, 17, 13, 11, 1, 0,
            47, 43, 41, 37, 23, 19, 17, 13, 0,
            53, 49, 41, 31, 23, 19, 13, 7, 0,
            53, 49, 43, 37, 29, 23, 11, 7, 1, 0,
            59, 49, 47, 41, 31, 29, 19, 17, 11, 0,
            53, 49, 47, 43, 37, 31, 23, 17, 11, 0,
            49, 47, 37, 29, 19, 13, 7, 0,
            59, 47, 43, 37, 31, 29, 23, 11, 1, 0,
            43, 41, 37, 31, 29, 23, 19, 17, 13, 1, 0,
            59, 53, 19, 13, 7, 0,
            59, 41, 31, 17, 13, 11, 7, 1, 0,
            53, 49, 47, 41, 29, 19, 17, 13, 11, 7, 1, 0,
            49, 37, 29, 17, 11, 7, 1, 0,
            53, 49, 41, 37, 23, 19, 13, 11, 7, 1, 0,
            59, 53, 43, 23, 17, 13, 11, 0,
            47, 43, 41, 31, 19, 17, 7, 0,
            59, 53, 47, 41, 23, 17, 13, 11, 0,
            59, 49, 37, 29, 13, 0,
            59, 53, 49, 47, 43, 41, 31, 19, 13, 7, 0,
            49, 47, 31, 29, 7, 1, 0,
            53, 47, 43, 41, 37, 29, 23, 13, 11, 1, 0,
            47, 37, 31, 19, 17, 13, 11, 1, 0,
            49, 47, 31, 29, 23, 19, 17, 7, 0,
            59, 41, 17, 13, 7, 0,
            59, 43, 41, 37, 29, 13, 11, 7, 0,
            59, 53, 47, 43, 31, 29, 7, 1, 0,
            59, 53, 49, 43, 29, 23, 19, 17, 11, 7, 1, 0,
            49, 43, 37, 23, 19, 13, 1, 0,
            53, 47, 43};

        /* baby-steps, giant-steps for b1 = 333, w = 60
        q0 = 360
        */
        const int num_b1_333_steps = 1110;
        static uint8_t[] b1_333_steps = {
            23, 13, 11, 7, 1, 0,
            53, 47, 41, 37, 31, 23, 19, 11, 1, 0,
            59, 49, 47, 41, 37, 31, 23, 19, 17, 13, 1, 0,
            53, 49, 41, 37, 31, 19, 17, 0,
            59, 53, 43, 37, 31, 29, 23, 13, 7, 1, 0,
            59, 53, 47, 43, 41, 29, 19, 17, 13, 7, 1, 0,
            59, 47, 43, 37, 29, 19, 11, 1, 0,
            53, 47, 41, 37, 29, 23, 19, 11, 7, 0,
            53, 43, 31, 29, 19, 17, 13, 11, 1, 0,
            47, 43, 41, 37, 23, 19, 17, 13, 0,
            53, 49, 41, 31, 23, 19, 13, 7, 0,
            53, 49, 43, 37, 29, 23, 11, 7, 1, 0,
            59, 49, 47, 41, 31, 29, 19, 17, 11, 0,
            53, 49, 47, 43, 37, 31, 23, 17, 11, 0,
            49, 47, 37, 29, 19, 13, 7, 0,
            59, 47, 43, 37, 31, 29, 23, 11, 1, 0,
            43, 41, 37, 31, 29, 23, 19, 17, 13, 1, 0,
            59, 53, 19, 13, 7, 0,
            59, 41, 31, 17, 13, 11, 7, 1, 0,
            53, 49, 47, 41, 29, 19, 17, 13, 11, 7, 1, 0,
            49, 37, 29, 17, 11, 7, 1, 0,
            53, 49, 41, 37, 23, 19, 13, 11, 7, 1, 0,
            59, 53, 43, 23, 17, 13, 11, 0,
            47, 43, 41, 31, 19, 17, 7, 0,
            59, 53, 47, 41, 23, 17, 13, 11, 0,
            59, 49, 37, 29, 13, 0,
            59, 53, 49, 47, 43, 41, 31, 19, 13, 7, 0,
            49, 47, 31, 29, 7, 1, 0,
            53, 47, 43, 41, 37, 29, 23, 13, 11, 1, 0,
            47, 37, 31, 19, 17, 13, 11, 1, 0,
            49, 47, 31, 29, 23, 19, 17, 7, 0,
            59, 41, 17, 13, 7, 0,
            59, 43, 41, 37, 29, 13, 11, 7, 0,
            59, 53, 47, 43, 31, 29, 7, 1, 0,
            59, 53, 49, 43, 29, 23, 19, 17, 11, 7, 1, 0,
            49, 43, 37, 23, 19, 13, 1, 0,
            53, 47, 43, 17, 0,
            59, 49, 41, 37, 31, 29, 23, 1, 0,
            49, 47, 31, 23, 19, 7, 0,
            53, 43, 41, 37, 29, 23, 17, 13, 11, 7, 1, 0,
            53, 49, 47, 41, 31, 29, 19, 11, 7, 0,
            53, 43, 31, 29, 23, 19, 17, 1, 0,
            47, 43, 37, 29, 23, 19, 1, 0,
            53, 43, 37, 31, 23, 13, 1, 0,
            47, 43, 37, 31, 29, 1, 0,
            59, 49, 41, 37, 23, 19, 11, 0,
            59, 53, 41, 37, 31, 11, 1, 0,
            59, 43, 17, 13, 11, 0,
            59, 53, 49, 37, 31, 23, 19, 11, 0,
            49, 47, 43, 41, 29, 1, 0,
            59, 53, 47, 41, 37, 31, 29, 17, 13, 1, 0,
            59, 49, 47, 31, 29, 13, 7, 0,
            47, 31, 23, 19, 17, 13, 11, 0,
            49, 41, 29, 23, 13, 11, 7, 1, 0,
            59, 53, 43, 41, 29, 19, 17, 7, 0,
            53, 47, 43, 37, 29, 23, 17, 1, 0,
            49, 47, 43, 29, 23, 19, 11, 1, 0,
            53, 47, 41, 19, 13, 11, 1, 0,
            47, 43, 37, 19, 17, 7, 0,
            53, 49, 47, 37, 23, 19, 11, 0,
            53, 49, 43, 41, 37, 31, 29, 17, 13, 0,
            53, 31, 19, 17, 13, 7, 1, 0,
            59, 53, 31, 29, 23, 7, 1, 0,
            49, 47, 41, 29, 13, 11, 7, 1, 0,
            47, 43, 41, 23, 0,
            59, 49, 43, 41, 31, 29, 19, 17, 7, 1, 0,
            59, 49, 47, 37, 31, 23, 0,
            53, 43, 41, 31, 23, 17, 7, 0,
            49, 43, 31, 19, 17, 0,
            59, 53, 49, 43, 37, 19, 17, 7, 0,
            53, 47, 43, 41, 37, 13, 11, 0,
            59, 53, 37, 29, 23, 17, 0,
            59, 43, 41, 37, 31, 29, 23, 17, 7, 1, 0,
            49, 37, 19, 17, 11, 7, 0,
            49, 41, 17, 13, 11, 7, 1, 0,
            59, 47, 43, 29, 0,
            59, 49, 43, 31, 17, 11, 1, 0,
            49, 47, 43, 37, 29, 23, 13, 11, 7, 0,
            53, 47, 41, 37, 31, 29, 19, 17, 1, 0,
            49, 41, 23, 19, 13, 1, 0,
            59, 53, 47, 41, 13, 7, 0,
            53, 49, 41, 31, 23, 11, 0,
            53, 49, 47, 43, 19, 7, 1, 0,
            59, 43, 37, 31, 17, 7, 0,
            53, 49, 19, 13, 7, 1, 0,
            53, 47, 43, 41, 29, 23, 19, 17, 11, 0,
            49, 43, 41, 37, 19, 17, 13, 1, 0,
            59, 53, 49, 23, 17, 11, 7, 0,
            59, 49, 17, 1, 0,
            59, 53, 49, 47, 43, 41, 31, 17, 11, 7, 0,
            59, 49, 43, 23, 19, 17, 11, 0,
            41, 37, 29, 19, 13, 7, 0,
            59, 53, 41, 37, 31, 29, 23, 19, 13, 11, 1, 0,
            59, 43, 37, 17, 13, 1, 0,
            47, 19, 13, 0,
            53, 49, 31, 23, 17, 13, 7, 0,
            53, 47, 41, 31, 29, 19, 7, 0,
            59, 49, 47, 37, 29, 17, 7, 0,
            43, 41, 37, 29, 23, 19, 11, 0,
            53, 43, 37, 31, 29, 23, 13, 1, 0,
            59, 49, 43, 37, 31, 23, 17, 7, 1, 0,
            59, 53, 47, 41, 31, 23, 0,
            59, 53, 31, 29, 11, 7, 0,
            59, 49, 19, 11, 0,
            53, 49, 47, 37, 31, 29, 23, 19, 1, 0,
            53, 41, 23, 7, 1, 0,
            59, 47, 41, 31, 29, 19, 17, 11, 1, 0,
            47, 43, 19, 17, 1, 0,
            59, 49, 47, 37, 17, 13, 11, 7, 0,
            59, 43, 37, 31, 29, 17, 1, 0,
            53, 49, 43, 13, 11, 1, 0,
            59, 53, 49, 43, 37, 29, 23, 19, 7, 1, 0,
            53, 41, 37, 23, 11, 1, 0,
            37, 31, 19, 13, 11, 0,
            49, 41, 23, 13, 7, 0,
            53, 49, 47, 41, 31, 23, 17, 13, 7, 0,
            37, 23, 13, 11, 0,
            59, 49, 47, 31, 29, 11, 0,
            47, 29, 23, 7, 0,
            49, 43, 41, 23, 19, 13, 11, 1, 0,
            53, 43, 37, 31, 23, 19, 13, 11, 1, 0,
            59, 47, 43, 37, 31, 29, 17, 13, 0,
            59, 41, 37, 31, 11, 7, 0,
            59, 53, 49, 41, 37, 23, 17, 13, 0,
            59, 47, 43, 41, 11, 7, 0,
            43, 37, 31, 19, 7, 0,
            53, 47, 43, 41, 37, 19, 13, 1, 0,
            53, 47, 43, 31, 29, 17, 0,
            47, 31, 29, 23, 1, 0,
            47, 41, 31, 19, 13, 11, 7, 0,
            59, 49, 43, 37, 13, 0,
            59, 53, 49, 41, 29, 11, 1, 0,
            59, 49, 47, 43, 37, 17, 11, 7, 0,
            53, 49, 47, 43, 29, 23};

        /* baby-steps, giant-steps pairing for b1 = 100, w = 60
        q0 = 120 */
        const int num_b1_100_pairs = 279;
        static uint32_t[] b1_100_pairs = {
            19, 17, 13, 11, 7, 29, 31, 37, 43, 47, 53, 59, 0,
            59, 49, 47, 43, 41, 29, 17, 13, 11, 7, 1, 23, 31, 37, 53, 0,
            53, 49, 47, 43, 29, 23, 13, 11, 7, 1, 19, 37, 41, 59, 0,
            59, 49, 47, 41, 37, 31, 23, 19, 17, 13, 1, 7, 11, 29, 43, 0,
            59, 53, 43, 37, 31, 29, 23, 13, 7, 1, 17, 19, 41, 47, 0,
            59, 47, 43, 37, 29, 19, 11, 1, 7, 13, 23, 31, 41, 49, 53, 0,
            53, 43, 31, 29, 19, 17, 13, 11, 1, 23, 37, 41, 47, 0,
            53, 49, 41, 31, 23, 19, 13, 7, 11, 17, 37, 59, 0,
            59, 49, 47, 41, 31, 29, 19, 17, 11, 7, 13, 23, 37, 43, 0,
            49, 47, 37, 29, 19, 13, 7, 1, 17, 23, 31, 59, 0,
            43, 41, 37, 31, 29, 23, 19, 17, 13, 1, 7, 47, 53, 0,
            59, 41, 31, 17, 13, 11, 7, 1, 19, 43, 47, 49, 53, 0,
            49, 37, 29, 17, 11, 7, 1, 19, 23, 41, 47, 53, 59, 0,
            59, 53, 43, 23, 17, 13, 11, 19, 29, 41, 0,
            59, 53, 47, 41, 23, 17, 13, 11, 1, 31, 0,
            59, 53, 49, 47, 43, 41, 31, 19, 13, 7, 11, 29, 0,
            53, 47, 43, 41, 37, 29, 23, 13, 11, 1, 49, 59, 0,
            49, 47, 31, 29, 23, 19, 17, 7, 1, 43, 53, 0,
            59, 43, 41, 37, 29, 13, 11, 7, 1, 17, 31, 53, 0,
            59, 53, 49, 43, 29, 23, 19, 17, 11, 7, 1, 37, 41, 47, 0,
            53, 47, 43 };

        /* baby-steps, giant-steps pairing for b1 = 333, w = 60
        q0 = 360
        */
        const int num_b1_333_pairs = 837;
        static uint8_t[] b1_333_pairs = {
            23, 13, 11, 7, 1, 19, 29, 37, 41, 49, 59, 0,
            59, 49, 47, 41, 37, 31, 23, 19, 17, 13, 1, 7, 11, 29, 43, 0,
            59, 53, 43, 37, 31, 29, 23, 13, 7, 1, 17, 19, 41, 47, 0,
            59, 47, 43, 37, 29, 19, 11, 1, 7, 13, 23, 31, 41, 49, 53, 0,
            53, 43, 31, 29, 19, 17, 13, 11, 1, 23, 37, 41, 47, 0,
            53, 49, 41, 31, 23, 19, 13, 7, 11, 17, 37, 59, 0,
            59, 49, 47, 41, 31, 29, 19, 17, 11, 7, 13, 23, 37, 43, 0,
            49, 47, 37, 29, 19, 13, 7, 1, 17, 23, 31, 59, 0,
            43, 41, 37, 31, 29, 23, 19, 17, 13, 1, 7, 47, 53, 0,
            59, 41, 31, 17, 13, 11, 7, 1, 19, 43, 47, 49, 53, 0,
            49, 37, 29, 17, 11, 7, 1, 19, 23, 41, 47, 53, 59, 0,
            59, 53, 43, 23, 17, 13, 11, 19, 29, 41, 0,
            59, 53, 47, 41, 23, 17, 13, 11, 1, 31, 0,
            59, 53, 49, 47, 43, 41, 31, 19, 13, 7, 11, 29, 0,
            53, 47, 43, 41, 37, 29, 23, 13, 11, 1, 49, 59, 0,
            49, 47, 31, 29, 23, 19, 17, 7, 1, 43, 53, 0,
            59, 43, 41, 37, 29, 13, 11, 7, 1, 17, 31, 53, 0,
            59, 53, 49, 43, 29, 23, 19, 17, 11, 7, 1, 37, 41, 47, 0,
            53, 47, 43, 17, 1, 11, 19, 23, 29, 31, 37, 59, 0,
            49, 47, 31, 23, 19, 7, 17, 37, 43, 53, 59, 0,
            53, 49, 47, 41, 31, 29, 19, 11, 7, 17, 37, 43, 59, 0,
            47, 43, 37, 29, 23, 19, 1, 7, 17, 59, 0,
            47, 43, 37, 31, 29, 1, 11, 19, 23, 41, 49, 0,
            59, 53, 41, 37, 31, 11, 1, 17, 43, 47, 49, 0,
            59, 53, 49, 37, 31, 23, 19, 11, 13, 17, 0,
            59, 53, 47, 41, 37, 31, 29, 17, 13, 1, 11, 0,
            47, 31, 23, 19, 17, 13, 11, 37, 49, 53, 59, 0,
            59, 53, 43, 41, 29, 19, 17, 7, 13, 23, 31, 37, 0,
            49, 47, 43, 29, 23, 19, 11, 1, 7, 13, 41, 59, 0,
            47, 43, 37, 19, 17, 7, 11, 13, 23, 41, 49, 0,
            53, 49, 43, 41, 37, 31, 29, 17, 13, 7, 47, 59, 0,
            59, 53, 31, 29, 23, 7, 1, 11, 13, 19, 47, 49, 0,
            47, 43, 41, 23, 1, 11, 17, 19, 29, 31, 53, 59, 0,
            59, 49, 47, 37, 31, 23, 7, 17, 19, 29, 43, 53, 0,
            49, 43, 31, 19, 17, 1, 7, 11, 23, 41, 53, 0,
            53, 47, 43, 41, 37, 13, 11, 1, 7, 23, 31, 0,
            59, 43, 41, 37, 31, 29, 23, 17, 7, 1, 11, 49, 53, 0,
            49, 41, 17, 13, 11, 7, 1, 31, 0,
            59, 49, 43, 31, 17, 11, 1, 13, 23, 37, 47, 53, 0,
            53, 47, 41, 37, 31, 29, 19, 17, 1, 11, 59, 0,
            59, 53, 47, 41, 13, 7, 11, 19, 29, 37, 49, 0,
            53, 49, 47, 43, 19, 7, 1, 17, 23, 29, 0,
            53, 49, 19, 13, 7, 1, 17, 31, 37, 41, 43, 0,
            49, 43, 41, 37, 19, 17, 13, 1, 7, 11, 53, 0,
            59, 49, 17, 1, 7, 11, 13, 19, 29, 43, 53, 0,
            59, 49, 43, 23, 19, 17, 11, 31, 41, 47, 53, 0,
            59, 53, 41, 37, 31, 29, 23, 19, 13, 11, 1, 17, 43, 47, 0,
            47, 19, 13, 7, 11, 29, 37, 43, 53, 0,
            53, 47, 41, 31, 29, 19, 7, 1, 11, 13, 23, 43, 0,
            43, 41, 37, 29, 23, 19, 11, 7, 17, 31, 47, 59, 0,
            59, 49, 43, 37, 31, 23, 17, 7, 1, 13, 19, 29, 0,
            59, 53, 31, 29, 11, 7, 1, 41, 49, 0,
            53, 49, 47, 37, 31, 29, 23, 19, 1, 7, 59, 0,
            59, 47, 41, 31, 29, 19, 17, 11, 1, 13, 43, 0,
            59, 49, 47, 37, 17, 13, 11, 7, 1, 23, 29, 31, 43, 0,
            53, 49, 43, 13, 11, 1, 7, 17, 23, 31, 37, 41, 59, 0,
            53, 41, 37, 23, 11, 1, 29, 47, 49, 0,
            49, 41, 23, 13, 7, 11, 19, 29, 37, 43, 47, 53, 0,
            37, 23, 13, 11, 1, 29, 31, 49, 0,
            47, 29, 23, 7, 11, 17, 19, 37, 41, 49, 59, 0,
            53, 43, 37, 31, 23, 19, 13, 11, 1, 17, 29, 47, 0,
            59, 41, 37, 31, 11, 7, 1, 19, 23, 43, 47, 0,
            59, 47, 43, 41, 11, 7, 17, 23, 29, 53, 0,
            53, 47, 43, 41, 37, 19, 13, 1, 7, 17, 29, 31, 0,
            47, 31, 29, 23, 1, 13, 19, 41, 49, 53, 0,
            59, 49, 43, 37, 13, 1, 7, 11, 19, 31, 0,
            59, 49, 47, 43, 37, 17, 11, 7, 13, 31 };

        /* baby-steps, giant-steps pairing for b1 = 666, b2 = 16650, w = 60
        q0 = 720*/
        const int num_b1_666_pairs = 1577;
        static uint8_t[] b1_666_pairs = {
            47, 43, 37, 29, 19, 11, 1, 7, 13, 23, 31, 41, 49, 53, 0,
            53, 43, 31, 29, 19, 17, 13, 11, 1, 23, 37, 41, 47, 0,
            53, 49, 41, 31, 23, 19, 13, 7, 11, 17, 37, 59, 0,
            59, 49, 47, 41, 31, 29, 19, 17, 11, 7, 13, 23, 37, 43, 0,
            49, 47, 37, 29, 19, 13, 7, 1, 17, 23, 31, 59, 0,
            43, 41, 37, 31, 29, 23, 19, 17, 13, 1, 7, 47, 53, 0,
            59, 41, 31, 17, 13, 11, 7, 1, 19, 43, 47, 49, 53, 0,
            49, 37, 29, 17, 11, 7, 1, 19, 23, 41, 47, 53, 59, 0,
            59, 53, 43, 23, 17, 13, 11, 19, 29, 41, 0,
            59, 53, 47, 41, 23, 17, 13, 11, 1, 31, 0,
            59, 53, 49, 47, 43, 41, 31, 19, 13, 7, 11, 29, 0,
            53, 47, 43, 41, 37, 29, 23, 13, 11, 1, 49, 59, 0,
            49, 47, 31, 29, 23, 19, 17, 7, 1, 43, 53, 0,
            59, 43, 41, 37, 29, 13, 11, 7, 1, 17, 31, 53, 0,
            59, 53, 49, 43, 29, 23, 19, 17, 11, 7, 1, 37, 41, 47, 0,
            53, 47, 43, 17, 1, 11, 19, 23, 29, 31, 37, 59, 0,
            49, 47, 31, 23, 19, 7, 17, 37, 43, 53, 59, 0,
            53, 49, 47, 41, 31, 29, 19, 11, 7, 17, 37, 43, 59, 0,
            47, 43, 37, 29, 23, 19, 1, 7, 17, 59, 0,
            47, 43, 37, 31, 29, 1, 11, 19, 23, 41, 49, 0,
            59, 53, 41, 37, 31, 11, 1, 17, 43, 47, 49, 0,
            59, 53, 49, 37, 31, 23, 19, 11, 13, 17, 0,
            59, 53, 47, 41, 37, 31, 29, 17, 13, 1, 11, 0,
            47, 31, 23, 19, 17, 13, 11, 37, 49, 53, 59, 0,
            59, 53, 43, 41, 29, 19, 17, 7, 13, 23, 31, 37, 0,
            49, 47, 43, 29, 23, 19, 11, 1, 7, 13, 41, 59, 0,
            47, 43, 37, 19, 17, 7, 11, 13, 23, 41, 49, 0,
            53, 49, 43, 41, 37, 31, 29, 17, 13, 7, 47, 59, 0,
            59, 53, 31, 29, 23, 7, 1, 11, 13, 19, 47, 49, 0,
            47, 43, 41, 23, 1, 11, 17, 19, 29, 31, 53, 59, 0,
            59, 49, 47, 37, 31, 23, 7, 17, 19, 29, 43, 53, 0,
            49, 43, 31, 19, 17, 1, 7, 11, 23, 41, 53, 0,
            53, 47, 43, 41, 37, 13, 11, 1, 7, 23, 31, 0,
            59, 43, 41, 37, 31, 29, 23, 17, 7, 1, 11, 49, 53, 0,
            49, 41, 17, 13, 11, 7, 1, 31, 0,
            59, 49, 43, 31, 17, 11, 1, 13, 23, 37, 47, 53, 0,
            53, 47, 41, 37, 31, 29, 19, 17, 1, 11, 59, 0,
            59, 53, 47, 41, 13, 7, 11, 19, 29, 37, 49, 0,
            53, 49, 47, 43, 19, 7, 1, 17, 23, 29, 0,
            53, 49, 19, 13, 7, 1, 17, 31, 37, 41, 43, 0,
            49, 43, 41, 37, 19, 17, 13, 1, 7, 11, 53, 0,
            59, 49, 17, 1, 7, 11, 13, 19, 29, 43, 53, 0,
            59, 49, 43, 23, 19, 17, 11, 31, 41, 47, 53, 0,
            59, 53, 41, 37, 31, 29, 23, 19, 13, 11, 1, 17, 43, 47, 0,
            47, 19, 13, 7, 11, 29, 37, 43, 53, 0,
            53, 47, 41, 31, 29, 19, 7, 1, 11, 13, 23, 43, 0,
            43, 41, 37, 29, 23, 19, 11, 7, 17, 31, 47, 59, 0,
            59, 49, 43, 37, 31, 23, 17, 7, 1, 13, 19, 29, 0,
            59, 53, 31, 29, 11, 7, 1, 41, 49, 0,
            53, 49, 47, 37, 31, 29, 23, 19, 1, 7, 59, 0,
            59, 47, 41, 31, 29, 19, 17, 11, 1, 13, 43, 0,
            59, 49, 47, 37, 17, 13, 11, 7, 1, 23, 29, 31, 43, 0,
            53, 49, 43, 13, 11, 1, 7, 17, 23, 31, 37, 41, 59, 0,
            53, 41, 37, 23, 11, 1, 29, 47, 49, 0,
            49, 41, 23, 13, 7, 11, 19, 29, 37, 43, 47, 53, 0,
            37, 23, 13, 11, 1, 29, 31, 49, 0,
            47, 29, 23, 7, 11, 17, 19, 37, 41, 49, 59, 0,
            53, 43, 37, 31, 23, 19, 13, 11, 1, 17, 29, 47, 0,
            59, 41, 37, 31, 11, 7, 1, 19, 23, 43, 47, 0,
            59, 47, 43, 41, 11, 7, 17, 23, 29, 53, 0,
            53, 47, 43, 41, 37, 19, 13, 1, 7, 17, 29, 31, 0,
            47, 31, 29, 23, 1, 13, 19, 41, 49, 53, 0,
            59, 49, 43, 37, 13, 1, 7, 11, 19, 31, 0,
            59, 49, 47, 43, 37, 17, 11, 7, 13, 31, 0,
            47, 37, 31, 23, 13, 11, 19, 29, 43, 0,
            59, 53, 19, 7, 1, 17, 23, 43, 0,
            59, 43, 41, 31, 17, 13, 11, 1, 7, 23, 29, 37, 49, 53, 0,
            53, 47, 41, 29, 23, 19, 13, 7, 1, 43, 59, 0,
            59, 49, 43, 41, 31, 19, 17, 13, 7, 53, 0,
            59, 49, 37, 31, 29, 1, 7, 11, 13, 41, 43, 0,
            53, 29, 17, 11, 7, 13, 31, 37, 41, 0,
            59, 53, 41, 37, 31, 19, 13, 1, 17, 43, 0,
            49, 41, 37, 23, 19, 17, 11, 31, 43, 53, 59, 0,
            59, 49, 47, 43, 41, 19, 17, 13, 7, 1, 11, 31, 53, 0,
            53, 49, 13, 1, 19, 23, 29, 31, 43, 0,
            59, 43, 41, 31, 23, 1, 13, 19, 29, 47, 49, 0,
            59, 53, 49, 37, 29, 23, 11, 7, 1, 17, 19, 31, 43, 47, 0,
            59, 53, 37, 31, 29, 19, 11, 7, 13, 47, 49, 0,
            43, 41, 19, 13, 11, 1, 23, 31, 53, 59, 0,
            59, 49, 41, 37, 31, 23, 19, 7, 11, 43, 47, 53, 0,
            53, 49, 47, 31, 19, 17, 7, 1, 11, 13, 23, 37, 0,
            49, 41, 13, 11, 7, 17, 19, 23, 37, 47, 59, 0,
            59, 47, 31, 29, 1, 7, 37, 41, 53, 0,
            53, 49, 41, 29, 23, 17, 13, 7, 11, 31, 43, 59, 0,
            47, 29, 19, 11, 1, 31, 37, 53, 59, 0,
            59, 53, 37, 31, 29, 17, 11, 19, 0,
            53, 47, 37, 13, 7, 17, 19, 29, 31, 43, 0,
            47, 43, 41, 29, 11, 1, 13, 17, 37, 53, 0,
            41, 37, 29, 23, 19, 7, 1, 31, 49, 0,
            49, 47, 31, 17, 7, 1, 11, 23, 37, 43, 0,
            53, 49, 37, 31, 29, 23, 17, 1, 7, 59, 0,
            53, 47, 43, 23, 19, 7, 17, 37, 41, 49, 59, 0,
            59, 43, 41, 29, 17, 19, 23, 47, 53, 0,
            59, 53, 49, 47, 41, 17, 13, 7, 23, 29, 43, 0,
            59, 47, 41, 31, 29, 19, 13, 7, 11, 37, 43, 49, 0,
            49, 47, 23, 19, 13, 11, 7, 1, 29, 37, 41, 43, 0,
            43, 37, 29, 13, 1, 11, 23, 41, 49, 0,
            59, 37, 31, 17, 13, 19, 41, 49, 53, 0,
            59, 47, 43, 29, 23, 7, 1, 11, 17, 31, 37, 0,
            59, 53, 47, 31, 23, 17, 11, 1, 13, 19, 37, 41, 0,
            49, 31, 23, 17, 7, 1, 19, 37, 43, 0,
            59, 49, 41, 31, 19, 17, 11, 1, 13, 53, 0,
            53, 49, 43, 41, 37, 19, 7, 1, 13, 23, 47, 0,
            47, 43, 37, 31, 17, 13, 19, 23, 29, 41, 0,
            53, 49, 41, 37, 29, 23, 17, 13, 19, 59, 0,
            53, 29, 23, 11, 7, 17, 19, 47, 0,
            59, 43, 41, 29, 23, 19, 1, 11, 17, 37, 47, 0,
            47, 37, 23, 7, 17, 31, 53, 59, 0,
            53, 47, 31, 11, 1, 7, 13, 17, 29, 41, 43, 49, 0,
            49, 43, 41, 37, 19, 11, 1, 7, 29, 31, 59, 0,
            47, 43, 41, 37, 19, 17, 13, 7, 1, 11, 0,
            43, 41, 31, 29, 11, 7, 17, 47, 0,
            53, 17, 11, 7, 1, 13, 37, 47, 0,
            59, 37, 31, 29, 1, 13, 23, 41, 43, 47, 0,
            59, 53, 31, 13, 11, 1, 7, 19, 23, 37, 47, 49, 0,
            59, 41, 31, 17, 1, 13, 23, 29, 37, 43, 0,
            49, 47, 19, 13, 11, 7, 1, 17, 29, 43, 59, 0,
            47, 43, 37, 29, 23, 19, 13, 7, 1, 11, 53, 0,
            59, 53, 49, 37, 29, 13, 11, 1, 7, 17, 43, 0,
            53, 49, 43, 31, 17, 13, 0,
            59, 47, 43, 37, 29, 19, 13, 1, 11, 17, 41, 53, 0,
            53, 47, 41, 23, 13, 7, 1, 19, 29, 31, 37, 49, 59, 0,
            53, 47, 41, 31, 29, 11, 1, 13, 17, 23, 0,
            53, 41, 37, 29, 19, 13, 7, 17, 31, 47, 0,
            59, 49, 41, 31, 19, 17, 1, 7, 29, 43, 47, 0,
            59, 53, 49, 41, 37, 7, 11, 13, 17, 19, 29, 47, 0,
            53, 49, 43, 37, 31, 23, 17, 19, 41, 47, 0,
            59, 53, 47, 41, 37, 23, 1, 11, 13, 31, 0,
            47, 23, 19, 17, 13, 11, 7, 31, 59, 0,
            59, 17, 13, 11, 7, 23, 29, 31, 49, 53, 0,
            53, 47, 19, 1, 13, 29, 41, 43, 49, 0,
            59, 29, 23, 19, 13, 7, 11, 37, 41, 47, 53, 0,
            41, 31, 13, 7, 1, 43, 47, 59, 0,
            49, 47, 31};


        /* baby-steps, giant-steps pairing for b1 = 1000, b2 = 25000, w = 60 */
        //q0 = 1080 (note this misses the first 3 primes in the range 1000 - 1080)
        const int num_b1_1000_pairs = 2275;
        static uint8_t[] b1_1000_pairs = {
            59, 49, 47, 41, 31, 29, 19, 17, 11, 7, 13, 23, 37, 43, 0,
            49, 47, 37, 29, 19, 13, 7, 1, 17, 23, 31, 59, 0,
            43, 41, 37, 31, 29, 23, 19, 17, 13, 1, 7, 47, 53, 0,
            59, 41, 31, 17, 13, 11, 7, 1, 19, 43, 47, 49, 53, 0,
            49, 37, 29, 17, 11, 7, 1, 19, 23, 41, 47, 53, 59, 0,
            59, 53, 43, 23, 17, 13, 11, 19, 29, 41, 0,
            59, 53, 47, 41, 23, 17, 13, 11, 1, 31, 0,
            59, 53, 49, 47, 43, 41, 31, 19, 13, 7, 11, 29, 0,
            53, 47, 43, 41, 37, 29, 23, 13, 11, 1, 49, 59, 0,
            49, 47, 31, 29, 23, 19, 17, 7, 1, 43, 53, 0,
            59, 43, 41, 37, 29, 13, 11, 7, 1, 17, 31, 53, 0,
            59, 53, 49, 43, 29, 23, 19, 17, 11, 7, 1, 37, 41, 47, 0,
            53, 47, 43, 17, 1, 11, 19, 23, 29, 31, 37, 59, 0,
            49, 47, 31, 23, 19, 7, 17, 37, 43, 53, 59, 0,
            53, 49, 47, 41, 31, 29, 19, 11, 7, 17, 37, 43, 59, 0,
            47, 43, 37, 29, 23, 19, 1, 7, 17, 59, 0,
            47, 43, 37, 31, 29, 1, 11, 19, 23, 41, 49, 0,
            59, 53, 41, 37, 31, 11, 1, 17, 43, 47, 49, 0,
            59, 53, 49, 37, 31, 23, 19, 11, 13, 17, 0,
            59, 53, 47, 41, 37, 31, 29, 17, 13, 1, 11, 0,
            47, 31, 23, 19, 17, 13, 11, 37, 49, 53, 59, 0,
            59, 53, 43, 41, 29, 19, 17, 7, 13, 23, 31, 37, 0,
            49, 47, 43, 29, 23, 19, 11, 1, 7, 13, 41, 59, 0,
            47, 43, 37, 19, 17, 7, 11, 13, 23, 41, 49, 0,
            53, 49, 43, 41, 37, 31, 29, 17, 13, 7, 47, 59, 0,
            59, 53, 31, 29, 23, 7, 1, 11, 13, 19, 47, 49, 0,
            47, 43, 41, 23, 1, 11, 17, 19, 29, 31, 53, 59, 0,
            59, 49, 47, 37, 31, 23, 7, 17, 19, 29, 43, 53, 0,
            49, 43, 31, 19, 17, 1, 7, 11, 23, 41, 53, 0,
            53, 47, 43, 41, 37, 13, 11, 1, 7, 23, 31, 0,
            59, 43, 41, 37, 31, 29, 23, 17, 7, 1, 11, 49, 53, 0,
            49, 41, 17, 13, 11, 7, 1, 31, 0,
            59, 49, 43, 31, 17, 11, 1, 13, 23, 37, 47, 53, 0,
            53, 47, 41, 37, 31, 29, 19, 17, 1, 11, 59, 0,
            59, 53, 47, 41, 13, 7, 11, 19, 29, 37, 49, 0,
            53, 49, 47, 43, 19, 7, 1, 17, 23, 29, 0,
            53, 49, 19, 13, 7, 1, 17, 31, 37, 41, 43, 0,
            49, 43, 41, 37, 19, 17, 13, 1, 7, 11, 53, 0,
            59, 49, 17, 1, 7, 11, 13, 19, 29, 43, 53, 0,
            59, 49, 43, 23, 19, 17, 11, 31, 41, 47, 53, 0,
            59, 53, 41, 37, 31, 29, 23, 19, 13, 11, 1, 17, 43, 47, 0,
            47, 19, 13, 7, 11, 29, 37, 43, 53, 0,
            53, 47, 41, 31, 29, 19, 7, 1, 11, 13, 23, 43, 0,
            43, 41, 37, 29, 23, 19, 11, 7, 17, 31, 47, 59, 0,
            59, 49, 43, 37, 31, 23, 17, 7, 1, 13, 19, 29, 0,
            59, 53, 31, 29, 11, 7, 1, 41, 49, 0,
            53, 49, 47, 37, 31, 29, 23, 19, 1, 7, 59, 0,
            59, 47, 41, 31, 29, 19, 17, 11, 1, 13, 43, 0,
            59, 49, 47, 37, 17, 13, 11, 7, 1, 23, 29, 31, 43, 0,
            53, 49, 43, 13, 11, 1, 7, 17, 23, 31, 37, 41, 59, 0,
            53, 41, 37, 23, 11, 1, 29, 47, 49, 0,
            49, 41, 23, 13, 7, 11, 19, 29, 37, 43, 47, 53, 0,
            37, 23, 13, 11, 1, 29, 31, 49, 0,
            47, 29, 23, 7, 11, 17, 19, 37, 41, 49, 59, 0,
            53, 43, 37, 31, 23, 19, 13, 11, 1, 17, 29, 47, 0,
            59, 41, 37, 31, 11, 7, 1, 19, 23, 43, 47, 0,
            59, 47, 43, 41, 11, 7, 17, 23, 29, 53, 0,
            53, 47, 43, 41, 37, 19, 13, 1, 7, 17, 29, 31, 0,
            47, 31, 29, 23, 1, 13, 19, 41, 49, 53, 0,
            59, 49, 43, 37, 13, 1, 7, 11, 19, 31, 0,
            59, 49, 47, 43, 37, 17, 11, 7, 13, 31, 0,
            47, 37, 31, 23, 13, 11, 19, 29, 43, 0,
            59, 53, 19, 7, 1, 17, 23, 43, 0,
            59, 43, 41, 31, 17, 13, 11, 1, 7, 23, 29, 37, 49, 53, 0,
            53, 47, 41, 29, 23, 19, 13, 7, 1, 43, 59, 0,
            59, 49, 43, 41, 31, 19, 17, 13, 7, 53, 0,
            59, 49, 37, 31, 29, 1, 7, 11, 13, 41, 43, 0,
            53, 29, 17, 11, 7, 13, 31, 37, 41, 0,
            59, 53, 41, 37, 31, 19, 13, 1, 17, 43, 0,
            49, 41, 37, 23, 19, 17, 11, 31, 43, 53, 59, 0,
            59, 49, 47, 43, 41, 19, 17, 13, 7, 1, 11, 31, 53, 0,
            53, 49, 13, 1, 19, 23, 29, 31, 43, 0,
            59, 43, 41, 31, 23, 1, 13, 19, 29, 47, 49, 0,
            59, 53, 49, 37, 29, 23, 11, 7, 1, 17, 19, 31, 43, 47, 0,
            59, 53, 37, 31, 29, 19, 11, 7, 13, 47, 49, 0,
            43, 41, 19, 13, 11, 1, 23, 31, 53, 59, 0,
            59, 49, 41, 37, 31, 23, 19, 7, 11, 43, 47, 53, 0,
            53, 49, 47, 31, 19, 17, 7, 1, 11, 13, 23, 37, 0,
            49, 41, 13, 11, 7, 17, 19, 23, 37, 47, 59, 0,
            59, 47, 31, 29, 1, 7, 37, 41, 53, 0,
            53, 49, 41, 29, 23, 17, 13, 7, 11, 31, 43, 59, 0,
            47, 29, 19, 11, 1, 31, 37, 53, 59, 0,
            59, 53, 37, 31, 29, 17, 11, 19, 0,
            53, 47, 37, 13, 7, 17, 19, 29, 31, 43, 0,
            47, 43, 41, 29, 11, 1, 13, 17, 37, 53, 0,
            41, 37, 29, 23, 19, 7, 1, 31, 49, 0,
            49, 47, 31, 17, 7, 1, 11, 23, 37, 43, 0,
            53, 49, 37, 31, 29, 23, 17, 1, 7, 59, 0,
            53, 47, 43, 23, 19, 7, 17, 37, 41, 49, 59, 0,
            59, 43, 41, 29, 17, 19, 23, 47, 53, 0,
            59, 53, 49, 47, 41, 17, 13, 7, 23, 29, 43, 0,
            59, 47, 41, 31, 29, 19, 13, 7, 11, 37, 43, 49, 0,
            49, 47, 23, 19, 13, 11, 7, 1, 29, 37, 41, 43, 0,
            43, 37, 29, 13, 1, 11, 23, 41, 49, 0,
            59, 37, 31, 17, 13, 19, 41, 49, 53, 0,
            59, 47, 43, 29, 23, 7, 1, 11, 17, 31, 37, 0,
            59, 53, 47, 31, 23, 17, 11, 1, 13, 19, 37, 41, 0,
            49, 31, 23, 17, 7, 1, 19, 37, 43, 0,
            59, 49, 41, 31, 19, 17, 11, 1, 13, 53, 0,
            53, 49, 43, 41, 37, 19, 7, 1, 13, 23, 47, 0,
            47, 43, 37, 31, 17, 13, 19, 23, 29, 41, 0,
            53, 49, 41, 37, 29, 23, 17, 13, 19, 59, 0,
            53, 29, 23, 11, 7, 17, 19, 47, 0,
            59, 43, 41, 29, 23, 19, 1, 11, 17, 37, 47, 0,
            47, 37, 23, 7, 17, 31, 53, 59, 0,
            53, 47, 31, 11, 1, 7, 13, 17, 29, 41, 43, 49, 0,
            49, 43, 41, 37, 19, 11, 1, 7, 29, 31, 59, 0,
            47, 43, 41, 37, 19, 17, 13, 7, 1, 11, 0,
            43, 41, 31, 29, 11, 7, 17, 47, 0,
            53, 17, 11, 7, 1, 13, 37, 47, 0,
            59, 37, 31, 29, 1, 13, 23, 41, 43, 47, 0,
            59, 53, 31, 13, 11, 1, 7, 19, 23, 37, 47, 49, 0,
            59, 41, 31, 17, 1, 13, 23, 29, 37, 43, 0,
            49, 47, 19, 13, 11, 7, 1, 17, 29, 43, 59, 0,
            47, 43, 37, 29, 23, 19, 13, 7, 1, 11, 53, 0,
            59, 53, 49, 37, 29, 13, 11, 1, 7, 17, 43, 0,
            53, 49, 43, 31, 17, 13, 0,
            59, 47, 43, 37, 29, 19, 13, 1, 11, 17, 41, 53, 0,
            53, 47, 41, 23, 13, 7, 1, 19, 29, 31, 37, 49, 59, 0,
            53, 47, 41, 31, 29, 11, 1, 13, 17, 23, 0,
            53, 41, 37, 29, 19, 13, 7, 17, 31, 47, 0,
            59, 49, 41, 31, 19, 17, 1, 7, 29, 43, 47, 0,
            59, 53, 49, 41, 37, 7, 11, 13, 17, 19, 29, 47, 0,
            53, 49, 43, 37, 31, 23, 17, 19, 41, 47, 0,
            59, 53, 47, 41, 37, 23, 1, 11, 13, 31, 0,
            47, 23, 19, 17, 13, 11, 7, 31, 59, 0,
            59, 17, 13, 11, 7, 23, 29, 31, 49, 53, 0,
            53, 47, 19, 1, 13, 29, 41, 43, 49, 0,
            59, 29, 23, 19, 13, 7, 11, 37, 41, 47, 53, 0,
            41, 31, 13, 7, 1, 43, 47, 59, 0,
            49, 47, 31, 29, 23, 19, 7, 11, 13, 0,
            59, 53, 41, 37, 13, 11, 23, 29, 31, 43, 0,
            49, 41, 37, 31, 19, 17, 1, 7, 11, 23, 43, 59, 0,
            59, 53, 47, 29, 19, 13, 11, 7, 1, 37, 0,
            53, 43, 37, 23, 1, 7, 29, 31, 47, 49, 0,
            49, 41, 23, 11, 13, 19, 37, 47, 53, 0,
            59, 49, 41, 23, 17, 13, 11, 7, 1, 19, 31, 43, 0,
            53, 49, 43, 37, 31, 29, 23, 11, 1, 19, 59, 0,
            59, 43, 41, 31, 17, 13, 19, 29, 0,
            53, 47, 31, 23, 13, 11, 1, 29, 0,
            53, 43, 41, 29, 17, 1, 11, 23, 31, 49, 59, 0,
            43, 41, 29, 23, 19, 13, 11, 47, 49, 59, 0,
            59, 43, 31, 23, 1, 7, 11, 13, 29, 49, 0,
            59, 49, 41, 29, 23, 17, 11, 7, 13, 47, 0,
            59, 53, 49, 47, 31, 19, 7, 11, 37, 41, 0,
            53, 47, 41, 37, 29, 23, 19, 1, 13, 43, 59, 0,
            59, 47, 17, 13, 7, 37, 0,
            59, 49, 41, 29, 19, 7, 1, 11, 23, 37, 53, 0,
            53, 47, 43, 37, 1, 19, 29, 59, 0,
            49, 47, 43, 41, 13, 1, 19, 53, 0,
            49, 43, 29, 11, 7, 1, 41, 59, 0,
            59, 43, 37, 19, 17, 7, 11, 13, 31, 49, 0,
            53, 47, 31, 19, 11, 1, 13, 59, 0,
            59, 53, 49, 37, 23, 19, 17, 13, 11, 7, 1, 29, 31, 43, 0,
            59, 53, 29, 19, 17, 7, 1, 11, 23, 37, 43, 49, 0,
            19, 1, 7, 17, 29, 37, 47, 59, 0,
            49, 47, 41, 37, 23, 7, 1, 13, 19, 43, 53, 0,
            59, 53, 31, 29, 7, 1, 17, 41, 43, 0,
            49, 47, 43, 29, 19, 17, 11, 7, 23, 31, 0,
            59, 53, 47, 43, 37, 31, 17, 13, 11, 1, 23, 41, 0,
            49, 47, 31, 19, 11, 7, 17, 43, 53, 0,
            59, 53, 47, 43, 41, 31, 11, 7, 1, 0,
            43, 41, 37, 13, 11, 1, 23, 29, 31, 0,
            47, 41, 29, 13, 1, 23, 53, 0,
            53, 43, 41, 29, 17, 13, 11, 7, 1, 47, 49, 0,
            31, 23, 7, 1, 17, 19, 41, 49, 59, 0,
            53, 41, 37, 19, 17, 1, 11, 13, 23, 31, 59, 0,
            59, 53, 31, 19, 13, 1, 23, 29, 37, 43, 49, 0,
            53, 49, 47, 29, 19, 13, 7, 37, 43, 0,
            47, 43, 41, 37, 19, 13, 17, 23, 31, 59, 0,
            47, 13, 1, 7, 11, 19, 23, 37, 41, 43, 49, 0,
            43, 41, 37, 31, 23, 13, 11, 1, 17, 47, 49, 0,
            59, 47, 37, 19, 7, 17, 31, 53, 0,
            53, 41, 37, 23, 19, 1, 11, 31, 0,
            49, 31, 23, 17, 1, 37, 43, 53, 0,
            53, 49, 43, 41, 29, 17, 13, 7, 1, 11, 31, 0,
            53, 47, 43, 41, 29, 11, 7, 59, 0,
            49, 47, 43, 41, 37, 29, 17, 13, 23, 0,
            59, 49, 43, 31, 7, 1, 13, 29, 41, 0,
            59, 49, 29, 19, 17, 11, 7, 13, 53, 0,
            59, 43, 41, 37, 29, 11, 1, 17, 19, 47, 0,
            59, 49, 31, 23, 17, 13, 7, 11, 53, 0,
            59, 49, 43, 19, 13, 1, 17, 23, 41, 53, 0,
            47, 37, 29, 23, 19, 13, 11, 1, 17, 31, 41, 59, 0,
            43, 29, 17, 1, 7, 13, 37, 41, 49, 0,
            53, 29, 11, 1, 13, 17, 31, 41, 47, 59, 0,
            43, 31, 29, 1, 17, 47, 59, 0,
            47, 23, 11, 17, 19, 29, 37, 41, 43, 0,
            59, 47, 41, 37, 31, 17, 13, 11, 7, 23, 29, 49, 0,
            41, 19, 17, 13, 7, 1, 29, 53, 59, 0,
            53, 49, 47, 23, 11, 7, 1, 13, 19, 29, 31, 37, 0,
            43, 29, 23, 19, 7, 1, 49, 0,
            59, 49, 43, 37, 29, 23, 17, 13, 11, 7, 1, 31, 0,
            59, 43, 37, 17, 11, 1, 7, 41, 0,
            43, 31, 23, 1, 11, 13, 19, 47, 53, 59, 0,
            59, 41, 37, 11, 7, 1, 19, 29, 47, 53, 0,
            53, 49, 29, 7, 11, 23, 31, 59, 0,
            49, 43, 37, 29, 23, 11, 13, 47, 0,
            59, 47, 41, 31, 19, 1, 7, 11, 37, 49, 0,
            53, 43, 41, 37, 17, 7, 11, 19, 29 };

    }
}
