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
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using uint64_t = System.UInt64;
using uint32_t = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using static MathGmp.Native.gmp_lib;
using yfactor_list_t = HigginsSoft.Math.Lib.Factorization;
using System.Security.Cryptography.X509Certificates;
using static HigginsSoft.Math.Lib.MathLib;

namespace HigginsSoft.Math.Lib
{
    public class Fermat
    {
        public void TestFermat()
        {

        }

        public static bool Factor(GmpInt n, out GmpInt p, out GmpInt q, int maxIterations = 1000)
        {
            p = 0; q = 0;
            if (n < 4)
            {
                p = n;
                q = n / p;
                return n < 2;
            }

            if (n.IsEven)
            {
                q = n >> 1;
                p = n;
                return false;
            }

            if (MathLib.IsPerfectSquare(n, out GmpInt a))
            {
                p = q = a;
                return false;
            }


            a += 1;

            var b2 = a * a - n;
            var b = MathLib.Sqrt(b2);
            bool noFactor = b * b != b2;
            for (var i = 0; noFactor && i < maxIterations; i++)
            {
                a++;
                b2 = a * a - n;
                b = MathLib.Sqrt(b2);
                noFactor = b * b != b2;
            }

            if (!noFactor)
            {
                q = a + b;
                p = a - b;
                return n == q || n == q;
            }


            return false;

        }

        public static bool FermatFactorization(int n, out int p, out int q, int maxIterations = 1000)
        {
            p = 0;
            q = 0;
            if (n < 2)
            {
                p = n;
                return false;
            }
            if ((n & 1) == 0)
            {
                // If the number is even, one factor is 2 and the other factor is n/2
                p = 2;
                q = n >> 1;
                return n == 2;
            }

            int a = (int)MathLib.Ceiling(MathLib.Sqrt(n));

            int b2 = a * a - n;
            int b = (int)MathLib.Sqrt(b2);
            bool noFactor = b * b != b2;
            for (var i = 0; noFactor && i < maxIterations; i++)
            {
                a++;
                b2 = a * a - n;
                b = (int)MathLib.Sqrt(b2);
                noFactor = b * b != b2;
            }

            if (!noFactor)
            {
                q = a + b;
                p = a - b;
                return n == q || n == q;
            }


            return false;
        }

        public static FactorizationState StartResumable(int n, int maxIterations = 1000)
        {
            var state = new FactorizationState(n: n);

            if (n < 2)
            {
                state.P = n;
                state.HasFactor = false;
                return state;
            }
            if ((n & 1) == 0)
            {
                // If the number is even, one factor is 2 and the other factor is n/2
                state.P = 2;
                state.Q = n >> 1;
                state.HasFactor = n == 2;
                return state;
            }

            state.A = (int)MathLib.Ceiling(MathLib.Sqrt(n));

            state.B2 = state.A * state.A - n;
            state.B = (int)MathLib.Sqrt(state.B2);
            bool noFactor = state.B * state.B != state.B2;

            if (!noFactor)
            {
                state.Q = state.A + state.B;
                state.P = state.A - state.B;
                state.HasFactor = n == state.Q || n == state.P;
            }
            else
            {
                Resume(state);
            }
            return state;
        }

        public static bool Resume(FactorizationState state, int maxIterations = 1000)
        {
            if (state.HasFactor) { return state.HasFactor; }


            //ref int p = ref state.P;
            //ref int q = ref state.Q;
            //ref int a = ref state.A;

            //ref int b2 = ref state.B2;
            //ref int b = ref state.B;


            int p = state.P;
            int q = state.Q;
            int a = state.A;

            int b2 = state.B2;
            int b = state.B;

            int n = state.N;
            bool noFactor = true;
            int i;
            for (i = 0; noFactor && i < maxIterations; i++)
            {
                a++;
                b2 = a * a - n;
                b = (int)MathLib.Sqrt(b2);
                noFactor = b * b != b2;
            }
            state.Iterations += i;
            if (!noFactor)
            {
                q = a + b;
                p = a - b;
                state.HasFactor = n == p || n == q;
            }

            state.P = p;
            state.Q = q;
            state.A = a;

            state.B2 = b2;
            state.B = b;


            return state.HasFactor;
        }

