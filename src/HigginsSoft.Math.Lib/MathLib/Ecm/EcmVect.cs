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

#define HAVE_ECM_AVX
#if HAVE_ECM_AVX

using System;
using uint64_t = System.UInt64;
using uint32_t = System.Int32;
using uint8_t = System.Int32;
using MathGmp.Native;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Collections;
using System.Security.Cryptography;
using _m512i = System.Runtime.Intrinsics.Vector256<long>;
using _m512d = System.Runtime.Intrinsics.Vector256<double>;
using _mmask8 = System.Int32;
using static HigginsSoft.Math.Lib.MathLib;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.ConstrainedExecution;
using System.Reflection;

namespace HigginsSoft.Math.Lib
{
    public partial class Ecm
    {

        const int uecm_DIGIT_SIZE = 52;
        const ulong uecm_DIGIT_MASK = 0x000fffffffffffffUL;
        private const int VextSize256 = 4;
        const int uecm_MB_WIDTH = VextSize256;
        const int uecm_SIMD_BYTES = 64;

        public static int VectorSize256 = Vector256<long>.Count;

        struct uecm_pt_x8
        {
            public _m512i X;
            public _m512i Z;
        }

        static _m512d dbias;
        static _m512i vbias1;
        static _m512i vbias2;
        static _m512i lo52mask;

        /*
              static _m512d dbias;
              static _m512i vbias1;
              static _m512i vbias2;
              static _m512i lo52mask;

               */

        static Ecm()
        {
            dbias = _mm512_castsi512_pd(uecm_set64((long)0x4670000000000000UL));
            vbias1 = uecm_set64((long)0x4670000000000000UL);
            vbias2 = uecm_set64((long)0x4670000000000001UL);
            lo52mask = _mm512_set1_epi64((long)0x000ffffffffffffful);
        }

       
/*
        
#define uecm_and64 _mm512_and_epi64
#define uecm_storeu64 _mm512_store_epi64
#define uecm_add64 _mm512_add_epi64
#define uecm_sub64 _mm512_sub_epi64
#define uecm_set64 _mm512_set1_epi64
#define uecm_srli64 _mm512_srli_epi64
#define uecm_loadu64 _mm512_load_epi64
#define uecm_castpd _mm512_castsi512_pd
#define uecm_castepu _mm512_castpd_si512
*/
       

        public Factorization[] FactorEcmAvx(ulong[] targets, int arbitrary, ref ulong state)
        {


            var factorizations = new List<Factorization>();
            var results = FactorAvx(targets, arbitrary, ref state);
            for (var i = 0; i < results.Length; i++)
            {
                Factorization result = new();
                var p = results[i];
                var n = targets[i];
                if (p > 1 && p < n)
                {
                    result.Add(n / p, 1);
                    result.Add(p, 1);     
                }
                else
                {
                    result.Add(n, 1);
                }
                factorizations.Add(result);

            }
            return factorizations.ToArray();
        }

        public unsafe ulong[] FactorAvx(ulong[] n, int arbitrary, ref ulong randomstate)
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
            var len = n.Length;

            var results = new ulong[n.Length];
            getfactor_uecm_x8_list(n, ref results, len, ref randomstate);
            return results;
        }




        #region entry points

        void getfactor_uecm_x8_list(uint64_t[] q64, ref uint64_t[] f64, uint32_t num_in, ref uint64_t pran)
        {
            int bits = uecm_get_bits(q64[0]);       // assume all the same size
            uecm_dispatch_x8_list(q64, ref f64, bits, num_in, ref pran);
            return;
        }

        void uecm_dispatch_x8_list(ulong[] n, ref ulong[] f, int targetBits, int num_in, ref ulong ploc_lcg)
        {
            int B1, curves;

            //todo: refactor and make a single call after the curves and b1 are set.
            if (targetBits <= 40)
            {
                B1 = 27;
                curves = 32;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }
            else if (targetBits <= 44)
            {
                B1 = 47;
                curves = 32;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }
            else if (targetBits <= 48)
            {
                B1 = 70;
                curves = 32;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }
            else if (targetBits <= 52)
            {
                B1 = 85;
                curves = 32;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }
            else if (targetBits <= 58)
            {
                B1 = 125;
                curves = 32;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }
            else if (targetBits <= 62)
            {
                B1 = 165;
                curves = 42;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }
            else if (targetBits <= 64)
            {
                B1 = 205;
                curves = 42;
                microecm_x8_list(n, ref f, B1, 25 * B1, curves, num_in, ref ploc_lcg);
            }

            return;
        }

