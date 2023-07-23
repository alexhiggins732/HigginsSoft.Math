#define INLINEPRIMECHECK
#undef INLINEPRIMECHECK
namespace HigginsSoft.Math.Demos
{
    using HigginsSoft.Math.Lib;
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Text;


    using Microsoft.CSharp;
    using System.CodeDom.Compiler;
    using System.Reflection;
    using System.Diagnostics;
    using BenchmarkDotNet.Attributes;
    using Microsoft.Diagnostics.Tracing.Parsers.Clr;

    public class PrimeCheckerBenchmarks
    {
        const int maxFactorPrime = 46337;
        static readonly int[] primes = new PrimeGeneratorUnsafe(maxFactorPrime).ToArray();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPrimeLoopMod6(int i)
        {
            if (i < 2) return false;
            if ((i & 1) == 0) return i == 2;

            if (i % 3 == 0) return i == 3;
            bool result = true;
            if (i > 24)
            {
                var root = System.Math.Sqrt(i);
                var k = 5;
                var j = 7;
                for (; (result = i % k > 0) && (result = i % j > 0) && (k <= root || j <= root); k += 6, j += 6) ;

            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPrimeLoopDiv(int i)
        {
            if (i < 2 || (i & 1) == 0) return i == 2;
            var arr = primes;
            var p = arr[1];
            var limit = System.Math.Sqrt(i);
            for (var j = 0; p <= limit && j < arr.Length; j++)
            {
                p = arr[j];
                if (i % p == 0) return i == p;
            }
            return true;
        }

        private static Func<int, bool> _isPrime = null!;

        static PrimeCheckerBenchmarks()
        {
            var maxPrime = 46337;
            CompileIsPrime(maxPrime);
        }


        //| Time_2p16_IsPrimeLoopMod6 |   2.522 ms | 0.0023 ms | 0.0021 ms |
        //[Benchmark]
        public int Time_2p16_IsPrimeLoopMod6()
        {
            int count = 0;
            var limit = 1 << 16;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopMod6(i);
                if (result)
                    count++;
            }
            return count;
        }

#if INLINEPRIMECHECK
        //| Time_2p16_Inline | 186.6 ms | 1.17 ms | 1.04 ms |
        [Benchmark]
        public int Time_2p16_Inline()
        {
            int count = 0;
            var limit = 1 << 16;
            for (var i = 0; i < limit; i++)
            {
                var result = InlinePrimeCheck.IsPrime(i);
                if (result)
                    count++;
            }
            return count;
        }
#endif
        //|        Time_2p16_Compiled | 186.736 ms | 0.7528 ms | 0.6674 ms |
        //[Benchmark]
        public int Time_2p16_Compiled()
        {
            int count = 0;
            var limit = 1 << 16;
            for (var i = 0; i < limit; i++)
            {
                var result = _isPrime(i);
                if (result)
                    count++;
            }
            return count;
        }

        //|  Time_2p16_IsPrimeLoopDiv |   2.189 ms | 0.0058 ms | 0.0049 ms |
        //[Benchmark]
        public int Time_2p16_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 16;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        // end 2p16


        //|        Time_2p20_Compiled | 2,354.67 ms | 11.426 ms | 10.129 ms
        //[Benchmark]
        public int Time_2p20_Compiled()
        {
            int count = 0;
            var limit = 1 << 20;
            for (var i = 0; i < limit; i++)
            {
                var result = _isPrime(i);
                if (result)
                    count++;
            }
            return count;
        }

        //| Time_2p20_IsPrimeLoopMod6 |   122.51 ms |  0.302 ms |  0.282 ms |
        //[Benchmark]
        public int Time_2p20_IsPrimeLoopMod6()
        {
            int count = 0;
            var limit = 1 << 20;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopMod6(i);
                if (result)
                    count++;
            }
            return count;
        }


        //end p20



