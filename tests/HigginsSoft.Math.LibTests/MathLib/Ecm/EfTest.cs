#define HAVE_ECM_AVX
#undef HAVE_ECM_AVX
#define SKIP_LONG_TESTS
//#undef SKIP_LONG_TESTS
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Disassemblers;
using Dapper;
using HigginsSoft.Math.Lib.Database;
using ManagedCuda.BasicTypes;
using Microsoft.Data.SqlClient;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftAntimalwareAMFilter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using static HigginsSoft.Math.Lib.MathLib;
using PrimalityType = HigginsSoft.Math.Lib.Database.PrimalityType;


namespace HigginsSoft.Math.Lib.Tests
{
    [TestClass()]
    public class EfTest
    {

        [TestMethod]
        public void EfSeedTest()
        {
            var rsa = RsaChallenge.Rsa1024;

            GmpInt root = rsa.Sqrt();

            int Ten_Million = 10_000_000;
            var comp = Enumerable.Range(1, Ten_Million).Select(i => (Offset: i, N: (GmpInt)(root + i).PowerMod(2, rsa))).ToList();

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            var config = configBuilder.Build();
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(provider => config);
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer("Server=localhost;Database=Factors;AttachDbFilename=E:\\sql\\Factors.mdf;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;"));

            var provider = services.BuildServiceProvider();


            using (var app = provider.CreateScope())
            {
                try
                {
                    var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                    //var dtos = comp.Select(x => new DbFactorization
                    //{
                    //    Offset = x.Offset,
                    //    N = x.N.ToString(),
                    //    Digits = x.N.ToString().Length,
                    //    Bits = MathLib.BitLength(x.N),
                    //    Type = x.N.Primality(),
                    //    Factors = (new[] { x }).Select(f => new DbFactor
                    //    {
                    //        P = f.N.ToString(),
                    //        Power = 1,
                    //        Bits = MathLib.BitLength(f.N),
                    //        Digits = f.N.ToString().Length,
                    //        Type = f.N.Primality(),
                    //    }).ToList()
                    //}).ToList();

                    var dtos = comp.Select(x => new DbFactorization
                    {
                        Offset = x.Offset,
                        N = x.N.ToString(),
                        Digits = x.N.ToString().Length,
                        Bits = MathLib.BitLength(x.N),
                        Type = (PrimalityType)x.N.Primality(),

                    }).ToList();

                    dtos.ForEach(f =>
                    {
                        f.Factors = (new[] { f }).Select(f => new DbFactor
                        {
                            P = f.N,
                            Power = 1,
                            Bits = f.Bits,
                            Digits = f.Digits,
                            Type = f.Type,
                        }).ToList();

                    });

                    dbContext.AddRange(dtos);
                    dbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

            }



        }


        IEnumerable<int> GetPrimesTo(int max)
        {
            var gen = new PrimeGenerator((int)max);
            var result = gen.ToList();
            return result;
        }
        [TestMethod]
        public void TFTest()
        {
            if (bool.Parse(bool.FalseString))
            {
                EfSeedTest();
            }
            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();


            var primes = GetPrimesTo(1_000_000);


            // if (bool.Parse(bool.FalseString))
            //{
            /*
               select distinct(cast(p as int)) from factors f  (nolock)where
               1=1 
               --and [type]>0 
               and Digits<20
               order by cast(p as int)
             */
            var smallFactors = GetSmallFactors();

            var filtered = primes.Where(p => smallFactors.Contains(p)).ToList();
            primes = filtered;
            //}

            while (true)
            {
                var sw = Stopwatch.StartNew();
                using var app = provider.CreateScope();
                using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();



                var selectWatch = Stopwatch.StartNew();
                var dbFact = dbContext.Factorizations.Include(x => x.Factors)
                    .FirstOrDefault(x => x.TDiv < 5 && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite));
                selectWatch.Stop();
                if (dbFact == null)
                    break;

                var unfactored = dbFact.Factors.Where(x => x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite).ToList();
                Stopwatch saveWatch = new Stopwatch();
                foreach (var unfactor in unfactored)
                {
                    if ((int)unfactor.Type > 0)
                        continue;

                    var factor = new GmpInt(unfactor.P);
                    var factorization = Factorization.FactorTrialDivide(factor, 99999, primes);
                    if (factorization.Factors.Count == 1)
                    {
                        dbFact.TDiv = 6;
                        saveWatch.Start();
                        dbContext.SaveChanges();
                        saveWatch.Stop();
                        continue;
                    }
                    else
                    {
                        dbFact.TDiv = 6;
                        dbFact.Factors.Remove(unfactor);
                        dbFact.Type = factorization.Factors.All(x => (int)x.P.Primality() > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                        dbFact.Factors.AddRange(factorization.Factors.Select(f =>
                                    new DbFactor
                                    {
                                        P = f.P.ToString(),
                                        Power = f.Power,
                                        Type = (PrimalityType)f.P.Primality(),
                                        Digits = f.P.ToString().Length,
                                        Bits = MathLib.BitLength(f.P)
                                    }
                            ));
                        saveWatch.Start();
                        dbContext.SaveChanges();
                        saveWatch.Stop();
                        var t = factorization.Factors.Select(x => x.P.Primality());
                    }

                }

                sw.Stop();
                if (dbFact.Id % 10 == 0)
                {
                    Log($"Factored {dbFact.Id} in {sw.Elapsed} - select {selectWatch.Elapsed} save - {saveWatch.Elapsed}");
                }



            }
        }