        void microecm_x8_list(ulong[] n64, ref ulong[] f, int B1, int B2, int curves, int num_in, ref ulong ploc_lcg)
        {
            //static void microecm_x8_list(uint64_t* n64, uint64_t* f, uint32_t B1, uint32_t B2,
            //    uint32_t curves, uint32_t num_in, uint64_t* ploc_lcg)
            //{
            //attempt to factor n with the elliptic curve method
            //following brent and montgomery's papers, and CP's book
            int result;
            int msk = 0;
            uint64_t[]
                uarray = new uint64_t[VextSize256],
                rarray = new uint64_t[VextSize256],
                tarray = new uint64_t[VextSize256],
                oarray = new uint64_t[VextSize256],
                narray = new uint64_t[VextSize256],
                carray = new uint64_t[VextSize256],
                fivea = new uint64_t[VextSize256],
                Rsqra = new uint64_t[VextSize256];

            uint32_t stg1_max = B1;
            _m512i v_u = new(), v_s = new(), v_n = new(), v_f = new(), v_r = new(), v_c = new(), v_o = new();
            uecm_pt_x8 v_P = new();
            int i;
            uint32_t j = 0;

            // default (failed) factors
            for (i = 0; i < num_in; i++)
            {
                f[i] = 1;
            }

            v_u = v_n = v_r = v_s = v_c = v_o = v_P.X = v_P.Z = _mm512_setzero_si512();

            while (true)
            {
                int lmsk = 0;

                // this keeps the vectors full
                // seems redundant after the first check
                for (i = 0; i < VextSize256; i++)
                {
                    if (((msk & (1 << i)) == 0) && (j < num_in))
                    {
                        // compute things we need for this new n.
                        uint64_t unityval = (1UL << 52) % n64[j];   // unityval ≡ R  (mod n)
                        uint64_t rho = uecm_multiplicative_inverse52(n64[j]);
                        uint64_t two = uecm_addmod52(unityval, unityval, n64[j]);
                        uint64_t four = uecm_addmod52(two, two, n64[j]);
                        uint64_t five = uecm_addmod52(unityval, four, n64[j]);
                        uint64_t eight = uecm_addmod52(four, four, n64[j]);
                        uint64_t sixteen = uecm_addmod52(eight, eight, n64[j]);

                        // an add, 3 sqrs, and 1 mul versus 3 sqrs and 2 muls
                        // with the sequence 2^4, 2^8, 2^16, 2^32, 2^48, 2^52
                        uint64_t two_5 = uecm_addmod52(sixteen, sixteen, n64[j]);
                        uint64_t two_8 = uecm_sqrredc52(sixteen, n64[j], rho);        // R*2^8         (mod n)
                        uint64_t two_13 = uecm_mulredc52(two_8, two_5, n64[j], rho);  // R*2^13         (mod n)
                        uint64_t two_26 = uecm_sqrredc52(two_13, n64[j], rho);        // R*2^26        (mod n)
                        uint64_t Rsqr = uecm_sqrredc52(two_26, n64[j], rho);          // R*2^52 ≡ R*R  (mod n)
                        uint64_t s;

                        // mark this index for loading into vectors
                        lmsk |= (1 << i);

                        // load the info we need into our vectors.
                        uarray[i] = unityval;
                        rarray[i] = rho;
                        narray[i] = n64[j];
                        carray[i] = 0;
                        oarray[i] = (ulong)j;
                        fivea[i] = five;
                        Rsqra[i] = Rsqr;

                        j++;
                    }
                }

                msk |= lmsk;
                // these are conditionally loaded depending on if
                // the input is newly loaded.
                v_u = _mm512_mask_loadu_epi64(v_u, lmsk, uarray);
                v_n = _mm512_mask_loadu_epi64(v_n, lmsk, narray);
                v_r = _mm512_mask_loadu_epi64(v_r, lmsk, rarray);
                v_c = _mm512_mask_loadu_epi64(v_c, lmsk, carray);
                v_o = _mm512_mask_loadu_epi64(v_o, lmsk, oarray);

                // build the curves
                var vect5 = _mm512_loadu_epi64(fivea);
                var vectSq = _mm512_loadu_epi64(Rsqra);
                v_s = uecm_build_x8(ref v_P, v_r, v_n, ref ploc_lcg, ref tarray, vect5, vectSq);


                // process any factors or errors resulting from modinv
                _m512i lgcd = _mm512_loadu_epi64(tarray);
                _mmask8 m1 = _mm512_cmplt_epi64_mask(lgcd, v_n);
                _mmask8 m2 = _mm512_cmpgt_epi64_mask(lgcd, uecm_set64(1));
#if _INTEL_COMPILER
                _m512i rem = _mm512_rem_epi64(v_n, lgcd);
#else
                for (i = 0; i < VextSize256; i++)
                {
                    tarray[i] = narray[i] % tarray[i];
                }
                _m512i rem = _mm512_loadu_epi64(tarray);
#endif
                _mmask8 m3 = _mm512_cmpeq_epi64_mask(rem, _mm512_setzero_si512());
                _mm512_mask_store_epi64(ref tarray, m2 & msk & m1 & m3, lgcd);
                msk &= (~(m2 & m1 & m3));

                //why didn't they use maskmove for this?
                //if m1, m2, or m3 is zero this check is reduant.
                // This can be refactored using QS mask solving logic.
                for (i = 0; i < VextSize256; i++)
                {
                    if (((m1 & m2 & m3) & (1 << i)) > 0)
                        f[oarray[i]] = tarray[i];
                }
                //in non-vectorized code p is initialized here with curves copied from prebuild
                uecm_stage1_x8(v_r, v_n, ref v_P, (uint64_t)stg1_max, v_s);

                _mm512_storeu_epi64(ref tarray, v_P.Z);
                // this loop can be reduced using vectors to compare t to 0, 1, and n.
                for (i = 0; i < VextSize256; i++)
                {
                    if ((msk & (1 << i)) > 0)
                    {
                        uint64_t t = 0;
                        //APH: optimization
                        if (tarray[i] > 1 && tarray[i] != narray[i])
                        {
                            result = uecm_check_factor(tarray[i], narray[i], ref t);

                            if (result == 1)
                            {
                                f[oarray[i]] = t;
                                msk &= (~(1 << i));
                            }
                        }
                    }
                }

                if (B2 > B1)
                {
                    /*-	stage 1 not initializing pt or vs
                    v_P	{HigginsSoft.Math.Lib.Ecm.uecm_pt_x8}	HigginsSoft.Math.Lib.Ecm.uecm_pt_x8
                    +		X	<0, 0, 0, 0>	System.Runtime.Intrinsics.Vector256<long>
                    +		Z	<0, 0, 0, 0>	System.Runtime.Intrinsics.Vector256<long>
                    */
                    _m512i stg2acc = uecm_stage2_x8(ref v_P, v_r, v_n, stg1_max, v_s, v_u);

                    _mm512_storeu_epi64(ref tarray, stg2acc);

                    for (i = 0; i < VextSize256; i++)
                    {
                        if ((msk & (1 << i)) > 0)
                        {
                            uint64_t t = 0;
                            //APH: optimization
                            if (tarray[i] > 1 && tarray[i] != narray[i])
                            {
                                result = uecm_check_factor(tarray[i], narray[i], ref t);

                                if (result == 1)
                                {
                                    f[oarray[i]] = t;
                                    msk &= (~(1 << i));
                                }
                            }
                        }
                    }
                }

                // if we've exhaused the curves count for anything in
                // the vector, flag it for replacement.
                v_c = _mm512_add_epi64(v_c, uecm_set64(1));
                msk &= _mm512_cmplt_epi64_mask(v_c, uecm_set64(curves));

                if ((j == num_in) && (msk == 0))
                    break;
            }

            return;
        }

        _m512i uecm_build_x8(ref uecm_pt_x8 P, _m512i rho, _m512i n, ref uint64_t ploc_lcg,
               ref uint64_t[] likely_gcd, _m512i five, _m512i Rsqr)
        {
            _m512i t1, t2, t3, t4;
            _m512i u, v, s;
            int i;
            uint64_t[] s8 = new uint64_t[VextSize256];
            uint32_t sigma = uecm_lcg_rand_32B(7, (uint32_t)(-1), ref ploc_lcg);

            for (i = 0; i < VextSize256; i++)
            {
                s8[i] = (uint64_t)sigma;
                sigma = uecm_lcg_rand_32B(7, (uint32_t)(-1), ref ploc_lcg);
            }
            s = _mm512_loadu_epi64(s8);

            u = uecm_mulredc_x8(s, Rsqr, n, rho);  // to_monty(sigma)

            //printf("sigma = %" PRIu64 ", u = %" PRIu64 ", n = %" PRIu64 "\n", sigma, u, n);

            v = uecm_addmod_x8(u, u, n);
            v = uecm_addmod_x8(v, v, n);            // 4*sigma

            //printf("v = %" PRIu64 "\n", v);

            u = uecm_sqrredc_x8(u, n, rho);
            t1 = five;

            //printf("monty(5) = %" PRIu64 "\n", t1);

            u = uecm_submod_x8(u, t1, n);           // sigma^2 - 5

            //printf("u = %" PRIu64 "\n", u);

            t1 = uecm_mulredc_x8(u, u, n, rho);
            _m512i tmpx = uecm_mulredc_x8(t1, u, n, rho);  // u^3

            _m512i v2 = uecm_addmod_x8(v, v, n);             // 2*v
            _m512i v4 = uecm_addmod_x8(v2, v2, n);           // 4*v
            _m512i v8 = uecm_addmod_x8(v4, v4, n);           // 8*v
            _m512i v16 = uecm_addmod_x8(v8, v8, n);          // 16*v
            _m512i t5 = uecm_mulredc_x8(v16, tmpx, n, rho);    // 16*u^3*v

            t1 = uecm_mulredc_x8(v, v, n, rho);
            _m512i tmpz = uecm_mulredc_x8(t1, v, n, rho);  // v^3

            //compute parameter A
            t1 = uecm_submod_x8(v, u, n);           // (v - u)
            t2 = uecm_sqrredc_x8(t1, n, rho);
            t4 = uecm_mulredc_x8(t2, t1, n, rho);   // (v - u)^3

            t1 = uecm_addmod_x8(u, u, n);           // 2u
            t2 = uecm_addmod_x8(u, v, n);           // u + v
            t3 = uecm_addmod_x8(t1, t2, n);         // 3u + v

            t1 = uecm_mulredc_x8(t3, t4, n, rho);   // a = (v-u)^3 * (3u + v)

            // u holds the denom (jeff note: isn't it t5 that has the denom?)
            // t1 holds the numer
            // accomplish the division by multiplying by the modular inverse
            t2 = uecm_set64(1);
            t5 = uecm_mulredc_x8(t5, t2, n, rho);   // take t5 out of monty rep

            uint64_t[] t3_64 = new uint64_t[VextSize256];
            uint64_t[] t5_64 = new uint64_t[VextSize256];
            uint64_t[] n_64 = new uint64_t[VextSize256];
            _mm512_storeu_epi64(ref t5_64, t5);
            _mm512_storeu_epi64(ref n_64, n);
            for (i = 0; i < VextSize256; i++)
            {
                t3_64[i] = uecm_modinv_64(t5_64[i], n_64[i], ref likely_gcd[i]);
            }

            t3 = _mm512_loadu_epi64(t3_64);

            t3 = uecm_mulredc_x8(t3, Rsqr, n, rho); // to_monty(t3)
            _m512i ps = uecm_mulredc_x8(t3, t1, n, rho);

            P.X = tmpx;
            P.Z = tmpz;

            return ps;
        }

