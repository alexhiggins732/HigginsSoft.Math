#define TIME
#define CLOUD_BUILD
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using HigginsSoft.Math.Lib;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HigginsSoft.Math.Demos.Tests
{
    [TestClass()]
    public class Qs16Tests
    {

//  limit unit test run time for cloud builds
#if CLOUD_BUILD
        const int QS_TEST_START = 100;
        const int QS_TEST_END = 200;
#else
        const int QS_TEST_START = 65536;
        const int QS_TEST_END = 65536 << 1;
#endif

        [TestMethod()]
        public void NaiveTest()
        {

            var s = new Qs16();
            var sw = Stopwatch.StartNew();
            for (var i = QS_TEST_START; i < QS_TEST_END; i++)
            {
                if (i == 16)
                {
                    string bp = "";
                }
                //Console.WriteLine($"Testing {i}");
                var result = s.Naive((short)i);
                var product = (int)result.GetProduct();
                if (i != product)
                {
                    var message = $"Factorization failed for {i} -> Actual: {result.FactorizationString()}";
                    Assert.AreEqual(i, product, message);
                }

            }
            sw.Stop();
            Console.WriteLine($"QS naive took {sw.Elapsed}");
        }

        [TestMethod()]
        public void NaiveWithFactorbaseTimeTest()
        {
            var start = QS_TEST_START;
            var stop = QS_TEST_END;
            var s = new Qs16();
            var pow = Qs16.t_div_pow_limit;
            int primecount = Qs16.factor_base_prime_count;
            var morerels = Qs16.min_extra_residues;

            TimeSpan last = TimeSpan.FromSeconds(1);
            for (var limit = start; limit <= stop; limit <<= 1)
            {
                var sw = Stopwatch.StartNew();
                for (var i = 2; i < limit; i++)
                {
                    var f = s.NaiveWithFactorBase(i);
#if TIME

                    Assert.AreEqual(i, f.GetProduct());
#else
#endif
                }
                sw.Stop();
                var mult = sw.ElapsedMilliseconds / last.TotalMilliseconds;
                if (mult < 1) mult = 1;
                last = sw.Elapsed;

#if CLOUD_BUILD
#else
                Console.WriteLine($"// Factor 0 <= i < {limit} took {sw.Elapsed} ({mult.ToString("p2")}) 2^{pow} res limit ({primecount} primes) (add. rels {morerels})");
#endif
            }


            //5.5 minutes
            // Factor 0 <= i < 4096 took 00:00:00.0871909 (100.00%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 8192 took 00:00:00.1550162 (177.77%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 16384 took 00:00:00.4196708 (270.29%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 32768 took 00:00:00.9011566 (214.69%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 65536 took 00:00:02.3245938 (257.89%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 131072 took 00:00:06.1758143 (265.64%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 262144 took 00:00:18.5358309 (300.12%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 524288 took 00:01:01.7694711 (333.24%) 2^16 res limit (16 primes) (add. rels 4)
            // Factor 0 <= i < 1048576 took 00:04:00.1191800 (388.73%) 2^16 res limit (16 primes) (add. rels 4)

            //3.6 minutes
            // Factor 0 <= i < 4096 took 00:00:00.1225094 (100.00%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 8192 took 00:00:00.1948529 (158.36%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 16384 took 00:00:00.3716667 (190.40%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 32768 took 00:00:00.9928336 (266.91%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 65536 took 00:00:02.4945621 (251.20%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 131072 took 00:00:05.4876894 (219.96%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 262144 took 00:00:14.4219825 (262.79%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 524288 took 00:00:44.2649967 (306.92%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 1048576 took 00:02:27.7697715 (333.83%) 2^16 res limit (12 primes) (add. rels 4)

            // Factor 0 <= i < 4096 took 00:00:00.1184369 (100.00%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 8192 took 00:00:00.1604978 (135.09%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 16384 took 00:00:00.3383549 (210.59%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 32768 took 00:00:00.6751533 (199.49%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 65536 took 00:00:01.6964957 (251.20%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 131072 took 00:00:04.3046983 (253.70%) 2^16 res limit (12 primes) (add. rels 4)

            // Factor 0 <= i < 4096 took 00:00:00.1140688 (100.00%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 8192 took 00:00:00.1588899 (138.51%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 16384 took 00:00:00.3461921 (217.76%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 32768 took 00:00:00.6606119 (190.65%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 65536 took 00:00:01.6922854 (256.13%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 131072 took 00:00:04.2131332 (248.95%) 2^16 res limit (12 primes) (add. rels 4)
            // Factor 0 <= i < 262144 took 00:00:11.2759884 (267.62%) 2^16 res limit (12 primes) (add. rels 4)

            // Factor 0 <= i < 65536 took 00:00:01.9035087 (190.30%) 2^16 res limit (8 primes) (add. rels 4)
            // Factor 0 <= i < 131072 took 00:00:03.9838302 (209.25%) 2^16 res limit (8 primes) (add. rels 4)
            // Factor 0 <= i < 262144 took 00:00:10.0510421 (252.29%) 2^16 res limit (8 primes) (add. rels 4)
            // Factor 0 <= i < 524288 took 00:00:26.5883782 (264.53%) 2^16 res limit (8 primes) (add. rels 4)
            // Factor 0 <= i < 1048576 took 00:01:11.9915453 (270.76%) 2^16 res limit (8 primes) (add. rels 4)

            //popcomp
            // Factor 0 <= i < 65536 took 00:00:01.8198859 (181.90%) 2^16 res limit (8 primes) (add. rels 4)
            // Factor 0 <= i < 65536 took 00:00:01.6585719 (165.80%) 2^16 res limit (8 primes) (add. rels 2)

            //popcomp, lists vs dictionary for factorizations, also static with clear.
            // can use array for factorizations with index, but primes require lookup.
            // also, instead of factorization class can use  arrays {primes[], power[], powergf2[]}
            // Factor 0 <= i < 16384 took 00:00:00.4138607 (100.00%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 32768 took 00:00:00.6590790 (159.23%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 65536 took 00:00:01.5812041 (239.88%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 131072 took 00:00:04.0690015 (257.34%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 262144 took 00:00:09.9862916 (245.42%) 2^16 res limit (8 primes) (add. rels 2)

            // Factor 0 <= i < 16384 took 00:00:00.3917768 (100.00%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 32768 took 00:00:00.6828019 (174.08%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 65536 took 00:00:01.5894094 (232.72%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 131072 took 00:00:04.0129854 (252.42%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 262144 took 00:00:10.6064675 (264.29%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 524288 took 00:00:27.8538409 (262.60%) 2^16 res limit (8 primes) (add. rels 2)
            // Factor 0 <= i < 1048576 took 00:01:18.1457961 (280.55%) 2^16 res limit (8 primes) (add. rels 2)

        }

        [TestMethod()]
        public void NaiveWithFactorbaseTest()
        {
            var primes = Primes.IntFactorPrimes.Take(32).ToList();


            var s = new Qs16();
#if CLOUD_BUILD
#else
            var t = s.NaiveWithFactorBase(441);
#endif
            var sw = Stopwatch.StartNew();



            int primeFailCount = 0;
            int minfail = 0;
            const int limit = QS_TEST_END << 0;
            for (var i = 2; i < limit; i++)
            {
                if (i == 671)
                {
                    string bp = "";
                }

                //Console.WriteLine($"Testing {i}");
                FactorizationInt result;
                try
                {
                    result = s.NaiveWithFactorBase(i);
                }
                catch (Exception ex)
                {
                    Console.Write($"Unhandled exception factoring {i}: {ex}");
                    continue;
                }
                var product = (int)result.GetProduct();
                if (i != product)
                {
                    var message = $"Factorization failed for {i} -> Actual: {result.FactorizationString()}";
                    Assert.AreEqual(i, product, message);
                }
                //if (result.Factors.Count == 1 && result.Factors.First().Power == 1)
                //{
                //    var isPrime = MathLib.IsPrime(product);
                //    if (!isPrime)
                //    {
                //        if (minfail == 0) minfail = i;
                //        primeFailCount++;
                //    }

                //    //Assert.IsTrue(isPrime, $"Value is not prime: {product}");
                //}

            }
            sw.Stop();
            var pow = Qs16.t_div_pow_limit;
            int primecount = Qs16.factor_base_prime_count;
            var morerels = Qs16.min_extra_residues;
            Console.WriteLine($"//  N <= {limit} 2^{pow} res limit ({primecount} primes) (add. rels {morerels}): {sw.Elapsed} with {s.opCount} ops - {primeFailCount} unfactored starting at {minfail}");

            // sieve window sqrt(n) - (sqrt(n) + root) << 1) + 32 primes + result prime check
            //  2^7 res limit: 00:00:02.2185446 with 175839 ops - 35430 starting at 597
            //  2^8 res limit: 03.6094225 with 200208 ops - 29501 unfactored starting at 1101
            //  512 res limit: 06.0483105 with 227512 ops - 22096 unfactored starting at 1527
            // 1024 res limit: 09.7942616 with 261194 ops - 12264 unfactored starting at 4589
            // 2048 res limit: 16.8328047 with 289138 ops - 3475 unfactored starting at 8417
            // 2^12 res limit: 24.9042940 with 296815 ops - 630 unfactored starting at 13645
            // 2^13 res limit: 34.2461100 with 298535 ops - 76 unfactored starting at 28115
            // 2^14 res limit: 44.3460186 with 301084 ops - 0 unfactored starting at 0

            // sieve window sqrt(n) + 2*sqrt(n) + 32 primes + result prime check
            //   2^8 res limit: 03.6094225 with 200208 ops - 29501 unfactored starting at 1101
            //  2^10 res limit: 03.2031409 with 219052 ops - 24159 unfactored starting at 2031
            //  2^12 res limit: 11.1256062 with 289776 ops - 3024 unfactored starting at 7405
            //  2^14 res limit: 20.8531353 with 300982 ops - 50 unfactored starting at 34411
            //  2^15 res limit: 25.7658783 with 301967 ops - 2 unfactored starting at 51103
            //  2^16 res limit: 27.6861852 with 302036 ops - 0 unfactored starting at 0

            // sieve window sqrt(n) + 2*sqrt(n) + 16 primes + result prime check
            //  2^10 res limit (16 primes): 00:00:02.3916643 with 216312 ops - 24931 unfactored starting at 203
            //  2^15 res limit: 00:00:11.9198667 with 301262 ops - 18 unfactored starting at 19771
            //  2^16 res limit: 00:00:12.9788927 with 301595 ops - 2 unfactored starting at 19771


            //  2^12 res limit (10 primes): 00:00:03.7922382 with 272579 ops - 7908 unfactored starting at 4589

            //  2^16 res limit (4 primes): 00:00:02.4835731 with 255653 ops - 11267 unfactored starting at 489
            //  2^16 res limit (6 primes): 00:00:03.7072648 with 289014 ops - 1961 unfactored starting at 951
            //  2^16 res limit (8 primes): 00:00:04.9636126 with 298061 ops - 302 unfactored starting at 951
            //  2^16 res limit (9 primes): 00:00:05.8808115 with 299822 ops - 147 unfactored starting at 3063
            //  2^16 res limit (10 primes): 00:00:06.6728965 with 300255 ops - 68 unfactored starting at 4589
            //  2^16 res limit (12 primes): 00:00:08.8422940 with 301412 ops - 17 unfactored starting at 4589
            //  2^16 res limit (16 primes): 00:00:12.8922946 with 301595 ops - 2 unfactored starting at 19771

            // --- with pre_check tdiv to max(factor_primes)^2
            //  2^16 res limit (4 primes): 00:00:02.6237672 with 241029 ops - 11267 unfactored starting at 489
            //  2^16 res limit (16 primes): 00:00:08.9598699 with 197278 ops - 2 unfactored starting at 19771
            //  2^16 res limit (18 primes): 00:00:09.7541925 with 192499 ops - 0 unfactored starting at 0
            //  2^16 res limit (32 primes): 00:00:13.3130120 with 155623 ops - 0 unfactored starting at 0

            //  2^10 res limit (32 primes): 00:00:00.4033586 with 108265 ops - 22627 unfactored starting at 17162
            //  2^12 res limit (32 primes): 00:00:01.1724801 with 151747 ops - 2998 unfactored starting at 17899
            //  2^14 res limit (32 primes): 00:00:06.0906657 with 156889 ops - 50 unfactored starting at 34411
            //  2^15 res limit (32 primes): 00:00:10.6678878 with 155665 ops - 2 unfactored starting at 51103
            //  2^16 res limit (32 primes): 00:00:13.2322209 with 155623 ops - 0 unfactored starting at 0

            // -- inline matrix solver:
            //  N <= 65536 2^16 res limit (10 primes): 00:00:02.6264054 with 208277 ops - 0 unfactored starting at 0

            //N <= 65536 2 ^ 16 res limit(10 primes) (add.rels 0): 00:00:02.6278359 with 204580 ops - 0 unfactored starting at 0


            // no sieve range limit - 
            //  N <= 65536 2^16 res limit (4 primes) (add. rels 0): 00:00:03.1065593 with 272240 ops - 0 unfactored starting at 0
            // no sieve range limit - no prime check in test
            //  N <= 65536 2^16 res limit (4 primes) (add. rels 0): 00:00:02.8398675 with 272240 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (5 primes) (add. rels 0): 00:00:02.4901779 with 250529 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (6 primes) (add. rels 0): 00:00:02.4654722 with 239818 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (8 primes) (add. rels 0): 00:00:02.2783670 with 215576 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (10 primes) (add. rels 0): 00:00:02.1824521 with 204583 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (11 primes) (add. rels 0): 00:00:02.3654673 with 203376 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (12 primes) (add. rels 0): 00:00:02.4828545 with 200036 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (14 primes) (add. rels 0): 00:00:02.5882537 with 196910 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (16 primes) (add. rels 0): 00:00:02.6199688 with 192154 ops - 0 unfactored starting at 0
            //  N <= 65536 2^16 res limit (32 primes) (add. rels 0): 00:00:03.5060761 with 155825 ops - 0 unfactored starting at 0





            //TODO: time with only composites and no tdiv prime check in matrix solver.
            // --theoritical optimization is use only primes that are residues
            // -- in practices this might be more overhead than it is worth.
        }

        [TestMethod()]
        public void QsConsiceTest()
        {
            var s = new QsConcise();
            var gen = Composites.GenerateTo(QS_TEST_END);
            foreach (var expected in gen)
            {
                var fact = s.FactorUnchecked(expected);
                var actual = fact.GetProduct();
                Assert.AreEqual(expected, actual);

            }
        }
    }
}