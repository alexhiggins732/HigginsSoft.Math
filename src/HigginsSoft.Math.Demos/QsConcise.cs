/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/
#define LOG
#undef LOG

using HigginsSoft.Math.Lib;
using PEFile;
using System;
using static HigginsSoft.Math.Demos.Qs16;

namespace HigginsSoft.Math.Demos
{
    public class QsConcise
    {
        readonly int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19 };
        public FactorizationInt FactorUnchecked(int value)
        {

            FactorizationInt result = new();

            int nt = value;
            if (MathLib.IsPerfectSquare(value, out int root))
            {
                var froot = FactorizationInt.FactorTrialDivide(root);
                result.Add(froot);
                result.Add(froot);
                return result;
            }
            var l = Enumerable.Range(root + 1, 1000)
                .Select(i => new { i, sq = i * i })
                .Select(x => new { x.i, x.sq, res = x.sq % value })
                .Select(x =>
                {
                    var fmask = 0;
                    return new
                    {
                        x.i,
                        x.sq,
                        x.res,
                        bsmooth = (nt = x.res) + (fmask = 0) == 0 || 1 <= primes.Aggregate(1, (m, p) =>
                            {
                                while (nt > 1 && nt % p == 0)
                                {
                                    nt /= p;
                                    fmask = fmask ^ m;
                                }
                                m <<= 1;
                                return m;
                            }) && nt < 2,
                        mask = fmask,
                    };
                })
                .Where(x => x != null && x.bsmooth)
                //.OrderBy(x => x.mask)
                .ToList();


            IEnumerable<(Func<GmpInt>, Func<GmpInt>)>
                recurse(int start, int mask, Func<GmpInt> getProduct, Func<GmpInt> getRes)
            {
                /* depth first search can be done in one loop*/
                if (mask == 0)
                {
                    var product = getProduct();
                    var res = getRes();
                    if (MathLib.IsPerfectSquare(res, out GmpInt fact)
                        || MathLib.IsPerfectSquare(product, out fact)
                        || (fact = MathUtil.Gcd(value, product)) > 1 && fact != value)
                        yield return (() => product,
                                      () => fact);
                }
                else
                    for (int i = start; i < l.Count; i++)
                    {
                        int nextMask;
                        var f = l[i];
                        if ((mask & f.mask) > 0)
                        {
                            if ((nextMask = (mask ^ f.mask)) == 0)
                                yield return (() => getProduct() * f.i,
                                    () => getRes() * f.res);
                            else
                                foreach (var result in recurse(
                                       i + 1,
                                       nextMask,
                                       () => getProduct() * f.i,
                                       () => getRes() * f.res)
                                    )
                                    yield return result;
                        }
                    }
            }




            var cmp = new PopComparer();
            var tmp = l.ToList();
            l = tmp.Take(20).OrderBy(x => x.mask, cmp).ToList();
            int i = 0;
            var f = l.Find(x =>
            {
                var solved = false;
                var solutions = recurse(i + 1, x.mask, () => x.i, () => x.res);
                foreach (var solution in solutions)
                {
                    var product = solution.Item1();
                    var res = solution.Item2();
                    if (GcdCheck(value, product + res, product - res, result))
                    {
                        solved = true;
                        break;
                    }
                }
                i++;
                return solved;
            });
            if (result.GetProduct() != value)
            {
                string bp = "";
            }
            return result;
        }




        public bool GcdCheck(int n, GmpInt p, GmpInt q, FactorizationInt result)
        {
            bool ret;
            var gcd = MathUtil.Gcd(n, p);
            if (gcd < 2 || gcd == n)
            {
                gcd = MathUtil.Gcd(n, q);
            }
            if (ret = (gcd > 1 && gcd != n))
            {
                result.Add(FactorizationInt.FactorTrialDivide((int)gcd));
                result.Add(FactorizationInt.FactorTrialDivide((int)(n / gcd)));

            }
            return ret;
        }
    }
}