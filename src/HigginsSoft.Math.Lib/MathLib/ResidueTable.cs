using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    public partial class s
    {
        public class Table
        {
            //public Dictionary<int, Dictionary<int, int>> Table { get; }
            //public int[][] Table { get; }
            public int C;
            public Table(int C)
            {
                this.C = C;
                //var d = new Dictionary<int, Dictionary<int, int>>();
                //int[][] d = new int[C][];
                //for (var i = 0; i < C; i++)
                //{
                //    d[i] = new int[C];// new Dictionary<int, int>();
                //    for (var k = 0; k < C; k++)
                //    {
                //        d[i][k] = i * k % C;
                //    }
                //}
                //this.Table = d;
            }



            public IEnumerable<(int P, int Q, int)> GetPairsViaMod(int n)
            {
                for (var i = 0; i < C; i++)
                {
                    for (var k = 0; k < C; k++)
                    {
                        if (i * k % C == n)
                            yield return (i, k, n);
                    }
                }
            }
            public IEnumerable<(int P, int Q, int)> GetPairs(int n)
            {
                int t;
                for (var i = 1; i < C; i++)
                {
                    t = 0;
                    for (var k = 1; k < C; k++)
                    {
                        t += i;
                        if (t > C) t -= C;
                        if (t == n)
                            yield return (i, k, n);
                    }
                }
            }

            public static Table s10 => new Table(10);
            public static Table s29 => new Table(29);
            public static Table s30 => new Table(30);
            public static Table s31 => new Table(31);
            public static Table s36 => new Table(36);


            public static int[] PrecomputedTableClasses = { 10, 29, 30, 31, 36 };
            public static Dictionary<int, Table> Precomputed => new Dictionary<int, Table>()
            {
                { 10, s10 },
                { 29, s29 },
                { 30, s30 },
                { 31, s31 },
                { 36, s36 }
            };
        }
    }
}
