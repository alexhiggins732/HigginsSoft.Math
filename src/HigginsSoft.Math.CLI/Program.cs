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

            if (args.Length > 0)
            {
                Action RunTest = () => TestPrimeCounts();
                Summary summary;
                switch (args[0])
                {
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