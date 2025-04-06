/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Diagnostics;
using System.Numerics;

namespace HigginsSoft.Math.Lib
{
    public partial class FactorizationBigInteger : IDisposable
    {
        public int num_factors => this.Factors.Count;

  
        public List<Factor<BigInteger>> Factors = new();

        public Stopwatch? TDivWatch;
        public Stopwatch? FermatWatch;
        public Stopwatch? RhoWatch;
        public Stopwatch? RhoP2Watch;
        public Stopwatch? RhoP3Watch;
        public string? FoundBy;
        private bool disposedValue;

        public string Timings
        {
            get
            {
                var tdiv = TDivWatch is null ? TimeSpan.Zero : TDivWatch.Elapsed;
                var fermat = FermatWatch is null ? TimeSpan.Zero : FermatWatch.Elapsed;
                var rho = RhoWatch is null ? TimeSpan.Zero : RhoWatch.Elapsed;
                var rho2 = RhoP2Watch is null ? TimeSpan.Zero : RhoP2Watch.Elapsed;
                var rho3 = RhoP3Watch is null ? TimeSpan.Zero : RhoP3Watch.Elapsed;
                var by = FoundBy is null ? "" : $"({FoundBy}) ";
                var result = $"{by}Tdiv: {tdiv} - Fermat: {fermat} - Rho: {rho}  - Rho(x^2+2): {rho2} - Rho(x^2+3): {rho3}";
                return result;
            }
        }



