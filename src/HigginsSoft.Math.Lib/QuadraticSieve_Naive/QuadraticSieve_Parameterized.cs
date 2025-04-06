using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;
    using System.Text.Json;

    using MathGmp.Native;


    public class QsParamTest
    {
        public static void RunSmallTest()
        {
            var a = MathUtil.GetNextPrime((GmpInt)(BigInteger)MathLib.GenerateRandomBigInt(32));
            var b = MathUtil.GetNextPrime((GmpInt)(BigInteger)MathLib.GenerateRandomBigInt(32));


            // Test the Quadratic Sieve on a small composite number.
            GmpInt n = (bool.Parse(bool.TrueString)) ? a * b : new GmpInt(8051);
            QuadraticSieve_Naive qs = new QuadraticSieve_Naive(n);
            GmpInt factor = qs.Factor();
            Console.WriteLine("Factor found: " + factor);
        }

        public static void RunSmallTest64(int numTests = 10, int bits = 16)
        {
            Dictionary<GmpInt, bool> factors = new();


            for (var i = 0; i < numTests; i++)
            {
                // Test the Quadratic Sieve on a small composite number.

                var a = MathUtil.GetNextPrime((GmpInt)(BigInteger)MathLib.GenerateRandomBigInt(bits));
                var b = MathUtil.GetNextPrime((GmpInt)(BigInteger)MathLib.GenerateRandomBigInt(bits));

                GmpInt n = a * b;

                Console.WriteLine($"Test {i}: Factoring {n}  = ({a} * {b})");
                var qs = new QuadraticSieve_Parameterized(n);

                Console.WriteLine($"{n}  = ({a} * {b})");
                var sw = Stopwatch.StartNew();
                GmpInt factor = qs.Factor();
                sw.Stop();
                Console.WriteLine($" => Dependencies: {qs.DependencyCount} - Factor Base : {qs.FactorBase.Count} - Relations: {qs.Relations.Count}  -> Factor: {factor}");
                Console.WriteLine($"    => {sw.Elapsed} ms");

                bool result = false;
                if (factor > 1 && factor < n)
                {
                    result = true;
                }
                factors.Add(n, result);
            }

            var found = factors.Where(x => x.Value).Count();
            Console.WriteLine($"Found {found} of {factors.Count} factors");
        }
    }
    public class QuadraticSieve_Parameterized
    {

        private GmpInt n;

        public List<SmoothRelation> Relations { get; set; }
        public List<int> FactorBase { get; private set; }
        public int DependencyCount { get; private set; }

        public QuadraticSieve_Parameterized(GmpInt n)
        {
            this.n = n;
        }



        // Public method to attempt factoring n using the Quadratic Sieve.
        public GmpInt Factor()
        {
            // 1. Compute floor(sqrt(n))
            GmpInt sqrtN = FloorSqrt();
            GmpInt factor = 1;
            var qsParams = ParameterSelection.Compute(n);
            //var bound = qsParams.Bound;
            // var sieveInterval = qsParams.SieveInterval;
            //var factorBaseBound = qsParams.FactorBaseBound;

            var startFactorBaseBound = (int)Math.Sqrt(qsParams.FactorBaseBound);
            int sieveInterval = qsParams.SieveInterval * 10;
            startFactorBaseBound = 17;
            var initialSieveInterval = sieveInterval = 510510*2;
            int startCount = 17;
            bool setStart = false;

            for (var factorBaseBound = startFactorBaseBound; ; startCount++, factorBaseBound += 25)
            {
                // 2. Select the factor base. (Tune 'factorBaseBound' as needed.)
                //int factorBaseBound1 = 125; // Example bound; adjust based on n.
                //if (n < factorBaseBound)
                //    factorBaseBound = (int)(n - 1);
                this.FactorBase = SelectFactorBaseByCount(startCount, n);
                if (!setStart)
                {
                    Console.WriteLine($"Factoring C{n.BitLength} with {FactorBase.Count} primes with sieve size {sieveInterval}");
                }

                sieveInterval += initialSieveInterval;
                Console.Title = $"Factoring C{n.BitLength} with {FactorBase.Count} primes";

                // 3. Sieve for smooth relations. (Tune 'sieveInterval' as needed.)
                ; // Example interval length.
                var sw= Stopwatch.StartNew();
                this.Relations = SieveForSmoothRelations(FactorBase, sqrtN, sieveInterval);
                sw.Stop();
                Console.WriteLine($"    => [{sw.Elapsed}] {FactorBase.Count} Primes With Relations: {Relations.Count}");
                // 4. Use linear algebra to find a dependency among the exponent vectors.
                int dependencyCount = 0;
                foreach (var dependency in FindDependency(Relations))
                {
                    dependencyCount++;
                    if (dependency.Count > 0)
                    {
                        Console.WriteLine($"        => Testing Dependency: {dependencyCount}");
                        // 5. Compute and return a factor from the dependency.
                        factor = ComputeFactor(dependency, sqrtN);
                        if (factor > 1 && factor < n)
                        {
                            return factor;
                        }
                    }
                }
                //List<SmoothRelation> dependency = FindDependency(Relations);
                //var serializableDependency = dependency.Select(x => new SmoothRelationSerualizable
                //{
                //    x = x.x,
                //    Qx = x.Qx.ToString(),
                //    Factorization = x.Factorization,
                //    ExponentVector = x.ExponentVector
                //}).ToList();
                //var json = JsonSerializer.Serialize(serializableDependency, new JsonSerializerOptions { WriteIndented = true });
                //Console.WriteLine(json);

                // 5. Compute and return a factor from the dependency.
            }
            return factor;
        }


        // Step 1: Compute floor(sqrt(n))
        private GmpInt FloorSqrt()
        {
            // TODO: Compute the integer square root of n using GMP routines.
            // For example, if your GmpInt wrapper provides a method for sqrt,
            // you can use that here.
            GmpInt sqrtN = n.Sqrt();
            return sqrtN;
        }


        // Step 2: Select the factor base.
        // Generate a list of primes for which n is a quadratic residue.
        private List<int> SelectFactorBaseByCount(int count, GmpInt n)
        {
            List<int> factorBase = new List<int>();
            // Generate primes up to the provided bound (naively here)
            for (int p = 2; ; p++)
            {
                if (IsPrime(p))
                {
                    // Check if n is a quadratic residue modulo p using the Legendre symbol.
                    if (LegendreSymbol(n, p) == 1)
                    {
                        factorBase.Add(p);
                        if (factorBase.Count >= count)
                        {
                            break;
                        }
                    }
                }
            }
            return factorBase;
        }

        // Step 2: Select the factor base.
        // Generate a list of primes for which n is a quadratic residue.
        private List<int> SelectFactorBase(int bound, GmpInt n)
        {
            List<int> factorBase = new List<int>();
            // Generate primes up to the provided bound (naively here)
            for (int p = 2; p <= bound; p++)
            {
                if (IsPrime(p))
                {
                    // Check if n is a quadratic residue modulo p using the Legendre symbol.
                    if (LegendreSymbol(n, p) == 1)
                    {
                        factorBase.Add(p);
                    }
                }
            }
            return factorBase;
        }

        // Naive prime checker.
        private bool IsPrime(int p)
        {
            if (p < 2)
                return false;
            for (int i = 2; i <= Math.Sqrt(p); i++)
            {
                if (p % i == 0)
                    return false;
            }
            return true;
        }

        // Compute the Legendre symbol (n|p).
        private int LegendreSymbol(GmpInt n, int p)
        {
            // TODO: Implement modular exponentiation using GMP to compute:
            // r = n^((p-1)/2) mod p, and return 1 if r == 1, -1 if r == p-1, else 0.
            // For now, return 1 as a placeholder.
            var pHalf = p >> 1;
            var res = (GmpInt)n.PowerMod(pHalf, p);
            if (res.IsOne)
                return 1;
            if (res == p - 1)
                return -1;
            return 0;

        }

        // Step 3: Sieving for smooth relations.
        // A smooth relation is a value x for which Q(x) factors completely over the factor base.
        private List<SmoothRelation> SieveForSmoothRelations(List<int> factorBase, GmpInt sqrtN, int sieveInterval)
        {
            List<SmoothRelation> relations = new List<SmoothRelation>();

            // Define Q(x) = (sqrtN + x)^2 - n and search in the interval [-sieveInterval, sieveInterval].
            int count = 0;
            for (int x = -sieveInterval; x <= sieveInterval; x++, count++)
            {

                // Compute Q(x)
                GmpInt candidate = ComputeQx(sqrtN, x);


                // Factor candidate over the factor base.
                var factorization = FactorOverBase(candidate, factorBase);
                if (factorization != null)
                {
                    // Create an exponent vector (mod 2) for the relation.
                    bool[] exponentVector = CreateExponentVector(factorization, factorBase);
                    SmoothRelation relation = new SmoothRelation
                    {
                        x = x,
                        Qx = candidate,
                        Factorization = factorization,
                        ExponentVector = exponentVector
                    };
                    relations.Add(relation);
                    if (relations.Count > factorBase.Count + 5)
                    {
                        return relations;
                    }
                }
            }

            return relations;
        }

        // Compute Q(x) = (sqrtN + x)^2 - n.
        private GmpInt ComputeQx(GmpInt sqrtN, int x)
        {
            // Assuming GmpInt supports basic arithmetic.
            GmpInt sum = sqrtN + new GmpInt(x);
            GmpInt result = sum * sum - n;
            return result;
        }

        // Attempt to factor 'candidate' over the factor base.
        // Returns a dictionary mapping prime factors to their exponents if candidate is B-smooth,
        // or null if it is not fully factorable over the factor base.
        private Dictionary<int, int> FactorOverBase(GmpInt candidate, List<int> factorBase)
        {
            Dictionary<int, int> factors = new Dictionary<int, int>();
            GmpInt remainder = candidate.Copy(); // Assuming your GmpInt supports a Copy method.
            foreach (int prime in factorBase)
            {
                int exponent = 0;
                // Divide out the prime as long as it divides the remainder.
                while (remainder % prime == 0)
                {
                    exponent++;
                    remainder /= prime;
                }
                if (exponent > 0)
                {
                    factors[prime] = exponent;
                }
            }

            // If the remainder is 1, candidate is B-smooth.
            if (remainder.Equals(new GmpInt(1)))
            {
                return factors;
            }
            else
            {
                return null; // Candidate is not B-smooth.
            }
        }

        // Create an exponent vector modulo 2 from the factorization.
        private bool[] CreateExponentVector(Dictionary<int, int> factorization, List<int> factorBase)
        {
            bool[] vector = new bool[factorBase.Count];
            for (int i = 0; i < factorBase.Count; i++)
            {
                int prime = factorBase[i];
                if (factorization.ContainsKey(prime))
                {
                    // Store true if the exponent is odd, false otherwise.
                    vector[i] = (factorization[prime] % 2 == 1);
                }
                else
                {
                    vector[i] = false;
                }
            }
            return vector;
        }

        // Step 4: Find a dependency among the exponent vectors using linear algebra over GF(2).
        private List<SmoothRelation> FindDependencyStub(List<SmoothRelation> relations)
        {
            // TODO: Implement Gaussian elimination over GF(2) on the binary matrix of exponent vectors.
            // Find a subset of relations whose vector sum is the zero vector.
            // For now, return an empty list as a placeholder.
            return new List<SmoothRelation>();
        }

        // Step 4: Find a dependency among the exponent vectors using Gaussian elimination over GF(2).
        private List<SmoothRelation> FindDependencOpt(List<SmoothRelation> relations)
        {
            int m = relations.Count;
            if (m == 0)
                return new List<SmoothRelation>();

            int n = relations[0].ExponentVector.Length;

            // Copy the exponent vectors into a matrix A and set up an identity matrix for tracking combinations.
            bool[][] A = new bool[m][];
            bool[][] comb = new bool[m][];
            for (int i = 0; i < m; i++)
            {
                A[i] = new bool[n];
                Array.Copy(relations[i].ExponentVector, A[i], n);
                comb[i] = new bool[m];
                comb[i][i] = true; // initialize as identity row
            }

            int row = 0;
            // Process each column
            for (int col = 0; col < n && row < m; col++)
            {
                // Find a pivot in this column.
                int pivot = -1;
                for (int i = row; i < m; i++)
                {
                    if (A[i][col])
                    {
                        pivot = i;
                        break;
                    }
                }
                // No pivot in this column: continue to the next column.
                if (pivot == -1)
                    continue;

                // Swap current row with pivot row if needed.
                if (pivot != row)
                {
                    bool[] temp = A[row];
                    A[row] = A[pivot];
                    A[pivot] = temp;

                    temp = comb[row];
                    comb[row] = comb[pivot];
                    comb[pivot] = temp;
                }

                // Eliminate the pivot column in all other rows.
                for (int i = 0; i < m; i++)
                {
                    if (i != row && A[i][col])
                    {
                        for (int j = 0; j < n; j++)
                        {
                            A[i][j] ^= A[row][j]; // XOR for GF(2)
                        }
                        for (int j = 0; j < m; j++)
                        {
                            comb[i][j] ^= comb[row][j];
                        }
                    }
                }
                row++;
            }

            // Look for a row in A that has become all zeros.
            for (int i = 0; i < m; i++)
            {
                bool allZero = true;
                for (int j = 0; j < n; j++)
                {
                    if (A[i][j])
                    {
                        allZero = false;
                        break;
                    }
                }
                if (allZero)
                {
                    // The corresponding comb[i] shows which relations (rows) form a dependency.
                    List<SmoothRelation> dependency = new List<SmoothRelation>();
                    for (int j = 0; j < m; j++)
                    {
                        if (comb[i][j])
                        {
                            dependency.Add(relations[j]);
                        }
                    }
                    return dependency;
                }
            }

            // No dependency found.
            return new List<SmoothRelation>();
        }


        /*
         * 
         * The key issue is that in the current implementation you compute the “square root” for each relation individually. However, in the Quadratic Sieve the product of the Q(x) values from a dependency is guaranteed to be a perfect square even if each Q(x) isn’t. In other words, you must first combine (i.e. sum) the factor exponents from all selected relations and then take half of that sum to form the square root of the product.

Below is a revised version of your ComputeFactor method that does the following:

Iterates over nonempty subsets of your dependency.
For each subset, it multiplies the corresponding 
(
sqrtN
+
𝑥
)
(sqrtN+x) values to form the “left” product.
It also builds a combined factorization by summing the exponents from each relation.
From the combined factorization it computes the “right” product by raising each prime to half its combined exponent.
Finally, it computes the gcd of 
∣
leftProduct
−
rightProduct
∣
∣leftProduct−rightProduct∣ with 
𝑛
n.*/

        // Step 4: Find a dependency among the exponent vectors using a brute-force search.
        // For small numbers of relations, this method iterates through every non-empty subset of relations
        // and returns the first subset whose XOR (mod 2) sum of exponent vectors is the zero vector.
        private IEnumerable<List<SmoothRelation>> FindDependency(List<SmoothRelation> relations)
        {
            int m = relations.Count;
            if (m == 0 || relations.Count == 0)
            {
                yield return new List<SmoothRelation>();
            }
            else
            {
                int vectorLength = relations[0].ExponentVector.Length;
                int subsetCount = 1 << m; // 2^m possible subsets

                // Iterate over all non-empty subsets (mask 1 to 2^m - 1)
                for (int mask = 1; mask < subsetCount; mask++)
                {
                    bool[] xorSum = new bool[vectorLength];
                    List<SmoothRelation> subset = new List<SmoothRelation>();

                    // Build the subset and compute the XOR of exponent vectors.
                    for (int i = 0; i < m; i++)
                    {
                        if ((mask & (1 << i)) != 0)
                        {
                            subset.Add(relations[i]);
                            for (int j = 0; j < vectorLength; j++)
                            {
                                xorSum[j] ^= relations[i].ExponentVector[j];
                            }
                        }
                    }

                    // Check if xorSum is the zero vector (all false).
                    bool isZero = true;
                    for (int j = 0; j < vectorLength; j++)
                    {
                        if (xorSum[j])
                        {
                            isZero = false;
                            break;
                        }
                    }

                    if (isZero)
                    {
                        DependencyCount++;
                        yield return subset;
                    }
                }

                // If no dependency is found, return an empty list.
                yield return new List<SmoothRelation>();
            }
        }


        // Step 5: Compute a factor using the dependency found.
        private GmpInt ComputeFactorSimple(List<SmoothRelation> dependency, GmpInt sqrtN)
        {
            // Multiply the corresponding (sqrtN + x) values and the square roots of Q(x) values.
            GmpInt leftProduct = new GmpInt(1);
            GmpInt rightProduct = new GmpInt(1);

            foreach (var relation in dependency)
            {
                leftProduct *= (sqrtN + new GmpInt(relation.x));
                // Compute the square root of Q(x) from its factorization.
                GmpInt qxSqrt = ComputeSqrtFromFactorization(relation.Factorization);
                rightProduct *= qxSqrt;
            }

            // We now have an equation of the form: leftProduct^2 ≡ rightProduct^2 (mod n)
            // Calculate the candidate factor as gcd(|leftProduct - rightProduct|, n).
            GmpInt diff = (leftProduct - rightProduct).Abs(); // Assuming an Abs() method exists.
            GmpInt factor = GmpInt.Gcd(diff, n);
            return factor;
        }


        // Step 5: Compute a factor using the dependency found.
        // This version iterates over all non-empty subsets of the dependency to
        // try and find a subset that yields a nontrivial factor.
        private GmpInt ComputeFactorV2(List<SmoothRelation> dependency, GmpInt sqrtN)
        {
            int m = dependency.Count;
            int subsetCount = 1 << m; // Total number of subsets

            // Try every non-empty subset of the dependency
            for (int mask = 1; mask < subsetCount; mask++)
            {
                GmpInt leftProduct = new GmpInt(1);
                GmpInt rightProduct = new GmpInt(1);

                // Build the subset and compute the products.
                for (int i = 0; i < m; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        var relation = dependency[i];
                        leftProduct *= (sqrtN + new GmpInt(relation.x));
                        // Compute the square root of Q(x) from its factorization.
                        GmpInt qxSqrt = ComputeSqrtFromFactorization(relation.Factorization);
                        rightProduct *= qxSqrt;
                    }
                }

                // We now have an equation: (leftProduct)^2 ≡ (rightProduct)^2 (mod n)
                // Compute the candidate factor as gcd(|leftProduct - rightProduct|, n)
                GmpInt diff = (leftProduct - rightProduct).Abs(); // Assuming Abs() is implemented.
                GmpInt factor = GmpInt.Gcd(diff, n);

                // If the factor is non-trivial, return it.
                if (!factor.Equals(new GmpInt(1)) && !factor.Equals(n))
                {
                    return factor;
                }
            }

            // No nontrivial factor found from any subset.
            return new GmpInt(1);
        }


        /*
         * Explanation
Combining Factorizations:
Instead of taking the square root for each relation, we sum the exponents from all relations in the subset. Since the dependency guarantees that every prime’s total exponent is even, we can safely take half of each summed exponent.

Left and Right Products:

The left product is computed by multiplying 
(
sqrtN
+
𝑥
)
(sqrtN+x) for each selected relation.
The right product is computed by taking, for each prime 
𝑝
p in the combined factorization, 
𝑝
(
combined exponent
/
2
)
p 
(combined exponent/2)
 . This gives the square root of the product of Q(x) values.
GCD Check:
The final factor is obtained as 
gcd
⁡
(
∣
leftProduct
−
rightProduct
∣
,
𝑛
)
gcd(∣leftProduct−rightProduct∣,n). If this factor is nontrivial (not 1 or 
𝑛
n), then you’ve successfully factored 
𝑛
n.

Next Steps
Verify that the combined exponents are indeed even for your dependency subsets.
Check your implementation of GMP arithmetic (like multiplication, power, gcd, etc.) to ensure they work as expected.
This adjustment should help yield a nontrivial factor when the dependency is valid.
        */
        // Step 5: Compute a factor using the dependency found.
        // Revised: Combine the factorizations from the dependency subset so that the square root
        // of the product of Q(x) values is computed correctly.
        private GmpInt ComputeFactor(List<SmoothRelation> dependency, GmpInt sqrtN)
        {
            int dedendecyChecks = 0;
            int m = dependency.Count;
            int subsetCount = 1 << m; // Total number of subsets

            // Try every non-empty subset of the dependency
            for (int mask = 1; mask < subsetCount; mask++)
            {
                GmpInt leftProduct = new GmpInt(1);
                Dictionary<int, int> combinedFactorization = new Dictionary<int, int>();

                // Build the subset: combine the (sqrtN + x) factors and accumulate the exponents.
                for (int i = 0; i < m; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        var relation = dependency[i];
                        leftProduct *= (sqrtN + new GmpInt(relation.x));

                        // Merge factorization: sum exponents for each prime.
                        foreach (var kvp in relation.Factorization)
                        {
                            int prime = kvp.Key;
                            int exp = kvp.Value;
                            if (combinedFactorization.ContainsKey(prime))
                                combinedFactorization[prime] += exp;
                            else
                                combinedFactorization[prime] = exp;
                        }
                    }
                }

                // Compute the right product: for each prime, raise it to half the combined exponent.
                GmpInt rightProduct = new GmpInt(1);
                bool validSquare = true;
                foreach (var kvp in combinedFactorization)
                {
                    int prime = kvp.Key;
                    int exp = kvp.Value;
                    // The combined exponent must be even for the product to be a perfect square.
                    if (exp % 2 != 0)
                    {
                        validSquare = false;
                        break;
                    }
                    int halfExp = exp / 2;
                    rightProduct *= GmpInt.Power(new GmpInt(prime), halfExp);
                }

                if (!validSquare)
                {
                    // This subset does not yield a perfect square.
                    continue;
                }

                // At this point we have:
                // leftProduct^2 ≡ rightProduct^2 (mod n)
                GmpInt diff = (leftProduct - rightProduct).Abs(); // Assuming Abs() is implemented.
                GmpInt factor = GmpInt.Gcd(diff, n);

                // Return nontrivial factor if found.
                if (!factor.Equals(new GmpInt(1)) && !factor.Equals(n))
                {
                    return factor;
                }
            }

            // If no nontrivial factor is found, return 1.
            return new GmpInt(1);
        }

        // Compute the square root of Q(x) from its factorization by halving the exponents.
        private GmpInt ComputeSqrtFromFactorization(Dictionary<int, int> factorization)
        {
            GmpInt result = new GmpInt(1);
            foreach (var kvp in factorization)
            {
                int prime = kvp.Key;
                int exponent = kvp.Value;
                int halfExp = exponent / 2;
                result *= GmpInt.Power(prime, halfExp);
            }
            return result;
        }


        // Helper class to store a smooth relation.
        public class SmoothRelation
        {
            public int x; // Offset from sqrt(n).
            public GmpInt Qx; // Q(x) = (sqrt(n) + x)^2 - n.
            public Dictionary<int, int> Factorization; // Factorization of Q(x) over the factor base.
            public bool[] ExponentVector; // Binary vector (mod 2) of exponents for linear algebra.
        }

        private class SmoothRelationSerualizable
        {
            public int x { get; set; } // Offset from sqrt(n).
            public string Qx { get; set; }// Q(x) = (sqrt(n) + x)^2 - n.
            public Dictionary<int, int> Factorization { get; set; } // Factorization of Q(x) over the factor base.
            public bool[] ExponentVector { get; set; } // Binary vector (mod 2) of exponents for linear algebra.
        }
    }

}
