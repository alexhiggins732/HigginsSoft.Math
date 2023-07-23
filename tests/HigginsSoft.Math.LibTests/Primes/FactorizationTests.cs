#define CLOUD_BUILD
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HigginsSoft.Math.Lib.Tests.FactorizationTests
{
    [TestClass()]
    public class FactorizationTest
    {
        [TestMethod()]
        public void FactorTrialDivideTest()
        {
            //Assert.Fail();
            //var primeCount = PrimeData.Counts[16].Count;
            //var primes = Primes.IntFactorPrimes.Take(primeCount).ToList();
            //int p;

            var start = 5000 >> 2;
            var stop = 5000;
            TimeSpan last = TimeSpan.FromSeconds(1);
            for (var limit = start; limit <= stop; limit <<= 1)
            {
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < limit; i++)
                {
                    var f = Factorization.FactorTrialDivide(i);

                    Assert.AreEqual(i, f.GetProduct());
                }
                sw.Stop();
                var mult = sw.ElapsedMilliseconds / last.TotalMilliseconds;
                if (mult < 1) mult = 1;
                last = sw.Elapsed;
                Console.WriteLine($"// Factor 0 <= i < {limit} took {sw.Elapsed} ({(mult-1).ToString("p2")})");
            }
            // Factor 0 <= i < 16384 took 00:00:00.2513217 (-74.90%)
            // Factor 0 <= i < 32768 took 00:00:00.3955474 (57.17%)
            // Factor 0 <= i < 65536 took 00:00:00.8192825 (107.05%)
            // Factor 0 <= i < 131072 took 00:00:01.5485718 (88.95%)
            // Factor 0 <= i < 262144 took 00:00:03.1350727 (102.44%)
            // Factor 0 <= i < 524288 took 00:00:06.0197520 (91.99%)
            // Factor 0 <= i < 1048576 took 00:00:14.1519721 (135.08%)


        }

        [TestMethod()]
        public void AddTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void AddTest1()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void FactorizationStringTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void GetProductTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void IsPerfectSquareTest()
        {
            //Assert.Fail();
        }
    }
}