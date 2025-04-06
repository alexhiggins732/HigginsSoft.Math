using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    /// <summary>
    /// Provides methods to select optimal parameters for the Quadratic Sieve.
    /// 
    /// Summary of Heuristics:
    /// 
    /// For a composite number N of b bits (with ln N ≈ b ln 2):
    /// 
    /// 1. Factor Base Bound (B):
    ///    B = exp( sqrt( ln N * ln ln N ) )
    /// 
    /// 2. Approximate Number of Primes in the Factor Base:
    ///    |F| ≈ B / ln(B)
    /// 
    /// 3. Sieve Interval (L):
    ///    L ≈ k * B, with k typically chosen between 3 and 5 (default here is 4).
    /// 
    /// Note: These formulas are asymptotic guidelines. In practice, experimental tuning may be necessary.
    /// </summary>
    public class ParameterSelection
    {
        public ParameterSelection(int factorBaseBound, int factorBaseCount, int sieveInterval)
        {
            FactorBaseBound = factorBaseBound;
            FactorBaseCount = factorBaseCount;
            SieveInterval = sieveInterval;
        }

        public int FactorBaseBound { get; }
        public int FactorBaseCount { get; }
        public int SieveInterval { get; }

        /// <summary>
        /// Computes optimal parameters for the Quadratic Sieve given a composite number N.
        /// </summary>
        /// <param name="N">The composite number (as a GmpInt) to be factored.</param>
        /// <param name="k">
        /// The multiplier for the sieve interval (default is 4, but values between 3 and 5 are typical).
        /// </param>
        /// <returns>
        /// A tuple containing:
        ///   - factorBaseBound (B) as an integer,
        ///   - factorBaseCount (approximate number of primes in the factor base),
        ///   - sieveInterval (L)
        /// </returns>
        public static ParameterSelection Compute(GmpInt N, double k = 4.0)
        {
            // Convert N to a double. This assumes that N.ToString() returns a value that can be parsed as a double.
            // For larger numbers, consider using a high-precision library or BigInteger.
            double nDouble = double.Parse(N.ToString());

           GmpInt n = N;
           
            // Compute the natural logarithm of N.
            double lnN = MathLib.Log(n);// Math.Log(nDouble);
            // Compute the natural logarithm of lnN.
            double lnlnN = MathLib.Log(lnN);

            // Factor base bound: B = exp( sqrt( ln N * ln ln N ) )
            MathLib.Exp(MathLib.Sqrt(lnN * lnlnN));
            //double B = Math.Exp(Math.Sqrt(lnN * lnlnN));
            double B = MathLib.Exp(MathLib.Sqrt(lnN * lnlnN));
            int factorBaseBound = (int)MathLib.Round(B);

            // Approximate number of primes in the factor base: |F| ≈ B / ln(B)
            int factorBaseCount = (int)MathLib.Round(B / MathLib.Log(B));

            // Sieve interval: L ≈ k * B (with k typically between 3 and 5, default here is 4)
            int sieveInterval = (int)MathLib.Round(k * B);

            //return (factorBaseBound, factorBaseCount, sieveInterval);
            return new ParameterSelection(factorBaseBound, factorBaseCount, sieveInterval);
        }
    }
}
