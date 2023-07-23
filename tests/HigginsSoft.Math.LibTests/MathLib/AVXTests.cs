#define SKIP_LONG_TESTS

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using MathGmp.Native;
using System.Diagnostics;
using static HigginsSoft.Math.Lib.Tests.AVXTests;
using System.Runtime.InteropServices;
using System.Collections;
using BenchmarkDotNet.Disassemblers;
using System.ComponentModel.DataAnnotations;
using BenchmarkDotNet.Engines;
using System.Buffers.Text;
using HigginsSoft.Math.Lib;
using static HigginsSoft.Math.Lib.MathLib.Vect;
using HigginsSoft.Math.Demos;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Security.Cryptography;

namespace HigginsSoft.Math.Lib.Tests
{
    [TestClass]
    public class AVXTests
    {
        [TestMethod]
        public void AvxDivRemTest()
        {
            int expected, count;

            int[] divisors = { 2, 3, 5, 7, 11, 13, 17, 19 }; // Set of small primes
            Vector256<double> quotient, remainder;
            string expectedQuotient, expectedRemainder;


            // set up: set n to 2^6 * 3 * 643
            var n = 123456;
            expected = 2;
            expectedQuotient = "<61728, 41152, 24691, 17636>";
            expectedRemainder = "<0, 0, 1, 4>";

            // act
            count = AvxDivRemDemo(n, divisors, out quotient, out remainder);

            // assert
            Assert.AreEqual(count, expected);
            Assert.AreEqual(expectedQuotient, quotient.ToString());
            Assert.AreEqual(expectedRemainder, remainder.ToString());


            // set up: set n = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19 
            n = divisors.Aggregate(1, (a, b) => a * b);
            expected = 4;
            expectedQuotient = "<4849845, 3233230, 1939938, 1385670>";
            expectedRemainder = "<0, 0, 0, 0>";

            // act
            count = AvxDivRemDemo(n, divisors, out quotient, out remainder);

            // assert
            Assert.AreEqual(count, expected);
            Assert.AreEqual(expectedQuotient, quotient.ToString());
            Assert.AreEqual(expectedRemainder, remainder.ToString());
        }

