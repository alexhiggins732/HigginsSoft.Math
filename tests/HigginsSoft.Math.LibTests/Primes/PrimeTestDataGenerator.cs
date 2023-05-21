using System.Collections;

namespace HigginsSoft.Math.Lib.Tests.PrimalityCheckTests
{
    public class PrimeTestDataGenerator 
    {

        static readonly List<PrimeTestData> data;

        public static List<PrimeTestData> GetSmallPrimes()
            => data;

        static PrimeTestDataGenerator()
        {
            data = new List<PrimeTestData>();
            var primes = Primes.IntFactorPrimes;


            var max = primes.Max();
            var j = 0;

            for (var i = 0; j < primes.Length && i <= max; i++)
            {
                var p = primes[j];

                var isPrime = p == i;
                data.Add(new(i, p == i));
                if (isPrime)
                {
                    j++;
                }
            }

        }
        public class PrimeTestData
        {
            public readonly int N;
            public readonly bool IsPrime;

            public PrimeTestData(int value, bool isPrime)
            {
                N = value;
                IsPrime = isPrime;
            }
        }
    }
}