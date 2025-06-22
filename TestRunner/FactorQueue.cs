using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TestRunner
{
    /// <summary>
    /// A queue to allow serial processing of factor removal from the database while clients can concurrently add factors even if they are duplicates
    /// </summary>
    public class FactoringQueue
    {

        public FactoringQueue()
        {
            var test = new FactorTest();
            test.SetConnectionString();
        }
        public void QueueFactor(int factorizationId, BigInteger prime)
        {
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                conn.Execute("INSERT INTO FactorQueue (Prime, factorizationId) VALUES ({0}, {1});", new { prime, factorizationId });
            }
        }

        public static void QueueFactors(List<int> factorizationIds, BigInteger prime)
        {
            var table = new DataTable();
            table.Columns.Add("Prime", typeof(string));
            table.Columns.Add("FactorizationId", typeof(int));

            foreach (var id in factorizationIds)
            {

                var row = table.NewRow();
                row["Prime"] = prime.ToString();
                row["FactorizationId"] = id;
                table.Rows.Add(row);

            }
            ExecuteWithRetry(() =>
            {
                using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
                conn.Open();

                using var bulk = new SqlBulkCopy(conn)
                {
                    DestinationTableName = "FactorQueue"
                };
                bulk.ColumnMappings.Add("Prime", "Prime");
                bulk.ColumnMappings.Add("FactorizationId", "FactorizationId");
                bulk.WriteToServer(table);
            });
        }

        public static void QueueFactors(List<(int FactorizationId, BigInteger Prime)> factors)
        {
            var table = new DataTable();
            table.Columns.Add("Prime", typeof(string));
            table.Columns.Add("FactorizationId", typeof(int));

            foreach (var factor in factors)
            {

                var row = table.NewRow();
                row["Prime"] = factor.Prime.ToString();
                row["FactorizationId"] = factor.FactorizationId;
                table.Rows.Add(row);

            }

            ExecuteWithRetry(() =>
            {
                using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
                conn.Open();

                using var bulk = new SqlBulkCopy(conn)
                {
                    DestinationTableName = "FactorQueue"
                };
                bulk.ColumnMappings.Add("Prime", "Prime");
                bulk.ColumnMappings.Add("FactorizationId", "FactorizationId");
                bulk.WriteToServer(table);
                Console.WriteLine($"[{DateTime.Now}] - Queued {factors.Count} factors");
            });
        }

        public static void QueueFactors(List<(int FactorizationId, string Prime)> factors)
        {
            var table = new DataTable();
            table.Columns.Add("Prime", typeof(string));
            table.Columns.Add("FactorizationId", typeof(int));

            foreach (var factor in factors)
            {

                var row = table.NewRow();
                row["Prime"] = factor.Prime;
                row["FactorizationId"] = factor.FactorizationId;
                table.Rows.Add(row);

            }

            ExecuteWithRetry(() =>
            {
                using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
                conn.Open();

                using var bulk = new SqlBulkCopy(conn)
                {
                    DestinationTableName = "FactorQueue"
                };
                bulk.ColumnMappings.Add("Prime", "Prime");
                bulk.ColumnMappings.Add("FactorizationId", "FactorizationId");
                bulk.WriteToServer(table);
                Console.WriteLine($"[{DateTime.Now}] - Queued {factors.Count} factors");
            });
        }

        static void ExecuteWithRetry(Action act, int numRetries = 20)
        {
            int sleep = 10;
            for (int i = 0; i < numRetries; i++)
            {
                try
                {
                    act();
                    return;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // deadlock
                    {
                        Console.WriteLine($"[{DateTime.Now}] - Deadlock detected, retrying in {sleep}ms");
                        Thread.Sleep(sleep);
                        sleep *= 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

        }

        static bool TryAcquireDbLock(SqlConnection conn, string lockName)
        {
            var cmd = new SqlCommand("sp_getapplock", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Resource", lockName);
            cmd.Parameters.AddWithValue("@LockMode", "Exclusive");
            cmd.Parameters.AddWithValue("@LockOwner", "Session");
            //cmd.Parameters.AddWithValue("@LockTimeout", 1000);

            var returnParam = cmd.Parameters.Add("@Result", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            cmd.ExecuteNonQuery();

            return (int)returnParam.Value >= 0;
        }

        public static void ProcessQueue(bool needsLock = true)
        {
            //const string mutexName = "Global\\FactorProcessorAppMutex";
            //bool createdNew;

            //using var mutex = new Mutex(true, mutexName, out createdNew);

            //if (!createdNew)
            //{
            //    Console.WriteLine("Another instance of the application is already running.");
            //    return;
            //}
            var test = new FactorTest();
            test.SetConnectionString();
            if (needsLock)
            {
                using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
                conn.Open();

                string lockName = "FactorProcessor_Global";

                if (!TryAcquireDbLock(conn, lockName))
                {
                    Console.WriteLine("Another instance is already processing. Exiting.");
                    return;
                }
            }
            // 🔐 This is the only running instance
            //AppDomain.CurrentDomain.ProcessExit += (s, e) => tr mutex.ReleaseMutex();


            Console.WriteLine($"Executing {nameof(ProcessQueue)}");
            try
            {
                ProcesseQueueBatched();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing queue: {ex}");
            }
            return;

            //var test = new FactorTest();
            //test.SetConnectionString();
            //// todo: use status to mark as picked up and completed to allow concurrency.
            //string nextQueuedIdQuery = "select top 1000 factorizationId, prime from factorqueue where processed=0";
            //var sw = Stopwatch.StartNew();
            //int count = 0;
            //using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            //{

            //    var batch = conn.Query<(int FactorizationId, string Prime)>(nextQueuedIdQuery).ToList();

            //    while (batch.Count > 0)
            //    {
            //        sw.Restart();
            //        List<int> failed = new();
            //        foreach (var item in batch)
            //        {
            //            count++;
            //            if (count % 100 == 0)
            //                Console.WriteLine($"[{DateTime.Now}] - Processed {count} factors");

            //            bool processed = ProcessFactor(item);
            //            if (!processed)
            //                failed.Add(item.FactorizationId);

            //            //conn.Execute($"update factorqueue set processed=1 where factorizationId={item.FactorizationId}");

            //        }
            //        var processedIds = batch.Where(x => !failed.Contains(x.FactorizationId)).Select(x => x.FactorizationId).ToList();
            //        if (processedIds.Count == 0)
            //        {
            //            Console.WriteLine($"[{DateTime.Now}] - No factors processed");
            //            break;
            //        }
            //        string ids = string.Join(", ", processedIds);
            //        conn.Execute($"update factorqueue set processed=1 where factorizationId in ({ids})");

            //        sw.Stop();
            //        Console.WriteLine($"[{DateTime.Now}] Factored {batch.Count} items in {sw.Elapsed}");
            //        batch = conn.Query<(int FactorizationId, string Prime)>(nextQueuedIdQuery).ToList();
            //    }

            //}
        }


        private static bool ProcessFactor((int FactorizationId, string Prime) item)
        {
            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();
            using var app = provider.CreateScope();
            using var _db = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            List<int> errorIds = new();
            var sw = Stopwatch.StartNew();

            var factorization = _db.Factorizations
                .Include(fz => fz.Factors)
                .FirstOrDefault(fz => fz.Id == item.FactorizationId);

            if (factorization == null)
            {
                return false;
            }

            var result = RemovePrimeFactor(factorization, item.Prime);
            if (result.hasError)
                return false;

            if (result.isDirty && !result.hasError)
            {
                _db.SaveChanges();
            }

            app.Dispose();
            _db.Dispose();
            return true;
        }

        /// <summary>
        /// Processes the queue of factors to be removed from the database.
        /// </summary>
        public static void ProcesseQueueBatched(int batchSize = 100)
        {
            var test = new FactorTest();
            test.SetConnectionString();
            // todo: use status to mark as picked up and completed to allow concurrency.
            string nextQueuedIdQuery = @"
                update top (100)
                    FactorQueue
                set JobId=@JobId
                Output
                    inserted.FactorizationId, inserted.Prime
                    where processed=0 and JobId is null;";
            var sw = Stopwatch.StartNew();
            var helperWatch = Stopwatch.StartNew();
            int count = 0;

            //var services = new ServiceCollection();
            //services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            //var provider = services.BuildServiceProvider();
            //using var app = provider.CreateScope();
            //using var _db = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            var helper = new FactorDbHelper();
            bool useFactorHelper = bool.Parse(bool.TrueString);


            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var jobId = Guid.NewGuid();

                var batch = conn.Query<(int FactorizationId, string Prime)>(nextQueuedIdQuery, new { jobId })
                    .ToLookup(x => x.FactorizationId)
                        .ToDictionary(x => x.Key, x => x.First().Prime);

                DateTime LastAdd = DateTime.Now;
                while (batch.Count > 0)
                {
                    Console.WriteLine($"[{LastAdd}] - Running batch of {batch.Count}");
                    sw.Restart();
                    helperWatch.Restart();
                    List<int> failed = new();

                    var batchIds = batch.Select(x => x.Key).ToList();



                    foreach (var item in batch)
                    {
                        count++;
                        if (count % 100 == 0)
                        {
                            var message = $"[{DateTime.Now}] - Processed {count.ToString("N0")} factors in {DateTime.Now.Subtract(LastAdd)}";
                            Console.WriteLine(message);
                            Console.Title = message;
                            LastAdd = DateTime.Now;
                        }
                        helperWatch.Start();
                        bool added = helper.AddFactor(item.Key, item.Value);
                        helperWatch.Stop();
                        if (!added)
                        {
                            failed.Add(item.Key);
                        }
                        //else
                        //{
                        //    conn.Execute("Update factorqueue set processed=1 where factorizationId=@factorizationId and prime=@prime",
                        //        new { factorizationId = item.Key, prime = item.Value });
                        //}



                    }
                    Console.WriteLine($"[{DateTime.Now}] - Helper added {batch.Count.ToString("N0")} factors in {sw.Elapsed} - Helper: {helperWatch.Elapsed}");



                    if (failed.Count > 0)
                    {
                        conn.Execute("insert into FailedFactorQueue (FactorizationId, Prime) select factorizationId, prime from factorqueue where factorizationId in @ids",
                            new { ids = failed });
                    }

                    conn.Execute("update FactorQueue set processed=1 where factorizationId in @ids",
                          new { ids = batchIds });

                    //var processedIds = batchIds.Where(x => !failed.Contains(x)).ToList();
                    //if (processedIds.Any())
                    //{
                    //    string ids = string.Join(", ", processedIds);
                    //    conn.Execute($"update factorqueue set processed=1 where factorizationId in ({ids})");
                    //}

                    // clear the lookup in each back to make sure factors are refreshing.

                    jobId = Guid.NewGuid();

                    batch = conn.Query<(int FactorizationId, string Prime)>(nextQueuedIdQuery, new { jobId })
                       .ToLookup(x => x.FactorizationId)
                           .ToDictionary(x => x.Key, x => x.First().Prime);
                }

            }
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now}] Factored {count} items in {sw.Elapsed}");
        }



        /// <summary>
        /// Processes the queue of factors to be removed from the database.
        /// </summary>
        public static void ProcesseQueueBatched2(int batchSize = 100)
        {
            var test = new FactorTest();
            test.SetConnectionString();
            // todo: use status to mark as picked up and completed to allow concurrency.
            string nextQueuedIdQuery = "select top 100 factorizationId, prime from factorqueue where processed=0 order by NEWID()";
            var sw = Stopwatch.StartNew();
            int count = 0;

            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();
            using var app = provider.CreateScope();
            using var _db = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            var helper = new FactorDbHelper();
            bool useFactorHelper = bool.Parse(bool.TrueString);

            List<(int FactorizationId, BigInteger N)>? nLookup = null;

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                Action setLookup = () =>
                {
                    if (nLookup == null)
                        nLookup = conn.Query<(int FactorizationId, string n)>("select id, n from factorizations where type<1")
                        .Select(x => (x.FactorizationId, BigInteger.Parse(x.n)))
                        .ToList();
                };
                var batch = conn.Query<(int FactorizationId, string Prime)>(nextQueuedIdQuery)
                    .ToLookup(x => x.FactorizationId)
                        .ToDictionary(x => x.Key, x => x.First().Prime);

                DateTime LastAdd = DateTime.Now;
                while (batch.Count > 0)
                {
                    sw.Restart();
                    List<int> failed = new();

                    var batchIds = batch.Select(x => x.Key).ToList();

                    if (useFactorHelper)
                    {
                        var helperWatch = Stopwatch.StartNew();
                        foreach (var item in batch)
                        {
                            count++;
                            if (count % 100 == 0)
                            {
                                var message = $"[{DateTime.Now}] - Processed {count.ToString("N0")} factors in {DateTime.Now.Subtract(LastAdd)}";
                                Console.WriteLine(message);
                                Console.Title = message;
                                LastAdd = DateTime.Now;
                            }

                            bool added = helper.AddFactor(item.Key, item.Value);
                            if (!added)
                            {
                                var factor = BigInteger.Parse(item.Value);
                                Console.WriteLine($"[{DateTime.Now}] - Failed to add factor {item.Value} to {item.Key}");
                                setLookup();

                                var factValue = nLookup?.FirstOrDefault(x => x.N % factor == 0);
                                if (factValue == null || !factValue.HasValue
                                    || factValue.Value.FactorizationId == item.Key
                                    || factValue.Value.FactorizationId == 0)
                                {
                                    conn.Execute("delete from factorqueue where factorizationId=@factorizationId and prime=@prime",
                                        new { factorizationId = item.Key, prime = item.Value });
                                }
                                //failed.Add(item.Key);
                                else
                                {
                                    var fact = factValue.Value;
                                    // update the database with the correct factorization.id
                                    Console.WriteLine($"[{DateTime.Now}] - Updating factor {item.Value} from {item.Key} to Id {fact.FactorizationId}");
                                    conn.Execute("update factorqueue set factorizationId=@newFactorizationId where factorizationId=@oldFactorizationId and prime=@prime",
                                        new { newFactorizationId = fact.FactorizationId, oldFactorizationId = item.Key, prime = item.Value });


                                }
                            }
                            else
                            {
                                conn.Execute("Update factorqueue set processed=1 where factorizationId=@factorizationId and prime=@prime",
                                    new { factorizationId = item.Key, prime = item.Value });
                            }



                        }
                        Console.WriteLine($"[{DateTime.Now}] - Helper added {batch.Count.ToString("N0")} factors in {sw.Elapsed}");

                    }

                    else
                    {

                        var selectWatch = Stopwatch.StartNew();
                        var factorizations = _db.Factorizations.Include(x => x.Factors).Where(x => batchIds.Contains(x.Id)).ToList();
                        selectWatch.Stop();
                        Console.WriteLine($"[{DateTime.Now}] - Loaded {batch.Count} factorizations in {sw.Elapsed}");

                        var factorWatch = Stopwatch.StartNew();
                        foreach (var factor in factorizations)
                        {
                            count++;
                            if (count % 100 == 0)
                                Console.WriteLine($"[{DateTime.Now}] - Processed {count} factors");


                            var prime = batch[factor.Id];
                            var result = RemovePrimeFactor(factor, prime);
                            if (result.hasError)
                                failed.Add(factor.Id);

                        }
                        factorWatch.Stop();
                        Console.WriteLine($"[{DateTime.Now}] - Factored {batch.Count} factorizations in {factorWatch.Elapsed}");

                        var saveWatch = Stopwatch.StartNew();
                        _db.SaveChanges();
                        saveWatch.Stop();
                        Console.WriteLine($"[{DateTime.Now}] - Saved {batch.Count} factorizations in {saveWatch.Elapsed}");
                    }
                    //var processedIds = batchIds.Where(x => !failed.Contains(x)).ToList();
                    //if (processedIds.Any())
                    //{
                    //    string ids = string.Join(", ", processedIds);
                    //    conn.Execute($"update factorqueue set processed=1 where factorizationId in ({ids})");
                    //}

                    // clear the lookup in each back to make sure factors are refreshing.
                    nLookup = null;
                    batch = conn.Query<(int FactorizationId, string Prime)>(nextQueuedIdQuery)
                        .ToLookup(x => x.FactorizationId)
                        .ToDictionary(x => x.Key, x => x.First().Prime);
                }

            }
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now}] Factored {count} items in {sw.Elapsed}");
        }

        /// <summary>
        /// Removes a prime factor from the factorization if it exists and validates the new factorization match the original number.
        /// </summary>
        /// <param name="factorization"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static (bool isDirty, bool hasError) RemovePrimeFactor(DbFactorization factorization, string number)
        {
            bool isDirty = false;
            var compositeFactors = factorization.Factors.Where(x => (int)x.Type < 1).ToList();
            var prime = BigInteger.Parse(number);
            foreach (var composite in compositeFactors)
            {

                var n = BigInteger.Parse(composite.P);
                var fact = new Factor<BigInteger>(prime, 0) { FactorType = MathLib.PrimalityType.Prime };

                while (n % prime == 0)
                {
                    fact.Power++;
                    n /= prime;
                }

                if (fact.Power > 0)
                {
                    isDirty = true;
                    //factorization.Factors.Remove(composite);


                    composite.P = n.ToString();
                    composite.Power = 1;
                    composite.Type = (PrimalityType)(int)GmpInt.Primality(n);
                    composite.Digits = composite.P.Length;
                    composite.Bits = MathLib.BitLength(n);

                    factorization.Factors.Add(new DbFactor
                    {
                        P = fact.P.ToString(),
                        Power = fact.Power,
                        Type = (PrimalityType)fact.FactorType,
                        Digits = fact.P.ToString().Length,
                        Bits = MathLib.BitLength(fact.P)
                    });

                }
            }

            var newValue = factorization.Factors
                .Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power))
                .Aggregate((a, b) => a * b);

            if (newValue.ToString() != factorization.N)
            {
                Console.WriteLine($"[{DateTime.Now}] - Revalidating Factorization {factorization.Id}: Failed removing {prime}");
                var f = new FactorizationBigInteger();
                var n = BigInteger.Parse(factorization.N);

                foreach (var factor in factorization.Factors)
                {
                    var fact = new Factor<BigInteger>(BigInteger.Parse(factor.P), 0);
                    while (n / fact.P == 0)
                    {
                        fact.Power++;
                        n /= fact.P;
                    }
                    if (fact.Power > 0)
                    {
                        f.Factors.Add(fact);
                    }
                }
                if (n > 1)
                {
                    f.Factors.Add(new Factor<BigInteger>(n, 1));
                }
                if (f.ToString() == factorization.N)
                {
                    factorization.Factors.Clear();
                    factorization.Factors = f.Factors.Select(x => new DbFactor
                    {
                        P = x.P.ToString(),
                        Power = x.Power,
                        Type = (PrimalityType)(int)GmpInt.Primality(x.P),
                        Digits = x.P.ToString().Length,
                        Bits = MathLib.BitLength(x.P)
                    }).ToList();
                    isDirty = true;
                }
                else
                {
                    var message = $"[{DateTime.Now}] - Factorization {factorization.Id}: Verification failed removing {prime}: {newValue} != {factorization.N}";
                    File.AppendAllText("FailedFactors.log", message + Environment.NewLine);
                    Console.WriteLine(message);
                    return (false, true);
                }

            }

            var newType = factorization.Factors.All(x => (int)x.Type > 0)
                ? PrimalityType.ProbablePrime
                : PrimalityType.Composite;

            if (newType != factorization.Type)
            {
                factorization.Type = newType;
                isDirty = true;
            }

            return (isDirty, false);
        }

        internal static void VerifyProcessed()
        {
            var test = new FactorTest();
            test.SetConnectionString();
            // todo: use status to mark as picked up and completed to allow concurrency.

            int startId = 0;
            string nextQueuedIdQuery = "select top 100 p.id, p.FactorizationId, p.Prime from ProcessedFactorQueue p join Factorizations z on p.FactorizationId=z.Id where p.Id>@startId and z.type<1";
            var sw = Stopwatch.StartNew();
            int count = 0;
            int hits = 0;
            int total = 0;
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                total = conn.ExecuteScalar<int>($@"select count(0) from ProcessedFactorQueue p join Factorizations z on p.FactorizationId=z.Id
                    where z.type<1");
            }


            var helper = new FactorDbHelper();

            bool hasMore = true;
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {

                while (hasMore)
                {


                    var batch = conn.Query<(int Id, int FactorizationId, string Prime)>(nextQueuedIdQuery, new { startId })
                            .ToList();
                    hasMore = batch.Count > 0;
                    if (hasMore)
                    {
                        foreach (var item in batch)
                        {
                            count++;
                            Console.Title = $"[{DateTime.Now}] Processing ({count.ToString("n0")} of {total.ToString("n0")})";
                            // Console.WriteLine("Executing AddFactor({0}, {1})", item.FactorizationId, item.Prime);
                            if (helper.AddFactor(item.FactorizationId, item.Prime))
                                hits++;
                            startId = item.Id;
                        }
                    }
                }


            }
        }

        internal static void ProcessBatchFiles()
        {
            Console.WriteLine($"[{DateTime.Now}] Processing batch files");
            var di= new DirectoryInfo(AppContext.BaseDirectory);
            var divBats = Directory.GetFiles(di.FullName, "*.tdiv.bat");
            Console.WriteLine($"[{DateTime.Now}] Found {divBats.Length} tdiv files");
            var factorBats = Directory.GetFiles(di.FullName, "*.factors.bat");
            Console.WriteLine($"[{DateTime.Now}] Found {factorBats.Length} fact files");
            {
                var divUpdates = divBats.SelectMany(x => File.ReadAllLines(x))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => $"update factorizations set tdiv={x.Trim().Split(' ')[3]} where id={x.Trim().Split(' ')[2]}")
                    .Distinct()
                    .ToList();
                File.WriteAllLines("tdiv.sql", divUpdates);
            }
            var factorUpdates = factorBats.SelectMany(x => File.ReadAllLines(x))
                 .Where(x => !string.IsNullOrWhiteSpace(x))
                 .Select(x => $"(GetDate() ,0, '{x.Trim().Split(' ')[3]}')")
                 .Distinct()
                 .ToList();
         
            File.WriteAllText("factors.sql", @$"
INSERT INTO [dbo].[FactorQueue]
([CreatedAt]
,[Processed]
,[Prime]
)
VALUES 
{string.Join(",\r\n", factorUpdates)})
    ");

        }
    }
}
