/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

#define SKIP_LONG_TESTS
#undef SKIP_LONG_TESTS  // uncomment to enable long tests

//some tests take several seconds and up to a minute to complete.
// These tests are disabled using SKIP_LONG_TESTS to keep unit testing short, and enable efficient Live Unit Testing
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Drawing;

namespace HigginsSoft.Math.Lib.Tests
{
    public class ConditionalConstants
    {
        public const string RUN_LONG_TESTS = nameof(RUN_LONG_TESTS);
    }

    namespace PrimeGeneratorTests
    {
        [TestClass()]
        public class PrimesTests
        {
            [TestMethod()]
            public void Primes_16_CountTest()
            {
                var a = Primes.Primes16;
                Assert.AreEqual(6, a.Length);
            }


            [TestMethod()]
            public void Primes_256_CountTest()
            {
                var a = Primes.Primes256;
                Assert.AreEqual(54, a.Length);
            }

            [TestMethod()]
            public void Primes_65536_CountTest()
            {
                var a = Primes.Primes65536;
                Assert.AreEqual(6542, a.Length);
            }

            [TestMethod()]
            public void Primes_IsPrime_Test()
            {

                Assert.IsFalse(Primes.IsPrime(-1));
                Assert.IsFalse(Primes.IsPrime(0));
                Assert.IsFalse(Primes.IsPrime(1));
                Assert.IsTrue(Primes.IsPrime(2));
                Assert.IsTrue(Primes.IsPrime(3));

            }
        }

        [TestClass()]
        public class TrialDivideTests
        {
            private void TrialDivide_Test(int bits, out int count, out int previousPrime, out int lastPrime)
            {

                int maxPrime = 1 << bits;
                var sieve = new PrimeGeneratorTrialDivide();
                count = 0;
                foreach (var prime in sieve)
                {
                    if (prime > maxPrime) break;
                    count++;
                }
                lastPrime = sieve.Current;
                previousPrime = sieve.Previous;

            }

