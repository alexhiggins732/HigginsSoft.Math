using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            using var app = provider.CreateScope();
            using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();

            for (var i = 0; i < primes.Count; i++)
            {
                var prime = primes[i];
                List<int> missingIds = GetMissingIds(prime);

                var factorizations = dbContext.Factorizations
                    .Include(x => x.Factors)
                    .Where(x => missingIds.Contains(x.Id))
                    .ToList();
                foreach (var factorization in factorizations)
                {

                    var factors = factorization.Factors.Where(x => (int)x.Type < 0).ToList();
                    foreach (var factor in factors)
                    {
                        var n = BigInteger.Parse(factorization.N);

                        var fact = new Factor<BigInteger>(prime, 0);
                        fact.FactorType = MathLib.PrimalityType.Prime;
                        while (n % prime == 0)
                        {
                            factor.Power++;
                            n /= prime;
                        }
                        if (fact.Power > 0)
                        {
                            factorization.Factors.Remove(factor);
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
                            break;
                        }

                    }
             
                }
                dbContext.SaveChanges();
            }
        }

        private List<int> GetMissingIds(int prime)
        {
            var found = GetDbFactors(prime);

            var class1 = found.Min(x => x.DbFactorizationId);
            var class2 = found.Max(x => x.DbFactorizationId);

            while (class1 - prime > 0)
            {
                class1 -= prime;
            }

            while (class2 - prime > 0)
            {
                class2 -= prime;
            }

            var class1Ids = new List<int>();
            var class2Ids = new List<int>();

            var maxId = 10_000_000;

            while (class1 < maxId)
            {
                class1Ids.Add(class1);
                class1 += prime;
            }

            while (class2 < maxId)
            {
                class2Ids.Add(class2);
                class2 += prime;
            }

            var allClassIds = class1Ids.Union(class2Ids).ToList();
            var allClassIdsString = string.Join(",", allClassIds);

            var query = @$"SELECT z.id
                    FROM Factorizations z -- join factors f on z.id=f.DbFactorizationId
	                    where z.Id in ({allClassIdsString})
                      AND NOT EXISTS (
                          SELECT 1 FROM Factors f
                          WHERE f.DbFactorizationId = z.Id
                            AND f.P = '{prime}' 
                      )";

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {

                var missingIds = conn.Query<int>(query).ToList();
                return missingIds;
            }
        }

        public List<(int Id, int DbFactorizationId)> GetDbFactors(int prime)
        {
            var test = new FactorTest();
            test.SetConnectionString();
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var query = "SELECT Id, DbFactorizationId FROM factors f where p=@p and DbFactorizationId is not null";
                var intPrimes = conn.Query<(int Id, int DbFactorizationId)>(query, new { p = prime }).ToList();
                return intPrimes;
            }
        }
        public List<int> GetDbPrimes()
        {
            var test = new FactorTest();
            test.SetConnectionString();

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var query = "SELECT DISTINCT cast(p as int) FROM factors f where bits<32";
                var intPrimes = conn.Query<int>(query).ToList();
                return intPrimes;
            }
        }
    }
}