        void TestQs16AvxRsaTest(Qs16Avx qs, int bits, TimeSpan? maxTimeout)
        {
            Stopwatch swOp;
            Stopwatch swWhole;
            bool isPrp;
            FactorizationInt64 f;
            Factorization fBig;

            if (maxTimeout is null)
                maxTimeout = TimeSpan.FromSeconds(1);
            Console.WriteLine($" =========== QS RSA({bits}) =========== ");

            swWhole = Stopwatch.StartNew();
            swOp = Stopwatch.StartNew();
            GmpInt rsa = MathLib.Rsa(bits, maxTimeout);
            swOp.Stop();

            Console.WriteLine($" -> Generate Rsa({bits}) took {swOp.Elapsed}");


            swOp = Stopwatch.StartNew();
            isPrp = rsa.IsProbablePrime();
            swOp.Stop();

            Assert.AreEqual(false, isPrp);
            Console.WriteLine($"Rsa({bits}) = {rsa} - PrpCheck took {swOp.Elapsed})");


            swOp = Stopwatch.StartNew();
            f = qs.FactorIntUnchecked((long)rsa, maxTimeout);
            swOp.Stop();

            swWhole.Stop();
            Console.WriteLine($"Rsa({bits}) took {swWhole.Elapsed} with {qs.PrimeCount} primes");

            swOp = Stopwatch.StartNew();
            fBig = Factorization.Factor(rsa, false);
            swOp.Stop();
            Console.WriteLine($"Rsa({bits}) = {fBig} ({swOp.Elapsed})");
            Console.WriteLine($" -> Timings: {fBig.Timings}");
            Console.WriteLine();
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void TestQs16AvxRsa63Test()
        {
            var qs = new Qs16Avx();
            qs.PrimeCount = 128;
            TimeSpan maxTimeout = TimeSpan.FromSeconds(3);
            TestQs16AvxRsaTest(qs, 63, maxTimeout);
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void TestQs16AvxRsaTest()
        {

            var qs = new Qs16Avx();
            qs.PrimeCount = 128;
            TimeSpan maxTimeout = TimeSpan.FromSeconds(3);

            int bits = 36;

         
            for (; bits <= 54; bits += 4)
            {
                TestQs16AvxRsaTest(qs, bits, maxTimeout);
            }


            for (; bits <= 60; bits += 1)
            {
                TestQs16AvxRsaTest(qs, bits, maxTimeout);
            }

        }
        public static bool ExecuteWithTimeout(Func<Task> operation, TimeSpan timeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                // Create a task that completes after the specified timeout duration
                var timeoutTask = Task.Delay(timeout, cancellationTokenSource.Token);

                // Start the operation task
                var operationTask = operation();

                // Wait for either the operation task or the timeout task to complete
                var completedTask = Task.WhenAny(operationTask, timeoutTask).GetAwaiter().GetResult();

                // Cancel the timeout task if it's still running
                cancellationTokenSource.Cancel();

                // Determine the result based on the completed task
                if (completedTask == operationTask)
                {
                    // The operation completed within the timeout
                    return true;
                }
                else
                {
                    // The operation timed out
                    return false;
                }
            }
        }
#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void TestQS16Avx()
        {
            var limit = 1 << 16;
            var composites = Composites.GenerateTo(limit).ToArray();

            int count = 0;
            int completed = 0;
            var sw = new Stopwatch();
            FactStruct result = Qs16Avx.DefaultFactStruct;
            var qs = new Qs16Avx();
            sw.Start();

            /*
             *   Qs16Avx factstr found 487 divisors in 58992 composites below 65536 in 00:00:00.1742493
                Qs16Avx factint found 848 divisors in 58992 composites below 65536 in 00:00:00.0198002
            */
            foreach (var composite in composites)
            {
                count++;
                var fact = qs.FactorUnchecked(composite, result);
                if (fact.Length > 0)
                    completed++;

            }
            sw.Stop();
            Console.WriteLine($"Qs16Avx factstr found {completed} divisors in {composites.Length} composites below {limit} in {sw.Elapsed}");




            sw.Restart();


            foreach (var composite in composites)
            {
                count++;
                var fact = qs.FactorIntUnchecked(composite);
                if (fact.Factors.Count > 0)
                    completed++;

            }
            sw.Stop();
            Console.WriteLine($"Qs16Avx factint found {completed} divisors in {composites.Length} composites below {limit} in {sw.Elapsed}");


;
        }

        [TestMethod]
        public void AvxDivRemBatchTest()
        {
            int expected, count;

            int[] divisors = { 2, 3, 5, 7, 11, 13, 17, 19 }; // Set of small primes
            double[] quotients = new double[divisors.Length];
            double[] remainders = new double[divisors.Length];

            string expectedQuotient, expectedRemainder, actualQuotient, actualRemainder;

            Func<double[], string> AsVectorString = x =>
            {
                var result = $"<{string.Join(", ", x)}>";
                return result;
            };




            // arrange
            var n = 123456;

            // act
            count = AvxDivRem(n, divisors, ref quotients, ref remainders);

            // assert

            expected = 2;
            expectedQuotient = "<61728, 41152, 24691, 17636, 11223, 9496, 7262, 6497>";
            expectedRemainder = "<0, 0, 1, 4, 3, 8, 2, 13>";
            actualQuotient = AsVectorString(quotients);
            actualRemainder = AsVectorString(remainders);

            Assert.AreEqual(count, expected);
            Assert.AreEqual(expectedQuotient, actualQuotient);
            Assert.AreEqual(expectedRemainder, actualRemainder);




            // arrange
            // set n = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19 
            n = divisors.Aggregate(1, (a, b) => a * b);


            // act
            count = AvxDivRem(n, divisors, ref quotients, ref remainders);

            // assert
            expected = 8;
            expectedQuotient = "<4849845, 3233230, 1939938, 1385670, 881790, 746130, 570570, 510510>";
            expectedRemainder = "<0, 0, 0, 0, 0, 0, 0, 0>";
            actualQuotient = AsVectorString(quotients);
            actualRemainder = AsVectorString(remainders);


            Assert.AreEqual(count, expected);
            Assert.AreEqual(expectedQuotient, actualQuotient);
            Assert.AreEqual(expectedRemainder, actualRemainder);
        }


        [TestMethod]
        public void AvxDivRemBatchTimeTest()
        {
            int expected, count = 0;

            int[] divisors = { 2, 3, 5, 7, 11, 13, 17, 19 }; // Set of small primes
            double[] quotients = new double[divisors.Length];
            double[] remainders = new double[divisors.Length];
            double[] d_divisors = divisors.Select(x => (double)x).ToArray();

            // get the reciprocals of our divisors so we can divide faster using multiplication
            double[] d_reciprocals = divisors.Select(x => 1.0 / x).ToArray();

            MathLib.Align(ref d_divisors);
            MathLib.Align(ref d_reciprocals);
            MathLib.Align(ref quotients);
            MathLib.Align(ref remainders);


            var limit = 1 << 16;
            var composites = Composites.GenerateTo(limit).ToArray();
            int divMask = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];

                count += AvxDivRemBatched(composite, d_divisors, d_reciprocals, ref quotients, ref remainders);
            }
            sw.Stop();