            private void TrialDivide_Test(int powerOfTwo)
            {
                TrialDivide_Test(powerOfTwo, out int count, out int maxPrime, out int NextPrime);
                var expected = PrimeData.Counts[powerOfTwo];
                Assert.AreEqual(expected.Count, count, $"2^{powerOfTwo} Count Failed: Count: {count}, Max Prime: {maxPrime}, Next Prime: {NextPrime}");
                Assert.AreEqual((int)expected.MaxPrime, maxPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {maxPrime}, Next Prime: {NextPrime}");
                Assert.AreEqual((int)expected.NextPrime, NextPrime, $"2^{powerOfTwo} Next Prime Failed Count: {count}, Max Prime: {maxPrime}, Next Prime: {NextPrime}");
            }

            [TestMethod()]
            public void Generator_GetEnumerator_TestToN()
            {
                var testPrimes = new[] { 2, 3, 5, 7, 65537, 262147 };
                foreach (var prime in testPrimes)
                {
                    var gen = new PrimeGeneratorTrialDivide(prime);
                    var iter = gen.GetEnumerator();
                    while (iter.MoveNext()) ;
                    Assert.AreEqual(prime, iter.Current);
                }

            }

            [TestMethod()]
            public void TrialDivide_Bits_01_to_16_Test()
            {
                for (var i = 1; i < 17; i++)
                {
                    TrialDivide_Test(i);
                }
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits_17_to_23_Test()
            {
                for (var i = 17; i < 24; i++)
                {
                    TrialDivide_Test(i);
                }
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits24_Test()
            {
                TrialDivide_Test(24);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits25_Test()
            {
                TrialDivide_Test(25);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits26_Test()
            {
                TrialDivide_Test(26);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits27_Test()
            {
                TrialDivide_Test(27);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif            
            [TestMethod()]
            public void TrialDivide_Bits28_Test()
            {
                TrialDivide_Test(28);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits29_Test()
            {
                TrialDivide_Test(29);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits30_Test()
            {
                TrialDivide_Test(30);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void TrialDivide_Bits31_Test()
            {
                TrialDivide_Test(31);
            }


        }
    }

    namespace PrimeGeneratorTests

    {
        [TestClass]
        public class PrimeGeneratorTests
        {
            [TestMethod()]
            public void Generator_GetEnumerator_TestToN()
            {
                var testPrimes = new[] { 2, 3, 5, 7, 65537, 262147, 1048583 };
                foreach (var prime in testPrimes)
                {
                    var gen = new PrimeGenerator(prime);
                    var iter = gen.GetEnumerator();
                    while (iter.MoveNext()) ;
                    Assert.AreEqual(prime, iter.Current);
                }

            }

            [TestMethod()]
            public void Generator_GetBitIndex_Test()
            {
                var gen = new PrimeGenerator();
                Assert.AreEqual(0, gen.GetBitIndex(5));
                Assert.AreEqual(1, gen.GetBitIndex(7));
                Assert.AreEqual(2, gen.GetBitIndex(11));
                Assert.AreEqual(3, gen.GetBitIndex(13));
                Assert.AreEqual(4, gen.GetBitIndex(17));
                Assert.AreEqual(5, gen.GetBitIndex(19));
            }

            [TestMethod()]
            public void Generator_GetValue_Test()
            {
                var gen = new PrimeGenerator();
                Assert.AreEqual(5, gen.GetBitValue(0));
                Assert.AreEqual(7, gen.GetBitValue(1));
                Assert.AreEqual(11, gen.GetBitValue(2));
                Assert.AreEqual(13, gen.GetBitValue(3));
                Assert.AreEqual(17, gen.GetBitValue(4));
                Assert.AreEqual(19, gen.GetBitValue(5));
            }

            [TestMethod()]
            public void Generator_Inc_Test()
            {
                var gen = new PrimeGenerator();
                gen.incNext();
                Assert.AreEqual(2, gen.Current);
                Assert.AreEqual(-2, gen.BitIndex);

                gen.incNext();
                Assert.AreEqual(3, gen.Current);
                Assert.AreEqual(-1, gen.BitIndex);

                gen.incNext();
                Assert.AreEqual(5, gen.Current);
                Assert.AreEqual(0, gen.BitIndex);

                gen.incNext();
                Assert.AreEqual(7, gen.Current);
                Assert.AreEqual(1, gen.BitIndex);



                gen.incNext();
                Assert.AreEqual(11, gen.Current);
                Assert.AreEqual(2, gen.BitIndex);

                gen.incNext();
                Assert.AreEqual(13, gen.Current);
                Assert.AreEqual(3, gen.BitIndex);




                gen.incNext();
                Assert.AreEqual(17, gen.Current);
                Assert.AreEqual(4, gen.BitIndex);

                gen.incNext();
                Assert.AreEqual(19, gen.Current);
                Assert.AreEqual(5, gen.BitIndex);



                gen.incNext();
                Assert.AreEqual(23, gen.Current);
                Assert.AreEqual(6, gen.BitIndex);

                //todo: make sure 25 is not returned
                //gen.MoveNext();
                //Assert.AreEqual(25, gen.Current);
                //Assert.AreEqual(7, gen.BitIndex);



                gen.incNext();
                Assert.AreEqual(29, gen.Current);
                Assert.AreEqual(8, gen.BitIndex);


                gen.incNext();
                Assert.AreEqual(31, gen.Current);
                Assert.AreEqual(9, gen.BitIndex);


                //todo: make sure 35 is not returned
                //gen.MoveNext();
                //Assert.AreEqual(35, gen.Current);
                //Assert.AreEqual(10, gen.BitIndex);

                gen.incNext();
                Assert.AreEqual(37, gen.Current);
                Assert.AreEqual(11, gen.BitIndex);

            }


            [TestMethod()]
            public void Generator_Inc_CompareToPrimes256_Test()
            {
                var primes = Primes.Primes256;
                var gen = new PrimeGenerator();
                for (var i = 0; i < primes.Length; i++)
                {
                    var prime = primes[i];
                    gen.incNext();
                    Assert.AreEqual(prime, gen.Current);
                }
            }

            [TestMethod()]
            public void Generator_Inc_CompareToPrimes65536_Test()
            {
                var primes = Primes.Primes65536;
                var gen = new PrimeGenerator();

                for (var i = 0; i < primes.Length; i++)
                {
                    var prime = primes[i];
                    gen.incNext();
                    Assert.AreEqual(prime, gen.Current, $"Compared Primes Are Equal Failed.");
                }

                var data = PrimeData.Counts[16];

                Assert.AreEqual(data.MaxPrime, gen.Current, $"MaxPrime Failed.");
            }


            [TestMethod()]
            public void Generator_Inc_CompareToEnumerator_2P17_Test()
            {
                var data = PrimeData.Counts[17];
                var cpuBoud = new PrimeGeneratorTrialDivide();
                var gen = new PrimeGenerator();

                foreach (var prime in cpuBoud)
                {
                    if (prime > data.MaxPrime)
                        break;

                    gen.incNext();
                    Assert.AreEqual(prime, gen.Current);
                }
            }

            [TestMethod()]
            public void Generator_Inc_CompareToEnumerator_2P20_Test()
            {
                var data = PrimeData.Counts[20];
                var cpuBoud = new PrimeGeneratorTrialDivide();
                var gen = new PrimeGenerator();

                foreach (var prime in cpuBoud)
                {
                    if (prime > data.MaxPrime)
                        break;

                    gen.incNext();
                    Assert.AreEqual(prime, gen.Current);
                }
            }




            [TestMethod()]
            public void Generator_P20_Count_Test()
            {
                var data = PrimeData.Counts[20];
                var gen = new PrimeGenerator();
                int count = 0;

                while (true)
                {
                    gen.incNext();
                    if (gen.Current > data.MaxPrime)
                        break;
                    count++;
                }

                Assert.AreEqual(data.Count, count);
                Assert.AreEqual(data.NextPrime, gen.Current);
                Assert.AreEqual(data.MaxPrime, gen.Previous);

            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_P24_Count_Test()
            {
                var data = PrimeData.Counts[24];
                var gen = new PrimeGenerator();
                int count = 0;

                while (true)
                {
                    gen.incNext();
                    if (gen.Current > data.MaxPrime)
                        break;
                    count++;
                }

                Assert.AreEqual(data.Count, count);
                Assert.AreEqual(data.NextPrime, gen.Current);
                Assert.AreEqual(data.MaxPrime, gen.Previous);

            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Inc_CompareToEnumerator_2P28_Test()
            {
                var data = PrimeData.Counts[28];
                var cpuBoud = new PrimeGeneratorTrialDivide();
                var gen = new PrimeGenerator();

                int power = 0;
                int mask = 1;
                foreach (var prime in cpuBoud)
                {
                    if (prime > data.MaxPrime)
                        break;
                    if (prime > mask)
                    {
                        power++; mask <<= 1;
                    }
                    gen.incNext();
                    Assert.AreEqual(prime, gen.Current);
                }

                Assert.AreEqual(data.MaxPrime, gen.Current, $"MaxPrime Failed.");

            }


            private void TestPrimeGenerator(int bits, long max, out int count, out int previousPrime, out int currentPrime)
            {

                var gen = new PrimeGenerator((int)max);
                count = 0;

                while (true)
                {
                    gen.incNext();
                    Assert.AreNotEqual(gen.Previous, gen.Current);
                    //Assert.IsTrue(Primes.IsPrime(gen.Current), $"Prime check failed for {gen.Current}");
                    if (bits == 31)
                    {
                        if (gen.Current >= max)
                        {
                            count++;
                            break;
                        }
                    }
                    if (gen.Current > max)
                    {
                        if (bits == 31)
                            count++;
                        break;
                    }

                    count++;
                }
                previousPrime = gen.Previous;
                currentPrime = gen.Current;


            }

            private void TestPrimeGenerator(int powerOfTwo)
            {
                var expected = PrimeData.Counts[powerOfTwo];
                TestPrimeGenerator(powerOfTwo, expected.MaxPrime, out int count, out int previousPrime, out int currentPrime);

                Assert.AreEqual(expected.Count, count, $"2^{powerOfTwo} Count Failed: Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                if (powerOfTwo < 31)
                {
                    Assert.AreEqual((int)expected.NextPrime, currentPrime, $"2^{powerOfTwo} Next Prime Failed Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                    Assert.AreEqual((int)expected.MaxPrime, previousPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                }
                else
                {
                    Assert.AreEqual((int)expected.MaxPrime, currentPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");

                }

            }

            [TestMethod()]
            public void Generator_Bits_01_to_16_Test()
            {
                for (var i = 3; i < 17; i++)
                {
                    TestPrimeGenerator(i);
                }
            }

            [TestMethod()]
            public void Generator_Bits_17_to_23_Test()
            {
                for (var i = 17; i < 24; i++)
                {
                    TestPrimeGenerator(i);
                }
            }

            [TestMethod()]
            public void Generator_Bits_24_Test()
            {
                TestPrimeGenerator(24);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_25_Test()
            {
                TestPrimeGenerator(25);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_26_Test()
            {
                TestPrimeGenerator(26);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_27_Test()
            {
                TestPrimeGenerator(27);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_28_Test()
            {
                TestPrimeGenerator(28);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_29_Test()
            {
                TestPrimeGenerator(29);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_30_Test()
            {
                TestPrimeGenerator(30);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_Test()
            {
                TestPrimeGenerator(31);
            }
        }
    }

    namespace PrimeGeneratorTests
    {
        [TestClass]
        public class PrimeGeneratorParallelTests
        {

            private void TestPrimeGenerator(int bits, long max, ProcessorCount threadCount, out int count, out int previousPrime, out int currentPrime)
            {

                var gen = new PrimeGeneratorParallel((int)max, threadCount);
                count = 0;

                while (true)
                {
                    gen.incNext();
                    Assert.AreNotEqual(gen.Previous, gen.Current);
                    //Assert.IsTrue(Primes.IsPrime(gen.Current), $"Prime check failed for {gen.Current}");
                    if (bits == 31)
                    {
                        if (gen.Current >= max)
                        {
                            count++;
                            break;
                        }
                    }
                    if (gen.Current > max)
                    {
                        if (bits == 31)
                            count++;
                        break;
                    }

                    count++;
                }
                previousPrime = gen.Previous;
                currentPrime = gen.Current;


            }

            private void TestPrimeGenerator(int powerOfTwo, int threadCount = 4)
            {

                Assert.IsTrue(Enum.IsDefined(typeof(ProcessorCount), threadCount));
                ProcessorCount processorCount = (ProcessorCount)threadCount;

                var expected = PrimeData.Counts[powerOfTwo];
                TestPrimeGenerator(powerOfTwo, expected.MaxPrime, processorCount, out int count, out int previousPrime, out int currentPrime);

                Assert.AreEqual(expected.Count, count, $"2^{powerOfTwo} Count Failed: Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                if (powerOfTwo < 31)
                {
                    Assert.AreEqual((int)expected.NextPrime, currentPrime, $"2^{powerOfTwo} Next Prime Failed Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                    Assert.AreEqual((int)expected.MaxPrime, previousPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                }
                else
                {
                    Assert.AreEqual((int)expected.MaxPrime, currentPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");

                }

            }

            [TestMethod()]
            public void Generator_GetEnumerator_TestToN()
            {
                var testPrimes = new[] { 2, 3, 5, 7, 65537, 262147, 1048583 };
                foreach (var prime in testPrimes)
                {
                    var gen = new PrimeGeneratorParallel(prime);
                    var iter = gen.GetEnumerator();
                    while (iter.MoveNext()) ;
                    Assert.AreEqual(prime, iter.Current);
                }

            }

            [TestMethod()]
            public void Generator_Bits_01_to_16_Test()
            {
                for (var i = 3; i < 17; i++)
                {
                    TestPrimeGenerator(i);
                }
            }

            [TestMethod()]
            public void Generator_Bits_17_to_23_Test()
            {
                for (var i = 17; i < 24; i++)
                {
                    TestPrimeGenerator(i);
                }
            }

            [TestMethod()]
            public void Generator_Bits_24_Test()
            {
                TestPrimeGenerator(24);
            }

            [TestMethod()]
            public void Generator_Bits_24_MaxCores_Test()
            {
                TestPrimeGenerator(24, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_25_Test()
            {
                TestPrimeGenerator(25);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_25_MaxCores_Test()
            {
                TestPrimeGenerator(25, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_26_Test()
            {
                TestPrimeGenerator(26);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_26_MaxCores_Test()
            {
                TestPrimeGenerator(26, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_27_Test()
            {
                TestPrimeGenerator(27);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_27_MaxCores_Test()
            {
                TestPrimeGenerator(27, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_28_Test()
            {
                TestPrimeGenerator(28);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_28_MaxCores_Test()
            {
                TestPrimeGenerator(28, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_29_Test()
            {
                TestPrimeGenerator(29);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_29_MaxCores_Test()
            {
                TestPrimeGenerator(29, Environment.ProcessorCount);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_30_Test()
            {
                TestPrimeGenerator(30);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_30_MaxCores_Test()
            {
                TestPrimeGenerator(30, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_Test()
            {
                TestPrimeGenerator(31);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_8Cores_Test()
            {
                TestPrimeGenerator(31, 8);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_MaxCores_Test()
            {
                TestPrimeGenerator(31, Environment.ProcessorCount);
            }

        }
    }


    namespace PrimeGeneratorTests
    {
        [TestClass]
        public class PrimeGeneratorUnsafeTests
        {

            private void TestPrimeGenerator(int bits, long max, ProcessorCount threadCount, out int count, out int previousPrime, out int currentPrime)
            {

                var gen = new PrimeGeneratorUnsafe((int)max);
                count = 0;
                var iter = gen.GetEnumerator();
                while (iter.MoveNext())
                {
                    Assert.AreNotEqual(gen.Previous, gen.Current);
                    count++;
                    if (gen.Current >= max)
                    {

                        break;
                    }
                }
                previousPrime = gen.Previous;
                currentPrime = gen.Current;


            }

            private void TestPrimeGenerator(int powerOfTwo, int threadCount = 4)
            {

                Assert.IsTrue(Enum.IsDefined(typeof(ProcessorCount), threadCount));
                ProcessorCount processorCount = (ProcessorCount)threadCount;

                var expected = PrimeData.Counts[powerOfTwo];
                TestPrimeGenerator(powerOfTwo, expected.MaxPrime, processorCount, out int count, out int previousPrime, out int currentPrime);

                Assert.AreEqual(expected.Count, count, $"2^{powerOfTwo} Count Failed: Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                Assert.AreEqual((int)expected.MaxPrime, currentPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");



            }

            [TestMethod()]
            public void Generator_GetEnumerator_TestToN()
            {
                var testPrimes = new[] { 2, 3, 5, 7, 65537, 262147, 1048583 };
                foreach (var prime in testPrimes)
                {
                    var gen = new PrimeGeneratorUnsafe(prime);
                    var iter = gen.GetEnumerator();
                    while (iter.MoveNext()) ;
                    Assert.AreEqual(prime, iter.Current);
                }

            }

            void TestRangeCountTest(int powerOfTwo)
            {
                var i = powerOfTwo;

                var low = 1 << i;
                var high = (int)((1u << (i + 1)) - 1);
                var lowData = PrimeData.Counts[i];
                var highData = PrimeData.Counts[i + 1];


                var gen = new PrimeGeneratorUnsafe(low, high);// (int)highData.MaxPrime);


                var iter = gen.GetEnumerator();
                int count = 0;
                if (iter.MoveNext())
                    count++;

                Assert.AreEqual(lowData.NextPrime, gen.Current, $"First prime check failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");

                while (iter.MoveNext())
                {
                    count++;
                }

                Assert.AreEqual(highData.MaxPrime, gen.Current, $"Last prime check failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");
                var expectedCount = highData.Count - lowData.Count;
                Assert.AreEqual(expectedCount, count, $"Prime count failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");


            }

            void TestRangeCompareTest(int powerOfTwo)
            {
                var i = powerOfTwo;

                var low = 1 << i;
                var high = (int)((1u << (i + 1)) - 1);
                var lowData = PrimeData.Counts[i];
                var highData = PrimeData.Counts[i + 1];


                var gen = new PrimeGeneratorUnsafe(low, high);// (int)highData.MaxPrime);
                var comp = new PrimeGenerator();
                var compiter = comp.GetEnumerator();

                while (compiter.MoveNext() && compiter.Current < lowData.NextPrime) ;

                var iter = gen.GetEnumerator();
                int count = 0;
                if (iter.MoveNext())
                    count++;

                Assert.AreEqual(lowData.NextPrime, gen.Current, $"First prime check failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");

                while (iter.MoveNext() && compiter.MoveNext())
                {
                    count++;
                    Assert.AreEqual(iter.Current, compiter.Current, $"Compare prime check failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");
                }


                Assert.AreEqual(highData.MaxPrime, gen.Current, $"Last prime check failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");
                var expectedCount = highData.Count - lowData.Count;
                Assert.AreEqual(expectedCount, count, $"Prime count failed at window {gen.CurrentWindow} for range 2^{i} - 2^{i + 1}");

            }

            void TestRange(int start, int end, int expected)
            {
                var gen = new PrimeGeneratorUnsafe(start, end);
                var iter = gen.GetEnumerator();
                int count = 0;
                while (iter.MoveNext())
                    count++;

                Assert.AreEqual(expected, count, $"Small range check failed for {start} to {end}");

            }

            [TestMethod()]
            public void Generator_Range_Small_Tests()
            {
            
                TestRange(0, 2, 1);
                TestRange(0, 3, 2);
                TestRange(0, 5, 3);
                TestRange(0, 7, 4);
                TestRange(0, 11, 5);
                TestRange(0, 13, 6);
                TestRange(0, 2, 1);

                TestRange(3, 5, 2);
                TestRange(3, 7, 3);


                TestRange(5, 7, 2);
                TestRange(7, 9, 1);

                TestRange(19, 31, 4);

                TestRange(1073741823, 1073741853, 5);
            }


            [TestMethod()]
            public void Generator_RangeCompare_P3_P20_Tests()
            {
                for (var i = 3; i < 21; i++)
                {
                    TestRangeCompareTest(i);
                }

            }

            [TestMethod()]
            public void Generator_RangeCompare_P24_Tests()
            {
                TestRangeCompareTest(24);
            }


            [TestMethod()]
            public void Generator_RangeCompare_P28_Tests()
            {
                TestRangeCompareTest(28);
            }




            [TestMethod()]
            public void Generator_RangeCompare_P31_Tests()
            {
                TestRangeCompareTest(30);
            }

            [TestMethod()]
            public void Generator_RangeCount_P3_P20_Tests()
            {
                for (var i = 3; i < 21; i++)
                {
                    TestRangeCountTest(i);
                }
            }

            [TestMethod()]
            public void Generator_RangeCount_P24_Tests()
            {
                TestRangeCountTest(24);
            }


            [TestMethod()]
            public void Generator_RangeCount_P28_Tests()
            {
                TestRangeCountTest(28);
            }

            [TestMethod()]
            public void Generator_RangeCount_P31_Tests()
            {
                TestRangeCountTest(30);
            }



            [TestMethod()]
            public void Generator_Bits_01_to_16_Test()
            {
                for (var i = 3; i < 17; i++)
                {
                    TestPrimeGenerator(i);
                }
            }

            [TestMethod()]
            public void Generator_Bits_17_to_23_Test()
            {
                for (var i = 17; i < 24; i++)
                {
                    TestPrimeGenerator(i);
                }
            }

            [TestMethod()]
            public void Generator_Bits_24_Test()
            {
                TestPrimeGenerator(24);
            }

            [TestMethod()]
            public void Generator_Bits_24_MaxCores_Test()
            {
                TestPrimeGenerator(24, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_25_Test()
            {
                TestPrimeGenerator(25);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_25_MaxCores_Test()
            {
                TestPrimeGenerator(25, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_26_Test()
            {
                TestPrimeGenerator(26);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_26_MaxCores_Test()
            {
                TestPrimeGenerator(26, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_27_Test()
            {
                TestPrimeGenerator(27);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_27_MaxCores_Test()
            {
                TestPrimeGenerator(27, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_28_Test()
            {
                TestPrimeGenerator(28);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_28_MaxCores_Test()
            {
                TestPrimeGenerator(28, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_29_Test()
            {
                TestPrimeGenerator(29);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_29_MaxCores_Test()
            {
                TestPrimeGenerator(29, Environment.ProcessorCount);
            }


#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_30_Test()
            {
                TestPrimeGenerator(30);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_30_MaxCores_Test()
            {
                TestPrimeGenerator(30, Environment.ProcessorCount);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_Test()
            {
                TestPrimeGenerator(31);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_8Cores_Test()
            {
                TestPrimeGenerator(31, 8);
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void Generator_Bits_31_MaxCores_Test()
            {
                TestPrimeGenerator(31, Environment.ProcessorCount);
            }

        }
    }
}

