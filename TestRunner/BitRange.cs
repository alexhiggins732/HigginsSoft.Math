using HigginsSoft.Math.Lib;
using MathGmp.Native;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;



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
                    if (root > 0)
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
        public void TimeFindPm1ModP()
        {
            /* factor N+1
             * P1 = 2
                P1 = 2
                P1 = 7
                P2 = 11
                P3 = 107
                P3 = 233
                P4 = 1327
                P10 = 4213988843
                C289 = 3145521369435433644063684274780079279591946774192489531985527059877740442570914100937886940511352613549998337231613510320651027555724854106699954163607672683154553907080373172786314647927992224592991009232315519287947285299252641357082155839999799831996571281727858627921871224016191642513
   */
            var n = RsaChallenge.Rsa1024BigInt;
            var gen = new PrimeGeneratorUnsafeUint();
            var count = 0;
  
            var sw = Stopwatch.StartNew();
            uint last = 0;
            foreach (var p in gen)
            {
                last = p;
                if (count % 1_000_000 == 0)
                {
                    Console.Title = $"[{DateTime.Now}] ({count.ToString("N0")}) {last.ToString("N0")}) - {sw.Elapsed}";
                }
                count++;

            }
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now}] ({count.ToString("N0")}) {last.ToString("N0")}) - {sw.Elapsed}");
        }

        public void TimeFindPm1ModPGcd()
        {
            /* factor N+1
             * P1 = 2
                P1 = 2
                P1 = 7
                P2 = 11
                P3 = 107
                P3 = 233
                P4 = 1327
                P10 = 4213988843
                C289 = 3145521369435433644063684274780079279591946774192489531985527059877740442570914100937886940511352613549998337231613510320651027555724854106699954163607672683154553907080373172786314647927992224592991009232315519287947285299252641357082155839999799831996571281727858627921871224016191642513
   */
            var n = RsaChallenge.Rsa1024BigInt;
            var gen = new PrimeGeneratorUnsafeUint();
            var count = 0;
            var found = 0;
            var sw = Stopwatch.StartNew();
            foreach (var p in gen)
            {
                if (count % 10000 == 0)
                {
                    Console.Title = $"[{DateTime.Now}] ({found.ToString("N0")} in {count.ToString("N0")}) - {sw.Elapsed}";
                }
                count++;
                var res = n % p;
                if (res == p - 1)
                {
                    found++;
                    Console.WriteLine($"[{DateTime.Now}] ({found.ToString("N0")} in {count.ToString("N0")}) {p.ToString("N0")} = 1 mod Rsa1024 - {sw.Elapsed}");
                }
            }
            sw.Stop();
        }
        internal void FactorFind1ModP()
        {
            var n = RsaChallenge.Rsa1024BigInt;
            var f = 2466;
            var q = BigInteger.Parse("234033023631364072706910043447690165193977560164064769975755949879420006941890843266657918920417558958318250779922213092319415841541132057978052877071145");
            var count = 0ul;
            var r = q % f;
            if (r != 1)
            {
                Console.WriteLine($"[{DateTime.Now}] {r.ToString("N0")} = {q.ToString("N0")} % {f} - {count.ToString("N0")} - {r}");
                return;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] {r.ToString("N0")} = {q.ToString("N0")} % {f} - {count.ToString("N0")}");
            }
            var sw = Stopwatch.StartNew();
            for (; count < long.MaxValue; count++)
            {

                if (count % 100_000 == 0)
                {
                    Console.Title = $"[{DateTime.Now}]  {count.ToString("N0")} - {sw.Elapsed}";
                }
                q += f;
                var gcd = BigInteger.GreatestCommonDivisor(q, n);

                if (gcd > 1)// && gcd < n)
                {
                    Console.WriteLine($"[{DateTime.Now}] {gcd.ToString("N0")} = gcd({q.ToString("N0")}, {n.ToString("N0")}) - {sw.Elapsed}");
                    break;
                }


            }
            sw.Stop();

        }
        internal void TimeFind1ModP()
        {
            /* factor N-1
                P1 = 2
                P1 = 3
                P1 = 3
                P3 = 137
                P305 = 54771456150038614497000493219304935092814872508380789750399490612946679077396404636634341910207586959366669619693621878091516796311421002328891523353314783552044209596262450166412392021801443029052356270913828673230945124328722305581380993298951135787866754410919924536135340853339415981812675173250969557
           */
            var n = RsaChallenge.Rsa1024BigInt;
            var gen = new PrimeGeneratorUnsafeUint();
            var count = 0;
            var found = 0;
            var sw = Stopwatch.StartNew();
            foreach (var p in gen)
            {
                if (count % 10000 == 0)
                {
                    Console.Title = $"[{DateTime.Now}] ({found.ToString("N0")} in {count.ToString("N0")}) - {sw.Elapsed}";
                }
                count++;
                var res = n % p;
                if (res == 1)
                {
                    found++;
                    Console.WriteLine($"[{DateTime.Now}] ({found.ToString("N0")} in {count.ToString("N0")}) {p.ToString("N0")} = 1 mod Rsa1024 - {sw.Elapsed}");
                }

            }
            sw.Stop();

        }

        internal void Crank()
        {
            var n = RsaChallenge.Rsa1024BigInt;

            BigInteger gcd = 1;
            BigInteger q = 1;
            var sw = Stopwatch.StartNew();
            ulong count = 0;
            while (gcd == 1)
            {
                if (count % 1000 == 0)
                {
                    Console.Title = $"[{DateTime.Now}] {count.ToString("N0")} - {sw.Elapsed}";
                }

                count++;
                var a = MathLib.Random64();
                var b = MathLib.Random64();
                q = a * b;
                while (q < n)
                {
                    q *= 10;
                }
        
                while (q > 1)
                {
                    gcd = BigInteger.GreatestCommonDivisor(q - 1, n);

                    if (gcd > 1)// && gcd < n)
                    {
                        Console.WriteLine($"[{DateTime.Now}] {gcd.ToString("N0")} = gcd({q.ToString("N0")}, {n.ToString("N0")}) - {sw.Elapsed}");
                        break;
                    }
                    else
                    {
                        q = q / 10;
                    }
                }
                count++;
            }
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now}] Cranked out with {q.ToString("N0")} - {sw.Elapsed}");
        }

        internal void Crank1()
        {
            var n = RsaChallenge.Rsa1024BigInt;
            BigInteger q = 10;
            while (q < n)
            {
                q *= 10;
            }
            var sw = Stopwatch.StartNew();
            while (q > 1)
            {
                var gcd = BigInteger.GreatestCommonDivisor(q - 1, n);

                if (gcd > 1)// && gcd < n)
                {
                    Console.WriteLine($"[{DateTime.Now}] {gcd.ToString("N0")} = gcd({q.ToString("N0")}, {n.ToString("N0")}) - {sw.Elapsed}");
                    break;
                }
                else
                {
                    q = q / 10;
                }
            }
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now}] Cranked out with {q.ToString("N0")} - {sw.Elapsed}");
        }
    }
}