        void Log(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);

        }

        [TestMethod]
        public void TFTestPaged()
        {
            Log($"[{DateTime.Now}] Starting test {nameof(TFTestPaged)}");
            if (bool.Parse(bool.FalseString))
            {
                Log($"[{DateTime.Now}] Seeding database {nameof(TFTestPaged)}");
                EfSeedTest();
            }
            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            Log($"[{DateTime.Now}] Getting Primes {nameof(TFTestPaged)}");
            var primes = GetPrimesTo(1_000_000);



            Log($"[{DateTime.Now}] Getting Factors {nameof(GetSmallFactors)}");
            var smallFactors = GetSmallFactors();

            Log($"[{DateTime.Now}] Filtering Primes {nameof(GetSmallFactors)}");
            var filtered = primes.Where(p => smallFactors.Contains(p)).ToList();
            primes = filtered;
            //}

            while (true)
            {
                //Log($"[{DateTime.Now}] Processing Batch {nameof(TFTestPaged)}");
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
                           .Where(x => x.TDiv < 5 && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite))
                           .OrderBy(x => x.Id)
                           .Take(20000).ToList();
                    }
                    catch (Exception ex)
                    {
                        if (!rebuiltStats)
                        {
                            rebuiltStats = true;
                            Log($"[{DateTime.Now}] Rebuilding stats after select timeout");
                            rebuildStats();
                        }
                        Log($"[{DateTime.Now}] Select DbError {retry + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }
                if (sleep != 10)
                {
                    Log($"[{DateTime.Now}] Resuming After Select DbError");
                }
                selectWatch.Stop();

                if (dbFacts.Count == 0)
                    break;

                Stopwatch saveWatch = new Stopwatch();
                //Log($"[{DateTime.Now}] Processing {dbFacts.Count.ToString("N0")} Factors {nameof(TFTestPaged)}");
                foreach (var dbFact in dbFacts)
                {


                    if (dbFact == null)
                        break;

                    var unfactored = dbFact.Factors.Where(x => x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite).ToList();

                    foreach (var unfactor in unfactored)
                    {
                        if ((int)unfactor.Type > 0)
                            continue;

                        var factor = new GmpInt(unfactor.P);
                        factWatch.Start();
                        var factorization = Factorization.FactorTrialDivide(factor, 99999, primes);
                        factWatch.Stop();
                        if (factorization.Factors.Count == 1)
                        {
                            dbFact.TDiv = 6;

                            continue;
                        }
                        else
                        {
                            dbFact.TDiv = 6;
                            dbFact.Factors.Remove(unfactor);
                            dbFact.Type = factorization.Factors.All(x => (int)x.P.Primality() > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                            dtoWatch.Start();
                            dbFact.Factors.AddRange(factorization.Factors.Select(f =>
                                        new DbFactor
                                        {
                                            P = f.P.ToString(),
                                            Power = f.Power,
                                            Type = (PrimalityType)f.P.Primality(),
                                            Digits = f.P.ToString().Length,
                                            Bits = MathLib.BitLength(f.P)
                                        }
                                ));
                            dtoWatch.Stop();
                        }

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
                            Log($"[{DateTime.Now}] Rebuilding stats after save timeout");
                            rebuildStats();

                        }
                        Log($"[{DateTime.Now}] Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
                        System.Threading.Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                }

                saveWatch.Stop();

                sw.Stop();
                if (sleep != 10)
                {
                    Log($"[{DateTime.Now}] Resuming After Save DbError");
                }
                Log($"[{DateTime.Now}] {dbFacts.Last().Id.ToString("N0")} Factored {dbFacts.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed} - dto {dtoWatch.Elapsed}");

            }
        }

        private void rebuildStats()
        {
            var connString = FactorDbContext.DbConnectionString;
            connString=$"{connString};CommandTimeout=300";

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
            var query = $@"  select distinct(cast(p as int)) from factors f(nolock) where
                      1=1 
                      --and [type]>0 
                      and Digits<20
                      order by cast(p as int)
                    ";

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                conn.Open();
                var result = conn.Query<int>(query).ToList();
                return result;
            }
        }

        private int GetIntValue(DbFactor x)
        {
            try
            {
                if (x == null)
                {
                    string bp = "";
                }
                var p = x.P;
                if (string.IsNullOrEmpty(p))
                {
                    string bp = "";
                }
                var value = int.Parse(p);
                return value;
            }
            catch (Exception ex)
            {
                string bp = "";
                return 0;
            }
        }
    }



    /*appsettings.json
    {
  "ConnectionStrings": {
    "EfSeedTest": "Server=localhost;Database=Factors;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
     * */
}