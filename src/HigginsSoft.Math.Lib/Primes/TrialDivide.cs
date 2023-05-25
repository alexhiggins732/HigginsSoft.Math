/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Numerics;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public partial class Primes
    {
        #region Trial Divide Int

        //use the first 1028 primes for trial division in a primality check
        const int probable_prime_tdiv_limit = 4099; 


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(int i) => TrialDivide(i);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(uint i) => TrialDivide(i);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(long i) => TrialDivide(i);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(ulong i) => TrialDivide(i);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(BigInteger i)
        {
            if (i <= 2) return i == 2;
            var bitLength = i.GetBitLength();
            if (bitLength < 65)
            {
                if (bitLength < 32) return IsPrime((int)i);
                if (bitLength < 33) return IsPrime((uint)i);
                if (bitLength < 64) return IsPrime((long)i);
                return IsPrime((uint)i);
            }
            var result = TrialDivide(i, probable_prime_tdiv_limit, out _);
            return result || ((GmpInt)i).IsProbablyPrimeRabinMiller(20);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(GmpInt i)
        {
            if (i <= 2) return i == 2;
            var bitLength = i.BitLength;
            if (bitLength < 65)
            {
                if (bitLength < 32) return IsPrime((int)i);
                if (bitLength < 33) return IsPrime((uint)i);
                if (bitLength < 64) return IsPrime((long)i);
                return IsPrime((uint)i);
            }
            var result = TrialDivide(i, probable_prime_tdiv_limit, out _);
            return result || i.IsProbablyPrimeRabinMiller();

        }


        /// <summary>
        /// Tests if a number <paramref name="n"/> is prime using trial division.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <returns><c>true</c> if the number <paramref name="n"/> is prime; otherwise, false.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(int n)
        {
            var result = TrialDivide(n, PrimeData.MaxIntFactorPrime, out _);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(int n, int maxPrime)
        {
            var result = TrialDivide(n, maxPrime, out _);
            return result;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(int n, out int factor)
            => TrialDivide(n, PrimeData.MaxIntFactorPrime, out factor);


        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division against the supplied <paramref name="candidates"/> and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(int n, IEnumerable<int> candidates, out int factor)
        {
            factor = 1;
            if (n < 2) return false;
            if (n == 2) return (factor = n) == 2;
            var root = (int)System.Math.Sqrt(n);
            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n;
                    break;
                }
            }
            return factor == n;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division up to the specified maximum prime and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <param name="maxPrime">The largest prime factor to check.</param>
        /// <param name="factor">The smallest factor of <paramref name="n"/> that was found during trial division, or 1 if <paramref name="n"/> is prime.</param>
        /// <returns>True if <paramref name="n"/> is prime, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(int n, int maxPrime, out int factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var arr = IntFactorPrimes;
            var root = (int)System.Math.Sqrt(n);
            var limit = MathLib.Min(maxPrime, root);
            int p = 0;
            for (var j = 0; p <= limit && j < arr.Length; j++)
            {
                p = arr[j];
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                if (p >= root)
                {
                    factor = n;
                    break;
                }
            }
            return factor == n;
        }

        #endregion Trial Divide Int


        #region Trial Divide Uint

        /// <summary>
        /// Tests if a number <paramref name="n"/> is prime using trial division.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <returns><c>true</c> if the number <paramref name="n"/> is prime; otherwise, false.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(uint n)
        {
            var result = TrialDivide(n, PrimeData.MaxUintFactorPrime, out _);
            return result;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(uint n, out uint factor)
            => TrialDivide(n, PrimeData.MaxUintFactorPrime, out factor);


        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division against the supplied <paramref name="candidates"/> and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(uint n, IEnumerable<int> candidates, out uint factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var root = (int)System.Math.Sqrt(n);
            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = (uint)p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n;
                    break;
                }
            }
            return factor == n;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division up to the specified maximum prime and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <param name="maxPrime">The largest prime factor to check.</param>
        /// <param name="factor">The smallest factor of <paramref name="n"/> that was found during trial division, or 1 if <paramref name="n"/> is prime.</param>
        /// <returns>True if <paramref name="n"/> is prime, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(uint n, int maxPrime, out uint factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var arr = UintFactorPrimes;
            var root = (int)System.Math.Sqrt(n);
            var limit = MathLib.Min(maxPrime, root);
            int p = 0;
            for (var j = 0; p <= limit && j <= arr.Length; j++)
            {
                p = arr[j];
                if (n % p == 0)
                {
                    factor = (uint)p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n;
                }
            }
            return factor == n;
        }

        #endregion Trial Divide Uint



        #region Trial Divide Long


        /// <summary>
        /// Tests if a number <paramref name="n"/> is prime using trial division.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <returns><c>true</c> if the number <paramref name="n"/> is prime; otherwise, false.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(long n)
        {
            var result = TrialDivide(n, PrimeData.MaxLongFactorPrime, out _);
            return result;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(long n, out long factor)
            => TrialDivide(n, PrimeData.MaxLongFactorPrime, out factor);


        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division against the supplied <paramref name="candidates"/> and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(long n, IEnumerable<uint> candidates, out long factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var root = (int)System.Math.Sqrt(n);
            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n;
                    break;
                }
            }
            return factor == n;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division up to the specified maximum prime and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <param name="maxPrime">The largest prime factor to check.</param>
        /// <param name="factor">The smallest factor of <paramref name="n"/> that was found during trial division, or 1 if <paramref name="n"/> is prime.</param>
        /// <returns>True if <paramref name="n"/> is prime, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(long n, uint maxPrime, out long factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var root = (uint)System.Math.Sqrt(n);
            var limit = MathLib.Min(maxPrime, root);
            var candidates = new PrimeGeneratorUnsafeUint(limit);
            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                if (p >= limit)
                {
                    if (p >= root) factor = n;
                    break;
                }

            }
            return factor == n;
        }


        #endregion Trial Divide Long


        #region Trial Divide Ulong

        /// <summary>
        /// Tests if a number <paramref name="n"/> is prime using trial division.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <returns><c>true</c> if the number <paramref name="n"/> is prime; otherwise, false.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(ulong n)
        {
            var result = TrialDivide(n, PrimeData.MaxUlongFactorPrime, out _);
            return result;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(ulong n, out ulong factor)
            => TrialDivide(n, PrimeData.MaxUlongFactorPrime, out factor);


        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division against the supplied <paramref name="candidates"/> and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(ulong n, IEnumerable<uint> candidates, out ulong factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var root = (int)System.Math.Sqrt(n);
            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n;
                    break;
                }
            }
            return factor == n;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division up to the specified maximum prime and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The number to test for primality.</param>
        /// <param name="maxPrime">The largest prime factor to check.</param>
        /// <param name="factor">The smallest factor of <paramref name="n"/> that was found during trial division, or 1 if <paramref name="n"/> is prime.</param>
        /// <returns>True if <paramref name="n"/> is prime, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(ulong n, uint maxPrime, out ulong factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            var root = (uint)System.Math.Sqrt(n);
            var limit = MathLib.Min(maxPrime, root);
            var candidates = new PrimeGeneratorUnsafeUint(limit);
            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= limit)
                {
                    if (p >= root) factor = n;
                    break;
                }
            }
            return factor == n;
        }


        #endregion Trial Divide Uint

        #region Traild Divide MP

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(GmpInt n, out GmpInt factor)
            => TrialDivide(n, PrimeData.MaxUintFactorPrime, out factor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(GmpInt n, int maxPrime, out GmpInt factor)
        {
            factor = 1;
            if (n <= 1) return false;
            GmpInt root = n.Sqrt();
            if (root == 1)
            {
                factor = n.Clone();
                return true;
            }
            var limit = MathLib.Min(maxPrime, root);
            var candidates = Primes.UintFactorPrimes;

            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= limit)
                {
                    if (p >= root) factor = n.Clone();
                    break;
                }

            }
            return factor == n;
        }


        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division against the supplied <paramref name="candidates"/> and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(GmpInt n, IEnumerable<int> candidates, out GmpInt factor)
        {
            factor = 1;
            if (n == 2) return (factor = n) == 2;
            if (n <= 1) return false;
            GmpInt root = n.Sqrt();
            if (root == 1)
            {
                factor = n.Clone();
                return true;
            }

            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n.Clone();
                    break;
                }

            }
            return factor == n;
        }

        /// <summary>
        /// Tests if <paramref name="n"/> is prime using trial division against the supplied <paramref name="candidates"/> and sets <paramref name="factor"/> to the first factor if one is found or 1 otherwise.
        /// </summary>
        /// <param name="n">The integer to be tested for primality.</param>
        /// <param name="factor">If <paramref name="n"/> is composite, the smallest prime factor of <paramref name="n"/>; otherwise, 1.</param>
        /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrialDivide(GmpInt n, IEnumerable<uint> candidates, out GmpInt factor)
        {
            factor = 1;
            if (n <= 1) return false;
            if (n == 2) return (factor = n) == 2;
            GmpInt root = n.Sqrt();
            if (root == 1)
            {
                factor = n.Clone();
                return true;
            }

            foreach (var p in candidates)
            {
                if (n % p == 0)
                {
                    factor = p;
                    break;
                }
                else if (p >= root)
                {
                    factor = n.Clone();
                    break;
                }
            }
            return factor == n;
        }


        #endregion
    }
}