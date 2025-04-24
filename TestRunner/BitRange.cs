using HigginsSoft.Math.Lib;
using MathGmp.Native;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;



namespace TestRunner
{
    public class BitRange
    {
        public readonly BigInteger StartValue;
        public readonly BigInteger EndValue;
        public readonly int StartBit;
        public readonly int EndBit;
        public BitRange(int bits)
            : this(bits, bits)
        {

        }
        public BitRange(int startBit, int endBit)
        {
            StartBit = startBit;
            EndBit = endBit;
            var one = BigInteger.One;
            StartValue = one << (startBit - 1);
            EndValue = (one << endBit) - 1;
        }

        internal void ValidateBounds(BigInteger rangeMinValue, BigInteger rangeMaxValue)
        {
            // validate range start and end don't overflow as uint.max.Value
            if (StartValue < rangeMinValue || EndValue > rangeMaxValue || StartValue >= EndValue)
            {
                throw new ArgumentOutOfRangeException($"Range {StartValue} - {EndValue} is out of bounds of {rangeMinValue} - {rangeMaxValue}");
            }
        }
    }
    public class SimplePrpFilter
    {
        private int[] primes;
        private long[] positions = null!;

        public SimplePrpFilter()
        {
            primes = Primes.IntFactorPrimes.ToArray();
        }

        public void TimeTonelliShanks()
        {
            var s = new FactorBaseSieverLongQueue();
            ulong start = 2_201_076_821_857; //1UL << 42;
            ulong end = start + 10_000_000;

            Console.WriteLine($"Timing filters from Start: {start.ToString("N0")} End: {end.ToString("N0")}");

            BigInteger n = RsaChallenge.Rsa1024BigInt;
            using GmpInt gmpN = n;
            using GmpInt gmpRoot = 0;
            using GmpInt gmpp = 0;
  
            var solver = new MathLib.ShanksSolver();


            for (var i = 0; i < 5; i++)
            {
                var count1 = 0;
                var count2 = 0;
                var count3 = 0;
                var sw1 = Stopwatch.StartNew();

                
                foreach (var p in s.NaiveLongPrimeGenerator2(start, end))
                {
                    var root = MathLib.TonelliShanksPy.TonelliShanksAlgo(n, p);
                    if (root>0)
                        count1++;
                }
                sw1.Stop();
                Console.WriteLine($"Count1: {count1.ToString("N0")} in {sw1.Elapsed}");

                var sw2 = Stopwatch.StartNew();

                foreach (var p in s.NaiveLongPrimeGenerator(start, end))
                {
                    count2++;
                }
                sw2.Stop();
                Console.WriteLine($"Count2: {count2.ToString("N0")} in {sw2.Elapsed}");

                var sw3 = Stopwatch.StartNew();

                foreach (var p in s.NaiveLongPrimeGeneratorFiltered(start, end))
                {
                    //gmp_lib.mpz_set_ui(gmpp.Data, (uint)(p >> 32));
                    //gmp_lib.mpz_mul_2exp(gmpp.Data, gmpp.Data, 32);
                    //gmp_lib.mpz_add_ui(gmpp.Data, gmpp.Data, (uint)p);
                    //int result = TonelliShanksPy.TonelliShanks(gmpRoot, gmpN, p);
                    solver.TonelliShanks(gmpRoot, gmpN, p);
                    count3++;
                }
                sw3.Stop();
                Console.WriteLine($"Count3: {count3.ToString("N0")} in {sw3.Elapsed}");



                    
                // print primes per second
                Console.WriteLine($"N / second: {(count1 / sw1.Elapsed.TotalSeconds).ToString("N4")}");
                Console.WriteLine($"F / second: {(count2 / sw2.Elapsed.TotalSeconds).ToString("N4")}");
                Console.WriteLine($"S / second: {(count3 / sw3.Elapsed.TotalSeconds).ToString("N4")}");
            }

        }
        public void TestTonelliShanks()
        {

            var s = new FactorBaseSieverLongQueue();
            ulong start = 1UL << 42;
            ulong end = start + 1_000_000;

            Console.WriteLine($"Timing filters from Start: {start.ToString("N0")} End: {end.ToString("N0")}");


            var count1 = 0;
            var count2 = 0;

            var sw1 = Stopwatch.StartNew();

            BigInteger n = RsaChallenge.Rsa1024BigInt;
            using GmpInt gmpN = n;
            using GmpInt gmpRoot = 0;
            foreach (var p in s.NaiveLongPrimeGenerator2(start, end))
            {
                var root = MathLib.TonelliShanksPy.TonelliShanksAlgo(n, p);
                using GmpInt gmpP = p;
                int result = MathLib.TonelliShanksPy.TonelliShanks(gmpRoot, n, p);
                if (gmpRoot.ToString() != root.ToString())
                {
                    Console.WriteLine($"TonelliShanks failed for {p}");
                }
                count1++;
            }
            sw1.Stop();
            Console.WriteLine($"Count: {count1.ToString("N0")} in {sw1.Elapsed}");

            //var sw2 = Stopwatch.StartNew();

            //foreach (var p in s.NaiveLongPrimeGeneratorFiltered(start, end))
            //{
            //    count2++;
            //}
            //sw2.Stop();
            //Console.WriteLine($"Count: {count2.ToString("N0")} in {sw2.Elapsed}");

            // print primes per second
            Console.WriteLine($"N / second: {(count1 / sw1.Elapsed.TotalSeconds).ToString("N4")}");
            // Console.WriteLine($"F / second: {(count2 / sw2.Elapsed.TotalSeconds).ToString("N4")}");
        }

