using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Reports;
using HigginsSoft.Math.Lib;
using System.Runtime.Intrinsics.X86;

namespace HigginsSoft.Math.Demos.Tests
{
    [TestClass()]
    public class GfMatrix32Tests
    {

        GfMatrix32 testMatrix1()
        {
            var d = new Dictionary<int, int>
            {
                { 0, 1 }
                , { 1, 2 }
                , { 2, 3 }
                , { 3, 4 }
            };
            var m = new GfMatrix32(d);
            return m;
        }

        public class PopComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {

                if (x == y) return 0;
                var popx = MathLib.PopCount(x);
                var popy = MathLib.PopCount(y);
                if (popx > popy) return 1; // higer popcount is higher
                if (popx < popy) return -1;// lower popcount is lower

                // see how has hights bit set.
                var xor = x ^ y;
                var xand = x & xor;
                var yand = y & xor;

                /* // Bitwise operation above is short hand for the following:
                 * var mask = 1 << 31;

                var a = x & mask;
                var b = y & mask;
                while (a == b)
                {
                    mask >>= 1;
                    a = x & mask;
                    b = y & mask;
                }

                return = (a > b) ? -1 : (a < b) ? 1 : 0;// (x > y) ? 1 : -1;
                 
                 */

                //highest bit set is lower,
                return xand > yand ? -1 : (xand < yand) ? 1 : 0;

               
            }
        }
        [TestMethod]
        public void BitOrderByFunc()
        {

            var popComp = new PopComparer();

            var res = popComp.Compare(0, 1);
            //lower popcount is lower
            Assert.IsTrue(res == -1);
            //same popcount, higher number is lower
            res = popComp.Compare(1, 2);
            Assert.IsTrue(res == 1);// 
            res = popComp.Compare(29, 30);
            //same popcount, highest set bit is higher

            var maxBitLength = 5;
            var range = 1 << maxBitLength;
            var a = Enumerable.Range(0, range);
            var numbers = a.OrderBy(x => x, popComp).ToList();

            foreach (var number in numbers)
            {
                var pop = MathLib.PopCount(number);
                var bin = Convert.ToString(number, 2).PadLeft(maxBitLength, 'x');
                Console.WriteLine($"{bin} - {pop} dependencies <= {number}");
            }
        }

        [TestMethod]
        public void BitEnumeratorTest()
        {
            IEnumerable<int> loop(int i)
            {
                if (i == 0)
                {
                    yield return 0;
                    yield return 1;
                }
                else
                {
                    var mask = 1 << i;
                    foreach (var value in loop(i - 1))
                    {
                        yield return mask | value;
                    }
                }
            }


            IEnumerable<int> EnumerateBitLength(int maxBitLength)
            {
                for (var i = 0; i <= maxBitLength; i++)
                {
                    foreach (var value in loop(i))
                        yield return value;
                }

            }

            int maxBitLength = 4;
            void printEnumerate(int i)
            {
                Console.WriteLine($"Loop: {i}");
                var numbers = EnumerateBitLength(i);
                foreach (var number in numbers)
                {
                    Console.WriteLine(Convert.ToString(number, 2).PadLeft(maxBitLength, '0'));
                }
            }

            for (var i = 0; i < maxBitLength; i++)
            {
                printEnumerate(i);
            }


        }