        public static FactorizationBigInteger Factor(BigInteger n, bool checkPrimality = true)
        {
            var sw = Stopwatch.StartNew();
            var res = FactorTrialDivide(n, checkPrimality);
            sw.Stop();
            res.TDivWatch = sw;

            if (res.Factors.Count > 1)
            {
                res.FoundBy = "TDiv";
            }
            if (res.Factors.Count == 1)
            {
                res.Clear();
                res.RhoWatch = new();
                res.FermatWatch = new();
                res.RhoP2Watch = new();
                res.RhoP3Watch = new();

                res.FermatWatch.Start();
                var resumable = Fermat.StartResumable(n);
                res.FermatWatch.Stop();

                bool factored = resumable.HasFactor;
                int maxIterations = (int)resumable.Iterations;
                while (!factored)
                {

                    res.FermatWatch.Start();
                    factored = Fermat.Resume(resumable, (int)(resumable.Iterations + maxIterations));
                    res.FermatWatch.Stop();
                    if (factored)
                    {
                        res.Add(resumable.P, 1);
                        res.Add(resumable.Q, 1);
                        res.FoundBy = nameof(Fermat);
                    }
                    else
                    {
                        res.RhoWatch.Start();
                        var Rho = MathLib.PollardRhoC(n, 1);
                        res.RhoWatch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = nameof(Rho);
                            break;
                        }

                        res.RhoP2Watch.Start();
                        Rho = MathLib.PollardRhoC(n, 2);
                        res.RhoP2Watch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = nameof(Rho) + "+2";
                            break;
                        }

                        res.RhoP3Watch.Start();
                        Rho = MathLib.PollardRhoC(n, 3);
                        res.RhoP3Watch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = nameof(Rho) + "+3";
                            break;
                        }
                        Rho = MathLib.PollardRhoZOld(n);
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = nameof(Rho) + "+3";
                            break;
                        }
                    }
                }

            }
            return res;
        }

        public void Clear()
        {
            //Factors.ForEach(x => x);
            Factors.Clear();
        }

        public static FactorizationBigInteger FactorTrialDivide(int n)
        {
            var result = new FactorizationBigInteger();
            if (n < 4 || MathLib.IsPrime(n))
            {
                result.Add(n, 1);
                return result;
            }

            var root = (int)MathLib.Sqrt(n);
            var primes = Primes.IntFactorPrimes;
            int prime;
            for (var j = 0; j < primes.Length && (prime = primes[j]) <= root; j++)
            {
                int count = 0;
                while (n > 0 && n % prime == 0)
                {
                    count++;
                    n = n / prime;
                }
                if (count > 0)
                    result.Add(prime, count);
                if (n == 0)
                    break;
            }
            if (n > 1)
                result.Add(n, 1);
            return result;
        }


        public static FactorizationBigInteger FactorTrialDivide(BigInteger n, int limit, IEnumerable<int> primes)
        {
            var result = new FactorizationBigInteger();
            if (n < 4 || MathLib.IsPrime(n))
            {
                result.Add(n, 1);
                return result;
            }

            foreach (var p in primes)
            {
                if (p > limit)
                    break;
                int count = 0;
                while (n > 0 && n % p == 0)
                {
                    count++;
                    n = n / p;
                }
                if (count > 0)
                    result.Add(p, count);
                if (n == 0)
                    break;
            }

            if (n > 1)
                result.Add(n, 1);
            return result;
        }

        public static FactorizationBigInteger FactorTrialDivide(BigInteger n, int limit)
        {
            var result = new FactorizationBigInteger();
            if (n < 4 || MathLib.IsPrime(n))
            {
                result.Add(n, 1);
                return result;
            }


            var primes = Primes.IntFactorPrimes;
            int prime;
            for (var j = 0; j < primes.Length && (prime = primes[j]) <= limit; j++)
            {
                int count = 0;
                while (n > 0 && n % prime == 0)
                {
                    count++;
                    n = n / prime;
                }
                if (count > 0)
                    result.Add(prime, count);
                if (n == 0)
                    break;
            }
            if (n > 1)
                result.Add(n, 1);
            return result;
        }

        public static FactorizationBigInteger FactorIntTrialDivide1(int n)
        {
            var result = new FactorizationBigInteger();
            if (n < 4 || MathLib.IsPrime(n))
            {
                result.Add(n, 1);
                return result;
            }

            var root = (int)MathLib.Sqrt(n);
            var primes = Primes.IntFactorPrimes;
            int prime;
            for (var j = 0; j < primes.Length && (prime = primes[j]) <= root; j++)
            {
                int count = 0;
                while (n > 0 && n % prime == 0)
                {
                    count++;
                    n = n / prime;
                }
                if (count > 0)
                    result.Add(prime, count);
                if (n == 0)
                    break;
            }
            if (n > 1)
                result.Add(n, 1);
            return result;
        }

        public static FactorizationBigInteger FactorTrialDivideUnchecked(BigInteger n, BigInteger root, FactorizationBigInteger result)
        {
            var primes = Primes.IntFactorPrimes;

            int prime;
            for (var j = 0; j < primes.Length && (prime = primes[j]) <= root; j++)
            {
                int count = 0;
                while (n > 0 && n % prime == 0)
                {
                    count++;
                    n = n / prime;
                }
                if (count > 0)
                    result.Add(prime, count);
                if (n == 0)
                    break;
            }
            if (n > 1)
                result.Add((int)n, 1);
            return result;
        }

        public static FactorizationBigInteger FactorTrialDivideUnchecked(BigInteger n)
        {
            var result = new FactorizationBigInteger();

            if (MathLib.IsPerfectSquare(n, out BigInteger root))
            {
                result.Add(root, 2);
                return result;
            }
            return FactorTrialDivideUnchecked(n, root, result);

        }

        public static FactorizationBigInteger FactorTrialDivide(BigInteger n, bool checkPrimality = true)
        {
            var result = new FactorizationBigInteger();
            if (n < 4 || checkPrimality && MathLib.IsPrime(n))
            {
                result.Add((int)n, 1);
                return result;
            }
            if (MathLib.IsPerfectSquare(n, out BigInteger root))
            {
                result.Add(root, 2);
                return result;
            }

            return FactorTrialDivideUnchecked(n, root, result);

        }

        public void Add(BigInteger root, int count)
        {
            Factors.Add(new(root, count));
        }
        public void Add(FactorizationBigInteger f)
        {
            foreach (var otherFactor in f.Factors)
            {
                var thisFactor = Factors.FirstOrDefault(x => x.P == otherFactor.P);
                if (thisFactor != null)
                    thisFactor.Power += otherFactor.Power;
                else
                    Factors.Add(new Factor<BigInteger>(otherFactor));
            }
            Factors.Sort();
        }

        public string FactorizationBigIntegerString()
        {
            var product = Factors.Aggregate(BigInteger.One, (a, b) => a * b.GetValue());
            var equation = EquationString();
            return $"{product} = {equation}";
        }

        public string EquationString()
        {
            var equation = string.Join(" * ",
                Factors.Select(x => x.FactorizationString()));
            return equation;
        }


        public override string ToString()
        {
            return FactorizationBigIntegerString();
        }

        public BigInteger GetProduct()
        {
            var product = BigInteger.One;

            for (var i = 0; i < Factors.Count; i++)
                product *= Factors[i].GetValue();

            return product;
        }

        public bool IsPerfectSquare()
            => Factors.All(x => (x.Power & 1) == 0);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FactorizationBigInteger()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}