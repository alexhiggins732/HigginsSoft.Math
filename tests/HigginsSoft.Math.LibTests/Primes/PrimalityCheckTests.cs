using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HigginsSoft.Math.Lib.Tests.PrimalityCheckTests
{


    [TestClass()]
    public class PrimalityCheckTest
    {
        [TestMethod()]
        public void IsPrimeTest()
        {
            var primes = Primes.IntFactorPrimes
                .Where(x => x > 300).ToArray();


            var max = primes.Max();
            var j = 0;

            for (var i = 300; j < primes.Length && i <= max; i++)
            {
                var p = primes[j];
                var isprime = CongruenceChecker.IsPrime(i);
                var expected = p == i;
                if (isprime != expected)
                {
                    var message = $"Method failed for {i}";
                    Assert.AreEqual(expected, isprime, message);
                }

                if (i == p)
                {
                    j++;
                }
            }

        }

        [TestMethod]
        public void CountPrimeCongruenceCandidates()
        {
            // First 88 congruence candidates when testing 1073741827
            var candidates = new[]
            {
                 32514, 32408, 32328, 32260, 32201, 32147, 32098, 32052, 32009, 31969, 31930, 31894,
                31858, 31825, 31792, 31760, 31730, 31700, 31671, 31643, 31616, 31590, 31564, 31538, 31513,
                31489, 31465, 31442, 31419, 31396, 31374, 31352, 31331, 31309, 31289, 31268, 31248, 31228,
                31208, 31189, 31170, 31151, 31132, 31114, 31096, 31078, 31060, 31042, 31025, 31008, 30991,
                30974, 30957, 30941, 30924, 30908, 30892, 30876, 30860, 30845, 30829, 30814, 30799, 30784,
                30769, 30754, 30739, 30724, 30710, 30696, 30681, 30667, 30653, 30639, 30625, 30611, 30598,
                30584, 30571, 30557, 30544, 30531, 30518, 30505, 30492, 30479, 30466, 30453, 30441,

            };

            var count1 = candidates.Where(x => Primes.IsPrime(x)).Count();
            var count2 = candidates.Where(x => CongruenceChecker.IsPrime(x)).Count();

            Assert.AreEqual(count1, count2);

        }

        //[Ignore("Takes too long to converge")]
        [TestMethod()]
        public void CheckIsPrimeTest()
        {
            CongruenceChecker.IsPrime(1);
            Primes.IsPrime(1);

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            var result1 = false;
            var result2 = false;
            for (var i = 300; i < 7500; i++)
            {
                sw1.Start();
                result1 = Primes.IsPrime(i);
                sw1.Stop();
                sw2.Start();
                result2 = CongruenceChecker.IsPrime(i);
                sw2.Stop();


                if (result1 != result2)
                {

                    var message = $"Conqruence checker failed for {i} - {result2} expected: {result1}";
                    Console.WriteLine(message);
                    Assert.AreEqual(result1, result2, message);
                }
            }
            var time1 = $"{sw1.Elapsed} - {nameof(Primes)}.{nameof(Primes.IsPrime)}";
            var time2 = $"{sw2.Elapsed} - {nameof(CongruenceChecker)}.{nameof(Primes.IsPrime)}";
            Console.WriteLine(time1);
            Console.WriteLine(time2);
        }
    }
}