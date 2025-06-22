using Dapper;
using FactoringAlgorithms;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using MathGmp.Native;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Text.Json;


namespace TestRunner
{

    internal class Program
    {
        public static IConfiguration Config;
        static void Main(string[] args)
        {
            // build configuration from appSettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Config = config;
            Console.WriteLine($"Running {AppContext.BaseDirectory}{Path.GetFileName(Process.GetCurrentProcess().ProcessName)}.exe - args: {string.Join(" ", args)}");
            Console.WriteLine($"[{DateTime.Now}] Starting test {nameof(Program)} with working directory {Path.GetFullPath(".")}");

            CommandLine.SetArguments(args);

            ProcessHelper.SetProcessAffinity(Process.GetCurrentProcess());


            bool isTimingTest = args.Any(x => x == "timing");
            if (isTimingTest)
            {
                ProcessHelper.SetProcessAffinity(Process.GetCurrentProcess(), ProcessPriorityClass.AboveNormal);
                var f = new SimplePrpFilter();
                f.TimeTonelliShanks();
                return;
            }
            var efTests = new FactorTest();
            if (args.Any(x => x == "crank"))
            {
                var f = new SimplePrpFilter();
                //f.TimeFindPm1ModP();
                // f.FindPow3Classes();
                int p = 3;
                var idx = args.ToList().IndexOf("crank");
                if (args.Any(x => x == "db"))
                {
                    f.FindDbPowNClasses();
                    return;
                }
                if (idx < args.Length - 1)
                {
                    if (int.TryParse(args[idx + 1], out int p2))
                    {
                        p = p2;
                    }
                }
                f.FindPowNClasses(p);
                return;
            }
            if (args.Any(x => x == "findpm1"))
            {
                var f = new SimplePrpFilter();
                f.TimeFindPm1ModP();
                return;
            }
            if (args.Any(x => x == "updateprogress"))
            {
                JobManager.UpdateProgress();
                return;
            }
            if (args.Any(x => x == "setjob"))
            {
                SetJobTemplate(args);
                return;
            }

            if (args.Any(x => x == "runjobs"))
            {
                RunJobs();
                return;
            }
            if (args.Any(x => x == "test"))
            {
                var n = BigInteger.Parse("1234567890123456789012345678901234567890");
                using var factor = FactorizationBigInteger.Factor(n, false, true, skipFermat: true, skipRho: true, skipRhoP2: true, skipRhoP3: true, skipRhoZ: true, skipPP1: true, skipPM1: true, skipECM: true, skipQS: true, skipFact: false);
                Console.WriteLine($"[{DateTime.Now}] {n} = {string.Join(" * ", factor.Factors.Select(x => x.P))} - {factor.Factors.Count} factors - {factor.GetProduct()}");
                return;
            }
            if (args.Any(x => x == "updateoffsets"))
            {
                var FactorBaseSiever = new DbFactorBaseSieve();
                FactorBaseSiever.SaveOffsets();
                return;
            }

            if (args.Any(x => x == "verify"))
            {
                bool verifyPrimality = false;
                if (args.Length > 1)
                {
                    bool.TryParse(args[1], out verifyPrimality);
                }
                efTests.VerifyFactorBase(verifyPrimality);
                return;
            }


            if (args.Any(x => x == "tonelli"))
            {
                efTests.TestTonelli();
                return;
            }

            if (args.Any(x => x == "factorbasesieve"))
            {
                int startBits = 0;
                int endBits = 15;

                if (args.Length > 1 && int.TryParse(args[1], out startBits)) { }
                if (args.Length > 2 && int.TryParse(args[2], out endBits)) { }
                var FactorBaseSiever = new FactorBaseSiever();
                FactorBaseSiever.SieveFactorBaseIntPrimes(startBits, endBits);
                return;
            }

            if (args.Any(x => x == "processqueue"))
            {
                bool.TryParse(args.Length > 1 ? args[1] : bool.TrueString, out bool needsLock);
                FactoringQueue.ProcessQueue(needsLock);
                return;
            }
            if (args.Any(x => x == "bat"))
            {
                FactoringQueue.ProcessBatchFiles();
                return;
            }

            if (args.Any(x => x == "verifyprocessqueue"))
            {
                FactoringQueue.VerifyProcessed();
                return;
            }

            if (args.Any(x => x == "factorbasesieveuint"))
            {
                int bit = 32;
                if (args.Length > 1 && int.TryParse(args[1], out bit)) { }
                var FactorBaseSiever = new FactorBaseSieverUint();
                FactorBaseSiever.SieveFactorBaseUintPrimes(bit);
                return;
            }
            if (args.Any(x => x == "factorbasesievelong"))
            {
                int bit = 33;
                if (args.Length > 1 && int.TryParse(args[1], out bit)) { }
                var FactorBaseSiever = new FactorBaseSieverLong();
                FactorBaseSiever.SieveFactorBaseLongPrimes(bit);
                return;
            }

            if (args.Any(x => x == "factorbasesievelongqueue"))
            {
                int bit = 37;
                if (args.Length > 1 && int.TryParse(args[1], out bit)) { }
                var FactorBaseSiever = new FactorBaseSieverLongQueue();
                FactorBaseSiever.SieveFactorBaseLongPrimes(bit);
                return;
            }

            if (args.Any(x => x == "factorbasesievelongmod3"))
            {
                int bit = 33;
                if (args.Length > 1 && int.TryParse(args[1], out bit)) { }
                var FactorBaseSiever = new FactorBaseSieverLongMod3();
                FactorBaseSiever.SieveFactorBaseLongPrimes(bit);
                return;
            }
            if (args.Any(x => x == "factorbasesievebig"))
            {
                int bit = 33;
                if (args.Length > 1 && int.TryParse(args[1], out bit)) { }
                var FactorBaseSiever = new FactorBaseSieverNumerics();
                FactorBaseSiever.SieveFactorBase(bit);
                return;
            }

            if (args.Any(x => x == "job"))
            {
                var runner = new JobRunner();
                runner.RunJob(args);
                return;
            }

            if (args.Any(x => x == "dbfactorbasesieve"))
            {
                var FactorBaseSiever = new DbFactorBaseSieve();
                FactorBaseSiever.SieveDbPrimes();
                return;
            }
            if (args.Any(x => x == "trialdivide"))
            {
                var test = new FactorTest();
                test.TestTrialDivide();
                return;
            }
            if (args.Any(x => x == "tinyecm"))
            {
                var test = new FactorTest();
                test.TinyEcm();
                return;
            }
            if (args.Any(x => x == "offsets"))
            {
                //FactorDbHelper.TestFactorial();
                // Example: c1 starts at 4 and steps 11, while c2 starts at 17 and steps 19.
                //var c1 = new MathLib.CongruenceNumerics(4, 11);
                //var c2 = new MathLib.CongruenceNumerics(17, 19);
                //var sol = MathLib.CRTNumerics((new[] { c1, c2 }).ToList());
                // sol[0] = n = step size
                // sol[1] = solution = where congruences first meet.
                // example, c1 starts at 4 and steps 11, while c2 starts at 17 and steps 19.
                // The first time they meet is at 169 = 4 + 11 * 15 and 17 + 19 * 8 = 36. So the solution is n = 169, step size = 11 * 19 = 209.
                // To determine the start index k for each congruence, we can use the formula k = (solution - start) / p.
                //      k = 169 - 4 = 165 / 11 = 15
                //      k = 169 - 17 = 152 / 19 = 8

                var findNext = args.Any(x => x == "next");
                if (args.Length > 3 && BigInteger.TryParse(args[1], out BigInteger n1) && BigInteger.TryParse(args[2], out BigInteger a) && BigInteger.TryParse(args[3], out BigInteger b))
                {

                    //sol[0] where the congruences meet
                    //sol[1] step size in between
                    /*
                    var c1 = new MathLib.CongruenceNumerics(4, 11);
                    var c2 = new MathLib.CongruenceNumerics(17, 19);
                    var sol = MathLib.CRTNumerics((new[] { c1, c2 }).ToList());
                    sol[0] where the congruences meet
                    sol[1] step size in between
                    */

                    var root = n1.Sqrt();
                    var offsetsA = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n1, a, root);
                    var offsetsB = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n1, b, root);


                    var a1b1 = MathLib.CRTNumerics((new MathLib.CongruenceNumerics[] { new(offsetsA.Item1, a), new(offsetsB.Item1, b) }).ToList());
                    var a1b2 = MathLib.CRTNumerics((new MathLib.CongruenceNumerics[] { new(offsetsA.Item1, a), new(offsetsB.Item2, b) }).ToList());
                    var a2b1 = MathLib.CRTNumerics((new MathLib.CongruenceNumerics[] { new(offsetsA.Item2, a), new(offsetsB.Item1, b) }).ToList());
                    var a2b2 = MathLib.CRTNumerics((new MathLib.CongruenceNumerics[] { new(offsetsA.Item2, a), new(offsetsB.Item2, b) }).ToList());

                    Console.WriteLine($"n = {n1}");
                    Console.WriteLine($"root = {root}");
                    Console.WriteLine($"a = {a}");
                    Console.WriteLine($"b = {b}");
                    Console.WriteLine($"c = {a * b}");
                    Console.WriteLine($"a1 = {offsetsA.Item1}");
                    Console.WriteLine($"a2 = {offsetsA.Item2}");
                    Console.WriteLine($"b1 = {offsetsB.Item1}");
                    Console.WriteLine($"b2 = {offsetsB.Item2}");

                    Console.WriteLine($"a1b1n = {a1b1.N}");
                    Console.WriteLine($"a1b1sol = {a1b1.Solution}");
                    Console.WriteLine($"a1b2n = {a1b2.N}");
                    Console.WriteLine($"a1b2sol = {a1b2.Solution}");
                    Console.WriteLine($"a2b1n = {a2b1.N}");
                    Console.WriteLine($"a2b1sol = {a2b1.Solution}");
                    Console.WriteLine($"a2b2n = {a2b2.N}");
                    Console.WriteLine($"a2b2sol = {a2b2.Solution}");

                    Console.WriteLine($"qxa1b1 = {BigInteger.ModPow(root + a1b1.Solution, 2, n1)}");
                    Console.WriteLine($"qxa1b2 = {BigInteger.ModPow(root + a1b2.Solution, 2, n1)}");
                    Console.WriteLine($"qxa2b1 = {BigInteger.ModPow(root + a2b1.Solution, 2, n1)}");
                    Console.WriteLine($"qxa2b2 = {BigInteger.ModPow(root + a2b2.Solution, 2, n1)}");

                    Console.WriteLine($"rqxa1b1 = {BigInteger.ModPow(root + a1b1.Solution, 2, n1) / a1b1.N}");
                    Console.WriteLine($"rqxa1b2 = {BigInteger.ModPow(root + a1b2.Solution, 2, n1) / a1b2.N}");
                    Console.WriteLine($"rqxa2b1 = {BigInteger.ModPow(root + a2b1.Solution, 2, n1) / a2b1.N}");
                    Console.WriteLine($"rqxa2b2 = {BigInteger.ModPow(root + a2b2.Solution, 2, n1) / a2b2.N}");

                    Console.WriteLine($"modexp(root+a1b1sol, 2,n)%a = {BigInteger.ModPow(root + a1b1.Solution, 2, n1) % a}");
                    Console.WriteLine($"modexp(root+a1b1sol, 2,n)%b = {BigInteger.ModPow(root + a1b1.Solution, 2, n1) % b}");

                    Console.WriteLine($"modexp(root+a1b2sol, 2,n)%a = {BigInteger.ModPow(root + a1b2.Solution, 2, n1) % a}");
                    Console.WriteLine($"modexp(root+a1b2sol, 2,n)%b = {BigInteger.ModPow(root + a1b2.Solution, 2, n1) % b}");

                    Console.WriteLine($"modexp(root+a2b1sol, 2,n)%a = {BigInteger.ModPow(root + a2b1.Solution, 2, n1) % a}");
                    Console.WriteLine($"modexp(root+a2b1sol, 2,n)%b = {BigInteger.ModPow(root + a2b1.Solution, 2, n1) % b}");

                    Console.WriteLine($"modexp(root+a2b2sol, 2,n)%a = {BigInteger.ModPow(root + a2b2.Solution, 2, n1) % a}");
                    Console.WriteLine($"modexp(root+a2b2sol, 2,n)%b = {BigInteger.ModPow(root + a2b2.Solution, 2, n1) % b}");


                    //var test0 = root + offsets.Item1;
                    //var qx0 = MathLib.PowerMod(test0, 2, n0);
                    //var check0 = qx0 % p0;

                    //var test1 = root + offsets.Item2;
                    //var qx1 = MathLib.PowerMod(test1, 2, n0);
                    //var check1 = qx1 % p0;

                    //Console.WriteLine($"c0: {check0}");
                    //Console.WriteLine($"c1: {check1}");


                    //Console.WriteLine($"0: {offsets.Item1}");
                    //Console.WriteLine($"1: {offsets.Item2}");
                    //Console.WriteLine($"p: {p0}");

                }
                else if (args.Length > 2 && BigInteger.TryParse(args[1], out BigInteger n0) && BigInteger.TryParse(args[2], out BigInteger p0))
                {
                    var root = n0.Sqrt();
                    var offsets = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n0, p0, root);

