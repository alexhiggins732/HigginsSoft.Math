/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Some routines inspired by the Stanford Bit Twiddling Hacks by Sean Eron Anderson:
// http://graphics.stanford.edu/~seander/bithacks.html

#define TARGET_64BIT


using System;
using System.Xml.Linq;

namespace HigginsSoft.Math.Lib
{
    public partial class MathLib
    {
        public class Congruence
        {
            public int Poly;
            public int Base;
            public int StartValue;
            public Congruence(int poly, int @base)
            {
                this.Poly = poly;
                this.Base = @base;
                this.Value = Poly;
                this.StartValue = Value;
            }
            public int MoveNext() => Value += Base;

            public int Value;

            public override string ToString()
            {
                return $"{Poly} + {Base} * x";
            }

            public void Reset() { Value = StartValue; }
        }
        public static (int N, int Solution) CRT(List<Congruence> congruences)
        {
            int[] remainders = congruences.Select(x => x.Poly).ToArray();
            int[] modulos = congruences.Select(x => x.Base).ToArray();
            var solver = new ChineseRemainderTheoremSolver(remainders, modulos);
            if (false)
            {
                var debug = CRTZDebug(congruences);
            }
            return solver.Solve2();

        }

        public static (GmpInt N, GmpInt Solution) CRTZ(List<Congruence> congruences)
        {
            int[] remainders = congruences.Select(x => x.Poly).ToArray();
            int[] modulos = congruences.Select(x => x.Base).ToArray();
            var solver = new ChineseRemainderTheoremSolver(remainders, modulos);
            return solver.SolveZ();
        }

        public static (GmpInt N, GmpInt Solution) CRTZDebug(List<Congruence> congruences)
        {
            int[] remainders = congruences.Select(x => x.Poly).ToArray();
            int[] modulos = congruences.Select(x => x.Base).ToArray();
            var solver = new ChineseRemainderTheoremSolver(remainders, modulos);
            return solver.SolveZDebug();
        }


        /// <summary>
        /// Efficient implement of CRT using precomputed M to allow for various cobminations of remainders to be solved for a system of  classes.
        /// </summary>

        public class CrtCongruenceSolver
        {

            private readonly int[] modulos;
            //private readonly long[] mi;
            //private readonly long[] miInverse;
            private readonly long[] m;
            public CrtCongruenceSolver(int[] modulos)
            {
                this.modulos = modulos;
                //this.mi = new long[modulos.Length];
                //this.miInverse= new long[modulos.Length];
                this.m = new long[modulos.Length];
            }

            public int N;
            public int ComputeN()
            {
                int len = modulos.Length;

                N = 1;

                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    N *= modulos[i];
                }
                if (N > int.MaxValue)
                    throw new OverflowException($"Solution N is larger than than int.MaxValue: {N.ToString("N0")}");


                for (int i = 0; i < len; i++)
                {
                    var mi = N / modulos[i];
                    var miInverse = ChineseRemainderTheoremSolver.ModularMultiplicativeInverse(mi, modulos[i]);
                    m[i] = (mi * miInverse) % N;
                }
                return this.N;
            }


            public (int N, int Result) Solve(int[] remainders)
            {
                if (remainders.Length != modulos.Length)
                {
                    throw new ArgumentException("The number of remainders must match the number of modulos.");
                }

                int len = remainders.Length;
                long result = 0;
                for (int i = 0; i < len; i++)
                {
                    if (false) // don't use precomputed m
                    {
                        /*
                        var t = remainders[i] * mi[i];
                        if (t > N) t %= N;
                        t *= miInverse[i];
                        if (t > N) t %= N;
                        t += result;
                        if (t > N) t %= N;
                        result = (int)t;
                        */
                    }
                    else
                    {
                        var t = remainders[i] * m[i];
                        if (t > N) t %= N;
                        t += result;
                        if (t > N) t %= N;
                        result = (int)t;
                    }

                }

                if (result < 0)
                {
                    result += N;
                }

                return ((int)N, (int)result);
            }
        }


        public class CrtCongruenceRecursiveEnumerator
        {

            private readonly int[] modulos;
            //private readonly long[] mi;
            //private readonly long[] miInverse;
            private readonly long[] m;
            public CrtCongruenceRecursiveEnumerator(int[] modulos)
            {
                this.modulos = modulos;
                //this.mi = new long[modulos.Length];
                //this.miInverse= new long[modulos.Length];
                this.m = new long[modulos.Length];
            }

