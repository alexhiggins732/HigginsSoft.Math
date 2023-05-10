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
//#undef SKIP_LONG_TESTS  // uncomment to enable long tests

//some tests take several seconds and up to a minute to complete.
// These tests are disabled using SKIP_LONG_TESTS to keep unit testing short, and enable efficient Live Unit Testing
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HigginsSoft.Math.Lib.Tests
{

    namespace PrimeGeneratorTests

    {

        [TestClass]
        public class PrimeGenerator_GenericInterfaceTests : PrimeGeneratorGenericInterfaceTests<PrimeGenerator>
        {
            public PrimeGenerator_GenericInterfaceTests() : base(x => new PrimeGenerator(x))
            {
            }
        }
        [TestClass]
        public class PrimeGeneratorRef_GenericInterfaceTests : PrimeGeneratorGenericInterfaceTests<PrimeGeneratorRef>
        {
            public PrimeGeneratorRef_GenericInterfaceTests() : base(x => new PrimeGeneratorRef(x))
            {
            }
        }

        [TestClass]
        public class PrimeGeneratorUnsafe_GenericInterfaceTests : PrimeGeneratorGenericInterfaceTests<PrimeGeneratorUnsafe>
        {
            public PrimeGeneratorUnsafe_GenericInterfaceTests() : base(x => new PrimeGeneratorUnsafe(x))
            {
            }
        }


        [TestClass]
        public class PrimeGeneratorTrialDivide_GenericInterfaceTests : PrimeGeneratorGenericInterfaceTests<PrimeGeneratorTrialDivide>
        {
            public PrimeGeneratorTrialDivide_GenericInterfaceTests() : base(x => new PrimeGeneratorTrialDivide(x))
            {
            }
        }

        public class PrimeGeneratorGenericInterfaceTests<T>
            where T : IPrimeGenerator, new()
        {
            private Func<int, T> ctor;


            public PrimeGeneratorGenericInterfaceTests(Func<int, T> ctor)
            {
                this.ctor = ctor;
            }

            [TestMethod()]
            public void Generator_GetEnumerator_TestToN()
            {
                var testPrimes = new[] { 2, 3, 5, 7, 65537, 262147, 1048583 };
                foreach (var prime in testPrimes)
                {
                    var gen = ctor(prime);
                    var iter = gen.GetEnumerator();
                    while (iter.MoveNext()) ;
                    Assert.AreEqual(prime, iter.Current);
                }
            }

            [TestMethod()]
            public void Generator_GetBitIndex_Test()
            {
                var gen = new T();
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
                var gen = new T();
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
                var gen = new T();
                var iter = gen.GetEnumerator();
                iter.MoveNext();
                Assert.AreEqual(2, gen.Current);
                Assert.AreEqual(-2, gen.BitIndex);

                iter.MoveNext();
                Assert.AreEqual(3, gen.Current);
                Assert.AreEqual(-1, gen.BitIndex);

                iter.MoveNext();
                Assert.AreEqual(5, gen.Current);
                Assert.AreEqual(0, gen.BitIndex);

                iter.MoveNext();
                Assert.AreEqual(7, gen.Current);
                Assert.AreEqual(1, gen.BitIndex);



                iter.MoveNext();
                Assert.AreEqual(11, gen.Current);
                Assert.AreEqual(2, gen.BitIndex);

                iter.MoveNext();
                Assert.AreEqual(13, gen.Current);
                Assert.AreEqual(3, gen.BitIndex);




                iter.MoveNext();
                Assert.AreEqual(17, gen.Current);
                Assert.AreEqual(4, gen.BitIndex);

                iter.MoveNext();
                Assert.AreEqual(19, gen.Current);
                Assert.AreEqual(5, gen.BitIndex);



                iter.MoveNext();
                Assert.AreEqual(23, gen.Current);
                Assert.AreEqual(6, gen.BitIndex);

                //todo: make sure 25 is not returned
                //gen.MoveNext();
                //Assert.AreEqual(25, gen.Current);
                //Assert.AreEqual(7, gen.BitIndex);



                iter.MoveNext();
                Assert.AreEqual(29, gen.Current);
                Assert.AreEqual(8, gen.BitIndex);


                iter.MoveNext();
                Assert.AreEqual(31, gen.Current);
                Assert.AreEqual(9, gen.BitIndex);


                //todo: make sure 35 is not returned
                //gen.MoveNext();
                //Assert.AreEqual(35, gen.Current);
                //Assert.AreEqual(10, gen.BitIndex);

                iter.MoveNext();
                Assert.AreEqual(37, gen.Current);
                Assert.AreEqual(11, gen.BitIndex);

            }


            [TestMethod()]
            public void Generator_Inc_CompareToPrimes256_Test()
            {
                var primes = Primes.Primes256;
                var gen = new T();
                var iter = gen.GetEnumerator();
                for (var i = 0; i < primes.Length; i++)
                {
                    var prime = primes[i];
                    iter.MoveNext();
                    Assert.AreEqual(prime, gen.Current);
                }
            }

            [TestMethod()]
            public void Generator_Inc_CompareToPrimes65536_Test()
            {
                var primes = Primes.Primes65536;
                var gen = new T();
                var iter = gen.GetEnumerator();
                for (var i = 0; i < primes.Length; i++)
                {
                    var prime = primes[i];
                    iter.MoveNext();
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
                var gen = new T();
                var iter = gen.GetEnumerator();
                foreach (var prime in cpuBoud)
                {
                    if (prime > data.MaxPrime)
                        break;

                    iter.MoveNext();
                    Assert.AreEqual(prime, gen.Current);
                }
            }

            [TestMethod()]
            public void Generator_Inc_CompareToEnumerator_2P20_Test()
            {
                var data = PrimeData.Counts[20];
                var cpuBoud = new PrimeGeneratorTrialDivide();
                var gen = new T();
                var iter = gen.GetEnumerator();
                foreach (var prime in cpuBoud)
                {
                    if (prime > data.MaxPrime)
                        break;

                    iter.MoveNext();
                    Assert.AreEqual(prime, gen.Current);
                }
            }




            [TestMethod()]
            public void Generator_P20_Count_Test()
            {
                var data = PrimeData.Counts[20];
                var gen = new T();
                int count = 0;
                var iter = gen.GetEnumerator();
                while (true)
                {
                    iter.MoveNext();
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
                var gen = new T();
                int count = 0;
                var iter = gen.GetEnumerator();
                while (true)
                {
                    iter.MoveNext();
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
                var gen = new T();

                int power = 0;
                int mask = 1;
                var iter = gen.GetEnumerator();
                foreach (var prime in cpuBoud)
                {
                    if (prime > data.MaxPrime)
                        break;
                    if (prime > mask)
                    {
                        power++; mask <<= 1;
                    }
                    iter.MoveNext();
                    Assert.AreEqual(prime, gen.Current);
                }

                Assert.AreEqual(data.MaxPrime, gen.Current, $"MaxPrime Failed.");

            }


            private void TestPrimeGenerator(int bits, long max, out int count, out int previousPrime, out int currentPrime)
            {

                var gen = ctor((int)max);
                count = 0;
                var iter= gen.GetEnumerator();
                while (iter.MoveNext())
                {
                  
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
                    //Assert.AreEqual((int)expected.p, currentPrime, $"2^{powerOfTwo} Next Prime Failed Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
                    Assert.AreEqual((int)expected.MaxPrime, currentPrime, $"2^{powerOfTwo} Max Prime Failed - Count: {count}, Max Prime: {previousPrime}, Next Prime: {currentPrime}");
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

#if SKIP_LONG_TESTS
            [Ignore]
#endif
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
}