            Console.WriteLine($"Avx Trial Divide found {count} divisors in {composites.Length} composites below {limit} in {sw.Elapsed}");

            expected = 95375;
            Assert.AreEqual(expected, count);
        }


        const int DivRemPow2Limit = 16;
        const int DivRemTestLimit = 1 << DivRemPow2Limit;
        [TestMethod]
        public void AvxDivRemData64TimeTest()
        {
            int expected, count = 0;
            int[] divisors = { 2, 3, 5, 7, 11, 13, 17, 19 };
            var data = new AvxDivRemData64(divisors);

            var limit = DivRemTestLimit;
            var composites = Composites.GenerateTo(limit).ToArray();

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                count += AvxDivRem(composite, data);
            }
            sw.Stop();

            Console.WriteLine($"Avx Trial Divide found {count} divisors in {composites.Length} composites below {limit} in {sw.Elapsed}");

            if (limit == 65536)
            {
                expected = 95375;
                Assert.AreEqual(expected, count);
            }

        }

        [TestMethod]
        public void AvxDivRemData32TimeTest()
        {
            int expected, count = 0;
            int[] divisors = { 2, 3, 5, 7, 11, 13, 17, 19 };
            var data = AvxDivRemData32.Create(divisors);

            var limit = DivRemTestLimit;
            var composites = Composites.GenerateTo(limit).ToArray();

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                count += AvxDivRem(composite, data);
            }
            sw.Stop();

            Console.WriteLine($"Avx Trial Divide found {count} divisors in {composites.Length} composites below {limit} in {sw.Elapsed}");

            if (limit == 65536)
            {
                expected = 95375;
                Assert.AreEqual(expected, count);
            }
        }

        [TestMethod]
        public void TestIsBSmooth_8_Divisors()
        {
            var data = AvxDivRemData32_8_Divisors.CreateSmallPrimesData();
            bool actual, expected;

            expected = true;
            var limit = DivRemTestLimit;
            limit = 65536;
            var composites = Composites.GenerateTo(limit).ToArray();
            var sw = Stopwatch.StartNew();
            var maxAssert = limit;// data.MaxDivisor;
            int count = 0;
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data);
                expected = MathLib.IsBSmooth(composite, data.Divisors, data.MaxDivisor);
                if (i < maxAssert)
                {
                    if (actual != expected)
                    {
                        string message = $"BSmooth failed for {composite}";
                        Assert.AreEqual(actual, expected, message);
                    }
                }
                else
                {
                    // string bp = "";
                }
                if (actual)
                    count++;
            }
            sw.Stop();

            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 8 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestIsBSmooth_32()
        {
            var data = AvxDivRemData32.Create(Primes.IntFactorPrimes.Take(16).ToArray());
            bool actual, expected;

            expected = true;
            var limit = DivRemTestLimit;
            //limit = 65536;
            var composites = Composites.GenerateTo(limit).ToArray();
            var sw = Stopwatch.StartNew();
            var maxAssert = limit;// data.MaxDivisor;
            int count = 0;
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data);
                expected = MathLib.IsBSmooth(composite, data.Divisors, data.MaxDivisor);
                if (i < maxAssert)
                {
                    if (actual != expected)
                    {
                        string message = $"BSmooth failed for {composite}";
                        Assert.AreEqual(actual, expected, message);
                    }
                }
                else
                {
                    // string bp = "";
                }
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestIsBSmooth_32_Timed()
        {
            var primes = Primes.IntFactorPrimes.Take(16).ToArray();
            var data = AvxDivRemData32.Create(primes);
            bool actual;


            var limit = DivRemTestLimit;

            var composites = Composites.GenerateTo(limit).ToArray();


            int count = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data);
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestIsBSmooth_64_Timed()
        {
            var primes = Primes.IntFactorPrimes.Take(16).ToArray();
            var data = AvxDivRemData64.Create(primes);
            bool actual;
            var limit = DivRemTestLimit;

            var composites = Composites.GenerateTo(limit).ToArray();

            int count = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data);
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestIsBSmooth_64()
        {
            var data = AvxDivRemData64.Create(Primes.IntFactorPrimes.Take(16).ToArray());
            bool actual, expected;

            expected = true;
            var limit = DivRemTestLimit;
            //limit = 65536;
            var composites = Composites.GenerateTo(limit).ToArray();
            var sw = Stopwatch.StartNew();
            var maxAssert = limit;// data.MaxDivisor;
            int count = 0;
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data);
                expected = MathLib.IsBSmooth(composite, data.Divisors, data.MaxDivisor);
                if (i < maxAssert)
                {
                    if (actual != expected)
                    {
                        string message = $"BSmooth failed for {composite}";
                        Assert.AreEqual(actual, expected, message);
                    }
                }
                else
                {
                    // string bp = "";
                }
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestIsBSmooth_64_WithMask()
        {
            var data = AvxDivRemData64.Create(Primes.IntFactorPrimes.Take(16).ToArray());
            bool actual, expected;

            expected = true;
            var limit = DivRemTestLimit;
            //limit = 65536;
            var composites = Composites.GenerateTo(limit).ToArray();

            var maxAssert = limit;// data.MaxDivisor;
            int count = 0;
            int actualMask = 0;
            int expectedMask = 0;
            int factorBaseMask = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data, out factorBaseMask, out actualMask);
                expected = MathLib.IsBSmooth(composite, data.Divisors, data.MaxDivisor, out expectedMask);
                if (i < maxAssert)
                {
                    if (actual != expected)
                    {
                        string message = $"BSmooth failed for {composite}";
                        Assert.AreEqual(expected, actual, message);
                    }
                    if (actualMask != expectedMask)
                    {
                        var actualBits = ((GmpInt)actualMask).ToString(2).PadLeft(data.Divisors.Length, '0');
                        var expectedBits = ((GmpInt)expectedMask).ToString(2).PadLeft(data.Divisors.Length, '0');

                        string message = $"BSmooth mask failed for {composite} - Actual {actualBits} - Excpected {expectedBits}";

                        Assert.AreEqual(expectedBits, actualBits, message);
                    }
                }
                else
                {
                    // string bp = "";
                }
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestIsBSmooth_64_WithMask_Timed()
        {
            var data = AvxDivRemData64.Create(Primes.IntFactorPrimes.Take(16).ToArray());
            bool actual, expected;


            var limit = DivRemTestLimit;
            //limit = 65536;
            var composites = Composites.GenerateTo(limit).ToArray();

            var maxAssert = limit;// data.MaxDivisor;
            int count = 0;
            int actualMask = 0;
            int factorBaseMask = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data, out factorBaseMask, out actualMask);
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }
#if SKIP_LONG_TESTS
        [Ignore]