        [TestMethod]
        public void SolutionSearchPatternTest()
        {

            void GenerateSolutions(List<int> rows)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    List<int> currentSolution = new List<int> { rows[i] };
                    GenerateSolutionsRecursive(rows, i + 1, currentSolution);
                }
            }

            void GenerateSolutionsRecursive(List<int> rows, int startIndex, List<int> currentSolution)
            {
                PrintSolution(currentSolution);

                for (int i = startIndex; i < rows.Count; i++)
                {
                    currentSolution.Add(rows[i]);
                    GenerateSolutionsRecursive(rows, i + 1, currentSolution);
                    currentSolution.RemoveAt(currentSolution.Count - 1);
                }
            }
            int solutionCount = 0;
            void PrintSolution(List<int> solution)
            {
                solutionCount++;
                Console.WriteLine("Solution: " + string.Join(", ", solution));
            }

            var rows = new List<int> { 4, 3, 2, 1 };

            GenerateSolutions(rows);
            Assert.AreEqual(15, solutionCount);
        }

        
        [TestMethod()]
        public void GfMatrix32GetSolutionsTest()
        {
            GfMatrix32 m;


            m = testMatrix1();
            var solutions = m.GetSolutions();
            var count = 0;
            foreach (var sol in solutions)
            {
                var mask = sol.Mask;
                Assert.AreEqual(0, mask);
                count++;
            }

            Assert.AreEqual(1, count);

            var newRows = m.Rows
                .Select(x => new GfMatrix32Row(m, x.Value + 4, x.Mask))
                .OrderByDescending(x=> x.Value)
                .ToList();
            m.Rows.AddRange(newRows);
            solutions = m.GetSolutions(false).ToList();
            count = 0;
            foreach (var sol in solutions)
            {
                count++;
                var mask = sol.Mask;
                Assert.AreEqual(0, mask);
                var rows = sol.GetSourceRows().Select(x => $"({x.Value} - {Convert.ToString(x.Mask, 2).PadLeft(3, '0')})").ToList();

                Console.WriteLine($"Solution {count}: {string.Join(", ", rows)}");

            }

            Assert.AreEqual(13, count);




        }

        [TestMethod()]
        public void GfMatrix32SolutionRowTest()
        {
            GfMatrix32 m;
            GfMatrix32Row left, right;
            GfMatrix32SolutionRow sol;

            m = testMatrix1();

            left = m.Rows[0];
            right = m.Rows[1];
            Assert.AreEqual(4, left.Mask);
            Assert.AreEqual(3, right.Mask);
            sol = new GfMatrix32SolutionRow(m, left, right);
            Assert.AreEqual(7, sol.Mask);


            left = m.Rows[1];
            right = m.Rows[2];
            Assert.AreEqual(3, left.Mask);
            Assert.AreEqual(2, right.Mask);
            sol = new GfMatrix32SolutionRow(m, left, right);
            Assert.AreEqual(1, sol.Mask);

            left = m.Rows[2];
            right = m.Rows[3];
            Assert.AreEqual(2, left.Mask);
            Assert.AreEqual(1, right.Mask);
            sol = new GfMatrix32SolutionRow(m, left, right);
            Assert.AreEqual(3, sol.Mask);

            sol = new GfMatrix32SolutionRow(m, sol, sol);
            Assert.AreEqual(0, sol.Mask);

            sol = new GfMatrix32SolutionRow(m, m.Rows[0], m.Rows[0]);
            Assert.AreEqual(0, sol.Mask);

            sol = new GfMatrix32SolutionRow(m, m.Rows[1], m.Rows[1]);
            Assert.AreEqual(0, sol.Mask);

            sol = new GfMatrix32SolutionRow(m, m.Rows[3], m.Rows[3]);
            Assert.AreEqual(0, sol.Mask);
        }

        [TestMethod()]
        public void GfMatrix32SortTest()
        {
            var m = testMatrix1();
            Assert.AreEqual(4, m.Rows.Count);
            Assert.AreEqual(4, m.Rows[0].Mask);
            Assert.AreEqual(3, m.Rows[1].Mask);
            Assert.AreEqual(2, m.Rows[2].Mask);
            Assert.AreEqual(1, m.Rows[3].Mask);
        }

        [TestMethod()]
        public void GfMatrix32GetSourceRowsTest()
        {
            GfMatrix32 m;
            GfMatrix32Row left, right;
            GfMatrix32SolutionRow sol, sol1, sol2;

            m = testMatrix1();
            left = m.Rows[0];
            right = m.Rows[1];
            sol1 = new GfMatrix32SolutionRow(m, left, right);

            left = m.Rows[2];
            right = m.Rows[3];
            sol2 = new GfMatrix32SolutionRow(m, left, right);

            sol = new GfMatrix32SolutionRow(m, sol1, sol2);

            var rows = sol.GetSourceRows().ToList();
            Assert.AreEqual(4, rows.Count);

        }

        [TestMethod()]
        public void ReduceTest()
        {
            //Assert.Fail();
            var d = new Dictionary<int, int>
            {
                { 0, 1 }
                , { 1, 2 }
                , { 3, 3 }
                , { 4, 4 }
            };
            var m = new GfMatrix32(d);
            Assert.AreEqual(3, m.BitLength, $"Matrix should have bitlength for 3 for mask 4 (100)");
            var r = m.Reduce();
            Assert.AreEqual(3, r.Rows.Count, "Reduced matrix should have 3 rows after removing 4");
            Assert.AreEqual(2, r.BitLength, "Reduced matrix should have bitlength for 2 for mask 3(10)");
        }

        [TestMethod()]
        public void GfMatrix()
        {

            var seed = new Dictionary<int, int>
            {
                { 0, 1 }
                , { 1, 2 }
                , { 3, 3 }
                , { 4, 4 }
            };
            var factorbase = new[] { 2, 3, 7, 13 };

            var facts = seed.ToDictionary(x => x, x =>
            {
                var fact = new FactorizationInt();
                var res = x.Value;
                for (var j = 0; j < 4; j++)
                {
                    if (1 == (res & (1 << j)))
                    {
                        fact.Add(factorbase[j], 1 + 2 * j);
                    }
                }
                return fact;
            });

            // convert to matrix column array

            //convert factors to 
            /*
                dict<int:prime, MatrixColumn(List<Factorization>, PopCount, Removed, PopCountGf2)
             */


        }

        public class MatrixRow
        {
            public bool Removed;

            // list of colums with dependence on this row.
            //public List<MatrixColumn> Columns = new();
            public FactorizationInt Factorization;

            // cache of powers for each prime.
            private Dictionary<int, int> Data;
            public MatrixRow(FactorizationInt f)
            {
                this.Factorization = f;
                this.Data = f.Factors.ToDictionary(x => x.P, x => x.Power);
            }

            public int Count(int prime) => Data.ContainsKey(prime) ? Data[prime] : 0;

            //public void AddColumn(MatrixColumn matrixColumn)
            //{
            //    this.Columns.Add(matrixColumn);
            //}
        }
        public class MatrixColumn
        {
            public List<MatrixRow> Facts;
            public int Prime;

            // this needs to calculate Sum(facts.Where(x=> x.Remove).Power);
            public int PopCount => Facts.Where(x => !x.Removed).Sum(x => x.Count(Prime));
            public int PopCountGf2 => PopCount & 1;
            public bool Removed;
            public MatrixColumn(List<FactorizationInt> facts, int prime)
            {
                Prime = prime;
                Facts = facts.Select(x => new MatrixRow(x)).ToList();
                //Facts.ForEach(x => x.AddColumn(this));
            }

        }
    }
}