        //| Time_2p24_IsPrimeLoopMod6 | 6,401.84 ms | 25.107 ms | 23.485 ms |
        //0.0003815 ms per number
        //[Benchmark]
        public int Time_2p24_IsPrimeLoopMod6()
        {
            int count = 0;
            var limit = 1 << 24;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopMod6(i);
                if (result)
                    count++;
            }
            return count;
        }


        //| Time_2p16IsPrimeLoopDiv | 2.207 ms | 0.0050 ms | 0.0044 ms |
        //[Benchmark]
        public int Time_2p16IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 16;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        // | Time_2p17IsPrimeLoopDiv | 5.306 ms | 0.0349 ms | 0.0292 ms |
        //[Benchmark]
        public int Time_2p17IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 17;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //| Time_2p118IsPrimeLoopDiv |     12.68 ms |  0.026 ms |  0.021 ms |
        //[Benchmark]
        public int Time_2p18IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 18;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //|  Time_2p19IsPrimeLoopDiv |     31.02 ms |  0.207 ms |  0.183 ms |
        //[Benchmark]
        public int Time_2p19IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 19;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //|  Time_2p20_IsPrimeLoopDiv |    76.66 ms |  0.199 ms |  0.186 ms |
        //[Benchmark]
        public int Time_2p20_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 20;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //| Time_2p21_IsPrimeLoopDiv |   189.5 ms | 0.23 ms | 0.21 ms |
        //[Benchmark]
        public int Time_2p21_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 21;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //| Time_2p22_IsPrimeLoopDiv |   475.4 ms | 4.85 ms | 4.30 ms |
        //[Benchmark]
        public int Time_2p22_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 22;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }


        //| Time_2p23_IsPrimeLoopDiv | 1,197.9 ms | 4.61 ms | 4.31 ms |
        //[Benchmark]
        public int Time_2p23_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 23;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }


        //|  Time_2p24_IsPrimeLoopDiv | 3,046.25 ms | 10.780 ms |  9.556 ms |
        // 0.0001815ms per number
        //[Benchmark]
        public int Time_2p24_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 24;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //| Time_2p25_IsPrimeLoopDiv |  7,786.03 ms | 39.802 ms | 37.231 ms |
        //[Benchmark]
        public int Time_2p25_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 25;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //| Time_2p26_IsPrimeLoopDiv | 20,104.57 ms | 24.984 ms | 23.370 ms |
        //[Benchmark]
        public int Time_2p26_IsPrimeLoopDiv()
        {
            int count = 0;
            var limit = 1 << 26;
            for (var i = 0; i < limit; i++)
            {
                var result = IsPrimeLoopDiv(i);
                if (result)
                    count++;
            }
            return count;
        }

        //end p20



        public static int TestLoopDiv()
        {
            int count = 0;
            var limit = 1 << 20;
            for (var i = 0; i < limit; i++)
            {
                var check = IsPrimeLoopDiv(i);
                var result = IsPrimeLoopMod6(i);
                if (check != result)
                {
                    string comp = $"{i} Result: {result} - Check: {check}";
                    Console.WriteLine(comp);
                }
                if (result)
                    count++;
            }
            return count;
        }

        public static int TestDiv()
        {

            int count = 0;
            var limit = int.MaxValue;
            for (var i = (limit - (1 << 20)); i < limit; i++)
            {
                var check = IsPrimeLoopDiv(i);
                var result = _isPrime(i);
                if (check != result)
                {
                    string comp = $"{i} Result: {result} - Check: {check}";
                    Console.WriteLine(comp);
                }
                if (result)
                    count++;
            }
            return count;

        }

        public static int TestCompiler()
        {

            int count = 0;
            var limit = int.MaxValue;
            for (var i = limit - (1 << 16); i < limit; i++)
            {
                var check = IsPrimeLoopMod6(i);
                var result = _isPrime(i);
                if (check != result)
                {
                    string comp = $"{i} Result: {result} - Check: {check}";
                    Console.WriteLine(comp);
                }
                if (result)
                    count++;
            }
            return count;

        }
        public static bool IsPrimeParensTest(int i)
        {
            var result =
                (i < 2 || ((i & 1) == 0 && i != 2))
                || (i > 2 * 2 && i % 3 == 0)
                || (i > 3 * 3 && i % 5 == 0)
                || (i > 5 * 5 && i % 7 == 0)
                || (i > 7 * 7 && i % 11 == 0)
                || i > 11 * 11;
            return !result;
        }


        public static void CompileIsPrime(int maxPrime)
        {

            //var result =
            //    (i < 2 || ((i & 1) == 0 && i != 2))
            //    || (i > 2 * 2 && i % 3 == 0)
            //    || (i > 3 * 3 && i % 5 == 0)
            //    || (i > 5 * 5 && i % 7 == 0)
            //    || (i > 7 * 7 && i % 11 == 0)
            //    || i > 11 * 11;
            // return !result;

            ParameterExpression parameter = Expression.Parameter(typeof(int), "i");


            // i<2
            BinaryExpression lessThanTwo = Expression.LessThan(parameter, Expression.Constant(2));
            // i & 1
            BinaryExpression mod2 = Expression.MakeBinary(ExpressionType.And, parameter, Expression.Constant(1));
            // i & 1 == 0
            BinaryExpression isEven = Expression.Equal(mod2, Expression.Constant(0));

            // i != 2 
            BinaryExpression not2 = Expression.NotEqual(parameter, Expression.Constant(2));

            // ((i & 1) == 0 && i != 2)
            BinaryExpression eventAndNotTwo = Expression.AndAlso(isEven, not2);

            //(i < 2 || ((i & 1) == 0 && i != 2))
            BinaryExpression lessThan2OrIsEvenAndNot2 = Expression.OrElse(lessThanTwo, eventAndNotTwo);


            //i > 2 * 2
            BinaryExpression greaterThanSquare = Expression.GreaterThan(parameter, Expression.Constant(2 * 2));

            //i % 3
            BinaryExpression modP = Expression.Modulo(parameter, Expression.Constant(3));

            //i % 3==0
            BinaryExpression zeroModP = Expression.Equal(modP, Expression.Constant(0));

            // (i > 2 * 2) && (i % 3==0)
            BinaryExpression right = Expression.AndAlso(greaterThanSquare, zeroModP);

            //    (i < 2 || ((i & 1) == 0 && i != 2))
            //    || (i > 2 * 2 && i % 3 == 0)
            BinaryExpression orElse = Expression.OrElse(lessThan2OrIsEvenAndNot2, right);

            var previousPrime = 3;
            for (var j = 2; j < primes.Length; j++)
            {
                var p = primes[j];
                var square = previousPrime * previousPrime;
                previousPrime = p;
                // (i > p * p)
                var gtThisSquare = Expression.GreaterThan(parameter, Expression.Constant(square));

                //i % p
                var mopThisP = Expression.Modulo(parameter, Expression.Constant(p));

                //i % p ==0
                var zeroModThisP = Expression.Equal(mopThisP, Expression.Constant(0));

                // (i > (p-1) * (p-1) ) && (i % p==0)
                var thisAlso = Expression.AndAlso(gtThisSquare, zeroModThisP);

                orElse = Expression.OrElse(orElse, thisAlso);
            }

            //    || i > 11 * 11;

            var lastSquare = Expression.Constant(previousPrime * previousPrime);
            var gtLastSquare = Expression.GreaterThan(parameter, lastSquare);

            //these last three are not needed but having a final || i > 11 * 11 seems to not be working
            //var lastModP = Expression.Modulo(parameter, Expression.Constant(previousPrime));
            //var zeroModLastP = Expression.Equal(lastModP, Expression.Constant(0));
            //var lastAlso = Expression.AndAlso(gtLastSquare, zeroModLastP);

            //but instead of wasting a nother modulu, just embed a false constant because the mod was already checked
            var lastAlso = Expression.AndAlso(gtLastSquare, Expression.Constant(false));

            orElse = Expression.OrElse(orElse, lastAlso);

            UnaryExpression not = Expression.Not(orElse);

            _isPrime = Expression.Lambda<Func<int, bool>>(not, parameter).Compile();


        }
    }

}
