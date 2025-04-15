using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            table.Columns.Add("Prime", typeof(int));
            table.Columns.Add("FactorizationId", typeof(int));

            foreach (var id in factorizationIds)
            {

                var row = table.NewRow();
                row["Prime"] = prime;
                row["FactorizationId"] = id;
                table.Rows.Add(row);

            }

            using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
            conn.Open();

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = "FactorQueue"
            };
            bulk.ColumnMappings.Add("Prime", "Prime");
            bulk.ColumnMappings.Add("FactorizationId", "FactorizationId");
            bulk.WriteToServer(table);
        }


        /// <summary>
        /// Processes the queue of factors to be removed from the database.
        /// </summary>
        public void ProcessQueue()
        {
            using var _db = new FactorDbContext();

            var queue = _db.FactorQueue
                .Where(q => !q.Processed)
                .OrderBy(q => q.Id)
                .Take(1000)
                .ToList();

            foreach (var item in queue)
            {
                var factorization = _db.Factorizations
                    .Include(fz => fz.Factors)
                    .FirstOrDefault(fz => fz.Id == item.FactorizationId);

                if (factorization == null)
                {
                    item.Processed = true;
                    continue;
                }

                var result = RemovePrimeFactor(factorization, item.Prime);
                if (result.hasError)
                {
                    Console.WriteLine($"Skipping due to error on prime {item.Prime}, offset {item.FactorizationId}");
                    continue;
                }

                if (result.isDirty)
                    _db.Update(factorization);

                item.Processed = true;
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Removes a prime factor from the factorization if it exists and validates the new factorization match the original number.
        /// </summary>
        /// <param name="factorization"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private (bool isDirty, bool hasError) RemovePrimeFactor(DbFactorization factorization, string number)
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
                    factorization.Factors.Remove(composite);

                    factorization.Factors.Add(new DbFactor
                    {
                        P = fact.P.ToString(),
                        Power = fact.Power,
                        Type = (PrimalityType)fact.FactorType,
                        Digits = fact.P.ToString().Length,
                        Bits = MathLib.BitLength(fact.P)
                    });

                    factorization.Factors.Add(new DbFactor
                    {
                        P = n.ToString(),
                        Power = 1,
                        Type = (PrimalityType)(int)GmpInt.Primality(n),
                        Digits = n.ToString().Length,
                        Bits = MathLib.BitLength(n)
                    });
                }
            }

            var newValue = factorization.Factors
                .Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power))
                .Aggregate((a, b) => a * b);

            if (newValue.ToString() != factorization.N)
            {
                Console.WriteLine($"[{DateTime.Now}] - Verification failed removing {prime}: {newValue} != {factorization.N}");
                return (false, true);
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
    }
}
