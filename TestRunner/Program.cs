using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using MathGmp.Native;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;

namespace TestRunner
{

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Running {AppContext.BaseDirectory}{Path.GetFileName(Process.GetCurrentProcess().ProcessName)}.exe - args: {string.Join(" ", args)}");
            Console.WriteLine($"[{DateTime.Now}] Starting test {nameof(Program)} with working directory {Path.GetFullPath(".")}");

            CommandLine.SetArguments(args);

            ProcessHelper.SetProcessAffinity(Process.GetCurrentProcess());


            if (args.Any(x => x == "test"))
            {
                var n = BigInteger.Parse("1234567890123456789012345678901234567890");
                using var factor = FactorizationBigInteger.Factor(n, false, true, skipFermat: true, skipRho: true, skipRhoP2: true, skipRhoP3: true, skipRhoZ: true, skipPP1: true, skipPM1: true, skipECM: true, skipQS: true, skipFact: false);
                Console.WriteLine($"[{DateTime.Now}] {n} = {string.Join(" * ", factor.Factors.Select(x => x.P))} - {factor.Factors.Count} factors - {factor.GetProduct()}");
                return;
            }
            var efTests = new FactorTest();

            if (args.Any(x => x == "verify"))
            {
                efTests.VerifyFactorBase();
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
                var FactorBaseSiever = new FactorBaseSiever();
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
    }

    public class FactorTest
    {



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
         "Server=192.168.2.170;Database=Factors;user=factor;password=F@act0#1;MultipleActiveResultSets=true;TrustServerCertificate=True;Command Timeout=300";

        }


        public void VerifyFactorBase()
        {
            SetConnectionString();
            using var serviceProvider = new ServiceCollection()
                       .AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString))
                       .BuildServiceProvider();

            var startId = 770000;
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
                bool hasUpdates = false;
                foreach (var dbFactorization in unFactored)
                {
                    var bigN = BigInteger.Parse(dbFactorization.N);
                    var precheck = dbFactorization.Factors.Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power)).Aggregate((a, b) => a * b);

                    if (precheck == bigN)
                    {
                        foreach (var f in dbFactorization.Factors)
                        {
                            var bigFi = BigInteger.Parse(f.P);
                            var fCheckType = GmpInt.Primality(bigFi);
                            if ((int)f.Type != (int)fCheckType)
                            {
                                f.Type = (PrimalityType)(int)fCheckType;
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
        public void ProcessDbFactors(int minDigits = 0, int maxDigits = 30, int batchSize = 100)
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
                        using var factored = FactorizationBigInteger.Factor(n, false, true, skipFermat: true, skipRho: true, skipRhoP2: true, skipRhoP3: true, skipRhoZ: true, skipPP1: true, skipPM1: true, skipECM: true, skipQS: true);
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
