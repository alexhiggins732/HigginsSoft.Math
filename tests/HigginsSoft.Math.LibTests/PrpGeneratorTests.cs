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
using MathGmp.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests
{

    namespace PrimeGeneratorTests
    {
        [TestClass]
        public class PrpGeneratorTests()
        {



            [TestMethod()]
            public void Generator_Gmp_24()
            {
                GenerateGmpPrp(24, 25);
            }

            [TestMethod()]
            public void Generator_BigInteger_24()
            {
                GenerateGmpPrp(24, 25);
            }

            [TestMethod()]
            public void Generator_Long_24()
            {
                GenerateGmpPrp(24, 25);
            }

            [TestMethod()]
            public void Generator_LongRabinMiller_24()
            {
                GenerateLongRabinMillerPrp(24, 25);
            }


            [TestMethod()]
            public void Generator_Gmp_33()
            {
                GenerateGmpPrp(33, 34, 100_000);
            }

            [TestMethod()]
            public void Generator_BigInteger_33()
            {
                GenerateBigIntegerPrp(33, 34, 100_000);
            }

            [TestMethod()]
            public void Generator_Long_33()
            {
                GenerateGmpPrp(33, 34, 100_000);
            }

            [TestMethod()]
            public void Generator_LongRabinMiller_33()
            {
                GenerateLongRabinMillerPrp(33, 34, 100_000);
            }


            public void GenerateGmpPrp(int testStartBit, int testEndBit, int maxCount = int.MaxValue)
            {

                int count = 0;
                var gen = new PrpGeneratorGmp(testStartBit, testEndBit);
                var sw = Stopwatch.StartNew();
                foreach (var prp in gen.PrpGenerator())
                {
                    count++;
                    if (count >= maxCount) break;
                }
                sw.Stop();
                Console.WriteLine($"GenerateGmpPrp Generated {count.ToString("N0")} pseudo primes between 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
                Debug.WriteLine($"GenerateGmpPrp Generated {count.ToString("N0")} pseudo primes up to 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");

            }


            public void GenerateBigIntegerPrp(int testStartBit, int testEndBit, int maxCount = int.MaxValue)
            {

                int count = 0;
                var gen = new PrpGeneratorNumerics(testStartBit, testEndBit);
                var sw = Stopwatch.StartNew();
                foreach (var prp in gen.PrpGenerator())
                {
                    count++;
                    if (count >= maxCount) break;
                }
                sw.Stop();
                Console.WriteLine($"GenerateBigIntegerPrp Generated {count.ToString("N0")} pseudo primes between 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
                Debug.WriteLine($"GenerateBigIntegerPrp Generated {count.ToString("N0")} pseudo primes up to 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
            }

            public void GenerateLongPrp(int testStartBit, int testEndBit, int maxCount = int.MaxValue)
            {

                int count = 0;
                var gen = new PrpGeneratorLong(testStartBit, testEndBit);
                var sw = Stopwatch.StartNew();
                foreach (var prp in gen.PrpGenerator())
                {
                    count++;
                    if (count >= maxCount) break;
                }
                sw.Stop();
                Console.WriteLine($"PrpGeneratorLong Generated {count.ToString("N0")} pseudo primes between 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
                Debug.WriteLine($"PrpGeneratorLong Generated {count.ToString("N0")} pseudo primes up to 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
            }

            public void GenerateLongRabinMillerPrp(int testStartBit, int testEndBit, int maxCount = int.MaxValue)
            {

                int count = 0;
                var gen = new PrpGeneratorLongRabinMiller(testStartBit, testEndBit);
                var sw = Stopwatch.StartNew();
                foreach (var prp in gen.PrpGenerator())
                {
                    count++;
                    if (count >= maxCount) break;
                }
                sw.Stop();
                Console.WriteLine($"GenerateLongRabinMillerPrp Generated {count.ToString("N0")} pseudo primes between 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
                Debug.WriteLine($"GenerateLongRabinMillerPrp Generated {count.ToString("N0")} pseudo primes up to 2^{testStartBit} and 2^{testEndBit}-1 in {sw.Elapsed}");
            }
        }

        public class PrpGeneratorGmp
        {
            long NextPrime;
            long MaxPrime;

            public PrpGeneratorGmp(int min, int max)
            {
                NextPrime = 1L << min;
                MaxPrime = (1L << max) - 1;
            }
            /// <summary>
            /// returns the next prime number greater than or equal to z
            /// </summary>
            /// <param name="z"></param>
            public static void GetNextPrime(GmpInt z)
            {
                if (gmp_lib.mpz_even_p(z.Data) > 0)
                {
                    gmp_lib.mpz_add_ui(z.Data, z.Data, 1);
                }

                //TODO: only test candidates that +/-1 mod 6
                var test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
                while (test == 0)
                {
                    gmp_lib.mpz_add_ui(z.Data, z.Data, 2);
                    test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
                }
            }
            public IEnumerable<long> PrpGenerator()
            {
                using GmpInt current = ((ulong)NextPrime + 1ul);
                using GmpInt max = ((ulong)MaxPrime + 1ul);
                while (true)
                {
                    GetNextPrime(current);
                    if (gmp_lib.mpz_sizeinbase(current.Data, 2) > 63 ||
                         gmp_lib.mpz_cmp(current.Data, max.Data) >= 0)
                        break;
                    NextPrime = (long)current;
                    yield return NextPrime;
                    gmp_lib.mpz_add_ui(current.Data, current.Data, 2u);
                }

            }
        }

        public class PrpGeneratorNumerics
        {
            long NextPrime;
            long MaxPrime;

            public PrpGeneratorNumerics(int min, int max)
            {
                NextPrime = 1L << min;
                MaxPrime = (1L << max) - 1;
            }
            /// <summary>
            /// returns the next prime number greater than or equal to z
            /// </summary>
            /// <param name="z"></param>
            public static void GetNextPrime(ref BigInteger z)
            {
                if (z.IsEven)
                {
                    z += 1;
                }
                //TODO: only test candidates that +/-1 mod 6
                var test = GmpInt.RabinMiller(z, 20);
                while (test == MathLib.PrimalityType.Composite)
                {
                    z += 2;
                    test = GmpInt.RabinMiller(z, 20);
                }
            }
            public IEnumerable<long> PrpGenerator()
            {
                BigInteger current = ((ulong)NextPrime + 1ul);
                BigInteger max = ((ulong)MaxPrime + 1ul);
                while (true)
                {
                    GetNextPrime(ref current);
                    if (current > MaxPrime)
                        break;
                    NextPrime = (long)current;
                    yield return NextPrime;
                    current += 2;
                }

            }
        }

        public class PrpGeneratorLong
        {
            long NextPrime;
            long MaxPrime;

            public PrpGeneratorLong(int min, int max)
            {
                NextPrime = 1L << min;
                MaxPrime = (1L << max) - 1;
            }
            /// <summary>
            /// returns the next prime number greater than or equal to z
            /// </summary>
            /// <param name="z"></param>
            public static void GetNextPrime(ref long z)
            {
                if ((z & 1) == 0)
                {
                    z += 1;
                }
                //TODO: only test candidates that +/-1 mod 6
                var test = GmpInt.RabinMillerLong(z, 20);
                while (test == MathLib.PrimalityType.Composite)
                {
                    z += 2;
                    test = GmpInt.RabinMillerLong(z, 20);
                }
            }
            public IEnumerable<long> PrpGenerator()
            {
                long current = (long)((ulong)NextPrime + 1ul);
                ulong max = ((ulong)MaxPrime + 1ul);
                while (true)
                {
                    GetNextPrime(ref current);
                    if (current > MaxPrime)
                        break;
                    yield return current;
                    current += 2;
                }

            }
        }

        public class PrpGeneratorLongRabinMiller
        {
            long NextPrime;
            long MaxPrime;

            public PrpGeneratorLongRabinMiller(int min, int max)
            {
                NextPrime = 1L << min;
                MaxPrime = (1L << max) - 1;
            }
            /// <summary>
            /// returns the next prime number greater than or equal to z
            /// </summary>
            /// <param name="z"></param>
            public static void GetNextPrime(ref long z)
            {
                if ((z & 1) == 0)
                {
                    z += 1;
                }
                //TODO: only test candidates that +/-1 mod 6
                var test = LongRabinMiller.RabinMillerLong(z, 20);
                while (test == MathLib.PrimalityType.Composite)
                {
                    z += 2;
                    test = GmpInt.RabinMillerLong(z, 20);
                }
            }
            public IEnumerable<long> PrpGenerator()
            {
                long current = (long)((ulong)NextPrime + 1ul);
                ulong max = ((ulong)MaxPrime + 1ul);
                while (true)
                {
                    GetNextPrime(ref current);
                    if (current > MaxPrime)
                        break;
                    yield return current;
                    current += 2;
                }

            }
        }

    }
}