        #endregion



        #region stages

        _m512i uecm_stage2_x8(ref uecm_pt_x8 P, _m512i rho, _m512i n,
                     uint32_t stg1_max, _m512i s, _m512i unityval)
        {


            int b;
            int i, j, k;
            uecm_pt_x8 Pa1 = new();
            ref uecm_pt_x8 Pa = ref Pa1;
            uecm_pt_x8[] Pb = new uecm_pt_x8[18];
            uecm_pt_x8 Pd1 = new();
            ref uecm_pt_x8 Pd = ref Pd1;
            int[] barray = null!;
            int numb = 0;

#if MICRO_ECM_VERBOSE_PRINTF
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

            _m512i[] Pbprod = new _m512i[18];

            // [1]Q
            Pb[1] = P;
            Pbprod[1] = uecm_mulredc_x8(Pb[1].X, Pb[1].Z, n, rho);

            // [2]Q
            _m512i diff1 = uecm_submod_x8(P.X, P.Z, n);
            _m512i sum1 = uecm_addmod_x8(P.X, P.Z, n);
            uecm_udup_x8(s, rho, n, sum1, diff1, ref Pb[2]);
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

                    uecm_pt_x8 pt5 = new(), pt6 = new();

            // Calculate all Pb: the following is specialized for MICRO_ECM_PARAM_D=60
            // [2]Q + [1]Q([1]Q) = [3]Q
            uecm_uadd_x8(rho, n, Pb[1], Pb[2], Pb[1], ref Pb[3]);        // <-- temporary

            // 2*[3]Q = [6]Q
            diff1 = uecm_submod_x8(Pb[3].X, Pb[3].Z, n);
            sum1 = uecm_addmod_x8(Pb[3].X, Pb[3].Z, n);
            uecm_udup_x8(s, rho, n, sum1, diff1, ref pt6);   // pt6 = [6]Q

            // [3]Q + [2]Q([1]Q) = [5]Q
            uecm_uadd_x8(rho, n, Pb[3], Pb[2], Pb[1], ref pt5);    // <-- pt5 = [5]Q
            Pb[3] = pt5;

            // [6]Q + [5]Q([1]Q) = [11]Q
            uecm_uadd_x8(rho, n, pt6, pt5, Pb[1], ref Pb[4]);    // <-- [11]Q

            i = 3;
            k = 4;
            j = 5;
            while ((j + 12) < MICRO_ECM_PARAM_D)
            {
                // [j+6]Q + [6]Q([j]Q) = [j+12]Q
                uecm_uadd_x8(rho, n, pt6, Pb[k], Pb[i], ref Pb[map[j + 12]]);
                i = k;
                k = map[j + 12];
                j += 6;
            }

            // [6]Q + [1]Q([5]Q) = [7]Q
            uecm_uadd_x8(rho, n, pt6, Pb[1], pt5, ref Pb[3]);    // <-- [7]Q
            i = 1;
            k = 3;
            j = 1;
            while ((j + 12) < MICRO_ECM_PARAM_D)
            {
                // [j+6]Q + [6]Q([j]Q) = [j+12]Q
                uecm_uadd_x8(rho, n, pt6, Pb[k], Pb[i], ref Pb[map[j + 12]]);
                i = k;
                k = map[j + 12];
                j += 6;
            }

            // Pd = [2w]Q
            // [31]Q + [29]Q([2]Q) = [60]Q
            uecm_uadd_x8(rho, n, Pb[9], Pb[10], Pb[2], ref Pd);   // <-- [60]Q

#if MICRO_ECM_VERBOSE_PRINTF
            ptadds++;
#endif

            // temporary - make [4]Q
            uecm_pt_x8 pt4 = new();
            diff1 = uecm_submod_x8(Pb[2].X, Pb[2].Z, n);
            sum1 = uecm_addmod_x8(Pb[2].X, Pb[2].Z, n);
            uecm_udup_x8(s, rho, n, sum1, diff1, ref pt4);   // pt4 = [4]Q


            // make all of the Pbprod's
            for (i = 3; i < 18; i++)
            {
                Pbprod[i] = uecm_mulredc_x8(Pb[i].X, Pb[i].Z, n, rho);
            }


            //initialize info needed for giant step
            uecm_pt_x8 Pad = new();

            // Pd = [w]Q
            // [17]Q + [13]Q([4]Q) = [30]Q
            uecm_uadd_x8(rho, n, Pb[map[17]], Pb[map[13]], pt4, ref Pad);    // <-- [30]Q

            // [60]Q + [30]Q([30]Q) = [90]Q
            uecm_uadd_x8(rho, n, Pd, Pad, Pad, ref Pa);

            uecm_pt_x8 pt90 = Pa;   // set pt90 = [90]Q
            uecm_pt_x8 pt60 = Pd;   // set pt60 = [60]Q

            // [90]Q + [30]Q([60]Q) = [120]Q
            uecm_uadd_x8(rho, n, Pa, Pad, Pd, ref Pd);

            // [120]Q + [30]Q([90]Q) = [150]Q
            uecm_uadd_x8(rho, n, Pd, Pad, Pa, ref Pa);


            //initialize accumulator
            _m512i acc = unityval;

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

                _m512i pt90prod = uecm_mulredc_x8(pt90.X, pt90.Z, n, rho);

                for (i = 0; i < numsteps; i++)
                {
                    b = steps[i];
                    // accumulate the cross product  (zimmerman syntax).
                    // page 342 in Cref P
                    _m512i tt1 = uecm_submod_x8(pt90.X, Pb[map[b]].X, n);
                    _m512i tt2 = uecm_addmod_x8(pt90.Z, Pb[map[b]].Z, n);
                    _m512i tt3 = uecm_mulredc_x8(tt1, tt2, n, rho);
                    tt1 = uecm_addmod_x8(tt3, Pbprod[map[b]], n);
                    tt2 = uecm_submod_x8(tt1, pt90prod, n);

                    _m512i tmp = uecm_mulredc_x8(acc, tt2, n, rho);
                    _mmask8 m8 = _mm512_cmpgt_epi64_mask(tmp, _mm512_setzero_si512());
                    acc = _mm512_mask_mov_epi64(acc, m8, tmp);
                }
            }
            else if (stg1_max == 70)
            {
                // first process these b's with A=120
                int[] steps = { 49, 47, 41, 37, 31, 23, 19, 17, 13, 11, 7, 29, 43, 53, 59 };
                // we currently have Pd=120

                _m512i pdprod = uecm_mulredc_x8(Pd.X, Pd.Z, n, rho);

                for (i = 0; i < 15; i++)
                {
                    b = steps[i];
                    // accumulate the cross product  (zimmerman syntax).
                    // page 342 in Cref P
                    _m512i tt1 = uecm_submod_x8(Pd.X, Pb[map[b]].X, n);
                    _m512i tt2 = uecm_addmod_x8(Pd.Z, Pb[map[b]].Z, n);
                    _m512i tt3 = uecm_mulredc_x8(tt1, tt2, n, rho);
                    tt1 = uecm_addmod_x8(tt3, Pbprod[map[b]], n);
                    tt2 = uecm_submod_x8(tt1, pdprod, n);

                    _m512i tmp = uecm_mulredc_x8(acc, tt2, n, rho);
                    _mmask8 m8 = _mm512_cmpgt_epi64_mask(tmp, _mm512_setzero_si512());
                    acc = _mm512_mask_mov_epi64(acc, m8, tmp);
                }
            }
            else if (stg1_max == 165)
            {
                // Currently we have Pa=150, Pd=120, Pad=30,  and pt60=60, pt90=90
                // Need Pa = 180, Pd = 120, Pad = 60
                // either of these should be fine
#if false
        // [150]Q + [30]Q([120]Q) = [180]Q
        uecm_uadd_x8(rho, n, *Pa, Pad, *Pd, Pa);
#else
                diff1 = uecm_submod_x8(pt90.X, pt90.Z, n);
                sum1 = uecm_addmod_x8(pt90.X, pt90.Z, n);
                uecm_udup_x8(s, rho, n, sum1, diff1, ref Pa);
#endif
                Pad = pt60;
                // have pa = 180, pd = 120, pad = 60
            }
            else if (stg1_max == 205)
            {
                // Currently we have Pa=150, Pd=120, Pad=30,  and pt60=60, pt90=90
                // need Pa = 210, Pd = 120, Pad = 90

                // [120]Q + [90]Q([30]Q) = [210]Q
                uecm_uadd_x8(rho, n, Pd, pt90, Pad, ref Pa);

                Pad = pt90;
            }

