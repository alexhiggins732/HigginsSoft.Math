using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    public class FactorBaseSiever
    {
        internal void SieveDbPrimes()
        {
            var primes = GetDbPrimes();
            primes.Sort();

            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            using var app = provider.CreateScope();
            using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            int startPrime = 5754043;
            for (var i = 0; i < primes.Count; i++)
            {

                var prime = primes[i];
                if (prime <= startPrime)
                {
                    continue;
                }
                var sw = Stopwatch.StartNew();
                //Console.WriteLine($"{DateTime.Now} ({(i + 1).ToString("N0")} of {primes.Count.ToString("N0")}) Processing {prime.ToString("N0")}");

                List<int> missingIds = GetMissingIds(prime);
                if (missingIds.Count == 0)
                {
                    sw.Stop();
                    if (i % 50 == 0)
                        dbContext.SaveChanges();
                    //Console.WriteLine($"{DateTime.Now} ({(i + 1).ToString("N0")} of {primes.Count.ToString("N0")}) Verified {prime.ToString("N0")} in {sw.Elapsed}");
                    continue;
                }
                Console.WriteLine($"{DateTime.Now} ({(i + 1).ToString("N0")} of {primes.Count.ToString("N0")}) - {prime.ToString("N0")} : Found {missingIds.Count.ToString("N0")} missing factors");
                var factorizations = dbContext.Factorizations
                    .Include(x => x.Factors)
                    .Where(x => missingIds.Contains(x.Id))
                    .ToList();
                foreach (var factorization in factorizations)
                {

                    var compositeFactors = factorization.Factors.Where(x => (int)x.Type < 1).ToList();
                    foreach (var composite in compositeFactors)
                    {
                        var n = BigInteger.Parse(composite.P);

                        var fact = new Factor<BigInteger>(prime, 0);
                        fact.FactorType = MathLib.PrimalityType.Prime;
                        while (n % prime == 0)
                        {
                            fact.Power++;
                            n /= prime;
                        }
                        if (fact.Power > 0)
                        {
                            factorization.Factors.Remove(composite);
                            factorization.Factors.Add(new DbFactor()
                            {
                                P = n.ToString(),
                                Power = 1,
                                Type = (PrimalityType)(int)GmpInt.Primality(n),
                                Digits = n.ToString().Length,
                                Bits = MathLib.BitLength(n)
                            });
                            factorization.Factors.Add(new DbFactor()
                            {
                                P = fact.P.ToString(),
                                Power = fact.Power,
                                Type = (PrimalityType)fact.FactorType,
                                Digits = fact.P.ToString().Length,
                                Bits = MathLib.BitLength(fact.P)
                            });
                            //break;
                        }

                    }

                    var newValue = factorization.Factors.Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power)).Aggregate((a, b) => a * b);
                    var newValueString = newValue.ToString();
                    var nValueString = factorization.N.ToString();
                    if (newValueString != nValueString)
                    {
                        string bp = "Invalid factorization";
                    }

                    factorization.Type = factorization.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                }
                if (i % 50 == 0)
                    dbContext.SaveChanges();
            }
            dbContext.SaveChanges();
        }

        private List<int> GetMissingIds(int prime)
        {
            var classWatch = Stopwatch.StartNew();
            var classes = GetDbResidueClasses(prime);
            classWatch.Stop();
            if (classes.Count == 0)
            {
                Console.WriteLine($"[{DateTime.Now}] Warning could not find class for prime {prime}");
                return new();
            }


            var class1 = classes.Min();
            var class2 = classes.Max();

            if (class1 == class2)
            {
                if (prime == 1)
                {
                    class1 = 2;
                }
                Console.WriteLine($"[{DateTime.Now}] Warning found only 1 class for prime {prime}");
            }

            //var allClassIds = class1Ids.Union(class2Ids).ToList();
            var allClassIdsString = string.Join(",", classes.Distinct());

            var query = @$"SELECT z.id
                    FROM Factorizations z -- join factors f on z.id=f.DbFactorizationId
	                    where z.Id% {prime} in ({allClassIdsString})
                      AND NOT EXISTS (
                          SELECT 1 FROM Factors f
                          WHERE f.DbFactorizationId = z.Id
                            AND f.P = '{prime}' 
                      )";
            var idWatch = Stopwatch.StartNew();
            List<int> missingIds = new();
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                missingIds = conn.Query<int>(query).ToList();
            }
            idWatch.Stop();

            //Console.WriteLine($"[{DateTime.Now}] - Verification {prime}: Found class in {classWatch} for {missingIds.Count} in {idWatch.Elapsed}");
            return missingIds;
        }

        Dictionary<int, List<int>>? residueClasses = null;
        Dictionary<int, List<int>> ResidueClasses
        {
            get
            {
                if (residueClasses == null)
                {
                    using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
                    {
                        var query = "select distinct cast(p as int), z.id %(cast(p as int)) as ClassId from Factorizations z join Factors f on z.Id=f.DbFactorizationId where f.Bits<32";
                        var intPrimes = conn.Query<(int, int)>(query).ToList();
                        residueClasses = new();
                        foreach (var item in intPrimes)
                        {
                            if (!residueClasses.ContainsKey(item.Item1))
                            {
                                residueClasses.Add(item.Item1, new());
                            }
                            residueClasses[item.Item1].Add(item.Item2);
                        }

                    }
                }
                return residueClasses;
            }
        }

        public List<int> GetDbResidueClasses(int prime)
        {
            if (ResidueClasses.ContainsKey(prime))
                return ResidueClasses[prime];
            return new();

        }

        public List<(int Id, int DbFactorizationId)> GetDbFactors(int prime)
        {
            var test = new FactorTest();
            test.SetConnectionString();
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var query = "SELECT Id, DbFactorizationId FROM factors f where f.bits<=31 and p=@p and DbFactorizationId is not null";
                var intPrimes = conn.Query<(int Id, int DbFactorizationId)>(query, new { p = prime.ToString() }).ToList();
                return intPrimes;
            }
        }
        public List<int> GetDbPrimes()
        {
            var test = new FactorTest();
            test.SetConnectionString();

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var query = "SELECT DISTINCT cast(p as int) FROM factors f where bits<32 and DbFactorizationId is not null ";
                var intPrimes = conn.Query<int>(query).ToList();
                return intPrimes;
            }
        }
    }
    public class FactorDbHelper()
    {
        const int MaxTDiv = 256;
        internal void SetTDiv(int factorizationId, int tDiv)
        {
            if (tDiv < MaxTDiv)
            {
                var t = new FactorTest();
                t.SetConnectionString();
                using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
                {
                    var query = "UPDATE Factorizations SET TDiv = @TDiv WHERE Id = @Id";
                    conn.Execute(query, new { TDiv = tDiv, Id = factorizationId });
                }
            }

        }
        internal void AddFactor(int factorizationId, string factorString)
        {
            var t = new FactorTest();
            t.SetConnectionString();
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {

                var dbFactorization = conn.QueryFirstOrDefault<(int id, string n)?>("SELECT id, n FROM Factorizations WHERE Id = @DbFactorizationId",
                    new { DbFactorizationId = factorizationId, P = factorString });

                if (dbFactorization is null)
                {
                    Console.WriteLine($"DbFactorization {factorizationId} not found");
                    return;
                }
                else if (dbFactorization.Value.n == factorString)
                {
                    Console.WriteLine($"Factor {factorString} already exists as N for the DbFactorization {factorizationId}");
                    return;
                }
                var factors = conn.Query<(int Id, string P, int Power, int Type)>("SELECT id, p, power, type FROM Factors WHERE DbFactorizationId = @DbFactorizationId",
                    new { DbFactorizationId = factorizationId, P = factorString });

                var bigN = BigInteger.Parse(dbFactorization.Value.n);
                var newFactor = BigInteger.Parse(factorString);
                var newFactorPrimalityType = (MathLib.PrimalityType)(int)GmpInt.Primality(newFactor);

                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {


                    foreach (var factor in factors)
                    {

                        var dbFactor = BigInteger.Pow(BigInteger.Parse(factor.P), factor.Power);

                        if (dbFactor <= newFactor)
                            continue;
                        // don't  
                        var f = new Factor<BigInteger>(newFactor, 0);
                        f.FactorType = newFactorPrimalityType;

                        while (dbFactor % newFactor == 0)
                        {
                            f.Power++;
                            dbFactor /= newFactor;
                        }
                        if (f.Power > 0)
                        {
                            // Remove the old factor
                            var deleteQuery = "update factors set dbFactorizationId = null where Id = @Id";
                            conn.Execute(deleteQuery, new { Id = factor.Id }, transaction: trans);
                            // Add the new factor
                            var insertQuery = "INSERT INTO Factors (DbFactorizationId, P, Power, Type, Digits, Bits) VALUES (@DbFactorizationId, @P, @Power, @Type, @Digits, @Bits)";
                            var pParams = new
                            {
                                DbFactorizationId = factorizationId,
                                P = f.P.ToString(),
                                Power = f.Power,
                                Type = (int)f.FactorType,
                                Digits = f.P.ToString().Length,
                                Bits = MathLib.BitLength(f.P)
                            };
                            conn.Execute(insertQuery, pParams, transaction: trans);

                            if (dbFactor > 1)
                            {
                                var nParams = new
                                {
                                    DbFactorizationId = factorizationId,
                                    P = dbFactor.ToString(),
                                    Power = 1,
                                    Type = (int)GmpInt.Primality(dbFactor),
                                    Digits = dbFactor.ToString().Length,
                                    Bits = MathLib.BitLength(dbFactor)
                                };
                                conn.Execute(insertQuery, nParams, transaction: trans);
                            }
                        }


                    }

                    // get updated factors from the database
                    // get updated factors from the database
                    factors = conn.Query<(int Id, string P, int Power, int Type)>("SELECT id, p, power, type FROM Factors WHERE DbFactorizationId = @DbFactorizationId",
                            new { DbFactorizationId = factorizationId, P = factorString }, trans);

                    var newFactorValue = factors.Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power)).Aggregate((a, b) => a * b);
                    if (newFactorValue != bigN)
                    {
                        var message = $"Invalid factorization for {factorizationId}: {newFactorValue} != {bigN}";
                        Console.WriteLine(message);
                        throw new Exception(message);
                    }
                    else
                    {
                        var newPrimalityType = factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                        var updateQuery = "UPDATE Factorizations SET Type = @newPrimalityType WHERE Id = @factorizationId";
                        conn.Execute(updateQuery, new { factorizationId, newPrimalityType }, trans);
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding factor: {ex}");
                    try
                    {
                        trans.Rollback();
                    }
                    catch { }
                }
            }
        }
    }
}
