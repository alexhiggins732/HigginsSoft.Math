using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public static IEnumerable<(int Value, int P, int Q)> GenerateSemiPrimes(int rootLimit = PrimeData.MaxIntFactorPrime)
        {
            for (var i = 0; i < Primes.IntFactorPrimes.Length; i++)
            {
                var x = Primes.IntFactorPrimes[i];

                for (var k = i; k < Primes.IntFactorPrimes.Length; k++)
                {
                    var y = Primes.IntFactorPrimes[k];
                    if (y >= rootLimit)
                        break;
                    yield return (x * y, x, y);
                }
                if (x >= rootLimit)
                    break;
            }

        }
        public static IEnumerable<int> GenerateTo(int limit)
        {
            if (limit > 0)
            {
                var lg = MathLib.ILogB(limit);

                var primes = new PrimeGeneratorUnsafe(limit);
                var iter = primes.GetEnumerator();
                iter.MoveNext();

                int prime = iter.Current;

                bool moved = true;
                for (var c = 2; c < limit; c++)
                {
                    if (c < prime || !moved)
                        yield return c;
                    else if (moved)
                    {
                        moved = iter.MoveNext();
                        if (moved)
                            prime = iter.Current;
                    }
                }
            }
        }

        public static IEnumerable<int> GenerateTo(int start, int limit)
        {
            if (limit > 0)
            {
                var lg = MathLib.ILogB(limit);

                var primes = new PrimeGeneratorUnsafe(start, limit);
                var iter = primes.GetEnumerator();
                iter.MoveNext();

                int prime = iter.Current;

                bool moved = true;
                for (var c = start; c < limit; c++)
                {
                    if (c < prime || !moved)
                        yield return c;
                    else if (moved)
                    {
                        moved = iter.MoveNext();
                        if (moved)
                            prime = iter.Current;
                    }
                }
            }
        }


        public static IEnumerable<int> GenerateAll(int count)
        {

            if (count > 0)
            {
                var primes = Primes.UintFactorPrimes;
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
