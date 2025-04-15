using Microsoft.Diagnostics.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace HigginsSoft.Math.Lib.Tests.FactorBaseTests
{
    [TestClass()]
    public class FactorBaseTests
    {


        public FactorBaseTests()
        {
            // Initialize the test class

        }

        [TestMethod()]
        public void FactorBaseTest()
        {

            BigInteger P;
            BigInteger Q;
            BigInteger N;
            BigInteger NRoot;


            P = 64007;
            Q = 64013;
            N = P * Q;
            NRoot = (int)MathLib.Sqrt(N);
            // Generate two random prime numbers

            // Arrange
            var factorBasePrimes = FactorBase.GetFactorBasePrimes(N, 20);
            var max = factorBasePrimes.Max();

            //verify the factor base primes.
            var foundAllPrimes = factorBasePrimes.All(p => Enumerable.Range(1, max).Any(i => ((NRoot + i) % N) % p == 0));
            Assert.IsTrue(foundAllPrimes, "Not all primes are found in the factor base.");

        }

        [TestMethod()]
        public void FactorBaseRs1024Test()
        {


            // Generate two random prime numbers

            // Arrange



            BigInteger N;
            BigInteger NRoot;


            N = RsaChallenge.Rsa1024BigInt;
            NRoot = MathLib.Sqrt(N);

            var rsa = RsaChallenge.Rsa1024BigInt;
            var factorBasePrimes = FactorBase.GetFactorBasePrimes(rsa, 20);
            var max = factorBasePrimes.Max();

            var expectedMax = 227;
            Assert.AreEqual(expectedMax, max, "The max prime is not as expected.");
            var expectedCount = 20;
            Assert.AreEqual(expectedCount, factorBasePrimes.Count, "The count of primes is not as expected.");
            var expectedMin = 2;
            Assert.AreEqual(expectedMin, factorBasePrimes.Min(), "The min prime is not as expected.");
            //verify the factor base primes.
            var foundAllPrimes = factorBasePrimes.All(p => Enumerable.Range(1, max).Any(i => ((NRoot + i) % N) % p == 0));
            Assert.IsTrue(foundAllPrimes, "Not all primes are found in the factor base.");

        }


        [TestMethod()]
        public void FactorBaseRs1024ShanksTest()
        {
            BigInteger N;
            BigInteger NRoot;


            N = RsaChallenge.Rsa1024BigInt;
            NRoot = MathLib.Sqrt(N);

            // Generate two random prime numbers

            // Arrange
            var rsa = RsaChallenge.Rsa1024BigInt;

            var factorBasePrimes = FactorBase.GetFactorBasePrimes(rsa, 20);
            var max = factorBasePrimes.Max();


            var expectedMax = 227;
            Assert.AreEqual(expectedMax, max, "The max prime is not as expected.");
            var expectedCount = 20;
            Assert.AreEqual(expectedCount, factorBasePrimes.Count, "The count of primes is not as expected.");
            var expectedMin = 2;
            Assert.AreEqual(expectedMin, factorBasePrimes.Min(), "The min prime is not as expected.");
            //verify the factor base primes.
            var foundAllPrimes = factorBasePrimes.All(p => Enumerable.Range(1, max).Any(i => ((NRoot + i) % N) % p == 0));
            Assert.IsTrue(foundAllPrimes, "Not all primes are found in the factor base.");

            var offsets = factorBasePrimes.ToDictionary(p => p,
                p =>
                {
                    var l = new List<int>();
                    for (var i = 1; l.Count < 2 && i <= p; i++)
                    {
                        var a = NRoot + i;
                        var s = a * a;
                        var r = s % N;
                        var m = r % p;
                        if (m == 0)
                        {
                            l.Add(i);
                        }
                    }
                    return l;
                });


            var expectedOffsets = new List<(int p, int a, int b)>
            {
                (2, 2, 4), // special case for 2
                (3, 2, 3),
                (31, 15, 18),
                (43, 12, 41),
                (47, 28, 40),
                (61, 23, 30),
                (67, 23, 31),
                (71, 58, 71),
                (79, 34, 52),
                (97, 40, 67),
                (101, 35, 83),
                (113, 41, 42),
                (127, 97, 120),
                (137, 114, 116),
                (173, 49, 105),
                (191, 119, 122),
                (193, 62, 141),
                (199, 85, 185),
                (211, 24, 39),
                (227, 7, 88),
            };

            Assert.AreEqual(expectedOffsets.Count, offsets.Count, "The number of offsets is not as expected.");
            foreach (var (p, a, b) in expectedOffsets)
            {
                Assert.IsTrue(offsets.ContainsKey(p), $"The prime {p} is not in the offsets.");
                var offset = offsets[p];
                Assert.AreEqual(a, offset[0], $"The offset for {p} is not as expected.");
                if (p > 2) // assert two offsets for p >2
                {
                    Assert.AreEqual(b, offset[1], $"The offset for {p} is not as expected.");
                }
            }
        }

        [TestMethod]
        public void SquareModNBruteGenerator_SeedTest()
        {
            BigInteger N;
            BigInteger NRoot;


            N = RsaChallenge.Rsa1024BigInt;
            NRoot = MathLib.Sqrt(N);

            var start = BigInteger.ModPow(NRoot + 1, 2, N);
            var next = BigInteger.ModPow(NRoot + 2, 2, N);

            var diff1 = next - start;

            var calcLast = next + diff1 + 2;
            var last = BigInteger.ModPow(NRoot + 3, 2, N);
            var delta = last - calcLast;
            Assert.AreEqual(0, delta, "The difference is not as expected.");
        }



        [TestMethod]
        public void SquareModNBruteGenerator_EnumerateTest()
        {
            BigInteger N;
            BigInteger NRoot;


            N = RsaChallenge.Rsa1024BigInt;
            NRoot = MathLib.Sqrt(N);

            int i = 1;
            var gen = SquareModNGenerator.Generate(N, NRoot);
            foreach (var r in gen)
            {
                var s = BigInteger.ModPow(NRoot + i, 2, N);
                var diff = r - s;
                Assert.AreEqual(0, diff, "The difference is not as expected.");
                i++;
                if (i < 20)
                    break;
            }

        }

        [TestMethod]
        public void SquareModNBruteGenerator_EnumerateTimeTests()
        {
            BigInteger N;
            BigInteger NRoot;


            N = RsaChallenge.Rsa1024BigInt;
            NRoot = MathLib.Sqrt(N);

            var maxCount = 1_000_000;
            var startCount = 1_000;

            // time generation for each power of 10 from startCount to maxCount

            var current = startCount;
            while (current <= maxCount)
            {
                var sw = Stopwatch.StartNew();
                var gen = SquareModNGenerator.Generate(N, NRoot).Take(current);
                foreach (var r in gen)
                {
                    // do nothing
                }
                sw.Stop();
                var message = $"Generated {current.ToString("N0")} mod roots: {sw.ToString()}";
                Console.WriteLine(message);
                Debug.WriteLine(message);
                current *= 10;
            }



        }





   

        [TestMethod]
        public void SquareModNBruteGenerator_EnumerateTimeEstimatesTests()
        {
            BigInteger N = RsaChallenge.Rsa1024BigInt;
            BigInteger NRoot = MathLib.Sqrt(N);

            var actualCounts = new List<double>();
            var actualTimes = new List<double>();

            var maxCount = 1_000_000;
            var startCount = 1_000;

            var current = startCount;
            while (current <= maxCount)
            {
                var sw = Stopwatch.StartNew();
                var gen = SquareModNGenerator.Generate(N, NRoot).Take(current);
                foreach (var r in gen)
                {
                    // intentionally do nothing
                }
                sw.Stop();
                double elapsedMs = sw.Elapsed.TotalMilliseconds;

                actualCounts.Add(MathLib.Log10(current));
                actualTimes.Add(MathLib.Log10(elapsedMs));

                var message = $"Generated {current:N0} mod roots in {elapsedMs:N2} ms";
                Console.WriteLine(message);
                Debug.WriteLine(message);

                current *= 10;
            }

            // Fit log-log linear regression: log(time) = a + b * log(count)
            double avgX = actualCounts.Average();
            double avgY = actualTimes.Average();
            double numerator = actualCounts.Zip(actualTimes, (x, y) => (x - avgX) * (y - avgY)).Sum();
            double denominator = actualCounts.Sum(x => MathLib.Pow(x - avgX, 2));
            double slope = numerator / denominator;
            double intercept = avgY - slope * avgX;

            Console.WriteLine($"\nEstimated timing model: log10(time_ms) = {intercept:F4} + {slope:F4} * log10(count)");

            Console.WriteLine("\nEstimated times:");
            foreach (var estCount in new[] { 1e7, 1e8, 1e9, 1e10, 1e11, 1e12 })
            {
                var logCount = MathLib.Log10(estCount);
                var logTime = intercept + slope * logCount;
                var estimatedTime = MathLib.Pow(10, logTime); // in milliseconds
                Console.WriteLine($"Estimated time for {estCount:E0} ({estCount.ToString("N0")}) mod roots: {estimatedTime / 1000:F2} sec ({estimatedTime / (60 * 1000):F2} m)");
            }
        }



        [TestMethod()]
        public void FactorBaseRs1024ShanksFactorBaseTest()
        {
            BigInteger N;
            BigInteger NRoot;


            N = RsaChallenge.Rsa1024BigInt;
            NRoot = MathLib.Sqrt(N);

            // Generate two random prime numbers

            // Arrange
            var rsa = RsaChallenge.Rsa1024BigInt;

            var factorBasePrimes = FactorBase.GetFactorBasePrimes(rsa, 20);
            var max = factorBasePrimes.Max();
            var offsets = FactorBase.GetFactorOffsets(N, NRoot, factorBasePrimes);

            var expectedMax = 227;
            Assert.AreEqual(expectedMax, max, "The max prime is not as expected.");
            var expectedCount = 20;
            Assert.AreEqual(expectedCount, factorBasePrimes.Count, "The count of primes is not as expected.");
            var expectedMin = 2;
            Assert.AreEqual(expectedMin, factorBasePrimes.Min(), "The min prime is not as expected.");
            //verify the factor base primes.
            var foundAllPrimes = factorBasePrimes.All(p => Enumerable.Range(1, max).Any(i => ((NRoot + i) % N) % p == 0));
            Assert.IsTrue(foundAllPrimes, "Not all primes are found in the factor base.");


            var expectedOffsets = new List<(int p, int a, int b)>
            {
                (2, 2, 4), // special case for 2
                (3, 2, 3),
                (31, 15, 18),
                (43, 12, 41),
                (47, 28, 40),
                (61, 23, 30),
                (67, 23, 31),
                (71, 58, 71),
                (79, 34, 52),
                (97, 40, 67),
                (101, 35, 83),
                (113, 41, 42),
                (127, 97, 120),
                (137, 114, 116),
                (173, 49, 105),
                (191, 119, 122),
                (193, 62, 141),
                (199, 85, 185),
                (211, 24, 39),
                (227, 7, 88),
            };

            Assert.AreEqual(expectedOffsets.Count, offsets.Count, "The number of offsets is not as expected.");
            foreach (var (p, a, b) in expectedOffsets)
            {
                Assert.IsTrue(offsets.ContainsKey(p), $"The prime {p} is not in the offsets.");
                var offset = offsets[p];
                Assert.AreEqual(a, offset[0], $"The offset for {p} is not as expected.");
                if (p > 2) // assert two offsets for p >2
                {
                    Assert.AreEqual(b, offset[1], $"The offset for {p} is not as expected.");
                }
            }
        }



    }

    public class FactorBase
    {
        public static List<int> GetFactorBasePrimes(BigInteger n, int count)
        {
            List<int> factorBase = new List<int>();
            // Generate primes up to the provided bound (naively here)
            var primes = Primes.IntFactorPrimes;
            foreach (var p in primes)
            {
                if (p > n)
                    break;

                // Check if n is a quadratic residue modulo p using the Legendre symbol.
                if (MathLib.LegendreSymbol(n, p) == 1)
                {
                    factorBase.Add(p);
                    if (factorBase.Count >= count)
                    {
                        break;
                    }
                }
            }

            return factorBase;
        }

        internal static Dictionary<int, List<int>> GetFactorOffsets(BigInteger n, BigInteger nRoot, List<int> factorBasePrimes)
        {
            var max = factorBasePrimes.Max();
            // pre-calculate the residues so they can be reused.
            var residues = Enumerable.Range(1, max).Select(i => BigInteger.ModPow(nRoot + i, 2, n)).ToList();

            var offsets = factorBasePrimes.ToDictionary(p => p,
                p =>
                {
                    var l = new List<int>();
                    for (var i = 1; l.Count < 2 && i <= p; i++)
                    {
                        var r = residues[i - 1];
                        var m = r % p;
                        if (m == 0)
                        {
                            l.Add(i);
                        }
                    }
                    return l;
                });

            return offsets;


        }
    }

    public class SquareModNGenerator
    {
        public static IEnumerable<BigInteger> Generate(BigInteger n, BigInteger nRoot)
        {

            var current = BigInteger.ModPow(nRoot + 1, 2, n);
            yield return current;

            var diff = (2 * (nRoot + 1) + 1) % n;
            while (true)
            {
                current = (current + diff) % n;
                yield return current;
                diff += 2;
                if (diff > n)
                    diff = diff % n;
            }
        }

        public static IEnumerable<GmpInt> Generate(GmpInt n, GmpInt nRoot)
        {

            var current = nRoot.PowerMod(2, n);
            yield return current;

            var diff = (2 * (nRoot + 1) + 1) % n;
            while (true)
            {
                current = (current + diff) % n;
                yield return current;
                diff += 2;
                if (diff > n)
                    diff = diff % n;
            }
        }
    }

}