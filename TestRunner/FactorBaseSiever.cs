using Dapper;
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using MathGmp.Native;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TestRunner
{
    public class FactorBaseSieverBase
    {
        public Dictionary<int, HashSet<T>> GetFactorLookup<T>(int minBits, int maxBits, Func<string, T> parse)
        {
            var lookup = new Dictionary<int, HashSet<T>>(capacity: 100_000); // estimate if known
            var test = new FactorTest();
            test.SetConnectionString();


            var loadWatch = Stopwatch.StartNew();
            using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
            conn.Open();

            var sql = $@"
                    SELECT fz.Id AS FactorizationId, f.P
                    FROM Factorizations fz
                    left  JOIN Factors f ON fz.Id = f.DbFactorizationId and f.bits between {minBits} and {maxBits}
                    WHERE fz.Type < 1
                ";

            var rows = conn.Query<(int FactorizationId, string? P)>(sql);

            foreach (var row in rows)
            {
                if (!lookup.TryGetValue(row.FactorizationId, out var set))
                {
                    set = new HashSet<T>();
                    lookup[row.FactorizationId] = set;
                }
                if (row.P != null)
                {
                    T value = parse(row.P);

                    set.Add(value);
                }
            }

            Console.WriteLine($"[{DateTime.Now}] Loaded {lookup.Count:N0} factorization entries with {minBits}-{maxBits} bit factors with dapper in {loadWatch.Elapsed}");
            return lookup;
        }


        protected void AddMissingFactors<T>(ref int factored, ServiceProvider provider, T primeFactor, List<int> missingIds)
        {
            if (missingIds == null || missingIds.Count == 0)
            {
                return;
            }
            using var app = provider.CreateScope();
            using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            bool isDirty = false;
            foreach (var missingId in missingIds)
            {
                factored++;

                var factorizations = dbContext.Factorizations
                    .Include(x => x.Factors)
                    .Where(x => missingIds.Contains(x.Id))
                    .ToList();

                foreach (var factorization in factorizations)
                {
                    var result = RemovePrimeFactor(factorization, primeFactor);
                    if (result.hasError)
                    {
                        isDirty = false;
                        break;
                    }

                    isDirty = isDirty | result.isDirty;
                }


            }
            if (isDirty)
            {
                dbContext.SaveChanges();
            }
            dbContext.Dispose();
            app.Dispose();
        }

        BigInteger Mod<T>(BigInteger n, T a)
        {
            if (a is int intVal)
                return n % intVal;
            else if (a is uint uintVal)
                return n % uintVal;
            else if (a is long longVal)
                return n % longVal;
            else if (a is ulong ulongVal)
                return n % ulongVal;
            else if (a is BigInteger bigIntVal)
                return n % bigIntVal;
            else
                throw new NotSupportedException($"Type {typeof(T)} not supported");
        }

        BigInteger Div<T>(BigInteger n, T a)
        {
            if (a is int intVal)
                return n / intVal;
            else if (a is uint uintVal)
                return n / uintVal;
            else if (a is long longVal)
                return n / longVal;
            else if (a is ulong ulongVal)
                return n / ulongVal;
            else if (a is BigInteger bigIntVal)
                return n / bigIntVal;
            else
                throw new NotSupportedException($"Type {typeof(T)} not supported");
        }

        BigInteger Cast<T>(T a)
        {
            if (a is int intVal)
                return intVal;
            else if (a is uint uintVal)
                return uintVal;
            else if (a is long longVal)
                return longVal;
            else if (a is ulong ulongVal)
                return ulongVal;
            else if (a is BigInteger bigIntVal)
                return bigIntVal;
            else
                throw new NotSupportedException($"Type {typeof(T)} not supported");
        }


        private (bool isDirty, bool hasError) RemovePrimeFactor<T>(DbFactorization factorization, T prime)
        {
            bool isDirty = false;
            var compositeFactors = factorization.Factors.Where(x => (int)x.Type < 1).ToList();
            foreach (var composite in compositeFactors)
            {
                var n = BigInteger.Parse(composite.P);

                var fact = new Factor<BigInteger>(Cast(prime), 0);
                fact.FactorType = MathLib.PrimalityType.Prime;
                while (Mod(n, prime) == 0)
                {
                    fact.Power++;
                    n = Div(n, prime);
                }
                if (fact.Power > 0)
                {
                    isDirty = true;
                    factorization.Factors.Remove(composite);
                    factorization.Factors.Add(new DbFactor()
                    {
                        P = fact.P.ToString(),
                        Power = fact.Power,
                        Type = (PrimalityType)fact.FactorType,
                        Digits = fact.P.ToString().Length,
                        Bits = MathLib.BitLength(fact.P)
                    });

                    factorization.Factors.Add(new DbFactor()
                    {
                        P = n.ToString(),
                        Power = 1,
                        Type = (PrimalityType)(int)GmpInt.Primality(n),
                        Digits = n.ToString().Length,
                        Bits = MathLib.BitLength(n)
                    });

                    //break;
                }

            }

            var newValue = factorization.Factors.Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power)).Aggregate((a, b) => a * b);
            var newValueString = newValue.ToString();
            var nValueString = factorization.N.ToString();
            if (newValueString != nValueString)
            {
                // if length> 20, truncate format to {First 5 digits,...}...{Last 5 digits}
                var shortNew = newValueString.Length > 11 ? newValueString.Substring(0, 5) + "..." + newValueString.Substring(newValueString.Length - 5) : newValueString;
                var shortN = nValueString.Length > 11 ? nValueString.Substring(0, 5) + "..." + nValueString.Substring(nValueString.Length - 5) : nValueString;
                Console.WriteLine($"[{DateTime.Now}] - Factor verification failed removing {prime}: {shortNew}[{newValueString.Length}] != {shortN}[{newValueString.Length}]");
                return (false, true);
            }

            var newType = factorization.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
            if (newType != factorization.Type)
            {
                factorization.Type = newType;
                isDirty = true;
            }
            return (isDirty, false);
        }

        protected void AddMissingFactors1(ref int factored, ServiceProvider provider, BigInteger primeFactor, List<int> missingIds)
        {
            if (missingIds == null || missingIds.Count == 0)
            {
                return;
            }
            using var app = provider.CreateScope();
            using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            bool isDirty = false;
            foreach (var missingId in missingIds)
            {
                factored++;

                var factorizations = dbContext.Factorizations
                    .Include(x => x.Factors)
                    .Where(x => missingIds.Contains(x.Id))
                    .ToList();

                foreach (var factorization in factorizations)
                {
                    var result = RemovePrimeFactor(factorization, primeFactor);
                    if (result.hasError)
                    {
                        isDirty = false;
                        break;
                    }

                    isDirty = isDirty | result.isDirty;
                }


            }
            if (isDirty)
            {
                dbContext.SaveChanges();
            }
            dbContext.Dispose();
            app.Dispose();
        }

        private (bool isDirty, bool hasError) RemovePrimeFactor1(DbFactorization factorization, BigInteger prime)
        {
            bool isDirty = false;
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
                    isDirty = true;
                    factorization.Factors.Remove(composite);
                    factorization.Factors.Add(new DbFactor()
                    {
                        P = fact.P.ToString(),
                        Power = fact.Power,
                        Type = (PrimalityType)fact.FactorType,
                        Digits = fact.P.ToString().Length,
                        Bits = MathLib.BitLength(fact.P)
                    });

                    factorization.Factors.Add(new DbFactor()
                    {
                        P = n.ToString(),
                        Power = 1,
                        Type = (PrimalityType)(int)GmpInt.Primality(n),
                        Digits = n.ToString().Length,
                        Bits = MathLib.BitLength(n)
                    });

                    //break;
                }

            }

            var newValue = factorization.Factors.Select(x => BigInteger.Pow(BigInteger.Parse(x.P), x.Power)).Aggregate((a, b) => a * b);
            var newValueString = newValue.ToString();
            var nValueString = factorization.N.ToString();
            if (newValueString != nValueString)
            {
                // if length> 20, truncate format to {First 5 digits,...}...{Last 5 digits}
                var shortNew = newValueString.Length > 11 ? newValueString.Substring(0, 5) + "..." + newValueString.Substring(newValueString.Length - 5) : newValueString;
                var shortN = nValueString.Length > 11 ? nValueString.Substring(0, 5) + "..." + nValueString.Substring(nValueString.Length - 5) : nValueString;
                Console.WriteLine($"[{DateTime.Now}] - Factor verification failed removing {prime}: {shortNew}[{newValueString.Length}] != {shortN}[{newValueString.Length}]");
                return (false, true);
            }

            var newType = factorization.Factors.All(x => (int)x.Type > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
            if (newType != factorization.Type)
            {
                factorization.Type = newType;
                isDirty = true;
            }
            return (isDirty, false);
        }


        protected void Log(string message, bool appendDate = true)
        {
            if (appendDate)
            {
                message = $"[{DateTime.Now}] {message}";
            }

            Console.Title = message;
            Console.WriteLine(message);
        }

    }


    public class FactorBaseSieverNumerics : FactorBaseSieverBase
    {

        public Dictionary<int, HashSet<BigInteger>> GetFactorLookup(int bits)
        {
            return base.GetFactorLookup(bits, bits, BigInteger.Parse);
        }
        public Dictionary<int, HashSet<BigInteger>> GetFactorLookup1(int bits)
        {
            var lookup = new Dictionary<int, HashSet<BigInteger>>(capacity: 100_000); // estimate if known
            var test = new FactorTest();
            test.SetConnectionString();


            var loadWatch = Stopwatch.StartNew();
            using var conn = new SqlConnection(FactorDbContext.DbConnectionString);
            conn.Open();

            var sql = $@"
                    SELECT fz.Id AS FactorizationId, f.P
                    FROM Factorizations fz
                    left  JOIN Factors f ON fz.Id = f.DbFactorizationId and f.bits={bits}
                    WHERE fz.Type < 1
                ";

            var rows = conn.Query<(int FactorizationId, string? P)>(sql);

            foreach (var row in rows)
            {
                if (!lookup.TryGetValue(row.FactorizationId, out var set))
                {
                    set = new HashSet<BigInteger>();
                    lookup[row.FactorizationId] = set;
                }
                if (row.P != null)
                {
                    if (BigInteger.TryParse(row.P, out var value))
                    {
                        set.Add(value);
                    }
                    else
                    {
                        string bp = "";
                    }
                }
            }

            Console.WriteLine($"[{DateTime.Now}] Loaded {lookup.Count:N0} factorization entries with {bits}-bit factors with dapper in {loadWatch.Elapsed}");
            return lookup;
        }

        BigInteger NextPrime = 1L << 32;
        BigInteger MaxPrime = 1L << 64;
        IEnumerable<BigInteger> NaiveLongPrimeGenerator()
        {

            while (true)
            {
                using GmpInt current = (NextPrime + 1);
                using var result = MathUtil.GetNextPrime(current);
                NextPrime = (BigInteger)result;
                if (result >= MaxPrime)
                    break;

                yield return NextPrime;
            }

        }

        internal void SieveFactorBase(int bits = 32)
        {
            var n = RsaChallenge.Rsa1024BigInt;
            //var gen = new PrimeGeneratorUnsafeUint();
            var maxDbId = 10_000_000;
            int i = 0;


            var range = new BitRange(bits);

            // validate range start and end don't overflow as uint.max.Value

            // validate that the range is valid
            if (range.StartBit < 0 || range.StartBit > range.EndBit)
            {
                throw new ArgumentOutOfRangeException($"Range {range.StartBit} - {range.EndBit} is out of bounds for uint");
            }

            var minPrime = (long)range.StartValue;
            var maxPrime = MaxPrime = (long)range.EndValue;

            int factored = 0;
            var sw = Stopwatch.StartNew();

            var test = new FactorTest();
            test.SetConnectionString();

            var message = $"[{DateTime.Now}] Executing {nameof(SieveFactorBase)} - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            Dictionary<int, HashSet<BigInteger>> factorLookup = new();



            factorLookup = GetFactorLookup(bits);

            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();

            message = $"[{DateTime.Now}] Entering {nameof(SieveFactorBase)} loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            DateTime lastLog = DateTime.MinValue;


            var gen = NaiveLongPrimeGenerator();
            foreach (var primeFactor in gen)
            {
                if (primeFactor > maxPrime)
                    break;
                if (primeFactor < minPrime)
                    continue;
                i++;

                if (i % 10 == 0 && (DateTime.Now - lastLog).TotalSeconds > 1)
                {
                    //message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                    message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")} - {sw.Elapsed}";
                    Console.Title = message;
                    Console.WriteLine(message);
                    lastLog = DateTime.Now;
                }
                if (primeFactor <= minPrime)
                {
                    continue;
                }
                if (!MathLib.IsQuadraticResidue(n, primeFactor))
                {
                    //Console.WriteLine($"{DateTime.Now} ({i.ToString("N0")} - Skipping {primeFactor.ToString("N0")}");
                    continue;
                }
                if (primeFactor == 2)// || primeFactor < minPrime)
                {
                    //Console.WriteLine($"{DateTime.Now} ({i.ToString("N0")} - Skipping {primeFactor.ToString("N0")}");
                    continue;
                }
                var solutions = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, primeFactor);


                List<int>? missingIds = null;


                missingIds = new();
                foreach (var residue in new[] { solutions.Item1, solutions.Item2 })
                {
                    // protects against overflow
                    if (residue >= maxDbId) continue;

                    int offset = (int)residue;
                    if (offset == 0)
                    {
                        if (primeFactor > int.MaxValue)
                            continue;
                        else
                            offset = (int)primeFactor;
                    }
                    //Console.WriteLine($"[{DateTime.Now}] Sieving {primeFactor} from offset {offset}");
                    for (; offset <= maxDbId;)
                    {
                        if (factorLookup.TryGetValue(offset, out var ids))
                        {
                            if (!ids.Contains(primeFactor))
                                missingIds.Add(offset);

                        }
                        if ((long)offset + primeFactor <= (long)int.MaxValue)
                        {
                            offset += (int)primeFactor;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (missingIds == null || missingIds.Count == 0)
                {
                    continue;
                }


                AddMissingFactors(ref factored, provider, primeFactor, missingIds);

                if ((DateTime.Now - lastLog).TotalSeconds > 1)
                {
                    message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                    lastLog = DateTime.Now;
                    Console.Title = message;
                    Console.WriteLine(message);
                }

            }

            message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - {sw.Elapsed}";

            Console.Title = message;
            Console.WriteLine(message);
        }


    }


    public class FactorBaseSieverLong : FactorBaseSieverBase
    {

        public Dictionary<int, HashSet<long>> GetFactorLookup(int bits)
        {
            return base.GetFactorLookup(bits, bits, long.Parse);
        }

        long NextPrime = 1L << 32;
        long MaxPrime = long.MaxValue;

        /// <summary>
        /// returns the next prime number greater than or equal to z
        /// </summary>
        /// <param name="z"></param>
        public static void GetNextPrime(GmpInt z)
        {
            if (gmp_lib.mpz_even_p(z.Data) > 0)
            {
                gmp_lib.mpz_add_ui(z.Data, z.Data, 1);
            }

            //TODO: only test candidates that +/-1 mod 6
            var test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
            while (test == 0)
            {
                gmp_lib.mpz_add_ui(z.Data, z.Data, 2);
                test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
            }
        }
        IEnumerable<long> NaiveLongPrimeGenerator()
        {
            using GmpInt current = ((ulong)NextPrime + 1ul);
            using GmpInt max = ((ulong)MaxPrime + 1ul);
            while (true)
            {
                GetNextPrime(current);
                if (gmp_lib.mpz_sizeinbase(current.Data, 2) > 63 ||
                     gmp_lib.mpz_cmp(current.Data, max.Data) >= 0)
                    break;
                NextPrime = (long)current;
                yield return NextPrime;
                gmp_lib.mpz_add_ui(current.Data, current.Data, 2u);
            }

        }

        internal void SieveFactorBaseLongPrimes(int bits = 32)
        {
            var n = RsaChallenge.Rsa1024BigInt;

            var maxDbId = 10_000_000;
            int i = 0;
            int factored = 0;


            var range = new BitRange(bits);
            range.ValidateBounds(0, long.MaxValue);
            // validate range start and end don't overflow as uint.max.Value

            // validate that the range is valid
            if (range.StartBit < 0 || range.EndBit > 63 || range.StartBit > range.EndBit)
            {
                throw new ArgumentOutOfRangeException($"Range {range.StartBit} - {range.EndBit} is out of bounds for uint");
            }

            var minPrime = NextPrime = (long)range.StartValue;
            var maxPrime = MaxPrime = (long)range.EndValue;

            var sw = Stopwatch.StartNew();

            var test = new FactorTest();
            test.SetConnectionString();

            var message = $"[{DateTime.Now}] Executing {nameof(SieveFactorBaseLongPrimes)} Sieve loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            Dictionary<int, HashSet<long>> factorLookup = new();



            factorLookup = GetFactorLookup(bits);

            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();


            var queue = new BlockingCollection<(long prime, (BigInteger r1, BigInteger r2) roots)>(boundedCapacity: 10000);

            var producer = Task.Run(() =>
            {
                var root = n.Sqrt();
                var gen = NaiveLongPrimeGenerator();
                foreach (var prime in gen)
                {
                    if (prime > maxPrime)
                        break;
                    if (prime == 2 || !MathLib.IsQuadraticResidue(n, prime))
                        continue;
                    var roots = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, prime, root, false);
                    queue.Add((prime, (roots.Item1, roots.Item2)));
                }

                queue.CompleteAdding();
            });




            message = $"[{DateTime.Now}] Entering {nameof(SieveFactorBaseLongPrimes)} loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            DateTime lastLog = DateTime.MinValue;

            long nextPrime = minPrime;



            var consumer = Task.Run(() =>
            {
                foreach (var (primeFactor, roots) in queue.GetConsumingEnumerable())
                {
                    if (primeFactor >= maxPrime)
                        break;
                    if (primeFactor < minPrime)
                        continue;
                    i++;

                    if (i % 10 == 0 && (DateTime.Now - lastLog).TotalSeconds > 1)
                    {
                        //message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                        message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")} - {sw.Elapsed}";
                        Console.Title = message;
                        Console.WriteLine(message);
                        lastLog = DateTime.Now;
                    }


                    var solutions = roots;
                    List<int>? missingIds = new();

                    foreach (var residue in new[] { solutions.Item1, solutions.Item2 })
                    {

                        if (residue >= maxDbId || residue == 0) continue;
                        var offset = (int)residue;

                        // TODO: this is not mean to be used with small long primes but someone possibly could.
                        // for performance, commenting it out, otherwise it needs a ULONG + bounds check for every offset
                        /*
                        for (; offset <= maxDbId; offset += primeFactor)
                        {
                            if (factorLookup.TryGetValue(offset, out var ids))
                            {
                                if (!ids.Contains(primeFactor))
                                    missingIds.Add(offset);

                            }
                        }*/

                        if (factorLookup.TryGetValue(offset, out var ids))
                        {
                            if (!ids.Contains(primeFactor))
                                missingIds.Add(offset);

                        }
                    }
                    if (missingIds.Count == 0)
                        continue;


                    AddMissingFactors(ref factored, provider, primeFactor, missingIds);
                    missingIds.Clear();


                }

                message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - {sw.Elapsed}";

                Console.Title = message;
                Console.WriteLine(message);
            });


            Task.WaitAll(producer, consumer);
            Log($"Total time elapsed: {sw.Elapsed}");
        }


    }

    public class FactorBaseSieverLongQueue : FactorBaseSieverBase
    {

        public Dictionary<int, HashSet<long>> GetFactorLookup(int bits)
        {
            return base.GetFactorLookup(bits, bits, long.Parse);
        }


        /// <summary>
        /// returns the next prime number greater than or equal to z
        /// </summary>
        /// <param name="z"></param>
        public static void GetNextPrime(GmpInt z)
        {
            if (gmp_lib.mpz_even_p(z.Data) > 0)
            {
                gmp_lib.mpz_add_ui(z.Data, z.Data, 1);
            }

            //TODO: only test candidates that +/-1 mod 6
            var test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
            while (test == 0)
            {
                gmp_lib.mpz_add_ui(z.Data, z.Data, 2);
                test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
            }
        }
        IEnumerable<long> NaiveLongPrimeGenerator(ulong nextPrime, ulong maxPrime)
        {
            using GmpInt current = (nextPrime + 1ul);
            using GmpInt max = (maxPrime + 1ul);
            while (true)
            {
                GetNextPrime(current);
                if (gmp_lib.mpz_sizeinbase(current.Data, 2) > 63 ||
                     gmp_lib.mpz_cmp(current.Data, max.Data) >= 0)
                    break;
                yield return (long)current;
                gmp_lib.mpz_add_ui(current.Data, current.Data, 2u);
            }

        }

        internal void SieveFactorBaseLongPrimes(int bits = 32)
        {
            var n = RsaChallenge.Rsa1024BigInt;

            var maxDbId = 10_000_000;
         
            int factored = 0;


            var range = new BitRange(bits);
            range.ValidateBounds(0, long.MaxValue);
            // validate range start and end don't overflow as uint.max.Value

            // validate that the range is valid
            if (range.StartBit < 0 || range.EndBit > 63 || range.StartBit > range.EndBit)
            {
                throw new ArgumentOutOfRangeException($"Range {range.StartBit} - {range.EndBit} is out of bounds for uint");
            }

            var minPrime = (long)range.StartValue;
            var maxPrime = (long)range.EndValue;

            var sw = Stopwatch.StartNew();

            var test = new FactorTest();
            test.SetConnectionString();

            Log($"Executing {nameof(SieveFactorBaseLongPrimes)} Sieve loop - {sw.Elapsed}");

            Dictionary<int, HashSet<long>> factorLookup = new();


            factorLookup = GetFactorLookup(bits);





            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();

            var args = CommandLine.GetFactorArguments();
            var queue = new BlockingCollection<(long prime, (BigInteger r1, BigInteger r2) roots)>(boundedCapacity: 10000);

            long firstPrime = 0;
            long lastPrime = 0;

            ConcurrentDictionary<int, BigInteger> jobTracker = new();
            Dictionary<int, string> checkpointFiles = new();
            var producer = Task.Run(() =>
            {
                var root = n.Sqrt();

                var jobCount = args.TotalJobs;
                if (jobCount < 1)
                    jobCount = 1;

                var jobName = args.JobName;


                var threads = args.TotalThreads == 0 ? 1 : args.TotalThreads;
                var thread = args.ProcessorIndex ?? 1;

                // valid thread between 0 and total threads.


                if (threads < 1)
                    threads = 1;

                if (thread < 0 || thread > threads - 1)
                {
                    Log($"Error Job - Thread {thread} is out of range for {threads} threads");
                    return;
                }


                var checkPointFilename = $"{(args.JobName ?? "job")}-{thread}-of-{threads}_{range.StartBit}-{range.EndBit}";
                if (string.IsNullOrEmpty(jobName))
                    jobName = checkPointFilename;

                // for 2^P want split job across t threads.
                // for example, if P=32, want to split 2^32 across 4 threads
                // so each thread gets 2^30
                // for example, if p=42, want to split 2^42 across 4 threads
                // so each thread gets 2^40
                // calculate each threads range from the total range size

                var rangeSize = range.EndValue - range.StartValue;
                var threadRangeSize = rangeSize / threads;

                // split the thread range size across jobs
                // for example, if p=32, our thread range size = 2^30.
                // and for example, our job size is 24
                // so we want to split the range size across 24 jobs that will run in a parallel for
                var jobSize = threadRangeSize / jobCount;


                //split range.StartValue / range.EndValue across jobs
                //var jobSize = rangeSize / jobCount;

                // calculate the range for the thread
                var threadStart = range.StartValue + (thread * threadRangeSize);
                var threadEnd = threadStart + threadRangeSize;


                var checkPointWatch = new Stopwatch();
                int i = 0;
                Parallel.For(i, jobCount, (j) =>
                {
                    //var jobStartPrime = range.StartValue + (j * jobSize);
                    var jobStartPrime = threadStart + (j * jobSize);
                    var jobEndPrime = jobStartPrime + jobSize;
                    if (j == jobCount - 1)
                        //jobEndPrime = range.EndValue;
                        jobEndPrime = threadEnd;

                    if (j == 0)
                        firstPrime = (long)jobStartPrime;
                    if (j == jobCount - 1)
                        lastPrime = (long)jobStartPrime;

                    Log($"Job {j} - {jobStartPrime} - {jobEndPrime}");

                    var checkpointFile = $"{checkPointFilename}-thread-{j}-of-{jobCount}.json";
                    checkpointFiles.Add(j, checkpointFile);
                    ulong resumeFrom = (ulong)JobManager.GetJobStart(j, thread, jobStartPrime, jobEndPrime, checkpointFile);
                    if (resumeFrom > jobStartPrime)
                    {
                        Log($"Resuming Job {j} from {resumeFrom}");
                    }
                    jobTracker.TryAdd(j, jobStartPrime);
                    var gen = NaiveLongPrimeGenerator(resumeFrom, (ulong)jobEndPrime);

                    foreach (var prime in gen)
                    {
                        jobTracker[j] = prime;
                        //try
                        //{
                        if (j == 0)
                            firstPrime = prime;
                        if (j == jobCount - 1)
                            lastPrime = prime;

                        if (prime > jobEndPrime)
                            break;
                        if (prime == 2 || !MathLib.IsQuadraticResidue(n, prime))
                            continue;
                        var roots = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, prime, root, false);
                        queue.Add((prime, (roots.Item1, roots.Item2)));

                    }

                });
                /*
                //todo split minPrime / maxPrime across threads and run in parallel
                var gen = NaiveLongPrimeGenerator((ulong)minPrime, (ulong)maxPrime);
                foreach (var prime in gen)
                {
                    if (prime > maxPrime)
                        break;
                    if (prime == 2 || !MathLib.IsQuadraticResidue(n, prime))
                        continue;
                    var roots = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, prime, root, false);
                    queue.Add((prime, (roots.Item1, roots.Item2)));
                }
                */
                queue.CompleteAdding();
            });



            long primeCount = 0;
            Log($"Entering {nameof(SieveFactorBaseLongPrimes)} loop - {sw.Elapsed}");
            DateTime lastLog = DateTime.MinValue;

            long nextPrime = minPrime;

            int saveBatchSize = 1000;
            ConcurrentBag<(int FactorizationId, BigInteger Prime)> factors = new();
            var consumer = Task.Run(() =>
            {
                var saveTimeout = Stopwatch.StartNew();
                Action<bool> saveFactors = (completed) =>
                {
                    if (factors.Any())
                    {
                        FactoringQueue.QueueFactors(factors.ToList());
                        factors.Clear();
                    }

                    foreach(var pair in checkpointFiles)
                    {
                        if(jobTracker.TryGetValue(pair.Key, out var lastPrime))
                        {
                            JobManager.Update(pair.Value, lastPrime, completed);
                        }
                    }
                    saveTimeout.Restart();
                };


                foreach (var (primeFactor, roots) in queue.GetConsumingEnumerable())
                {
                    if (primeFactor >= maxPrime)
                        break;
                    if (primeFactor < minPrime)
                        continue;
                    primeCount++;

                    if (primeCount % 10 == 0 && (DateTime.Now - lastLog).TotalSeconds > 1)
                    {
                        //message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                        Log($"({primeCount.ToString("N0")}) {factored.ToString("N0")} Factors: {firstPrime.ToString("N0")} - {lastPrime.ToString("N0")} in {sw.Elapsed}");
                        lastLog = DateTime.Now;
                    }
                    var solutions = roots;
                    foreach (var residue in new[] { solutions.Item1, solutions.Item2 })
                    {

                        if (residue >= maxDbId || residue == 0) continue;
                        var offset = (int)residue;
                        if (factorLookup.TryGetValue(offset, out var ids))
                        {
                            if (!ids.Contains(primeFactor))
                            {
                                factored++;
                                factors.Add((offset, primeFactor));
                            }
                        }
                    }

                    if (factors.Count >= saveBatchSize || saveTimeout.Elapsed.TotalMinutes > 5)
                    {
                        saveFactors(false);
                    }
                }

                if (factors.Any())
                {
                    saveFactors(true);
                }

                Log($"({primeCount.ToString("N0")}) Factored {factored.ToString("N0")} - {sw.Elapsed}");

            });


            Task.WaitAll(producer, consumer);



            Log($"Total time elapsed: {sw.Elapsed}");
        }


    }

    public class FactorBaseSieverLongMod3 : FactorBaseSieverBase
    {

        public Dictionary<int, HashSet<long>> GetFactorLookup(int bits)
        {
            return base.GetFactorLookup(bits, bits, long.Parse);
        }

        long NextPrime = 1L << 32;
        long MaxPrime = long.MaxValue;

        /// <summary>
        /// returns the next prime number greater than or equal to z
        /// </summary>
        /// <param name="z"></param>
        public static void GetNextPrime(GmpInt z)
        {
            if (gmp_lib.mpz_even_p(z.Data) > 0)
            {
                gmp_lib.mpz_add_ui(z.Data, z.Data, 1);
            }

            //TODO: only test candidates that are 3 mod 4
            var test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
            while (test == 0)
            {
                gmp_lib.mpz_add_ui(z.Data, z.Data, 2);
                test = gmp_lib.mpz_probab_prime_p(z.Data, 20);
            }
        }


        IEnumerable<long> Mod3PrimeGenerator()
        {

            using GmpInt current = ((ulong)NextPrime + 1ul);
            using GmpInt max = ((ulong)MaxPrime + 1ul);
            using GmpInt tmp = 0;
            while (true)
            {
                GetNextPrime(current);
                if (gmp_lib.mpz_sizeinbase(current.Data, 2) > 63 ||
                     gmp_lib.mpz_cmp(current.Data, max.Data) >= 0)
                    break;
                NextPrime = (long)current;
                var res = (uint)NextPrime % 4;
                if (res == 3u)
                {
                    yield return NextPrime;
                }

                var nextRes = 4 - (3 - res);
                gmp_lib.mpz_add_ui(current.Data, current.Data, nextRes);


            }

        }

        internal void SieveFactorBaseLongPrimes(int bits = 32)
        {
            var n = RsaChallenge.Rsa1024BigInt;

            var maxDbId = 10_000_000;
            int i = 0;
            int factored = 0;


            var range = new BitRange(bits);
            range.ValidateBounds(0, long.MaxValue);
            // validate range start and end don't overflow as uint.max.Value

            // validate that the range is valid
            if (range.StartBit < 0 || range.EndBit > 63 || range.StartBit > range.EndBit)
            {
                throw new ArgumentOutOfRangeException($"Range {range.StartBit} - {range.EndBit} is out of bounds for uint");
            }

            var minPrime = NextPrime = (long)range.StartValue;
            var maxPrime = MaxPrime = (long)range.EndValue;

            var sw = Stopwatch.StartNew();

            var test = new FactorTest();
            test.SetConnectionString();

            var message = $"[{DateTime.Now}] Executing {nameof(SieveFactorBaseLongPrimes)} Sieve loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            Dictionary<int, HashSet<long>> factorLookup = new();



            factorLookup = GetFactorLookup(bits);

            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();


            var queue = new BlockingCollection<(long prime, (BigInteger r1, BigInteger r2) roots)>(boundedCapacity: 10000);

            var producer = Task.Run(() =>
            {
                var root = n.Sqrt();
                var gen = Mod3PrimeGenerator();
                BigInteger k, Q;
                foreach (var prime in gen)
                {
                    if (prime > maxPrime)
                        break;
                    if (prime == 2 || !MathLib.IsQuadraticResidue(n, prime))
                        continue;
                    BigInteger pm1 = prime - 1;
                    if (BigInteger.ModPow(n, pm1 / 2, prime) != 1)
                    {
                        //return 0;
                        continue;
                    }
                    k = (prime / 4);
                    Q = BigInteger.ModPow(n, k + 1, prime) % prime;

                    var roots = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, prime, root, Q, false);
                    //var roots= MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, prime, root, false);
                    queue.Add((prime, (roots.Item1, roots.Item2)));
                }

                queue.CompleteAdding();
            });




            message = $"[{DateTime.Now}] Entering {nameof(SieveFactorBaseLongPrimes)} loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            DateTime lastLog = DateTime.MinValue;

            long nextPrime = minPrime;



            var consumer = Task.Run(() =>
            {
                foreach (var (primeFactor, roots) in queue.GetConsumingEnumerable())
                {
                    if (primeFactor >= maxPrime)
                        break;
                    if (primeFactor < minPrime)
                        continue;
                    i++;

                    if (i % 10 == 0 && (DateTime.Now - lastLog).TotalSeconds > 1)
                    {
                        //message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                        message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")} - {sw.Elapsed}";
                        Console.Title = message;
                        Console.WriteLine(message);
                        lastLog = DateTime.Now;
                    }


                    var solutions = roots;
                    List<int>? missingIds = new();

                    foreach (var residue in new[] { solutions.Item1, solutions.Item2 })
                    {

                        if (residue >= maxDbId || residue == 0) continue;
                        var offset = (int)residue;

                        // TODO: this is not mean to be used with small long primes but someone possibly could.
                        // for performance, commenting it out, otherwise it needs a ULONG + bounds check for every offset
                        /*
                        for (; offset <= maxDbId; offset += primeFactor)
                        {
                            if (factorLookup.TryGetValue(offset, out var ids))
                            {
                                if (!ids.Contains(primeFactor))
                                    missingIds.Add(offset);

                            }
                        }*/

                        if (factorLookup.TryGetValue(offset, out var ids))
                        {
                            if (!ids.Contains(primeFactor))
                                missingIds.Add(offset);

                        }
                    }
                    if (missingIds.Count == 0)
                        continue;


                    AddMissingFactors(ref factored, provider, primeFactor, missingIds);
                    missingIds.Clear();


                }

                message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - {sw.Elapsed}";

                Console.Title = message;
                Console.WriteLine(message);
            });


            Task.WaitAll(producer, consumer);
            Log($"Total time elapsed: {sw.Elapsed}");
        }


    }


    public class FactorBaseSieverUint : FactorBaseSieverBase
    {

        public Dictionary<int, HashSet<uint>> GetFactorLookup(int bits)
        {
            return base.GetFactorLookup(bits, bits, uint.Parse);
        }

        internal void SieveFactorBaseUintPrimes(int bits = 32)
        {
            var n = RsaChallenge.Rsa1024BigInt;

            var range = new BitRange(bits);
            range.ValidateBounds(uint.MinValue, uint.MaxValue);
            // validate range start and end don't overflow as uint.max.Value

            // validate that the range is valid
            if (range.StartBit < 0 || range.EndBit > 32 || range.StartBit > range.EndBit)
            {
                throw new ArgumentOutOfRangeException($"Range {range.StartBit} - {range.EndBit} is out of bounds for uint");
            }

            uint minPrime = (uint)range.StartValue;
            var maxPrime = (uint)range.EndValue;

            var gen = new PrimeGeneratorUnsafeUint(minPrime, maxPrime);
            var maxDbId = 10_000_000;
            int i = 0;
            // continue from int.MaxValue, use FactorBaseSiever for int primes;

            // continue from int.MaxValue, use FactorBaseSiever for int primes;
            int factored = 0;
            var sw = Stopwatch.StartNew();

            var test = new FactorTest();
            test.SetConnectionString();

            var message = $"[{DateTime.Now}] Executing {nameof(SieveFactorBaseUintPrimes)} Sieve loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            Dictionary<int, HashSet<uint>> factorLookup = new();



            factorLookup = GetFactorLookup(bits);


            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();

            message = $"[{DateTime.Now}] Entering {nameof(SieveFactorBaseUintPrimes)} loop - {sw.Elapsed}";
            Console.Title = message;
            Console.WriteLine(message);
            DateTime lastLog = DateTime.MinValue;

            foreach (var primeFactor in gen)
            {
                i++;

                if (i % 10 == 0 && (DateTime.Now - lastLog).TotalSeconds > 1)
                {
                    //message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                    message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")} - {sw.Elapsed}";
                    Console.Title = message;
                    Console.WriteLine(message);
                    lastLog = DateTime.Now;
                }
                if (primeFactor <= minPrime)
                {
                    continue;
                }
                if (!MathLib.IsQuadraticResidue(n, primeFactor))
                {
                    //Console.WriteLine($"{DateTime.Now} ({i.ToString("N0")} - Skipping {primeFactor.ToString("N0")}");
                    continue;
                }
                if (primeFactor == 2)// || primeFactor < minPrime)
                {
                    //Console.WriteLine($"{DateTime.Now} ({i.ToString("N0")} - Skipping {primeFactor.ToString("N0")}");
                    continue;
                }
                var solutions = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, primeFactor);


                List<int>? missingIds = null;

                missingIds = new();
                foreach (var residue in new[] { (uint)solutions.Item1, (uint)solutions.Item2 })
                {

                    if (residue >= maxDbId) continue;
                    int offset = (int)residue;
                    if (offset == 0)
                    {
                        if (primeFactor > int.MaxValue)
                            continue;
                        else
                            offset = (int)primeFactor;
                    }
                    //Console.WriteLine($"[{DateTime.Now}] Sieving {primeFactor} from offset {offset}");
                    for (; offset <= maxDbId;)
                    {
                        if (factorLookup.TryGetValue(offset, out var ids))
                        {
                            if (!ids.Contains(primeFactor))
                                missingIds.Add(offset);

                        }
                        if ((ulong)offset + primeFactor <= (uint)int.MaxValue)
                        {
                            offset += (int)primeFactor;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (missingIds == null || missingIds.Count == 0)
                {
                    continue;
                }
                AddMissingFactors(ref factored, provider, primeFactor, missingIds);
                if ((DateTime.Now - lastLog).TotalSeconds > 1)
                {
                    message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}";
                    lastLog = DateTime.Now;
                    Console.Title = message;
                    Console.WriteLine(message);
                }



            }


            message = $"{DateTime.Now} ({i.ToString("N0")}) Factored {factored.ToString("N0")} - {sw.Elapsed}";

            Console.Title = message;
            Console.WriteLine(message);
        }



    }


    public class FactorBaseSiever : FactorBaseSieverBase
    {

        public Dictionary<int, HashSet<int>> GetFactorLookup(int startBit, int endBit)
        {
            return base.GetFactorLookup(startBit, endBit, int.Parse);
        }

        internal void SieveFactorBaseIntPrimes(int startBit = 0, int endBit = 31)
        {


            Log($"Executing {nameof(SieveFactorBaseIntPrimes)}({startBit}, {endBit})");

            var n = RsaChallenge.Rsa1024BigInt;

            var maxDbId = 10_000_000;
            int i = 0;
            int factored = 0;

            var sw = Stopwatch.StartNew();

            var range = new BitRange(startBit, endBit);
            range.ValidateBounds(0, int.MaxValue);
            // validate range start and end don't overflow as uint.max.Value

            // validate that the range is valid
            if (range.StartBit < 0 || range.EndBit > 31 || range.StartBit > range.EndBit)
            {
                throw new ArgumentOutOfRangeException($"Range {range.StartBit} - {range.EndBit} is out of bounds for uint");
            }

            var minPrime = (int)range.StartValue;
            var maxPrime = (int)range.EndValue;

            var services = new ServiceCollection();
            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));
            var provider = services.BuildServiceProvider();


            var queue = new BlockingCollection<(int prime, (int r1, int r2) roots)>(boundedCapacity: 10000);

            var producer = Task.Run(() =>
            {
                var root = n.Sqrt();
                var gen = new PrimeGeneratorUnsafe(minPrime, maxPrime);
                foreach (var prime in gen)
                {
                    if (prime > maxPrime)
                        break;
                    if (prime == 2 || prime < minPrime || !MathLib.IsQuadraticResidue(n, prime))
                        continue;

                    var roots = MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, prime, root, false);
                    queue.Add((prime, ((int)roots.Item1, (int)roots.Item2)));
                }

                queue.CompleteAdding();
            });

            var consumer = Task.Run(() =>
            {

                Log($"Executing {nameof(SieveFactorBaseIntPrimes)} Sieve loop {nameof(GetFactorLookup)} - {sw.Elapsed}");
                Dictionary<int, HashSet<int>> factorLookup = GetFactorLookup(range.StartBit, range.EndBit);

                Log($"Entering {nameof(SieveFactorBaseIntPrimes)} loop - {sw.Elapsed}");

                DateTime lastLog = DateTime.MinValue;
                foreach (var (primeFactor, roots) in queue.GetConsumingEnumerable())
                {
                    i++;
                    if (i % 10 == 0 && (DateTime.Now - lastLog).TotalSeconds > 1)
                    {

                        Log($"({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")} - {sw.Elapsed}");
                        lastLog = DateTime.Now;
                    }
                    var solutions = roots;
                    List<int>? missingIds = new();

                    foreach (var residue in new[] { (int)solutions.Item1, (int)solutions.Item2 })
                    {

                        if (residue >= maxDbId) continue;
                        var offset = residue == 0 ? primeFactor : residue;
                        for (; offset <= maxDbId; offset += primeFactor)
                        {
                            if (factorLookup.TryGetValue(offset, out var ids))
                            {
                                if (!ids.Contains(primeFactor))
                                    missingIds.Add(offset);

                            }
                        }
                    }
                    if (missingIds.Count == 0)
                        continue;

                    base.AddMissingFactors(ref factored, provider, primeFactor, missingIds);

                    if ((DateTime.Now - lastLog).TotalSeconds > 1)
                    {
                        Log($"({i.ToString("N0")}) Factored {factored.ToString("N0")} - Prime {primeFactor.ToString("N0")}  {sw.Elapsed}");
                        lastLog = DateTime.Now;

                    }
                }

                Log($"({i.ToString("N0")}) Factored {factored.ToString("N0")} - {sw.Elapsed}");
            });


            Task.WaitAll(producer, consumer);
            Log($"Total time elapsed: {sw.Elapsed}");
        }


    }


    public class DbFactorBaseSieve
    {
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
        public List<BigInteger> GetDbPrimesLargerThan(int bits)
        {
            var test = new FactorTest();

            test.SetConnectionString();

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var query = $"SELECT DISTINCT p FROM factors f where bits>{bits} and type>0 and DbFactorizationId is not null ";
                var primeList = conn.Query<string>(query);
                var primes = primeList.Select(x => BigInteger.Parse(x)).ToList();
                return primes;
            }
        }


        public void SaveOffsets()
        {
            var test = new FactorTest();
            test.SetConnectionString();



            var n = RsaChallenge.Rsa1024BigInt;

            var connectionString = FactorDbContext.DbConnectionString.Replace("300", "600");
            //"Server=localhost;Database=Factors;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Command Timeout=300";
            using (var conn = new SqlConnection(connectionString))
            {
                var dropExistingQuery = @"
                    IF OBJECT_ID('LargeResidueOffsets', 'U') IS NOT NULL
						Drop Table LargeResidueOffsets;
                ";

                var ensureCreatedQuery = @"
                    IF OBJECT_ID('LargeResidueOffsets', 'U') IS NULL
						CREATE TABLE LargeResidueOffsets (
							Id INT IDENTITY(1,1) PRIMARY KEY,
							PDigits int,
							PBits int,
							Offset1Digits int,
							Offset1Bits int,
							Offset2Digits int,
							Offset2Bits int,						
							P NVARCHAR(400),
							Offset1 NVARCHAR(400),
							Offset2 NVARCHAR(400)
						);
                ";
                conn.Execute(dropExistingQuery);
                conn.Execute(ensureCreatedQuery);

                List<(BigInteger Prime, BigInteger Offset)> primes = null!;
                var sw = Stopwatch.StartNew();
                // execute in a block to free the list after conversion.
                {

                    //var query = $"SELECT DISTINCT p FROM factors f where bits>{bits} and type>0 and DbFactorizationId is not null ";
                    var query = $"SELECT p, min(z.Id) as offset FROM Factorizations z join Factors p on z.id=p.DbFactorizationId where p.Type>0 group by p";
                    var pList = conn.Query<(string p, int offset)>(query).ToList();

                    sw.Stop();

                    Console.WriteLine($"Queried {pList.Count} distinct factor primes in {sw.Elapsed}");

                    sw.Restart();
                    primes = pList.Select(x => (BigInteger.Parse(x.p), (BigInteger)x.offset)).ToList();
                    sw.Stop();
                    pList.Clear();
                    Console.WriteLine($"Parsed {primes.Count} distinct factor primes in {sw.Elapsed}");

                    sw.Restart();
                    primes = primes.OrderBy(x => x.Prime).ToList();
                    sw.Stop();
                    Console.WriteLine($"Sorted {primes.Count} distinct factor primes in {sw.Elapsed}");


                }

                sw = sw ?? Stopwatch.StartNew();
                sw.Restart();
                var offsets = HigginsSoft.Math.Lib.MathLib.TonelliShanksPy.GetFactorBaseOffsets(n, primes);
                sw.Stop();
                Console.WriteLine($"Calculated {primes.Count} root solutions in {sw.Elapsed}");

                // Prepare DataTable
                var table = new DataTable();
                table.Columns.Add("PDigits", typeof(int));
                table.Columns.Add("PBits", typeof(int));
                table.Columns.Add("Offset1Digits", typeof(int));
                table.Columns.Add("Offset1Bits", typeof(int));
                table.Columns.Add("Offset2Digits", typeof(int));
                table.Columns.Add("Offset2Bits", typeof(int));
                table.Columns.Add("P", typeof(string));
                table.Columns.Add("Offset1", typeof(string));
                table.Columns.Add("Offset2", typeof(string));

                sw.Restart();
                foreach (var kv in offsets)
                {
                    var P = kv.Key.ToString();
                    var Offset1 = kv.Value.Offset1.ToString();
                    var Offset2 = kv.Value.Offset2.ToString();

                    var PDigits = P.Length;
                    var PBits = kv.Key.GetBitLength();

                    var Offset1Digits = Offset1.Length;
                    var Offset1Bits = BigInteger.Parse(Offset1).GetBitLength();

                    var Offset2Digits = Offset2.Length;
                    var Offset2Bits = BigInteger.Parse(Offset2).GetBitLength();

                    var row = table.NewRow();
                    row["PDigits"] = PDigits;
                    row["PBits"] = PBits;
                    row["Offset1Digits"] = Offset1Digits;
                    row["Offset1Bits"] = Offset1Bits;
                    row["Offset2Digits"] = Offset2Digits;
                    row["Offset2Bits"] = Offset2Bits;
                    row["P"] = P;
                    row["Offset1"] = Offset1;
                    row["Offset2"] = Offset2;
                    table.Rows.Add(row); ;
                }
                sw.Stop();
                Console.WriteLine($"Create data table with {primes.Count} rows in {sw.Elapsed}");

                // Bulk insert
                // todo log the time taken for each batch
                using var bulk = new SqlBulkCopy(conn)
                {
                    DestinationTableName = "LargeResidueOffsets",
                    BatchSize = 100_000,
                    BulkCopyTimeout = 600

                };

                bulk.ColumnMappings.Add("PDigits", "PDigits");
                bulk.ColumnMappings.Add("PBits", "PBits");
                bulk.ColumnMappings.Add("Offset1Digits", "Offset1Digits");
                bulk.ColumnMappings.Add("Offset1Bits", "Offset1Bits");
                bulk.ColumnMappings.Add("Offset2Digits", "Offset2Digits");
                bulk.ColumnMappings.Add("Offset2Bits", "Offset2Bits");
                bulk.ColumnMappings.Add("P", "P");
                bulk.ColumnMappings.Add("Offset1", "Offset1");
                bulk.ColumnMappings.Add("Offset2", "Offset2");



                sw.Restart();
                conn.Open();
                // todo log the time taken for each batch

                var copyWatch = Stopwatch.StartNew();
                bulk.SqlRowsCopied += (sender, e) =>
                {
                    copyWatch.Stop();
                    Console.WriteLine($"Copied {e.RowsCopied} rows in {copyWatch.Elapsed}");
                    copyWatch.Restart();
                };

                bulk.WriteToServer(table);
                sw.Stop();
                Console.WriteLine($"Inserted {table.Rows.Count:N0} rows into LargeResidueOffsets in {sw.Elapsed}");

            }

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
            return GetMissingIds(prime, [class1, class2]);

        }

        public List<int> GetIntDbPrimes()
        {
            var test = new FactorTest();
            test.SetConnectionString();

            using (var conn = new SqlConnection(FactorDbContext.DbConnectionString))
            {
                var query1 = "SELECT DISTINCT cast(p as int) FROM factors f where bits<32 and DbFactorizationId is not null ";
                var query = "select pint from LargeResidueOffsets where PBits<32";
                var intPrimes = conn.Query<int>(query).ToList();
                return intPrimes;
            }
        }
        private List<int> GetMissingIds(int prime, int offset1, int offset2)
        {
            return GetMissingIds(prime, [offset1, offset2]);
        }
        private List<int> GetMissingIds(int prime, IEnumerable<int> offsets)
        {

            //var allClassIds = class1Ids.Union(class2Ids).ToList();
            var allClassIdsString = string.Join(",", offsets.Distinct());

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
        internal void SieveDbPrimes()
        {
            var primes = GetIntDbPrimes();
            primes.Sort();

            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer(FactorDbContext.DbConnectionString));

            var provider = services.BuildServiceProvider();

            using var app = provider.CreateScope();
            using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();
            int startPrime = 6990679;
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

    }
}
