#define SKIP_LONG_TESTS
#define HAVE_CUDA
#undef HAVE_CUDA
using BenchmarkDotNet.Mathematics;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.NVRTC;
using MathGmp.Native;
using Microsoft.Diagnostics.Runtime.Utilities;
using Microsoft.Diagnostics.Tracing.Analysis;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.VisualStudio.CodeCoverage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Perfolizer.Mathematics.Functions;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace HigginsSoft.Math.Lib.Tests
{

#if HAVE_CUDA
    [TestClass]
#endif
    public class RecipDivideCudaTests
    {
        [TestMethod]
        public void IntModDivideCudaTest()
        {

            int numElements = 40000;

            var cuda = Cuda.Instance;
            var kernel = cuda.GetOrCompile(CudaKernels.VecMod32, nameof(CudaKernels.VecMod32));

            kernel.GridDimensions = (numElements + 255) / 256;
            kernel.BlockDimensions = 256;

            // Allocate input vectors h_A and h_B in host memory
            var gen = Composites.GenerateAll(numElements);
            var composites = gen.ToArray();
            var empty = new int[numElements];
            int[] h_A = composites.ToArray();
            //float[] h_A  =  new float[numElements];
            //float[] h_B = new float[numElements];
            int[] h_B = h_A.Select(x => 2).ToArray();
            // TODO: Initialize input vectors h_A, h_B
            Random rand = new Random(0);






            // Allocate vectors in device memory and copy vectors from host memory to device memory 
            CudaDeviceVariable<int> d_A = h_A;
            CudaDeviceVariable<int> d_B = h_B;
            CudaDeviceVariable<int> d_C = new CudaDeviceVariable<int>(numElements);

            // Invoke kernel
            kernel.Run(d_A.DevicePointer, d_B.DevicePointer, d_C.DevicePointer, numElements);

            // Copy result from device memory to host memory
            // h_C contains the result in host memory
            int[] h_C = d_C;

            for (int i = 0; i < numElements; ++i)
            {
                if (h_A[i] % h_B[i] != h_C[i])
                {
                    var expected = h_A[i] % h_B[i];
                    var actual = h_C[i];
                    Assert.AreEqual(expected, actual, "Result verification failed at element {0}!\n", i);
                    return;
                }
            }

            kernel = cuda.GetOrCompile(CudaKernels.VecMod32I, nameof(CudaKernels.VecMod32I));


            d_C = empty;
            kernel.SetComputeSize((uint)numElements, 1);
            kernel.Run(d_A.DevicePointer, 3, d_C.DevicePointer, numElements);

            h_C = d_C;

            for (int i = 0; i < numElements; ++i)
            {
                if (h_A[i] % 3 != h_C[i])
                {
                    var expected = h_A[i] % h_B[i];
                    var actual = h_C[i];
                    if (expected != actual)
                    {
                        d_A.Dispose();
                        d_B.Dispose();

                        d_C.Dispose();
                        Assert.AreEqual(expected, actual, "Result verification failed at element {0}!\n", i);
                    }

                    //return;
                }
            }

            d_A.Dispose();
            d_B.Dispose();
            d_C.Dispose();

        }
        [TestMethod]
        public void VectAddTest()
        {
            int numElements = 50000;

            var cuda = Cuda.Instance;
            var kernel = cuda.GetOrCompile(CudaKernels.VecAdd, nameof(CudaKernels.VecAdd));

            kernel.GridDimensions = (numElements + 255) / 256;
            kernel.BlockDimensions = 256;

            // Allocate input vectors h_A and h_B in host memory
            float[] h_A = new float[numElements];
            float[] h_B = new float[numElements];

            // TODO: Initialize input vectors h_A, h_B
            Random rand = new Random(0);

            // Initialize the host input vectors
            for (int i = 0; i < numElements; ++i)
            {
                h_A[i] = (float)rand.NextDouble();
                h_B[i] = (float)rand.NextDouble();
            }




            // Allocate vectors in device memory and copy vectors from host memory to device memory 
            CudaDeviceVariable<float> d_A = h_A;
            CudaDeviceVariable<float> d_B = h_B;
            CudaDeviceVariable<float> d_C = new CudaDeviceVariable<float>(numElements);

            // Invoke kernel
            kernel.Run(d_A.DevicePointer, d_B.DevicePointer, d_C.DevicePointer, numElements);

            // Copy result from device memory to host memory
            // h_C contains the result in host memory
            float[] h_C = d_C;

            for (int i = 0; i < numElements; ++i)
            {
                if (MathLib.Abs(h_A[i] + h_B[i] - h_C[i]) > 1e-5)
                {
                    var expected = h_A[i] + h_B[i];
                    var actual = h_C[i];
                    Assert.AreEqual(expected, actual, "Result verification failed at element {0}!\n", i);
                    return;
                }
            }

        }


        void QsTDiv(int[] n, int[] bsmooth, int[] gf2, int[] div, int[] primes, int N, int P)
        {
            int i = 0; // blockDim.x * blockIdx.x + threadIdx.x;
            if (i < N)
            {
                int mask = 0;
                for (var j = 0; n[i] > 1 && j < P; j++)
                {
                    mask = 1 << j;
                    while (n[i] % primes[j] == 0)
                    {
                        n[i] /= primes[j];
                        gf2[i] ^= mask;
                        div[i] |= mask;
                    }
                }
                bsmooth[i] = n[i] == 1 ? 1 : 0;
            }
        }

        [TestMethod]
        public void QsTivCudaTest()
        {

            int numElements = 40000;

            var cuda = Cuda.Instance;
            var kernel = cuda.GetOrCompile(CudaKernels.QsTDiv, nameof(CudaKernels.QsTDiv));

            kernel.GridDimensions = (numElements + 255) / 256;
            kernel.BlockDimensions = 256;

            // Allocate input vectors h_A and h_B in host memory
            var gen = Composites.GenerateAll(numElements);
            var composites = gen.ToArray();
            var empty = new int[numElements];


            int[] h_n = composites.ToArray();

            int numPrimes = 32;
            int[] primes = Primes.IntFactorPrimes.Take(numPrimes).ToArray();


            // Allocate vectors in device memory and copy vectors from host memory to device memory 
            CudaDeviceVariable<int> d_N = h_n;
            CudaDeviceVariable<int> d_BSmooth = new CudaDeviceVariable<int>(numElements);
            CudaDeviceVariable<int> d_Gf2Masks = new CudaDeviceVariable<int>(numElements);
            CudaDeviceVariable<int> d_DivMasks = new CudaDeviceVariable<int>(numElements);
            CudaDeviceVariable<int> d_Primes = primes;
            // Invoke kernel
            var sw = Stopwatch.StartNew();
            kernel.Run(d_N.DevicePointer,
                d_BSmooth.DevicePointer,
                d_Gf2Masks.DevicePointer,
                d_DivMasks.DevicePointer,
                d_Primes.DevicePointer,
                numElements,
                numPrimes);
            sw.Stop();
            // Copy result from device memory to host memory
            // h_C contains the result in host memory

            void Dispose()
            {

                d_N.Dispose();
                d_BSmooth.Dispose();
                d_Gf2Masks.Dispose();
                d_DivMasks.Dispose();
                d_Primes.Dispose();
            }

            int[] h_C = d_N;
            int[] h_bsmooth = d_BSmooth;
            int[] h_gf2_masks = d_Gf2Masks;
            int[] h_div_masks = d_DivMasks;
            var nWatch = Stopwatch.StartNew();
            for (int i = 0; i < numElements; i++)
            {
                var n = composites[i];
                var gfMask = 0;
                var divMask = 0;
                var isBSmooth = 0;

                for (var j = 0; n > 1 && j < numPrimes; j++)
                {
                    int p = primes[j];
                    var mask = 1 << j;
                    while (n % p == 0)
                    {
                        n /= p;
                        divMask = divMask | mask;
                        gfMask = gfMask ^ mask;
                    }
                }
                if (n == 1)
                {
                    isBSmooth = 1;
                }

                var actualN = h_C[i];
                var actualGfMask = h_gf2_masks[i];
                var actualDivMask = h_div_masks[i];
                var actualBSmooth = h_bsmooth[i];

                if (actualN != n || actualGfMask != gfMask || actualDivMask != divMask || actualBSmooth != isBSmooth)
                {
                    Dispose();

                    Assert.AreEqual(n, actualN, "N failed");
                    Assert.AreEqual(gfMask, actualGfMask, "Gf2 mask failed");
                    Assert.AreEqual(divMask, actualDivMask, "Div mask failed");
                    Assert.AreEqual(isBSmooth, actualBSmooth, "BSmooth  failed");
                }
            }
            nWatch.Stop();

            Console.WriteLine($"Factored {numElements.ToString("N0")} with {numPrimes} primes. Cuda: {sw.Elapsed} - Native: {nWatch.Elapsed}");
            Dispose();
        }


        [TestMethod]
        public void QsTivCudaRootTest()
        {



            var cuda = Cuda.Instance;
            var kernel = cuda.GetOrCompile(CudaKernels.QsTDiv, nameof(CudaKernels.QsTDiv));



            // Allocate input vectors h_A and h_B in host memory

            int limit = 26;
            var composites = Composites.GenerateTo(1 << limit).ToArray();//.Skip(1 << 30).ToArray();
            int numElements = composites.Length;
            kernel.GridDimensions = (numElements + 255) / 256;
            kernel.BlockDimensions = 256;
            //var composites = gen.ToArray();
            var empty = new int[numElements];


            int[] h_n = composites.ToArray();

            int numPrimes = 32;
            int[] primes = Primes.IntFactorPrimes.Take(numPrimes).ToArray();


            // Allocate vectors in device memory and copy vectors from host memory to device memory 
            CudaDeviceVariable<int> d_N = h_n;
            CudaDeviceVariable<int> d_BSmooth = new CudaDeviceVariable<int>(numElements);
            CudaDeviceVariable<int> d_Gf2Masks = new CudaDeviceVariable<int>(numElements);
            CudaDeviceVariable<int> d_DivMasks = new CudaDeviceVariable<int>(numElements);
            CudaDeviceVariable<int> d_Primes = primes;
            // Invoke kernel
            var sw = Stopwatch.StartNew();
            //kernel.SetComputeSize()
            kernel.Run(d_N.DevicePointer,
                d_BSmooth.DevicePointer,
                d_Gf2Masks.DevicePointer,
                d_DivMasks.DevicePointer,
                d_Primes.DevicePointer,
                numElements,
                numPrimes);
            sw.Stop();
            // Copy result from device memory to host memory
            // h_C contains the result in host memory

            void Dispose()
            {

                d_N.Dispose();
                d_BSmooth.Dispose();
                d_Gf2Masks.Dispose();
                d_DivMasks.Dispose();
                d_Primes.Dispose();
            }

            int[] h_C = d_N;
            int[] h_bsmooth = d_BSmooth;
            int[] h_gf2_masks = d_Gf2Masks;
            int[] h_div_masks = d_DivMasks;
            var nWatch = Stopwatch.StartNew();
            int bSmoothCount = 0;
            for (int i = 0; i < numElements; i++)
            {
                var n = composites[i];
                var gfMask = 0;
                var divMask = 0;
                var isBSmooth = 0;

                for (var j = 0; n > 1 && j < numPrimes; j++)
                {
                    int p = primes[j];
                    var mask = 1 << j;
                    while (n % p == 0)
                    {
                        n /= p;
                        divMask = divMask | mask;
                        gfMask = gfMask ^ mask;
                    }
                }
                if (n == 1)
                {
                    isBSmooth = 1;
                    bSmoothCount++;
                }

                var actualN = h_C[i];
                var actualGfMask = h_gf2_masks[i];
                var actualDivMask = h_div_masks[i];
                var actualBSmooth = h_bsmooth[i];

                if (actualN != n || actualGfMask != gfMask || actualDivMask != divMask || actualBSmooth != isBSmooth)
                {
                    Dispose();

                    Assert.AreEqual(n, actualN, "N failed");
                    Assert.AreEqual(gfMask, actualGfMask, "Gf2 mask failed");
                    Assert.AreEqual(divMask, actualDivMask, "Div mask failed");
                    Assert.AreEqual(isBSmooth, actualBSmooth, "BSmooth  failed");
                }
            }
            nWatch.Stop();

            Console.WriteLine($"Found {bSmoothCount.ToString("N0")} bsmooth composites among {numElements.ToString("N0")} composites up to 2^{limit} using {numPrimes} primes.");
            Console.WriteLine($" => Cuda: {sw.Elapsed}");
            Console.WriteLine($" => Native: {nWatch.Elapsed}");
            Dispose();
        }
    }

    [TestClass]
    public class KaratsubaTests
    {
        [TestMethod]
        public void KatasubaMultiplyTest()
        {
            var a = (BigInteger)MathLib.GenerateRandomBigInt(4096);
            var b = (BigInteger)MathLib.GenerateRandomBigInt(4096);
            var actual = MathLib.Karatsuba(a, b);
            var expected = a * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba multiply failed for {a} * {b}");
            }
        }

        [TestMethod]
        public void KatasubaSquareTest()
        {
            var a = (BigInteger)MathLib.GenerateRandomBigInt(4096);

            var actual = MathLib.KaratsubaSquare(a);
            var expected = a * a;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a}");
            }
        }

        [TestMethod]
        public void AvxCMulTest()
        {

            uint[] a = { 5, 7 };
            uint[] b = { 11, 13 };

            BigInteger bigA = MathLib.ToBigInteger(a);
            BigInteger bigb = MathLib.ToBigInteger(b);
            var expected = bigA * bigb;

            var actualWords = MathLib.AVXZ.AvxMul(a[0], a[1], b[0], b[1]);
            var actual = MathLib.ToBigInteger(actualWords) + 0; // +0 to normalize any leading zero bits

            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba AvxMul failed for {a}");
            }
        }


        [TestMethod]
        public void KatasubaSquare2Test()
        {
            var words = new uint[] { 8, 9, 4, 3 };

            var a = MathLib.ToBigInteger(words);
            var actual = MathLib.KaratsubaSquare(a, 32);
            var expected = a * a;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a}");
            }
        }


        [TestMethod]
        public void KatasubaMultiplyOverflowTest()
        {
            BigInteger a = 1;
            a <<= 64;
            a -= 1;

            var actual = MathLib.Karatsuba(a, a, 32);
            var expected = a * a;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a}");
            }
        }

        [TestMethod]
        public void KatasubaMultiplyOverflow2Test()
        {
            BigInteger a = 1;
            a <<= 65;
            a -= 1;


            var actual = MathLib.Karatsuba(a, a, 32);
            var expected = a * a;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a}");
            }
        }



        [TestMethod]
        public void KatasubaSquareOverflowTest()
        {
            BigInteger a = 1;
            a <<= 64;
            a -= 1;

            var actual = MathLib.KaratsubaSquare(a, 32);
            var expected = a * a;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a}");
            }
        }

        [Ignore("Fix Me")]
        [TestMethod]
        public unsafe void KatasubaInlineDebugTest()
        {

            MathLib.AVXZ.Karatsuba.Test4x4DebugNative();


            var words = new uint[] { 1, 0x0000_000E };


            var a = MathLib.ToBigInteger(words);
            var b = MathLib.BigIntFromBits(1, words);
            var actual = MathLib.KaratsubaSquare(a, 32);
            var expected = a * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a}");
            }
        }

        [TestMethod]
        public unsafe void KatasubaWordAlignedTest()
        {

            var wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15 };



            var a = MathLib.ToBigInteger(wordsA);
            var b = MathLib.ToBigInteger(wordsB);
            var actual = MathLib.KaratsubaWordAligned(a, b, 32);
            var expected = a * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a} * {b}");
            }
        }

        [TestMethod]
        public unsafe void KatasubaWordAligned2Test()
        {

            var wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15 };

            //wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            //wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15, 0, 1, 2, 3, 4, 5, 6, 7 };


            var a = MathLib.ToBigInteger(wordsA);
            var b = MathLib.ToBigInteger(wordsB);
            var actual = MathLib.KaratsubaWordAligned2(a, b, out _, out _, 32);
            var expected = a * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a} * {b}");
            }
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public unsafe void KatasubaWordAligned3Test()
        {

            var wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15 };
            // 8x8 (63) takes 21 (3*7) muls ~=1.46410

            wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15, 0, 1, 2, 3, 4, 5, 6, 7 };
            // 16x16 (256) takes 63 (9*7|3*3*7) muls ~= 16^ 1.4943


            wordsA = wordsA.Concat(wordsA).ToArray();
            wordsB = wordsB.Concat(wordsB).ToArray();
            //32 x 32 (1,024) takes 189 (27*7|3*3*3*7) muls -> 32^ 1.512448

            wordsA = wordsA.Concat(wordsA).ToArray();
            wordsB = wordsB.Concat(wordsB).ToArray();

            //64 x 64 (4,096)  takes 567(27*7|3*3*3*3*7) muls ~=  32^ 1.52453
            //2^6 = 3^4*7

            wordsA = wordsA.Concat(wordsA).ToArray();
            wordsB = wordsB.Concat(wordsB).ToArray();
            //128 x 128 (16,384) takes 1701 (3^5*7) muls ~= 128^ 1.1.5331
            //2^7 = 3^5*7

            wordsA = wordsA.Concat(wordsA).ToArray();
            wordsB = wordsB.Concat(wordsB).ToArray();
            //256 x 256 (65,536) takes 5103 (3^5*7) muls ~= 256^ 1.53964
            //2^8 = 3^6*7
            //2^20 = 1,048,576 (represents 2^32M) = (3^18)*7 = 2,711,943,423 = 1^20^1.56683

            //2^26 = (67,108,864 words * 67,108,864 words) =  (3^24)*7 =  1,977,006,755,367 = ~2 trillion ops

            wordsA = wordsA.Concat(wordsA).ToArray();
            wordsB = wordsB.Concat(wordsB).ToArray();
            wordsA = wordsA.Concat(wordsA).ToArray();
            wordsB = wordsB.Concat(wordsB).ToArray();
            //1024 x 1024 (1,048,576) takes 45927 (3^8*7) muls -> 256^ 1.539575 
            //2^10 = 3^8*7

            //ops - for 2^n*2^n = 3^(n-2)*7
            //2^32 = 3^30*7
            //2^1M ~= 3^1M*7

            var a = MathLib.ToBigInteger(wordsA);
            var b = MathLib.ToBigInteger(wordsB);
            var actual = MathLib.KaratsubaWordAligned3(a, b, 32) + 0;
            var expected = a * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba failed for {a} * {b} ({MathLib.FormatHex(a)} * {MathLib.FormatHex(b)}) - Actual: {MathLib.FormatHex(actual)} Expected: {MathLib.FormatHex(expected)} ");
            }
        }

        [TestMethod]
        public unsafe void KatasubaOOTest()
        {

            var wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15 };

            //wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            //wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15, 0, 1, 2, 3, 4, 5, 6, 7 };


            var a = MathLib.ToBigInteger(wordsA);
            var b = MathLib.ToBigInteger(wordsB);
            var actual = MathLib.KaratsubaOO(a, b, 32);
            var expected = a * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {a} * {b}");
            }
        }


        [TestMethod]
        public unsafe void KatasubaSquareWordAlignedTest()
        {

            //var wordsA = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var wordsB = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15 };

            var b = MathLib.ToBigInteger(wordsB);
            var actual = MathLib.KaratsubaSquareWordAligned(b, 32);
            var expected = b * b;
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Karatsuba Square failed for {b}");
            }
        }



    }


    [TestClass]
    public class LLMModMapTests
    {
        /*
         Routes:
        //for 2^p-1 there are p routes, each recievs p combinations of input bits.
                // each channel i recieves direct mod input of i and overflow mod input of i+p,
                // except channel[p-1] which only recieves direct input.
            for (var c=0; i<p; c++)
                channel[i].recieves = {c, c+p}

                // eg 0..1 for 5 bits, or 0..2 for bits.
                for(var k=0; k<=c;k++)
                {
                    channel[i].inputs.add(k, c-k); // direct channels
                }
                // calc mod channels.
                var overflowmod= i+p; // eg 2..4 for c=1 for 5 bits or 3..4 for c=2 for 5 bits
                for (var k=c; k<p-1;k++)
                {
                    channel[i].inputs.add(c, overflowmod-k); // overflow channels
                }
            // 64 million bytes, 64 million channels = 8,000 channels per gpu core.
            //	if each channel has 64M operations that's 512B operations per core, or 355.55555 seconds * 1440 MHz
            //		clock/memory: 1440 MHz, which can be boosted up to 1710 MHz, memory is running at 1188 MHz (19 Gbps effective).
            // old school: o(n^2) 2M ints 8000 gpu cores = 250 ops per gpu core 
            //	With 2 million ops = 500M total operations per core or 0.347 * 1440 mhz.
            //		more simply 2M^2/8000 ops.
            //		That doesn't considered cost for atomic add and carry.
            //	Classic karatsuba at o(n^1.585)
            //		ops = 9,708,062,041.60 / 1,213,507.75 ops per core/1400 = 0.0008427 ~= 8us per op.
            //	Karatsuba square:
                    karatsuba: 12 x 43
                        a2 =1 x 4 = 4
                        d2 = 2 * 3 = 6
                        = (1+2)*(4+3) - 4 - 6
                    karat square: 47
                        a = 4*4 (can use recursive square here)
                        b = 7*7 (can use recurisive here)
                        c = (4*4) * (7*7) - 16 - 29
                            note, I believe there is a short hand for c mult using difference of squares that doesn't involve old school.
                            49-16=33
                            49-33=16
                            33-16=17

            2 bits:
                indexes 0, 1 -> max= 1+1=2;
                0: recieves 0, 2 = index mod 2=0
                    0 0, 1 1
                1: recieves 1, 3 = index mod 2=1
                    0 1, 1 0
            3 bits:
                indexes 0, 1, 2 -> max= 2+2=4;
                0: recieves index mod 3=0  	=> 0, 3  
                    0 0, 1 2, 2 1, 
                1: recieves index mod 3=1 	=> 1, 4
                    1 0, 0 1, 2 2
                2: recieves index mod 3 = 2 => 2, 
                    0 1, 1 1, 2 0 

            5 bits:
                index 0 .. 4, max = 8
                0: recieve index mod 5 =0	=> 0, 5
                    0 0, 1 4, 2 3, 3 2, 4 1
                1: recieve index mod 5 = 1 	=> 1, 6
                    0 1, 1 0, 2 4, 3 3, 4 2
                2:
                    recieve index mod 5 = 2 => 2, 7
                    0 2, 1 1, 2 0, 3 4, 4 3
                3:
                    recieve index mod 5 = 3 => 3, 8
                    0 3, 1 2, 2 1, 3 0, 4 4
                4:
                    recieve index mod 5 = 4 => 4, (9)
                    0 4, 4 0, 1 3, 3 1, 2 2

            7 bits:
                index 0 .. 6, max = 12
                0: recieve index mod 5 =0	=> 0, 7
                    0 0, 1 6, 2 5, 3 4, 4 3, 5 2, 6 1
                1: recieve index mod 5 = 1 	=> 1, 8
                    0 1, 1 0, 2 6, 3 5, 5 3, 4 4, 6 2
                2:
                    recieve index mod 5 = 2 => 2, 9
                    0 2, 1 1, 2 0, 3 6, 4 5, 5 4, 6 3
                3:
                    recieve index mod 5 = 3 => 3, 10
                    0 3, 1 2, 2 1, 3 0, 4 6, 5 5, 6 4 
                4:
                    recieve index mod 5 = 4 => 4, 11
                    0 4, 1 3, 2 2, 3 1, 4 0, 5 6, 6 5	
                5:
                    recieve index mod 5 = 4 => 5, 12
                    0 5, 1 4, 2 3, 3 2, 4 1, 5 0, 6 6	
                6:
                    recieve index mod 5 = 4 => 6, (13)
                    0 6, 1 5, 2 4, 3 3, 4 2, 5 1, 6 0

            11 bits:
                index 0 .. 10, max = 20
                0: recieve index mod 5 =0	=> 0, 11
                    0 0, 1 6, 2 5, 3 4, 4 3, 5 2, 6 1
                1: recieve index mod 5 = 1 	=> 1, 8
                    0 1, 1 0, 2 4, 3 3, 4 2
                2:
                    recieve index mod 5 = 2 => 2, 9
                    0 2, 1 1, 2 0, 3 6, 4 5, 5 4, 6 3
                3:
                    recieve index mod 5 = 3 => 3, 10
                    0 3, 1 2, 2 1, 3 0, 4 6, 5 5, 6 4 
                4:
                    recieve index mod 5 = 4 => 4, 11
                    0 4, 1 3, 2 2, 3 1, 4 0, 5 6, 6 5	
                5:
                    recieve index mod 5 = 4 => 5, 12
                    0 5, 1 4, 2 3, 3 2, 4 1, 5 0, 6 6	
                6:
                    recieve index mod 5 = 4 => 6, (13)
                    0 6, 1 5, 2 4, 3 3, 4 2, 5 1, 6 0	
        */
        public class ModChannel
        {
            public List<(int low, int high)> Recieves = new();

            public List<List<(int i, int j)>> Inputs = new();
        }
        [TestMethod]
        public void PrintModMaps()
        {

            for (var i = 2; i < 32; i++)
            {
                if (!MathLib.IsPrime(i)) continue;
                var map = new ModChannel();
                int bits = i;
                for (var c = 0; c < bits; c++)
                {
                    var low = c;
                    var high = c + bits;
                    map.Recieves.Add(new(low, high));

                    map.Inputs.Add(new());
                    var channel = map.Inputs.Last();
                    // eg 0..1 for 5 bits, or 0..2 for bits.


                    for (var k = 0; k <= c; k++)
                    {
                        low = k;
                        high = c - k;
                        if ((low + high) % bits != c)
                        {
                            Assert.IsTrue((low + high) % bits == c);
                        }

                        channel.Add(new(low, high)); // direct channels
                    }
                    // calc mod channels.
                    var overflowmod = c + bits; // eg 2..4 for c=1 for 5 bits or 3..4 for c=2 for 5 bits
                    for (var k = c + 1; k < bits; k++)
                    {
                        low = k; high = overflowmod - k;
                        if ((low + high) % bits != c)
                        {
                            Assert.IsTrue((low + high) % bits == c);
                        }
                        channel.Add(new(low, high));  // overflow channels
                    }
                }


                //print summary:
                Console.WriteLine($"{bits} bits: ");
                int max = ((bits - 1) << 1);
                Console.WriteLine($"\tindex 0 .. {bits - 1}, max = {max}");

                for (var j = 0; j < map.Recieves.Count; j++)
                {
                    var recieve = map.Recieves[j];
                    Console.WriteLine($"\t{j}: recieves index mod {bits} = {j} \t=> {recieve.low}{(recieve.high > max ? "" : $", {recieve.high}")}");
                    Console.WriteLine($"\t\t input bits: {string.Join(", ", map.Inputs[j].Select(x => $"{x.i} + {x.j}"))}");
                }

            }

        }
    }
    [TestClass]
    public class LLM
    {
        [TestMethod]
        public void HigginsTestBig()
        {
            for (var i = 6; i < 1000; i += 6)
            {
                if (MathLib.IsPrime(i - 1))
                {

                    if (HigginsTestBig(i - 1))
                    {
                        Console.WriteLine("2^{0}-1 is Prime ", i - 1);
                    }
                }

                if (MathLib.IsPrime(i + 1))
                {
                    if (HigginsTestBig(i + 1))
                    {
                        Console.WriteLine("2^{0}-1 is Prime ", i + 1);
                    }
                }

            }
        }

        [TestMethod]
        public void HigginsTestGmp()
        {
            for (var i = 6; i < 1000; i += 6)
            {
                if (MathLib.IsPrime(i - 1))
                {

                    if (HigginsTestGmp(i - 1))
                    {
                        Console.WriteLine("2^{0}-1 is Prime ", i - 1);
                    }
                }

                if (MathLib.IsPrime(i + 1))
                {
                    if (HigginsTestGmp(i + 1))
                    {
                        Console.WriteLine("2^{0}-1 is Prime ", i + 1);
                    }
                }

            }
        }

        [Ignore("Fix Me")]
        [TestMethod]
        public void HigginsTestCuda()
        {
            for (var i = 6; i < 1000; i += 6)
            {
                if (MathLib.IsPrime(i - 1))
                {

                    if (HigginsTestCudaAtomic(i - 1))
                    {
                        Console.WriteLine("2^{0}-1 is Prime ", i - 1);
                    }
                }

                if (MathLib.IsPrime(i + 1))
                {
                    if (HigginsTestCudaAtomic(i + 1))
                    {
                        Console.WriteLine("2^{0}-1 is Prime ", i + 1);
                    }
                }

            }
        }

        [TestMethod]
        public void TestCalcMpLow()
        {
            for (var i = 6; i < 1000; i += 6)
            {
                if (MathLib.IsPrime(i - 1))
                {

                    TestCalcMpLow(i - 1);
                }

                if (MathLib.IsPrime(i + 1))
                {
                    TestCalcMpLow(i + 1);
                }

            }
        }


        private bool HigginsTestGmp(int p)
        {
            //GmpInt s = 3;
            //GmpInt mp = (1 << p) - 1;

            mpz_t sz = new(), mpz = new(), tz = new();

            gmp_lib.mpz_init_set_ui(sz, 3);


            gmp_lib.mpz_init_set_ui(mpz, 1);
            gmp_lib.mpz_mul_2exp(mpz, mpz, (uint)p);
            gmp_lib.mpz_sub_ui(mpz, mpz, 1);


            gmp_lib.mpz_init_set_ui(tz, 0);
            int cmp = 0;
            for (var j = 0; j <= p - 2; j++)
            {

                //s = s * s;
                gmp_lib.mpz_mul(sz, sz, sz);


                //while (s > mp)
                //{
                //    s = (s & mp) + (s >> p);
                //}
                //TODO: Test if this is faster than mod.
                cmp = gmp_lib.mpz_cmp(sz, mpz);
                while (cmp > 0)
                {
                    gmp_lib.mpz_tdiv_q_2exp(tz, sz, (uint)p);
                    gmp_lib.mpz_and(sz, sz, mpz);
                    gmp_lib.mpz_add(sz, sz, tz);
                    cmp = gmp_lib.mpz_cmp(sz, mpz);
                }
            }
            gmp_lib.mpz_sub_ui(tz, mpz, 3);
            cmp = gmp_lib.mpz_cmp(tz, sz);

            gmp_lib.mpz_clears(tz, mpz, sz, null);
            return cmp == 0;
        }

        private bool HigginsTestBig(int p)
        {
            BigInteger s = 3;
            BigInteger mp = BigInteger.Pow(2, p) - 1;
            for (var j = 0; j <= p - 2; j++)
            {
                s = s * s;
                while (s > mp)
                {
                    s = (s & mp) + (s >> p);
                }
            }
            return mp - 3 == s;
        }


        private unsafe void TestCalcMpLow(int p)
        {

            var msbBits = p & 31;
            var wordSize32 = 1 + (p >> 5);

            bool oddWordSize = (wordSize32 & 1) == 1;
            if (oddWordSize)
                wordSize32 += 1;


            int wordSize = sizeof(uint) * 8;
            int numInt32Words = (p + wordSize - 1) / wordSize;
            int numInt64Words = (p + sizeof(ulong) * 8 - 1) / (sizeof(ulong) * 8);

            //Console.WriteLine("Number of int32 words: " + numInt32Words);
            //Console.WriteLine("Number of int64 words: " + numInt64Words);

            mpz_t mpz = new mpz_t();
            gmp_lib.mpz_init_set_ui(mpz, 1);
            gmp_lib.mpz_mul_2exp(mpz, mpz, (uint)p);
            gmp_lib.mpz_sub_ui(mpz, mpz, 1);

            var cnt = GmpInt.One.BitLength;
            var limbs = gmp_lib.mpz_limbs_read(mpz);
            var limbData = limbs.Select(x => (ulong)x).ToArray();
            var words32 = new uint[limbData.Length << 1];

            Buffer.BlockCopy(limbData, 0, words32, 0, limbData.Length * sizeof(ulong));

            var h_mp1 = new ulong[limbData.Length << 1];

            for (var j = 0; j < words32.Length; j++)
            {
                h_mp1[j] = (ulong)words32[j];
            }


            var h_mp = new ulong[wordSize32];
            Array.Fill(h_mp, (ulong)uint.MaxValue);

            if (oddWordSize == false)
            {
                h_mp[h_mp.Length - 1] = (1ul << msbBits) - 1;
            }
            else
            {
                h_mp[h_mp.Length - 1] = 0;
                h_mp[h_mp.Length - 2] = (1ul << msbBits) - 1;
            }

            var eq = h_mp.SequenceEqual(h_mp1);
            if (!eq)
            {
                Assert.IsTrue(eq, "Calculated mp does not match actual mp");
            }



        }

        private unsafe bool HigginsTestCudaAtomic(int p)
        {

            var msbBits = p & 31;
            var wordSize32 = 1 + (p >> 5);
            var mask = (1u << msbBits) - 1;


            var h_mp = new ulong[wordSize32];
            Array.Fill(h_mp, (ulong)uint.MaxValue);


            h_mp[h_mp.Length - 1] = (1ul << msbBits) - 1;



            var h_a = new ulong[h_mp.Length];
            h_a[0] = 3;


            var actual = NativeLLMNaive(h_a, p, mask);
            var expected = HigginsTestGmp(p);
            if (actual != expected)
            {
                Assert.AreEqual(expected, actual, $"Native LLM failed for {p}");
            }
            return expected;

        }

        bool NativeLLMNaive(ulong[] s1, int p, uint mask)
        {
            //BigInteger s = 3;
            Assert.AreEqual(3UL, s1[0]);
            for (var i = 1; i < s1.Length; i++)
            {
                Assert.AreEqual(0UL, s1[i]);
            }
            var wordsSize = p >> 31;
            var len = s1.Length;
            ulong[] c = new ulong[len << 1];
            ulong[] tmp1 = new ulong[c.Length];
            ulong[] tmp2 = new ulong[c.Length];
            var s = s1.ToArray();

            //kernel SquareNaive
            void SquareNaive()
            {
                for (int ai = 0; ai < len; ai++)
                {
                    for (int bi = 0; bi < len; bi++)
                    {
                        ulong t = s[ai] * s[bi];
                        c[ai + bi] += t & uint.MaxValue;
                        c[ai + bi + 1] += t >> 32;
                    }
                }
            }

            //kernel Carry32
            void Carry32()
            {
                for (var i = 0; i < len; i++)
                {
                    ulong t = c[i];
                    ulong high = t >> 32;
                    ulong low = t & uint.MaxValue;
                    tmp1[i] = low;
                    tmp1[i + i] += high;
                }
                Buffer.BlockCopy(tmp1, 0, c, 0, tmp1.Length * 8);
            }

            //kernel ClearAll()
            void ClearAll()
            {
                ClearC();
                ClearTmp1();
                ClearTmp2();
            }

            //kernel ClearC
            void ClearC()
            {
                Array.Fill(c, 0u);
            }

            //kernel ClearTmp1
            void ClearTmp1()
            {
                Array.Fill(tmp1, 0u);
            }

            //kernel ClearTmp2
            void ClearTmp2()
            {
                Array.Fill(tmp2, 0u);
            }

            //kernel Cmp()
            int Cmp()
            {
                int result = -1;
                for (var i = c.Length - 1; result < 0 && i > wordsSize; i--)
                {
                    if (c[i] > 0) result = 1;
                }
                if (c[wordsSize] > mask) return 1;
                else if (c[wordsSize] < mask) return -1;

                for (var i = wordsSize - 1; i >= 0; i++)
                {
                    if (c[i] < uint.MaxValue) return -1;
                }

                return result;
            }

            //kernel GetLow()
            void GetLow()
            {
                ClearTmp1();
                for (var i = 0; i < wordsSize - 1; i++)
                {
                    tmp1[i] = c[i];
                }
                tmp1[wordsSize] = c[wordsSize] & mask;
            }

            //kernel GetHigh()
            void GetHigh()
            {
                ClearTmp2();
                tmp2[wordsSize] = c[wordsSize] >> (p & 31);
                for (var i = wordsSize; i < len - 1; i++)
                {
                    tmp2[i] = c[i];
                }

            }

            //Kernel AddLowAndHigh
            void AddLowAndHigh()
            {

            }

            //kernel CopyToS()
            void CopyToS()
            {
                Array.Copy(c, 0, s, 0, len);
            }

            for (var j = 0; j <= p - 1; j++)
            {

                /*

                    for (var j = 0; j <= p - 2; j++)
                    {
                        s = s * s;
                        while (s > mp)
                        {
                            s = (s & mp) + (s >> p);
                        }
                    }
                    return mp - 3 == s;
                */

                ClearAll();

                //    s = s * s;
                //kernal launch: SquareNaive
                SquareNaive();
                Carry32();

                //while (s > mp)
                int cmp = Cmp();
                while (cmp > 0)
                {
                    GetLow();
                    GetHigh();
                    AddLowAndHigh();
                }

                CopyToS();


            }


            if (s[0] == 9UL)
            {
                for (var i = 1; i < s.Length; i++)
                {
                    if (s[i] != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;

        }

        bool NativeLLM1(ulong[] s1, ref ulong[] c, int p, uint mask)
        {

            BigInteger mp = 1;
            mp <<= p;
            mp -= 1;

            var mask64 = 1ul << (p & 63);
            var shift64 = (p & 63);

            //BigInteger s = 3;
            Assert.AreEqual(3UL, s1[0]);
            for (var i = 1; i < s1.Length; i++)
            {
                Assert.AreEqual(0UL, s1[i]);
            }

            var s = s1.ToArray();
            Array.Fill(c, 0ul);
            var len = s.Length;
            ulong[] unaligned = c.ToArray();
            //BigInteger mp = BigInteger.Pow(2, p) - 1;
            for (var j = 0; j <= p - 1; j++)
            {
                Array.Fill(c, 0ul);
                var cbValues = s.Select(x => (uint)x).ToArray();
                var buffer = new byte[cbValues.Length << 2];
                Buffer.BlockCopy(cbValues, 0, buffer, 0, buffer.Length);
                var sb = new BigInteger(buffer);
                var sqb = sb * sb;
                var cb = sqb % mp;
                var cbBuffer = cb.ToByteArray();
                Array.Fill(cbValues, 0u);
                Buffer.BlockCopy(cbBuffer, 0, cbValues, 0, cbBuffer.Length);

                //    s = s * s;
                for (var ai = 0; ai < len; ai++)
                {
                    for (var bi = 0; bi < len; bi++)
                    {
                        var ci = ai + bi;
                        var t = s[ai] * s[bi];
                        if (ci < len || len == 1)
                        {
                            c[ci] += t;
                        }
                        else
                        {
                            //need to work ushort values to prevent this.
                            //values not word aligned to word % p, need to be normalized to word size and shifted
                            unaligned[ci] += t;
                        }
                    }
                }
                bool carried = true;
                while (carried)
                {
                    //    while (s > mp)
                    for (var i = 0; i < len - 1; i++)
                    {
                        // s = (s & mp) + (s >> p);
                        ref ulong w = ref c[i];
                        if (w > uint.MaxValue)
                        {
                            var high = w >> 32;
                            c[i + 1] += high;
                            w = w & uint.MaxValue;
                        }
                    }

                    // at this point we have  s*s and need to perform the mod.
                    var cLast = c[len - 1];
                    carried = cLast > mask;
                    if (carried)
                    {
                        var low = cLast & mask;
                        var high = cLast >> (p & 31);
                        if (len == 1)
                        {
                            c[0] = high + low;
                            carried = c[0] > mask;
                        }
                        else
                        {
                            c[0] += high;
                            c[len - 1] = low;
                            carried = c[0] > uint.MaxValue;
                        }
                    }
                }
                var cu = c.Select(x => (uint)x).ToList();
                var eq = cu.SequenceEqual(cbValues);
                if (!eq)
                {
                    string bp = "";
                }
                Array.Copy(c, 0, s, 0, len);
            }
            if (s[0] == 9UL)
            {
                for (var i = 1; i < s.Length; i++)
                {
                    if (s[i] != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }



        [TestMethod]
        public void MersenneMulAtomic()
        {

            //var mp = new ulong[];

            //ulong[][] data_h;
            //CudaDeviceVariable<CUdeviceptr> data_d;
            //CUdeviceptr[] ptrsToData_h; //represents data_d on host side
            //CudaDeviceVariable<ulong>[] arrayOfarray_d; //Array of CudaDeviceVariables to manage memory, source for pointers in ptrsToData_h.

            //int sizeX = 512;
            //int sizeY = 256;

            //data_h = new ulong[sizeX][];
            //arrayOfarray_d = new CudaDeviceVariable<ulong>[sizeX];
            //data_d = new CudaDeviceVariable<CUdeviceptr>(sizeX);
            //ptrsToData_h = new CUdeviceptr[sizeX];
            //for (int x = 0; x < sizeX; x++)
            //{
            //    data_h[x] = new ulong[sizeY];
            //    arrayOfarray_d[x] = new CudaDeviceVariable<ulong>(sizeY);
            //    ptrsToData_h[x] = arrayOfarray_d[x].DevicePointer;
            //    //ToDo: init data on host...
            //}
            ////Copy the pointers once:
            //data_d.CopyToDevice(ptrsToData_h);

            ////Copy data:
            //for (int x = 0; x < sizeX; x++)
            //{
            //    arrayOfarray_d[x].CopyToDevice(data_h[x]);
            //}

        }



#if SKIP_LONG_TESTS
        [Ignore]
#else

#endif
        [TestMethod]
        public void MersenneBinaryMod64Pack32NativeTest()
        {
            var primes = Primes.UintFactorPrimes.Skip(2).Take(164).ToList();
            foreach (var p in primes)
            {
                if (MersenneBinaryMod64Pack32Native(p))
                {
                    Console.WriteLine("2^{0}-1 is Prime ", p);
                }
            }
        }

        bool MersenneBinaryMod64Pack32Native(int p)
        {
            var wordLen = 1 + (p >> 5);
            var s = new ulong[wordLen];
            s[0] = 3;
            var perms = new ulong[32][];

            void initPerms()
            {
                for (var j = 0; j < 32; j++)
                {
                    perms[j] = new ulong[wordLen];
                }
            }

            void permuateS()
            {
                //perms[0] = new ulong[wordLen];
                Buffer.BlockCopy(s, 0, perms[0], 0, s.Length * 8);
                for (var j = 1; j < 32; j++)
                {
                    perms[j] = new ulong[wordLen];
                    Buffer.BlockCopy(perms[j - 1], 0, perms[j], 0, s.Length * 8);
                    RotateLeft32Packed64(perms[j], p);
                }

            }

            void BinaryMul(ulong[] src)
            {
                for (var wordIndex = 0; wordIndex < wordLen; wordIndex++)
                {
                    var word = src[wordIndex];
                    for (var j = 0; word > 0 && j < 32; j++)
                    {
                        //var mask = 1ul << j;
                        //if ((src[wordIndex] & mask) > 0)
                        if ((word & 1) > 0)
                        {
                            var perm = perms[j];
                            var dest = wordIndex;
                            for (var k = 0; k < perm.Length; k++)
                            {
                                s[dest] += perm[k];
                                dest++;
                                if (dest >= s.Length) dest -= s.Length;

                            }
                        }
                        word >>= 1;
                    }
                }
            }

            initPerms();



            int numSquares = p - 1;
            for (var step = 0; step < numSquares; step++)
            {
                permuateS();

                var src = perms[0];
                Array.Fill(s, 0ul);

                BinaryMul(src);

                //6,561= 81^2
                Mod32Packed64(s, p);

            }
            //BigInteger s = 3;
            //BigInteger mp = BigInteger.Pow(2, p) - 1;
            //for (var j = 0; j <= p - 2; j++)
            //{
            //    s = s * s;
            //    while (s > mp)
            //    {
            //        s = (s & mp) + (s >> p);
            //    }
            //}
            //return mp - 3 == s;

            bool isPrime = s[0] == 9;
            if (isPrime)
            {
                for (var j = 1; isPrime && j < s.Length; j++)
                {
                    isPrime = s[j] == 0;
                }
            }
            return false;
        }



        [TestMethod]
        public void RotateLeftTest()
        {
            int value = 0b10101010;  // Binary: 170
            int rotatedValue = RotateLeft(value, 3, 8);  // Rotate left by 3 bits within an 8-bit length

            Console.WriteLine(Convert.ToString(rotatedValue, 2));  // Output: 10101001 (Binary: 169)
        }
        int RotateLeft(int value, int shift, int bitLength)
        {
            // Ensure the number of bits to rotate is within the valid range
            shift = shift % bitLength;

            // Perform the rotation
            int rotatedValue = (value << shift) | (value >> (bitLength - shift));

            return rotatedValue;
        }



        private void RotateLeft32Packed64(ulong[] perm, int p)
        {
            for (var i = 0; i < perm.Length; i++)
            {
                perm[i] <<= 1;
            }
            for (var i = 1; i < perm.Length; i++)
            {
                ref ulong prev = ref perm[i - 1];
                ref ulong curr = ref perm[i];
                curr = curr | prev >> 32;
                prev = (uint)prev;
            }
            {
                ref ulong prev = ref perm[perm.Length - 1];
                ref ulong curr = ref perm[0];
                curr = curr | prev >> p & 31;
                prev = prev & ((1ul << p) - 1);
            }
        }

        private void Mod32Packed64(ulong[] perm, int p)
        {

            ref ulong prev = ref perm[perm.Length - 1];
            var carry = prev >> p & 31;
            while (carry > 0)
            {
                ref ulong curr = ref perm[0];
                curr += carry;
                carry = curr >> 32;
                curr = (uint)curr;
                for (var j = 1; carry > 0 && j < perm.Length; j++)
                {
                    curr = ref perm[j];
                    curr += carry;
                    carry = curr >> 32;
                    curr = (uint)curr;
                }
            }

        }



#if SKIP_LONG_TESTS
        [Ignore]
#else

#endif
        [TestMethod]
        public void MersenneBinaryIndexModNativeTest()
        {
            var primes = Primes.UintFactorPrimes.Skip(3).Take(164).ToList();
            foreach (var p in primes)
            {
                if (MersenneBinaryIndexModNative(p))
                {
                    Console.WriteLine("2^{0}-1 is Prime ", p);
                }
            }
        }

        bool MersenneBinaryIndexModNative(int p)
        {
            var bits = new int[p];
            var result = new int[p << 1];
            bits[0] = bits[1] = 2;
            int numSquares = p;
            int[] checkedIndex = new int[p];

            int checkIdx = 0;
            checkedIndex[checkIdx++] = 0;
            checkedIndex[checkIdx++] = 1;


            void mul()
            {
                for (var i = 0; i < checkIdx; i++)
                {
                    var offset = checkedIndex[i];
                    for (var j = 0; j < checkIdx; j++)
                    {
                        result[offset + checkedIndex[j]]++;
                    }
                }
            }

            void carry()
            {

                bool hasCarry = true;
                while (hasCarry)
                {

                    //
                    checkIdx = 0;
                    int idxLow = 0;
                    int idxHigh = p;
                    for (; idxLow < p; idxLow++, idxHigh++)
                    {
                        ref int high = ref result[idxHigh];
                        ref int low = ref result[idxLow];

                        low += high;
                        high = 0;
                        if (low < 2)
                        {
                            hasCarry = false;
                        }
                        else
                        {

                            ref int next = ref result[idxLow + 1];
                            next += low >> 1;
                            low = low & 1;
                            hasCarry = true;
                        }
                        //todo: would it be cheaper to a final check after carry is done?
                        if (low > 0)
                        {
                            checkedIndex[checkIdx++] = idxLow;
                        }
                    }
                }
            }

            for (var step = 0; step < numSquares; step++)
            {

                //clear the result;
                Array.Fill(result, 0);
                mul();
                carry();
                Buffer.BlockCopy(result, 0, bits, 0, bits.Length * 4);
                //at step -2, checkedIndex[checkIdx-1]=p, checkedIndex[0]=2, and checkedIndex[0..checkIdx] are sequence
            }

            //return bits==..... 1.0.0.1
            var isPrime = checkIdx == 2 && checkedIndex[0] == 0 && checkedIndex[1] == 3;

            return isPrime;
        }
    }

    public class Cuda : IDisposable
    {
        public static readonly Cuda Instance = new Cuda();

        public CudaContext CudaContext { get; private set; }
        public Cuda()
        {
            this.CudaContext = new CudaContext(0);
        }
        public static CudaContext Context => Instance.CudaContext;

        public void Dispose()
        {
            ((IDisposable)CudaContext).Dispose();
        }
        public Dictionary<string, CudaKernel> Kernels = new();

        //TODO: all multiple
        public CudaKernel GetOrCompile(string src, string kernelName, params string[] compilerArgs)
        {
            if (Kernels.ContainsKey(kernelName)) return Kernels[kernelName];
            return Compile(src, kernelName, compilerArgs);
        }


        public CudaKernel Compile(string src, string kernelName, params string[] args)
        {
            if (Kernels.ContainsKey(kernelName))
            {
                throw new Exception($"Kernal has already been defined: {kernelName}");
            }
            if (!Kernels.ContainsKey(kernelName))
            {
                CudaRuntimeCompiler rtc = new CudaRuntimeCompiler(src, $"{kernelName}_kernel");
                rtc.Compile(args);
                var log = rtc.GetLogAsString();
                byte[] ptx = rtc.GetPTX();
                CudaKernel result = Context.LoadKernelPTX(ptx, kernelName);
                Kernels[kernelName] = result;
                rtc.Dispose();
            }

            return Kernels[kernelName];
        }


        public Dictionary<string, CudaKernel> GetOrCompile(string src, string name, IEnumerable<string> kernelNames, params string[] compilerArgs)
        {
            var existing = kernelNames.Where(x => kernelNames.Contains(x)).ToList();
            var missing = kernelNames.Where(x => !kernelNames.Contains(x)).ToList();

            var result = existing.ToDictionary(x => x, x => Kernels[x]);
            if (missing.Any())
            {
                var uncached = Compile(src, name, missing, compilerArgs);
                foreach (var kvp in uncached)
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            return result;

        }


        public Dictionary<string, CudaKernel> Compile(string src, string name, IEnumerable<string> kernelNames, params string[] args)
        {
            var existing = kernelNames.Where(x => Kernels.ContainsKey(x)).ToList();
            if (existing.Any())
            {
                throw new Exception($"Kernels have already been defined: {string.Join(", ", existing)}");
            }

            CudaRuntimeCompiler rtc = new CudaRuntimeCompiler(src, $"{name}_kernel");
            rtc.Compile(args);
            var log = rtc.GetLogAsString();
            byte[] ptx = rtc.GetPTX();
            var kernels = kernelNames.ToDictionary(x => x, x => Context.LoadKernelPTX(ptx, x));
            foreach (var kvp in kernels)
            {
                Kernels[kvp.Key] = kvp.Value;
            }
            return kernels;
        }

    }

    public class CudaKernels
    {
        public static string VecAdd
        {
            get
            {
                var result = $@"
                //Kernel code:
                extern ""C""  {{   
                    // Device code
                    __global__ void {nameof(VecAdd)}(const float* A, const float* B, float* C, int N)
                    {{
                        int i = blockDim.x * blockIdx.x + threadIdx.x;
                        if (i < N)
                            C[i] = A[i] + B[i];
                    }}
                }}";
                return result;
            }
        }

        public static string VecMod32
        {
            get
            {
                var result = $@"
                //Kernel code:
                extern ""C""  {{   
                    // Device code
                    __global__ void {nameof(VecMod32)}(const int* A, const int* B, int* C, int N)
                    {{
                        int i = blockDim.x * blockIdx.x + threadIdx.x;
                        if (i < N)
                            C[i] = A[i] % B[i];
                    }}
                }}";
                return result;
            }
        }

        public static string VecMod32I
        {
            get
            {
                var result = $@"
                //Kernel code:
                extern ""C""  {{   
                    // Device code
                    __global__ void {nameof(VecMod32I)}(const int* A, int B, int* C, int N)
                    {{
                        int i = blockDim.x * blockIdx.x + threadIdx.x;
                        if (i < N)
                            C[i] = A[i] % B;
                    }}
                }}";
                return result;
            }
        }
        public static string QsTDiv
        {
            get
            {
                /*            kernel.Run(d_N.DevicePointer,
                d_BSmooth.DevicePointer,
                d_Gf2Masks.DevicePointer,
                d_DivMasks.DevicePointer,
                d_Primes.DevicePointer,
                numElements,
                numPrimes);*/
                var result = $@"
                //Kernel code:
                extern ""C""  {{   
                    // Device code
                    __global__ void {nameof(QsTDiv)}(int* n, int* bsmooth, int* gf2, int* div, const int* primes, int N, int P)
                    {{
                        int i = blockDim.x * blockIdx.x + threadIdx.x;
                        if (i < N)
                        {{
                            int mask = 0;
                            for (int j=0; n[i] > 1 && j < P; j++)
                            {{
                                mask = 1 << j;
                                while(n[i] % primes[j] == 0)
                                {{
                                     n[i] =   n[i] / primes[j];
                                     gf2[i] = gf2[i] ^ mask;
                                     div[i] = div[i] | mask;
                                }}
                            }}
                            bsmooth[i] = n[i] == 1 ? 1 : 0;
                        }}
                    }}
                }}";
                return result;
            }
        }
    }


}