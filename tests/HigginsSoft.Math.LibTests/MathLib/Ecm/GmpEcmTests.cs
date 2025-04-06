#define HAVE_ECM_AVX
#undef HAVE_ECM_AVX
#define SKIP_LONG_TESTS
//#undef SKIP_LONG_TESTS
using HigginsSoft.Math.Lib;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests
{
    [TestClass()]
    public class GmpEcmTests
    {


        int RunTest(IEnumerable<int> intTests, Func<GmpInt, Factorization> factorize, bool debugFactors = true)
        {
            int failCount = 0;
            foreach (var n in intTests)
            {
                var sw = Stopwatch.StartNew();
                var factorization = factorize(n);
                sw.Stop();

                if (factorization.Factors.Count == 0 || (factorization.Factors.Count == 1 && factorization.Factors.First().Power == 1))
                {
                    failCount++;
                    if (debugFactors) Debug.WriteLine($"Failed to factor {n} in {sw.Elapsed}");
                }
                else
                {
                    Assert.IsTrue(factorization.Factors.Count > 0);
                    var value = factorization.GetProduct();
                    Assert.AreEqual(n, value);
                    if (debugFactors)
                    {
                        Console.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                        System.Diagnostics.Debug.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                    }

                }
            }
            return failCount;
        }

        int RunGmpTest(IEnumerable<GmpInt> intTests, Func<GmpInt, Factorization> factorize, bool debugFactors = true)
        {
            int failCount = 0;
            foreach (var n in intTests)
            {
                var sw = Stopwatch.StartNew();
                var factorization = factorize(n);
                sw.Stop();

                if (factorization.Factors.Count == 0 || (factorization.Factors.Count == 1 && factorization.Factors.First().Power == 1))
                {
                    failCount++;
                    if (debugFactors)
                    {
                        Console.WriteLine($"Failed to factor {n} in {sw.Elapsed}");
                        Debug.WriteLine($"Failed to factor {n} in {sw.Elapsed}");
                    }
                }
                else
                {
                    Assert.IsTrue(factorization.Factors.Count > 0);
                    var value = factorization.GetProduct();
                    Assert.AreEqual(n, value);
                    if (debugFactors)
                    {
                        Console.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                        System.Diagnostics.Debug.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                    }

                }
            }
            return failCount;
        }

        int RunTestNumerics(IEnumerable<int> intTests, Func<BigInteger, Factorization> factorize)
        {
            int failCount = 0;
            foreach (var n in intTests)
            {
                var sw = Stopwatch.StartNew();
                var factorization = factorize(n);
                sw.Stop();

                if (factorization.Factors.Count == 0 || (factorization.Factors.Count == 1 && factorization.Factors.First().Power == 1))
                {
                    failCount++;
                    Debug.WriteLine($"Failed to factor {n} in {sw.Elapsed}");
                }
                else
                {
                    Assert.IsTrue(factorization.Factors.Count > 0);
                    var value = factorization.GetProduct();
                    Assert.AreEqual(n, value);
                    Console.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                    System.Diagnostics.Debug.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                }
            }
            return failCount;
        }

        [TestMethod()]
        public void FactorEcmTest_ECM_2P29_Rng_Plus_10000()
        {

            var comp = Composites.GenerateTo(1 << 29, 1 << 30).Take(10000).ToArray();
            var ecm = new GmpEcm();
            var failCount = RunTest(comp, n => ecm.ECM(n));
            Console.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);
        }



        [TestMethod()]
        public void FactorEcmTest_PP1_2P29_Rng_Plus_10000()
        {

            var comp = Composites.GenerateTo(1 << 29, 1 << 30).Take(10000).ToArray();
            var ecm = new GmpEcm();
            var failCount = RunTest(comp, n => ecm.PP1(n));
            Console.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);
        }

        [TestMethod()]
        public void FactorEcmTest_PM1_2P29_Rng_Plus_10000()
        {

            var comp = Composites.GenerateTo(1 << 29, 1 << 30).Take(10000).ToArray();
            var ecm = new GmpEcm();
            var failCount = RunTest(comp, n => ecm.PM1(n));
            Console.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);
        }



        [TestMethod()]
        public void FactorEcm_ECM_Test_To_2P10()
        {
            var bits = 10;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var ecm = new GmpEcm();
            var failCount = RunTest(comp, n => ecm.ECM(n));
            Console.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);

        }


        [TestMethod()]
        public void FactorEcm_PP1_Test_To_2P8()
        {
            var bits = 8;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var ecm = new GmpEcm();
            var failCount = RunTest(comp, n => ecm.PP1(n));
            Console.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);

        }

        [TestMethod()]
        public void FactorEcm_PM1_Test_To_2P8()
        {
            var bits = 8;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var ecm = new GmpEcm();
            var failCount = RunTest(comp, n => ecm.PM1(n));
            Debug.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);

        }


        [TestMethod()]
        public void FactorEcmTestCompositesTo216()
        {
            var bits = 16;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var ecm = new GmpEcm();
            int failCount = 0;
            foreach (var n in comp)
            {
                var sw = Stopwatch.StartNew();
                var factorization = ecm.ECM(n);
                sw.Stop();
                if (factorization.Factors.Count == 0 || (factorization.Factors.Count == 1 && factorization.Factors.First().Power == 1))
                {
                    failCount++;
                    Debug.WriteLine($"Failed to factor {n} in {sw.Elapsed}");
                }
                else
                {
                    Assert.IsTrue(factorization.Factors.Count > 0);
                    var value = factorization.GetProduct();
                    Assert.AreEqual(n, value);
                    Console.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                    System.Diagnostics.Debug.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                }

                //var product = factors.Aggregate((a, b) => a * b);
                //Assert.AreEqual(n, product);
            }

            Console.WriteLine("Small composite factorization failure rate: {0}%", 100.0 * failCount / comp.Length);
        }


        [TestMethod()]
        public void FactorEcmTestCompositesTo229RngPlus10000()
        {

            var comp = Composites.GenerateTo(1 << 29, 1 << 30).Take(10000).ToArray();

            var ecm = new GmpEcm();
            int failCount = 0;
            foreach (var n in comp)
            {
                var sw = Stopwatch.StartNew();
                var factorization = ecm.ECM(n);
                sw.Stop();
                if (factorization.Factors.Count == 0 || (factorization.Factors.Count == 1 && factorization.Factors.First().Power == 1))
                {
                    failCount++;
                    Debug.WriteLine($"Failed to factor {n} in {sw.Elapsed}");
                }
                else
                {
                    Assert.IsTrue(factorization.Factors.Count > 0);
                    var value = factorization.GetProduct();
                    Assert.AreEqual(n, value);
                    Console.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                    System.Diagnostics.Debug.WriteLine($"Factored {factorization} in {sw.Elapsed}");
                }

                //var product = factors.Aggregate((a, b) => a * b);
                //Assert.AreEqual(n, product);
            }

            Console.WriteLine("Small composite factorization failure rate: {0}%", 100.0 * failCount / comp.Length);
        }


        [TestMethod()]
        public void BinaryPm1_N299()
        {
            var n = 299;
            var generator = new MersenneNumberGenerator();
            var residues = new List<BigInteger>();
            Dictionary<int, BigInteger> factors = new();
            int i = -1;
            foreach (var m in generator.GenerateMersenneNumbers(300))
            {
                i++;
                var res = m % n;
                if (residues.Contains(res))
                {
                    Debug.WriteLine($"Found duplicate residue {res} for {m}");
                    //break;
                    residues.Add(res);
                }
                else
                {
                    residues.Add(res);
                }
                var gcd = MathUtil.Gcd(m, n);
                if (gcd == 1)
                {
                    continue;
                }
                else if (gcd == n)
                {
                    Debug.WriteLine($"Gcd({m}, {n}) = {gcd}");
                    continue;
                }
                else
                {
                    var factor = n / gcd;
                    factors.Add(i, factor);
                    Debug.WriteLine($"Gcd({m}, {n}) = {gcd} factor = {factor}");

                    var gcdRes = MathUtil.Gcd(n, res);
                    var factorRes = n / gcdRes;
                    Debug.WriteLine($"Gcd({res}, {n}) = {gcdRes} factor = {factorRes}");
                    //break;
                }

            }
        }


        [TestMethod()]
        public void BinaryPm1_2P8()
        {
            var bits = 8;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var mpm1 = new MersennePm1Factor();


            var failCount = RunTest(comp, n => MersennePm1Factor.MPm1Factor(n));

            Debug.WriteLine("Factorization failure rate: {0}%", 100.0 * failCount / comp.Length);

        }


        [TestMethod()]
        public void BinaryPm1_2P12_List_vs_Cycle()
        {
            var bits = 12;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var mpm1 = new MersennePm1Factor();

            int tests = 1000;
            var sw = Stopwatch.StartNew();
            var failCount = RunTest(comp, n => MersennePm1Factor.MPm1Factor(n, tests), false);
            sw.Stop();
            Debug.WriteLine("Factorization failure rate: {0}% in {1}", 100.0 * failCount / comp.Length, sw.Elapsed);

            sw = Stopwatch.StartNew();
            var failListCount = RunTest(comp, n => MersennePm1Factor.MPm1FactorCycle(n, tests), false);
            sw.Stop();
            Debug.WriteLine("Factorization failure rate: {0}% in {1}", 100.0 * failListCount / comp.Length, sw.Elapsed);
        }

        [TestMethod()]
        public void BinaryPm1_2P16Cycle()
        {
            var bits = 16;
            var comp = Composites.GenerateTo(1 << bits).ToArray();

            var mpm1 = new MersennePm1Factor();

            int tests = 1000;
            var sw = Stopwatch.StartNew();


            var failListCount = RunTest(comp, n => MersennePm1Factor.MPm1FactorCycle(n, tests), false);
            sw.Stop();
            Debug.WriteLine("Factorization failure rate: {0}% in {1}", 100.0 * failListCount / comp.Length, sw.Elapsed);
        }


        [TestMethod()]
        public void BinaryPm1_Rsa1024Root_MPM1_Cycle()
        {
            var n = RsaChallenge.Rsa1024;
            GmpInt root = n.Sqrt();

            var comp = Enumerable.Range(1, 1000).Select(i => (GmpInt)(root + i).PowerMod(2, n)).ToArray();


            var mpm1 = new MersennePm1Factor();

            int tests = 100;
            var sw = Stopwatch.StartNew();
            var failListCount = RunGmpTest(comp, n => MersennePm1Factor.MPm1FactorCycle(n, tests), false);
            sw.Stop();
            Debug.WriteLine("Factorization failure rate: {0}% in {1}", 100.0 * failListCount / comp.Length, sw.Elapsed);
        }


        [TestMethod()]
        public void BinaryPm1_Rsa1024Root_MPM1_CycleRecurse()
        {
            var rsa1024 = RsaChallenge.Rsa1024;
            GmpInt root = rsa1024.Sqrt();
            var comp = Enumerable.Range(1, 1000).Select(i => (GmpInt)(root + i).PowerMod(2, rsa1024)).ToArray();

            int failListCount = 0;
            var mpm1 = new MersennePm1Factor();
            int tests = 5000;
            var totalSw = Stopwatch.StartNew();
            bool debugFactors = false;
            GmpInt maxN = 0;
            int i = 0;
            foreach (var cmp in comp)
            {
                i++;
                Debug.WriteLine($"Factoring root + {i}");
                Console.WriteLine($"Factoring root + {i}");
                var sw = Stopwatch.StartNew();
                var n = cmp;
                while ((n & 1) == 0)
                {
                    n >>= 1;
                }
                bool failed = true;
                sw = Stopwatch.StartNew();
                var factorization = MersennePm1Factor.MPm1FactorCycle(n, tests);
                sw.Stop();
                string indent = "";
                while (factorization.Factors.Count > 1 || (factorization.Factors.Count == 1 && factorization.Factors.First().Power > 1))
                {
                    failed = false;
                    Assert.IsTrue(factorization.Factors.Count > 0);
                    var value = factorization.GetProduct();
                    Assert.AreEqual(n, value);
                    Console.WriteLine($"{indent}Factored {factorization} in {sw.Elapsed}");
                    System.Diagnostics.Debug.WriteLine($"{indent}Factored {factorization} in {sw.Elapsed}");


                    var c = factorization.Factors.First().GetValue();
                    var p = n / c;
                    var q = n / p;

                    indent = "  ";
                    Debug.WriteLine($"{indent}q = {q} p = {p}");
                    n = p;
                    if (q > maxN)
                    {
                        maxN = q;
                    }
                    sw = Stopwatch.StartNew();
                    factorization = MersennePm1Factor.MPm1FactorCycle(n, tests);
                    sw.Stop();

                }
                if (failed)
                {
                    failListCount++;
                }
                failed = false;
                Debug.WriteLine($"{indent}Failed to factor {n} in {sw.Elapsed}");
                Console.WriteLine($"{indent}Failed to factor {n} in {sw.Elapsed}");
            }
            totalSw.Stop();
            //int tests = 100;
            //var sw = Stopwatch.StartNew();
            //var failListCount = RunGmpTest(comp, n => MersennePm1Factor.MPm1FactorCycle(n, tests), false);
            //sw.Stop();
            Debug.WriteLine("Factorization failure rate: {0}% in {1}", 100.0 * failListCount / comp.Length, totalSw.Elapsed);
            Debug.WriteLine($"Largest n: {maxN}");
            Console.WriteLine("Factorization failure rate: {0}% in {1}", 100.0 * failListCount / comp.Length, totalSw.Elapsed);
            Console.WriteLine($"Largest n: {maxN}");

        }



    }

    public class MersennePm1Factor
    {
        public static Factorization MPm1Factor(BigInteger n)
        {
            var factorization = new Factorization();
            var generator = new MersenneNumberGenerator();
            var residues = new List<BigInteger>();
            var i = 1;
            foreach (var m in generator.GenerateMersenneNumbers(300))
            {
                i++;
                var res = m % n;
                var gcd = MathUtil.Gcd(m, n);
                if (gcd > 1 && gcd < n)
                {
                    var factor = n / gcd;
                    var p = new GmpInt(gcd);
                    var q = new GmpInt(factor);

                    factorization.Add(p, 1);
                    factorization.Add(q, 1);
                    break;
                }
            }
            return factorization;
        }


        public static Factorization MPm1FactorCycle(GmpInt n, int maxTests = 100)
        {
            var factorization = new Factorization();
            var generator = new MersenneNumberGenerator();
            bool setCycles = false;
            GmpInt cycleResidue = 0;
            var i = 1;
            foreach (var m in generator.GenerateMersenneNumbers(maxTests))
            {
                i++;
                var res = m % n;
                if (!setCycles)
                {
                    if (m > n)
                    {
                        setCycles = true;
                        cycleResidue = res;
                    }
                }
                else if (res == 3 || cycleResidue == res)
                {
                    Debug.WriteLine($"Found duplicate residue {res} for {n} after {i} steps");
                    break;
                }


                var gcd = MathUtil.Gcd(m, n);
                if (gcd > 1 && gcd < n)
                {
                    var factor = n / gcd;
                    var p = new GmpInt(gcd);
                    var q = new GmpInt(factor);

                    factorization.Add(p, 1);
                    factorization.Add(q, 1);
                    return factorization;

                }

            }

            //Debug.WriteLine($"Exceeded max tests for {n} after {i} steps");
            return factorization;
        }

        public static Factorization MPm1Factor(GmpInt n, int maxTests = 100)
        {
            var factorization = new Factorization();
            var generator = new MersenneNumberGenerator();
            var residues = new List<GmpInt>();
            var i = 1;
            foreach (var m in generator.GenerateMersenneNumbers(maxTests))
            {
                i++;
                var res = m % n;
                if (res != n)
                {
                    if (residues.Contains(res))
                    {
                        Debug.WriteLine($"Found duplicate residue {res} for {n} after {i} steps");
                        break;
                    }
                    else
                    {
                        residues.Add(res);
                    }
                }
                var gcd = MathUtil.Gcd(m, n);
                if (gcd > 1 && gcd < n)
                {
                    var factor = n / gcd;
                    var p = new GmpInt(gcd);
                    var q = new GmpInt(factor);

                    factorization.Add(p, 1);
                    factorization.Add(q, 1);
                    return factorization;

                }

            }

            //Debug.WriteLine($"Exceeded max tests for {n} after {i} steps");
            return factorization;
        }
    }
    public class MersenneNumberGenerator
    {
        public IEnumerable<System.Numerics.BigInteger> GenerateMersenneNumbers(int count)
        {
            var seed = new System.Numerics.BigInteger(1);

            for (int i = 1; i <= count; i++)
            {
                seed <<= 1;
                seed += 1;
                yield return seed;
            }
        }
    }
}