using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    public static class PollardPm1
    {
        public static BigInteger Run(BigInteger n, int B1 = 100_000, int B2 = 1_000_000, int baseA = 2)
        {
            if (n.IsEven)
                return 2;

            //var a = new BigInteger(baseA);

            var gen = new PrimeGeneratorUnsafe(B2);

            // ======== Stage 1 ========
            //var primes = GeneratePrimes(B1);
            var allPrimes = gen.ToList();
            var primes = allPrimes.Where(x => x <= B1);// GeneratePrimes(B1);
            BigInteger M = 1;
            foreach (var p in primes)
            {
                int e = (int)(BigInteger.Log(B1) / BigInteger.Log(p));
                M *= BigInteger.Pow(p, e);
            }

            var x = BigInteger.ModPow(baseA, M, n);
            var g = BigInteger.GreatestCommonDivisor(x - 1, n);
            if (g > 1 && g < n)
                return g;

           // a = x; //save stage 1
            // ======== Stage 2 ========
            //primes = GeneratePrimes(B2);
            primes = primes.Where(x => x > B1).ToList();
            foreach (var q in primes.Where(p => p > B1))
            {
                var y = BigInteger.ModPow(x, q, n); // x from Stage 1
                var gcd = BigInteger.GreatestCommonDivisor(y - 1, n);
                if (gcd > 1 && gcd < n)
                    return g;
            }

            return 1; // No factor found
        }
    }

}