            public int N;
            public int ComputeN()
            {
                int len = modulos.Length;

                N = 1;

                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    N *= modulos[i];
                }
                if (N > int.MaxValue)
                    throw new OverflowException($"Solution N is larger than than int.MaxValue: {N.ToString("N0")}");


                for (int i = 0; i < len; i++)
                {
                    var mi = N / modulos[i];
                    var miInverse = ChineseRemainderTheoremSolver.ModularMultiplicativeInverse(mi, modulos[i]);
                    m[i] = (mi * miInverse) % N;
                }
                return this.N;
            }


            public IEnumerable<int> EnumerateResults(List<List<Congruence>> pList)
            {
                return EnumerateResults(0, pList, 0);
            }

            IEnumerable<int> EnumerateResults(int start, List<List<Congruence>> pList, long result)
            {
                var clist = pList[start];
                if (start == pList.Count - 1)
                {
                    for (var i = 0; i < clist.Count; i++)
                    {
                        long t = result + clist[i].Poly * m[start];
                        if (t > N) t %= N;

                        yield return (int)t;
                    }
                }
                else
                {
                    for (var i = 0; i < clist.Count; i++)
                    {
                        long t = result + clist[i].Poly * m[start];
                        if (t > N) t %= N;

                        foreach (var value in EnumerateResults(start + 1, pList, t))
                        {
                            yield return value;
                        }
                    }
                }
            }



        }


        public class CrtCongruenceInlineEnumeratorWIP
        {

            private readonly int[] modulos;
            //private readonly long[] mi;
            //private readonly long[] miInverse;
            private readonly long[] m;
            public CrtCongruenceInlineEnumeratorWIP(int[] modulos)
            {
                this.modulos = modulos;
                //this.mi = new long[modulos.Length];
                //this.miInverse= new long[modulos.Length];
                this.m = new long[modulos.Length];
            }

            public int N;
            public int ComputeN()
            {
                int len = modulos.Length;

                N = 1;

                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    N *= modulos[i];
                }
                if (N > int.MaxValue)
                    throw new OverflowException($"Solution N is larger than than int.MaxValue: {N.ToString("N0")}");


                for (int i = 0; i < len; i++)
                {
                    var mi = N / modulos[i];
                    var miInverse = ChineseRemainderTheoremSolver.ModularMultiplicativeInverse(mi, modulos[i]);
                    m[i] = (mi * miInverse) % N;
                }
                return this.N;
            }

            //Logic Testmethod: TestJaggedEnumeratorInline
            public IEnumerable<int> EnumerateResults(List<List<Congruence>> l)
            {
                //return EnumerateResults(0, pList, 0);

                int elements = l.Count;
                var len = elements - 1;
                var outerList = l[len];
                var outerCount = outerList.Count;
                bool hasMore = true;
                var indexes = new int[elements];
                var counts = l.Select(x => x.Count - 1).ToArray();
                var mulcache = l.Select(x => 0l).ToArray();
                var startmults = l.Select(x => 0l).ToArray();
                int count = 0;
                bool showWork = false;

                long result = 0;
                long t = 0;
                int idx = 0;
                for (idx = 0; idx < len - 1; idx++)
                {
                    t = l[idx][0].Poly * m[idx];
                    startmults[idx] = mulcache[idx] = t;
                    result += t;
                }
                t = result;

                while (hasMore)
                {
                    // loop throw all words in the outer index using ref int;

                    for (idx = 0; idx < outerCount; idx++)
                    {
                        count++;
                        if (showWork)
                        {
                            Console.WriteLine($"Values: [{string.Join(", ", indexes)}]");
                        }
                        t = result + outerList[idx].Poly * m[idx];
                        if (t > N) t %= N;
                        yield return (int)result;
                    }

                    //reset ref idx = ref indexes[len] to zero;


                    //borrow for the overflow starting at outer-1 to 0;
                    for (var start = len - 1; hasMore && start >= 0; start--)
                    {
                        idx = indexes[start]++;
                        if (idx < counts[start])
                        {
                            t = l[start][idx].Poly * m[idx];
                            mulcache[start] = t;
                            if (idx > 1)
                            {

                                result = mulcache[start - 1] + t;
                            }
                            // borrow accounted for
                            break;
                        }
                        else if (start > 0)
                        {
                            t = startmults[0];
                            // need to borrow more
                            indexes[start] = 0;
                        }
                        else // nothing left to borrow
                            hasMore = false;
                    }
                }

            }


            //Testmethod: TestJaggedRecursiveInline
            IEnumerable<int> EnumerateResults(int start, List<List<Congruence>> pList, long result)
            {
                var clist = pList[start];
                if (start == pList.Count - 1)
                {
                    for (var i = 0; i < clist.Count; i++)
                    {
                        long t = result + clist[i].Poly * m[start];
                        if (t > N) t %= N;

                        yield return (int)t;
                    }
                }
                else
                {
                    for (var i = 0; i < clist.Count; i++)
                    {
                        long t = result + clist[i].Poly * m[start];
                        if (t > N) t %= N;

                        foreach (var value in EnumerateResults(start + 1, pList, t))
                        {
                            yield return value;
                        }
                    }
                }
            }



        }

        public class ChineseRemainderTheoremSolver
        {
            private readonly int[] remainders;
            private readonly int[] modulos;

            public ChineseRemainderTheoremSolver(int[] remainders, int[] modulos)
            {
                if (remainders.Length != modulos.Length)
                {
                    throw new ArgumentException("The number of remainders must match the number of modulos.");
                }

                this.remainders = remainders;
                this.modulos = modulos;
            }

            public (int N, int Solution) Solve2Checked()
            {
                int len = modulos.Length;
                int[] nComplements = new int[len];
                int N = 1;
                double overFlowLog = 0;
                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    overFlowLog = MathLib.Log2(N) + MathLib.Log2(modulos[i]);
                    if (overFlowLog > 31)
                    {
                        var message = $"OverflowException: {nameof(N)} ({N}) * = {nameof(modulos)}[{i}] ({modulos[i]}) = {overFlowLog} > 31";
                        Console.WriteLine(message);
                        throw new OverflowException(message);
                    }
                    N *= modulos[i];
                }

                // Calculate nComplements
                for (int i = 0; i < len; i++)
                {
                    nComplements[i] = N / modulos[i];
                }

                int result = 0;

                for (int i = 0; i < len; i++)
                {

                    long mi = N / modulos[i];
                    long miInverse = ModularMultiplicativeInverse(mi, modulos[i]);

                    overFlowLog = MathLib.Log2(mi) + MathLib.Log2(miInverse);
                    if (overFlowLog > 63)
                    {
                        var message = $"OverflowException: {nameof(mi)} ({mi}) * = {nameof(miInverse)} ({miInverse}) = {overFlowLog} > 31";
                        Console.WriteLine(message);
                        throw new OverflowException(message);
                    }

                    overFlowLog += MathLib.Log2(remainders[i]);
                    if (overFlowLog > 63)
                    {
                        var message = $"OverflowException: result += {nameof(remainders)}[{i}] ({remainders[i]}) * {nameof(mi)} ({mi}) * {nameof(miInverse)} ({miInverse}) = {overFlowLog} > 31";
                        Console.WriteLine(message);
                        throw new OverflowException(message);
                    }

                    if (true)
                    {
                        var t = remainders[i] * mi;
                        if (t > N) t %= N;
                        t *= miInverse;
                        if (t > N) t %= N;
                        t += result;
                        if (t > N) t %= N;
                        result = (int)t;
                    }
                    else
                    {
                        GmpInt expected = (result + (GmpInt)remainders[i] * mi * miInverse) % N;
                        //result += remainders[i] * mi * miInverse;
                        //result %= N;

                        var t = remainders[i] * mi;
                        if (t > N) t %= N;
                        t *= miInverse;
                        if (t > N) t %= N;
                        t += result;
                        if (t > N) t %= N;

                        if (t != expected)
                        {
                            var message = $"OverflowException: result += {nameof(remainders)}[{i}] ({remainders[i]}) * {nameof(mi)} ({mi}) * {nameof(miInverse)} ({miInverse}) = {overFlowLog} > 31";
                            Console.WriteLine(message);
                            throw new OverflowException(message);
                        }
                        result = (int)t;
                    }

                }

                if (result < 0)
                {
                    result += N;
                }

                return (N, result);
            }

            public (int N, int Solution) Solve2()
            {
                int len = modulos.Length;

                long N = 1;

                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    N *= modulos[i];
                }
                if (N > int.MaxValue)
                    throw new OverflowException($"Solution N is larger than than int.MaxValue: {N.ToString("N0")}");

                long result = 0;
                for (int i = 0; i < len; i++)
                {

                    long mi = N / modulos[i];
                    long miInverse = ModularMultiplicativeInverse(mi, modulos[i]);

                    //result += remainders[i] * mi * miInverse;
                    //result %= N;

                    //same as above but work mod N to prevent overflows.
                    var t = remainders[i] * mi;
                    if (t > N) t %= N;
                    t *= miInverse;
                    if (t > N) t %= N;
                    t += result;
                    if (t > N) t %= N;
                    result = (int)t;
                }

                if (result < 0)
                {
                    result += N;
                }

                return ((int)N, (int)result);
            }



            public int Solve()
            {
                int len = modulos.Length;
                int[] nComplements = new int[len];
                int N = 1;

                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    N *= modulos[i];
                }

                // Calculate nComplements
                for (int i = 0; i < len; i++)
                {
                    nComplements[i] = N / modulos[i];
                }

                int result = 0;

                for (int i = 0; i < len; i++)
                {

                    int mi = N / modulos[i];
                    int miInverse = ModularMultiplicativeInverse(mi, modulos[i]);

                    result += remainders[i] * mi * miInverse;
                    result %= N;
                }

                if (result < 0)
                {
                    result += N;
                }

                return result;
            }

            public (GmpInt N, GmpInt Solution) SolveZ()
            {
                int len = modulos.Length;
                //GmpInt[] nComplements = new GmpInt[len];
                GmpInt N = 1;

                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {
                    N *= modulos[i];
                }

                // Calculate nComplements
                //for (int i = 0; i < len; i++)
                //{
                //    nComplements[i] = N / modulos[i];
                //}

                GmpInt result = 0;

                for (int i = 0; i < len; i++)
                {

                    GmpInt mi = N / modulos[i];
                    GmpInt miInverse = ModularMultiplicativeInverseZ(mi, modulos[i]);

                    result += remainders[i] * mi * miInverse;
                    result %= N;
                }

                if (result < 0)
                {
                    result += N;
                }

                return (N, result);
            }

            public (GmpInt N, GmpInt Solution) SolveZDebug()
            {
                int len = modulos.Length;
                GmpInt[] nComplements = new GmpInt[len];
                GmpInt N = 1;

                int Ni = 1;
                //int[] nComplementsi = new int[len];
                // Calculate N (the product of all modulos)
                // TODO: verify modulus are coprime.
                for (int i = 0; i < len; i++)
                {

                    N *= modulos[i];
                    var modulus = modulos[i];
                    var logN = MathLib.Log2((double)Ni);
                    var logMod = MathLib.Log2((double)modulus);
                    var testLog = logN + logMod;
                    if (testLog > 31)
                    {
                        throw new OverflowException($"{logN} + {logMod} = {testLog}");
                    }
                    Ni *= modulos[i];
                    if (N != Ni)
                    {
                        throw new OverflowException($"N != Ni");
                    }
                }

                // Calculate nComplements
                //for (int i = 0; i < len; i++)
                //{
                //    nComplements[i] = N / modulos[i];
                //    nComplementsi[i] = Ni / modulos[i];
                //    if (nComplements[i] != nComplementsi[i])
                //    {
                //        throw new Exception($"nComplements[i] != nComplementsi[i]");
                //    }
                //}

                GmpInt result = 0;
                long resulti = 0;

                for (int i = 0; i < len; i++)
                {

                    GmpInt mi = N / modulos[i];
                    long mii = (long)(N / modulos[i]);
                    if (mii != mi)
                    {
                        throw new Exception($"imi != miL: actual {mii} - expected {mi}");
                    }

                    GmpInt miInverse = ModularMultiplicativeInverseDebug(mi, modulos[i]);
                    long miInversei = ModularMultiplicativeInverse(mii, modulos[i]);
                    if (miInversei != miInverse)
                    {
                        throw new Exception("imiInverse != miInverse");
                    }

                    result += remainders[i] * mi * miInverse;
                    result %= N;

                    var remI = remainders[i];
                    var logRemI = MathLib.Log2(remI);

                    var logMii = MathLib.Log2(mii);
                    var logMiInversei = MathLib.Log2(miInversei);
                    if (logMii + logMiInversei > 63)
                    {
                        throw new OverflowException($"logMii + logMiInversei > 31  = {logMii + logMiInversei}");
                    }

                    if (logMii + logMiInversei + logRemI > 63)
                    {
                        throw new OverflowException($"logMii + logMiInversei + logRemI > 31  = {logMii + logMiInversei + logRemI}");
                    }

                    var t = remainders[i] * mii;
                    if (t > Ni) t %= Ni;
                    t *= miInversei;
                    if (t > Ni) t %= Ni;
                    t += resulti;
                    if (t > Ni) t %= Ni;
                    resulti = (int)(t % Ni);

                    if (result != resulti)
                    {
                        throw new Exception("result != resulti: remainders[i] * mi * miInverse");
                    }


                    if (result != resulti)
                    {
                        throw new Exception("result != resulti: result%=N");
                    }
                }

                if (result < 0)
                {
                    result += N;
                }

                return (N, result);
            }


            internal static int ModularMultiplicativeInverse(int a, int m)
            {
                int m0 = m;
                int y = 0, x = 1;

                if (m == 1)
                {
                    return 0;
                }

                while (a > 1)
                {
                    int q = a / m;
                    int t = m;

                    m = a % m;
                    a = t;
                    t = y;

                    y = x - q * y;
                    x = t;
                }

                if (x < 0)
                {
                    x += m0;
                }

                return x;
            }

            internal static long ModularMultiplicativeInverse(long a, long m)
            {
                long m0 = m;
                long y = 0, x = 1;

                if (m == 1)
                {
                    return 0;
                }

                while (a > 1)
                {
                    long q = a / m;
                    long t = m;

                    m = a % m;
                    a = t;
                    t = y;

                    y = x - q * y;
                    x = t;
                }

                if (x < 0)
                {
                    x += m0;
                }

                return x;
            }

            private GmpInt ModularMultiplicativeInverseZ(GmpInt a, GmpInt m)
            {
                GmpInt m0 = m;
                GmpInt y = 0, x = 1;

                if (m == 1)
                {
                    return 0;
                }

                while (a > 1)
                {
                    GmpInt q = a / m;
                    GmpInt t = m;

                    m = a % m;
                    a = t;
                    t = y;

                    y = x - q * y;
                    x = t;
                }

                if (x < 0)
                {
                    x += m0;
                }

                return x;
            }

            private GmpInt ModularMultiplicativeInverseDebug(GmpInt a, GmpInt m)
            {
                int m0i = (int)m;
                int yi = 0, xi = 1;
                int mi = (int)m;
                int ai = (int)a;


                GmpInt m0 = m;
                GmpInt y = 0, x = 1;

                if (m0i != m0)
                {
                    throw new Exception("m0i != m0");
                }

                if (mi != m)
                {
                    throw new Exception("mi != m");
                }

                if (m == 1)
                {
                    if (mi != 1)
                    {
                        throw new Exception("m != mi == 1");
                    }
                    return 0;
                }

                while (a > 1)
                {
                    if (ai != a)
                    {
                        throw new Exception("ai != a");
                    }
                    GmpInt q = a / m;
                    GmpInt t = m;

                    int qi = ai / mi;
                    int ti = (int)t;
                    if (qi != q)
                    {
                        throw new Exception("qi != q");
                    }


                    m = a % m;
                    mi = ai % mi;

                    if (m != mi)
                    {
                        throw new Exception("mi != m");
                    }


                    a = t;
                    ai = ti;
                    if (a != ai)
                    {
                        throw new Exception("ai != a");
                    }


                    t = y;
                    ti = yi;
                    if (t != ti)
                    {
                        throw new Exception("ti != t");
                    }



                    y = x - q * y;
                    yi = xi - qi * yi;
                    if (y != yi)
                    {
                        throw new Exception("yi != y");
                    }

                    x = t;
                    xi = ti;
                    if (xi != x)
                    {
                        throw new Exception("xi != x");
                    }

                }

                if (x < 0)
                {
                    x += m0;
                    xi += m0i;

                    if (xi != x)
                    {
                        throw new Exception("xi != x");
                    }
                }

                if (xi != x)
                {
                    if (xi != x)
                    {
                        throw new Exception("xi != x");
                    }
                }
                return x;
            }


            private int[] CalculateBezoutsIdentity(int a, int b)
            {
                int[] bezoutsIdentity = new int[2];
                int a0 = a;
                int b0 = b;
                int x0 = 0;
                int x1 = 1;

                if (b0 == 1)
                {
                    bezoutsIdentity[0] = 1;
                    bezoutsIdentity[1] = 0;
                    return bezoutsIdentity;
                }

                while (b0 > 1)
                {
                    int q = a0 / b0;
                    int temp = b0;

                    b0 = a0 % b0;
                    a0 = temp;

                    temp = x0;
                    x0 = x1 - q * x0;
                    x1 = temp;
                }

                if (x1 < 0)
                {
                    x1 += a;
                }

                bezoutsIdentity[0] = x1;
                bezoutsIdentity[1] = (a - x1 * b) / b;

                return bezoutsIdentity;
            }
        }
    }
}