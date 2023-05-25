#define RUN_LONG_TESTS
#undef RUN_LONG_TESTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HigginsSoft.Math.Lib;
using Microsoft.Diagnostics.Runtime.Utilities;
using Newtonsoft.Json.Linq;


namespace HigginsSoft.Math.Demos.Tests
{

    [TestClass()]
    public class Unsigned8PerfectSquareResiduesTests
    {

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void CountByteTest()
        {
            Func<byte, int> count = PerfectSquareResidues.Count;

            int last = 0;
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                if (last > i)
                    throw new Exception($"last({last}) > i({i}) ");
                last = i;
                try
                {
                    var result = count((byte)i);
                    Console.WriteLine($"{i}\t{result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void CountBytePrimesTest()
        {
            Func<ushort, int> count = PerfectSquareResidues.Count;

            int last = 0;
            var primes = new PrimeGeneratorUnsafe(byte.MaxValue).ToList();

            for (int i = 0; i < primes.Count; i++)
            {
                var p = primes[i];

                try
                {
                    var result = count((ushort)p);
                    Console.WriteLine($"{p}\t{result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintBytePerfectSquaresTest()
        {
            Func<int, List<int>> get = PerfectSquareResidues.GetPerfectSquareResidues;

            for (int i = 0; i < byte.MaxValue; i++)
            {
                var p = i;
                var squares = get(p);

                Console.WriteLine($"{p}\t{squares.Count}\t{string.Join("\t", squares)}");
            }
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintBytePerfectSquareDistributionTest()
        {
            Func<int, List<int>> get = PerfectSquareResidues.GetPerfectSquareResidueDistributions;

            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var p = i;
                var squares = get(p);

                Console.WriteLine($"{p}\t{squares.Count}\t{string.Join("\t", squares)}");
            }
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintBytePrimePerfectSquaresTest()
        {
            Func<int, List<int>> get = PerfectSquareResidues.GetPerfectSquareResidues;

            int last = 0;
            var primes = new PrimeGeneratorUnsafe(byte.MaxValue).ToList();

            for (int i = 0; i < primes.Count; i++)
            {
                var p = primes[i];
                var squares = get(p);

                Console.WriteLine($"{p}\t{squares.Count}\t{string.Join("\t", squares)}");
            }
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintByteResiduesTest()
        {
            PrintResidues(Enumerable.Range(1, 255));
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintBytePrimeResiduesTest()
        {
            var gen = new PrimeGeneratorUnsafe(byte.MaxValue);
            PrintResidues(gen);
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        void PrintResidues(IEnumerable<int> values)
        {
            var sb = new StringBuilder();
            sb.AppendLine("|N\t|Residues");

            foreach (var value in values)
            {
                var root = MathLib.Sqrt(value, out bool isexact);
                int start = isexact ? root : root + 1;
                //if (isexact) { return value < 1 ? 0 : 2; }
                int end = (int)value - 1;
                List<int> result = new();

                var residues = new List<int>();
                for (var i = start; i <= end; i++)
                {
                    var square = i * i;
                    var res = square % value;
                    residues.Add(res);
                }
                sb.AppendLine($"|{value}\t|{string.Join(" ", residues)}");
            }
            Console.WriteLine(sb.ToString());
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintBytePrimeResiduesUniqueFactorsTest()
        {
            var values = new PrimeGeneratorUnsafe(byte.MaxValue);


            var sb = new StringBuilder();
            sb.AppendLine("|N\t|Residues|Unique Residues Factors");

            foreach (var value in values)
            {
                var root = MathLib.Sqrt(value, out bool isexact);
                int start = isexact ? root : root + 1;
                //if (isexact) { return value < 1 ? 0 : 2; }
                int end = (int)value - 1;


                var residues = new List<int>();
                var factors = new List<int>();
                for (var i = start; i <= end; i++)
                {
                    var square = i * i;
                    var res = square % value;
                    residues.Add(res);
      
                    var factorization = Factorization.FactorTrialDivide(res);
                    var primeResidues = factorization.Factors.Select(x => (int)x.P).Distinct().ToList();
                    factors.AddRange(primeResidues);
               
                }
                var distinctFactors = factors.Distinct().OrderBy(x => x).ToList();
                sb.AppendLine($"|{value}\t|{string.Join(" ", distinctFactors)}\t|{string.Join(" ", residues)}");
            }
            Console.WriteLine(sb.ToString());
        }

#if RUN_LONG_TESTS
#else
        [Ignore]
#endif
        [TestMethod()]
        public void PrintByteAllResiduesUniqueFactorsTest()
        {
            var values = Enumerable.Range(1, 255);


            var sb = new StringBuilder();
            sb.AppendLine("|N\t|Residues|Unique Residues Factors");

            foreach (var value in values)
            {
                var root = MathLib.Sqrt(value, out bool isexact);
                int start = isexact ? root : root + 1;
                //if (isexact) { return value < 1 ? 0 : 2; }
                int end = (int)value - 1;


                var residues = new List<int>();
                var factors = new List<int>();
                for (var i = start; i <= end; i++)
                {
                    var square = i * i;
                    var res = square % value;
                    residues.Add(res);
 
                    var factorization = Factorization.FactorTrialDivide(res);
                    var primeResidues = factorization.Factors.Select(x => (int)x.P).Distinct().ToList();
                    factors.AddRange(primeResidues);

                }
                var distinctFactors = factors.Distinct().OrderBy(x => x).ToList();
                sb.AppendLine($"|{value}\t|{string.Join(" ", distinctFactors)}\t|{string.Join(" ", residues)}");
            }
            Console.WriteLine(sb.ToString());
        }


    }

#if RUN_LONG_TESTS
#else
    [Ignore]
#endif
    [TestClass()]
    public class Signed16SPerfectSquareResiduesTests
    {

        [TestMethod()]
        public void CountShortTest()
        {
            Func<ushort, int> count = PerfectSquareResidues.Count;

            int last = 0;
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                if (last > i)
                    throw new Exception($"last({last}) > i({i}) ");
                last = i;
                try
                {
                    var result = count((ushort)i);
                    Console.WriteLine($"{i}\t{result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


        [TestMethod()]
        public void CountShortPrimesTest()
        {
            Func<ushort, int> count = PerfectSquareResidues.Count;

            int last = 0;
            var primes = new PrimeGeneratorUnsafe(ushort.MaxValue).ToList();

            for (int i = 0; i < primes.Count; i++)
            {
                var p = primes[i];

                try
                {
                    var result = count((ushort)p);
                    Console.WriteLine($"{p}\t{result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


        [TestMethod()]
        public void CountTest()
        {
            Func<short, int> count = PerfectSquareResidues.Count;
            count(short.MaxValue);
            for (short i = 0; i <= short.MaxValue; i++)
            {
                try
                {
                    var result = count(i);
                    Console.WriteLine($"{i}\t{result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        [TestMethod()]
        public void FactorTest()
        {
            //Assert.Fail();
        }
    }
}