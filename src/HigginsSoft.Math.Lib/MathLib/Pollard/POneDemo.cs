using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FactoringAlgorithms
{
    public static class Factoring
    {
        /// <summary>
        /// Generates a list of all primes up to the specified limit using the Sieve of Eratosthenes.
        /// </summary>
        /// <param name="limit">The inclusive upper bound for primes.</param>
        /// <returns>A list of prime numbers up to limit.</returns>
        public static List<int> SievePrimes(int limit)
        {
            bool[] isComposite = new bool[limit + 1];
            List<int> primes = new List<int>();
            for (int i = 2; i <= limit; i++)
            {
                if (!isComposite[i])
                {
                    primes.Add(i);
                    for (int j = i * 2; j <= limit; j += i)
                    {
                        isComposite[j] = true;
                    }
                }
            }
            return primes;
        }

        /// <summary>
        /// Attempts to factor the composite number n using Pollard's P–1 algorithm.
        /// The method computes a = 2^(p^e) mod n for every prime p ≤ B (with e the largest exponent such that p^e ≤ B)
        /// and then returns gcd(a – 1, n) if it is a nontrivial factor.
        /// </summary>
        /// <param name="n">The composite number to factor.</param>
        /// <param name="B">The smoothness bound.</param>
        /// <returns>A nontrivial factor of n if found; otherwise, returns 1.</returns>
        public static BigInteger PollardPMinusOne(BigInteger n, int B)
        {
            BigInteger a = 2;
            List<int> primes = SievePrimes(B);

            foreach (int p in primes)
            {
                // Determine the largest exponent e such that p^e ≤ B.
                int e = (int)Math.Floor(Math.Log(B) / Math.Log(p));
                BigInteger exponent = BigInteger.Pow(p, e);
                a = BigInteger.ModPow(a, exponent, n);
            }

            BigInteger d = BigInteger.GreatestCommonDivisor(a - 1, n);
            return (d > 1 && d < n) ? d : 1;
        }

        /// <summary>
        /// Attempts to factor the composite number n using Pollard's P+1 algorithm.
        /// It computes a = 2^(p^e) mod n for every prime p ≤ B (with e as the maximal exponent such that p^e ≤ B)
        /// and returns gcd(a + 1, n) if it is a nontrivial factor.
        /// </summary>
        /// <param name="n">The composite number to factor.</param>
        /// <param name="B">The smoothness bound.</param>
        /// <returns>A nontrivial factor of n if found; otherwise, returns 1.</returns>
        public static BigInteger PollardPPlusOne(BigInteger n, int B)
        {
            BigInteger a = 2;
            List<int> primes = SievePrimes(B);

            foreach (int p in primes)
            {
                int e = (int)Math.Floor(Math.Log(B) / Math.Log(p));
                BigInteger exponent = BigInteger.Pow(p, e);
                a = BigInteger.ModPow(a, exponent, n);
            }

            BigInteger d = BigInteger.GreatestCommonDivisor(a + 1, n);
            return (d > 1 && d < n) ? d : 1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Example composite number (e.g., 10403 = 101 * 103)
            BigInteger composite = 10403;
            int smoothnessBound = 1000; // Adjust B as needed

            Console.WriteLine($"Attempting Pollard P–1 factorization on {composite} with B = {smoothnessBound}.");
            BigInteger factorPMinus = Factoring.PollardPMinusOne(composite, smoothnessBound);
            if (factorPMinus > 1 && factorPMinus < composite)
                Console.WriteLine($"Pollard P–1 found factor: {factorPMinus}");
            else
                Console.WriteLine("Pollard P–1 failed to find a factor.");

            Console.WriteLine();

            Console.WriteLine($"Attempting Pollard P+1 factorization on {composite} with B = {smoothnessBound}.");
            BigInteger factorPPlus = Factoring.PollardPPlusOne(composite, smoothnessBound);
            if (factorPPlus > 1 && factorPPlus < composite)
                Console.WriteLine($"Pollard P+1 found factor: {factorPPlus}");
            else
                Console.WriteLine("Pollard P+1 failed to find a factor.");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