            //initialize Paprod
            _m512i Paprod = uecm_mulredc_x8(Pa.X, Pa.Z, n, rho);

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
                barray = b1_125;
                numb = numb1_125;
            }
            else if (stg1_max == 165)
            {
                barray = b1_165;
                numb = numb1_165;
            }
            else if (stg1_max == 205)
            {
                barray = b1_205;
                numb = numb1_205;
            }

            for (i = 0; i < numb; i++)
            {
                if (barray[i] == 0)
                {
                    //giant step - use the addition formula for ECM
                    uecm_pt_x8 point = Pa;

                    //Pa + Pd
                    uecm_uadd_x8(rho, n, Pa, Pd, Pad, ref Pa);

                    //Pad holds the previous Pa
                    Pad = point;

                    //and Paprod
                    Paprod = uecm_mulredc_x8(Pa.X, Pa.Z, n, rho);

                    i++;
                }

                //we accumulate XrZd - XdZr = (Xr - Xd) * (Zr + Zd) + XdZd - XrZr
                //in CP notation, Pa -> (Xr,Zr), Pb -> (Xd,Zd)

                b = barray[i];
                // accumulate the cross product  (zimmerman syntax).
                // page 342 in Cref P
                _m512i tt1 = uecm_submod_x8(Pa.X, Pb[map[b]].X, n);
                _m512i tt2 = uecm_addmod_x8(Pa.Z, Pb[map[b]].Z, n);
                _m512i tt3 = uecm_mulredc_x8(tt1, tt2, n, rho);
                tt1 = uecm_addmod_x8(tt3, Pbprod[map[b]], n);
                tt2 = uecm_submod_x8(tt1, Paprod, n);

                _m512i tmp = uecm_mulredc_x8(acc, tt2, n, rho);
                _mmask8 m8 = _mm512_cmpgt_epi64_mask(tmp, _mm512_setzero_si512());
                acc = _mm512_mask_mov_epi64(acc, m8, tmp);
            }

            return acc;


        }

        void uecm_stage1_x8(_m512i rho, _m512i n, ref uecm_pt_x8 P, ulong stg1, _m512i s)
        {
            //static void uecm_stage1_x8(_m512i rho, _m512i n, uecm_pt_x8* P, uint64_t stg1, _m512i s)

            uint64_t q;

            // handle the only even case
            q = 2;
            while (q < stg1 * 4)  // jeff: multiplying by 4 improves perf ~1%
            {
                _m512i diff1 = uecm_submod_x8(P.X, P.Z, n);
                _m512i sum1 = uecm_addmod_x8(P.X, P.Z, n);
                uecm_udup_x8(s, rho, n, sum1, diff1, ref P);
                q *= 2;
            }

            if (stg1 == 27)
            {
                uecm_uprac_x8(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 11, 0.580178728295464130, s);
                uecm_uprac_x8(rho, n, ref P, 13, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 17, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 19, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 23, 0.522786351415446049, s);
            }
            else if (stg1 == 47)
            {
                // jeff: improved perf slightly by using one more uprac for 3,
                // and removing uprac for 47.
                uecm_uprac_x8(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 11, 0.580178728295464130, s);
                uecm_uprac_x8(rho, n, ref P, 13, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 17, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 19, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 23, 0.522786351415446049, s);
                uecm_uprac_x8(rho, n, ref P, 29, 0.548409048446403258, s);
                uecm_uprac_x8(rho, n, ref P, 31, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 37, 0.580178728295464130, s);
                uecm_uprac_x8(rho, n, ref P, 41, 0.548409048446403258, s);
                uecm_uprac_x8(rho, n, ref P, 43, 0.618033988749894903, s);
                //        uecm_uprac(rho, n, P, 47, 0.548409048446403258, s);
            }
            else if (stg1 == 59)
            {   // jeff: probably stg1 of 59 would benefit from similar changes
                // as stg1 of 47 above, but I didn't bother. Stg1 of 59 seems to
                // always perform worse than stg1 of 47, so there doesn't seem
                // to be any reason to ever use stg1 of 59.
                uecm_uprac_x8(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac_x8(rho, n, ref P, 3, 0.61803398874989485, s);
                uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 7, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 11, 0.580178728295464130, s);
                uecm_uprac_x8(rho, n, ref P, 13, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 17, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 19, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 23, 0.522786351415446049, s);
                uecm_uprac_x8(rho, n, ref P, 29, 0.548409048446403258, s);
                uecm_uprac_x8(rho, n, ref P, 31, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 1961, 0.552936068843375, s);   // 37 * 53
                uecm_uprac_x8(rho, n, ref P, 41, 0.548409048446403258, s);
                uecm_uprac_x8(rho, n, ref P, 43, 0.618033988749894903, s);
                uecm_uprac_x8(rho, n, ref P, 47, 0.548409048446403258, s);
                uecm_uprac_x8(rho, n, ref P, 59, 0.548409048446403258, s);
            }
            else if (stg1 == 70)
            {
                // call prac with best ratio found in deep search.
                // some composites are cheaper than their
                // constituent primes.
                uecm_uprac70_x8(rho, n, ref P, s);
            }
            else // if (stg1 >= 85)
            {
                uecm_uprac85_x8(rho, n, ref P, s);

                if (stg1 == 85)
                {
                    uecm_uprac_x8(rho, n, ref P, 61, 0.522786351415446049, s);
                }
                else
                {
                    uecm_uprac_x8(rho, n, ref P, 5, 0.618033988749894903, s);
                    uecm_uprac_x8(rho, n, ref P, 11, 0.580178728295464130, s);
                    //            uecm_uprac(rho, n, P, 61, 0.522786351415446049, s);
                    uecm_uprac_x8(rho, n, ref P, 89, 0.618033988749894903, s);
                    uecm_uprac_x8(rho, n, ref P, 97, 0.723606797749978936, s);
                    uecm_uprac_x8(rho, n, ref P, 101, 0.556250337855490828, s);
                    uecm_uprac_x8(rho, n, ref P, 107, 0.580178728295464130, s);
                    uecm_uprac_x8(rho, n, ref P, 109, 0.548409048446403258, s);
                    uecm_uprac_x8(rho, n, ref P, 113, 0.618033988749894903, s);

                    if (stg1 == 125)
                    {
                        // jeff: moved 61 to here
                        uecm_uprac_x8(rho, n, ref P, 61, 0.522786351415446049, s);
                        uecm_uprac_x8(rho, n, ref P, 103, 0.632839806088706269, s);
                    }
                    else
                    {
                        uecm_uprac_x8(rho, n, ref P, 7747, 0.552188778811121, s); // 61 x 127
                        uecm_uprac_x8(rho, n, ref P, 131, 0.618033988749894903, s);
                        uecm_uprac_x8(rho, n, ref P, 14111, 0.632839806088706, s);  // 103 x 137
                        uecm_uprac_x8(rho, n, ref P, 20989, 0.620181980807415, s);  // 139 x 151
                        uecm_uprac_x8(rho, n, ref P, 157, 0.640157392785047019, s);
                        uecm_uprac_x8(rho, n, ref P, 163, 0.551390822543526449, s);

                        if (stg1 == 165)
                        {
                            uecm_uprac_x8(rho, n, ref P, 149, 0.580178728295464130, s);
                        }
                        else
                        {
                            uecm_uprac_x8(rho, n, ref P, 13, 0.618033988749894903, s);
                            uecm_uprac_x8(rho, n, ref P, 167, 0.580178728295464130, s);
                            uecm_uprac_x8(rho, n, ref P, 173, 0.612429949509495031, s);
                            uecm_uprac_x8(rho, n, ref P, 179, 0.618033988749894903, s);
                            uecm_uprac_x8(rho, n, ref P, 181, 0.551390822543526449, s);
                            uecm_uprac_x8(rho, n, ref P, 191, 0.618033988749894903, s);
                            uecm_uprac_x8(rho, n, ref P, 193, 0.618033988749894903, s);
                            uecm_uprac_x8(rho, n, ref P, 29353, 0.580178728295464, s);  // 149 x 197
                            uecm_uprac_x8(rho, n, ref P, 199, 0.551390822543526449, s);
                        }
                    }
                }
            }
            return;


        }

        #endregion



        #region upprac

        static void uecm_uprac70_x8(_m512i rho, _m512i n, ref uecm_pt_x8 P, _m512i s)
        {
            _m512i s1, s2, d1, d2;
            _m512i swp;
            int i;
            uint8_t[] steps = {
                0,6,0,6,0,6,0,4,6,0,4,6,0,4,4,6,
                0,4,4,6,0,5,4,6,0,3,3,4,6,0,3,5,
                4,6,0,3,4,3,4,6,0,5,5,4,6,0,5,3,
                3,4,6,0,3,3,4,3,4,6,0,5,3,3,3,3,
                3,3,3,3,4,3,3,4,6,0,5,4,3,3,4,6,
                0,3,4,3,5,4,6,0,5,3,3,3,4,6,0,5,
                4,3,5,4,6,0,5,5,3,3,4,6,0,4,3,3,
                3,5,4,6 };

            uecm_pt_x8 pt1 = new(), pt2 = new(), pt3 = new();
            for (i = 0; i < 116; i++)
            {
                if (steps[i] == 0)
                {
                    pt1.X = pt2.X = pt3.X = P.X;
                    pt1.Z = pt2.Z = pt3.Z = P.Z;

                    d1 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod_x8(pt1.X, pt1.Z, n);
                    uecm_udup_x8(s, rho, n, s1, d1, ref pt1);
                }
                else if (steps[i] == 3)
                {
                    // integrate step 4 followed by swap(1,2)
                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

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
                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

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
                    d2 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod_x8(pt1.X, pt1.Z, n);

                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt2);        // B = B + A (C)
                    uecm_udup_x8(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (steps[i] == 6)
                {
                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref P);     // A = A + B (C)
                }
            }
            return;
        }

        static void uecm_uprac85_x8(_m512i rho, _m512i n, ref uecm_pt_x8 P, _m512i s)
        {
            _m512i s1, s2, d1, d2;
            _m512i swp;
            int i;
            uint8_t[] steps = {
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

            uecm_pt_x8 pt1 = new(), pt2 = new(), pt3 = new();
            for (i = 0; i < 146; i++)
            {
                if (steps[i] == 0)
                {
                    pt1.X = pt2.X = pt3.X = P.X;
                    pt1.Z = pt2.Z = pt3.Z = P.Z;

                    d1 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod_x8(pt1.X, pt1.Z, n);
                    uecm_udup_x8(s, rho, n, s1, d1, ref pt1);
                }
                else if (steps[i] == 3)
                {
                    // integrate step 4 followed by swap(1,2)
                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

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
                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

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
                    d2 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod_x8(pt1.X, pt1.Z, n);

                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt2);        // B = B + A (C)
                    uecm_udup_x8(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (steps[i] == 6)
                {
                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref P);     // A = A + B (C)
                }
            }
            return;
        }

        static void uecm_uprac_x8(_m512i rho, _m512i n, ref uecm_pt_x8 P, uint64_t c, double v, _m512i s)
        {
            uint64_t d, e, r;
            int i;
            _m512i s1, s2, d1, d2;
            _m512i swp;

            // we require c != 0
            int shift = _trail_zcnt64(c);
            c = c >> shift;

            d = c;
            r = (uint64_t)((double)d * v + 0.5);

            d = c - r;
            e = 2 * r - c;

            uecm_pt_x8 pt1, pt2, pt3;

            // the first one is always a doubling
            // point1 is [1]P
            pt1.X = pt2.X = pt3.X = P.X;
            pt1.Z = pt2.Z = pt3.Z = P.Z;

            d1 = uecm_submod_x8(pt1.X, pt1.Z, n);
            s1 = uecm_addmod_x8(pt1.X, pt1.Z, n);

            // point2 is [2]P
            uecm_udup_x8(s, rho, n, s1, d1, ref pt1);

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

                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref pt4); // T = A + B (C)
                    uecm_pt_x8 pt5 = new();
                    uecm_uadd_x8(rho, n, pt4, pt1, pt2, ref pt5); // T2 = T + A (B)
                    uecm_uadd_x8(rho, n, pt2, pt4, pt1, ref pt2); // B = B + T (A)

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

                    d1 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod_x8(pt1.X, pt1.Z, n);

                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref pt2);        // B = A + B (C)
                    uecm_udup_x8(s, rho, n, s1, d1, ref pt1);        // A = 2A
                }
                else if ((d + 3) / 4 <= e)
                {
                    d -= e;

                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt4);        // T = B + A (C)

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

                    d2 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod_x8(pt1.X, pt1.Z, n);

                    uecm_uadd_x8(rho, n, pt2, pt1, pt3, ref pt2);        // B = B + A (C)
                    uecm_udup_x8(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (d % 2 == 0)
                {
                    d /= 2;

                    d2 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod_x8(pt1.X, pt1.Z, n);

                    uecm_uadd_x8(rho, n, pt3, pt1, pt2, ref pt3);        // C = C + A (B)
                    uecm_udup_x8(s, rho, n, s2, d2, ref pt1);        // A = 2A
                }
                else if (d % 3 == 0)
                {
                    d = d / 3 - e;

                    d1 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s1 = uecm_addmod_x8(pt1.X, pt1.Z, n);

                    uecm_pt_x8 pt4 = new();
                    uecm_udup_x8(s, rho, n, s1, d1, ref pt4);        // T = 2A
                    uecm_pt_x8 pt5 = new();
                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref pt5);        // T2 = A + B (C)
                    uecm_uadd_x8(rho, n, pt4, pt1, pt1, ref pt1);        // A = T + A (A)
                    uecm_uadd_x8(rho, n, pt4, pt5, pt3, ref pt4);        // T = T + T2 (C)

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

                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref pt4);        // T = A + B (C)


                    d2 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod_x8(pt1.X, pt1.Z, n);
                    uecm_uadd_x8(rho, n, pt4, pt1, pt2, ref pt2);        // B = T + A (B)
                    uecm_udup_x8(s, rho, n, s2, d2, ref pt4);        // T = 2A
                    uecm_uadd_x8(rho, n, pt1, pt4, pt1, ref pt1);        // A = A + T (A) = 3A
                }
                else if ((d - e) % 3 == 0)
                {
                    d = (d - e) / 3;

                    uecm_pt_x8 pt4 = new();
                    uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref pt4);        // T = A + B (C)

                    d2 = uecm_submod_x8(pt1.X, pt1.Z, n);
                    s2 = uecm_addmod_x8(pt1.X, pt1.Z, n);
                    uecm_uadd_x8(rho, n, pt3, pt1, pt2, ref pt3);        // C = C + A (B)

                    swp = pt2.X;
                    pt2.X = pt4.X;
                    pt4.X = swp;
                    swp = pt2.Z;
                    pt2.Z = pt4.Z;
                    pt4.Z = swp;

                    uecm_udup_x8(s, rho, n, s2, d2, ref pt4);        // T = 2A
                    uecm_uadd_x8(rho, n, pt1, pt4, pt1, ref pt1);        // A = A + T (A) = 3A
                }
                else
                {
                    e /= 2;

                    d2 = uecm_submod_x8(pt2.X, pt2.Z, n);
                    s2 = uecm_addmod_x8(pt2.X, pt2.Z, n);

                    uecm_uadd_x8(rho, n, pt3, pt2, pt1, ref pt3);        // C = C + B (A)
                    uecm_udup_x8(s, rho, n, s2, d2, ref pt2);        // B = 2B
                }
            }
            uecm_uadd_x8(rho, n, pt1, pt2, pt3, ref P);     // A = A + B (C)

            for (i = 0; i < shift; i++)
            {
                d1 = uecm_submod_x8(P.X, P.Z, n);
                s1 = uecm_addmod_x8(P.X, P.Z, n);
                uecm_udup_x8(s, rho, n, s1, d1, ref P);     // P = 2P
            }
            return;
        }

        #endregion




        //#define uecm_and64 _mm512_and_epi64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_and_epi64(_m512i a, _m512i b) => Avx2.And(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_and64(_m512i a, _m512i b) => _mm512_and_epi64(a, b);

        //#define uecm_storeu64 _mm512_store_epi64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void _mm512_store_epi64(long* a, _m512i b) => Avx.Store(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void uecm_storeu64(long* a, _m512i b) => _mm512_store_epi64(a, b);

        //define uecm_add64: _mm512_add_epi64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_add_epi64(_m512i a, _m512i b) => Avx2.Add(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_add64(_m512i a, _m512i b) => _mm512_add_epi64(a, b);

        // #define uecm_sub64 _mm512_sub_epi64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_sub_epi64(_m512i a, _m512i b) => Avx2.Subtract(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_sub64(_m512i a, _m512i b) => _mm512_sub_epi64(a, b);

        //#define uecm_set64 _mm512_set1_epi64
        //extern _m512i __cdecl _mm512_set1_epi64(__int64);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_set1_epi64(long value) => Vector256.Create(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_set64(long value) => _mm512_set1_epi64(value);

        //#define uecm_srli64 _mm512_srli_epi64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_srli_epi64(_m512i a, int count) => Avx2.ShiftRightLogical(a, (byte)count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_srli64(_m512i a, int count) => _mm512_srli_epi64(a, (byte)count);

        //#define uecm_loadu64 _mm512_load_epi64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe _m512i _mm512_load_epi64(long* address) => Avx.LoadVector256(address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe _m512i uecm_loadu64(long* address) => _mm512_load_epi64(address);

        // #define uecm_castpd _mm512_castsi512_pd
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512d _mm512_castsi512_pd(_m512i a) => Vector256.As<long, double>(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512d uecm_castpd(_m512i a) => _mm512_castsi512_pd(a);

        //#define uecm_castepu _mm512_castpd_si512
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_castpd_si512(_m512d a) => Vector256.As<double, long>(a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_castepu(_m512d a) => _mm512_castpd_si512(a);

        /*
#if GCC_VERSION < 95000
#define _mm512_loadu_epi64 _mm512_load_epi64
#define _mm512_storeu_epi64 _mm512_store_epi64
#endif
        */
        //#define uecm_castepu _mm512_castpd_si512
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe _m512i _mm512_loadu_epi64(uint64_t[] a)
        {
            fixed (ulong* ptr = a)
            {
                return Avx.LoadVector256((long*)ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void _mm512_store_epi64(ref long[] tarray, _m512i z)
        {
            fixed (long* ptr = tarray)
            {
                Avx2.Store((long*)ptr, z);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void _mm512_storeu_epi64(ref ulong[] tarray, _m512i z)
        {
            fixed (ulong* ptr = tarray)
            {
                Avx2.Store((long*)ptr, z);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_setzero_si512() => Vector256.Create(0L);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void uecm_udup_x8(_m512i s, _m512i rho, _m512i n, _m512i insum, _m512i indiff, ref uecm_pt_x8 P)
        {
            _m512i tt1 = uecm_sqrredc_x8(indiff, n, rho);          // U=(x1 - z1)^2
            _m512i tt2 = uecm_sqrredc_x8(insum, n, rho);           // V=(x1 + z1)^2
            P.X = uecm_mulredc_x8(tt1, tt2, n, rho);         // x=U*V

            _m512i tt3 = uecm_submod_x8(tt2, tt1, n);          // w = V-U
            tt2 = uecm_mulredc_x8(tt3, s, n, rho);      // w = (A+2)/4 * w
            tt2 = uecm_addmod_x8(tt2, tt1, n);          // w = w + U
            P.Z = uecm_mulredc_x8(tt2, tt3, n, rho);         // Z = w*(V-U)
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void uecm_uadd_x8(_m512i rho, _m512i n, uecm_pt_x8 P1, uecm_pt_x8 P2, uecm_pt_x8 Pin, ref uecm_pt_x8 Pout)
        {
            // compute:
            //x+ = z- * [(x1-z1)(x2+z2) + (x1+z1)(x2-z2)]^2
            //z+ = x- * [(x1-z1)(x2+z2) - (x1+z1)(x2-z2)]^2
            // where:
            //x- = original x
            //z- = original z
            // given the sums and differences of the original points
            _m512i diff1 = uecm_submod_x8(P1.X, P1.Z, n);
            _m512i sum1 = uecm_addmod_x8(P1.X, P1.Z, n);
            _m512i diff2 = uecm_submod_x8(P2.X, P2.Z, n);
            _m512i sum2 = uecm_addmod_x8(P2.X, P2.Z, n);

            _m512i tt1 = uecm_mulredc_x8(diff1, sum2, n, rho); //U
            _m512i tt2 = uecm_mulredc_x8(sum1, diff2, n, rho); //V

            _m512i tt3 = uecm_addmod_x8(tt1, tt2, n);
            _m512i tt4 = uecm_submod_x8(tt1, tt2, n);
            tt1 = uecm_sqrredc_x8(tt3, n, rho);   //(U + V)^2
            tt2 = uecm_sqrredc_x8(tt4, n, rho);   //(U - V)^2

            _m512i tmpx = uecm_mulredc_x8(tt1, Pin.Z, n, rho);     //Z * (U + V)^2
            _m512i tmpz = uecm_mulredc_x8(tt2, Pin.X, n, rho);     //x * (U - V)^2
            Pout.X = tmpx;
            Pout.Z = tmpz;

            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_sqrredc_x8(_m512i x, _m512i N, _m512i invN)
        {
            // invN is the positive variant = 0 - nhat (the standard negative inverse)
            _m512i T_hi; // = uecm_mul52hi(x, y, dbias, vbias1);
            _m512i T_lo; // = uecm_mul52lo(x, y);
            uecm_mul52lohi(x, x, out T_lo, out T_hi);
            _m512i m = _mm512_and_si512(_mm512_mullo_epi64(T_lo, invN), lo52mask);
            _m512i mN_hi = uecm_mul52hi(m, N);
            _m512i tmp = uecm_add64(T_hi, N);
            tmp = uecm_sub64(tmp, mN_hi);
            _m512i result = uecm_sub64(T_hi, mN_hi);
            _mmask8 msk = _mm512_cmplt_epu64_mask(T_hi, mN_hi);
            return _mm512_mask_mov_epi64(result, msk, tmp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_mulredc_x8(_m512i x, _m512i y, _m512i N, _m512i invN)
        {
            // invN is the positive variant = 0 - nhat (the standard negative inverse)
            _m512i T_hi; // = uecm_mul52hi(x, y, dbias, vbias1);
            _m512i T_lo; // = uecm_mul52lo(x, y);
            uecm_mul52lohi(x, y, out T_lo, out T_hi);
            _m512i m = _mm512_and_si512(_mm512_mullo_epi64(T_lo, invN), lo52mask);
            _m512i mN_hi = uecm_mul52hi(m, N);
            _m512i tmp = uecm_add64(T_hi, N);
            tmp = uecm_sub64(tmp, mN_hi);
            _m512i result = uecm_sub64(T_hi, mN_hi);               // result is (T - mN) >> 52
            int msk = _mm512_cmplt_epu64_mask(T_hi, mN_hi);    // unless T_hi < mN_hi
            return _mm512_mask_mov_epi64(result, msk, tmp);         // then return (T_hi + N - mN_hi)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_addmod_x8(_m512i x, _m512i y, _m512i N)
        {
            _m512i t = uecm_add64(x, y);
            _mmask8 m = _mm512_cmpge_epu64_mask(t, N);
            return _mm512_mask_sub_epi64(t, m, t, N);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_submod_x8(_m512i x, _m512i y, _m512i N)
        {
            _m512i t = uecm_sub64(x, y);
            _mmask8 m = _mm512_cmpgt_epu64_mask(y, x);
            return _mm512_mask_add_epi64(t, m, t, N);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe static _m512i _mm512_mask_add_epi64(_m512i src, int mask, _m512i a, _m512i b)
        {
            /*
                FOR j := 0 to 7
                    i := j*64
                    IF k[j]
                        dst[i+63:i] := a[i+63:i] + b[i+63:i]
                    ELSE
                        dst[i+63:i] := src[i+63:i]
                    FI
                ENDFOR
                dst[MAX:512] := 0
            */

            var len = _m512i.Count;
            var result = stackalloc long[len];
            for (var j = 0; j < len; j++)
            {
                if ((mask & (1 << j)) > 0)
                {
                    result[j] = a.GetElement(j) + b.GetElement(j);
                }
                else
                {
                    result[j] = src.GetElement(j);
                }

            }
            return Avx.LoadVector256(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe static _m512i _mm512_mask_sub_epi64(_m512i src, int mask, _m512i a, _m512i b)
        {
            /*
             * FOR j := 0 to 7
                i := j*64
                IF k[j]
                    dst[i+63:i] := a[i+63:i] - b[i+63:i]
                ELSE
                    dst[i+63:i] := src[i+63:i]
                FI	
            ENDFOR
            dst[MAX:512] := 0
            */
            var len = _m512i.Count;
            var result = stackalloc long[len];
            for (var j = 0; j < len; j++)
            {
                if ((mask & (1 << j)) > 0)
                {
                    result[j] = a.GetElement(j) - b.GetElement(j);
                }
                else
                {
                    result[j] = src.GetElement(j);
                }

            }
            return Avx.LoadVector256(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#text=_mm512_mask_mov_epi64&ig_expand=6551,4574&techs=AVX_512
        static unsafe _m512i _mm512_mask_mov_epi64(_m512i src, int mask, _m512i a)
        {
            /*
                      FOR j := 0 to 7
                i := j*64
                IF k[j]
                    dst[i+63:i] := a[i+63:i]
                ELSE
                    dst[i+63:i] := src[i+63:i]
                FI
            ENDFOR
            dst[MAX:512] := 0 
              */
            var len = _m512i.Count;
            var result = stackalloc long[len];
            for (var j = 0; j < len; j++)
            {
                if ((mask & (1 << j)) > 0)
                {
                    result[j] = a.GetElement(j);
                }
                else
                {
                    result[j] = src.GetElement(j);
                }

            }
            return Avx.LoadVector256(result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i uecm_mul52hi(_m512i b, _m512i c)
        {
            _m512d prod1_ld = _mm512_cvtepu64_pd(b);
            _m512d prod2_ld = _mm512_cvtepu64_pd(c);
            prod1_ld = _mm512_fmadd_round_pd(prod1_ld, prod2_ld, dbias);
            return _mm512_sub_epi64(uecm_castepu(prod1_ld), vbias1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_and_si512(_m512i a, _m512i b)
        {
            return Avx2.And(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512i _mm512_mullo_epi64(_m512i a, _m512i b)
        {
            /*
             * FOR j := 0 to 7
                    i := j*64
                    tmp[127:0] := a[i+63:i] * b[i+63:i]
                    dst[i+63:i] := tmp[63:0]
                ENDFOR
                dst[MAX:512] := 0*/
            var t = a.AsVector<long>();
            var u = b.AsVector<long>();
            var result = t * u;
            return result.AsVector256();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void uecm_mul52lohi(_m512i b, _m512i c, out _m512i l, out _m512i h)
        {
            _m512d prod1_ld = _mm512_cvtepu64_pd(b);
            _m512d prod2_ld = _mm512_cvtepu64_pd(c);
            //_m512d prod1_hd = _mm512_fmadd_round_pd(prod1_ld, prod2_ld, dbias, (_MM_FROUND_TO_ZERO | _MM_FROUND_NO_EXC));
            _m512d prod1_hd = _mm512_fmadd_round_pd(prod1_ld, prod2_ld, dbias);
            h = _mm512_sub_epi64(uecm_castepu(prod1_hd), vbias1);
            prod1_hd = _mm512_sub_pd(_mm512_castsi512_pd(vbias2), prod1_hd);
            prod1_ld = _mm512_fmadd_round_pd(prod1_ld, prod2_ld, prod1_hd);
            l = _mm512_castpd_si512(prod1_ld);
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512d _mm512_sub_pd(_m512d a, _m512d b) => Avx.Subtract(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _m512d _mm512_fmadd_round_pd(_m512d a, _m512d b, _m512d bias)
            => Avx.RoundToZero(Avx.Add(Avx.Add(a, b), bias));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe _m512d _mm512_cvtepu64_pd(_m512i a)
        {
            //var lower = a.GetLower();
            //var higher = a.GetUpper();
            //_m512d dLower=  Avx.ConvertToVector256Double(lower);
            //var s = Vector256.Create(lower, higher);
            //avx.ConvertToVector128Double(lower); <=methods don't take 64 bit inputs.
            var len = _m512i.Count;
            var result = stackalloc double[len];
            for (var i = 0; i < len; i++)
            {
                result[i] = (double)a.GetElement(i);
            }
            return Avx.LoadVector256(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _mmask8 _mm512_cmpeq_epi64_mask(_m512i a, _m512i b)
        {
            var left = _mm512_castsi512_pd(a);
            var right = _mm512_castsi512_pd(b);
            var comp = Avx2.CompareEqual(left, right);
            var mask = Avx2.MoveMask(comp);
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _mmask8 _mm512_cmpge_epu64_mask(_m512i a, _m512i b)
        {
            var left = _mm512_castsi512_pd(a);
            var right = _mm512_castsi512_pd(b);
            var comp = Avx2.CompareGreaterThanOrEqual(left, right);
            var mask = Avx2.MoveMask(comp);
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _mmask8 _mm512_cmpgt_epi64_mask(_m512i a, _m512i b)
        {
            var left = _mm512_castsi512_pd(a);
            var right = _mm512_castsi512_pd(b);
            var comp = Avx2.CompareGreaterThan(left, right);
            var mask = Avx2.MoveMask(comp);
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _mmask8 _mm512_cmplt_epi64_mask(_m512i a, _m512i b)
        {
            var left = _mm512_castsi512_pd(a);
            var right = _mm512_castsi512_pd(b);
            var comp = Avx2.CompareLessThan(left, right);
            var mask = Avx2.MoveMask(comp);
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _mmask8 _mm512_cmplt_epu64_mask(_m512i a, _m512i b)
        {
            var left = _mm512_castsi512_pd(a);
            var right = _mm512_castsi512_pd(b);
            var comp = Avx2.CompareLessThan(left, right);
            var mask = Avx2.MoveMask(comp);
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static _mmask8 _mm512_cmpgt_epu64_mask(_m512i a, _m512i b)
        {
            var left = _mm512_castsi512_pd(a);
            var right = _mm512_castsi512_pd(b);
            var comp = Avx2.CompareGreaterThan(left, right);
            var mask = Avx2.MoveMask(comp);
            return mask;
        }

        /*
         * https://www.felixcloutier.com/x86/movdqa:vmovdqa32:vmovdqa64
        (KL, VL) = (2, 128), (4, 256), (8, 512)
        FOR j := 0 TO KL-1
            i := j* 64
            IF k1[j] OR* no writemask*
                THEN DEST[i + 63:i] := SRC[i + 63:i]
                ELSE
                    IF* merging-masking*
                        THEN* DEST[i + 63:i] remains unchanged*
                        ELSE DEST[i + 63:i] := 0 ; zeroing-masking
                    FI
            FI;
        ENDFOR
        DEST[MAXVL - 1:VL] := 0
        */
        /// <summary>_mm512_mask_loadu_epi64
        /// Loads int64 vector. Corresponding instruction is VMOVDQA64. This intrinsic only applies to Intel® Many Integrated Core Architecture (Intel® MIC Architecture).
        /// </summary>
        /// <param name="v_u">Source vector that retains old values of the destination vector; 
        /// the resulting vector gets corresponding elements from v1_old for zero mask bits</param>
        /// <param name="lmsk">
        /// Writemask; only those elements of the source vectors with corresponding
        /// bit set to '1' in the k1 mask are computed and stored in the result; 
        /// elements in the result vector corresponding to zero bit in k1 are 
        /// copied from corresponding elements of vector v1_old</param>
        /// <param name="uarray">	
        /// memory address to load from</param>
        /// <remarks>Loads eight 64-bit integer values from memory address mt into int64 vector. 
        /// The address mt must be 64-byte-aligned. In the masked variant, only those elements 
        /// with the corresponding bit set in vector mask register k1 are computed.
        /// Elements in resulting vector with the corresponding bit clear in k1 obtain values from the v1_old vector.
        /// </remarks>
        /// <returns>Returns the result of the load operation.</returns>
        /// (KL, VL) = (2, 128), (4, 256), (8, 512)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe _m512i _mm512_mask_loadu_epi64(_m512i v_u, int lmsk, ulong[] uarray)
        {
            //TODO: figure out correct avx256 instruction
            //Avx2.MaskLoad(
            var dest = stackalloc long[_m512i.Count];
            Avx2.Store(dest, v_u);
            for (var j = 0; j < _m512i.Count; j++)
            {
                if ((lmsk & (1 << j)) > 0)
                {
                    dest[j] = (long)uarray[j];
                }
            }
            return _mm512_load_epi64(dest);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void _mm512_mask_store_epi64(ref ulong[] dest, int mask, _m512i src)
        {
            for (var j = 0; j < _m512i.Count; j++)
            {
                if ((mask & (1 << j)) > 0)
                {
                    dest[j] = (ulong)src.GetElement(j);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_sqrredc52(uint64_t x, uint64_t N, uint64_t invN)
        {
            return uecm_mulredc52(x, x, N, invN);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_mulredc52(uint64_t x, uint64_t y, uint64_t N, uint64_t invN)
        {
#if (_MSC_VER)
            uint64_t T_hi = 0;
            uint64_t T_lo = _umul128(x, y, ref T_hi);
            uint64_t m = T_lo * invN;
            uint64_t mN_hi = _umulh(m, N);
#else
            __uint128_t prod = (__uint128_t)x * y;
            uint64_t T_hi = (uint64_t)(prod >> 52);
            uint64_t T_lo = (uint64_t)(prod & 0x000fffffffffffffull);
            uint64_t m = (T_lo * invN) & 0x000fffffffffffffull;
            __uint128_t mN = (__uint128_t)m * N;
            uint64_t mN_hi = (uint64_t)(mN >> 52);
#endif
            uint64_t tmp = T_hi + N;
#if MICRO_ECM_ALT_MULREDC_USE_INLINE_ASM_X86
    __asm__(
        "subq %[mN_hi], %[tmp] \n\t"    /* tmp = T_hi + N - mN_hi */
        "subq %[mN_hi], %[T_hi] \n\t"   /* T_hi = T_hi - mN_hi */
        "cmovaeq %[T_hi], %[tmp] \n\t"  /* tmp = (T_hi >= mN_hi) ? T_hi : tmp */
        : [tmp] "+&r"(tmp), [T_hi]"+&r"(T_hi)
        : [mN_hi] "r"(mN_hi)
        : "cc");
    uint64_t result = tmp;
#else
            tmp = tmp - mN_hi;
            uint64_t result = T_hi - mN_hi;
            result = (T_hi < mN_hi) ? tmp : result;
#endif
            return result & 0x000ffffffffffffful;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_multiplicative_inverse52(uint64_t a)
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
            return x4 & 0x000ffffffffffffful;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_submod52(uint64_t a, uint64_t b, uint64_t n)
        {
            uint64_t r0 = 0;
            if (Ecm._subborrow_u64(0, a, b, ref r0))
                r0 += n;
            return r0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint64_t uecm_addmod52(uint64_t x, uint64_t y, uint64_t n)
        {
            // FYI: The clause above often compiles with a branch in MSVC.
            // The statement below often compiles without a branch (uses cmov) in MSVC.
            return (x >= n - y) ? x - (n - y) : x + y;
        }
    }

}
#endif