                    if (findNext)
                    {
                        while (offsets.Item1 < 0 || offsets.Item2 < 0)
                        {
                            using var gp = (GmpInt)(p0 + 2);
                            using var gnext = MathUtil.GetNextPrime(gp);
                            p0 = (BigInteger)gnext;
                            Console.WriteLine($"Testing: {p0}");
                            offsets = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n0, p0);


                        }


                        var test0 = root + offsets.Item1;
                        var qx0 = MathLib.PowerMod(test0, 2, n0);
                        var check0 = qx0 % p0;

                        var test1 = root + offsets.Item2;
                        var qx1 = MathLib.PowerMod(test1, 2, n0);
                        var check1 = qx1 % p0;

                        Console.WriteLine($"n = {n0}");
                        Console.WriteLine($"root = {root}");

                        Console.WriteLine($"c0: {check0}");
                        Console.WriteLine($"c1: {check1}");


                        Console.WriteLine($"0: {offsets.Item1}");
                        Console.WriteLine($"1: {offsets.Item2}");
                        Console.WriteLine($"p: {p0}");
                    }
                    else
                    {
                        Console.WriteLine($"n = {n0}");
                        Console.WriteLine($"root = {root}");
                        Console.WriteLine($"p0 = {p0}");
                        Console.WriteLine($"a0 = {offsets.Item1}");
                        Console.WriteLine($"a1 ={offsets.Item2}");
                        Console.WriteLine($"modexp(root+a0, 2,n)%p0 = {BigInteger.ModPow(root + offsets.Item1, 2, n0) % p0}");
                        Console.WriteLine($"modexp(root+a1, 2,n)%p0 = {BigInteger.ModPow(root + offsets.Item2, 2, n0) % p0}");
                    }

                }
                else if (args.Length > 1 && BigInteger.TryParse(args[1], out BigInteger p))
                {
                    var n = RsaChallenge.Rsa1024BigInt;
                    var root = n.Sqrt();
                    var offsets = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, p);

                    Console.WriteLine($"n = {n}");
                    Console.WriteLine($"root = {root}");
                    Console.WriteLine($"px = {p}");
                    Console.WriteLine($"a0x = {offsets.Item1}");
                    Console.WriteLine($"a1x ={offsets.Item2}");
                    Console.WriteLine($"modexp(root+a0x, 2,n)%px = {BigInteger.ModPow(root + offsets.Item1, 2, n) % p}");
                    Console.WriteLine($"modexp(root+a1x, 2,n)%px = {BigInteger.ModPow(root + offsets.Item2, 2, n) % p}");
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now}] Error parsing {args[1]}");
                }
                return;
            }

            if (args.Any(x => x == "factortdiv"))
            {
                var test = new FactorTest();
                test.FactorTDiv();
                return;
            }
            if (args.Length > 2 && args.Any(x => x == "add") && int.TryParse(args[1], out int factorizationId) && !string.IsNullOrWhiteSpace(args[2]))
            {
                var factorString = args[2];
                if (bool.Parse(bool.FalseString))
                {
                    factorizationId = 5;
                    factorString = "3646462392280632369106560642497975540630143537"; //args[2]
                }
                var helper = new FactorDbHelper();
                Console.WriteLine("Executing AddFactor({0}, {1})", factorizationId, factorString);
                helper.AddFactor(factorizationId, factorString);
                return;
            }
            if (args.Length > 2 && args.Any(x => x == "tdiv") && int.TryParse(args[1], out factorizationId) && int.TryParse(args[2], out int tDiv))
            {
                var factorString = args[2];
                if (bool.Parse(bool.FalseString))
                {
                    factorizationId = 5;
                    //factored with ecm_gpu -gpu -c 4 -i 10 43e6
                    // actual curves was 4352, supposedly calculated by cuda per GMP-ECM readme.
                    tDiv = 50;
                }
                var helper = new FactorDbHelper();
                helper.SetTDiv(factorizationId, tDiv);
                return;
            }

            //return;
            //efTests.ProcessUnknownFactors();
            //efTests.UpdateFactorizationPrimality();
            /*
             * var FactorBaseSiever = new FactorBaseSiever();
            FactorBaseSiever.SieveDbPrimes();
            */
            //efTests.ProcessUnknownFactors();
            if (args.Length >= 3 && int.TryParse(args[0], out int minDigits) && int.TryParse(args[1], out int maxDigits) && int.TryParse(args[2], out int batchSize))
            {
                efTests.ProcessDbFactors(minDigits, maxDigits, batchSize);
            }
            else if (args.Length >= 2 && int.TryParse(args[0], out minDigits) && int.TryParse(args[1], out maxDigits))
            {
                efTests.ProcessDbFactors(minDigits, maxDigits);
            }
            else if (args.Length >= 1 && int.TryParse(args[0], out maxDigits))
            {
                efTests.ProcessDbFactors(0, maxDigits);
            }
            else
            {
                //efTests.ProcessBatchFile();
                efTests.ProcessDbFactors();
            }

            //efTests.TDiv20();

        }

        private static void SetJobTemplate(string[] args)
        {
            //testrunner setjobs 1 12 
            Console.WriteLine($"[{DateTime.Now}] Setting Job Template");
            var client = int.Parse(args[1]);
            var clients = int.Parse(args[2]);
            var jobsSize = Environment.ProcessorCount;
            var threads = clients * jobsSize;
            var jsonFilePath = Path.Combine(Path.GetFullPath("."), "job-template.json");

            var template = JsonSerializer.Deserialize<JobTemplate>(File.ReadAllText(jsonFilePath));
            template.Threads = threads;
            template.StartThread = (client - 1) * jobsSize;
            template.EndThread = (client * jobsSize) - 1;
            template.JobStart = DateTime.Now.ToString();
            File.WriteAllText(jsonFilePath, JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true }));

        }

        static void RunSplitEcm(string[] args)
        {

            var idx = args.ToList().IndexOf("n");
            var n = args[idx + 1];
            idx = args.ToList().IndexOf("threads");

            int.TryParse(args[idx + 1], out int numThreads);
            if (numThreads == 0)
                numThreads = 1;


            var curDir = Path.GetFullPath(".");
            var baseDir = Path.Combine(curDir, "jobs");
            var workStart = 30;
            bool setMaxWork = false;
            for (var i = 0; i < numThreads; i++)
            {
                var work = workStart + (5 * i);
                var pretest = work + 2;
                if (work >= 65)
                {
                    if (setMaxWork)
                        Console.WriteLine($"No more work after {65}");
                    else
                    {
                        work = 64;
                        pretest = 65;
                    }
                }

                var cmd = $"\"{curDir}\\binaries\\yafu-x64.exe\"";
                var cmdArgs = $"-work {work} -pretest {pretest} factor({n})";
                var workingDirectory = Path.Combine(baseDir, $"job_{i}");

                Directory.CreateDirectory(workingDirectory);
                File.WriteAllLines(Path.Combine(workingDirectory, "yafu.ini"), ["threads=2"]);
                var info = new ProcessStartInfo()
                {
                    FileName = cmd,
                    Arguments = cmdArgs,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = true
                };

                Process.Start(info);
            }
        }

        private static void RunJobs()
        {
            /*
            {
                "Args": "factorbasesievelongqueue 41 job sieve2p42T[thread] 2 thread [thread] threads [threads]",
                "Threads": 16,
                "StartThread": 0,
                "EndThread": 15
            }
             * */
            var jsonFilePath = Path.Combine(Path.GetFullPath("."), "job-template.json");
            if (!File.Exists(jsonFilePath))
            {
                var example = new JobTemplate()
                {
                    Args = "factorbasesievelongqueue 41 job sieve2p42T[thread] 2 thread [thread] threads [threads]",
                    Threads = 16,
                    StartThread = 0,
                    EndThread = 15
                };
                Console.WriteLine($"Failed to find {jsonFilePath}. Example:");
                Console.WriteLine(JsonSerializer.Serialize(example, new JsonSerializerOptions { WriteIndented = true }));
                return;
            }
            var json = File.ReadAllText(jsonFilePath);
            var template = JsonSerializer.Deserialize<JobTemplate>(json);
            if (template == null)
            {
                Console.WriteLine($"Failed to deserialize {jsonFilePath}");
                return;
            }

            new FactorTest().UpdateBenchmarkReport();

            for (var i = template.StartThread; i <= template.EndThread; i++)
            {
                var args = template.Args
                    .Replace("[threads]", $"{template.Threads}")
                    .Replace("[thread]", $"{i}");

                if (!args.StartsWith("testrunner"))
                {
                    args = $"testrunner {args}";
                }
                //$"testrunner factorbasesievelongqueue 41 job sieve2p42T{i} 2 thread {i} threads 16";
                // process creation overhead makes gmp-ecm inefficient for small jobs. Need to code pipe to pass data to the process through standard in.
                //args = $"testrunner 0 180 100 pm1 250000 digits 19 thread {i} threads 32";
                var info = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/k {args}",
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = AppContext.BaseDirectory,
                    UseShellExecute = true // required to open in a new window
                };
                using var process = new Process
                {
                    StartInfo = info
                };

                process.Start();
            }
        }
        public class JobTemplate
        {
            public string Args { get; set; } = ""; // factorbasesievelongqueue 41 job sieve2p42T[thread] 2 thread [thread] threads [threads]
            public int Threads { get; set; } = 0;
            public int StartThread { get; set; } = 0;
            public int EndThread { get; set; } = 0;
            public string JobStart { get; set; } = DateTime.Now.ToString();
        }
    }

    public class BenchmarkSettings
    {
        public string FileName { get; set; } = "benchmark.rpt";
        public string Url { get; set; } = "http://localhost:5000/api/benchmark";

    }
    public class FactorTest
    {

        public void TestTonelli()
        {
            var n = RsaChallenge.Rsa1024BigInt;
            var offsets = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n);
            foreach (var solution in offsets)
            {
                Console.WriteLine($"{solution.Key}\t{solution.Value.First()}\t{solution.Value.Last()}");
            }



            MathLib.TonelliShanksPy.Factorize(n);


            var sqrt = n.Sqrt();
            var smallPrimes = GetPrimesTo(1000);
            var modRoots = smallPrimes.Select(x => new { P = x, Offset = MathLib.TonelliShanks2.ModSqrt(n, x) })
                .Where(x => x.Offset > -1).ToList();
            var solutions = modRoots.Select(x =>
            {
                var solutions2 = MathLib.TonelliShanks2.SolveResidueOffsets(n, x.Offset, x.P);
                var solutions = MathLib.TonelliShanks.GetSolutions(n, x.P);
                var solutions3 = MathLib.TonelliShanksPy.TonelliShanksAlgo(n, x.P);

                return new
                {
                    x.P,
                    x.Offset,
                    Class1 = solutions2.a,
                    Class2 = solutions2.b,
                };
            }).ToList();



            foreach (var solution in solutions)
            {
                Console.WriteLine($"{solution.P}: {solution.Offset} - Class1: {solution.Class1} - Class2: {solution.Class2}");
            }


        }

        void Log(string message, bool appendDate = true, bool appendThread = true)
        {
            if (appendThread)
            {
                message = $"[{Thread}] {message}";
            }

            if (appendDate)
            {
                message = $"[{DateTime.Now}] {message}";
            }

            Debug.WriteLine(message);
            Console.WriteLine(message);

        }

        IEnumerable<int> GetPrimesTo(int max)
        {
            var gen = new PrimeGenerator((int)max);
            var result = gen.ToList();
            return result;
        }

        public void ProcessBatchFile()
        {
            var path = @"C:\factor\input.txt";

            var lines = File.ReadAllLines(path);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                Console.Title = $"({i + 1} of {lines.Length}) Processing {line}";
                if (line.StartsWith("factor("))
                {
                    line = line.Substring("factor(".Length).Trim().TrimEnd(')');

                }
                BigInteger n = BigInteger.Parse(line);
                var sw = Stopwatch.StartNew();
                var fact = FactorizationBigInteger.Factor(n, false, true);

                fact.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                var composites = fact.Factors.Where(x => x.FactorType != MathLib.PrimalityType.ProbablePrime && x.FactorType != MathLib.PrimalityType.Prime).ToList();
                foreach (var c in composites)
                {
                    fact.Factors.Remove(c);
                    var subfac = FactorizationBigInteger.Factor(c.P, false, true);
                    if (c.Power > 1)
                    {
                        Console.WriteLine("Need to handle powers");
                    }
                    fact.Add(subfac);
                }

                sw.Stop();
                if (i % 10 == 0)
                    Console.WriteLine($"[{DateTime.Now}] {n} = {string.Join(" * ", fact.Factors.Select(x => x.P))} - {fact.Factors.Count} factors in {sw.Elapsed}");

                var payload = new DbFactorization
                {
                    Id = 0,
                    N = n.ToString(),
                    Type = PrimalityType.Unknown,
                    TDiv = 0,
                    Factors = fact.Factors.Select(x => new DbFactor
                    {
                        P = x.P.ToString(),
                        Power = x.Power,
                        Type = (PrimalityType)(int)x.FactorType,
                        Digits = x.P.ToString().Length,
                        Bits = MathLib.BitLength(x.P)
                    }).ToList()
                };


            }
        }

        public void SetConnectionString()
        {
            FactorDbContext.DbConnectionString =
         "Server=192.168.2.170;Database=Factors;user=factor;password=F@act0#1;MultipleActiveResultSets=true;TrustServerCertificate=True;Command Timeout=1000";

        }


        public void VerifyFactorBase(bool verifyPrimality, int startId = 0)
        {
            SetConnectionString();
            using var serviceProvider = new ServiceCollection()
                       .AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString))
                       .BuildServiceProvider();

            //var startId = 770000;
            int idx = 0;
            while (true)
            {
                var sw = Stopwatch.StartNew();
                List<DbFactorization> unFactored = new();
                using var scope = serviceProvider.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<FactorDbContext>();
                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {

                        unFactored = dbContext.Factorizations
                              .Include(x => x.Factors)
                              .OrderBy(x => x.Id)
                              .Where(x => x.Id >= startId)
                              .Take(250_000)

                              .ToList();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                selectWatch.Stop();
                if (!unFactored.Any())
                    break;
                startId = unFactored.Max(x => x.Id) + 1;
                var factorWatch = Stopwatch.StartNew();
                bool hasUpdates = false;

                foreach (var dbFactorization in unFactored)
                {
                    var bigN = BigInteger.Parse(dbFactorization.N);
                    var precheck = dbFactorization.Factors.Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power)).Aggregate((a, b) => a * b);

                    if (precheck == bigN)
                    {
                        if (!verifyPrimality)
                            continue;
                        foreach (var f in dbFactorization.Factors)
                        {
                            var bigFi = BigInteger.Parse(f.P);
                            var fCheckType = GmpInt.Primality(bigFi);
                            var ifCheckType = (int)fCheckType;
                            var ifType = (int)f.Type;
                            if (ifType != ifCheckType)
                            {
                                f.Type = (PrimalityType)ifCheckType;
                                hasUpdates = true;
                            }

                        }
                        var checkType = dbFactorization.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                        if ((int)dbFactorization.Type != (int)checkType)
                        {
                            dbFactorization.Type = (PrimalityType)(int)checkType;
                            hasUpdates = true;
                        }
                        continue;
                    }
                    Console.WriteLine($"[{DateTime.Now}] {dbFactorization.Id.ToString("N0")} - Check Failed {bigN.ToString()} != {precheck.ToString()}");
                    hasUpdates = true;
                    var check = new FactorizationBigInteger();


                    var distinctFactors = dbFactorization.Factors.ToLookup(x => x.P).Select(x => x.First()).ToList();
                    foreach (var dbFact in distinctFactors)
                    {
                        var bigInt = BigInteger.Parse(dbFact.P);
                        var f = new Factor<BigInteger>(bigInt, 0);

                        while (bigN % bigInt == 0 && bigN > 1)
                        {
                            bigN /= bigInt;
                            f.Power++;
                        }
                        if (f.Power > 0)
                        {
                            check.Add(bigInt, f.Power);
                        }
                    }

                    if (bigN > 1)
                    {
                        var f = new Factor<BigInteger>(bigN, 1);
                        check.Add(bigN, f.Power);
                    }

                    check.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                    var value = check.GetProduct();
                    if (value.ToString() != dbFactorization.N)
                    {
                        Console.WriteLine($"[{DateTime.Now}] Error {dbFactorization.Id} {dbFactorization.N} != {value}");
                        continue;
                    }
                    dbFactorization.Factors.Clear();
                    dbFactorization.Factors.AddRange(check.Factors.OrderBy(x => x.P.GetBitLength()).ThenBy(x => x.P.ToString()).Select(x => new DbFactor
                    {
                        P = x.P.ToString(),
                        Power = x.Power,
                        Type = (PrimalityType)(int)x.FactorType,
                        Digits = x.P.ToString().Length,
                        Bits = MathLib.BitLength(x.P)
                    }));
                    dbFactorization.Type = dbFactorization.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                }


                factorWatch.Stop();
                Stopwatch saveWatch = Stopwatch.StartNew();
                if (hasUpdates)
                {

                    sleep = 10;
                    for (var i = 0; i < 10; i++)
                    {
                        try
                        {
                            dbContext.SaveChanges();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                            System.Threading.Thread.Sleep(sleep);
                            sleep *= 2;
                        }
                    }


                }
                saveWatch.Stop();
                sw.Stop();
                Log($"{unFactored.Last().Id.ToString("N0")} Factored {unFactored.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factorWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed}");


            }

        }

        public void ProcessUnknownFactors()
        {
            SetConnectionString();
            using var serviceProvider = new ServiceCollection()
                       .AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString))
                       .BuildServiceProvider();

            var startId = 0;
            int idx = 0;
            while (true)
            {
                var sw = Stopwatch.StartNew();
                List<DbFactorization> unFactored = new();
                using var scope = serviceProvider.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<FactorDbContext>();
                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {

                        unFactored = dbContext.Factorizations
                              .Include(x => x.Factors)
                              .Where(x => x.Id > startId &&
                                    x.Factors.Any(f => f.Type == PrimalityType.Unknown)
                                   && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite))
                              .OrderBy(x => x.Id)
                              .Take(10000)
                              .ToList();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                selectWatch.Stop();
                if (!unFactored.Any())
                    break;
                startId = unFactored.Max(x => x.Id) + 1;
                var factorWatch = Stopwatch.StartNew();
                foreach (var fact in unFactored)
                {
                    idx++;
                    if (idx % 100 == 0)
                    {
                        Console.Title = $"({idx}) Processing {fact.Id}";
                    }
                    var smallFactors = fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList();
                    foreach (var smallFactor in smallFactors)
                    {
                        if (smallFactor.Type == PrimalityType.Unknown)
                        {
                            smallFactor.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(smallFactor.P));
                            if (smallFactor.Type != PrimalityType.Composite)
                                continue;
                        }
                        if (smallFactor.P.Length > 30)
                        {
                            continue;
                        }

                        var n = BigInteger.Parse(smallFactor.P);
                        var thisfactorWatch = Stopwatch.StartNew();
                        using var factored = FactorizationBigInteger.Factor(n, false, true);
                        thisfactorWatch.Stop();
                        factored.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                        if (factored.Factors.Count > 1)
                        {


                            // recursively factor small composites less than 20 digits
                            var composites = factored.Factors.Where(x => x.P.ToString().Length <= 20 && (x.FactorType != MathLib.PrimalityType.ProbablePrime && x.FactorType != MathLib.PrimalityType.Prime)).ToList();
                            foreach (var c in composites)
                            {
                                factored.Factors.Remove(c);
                                thisfactorWatch.Start();
                                using var subfac = FactorizationBigInteger.Factor(c.P, false, true);
                                thisfactorWatch.Stop();
                                if (c.Power > 1)
                                {
                                    Console.WriteLine("Need to handle powers");
                                }
                                subfac.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                                factored.Add(subfac);
                                subfac.Dispose();
                            }
                            composites.Clear();
                            composites = null;
                            fact.Factors.Remove(smallFactor);
                            fact.Factors.AddRange(factored.Factors.Select(x => new DbFactor
                            {
                                P = x.P.ToString(),
                                Power = x.Power,
                                Type = (PrimalityType)x.FactorType,
                                Digits = x.P.ToString().Length,
                                Bits = MathLib.BitLength(x.P)
                            }));

                        }
                        factored.Dispose();
                    }

                    fact.Type = fact.Factors.All(x => x.Type == PrimalityType.ProbablePrime || x.Type == PrimalityType.Prime) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                }

                factorWatch.Stop();
                Stopwatch saveWatch = Stopwatch.StartNew();
                sleep = 10;
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        dbContext.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }

                saveWatch.Stop();
                sw.Stop();
                Log($"{unFactored.Last().Id.ToString("N0")} Factored {unFactored.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factorWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed}");


            }

        }

        public void UpdateFactorizationPrimality()
        {
            SetConnectionString();
            using var serviceProvider = new ServiceCollection()
                       .AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString))
                       .BuildServiceProvider();

            var startId = 0;
            int idx = 0;
            while (true)
            {
                using var scope = serviceProvider.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<FactorDbContext>();
                List<DbFactorization> unFactored = new();
                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {
                        unFactored = dbContext.Factorizations
                              .Include(x => x.Factors)
                              .Where(x => x.Id > startId &&
                                   x.Factors.Any(f => (int)f.Type < 1)
                                   && (x.Type > 0))
                              .OrderBy(x => x.Id)
                              .Take(1000)
                              .ToList();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }

                selectWatch.Stop();
                if (!unFactored.Any())
                {
                    Log($"No more factors to process after Id={startId}");
                    break;
                }


                Log($"Running batch - {unFactored.Min(x => x.Id)} - {unFactored.Max(x => x.Id)}");

                startId = unFactored.Max(x => x.Id) + 1;
                foreach (var dbFact in unFactored)
                {
                    idx++;
                    dbFact.Type = PrimalityType.Composite;
                    if (idx % 100 == 0)
                    {
                        Console.Title = $"({idx}) Processing {dbFact.Id}";
                    }
                }

                var saveWatch = Stopwatch.StartNew();
                sleep = 10;

                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        dbContext.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
            }
        }

        public int Thread = -1;

        public void UpdateBenchmarkReport()
        {
            var config = FactorConfig.GetCommandLineConfig();
            if (config.Offline)
            {
             
                var benchmarkSettings = new BenchmarkSettings();

                Log($"Updated in offline benchmark - {config.OfflineFiilePath}");
                Program.Config.Bind("Benchmark", benchmarkSettings);
                benchmarkSettings.FileName = Path.Combine(AppContext.BaseDirectory, benchmarkSettings.FileName);
                config.OfflineFiilePath = benchmarkSettings.FileName;

                if (File.Exists(config.OfflineFiilePath))
                {
                    File.Delete(config.OfflineFiilePath);
                }

                if (!File.Exists(benchmarkSettings.FileName))
                {
                    Log($"Downloading Benchmark file {benchmarkSettings.FileName}");

                    var downloadWatch = Stopwatch.StartNew();
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(benchmarkSettings.Url, benchmarkSettings.FileName);
                    }
                    downloadWatch.Stop();
                    Log($"Downloaded Benchmark file {benchmarkSettings.FileName} in {downloadWatch.Elapsed}");
                }



            }
        }
        public void ProcessDbFactors(int minDigits = 0, int maxDigits = 30, int batchSize = 100)
        {


            var config = FactorConfig.GetCommandLineConfig();
            var commandLineArgs = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
            this.Thread = config.ProcessorIndex.HasValue ? config.ProcessorIndex.Value : -1;
            var totalThreads = config.TotalThreads == 0 ? 1 : config.TotalThreads;
            Log($"Starting test {nameof(ProcessDbFactors)}(minDigits={minDigits}, maxDigits={maxDigits}, batchSize={batchSize}) args: {commandLineArgs}");
            var initWatch = Stopwatch.StartNew();
            var init = false;
            SetConnectionString();
            var benchmarkSettings = new BenchmarkSettings();
            if (config.Offline)
            {
                Log($"Running in offline mode - {config.OfflineFiilePath}");
                Program.Config.Bind("Benchmark", benchmarkSettings);
                benchmarkSettings.FileName = Path.Combine(AppContext.BaseDirectory, benchmarkSettings.FileName);
                config.OfflineFiilePath = benchmarkSettings.FileName;
                if (!File.Exists(benchmarkSettings.FileName))
                {
                    Log($"Downloading Benchmark file {benchmarkSettings.FileName}");

                    var downloadWatch = Stopwatch.StartNew();
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(benchmarkSettings.Url, benchmarkSettings.FileName);
                    }
                    downloadWatch.Stop();
                    Log($"Downloaded Benchmark file {benchmarkSettings.FileName} in {downloadWatch.Elapsed}");
                }


            }

            var startId = config.Start;
            int idx = 0;
            int factorCount = 0;


            const int maxEffectiveDigits = 256;
            int effectiveDigits = config.Digits.HasValue && (config.skipFact == false || config.skipECM == false || config.skipPM1 == false || config.skipPM1 == false) ? config.Digits.Value : maxEffectiveDigits;

            bool useBatchFile = (config.skipECM == false || config.skipPM1 == false || config.skipPM1 == false);
            string batchFileName = $"batch-{Thread}.txt";
            Process? processor = null;
            Action runProcessor = () =>
            {
                if (processor == null || processor.HasExited)
                {
                    var p = new Process();
                    p.StartInfo.FileName = "cmd"; //Path.Combine(AppContext.BaseDirectory, "testrunner.exe");
                    p.StartInfo.Arguments = $@"/c ""{Path.Combine(AppContext.BaseDirectory, "testrunner.exe")}"" processqueue";
                    p.StartInfo.WorkingDirectory = AppContext.BaseDirectory;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processor = p;
                    processor.Start();

                }
            };
            if (config.Offline)
                runProcessor = () => { };

            while (true)
            {
                var sw = Stopwatch.StartNew();
                List<DbFactorization> unFactored = new();

                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {
                        //unFactored = dbContext.Factorizations
                        //      .Include(x => x.Factors)
                        //      .Where(x => x.Id > startId && x.TDiv < effectiveDigits &&
                        //       x.Factors.Any(f => f.Digits >= minDigits && f.Digits <= maxDigits && (f.Type == PrimalityType.Unknown || f.Type == PrimalityType.Composite))
                        //           //&& (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)
                        //           )
                        //      .OrderBy(x => x.Id)
                        //      .Take(batchSize)
                        //      .ToList();
                        if (config.Offline)
                        {
                            using (var sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, config.OfflineFiilePath)))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var line = sr.ReadLine();


                                    if (string.IsNullOrWhiteSpace(line))
                                        continue;
                                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length != 5)
                                        continue;
                                    if (!int.TryParse(parts[0], out int id))
                                        continue;
                                    if (id < startId)
                                        continue;
                                    if (id >= config.End)
                                        break;
                                    var n = parts[1];
                                    if (n.Length < minDigits || n.Length > maxDigits)
                                        continue;
                                    if (!int.TryParse(parts[2], out int _type))
                                        continue;
                                    var type = (PrimalityType)(_type);
                                    if (type != PrimalityType.Unknown && type != PrimalityType.Composite)
                                        continue;
                                    if (!int.TryParse(parts[3], out int tdiv))
                                        continue;
                                    if (tdiv >= effectiveDigits)
                                        continue;
                                    if (totalThreads > 1 && id % totalThreads != Thread)
                                        continue;
                                    var factor = parts[4];

                                    var dbFactor = new DbFactor
                                    {
                                        Digits = factor.Length,
                                        Bits = MathLib.BitLength(BigInteger.Parse(factor)),
                                        P = factor,
                                        Power = 1,
                                        Type = PrimalityType.Unknown

                                    };

                                    var z = new DbFactorization
                                    {
                                        Digits = n.Length,
                                        Bits = MathLib.BitLength(BigInteger.Parse(n)),
                                        Id = id,
                                        N = n,
                                        Type = type,
                                        TDiv = tdiv,
                                        Factors = new List<DbFactor> { dbFactor }
                                    };

                                    if (!z.Factors.Any(x => x.Digits >= minDigits && x.Digits <= maxDigits))
                                        continue;
                                    if (z.TDiv >= effectiveDigits)
                                        continue;
                                    unFactored.Add(z);
                                    if (unFactored.Count >= batchSize)
                                        break;
                                }
                            }

                        }


                        else
                        {
                            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
                            {
                                string threadFilter = totalThreads > 1 ? $" and z.id % {totalThreads} = {Thread} " : string.Empty;
                                string endFilter = config.End != int.MaxValue ? $" and z.id < {config.End} " : string.Empty;
                                var query = $@"select top {batchSize} z.*, f.* from Factorizations z join CompositeFactors f on z.id=f.dbFactorizationId
                                    where z.id>={startId} {threadFilter} {endFilter}
                                        and z.TDiv < {effectiveDigits} 
                                        and f.Digits >= {minDigits} and f.Digits <= {maxDigits} 
                                        and z.type < 1 and f.Type < 1 
                                    order by z.id
                                ";
                                unFactored = conn.Query<DbFactorization, DbFactor, DbFactorization>(query, (x, y) =>
                                {
                                    x.Factors.Add(y);
                                    return x;
                                }, splitOn: "Id").ToList();
                            }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {

                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                if (!init)
                {
                    init = true;
                    initWatch.Stop();
                    Log($"Initialized test {nameof(ProcessDbFactors)}(minDigits={minDigits}, maxDigits={maxDigits}, batchSize={batchSize}) in {initWatch.Elapsed}");
                }

                selectWatch.Stop();



                if (!unFactored.Any())
                {
                    runProcessor();
                    Log($"No more factors to process after Id={startId}");
                    break;
                }

                int batchFactored = 0;
                Log($"Running batch of {unFactored.Count} ({unFactored.Min(x => x.Id).ToString("N0")} - {unFactored.Max(x => x.Id).ToString("N0")}) - {commandLineArgs}");

                var l = new List<(int, string)>();

                Action<DbFactorization, FactorizationBigInteger, Stopwatch> processFactors = (dbFact, factored, thisfactorWatch) =>
                {

                    var factString = factored.GetProduct().ToString();
                    if (factored.Factors.Count > 1)
                    {
                        batchFactored++;
                        factorCount++;
                        factored.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));

                        // recursively factor small composites less than 20 digits
                        var composites = factored.Factors.Where(x => x.P.ToString().Length <= 20 && (x.FactorType != MathLib.PrimalityType.ProbablePrime && x.FactorType != MathLib.PrimalityType.Prime)).ToList();
                        foreach (var c in composites)
                        {

                            thisfactorWatch.Start();
                            using var subfac = FactorizationBigInteger.Factor(c.P, false, true);
                            if (subfac.Factors.Count > 1)
                            {
                                factored.Factors.Remove(c);
                                thisfactorWatch.Stop();
                                if (c.Power > 1)
                                {
                                    Log($"Need to handle powers");
                                }
                                subfac.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                                factored.Add(subfac);
                            }

                            subfac.Dispose();
                        }
                        composites.Clear();
                        composites = null;
                        foreach (var f in factored.Factors)
                        {
                            l.Add((dbFact.Id, f.P.ToString()));
                        }
                    }

                };


                startId = unFactored.Max(x => x.Id) + 1;
                var factorWatch = Stopwatch.StartNew();
                List<int> tdivUpdates = new List<int>();
                if (useBatchFile)
                {
                    var workingDirectory = Path.Combine(Path.GetFullPath("."), "binaries");
                    using (var writer = new StreamWriter(Path.Combine(workingDirectory, batchFileName), false))
                    {
                        foreach (var fact in unFactored)
                        {
                            if (fact.TDiv > effectiveDigits)
                                continue;
                            fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList()
                                .ForEach(x => x.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(x.P)));
                            idx++;
                            var smallFactors = fact.Factors.Where(x => x.Digits >= minDigits && x.Digits <= maxDigits && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)).ToList();
                            foreach (var composite in smallFactors)
                                writer.WriteLine($"{composite.P}");
                        }
                    }
                    Console.Title = $"({idx.ToString("N0")}) Id {unFactored.Last().Id.ToString("N0")} Count: {factorCount.ToString("N0")} - {commandLineArgs}";
                    // run the batch file
                    //ecm -pm1 25000 < pp1.txt
                    bool useGpu = config.EnableGpu.HasValue && config.EnableGpu.Value;
                    var exe = useGpu ? "ecm_gpu.exe -gpu" : "ecm.exe";
                    var algo = "";
                    if (config.skipPM1 == false) algo = "-pm1";
                    if (config.skipPP1 == false) algo = "-pp1";

                    var cmd = $"{exe} {algo}";
                    if (config.Curves.HasValue && config.Curves.Value > 0)
                        cmd = $"{cmd}{(useGpu ? "-gpucurves " : "-c ")}{config.Curves}";
                    cmd = $"{cmd} {config.B1}";

                    if (config.B2.HasValue && config.B2.Value > config.B1.Value)
                        cmd = $"{cmd} {config.B2}";
                    cmd = $"{cmd} < {batchFileName}";
                    var thisfactorWatch = Stopwatch.StartNew();
                    //Console.WriteLine($"Executing {cmd}");

                    var processResult = ProcessHelper.RunProcess(cmd, workingDirectory, WaitForExit: false);
                    thisfactorWatch.Stop();
                    var results = processResult.Output.Split("Input number is").Skip(1).ToList();
                    //Console.WriteLine(processResult.Output);
                    for (var i = 0; i < unFactored.Count; i++)
                    {
                        var fact = unFactored[i];
                        var result = results[i];
                        using var f = GmpEcm.ParseFactorsNumeric(result);
                        if (f.Factors.Count > 1)
                        {
                            processFactors(fact, f, thisfactorWatch);
                        }

                        if (fact.TDiv < effectiveDigits && effectiveDigits < maxEffectiveDigits)
                        {
                            tdivUpdates.Add(fact.Id);
                            //fact.TDiv = effectiveDigits;
                        }
                    }

                }

                else
                {

                    foreach (var fact in unFactored)
                    {
                        if (fact.TDiv > effectiveDigits)
                            continue;
                        fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList()
                             .ForEach(x => x.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(x.P)));

                        idx++;
                        if (batchSize < 10 || idx % 10 == 0)
                        {
                            Console.Title = $"({idx.ToString("N0")}) Id {fact.Id.ToString("N0")} Count: {factorCount.ToString("N0")} - {commandLineArgs}";
                        }
                        var smallFactors = fact.Factors.Where(x => x.Digits >= minDigits && x.Digits <= maxDigits && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)).ToList();



                        foreach (var smallFactor in smallFactors)
                        {
                            if (smallFactor.Type == PrimalityType.Unknown || smallFactor.Type == PrimalityType.Composite)
                            {
                                smallFactor.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(smallFactor.P));
                                if (smallFactor.Type == PrimalityType.ProbablePrime || smallFactor.Type == PrimalityType.Prime)
                                    continue;
                            }
                            var n = BigInteger.Parse(smallFactor.P);
                            var thisfactorWatch = Stopwatch.StartNew();
                            // get algorithms from the command line or use one rho algo at random
                            using var factored = FactorizationBigInteger.Factor(n, false, true, skipFermat: true, skipRho: true, skipRhoP2: true, skipRhoP3: true, skipRhoZ: true, skipPP1: true, skipPM1: true, skipECM: true, skipQS: true, skipFact: true);
                            thisfactorWatch.Stop();



                            if (factored.Factors.Count > 1 && factored.GetProduct() == n)
                            {
                                batchFactored++;
                                factorCount++;
                                factored.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));

                                // recursively factor small composites less than 20 digits
                                var composites = factored.Factors.Where(x => x.P.ToString().Length <= 20 && (x.FactorType != MathLib.PrimalityType.ProbablePrime && x.FactorType != MathLib.PrimalityType.Prime)).ToList();
                                foreach (var c in composites)
                                {

                                    thisfactorWatch.Start();
                                    using var subfac = FactorizationBigInteger.Factor(c.P, false, true);
                                    if (subfac.Factors.Count > 1)
                                    {
                                        factored.Factors.Remove(c);
                                        thisfactorWatch.Stop();
                                        if (c.Power > 1)
                                        {
                                            Log($"Need to handle powers");
                                        }
                                        subfac.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                                        factored.Add(subfac);
                                    }

                                    subfac.Dispose();
                                }
                                composites.Clear();
                                composites = null;
                                foreach (var f in factored.Factors)
                                {
                                    l.Add((fact.Id, f.P.ToString()));
                                }
                                // not sure if we need this here, since the factor queue processor will take care of it going forward.
                                //foreach (var f in factored.Factors)
                                //{
                                //    FactoringQueue.RemovePrimeFactor(fact, f.P.ToString());
                                //}


                            }
                            factored.Dispose();
                        }

                        smallFactors.Clear();
                        smallFactors = null;





                        if (fact.TDiv < effectiveDigits && effectiveDigits < maxEffectiveDigits)
                        {
                            tdivUpdates.Add(fact.Id);
                            //fact.TDiv = effectiveDigits;
                        }

                    }

                }
                factorWatch.Stop();
                if (config.Offline)
                {
                    if (l.Any())
                    {
                        var filePath = config.OfflineFiilePath + $".{Thread}.factors.bat";
                        var lines = l.Select(x => $"testrunner add {x.Item1} {x.Item2}").ToArray();
                        File.AppendAllLines(filePath, lines);
                    }
                    if (tdivUpdates.Any())
                    {
                        var filePath = config.OfflineFiilePath + $".{Thread}.tdiv.bat";
                        var lines = tdivUpdates.Select(x => $"testrunner tdiv {x} {effectiveDigits}").ToArray();
                        File.AppendAllLines(filePath, lines);
                    }

                }
                else
                {
                    FactoringQueue.QueueFactors(l);
                    if (tdivUpdates.Any())
                    {
                        using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
                        {
                            var query = $"update Factorizations set TDiv={effectiveDigits} where Id in ({string.Join(",", tdivUpdates)})";
                            conn.Execute(query);
                        }
                    }
                }
                sw.Stop();
                Log($"{Console.Title}");
                Log($"{unFactored.Last().Id.ToString("N0")} Factored {batchFactored} of {unFactored.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factorWatch.Elapsed} select {selectWatch.Elapsed}");
                runProcessor();
            }



        }


        public void ProcessDbFactorsEf(int minDigits = 0, int maxDigits = 30, int batchSize = 100)
        {


            var config = FactorConfig.GetCommandLineConfig();
            var commandLineArgs = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
            this.Thread = config.ProcessorIndex.HasValue ? config.ProcessorIndex.Value : -1;

            Log($"Starting test {nameof(ProcessDbFactors)}(minDigits={minDigits}, maxDigits={maxDigits}, batchSize={batchSize}) args: {commandLineArgs}");
            var initWatch = Stopwatch.StartNew();
            var init = false;
            SetConnectionString();
            using var serviceProvider = new ServiceCollection()
                       .AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString))
                       .BuildServiceProvider();

            var startId = 0;
            int idx = 0;
            int factorCount = 0;


            const int maxEffectiveDigits = 256;
            int effectiveDigits = config.Digits.HasValue && (config.skipFact == false || config.skipECM == false) ? config.Digits.Value : maxEffectiveDigits;


            while (true)
            {
                var sw = Stopwatch.StartNew();
                List<DbFactorization> unFactored = new();

                using var scope = serviceProvider.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<FactorDbContext>();
                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {
                        unFactored = dbContext.Factorizations
                              .Include(x => x.Factors)
                              .Where(x => x.Id > startId && x.TDiv < effectiveDigits &&
                               x.Factors.Any(f => f.Digits >= minDigits && f.Digits <= maxDigits && (f.Type == PrimalityType.Unknown || f.Type == PrimalityType.Composite))
                                   //&& (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)
                                   )
                              .OrderBy(x => x.Id)
                              .Take(batchSize)
                              .ToList();
                        break;
                    }
                    catch (Exception ex)
                    {

                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                if (!init)
                {
                    init = true;
                    initWatch.Stop();
                    Log($"Initialized test {nameof(ProcessDbFactors)}(minDigits={minDigits}, maxDigits={maxDigits}, batchSize={batchSize}) in {initWatch.Elapsed}");
                }

                selectWatch.Stop();



                if (!unFactored.Any())
                {
                    Log($"No more factors to process after Id={startId}");
                    break;
                }

                int batchFactored = 0;
                Log($"Running batch of {unFactored.Count} ({unFactored.Min(x => x.Id).ToString("N0")} - {unFactored.Max(x => x.Id).ToString("N0")}) - {commandLineArgs}");

                startId = unFactored.Max(x => x.Id) + 1;
                var factorWatch = Stopwatch.StartNew();


                foreach (var fact in unFactored)
                {
                    if (fact.TDiv > effectiveDigits)
                        continue;
                    fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList()
                         .ForEach(x => x.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(x.P)));

                    idx++;
                    if (batchSize < 10 || idx % 10 == 0)
                    {
                        Console.Title = $"({idx.ToString("N0")}) Id {fact.Id.ToString("N0")} Count: {factorCount.ToString("N0")} - {commandLineArgs}";
                    }
                    var smallFactors = fact.Factors.Where(x => x.Digits >= minDigits && x.Digits <= maxDigits && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)).ToList();



                    foreach (var smallFactor in smallFactors)
                    {
                        if (smallFactor.Type == PrimalityType.Unknown || smallFactor.Type == PrimalityType.Composite)
                        {
                            smallFactor.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(smallFactor.P));
                            if (smallFactor.Type == PrimalityType.ProbablePrime || smallFactor.Type == PrimalityType.Prime)
                                continue;
                        }
                        var n = BigInteger.Parse(smallFactor.P);
                        var thisfactorWatch = Stopwatch.StartNew();
                        // get algorithms from the command line or use one rho algo at random
                        using var factored = FactorizationBigInteger.Factor(n, false, true, skipFermat: true, skipRho: true, skipRhoP2: true, skipRhoP3: true, skipRhoZ: true, skipPP1: true, skipPM1: true, skipECM: true, skipQS: true, skipFact: true);
                        thisfactorWatch.Stop();





                        if (factored.Factors.Count > 1 && factored.GetProduct() == n)
                        {
                            batchFactored++;

                            factorCount++;
                            factored.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));

                            // recursively factor small composites less than 20 digits
                            var composites = factored.Factors.Where(x => x.P.ToString().Length <= 20 && (x.FactorType != MathLib.PrimalityType.ProbablePrime && x.FactorType != MathLib.PrimalityType.Prime)).ToList();
                            foreach (var c in composites)
                            {

                                thisfactorWatch.Start();
                                using var subfac = FactorizationBigInteger.Factor(c.P, false, true);
                                if (subfac.Factors.Count > 1)
                                {
                                    factored.Factors.Remove(c);
                                    thisfactorWatch.Stop();
                                    if (c.Power > 1)
                                    {
                                        Log($"Need to handle powers");
                                    }
                                    subfac.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                                    factored.Add(subfac);
                                }

                                subfac.Dispose();
                            }
                            composites.Clear();
                            composites = null;

                            foreach (var f in factored.Factors)
                            {
                                FactoringQueue.RemovePrimeFactor(fact, f.P.ToString());

                            }
                            //fact.Factors.Remove(smallFactor);
                            //fact.Factors.AddRange(factored.Factors.Select(x => new DbFactor
                            //{
                            //    P = x.P.ToString(),
                            //    Power = x.Power,
                            //    Type = (PrimalityType)x.FactorType,
                            //    Digits = x.P.ToString().Length,
                            //    Bits = MathLib.BitLength(x.P)
                            //}));

                        }
                        factored.Dispose();
                    }

                    smallFactors.Clear();
                    smallFactors = null;
                    if (fact.TDiv < effectiveDigits && effectiveDigits < maxEffectiveDigits)
                    {
                        fact.TDiv = effectiveDigits;
                    }
                    fact.Type = fact.Factors.All(x => x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                }

                foreach (var fact in unFactored)
                {
                    fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList()
                         .ForEach(x => x.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(x.P)));
                }

                factorWatch.Stop();
                Stopwatch saveWatch = Stopwatch.StartNew();
                sleep = 10;
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        dbContext.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {

                        Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                saveWatch.Stop();
                sw.Stop();
                Log($"{Console.Title}");
                Log($"{unFactored.Last().Id.ToString("N0")} Factored {batchFactored} of {unFactored.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factorWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed}");
                dbContext.Dispose();
                scope.Dispose();
            }



        }

        public void TestTrialDivide()
        {
            Log($"Starting test {nameof(TestTrialDivide)}");
            if (bool.Parse(bool.FalseString))
            {
                Log($"Seeding database {nameof(TestTrialDivide)}");
                //EfSeedTest();
            }
            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            Log($"Getting Primes {nameof(TestTrialDivide)}");
            var primes = GetPrimesTo(1_000_000);



            Log($"Getting Factors {nameof(GetSmallFactors)}");
            var smallFactors = GetSmallFactors();

            Log($"Filtering Primes {nameof(GetSmallFactors)}");
            var filtered = primes.Where(p => smallFactors.Contains(p)).ToList();
            primes = filtered;
            //}

            while (true)
            {
                //Log2($"Processing Batch {nameof(TFTestPaged)}");
                var sw = Stopwatch.StartNew();
                var factWatch = new Stopwatch();
                var dtoWatch = new Stopwatch();
                using var app = provider.CreateScope();
                using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();



                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                List<DbFactorization> dbFacts = new();

                bool rebuiltStats = false;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {
                        dbFacts = dbContext.Factorizations.Include(x => x.Factors)
                           .Where(x => x.TDiv < 6 && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite))
                           .OrderBy(x => x.Id)
                           .Take(20000).ToList();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!rebuiltStats)
                        {
                            rebuiltStats = true;
                            Log($"Rebuilding stats after select timeout");
                            rebuildStats();
                        }
                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                if (sleep != 10)
                {
                    Log($"Resuming After Select DbError");
                }
                selectWatch.Stop();
                if (selectWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
                if (dbFacts.Count == 0)
                    break;

                Stopwatch saveWatch = new Stopwatch();
                //Log2($"Processing {dbFacts.Count.ToString("N0")} Factors {nameof(TFTestPaged)}");
                foreach (var dbFact in dbFacts)
                {


                    if (dbFact == null)
                        break;

                    var unfactored = dbFact.Factors.Where(x => x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite).ToList();

                    foreach (var unfactor in unfactored)
                    {
                        if ((int)unfactor.Type > 0)
                            continue;

                        BigInteger factor = BigInteger.Parse(unfactor.P);
                        factWatch.Start();
                        using var factorization = FactorizationBigInteger.FactorTrialDivide(factor, 99999, primes);
                        factWatch.Stop();
                        if (factorization.Factors.Count == 1)
                        {
                            dbFact.TDiv = 6;

                            continue;
                        }
                        else
                        {

                            factorization.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));

                            dbFact.TDiv = 6;
                            dbFact.Factors.Remove(unfactor);
                            dbFact.Type = factorization.Factors.All(x => (int)x.FactorType > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                            dtoWatch.Start();
                            dbFact.Factors.AddRange(factorization.Factors.Select(f =>
                                        new DbFactor
                                        {
                                            P = f.P.ToString(),
                                            Power = f.Power,
                                            Type = (PrimalityType)(int)f.FactorType,
                                            Digits = f.P.ToString().Length,
                                            Bits = MathLib.BitLength(f.P)
                                        }
                                ));
                        }
                        factorization.Dispose();

                    }




                }
                saveWatch.Start();
                sleep = 10;
                rebuiltStats = false;
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        dbContext.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!rebuiltStats)
                        {
                            rebuiltStats = true;
                            Log($"Rebuilding stats after save timeout");
                            rebuildStats();

                        }
                        Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }

                saveWatch.Stop();
                if (saveWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
                sw.Stop();
                if (sleep != 10)
                {
                    Log($"Resuming After Save DbError");
                }
                Log($"{dbFacts.Last().Id.ToString("N0")} Factored {dbFacts.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed} - dto {dtoWatch.Elapsed}");


                app.Dispose();

                dbContext.Dispose();
            }
        }

        public void TinyEcm()
        {
            var ecm = new NumericsEcm();
            var tDiv = 20;
            RunFactorization(x => ecm.ECM(x, 10, B1: 2000, enableGpu: true), tDiv - 2, tDiv - 2);
        }
        public void FactorTDiv()
        {
            //RunFactorization(x => FactorizationBigInteger.FactorTrialDivide(x), 20);
            var ecm = new NumericsEcm();
            var tDiv = 20;
            RunFactorization(x => ecm.PM1(x, tDiv), tDiv - 4, tDiv - 4);
            RunFactorization(x => ecm.PP1(x, tDiv), tDiv - 3, tDiv - 3);
            //RunFactorization(x => ecm.PP1(x, tDiv), tDiv - 2, tDiv - 2);
            //RunFactorization(x => ecm.PP1(x, tDiv), tDiv - 1, tDiv - 1);
            RunFactorization(x => ecm.ECM(x, tDiv), tDiv, tDiv);
        }


        public void RunFactorization2(Func<BigInteger, FactorizationBigInteger> factorIt, int minDigits = 0, int maxDigits = 30, int batchSize = 100)
        {
            var config = FactorConfig.GetCommandLineConfig();
            Thread = config.ProcessorIndex.HasValue ? config.ProcessorIndex.Value : -1;

            Log($"Starting test {nameof(ProcessDbFactors)}(minDigits={minDigits},maxDigits={maxDigits},batchSize={batchSize})");
            var initWatch = Stopwatch.StartNew();
            var init = false;
            SetConnectionString();
            using var serviceProvider = new ServiceCollection()
                       .AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString))
                       .BuildServiceProvider();

            var startId = 0;
            int idx = 0;
            int factorCount = 0;





            const int maxEffectiveDigits = 256;
            int effectiveDigits = config.Digits.HasValue && (config.skipFact == false || config.skipECM == false) ? config.Digits.Value : maxEffectiveDigits;
            var commandLineArgs = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));

            while (true)
            {
                var sw = Stopwatch.StartNew();
                List<DbFactorization> unFactored = new();

                using var scope = serviceProvider.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<FactorDbContext>();
                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {
                        unFactored = dbContext.Factorizations
                              .Include(x => x.Factors)
                              .Where(x => x.Id > startId && x.TDiv < effectiveDigits &&
                               x.Factors.Any(f => f.Digits >= minDigits && f.Digits <= maxDigits && (f.Type == PrimalityType.Unknown || f.Type == PrimalityType.Composite))
                                   //&& (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)
                                   )
                   .OrderBy(x => x.Id)
                              .Take(batchSize)
                              .ToList();
                        break;
                    }
                    catch (Exception ex)
                    {

                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                if (!init)
                {
                    initWatch.Stop();
                    Log($"Initialized test {nameof(ProcessDbFactors)}(minDigits={minDigits}, maxDigits={maxDigits}, batchSize={batchSize}) in {initWatch.Elapsed}");
                }

                selectWatch.Stop();



                if (!unFactored.Any())
                {
                    Log($"No more factors to process after Id={startId}");
                    break;

                }

                Log($"Running batch of {unFactored.Count} - {unFactored.Min(x => x.Id)} - {unFactored.Max(x => x.Id)} - {commandLineArgs}");

                startId = unFactored.Max(x => x.Id) + 1;
                var factorWatch = Stopwatch.StartNew();


                foreach (var fact in unFactored)
                {
                    if (fact.TDiv > effectiveDigits)
                        continue;
                    fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList()
                         .ForEach(x => x.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(x.P)));

                    idx++;
                    if (batchSize < 10 || idx % 10 == 0)
                    {
                        Console.Title = $"({idx.ToString("N0")}) Id {fact.Id.ToString("N0")} Count: {factorCount.ToString("N0")} - {commandLineArgs}";
                    }
                    var smallFactors = fact.Factors.Where(x => x.Digits >= minDigits && x.Digits <= maxDigits && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite)).ToList();



                    foreach (var smallFactor in smallFactors)
                    {
                        if (smallFactor.Type == PrimalityType.Unknown || smallFactor.Type == PrimalityType.Composite)
                        {
                            smallFactor.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(smallFactor.P));
                            if (smallFactor.Type == PrimalityType.ProbablePrime || smallFactor.Type == PrimalityType.Prime)
                                continue;
                        }
                        var n = BigInteger.Parse(smallFactor.P);
                        var thisfactorWatch = Stopwatch.StartNew();
                        // get algorithms from the command line or use one rho algo at random
                        using var factored = factorIt(n);
                        thisfactorWatch.Stop();
                        if (factored.Factors.Count > 1)
                        {

                            factorCount++;
                            factored.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));

                            // recursively factor small composites less than 20 digits
                            var composites = factored.Factors.Where(x => x.P.ToString().Length <= 20 && (x.FactorType != MathLib.PrimalityType.ProbablePrime && x.FactorType != MathLib.PrimalityType.Prime)).ToList();
                            foreach (var c in composites)
                            {

                                thisfactorWatch.Start();
                                using var subfac = FactorizationBigInteger.Factor(c.P, false, true);
                                if (subfac.Factors.Count > 1)
                                {
                                    factored.Factors.Remove(c);
                                    thisfactorWatch.Stop();
                                    if (c.Power > 1)
                                    {
                                        Console.WriteLine("Need to handle powers");
                                    }
                                    subfac.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));
                                    factored.Add(subfac);
                                }

                                subfac.Dispose();
                            }
                            composites.Clear();
                            composites = null;

                            fact.Factors.Remove(smallFactor);
                            fact.Factors.AddRange(factored.Factors.Select(x => new DbFactor
                            {
                                P = x.P.ToString(),
                                Power = x.Power,
                                Type = (PrimalityType)x.FactorType,
                                Digits = x.P.ToString().Length,
                                Bits = MathLib.BitLength(x.P)
                            }));

                        }
                        factored.Dispose();
                    }

                    smallFactors.Clear();
                    smallFactors = null;
                    if (fact.TDiv < effectiveDigits && effectiveDigits < maxEffectiveDigits)
                    {
                        fact.TDiv = effectiveDigits;
                    }
                    fact.Type = fact.Factors.All(x => x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                }

                foreach (var fact in unFactored)
                {
                    fact.Factors.Where(x => x.Type == PrimalityType.Unknown).ToList()
                         .ForEach(x => x.Type = (PrimalityType)(int)GmpInt.Primality(BigInteger.Parse(x.P)));
                }

                factorWatch.Stop();
                Stopwatch saveWatch = Stopwatch.StartNew();
                sleep = 10;
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        dbContext.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {

                        Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                saveWatch.Stop();
                sw.Stop();
                Log(Console.Title);
                Log($"{unFactored.Last().Id.ToString("N0")} Factored of {unFactored.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factorWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed}");

                dbContext.Dispose();
                scope.Dispose();
            }



        }


        public void RunFactorization(Func<BigInteger, FactorizationBigInteger> factorIt, int maxDbTDiv, int updateDbTDiv, int batchSize = 2000)
        {
            var config = FactorConfig.GetCommandLineConfig();
            Thread = config.ProcessorIndex.HasValue ? config.ProcessorIndex.Value : -1;

            Log($"Starting test {nameof(RunFactorization)}");
            if (bool.Parse(bool.FalseString))
            {
                Log($"Seeding database {nameof(TestTrialDivide)}");
                //EfSeedTest();
            }
            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            //Log2($"Getting Primes {nameof(TFTestPaged)}");
            // var primes = GetPrimesTo(1_000_000);



            //Log2($"Getting Factors {nameof(GetSmallFactors)}");
            //var smallFactors = GetSmallFactors();

            //Log2($"Filtering Primes {nameof(GetSmallFactors)}");
            //var filtered = primes.Where(p => smallFactors.Contains(p)).ToList();
            //primes = filtered;
            //}


            const int maxEffectiveDigits = 256;
            int effectiveDigits = config.Digits.HasValue && (config.skipFact == false || config.skipECM == false) ? config.Digits.Value : maxEffectiveDigits;
            var commandLineArgs = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));


            while (true)
            {
                //Log2($"Processing Batch {nameof(TFTestPaged)}");
                var sw = Stopwatch.StartNew();
                var factWatch = new Stopwatch();
                var dtoWatch = new Stopwatch();
                using var app = provider.CreateScope();
                using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();



                var selectWatch = Stopwatch.StartNew();
                int sleep = 10;
                List<DbFactorization> dbFacts = new();

                int startId = 0;
                bool rebuiltStats = false;
                for (var retry = 0; retry < 10; retry++)
                {
                    try
                    {
                        dbFacts = dbContext.Factorizations.Include(x => x.Factors)
                           .Where(x => x.Id > startId && x.TDiv < maxDbTDiv && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite))
                           .OrderBy(x => x.Id)
                           .Take(batchSize).ToList();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!rebuiltStats)
                        {
                            rebuiltStats = true;
                            Log($"Rebuilding stats after select timeout");
                            rebuildStats();
                        }
                        Log($"Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                if (sleep != 10)
                {
                    Log($"Resuming After Select DbError");
                }
                selectWatch.Stop();
                if (selectWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
                if (!dbFacts.Any())
                {
                    Log($"No more factors to process after Id={startId}");
                    break;

                }
                startId = dbFacts.Max(x => x.Id) + 1;

                Log($"Running batch of {dbFacts.Count} - {dbFacts.Min(x => x.Id)} - {dbFacts.Max(x => x.Id)} - {commandLineArgs}");


                int idx = 0;
                //Log2($"Processing {dbFacts.Count.ToString("N0")} Factors {nameof(TFTestPaged)}");
                int factored = 0;
                foreach (var dbFact in dbFacts)
                {
                    idx++;
                    if (idx % 100 == 0)
                    {
                        Console.Title = $"({idx}) Processing {dbFact.Id}";
                    }
                    if (dbFact == null)
                        break;

                    var unfactored = dbFact.Factors.Where(x => x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite).ToList();
                    foreach (var unfactor in unfactored)
                    {
                        if ((int)unfactor.Type > 0)
                            continue;

                        BigInteger factor = BigInteger.Parse(unfactor.P);
                        factWatch.Start();
                        //using var factorization = FactorizationBigInteger.FactorTrialDivide(factor, 99999, primes);
                        using var factorization = factorIt(factor);
                        factWatch.Stop();
                        if (factorization.Factors.Count < 2)
                        {

                            continue;
                        }
                        else
                        {
                            factored++;
                            factorization.Factors.ForEach(x => x.FactorType = (MathLib.PrimalityType)(int)GmpInt.Primality(x.P));


                            dbFact.Factors.Remove(unfactor);
                            dtoWatch.Start();
                            dbFact.Factors.AddRange(factorization.Factors.Select(f =>
                            {
                                var result = new DbFactor
                                {
                                    P = f.P.ToString(),
                                    Power = f.Power,
                                    Type = (PrimalityType)(int)f.FactorType,
                                    Bits = MathLib.BitLength(f.P)
                                };
                                result.Digits = result.P.Length;
                                return result;
                            }
                                ));
                            dtoWatch.Stop();
                        }
                        factorization.Dispose();

                    }
                    if (dbFact.TDiv < effectiveDigits && effectiveDigits < maxEffectiveDigits)
                    {
                        dbFact.TDiv = effectiveDigits;
                    }

                    dbFact.Type = dbFact.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;


                }
                var saveWatch = Stopwatch.StartNew();
                sleep = 10;
                rebuiltStats = false;
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        dbContext.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!rebuiltStats)
                        {
                            rebuiltStats = true;
                            Log($"Rebuilding stats after save timeout");
                            rebuildStats();

                        }
                        Log($"Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }

                saveWatch.Stop();
                if (saveWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
                sw.Stop();
                if (sleep != 10)
                {
                    Log($"Resuming After Save DbError");
                }
                Log($"{dbFacts.Last().Id.ToString("N0")} Factored {dbFacts.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed} - dto {dtoWatch.Elapsed}");


                app.Dispose();

                dbContext.Dispose();
            }
        }

        private void rebuildStats()
        {
            var connString = FactorDbContext.DbConnectionString;
            connString = $"{connString};Command Timeout=900";

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                conn.Execute("exec sp_updatestats");
                conn.Execute("backup database factors to disk='nul'");
                conn.Execute("backup log factors to disk='nul'");
            }

        }

        private List<int> GetSmallFactors()
        {

            var connString = FactorDbContext.DbConnectionString;
            connString = $"{connString};Command Timeout=900";
            var query = $@"  select distinct(cast(p as int)) from factors f(nolock) where
                      1=1 
                      --and [type]>0 
                      and Digits<7
                      order by cast(p as int)
                    ";

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var result = conn.Query<int>(query).ToList();
                return result;
            }
        }
    }
}
