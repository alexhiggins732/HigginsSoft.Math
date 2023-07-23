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
using mpir = MathGmp.Native.gmp_lib;
using static MathGmp.Native.gmp_lib;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using static HigginsSoft.Math.Lib.MathLib;

namespace HigginsSoft.Math.Lib
{



    public partial class GmpInt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static mpz_t newz()
        {
            var z = new mpz_t();
            mpz_init(z);
            return z;
        }

        public static mpz_t NewMpz() => newz();

        #region Basic Arithmetic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static mpz_t run(GmpInt a, Action<mpz_t, mpz_t> fn)
        {
            var z = newz();
            fn(z, a);
            return z;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static mpz_t run(GmpInt a, GmpInt b, Action<mpz_t, mpz_t, mpz_t> fn)
        //{
        //    var z = newz();
        //    fn(z, a, b);
        //    return z;
        //}

        /// Returns a new mpz_t which is the absolute value of this value.
        public mpz_t Abs()
        {
            /* var z = newz();
            mpz_abs(z, this);
            return z;
            */
            return run(this, mpz_abs);

        }



        public mpz_t Negate()
        {
            return -this;
        }

        public static mpz_t Negate(mpz_t value)
        {
            //var z = newz();
            //mpz_neg(z, value);
            //return z;
            return run(value, mpz_neg);
        }

        public mpz_t Complement()
        {
            return (~this);
        }

        public mpz_t Add(mpz_t x)
        {
            return this + x;
        }

        public mpz_t Add(int x)
        {
            return this + x;
        }

        public mpz_t Add(uint x)
        {
            return this + x;
        }

        public mpz_t Subtract(mpz_t x)
        {
            return this - x;
        }

        public mpz_t Subtract(int x)
        {
            return this - x;
        }

        public mpz_t Subtract(uint x)
        {
            return this - x;
        }

        public mpz_t Multiply(mpz_t x)
        {
            return this * x;
        }

        public mpz_t Multiply(int x)
        {
            return this * x;
        }

        public mpz_t Multiply(uint x)
        {
            return this * x;
        }

        public mpz_t Square()
        {
            return this * this;
        }

        public mpz_t Divide(mpz_t divisor)
        {
            return this / divisor;
        }

        public mpz_t Divide(int divisor)
        {
            return this / divisor;
        }

        public mpz_t Divide(uint divisor)
        {
            return this / divisor;
        }

        public mpz_t Divide(mpz_t divisor, out mpz_t remainder)
        {
            var quotient = newz();
            remainder = newz();

            mpz_tdiv_qr(quotient, remainder, this, divisor);
            return quotient;
        }

        public mpz_t Divide(int divisor, out mpz_t remainder)
        {
            var quotient = newz();
            remainder = newz();

            if (divisor >= 0)
            {
                mpz_tdiv_qr_ui(quotient, remainder, this, (uint)divisor);
                return quotient;
            }
            else
            {
                mpz_tdiv_qr_ui(quotient, remainder, this, (uint)(-divisor));
                mpz_t res = Negate(quotient);
                quotient.Dispose();
                return res;
            }
        }

        public mpz_t Divide(int divisor, out int remainder)
        {
            var quotient = newz();

            if (divisor >= 0)
            {
                remainder = (int)mpz_tdiv_q_ui(quotient, this, (uint)divisor);
                return quotient;
            }
            else
            {
                remainder = -(int)mpz_tdiv_q_ui(quotient, this, (uint)(-divisor));
                mpz_t res = Negate(quotient);
                quotient.Dispose();
                return res;
            }
        }

        public mpz_t Divide(uint divisor, out mpz_t remainder)
        {
            var quotient = newz();
            remainder = newz();

            mpz_tdiv_qr_ui(quotient, remainder, this, divisor);
            return quotient;
        }

        public mpz_t Divide(uint divisor, out ulong remainder)
        {
            var quotient = newz();
            remainder = mpz_tdiv_q_ui(quotient, this, divisor);
            return quotient;
        }

        public mpz_t Divide(uint divisor, out int remainder)
        {
            var quotient = newz();

            ulong uintRemainder = mpz_tdiv_q_ui(quotient, this, divisor);
            if (uintRemainder > (uint)int.MaxValue)
                throw new OverflowException();

            if (this >= 0)
                remainder = (int)uintRemainder;
            else
                remainder = -(int)uintRemainder;

            return quotient;
        }

        public mpz_t Remainder(mpz_t divisor)
        {
            var z = newz();
            mpz_tdiv_r(z, this, divisor);
            return z;
        }

        public bool IsDivisibleBy(mpz_t divisor)
        {
            return mpz_divisible_p(this, divisor) != 0;
        }

        public bool IsDivisibleBy(int divisor)
        {
            if (divisor >= 0)
                return mpz_divisible_ui_p(this, (uint)divisor) != 0;
            else
                return mpz_divisible_ui_p(this, (uint)(-divisor)) != 0;
        }

        public bool IsDivisibleBy(uint divisor)
        {
            return mpz_divisible_ui_p(this, divisor) != 0;
        }

        /// <summary>
        /// Divides exactly. Only works when the division is gauranteed to be exact (there is no remainder).
        /// </summary>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public mpz_t DivideExactly(mpz_t divisor)
        {
            var z = newz();
            mpz_divexact(z, this, divisor);
            return z;
        }

        public mpz_t DivideExactly(int divisor)
        {
            var z = newz();
            if (divisor < 0)
            {
                mpz_divexact_ui(z, this, (uint)-divisor);
                z = Negate(z);
            }
            else
            {
                mpz_divexact_ui(z, this, (uint)divisor);

            }
            return z;
        }

        public mpz_t DivideExactly(uint divisor)
        {
            var z = newz();
            mpz_divexact_ui(z, this, divisor);
            return z;
        }

        public mpz_t DivideMod(mpz_t divisor, mpz_t mod)
        {
            return (this * InvertMod(divisor, mod) % mod);
        }

        public mpz_t And(mpz_t x)
        {
            return (this & x);
        }

        public mpz_t Or(mpz_t x)
        {
            return (this | x);
        }

        public mpz_t Xor(mpz_t x)
        {
            return (this ^ x);
        }

        public mpz_t Mod(mpz_t mod)
        {
            return (this % mod);
        }

        public mpz_t Mod(int mod)
        {
            return (this % mod);
        }

        public mpz_t Mod(uint mod)
        {
            return (this % mod);
        }

        public int ModAsInt32(int mod)
        {
            if (mod < 0)
                throw new ArgumentOutOfRangeException();

            return (int)mpz_fdiv_ui(this, (uint)mod);
        }

        public uint ModAsUInt32(uint mod)
        {
            return (uint)mpz_fdiv_ui(this, mod);
        }

        public mpz_t ShiftLeft(int shiftAmount)
        {
            return (this << shiftAmount);
        }

        public mpz_t ShiftRight(int shiftAmount)
        {
            return (this >> shiftAmount);
        }

        public mpz_t PowerMod(mpz_t exponent, mpz_t mod)
        {
            var z = newz();
            mpz_powm(z, this, exponent, mod);
            return z;
        }

        public mpz_t PowerMod(int exponent, mpz_t mod)
        {
            var z = newz();
            if (exponent >= 0)
            {
                mpz_powm_ui(z, this, (uint)exponent, mod);
            }
            else
            {
                mpz_t bigExponent = (GmpInt)exponent;
                mpz_t inverse = InvertMod(bigExponent, mod);
                mpz_powm_ui(z, inverse, (uint)exponent, mod);
            }

            return z;
        }

        public mpz_t PowerMod(uint exponent, mpz_t mod)
        {
            var z = newz();
            mpz_powm_ui(z, this, (uint)exponent, mod);
            return z;


        }

        public mpz_t Power(int exponent)
        {
            if (exponent < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_pow_ui(z, this, (uint)exponent);
            return z;
        }

        public mpz_t Power(uint exponent)
        {
            var z = newz();
            mpz_pow_ui(z, this, exponent);
            return z;
        }

        public static mpz_t Power(uint x, int exponent)
        {
            if (exponent < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_ui_pow_ui(z, (uint)x, (uint)exponent);
            return z;
        }

        public static mpz_t Power(uint x, uint exponent)
        {
            var z = newz();
            mpz_ui_pow_ui(z, x, exponent);
            return z;
        }
        public static mpz_t InvertMod(mpz_t x, mpz_t mod)
        {
            var z = newz();
            int status = mpz_invert(z, x, mod);
            if (status == 0)
                throw new ArithmeticException("This modular inverse does not exists.");
            return z;
        }
        public mpz_t InvertMod(mpz_t mod)
        {
            var z = newz();
            int status = mpz_invert(z, this, mod);
            if (status == 0)
                throw new ArithmeticException("This modular inverse does not exists.");
            return z;
        }

        public bool TryInvertMod(mpz_t mod, out mpz_t result)
        {
            var z = newz();
            int status = mpz_invert(z, this, mod);

            if (status == 0)
            {
                result = GmpInt.Zero.Data;
                return false;
            }
            else
            {
                result = z;
                return true;
            }
        }

        public bool InverseModExists(mpz_t mod)
        {
            mpz_t result;
            bool exists = TryInvertMod(mod, out result);
            return exists;
        }

        public int BitLength
        {
            get
            {
                return (int)mpz_sizeinbase(this, 2);
            }
        }

        #endregion

        #region Roots

        public static mpz_t Sqrt(GmpInt value)
        {
            var z = newz();
            mpz_sqrt(z, value);
            return z;
        }

        public static mpz_t Sqrt(GmpInt value, out mpz_t remainder)
        {
            var z = newz();
            remainder = newz();
            mpz_sqrtrem(z, remainder, value);
            return z;
        }

        public static mpz_t Sqrt(GmpInt value, out bool isExact)
        {
            var z = newz();
            int result = mpz_root(z, value, 2);
            isExact = result != 0;
            return z;
        }


        public mpz_t Sqrt()
        {
            var z = newz();
            mpz_sqrt(z, this);
            return z;
        }

        public mpz_t Sqrt(out mpz_t remainder)
        {
            var z = newz();
            remainder = newz();
            mpz_sqrtrem(z, remainder, this);
            return z;
        }

        public mpz_t Sqrt(out bool isExact)
        {
            var z = newz();
            int result = mpz_root(z, this, 2);
            isExact = result != 0;
            return z;
        }

        public mpz_t Root(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_root(z, this, (uint)n);
            return z;
        }

        public mpz_t Root(uint n)
        {
            var z = newz();
            mpz_root(z, this, n);
            return z;
        }

        public mpz_t Root(int n, out bool isExact)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            int result = mpz_root(z, this, (uint)n);
            isExact = result != 0;
            return z;
        }

        public mpz_t Root(uint n, out bool isExact)
        {
            var z = newz();
            int result = mpz_root(z, this, n);
            isExact = result != 0;
            return z;
        }

        public mpz_t Root(int n, out mpz_t remainder)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            remainder = newz();
            mpz_rootrem(z, remainder, this, (uint)n);
            return z;
        }

        public mpz_t Root(uint n, out mpz_t remainder)
        {
            var z = newz();
            remainder = newz();
            mpz_rootrem(z, remainder, this, n);
            return z;
        }

        public bool IsPerfectSquare()
        {
            return mpz_perfect_square_p(this) != 0;
        }

        public bool IsPerfectSquare(out mpz_t root)
            => IsPerfectSquare(this, out root);

        public bool IsPerfectSquare(out GmpInt root)
        {
            var result = IsPerfectSquare(this, out mpz_t sqrt);
            root = sqrt;
            return result;
        }

        public static bool IsPerfectSquare(mpz_t value, out mpz_t root)
        {
            var z = newz();
            var sign = mpz_sgn(value);
            if (sign < 1)
            {
                root = z;
                return false;
            }

            var remainder = newz();
            mpz_rootrem(z, remainder, value, 2);
            sign = mpz_sgn(remainder);
            mpz_clear(remainder);
            root = z;
            return sign == 0;
        }

        public bool IsPerfectPower()
        {
            // There is a known issue with this function for negative inputs in GMP 4.2.4.
            // Haven't heard of any issues in MPIR 5.x though.
            return mpz_perfect_power_p(this) != 0;
        }

        #endregion

        #region Number Theoretic Functions

        public bool IsProbablyPrimeRabinMiller(int num_witnesses = 20)
        {
            int result = mpz_probab_prime_p(this, num_witnesses);

            return result != 0;
        }

        public bool IsProbablePrime(int num_witnesses = 20)
            => IsProbablePrime(this.Data, num_witnesses);
        //from yafu.
        public static bool IsProbablePrime(mpz_t n, int num_witnesses = 20)
        {
            int i = mpz_probab_prime_p(n, num_witnesses);
            return ((i == 1) || (i == 2)) && mpz_strongbpsw_prp(n) == 1;
        }

        public static PrimalityType is_mpz_prp(mpz_t n, int num_witnesses)
        {
            int i = mpz_probab_prime_p(n, num_witnesses);
            if (i == 1) return PrimalityType.Prime;
            else if (i == 2 && mpz_strongbpsw_prp(n) == 1)
                return PrimalityType.ProbablePrime;
            return PrimalityType.Composite;
        }

        /* ****************************************************************************************
         * mpz_strongbpsw_prp:
         * A "strong Baillie-Pomerance-Selfridge-Wagstaff pseudoprime" is a composite n such that
         * n is a strong pseudoprime to the base 2 and
         * n is a strong Lucas pseudoprime using the Selfridge parameters.
         * ****************************************************************************************/
        public static int mpz_strongbpsw_prp(mpz_t n)
        {
            int ret = 0;
            mpz_t two = new();

            mpz_init_set_ui(two, 2);

            ret = mpz_sprp(n, two);
            mpz_clear(two);

            /* with a base of 2,  mpz_sprp won't return PRP_ERROR */
            /* so, only check for PRP_COMPOSITE or PRP_PRIME here */
            if ((ret == PRP_COMPOSITE) || (ret == PRP_PRIME))
                return ret;

            return mpz_strongselfridge_prp(n);

        }/* method mpz_strongbpsw_prp */


        /* *********************************************************************************************************
         * mpz_strongselfridge_prp:
         * A "strong Lucas-Selfridge pseudoprime" n is a "strong Lucas pseudoprime" using Selfridge parameters of:
         * Find the first element D in the sequence {5, -7, 9, -11, 13, ...} such that Jacobi(D,n) = -1
         * Then use P=1 and Q=(1-D)/4 in the strong Lucase pseudoprime test.
         * Make sure n is not a perfect square, otherwise the search for D will only stop when D=n.
         * **********************************************************************************************************/
        public static int mpz_strongselfridge_prp(mpz_t n)
        {
            long d = 5, p = 1, q = 0;
            int max_d = 1000000;
            int jacobi = 0;
            mpz_t zD = new();

            if (mpz_cmp_ui(n, 2) < 0)
                return PRP_COMPOSITE;

            if (mpz_divisible_ui_p(n, 2) > 0)
            {
                if (mpz_cmp_ui(n, 2) == 0)
                    return PRP_PRIME;
                else
                    return PRP_COMPOSITE;
            }

            mpz_init_set_ui(zD, (uint)d);

            while (true)
            {
                jacobi = mpz_jacobi(zD, n);

                /* if jacobi == 0, d is a factor of n, therefore n is composite... */
                /* if d == n, then either n is either prime or 9... */
                if (jacobi == 0)
                {
                    if ((mpz_cmpabs(zD, n) == 0) && (mpz_cmp_ui(zD, 9) != 0))
                    {
                        mpz_clear(zD);
                        return PRP_PRIME;
                    }
                    else
                    {
                        mpz_clear(zD);
                        return PRP_COMPOSITE;
                    }
                }
                if (jacobi == -1)
                    break;

                /* if we get to the 5th d, make sure we aren't dealing with a square... */
                if (d == 13)
                {
                    if (mpz_perfect_square_p(n) > 0)
                    {
                        mpz_clear(zD);
                        return PRP_COMPOSITE;
                    }
                }

                if (d < 0)
                {
                    d *= -1;
                    d += 2;
                }
                else
                {
                    d += 2;
                    d *= -1;
                }

                /* make sure we don't search forever */
                if (d >= max_d)
                {
                    mpz_clear(zD);
                    return PRP_ERROR;
                }

                mpz_set_si(zD, (int)d);
            }
            mpz_clear(zD);

            q = (1 - d) / 4;

            return mpz_stronglucas_prp(n, p, q);

        }/* method mpz_strongselfridge_prp */

        public static int mpz_stronglucas_prp(mpz_t n, long p, long q)
        {
            mpz_t zD = new();
            mpz_t s = new();
            mpz_t nmj = new(); /* n minus jacobi(D/n) */
            mpz_t res = new();
            /* these are needed for the LucasU and LucasV part of this function */

            mpz_t uh = new(), vl = new(), vh = new(), ql = new(), qh = new(), tmp = new();
            long d = p * p - 4 * q;
            ulong r = 0;
            int ret = 0;
            ulong j = 0;

            if (d == 0) /* Does not produce a proper Lucas sequence */
                return PRP_ERROR;

            if (mpz_cmp_ui(n, 2) < 0)
                return PRP_COMPOSITE;

            if (mpz_divisible_ui_p(n, 2) > 0)
            {
                if (mpz_cmp_ui(n, 2) == 0)
                    return PRP_PRIME;
                else
                    return PRP_COMPOSITE;
            }

            mpz_init_set_si(zD, (int)d);
            mpz_init(res);

            mpz_mul_si(res, zD, (int)q);
            mpz_mul_ui(res, res, 2);
            mpz_gcd(res, res, n);
            if ((mpz_cmp(res, n) != 0) && (mpz_cmp_ui(res, 1) > 0))
            {
                mpz_clear(zD);
                mpz_clear(res);
                return PRP_COMPOSITE;
            }

            mpz_init(s);
            mpz_init(nmj);

            /* nmj = n - (D/n), where (D/n) is the Jacobi symbol */
            mpz_set(nmj, n);
            ret = mpz_jacobi(zD, n);
            if (ret == -1)
                mpz_add_ui(nmj, nmj, 1);
            else if (ret == 1)
                mpz_sub_ui(nmj, nmj, 1);

            r = mpz_scan1(nmj, 0);
            mpz_fdiv_q_2exp(s, nmj, (mp_bitcnt_t)r);

            /* make sure U_s == 0 mod n or V_((2^t)*s) == 0 mod n, for some t, 0 <= t < r */
            mpz_init_set_si(uh, 1);
            mpz_init_set_si(vl, 2);
            mpz_init_set_si(vh, (int)p);
            mpz_init_set_si(ql, 1);
            mpz_init_set_si(qh, 1);
            mpz_init_set_si(tmp, 0);

            for (j = mpz_sizeinbase(s, 2) - 1; j >= 1; j--)
            {
                /* ql = ql*qh (mod n) */
                mpz_mul(ql, ql, qh);
                mpz_mod(ql, ql, n);
                if (mpz_tstbit(s, (mp_bitcnt_t)j) == 1)
                {
                    /* qh = ql*q */
                    mpz_mul_si(qh, ql, (int)q);

                    /* uh = uh*vh (mod n) */
                    mpz_mul(uh, uh, vh);
                    mpz_mod(uh, uh, n);

                    /* vl = vh*vl - p*ql (mod n) */
                    mpz_mul(vl, vh, vl);
                    mpz_mul_si(tmp, ql, (int)p);
                    mpz_sub(vl, vl, tmp);
                    mpz_mod(vl, vl, n);

                    /* vh = vh*vh - 2*qh (mod n) */
                    mpz_mul(vh, vh, vh);
                    mpz_mul_si(tmp, qh, 2);
                    mpz_sub(vh, vh, tmp);
                    mpz_mod(vh, vh, n);
                }
                else
                {
                    /* qh = ql */
                    mpz_set(qh, ql);

                    /* uh = uh*vl - ql (mod n) */
                    mpz_mul(uh, uh, vl);
                    mpz_sub(uh, uh, ql);
                    mpz_mod(uh, uh, n);

                    /* vh = vh*vl - p*ql (mod n) */
                    mpz_mul(vh, vh, vl);
                    mpz_mul_si(tmp, ql, (int)p);
                    mpz_sub(vh, vh, tmp);
                    mpz_mod(vh, vh, n);

                    /* vl = vl*vl - 2*ql (mod n) */
                    mpz_mul(vl, vl, vl);
                    mpz_mul_si(tmp, ql, 2);
                    mpz_sub(vl, vl, tmp);
                    mpz_mod(vl, vl, n);
                }
            }
            /* ql = ql*qh */
            mpz_mul(ql, ql, qh);

            /* qh = ql*q */
            mpz_mul_si(qh, ql, (int)q);

            /* uh = uh*vl - ql */
            mpz_mul(uh, uh, vl);
            mpz_sub(uh, uh, ql);

            /* vl = vh*vl - p*ql */
            mpz_mul(vl, vh, vl);
            mpz_mul_si(tmp, ql, (int)p);
            mpz_sub(vl, vl, tmp);

            /* ql = ql*qh */
            mpz_mul(ql, ql, qh);

            mpz_mod(uh, uh, n);
            mpz_mod(vl, vl, n);

            /* uh contains LucasU_s and vl contains LucasV_s */
            if ((mpz_cmp_ui(uh, 0) == 0) || (mpz_cmp_ui(vl, 0) == 0))
            {
                mpz_clear(zD);
                mpz_clear(s);
                mpz_clear(nmj);
                mpz_clear(res);
                mpz_clear(uh);
                mpz_clear(vl);
                mpz_clear(vh);
                mpz_clear(ql);
                mpz_clear(qh);
                mpz_clear(tmp);
                return PRP_PRP;
            }

            for (j = 1; j < r; j++)
            {
                /* vl = vl*vl - 2*ql (mod n) */
                mpz_mul(vl, vl, vl);
                mpz_mul_si(tmp, ql, 2);
                mpz_sub(vl, vl, tmp);
                mpz_mod(vl, vl, n);

                /* ql = ql*ql (mod n) */
                mpz_mul(ql, ql, ql);
                mpz_mod(ql, ql, n);

                if (mpz_cmp_ui(vl, 0) == 0)
                {
                    mpz_clear(zD);
                    mpz_clear(s);
                    mpz_clear(nmj);
                    mpz_clear(res);
                    mpz_clear(uh);
                    mpz_clear(vl);
                    mpz_clear(vh);
                    mpz_clear(ql);
                    mpz_clear(qh);
                    mpz_clear(tmp);
                    return PRP_PRP;
                }
            }

            mpz_clear(zD);
            mpz_clear(s);
            mpz_clear(nmj);
            mpz_clear(res);
            mpz_clear(uh);
            mpz_clear(vl);
            mpz_clear(vh);
            mpz_clear(ql);
            mpz_clear(qh);
            mpz_clear(tmp);
            return PRP_COMPOSITE;

        }/* method mpz_stronglucas_prp */




        /* *********************************************************************************************
         * mpz_sprp: (also called a Miller-Rabin pseudoprime)
         * A "strong pseudoprime" to the base a is an odd composite n = (2^r)*s+1 with s odd such that
         * either a^s == 1 mod n, or a^((2^t)*s) == -1 mod n, for some integer t, with 0 <= t < r.
         * *********************************************************************************************/
        public static int mpz_sprp(mpz_t n, mpz_t a)
        {
            mpz_t s = new();
            mpz_t nm1 = new();
            mpz_t mpz_test = new();
            ulong r = 0;

            if (mpz_cmp_ui(a, 2) < 0)
                return PRP_ERROR;

            if (mpz_cmp_ui(n, 2) < 0)
                return PRP_COMPOSITE;

            if (mpz_divisible_ui_p(n, 2) == 1)
            {
                if (mpz_cmp_ui(n, 2) == 0)
                    return PRP_PRIME;
                else
                    return PRP_COMPOSITE;
            }

            mpz_init_set_ui(mpz_test, 0);
            mpz_init_set_ui(s, 0);
            mpz_init_set(nm1, n);
            mpz_sub_ui(nm1, nm1, 1);

            /***********************************************/
            /* Find s and r satisfying: n-1=(2^r)*s, s odd */
            r = mpz_scan1(nm1, 0);
            mpz_fdiv_q_2exp(s, nm1, (mp_bitcnt_t)r);


            /******************************************/
            /* Check a^((2^t)*s) mod n for 0 <= t < r */
            mpz_powm(mpz_test, a, s, n);
            if ((mpz_cmp_ui(mpz_test, 1) == 0) || (mpz_cmp(mpz_test, nm1) == 0))
            {
                mpz_clear(s);
                mpz_clear(nm1);
                mpz_clear(mpz_test);
                return PRP_PRP;
            }

            while (--r > 0)
            {
                /* mpz_test = mpz_test^2%n */
                mpz_mul(mpz_test, mpz_test, mpz_test);
                mpz_mod(mpz_test, mpz_test, n);

                if (mpz_cmp(mpz_test, nm1) == 0)
                {
                    mpz_clear(s);
                    mpz_clear(nm1);
                    mpz_clear(mpz_test);
                    return PRP_PRP;
                }
            }

            mpz_clear(s);
            mpz_clear(nm1);
            mpz_clear(mpz_test);
            return PRP_COMPOSITE;

        }/* method mpz_sprp */


        /* **********************************************************************************
         * mpz_bpsw_prp:
         * A "Baillie-Pomerance-Selfridge-Wagstaff pseudoprime" is a composite n such that
         * n is a strong pseudoprime to the base 2 and
         * n is a Lucas pseudoprime using the Selfridge parameters.
         * **********************************************************************************/
        public static int mpz_bpsw_prp(mpz_t n)
        {
            int ret = 0;
            mpz_t two = new();

            mpz_init_set_ui(two, 2);

            ret = mpz_sprp(n, two);
            mpz_clear(two);

            /* with a base of 2, mpz_sprp, won't return PRP_ERROR */
            /* so, only check for PRP_COMPOSITE or PRP_PRIME here */
            if ((ret == PRP_COMPOSITE) || (ret == PRP_PRIME))
                return ret;

            return mpz_selfridge_prp(n);

        }/* method mpz_bpsw_prp */

        /* ***********************************************************************************************
         * mpz_selfridge_prp:
         * A "Lucas-Selfridge pseudoprime" n is a "Lucas pseudoprime" using Selfridge parameters of:
         * Find the first element D in the sequence {5, -7, 9, -11, 13, ...} such that Jacobi(D,n) = -1
         * Then use P=1 and Q=(1-D)/4 in the Lucas pseudoprime test.
         * Make sure n is not a perfect square, otherwise the search for D will only stop when D=n.
         * ***********************************************************************************************/
        public static int mpz_selfridge_prp(mpz_t n)
        {
            long d = 5, p = 1, q = 0;
            int max_d = 1000000;
            int jacobi = 0;
            mpz_t zD = new();

            if (mpz_cmp_ui(n, 2) < 0)
                return PRP_COMPOSITE;

            if (mpz_divisible_ui_p(n, 2) > 0)
            {
                if (mpz_cmp_ui(n, 2) == 0)
                    return PRP_PRIME;
                else
                    return PRP_COMPOSITE;
            }

            mpz_init_set_ui(zD, (uint)d);

            while (true)
            {
                jacobi = mpz_jacobi(zD, n);

                /* if jacobi == 0, d is a factor of n, therefore n is composite... */
                /* if d == n, then either n is either prime or 9... */
                if (jacobi == 0)
                {
                    if ((mpz_cmpabs(zD, n) == 0) && (mpz_cmp_ui(zD, 9) != 0))
                    {
                        mpz_clear(zD);
                        return PRP_PRIME;
                    }
                    else
                    {
                        mpz_clear(zD);
                        return PRP_COMPOSITE;
                    }
                }
                if (jacobi == -1)
                    break;

                /* if we get to the 5th d, make sure we aren't dealing with a square... */
                if (d == 13)
                {
                    if (mpz_perfect_square_p(n) > 0)
                    {
                        mpz_clear(zD);
                        return PRP_COMPOSITE;
                    }
                }

                if (d < 0)
                {
                    d *= -1;
                    d += 2;
                }
                else
                {
                    d += 2;
                    d *= -1;
                }

                /* make sure we don't search forever */
                if (d >= max_d)
                {
                    mpz_clear(zD);
                    return PRP_ERROR;
                }

                mpz_set_si(zD, (int)d);
            }
            mpz_clear(zD);

            q = (1 - d) / 4;

            return mpz_lucas_prp(n, p, q);

        }/* method mpz_selfridge_prp */

        /* *******************************************************************************
         * mpz_lucas_prp:
         * A "Lucas pseudoprime" with parameters (P,Q) is a composite n with D=P^2-4Q,
         * (n,2QD)=1 such that U_(n-(D/n)) == 0 mod n [(D/n) is the Jacobi symbol]
         * *******************************************************************************/
        public static int mpz_lucas_prp(mpz_t n, long p, long q)
        {
            mpz_t zD = new();
            mpz_t res = new();
            mpz_t index = new();
            mpz_t uh = new(), vl = new(), vh = new(), ql = new(), qh = new(), tmp = new(); /* used for calculating the Lucas U sequence */
            int s = 0, j = 0;
            int ret = 0;
            long d = p * p - 4 * q;

            if (d == 0) /* Does not produce a proper Lucas sequence */
                return PRP_ERROR;

            if (mpz_cmp_ui(n, 2) < 0)
                return PRP_COMPOSITE;

            if (mpz_divisible_ui_p(n, 2) > 0)
            {
                if (mpz_cmp_ui(n, 2) == 0)
                    return PRP_PRIME;
                else
                    return PRP_COMPOSITE;
            }

            mpz_init(index);
            mpz_init_set_si(zD, (int)d);
            mpz_init(res);

            mpz_mul_si(res, zD, (int)q);
            mpz_mul_ui(res, res, 2);
            mpz_gcd(res, res, n);
            if ((mpz_cmp(res, n) != 0) && (mpz_cmp_ui(res, 1) > 0))
            {
                mpz_clear(zD);
                mpz_clear(res);
                mpz_clear(index);
                return PRP_COMPOSITE;
            }

            /* index = n-(D/n), where (D/n) is the Jacobi symbol */
            mpz_set(index, n);
            ret = mpz_jacobi(zD, n);
            if (ret == -1)
                mpz_add_ui(index, index, 1);
            else if (ret == 1)
                mpz_sub_ui(index, index, 1);

            /* mpz_lucasumod(res, p, q, index, n); */
            mpz_init_set_si(uh, 1);
            mpz_init_set_si(vl, 2);
            mpz_init_set_si(vh, (int) p);
            mpz_init_set_si(ql, 1);
            mpz_init_set_si(qh, 1);
            mpz_init_set_si(tmp, 0);

            s = (int)mpz_scan1(index, 0);
            for (j = (int)mpz_sizeinbase(index, 2) - 1; j >= s + 1; j--)
            {
                /* ql = ql*qh (mod n) */
                mpz_mul(ql, ql, qh);
                mpz_mod(ql, ql, n);
                if (mpz_tstbit(index, (mp_bitcnt_t) j) == 1)
                {
                    /* qh = ql*q */
                    mpz_mul_si(qh, ql, (int)q);

                    /* uh = uh*vh (mod n) */
                    mpz_mul(uh, uh, vh);
                    mpz_mod(uh, uh, n);

                    /* vl = vh*vl - p*ql (mod n) */
                    mpz_mul(vl, vh, vl);
                    mpz_mul_si(tmp, ql, (int)p);
                    mpz_sub(vl, vl, tmp);
                    mpz_mod(vl, vl, n);

                    /* vh = vh*vh - 2*qh (mod n) */
                    mpz_mul(vh, vh, vh);
                    mpz_mul_si(tmp, qh, 2);
                    mpz_sub(vh, vh, tmp);
                    mpz_mod(vh, vh, n);
                }
                else
                {
                    /* qh = ql */
                    mpz_set(qh, ql);

                    /* uh = uh*vl - ql (mod n) */
                    mpz_mul(uh, uh, vl);
                    mpz_sub(uh, uh, ql);
                    mpz_mod(uh, uh, n);

                    /* vh = vh*vl - p*ql (mod n) */
                    mpz_mul(vh, vh, vl);
                    mpz_mul_si(tmp, ql, (int)p);
                    mpz_sub(vh, vh, tmp);
                    mpz_mod(vh, vh, n);

                    /* vl = vl*vl - 2*ql (mod n) */
                    mpz_mul(vl, vl, vl);
                    mpz_mul_si(tmp, ql, (int)2);
                    mpz_sub(vl, vl, tmp);
                    mpz_mod(vl, vl, n);
                }
            }
            /* ql = ql*qh */
            mpz_mul(ql, ql, qh);

            /* qh = ql*q */
            mpz_mul_si(qh, ql, (int)q);

            /* uh = uh*vl - ql */
            mpz_mul(uh, uh, vl);
            mpz_sub(uh, uh, ql);

            /* vl = vh*vl - p*ql */
            mpz_mul(vl, vh, vl);
            mpz_mul_si(tmp, ql, (int)p);
            mpz_sub(vl, vl, tmp);

            /* ql = ql*qh */
            mpz_mul(ql, ql, qh);

            for (j = 1; j <= s; j++)
            {
                /* uh = uh*vl (mod n) */
                mpz_mul(uh, uh, vl);
                mpz_mod(uh, uh, n);

                /* vl = vl*vl - 2*ql (mod n) */
                mpz_mul(vl, vl, vl);
                mpz_mul_si(tmp, ql, 2);
                mpz_sub(vl, vl, tmp);
                mpz_mod(vl, vl, n);

                /* ql = ql*ql (mod n) */
                mpz_mul(ql, ql, ql);
                mpz_mod(ql, ql, n);
            }

            mpz_mod(res, uh, n); /* uh contains our return value */

            mpz_clear(zD);
            mpz_clear(index);
            mpz_clear(uh);
            mpz_clear(vl);
            mpz_clear(vh);
            mpz_clear(ql);
            mpz_clear(qh);
            mpz_clear(tmp);

            if (mpz_cmp_ui(res, 0) == 0)
            {
                mpz_clear(res);
                return PRP_PRP;
            }
            else
            {
                mpz_clear(res);
                return PRP_COMPOSITE;
            }

        }/* method mpz_lucas_prp */

        public int SizeInBase10 => gmp_base10(this.Data);
        public static int gmp_base10(mpz_t x)
        {
            mpz_t t = new();    //temp
            int g;      //guess: either correct or +1

            mpz_init(t);
            g = (int)mpz_sizeinbase(x, 10);
            mpz_set_ui(t, 10);
            mpz_pow_ui(t, t, (uint)g - 1);
            g = g - (mpz_cmp(t, x) > 0 ? 1 : 0);
            mpz_clear(t);
            return g;
        }

        // TODO: Create a version of this method which takes in a parameter to represent how well tested the prime should be.
        public mpz_t NextPrimeGMP()
        {
            var z = newz();
            mpz_nextprime(z, this);
            return z;
        }

        public static mpz_t Gcd(mpz_t x, mpz_t y)
        {
            var z = newz();
            mpz_gcd(z, x, y);
            return z;
        }

        public static mpz_t Gcd(mpz_t x, int y)
        {
            var z = newz();
            if (y >= 0)
                mpz_gcd_ui(z, x, (uint)y);
            else
                mpz_gcd_ui(z, x, (uint)(-y));

            return z;
        }

        public static mpz_t Gcd(int x, mpz_t y)
        {
            var z = newz();
            if (x >= 0)
                mpz_gcd_ui(z, y, (uint)x);
            else
                mpz_gcd_ui(z, y, (uint)(-x));

            return z;
        }

        public static mpz_t Gcd(mpz_t x, uint y)
        {
            var z = newz();
            mpz_gcd_ui(z, x, y);
            return z;
        }

        public static mpz_t Gcd(uint x, mpz_t y)
        {
            var z = newz();
            mpz_gcd_ui(z, y, x);
            return z;
        }

        public static mpz_t Gcd(mpz_t x, mpz_t y, out mpz_t a, out mpz_t b)
        {
            var z = newz();
            a = newz();
            b = newz();

            mpz_gcdext(z, a, b, x, y);
            return z;
        }

        public static mpz_t Gcd(mpz_t x, mpz_t y, out mpz_t a)
        {
            var z = newz();
            a = newz();

            mpz_gcdext(z, a, null, x, y);
            return z;
        }

        public static mpz_t Lcm(mpz_t x, mpz_t y)
        {
            var z = newz();
            mpz_lcm(z, x, y);
            return z;
        }

        public static mpz_t Lcm(mpz_t x, int y)
        {
            var z = newz();
            if (y >= 0)
                mpz_lcm_ui(z, x, (uint)y);
            else
                mpz_lcm_ui(z, x, (uint)(-y));

            return z;
        }

        public static mpz_t Lcm(int x, mpz_t y)
        {
            var z = newz();
            if (x >= 0)
                mpz_lcm_ui(z, y, (uint)x);
            else
                mpz_lcm_ui(z, y, (uint)(-x));

            return z;
        }

        public static mpz_t Lcm(mpz_t x, uint y)
        {
            var z = newz();
            mpz_lcm_ui(z, x, y);
            return z;
        }

        public static mpz_t Lcm(uint x, mpz_t y)
        {
            var z = newz();
            mpz_lcm_ui(z, y, x);
            return z;
        }

        public static int LegendreSymbol(int x, int primeY)
        {

            if ((GmpInt)primeY == 2) return 0;
            return mpz_jacobi((GmpInt)x, (GmpInt)primeY);
        }

        public static int LegendreSymbol(GmpInt x, GmpInt primeY)
        {
            if (primeY == 2) return 0;
            return mpz_jacobi(x.Data, primeY.Data);
        }

        public static int LegendreSymbol(mpz_t x, mpz_t primeY)
        {

            if ((GmpInt)primeY == 2) return 0;
            return mpz_jacobi(x, primeY);
        }

        public static int JacobiSymbol(mpz_t x, mpz_t y)
        {
            return mpz_jacobi(x, y);
        }

        public static int JacobiSymbol(mpz_t x, int y)
        {
            return mpz_kronecker_si(x, y);
        }

        public static int JacobiSymbol(int x, mpz_t y)
        {
            return mpz_si_kronecker(x, y);
        }

        public static int JacobiSymbol(mpz_t x, uint y)
        {
            return mpz_kronecker_ui(x, y);
        }

        public static int JacobiSymbol(uint x, mpz_t y)
        {
            return mpz_ui_kronecker(x, y);
        }

        public static int KroneckerSymbol(mpz_t x, mpz_t y)
        {
            return mpz_kronecker(x, y);
        }

        public static int KroneckerSymbol(mpz_t x, int y)
        {
            return mpz_kronecker_si(x, y);
        }

        public static int KroneckerSymbol(int x, mpz_t y)
        {
            return mpz_si_kronecker(x, y);
        }

        public static int KroneckerSymbol(mpz_t x, uint y)
        {
            return mpz_kronecker_ui(x, y);
        }

        public static int KroneckerSymbol(uint x, mpz_t y)
        {
            return mpz_ui_kronecker(x, y);
        }

        public mpz_t RemoveFactor(mpz_t factor)
        {
            var z = newz();
            mpz_remove(z, this, factor);
            return z;
        }

        public mpz_t RemoveFactor(mpz_t factor, out int count)
        {
            var z = newz();
            count = (int)mpz_remove(z, this, factor);
            return z;
        }

        public static mpz_t Factorial(int x)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_fac_ui(z, (uint)x);
            return z;
        }

