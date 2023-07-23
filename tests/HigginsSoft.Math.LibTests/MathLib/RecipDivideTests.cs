#define SKIP_LONG_TESTS

using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using MathGmp.Native;
using System.Numerics;
using BenchmarkDotNet.Environments;
using System.Drawing;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using System.Security.Principal;
using static HigginsSoft.Math.Demos.LoopBenchmark;
using HigginsSoft.Math.Demos;

namespace HigginsSoft.Math.Lib.Tests
{

    [TestClass()]
    public class RecipDivideTests
    {
        private const int Length = 32 * 1024;
        private int[] data;

        [Params(8, 32)]
        public int Alignment { get; set; } = 32;

        [GlobalSetup]
        public unsafe void GlobalSetup()
        {
            for (; ; )
            {
                data = Enumerable.Range(0, Length).ToArray();

                fixed (int* ptr = data)
                {
                    if ((Alignment == 32 && (uint)ptr % 32 == 0) || (Alignment == 8 && (uint)ptr % 16 != 0))
                    {
                        break;
                    }
                }
            }
        }


        [TestMethod()]
        public void PrintTDivCounts()
        {
            TDivCount.PrintCounts();
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif       
        [TestMethod()]
        public void PrintTDivCountIncremental()
        {
            TDivCount.PrintCountIncremental();
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod()]
        public void PrintTDivCountIncrementalAvx()
        {
            TDivCount.PrintCountIncrementalAvx();
        }


        [TestMethod()]
        public void RunTest()
        {
            GlobalSetup();
            Array.Fill(data, 1);


            var d = new Dictionary<string, long>
            {
                {nameof(LL.Sum), 0 },
                {nameof(LL.SumAligned), 0 },
                {nameof(LL.SumPipelined), 0 },
                {nameof(LL.SumAlignedPipelined), 0 }

            };


            int result, expected;
            int numTests = 1000;
            for (var i = 0; i < 1000; i++)
            {
                var sw = Stopwatch.StartNew();
                result = LL.Sum(data);
                sw.Stop();
                expected = data.Length;
                Assert.AreEqual(expected, result);

                d[nameof(LL.Sum)] += sw.ElapsedTicks;

                sw.Restart();
                result = LL.SumAligned(data);
                sw.Stop();
                expected = data.Length;
                Assert.AreEqual(expected, result);
                d[nameof(LL.SumAligned)] += sw.ElapsedTicks;

                sw.Restart();
                result = LL.SumPipelined(data);
                sw.Stop();
                expected = data.Length;
                Assert.AreEqual(expected, result);
                d[nameof(LL.SumPipelined)] += sw.ElapsedTicks;

                sw.Restart();
                result = LL.SumAlignedPipelined(data);
                sw.Stop();
                expected = data.Length;
                Assert.AreEqual(expected, result);
                d[nameof(LL.SumAlignedPipelined)] += sw.ElapsedTicks;
            }

            foreach (var t in d)
            {
                var avg = TimeSpan.FromTicks(t.Value / numTests);
                Console.WriteLine($"{t.Key} took {TimeSpan.FromTicks(t.Value)} - Average: {avg}");
            }


        }

        const ulong AlignmentMask = 31UL;
        const int VectorSizeInInt64s = 4;
        const int BlockSizeInInts = 32;


        //[TestMethod]
        //public unsafe void RecipTDivideAvxTestPlus1()
        //{
        //    int limit = 1 << 20;
        //    List<int> composites = Composites.GenerateTo(limit).ToList();

        //    int[] primes = Primes.IntFactorPrimes;
        //    double[] recip = primes.Select(x => 1.0 / x).ToArray();

        //    var err = 0.00000001;
        //    int recipCount = 0;

        //    int batchSize = 4; // Process four reciprocals at a time
        //    int numBatches = recip.Length / batchSize;

        //    Vector256<double> errVector = Vector256.Create((double)err);

        //    fixed (int* primePtr = &primes[0])
        //    fixed (double* recipPtr = &recip[0])
        //    {
        //        var aligned = (double*)(((ulong)recipPtr + AlignmentMask) & ~AlignmentMask);
        //        foreach (var composite in composites)
        //        {
        //            // Load four composite values into a Vector256<double>
        //            Vector256<double> compositeVector = Vector256.Create((double)composite);

        //            // Process four reciprocals at a time
        //            for (int batchIndex = 0; batchIndex < numBatches; batchIndex++)
        //            {
        //                // Load four recip values into a Vector256<double>
        //                Vector256<double> recipVec = Avx.LoadAlignedVector256(aligned + batchIndex * VectorSizeInInts);

        //                // Multiply the reciprocals with the composite in parallel
        //                Vector256<double> mulVec = Avx.Multiply(recipVec, compositeVector);
        //                Vector256<double> addErrVec = Avx.Add(mulVec, errVector);

        //                // Convert the addErrVec to a Vector256<int> using shuffles and blends
        //                Vector256<int> floorVec = ConvertToVectorInt32(addErrVec);

        //                // Load four primes into a Vector256<int>
        //                Vector256<int> primeVec = Avx.LoadAlignedVector256(primePtr + batchIndex * VectorSizeInInts);

        //                // Multiply the floorVec with the primeVec
        //                Vector256<int> mulPrime = MultiplyInt32(floorVec, primeVec);

        //                // Compare the mulPrime with the primeVec
        //                Vector256<int> cmpVec = Avx.Compare(mulPrime, primeVec, FloatComparisonMode.OrderedEqualNonSignaling);

        //                // Extract the mask from the comparison vector
        //                int mask = Avx.MoveMask(cmpVec.AsDouble());

        //                // Update the recipCount based on the mask
        //                recipCount += BitOperations.PopCount((uint)mask);
        //            }
        //        }
        //    }
        //}

        //private Vector256<int> ConvertToVectorInt32(Vector256<double> vector)
        //{
        //    // Convert the double vector to a long vector
        //    Vector256<long> longVector = Avx2.ConvertToVector256Int64(vector);

        //    // Extract the lower 128 bits of the long vector
        //    Vector128<long> lowerLongVector = Avx2.ExtractVector128(longVector, 0);

        //    // Convert the lower long vector to an int vector
        //    Vector128<int> intVector = Sse2.ConvertToInt32(lowerLongVector);


        //    // Zero - extend the int vector to a 256 - bit vector
        //    Vector256<int> extendedIntVector = Avx2.ZeroExtend(intVector);

        //    // Broadcast the extended int vector to all 8 elements
        //    Vector256<int> finalVector = Avx2.BroadcastScalarToVector256(extendedIntVector);

        //    return finalVector;
        //}

        //private Vector256<int> MultiplyInt32(Vector256<int> vector1, Vector256<int> vector2)
        //{
        //    // Multiply the two int vectors
        //    Vector256<long> longVector = Avx2.Multiply(vector1.AsInt64(), vector2.AsInt64());

        //    // Convert the long vector to an int vector
        //    Vector256<int> intVector = Avx2.ConvertToInt32(longVector);

        //    return intVector;
        //}








        const int recipPowLimit = 16;
        const int testLimit = 1 << recipPowLimit;

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public unsafe void RecipTDivideAvxTestPlus1()
        {
            int limit = testLimit;
            int[] composites = Composites.GenerateTo(limit).ToArray();

            // Tests for all compsite of up 1<<DISCRIMINATOR_BITS
            //  1.  n / p  ==  n * 1/p + (err) 
            //  2.  given 1, set a = n * 1/p + (err). note a is a whole number 
            //      verify when  n % p = 0  == a & (n/a)


            int[] primes = Primes.IntFactorPrimes;

            double[] recip = primes.Select(x => 1.0 / x).ToArray();
            double[] primesd = primes.Select(x => (double)x).ToArray();

            MathLib.Align(ref primes);
            MathLib.Align(ref recip);
            MathLib.Align(ref primesd);
            MathLib.Align(ref composites);

            var err = 0.00000001;


            int recipCount = 0;


            //sw.Restart();



            int batchSize = 4; // Process four reciprocals at a time
            int numBatches = recip.Length / batchSize;
            int primeBatches = primes.Length / batchSize;
            Vector256<double> errVector = Vector256.Create((double)err);

            int cCount = 0;
            //fixed (int* primePtr = &primes[0])
            var sw = Stopwatch.StartNew();
            int pCount = 0;
            fixed (double* primedPtr = &primesd[0])
            fixed (double* recipPtr = &recip[0])
            {

                var aligned = (double*)(((ulong)recipPtr + AlignmentMask) & ~AlignmentMask);
                var pos = (int)(aligned - recipPtr);
                if (pos != 0)
                {
                    Assert.AreEqual(0, pos, "Failed to align array");
                }
                foreach (var composite in composites)
                {
                    cCount++;
                    //handle unaligned primes

                    //convert the following to paralle avx instructions
                    //  var mul = ((int)composite * recip + err);
                    //  var actual = mul * p == c;
                    //  if (actual) recipCount++;
                    // load up four recipricols
                    Vector256<double> compositeVector = Vector256.Create((double)composite);

                    for (int batchIndex = 0; batchIndex < numBatches; batchIndex++)
                    {

                        // Load four recip values into a Vector256<double>

                        //for debugging:
                        //Vector256<int> primeVec = Avx.LoadAlignedVector256(primePtr + pos + (batchIndex * VectorSizeInInts));
                        Vector256<double> primedVec = Avx.LoadAlignedVector256(primedPtr + (batchIndex * VectorSizeInInt64s));

                        pCount += 4;
                        // doing ((int)composite * recip + err) 4 elements at a time, but prime vect is 8 at a time.
                        //this throws exception if not aligned
                        Vector256<double> recipVec = Avx.LoadAlignedVector256(recipPtr + (batchIndex * VectorSizeInInt64s));

                        Vector256<double> mulVec = Avx.Multiply(recipVec, compositeVector);
                        Vector256<double> addErrVec = Avx.Add(mulVec, errVector);
                        Vector256<double> flooredDVec = Avx.Floor(addErrVec);

                        var mulFloored = Avx.Multiply(flooredDVec, primedVec);



                        var eqVector = Avx.Compare(mulFloored, compositeVector, FloatComparisonMode.UnorderedEqualNonSignaling);
                        var m = Avx.MoveMask(eqVector);

                        var divCount = MathLib.PopCount(m);
                        recipCount += divCount;


                        //TODO: Cast addErrVec to vector of int
                        //Vector256<long> floorVec = Avx.Floor(addErrVec).AsInt64();

                        //Vector256<int> primeVec = Avx.LoadAlignedVector256(primePtr + batchIndex * VectorSizeInInts);

                        // appears you can't elementwise multiply, only broadcast
                        //Vector128<int> mulPrime = Avx.Multiply(floorVec, primeVec);
                        //Vector256<int> cmpVec = Avx.Compare(mulPrime, primeVec, FloatComparisonMode.OrderedEqualNonSignaling);


                        // Extract the mask from the comparison vector
                        //int mask = 0;// Avx.MoveMask(cmpVec.AsDouble());


                        // Update the recipCountArr based on the mask
                        //recipCount += BitOperations.PopCount((uint)mask);

                    }

                    //todo: handle unaligned primes for composite
                }
            }
            sw.Stop();
            //Control found 170239 divisors in 65515 composites up to 65536 in 00:00:03.8926143
            Console.WriteLine($"AVX found {recipCount} divisors with {pCount} primes in {cCount} composites up to {limit} in {sw.Elapsed}");


        }


#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public unsafe void RecipTDivideAvxTestPlus1Debug()
        {
            int limit = testLimit;
            int[] composites = Composites.GenerateTo(limit).ToArray();

            // Tests for all compsite of up 1<<DISCRIMINATOR_BITS
            //  1.  n / p  ==  n * 1/p + (err) 
            //  2.  given 1, set a = n * 1/p + (err). note a is a whole number 
            //      verify when  n % p = 0  == a & (n/a)


            int[] primes = Primes.IntFactorPrimes;

            double[] recip = primes.Take(limit).Select(x => 1.0 / x).ToArray();
            double[] primesd = primes.Take(limit).Select(x => (double)x).ToArray();


            MathLib.Align(ref primes);
            MathLib.Align(ref recip);
            MathLib.Align(ref primesd);
            MathLib.Align(ref composites);

            var err = 0.00000001;


            int recipCount = 0;


            //sw.Restart();



            int batchSize = 4; // Process four reciprocals at a time
            int numBatches = recip.Length / batchSize;
            int primeBatches = primes.Length / batchSize;
            Vector256<double> errVector = Vector256.Create((double)err);

            int cCount = 0;
            //fixed (int* primePtr = &primes[0])



            int timeRecipCount = 0;
            int timeCCount = 0;
            int timePCount = 0;


            int pCount = 0;



            var sw = Stopwatch.StartNew();
            var timeSw = Stopwatch.StartNew();
            fixed (double* primedPtr = &primesd[0])
            fixed (double* recipPtr = &recip[0])
            {

                var aligned = (double*)(((ulong)recipPtr + AlignmentMask) & ~AlignmentMask);
                var pos = (int)(aligned - recipPtr);
                if (pos != 0)
                {
                    Assert.AreEqual(0, pos, "Failed to align array");
                }
                foreach (var composite in composites)
                {
                    int debugRestoreTimeCCount = timeCCount;
                    int debugRestoreCCount = cCount;

                    timeCCount = debugRestoreTimeCCount;
                    cCount = debugRestoreCCount;

                    timeCCount++;
                    cCount++;
                    //handle unaligned primes

                    //convert the following to paralle avx instructions
                    //  var mul = ((int)composite * recip + err);
                    //  var actual = mul * p == c;
                    //  if (actual) recipCount++;
                    // load up four recipricols
                    Vector256<double> compositeVector = Vector256.Create((double)composite);
                    int foundVDisors = 0;
                    List<string> debugMessages = new();
                    for (int batchIndex = 0; batchIndex < numBatches; batchIndex++)
                    {

                        // Load four recip values into a Vector256<double>

                        //for debugging:
                        //Vector256<int> primeVec = Avx.LoadAlignedVector256(primePtr + pos + (batchIndex * VectorSizeInInts));
                        Vector256<double> primedVec = Avx.LoadAlignedVector256(primedPtr + (batchIndex * VectorSizeInInt64s));

                        pCount += 4;
                        // doing ((int)composite * recip + err) 4 elements at a time, but prime vect is 8 at a time.
                        //this throws exception if not aligned
                        Vector256<double> recipVec = Avx.LoadAlignedVector256(recipPtr + (batchIndex * VectorSizeInInt64s));

                        //mul= composite*recip;
                        Vector256<double> mulVec = Avx.Multiply(recipVec, compositeVector);
                        //mul= composite*recip + err;
                        Vector256<double> addErrVec = Avx.Add(mulVec, errVector);
                        //mul= (int)(composite*recip + err;)
                        Vector256<double> flooredDVec = Avx.Floor(addErrVec);

                        //var actual = mul * p;
                        var mulFloored = Avx.Multiply(flooredDVec, primedVec);

                        //var actual = mul * p == c;
                        var eqVector = Avx.Compare(mulFloored, compositeVector, FloatComparisonMode.UnorderedEqualNonSignaling);
                        var m = Avx.MoveMask(eqVector);

                        var divCount = MathLib.PopCount(m);
                        foundVDisors += divCount;

                        string mulFlooredS = mulFloored.ToString();
                        string primeS = primedVec.ToString();

                        var debugMessage = $"found {divCount} divisors using primes {primeS} for {mulFlooredS} for Composite {composite}";
                        debugMessages.Add(debugMessage);

                    }
                    sw.Stop();


                    int foundDebugDivisors = 0;
                    timeSw.Start();

                    for (var i = 0; i < recip.Length; i++)
                    {
                        timePCount++;

                        var timePrime = primes[i];
                        var timeRecip = recip[i];

                        //var mul = (int)(((c * recip) - c) + err);
                        var timeMul = (int)(composite * timeRecip + err);
                        var timeActual = timeMul * timePrime == composite;
                        if (timeActual)
                        {
                            foundDebugDivisors++;
                        }
                    }
                    timeSw.Stop();

                    if (foundVDisors != foundDebugDivisors)
                    {
                        var debugMessageString = string.Join("\n", debugMessages);
                        string message = $"Avx found {foundVDisors} divisors and debug found {foundDebugDivisors}";
                        Assert.AreEqual(foundVDisors, foundDebugDivisors, message);
                    }
                    debugMessages.Clear();

                    recipCount += foundVDisors;
                    timeRecipCount += foundDebugDivisors;
                }
            }
            sw.Stop();
            //Control found 170239 divisors in 65515 composites up to 65536 in 00:00:03.8926143
            Console.WriteLine($"AVX found {recipCount} divisors with {pCount} primes in {cCount} composites up to {limit} in {sw.Elapsed}");
            timeSw.Stop();
            Console.WriteLine($"Rec found {timeRecipCount} divisors with {timePCount} primes in {timeCCount} composites up to {limit} in {timeSw.Elapsed}");


        }

#if SKIP_LONG_TESTS
        [Ignore]
#else

#endif
        [TestMethod]
        public void TimeRecip()
        {
            int limit = testLimit;
            int[] composites = Composites.GenerateTo(limit).ToArray();
            var err = 0.00000001;
            int[] primes = Primes.IntFactorPrimes;
            double[] recip = primes.Select(x => 1.0 / x).ToArray();


            int timeRecipCount = 0;
            int timeCCount = 0;
            int timePCount = 0;
            var timeSw = Stopwatch.StartNew();


            foreach (var c in composites)
            {
                timeCCount++;
                for (var i = 0; i < recip.Length; i++)
                {
                    timePCount++;

                    var timePrime = primes[i];
                    var timeRecip = recip[i];

                    //var mul = (int)(((c * recip) - c) + err);
                    var timeMul = (int)(c * timeRecip + err);
                    var timeActual = timeMul * timePrime == c;
                    if (timeActual)
                    {
                        timeRecipCount++;
                    }

                }
            }
            timeSw.Stop();
            Console.WriteLine($"Rec found {timeRecipCount} divisors with {timePCount} primes in {timeCCount} composites up to {limit} in {timeSw.Elapsed}");

        }



#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void TimeMod()
        {
            int limit = testLimit;
            int[] composites = Composites.GenerateTo(limit).ToArray();
            var err = 0.00000001;
            int[] primes = Primes.IntFactorPrimes;
            double[] recip = primes.Select(x => 1.0 / x).ToArray();


            int timeModCount = 0;
            int timeCCount = 0;
            int timePCount = 0;
            var timeSw = Stopwatch.StartNew();


            foreach (var c in composites)
            {
                timeCCount++;
                for (var i = 0; i < recip.Length; i++)
                {
                    timePCount++;

                    var timePrime = primes[i];
                    var timeActual = c % timePrime == 0;
                    if (timeActual) timeModCount++;
                }
            }
            timeSw.Stop();
            Console.WriteLine($"Rec found {timeModCount} divisors with {timePCount} primes in {timeCCount} composites up to {limit} in {timeSw.Elapsed}");

        }

        /*
           Ctl found 165490 divisors with 282689664 primes in 58992 composites up to 65536 in 00:00:01.0651085
            Rec found 165490 divisors with 282689664 primes in 58992 composites up to 65536 in 00:00:00.5396997
            Mod found 165490 divisors with 282689664 primes in 58992 composites up to 65536 in 00:00:01.0607633
           */

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void RecipTDivideTestPlus1()
        {
            int limit = testLimit;
            var gen = Composites.GenerateTo(limit).ToList();
            var count = PrimeData.Counts[recipPowLimit].Count;
            var expectedCompositeCount = limit - count - 2;
            var actualCompositeCount = gen.Count;
            Assert.AreEqual(expectedCompositeCount, actualCompositeCount);

            // Tests for all compsite of up 1<<DISCRIMINATOR_BITS
            //  1.  n / p  ==  n * 1/p + (err) 
            //  2.  given 1, set a = n * 1/p + (err). note a is a whole number 
            //      verify when  n % p = 0  == a & (n/a)


            var primes = Primes.IntFactorPrimes;
            var p_recip = primes.Select(x => new { Prime = x, Recip = (1.0 / x) }).ToArray();
            var err = 0.00000001;

            int controlCount = 0;
            int cCount = 0;
            int pCount = 0;
            var sw = Stopwatch.StartNew();
            foreach (var c in gen)
            {
                cCount++;
                for (var i = 0; i < p_recip.Length; i++)
                {
                    pCount++;
                    var kvp = p_recip[i];
                    var p = kvp.Prime;
                    var recip = kvp.Recip;
                    //var mul = (int)(((c * recip) - c) + err);
                    var mul = (int)(c * recip + err);

                    int mod = c % p;
                    var expected = mod == 0;
                    var actual = (int)mul * p == c;
                    if (actual)
                        controlCount++;

                    if (expected != actual)
                    {
                        var message = $"Test failed for ({c} * {recip} - {c}) + {err})";
                        Assert.AreEqual(expected, actual, message);
                    }
                }
            }
            Console.WriteLine($"Ctl found {controlCount} divisors with {pCount} primes in {cCount} composites up to {limit} in {sw.Elapsed}");


            int recipCount = 0;
            cCount = 0;
            pCount = 0;
            sw.Restart();
            foreach (var c in gen)
            {
                cCount++;
                for (var i = 0; i < p_recip.Length; i++)
                {
                    pCount++;
                    var kvp = p_recip[i];
                    var p = kvp.Prime;
                    var recip = kvp.Recip;
                    //var mul = (int)(((c * recip) - c) + err);
                    var mul = (int)(c * recip + err);
                    var actual = mul * p == c;
                    if (actual)
                    {
                        recipCount++;
                    }

                }
            }
            sw.Stop();
            Console.WriteLine($"Rec found {recipCount} divisors with {pCount} primes in {cCount} composites up to {limit} in {sw.Elapsed}");

            int modCount = 0;
            cCount = 0;
            pCount = 0;
            sw.Restart();
            foreach (var c in gen)
            {
                cCount++;
                for (var i = 0; i < p_recip.Length; i++)
                {
                    pCount++;
                    var kvp = p_recip[i];
                    var p = kvp.Prime;
                    var recip = kvp.Recip;
                    var actual = c % p == 0;
                    if (actual) modCount++;
                }
            }
            sw.Stop();
            Console.WriteLine($"Mod found {modCount} divisors with {pCount} primes in {cCount} composites up to {limit} in {sw.Elapsed}");
        }


        /*
            Ctl found 165490 divisors with 282689664 primes in 58992 composites up to 65536 in 00:00:01.0651085
            Rec found 165490 divisors with 282689664 primes in 58992 composites up to 65536 in 00:00:00.5396997
            Mod found 165490 divisors with 282689664 primes in 58992 composites up to 65536 in 00:00:01.0607633
            */

#if SKIP_LONG_TESTS
        [Ignore]
#endif      
        [TestMethod]
        public void RecipTDivideTest()
        {

            int DISCRIMINATOR_BITS = 15;
            int limit = 1 << DISCRIMINATOR_BITS;



            double err = 1.0 / (1 << DISCRIMINATOR_BITS);
            /*
            // for 2^16 delta checks fail starting at 2^15, but conditions still hold for test 1 and 2.
            // So you can squeeze higher bits at the expense of additionally checking
            //      VALID: With Discriminator 2^15 and checking composites up to 2^16
            //          => Up to 2^15, this holds
            //              DividesExact = Cieling(n*inverse(p)) -  n*inverse(p)) < error
            //          => Upto 2^16,
            //              checking n*inverse(p)) < error  and:
            //              For remainder=0  check:  delta <= || (n*inverse(p) * (c / n*inverse(p))) != n
            //              For remainder!=0 check:  delta < error (n*inverse(p) * (c / n*inverse(p)))==n
                    => issue is still need division to verify correctnesss
             */

            var gen = Composites.GenerateTo(65536);

            // Tests for all compsite of up 1<<DISCRIMINATOR_BITS
            //  1.  n / p  ==  n * 1/p + (err) 
            //  2.  given 1, set a = n * 1/p + (err). note a is a whole number 
            //      verify when  n % p = 0  == a & (n/a)


            var primes = Primes.IntFactorPrimes;
            var p_recip = primes.Select(x => 1.0 / x).ToArray();
            int n, res_p;
            int p;
            int div_p;
            int recip_n, recip_n_ciel;
            double r, res_r, temp_res_r;


            // first failure:  1031 => 0.9990300678952474
            //err = 1 - 0.9990300678952474;
            const double ROUND_UP_DOUBLE = 0.9999999665;
            //0100 00111 11 00000000000000000000000000000000000000000000000000000
            foreach (var c in gen)
            {
                for (var i = 0; i < primes.Length; i++)
                {
                    p = primes[i];
                    if (c <= p)
                        continue;
                    r = p_recip[i];

                    res_p = c % p;
                    div_p = c / p;
                    temp_res_r = c * r;
                    res_r = temp_res_r + err;
                    recip_n = (int)(res_r);
                    recip_n_ciel = (int)(temp_res_r + ROUND_UP_DOUBLE);
                    double delta = recip_n_ciel - res_r;
                    bool dividesExact = delta < err;
                    if (recip_n != div_p)
                    {
                        Assert.AreEqual(div_p, recip_n, $"Division failed for {c}/{p}={div_p} != raw: {temp_res_r}");
                    }

                    if ((res_p == 0) != dividesExact)
                    {
                        //string message = $"Delta Check Failed: {c} % {p} == {res_p} with {delta} for {res_r}";
                        //Console.WriteLine(message);
                    }
                    res_r = temp_res_r + ROUND_UP_DOUBLE;
                    if ((res_p == 0) && (delta >= err || (recip_n * (c / recip_n)) != c))
                    //if ((res_p == 0) != ((int)temp_res_r == recip_n))
                    {
                        string message = $"Mod 0 Check Failed: {c} % {p} == {res_p} with (double)recip_n {recip_n} != (int){(int)res_r}";
                        Console.WriteLine(message);
                        break;
                        //Assert.AreEqual((int)temp_res_r, (int)res_r, "Division failed for {p}");
                    }
                    else if ((res_p != 0) && delta < err && (recip_n_ciel * (c / recip_n_ciel)) == c)
                    {
                        string message = $"Mod != 0 Check Failed: {c} % {p} == {res_p} with (double)recip_n {recip_n_ciel} * (int){(int)recip_n_ciel} = 0 mod {c} - {delta}";
                        Console.WriteLine(message);
                        break;
                    }

                }
            }


        }
    }
}