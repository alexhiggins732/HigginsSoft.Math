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
            if (sign < 1) {
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

        public bool IsProbablyPrimeRabinMiller(int repetitions = 20)
        {
            int result = mpz_probab_prime_p(this, repetitions);

            return result != 0;
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