        public class FactorizationState
        {
            public int N;
            public int Sqrt;
            public int A;
            public int B2;
            public int B;
            public int P;
            public int Q;
            public long Iterations;
            public bool HasFactor;

            public FactorizationState(int n)
            {
                N = n;

            }
        }


        public static FactorizationState<GmpInt> StartResumable(GmpInt n, int maxIterations = 1000)
        {
            var state = new FactorizationState<GmpInt>(n: n);

            if (n < 2)
            {
                state.P = n;
                state.HasFactor = false;
                return state;
            }
            if (n < 4)
            {
                state.P = n;
                state.HasFactor = true;
                return state;
            }

            if ((n & 1) == 0)
            {
                // If the number is even, one factor is 2 and the other factor is n/2
                state.P = 2;
                state.Q = n >> 1;
                state.HasFactor = true;
                return state;
            }

            if (MathLib.IsPerfectSquare(n, out GmpInt sqrt))
            {
                state.P = sqrt;
                state.Q = sqrt;
                state.HasFactor = true;
                return state;
            }
            state.A = sqrt + 1;

            state.B2 = state.A * state.A - n;
            state.B = MathLib.Sqrt(state.B2);
            bool noFactor = state.B * state.B != state.B2;

            if (!noFactor)
            {
                state.Q = state.A + state.B;
                state.P = state.A - state.B;
                state.HasFactor = n == state.Q || n == state.P;
            }
            else
            {
                Resume(state);
            }
            return state;
        }

        public static bool Resume<T>(FactorizationState<T> state, int maxIterations = 1000)
        {
            if (state.HasFactor) { return state.HasFactor; }


            //ref int p = ref state.P;
            //ref int q = ref state.Q;
            //ref int a = ref state.A;

            //ref int b2 = ref state.B2;
            //ref int b = ref state.B;


            T p = state.P;
            T q = state.Q;
            T a = state.A;

            T b2 = state.B2;
            T b = state.B;

            T n = state.N;
            bool noFactor = true;
            int i;
            T one = Ops<T>.ConvertFromInt(1);
            for (i = 0; noFactor && i < maxIterations; i++)
            {
                /*           
                a++;
                b2 = a * a - n;
                b = (int)MathLib.Sqrt(b2);
                noFactor = b * b != b2
                ;
                */
                a = Ops<T>.AddT(a, one);
                b2 = Ops<T>.MultiplyT(a, a);
                b2 = Ops<T>.SubtractT(b2, n);
                var root = MathLib.Sqrt(Ops<T>.ToGmpInt(b2));
                b = Ops<T>.FromGmpInt(root);

                var bMul = Ops<T>.MultiplyT(b, b);
                var neq = Ops<T>.NotEqualT(bMul, b2);
                noFactor = neq;
            }
            state.Iterations += i;
            if (!noFactor)
            {
                q = Ops<T>.AddT(a, b);
                p = Ops<T>.SubtractT(a, b);
                state.HasFactor = Ops<T>.EqualT(n, p) || Ops<T>.EqualT(n, q);
            }

            state.P = p;
            state.Q = q;
            state.A = a;

            state.B2 = b2;
            state.B = b;


            return state.HasFactor;
        }

        public class FactorizationState<T>
        {
            public T N;
            public T Sqrt;
            public T A;
            public T B2;
            public T B;
            public T P;
            public T Q;
            public long Iterations;
            public bool HasFactor;

            public FactorizationState(T n)
            {
                N = n;

            }
        }
    }

    public partial class Factorization
    {
        public int num_factors => this.Factors.Count;

        public FactorArray factors => new FactorArray(this);

