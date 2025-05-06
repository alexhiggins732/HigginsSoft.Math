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
    public class FuncEnumeratorTests
    {

        [TestMethod()]
        public void IsPrimeTestTo2p16()
            => IsPrimeTestTo2pN(16);

        [TestMethod()]
        public void IsPrimeTestTo2p20()
            => IsPrimeTestTo2pN(20);

        [TestMethod()]
        public void IsPrimeTestTo2p24()
            => IsPrimeTestTo2pN(24);


        public void IsPrimeTestTo2pN(int N)
        {
            List<Func<int, bool>> tests = new(); // todo set capacity based on phi(n)

            Func<int, Func<int, bool>> buildTest = (prime) =>
            {
                var current = prime;
                Func<int, bool> test = (value) =>
                {
                    while (current < value)
                    {
                        current += prime;
                    }
                    return current > value;
                };

                return test;

            };
            tests.Add(buildTest(2));
            int count = 0;
            var max = 1 << N;
            var root = (int)MathLib.Sqrt(max);
            var sw = Stopwatch.StartNew();
            for (var i = 3; i <= max; i++)
            {
                bool isPrime = tests.Any(test => test(i));
                if (isPrime)
                {
                    count++;
                    //Console.WriteLine($"{nameof(i)} is prime");
                    if (i <= root)
                        tests.Add(buildTest(i));
                }
            }
            sw.Stop();
            Console.WriteLine($"Count of primes: {count} in {sw}");
        }

        [TestMethod()]
        public void IsPrimePartitionTestTo2p16()
        {
            IsPrimePartitionTestTo2pN(16);
        }

        [TestMethod()]
        public void IsPrimePartitionTestTo2p20()
        {
            IsPrimePartitionTestTo2pN(20);
        }

        public void IsPrimePartitionTestTo2pN(int N)
        {
            //List<Func<int, bool>> tests = new(); // todo set capacity based on phi(n)

            var tests = Enumerable.Range(0, 256)
                 .ToDictionary(i => i, i => new List<Func<int, bool>>());

            Action<int> addTest = (prime) =>
            {

                var current = prime << 1;
                Func<int, bool> ptr = null!;
                Func<int, bool> test = (value) =>
                {
                    if (current > value)
                    {
                        return false;
                    }
                    var key = current & 255;
                    bool result = current == value;
                    while (current <= value)
                    {
                        current += prime;
                        if (current == value)
                        {
                            result = true;
                        }
                    }
                    tests[key].Remove(ptr);
                    tests[current & 255].Add(ptr);
                    return result;
                };
                ptr = test;
                tests[current & 255].Add(ptr);


            };
            addTest(2);
            int count = 0;
            var max = 1 << N;
            var root = (int)MathLib.Sqrt(max);
            var sw = Stopwatch.StartNew();
            for (var i = 3; i <= max; i++)
            {
                bool isComposite = tests[i & 255].Any(test => test(i));
                if (!isComposite)
                {
                    count++;
                    //Console.WriteLine($"{nameof(i)} is prime");
                    if (i <= root)
                        addTest(i);
                }
            }
            sw.Stop();
            Console.WriteLine($"Count of primes: {count} in {sw}");
        }


        [TestMethod()]
        public void IsPrimeTestTo2p16Inline()
        {
            List<Func<int, bool>> tests = new();

            Func<int, Func<int, bool>> buildTest = (prime) =>
            {
                var current = prime;
                Func<int, bool> test = (value) =>
                {
                    while (current < value)
                    {
                        current += prime;
                    }
                    return current == value;
                };

                return test;

            };
            tests.Add(buildTest(2));
            int count = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 3; i < 1 << 16; i++)
            {
                bool composite = tests.Any(test => test(i));
                if (!composite)
                {
                    count++;
                    Console.WriteLine($"{nameof(i)} - {i} is prime");
                    tests.Add(buildTest(i));
                }
            }
            sw.Stop();
            Console.WriteLine($"Count of primes: {count} in {sw}");
        }


        [TestMethod()]
        public void IsPrimeMod6TestTo2p16Inline()
        {
            List<Func<int, bool>> tests = new();

            Func<int, Func<int, bool>> buildTest = (prime) =>
            {
                var current1 = prime * 5;
                var current0 = prime * prime;
                var step = prime << 1;
                Func<int, bool> test = (value) =>
                {
                    while (current0 < value)
                    {
                        current0 += step;
                    }
                    if (current0 == value) return true;
                    while (current1 < value)
                    {
                        current1 += step;
                    }
                    return current1 == value;
                };

                return test;


            };

            tests.Add(buildTest(5));
            int count = 0;
            var sw = Stopwatch.StartNew();
            for (var i = 5; i < 1 << 16; i += 4)
            {
                bool composite = tests.Any(test => test(i));
                if (!composite)
                {
                    count++;
                    Console.WriteLine($"{nameof(i)} - {i} is prime");
                    tests.Add(buildTest(i));
                }
                i += 2;
                composite = tests.Any(test => test(i));
                if (!composite)
                {
                    count++;
                    Console.WriteLine($"{nameof(i)} - {i} is prime");
                    tests.Add(buildTest(i));
                }

            }
            sw.Stop();
            Console.WriteLine($"Count of primes: {count} in {sw}");
        }


        [TestMethod()]
        public void IsPrimeMod6TestTo2p16()
            => IsPrimeMod6TestTo2pN(16);

        [TestMethod()]
        public void IsPrimeMod6TestTo2p20()
            => IsPrimeMod6TestTo2pN(20);

        [TestMethod()]
        public void IsPrimeMod6TestTo2p24()
            => IsPrimeMod6TestTo2pN(24);

        public void IsPrimeMod6TestTo2pN(int N)
        {
            List<Func<int, bool>> tests = new();// todo set capacity based on phi(n)

            Func<int, Func<int, bool>> buildTest = (prime) =>
            {
                var current0 = prime;
                var current1 = prime * 5;
                var step = prime << 1;
                Func<int, bool> test = (value) =>
                {
                    while (current0 < value)
                    {
                        current0 += step;
                    }
                    if (current0 == value) return true;
                    while (current1 < value)
                    {
                        current1 += step;
                    }
                    return current1 == value;
                };

                return test;

            };
            tests.Add(buildTest(5));
            tests.Add(buildTest(7));
            int count = 4; // add 2, 3, 5 and 7 to the count
            var max = 1 << N;
            var root = (int)MathLib.Sqrt(max);
            var sw = Stopwatch.StartNew();
            for (var i = 11; i <= max; i += 4)
            {
                bool isComposite = tests.Any(test => test(i));
                if (!isComposite)
                {
                    count++;
                    //Console.WriteLine($"{nameof(i)} {i} is prime");
                    if (i <= root)
                        tests.Add(buildTest(i));
                }
                i += 2;
                isComposite = tests.Any(test => test(i));
                if (!isComposite)
                {
                    count++;
                    //Console.WriteLine($"{nameof(i)} {i} is prime");
                    if (i <= root)
                        tests.Add(buildTest(i));
                }

            }
            sw.Stop();
            Console.WriteLine($"Count of primes: {count} in {sw}");
        }

    }
    [TestClass()]
    public class PrimalityCheckTest
    {
        [TestMethod()]
        public void IsPrimeTest()
        {
            var primes = Primes.IntFactorPrimes
                .Where(x => x > 300).Take(300).ToArray();


            var max = primes.Max();
            var j = 0;

            for (var i = 300; j < primes.Length && i <= max; i++)
            {
                var p = primes[j];
                var isprime = PrimeSquareCongruenceChecker.IsPrime(i);
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
            var count2 = candidates.Where(x => PrimeSquareCongruenceChecker.IsPrime(x)).Count();

            Assert.AreEqual(count1, count2);

        }

        //[Ignore("Takes too long to converge")]
        [TestMethod()]
        public void CheckIsPrimeTest()
        {
            PrimeSquareCongruenceChecker.IsPrime(1);
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
                result2 = PrimeSquareCongruenceChecker.IsPrime(i);
                sw2.Stop();


                if (result1 != result2)
                {

                    var message = $"Conqruence checker failed for {i} - {result2} expected: {result1}";
                    Console.WriteLine(message);
                    Assert.AreEqual(result1, result2, message);
                }
            }
            var time1 = $"{sw1.Elapsed} - {nameof(Primes)}.{nameof(Primes.IsPrime)}";
            var time2 = $"{sw2.Elapsed} - {nameof(PrimeSquareCongruenceChecker)}.{nameof(Primes.IsPrime)}";
            Console.WriteLine(time1);
            Console.WriteLine(time2);
        }
    }
}