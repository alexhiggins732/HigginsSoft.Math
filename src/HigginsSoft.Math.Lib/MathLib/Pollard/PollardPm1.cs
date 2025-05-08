using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    public static class PollardPm1
    {
        public static BigInteger Run(BigInteger n, int B1 = 100_000, int B2 = 1_000_000, int baseA = 2)
        {
            if (n.IsEven)
                return 2;

            //var a = new BigInteger(baseA);

            var gen = new PrimeGeneratorUnsafe(B2);

            // ======== Stage 1 ========
            //var primes = GeneratePrimes(B1);
            var allPrimes = gen.ToList();
            var primes = allPrimes.Where(x => x <= B1);// GeneratePrimes(B1);
            BigInteger M = 1;
            foreach (var p in primes)
            {
                int e = (int)(BigInteger.Log(B1) / BigInteger.Log(p));
                M *= BigInteger.Pow(p, e);
            }

            var x = BigInteger.ModPow(baseA, M, n);
            var g = BigInteger.GreatestCommonDivisor(x - 1, n);
            if (g > 1 && g < n)
                return g;

            // a = x; //save stage 1
            // ======== Stage 2 ========
            //primes = GeneratePrimes(B2);
            primes = primes.Where(x => x > B1).ToList();
            foreach (var q in primes.Where(p => p > B1))
            {
                var y = BigInteger.ModPow(x, q, n); // x from Stage 1
                var gcd = BigInteger.GreatestCommonDivisor(y - 1, n);
                if (gcd > 1 && gcd < n)
                    return g;
            }

            return 1; // No factor found
        }


        public static BigInteger RunStage1(BigInteger n, int B1, int baseA = 2, string? checkpointFile = null)
        {
            var a = new BigInteger(baseA);
            //var primes = GeneratePrimes(B1);
            var primes = new PrimeGeneratorUnsafe(B1);
            foreach (var p in primes)
            {
                int e = (int)(BigInteger.Log(B1) / BigInteger.Log(p));
                if (e == 0) continue;

                var pe = BigInteger.Pow(p, e);
                a = BigInteger.ModPow(a, pe, n); // incremental mod exponentiation
            }
            if (checkpointFile != null)
                SaveStage1Checkpoint(checkpointFile, new Stage1Checkpoint(n, a, B1, 2));

            //var gcd = BigInteger.GreatestCommonDivisor(a - 1, n);
            return a;
        }

        public static BigInteger RunStage2(BigInteger n, string checkpointFile, int B2)
        {
            var ckpt = LoadStage1Checkpoint(checkpointFile);
            var factor = RunStage2(ckpt.N, ckpt.Residue, ckpt.B1, B2);
            Console.WriteLine($"Factor found: {factor}");
            return factor;
        }
        //TODO: allow incrementing to a large b1.
        public static BigInteger RunStage2(BigInteger n, string checkpointFile, int newB1, int B2)
        {
            /*
             Process:
            1. Load checkpoint
            2. Check if B1 is less than newB1
            3. If so, run stage 1 adding primes in range from ckpt.B1 to newB1
                Additionally, check each prime<= ckpt.B1 for increased exponent.
                So if, we go from B1=1000 to B1=4000, we need to check all primes
                    e for 2^e for ckpt.B1 would be 10, s^10=1024
                    but for newB1 we need to check 2^11=2048, and if larger, 2^12=4096 etc
                    se we beed to raise  ckpt.Residue to power 2^11-2^10=2^1 to account for increased b1
                
                in essence, for each prime p in be need to add p^e(b2)-p^e(b1)  until p^e > newB1 for all primes

             */

            var ckpt = LoadStage1Checkpoint(checkpointFile);
            var residue = ckpt.Residue;
            if (newB1 > ckpt.B1)
            {
                var B1 = ckpt.B1;
                var genold = new PrimeGeneratorUnsafe(B1);
                // is it worth consolidating this into a single generator?
                foreach (var prime in genold)
                {
                    int olde = (int)(BigInteger.Log(B1) / BigInteger.Log(prime));
                    var e = (int)(BigInteger.Log(newB1) / BigInteger.Log(prime));
                    var needed = e - olde;
                    if (needed > 0)
                    {
                        var pe = BigInteger.Pow(prime, needed);
                        residue = BigInteger.ModPow(residue, pe, n);
                        //Console.WriteLine($"Adding {prime}^{needed} to residue");
                    }
                }
                var gen = new PrimeGeneratorUnsafe(B1, newB1);
                foreach (var prime in gen)
                {
                    var e = (int)(BigInteger.Log(newB1) / BigInteger.Log(prime));
                    var pe = BigInteger.Pow(prime, e);
                    residue = BigInteger.ModPow(residue, pe, n);
                    //Console.WriteLine($"Adding {prime}^{needed} to residue");

                }
                //update the checkpoint;
                SaveStage1Checkpoint(checkpointFile, new Stage1Checkpoint(n, residue, newB1, 2));
                var gcd = BigInteger.GreatestCommonDivisor(residue - 1, n);
                if (gcd > 1 && gcd < n)
                    return gcd;


            }


            var factor = RunStage2(ckpt.N, residue, ckpt.B1, B2);
            Console.WriteLine($"Factor found: {factor}");
            return factor;
        }
        public static BigInteger RunStage2(BigInteger n, BigInteger x, int B1, int B2)
        {
            var primes = new PrimeGeneratorUnsafe(B1, B2);

            //foreach (var q in primes.Where(p => p > B1))
            foreach (var q in primes)
            {
                var y = BigInteger.ModPow(x, q, n);
                var g = BigInteger.GreatestCommonDivisor(y - 1, n);
                if (g > 1 && g < n)
                    return g;
            }
            return 1;
        }

        public record Stage1Checkpoint(BigInteger N, BigInteger Residue, int B1, int Base);

        public static void SaveStage1Checkpoint(string path, Stage1Checkpoint ckpt)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(ckpt, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static Stage1Checkpoint LoadStage1Checkpoint(string path)
        {
            return JsonSerializer.Deserialize<Stage1Checkpoint>(File.ReadAllText(path))!;
        }

        //public static BigInteger RunStage1(BigInteger n, int B1, int baseA = 2, int threads = 1)
        //{
        //    var primes = new PrimeGeneratorUnsafe(B1).ToList();

        //    // Use partitioning to parallelize base exponentiations
        //    var partitions = Partitioner.Create(primes, EnumerablePartitionerOptions.NoBuffering);
        //    var aParts = new ConcurrentBag<BigInteger>();

        //    Parallel.ForEach(partitions, new ParallelOptions { MaxDegreeOfParallelism = threads }, partition =>
        //    {
        //        BigInteger partial = baseA;

        //        foreach (var p in partition)
        //        {
        //            int e = (int)(BigInteger.Log(B1) / BigInteger.Log(p));
        //            if (e == 0) continue;

        //            var pe = BigInteger.Pow(p, e);
        //            partial = BigInteger.ModPow(partial, pe, n);
        //        }

        //        aParts.Add(partial);
        //    });

        //    // Combine partial results: multiply exponents (mod n)
        //    var result = aParts.Aggregate((x, y) => BigInteger.ModPow(x, y, n));

        //    return result;
        //}
    }

}