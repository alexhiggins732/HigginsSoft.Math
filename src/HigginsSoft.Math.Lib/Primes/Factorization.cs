/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public partial class Factorization
    {
        public List<Factor> Factors = new();

        public Stopwatch? TDivWatch;
        public Stopwatch? FermatWatch;
        public Stopwatch? RhoWatch;
        public Stopwatch? RhoP2Watch;
        public Stopwatch? RhoP3Watch;
        public string? FoundBy;
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
        public static Factorization Factor(GmpInt n, bool checkPrimality = true)
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

        public void Clear() => Factors.Clear();

        public static Factorization FactorTrialDivide(int n)
        {
            var result = new Factorization();
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
        public static FactorizationInt FactorIntTrialDivide1(int n)
        {
            var result = new FactorizationInt();
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

        public static Factorization FactorTrialDivideUnchecked(GmpInt n, GmpInt root, Factorization result)
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

        public static Factorization FactorTrialDivideUnchecked(GmpInt n)
        {
            var result = new Factorization();

            if (MathLib.IsPerfectSquare(n, out GmpInt root))
            {
                result.Add(root, 2);
                return result;
            }
            return FactorTrialDivideUnchecked(n, root, result);

        }

        public static Factorization FactorTrialDivide(GmpInt n, bool checkPrimality = true)
        {
            var result = new Factorization();
            if (n < 4 || checkPrimality && MathLib.IsPrime(n))
            {
                result.Add((int)n, 1);
                return result;
            }
            if (MathLib.IsPerfectSquare(n, out GmpInt root))
            {
                result.Add(root, 2);
                return result;
            }

            return FactorTrialDivideUnchecked(n, root, result);

        }

        public void Add(GmpIntConvertible root, int count)
        {
            Factors.Add(new(root, count));
        }
        public void Add(Factorization f)
        {
            foreach (var otherFactor in f.Factors)
            {
                var thisFactor = Factors.FirstOrDefault(x => x.P == otherFactor.P);
                if (thisFactor != null)
                    thisFactor.Power += otherFactor.Power;
                else
                    Factors.Add(new Factor(otherFactor));
            }
            Factors.Sort();
        }

        public string FactorizationString()
        {
            var product = Factors.Aggregate(GmpInt.One, (a, b) => a * b.GetValue());
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
            return FactorizationString();
        }

        public GmpInt GetProduct()
        {
            var product = GmpInt.One;

            for (var i = 0; i < Factors.Count; i++)
                product *= Factors[i].GetValue();

            return product;
        }

        public bool IsPerfectSquare()
            => Factors.All(x => (x.Power & 1) == 0);

    }

    public class FactStruct
    {

        public int[] FactorCount;
        private int[] Factors;

        private Dictionary<int, int> factorLookup;
        private Dictionary<int, int> additionalFactors;
        private int length;
        public int Length => length;
        public FactStruct(int[] factorBase)
        {

            Factors = factorBase.ToList().OrderBy(x => x).ToArray();
            FactorCount = new int[factorBase.Length];
            this.factorLookup = factorBase.ToDictionary(x => x, x => Array.IndexOf(factorBase, x));
            additionalFactors = new();
        }

        private FactStruct
            (int[] sortedFactors,
            Dictionary<int, int> factorLookup
           )
        {
            Factors = sortedFactors.ToArray();
            FactorCount = new int[Factors.Length];
            this.factorLookup = factorLookup;
            additionalFactors = new();
        }

        public FactStruct EmptyClone()
        {
            var result = new FactStruct(Factors, factorLookup);
            return result;
        }
        public void Add(int n, int power)
        {
            if (!factorLookup.ContainsKey(n))
            {
                if (additionalFactors.ContainsKey(n))
                    additionalFactors[n] += power;
                else
                {
                    additionalFactors[n] = power;
                    length++;
                }

            }
            else
            {
                var idx = factorLookup[n];
                ref int count = ref FactorCount[idx];
                if (count == 0)
                    length++;
                count += power;
            }

        }

        public void Add(FactStruct f)
        {
            for (var i = 0; i < f.FactorCount.Length; i++)
            {
                var power = f.FactorCount[i];
                if (power > 0)
                {
                    Add(Factors[i], power);
                }

            }
        }

        public void Clear()
        {
            Array.Fill(FactorCount, 0);
            length = 0;
            additionalFactors.Clear();
        }

        public string FactorizationString()
        {
            var product = GetProduct();
            var equation = EquationString();
            return $"{product} = {equation}";
        }

        public string EquationString()
        {
            List<string> equations = new();
            var product = 1;
            for (var i = 0; i < Factors.Length; i++)
            {
                var count = FactorCount[i];
                if (count > 0)
                    equations.Add($"{Factors[i]} ^ {count}");
            }

            var equation = string.Join(" * ",
                Factors.Select(x => equations));
            return equation;
        }
        public override string ToString()
        {
            return FactorizationString();
        }

        public int GetProduct()
        {
            var product = 1;
            for (var i = 0; i < Factors.Length; i++)
                for (var j = 0; j < FactorCount[i]; j++)
                    product *= Factors[j];

            return product;
        }

        public bool IsPerfectSquare()
            => FactorCount.All(x => (x & 1) == 0);

        public static FactStruct FactorTrialDivide(int n, FactStruct result)
        {

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
    }

    public class FactorizationInt

    {
        public List<IntFactor> Factors = new();
        public int Offset;

        public void Clear()
        {
            Factors.Clear();
        }
        public static FactorizationInt FactorTrialDivide(GmpInt n)
        {
            var result = new FactorizationInt();
            if (n < 4 || MathLib.IsPrime(n))
            {
                result.Add((int)n, 1);
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
                result.Add((int)n, 1);
            return result;


        }
        public Stopwatch? TDivWatch;
        public Stopwatch? FermatWatch;
        public Stopwatch? RhoWatch;
        public Stopwatch? RhoP2Watch;
        public Stopwatch? RhoP3Watch;
        public Stopwatch? RhoRndWatch;
        public string? FoundBy;
        public string Timings
        {
            get
            {
                var tdiv = TDivWatch is null ? TimeSpan.Zero : TDivWatch.Elapsed;
                var fermat = FermatWatch is null ? TimeSpan.Zero : FermatWatch.Elapsed;
                var rho = RhoWatch is null ? TimeSpan.Zero : RhoWatch.Elapsed;
                var rho2 = RhoP2Watch is null ? TimeSpan.Zero : RhoP2Watch.Elapsed;
                var rho3 = RhoP3Watch is null ? TimeSpan.Zero : RhoP3Watch.Elapsed;
                var rhoRnd = RhoP3Watch is null ? TimeSpan.Zero : RhoRndWatch.Elapsed;
                var by = FoundBy is null ? "" : $"({FoundBy}) ";
                var result = $"{by}Tdiv: {tdiv} - Fermat: {fermat} - Rho: {rho}  - Rho(x^2+2): {rho2} - Rho(x^2+3): {rho3} - Rho(x^2+Rnd): {RhoRndWatch}";
                return result;
            }
        }

        public static FactorizationInt Factor(int n, bool checkPrimality = true, int tDivLimit = PrimeData.MaxIntFactorPrime)
        {

            var sw = Stopwatch.StartNew();
            var res = FactorTrialDivide(n, checkPrimality, tDivLimit);
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
                var resumable = Fermat.StartResumable(n, 100);
                res.FermatWatch.Stop();

                bool factored = resumable.HasFactor;
                int maxIterations = (int)resumable.Iterations;
                while (!factored)
                {

                    res.FermatWatch.Start();
                    //factored = Fermat.Resume(resumable, (int)(resumable.Iterations + maxIterations));
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
                        var Rho = MathLib.PollardRho31(n, new MathLib.RhoPoly(1));
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
                        Rho = MathLib.PollardRho31(n, new MathLib.RhoPoly(2));
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
                        Rho = MathLib.PollardRho31(n, new MathLib.RhoPoly(3));
                        res.RhoP3Watch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = nameof(Rho) + "+3";
                            break;
                        }


                        Rho = MathLib.PollardRho31(n);
                        res.RhoP3Watch.Stop();
                        if (Rho != n)
                        {
                            res.Add(Rho, 1);
                            res.Add(n / Rho, 1);
                            factored = true;
                            res.FoundBy = nameof(Rho) + "+rnd";
                            break;
                        }

                    }
                }

            }
            return res;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactorizationInt FactorTrialDivide(int n, bool checkIsPrime = true, int maxPrime = PrimeData.MaxIntFactorPrime)
        {
            var result = new FactorizationInt();
            if (n < 4 || (checkIsPrime && MathLib.IsPrime(n)))
            {
                result.Add(n, 1);
                return result;
            }

            var root = (int)MathLib.Sqrt(n);
            var primes = Primes.IntFactorPrimes;

            int prime;
            for (var j = 0; j < primes.Length && (prime = primes[j]) <= root && prime <= maxPrime; j++)
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountTrialDivideOps(int n)
        {
            int count = 0;
            if (n > 4)
            {
                var candidates = Primes.IntFactorPrimes;
                var root = (int)System.Math.Sqrt(n);
                int res;
                foreach (var p in candidates)
                {
                    if (p > root) break;
                    count++;
                    res = n % p;
                    Console.WriteLine($"{n}%{p}={res} - count: {count}");
                    if (res == 0)
                        break;
                }
            }
            return count;
        }


        public void Add(int root, int count)
        {
            Factors.Add(new(root, count));
        }
        public void Add(FactorizationInt f)
        {
            foreach (var otherFactor in f.Factors)
            {
                var thisFactor = Factors.FirstOrDefault(x => x.P == otherFactor.P);
                if (thisFactor != null)
                    thisFactor.Power += otherFactor.Power;
                else
                    Factors.Add(new IntFactor(otherFactor));
            }
            Factors.Sort();
        }

        public string FactorizationString()
        {
            var product = Factors.Aggregate(1L, (a, b) => a * b.GetValue());
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
            return FactorizationString();
        }

        public int GetProduct()
        {
            var product = 1;

            for (var i = 0; i < Factors.Count; i++)
                product *= Factors[i].GetValue();

            return product;
        }

        public bool IsPerfectSquare()
            => Factors.All(x => (x.Power & 1) == 0);
    }

    public class FactorizationInt64

    {
        public List<IntFactor64> Factors = new();
        public int Offset;

        public void Clear()
        {
            Factors.Clear();
        }

        public static FactorizationInt64 FactorTrialDivide(GmpInt n)
        {
            var result = new FactorizationInt64();
            if (n < 4 || n.IsProbablePrime())
            {
                result.Add((long)n, 1);
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
                result.Add((long)n, 1);
            return result;


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactorizationInt64 FactorTrialDivide(long n)
        {
            var result = new FactorizationInt64();
            if (n < 4 || MathLib.IsPrime(n))
            {
                result.Add(n, 1);
                return result;
            }

            var root = (long)MathLib.Sqrt(n);
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



        public void Add(long root, int count)
        {
            Factors.Add(new(root, count));
        }
        public void Add(FactorizationInt64 f)
        {
            foreach (var otherFactor in f.Factors)
            {
                var thisFactor = Factors.FirstOrDefault(x => x.P == otherFactor.P);
                if (thisFactor != null)
                    thisFactor.Power += otherFactor.Power;
                else
                    Factors.Add(new IntFactor64(otherFactor));
            }
            Factors.Sort();
        }

        public string FactorizationString()
        {
            var product = Factors.Aggregate(1L, (a, b) => a * b.GetValue());
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
            return FactorizationString();
        }

        public long GetProduct()
        {
            var product = 1L;

            for (var i = 0; i < Factors.Count; i++)
                product *= Factors[i].GetValue();

            return product;
        }

        public bool IsPerfectSquare()
            => Factors.All(x => (x.Power & 1) == 0);
    }

    public class IntFactor : Factor<int>
    {
        public IntFactor(Factor<int> otherFactor) : base(otherFactor)
        {
        }

        public IntFactor(int value, int count) : base(value, count)
        {
        }
    }
    public class IntFactor64 : Factor<long>
    {
        public IntFactor64(Factor<long> otherFactor) : base(otherFactor)
        {
        }

        public IntFactor64(long value, int count) : base(value, count)
        {
        }
    }


    public class Factor<T> :
        ICloneable,
        IComparable<Factor<T>>,
        IComparable
        where T : struct, IComparable<T>
    {
        public Factor(Factor<T> otherFactor)
        {
            this.P = otherFactor.P;
            this.Power = otherFactor.Power;
        }

        public Factor(T value, int count)
        {
            P = value;
            Power = count;
        }

        public T P { get; private set; }
        public int Power { get; set; }

        public Factor<T> Clone() => new Factor<T>(this);


        public T GetValue()
            => Ops<T>.Power(P, Ops<T>.ConvertFromInt(Power));

        object ICloneable.Clone() => Clone();

        public string FactorizationString()
        {
            return $"{P}^{Power}";
        }

        public int CompareTo(Factor<T>? other)
        {
            if (other is null) return 1;
            var result = this.P.CompareTo(other.P);
            if (result == 0) result = this.Power.CompareTo(other.Power);
            return result;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is Factor other) return CompareTo(other);
            throw new ArgumentException("Value must be a factor", nameof(obj));
        }

        public override string ToString()
        {
            return FactorizationString();
        }
    }
    public partial class Factor :
        ICloneable,
        IComparable<Factor>,
        IComparable
    {
        public Factor(Factor otherFactor)
        {
            this.P = otherFactor.P.Clone();
            this.Power = otherFactor.Power;
        }

        public Factor(GmpIntConvertible value, int count)
        {
            P = value;
            Power = count;
        }

        public GmpInt P { get; private set; }
        public int Power { get; set; }

        public Factor Clone() => new Factor(this);


        public GmpInt GetValue()
            => P.Power(Power);

        object ICloneable.Clone() => Clone();

        public string FactorizationString()
        {
            return $"{P}^{Power}";
        }

        public int CompareTo(Factor? other)
        {
            if (other is null) return 1;
            var result = this.P.CompareTo(other.P);
            if (result == 0) result = this.Power.CompareTo(other.Power);
            return result;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is Factor other) return CompareTo(other);
            throw new ArgumentException("Value must be a factor", nameof(obj));
        }

        public override string ToString()
        {
            return FactorizationString();
        }
    }
}