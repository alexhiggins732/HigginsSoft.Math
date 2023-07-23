/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

namespace HigginsSoft.Math.Lib
{

    /// <summary>
    /// Factors N by searching for conqurence of squares from sqrt(n)+ ... n-1. Faster than trial division for numbers between 300 and ~7000.
    /// </summary>
    /// <remarks>Additional data should be collected to determine cuttover to trial division for large numbers as congruences quickly
    /// eliminate larger primes &lt; sqrt(n) as candidates. At the point of convergence with prime saturation, an upper limit for primes
    /// to be search by trial division can be selected.
    /// 
    /// Additional optimization opportunities include checking if candidate is prime, and calculating candidates without the square root.
    /// 
    /// For example, factoring 1073741827 starts with candidate 32514 which is not prime and after just 88 steps is down to candidate 30441,
    /// with delta of 12 from the previous candidate. That's lowers the maximum size of prime q by 2,073 in just 88 steps, using naive approach.
    /// 
    /// Those 88 candidates filter down to just 9 actual primes that need to be tested. See: PrimalityCheckTest.CountPrimeCongruenceCandidates()
    /// Note the <paramref name="CheckIsPrimeBruteForce"/> method itself does not work if just prime candidate are tested, but but using the 
    /// coungruences it generates may be a way to cut down a portion of larger primes just under sqrt(n) when trial dividing.
    /// </remarks>
    public class PrimeSquareCongruenceChecker
    {
        public static PrimeSquareCongruenceChecker instance = new();
        public static bool IsPrime(int n) => instance.CheckIsPrimeBruteForce(n, out _, out _);

        /// <summary>
        /// Searches for conqurence of squares from sqrt(n)+ ... n-1
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIsPrimeBruteForce(int n, out int a, out int b)
        {
            a = n; b = 1;
            if (n < 2) return false;
            //if ((n & 1) == 0) return n == 2;

            //var res = n % 6;
            //if (res!=1 && res!=5) return false;
            var result = false;
            if (n < 300)
            {
                result = Primes.TrialDivide(n, 17, out b);
                if (result) a = n / b;
                return result;
            }

            // breakout if number is perfect square
            if (MathLib.IsPerfectSquare(n, out int root))
            {
                a = b = root;
                result = false;
            }
            else
            {


                int p;
                result = true;

                for (var i = root + 1; i < n; i++)
                {
                    p = i - (int)MathLib.Sqrt(((long)i * i) - n);

                    // no need to test prime candidates less than 2.
                    if (p < 2) break;


                    // check if the candidate divides n.
                    // TODO: Can potentially optimize this method by only checking prime candidates.
                    if (n % p == 0)
                    {
                        a = p;
                        b = n / p;
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        public bool CheckIsPrimeBruteForceDebug(int n, out int a, out int b)
        {
            a = n; b = 1;
            if (n < 2) return false;
            //if ((n & 1) == 0) return n == 2;

            //var res = n % 6;
            //if (res!=1 && res!=5) return false;
            var result = false;
            if (n < 300)
            {
                result = Primes.TrialDivide(n, 17, out b);
                if (result) a = n / b;
                return result;
            }

            // breakout if number is perfect square
            if (MathLib.IsPerfectSquare(n, out int root))
            {
                a = b = root;
                result = false;
            }
            else
            {

                int limit = root * root;
                int p;
                result = true;
                long iSquared;
                long iSquaredMinusN;
                double iSquaredMinusNRoot;
                int iMinusiSquaredMinusNRoot;
                int last = 0;
                int delta = 0;
                int i;
                // Iterating over to get the
                // closest average value
                var message = $"{nameof(n)}\t{nameof(i)}\t{nameof(iSquared)}\t{nameof(iSquaredMinusN)}\t{nameof(iSquaredMinusNRoot)}\t{nameof(iMinusiSquaredMinusNRoot)}\t{nameof(delta)}";
                Console.WriteLine(message);

                for (i = root + 1; i < n; i++)
                {
                    iSquared = (long)i * i;
                    iSquaredMinusN = iSquared - n;
                    iSquaredMinusNRoot = (int)MathLib.Sqrt(iSquaredMinusN);
                    iMinusiSquaredMinusNRoot = i - (int)iSquaredMinusNRoot;
                    delta = last == 0 ? 0 : last - iMinusiSquaredMinusNRoot;
                    last = iMinusiSquaredMinusNRoot;
                    message = $"{n}\t{i}\t{iSquared}\t{iSquaredMinusN}\t{iSquaredMinusNRoot}\t{iMinusiSquaredMinusNRoot}\t{delta}";
                    Console.WriteLine(message);
                    // 1st Factor
                    p = i - (int)MathLib.Sqrt(((long)i * i) - n);

                    // 2nd Factor
                    //int q = n / p;

                    //// To avoid Convergence
                    //if (p < 2 || q < 2)
                    //{
                    //    break;
                    //}
                    if (p < 2) break;

                    // checking semi-prime condition
                    //if ((p * q) == n)
                    if (n % p == 0)
                    {
                        a = p;
                        b = n / p;
                        result = false;
                        message = $"{n}\t{a}\t{b}\t{result}\t-\t-\t-";
                        Console.WriteLine(message);
                        break;
                    }

                    //// If convergence found
                    //// then number is semi-prime
                    //else
                    //{

                    //    // convergence not found
                    //    // then number is prime
                    //    result = true;
                    //}
                }
            }
            if (result)
            {
                string message = $"{n}\t-\t-\t{result}\t-\t-\t-";
                Console.WriteLine(message);
            }
            return result;
        }
    }
}