        // if a number is <= aprcl_prove_cutoff, we will prove it prime or composite
        public int aprcl_prove_cutoff = 500;
        // if a number is >= aprcl_display_cutoff, we will show the APRCL progress
        public int aprcl_display_cutoff = 200;
        public class FactorArray
        {
            private yfactor_list_t fact { get; }
            public FactorArray(Factorization fact)
            {
                this.fact = fact;
            }
            public Factor this[int i]
            {
                get => fact.Factors[i];
            }
        }
    }
    public partial class Factor
    {
        public GmpInt factor => this.P;
        public int count
        {
            get => this.Power;
            set => this.Power++;
        }
        public PrimalityType type;
    }

    // ported from bbuhrow's Yafu
    public class FermatZ
    {
        //#define setbit(a,b) (((a)[(b) >> 3]) |= (nmasks[(b) & 7])) 
        //#define getbit(a,b) (((a)[(b) >> 3]) & (nmasks[(b) & 7]))

        static byte[] nmasks = { };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void setbit(byte[] a, ulong b) => a[b >> 3] |= nmasks[b & 7];
        static int getbit(byte[] a, ulong b) => a[b >> 3] & nmasks[(b) & 7];

        public struct fact_obj_t
        {
            public div_obj div_obj;
            public yfactor_list_t factors;
            public int VFLAG = 0;
            public int NUM_WITNESSES = 20;
            public bool LOGFLAG;
            public string flogname;
            public fact_obj_t()
            {
                NUM_WITNESSES = 20;
                VFLAG = 0;
                factors = new();
                div_obj = new();
                LOGFLAG = false;
                flogname = typeof(fact_obj_t).Assembly.ManifestModule.Name;
            }
        }
        public struct div_obj
        {
            public mpz_t gmp_n;
            public div_obj()
            {
                gmp_n = new();
            }
        }

        int printf(string message, params object[] args)
            => printf(string.Format(message, args));
        int printf(string message)
        {
            Console.Write(message);
            return message.Length;
        }

        const PrimalityType UNKNOWN = PrimalityType.UNKNOWN;
        const PrimalityType PRIME = PrimalityType.Prime;
        const PrimalityType COMPOSITE = PrimalityType.Composite;
        const PrimalityType PRP = PrimalityType.ProbablePrime;
        const PrimalityType PRP_COMPOSITE = PrimalityType.Composite;




        int add_to_factor_list(ref yfactor_list_t flist, mpz_t n, int VFLAG, int NUM_WITNESSES)
        {
            // stick the number n into the provided factor list.
            // return the index into which the factor was added.
            int i;
            int fid;
            int found = 0, v = 0;

            // look to see if this factor is already in the list
            for (i = 0; i < flist.num_factors && found == 0; i++)
            {
                if (mpz_cmp(n, flist.factors[i].factor) == 0)
                {
                    found = 1;
                    flist.factors[i].count++;
                    return i;
                }
            }

            // otherwise, allocate another factor to the list and add it
            fid = flist.num_factors;
            //if (flist.num_factors >= flist.alloc_factors)
            //{
            //    flist.alloc_factors *= 2;
            //    flist.factors = (yfactor_t*)realloc(flist.factors,
            //        flist.alloc_factors * sizeof(yfactor_t));
            //}

            mpz_init(flist.factors[fid].factor);
            mpz_set(flist.factors[fid].factor, n);
            flist.factors[fid].count = 1;
            flist.factors[fid].type = UNKNOWN;

            if (gmp_base10(n) <= flist.aprcl_prove_cutoff) /* prove primality of numbers <= aprcl_prove_cutoff digits */
            {
                PrimalityType ret = 0;

                //if (VFLAG > 0)
                //    v = (gmp_base10(n) < flist.aprcl_display_cutoff) ? APRTCLE_VERBOSE0 : APRTCLE_VERBOSE1;
                //else
                //    v = APRTCLE_VERBOSE0;

                //if (v == APRTCLE_VERBOSE1)
                //    printf("\n");

                //ret = mpz_aprtcle(n, v);
                ret = GmpInt.is_mpz_prp(n, v);
                //printf("aprtcle returned %d\n", ret);

                //if (v == APRTCLE_VERBOSE1)
                //    printf("\n");

                //if (ret == APRTCLE_PRIME)
                if (ret == PRIME)
                    flist.factors[fid].type = PRIME;
                else
                {
                    if (GmpInt.mpz_bpsw_prp(n) != (int)PRP_COMPOSITE)
                    {
                        // if BPSW doesn't think this composite number is actually composite, then ask the user
                        // to report this fact to the YAFU sub-forum at mersenneforum.org
                        printf(" *** ATTENTION: BPSW issue found.  Please report the following number to:\n");
                        printf(" *** ATTENTION: http://www.mersenneforum.org/forumdisplay.php?f=96\n");
                        gmp_printf(" *** ATTENTION: n = %Zd\n", n);
                    }
                    flist.factors[fid].type = COMPOSITE;
                }
            }
            else if (is_mpz_prp(n, NUM_WITNESSES) > 0)
            {
                if (mpz_cmp_ui(n, 100000000) < 0)
                    flist.factors[fid].type = PRIME;
                else
                    flist.factors[fid].type = PRP;
            }
            else
                flist.factors[fid].type = COMPOSITE;

            //flist.num_factors++;
            return fid;
        }

