/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using HigginsSoft.Math.Lib;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace HigginsSoft.Math.CLI
{
    public class ArgHelper
    {
        public static string[] CommandLineArgs => Environment.GetCommandLineArgs().Skip(1).ToArray();
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Any(x => x == "hitcount"))
            {
                SmoothFactorCountsByBit(64, 1_000_000);
            }
            //QsTest.RunSmallTest64(bits: 16);
            // 18 bits, 8 primes 510510 size.
            // 20 bits, 11 primes 510510 size.
            // 22 bits, 16 primes 510510 size. -> crawls up to 22 primes
            // 24 bits, 16 primes 510510 += 510510 size. -> crawls up to 22 primes
            // 28 bits, 20 primes 510510 +=
            QsParamTest.RunSmallTest64(bits: 28);
            if (args.Length > 0)
            {
                Action RunTest = () => TestPrimeCounts();
                Summary summary;
                switch (args[0])
                {
                    case "rsa1024":
                        RunTest = () => Rsa1024Factoring.Run(args);
                        break;
                    case "threads":
                        RunTest = Mpi.RunThreads;
                        break;
                    case "thread":
                        RunTest = Mpi.RunThread;
                        break;
                    case "count":
                        RunTest = TestPrimeCounts;
                        break;
                    case "range":
                        RunTest = TestPrimeRangeCounts;
                        break;
                    case "mpibench":
                        summary = BenchmarkRunner.Run<MpiBenchmark>();
                        RunTest = () => { };
                        break;
                    case "mpibenchthread":
                        //summary = BenchmarkRunner.Run<MpiBenchmarkThread>();
                        RunTest = () => { };
                        break;
                    case "mpibenchrun":
                        RunTest = MpiBenchmark.Run;
                        break;
                }
                RunTest();
            }

            //Console.WriteLine($"[{DateTime.Now}] Finished.");
        }

        static void SmoothFactorCountsByBit(int maxBits, int rangeSize)
        {
            // for bit 2, we have 2 primes
            // two will hit half of the range
            // three will hit 2/3 of the range, 1/3 for each root

            var hits = new Dictionary<int, int>();
            hits[1] = (int)(rangeSize / 2 + rangeSize * .66);

            // add 32 bit prime hits
            var primeData = PrimeDataHelper.GetPrimeData();
            var keys = primeData.Keys.OrderBy(x => x).ToList();
            foreach (var key in keys)
            {
                var data = primeData[key];
                var bit = data.Bits;
                if (bit <= 2)
                    continue; //skip 2 and 3, already handled as a special case

                var prev = primeData[bit - 1];
                var primesToPrev = prev.Count;
                var primesToBit = data.Count;
                var primeCount = primesToBit - primesToPrev;
                var firstPrime = prev.NextPrime;
                var lastPrime = data.MaxPrime;

                var averagePrime = (decimal)(firstPrime + lastPrime) / 2.0m;
                var hitsPerAveragePrime = (2.0m / averagePrime);
                var averageHitCount = hitsPerAveragePrime * rangeSize;
          
                var totalHits = (int)(averageHitCount * (decimal)(primeCount>>1));
                hits.Add(bit, totalHits);
            }

            using (var sw = new StreamWriter($"hitsbybit_{rangeSize.ToString("N0")}.txt"))
            {
                sw.WriteLine($"Bit\thits");
                foreach (var kvp in hits.OrderBy(x => x.Key))
                {
                    sw.WriteLine($"{kvp.Key}\t{kvp.Value}");
                }
            }

            /*
            for (var bit = 1; bit <= maxBits; bit++)
            {
                //data start = 2^bits
                // data end = 2^(bits+1)-1
                var data = PrimeData.Counts[bit];
                var estimatedQuadraticResidues = data.Count / 2;
                var firstPrime = data.NextPrime;
                var lastPrime = data.MaxPrime;
                var averagePrime = (firstPrime + lastPrime) / 2.0;
                var averageHitCount = (2.0 / averagePrime) * (rangeSize / averagePrime);
                var primeCount = data.Count;
                var totalHits = (int)(averageHitCount * primeCount);
                hits.Add(bit, totalHits);

                // half of the primes are quadratic residues, and hits can be mathematically calculated for a range regardless of N
                // the variables that alter actual hits are:
                // 1. The actual of primes that are quadratic residues for a given N will different
                //      a) - However, in aggregate for larger ranges:
                //              The number of quadratic residues will be the same for a given range, but the actual primes will be different
                //      b) - The roots of the quadratic residues will be different for each prime, so there will be a variance in the hits
                //              for a given N as p approaches N and the after P is larger than N.
                //              However, the probabilities still stand in aggregate.
                // 2. The number of primes that are quadratic residues for a given N will be different

                // each residue will hit 1/p for each for the two roots, so has a total probability of 2/p * p/rangeSize
                // for example, if the size is 100
                // for bits=3
                //      for p= 5, then we have 2/5 * 100/5 = 40 total hits
                //      for p= 7, then we have 2/7 * 100/7 = 28.57 total hits
                // for bits=4
                //      for p= 11, then we have 2/11 * 100/11 = 18.18 total hits
                //      for p= 13, then we have 2/13 * 100/13 = 15.38 total hits
                // for bits=5
                //      for p= 17, then we have 2/17 * 100/17 = 11.76 total hits
                //      for p= 19, then we have 2/19 * 100/19 = 10.53 total hits
                //      for p= 23, then we have 2/23 * 100/23 = 8.69 total hits
                //      for p= 29, then we have 2/29 * 100/29 = 6.90 total hits
                //      for p= 31, then we have 2/31 * 100/31 = 6.45 total hits
                // for bits=6
                //      for p= 37, then we have 2/37 * 100/37 = 5.41 total hits
                //      for p= 41, then we have 2/41 * 100/41 = 4.88 total hits
                //      for p= 43, then we have 2/43 * 100/43 = 4.65 total hits
                //      for p= 47, then we have 2/47 * 100/47 = 4.26 total hits
                //      for p= 53, then we have 2/53 * 100/53 = 3.77 total hits
                //      for p= 59, then we have 2/59 * 100/59 = 3.39 total hits
                //      for p= 61, then we have 2/61 * 100/61 = 3.28 total hits
                // for bits=7
                //      for p= 67, then we have 2/67 * 100/67 = 2.99 total hits
                //      for p= 71, then we have 2/71 * 100/71 = 2.82 total hits
                //      for p= 73, then we have 2/73 * 100/73 = 2.74 total hits
                //      for p= 79, then we have 2/79 * 100/79 = 2.53 total hits
                //      for p= 83, then we have 2/83 * 100/83 = 2.41 total hits
                //      for p= 89, then we have 2/89 * 100/89 = 2.25 total hits
                //      for p= 97, then we have 2/97 * 100/97 = 2.06 total hits
                //      for p= 101, then we have 2/101 * 100/101 = 1.98 total hits
                //      for p= 103, then we have 2/103 * 100/103 = 1.94 total hits
                //      for p= 107, then we have 2/107 * 100/107 = 1.87 total hits
                //      for p= 109, then we have 2/109 * 100/109 = 1.83 total hits
                //      for p= 113, then we have 2/113 * 100/113 = 1.77 total hits
                //      for p= 127, then we have 2/127 * 100/127 = 1.57 total hits
                // for bits=8
                //      for p= 131, then we have 2/131 * 100/131 = 1.53 total hits
                //      for p= 137, then we have 2/137 * 100/137 = 1.46 total hits
                //      for p= 139, then we have 2/139 * 100/139 = 1.44 total hits
                //      for p= 149, then we have 2/149 * 100/149 = 1.34 total hits
                //      for p= 151, then we have 2/151 * 100/151 = 1.32 total hits
                //      for p= 157, then we have 2/157 * 100/157 = 1.27 total hits
                //      for p= 163, then we have 2/163 * 100/163 = 1.22 total hits
                //      for p= 167, then we have 2/167 * 100/167 = 1.19 total hits
                //      for p= 173, then we have 2/173 * 100/173 = 1.15 total hits
                //      for p= 179, then we have 2/179 * 100/179 = 1.11 total hits
                //      for p= 181, then we have 2/181 * 100/181 = 1.10 total hits
                //      for p= 191, then we have 2/191 * 100/191 = 1.05 total hits
                //      for p= 193, then we have 2/193 * 100/193 = 1.03 total hits
                //      for p= 197, then we have 2/197 * 100/197 = 1.01 total hits
                //      for p= 199, then we have 2/199 * 100/199 = 1.00 total hits
                //      for p= 211, then we have 2/211 * 100/211 = 0.95 total hits
                //      for p= 223, then we have 2/223 * 100/223 = 0.89 total hits
                //      for p= 227, then we have 2/227 * 100/227 = 0.88 total hits
                //      for p= 229, then we have 2/229 * 100/229 = 0.87 total hits
                //      for p= 233, then we have 2/233 * 100/233 = 0.85 total hits
                //      for p= 239, then we have 2/239 * 100/239 = 0.84 total hits
                //      for p= 241, then we have 2/241 * 100/241 = 0.83 total hits
                //      for p= 251, then we have 2/251 * 100/251 = 0.79 total hits
                // for bits=9
                //      for p= 257, then we have 2/257 * 100/257 = 0.78 total hits


            }
            */

        }

        static void TestPrimeRangeCounts()
        {
            var args = Environment.GetCommandLineArgs().Where(x => int.TryParse(x, out int result)).ToArray();

            int powerOfTwo = 24;
            if (args.Length > 0 && int.TryParse(args[0], out int powerOfTwoArg))
            {
                powerOfTwo = powerOfTwoArg;
            }

            if (powerOfTwo > 31)
            {
                Console.WriteLine("Starting power of 2 must be between 3 and 30");
            }
            var c = new PrimeCounts();
            var tests = new TestData[] {
                new(nameof(c.TimeRangeCountsUnsafe), ()=> c.TimeRangeCountsUnsafe(powerOfTwo)),
                };
            RunTests(tests);

        }

        static void TestPrimeCounts()
        {
            var args = Environment.GetCommandLineArgs().Where(x => int.TryParse(x, out int result)).ToArray();

            int powerOfTwo = 24;
            if (args.Length > 0 && int.TryParse(args[0], out int powerOfTwoArg))
            {
                powerOfTwo = powerOfTwoArg;
            }
            var c = new PrimeCounts();
            var tests = new TestData[] {
                new(nameof(c.TimeCountsUnsafe), ()=> c.TimeCountsUnsafe(powerOfTwo)),
                new(nameof(c.TimeCountsGeneratorRef), ()=> c.TimeCountsGeneratorRef(powerOfTwo)),
                new(nameof(c.TimeCountsGenerator), ()=> c.TimeCountsGenerator(powerOfTwo)),

                };

            RunTests(tests);
        }

        static void RunTests(IEnumerable<TestData> tests)
        {
            foreach (var test in tests)
            {
                for (var i = 0; i < 5; i++)
                {
                    var result = test.Act();
                    if (i > 0)
                        test.Add(result);

                }
                Console.WriteLine($"Test {test.Name}: {test.Average}");
            }

            Console.WriteLine($"\n\n ----------- Results  ----------- \n\n");
            foreach (var test in tests)
            {
                Console.WriteLine($"Test {test.Name}: {test.Average}");
            }
        }




        public class TestData
        {
            public TestData(string name, Func<TimeSpan> act)
            {
                this.Name = name;
                this.Act = act;
            }

            public string Name { get; }
            public Func<TimeSpan> Act { get; }
            public List<TimeSpan> Timings = new();

            public void Add(TimeSpan value) => Timings.Add(value);
            public TimeSpan Average => Timings.Average();

        }

        static void Junk()
        {
            Console.WriteLine("Hello, World!");
            var m = -1;
            var m2 = -m;

            GmpInt big = ulong.MaxValue;
            BigInteger big2 = (BigInteger)(big * big);

            var data = big.RawData();
            var data2 = ((GmpInt)big2).RawData();
            //Generator.GenerateGcdClass();
        }
    }
    public static class TimespanExtensions
    {
        public static TimeSpan Average(this IEnumerable<TimeSpan> values)
        {
            if (values.Count() == 0) return TimeSpan.Zero;
            var total = values.Sum(x => x.Ticks);
            var average = total / values.Count();
            return TimeSpan.FromTicks(average);
        }
    }
}