        public static mpz_t Factorial(uint x)
        {
            var z = newz();
            mpz_fac_ui(z, x);
            return z;
        }

        public static mpz_t Binomial(mpz_t n, uint k)
        {
            var z = newz();
            mpz_bin_ui(z, n, k);
            return z;
        }

        public static mpz_t Binomial(mpz_t n, int k)
        {
            if (k < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_bin_ui(z, n, (uint)k);
            return z;
        }

        public static mpz_t Binomial(uint n, uint k)
        {
            var z = newz();
            mpz_bin_uiui(z, n, k);
            return z;
        }

        public static mpz_t Binomial(int n, int k)
        {
            if (k < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            if (n >= 0)
            {
                mpz_bin_uiui(z, (uint)n, (uint)k);
                return z;
            }
            else
            {
                // Use the identity bin(n,k) = (-1)^k * bin(-n+k-1,k)
                mpz_bin_uiui(z, (uint)(-n + k - 1), (uint)k);

                if ((k & 1) != 0)
                {
                    mpz_t res = Negate(z);
                    z.Dispose();
                    return res;
                }
                else
                {
                    return z;
                }
            }

        }


        public static mpz_t Fibonacci(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_fib_ui(z, (uint)n);
            return z;
        }

        public static mpz_t Fibonacci(uint n)
        {
            var z = newz();
            mpz_fib_ui(z, n);
            return z;
        }

        public static mpz_t Fibonacci(int n, out mpz_t previous)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            previous = newz();
            mpz_fib2_ui(z, previous, (uint)n);
            return z;
        }

        public static mpz_t Fibonacci(uint n, out mpz_t previous)
        {
            var z = newz();
            previous = newz();
            mpz_fib2_ui(z, previous, n);
            return z;
        }

        public static mpz_t Lucas(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            mpz_lucnum_ui(z, (uint)n);
            return z;
        }

        public static mpz_t Lucas(uint n)
        {
            var z = newz();
            mpz_lucnum_ui(z, n);
            return z;
        }

        public static mpz_t Lucas(int n, out mpz_t previous)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException();

            var z = newz();
            previous = newz();
            mpz_lucnum2_ui(z, previous, (uint)n);
            return z;
        }

