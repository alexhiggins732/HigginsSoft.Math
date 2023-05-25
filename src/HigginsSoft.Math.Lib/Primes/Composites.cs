using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    public partial class Composites
    {
        /// <summary>
        /// Generates composites with distinct prime factors.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<int> GenerateWithUniqueFactors(int count)
        {
            int j = 0;
            for (var i = 3; j < count; i++)
            {
                if (MathLib.IsPowerOfTwo(i))
                    continue;
                j++;
                var mask = i;
                var product = 1;
                for (var bit = 0; mask > 0; bit++, mask >>= 1)
                {
                    if ((mask & 1) == 1)
                    {
                        product *= Primes.IntFactorPrimes[bit];
                    }
                }
                yield return product;
            }
        }

        public static IEnumerable<int> GenerateTo(int limit)
        {
            if (limit > 3)
            {
                var primes = Primes.IntFactorPrimes;
                var c = 2;
                var prime = 2;
                for (var i = 0; c <= limit && i < primes.Length; i++)
                    for (prime = primes[i];  c <= prime && c <= limit;c++)
                        if (c != prime)
                            yield return c;

            }
        }
        public static IEnumerable<int> GenerateAll(int count)
        {

            if (count > 0)
            {
                var primes = Primes.IntFactorPrimes;
                var c = 2;
                var prime = 2;
                var j = 0;
                for (var i = 0; j < count && i < primes.Length; i++)
                {
                    prime = primes[i];
                    while (c <= prime && j < count)
                    {
                        if (c != prime)
                        {
                            yield return c;
                            j++;
                        }
                        c++;

                    }
                }
            }
        }

    }
}
