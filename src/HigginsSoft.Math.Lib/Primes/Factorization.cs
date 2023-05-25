/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public class Factorization
    {
        public List<Factor> Factors = new();


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
        public static Factorization FactorTrialDivide(GmpInt n)
        {
            var result = new Factorization();
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


    public class FactorizationInt

    {
        public List<IntFactor> Factors = new();
        public int Offset;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactorizationInt FactorTrialDivide(int n)
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

    public class IntFactor : Factor<int>
    {
        public IntFactor(Factor<int> otherFactor) : base(otherFactor)
        {
        }

        public IntFactor(int value, int count) : base(value, count)
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
    public class Factor :
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