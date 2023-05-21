using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Demos
{
    public class PrimeResidueClass
    {
        public int Prime;
        public List<int> Residues = new();
        public PrimeResidueClass? PreviousResidue = null;

        public PrimeResidueClass(int prime, IEnumerable<int> residues)
        {
            Prime = prime;
            Residues = residues.ToList();
        }

        public PrimeResidueClass(int prime, PrimeResidueClass previousClass)
        {
            Prime = prime;
            PreviousResidue = previousClass;
        }

        private int? factorial;
        public int Factorial => factorial.HasValue ? factorial.Value :
                (factorial = (PreviousResidue is null ? Prime : Prime * PreviousResidue.Factorial)).Value;

        public List<int> ClassPrimes => PreviousResidue is null ?
                (new[] { Prime }).ToList() :
                PreviousResidue.ClassPrimes.Concat(new[] { Prime }).ToList();


    }
    public partial class PrimeResidueClasses
    {

        // 1 classes
        private static PrimeResidueClass Mod_2 = new(2, new[] { 1 });

        // 2 classes
        private static PrimeResidueClass Mod_6 => CalculateResidues(Mod_2, 3);

        // 8 classes
        private static PrimeResidueClass Mod_30 => CalculateResidues(Mod_6, 5);

        // 48 classes
        private static PrimeResidueClass Mod_210 => CalculateResidues(Mod_30, 7);

        // 480 classes
        private static PrimeResidueClass Mod_2_310 => CalculateResidues(Mod_210, 11);

        // 5,760 classes
        private static PrimeResidueClass Mod_30_030 => CalculateResidues(Mod_2_310, 13);

        // 92,160 classes
        private static PrimeResidueClass Mod_510_510 => CalculateResidues(Mod_30_030, 17);

        // 1,658,880 classes
        private static PrimeResidueClass Mod_59_699_690 => CalculateResidues(Mod_510_510, 19);

        // 36,495,360 classes
        private static PrimeResidueClass Mod_223_092_870 => CalculateResidues(Mod_59_699_690, 23);


        private static PrimeResidueClass CalculateResidues(PrimeResidueClass previousClass, int prime)
        {
            var residueClass = new PrimeResidueClass(prime, previousClass);
            var limit = residueClass.Factorial;
            var factors = residueClass.ClassPrimes;
            var residues = new List<int>();
            for (var i = 0; i < limit; i++)
            {
                if (!factors.Any(p => i % p == 0))
                {
                    residues.Add(i);
                }
            }
            residueClass.Residues = residues;
            return residueClass;

        }


        static readonly int[] factorialPrimes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 };
        public static void ShowProductFactorial()
        {
            var result = 1;
            for (var i = 0; i < factorialPrimes.Length; i++) { result *= factorialPrimes[i]; }
            Console.WriteLine($"Product factor: {result.ToString("N0")}");

        }

        public static void ProductFactorialFactorV1()
        {
            var primes = factorialPrimes.ToList();
            for (var n = 2; n < 256; n++)
            {
                var isPrime = FactorialIsPrime(n, 2, primes);
                Console.WriteLine($"{n} {isPrime}");
            }
        }

        public static bool IsPrime(int i)
        {
            var result = i > 1
                && ((i & 1) == 1 || i == 2)
                && ((i % 3) > 0 || i == 3)
                && ((i % 5) > 0 || i == 5);


            if (result && i > 25)
            {
                var root = System.Math.Sqrt(i);
                var k = 5;
                var j = 7;
                for (; (result = i % k > 0) && (result = i % j > 0) && (k <= root || j <= root); k += 6, j += 6) ;

            }

            return result;
        }
        //public static int InlinePrimeCheckCount()
        //{
        //    var count = 0;
        //    var limit = 20000 * 20000;
        //    for (var i = 0; i < limit; i++)
        //    {
        //        bool result = InlinePrimeCheck.IsPrime(i);
        //        if (result)
        //            count++;
        //    }
        //    return count;
        //}

        static void Print2P31RootPrimes()
        {
            int limit = (int)System.Math.Sqrt(int.MaxValue);
            var sb = new System.Text.StringBuilder();
            for (var i = 0; i <= limit; i++)
            {
                if (IsPrime(i))
                {
                    sb.AppendLine(i.ToString());
                }
            }
            var s = sb.ToString();
            Console.WriteLine(s);
        }

        public static bool IsPrimeInline(int i)
        {
            //var result = i > 1
            //    && (((i & 1) == 1 || i == 2));
            //if (!result) return result;

            //result = i % 3 > 0;
            //if (!result || i <= 3 * 3) return result;

            //result = i % 5 > 0;
            //if (!result || i <= 5 * 5) return result;

            //result = i % 7 > 0;
            //if (!result || i <= 7 * 7) return result;

            //result = i % 11 > 0;
            //if (!result || i <= 11 * 11) return result;

            // || terminates at first true statement.
            // && terminates at first false statement
            var result = i < 2 || ((i & 1) == 0 && i != 2) || i > 2 * 2
                && i % 3 == 0 || i > 3 * 3
                && i % 5 == 0 || i > 5 * 5
                && i % 7 == 0 || i > 7 * 7
                && i % 11 == 0 || i > 11 * 11
                && i % 13 == 0 || i > 11 * 13
                && i % 17 == 0 || i > 17 * 17
                && i % 19 == 0 || i > 19 * 19
                && i % 23 == 0 || i > 23 * 23
                && i % 29 == 0 || i > 29 * 29
                && i % 31 == 0 || i > 31 * 31
                && i % 37 == 0 || i > 37 * 37
                && i % 41 == 0 || i > 41 * 41
                && i % 43 == 0 || i > 43 * 43
                && i % 47 == 0 || i > 47 * 47
                && i % 51 == 0 || i > 51 * 51
                && i % 53 == 0 || i > 53 * 53
                && i % 57 == 0 || i > 57 * 57
                && i % 59 == 0 || i > 59 * 59
                && i % 61 == 0 || i > 61 * 61;


            return !result;
        }



        public static void ProductFactorialFactor()
        {
            for (var n = 2; n < 256; n++)
            {
                var isPrime = IsProductFactorialPrime(n);
                var check = IsPrime(n);
                string result = $"{n} {isPrime}";
                if (check != isPrime)
                {
                    result = $"Failed: {result}";
                }

                Console.WriteLine(result);
            }
        }

        //computes if in is coprime to each factorial wheel[p0,...,pN],wheel[p0,...,pN-1],...wheel[p0]
        public static bool IsProductFactorialPrime(int n)
        {
            var primes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 };

            var i = 0;
            var p = primes[i++];
            var factors = new List<int>(new[] { p });
            var f = p;

            //compute the first factorial of primes greater than n
            while (f < n)
            {
                p = primes[i++];
                factors.Add(p);
                f *= p;
            }

            // check that largest prime in the factorial does not divide n
            var res = n % p;
            var result = res > 0;
            if (!result)
                return n == p;

            // calculate the previous factorial
            f /= p;
            factors.RemoveAt(--i);
            p = factors[--i];

            //check res is coprime to the wheel:
            //  1. Compute the residue n % factorial
            //  2. If the residue is zero it not coprime.
            //  3. If the residue is a prime used to compute the wheel it is not coprime.
            //  4. If the residue is coprime:
            //      A. If the factorial is 1, return true
            //      B. Reduce the wheel to previous factorial and go to 1.
            while (result && f > 1)
            {
                res = n % p;
                result = res > 0;
                if (!result)
                    return n == p;

                res = n % f;
                result = res > 0;
                if (result && res > 1)
                {
                    result = res % p > 0;
                    if (!result)
                    {
                        return n == p || n % p == 0;
                    }
                }
                if (result)
                {
                    result = res > 1 || factors.IndexOf(res) == -1;
                    if (result)
                    {
                        f /= p;
                        if (i > 0)
                        {
                            factors.RemoveAt(i--);
                            p = factors[i];

                        }
                    }
                }
            }

            return result;
        }


        private static bool FactorialIsPrime(int n, int p, List<int> wheelPrimes)
        {
            if (n == 0) return false;
            if (n == 1) return true;
            if (n < p) return false;
            var idx = wheelPrimes.IndexOf(n);
            if (idx < 0)
            {
                var factorial = 1;
                var test = factorial;
                for (var i = 0; i < wheelPrimes.Count; i++)
                {
                    var fp = factorialPrimes[i];
                    test = fp * factorial;
                    if (test > n)
                    {
                        var res = n % factorial;
                        var result = FactorialIsPrime(res, fp, wheelPrimes);
                        return result;
                    }
                    else
                    {
                        factorial = test;
                    }
                }
            }
            return true;
        }

        public static void PrintResidueClasses()
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"{nameof(Mod_6)}.Count = {Mod_6.Residues.Count.ToString("N0")} ");
            Console.WriteLine($"{nameof(Mod_30)}.Count = {Mod_30.Residues.Count.ToString("N0")} ");
            Console.WriteLine($"{nameof(Mod_210)}.Count =  {Mod_210.Residues.Count.ToString("N0")} ");
            Console.WriteLine($"{nameof(Mod_2_310)}.Count = {Mod_2_310.Residues.Count.ToString("N0")} ");
            Console.WriteLine($"{nameof(Mod_30_030)}.Count = {Mod_30_030.Residues.Count.ToString("N0")} ");
            Console.WriteLine($"{nameof(Mod_510_510)}.Count = {Mod_510_510.Residues.Count.ToString("N0")} ");
            Console.WriteLine($"{nameof(Mod_59_699_690)}.Count = {Mod_59_699_690.Residues.Count.ToString("N0")} ");
            //Console.WriteLine($"{nameof(Mod_223_092_870)}.Count = {Mod_223_092_870.Residues.Count.ToString("N0")} ");
            sw.Stop();
            Console.WriteLine($"Counted residue classes in {sw.Elapsed}");
            Console.WriteLine();

            sw.Restart();
            Console.WriteLine($"{nameof(Mod_6)} {Mod_6.Residues.Count} = {string.Join(", ", Mod_6.Residues)}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{nameof(Mod_30)} {Mod_30.Residues.Count} =  {string.Join(", ", Mod_30.Residues)}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{nameof(Mod_210)}  {Mod_210.Residues.Count} = {string.Join(", ", Mod_210.Residues)}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{nameof(Mod_2_310)} {Mod_2_310.Residues.Count} = {string.Join(", ", Mod_2_310.Residues)}");
            Console.WriteLine();
            Console.WriteLine();
            //Console.WriteLine($"{nameof(Mod_30_030)} {Mod_30_030.Residues.Count} = {string.Join(", ", Mod_30_030.Residues)}");
            //Console.WriteLine($"{nameof(Mod_510_510)}{Mod_510_510.Residues.Count} =  {string.Join(", ", Mod_510_510.Residues)}");
            sw.Stop();
            Console.WriteLine($"Printed residue classes in {sw.Elapsed}");
            Console.WriteLine();

        }
    }
}
