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
using static HigginsSoft.Math.Lib.Fermat;

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
        public Stopwatch? RhoZWatch;
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
                var rhoZ = RhoZWatch is null ? TimeSpan.Zero : RhoZWatch.Elapsed;
                var by = FoundBy is null ? "" : $"({FoundBy}) ";
                var result = $"{by}Tdiv: {tdiv} - Fermat: {fermat} - Rho: {rho}  - Rho(x^2+2): {rho2} - Rho(x^2+3): {rho3} - Rho(Z) = {rhoZ}";
                return result;
            }
        }


        public class FactorizationMethod
        {
            public const string Fermat = nameof(Fermat);
            public const string Rho = nameof(Rho);
            public const string RhoP2 = nameof(Rho) + "+2";
            public const string RhoP3 = nameof(Rho) + "+3";
            public const string RhoZ = nameof(Rho) + "+Z";
            public const string PP1 = nameof(PP1);
            public const string PM1 = nameof(PM1);
            public const string ECM = nameof(ECM);
            public const string QS = nameof(QS);
            public const string TDiv = nameof(TDiv);
        }

        public class FactorConfig
        {
            public bool checkPrimality = true;
            public bool skipTrialDivide = true;
            public bool skipFermat = true;
            public bool skipRho = true;
            public bool skipRhoP2 = true;
            public bool skipRhoP3 = true;
            public bool skipRhoZ = true;
            public bool skipPP1 = true;
            public bool skipPM1 = true;
            public bool skipECM = true;
            public bool skipQS = true;
            public int? B1 = null;
            public int? B2 = null;
            public int? Curves = null;
            public int? Digits = null;
        }

        static FactorConfig? commandLineConfig = null;
        static FactorConfig GetCommandLineConfig()
        {
            if (commandLineConfig is null)
            {
                commandLineConfig = new FactorConfig();
                var args = Environment.GetCommandLineArgs().Select(x => x.Trim().ToLower()).ToList();

                int idx = -1;
                if (args.Contains("digits"))
                {
                    idx = args.IndexOf("digits");
                    if (idx < args.Count - 1 && int.TryParse(args[idx + 1], out var digits))
                    {
                        commandLineConfig.Digits = digits;
                    }
                }

                foreach (var arg in args)
                {
                    bool missed = true;
                    if (arg.Equals("fermat", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipFermat = missed = false;
                    if (arg.Equals("rho", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipRho = missed = false;
                    if (arg.Equals("rhop2", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipRhoP2 = missed = false;
                    if (arg.Equals("rhop3", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipRhoP3 = missed = false;
                    if (arg.Equals("rhoz", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipRhoZ = missed = false;
                    if (arg.Equals("pp1", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipPP1 = missed = false;
                    if (arg.Equals("pm1", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipPM1 = missed = false;
                    if (arg.Equals("ecm", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipECM = missed = false;
                    if (arg.Equals("qs", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipQS = missed = false;
                    if (arg.Equals("tdiv", StringComparison.CurrentCultureIgnoreCase))
                        commandLineConfig.skipTrialDivide = missed = false;

                    if (!missed && arg == "ecm" || arg == "pp1" || arg == "pm1")
                    {
                        var allArgs = new[] { "fermat", "rho", "rhop2", "rhop3", "rhoz", "pp1", "pm1", "ecm", "qs", "tdiv" }.ToList();
                        var hit = args.First(x => allArgs.Contains(x, StringComparer.CurrentCultureIgnoreCase));
                        idx = args.IndexOf(hit);

                        if (idx < args.Count - 3 && arg == "ecm"
                            && int.TryParse(args[idx + 1], out int b1)
                            && int.TryParse(args[idx + 2], out int b2)
                            && int.TryParse(args[idx + 3], out int curves)
                            )
                        {
                            commandLineConfig.B1 = b1;
                            commandLineConfig.B2 = b2;
                            commandLineConfig.Curves = curves;
                        }

                        // test for b1 and b2 following the arg
                        else if (idx < args.Count - 2 && int.TryParse(args[idx + 1], out b1) && int.TryParse(args[idx + 2], out b2))
                        {
                            commandLineConfig.B1 = b1;
                            commandLineConfig.B2 = b2;
                        }
                        else if (idx < args.Count - 1 && int.TryParse(args[idx + 1], out b1))
                        {
                            commandLineConfig.B1 = b1;

                        }

                    }
                }
            }
            return commandLineConfig;
        }

        static NumericsEcm ecm = new();
        public static FactorizationBigInteger Factor(BigInteger n,
            bool checkPrimality = true,
            bool skipTrialDivide = true,
            bool skipFermat = false,
            bool skipRho = false,
            bool skipRhoP2 = false,
            bool skipRhoP3 = false,
            bool skipRhoZ = false,
            bool skipPP1 = false,
            bool skipPM1 = false,
            bool skipECM = false,
            bool skipQS = false,
            int autoSiqsLimit = 50)
        {
            var sw = Stopwatch.StartNew();

            FactorizationBigInteger res;
            if (skipTrialDivide)
            {
                res = new FactorizationBigInteger(); res.Add(n, 1);
            }
            else
            {
                res = FactorTrialDivide(n, checkPrimality);
            }
            sw.Stop();
            res.TDivWatch = sw;

            if (res.Factors.Count > 1)
            {
                res.FoundBy = FactorizationMethod.TDiv;
            }
            if (res.Factors.Count == 1)
            {
                res.Clear();
                res.RhoWatch = new();
                res.FermatWatch = new();
                res.RhoP2Watch = new();
                res.RhoP3Watch = new();
                res.RhoZWatch = new();

                FactorizationState<BigInteger>? resumable = null;
                bool factored = false;

                int maxIterations = 10000;
                if (!skipFermat)
                {
                    res.FermatWatch.Start();
                    resumable = Fermat.StartResumable(n, maxIterations);
                    res.FermatWatch.Stop();

                    factored = resumable.HasFactor;

                }


                int maxRuns = 1;
                int run = 0;

                var config = GetCommandLineConfig();

                Func<bool> factoredFermat = () =>
                {
                    if (resumable == null)
                        return false;
                    res.FermatWatch.Start();
                    factored = Fermat.Resume(resumable, 10000);
                    res.FermatWatch.Stop();
                    if (factored)
                    {
                        res.Add(resumable.P, 1);
                        res.Add(resumable.Q, 1);
                        res.FoundBy = FactorizationMethod.Fermat;
                    }
                    return factored;
                };

                Func<bool> factoredRho = () =>
                {
                    res.RhoWatch.Start();
                    var Rho = MathLib.PollardRhoC(n, 1, 1000);
                    res.RhoWatch.Stop();
                    if (Rho != n)
                    {
                        res.Add(Rho, 1);
                        res.Add(n / Rho, 1);
                        factored = true;
                        res.FoundBy = FactorizationMethod.Rho;
                    }
                    return factored;
                };

                Func<bool> factoredRhoP2 = () =>
                {
                    res.RhoP2Watch.Start();
                    var Rho = MathLib.PollardRhoC(n, 2, 1000);
                    res.RhoP2Watch.Stop();
                    if (Rho != n)
                    {
                        res.Add(Rho, 1);
                        res.Add(n / Rho, 1);
                        factored = true;
                        res.FoundBy = FactorizationMethod.RhoP2;
                    }
                    return factored;
                };

                Func<bool> factoredRhoP3 = () =>
                {
                    res.RhoP3Watch.Start();
                    var Rho = MathLib.PollardRhoC(n, 3, 1000);
                    res.RhoP3Watch.Stop();
                    if (Rho != n)
                    {
                        res.Add(Rho, 1);
                        res.Add(n / Rho, 1);
                        factored = true;
                        res.FoundBy = FactorizationMethod.RhoP3;
                    }
                    return factored;
                };

                Func<bool> factoredRhoZ = () =>
                {
                    res.RhoZWatch.Start();
                    var Rho = MathLib.PollardRhoZOld(n, 200000);
                    res.RhoZWatch.Stop();
                    if (Rho != n)
                    {
                        res.Add(Rho, 1);
                        res.Add(n / Rho, 1);
                        factored = true;
                        res.FoundBy = FactorizationMethod.RhoZ;
                    }
                    return factored;
                };

                Func<bool> factoredPP1 = () =>
                {
                    using var pp1 = ecm.PP1(
                        n,
                        targetDigits: config.Digits,
                        B1: config.B1,
                        B2: config.B2
                    );

                    if (pp1.Factors.Count > 1)
                    {
                        res.Add(pp1);
                        res.FoundBy = FactorizationMethod.PP1;
                        factored = true;
                    }
                    return factored;
                };

                Func<bool> factoredPM1 = () =>
                {
                    using var pp1 = ecm.PM1(
                        n,
                        targetDigits: config.Digits,
                        B1: config.B1,
                        B2: config.B2
                    );

                    if (pp1.Factors.Count > 1)
                    {
                        res.Add(pp1);
                        res.FoundBy = FactorizationMethod.PM1;
                        factored = true;
                    }
                    return factored;
                };

                Func<bool> factoredECM = () =>
                {
                    using var pp1 = ecm.ECM(
                        n,
                        targetDigits: config.Digits,
                        B1: config.B1,
                        B2: config.B2,
                        curves: config.Curves
                    );

                    if (pp1.Factors.Count > 1)
                    {
                        res.Add(pp1);
                        res.FoundBy = FactorizationMethod.ECM;
                        factored = true;
                    }
                    return factored;
                };

                Func<bool> factoredQS = () =>
                {
                    using var qs = new NumericsYafu().QS(n);
                    if (qs.Factors.Count > 1)
                    {
                        res.Add(qs);
                        res.FoundBy = FactorizationMethod.QS;
                        factored = true;
                    }
                    return factored;
                };



                List<Func<bool>> factoredMethods = new();
                if (!skipFermat || !config.skipFermat)
                    factoredMethods.Add(factoredFermat);
                if (!skipRho || !config.skipRho)
                    factoredMethods.Add(factoredRho);
                if (!skipRhoP2 || !config.skipRhoP2)
                    factoredMethods.Add(factoredRhoP2);
                if (!skipRhoP3 || !config.skipRhoP3)
                    factoredMethods.Add(factoredRhoP3);
                if (!skipRhoZ || !config.skipRhoZ)
                    factoredMethods.Add(factoredRhoZ);
                if (!skipPP1 || !config.skipPP1)
                    factoredMethods.Add(factoredPP1);
                if (!skipPM1 || !config.skipPM1)
                    factoredMethods.Add(factoredPP1);
                if (!skipECM || !config.skipECM)
                    factoredMethods.Add(factoredECM);
                if (!skipQS || !config.skipQS)
                    factoredMethods.Add(factoredQS);

                if (factoredMethods.Count == 0)
                {
                    factoredMethods.Add(new[] { factoredRho, factoredRhoP2, factoredRhoP3 }
                        .OrderBy(x => Guid.NewGuid()).First());
                }

                while (!factored && run < maxRuns)
                {
                    run++;

                    var randomMethods = factoredMethods.OrderBy(x => Guid.NewGuid()).ToList();
                    foreach (var method in randomMethods)
                    {
                        factored = method();
                        if (factored)
                            break;
                    }


                    /*
                    res.FermatWatch.Start();
                    factored = Fermat.Resume(resumable, 10000);
                    res.FermatWatch.Stop();
                    if (factored)
                    {
                        res.Add(resumable.P, 1);
                        res.Add(resumable.Q, 1);
                        res.FoundBy = FactorizationMethod.Fermat;
                    }
                    else
                    {
                        res.RhoWatch.Start();
                        var Rho = MathLib.PollardRhoC(n, 1, 1000);
                        res.RhoWatch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = FactorizationMethod.Rho;
                            break;
                        }

                        res.RhoP2Watch.Start();
                        Rho = MathLib.PollardRhoC(n, 2, 1000);
                        res.RhoP2Watch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = FactorizationMethod.RhoP2;
                            break;
                        }

                        res.RhoP3Watch.Start();
                        Rho = MathLib.PollardRhoC(n, 3, 1000);
                        res.RhoP3Watch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = FactorizationMethod.RhoP3;
                            break;
                        }
                        res.RhoZWatch.Start();
                        Rho = MathLib.PollardRhoZOld(n, 200000);
                        res.RhoZWatch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = FactorizationMethod.RhoZ;
                            break;
                        }
                    }
                    */

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
                result.Add(n, 1);
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