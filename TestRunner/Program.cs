using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Numerics;

namespace TestRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var efTests = new FactorTest();
            efTests.TDiv20();

        }
    }

    public class FactorTest
    {
        void Log(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);

        }
        IEnumerable<int> GetPrimesTo(int max)
        {
            var gen = new PrimeGenerator((int)max);
            var result = gen.ToList();
            return result;
        }

        public void TFTestPaged()
        {
            Log($"[{DateTime.Now}] Starting test {nameof(TFTestPaged)}");
            if (bool.Parse(bool.FalseString))
            {
                Log($"[{DateTime.Now}] Seeding database {nameof(TFTestPaged)}");
                //EfSeedTest();
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
                if (selectWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
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
                            Log($"[{DateTime.Now}] Rebuilding stats after save timeout");
                            rebuildStats();

                        }
                        Log($"[{DateTime.Now}] Save DbError {i + 1} sleeping until {DateTime.Now.AddMilliseconds(sleep)} - {ex.Message}");
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
                    Log($"[{DateTime.Now}] Resuming After Save DbError");
                }
                Log($"[{DateTime.Now}] {dbFacts.Last().Id.ToString("N0")} Factored {dbFacts.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed} - dto {dtoWatch.Elapsed}");


                app.Dispose();

                dbContext.Dispose();
            }
        }


        public void TDiv20()
        {
            //RunFactorization(x => FactorizationBigInteger.FactorTrialDivide(x), 20);
            var ecm = new NumericsEcm();
            var tDiv = 20;
            RunFactorization(x => ecm.PM1(x, tDiv), tDiv - 4, tDiv - 4);
            RunFactorization(x => ecm.PP1(x, tDiv), tDiv - 3, tDiv - 3);
            RunFactorization(x => ecm.PP1(x, tDiv), tDiv - 2, tDiv - 2);
            RunFactorization(x => ecm.PP1(x, tDiv), tDiv - 1, tDiv - 1);
            RunFactorization(x => ecm.ECM(x, tDiv), tDiv, tDiv);
        }


        public void RunFactorization(Func<BigInteger, FactorizationBigInteger> factorIt, int maxDbTDiv, int updateDbTDiv, int batchSize = 2000)
        {
            Log($"[{DateTime.Now}] Starting test {nameof(TFTestPaged)}");
            if (bool.Parse(bool.FalseString))
            {
                Log($"[{DateTime.Now}] Seeding database {nameof(TFTestPaged)}");
                //EfSeedTest();
            }
            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            //Log($"[{DateTime.Now}] Getting Primes {nameof(TFTestPaged)}");
            // var primes = GetPrimesTo(1_000_000);



            //Log($"[{DateTime.Now}] Getting Factors {nameof(GetSmallFactors)}");
            //var smallFactors = GetSmallFactors();

            //Log($"[{DateTime.Now}] Filtering Primes {nameof(GetSmallFactors)}");
            //var filtered = primes.Where(p => smallFactors.Contains(p)).ToList();
            //primes = filtered;
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
                           .Where(x => x.TDiv < maxDbTDiv && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite))
                           .OrderBy(x => x.Id)
                           .Take(batchSize).ToList();
                        break;
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
                if (selectWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
                if (dbFacts.Count == 0)
                    break;

                Stopwatch saveWatch = new Stopwatch();
                int idx = 0;
                //Log($"[{DateTime.Now}] Processing {dbFacts.Count.ToString("N0")} Factors {nameof(TFTestPaged)}");
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
                            dbFact.TDiv = updateDbTDiv;

                            continue;
                        }
                        else
                        {

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

                    dbFact.TDiv = maxDbTDiv;
                    dbFact.Type = dbFact.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;


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
                if (saveWatch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    rebuildStats();
                }
                sw.Stop();
                if (sleep != 10)
                {
                    Log($"[{DateTime.Now}] Resuming After Save DbError");
                }
                Log($"[{DateTime.Now}] {dbFacts.Last().Id.ToString("N0")} Factored {dbFacts.Count.ToString("N0")} factors in {sw.Elapsed} - factor {factWatch.Elapsed} select {selectWatch.Elapsed} save - {saveWatch.Elapsed} - dto {dtoWatch.Elapsed}");


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