        public void TimeQrFilter()
        {

            var s = new FactorBaseSieverLongQueue();
            ulong start = 1UL << 42;
            ulong end = start + 100_000_000;

            Console.WriteLine($"Timing filters from Start: {start.ToString("N0")} End: {end.ToString("N0")}");


            var count1 = 0;
            var count2 = 0;

            var sw1 = Stopwatch.StartNew();
            foreach (var p in s.NaiveLongPrimeGenerator(start, end))
            {
                count1++;
            }
            sw1.Stop();
            Console.WriteLine($"Count: {count1.ToString("N0")} in {sw1.Elapsed}");

            var sw2 = Stopwatch.StartNew();

            foreach (var p in s.NaiveLongPrimeGenerator2(start, end))
            {
                count2++;
            }
            sw2.Stop();
            Console.WriteLine($"Count: {count2.ToString("N0")} in {sw2.Elapsed}");

            var size = end - start;

            // print primes per second
            Console.WriteLine($"N / second: {(count1 / sw1.Elapsed.TotalSeconds).ToString("N2")} - i/sec {(size / sw1.Elapsed.TotalSeconds).ToString("N2")}");
            Console.WriteLine($"F / second: {(count2 / sw2.Elapsed.TotalSeconds).ToString("N2")} - i/sec {(size / sw2.Elapsed.TotalSeconds).ToString("N2")}");
        }


        public void TimePrpFilter()
        {

            var s = new FactorBaseSieverLongQueue();
            ulong start = 1UL << 42;
            ulong end = start + 1_000_000;

            Console.WriteLine($"Timing filters from Start: {start.ToString("N0")} End: {end.ToString("N0")}");


            var count1 = 0;
            var count2 = 0;

            var sw1 = Stopwatch.StartNew();
            foreach (var p in s.NaiveLongPrimeGenerator(start, end))
            {
                count1++;
            }
            sw1.Stop();
            Console.WriteLine($"Count: {count1.ToString("N0")} in {sw1.Elapsed}");

            var sw2 = Stopwatch.StartNew();

            foreach (var p in s.NaiveLongPrimeGeneratorFiltered(start, end))
            {
                count2++;
            }
            sw2.Stop();
            Console.WriteLine($"Count: {count2.ToString("N0")} in {sw2.Elapsed}");

            // print primes per second
            Console.WriteLine($"N / second: {(count1 / sw1.Elapsed.TotalSeconds).ToString("N4")}");
            Console.WriteLine($"F / second: {(count2 / sw2.Elapsed.TotalSeconds).ToString("N4")}");
        }

        public static void TestPrpFilter(bool print = false)
        {
            var gen = new SimplePrpFilter();
            var start = 1L;
            var limit = 1000000L;
            var count = 0;
            var sw = Stopwatch.StartNew();
            foreach (var candidate in gen.Generate(start, limit))
            {
                if (print)
                    Console.WriteLine(candidate);
                count++;
            }
            sw.Stop();
            Console.WriteLine($"Count: {count} in {sw.Elapsed}");
        }


        public IEnumerable<long> Generate(long start, long limit)
        {
            // initialize the start position of each candidate so the value is the highest value of P < start; 

            int maxPrime = primes[primes.Length - 1];
            if (start < maxPrime)
            {
                for (var i = 0; i < primes.Length; i++)
                {
                    if (start < primes[i])
                    {
                        start = primes[i];
                        yield return start;
                    }
                }
            }





            // now we have the start value, we can generate the candidates
            // we can generate the candidates by adding the prime to the start value

            // move start to +/-1 %6
            // this is the first candidate
            var inc = 2;
            var startMod = start % 6;
            if (startMod == 0)
            {
                start += 1; inc = 4;// move to 15 mod 6 and set inc to 4 
            }
            else
            {
                // move to 5 mod 6 and leave inc at 2
                if (startMod == 2)
                    start += 3;
                else if (startMod == 3)
                    start += 2; // move to 5 mod 6 and leave inc at 2
                else if (startMod == 4)
                    start += 1;
            }

            primes = primes.Skip(2).ToArray();
            positions = new long[primes.Length];
            // calculate the start offset for each prime
            for (var i = 0; i < primes.Length; i++)
            {
                var prime = primes[i];
                var value = start - (start % prime);
                if (value == 0)
                {
                    value = prime;
                }
                positions[i] = value;

            }


            while (start < limit)
            {
            restart:
                // no need to sieve 2 or 3 since we are generating at +1 or -1 mod 6
                for (var i = 0; i < primes.Length; i++)
                {
                    var prime = primes[i];
                    ref long value = ref positions[i];
                    if (value > start)
                        continue;

                    while (value < start)
                    {
                        value += prime;
                    }
                    if (value == start)
                    {
                        value += prime;
                        start += inc;
                        inc = inc == 2 ? 4 : 2;
                        goto restart;
                    }
                }

                yield return start;
                start += inc;
                inc = inc == 2 ? 4 : 2;

            }

        }
    }
}
