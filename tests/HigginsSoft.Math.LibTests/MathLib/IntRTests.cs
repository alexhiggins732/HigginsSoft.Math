#define SKIP_LONG_TESTS
using Gee.External.Capstone.X86;
using MathGmp.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using static HigginsSoft.Math.Lib.MathLib.TonelliShanks;
using Congruence = HigginsSoft.Math.Lib.MathLib.Congruence;


namespace HigginsSoft.Math.Lib.Tests
{
    [TestClass()]
    public class IntRTests
    {
        [TestMethod()]
        public void IntRTest()
        {

            for (var radix = 2; radix <= 32; radix++)
            {
                var radixSquared = radix * radix;
                var r = new IntR(radixSquared, radix);
                var actual = r.Words.Length;
                var expected = 3;
                if (true)
                {
                    Assert.AreEqual(expected, actual);
                }
                var s = r.ToString();
                Assert.AreEqual("[1 0 0]", s);

                var value = (int)r.DecimalValue;
                Assert.AreEqual(radixSquared, value);
            }
        }



        [TestMethod()]
        public void ToStringTest()
        {
            // test in base two as it is easy to read.
            // however, in larger bases we just want to be able to have an index denoted seperator.
            // perhaps using array notation might be cleaner, eg for 0<= i <n, words[i] = {}\n
            var radix = 2;

            var r = new IntR(0, radix);
            var s = r.ToString();
            var expected = "[0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }


            r = new IntR(1, radix);
            s = r.ToString();
            expected = "[1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(2, radix);
            s = r.ToString();
            expected = "[1 0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(3, radix);
            s = r.ToString();
            expected = "[1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }


            r = new IntR(4, radix);
            s = r.ToString();
            expected = "[1 0 0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(7, radix);
            s = r.ToString();
            expected = "[1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(15, radix);
            s = r.ToString();
            expected = "[1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(16, radix);
            s = r.ToString();
            expected = "[1] [0 0 0 0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(17, radix);
            s = r.ToString();
            expected = "[1] [0 0 0 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(31, radix);
            s = r.ToString();
            expected = "[1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(32, radix);
            s = r.ToString();
            expected = "[1 0] [0 0 0 0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(64, radix);
            s = r.ToString();
            expected = "[1 0 0] [0 0 0 0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }


            r = new IntR(128, radix);
            s = r.ToString();
            expected = "[1 0 0 0] [0 0 0 0]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(255, radix);
            s = r.ToString();
            expected = "[1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(511, radix);
            s = r.ToString();
            expected = "[1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(1023, radix);
            s = r.ToString();
            expected = "[1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }


            r = new IntR(4095, radix);
            s = r.ToString();
            expected = "[1 1 1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(65535, radix);
            s = r.ToString();
            expected = "[1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(131071, radix);
            s = r.ToString();
            expected = "[1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(int.MaxValue, radix);
            s = r.ToString();
            expected = "[1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }


            r = new IntR(uint.MaxValue, radix);
            s = r.ToString();
            expected = "[1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

            r = new IntR(ulong.MaxValue, radix);
            s = r.ToString();
            expected = "[1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] -- [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1] - [1 1 1 1] [1 1 1 1]";
            if (true)
            {
                Assert.AreEqual(expected, s);
            }

        }
    }

#if SKIP_LONG_TESTS
    [Ignore]
#endif
    [TestClass]
    public class SemiPrimeTests
    {
        [TestMethod]
        public void PrintBase35()
        {
            var comp = Composites.GenerateTo(1 << 16);
            var d = new Dictionary<int, Dictionary<int, int>>();
            var d2 = new Dictionary<int, Dictionary<int, int>>();

            foreach (var c in comp)
            {
                var fact = FactorizationInt.FactorTrialDivide(c);
                var res35 = c % 35;
                if (!d.ContainsKey(res35))
                    d.Add(res35, new());
                if (!d2.ContainsKey(res35))
                    d2.Add(res35, new());
                var counts = d[res35];
                var counts2 = d2[res35];
                foreach (var f in fact.Factors)
                {
                    var modP = f.P % 35;
                    if (!counts.ContainsKey(modP))
                        counts.Add(modP, 1);
                    else
                    {
                        counts[modP]++;
                    }
                    if (!counts2.ContainsKey(f.P))
                        counts2.Add(f.P, 1);
                    else
                    {
                        counts2[f.P]++;
                    }
                }
            }

            Console.WriteLine("------------------");
            for (var i = 0; i < 35; i++)
            {

                if (d.ContainsKey(i))
                {
                    var primes = d[i].OrderBy(x => x.Key).Select(x => x.Key).ToArray();
                    Console.WriteLine($"class[{i}] = {string.Join(", ", primes)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();
            for (var i = 0; i < 35; i++)
            {

                if (d.ContainsKey(i))
                {
                    var missing = Enumerable.Range(0, 35).Where(k => !d[i].ContainsKey(k));
                    Console.WriteLine($"class[{i}] != {string.Join(", ", missing)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();
            for (var i = 0; i < 35; i++)
            {

                if (d2.ContainsKey(i))
                {
                    var primes = d2[i].OrderBy(x => x.Key).Select(x => x.Key).ToArray();
                    Console.WriteLine($"class[{i}] = {string.Join(", ", primes)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();

            var factorPrimes = d2.SelectMany(x => d2[x.Key].Select(x => x.Key)).Distinct().OrderBy(x => x).ToList();
            for (var i = 0; i < 35; i++)
            {

                if (d2.ContainsKey(i))
                {
                    var primes = d2[i].OrderBy(x => x.Key).Select(x => x.Key).OrderBy(x => x).ToList();
                    var missing = factorPrimes.Where(k => !primes.Contains(k)).ToList();
                    if (missing.Count == 0)
                    {
                        Console.WriteLine($"class[{i}] != {{}}");
                    }
                    else
                    {
                        Console.WriteLine($"class[{i}] != {string.Join(", ", missing)}");
                    }
                }
                else
                {
                    Console.WriteLine($"class[{i}] = is null");
                }
            }

        }


        [TestMethod]
        public void PrintBase35Semiprimes()
        {
            var comp = Composites.GenerateSemiPrimes().ToList();
            var d = new Dictionary<int, Dictionary<int, int>>();
            var d2 = new Dictionary<int, Dictionary<int, int>>();

            int factored = 0;
            foreach (var c in comp)
            {
                factored++;

                var res35 = c.Value % 35;
                if (!d.ContainsKey(res35))
                    d.Add(res35, new());
                if (!d2.ContainsKey(res35))
                    d2.Add(res35, new());
                var counts = d[res35];
                var counts2 = d2[res35];
                foreach (var f in new[] { c.P, c.Q })
                {
                    var modP = f % 35;
                    if (!counts.ContainsKey(modP))
                        counts.Add(modP, 1);
                    else
                    {
                        counts[modP]++;
                    }
                    if (!counts2.ContainsKey(f))
                        counts2.Add(f, 1);
                    else
                    {
                        counts2[f]++;
                    }
                }
            }

            Console.WriteLine("------------------");
            for (var i = 0; i < 35; i++)
            {

                if (d.ContainsKey(i))
                {
                    var primes = d[i].OrderBy(x => x.Key).Select(x => x.Key).ToArray();
                    Console.WriteLine($"class[{i}] = {string.Join(", ", primes)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();
            for (var i = 0; i < 35; i++)
            {

                if (d.ContainsKey(i))
                {
                    var missing = Enumerable.Range(0, 35).Where(k => !d[i].ContainsKey(k));
                    Console.WriteLine($"class[{i}] != {string.Join(", ", missing)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();
            for (var i = 0; i < 35; i++)
            {

                if (d2.ContainsKey(i))
                {
                    var primes = d2[i].OrderBy(x => x.Key).Select(x => x.Key).ToArray();
                    Console.WriteLine($"class[{i}] = {string.Join(", ", primes)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();

            var factorPrimes = d2.SelectMany(x => d2[x.Key].Select(x => x.Key)).Distinct().OrderBy(x => x).ToList();
            for (var i = 0; i < 35; i++)
            {

                if (d2.ContainsKey(i))
                {
                    var primes = d2[i].OrderBy(x => x.Key).Select(x => x.Key).OrderBy(x => x).ToList();
                    var missing = factorPrimes.Where(k => !primes.Contains(k)).ToList();
                    if (missing.Count == 0)
                    {
                        Console.WriteLine($"class[{i}] != {{}}");
                    }
                    else
                    {
                        Console.WriteLine($"class[{i}] != {string.Join(", ", missing)}");
                    }
                }
                else
                {
                    Console.WriteLine($"class[{i}] = is null");
                }
            }

        }


        void PrintSemiprimeClasses(int modulus, bool excludeModulus)
        {
            var comp = Composites.GenerateSemiPrimes().ToList();

            var SemiprimeFactorModNCounts = new Dictionary<int, Dictionary<int, int>>();
            var SemiprimeFactorCounts = new Dictionary<int, Dictionary<int, int>>();

            Console.WriteLine($"Semiprime Classes Mod {modulus} - Exclude Modulus Factors: {excludeModulus}");
            Console.WriteLine($"==========================================================================");
            var modulusFactorization = FactorizationInt.Factor(modulus);
            var modulusFactors = modulusFactorization.Factors.Select(x => x.P).ToArray();
            int factored = 0;
            foreach (var c in comp)
            {
                if (excludeModulus)
                {
                    if (c.P == modulus || c.Q == modulus || modulusFactors.Contains(c.P) || modulusFactors.Contains(c.Q))
                    {
                        continue;
                    }
                }

                factored++;

                var res = c.Value % modulus;
                if (!SemiprimeFactorModNCounts.ContainsKey(res))
                    SemiprimeFactorModNCounts.Add(res, new());
                if (!SemiprimeFactorCounts.ContainsKey(res))
                    SemiprimeFactorCounts.Add(res, new());
                var counts = SemiprimeFactorModNCounts[res];
                var counts2 = SemiprimeFactorCounts[res];
                foreach (var f in new[] { c.P, c.Q })
                {
                    var modP = f % modulus;
                    if (!counts.ContainsKey(modP))
                        counts.Add(modP, 1);
                    else
                    {
                        counts[modP]++;
                    }
                    if (!counts2.ContainsKey(f))
                        counts2.Add(f, 1);
                    else
                    {
                        counts2[f]++;
                    }
                }
            }

            Console.WriteLine($"--------{nameof(SemiprimeFactorModNCounts)} - inclusive ----------");
            for (var i = 0; i < modulus; i++)
            {

                if (SemiprimeFactorModNCounts.ContainsKey(i))
                {
                    var primes = SemiprimeFactorModNCounts[i].OrderBy(x => x.Key).Select(x => x.Key).ToArray();
                    Console.WriteLine($"class[{i}] = {string.Join(", ", primes)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{empty}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"--------{nameof(SemiprimeFactorModNCounts)} - does not include ----------");
            Console.WriteLine();
            for (var i = 0; i < modulus; i++)
            {

                if (SemiprimeFactorModNCounts.ContainsKey(i))
                {
                    var missing = Enumerable.Range(0, 35).Where(k => !SemiprimeFactorModNCounts[i].ContainsKey(k));
                    Console.WriteLine($"class[{i}] != {string.Join(", ", missing)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] != {{empty}}");
                }
            }


            Console.WriteLine();
            Console.WriteLine($"--------{nameof(SemiprimeFactorCounts)} - inclusive ----------");
            Console.WriteLine();
            for (var i = 0; i < modulus; i++)
            {

                if (SemiprimeFactorCounts.ContainsKey(i))
                {
                    var primes = SemiprimeFactorCounts[i].OrderBy(x => x.Key).Select(x => x.Key).ToArray();
                    Console.WriteLine($"class[{i}] = {string.Join(", ", primes)}");
                }
                else
                {
                    Console.WriteLine($"class[{i}] = {{}}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"--------{nameof(SemiprimeFactorCounts)} - does not include ----------");
            Console.WriteLine();

            var factorPrimes = SemiprimeFactorCounts.SelectMany(x => SemiprimeFactorCounts[x.Key].Select(x => x.Key)).Distinct().OrderBy(x => x).ToList();
            for (var i = 0; i < modulus; i++)
            {

                if (SemiprimeFactorCounts.ContainsKey(i))
                {
                    var primes = SemiprimeFactorCounts[i].OrderBy(x => x.Key).Select(x => x.Key).OrderBy(x => x).ToList();
                    var missing = factorPrimes.Where(k => !primes.Contains(k)).ToList();
                    if (missing.Count == 0)
                    {
                        Console.WriteLine($"class[{i}] != {{}}");
                    }
                    else
                    {
                        Console.WriteLine($"class[{i}] != {string.Join(", ", missing)}");
                    }
                }
                else
                {
                    Console.WriteLine($"class[{i}] = is null");
                }
            }

        }


        [TestMethod]
        public void SemiPrimesMod2()
        {
            PrintSemiprimeClasses(2, false);
            PrintSemiprimeClasses(2, true);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizations()
        {
            var semiprimes = Composites.GenerateSemiPrimes().OrderBy(x => x.Value).ToList();

            foreach (var semiprime in semiprimes)
            {
                Console.WriteLine($"{semiprime.Value} = {semiprime.P} * {semiprime.Q}");
            }
        }

        List<(int Value, int P, int Q)> semiprimes = Composites.GenerateSemiPrimes().OrderBy(x => x.Value).ToList();
        public void PrintSemiprimeValueClass(int modulus, int Class, int limit = int.MaxValue, bool excludeModulus = false)
        {

            var modulusFactorization = FactorizationInt.Factor(modulus);
            var modulusFactors = modulusFactorization.Factors.Select(x => x.P).ToArray();

            List<string> values = new();
            foreach (var semiprime in semiprimes)
            {
                if (excludeModulus)
                {
                    if (semiprime.P == modulus || semiprime.Q == modulus || modulusFactors.Contains(semiprime.P) || modulusFactors.Contains(semiprime.Q))
                    {
                        continue;
                    }
                }

                if (semiprime.Value > limit) break;
                if (semiprime.Value % modulus == Class)
                    values.Add(SemiprimeFactorization(semiprime));
            }
            Console.WriteLine($"class[{Class} mod {modulus}]:");
            Console.Write("\t");
            if (values.Count > 0)
                Console.WriteLine(string.Join(", ", values));
            else
                Console.WriteLine("{{empty set}}");
        }




        public void PrintSemiprimeFactorizationClasses(int c, int limit = int.MaxValue, bool excludeModulus = false)
        {

            var semiprimes = Composites.GenerateSemiPrimes().OrderBy(x => x.Value).ToList();

            for (var i = 0; i < c; i++)
            {
                PrintSemiprimeValueClass(c, i, limit, excludeModulus);

            }
        }

        public void PrintSemiprimeFactorizationClassConstraints(int C, int limit = 100)
        {

            Console.WriteLine("Classes without constraint.");
            PrintSemiprimeFactorizationClasses(C, limit, false);

            Console.WriteLine();
            Console.WriteLine("Classes with constraint.");
            PrintSemiprimeFactorizationClasses(C, limit, true);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizationClassesMod2()
        {
            PrintSemiprimeFactorizationClasses(2, 100, false);
            PrintSemiprimeFactorizationClasses(2, 100, true);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizationClassesMod3()
        {
            Console.WriteLine("Classes without constraint.");
            PrintSemiprimeFactorizationClasses(3, 100, false);

            Console.WriteLine();
            Console.WriteLine("Classes with constraint.");
            PrintSemiprimeFactorizationClasses(3, 100, true);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizationClassesMod4()
        {
            int C = 4;
            PrintSemiprimeFactorizationClassConstraints(C);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizationClassesMod6()
        {
            int C = 6;
            PrintSemiprimeFactorizationClassConstraints(C);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizationClassesMod9()
        {
            int C = 9;
            PrintSemiprimeFactorizationClassConstraints(C);
        }

        [TestMethod]
        public void PrintSemiprimeFactorizationClassesMod36()
        {
            int C = 36;
            PrintSemiprimeFactorizationClassConstraints(C, 1000);
        }

        private static string SemiprimeFactorization((int Value, int P, int Q) semiprime)
        {
            return $"{semiprime.Value} = {semiprime.P} * {semiprime.Q}";
        }

    }

#if SKIP_LONG_TESTS
    [Ignore]
#endif
    [TestClass]
    public class CrtCombinationTests
    {
        [TestMethod]
        public void ShowCombinations()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");
            var classes = Table.PrecomputedTableClasses;
            for (var i = 0; i < classes.Length; i++)
            {
                var c = classes[i];
                var tbl = Table.Precomputed[c];
                var n = (int)(rsa65 % c);
                Console.WriteLine($"C[{c}]");
                Console.WriteLine($" n = {n} % {c}");
                Console.WriteLine($"    In C[{c}][{n}] we have:");


                var displayed = new List<int>();

                var unfilteredPairs = tbl.GetPairs(n).ToList();

                var pairs = unfilteredPairs.Take(0).ToList();
                for (var k = 0; k < unfilteredPairs.Count; k++)
                {
                    var pair = unfilteredPairs[k];
                    var p = pair.P;
                    var q = pair.Q;
                    if (!displayed.Contains(p) && !displayed.Contains(q))
                    {
                        displayed.Add(p);
                        displayed.Add(q);
                        pairs.Add(pair);
                    }
                }

                for (var k = 0; k < pairs.Count; k++)
                {
                    var pair = pairs[k];
                    var p = pair.P;
                    var q = pair.Q;



                    Console.Write($"       p = {p} and q = {q} or p = {q} and q = {p}");
                    if (k < pairs.Count - 1)
                        Console.Write("; or,");
                    Console.WriteLine();


                }
            }


        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void ShowClassDensities()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");

            for (var c = 2; c < 10000; c++)
            {
                var cBits = MathLib.Log2(c);
                var tbl = new Table(c);
                var n = (int)(rsa65 % c);

                var unfilteredPairs = tbl.GetPairs(n).ToList();
                var density = unfilteredPairs.Count / cBits;
                var bitDensity = cBits / unfilteredPairs.Count;

                Console.WriteLine($"{c.ToString().PadLeft(4, ' ')} (2^{cBits.ToString("N4")}) - {unfilteredPairs.Count} Pairs\tBit Density: {bitDensity.ToString("P").PadRight(7, ' ')}\tRatio Density: {density.ToString("N4")}");
            }
        }

        [TestMethod]
        public void ShowCombinationsOfN()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");


            var c = 230;
            var cBits = MathLib.Log2(c);
            var tbl = new Table(c);
            var n = (int)(rsa65 % c);

            var unfilteredPairs = tbl.GetPairs(n).ToList();
            var density = unfilteredPairs.Count / cBits;
            var bitDensity = cBits / unfilteredPairs.Count;
            Console.WriteLine($"C[{c}]");
            Console.WriteLine($" n = {n} % {c}");
            Console.WriteLine($"    In C[{c}][{n}] we have {unfilteredPairs.Count} classes (density: {bitDensity.ToString("N2")}):");


            var displayed = new List<int>();
            var pairs = unfilteredPairs.Take(0).ToList();
            for (var k = 0; k < unfilteredPairs.Count; k++)
            {
                var pair = unfilteredPairs[k];
                var p = pair.P;
                var q = pair.Q;
                if (!displayed.Contains(p) && !displayed.Contains(q))
                {
                    displayed.Add(p);
                    displayed.Add(p);
                    pairs.Add(pair);
                }
            }


            for (var k = 0; k < pairs.Count; k++)
            {
                var pair = pairs[k];
                var p = pair.P;
                var q = pair.Q;

                Console.Write($"       p = {p} and q = {q} or p = {q} and q = {p}");
                if (k < pairs.Count - 1)
                    Console.Write("; or,");
                Console.WriteLine();


            }
        }

        [TestMethod]
        public void CongruenceBruteForce()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");
            int[] classes = { 360, 290, 36, 29, 10 };


            var congruences = new Dictionary<int, Dictionary<string, List<Congruence>>>();
            for (var i = 0; i < classes.Length; i++)
            {
                var c = classes[i];
                var tbl = new Table(c);

                var n = (int)(rsa65 % c);
                Console.WriteLine($"C[{c}]");
                Console.WriteLine($" n = {n} % {c}");
                Console.WriteLine($"    In C[{c}][{n}] we have:");


                var displayed = new List<int>();

                var unfilteredPairs = tbl.GetPairs(n).ToList();

                var classCongruences = new Dictionary<string, List<Congruence>>();
                congruences.Add(c, classCongruences);

                var conqruenceP = new List<Congruence>();
                var conqruenceQ = new List<Congruence>();
                classCongruences.Add("p", conqruenceP);
                classCongruences.Add("q", conqruenceQ);
                for (var k = 0; k < unfilteredPairs.Count; k++)
                {
                    var pair = unfilteredPairs[k];
                    var p = pair.P;
                    var q = pair.Q;
                    if (!displayed.Contains(p) && !displayed.Contains(q))
                    {
                        displayed.Add(p);
                        displayed.Add(q);
                        conqruenceP.Add(new Congruence(p, c));
                        conqruenceQ.Add(new Congruence(q, c));
                    }
                }
            }


            var pList = congruences.Select(x => x.Value["p"].ToList()).ToList();

            SearchCongruences(pList);
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void CongruenceOfRsa65SolutionCount()
        {

            /*
                x	≡	4	(mod 31)
                x	≡	15	(mod 29)
                x	≡	35	(mod 36)
                x	≡	34	(mod 37)
                x	≡	7	(mod 41)
                x	≡	3	(mod 43)
                x ≡ 159146099 (mod 2111136084)
             * */


            var l = new List<List<Congruence>>();
            for (var i = 0; i < 4; i++)
            {
                var t = new List<Congruence> { new(0, 2), new(1, 2) };
                l.Add(t);
            }
            var sb = new StringBuilder();
            foreach (var c in EnumerateCongruences(l))
            {
                var bits = c.Select(x => x.Poly).ToArray();
                var bitString = string.Join("", bits);
                sb.AppendLine(bitString);
            }
            //Console.WriteLine(sb.ToString());

            var count = EnumerateCongruences(l).Count();
            Assert.AreEqual(16, count);

            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");

            CountSolutions(rsa65, new[] { 2, 3 });
            CountSolutions(rsa65, new[] { 2, 3, 5 });
            CountSolutions(rsa65, new[] { 2, 3, 5, 7 });
            CountSolutions(rsa65, new[] { 2, 3, 5, 7, 11 });
            CountSolutions(rsa65, new[] { 2, 3, 5, 7, 11, 13 });
            CountSolutions(rsa65, new[] { 2, 3, 5, 7, 11, 13, 17 });
            CountSolutions(rsa65, new[] { 6, 5, 7, 11, 13, 17, 19 });
            //CountSolutions(rsa65, new[] { 2 * 3 * 5, 7 * 11 * 13, 17 * 19 * 23, 29 * 31 * 37 });
            var class360 = 2 * 2 * 3 * 3 * 10;
            int[] classes = { 360, 290, 36, 29, 10 };
            CountSolutions(rsa65, classes);
            //exceeds square root - either a good thing that leads to an exact solution or need to find combinations of classes less than square root.
            // CountSolutions(rsa65, new[] { 360, 290, 36, 29, 10, 100 });

            CountSolutions(rsa65, new[] { 31, 29, 36, 37, 41, 43 });
            CountSolutions(rsa65, new[] { 360, 19 * 41, 23 * 37, 29 * 31 });
            CountSolutions(rsa65, new[] { 360, 7 * 11 * 17 * 23 });
        }


        public List<List<Congruence>> GetCongruenceSolutions(GmpInt n, int[] classes)
        {
            var congruences = new Dictionary<int, Dictionary<string, List<Congruence>>>();
            for (var i = 0; i < classes.Length; i++)
            {
                var c = classes[i];
                var tbl = new Table(c);

                var N = (int)(n % c);

                var displayed = new List<int>();

                var unfilteredPairs = tbl.GetPairs(N).ToList();

                var classCongruences = new Dictionary<string, List<Congruence>>();
                congruences.Add(c, classCongruences);

                var conqruenceP = new List<Congruence>();
                var conqruenceQ = new List<Congruence>();
                classCongruences.Add("p", conqruenceP);
                classCongruences.Add("q", conqruenceQ);
                for (var k = 0; k < unfilteredPairs.Count; k++)
                {
                    var pair = unfilteredPairs[k];
                    var p = pair.P;
                    var q = pair.Q;
                    if (!displayed.Contains(p) && !displayed.Contains(q))
                    {

                        displayed.Add(p);
                        if (p != q)
                            displayed.Add(q);
                        conqruenceP.Add(new Congruence(p, c));
                        if (p != q)
                            conqruenceQ.Add(new Congruence(q, c));
                    }
                }

                Console.WriteLine($"C[{c}] - n = {N} % {c}");
                Console.WriteLine($"    In C[{c}][{N}] we have {conqruenceP.Count} P solutions and {conqruenceQ.Count} Q Solutions.");
            }


            var pList = congruences.Select(x => x.Value["p"].ToList()).ToList();
            var qList = congruences.Select(x => x.Value["q"].ToList()).ToList();
            for (var i = 0; i < pList.Count; i++)
            {
                pList[i].AddRange(qList[i]);
            }
            return pList;
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void Count2pSolutions()
        {

            int k = 0;
            for (var n = 4; n < 2048; n <<= 1)
            {
                BigInteger N = 1;
                N <<= n;
                BigInteger root = N >> (n / 2);
                BigInteger limit = root >> (n / 4);
                BigInteger size = 1;
                BigInteger count = 1;
                List<int> classes = new();
                for (var i = 0; size < limit && i < Primes.UintFactorPrimes.Length; i++)
                {
                    int p = Primes.UintFactorPrimes[i];
                    classes.Add(p);
                    size *= p;
                    count *= (p - 1);
                }



                var stepBits = MathLib.Log2(size);

                var OpsPerSolution = root / size;
                var TotalOps = OpsPerSolution * count;
                var bits = MathLib.Log2(OpsPerSolution);
                var totalBits = MathLib.Log2(TotalOps);
                var totalBitsN = MathLib.Log2(N);
                var s = totalBitsN.ToString("n1");
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------------------------------------------------");
                Console.WriteLine($" => System has {count.ToString("N0")} P solutions for n (2^{totalBitsN.ToString("N2")}) mod {N} (2^{stepBits.ToString("N2")}) for classes: {string.Join(", ", classes)}");
                Console.WriteLine($"    => 2^{bits.ToString("N2")} ops/sol == 2^{totalBits.ToString("N2")} ops total");

                Console.WriteLine("-------------------------------------------------------------------------------------------------");
                Console.WriteLine();


            }
        }

        public void CountSolutions(GmpInt n, int[] classes)
        {
            var congruences = new Dictionary<int, Dictionary<string, List<Congruence>>>();
            for (var i = 0; i < classes.Length; i++)
            {
                var c = classes[i];
                var tbl = new Table(c);

                var j = (int)(n % c);
                //Console.WriteLine($"C[{c}]");
                //Console.WriteLine($" n = {} % {c}");
                //Console.WriteLine($"    In C[{c}][{}] we have:");


                var displayed = new List<int>();

                //var unfilteredModPairs = tbl.GetPairsViaMod().ToList();
                //foreach(var Pair in tbl.GetPairs())
                //{
                //    Console.WriteLine(Pair);
                //}
                var unfilteredPairs = tbl.GetPairs(j).ToList();

                var classCongruences = new Dictionary<string, List<Congruence>>();
                congruences.Add(c, classCongruences);

                var conqruenceP = new List<Congruence>();
                var conqruenceQ = new List<Congruence>();
                classCongruences.Add("p", conqruenceP);
                classCongruences.Add("q", conqruenceQ);
                for (var k = 0; k < unfilteredPairs.Count; k++)
                {
                    var pair = unfilteredPairs[k];
                    var p = pair.P;
                    var q = pair.Q;
                    if (!displayed.Contains(p) && !displayed.Contains(q))
                    {
                        displayed.Add(p);
                        displayed.Add(q);
                        conqruenceP.Add(new Congruence(p, c));
                        conqruenceQ.Add(new Congruence(q, c));
                    }
                }

                Console.WriteLine($"C[{c}] - n = {j} % {c}");
                Console.WriteLine($"    In C[{c}][{j}] we have {conqruenceP.Count} P solutions and {conqruenceQ.Count} Q Solutions.");
            }


            var pList = congruences.Select(x => x.Value["p"].ToList()).ToList();

            //var count = EnumerateCongruences(pList).Count();
            BigInteger count = 1L;
            for (var i = 0; i < pList.Count; i++)
                count *= pList[i].Count;

            BigInteger size = 1;
            foreach (var p in classes)
                size *= p;
            var N = size;
            var stepBits = MathLib.Log2(N);
            var root = MathLib.Sqrt(n);
            var OpsPerSolution = root / size;
            var TotalOps = OpsPerSolution * count;
            var bits = MathLib.Log2(OpsPerSolution);
            var totalBits = MathLib.Log2(TotalOps);
            var totalBitsN = MathLib.Log2(n);
            var s = totalBitsN.ToString("n1");
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            Console.WriteLine($" => System has {count.ToString("N0")} P solutions for n (2^{totalBitsN.ToString("N2")}) mod {N} (2^{stepBits.ToString("N2")}) for classes: {string.Join(", ", classes)}");
            Console.WriteLine($"    => 2^{bits.ToString("N2")} ops/sol == 2^{totalBits.ToString("N2")} ops total");

            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            Console.WriteLine();
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void CongruenceOfRsa126SolutionCount()
        {

            /*
                x	≡	4	(mod 31)
                x	≡	15	(mod 29)
                x	≡	35	(mod 36)
                x	≡	34	(mod 37)
                x	≡	7	(mod 41)
                x	≡	3	(mod 43)
                x ≡ 159146099 (mod 2111136084)
             * */


            GmpInt rsa126 = "46935327687222996017896600397399584183";

            Console.WriteLine($"n = {rsa126}");

            //CountSolutions(rsa126, new[] { 2, 3 });
            //CountSolutions(rsa126, new[] { 2, 3, 5 });
            //CountSolutions(rsa126, new[] { 2, 3, 5, 7 });
            //CountSolutions(rsa126, new[] { 2, 3, 5, 7, 11 });
            //CountSolutions(rsa126, new[] { 2, 3, 5, 7, 11, 13 });
            //CountSolutions(rsa126, new[] { 2, 3, 5, 7, 11, 13, 17 });
            //CountSolutions(rsa126, new[] { 6, 5, 7, 11, 13, 17, 19 });
            //CountSolutions(rsa65, new[] { 2 * 3 * 5, 7 * 11 * 13, 17 * 19 * 23, 29 * 31 * 37 });
            var class360 = 2 * 2 * 3 * 3 * 10;
            int[] classes = { 360, 290, 36, 25, 3 * 3 * 5 * 5, 29, 10 };
            CountSolutions(rsa126, classes);

            classes = new[] { 360, 290, 36, 25, 3 * 3 * 5 * 5, 29, 10, 23, 19, 17, 13, 7 };
            CountSolutions(rsa126, classes);


            CountSolutions(rsa126, new[] { 2 * 2 * 3 * 3, 5 * 5 * 7 * 7 });


            //var val = BigInteger.Pow(2, 6) * BigInteger.Pow(3, 6);
            ////var val2 = BigInteger.Pow(5, 6) * BigInteger.Pow(3, 6);
            //CountSolutions(rsa126, (new[] { (int)val }).Concat(Primes.IntFactorPrimes.Skip(2).Take(11)).ToArray());


            //  => 2^3.17 ops/sol == 2^46.50 ops total
            CountSolutions(rsa126, new[] { 2, 3, 6, 2 * 2 * 3 * 3, 2 * 2 * 2 * 3 * 3 * 3, 5 * 5 * 7 * 7, 11, 13, 11 * 13, 17, 19, 17 * 19 });

            //exceeds square root - either a good thing that leads to an exact solution or need to find combinations of classes less than square root.
            // CountSolutions(rsa65, new[] { 360, 290, 36, 29, 10, 100 });

            //CountSolutions(rsa126, new[] { 31, 29, 36, 37, 41, 43 });

            CountSolutions(rsa126, new[] { 19, 23, 31, 29, 36, 37, 41, 43, 47, 51, 53, 57, });

            int skip = 15, take = 10;
            CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(15).Take(10).ToArray());
            Console.WriteLine($"//Skip({skip}).Take({take})");

            CountSolutions(rsa126, new[] { 53 * 59, 61 * 67, 71 * 73, 79 * 83, 89 * 97 });


            skip = 500; take = 5;
            CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(skip).Take(take).ToArray());
            Console.WriteLine($"//Skip({skip}).Take({take})");

            skip = 4; take = 10;
            CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(skip).Take(take).ToArray());
            Console.WriteLine($"//Skip({skip}).Take({take})");

            skip = 4; take = 12;
            CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(skip).Take(take).ToArray());
            Console.WriteLine($"//Skip({skip}).Take({take})");
            //overflow
            //Console.Write("Skip(4).Take(13");
            //CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(4).Take(13).ToArray());

            skip = 3; take = 13;
            CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(skip).Take(take).ToArray());
            Console.WriteLine($"//Skip({skip}).Take({take})");

            skip = 2; take = 14;
            CountSolutions(rsa126, Primes.UintFactorPrimes.Skip(skip).Take(take).ToArray());
            Console.WriteLine($"//Skip({skip}).Take({take})");

            //CountSolutions(rsa126, new[] { 53 * 59* 61,  67, 71 * 73, 79 * 83, 89 * 97 });
            //CountSolutions(rsa126, new[] { 360, 19 * 41, 23 * 37, 29 * 31,  });
            // CountSolutions(rsa126, new[] { 360, 7 * 11 * 17 * 23 });
        }

        [TestMethod]
        public void SolveSolutionRsa65BitWithList()
        {

            /*    
            C[31] - n = 6 % 31
                In C[31][6] we have 15 P solutions and 15 Q Solutions.
            C[29] - n = 7 % 29
                In C[29][7] we have 15 P solutions and 15 Q Solutions.
            C[36] - n = 29 % 36
                In C[36][29] we have 6 P solutions and 6 Q Solutions.
            C[37] - n = 29 % 37
                In C[37][29] we have 18 P solutions and 18 Q Solutions.
            C[41] - n = 36 % 41
                In C[41][36] we have 21 P solutions and 21 Q Solutions.
            C[43] - n = 29 % 43
                In C[43][29] we have 21 P solutions and 21 Q Solutions.

            -------------------------------------------------------------------------------------------------
             => System has 10,716,300 P solutions for n (2^64.44) mod 2111136084 for classes: 31, 29, 36, 37, 41, 43
                => 2^1.00 ops/sol == 2^24.35 ops total
            -------------------------------------------------------------------------------------------------*/


            GmpInt rsa65Gmp = "25063116026386232513";
            BigInteger rsa65 = (BigInteger)rsa65Gmp;
            long np = 4381418267;
            long nq = 720320339;
            var root = (long)MathLib.Sqrt(rsa65);
            int[] classes = { 31, 29, 36, 37, 41, 43 };

            int[] psolutions = classes.Select(c => (int)(np % c)).ToArray();
            int[] qsolutions = classes.Select(c => (int)(nq % c)).ToArray();
            int[] expectedSteps = psolutions.Select(x => 0).ToArray();

            //TODO: 1) enumerate s directly
            //      2) More efficient, combine CRT solver and enumerate start values directly.
            var solutions = GetCongruenceSolutions(rsa65, classes);

            for (var i = 0; i < psolutions.Length; i++)
            {
                var classSolutions = solutions[i].Select(x => x.Poly).ToList();
                var pExpected = psolutions[i];
                var actual = classSolutions.Contains(pExpected);
                var expected = true;
                if (actual != expected)
                {
                    var className = classes[i];
                    var message = $"Class solutions {className} doesn not contain p solution {pExpected}";
                    Assert.AreEqual(expected, actual, message);
                }
                else
                {
                    expectedSteps[i] = i == 0 ? classSolutions.IndexOf(pExpected) + 1 : classSolutions.Count;
                }
            }

            long expectedCongruenceChecks = 1;
            for (var i = 0; i < expectedSteps.Length; i++)
            {
                expectedCongruenceChecks *= expectedSteps[i];
            }

            var solutionCount = 1;
            foreach (var solution in solutions)
            {
                solutionCount *= solution.Count;
            }
            Console.WriteLine($"Searching {solutionCount.ToString("N0")} solutions - Expected solution at step {expectedCongruenceChecks}");
            var solver = new MathLib.CrtCongruenceSolver(classes);
            var N = solver.ComputeN();
            int totalSteps = 0;

            //took: 14696081 steps but only expected
            int congruenceCount = 0;
            double pctComplete = congruenceCount / solutionCount * 100;
            var sw = Stopwatch.StartNew();
            if (false)
            {



                long totalRemainders = 0;
                foreach (var congruence in EnumerateCongruences(solutions))
                {
                    congruenceCount++;
                    totalRemainders += congruence.Count;
                    if (congruenceCount == expectedCongruenceChecks)
                    {
                        sw.Stop();
                        break;
                    }
                }
                Console.WriteLine($"EnumerateCongruences took {sw.Elapsed}.");
            }

            congruenceCount = 0;
            int[] remainders = psolutions.Select(x => 0).ToArray();
            BigInteger gcd = 1;
            sw = Stopwatch.StartNew();
            foreach (var congruence in EnumerateCongruences(solutions))
            {
                congruenceCount++;
                pctComplete = (double)congruenceCount / solutionCount * 100;
                var isExpectedSolution = true;
                for (var i = 0; i < remainders.Length; i++)
                {
                    ref int remainder = ref remainders[i];
                    remainder = congruence[i].Poly;
                    isExpectedSolution &= remainder == psolutions[i];
                }

                if (isExpectedSolution)
                {
                    string bp = $"Found expected solution after checking {congruenceCount.ToString("N0")} solutions";
                    Console.WriteLine(bp);
                }
                var solution = solver.Solve(remainders);

                var step = solution.N;
                long value = (long)solution.Result;

                //validate the solution.
                if (false)
                {
                    foreach (var c in congruence)
                    {
                        var res = ((long)solution.Result + step) % c.Base;
                        if (res != c.Poly)
                        {
                            Assert.AreEqual(c.Poly, res);
                        }
                    }
                }
                int solutionStep = 0;
                while (value < root)
                {
                    totalSteps++;
                    solutionStep++;
                    if (true)
                    {

                        gcd = MathUtil.Gcd(rsa65, value);

                        if (gcd > 1 && gcd < rsa65)
                        {
                            sw.Stop();
                            var p = gcd;
                            var q = rsa65 / p;
                            var cString = $"({string.Join(") (", congruence)})";
                            string message = $"Found divisors {p} * {q} = {rsa65} after {totalSteps.ToString("N0")} total steps in {sw.Elapsed}.\n\t Congruences: {congruenceCount.ToString("N0")} ({pctComplete.ToString("n2")}%) - step {solutionStep} -  system {cString} ";
                            Console.WriteLine(message);
                            break;
                        }
                    }
                    value += step;
                }
                if (isExpectedSolution)
                {
                    if (gcd == 1 || gcd == rsa65)
                    {
                        string message = "Failed to find factor using expected solution";
                        Assert.IsFalse(isExpectedSolution, message);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        [TestMethod]
        public void SolveSolutionRsa65BitWithArray()
        {

            /*    
            C[31] - n = 6 % 31
                In C[31][6] we have 15 P solutions and 15 Q Solutions.
            C[29] - n = 7 % 29
                In C[29][7] we have 15 P solutions and 15 Q Solutions.
            C[36] - n = 29 % 36
                In C[36][29] we have 6 P solutions and 6 Q Solutions.
            C[37] - n = 29 % 37
                In C[37][29] we have 18 P solutions and 18 Q Solutions.
            C[41] - n = 36 % 41
                In C[41][36] we have 21 P solutions and 21 Q Solutions.
            C[43] - n = 29 % 43
                In C[43][29] we have 21 P solutions and 21 Q Solutions.

            -------------------------------------------------------------------------------------------------
             => System has 10,716,300 P solutions for n (2^64.44) mod 2111136084 for classes: 31, 29, 36, 37, 41, 43
                => 2^1.00 ops/sol == 2^24.35 ops total
            -------------------------------------------------------------------------------------------------*/


            GmpInt rsa65Gmp = "25063116026386232513";
            BigInteger rsa65 = (BigInteger)rsa65Gmp;
            long np = 4381418267;
            long nq = 720320339;
            var root = (long)MathLib.Sqrt(rsa65);
            int[] classes = { 31, 29, 36, 37, 41, 43 };

            int[] psolutions = classes.Select(c => (int)(np % c)).ToArray();
            int[] qsolutions = classes.Select(c => (int)(nq % c)).ToArray();
            int[] expectedSteps = psolutions.Select(x => 0).ToArray();

            //TODO: 1) enumerate s directly
            //      2) More efficient, combine CRT solver and enumerate start values directly.
            var solutions = GetCongruenceSolutions(rsa65, classes);

            for (var i = 0; i < psolutions.Length; i++)
            {
                var classSolutions = solutions[i].Select(x => x.Poly).ToList();
                var pExpected = psolutions[i];
                var actual = classSolutions.Contains(pExpected);
                var expected = true;
                if (actual != expected)
                {
                    var className = classes[i];
                    var message = $"Class solutions {className} doesn not contain p solution {pExpected}";
                    Assert.AreEqual(expected, actual, message);
                }
                else
                {
                    expectedSteps[i] = i == 0 ? classSolutions.IndexOf(pExpected) + 1 : classSolutions.Count;
                }
            }

            long expectedCongruenceChecks = 1;
            for (var i = 0; i < expectedSteps.Length; i++)
            {
                expectedCongruenceChecks *= expectedSteps[i];
            }

            var solutionCount = 1;
            foreach (var solution in solutions)
            {
                solutionCount *= solution.Count;
            }
            Console.WriteLine($"Searching {solutionCount.ToString("N0")} solutions - Expected solution at step {expectedCongruenceChecks}");
            var solver = new MathLib.CrtCongruenceSolver(classes);
            var N = solver.ComputeN();
            int totalSteps = 0;

            //took: 14696081 steps but only expected
            long congruenceCount = 0;
            double pctComplete = congruenceCount / solutionCount * 100;
            var sw = Stopwatch.StartNew();
            if (true)
            {

                long totalRemainders = 0;
                foreach (var congruence in EnumerateCongruencesArray(solutions))
                {
                    congruenceCount++;
                    totalRemainders += congruence.Length;
                    if (congruenceCount == expectedCongruenceChecks)
                    {
                        sw.Stop();
                        break;
                    }
                }
                Console.WriteLine($"EnumerateCongruences took {sw.Elapsed}.");
            }

            congruenceCount = 0;
            //int[] remainders = psolutions.Select(x => 0).ToArray();
            var isExpectedSolution = true;
            BigInteger gcd = 1;
            sw = Stopwatch.StartNew();
            Stopwatch solveWatch = new();
            bool checkSolution = false;
            foreach (var congruence in EnumerateCongruencesArray(solutions))
            {
                int[] remainders = congruence;
                congruenceCount++;
                pctComplete = (double)congruenceCount / solutionCount * 100;

                if (checkSolution)
                {
                    isExpectedSolution = true;
                    for (var i = 0; isExpectedSolution && i < remainders.Length; i++)
                    {
                        isExpectedSolution &= remainders[i] == psolutions[i];
                    }

                    if (isExpectedSolution)
                    {
                        string bp = $"Found expected solution after checking {congruenceCount.ToString("N0")} solutions";
                        Console.WriteLine(bp);
                    }
                }

                solveWatch.Start();
                var solution = solver.Solve(remainders);
                solveWatch.Stop();
                var step = solution.N;
                long value = (long)solution.Result + step;

                //validate the solution.

                int solutionStep = 0;
                var test = value + step;
                //if step size is sufficient should only need to test the last value before sqrt(n);
                while (test < root)
                {
                    value = test;
                    test += step;
                }
                while (value < root)
                {
                    totalSteps++;
                    solutionStep++;
                    if (true)
                    {

                        gcd = MathUtil.Gcd(rsa65, value);

                        if (gcd > 1 && gcd < rsa65)
                        {
                            sw.Stop();
                            var p = gcd;
                            var q = rsa65 / p;
                            var cString = $"({string.Join(") (", congruence)})";
                            string message = $"Found divisors {p} * {q} = {rsa65} after {totalSteps.ToString("N0")} total steps in {sw.Elapsed}.\n\t Congruences: {congruenceCount.ToString("N0")} ({pctComplete.ToString("n2")}%) - step {solutionStep} -  system {cString}\n\t Solving took {solveWatch.Elapsed} ";
                            Console.WriteLine(message);
                            if (checkSolution)
                            {
                                break;
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                    value += step;
                }
                if (checkSolution)
                {
                    if (isExpectedSolution)
                    {
                        if (gcd == 1 || gcd == rsa65)
                        {
                            string message = "Failed to find factor using expected solution";
                            Assert.IsFalse(isExpectedSolution, message);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SolveSolutionRsa65BitWithDirectResult()
        {

            /*    
            C[31] - n = 6 % 31
                In C[31][6] we have 15 P solutions and 15 Q Solutions.
            C[29] - n = 7 % 29
                In C[29][7] we have 15 P solutions and 15 Q Solutions.
            C[36] - n = 29 % 36
                In C[36][29] we have 6 P solutions and 6 Q Solutions.
            C[37] - n = 29 % 37
                In C[37][29] we have 18 P solutions and 18 Q Solutions.
            C[41] - n = 36 % 41
                In C[41][36] we have 21 P solutions and 21 Q Solutions.
            C[43] - n = 29 % 43
                In C[43][29] we have 21 P solutions and 21 Q Solutions.

            -------------------------------------------------------------------------------------------------
             => System has 10,716,300 P solutions for n (2^64.44) mod 2111136084 for classes: 31, 29, 36, 37, 41, 43
                => 2^1.00 ops/sol == 2^24.35 ops total
            -------------------------------------------------------------------------------------------------*/


            GmpInt rsa65Gmp = "25063116026386232513";
            BigInteger rsa65 = (BigInteger)rsa65Gmp;
            long np = 4381418267;
            long nq = 720320339;
            var root = (long)MathLib.Sqrt(rsa65);
            int[] classes = { 31, 29, 36, 37, 41, 43 };

            int[] psolutions = classes.Select(c => (int)(np % c)).ToArray();
            int[] qsolutions = classes.Select(c => (int)(nq % c)).ToArray();
            int[] expectedSteps = psolutions.Select(x => 0).ToArray();

            //TODO: 1) enumerate s directly
            //      2) More efficient, combine CRT solver and enumerate start values directly.
            var solutions = GetCongruenceSolutions(rsa65, classes);

            for (var i = 0; i < psolutions.Length; i++)
            {
                var classSolutions = solutions[i].Select(x => x.Poly).ToList();
                var pExpected = psolutions[i];
                var actual = classSolutions.Contains(pExpected);
                var expected = true;
                if (actual != expected)
                {
                    var className = classes[i];
                    var message = $"Class solutions {className} doesn not contain p solution {pExpected}";
                    Assert.AreEqual(expected, actual, message);
                }
                else
                {
                    expectedSteps[i] = i == 0 ? classSolutions.IndexOf(pExpected) + 1 : classSolutions.Count;
                }
            }

            long expectedCongruenceChecks = 1;
            for (var i = 0; i < expectedSteps.Length; i++)
            {
                expectedCongruenceChecks *= expectedSteps[i];
            }

            var solutionCount = 1;
            foreach (var solution in solutions)
            {
                solutionCount *= solution.Count;
            }
            Console.WriteLine($"Searching {solutionCount.ToString("N0")} solutions - Expected solution at step {expectedCongruenceChecks}");
            var solver = new MathLib.CrtCongruenceRecursiveEnumerator(classes);
            var N = solver.ComputeN();
            int totalSteps = 0;

            //took: 14696081 steps but only expected
            long congruenceCount = 0;
            double pctComplete = congruenceCount / solutionCount * 100;
            var sw = Stopwatch.StartNew();
            if (true)
            {

                foreach (var congruence in solver.EnumerateResults(solutions))
                {
                    congruenceCount++;
                    if (congruenceCount == expectedCongruenceChecks)
                    {
                        sw.Stop();
                        break;
                    }
                }
                Console.WriteLine($"EnumerateCongruences took {sw.Elapsed}.");
            }

            congruenceCount = 0;
            //int[] remainders = psolutions.Select(x => 0).ToArray();
            var isExpectedSolution = true;
            BigInteger gcd = 1;
            sw = Stopwatch.StartNew();

            bool checkSolution = false;
            var solveWatch = Stopwatch.StartNew();
            foreach (var solution in solver.EnumerateResults(solutions))
            {
                solveWatch.Stop();
                congruenceCount++;
                pctComplete = (double)congruenceCount / solutionCount * 100;


                var step = N;
                long value = (long)solution + step;

                //validate the solution.

                int solutionStep = 0;
                var test = value + step;
                //if step size is sufficient should only need to test the last value before sqrt(n);
                while (test < root)
                {
                    value = test;
                    test += step;
                }
                while (value < root)
                {
                    totalSteps++;
                    solutionStep++;
                    if (true)
                    {

                        gcd = MathUtil.Gcd(rsa65, value);

                        if (gcd > 1 && gcd < rsa65)
                        {
                            sw.Stop();

                            var p = gcd;
                            var q = rsa65 / p;
                            var cString = $"()";
                            string message = $"Found divisors {p} * {q} = {rsa65} after {totalSteps.ToString("N0")} total steps in {sw.Elapsed}.\n\t Congruences: {congruenceCount.ToString("N0")} ({pctComplete.ToString("n2")}%) - step {solutionStep} -  system {cString}\n\t Solving took {solveWatch.Elapsed} ";
                            Console.WriteLine(message);
                            if (checkSolution)
                            {
                                break;
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                    value += step;
                }
                if (checkSolution)
                {
                    if (isExpectedSolution)
                    {
                        if (gcd == 1 || gcd == rsa65)
                        {
                            string message = "Failed to find factor using expected solution";
                            Assert.IsFalse(isExpectedSolution, message);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                solveWatch.Start();
            }
        }


        [TestMethod]
        public void SolveSolution2047BitWithDirectResult()
        {

            /*    
            C[31] - n = 6 % 31
                In C[31][6] we have 15 P solutions and 15 Q Solutions.
            C[29] - n = 7 % 29
                In C[29][7] we have 15 P solutions and 15 Q Solutions.
            C[36] - n = 29 % 36
                In C[36][29] we have 6 P solutions and 6 Q Solutions.
            C[37] - n = 29 % 37
                In C[37][29] we have 18 P solutions and 18 Q Solutions.
            C[41] - n = 36 % 41
                In C[41][36] we have 21 P solutions and 21 Q Solutions.
            C[43] - n = 29 % 43
                In C[43][29] we have 21 P solutions and 21 Q Solutions.

            -------------------------------------------------------------------------------------------------
             => System has 10,716,300 P solutions for n (2^64.44) mod 2111136084 for classes: 31, 29, 36, 37, 41, 43
                => 2^1.00 ops/sol == 2^24.35 ops total
            -------------------------------------------------------------------------------------------------*/



            BigInteger[] tests = new BigInteger[] { 2047, (1 << 23) - 1, (1 << 29) - 1 };

            foreach (var t in tests)
            {
                var n = t;
                var f = Factorization.Factor(t);
                long p = (long)f.factors[0].P;
                long q = (long)(t / p);


                var result = 0L;


                result = SolveGeneralSolutionBitWithDirectResult(n, new[] { 2, 3 }, p, q);
                result = SolveGeneralSolutionBitWithDirectResult(n, new[] { 2, 3, 5 }, p, q);
                result = SolveGeneralSolutionBitWithDirectResult(n, new[] { 6, 5 }, p, q);
                result = SolveGeneralSolutionBitWithDirectResult(n, new[] { 6, 25 }, p, q);
                result = SolveGeneralSolutionBitWithDirectResult(n, new[] { 5, 36 }, p, q);
                result = SolveGeneralSolutionBitWithDirectResult(n, new[] { 25, 36 }, p, q);
            }
        }
        public long SolveGeneralSolutionBitWithDirectResult(BigInteger n, int[] classes, long np, long nq)
        {
            var root = (long)MathLib.Sqrt(n);
            //int[] classes = { 2, 3 };

            int[] psolutions = classes.Select(c => (int)(np % c)).ToArray();
            int[] qsolutions = classes.Select(c => (int)(nq % c)).ToArray();
            int[] expectedSteps = psolutions.Select(x => 0).ToArray();

            //TODO: 1) enumerate s directly
            //      2) More efficient, combine CRT solver and enumerate start values directly.
            var solutions = GetCongruenceSolutions(n, classes);

            for (var i = 0; i < psolutions.Length; i++)
            {
                var classSolutions = solutions[i].Select(x => x.Poly).ToList();
                var pExpected = psolutions[i];
                var actual = classSolutions.Contains(pExpected);
                var expected = true;
                if (actual != expected)
                {
                    var className = classes[i];
                    var message = $"Class solutions {className} doesn not contain p solution {pExpected}";
                    Assert.AreEqual(expected, actual, message);
                }
                else
                {
                    expectedSteps[i] = i == 0 ? classSolutions.IndexOf(pExpected) + 1 : classSolutions.Count;
                }
            }

            long expectedCongruenceChecks = 1;
            for (var i = 0; i < expectedSteps.Length; i++)
            {
                expectedCongruenceChecks *= expectedSteps[i];
            }

            var solutionCount = 1;
            foreach (var solution in solutions)
            {
                solutionCount *= solution.Count;
            }
            Console.WriteLine($"Searching {solutionCount.ToString("N0")} solutions - Expected solution at step {expectedCongruenceChecks}");
            var solver = new MathLib.CrtCongruenceRecursiveEnumerator(classes);
            var N = solver.ComputeN();
            int totalSteps = 0;

            //took: 14696081 steps but only expected
            long congruenceCount = 0;
            double pctComplete = congruenceCount / solutionCount * 100;
            var sw = Stopwatch.StartNew();
            if (true)
            {

                foreach (var congruence in solver.EnumerateResults(solutions))
                {
                    congruenceCount++;
                    if (congruenceCount == expectedCongruenceChecks)
                    {
                        sw.Stop();
                        break;
                    }
                }
                Console.WriteLine($"EnumerateCongruences took {sw.Elapsed}.");
            }

            congruenceCount = 0;
            //int[] remainders = psolutions.Select(x => 0).ToArray();
            var isExpectedSolution = false;
            BigInteger gcd = 1;
            sw = Stopwatch.StartNew();

            bool checkSolution = false;
            var solveWatch = Stopwatch.StartNew();
            bool isRsaNumber = false;
            foreach (var solution in solver.EnumerateResults(solutions))
            {
                solveWatch.Stop();
                congruenceCount++;
                pctComplete = (double)congruenceCount / solutionCount * 100;


                var step = N;
                long value = (long)solution;

                //validate the solution.

                int solutionStep = 0;
                var test = value;
                if (test == 1) test += step;
                if (isRsaNumber)
                {
                    //if step size is sufficient should only need to test the last value before sqrt(n) for an rsa number;
                    while (test < root)
                    {
                        value = test;
                        test += step;
                    }
                }
                do
                {
                    totalSteps++;
                    solutionStep++;
                    if (true)
                    {

                        gcd = MathUtil.Gcd(n, value);

                        if (gcd > 1 && gcd < n)
                        {
                            sw.Stop();

                            var p = gcd;
                            var q = n / p;
                            var cString = $"()";
                            string message = $"Found divisors {p} * {q} = {n} after {totalSteps.ToString("N0")} total steps in {sw.Elapsed}.\n\t Congruences: {congruenceCount.ToString("N0")} ({pctComplete.ToString("n2")}%) - step {solutionStep} -  system {cString}\n\t Solving took {solveWatch.Elapsed} ";
                            Console.WriteLine(message);
                            if (checkSolution)
                            {
                                break;
                            }
                            else
                            {
                                return (long)p;
                            }

                        }
                    }
                    if (value > root)
                        break;
                    value += step;
                } while (true);

                if (checkSolution)
                {
                    if (isExpectedSolution)
                    {
                        if (gcd == 1 || gcd == n)
                        {
                            string message = "Failed to find factor using expected solution";
                            //Assert.IsFalse(isExpectedSolution, message);

                        }
                        else
                        {
                            break;
                        }
                    }
                }
                solveWatch.Start();
            }
            return (long)n;
        }



        //IEnumerable<List<Congruence>> EnumerateCongruences2(List<List<Congruence>> pList)
        //{
        //    var counts = pList.Select(x => x.Count).ToArray();
        //    var indexes = counts.Select(x => 0).ToArray();
        //    var results = new Congruence[counts.Length];
        //    var arr = pList.Select(x => x.ToArray()).ToArray();
        //    indexes[indexes.Length - 1] = -1;
        //    bool hasMore = true;
        //    while (hasMore)
        //    {
        //        indexes[0]++;
        //        if (indexes[0] == counts[0])
        //        {
        //            indexes[0] = 0;
        //            for (var j = 1; hasMore && j < indexes.Length; j++)
        //            {
        //                indexes[j]++;
        //                if (indexes[j] < counts[j])
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    if (j == counts.Length - 1)
        //                    {
        //                        hasMore = false;
        //                    }
        //                    else
        //                    {
        //                        indexes[j] = 0;
        //                    }
        //                }
        //            }
        //        }
        //    }



        //}

        IEnumerable<List<Congruence>> EnumerateCongruences(List<List<Congruence>> pList)
        {
            List<Congruence> current = new();

            foreach (var value in EnumerateCongruences(0, pList, current))
                yield return value;

        }

        IEnumerable<List<Congruence>> EnumerateCongruences(int start, List<List<Congruence>> pList, List<Congruence> current)
        {
            if (start == pList.Count)
                yield return current;
            else
            {
                var clist = pList[start];
                for (var i = 0; i < clist.Count; i++)
                {
                    current.Add(clist[i]);
                    foreach (var value in EnumerateCongruences(start + 1, pList, current))
                        yield return value;
                    current.Remove(clist[i]);
                }
            }
        }

        IEnumerable<int[]> EnumerateCongruencesArray(List<List<Congruence>> pList)
        {
            int[] current = new int[pList.Count];

            foreach (var value in EnumerateCongruencesArray(0, pList, current))
                yield return value;

        }

        IEnumerable<int[]> EnumerateCongruencesArray(int start, List<List<Congruence>> pList, int[] current)
        {

            var clist = pList[start];
            if (start == pList.Count - 1)
            {
                for (var i = 0; i < clist.Count; i++)
                {
                    current[start] = clist[i].Poly;
                    yield return current;

                }
            }
            else
            {
                for (var i = 0; i < clist.Count; i++)
                {
                    current[start] = clist[i].Poly;
                    foreach (var value in EnumerateCongruencesArray(start + 1, pList, current))
                        yield return value;
                }
            }
        }




        private void SearchCongruences(List<List<Congruence>> pList)
        {
            foreach (var congruence in EnumerateCongruences(pList))
            {
                Console.WriteLine($"Searching: {string.Join(", ", congruence)}");

                bool found = false;
                var cleft = congruence[0];

                for (var i = 0; i < congruence.Count; i++)
                {
                    congruence[i].Value = congruence[i].Poly; // reset to poly+ base*0;
                }
                var leftValue = cleft.MoveNext();
                while (!found)
                {

                    found = true;
                    for (var j = 1; found && j < congruence.Count; j++)
                    {
                        var cRight = congruence[j];
                        var rightValue = cRight.Value;
                        while (rightValue < leftValue)
                        {
                            rightValue = cRight.MoveNext();
                        }
                        if (rightValue % cleft.Base != cleft.Poly)
                        {
                            found = false;
                        }
                        else
                        {
                            string bp = $"Found coungruence {leftValue} == {rightValue} at {rightValue} ";
                            Console.WriteLine(bp);
                            bp = $"{cleft} = {cleft.Poly} + {(cleft.Value - cleft.Poly) / cleft.Base} * {cleft.Base} = {leftValue}";
                            Console.WriteLine(bp);
                            var bp2 = $"{cRight} = {cRight.Poly} + {(cRight.Value - cRight.Poly) / cRight.Base} * {cRight.Base} = {rightValue}";
                            Console.WriteLine(bp2);
                        }

                    }
                    //todo: movenext while(leftValue<rightvalue);
                    leftValue = cleft.MoveNext();
                }
                if (found)
                {
                    string message = "Found Coungrence for all pairs";
                    Console.WriteLine(message);
                    break;
                }

            }
        }



        public class Table
        {
            //public Dictionary<int, Dictionary<int, int>> Table { get; }
            //public int[][] Table { get; }
            public int C;
            public Table(int C)
            {
                this.C = C;
                //var d = new Dictionary<int, Dictionary<int, int>>();
                //int[][] d = new int[C][];
                //for (var i = 0; i < C; i++)
                //{
                //    d[i] = new int[C];// new Dictionary<int, int>();
                //    for (var k = 0; k < C; k++)
                //    {
                //        d[i][k] = i * k % C;
                //    }
                //}
                //this.Table = d;
            }



            public IEnumerable<(int P, int Q, int)> GetPairsViaMod(int n)
            {
                for (var i = 0; i < C; i++)
                {
                    for (var k = 0; k < C; k++)
                    {
                        if (i * k % C == n)
                            yield return (i, k, n);
                    }
                }
            }
            public IEnumerable<(int P, int Q, int)> GetPairs(int n)
            {
                int t;
                for (var i = 1; i < C; i++)
                {
                    t = 0;
                    for (var k = 1; k < C; k++)
                    {
                        t += i;
                        if (t > C) t -= C;
                        if (t == n)
                            yield return (i, k, n);
                    }
                }
            }

            public static Table s10 => new Table(10);
            public static Table s29 => new Table(29);
            public static Table s30 => new Table(30);
            public static Table s31 => new Table(31);
            public static Table s36 => new Table(36);


            public static int[] PrecomputedTableClasses = { 10, 29, 30, 31, 36 };
            public static Dictionary<int, Table> Precomputed => new Dictionary<int, Table>()
            {
                { 10, s10 },
                { 29, s29 },
                { 30, s30 },
                { 31, s31 },
                { 36, s36 }
            };
        }
    }

    [TestClass]
    public class CrtTests
    {
        [TestMethod]
        public void CongruenceCrtMod6()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2), new(2, 3)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 6;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 5;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_30()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 30;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 29;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }


        [TestMethod]
        public void CongruenceCrtMod_30_EnumTest()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 30;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for new(1, 2) , new(2, 3), new (4,5)";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 29;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for new(1, 2) , new(2, 3), new (4,5) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }

            var modulos = congruenceList.Select(x => x.Base).ToArray();
            var remainders = congruenceList.Select(x => x.Poly).ToArray();
            var crtEnum = new MathLib.CrtCongruenceSolver(modulos);
            actualN = crtEnum.ComputeN();
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrtEnumerator failed for new(1, 2) , new(2, 3), new (4,5)";
                Assert.AreEqual(expectedN, actualN);
            }

            actual = crtEnum.Solve(remainders);
            actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for new(1, 2) , new(2, 3), new (4,5)";
                Assert.AreEqual(expectedSolution, actualSolution);
            }

        }

        [TestMethod]
        public void CongruenceCrtMod_30Z()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 30;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 29;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }


        [TestMethod]
        public void CongruenceCrtMod_210()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 210;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 209;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_210Z()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 210;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 209;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }


        [TestMethod]
        public void CongruenceCrtMod_2310()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 2310;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 2309;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }
        [TestMethod]
        public void CongruenceCrtMod_2310Z()
        {
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 2310;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 2309;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_30030()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 30030;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 30029;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }
        [TestMethod]
        public void CongruenceCrtMod_30030Z()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 30030;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 30029;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }



        [TestMethod]
        public void CongruenceCrtMod_510510()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 510510;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 510509;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_510510Z()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 510510;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 510509;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_9_699_690()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 9_699_690;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrt failed for (new(1, 2), new(2, 3) ";
                Assert.AreEqual(expectedN, actualN);
            }
            var expectedSolution = 9_699_690 - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_9_699_690_M1()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 4_849_845;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for {expectedN}";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = expectedN - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for {expectedN} ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_9_699_690_M1Z()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 4_849_845;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for 9_699_690";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = expectedN - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for 9_699_690 ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }


        [TestMethod]
        public void CongruenceCrtMod_9_699_690_M2()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
                x	≡	4	(mod 5)
                x	≡	6	(mod 7)
                x	≡	10	(mod 11)
                x	≡	12	(mod 13)
                x	≡	16	(mod 17)
                x	≡	18	(mod 19)
                x ≡ 1616614 (mod 1616615).
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 1616615;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for {expectedN}";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 1616614;// expectedN - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for {expectedN} ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }


        [TestMethod]
        public void CongruenceCrtMod_9_699_690_M2Z()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
                x	≡	4	(mod 5)
                x	≡	6	(mod 7)
                x	≡	10	(mod 11)
                x	≡	12	(mod 13)
                x	≡	16	(mod 17)
                x	≡	18	(mod 19)
                x ≡ 1616614 (mod 1616615).
            */
            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 1616615;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for {expectedN}";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 1616614;// expectedN - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for {expectedN} ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }

        [TestMethod]
        public void CongruenceCrtMod_9_699_690_M3()
        {

            var congruenceList = new List<Congruence>
            {
                new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
                x	≡	6	(mod 7)
                x	≡	10	(mod 11)
                x	≡	12	(mod 13)
                x	≡	16	(mod 17)
                x	≡	18	(mod 19)
                x ≡ 323322 (mod 323323)
            */
            var actual = MathLib.CRT(congruenceList);
            var expectedN = 323323;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for {expectedN}";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 323322;// expectedN - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for {expectedN} ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }


        [TestMethod]
        public void CongruenceCrtMod_9_699_690_M3Z()
        {
            var congruenceList = new List<Congruence>
            {
                new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
                x	≡	6	(mod 7)
                x	≡	10	(mod 11)
                x	≡	12	(mod 13)
                x	≡	16	(mod 17)
                x	≡	18	(mod 19)
                x ≡ 323322 (mod 323323)
            */
            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 323323;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for {expectedN}";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 323322;// expectedN - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for {expectedN} ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }



        [TestMethod]
        public void CongruenceCrtMod_9_699_690_Z()
        {
            //x ≡ 30029 (mod 30030).
            var congruenceList = new List<Congruence>
            {
                new(1, 2) , new(2, 3), new (4,5), new (6,7), new (10,11), new (12, 13),  new (16, 17), new(18,19)
            };
            /*
             x	≡	1	(mod 2)
            x	≡	2	(mod 3)
            x ≡ 5 (mod 6).
            */
            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 9_699_690;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed for 9_699_690";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 9_699_690 - 1;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for 9_699_690 ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }
        }


        [TestMethod]
        public void CongruenceCrt_01()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");

            /*
                x	≡	4	(mod 31)
                x	≡	15	(mod 29)
                x	≡	35	(mod 36)
                x	≡	34	(mod 37)
                x	≡	7	(mod 41)
                x	≡	3	(mod 43)
                x ≡ 159146099 (mod 2111136084)
             * */

            var congruenceList = new List<Congruence> {
                {new(4, 31) }, new(15, 29),  new (35, 36), new(34,37), new (7, 41), new (3, 43)
            };

            // as int x ≡ 624 (mod 899)., as GmpInt x ≡ 421 (mod 899).

            //Assert.ThrowsException<OverflowException>(() => MathLib.CRT(congruenceList));
            if (true)
            {
                var actual = MathLib.CRT(congruenceList);



                var expectedN = 2111136084;
                var actualN = actual.N;
                if (actualN != expectedN)
                {
                    string message = $"N failed";
                    Assert.AreEqual(expectedN, actualN, message);
                }
                var expectedSolution = 159146099;
                var actualSolution = actual.Solution;

                // solution fails due to overflow error.
                if (actualSolution != expectedSolution)
                {
                    string message = $"Solution failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                    Assert.AreEqual(expectedSolution, actualSolution, message);
                }
            }
        }





        [TestMethod]
        public void CongruenceCrt_01_Enum()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");

            /*
                x	≡	4	(mod 31)
                x	≡	15	(mod 29)
                x	≡	35	(mod 36)
                x	≡	34	(mod 37)
                x	≡	7	(mod 41)
                x	≡	3	(mod 43)
                x ≡ 159146099 (mod 2111136084)
             * */

            var congruenceList = new List<Congruence> {
                {new(4, 31) }, new(15, 29),  new (35, 36), new(34,37), new (7, 41), new (3, 43)
            };


            var actual = MathLib.CRT(congruenceList);

            var expectedN = 2111136084;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed";
                Assert.AreEqual(expectedN, actualN, message);
            }

            var expectedSolution = 159146099;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }

            var modulos = congruenceList.Select(x => x.Base).ToArray();
            var remainders = congruenceList.Select(x => x.Poly).ToArray();
            var crtEnum = new MathLib.CrtCongruenceSolver(modulos);
            actualN = crtEnum.ComputeN();
            if (actualN != expectedN)
            {
                string bp = $"CongruenceCrtEnumerator failed for new(1, 2) , new(2, 3), new (4,5)";
                Assert.AreEqual(expectedN, actualN);
            }

            actual = crtEnum.Solve(remainders);
            actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string bp = $"CongruenceCrt failed for new(1, 2) , new(2, 3), new (4,5)";
                Assert.AreEqual(expectedSolution, actualSolution);
            }
        }



        [TestMethod]
        public void CongruenceCrt_01Z()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");

            /*
                x	≡	4	(mod 31)
                x	≡	15	(mod 29)
                x	≡	35	(mod 36)
                x	≡	34	(mod 37)
                x	≡	7	(mod 41)
                x	≡	3	(mod 43)
                x ≡ 159146099 (mod 2111136084)
             * */

            var congruenceList = new List<Congruence> {
                {new(4, 31) }, new(15, 29),  new (35, 36), new(34,37), new (7, 41), new (3, 43)
            };

            // as int x ≡ 624 (mod 899)., as GmpInt x ≡ 421 (mod 899).

            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 2111136084;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 159146099;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }

        }

        [TestMethod]
        public void CongruenceCrt_01ZDebug()
        {
            GmpInt rsa65 = "25063116026386232513";
            Console.WriteLine($"n = {rsa65}");

            /*
                x	≡	4	(mod 31)
                x	≡	15	(mod 29)
                x	≡	35	(mod 36)
                x	≡	34	(mod 37)
                x	≡	7	(mod 41)
                x	≡	3	(mod 43)
                x ≡ 159146099 (mod 2111136084)
             * */

            var congruenceList = new List<Congruence> {
                {new(4, 31) }, new(15, 29),  new (35, 36), new(34,37), new (7, 41), new (3, 43)
            };

            // as int x ≡ 624 (mod 899)., as GmpInt x ≡ 421 (mod 899).

            var actual = MathLib.CRTZ(congruenceList);
            var expectedN = 2111136084;
            var actualN = actual.N;
            if (actualN != expectedN)
            {
                string message = $"N failed";
                Assert.AreEqual(expectedN, actualN, message);
            }
            var expectedSolution = 159146099;
            var actualSolution = actual.Solution;
            if (actualSolution != expectedSolution)
            {
                string message = $"Solution failed for (new(4, 31), new(15, 19), new (35, 36), new(34,37), new (7, 41), new (3, 43)) ";
                Assert.AreEqual(expectedSolution, actualSolution, message);
            }

        }


    }

    [TestClass]
    public class EnumerationTests
    {
        [TestMethod]
        public void TestJaggedRecursiveWhile()
        {
            var l = new List<List<int>>
            {
                new(){0},
                new(){0,1},
                new(){0,1,2},
                new(){0,1,2,3},
                new(){0,1,2,3,4},
                new(){0,1,2,3,4,5},
                new(){0,1,2,3,4,5,6},
                new(){0,1,2,3,4,5,6,7},
                new(){0,1,2,3,4,5,6,7,8},
                new(){0,1,2,3,4,5,6,7,8,9},
            };

            int elements = l.Count;
            var indexes = new int[elements];
            //var values = new int[elements];

            int outerIndex = 0;
            bool didWork = false;
            int count = 0;
            bool showWork = false;
            var sw = Stopwatch.StartNew();
            while (true)
            {
                didWork = false;
                //values[outerIndex] = l[outerIndex][indexes[outerIndex]];
                if (outerIndex == elements - 1)
                {
                    didWork = true;
                    count++;
                    // Do work with values
                    if (showWork)
                    {
                        Console.WriteLine($"Values: [{string.Join(", ", indexes)}]");
                    }

                }

                int start = outerIndex;
                while (start < elements)
                {
                    //var startValue = values[start - 1];
                    ref int idx = ref indexes[start];
                    var innerList = l[start];
                    //values[start] = innerList[idx++];
                    if (idx == innerList.Count)
                    {
                        idx = 0;
                        start--;
                    }
                    else
                    {
                        start++;
                    }
                }

                outerIndex = elements - 1;
                while (outerIndex >= 0 && indexes[outerIndex] == l[outerIndex].Count - 1)
                {
                    indexes[outerIndex] = 0;
                    outerIndex--;
                }

                if (outerIndex < 0)
                {
                    break;
                }
                if (!didWork && outerIndex == elements - 1)
                {
                    count++;
                    // Do work with values
                    if (showWork)
                    {
                        Console.WriteLine($"Values: [{string.Join(", ", indexes)}]");
                    }
                }

                indexes[outerIndex]++;
            }
            sw.Stop();
            Console.WriteLine($"Enumerated {count} items in {sw.Elapsed}.");
            var expected = 3628800;
            if (expected != count)
            {
                var message = "Expected 720 items.";
                Assert.AreEqual(expected, count, message);
            }
        }

        [TestMethod]
        public void TestJaggedRecursiveInline()
        {

            var l = new List<List<int>>
            {
                new(){0},
                new(){0,1},
                new(){0,1,2},
                new(){0,1,2,3},
                new(){0,1,2,3,4},
                new(){0,1,2,3,4,5},
                new(){0,1,2,3,4,5,6},
                new(){0,1,2,3,4,5,6,7},
                new(){0,1,2,3,4,5,6,7,8},
                new(){0,1,2,3,4,5,6,7,8,9},
            };


            int elements = l.Count;
            var indexes = new int[elements];


            bool hasMore = true;
            var len = elements - 1;
            var outerCount = l[len].Count;
            int count = 0;
            bool showWork = false;
            var sw = Stopwatch.StartNew();
            while (hasMore)
            {
                // loop throw all words in the outer index using ref int;
                ref int idx = ref indexes[len];
                for (; idx < outerCount; idx++)
                {
                    count++;
                    if (showWork)
                    {
                        Console.WriteLine($"Values: [{string.Join(", ", indexes)}]");
                    }
                }

                //reset ref idx = ref indexes[len] to zero;
                idx = 0;

                //borrow for the overflow starting at outer-1 to 0;
                for (var start = len - 1; hasMore && start >= 0; start--)
                {
                    ref int a = ref indexes[start];
                    a++;
                    if (a < l[start].Count) // borrow accounted for
                        break;
                    else if (start > 0) // need to borrow more
                        a = 0;
                    else // nothing left to borrow
                        hasMore = false;
                }
            }
            sw.Stop();
            Console.WriteLine($"Enumerated {count} items inline ref in {sw.Elapsed}.");
            var expected = 3628800;
            var actual = count;
            if (expected != count)
            {
                var message = "Expected 720 items.";
                Assert.AreEqual(expected, count, message);
            }
        }

        [TestMethod]
        public void TestJaggedEnumeratorInline()
        {

            var l = new List<List<int>>
            {
                new(){0},
                new(){0,1},
                new(){0,1,2},
                new(){0,1,2,3},
                new(){0,1,2,3,4},
                new(){0,1,2,3,4,5},
                new(){0,1,2,3,4,5,6},
                new(){0,1,2,3,4,5,6,7},
                new(){0,1,2,3,4,5,6,7,8},
                new(){0,1,2,3,4,5,6,7,8,9},
            };


            int elements = l.Count;
            var indexes = new int[elements];
            var counts = l.Select(x => x.Count - 1).ToArray();

            bool hasMore = true;
            var len = elements - 1;
            var outerCount = l[len].Count;
            int count = 0;
            bool showWork = false;
            var sw = Stopwatch.StartNew();
            while (hasMore)
            {
                // loop throw all words in the outer index using ref int;
                int idx = indexes[len];
                for (; idx < outerCount; idx++)
                {
                    // last index (indexes[len]) only being set for testing purposes.
                    indexes[len] = idx;
                    count++;
                    if (showWork)
                    {
                        Console.WriteLine($"Values: [{string.Join(", ", indexes)}]");
                    }
                }

                // last index (indexes[len]) only being set for testing purposes.
                indexes[len] = 0;


                //borrow for the overflow starting at outer-1 to 0;
                for (var start = len - 1; hasMore && start >= 0; start--)
                {
                    idx = indexes[start]++;
                    if (idx < counts[start]) // borrow accounted for
                        break;
                    else if (start > 0) // need to borrow more
                        indexes[start] = 0;
                    else // nothing left to borrow
                        hasMore = false;
                }
            }
            sw.Stop();
            Console.WriteLine($"Enumerated {count} items inline no ref in {sw.Elapsed}.");
            var expected = 3628800;
            var actual = count;
            if (expected != count)
            {
                var message = "Expected 720 items.";
                Assert.AreEqual(expected, count, message);
            }
        }


    }
}