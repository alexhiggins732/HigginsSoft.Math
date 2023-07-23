using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Demos
{
    public class LoopBenchmark
    {
        /*
        |   Method |    Mean |    Error |   StdDev |
        |--------- |--------:|---------:|---------:|
        |  LoopInt | 1.114 s | 0.0085 s | 0.0075 s |
        | LoopUint | 2.243 s | 0.0207 s | 0.0184 s |
        */

        /// <summary>
        /// Integer loop takes mean 1.114s with error of 0.0085 s and StdDev of 0.0075 s
        /// </summary>
        /// <returns></returns>

        // [Benchmark]
        public int LoopInt()
        {
            var result = 0;
            for (var i = 0; i < int.MaxValue; i++)
            {
                result = i;
            }
            return result;
        }

        /// <summary>
        /// Uint loop takes mean 2.243 s with error of 0.0207 s and StdDev of 0.0184 s
        /// </summary>
        /// <returns></returns>

        // [Benchmark]
        public uint LoopUint()
        {
            var result = 0u;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                result = i;
            }
            return result;
        }

        //|      LoopIntAnd1 |    560.2 ms |   4.44 ms |   3.93 ms |
        // [Benchmark]
        public int LoopIntAnd1()
        {
            var result = 0;
            for (var i = 0; i < int.MaxValue; i++)
            {
                result = i & 1;
            }
            return result;
        }

        //|     LoopUintAnd1 |  1,119.4 ms |   7.63 ms |   7.14 ms |
        // [Benchmark]
        public uint LoopUintAnd1()
        {
            var result = 0u;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                result = i & 1;
            }
            return result;
        }

        //|  LoopIntAnd1Bool |  1,111.3 ms |   6.77 ms |   6.33 ms |
        // [Benchmark]
        public bool LoopIntAnd1Bool()
        {
            var result = false;
            for (var i = 0; i < int.MaxValue; i++)
            {
                result = (i & 1) == 1;
            }
            return result;
        }

        //| LoopUintAnd1Bool |  2,220.0 ms |   7.01 ms |   5.47 ms |
        // [Benchmark]
        public bool LoopUintAnd1Bool()
        {
            var result = false;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                result = (i & 1) == 1;
            }
            return result;
        }

        //|      LoopIntMod3 |  2,229.8 ms |  43.34 ms |  46.37 ms |
        // [Benchmark]
        public bool LoopIntMod3()
        {
            var result = false;
            for (var i = 0; i < int.MaxValue; i++)
            {
                result = (i % 3) == 0;
            }
            return result;
        }

        //|     LoopUintMod3 |  3,542.9 ms |  10.26 ms |   9.59 ms |
        // [Benchmark]
        public bool LoopUintMod3()
        {
            var result = false;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                result = (i % 3) == 0;
            }
            return result;
        }

        //|      LoopIntMod6 |  2,806.1 ms |  15.71 ms |  14.69 ms |
        // [Benchmark]
        public bool LoopIntMod6()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 6);
                result = res == 5 || res == 1;
            }
            return result;
        }

        //|     LoopUintMod6 |  4,653.0 ms |  19.05 ms |  16.89 ms |
        // [Benchmark]
        public bool LoopUintMod6()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i % 6u);
                result = res == 5 || res == 1;
            }
            return result;
        }

        //|            LoopIntMod30Or |  4.259 s | 0.0139 s | 0.0123 s |
        //[Benchmark]
        public bool LoopIntMod30Or()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 30);
                result =
                    res == 1 || res == 7 || res == 11 || res == 13 ||
                    res == 17 || res == 19 || res == 23 || res == 29;
            }
            return result;
        }

        //|           LoopUintMod30Or |  9.473 s | 0.0584 s | 0.0546 s |
        //[Benchmark]
        public bool LoopUintMod30Or()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i % 30u);
                result =
                           res == 1 || res == 7 || res == 11 || res == 13 ||
                           res == 17 || res == 19 || res == 23 || res == 29;
            }
            return result;
        }

        //| LoopUintMod30OrHardCodeU | 9.549 s | 0.0454 s | 0.0424 s |
        //[Benchmark]
        public bool LoopUintMod30OrHardCodeU()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i % 30u);
                result =
                           res == 1u || res == 7u || res == 11u || res == 13u ||
                           res == 17u || res == 19u || res == 23 || res == 29u;
            }
            return result;
        }

        //|   LoopUintMod30OrCastInt | 9.455 s | 0.1043 s | 0.0925 s |
        //[Benchmark]
        public bool LoopUintMod30OrCastInt()
        {
            var result = false;
            int res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (int)(i % 30u);
                result =
                           res == 1 || res == 7 || res == 11 || res == 13 ||
                           res == 17 || res == 19 || res == 23 || res == 29;
            }
            return result;
        }

        //|  LoopIntMod_2_3_5_OrElse | 1.944 s | 0.0178 s | 0.0167 s |
        //[Benchmark]
        public bool LoopIntMod_2_3_5_OrElse()
        {
            var result = false;
            for (var i = 0; i < int.MaxValue; i++)
            {
                result = (i & 1) == 1 || (i % 3) > 0 || (i % 5) > 0;
            }
            return result;
        }

        //| LoopUintMod_2_3_5_OrElse | 4.582 s | 0.0739 s | 0.0935 s |
        //[Benchmark]
        public bool LoopUintMod_2_3_5_OrElse()
        {
            var result = false;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                result = (i & 1) == 1 || (i % 3) > 0 || (i % 5) > 0;
            }
            return result;
        }

        //|      LoopIntMod_2_3_5_If | 2.641 s | 0.0136 s | 0.0113 s |
        //[Benchmark]
        public bool LoopIntMod_2_3_5_If()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i & 1);
                if (res == 1)
                {
                    res = (i % 3);
                    if (res > 0)
                        res = res % 5;

                }
                result = res > 0;
            }
            return result;
        }

        // |  LoopIntMod_2_3_5_BinAnd | 4.221 s | 0.0187 s | 0.0156 s |
        //[Benchmark]
        public bool LoopIntMod_2_3_5_BinAnd()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i & 1) & (i % 3) & (i % 5);
                result = res > 0;
            }
            return result;
        }

        //| LoopIntMod_2_3_5_AndAlso | 2.297 s | 0.0094 s | 0.0083 s |
        //[Benchmark]
        public bool LoopIntMod_2_3_5_AndAlso()
        {
            var result = false;
            for (var i = 0; i < int.MaxValue; i++)
            {
                result = (i & 1) == 1 && (i % 3) > 0 && (i % 5) > 1;
            }
            return result;
        }

        //| LoopUintMod_2_3_5_BinAnd | 6.580 s | 0.0166 s | 0.0147 s |
        //[Benchmark]
        public bool LoopUintMod_2_3_5_BinAnd()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i & 1) & (i % 3) & (i % 5);
                result = res > 0;
            }
            return result;
        }

        //| LoopUintMod_2_3_5_AndAlso | 4.487 s | 0.0373 s | 0.0349 s |
        //[Benchmark]
        public bool LoopUintMod_2_3_5_AndAlso()
        {
            var result = false;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                result = (i & 1) == 1 && (i % 3) > 0 && (i % 5) > 1;
            }
            return result;
        }

        //[Benchmark]
        public int PrimeCount2P()
        {
            int count = 0;
            var i = 0;
            int limit = 1 << 22;
            for (; i < limit; i++)
            {
                if (IsPrime_V1(i)) count++;
            }
            if (IsPrime_V1(i))
            {
                count++;
            }
            return count;
        }

        //|        IsPrimeMod_2_3_5_Count | 7.031 s | 0.0230 s | 0.0204 s |
        //[Benchmark]
        public int IsPrimeMod_2_3_5_Count()
        {
            int count = 0;
            var i = 0;
            int limit = int.MaxValue;
            for (; i < limit; i++)
            {
                if (IsPrimeMod_2_3_5(i)) count++;
            }
            if (IsPrimeMod_2_3_5(i))
            {
                count++;
            }
            return count;
        }


        //[Benchmark]
        public bool IsPrimeMod_2_3_5(int i)
        {
            var result = ((i & 1) == 1 || i == 2)
              || ((i % 3) > 0 || i == 3)
              || ((i % 5) > 0 || i == 5);
            return result;
        }

        //| IsPrimeMod_2_3_5_Count_Inline | 2.617 s | 0.0426 s | 0.0378 s |
        //[Benchmark]
        public int IsPrimeMod_2_3_5_Count_Inline()
        {
            int count = 0;
            var i = 0;
            int limit = int.MaxValue;
            for (; i < limit; i++)
            {
                if (IsPrimeMod_2_3_5_Inline(i)) count++;
            }
            if (IsPrimeMod_2_3_5_Inline(i))
            {
                count++;
            }
            return count;
        }

        //[Benchmark]
        //public int InlinePrimeCheckCount()
        //{
        //    var count = 0;
        //    var limit = 46337 * 46337;
        //    int i;
        //    bool result;
        //    for (i = 0; i < int.MaxValue; i++)
        //    {
        //        result = InlinePrimeCheck.IsPrime(i);
        //        if (result)
        //            count++;
        //    }
        //    result = InlinePrimeCheck.IsPrime(i);
        //    if (result)
        //        count++;
        //    return count;
        //}



        //| IsPrimeMod_2_3_5_Count_Inline | 2.881 s | 0.0260 s | 0.0217 s |
        //[Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrimeMod_2_3_5_Inline(int i)
        {
            var result = ((i & 1) == 1 || i == 2)
              || ((i % 3) > 0 || i == 3)
              || ((i % 5) > 0 || i == 5);
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrime_V1(int i)
        {
            var result = ((i & 1) == 1 || i == 2)
                || ((i % 3) > 0 || i == 3)
                || ((i % 5) > 0 || i == 5);


            if (result && i > 25)
            {
                var root = System.Math.Sqrt(i);
                var k = 5;
                var j = 7;
                for (; (result = i % k > 0) && (result = i % j > 0) && (k <= root || j <= root); k += 6, j += 6) ;

            }

            return result;
        }

        /// <summary>
        //|      IsPrimeMod30Count | 4.919 s | 0.0949 s | 0.1299 s |
        //[Benchmark]
        public int IsPrimeMod30Count()
        {
            int count = 0;
            int i;
            for (i = 0; i < int.MaxValue; i++)
            {
                if (IsPrimeMod30(i)) count++;
            }
            if (IsPrimeMod30(i)) count++;
            return count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrimeMod30(int i)
        {
            var res = i % 30;
            var result = res == 1 || res == 7 || res == 11 || res == 13 ||
                           res == 17 || res == 19 || res == 23 || res == 29;
            return result;
        }

        //| IsPrimeMod30Count_V2 | 4.921 s | 0.0846 s | 0.1070 s |
        //[Benchmark]
        public int IsPrimeMod30Count_V2()
        {
            int count = 0;
            int i;
            for (i = 0; i < int.MaxValue; i++)
            {
                if (IsPrimeMod30_V2(i)) count++;
            }
            if (IsPrimeMod30_V2(i)) count++;
            return count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrimeMod30_V2(int i)
        {
            var res = i % 30;
            var result = (res & 1) == 1 || ((res % 3 > 0) && (res % 5 > 0));

            return result;
        }



        static readonly int[] mod30Ints =
            new int[] { 1, 7, 11, 13, 17, 19, 23, 29 };

        //|       LoopIntMod30IndexOf | 18.734 s | 0.0528 s | 0.0494 s |
        //[Benchmark]
        public bool LoopIntMod30IndexOf()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 30);
                result = Array.IndexOf(mod30Ints, res) == 0;
            }
            return result;
        }

        //| LoopIntMod30IndexOfInline | 19.190 s | 0.0699 s | 0.0653 s |
        //[Benchmark]
        public bool LoopIntMod30IndexOfInline()
        {
            var result = false;
            int res;
            var mod30Ints = new[] { 1, 7, 11, 13, 17, 19, 23, 29 };
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 30);
                result = Array.IndexOf(mod30Ints, res) == 0;
            }
            return result;
        }


        static readonly uint[] mod30Uints =
           new uint[] { 1, 7, 11, 13, 17, 19, 23, 29 };

        //|      LoopUintMod30IndexOf | 38.292 s | 0.0920 s | 0.0860 s |
        //[Benchmark]
        public bool LoopUintMod30IndexOf()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i % 30u);
                result = Array.IndexOf(mod30Uints, res) == 0;
            }
            return result;
        }

        //|    LoopIntMod210 | 17,973.2 ms | 151.86 ms | 142.05 ms |
        // [Benchmark]
        public bool LoopIntMod210Or()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 210);
                result =
                    res == 11 || res == 13 || res == 17 || res == 19 || res == 23 || res == 29 ||
                    res == 37 || res == 41 || res == 43 || res == 47 || res == 53 || res == 59 ||
                    res == 61 || res == 67 || res == 71 || res == 73 || res == 79 || res == 83 ||
                    res == 89 || res == 97 || res == 101 || res == 103 || res == 107 || res == 109 ||
                    res == 113 || res == 127 || res == 131 || res == 137 || res == 139 || res == 149 ||
                    res == 151 || res == 157 || res == 163 || res == 167 || res == 173 || res == 179 ||
                    res == 181 || res == 191 || res == 193 || res == 197 || res == 199 || res == 209;
            }
            return result;
        }

        //| LoopIntMod210V2 |  7.337 s | 0.1462 s | 0.2188 s |
        //[Benchmark]
        public bool LoopIntMod210V2()
        {
            bool result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 210);
                result = !((res & 1) == 0 || res % 3 == 0 || res % 5 == 0 || res % 7 == 0 || res % 11 == 0 || res % 13 == 0);
            }
            return result;
        }


        //|   LoopUintMod210 | 33,201.3 ms | 177.08 ms | 156.98 ms |
        // [Benchmark]
        public bool LoopUintMod210Or()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i % 210u);
                result =
                    res == 11 || res == 13 || res == 17 || res == 19 || res == 23 || res == 29 ||
                    res == 37 || res == 41 || res == 43 || res == 47 || res == 53 || res == 59 ||
                    res == 61 || res == 67 || res == 71 || res == 73 || res == 79 || res == 83 ||
                    res == 89 || res == 97 || res == 101 || res == 103 || res == 107 || res == 109 ||
                    res == 113 || res == 127 || res == 131 || res == 137 || res == 139 || res == 149 ||
                    res == 151 || res == 157 || res == 163 || res == 167 || res == 173 || res == 179 ||
                    res == 181 || res == 191 || res == 193 || res == 197 || res == 199 || res == 209;

            }
            return result;
        }


        static readonly int[] mod210Ints = new[] {   11, 13, 17, 19, 23, 29,
                    37, 41, 43, 47, 53, 59,
                    61, 67, 71, 73, 79, 83,
                    89, 97, 101, 103, 107, 109,
                    113, 127, 131, 137, 139, 149,
                    151, 157, 163, 167, 173, 179,
                    181, 191, 193, 197, 199, 209
        };

        //[Benchmark]
        public bool LoopIntMod210IndexOf()
        {
            var result = false;
            int res;
            for (var i = 0; i < int.MaxValue; i++)
            {
                res = (i % 210);
                result = Array.IndexOf(mod210Ints, res) > -1;
            }
            return result;
        }


        static readonly uint[] mod210Uints = new uint[] {   11, 13, 17, 19, 23, 29,
                    37, 41, 43, 47, 53, 59,
                    61, 67, 71, 73, 79, 83,
                    89, 97, 101, 103, 107, 109,
                    113, 127, 131, 137, 139, 149,
                    151, 157, 163, 167, 173, 179,
                    181, 191, 193, 197, 199, 209
        };

        //|   LoopUintMod210 | 33,201.3 ms | 177.08 ms | 156.98 ms |
        //[Benchmark]
        public bool LoopUintMod210IndexOf()
        {
            var result = false;
            uint res;
            for (var i = 0u; i < uint.MaxValue; i++)
            {
                res = (i % 210u);
                result = Array.IndexOf(mod210Uints, res) > -1;
            }
            return result;
        }

        //[Benchmark]
        public int CheckModWheels()
        {
            int count = 0;
            for (var i = 0; i < int.MaxValue; i++)
            {
                bool result = Check_Mod_Wheels(i);
                if (result)
                    count++;
            }
            return count;
        }
        public class ModConts
        {
            public const int Wheel_6 = 2 * 3;
            public const int Wheel_30 = Wheel_6 * 5;
            public const int Wheel_210 = Wheel_30 * 7;
            public const int Wheel_2_310 = Wheel_210 * 11;
            public const int Wheel_30_030 = Wheel_2_310 * 13;
            public const int Wheel_510_510 = Wheel_30_030 * 17;
            public const int Wheel_9_699_690 = Wheel_510_510 * 19;
            public const int Wheel_223_092_870 = Wheel_9_699_690 * 23;
        }

        // CheckModWheels | 11.754 s | 0.0406 s | 0.0380 s |
        public bool Check_Mod_Wheels(int i)
        {
            bool result = ((i & 1) == 1 || i == 2) || (i % 3 > 0 || i == 3);
            if (!result)
                result = i > ModConts.Wheel_6 && Check_Mod_6(i)
                    && i > ModConts.Wheel_30 && Check_Mod_30(i)
                    && i > ModConts.Wheel_210 && Check_Mod_210(i)
                    && i > ModConts.Wheel_2_310 && Check_Mod_2_310(i)
                    && i > ModConts.Wheel_30_030 && Check_Mod_30_030(i)
                    && i > ModConts.Wheel_510_510 && Check_Mod_510_510(i)
                    && i > ModConts.Wheel_9_699_690 && Check_Mod_9_699_690(i)
                    && i > ModConts.Wheel_223_092_870 && Check_Mod_223_092_870(i);
            return result;

        }

        public bool Check_Mod_6(int i)
        {
            //2*3
            var res = i % 6;
            return res == 1 || res == 5;
        }

        public bool Check_Mod_30(int i)
        {
            //2*3*5
            var res = i % 30;
            return res == 1 || res > 5;
        }

        public bool Check_Mod_210(int i)
        {
            //2310 = 2 * 3 * 5 * 7 ;
            var res = i % 210;
            var result = res == 1 || res > 7;
            return result;
        }

        public bool Check_Mod_2_310(int i)
        {
            //2310 = 2 * 3 * 5 * 7 * 11;
            var res = i % 2310;
            var result = res == 1 || res > 11;
            return result;
        }

        public bool Check_Mod_30_030(int i)
        {
            //2310 = 2 * 3 * 5 * 7 * 11 * 13;
            var res = i % 30_030;
            var result = res == 1 || res > 13;
            return result;
        }

        public bool Check_Mod_510_510(int i)
        {
            //2310 = 2 * 3 * 5 * 7 * 11 * 13 * 17;
            var res = i % 510_510;
            var result = res == 1 || res > 17;
            return result;
        }

        public bool Check_Mod_9_699_690(int i)
        {
            //2310 = 2 * 3 * 5 * 7 * 11 * 13 * 17;
            var res = i % 9_699_690;
            var result = res == 1 || res > 19;
            return result;
        }

        public bool Check_Mod_223_092_870(int i)
        {
            //2310 = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19 ;
            var res = i % 223_092_870;
            var result = res == 1 || res > 19;
            return result;
        }

    }
    internal class PrimalityDemo
    {
        public static void Run()
        {

        }


    }
}