        public static mpz_t Lucas(uint n, out mpz_t previous)
        {
            var z = newz();
            previous = newz();
            mpz_lucnum2_ui(z, previous, n);
            return z;
        }

        #endregion

        #region Bitwise Functions

        public int CountOnes()
        {
            return (int)mpz_popcount(this);
        }

        public static int HammingDistance(mpz_t x, mpz_t y)
        {
            return (int)mpz_hamdist(x, y);
        }

        public int IndexOfZero(int startingIndex)
        {
            unchecked
            {
                if (startingIndex < 0)
                    throw new ArgumentOutOfRangeException();

                // Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
                return (int)mpz_scan0(this, (uint)startingIndex);
            }
        }

        public int IndexOfOne(int startingIndex)
        {
            unchecked
            {
                if (startingIndex < 0)
                    throw new ArgumentOutOfRangeException();

                // Note that the result might be uint.MaxValue in which case it gets cast to -1, which is what is intended.
                return (int)mpz_scan1(this, (uint)startingIndex);
            }
        }

        public int GetBit(int index)
        {
            return mpir.mpz_tstbit(this, (uint)index);
        }

        public GmpInt SetBit(int index, int value)
        {
            mpz_t z = (mpz_t)this;

            if (value == 0)
                mpir.mpz_clrbit(z, (uint)index);
            else
                mpir.mpz_setbit(z, (uint)index);

            return z;
        }

