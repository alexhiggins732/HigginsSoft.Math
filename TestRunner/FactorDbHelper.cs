using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Protocols;
using System.Data;
using System.Numerics;


namespace TestRunner
{
    public class FactorDbHelper()
    {
        const int MaxTDiv = 256;
        internal void SetTDiv(int factorizationId, int tDiv)
        {
            if (tDiv < MaxTDiv)
            {
                Console.Write("Setting TDiv to {0} for factorization {1}", tDiv, factorizationId);
                var t = new FactorTest();
                t.SetConnectionString();
                using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
                {
                    var query = "UPDATE Factorizations SET TDiv = @TDiv WHERE Id = @Id";
                    conn.Execute(query, new { TDiv = tDiv, Id = factorizationId });
                }
            }

        }
        internal bool AddFactor(int factorizationId, string factorString)
        {
            var t = new FactorTest();
            t.SetConnectionString();
            bool logDebug = bool.Parse(bool.FalseString);
            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {

                var dbFactorization = conn.QueryFirstOrDefault<(int id, string n)?>("SELECT id, n FROM Factorizations WHERE Id = @DbFactorizationId",
                    new { DbFactorizationId = factorizationId, P = factorString });

                if (dbFactorization is null)
                {
                    Console.WriteLine($"DbFactorization {factorizationId} not found");
                    return false;
                }
                else if (dbFactorization.Value.n == factorString) // this is bad check.
                {
                    Console.WriteLine($"Factor {factorString} already exists as N for the DbFactorization {factorizationId}");
                    return false;
                }
                var factors = conn.Query<(int Id, string P, int Power, int Type)>("SELECT id, p, power, type FROM Factors WHERE DbFactorizationId = @DbFactorizationId",
                    new { DbFactorizationId = factorizationId, P = factorString });

                var bigN = BigInteger.Parse(dbFactorization.Value.n);
                var newFactor = BigInteger.Parse(factorString);
                var newFactorPrimalityType = (MathLib.PrimalityType)(int)GmpInt.Primality(newFactor);
                if (bigN % newFactor != 0)
                {
                    return false;
                }
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {

                    bool result = false;
                    foreach (var factor in factors)
                    {

                        var dbFactor = BigInteger.Pow(BigInteger.Parse(factor.P), factor.Power);

                        if (dbFactor < newFactor)
                        {
                            if ((int)newFactorPrimalityType > 0)
                                continue;

                            var gcd = MathUtil.Gcd(newFactor, dbFactor);
                            if (gcd == 1)
                            {
                                continue;
                            }
                            else
                            {
                                bool found = false;
                                var gcdString = gcd.ToString();
                                foreach (var other in factors)
                                {
                                    if (other.P == gcdString)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                {
                                    // mark the factor as found
                                    return true;
                                }
                                else
                                {
                                    var query = "INSERT INTO Factors (DbFactorizationId, P, Power, Type, Digits, Bits) VALUES (@DbFactorizationId, @P, @Power, @Type, @Digits, @Bits)";
                                    var pParams = new
                                    {
                                        DbFactorizationId = factorizationId,
                                        P = gcdString,
                                        Power = 1,
                                        Type = (int)GmpInt.Primality(gcd),
                                        Digits = gcdString.Length,
                                        Bits = MathLib.BitLength(newFactor)
                                    };
                                    conn.Execute(query, pParams, transaction: trans);
                                    trans.Commit();
                                    return true;
                                }

                            }
                        }

                        else if (dbFactor == newFactor)
                        {
                            if ((int)newFactorPrimalityType > 0)
                            {
                                result = true;
                            }
                            else
                            {
                                string p = "";
                            }
                        }// && (int)GmpInt.Primality(dbFactor) > 0)

                        // don't  
                        else
                        {
                            var f = new Factor<BigInteger>(newFactor, 0);
                            f.FactorType = newFactorPrimalityType;

                            while (dbFactor % newFactor == 0)
                            {
                                f.Power++;
                                dbFactor /= newFactor;
                            }
                            if (f.Power > 0)
                            {
                                result = true;

                                var dbPrimality = GmpInt.Primality(dbFactor);
                                if (logDebug || ((int)dbPrimality > 0) || true)
                                {
                                    var fPrimality = $"{(newFactorPrimalityType == MathLib.PrimalityType.Composite ? "C" : "PRP")}{factorString.Length}";

                                    Console.WriteLine($"Adding factor {f.P} {fPrimality} for C{factor.P.ToString().Length} to {(dbPrimality == MathLib.PrimalityType.Composite ? "C" : "Prp")}{dbFactor.ToString().Length} {factorizationId} ");
                                }

                                // Remove the old factor
                                //var deleteQuery = "update factors set dbFactorizationId = null where Id = @Id";
                                //conn.Execute(deleteQuery, new { Id = factor.Id }, transaction: trans);
                                // Add the new factor
                                var insertQuery = "INSERT INTO Factors (DbFactorizationId, P, Power, Type, Digits, Bits) VALUES (@DbFactorizationId, @P, @Power, @Type, @Digits, @Bits)";
                                var pParams = new
                                {
                                    DbFactorizationId = factorizationId,
                                    P = factorString,
                                    Power = f.Power,
                                    Type = (int)f.FactorType,
                                    Digits = factorString.Length,
                                    Bits = MathLib.BitLength(f.P)
                                };
                                conn.Execute(insertQuery, pParams, transaction: trans);
                                // update the old factor
                                if (dbFactor > 1)
                                {
                                    var updateQuery = "Update factors set P=@p, Power=@Power, Type=@Type, Digits=@Digits, Bits=@Bits where Id = @Id";
                                    var nParams = new
                                    {
                                        Id = factor.Id,
                                        P = dbFactor.ToString(),
                                        Power = 1,
                                        Type = (int)GmpInt.Primality(dbFactor),
                                        Digits = dbFactor.ToString().Length,
                                        Bits = MathLib.BitLength(dbFactor)
                                    };
                                    conn.Execute(updateQuery, nParams, transaction: trans);
                                }
                            }
                        }

                    }
                    if (!result)
                    {
                        if (newFactorPrimalityType > 0)
                        {
                            // insert the new factor;
                            var query = "INSERT INTO Factors (DbFactorizationId, P, Power, Type, Digits, Bits) VALUES (@DbFactorizationId, @P, @Power, @Type, @Digits, @Bits)";
                            var pParams = new
                            {
                                DbFactorizationId = factorizationId,
                                P = factorString,
                                Power = 1,
                                Type = (int)newFactorPrimalityType,
                                Digits = factorString.Length,
                                Bits = MathLib.BitLength(newFactor)
                            };
                            conn.Execute(query, pParams, transaction: trans);
                            trans.Commit();
                            return true;
                        }

                        else
                        {
                            // a new composite factor that doesn't divide an existing composite;
                            using var factored = FactorizationBigInteger.Factor(newFactor, false, true, skipFermat: true, skipRho: true, skipRhoP2: true, skipRhoP3: true, skipRhoZ: true, skipPP1: true, skipPM1: true, skipECM: true, skipQS: true, skipFact: true);
                            foreach (var child in factored.Factors)
                            {

                                var childPrimalityType = (MathLib.PrimalityType)(int)GmpInt.Primality(child.P);
                                if (childPrimalityType > 0)
                                {
                                    var query = "INSERT INTO Factors (DbFactorizationId, P, Power, Type, Digits, Bits) VALUES (@DbFactorizationId, @P, @Power, @Type, @Digits, @Bits)";
                                    var pParams = new
                                    {
                                        DbFactorizationId = factorizationId,
                                        P = child.P.ToString(),
                                        Power = child.Power,
                                        Type = (int)childPrimalityType,
                                        Digits = child.P.ToString().Length,
                                        Bits = MathLib.BitLength(child.P)
                                    };
                                    conn.Execute(query, pParams, transaction: trans);
                                }
                            }
                            trans.Commit();
                            return true;
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
                        return result;
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
                    return false;
                }
            }
        }
    }
}