#else
        [Ignore]
#endif
        [TestMethod]
        public void TestIsBSmooth_64_P30_to_31()
        {
            var data = AvxDivRemData64.Create(Primes.IntFactorPrimes.Take(16).ToArray());
            bool actual, expected;

            expected = true;
            var limit = DivRemTestLimit;
            //limit = 65536;

            var composites = Composites.GenerateTo(int.MaxValue).Skip(1 << 30).ToArray();
            var sw = Stopwatch.StartNew();
            var maxAssert = limit;// data.MaxDivisor;
            int count = 0;
            for (var i = 0; i < composites.Length; i++)
            {
                var composite = composites[i];
                actual = IsBSmooth(composite, data);
                expected = MathLib.IsBSmooth(composite, data.Divisors, data.MaxDivisor);
                if (i < maxAssert)
                {
                    if (actual != expected)
                    {
                        string message = $"BSmooth failed for {composite}";
                        Assert.AreEqual(actual, expected, message);
                    }
                }
                else
                {
                    // string bp = "";
                }
                if (actual)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Found {count} b-smooth numbers in {composites.Length} composites below {limit} using 16 primes in {sw.Elapsed}");

        }

        [TestMethod]
        public void TestSquareGenerator()
        {
            int start = 0, count = 0, expected = 0, actual = 0;
            SquareGenerator gen;

            gen = new SquareGenerator();
            var iter = gen.GetEnumerator();
            while (iter.MoveNext() && iter.Current < 50)
            {
                count++;
                expected = count * count;
                actual = iter.Current;
                Assert.AreEqual(expected, actual);
            }


            expected = 7;
            Assert.AreEqual(expected, count, $"There are y squares less than 50");


            start = (SquareGenerator.MaxSquareRoot - 2);
            start *= start;
            gen = new SquareGenerator(start);
            actual = gen.Count();
            expected = 3;
            Assert.AreEqual(expected, actual, $"There are 3 squares between 2,147,210,244 and 2_147_395_600");
        }


        [TestMethod]
        public void TestSquaresAvxGenerate()
        {

            int start = 0, count = 0, expected = 0, actual = 0, len = Vector<int>.Count;
            IEnumerable<Vector256<int>> gen;

            gen = GenerateSquares(0);
            var iter = gen.GetEnumerator();


            while (actual < 24 * 24 && iter.MoveNext())
            {
                var current = iter.Current;
                string bp = "vssucks";
                for (var i = 0; i < len; i++)
                {
                    count++;

                    actual = current.GetElement(i);
                    expected = count * count;
                    Assert.AreEqual(expected, actual);

                }
            }


            expected = 24;
            Assert.AreEqual(expected, count, $"There are {expected} squares less than {expected * expected}");


            start = (SquareGenerator.MaxSquareRoot - 2);
            start *= start;
            gen = GenerateSquares(start);
            actual = gen.Count();
            expected = 0;
            Assert.AreEqual(expected, actual, $"Avx generator can only generate squares up to 46,336^2 ");


            start = 46328;
            start *= start;
            gen = GenerateSquares(start);
            actual = gen.Count();
            expected = 1;
            Assert.AreEqual(expected, actual, $"Avx generator can only generate squares up to 46,336^2 ");

            gen = GenerateSquares(start);
            iter = gen.GetEnumerator();
            iter.MoveNext();
            count = 46328;
            for (var i = 0; i < len; i++)
            {
                actual = iter.Current.GetElement(i);
                expected = count * count;
                Assert.AreEqual(expected, actual);
                count++;

            }
        }

        [TestMethod]
        public void TestSquaresEnumerateBlock2()
        {
            EnumerateBlock2();

        }

        [TestMethod]
        public void TestSquaresEnumerateBlock4()
        {
            EnumerateBlock4();
        }

        [TestMethod]
        public void TestSquaresEnumerateBlock8()
        {
            EnumerateBlock8();
        }

        [TestMethod]
        public void DivByteVect()
        {
            var bytePrimes = Primes.IntFactorPrimes.Take(32).Select(x => (byte)x).ToArray();
            byte[] result = bytePrimes.Select(x => (byte)0).ToArray();
            byte[] products = result.ToArray();


            var divs = new Vector<byte>(bytePrimes);
            var vectN = new Vector<byte>(6);


            var bigN = vectN.AsVector256();
            var bigDiv = divs.AsVector256();


            var res = vectN / divs;
            var prod = res * divs;

            res.CopyTo(result);
            prod.CopyTo(products);
            var big = prod.AsVector256<byte>();
            var comp = Avx2.CompareEqual(big, bigN);
            var mask = Avx2.MoveMask(comp);

            var len = Vector<byte>.Count;
            Assert.AreEqual(32, len);
            Assert.AreEqual(3, mask);//6 is divisible by primes[0] and primes[1];
        }

        [TestMethod]
        public void TestSquareGeneratorAvx()
        {
            var gen = new SquareGeneratorAvx();

            bool moved;
            int count = 0, expected, actual;
            for (var j = 0; j < 4; j++)
            {
                moved = gen.MoveNext();
                Assert.AreEqual(true, moved);

                var vector = gen.Current;
                for (var i = 0; i < SquareGeneratorAvx.Count; i++)
                {
                    count++;
                    expected = count * count;
                    actual = vector.GetElement(i);
                    Assert.AreEqual(expected, actual);
                }
            }

            gen = new SquareGeneratorAvx();
            count = 0;

            moved = true;
            var sw = Stopwatch.StartNew();
            while (gen.MoveNext())
            {
                count += SquareGeneratorAvx.Count;
            }
            sw.Stop();
            Console.WriteLine($"Avx generated {count} squares in {sw.Elapsed}");
            expected = 46336;
            Assert.AreEqual(expected, count);

            sw.Restart();
            count = 0;
            for (var i = 0; i <= 46336; i++)
            {
                var sq = i * i;
                if (sq > 0)
                    count++;
            }
            sw.Stop();
            Console.WriteLine($"Mul generated {count} squares in {sw.Elapsed}");


            gen = new SquareGeneratorAvx(4);
            gen.MoveNext();
            Assert.AreEqual(4, gen.Current.GetElement(0));      // (2+0)^2
            Assert.AreEqual(81, gen.Current.GetElement(7));     // (2+7)^2
            gen.MoveNext();
            Assert.AreEqual(100, gen.Current.GetElement(0));    // (2+8)^2
            Assert.AreEqual(289, gen.Current.GetElement(7));    // (2+15)^2
            gen.MoveNext();
            Assert.AreEqual(324, gen.Current.GetElement(0));    // (2+16)^2

            for (var i = 5; i < 9; i++)
            {
                gen = new SquareGeneratorAvx(i);
                gen.MoveNext();
                Assert.AreEqual(9, gen.Current.GetElement(0));      // (3+0)^2
                Assert.AreEqual(100, gen.Current.GetElement(7));    // (3+7)^2
                gen.MoveNext();
                Assert.AreEqual(121, gen.Current.GetElement(0));    // (3+8)^2
                Assert.AreEqual(324, gen.Current.GetElement(7));    // (3+15)^2
                gen.MoveNext();
                Assert.AreEqual(361, gen.Current.GetElement(0));    // (3+16)^2
            }
        }

        void EnumerateBlock2()
        {
            int[] sq = new[] { 1, 4, 0, 0, 0, 0, 0, 0 };
            //seed initial deltas, incrementing by 2*2 (blockSize*2)
            var delta = new Vector<int>(new[] { 8, 12, 0, 0, 0, 0, 0, 0 });
            //seed delta incremements as 2*2*2 (blockSize*blocksize*2)
            var inc = new Vector<int>(new[] { 8, 8, 0, 0, 0, 0, 0, 0 });

            Span<int> buffer = sq.AsSpan();
            var batchSize = Vector<int>.Count;
            for (var i = 0; i < buffer.Length; i++)
            {
                var currentSlice = buffer.Slice(0, batchSize);
                Console.WriteLine($"Step: {i} a - {string.Join(", ", currentSlice.ToArray().Take(2))}");
                var v = new Vector<int>(currentSlice);
                v = v + delta;
                delta = delta + inc;

                v.CopyTo(currentSlice);
                //Console.WriteLine($"    => Step: {i} b - {string.Join(", ", currentSlice.ToArray().Take(2))}");
            }

            // return spanBack;
        }

        void EnumerateBlock4()
        {
            int[] sq = new[] { 1, 4, 9, 16, 0, 0, 0, 0 };
            //seed initial deltas, incrementing by 4*2 (blockSize*2)
            var delta = new Vector<int>(new[] { 24, 32, 40, 48, 0, 0, 0, 0 });
            //seed delta incremements as 4*4*2 (blockSize*blocksize*2)
            var inc = new Vector<int>(new[] { 32, 32, 32, 32, 0, 0, 0, 0 });

            Span<int> buffer = sq.AsSpan();
            var batchSize = Vector<int>.Count;
            for (var i = 0; i < buffer.Length; i++)
            {
                var currentSlice = buffer.Slice(0, batchSize);
                Console.WriteLine($"Step: {i} a - {string.Join(", ", currentSlice.ToArray().Take(4))}");
                var v = new Vector<int>(currentSlice);
                v = v + delta;
                delta = delta + inc;

                v.CopyTo(currentSlice);
            }
        }

        void EnumerateBlock8()
        {
            int[] sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
            //seed initial deltas, incrementing by 8*2 (blockSize*2)
            var delta = new Vector<int>(new[] { 80, 96, 112, 128, 144, 160, 176, 192 });
            //seed delta incremements as 8*8*2 (blockSize*blocksize*2)
            var inc = new Vector<int>(new[] { 128, 128, 128, 128, 128, 128, 128, 128 });

            Span<int> buffer = sq.AsSpan();
            var batchSize = Vector<int>.Count;
            for (var i = 0; i < buffer.Length; i++)
            {
                var currentSlice = buffer.Slice(0, batchSize);
                Console.WriteLine($"Step: {i} a - {string.Join(", ", currentSlice.ToArray().Take(8))}");
                var v = new Vector<int>(currentSlice);
                v = v + delta;
                delta = delta + inc;

                v.CopyTo(currentSlice);
            }
        }



        public IEnumerable<Vector256<int>> GenerateSquares()
        {
            Vector<int> baseV;
            Vector<int> accV;

            if (Vector<int>.Count == 4)
            {
                baseV = new Vector<int>(new[] { 4, 4, 4, 4 });
                accV = new Vector<int>(new[] { 1, 3, 5, 7 });
            }
            else //if (Vector<int>.Count == 8)
            {
                baseV = new Vector<int>(new[] { 8, 8, 8, 8, 8, 8, 8, 8 });
                accV = new Vector<int>(new[] { 1, 3, 5, 7, 9, 11, 13, 15 });
            }
            yield return baseV.AsVector256<int>();
        }

        ////[DisassemblyDiagnoser(printAsm: true, printIL: true, printSource: true, printDiff: true)]
        public IEnumerable<Vector256<int>> GenerateSquares(int start)
        {
            var len = Vector<int>.Count;
            int[] squares = new int[len];
            //var bitLen = MathLib.Log2(len);
            var inc = (len << 1);
            int Current, Delta;
            if (start == 0)
            {
                Current = 1;
                Delta = 1;
            }
            else
            {
                var root = (int)MathLib.Sqrt(start);
                var sq = root * root;
                var next = root + 1;
                Current = next * next;
                Delta = Current - sq;
                if (sq == start)
                {
                    Current = sq;
                    Delta -= 2;
                }
            }




            //const int MaxIntSquare = 2_147_395_600;
            const int MaxIntSquareRoot = 46340;
            int maxVectorSquareRoot = MaxIntSquareRoot - (MaxIntSquareRoot % len) - len;
            int maxVectorSquare = maxVectorSquareRoot * maxVectorSquareRoot;
            Vector<int> s;

            if (len == 4)
            {
                while (Current <= maxVectorSquare)
                {
                    squares[0] = Current;
                    squares[1] = (Current += (Delta += 2));
                    squares[2] = (Current += (Delta += 2));
                    squares[3] = (Current += (Delta += 2));
                    Current += (Delta += 2);

                    s = new Vector<int>(squares);
                    var result = s.AsVector256<int>();
                    yield return result;
                    if (Current >= maxVectorSquare)
                        break;
                }
            }
            else
            {
                while (Current <= maxVectorSquare)
                {
                    //unrolled loop for future SIMD
                    squares[0] = Current;
                    squares[1] = (Current += (Delta += 2));
                    squares[2] = (Current += (Delta += 2));
                    squares[3] = (Current += (Delta += 2));
                    squares[4] = (Current += (Delta += 2));
                    squares[5] = (Current += (Delta += 2));
                    squares[6] = (Current += (Delta += 2));
                    squares[7] = (Current += (Delta += 2));
                    Current += (Delta += 2);

                    s = new Vector<int>(squares);
                    var result = s.AsVector256<int>();
                    yield return result;
                    if (Current >= maxVectorSquare)
                        break;
                }
            }


        }


        /// <summary>
        /// Generates the sequence of squares starting from the square root of n.
        /// </summary>
        public class SquareGenerator : IEnumerable<int>
        {
            private int Delta;
            private int Current;
            public const int MaxSquare = 2_147_395_600;
            public const int MaxSquareRoot = 46340;
            public SquareGenerator()
            {
                Current = 1;
                Delta = 1;
            }

            public SquareGenerator(int n)
            {
                var root = (int)MathLib.Sqrt(n);
                var sq = root * root;
                var next = root + 1;
                Current = next * next;
                Delta = Current - sq;
                if (sq == n)
                {
                    Current = sq;
                    Delta -= 2;
                }
            }

            public IEnumerator<int> GetEnumerator()
            {
                while (Current < MaxSquare)
                {
                    yield return Current;
                    Delta += 2;
                    Current += Delta;
                }
                yield return MaxSquare;
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

        }

        //TODO: Move to MathLib and expose a public api.

        //TODO: Decide wether to retire method. Could be useful to be called by other AVX methods
        public static int AvxDivRemDemo(int N, int[] divisors, out Vector256<double> quotient, out Vector256<double> remainder)
        {
            double[] d_divisors = divisors.Select(x => (double)x).ToArray();

            // get the reciprocals of our divisors so we can divide faster using multiplication
            double[] d_reciprocals = divisors.Select(x => 1.0 / x).ToArray();

            //align the array to a 32 bit word boundary
            MathLib.Align(ref d_divisors);
            //align the array to a 32 bit word boundary
            MathLib.Align(ref d_reciprocals);

            return AvxDivRemAlignedDemo(N, d_divisors, d_reciprocals, out quotient, out remainder);
        }

        public static unsafe int AvxDivRemAlignedDemo(int N, double[] d_divisors, double[] d_reciprocals,
                 out Vector256<double> quotient, out Vector256<double> remainder)
        {
            // an error to add to multiplication results to account for precision errors.


            Vector256<double> errVector = Vector256.Create((double)err);

            Vector256<double> nVector = Vector256.Create((double)N);


            //TODO: Loop through elements in divisors in batches of 4 to calculuate all divisors.
            //  This code currently process the first 4 elements only.
            int dividesExactlyCount = 0;
            fixed (double* divPtr = &d_divisors[0])
            fixed (double* recipPtr = &d_reciprocals[0])
            {
                Vector256<double> primeVect = Avx.LoadAlignedVector256(divPtr);
                Vector256<double> recipVec = Avx.LoadAlignedVector256(recipPtr);



                //1.) Multiply n * (1.0/d);
                /// <summary>
                /// __m256d _mm256_mul_pd (__m256d a, __m256d b)
                ///   VMULPD ymm, ymm, ymm/m256
                ///   https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#cats=Arithmetic&avxnewtechs=AVX&ig_expand=4701
                ///   Latency	Throughput (CPI)
                ///   Skylake	4	0.5
                /// </summary>
                Vector256<double> mulVec = Avx.Multiply(recipVec, nVector);



                //2.) Add error: n * (1.0/d) + 0.00000001
                /// <summary>
                /// __m256d _mm256_add_pd (__m256d a, __m256d b)
                ///   VADDPD ymm, ymm, ymm/m256
                ///  https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#cats=Arithmetic&avxnewtechs=AVX&ig_expand=137
                ///   Skylake	4	0.5
                /// </summary>
                Vector256<double> addErrVec = Avx.Add(mulVec, errVector);



                //3.) Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001);
                //      TODO: There is probaly a more efficient operation than floor, such as truncate. Simply trying to perform ((int)x)

                /// <summary>
                /// __m256d _mm256_floor_pd (__m256d a)
                ///   VROUNDPS ymm, ymm/m256, imm8(9)
                ///   https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#avxnewtechs=AVX&ig_expand=6699,6933,6903,3074&text=floor
                ///   Skylake	8	1
                /// </summary>
                quotient = Avx.Floor(addErrVec);



                //4.) Calculate the multiplicand (q*d);
                /// <summary>
                /// __m256d _mm256_mul_pd (__m256d a, __m256d b)
                ///   VMULPD ymm, ymm, ymm/m256
                ///   https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#cats=Arithmetic&avxnewtechs=AVX&ig_expand=4701
                ///   Skylake	4	0.5
                /// </summary>
                var multiplicand = Avx.Multiply(quotient, primeVect);


                //5.) Compare N == (q*d)
                /// <summary>
                /// __m256d _mm256_cmp_pd (__m256d a, __m256d b, const int imm8)
                ///   VCMPPD ymm, ymm, ymm/m256, imm8
                ///  https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#avxnewtechs=AVX&ig_expand=6699,6933,6903,3074,827&text=cmp
                ///  Skylake	4	0.5
                /// </summary>
                var eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);


                //6) Reduce the comparison N == (q*d) into a bit mask
                /// <summary>
                /// int _mm256_movemask_pd (__m256d a)
                ///   VMOVMSKPD reg, ymm
                ///   https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#avxnewtechs=AVX&ig_expand=6699,6933,6903,3074,827,4637&text=movemask
                ///   Skylake	2	1
                /// </summary>
                var dividesExactlyMask = Avx.MoveMask(eqVector);

                //7) Get a count of N == (q*d) from the mask
                /// <summary>
                /// Returns the population count (number of bits set) of a mask.
                /// int _mm_popcnt_u32 (unsigned int a)
                ///   POPCNT reg, reg/m32
                /// </summary>
                dividesExactlyCount = MathLib.PopCount(dividesExactlyMask);


                //4.) Calculate the remainder:  N-(q*d);
                /// <summary>
                /// __m256d _mm256_sub_pd (__m256d a, __m256d b)
                ///   VSUBPD ymm, ymm, ymm/m256
                ///   https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#cats=Arithmetic&avxnewtechs=AVX&ig_expand=6699
                ///   Skylake	4	0.5
                /// </summary>
                remainder = Avx.Subtract(nVector, multiplicand);


            }
            return dividesExactlyCount;
        }





    }
}