        public bool EqualsMod(mpz_t x, mpz_t mod)
        {
            return mpir.mpz_congruent_p(this, x, mod) != 0;
        }

        public bool EqualsMod(int x, int mod)
        {
            if (mod < 0)
                throw new ArgumentOutOfRangeException();

            if (x >= 0)
            {
                return mpir.mpz_congruent_ui_p(this, (uint)x, (uint)mod) != 0;
            }
            else
            {
                uint xAsUint = (uint)((x % mod) + mod);
                return mpir.mpz_congruent_ui_p(this, xAsUint, (uint)mod) != 0;
            }
        }

        public bool EqualsMod(uint x, uint mod)
        {
            return mpir.mpz_congruent_ui_p(this, x, mod) != 0;
        }


        public int CompareAbsTo(object obj)
        {
            GmpInt? objAsBigInt = obj as GmpInt;

            if (object.ReferenceEquals(objAsBigInt, null))
            {
                if (obj is int)
                    return this.CompareAbsTo((int)obj);
                else if (obj is uint)
                    return this.CompareAbsTo((uint)obj);
                else if (obj is long)
                    return this.CompareAbsTo((long)obj);
                else if (obj is ulong)
                    return this.CompareAbsTo((ulong)obj);
                else if (obj is double)
                    return this.CompareAbsTo((double)obj);
                else if (obj is float)
                    return this.CompareAbsTo((float)obj);
                else if (obj is short)
                    return this.CompareAbsTo((short)obj);
                else if (obj is ushort)
                    return this.CompareAbsTo((ushort)obj);
                else if (obj is byte)
                    return this.CompareAbsTo((byte)obj);
                else if (obj is sbyte)
                    return this.CompareAbsTo((sbyte)obj);
                else if (obj is decimal)
                    return this.CompareAbsTo((decimal)obj);
                else if (obj is BigInteger)
                    return this.CompareAbsTo((BigInteger)obj);
                else if (obj is string)
                    return this.CompareAbsTo((obj as string));
                else if (obj is mpz_t)
                    return this.CompareAbsTo((GmpInt)(mpz_t)obj);
                else
                    throw new ArgumentException("Cannot compare to " + obj.GetType());
            }

            return this.CompareAbsTo(objAsBigInt);
        }

        public int CompareAbsTo(mpz_t other)
        {
            return mpir.mpz_cmpabs(this, other);
        }

        public int CompareAbsTo(int other)
        {
            return mpir.mpz_cmpabs_ui(this, (uint)other);
        }

        public int CompareAbsTo(uint other)
        {
            return mpir.mpz_cmpabs_ui(this, other);
        }

        public int CompareAbsTo(long other)
        {
            return this.CompareAbsTo((GmpInt)other);
        }

        public int CompareAbsTo(ulong other)
        {
            return this.CompareAbsTo((GmpInt)other);
        }

        public int CompareAbsTo(double other)
        {
            return mpir.mpz_cmpabs_d(this, other);
        }

        public int CompareAbsTo(decimal other)
        {
            return mpir.mpz_cmpabs(this, (GmpInt)other);
        }

        public int CompareAbsTo(BigInteger other)
        {
            return mpir.mpz_cmpabs(this, (GmpInt)other);
        }

        #endregion

    }
}