        private int is_mpz_prp(mpz_t n, int num_witnesses)
            => (int)GmpInt.is_mpz_prp(n, num_witnesses);


        private int gmp_base10(mpz_t n)
            => GmpInt.gmp_base10(n);


        void zFermat(uint64_t limit, uint32_t mult, ref fact_obj_t fobj)
        {
            // Fermat's factorization method with a sieve-based improvement
            // provided by 'neonsignal'
            mpz_t a = new(), b2 = new(), tmp = new(), multN = new(), a2 = new();
            int i;
            int sqchecks = 0;
            int numChars;
            uint64_t reportIt, reportInc;
            uint64_t count;
            uint64_t i64;

            const uint32_t M = 2 * 2 * 2 * 2 * 3 * 3 * 5 * 5 * 7 * 7; //176400u
            const uint32_t M1 = 11 * 17 * 23 * 31; //133331u
            const uint32_t M2 = 13 * 19 * 29 * 37; //265031u
            uint8_t[] sqr, sqr1, sqr2, mod, mod1, mod2;
            uint16_t[] skip;
            uint32_t m, mmn, s, d;
            uint8_t[] masks = { 0xfe, 0xfd, 0xfb, 0xf7, 0xef, 0xdf, 0xbf, 0x7f };
            uint8_t[] nmasks = new uint8_t[8];
            uint32_t iM = 0, iM1 = 0, iM2 = 0;

            if (mpz_even_p(fobj.div_obj.gmp_n) > 0)
            {
                mpz_init(tmp);
                mpz_set_ui(tmp, 2);
                mpz_tdiv_q_2exp(fobj.div_obj.gmp_n, fobj.div_obj.gmp_n, 1);
                add_to_factor_list(ref fobj.factors, tmp, fobj.VFLAG, fobj.NUM_WITNESSES);
                mpz_clear(tmp);
                return;
            }

            StreamWriter? flog = null;
            if (mpz_perfect_square_p(fobj.div_obj.gmp_n) > 0)
            {
                //open the log file
                if (fobj.LOGFLAG)
                {
                    flog = new StreamWriter(fobj.flogname, true);
                }
                mpz_sqrt(fobj.div_obj.gmp_n, fobj.div_obj.gmp_n);

                string ns = ((GmpInt)fobj.div_obj.gmp_n).ToString();
                if (is_mpz_prp(fobj.div_obj.gmp_n, fobj.NUM_WITNESSES) > 0)
                {
                    logprint(flog, "Fermat method found perfect square factorization:\n");
                    logprint(flog, "prp%d = %s\n",
                        gmp_base10(fobj.div_obj.gmp_n), ns);
                    logprint(flog, "prp%d = %s\n",
                        gmp_base10(fobj.div_obj.gmp_n), ns);
                }
                else
                {
                    logprint(flog, "Fermat method found perfect square factorization:\n");
                    logprint(flog, "c%d = %s\n",
                        gmp_base10(fobj.div_obj.gmp_n), ns);
                    logprint(flog, "c%d = %s\n",
                        gmp_base10(fobj.div_obj.gmp_n), ns);
                }

                //free(s);

                add_to_factor_list(ref fobj.factors, fobj.div_obj.gmp_n, fobj.VFLAG, fobj.NUM_WITNESSES);
                add_to_factor_list(ref fobj.factors, fobj.div_obj.gmp_n, fobj.VFLAG, fobj.NUM_WITNESSES);
                mpz_set_ui(fobj.div_obj.gmp_n, 1);

                if (fobj.LOGFLAG)
                {
                    if (flog is not null) fclose(flog);
                }
                return;
            }

            mpz_init(a);
            mpz_init(b2);
            mpz_init(tmp);
            mpz_init(multN);
            mpz_init(a2);

            // apply the user supplied multiplier
            mpz_mul_ui(multN, fobj.div_obj.gmp_n, mult);

            // compute ceil(sqrt(multN))
            mpz_sqrt(a, multN);

            // form b^2
            mpz_mul(b2, a, a);
            mpz_sub(b2, b2, multN);

            // test successive 'a' values using a sieve-based approach.
            // the idea is that not all 'a' values allow a^2 or b^2 to be square.  
            // we pre-compute allowable 'a' values modulo various smooth numbers and 
            // build tables to allow us to quickly iterate over 'a' values that are 
            // more likely to produce squares.
            // init sieve structures
            //sqr = (uint8_t*)calloc((M / 8 + 1), sizeof(uint8_t));
            //sqr1 = (uint8_t*)calloc((M1 / 8 + 1), sizeof(uint8_t));
            //sqr2 = (uint8_t*)calloc((M2 / 8 + 1), sizeof(uint8_t));
            //mod = (uint8_t*)calloc((M / 8 + 1), sizeof(uint8_t));
            //mod1 = (uint8_t*)calloc((M1 / 8 + 1), sizeof(uint8_t));
            //mod2 = (uint8_t*)calloc((M2 / 8 + 1), sizeof(uint8_t));
            //skip = (uint16_t*)malloc(M * sizeof(uint16_t));

            sqr = new uint8_t[M / 8 + 1]; // (uint8_t*)calloc((M / 8 + 1), sizeof(uint8_t));
            sqr1 = new uint8_t[M1 / 8 + 1]; // (uint8_t*)calloc((M1 / 8 + 1), sizeof(uint8_t));
            sqr2 = new uint8_t[M2 / 8 + 1]; // (uint8_t*)calloc((M2 / 8 + 1), sizeof(uint8_t));
            mod = new uint8_t[M / 8 + 1]; // (uint8_t*)calloc((M / 8 + 1), sizeof(uint8_t));
            mod1 = new uint8_t[M1 / 8 + 1]; //  (uint8_t*)calloc((M1 / 8 + 1), sizeof(uint8_t));
            mod2 = new uint8_t[M2 / 8 + 1]; // (uint8_t*)calloc((M2 / 8 + 1), sizeof(uint8_t));
            skip = new uint16_t[M + 1]; // (uint16_t*)malloc(M * sizeof(uint16_t));

            for (i = 0; i < 8; i++)
                nmasks[i] = (uint8_t)~(masks[i]);

            // marks locations where squares can occur mod M, M1, M2
            for (i64 = 0; i64 < M; ++i64)
                setbit(sqr, (i64 * i64) % M);

            for (i64 = 0; i64 < M1; ++i64)
                setbit(sqr1, (i64 * i64) % M1);

            for (i64 = 0; i64 < M2; ++i64)
                setbit(sqr2, (i64 * i64) % M2);

            // test it.  This will be good enough if |u*p-v*q| < 2 * N^(1/4), where
            // mult = u*v
            count = 0;
            if (mpz_perfect_square_p(b2) > 0)
                goto found;

            for (i = 0; i < 8; i++)
                nmasks[i] = (byte)~masks[i];

            // marks locations where squares can occur mod M, M1, M2
            for (i64 = 0; i64 < M; ++i64)
                setbit(sqr, (i64 * i64) % M);

            for (i64 = 0; i64 < M1; ++i64)
                setbit(sqr1, (i64 * i64) % M1);

            for (i64 = 0; i64 < M2; ++i64)
                setbit(sqr2, (i64 * i64) % M2);

            // for the modular sequence of b*b = a*a - n values 
            // (where b2_2 = b2_1 * 2a + 1), mark locations where
            // b^2 can be a square
            m = mpz_mod_ui(tmp, a, M);
            mmn = mpz_mod_ui(tmp, b2, M);
            for (i = 0; i < M; ++i)
            {
                if (getbit(sqr, mmn) > 0) setbit(mod, (ulong)i);
                mmn = (mmn + m + m + 1) % M;
                m = (m + 1) % M;
            }

            // we only consider locations where the modular sequence mod M can
            // be square, so compute the distance to the next square location
            // at each possible value of i mod M.
            s = 0;
            d = 0;
            for (i = 0; getbit(mod, (ulong)i) == 0; ++i)
                ++s;
            for (i = (int)M; i > 0;)
            {
                --i;
                ++s;
                skip[i] = (ushort)s;
                if (s > d) d = s;
                if (getbit(mod, (ulong)i) > 0) s = 0;
            }
            //printf("maxSkip = %u\n", d);

            // for the modular sequence of b*b = a*a - n values 
            // (where b2_2 = b2_1 * 2a + 1), mark locations where the
            // modular sequence can be a square mod M1.  These will
            // generally differ from the sequence mod M.
            m = mpz_mod_ui(tmp, a, M1);
            mmn = mpz_mod_ui(tmp, b2, M1);
            for (i = 0; i < M1; ++i)
            {
                if (getbit(sqr1, mmn) > 0) setbit(mod1, (ulong)i);
                mmn = (mmn + m + m + 1) % M1;
                m = (m + 1) % M1;
            }

            // for the modular sequence of b*b = a*a - n values 
            // (where b2_2 = b2_1 * 2a + 1), mark locations where the
            // modular sequence can be a square mod M2.  These will
            // generally differ from the sequence mod M or M1.
            m = mpz_mod_ui(tmp, a, M2);
            mmn = mpz_mod_ui(tmp, b2, M2);
            for (i = 0; i < M2; ++i)
            {
                if (getbit(sqr2, mmn) > 0) setbit(mod2, (ulong)i);
                mmn = (mmn + m + m + 1) % M2;
                m = (m + 1) % M2;
            }

            // loop, checking for perfect squares
            mpz_mul_2exp(a2, a, 1);
            count = 0;
            numChars = 0;
            reportIt = limit / 100;
            reportInc = reportIt;

            do
            {
                d = 0;
                i64 = 0;
                do
                {
                    // skip to the next possible square  of b*b mod M
                    s = skip[iM];

                    // remember how far we skipped
                    d += s;

                    // update the other  indices
                    if ((iM1 += s) >= M1) iM1 -= M1;
                    if ((iM2 += s) >= M2) iM2 -= M2;
                    if ((iM += s) >= M) iM -= M;

                    // some multpliers can lead to infinite loops.  bail out if so.
                    if (++i64 > M) goto done;

                    // continue if either of the other s indicates non-square.
                } while (getbit(mod1, iM1) == 0 || getbit(mod2, iM2) == 0);

                // form b^2 by incrementing by many factors of 2*a+1
                mpz_add_ui(tmp, a2, d);
                mpz_mul_ui(tmp, tmp, d);
                mpz_add(b2, b2, tmp);

                // accumulate so that we can reset d 
                // (and thus keep it single precision)
                mpz_add_ui(a2, a2, d * 2);

                count += d;
                if (count > limit)
                    break;

                //progress report
                if ((count > reportIt) && (fobj.VFLAG > 1))
                {
                    for (i = 0; i < numChars; i++)
                        printf("\b");
                    numChars = printf("{0}", (uint64_t)((double)count / (double)limit * 100));
                    //fflush(stdout);
                    reportIt += reportInc;
                }
                sqchecks++;

            } while (mpz_perfect_square_p(b2) == 0);

            if (fobj.VFLAG > 1)
                printf("fmt: performed %d perfect square checks\n", sqchecks);

            found:

            // 'count' is how far we had to scan 'a' to find a square b
            mpz_add_ui(a, a, (uint)count);

            if ((mpz_size(b2) > 0) && mpz_perfect_square_p(b2) > 0)
            {
                mpz_sqrt(tmp, b2);
                mpz_add(tmp, a, tmp);
                mpz_gcd(tmp, fobj.div_obj.gmp_n, tmp);

                if (fobj.LOGFLAG)
                {
                    flog = new StreamWriter(fobj.flogname, true);
                    logprint(flog, "Fermat method found factors:\n");
                }

                add_to_factor_list(ref fobj.factors, tmp, fobj.VFLAG, fobj.NUM_WITNESSES);
                string ns = ((GmpInt)tmp).ToString();
                if (is_mpz_prp(tmp, fobj.NUM_WITNESSES) > 0)
                {
                    logprint(flog, "prp%d = %s\n",
                        gmp_base10(tmp), ns);
                }
                else
                {
                    logprint(flog, "c%d = %s\n",
                        gmp_base10(tmp), ns);
                }
                //free(s);

                mpz_tdiv_q(fobj.div_obj.gmp_n, fobj.div_obj.gmp_n, tmp);
                mpz_sqrt(tmp, b2);
                mpz_sub(tmp, a, tmp);
                mpz_gcd(tmp, fobj.div_obj.gmp_n, tmp);

                add_to_factor_list(ref fobj.factors, tmp, fobj.VFLAG, fobj.NUM_WITNESSES);
                ns = ns = ((GmpInt)tmp).ToString();

                if (is_mpz_prp(tmp, fobj.NUM_WITNESSES)>0)
                {
                    logprint(flog, "prp%d = %s\n",
                        gmp_base10(tmp), ns);
                }
                else
                {
                    logprint(flog, "c%d = %s\n",
                        gmp_base10(tmp), ns);
                }
                //free(s);

                mpz_tdiv_q(fobj.div_obj.gmp_n, fobj.div_obj.gmp_n, tmp);
            }

        done:
            mpz_clear(tmp);
            mpz_clear(a);
            mpz_clear(b2);
            mpz_clear(multN);
            mpz_clear(a2);
            free(ref sqr);
            free(ref sqr1);
            free(ref sqr2);
            free(ref mod);
            free(ref mod1);
            free(ref mod2);
            free(ref skip);

            if (fobj.LOGFLAG && (flog != null))
            {
                fclose(flog);
            }
            return;

        }

        private void free(ref ushort[] value)
        {

        }

        private void free(ref byte[] value)
        {
          
        }

        private void fclose(StreamWriter flog)
        {
            flog?.Close();
        }

        static void logprint(StreamWriter? flog, string message)
        {
            flog?.Write(message);
        }
        private void logprint(StreamWriter? flog, string message, params object[] args)
            => logprint(flog, string.Format(message, args));

    }
}