/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Some routines inspired by the Stanford Bit Twiddling Hacks by Sean Eron Anderson:
// http://graphics.stanford.edu/~seander/bithacks.html

#define TARGET_64BIT


using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace HigginsSoft.Math.Lib
{

    public partial class MathLib
    {
        public class AVXZ
        {
            public class Karatsuba
            {
                public static void Test4x4DebugNative()
                {
                    ulong[] a = { 1, 2, 3, 4 };
                    ulong[] b = { 5, 6, 7, 8 };
                    Test4x4DebugNative(a, b);

                }

                private static void Test4x4DebugNative(ulong[] a, ulong[] b)
                {
                    int size = a.Length;
                    int maxBlockSize = MathLib.ILogB(size);// size >> 1;
                    ulong[] result = new ulong[size << 1];
                    ulong[] t0 = new ulong[size << 1];

                    /*
                     
                        BigInteger b = x >> n;			            //low(x) 	// right(x) // lsb_half(x)
                        BigInteger a = x - (b << n);	            //high(x) 	// left(x)	// msb_half(x)
                        BigInteger d = y >> n;			            //low(y)	// right(y)	// lsb_half(y)
                        BigInteger c = y - (d << n);                //high(y)	// left(y)	// msb_half(y)


                        BigInteger ac = Karatsuba(a, c);	        // high(x) * high(y) // msb_half(x) * msb_half(y)
                        BigInteger bd = Karatsuba(b, d);	        // low(x) * low(y) 	 // lsb_half(x) * lsb_half(y)
                        BigInteger abcd = Karatsuba(a + b, c + d);	// (lsb_half(x) + msb_half(x)) * (lsb_half(y) + msb_half(y));

                        return ac + ((abcd - ac - bd) << n) + (bd << (2 * n));
                    */

                    ulong t = 0;
                    //block size 1 and 2 don't need loops.
                    //blocksize=1

                    int blockSize = 1;
                    int destOffsetShift = 1 << blockSize;
                    int destIndex;
                    for (var i = 0; i < size; i++)
                    {
                        t = a[i] * b[i];
                        destIndex = i * destOffsetShift;
                        result[destIndex] = (uint)t;
                        result[destIndex + 1] = (uint)(t >> 32);
                    }



                    int srcIndexA;
                    int srcIndexB;
                    blockSize = 2;
                    destOffsetShift = 1 << blockSize;
                    int halfBlockSize = destOffsetShift >> 1;

                    size >>= 1;
                    for (var i = 0; i < size; i ++)
                    {

                        destIndex = i * destOffsetShift;
                        srcIndexA = destIndex;
                        srcIndexB = srcIndexA + halfBlockSize;



                        ref ulong t0_0 = ref t0[destIndex];
                        ref ulong t0_1 = ref t0[destIndex + 1];
                        ref ulong t0_2 = ref t0[destIndex + 2];
                        ref ulong t0_3 = ref t0[destIndex + 3];

                        // Why are we pulling from a and b directly after computing result?
                        // because a0,a1 is left and b0,b1, is right.
                        var a0a1 = a[srcIndexA] + a[srcIndexA + 1];
                        var b0b1 = b[srcIndexA] + b[srcIndexA + 1];

                        // this can overflow: needs to be array
                        var acbd = a0a1 * b0b1;

                        //this is zero aligned instead of word aligned.
                        var mid = (acbd - result[srcIndexA] - result[srcIndexB]);
                        //mid can already overflow without shift, perhaps safe once subtracting lsb and msb.
                        mid <<= 32;


                        //return ac + (mid << n) + (bd << (2 * n));

                        //BigInteger bd = Karatsuba(b, d);
                        //lolsb * himsb
                        t0_0 = result[srcIndexA];     //a0*b0;

                        //BigInteger ac = Karatsuba(a, c);
                        //hilsb * himsb
                        t0_3 = result[srcIndexA + 1] * result[srcIndexB + 1];   //aa*b1;


                        //BigInteger abcd = Karatsuba(a + b, c + d);	// (lsb_half(x) + msb_half(x)) * (lsb_half(y) + msb_half(y));

                        t0_1 = result[srcIndexA] * result[srcIndexB + 1];   //a0*b1;
                        t0_2 = result[srcIndexA + 1] * result[srcIndexB];     //a1*b0;
                      

                        //carry
                        t0_1 += t0_0 >> 32;
                        t0_0 = (uint)t0_0;

                        t0_2 += t0_1 >> 32;
                        t0_1 = (uint)t0_1;

                        t0_3 += t0_2 >> 32;
                        t0_2 = (uint)t0_2;

                        Debug.Assert(t0_3 <= int.MaxValue);


                    }

                    for(var i=0; i < size; i++)
                    {
                      

                    }

                    for (; blockSize <= maxBlockSize; blockSize <<= 1)
                    {
                        blockSize++;
                        destOffsetShift = 1 << blockSize;
                        halfBlockSize = destOffsetShift >> 1;
                        size >>= 1;

                        for (var i = 0; i < size; i++)
                        {

                            destIndex = i * destOffsetShift;
                            srcIndexA = destIndex;
                            srcIndexB = srcIndexA + halfBlockSize;

                            // grammar school needs t0...t[destOffsetShift]
                            // but should no longer grammer schooling here.
                            // need to karatsuba inner
                            //  left[destIndex..destIndex+halfblocksize-1] or left[srcIndexA...[srcIndexB|halfblocksize]-1] 
                            //  right [destIndex+halfblocksize ... destIndex+blockSize] or left[srcIndexB..srcIndexB+halfblocksize-1] 
                            ref ulong t0_0 = ref t0[destIndex];
                            ref ulong t0_1 = ref t0[destIndex + 1];
                            ref ulong t0_2 = ref t0[destIndex + 2];
                            ref ulong t0_3 = ref t0[destIndex + 3];



                            t0_0 = result[srcIndexA] * result[srcIndexB];     //a0*b0;
                            t0_1 = result[srcIndexA] * result[srcIndexB + 1];   //a0*b1;
                            t0_2 = result[srcIndexA + 1] * result[srcIndexB];     //a1*b0;
                            t0_3 = result[srcIndexA + 1] * result[srcIndexB + 1];   //a0*b1;

                            //carry
                            t0_1 += t0_0 >> 32;
                            t0_0 = (uint)t0_0;

                            t0_2 += t0_1 >> 32;
                            t0_1 = (uint)t0_1;

                            t0_3 += t0_2 >> 32;
                            t0_2 = (uint)t0_2;

                            Debug.Assert(t0_3 <= int.MaxValue);


                        }


                        //blocksize=1
                        //result[0]= low(a[0]*b[0])
                        //result[1]= high(a[0]*b[0])


                        //calculate c[
                        for (var srcOffset = 0; srcOffset < size; srcOffset += blockSize)
                        {
                            for (var i = 0; i < blockSize; i++)
                            {

                                var wa = a[srcOffset + i];

                                result[i] = a[srcOffset + i] * b[srcOffset + i];
                                for (var j = 0; j < blockSize; j++)
                                {
                                    var wb = b[srcOffset + j];
                                    t = wa * wb;
                                    result[srcOffset + i + j] += (uint)t;
                                    result[srcOffset + i + j + 1] += t >> 32;

                                }
                            }

                        }
                    }
                }

                private unsafe static void Test4x4()
                {
                    // multiply 8765*4321
                    ulong[] a = { 1, 2, 3, 4 };
                    ulong[] b = { 5, 6, 7, 8 };

                    int size = a.Length;
                    int halfSize = size >> 1;
                    ulong[] result = new ulong[size << 1];
                    // load the following arrays while waiting on avx latency
                    // ulong[] a0xb = { a[0], a[1], a[2], a[3] };   // for old school mult
                    // ulong[] a1xb = { b[1], a[0], b[3], b[3] };   // for old school mult
                    // ulong[] low = { 1, 3, 5, 7 };                // only for karatsuba
                    // ulong[] high = { 2, 4, 6, 8 };               // only for karatsuba
                    //test with a and b being passed int
                    fixed (ulong* p_a = a)
                    fixed (ulong* p_b = b)
                    {
                        // 7 - .33 mm256_loadu_si256(__m256i const * mem_addr)
                        Vector256<uint> v_a0 = Avx.LoadVector256((uint*)p_a);// in practice will be offsetting the pointer
                        Vector256<uint> v_b0 = Avx.LoadVector256((uint*)p_b); // in practice will be offsetting the pointer


                        //initialize a1xb as v_a0 and v_b0 load in 7 cycles in parallel.
                        ulong[] axb = { b[1], b[0], b[3], b[2] };
                        fixed (ulong* p_axb = axb)
                        {
                            Vector256<uint> v_axb = Avx.LoadVector256((uint*)p_axb);
                            // v_a0 and v_b0 now available execute the mulitply
                            var product = Avx2.Multiply(v_a0, v_a0);

                            int offset = 0;
                            int idx0, idx1, idx2, idx3;

                            //initialize t0 as v_a1xb loads in 7 cycles in parallel and product computes in 5 cycles in parallel.
                            ulong[] t0 = { 0, 0, 0, 0 }; // in practice t0 will already be created, but still need to fix at a pointer offset and clear it.
                            fixed (ulong* p_t0 = t0)
                            {
                                Avx.Store(p_t0, product); //t0 = {1, 12, 21, 32} = {a0*b0, a1*b1, a2*b3, a3*b3} 
                                // v_a1x now available
                                var innerproduct = Avx2.Multiply(v_a0, v_axb);


                                ulong t;
                                //initialize t1 as innerproduct computes in 5 cycles in parallel.
                                ulong[] t1 = { 0, 0, 0, 0 }; // in practice t0 will already be created, but still need to fix at a pointer offset and clear it.
                                fixed (ulong* p_t1 = t1)
                                {
                                    // innerproduct now available.
                                    Avx.Store(p_t1, innerproduct); //t1 = {1, 12, 21, 32} = {a0*b1, b1*a0, a2*b3, a3*b2}
                                                                   //TODO: can we start initial calculations of high-low in avx while we normalize results?

                                    ulong[] c = t0;
                                    ulong[] d = t1;
                                    for (idx0 = offset; idx0 < offset + 8; idx0 += 4)
                                    {

                                        idx1 = idx0 + 1;
                                        idx2 = idx0 + 2;
                                        idx3 = idx0 + 3;
                                        t = c[idx0];      // a0*b0

                                        result[idx0] = (uint)t; //low(a0*b0);


                                        t >>= 32; // c[0] >> 32 //carry high(a0*b0);
                                        t += (uint)d[idx0];     //low(a0*b1);
                                        t += (uint)d[idx1];     //low(a1*b0);
                                        result[idx1] = (uint)t; //low( high(a0*b0) + low(a0*b1) + low(a1*b0));


                                        t >>= 32;               //carry high((a0*b0) + low(a0*b1) + low(a1*b0))
                                        t += (d[idx0] >> 32);   //high(a0*b1);
                                        t += (d[idx1] >> 32);   //high(a1*b0);
                                        t += (uint)c[idx1];     //low(a1*b1);
                                        result[idx2] = (uint)t; //low( high((a0*b0) + low(a0*b1) + low(a1*b0)) + high(a0*b1) + high(a1*b0) + low(a1*b1));

                                        t >>= 32;               // carry high((a0*b0) + low(a0*b1) + low(a1*b0)) + high(a0*b1) + high(a1*b0) + low(a1*b1)
                                        t += (c[idx1] >> 32);   //high(a1*b1);
                                        result[idx3] = (uint)t;

                                    }

                                }

                                // we now have left (ac) and right side (bd) products in result, need to add left and right cross multiply.
                                //  BigInteger ac = Karatsuba(a, c);    // msb_half(x) * msb_half(y)
                                //  BigInteger bd = Karatsuba(b, d);    // lsb_half(x) * lsb_half(y)

                                // and need to multiply  ((lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x))
                                //  BigInteger abcd = Karatsuba(a + b, c + d);
                                // 


                                //This is overkill for 2x2 but working through the steps for larger block sizes
                                //add a+b into t0 at offset
                                idx0 = offset;
                                idx1 = idx0 + 1;
                                idx2 = idx0 + 2;
                                idx3 = idx0 + 3;

                                t = result[idx0] + result[idx2];
                                t0[idx0] = (uint)t;
                                t >>= 32;
                                t += result[idx1];
                                t += result[idx3];
                                t0[idx1] = (uint)t;
                                t >>= 32;
                                System.Diagnostics.Debug.Assert(t == 0);

                                //add c+d into t0 at offset+4
                                idx0 += 4;
                                idx1 = idx0 + 1;
                                idx2 = idx0 + 2;
                                idx3 = idx0 + 3;

                                t = result[idx0] + result[idx2];
                                t0[idx0] = (uint)t;
                                t >>= 32;
                                t += result[idx1];
                                t += result[idx3];
                                t0[idx1] = (uint)t;
                                t >>= 32;
                                System.Diagnostics.Debug.Assert(t == 0);


                                //Again overkill for 2x2, wastes cycles loading vectors and multiplying
                                //  But this is to debug the iterative algorithm .
                                //BigInteger abcd = Karatsuba(a + b, c + d)
                                mul2x2(t0, ref t1, offset, offset);

                                subtract(t1, result, ref t1, halfSize, offset, offset + halfSize);


                                // return return ac + ((abcd - ac - bd) << n) + (bd << (2 * n));
                            }
                        }
                    }
                }

                private static void subtract(ulong[] upper, ulong[] lower, ref ulong[] result, int size, int srcOffset, int destOffset)
                {
                    ulong borrow = 0;
                    for (var i = 0; i < size; i++)
                    {
                        var upperValue = upper[i + srcOffset];
                        var lowerValue = lower[i + srcOffset];

                        ulong diff = upperValue - lowerValue - borrow;
                        result[i + destOffset] = diff;
                        borrow = (diff > upperValue) ? 1UL : 0UL;
                    }
                }

                private unsafe static void mul2x2(ulong[] c, ref ulong[] result, int srcOffset, int dstOffset)
                {
                    fixed (ulong* aptr = &c[srcOffset])
                    fixed (ulong* bptr = &c[srcOffset + 2])
                    fixed (ulong* cptr = &c[srcOffset])

                    {
                        Vector256<uint> av = Avx.LoadVector256((uint*)aptr);
                        Vector256<uint> bv = Avx.LoadVector256((uint*)bptr);
                        var product = Multiply(av, bv);
                        Avx.Store(cptr, product);



                        //maybe better to use ref T
                        int idx0 = dstOffset;
                        int idx1 = idx0 + 1;
                        int idx2 = idx0 + 2;
                        int idx3 = idx0 + 3;

                        ulong t = c[idx0];

                        result[idx0] = (uint)t;
                        t >>= 32; // c[0] >> 32

                        t += (uint)c[idx1];
                        t += (uint)c[idx2];
                        result[idx1] = (uint)t;


                        t >>= 32; //carry
                        t += (c[idx1] >> 32);
                        t += (c[idx2] >> 32);

                        t += (uint)c[idx3];
                        result[idx2] = (uint)t;

                        t >>= 32;
                        t += (c[idx3] >> 32);
                        result[idx3] = (uint)t;
                    }
                }

                private unsafe static void Test4x4_Karatsuba()
                {
                    // here for illustrative purposes to show  single element wastes cycles waiting for data to load.
                    //for square, a and b will be part of the same array, allowing more loads to hide latency.
                    ulong[] a = { 1, 2, 3, 4 };
                    ulong[] b = { 5, 6, 7, 8 };

                    // multi 1*2, 3*4, 5*6, 7*8
                    // todo: see if there are avx instructions to perform this permuation.
                    //      perhaps mask move or shuffle (1-1)
                    ulong[] low = { 1, 3, 5, 7 };
                    ulong[] high = { 2, 4, 6, 8 };
                    ulong[] t0 = { 0, 0, 0, 0 };
                    ulong[] t1 = { 0, 0, 0, 0 };
                    ulong[] result = new ulong[8];

                    fixed (ulong* p_a = a)
                    fixed (ulong* p_b = b)
                    fixed (ulong* p_t0 = t0)
                    fixed (ulong* p_t1 = t1)
                    fixed (ulong* p_low = low)
                    fixed (ulong* p_high = high)
                    {


                        //karatsuba single element wastes to many cycles waiting for data to load.

                        //mm256_loadu_si256 (__m256i const * mem_addr)
                        //latency 7 throughput 0.33 to .5
                        Vector256<uint> a0 = Avx.LoadVector256((uint*)p_a);
                        Vector256<uint> b0 = Avx.LoadVector256((uint*)p_b);
                        Vector256<uint> low0 = Avx.LoadVector256((uint*)p_low);
                        Vector256<uint> high0 = Avx.LoadVector256((uint*)p_high);


                        //Waste: 5 cycles waiting on a0,b0 
                        //_mm256_mul_epu32: latency 5 throughput	0.5
                        var product = Avx2.Multiply(a0, b0);
                        //5 - 0.5 - Waste: 0 if product blocks, else 6 cycles waiting on low0, low1
                        var lowXHigh = Avx2.Multiply(low0, high0);

                        //waste: 0 if lowXhigh blocks, else 4 cycles
                        //_mm256_storeu_si256:   // 1 - 0.5
                        Avx.Store(p_t0, product); //t0 = {1, 12, 21, 32}

                        //waste: 0 if lowXhigh blocks, else 4 cycles
                        //latency 5 throughput    0.5
                        Avx.Store(p_t1, lowXHigh);

                    }

                }
                public static ulong[] Karasuba4x4()
                {
                    ulong[] result = { };



                    return result;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector128<long> Multiply(Vector128<int> a, Vector128<int> b)
                => Avx.Multiply(a, b);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector128<ulong> Multiply(Vector128<uint> a, Vector128<uint> b)
                => Avx.Multiply(a, b);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector128<double> Multiply(Vector128<double> a, Vector128<double> b)
               => Avx.Multiply(a, b);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector128<float> Multiply(Vector128<float> a, Vector128<float> b)
                => Avx.Multiply(a, b);


            /// <summary>
            /// Multiply the low unsigned 32-bit integers from each packed 64-bit element in a and b, and store the unsigned 64-bit results in dst
            /// </summary>
            /// <param name="a0"></param>
            /// <param name="a1"></param>
            /// <param name="b0"></param>
            /// <param name="b1"></param>
            /// <returns></returns>
            public static unsafe uint[] AvxMul(uint a0, uint a1, uint b0, uint b1)
            {
                // Vector256<float> divVect = Avx.LoadAlignedVector256(divPtr + i);
                var a = new uint[] { a0, 0, a0, 0, a1, 0, a1, 0 };
                var b = new uint[] { b0, 0, b1, 0, b0, 0, b1, 0 };
                var c = new ulong[4];
                var result = new uint[4];
                fixed (uint* aptr = &a[0])
                fixed (uint* bptr = &b[0])
                fixed (ulong* cptr = &c[0])

                {
                    Vector256<uint> av = Avx.LoadVector256(aptr);
                    Vector256<uint> bv = Avx.LoadVector256(bptr);
                    var product = Multiply(av, bv);
                    Avx.Store(cptr, product);


                    ulong t = c[0];

                    result[0] = (uint)t;
                    t >>= 32; // c[0] >> 32
                    t += (uint)c[1];
                    t += (uint)c[2];
                    result[1] = (uint)t;


                    t >>= 32; //carry
                    t += (c[1] >> 32);
                    t += (c[2] >> 32);
                    t += (uint)c[3];
                    result[2] = (uint)t;

                    t >>= 32;
                    t += (c[3] >> 32);
                    result[3] = (uint)t;

                }
                return result;

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector256<ulong> Multiply(Vector256<uint> a, Vector256<uint> b)
              => Avx2.Multiply(a, b);



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector256<long> Multiply(Vector256<int> a, Vector256<int> b)
                => Avx2.Multiply(a, b);


            /// <summary>
            /// Multiply the packed signed 32-bit integers in a and b, producing intermediate 64-bit integers, and store the low 32 bits of the intermediate integers in dst
            /// _mm256_mullo_epi32 Latency 10 throughput 1
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector256<uint> MultiplyLow(Vector256<uint> a, Vector256<uint> b)
                => Avx2.MultiplyLow(a, b);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector256<int> MultiplyLow(Vector256<int> a, Vector256<int> b)
               => Avx2.MultiplyLow(a, b);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector256<double> Multiply(Vector256<double> a, Vector256<double> b)
               => Avx.Multiply(a, b);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector256<float> Multiply(Vector256<float> a, Vector256<float> b)
                => Avx.Multiply(a, b);



        }
        public partial class Vect
        {
            /*
             *  Bemchmark: HigginsSoft.Math.Demos.AvxSquareGenBenchmark 

            |       Method |     Mean |    Error |   StdDev |
            |------------- |---------:|---------:|---------:|
            | AvxSquareGen | 18.81 us | 0.234 us | 0.195 us |
            | MulSquareGen | 20.45 us | 0.130 us | 0.122 us |
            | AddSquareGen | 24.22 us | 0.203 us | 0.190 us |
            */

            /// <summary>
            /// Generates integer squares up to 46336^2 using addition taking advantage of AVX instructions.
            /// 24% faster than native addtion, nearly 10% faster than native multiplication.
            /// </summary>
            public ref struct SquareGeneratorAvx
            {
                private int[] sq;
                private int[] deltas;
                private int[] incs;
                private Vector<int> delta;
                private Vector<int> inc;
                private Span<int> buffer;
                private Span<int> deltaBuffer;

                const int MaxIntSquareRoot = 46340;
                const int MaxVectorSquare = 46336;
                public const int Count = 8;
                int step = 0;
                public int CurrentRoot => maxStep * 8;
                const int maxStep = MaxVectorSquare / 8;
                public Vector256<int> Current;
                public SquareGeneratorAvx()
                {
                    sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    //seed initial deltas, incrementing by 8*2 (blockSize*2)
                    deltas = new[] { 80, 96, 112, 128, 144, 160, 176, 192 };
                    incs = new[] { 128, 128, 128, 128, 128, 128, 128, 128 };

                    delta = new Vector<int>(deltas);
                    //seed delta incremements as 8*8*2 (blockSize*blocksize*2)
                    inc = new Vector<int>(incs);
                    buffer = sq.AsSpan();
                    deltaBuffer = deltas.AsSpan();
                    step = 0;
                    Current = default(Vector256<int>);

                }

                public void MoveTo(int start)
                {
                    bool isPerfect = MathLib.IsPerfectSquare(start, out int root);
                    var startsq = start;
                    if (!isPerfect)
                    {
                        root = root + 1;
                        startsq = root * root;
                    }
                    var nextroot = (root + 1);
                    var nextsq = nextroot * nextroot;
                    var startdelta = nextsq - startsq;
                    startdelta -= 2;
                    //calculate the first 8 squares

                    buffer[0] = startsq;
                    buffer[1] = startsq += startdelta += 2;
                    buffer[2] = startsq += startdelta += 2;
                    buffer[3] = startsq += startdelta += 2;
                    buffer[4] = startsq += startdelta += 2;
                    buffer[5] = startsq += startdelta += 2;
                    buffer[6] = startsq += startdelta += 2;
                    buffer[7] = startsq += startdelta += 2;




                    startsq += startdelta += 2;
                    startdelta = startsq - sq[0];



                    //sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    //seed initial deltas, incrementing by 8*2 (blockSize*2)
                    //delta = new Vector<int>(new[] { 80, 96, 112, 128, 144, 160, 176, 192 });

                    //TODO get rid of allocation
                    deltaBuffer[0] = startdelta;
                    deltaBuffer[1] = startdelta += 8 << 1;
                    deltaBuffer[2] = startdelta += 8 << 1;
                    deltaBuffer[3] = startdelta += 8 << 1;
                    deltaBuffer[4] = startdelta += 8 << 1;
                    deltaBuffer[5] = startdelta += 8 << 1;
                    deltaBuffer[6] = startdelta += 8 << 1;
                    deltaBuffer[7] = startdelta += 8 << 1;



                    delta = new Vector<int>(deltaBuffer);

                    //seed delta incremements as 8*8*2 (blockSize*blocksize*2)
                    //inc = new Vector<int>(new[] { 128, 128, 128, 128, 128, 128, 128, 128 });
                    //buffer = sq.AsSpan();
                    //step = 0;
                    Current = default(Vector256<int>);
                }




                /// <summary>
                /// Generates sequence of squares starting with the next square greater than <paramref name="start"/>.
                /// If <paramref name="start"/> is a perfect square then the sequences begins at <paramref name="start"/>.
                /// </summary>
                /// <param name="start"></param>
                public SquareGeneratorAvx(int start)
                    : this()
                {
                    MoveTo(start);
                    //bool isPerfect = MathLib.IsPerfectSquare(start, out int root);
                    //var startsq = start;
                    //if (!isPerfect)
                    //{
                    //    root = root + 1;
                    //    startsq = root * root;
                    //}
                    //var nextroot = (root + 1);
                    //var nextsq = nextroot * nextroot;
                    //var startdelta = nextsq - startsq;
                    //startdelta -= 2;

                    //sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    ////seed initial deltas, incrementing by 8*2 (blockSize*2)
                    //deltas = new[] { 80, 96, 112, 128, 144, 160, 176, 192 };
                    //incs = new[] { 128, 128, 128, 128, 128, 128, 128, 128 };


                    ////calculate the first 8 squares
                    //sq = new[] {
                    //    startsq,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2
                    //};
                    //startsq += startdelta += 2;
                    //startdelta = startsq - sq[0];

                    ////sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    ////seed initial deltas, incrementing by 8*2 (blockSize*2)
                    ////delta = new Vector<int>(new[] { 80, 96, 112, 128, 144, 160, 176, 192 });
                    //delta = new Vector<int>(new[] {
                    //    startdelta,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1
                    //});

                    ////seed delta incremements as 8*8*2 (blockSize*blocksize*2)
                    //inc = new Vector<int>(new[] { 128, 128, 128, 128, 128, 128, 128, 128 });
                    //buffer = sq.AsSpan();
                    //deltaBuffer = deltas.AsSpan();
                    //step = 0;// todo need to calculate step to prevent overflow.
                    //Current = default(Vector256<int>);

                }

                public bool MoveNext()
                {
                    if (step < maxStep)
                    {
                        step++;
                        var v = new Vector<int>(buffer);
                        Current = v.AsVector256<int>();
                        v = v + delta;
                        delta = delta + inc;
                        v.CopyTo(buffer);
                        return true;
                    }
                    return false;
                }
            }


            public ref struct SquareGeneratorAvx64
            {
                private long[] sq;
                private long[] deltas;
                private long[] incs;
                private Vector<long> delta;
                private Vector<long> inc;
                private Span<long> buffer;
                private Span<long> deltaBuffer;

                const int MaxIntSquareRoot = 46340;
                const long MaxVectorSquare = 1L << 48;
                public const int Count = 4;
                int step = 0;
                public long CurrentRoot => maxStep * 4;
                const long maxStep = MaxVectorSquare / 4;
                public Vector256<long> Current;
                public SquareGeneratorAvx64()
                {
                    sq = new[] { 1L, 4L, 9L, 16L };//, 25L, 36L, 49L, 64L };
                    //seed initial deltas, incrementing by 4*2 (blockSize*2)
                    deltas = new[] { 24L, 32L, 40L, 48L };// 144L, 160L, 176L, 192L };
                    //set inc to 2 * 2* blocksize
                    incs = new[] { 32L, 32L, 32L, 32L };//, 128L, 128L, 128L, 128L };

                    delta = new Vector<long>(deltas);
                    //seed delta incremements as 8*8*2 (blockSize*blocksize*2)
                    inc = new Vector<long>(incs);
                    buffer = sq.AsSpan();
                    deltaBuffer = deltas.AsSpan();
                    step = 0;
                    Current = default(Vector256<long>);

                }

                public void MoveTo(long start)
                {
                    bool isPerfect = MathLib.IsPerfectSquare(start, out uint sqrt);
                    var startsq = start;
                    long root = sqrt;
                    if (!isPerfect)
                    {
                        root = root + 1;
                        startsq = root * root;
                    }
                    var nextroot = (root + 1);
                    var nextsq = nextroot * nextroot;
                    var startdelta = nextsq - startsq;
                    startdelta -= 2;
                    //calculate the first 8 squares

                    buffer[0] = startsq;
                    buffer[1] = startsq += startdelta += 2;
                    buffer[2] = startsq += startdelta += 2;
                    buffer[3] = startsq += startdelta += 2;
                    //buffer[4] = startsq += startdelta += 2;
                    //buffer[5] = startsq += startdelta += 2;
                    //buffer[6] = startsq += startdelta += 2;
                    //buffer[7] = startsq += startdelta += 2;




                    startsq += startdelta += 2;
                    startdelta = startsq - sq[0];



                    //sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    //seed initial deltas, incrementing by 8*2 (blockSize*2)
                    //delta = new Vector<int>(new[] { 80, 96, 112, 128, 144, 160, 176, 192 });

                    //TODO get rid of allocation
                    deltaBuffer[0] = startdelta;
                    deltaBuffer[1] = startdelta += 4 << 1;
                    deltaBuffer[2] = startdelta += 4 << 1;
                    deltaBuffer[3] = startdelta += 4 << 1;
                    //deltaBuffer[4] = startdelta += 8 << 1;
                    //deltaBuffer[5] = startdelta += 8 << 1;
                    //deltaBuffer[6] = startdelta += 8 << 1;
                    //deltaBuffer[7] = startdelta += 8 << 1;



                    delta = new Vector<long>(deltaBuffer);

                    //seed delta incremements as 8*8*2 (blockSize*blocksize*2)
                    //inc = new Vector<int>(new[] { 128, 128, 128, 128, 128, 128, 128, 128 });
                    //buffer = sq.AsSpan();
                    //step = 0;
                    Current = default(Vector256<long>);
                }




                /// <summary>
                /// Generates sequence of squares starting with the next square greater than <paramref name="start"/>.
                /// If <paramref name="start"/> is a perfect square then the sequences begins at <paramref name="start"/>.
                /// </summary>
                /// <param name="start"></param>
                public SquareGeneratorAvx64(long start)
                    : this()
                {
                    MoveTo(start);
                    //bool isPerfect = MathLib.IsPerfectSquare(start, out int root);
                    //var startsq = start;
                    //if (!isPerfect)
                    //{
                    //    root = root + 1;
                    //    startsq = root * root;
                    //}
                    //var nextroot = (root + 1);
                    //var nextsq = nextroot * nextroot;
                    //var startdelta = nextsq - startsq;
                    //startdelta -= 2;

                    //sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    ////seed initial deltas, incrementing by 8*2 (blockSize*2)
                    //deltas = new[] { 80, 96, 112, 128, 144, 160, 176, 192 };
                    //incs = new[] { 128, 128, 128, 128, 128, 128, 128, 128 };


                    ////calculate the first 8 squares
                    //sq = new[] {
                    //    startsq,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2,
                    //    startsq += startdelta += 2
                    //};
                    //startsq += startdelta += 2;
                    //startdelta = startsq - sq[0];

                    ////sq = new[] { 1, 4, 9, 16, 25, 36, 49, 64 };
                    ////seed initial deltas, incrementing by 8*2 (blockSize*2)
                    ////delta = new Vector<int>(new[] { 80, 96, 112, 128, 144, 160, 176, 192 });
                    //delta = new Vector<int>(new[] {
                    //    startdelta,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1,
                    //    startdelta += 8 << 1
                    //});

                    ////seed delta incremements as 8*8*2 (blockSize*blocksize*2)
                    //inc = new Vector<int>(new[] { 128, 128, 128, 128, 128, 128, 128, 128 });
                    //buffer = sq.AsSpan();
                    //deltaBuffer = deltas.AsSpan();
                    //step = 0;// todo need to calculate step to prevent overflow.
                    //Current = default(Vector256<int>);

                }

                public bool MoveNext()
                {
                    if (step < maxStep)
                    {
                        step++;
                        var v = new Vector<long>(buffer);
                        Current = v.AsVector256<long>();
                        v = v + delta;
                        delta = delta + inc;
                        v.CopyTo(buffer);
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// Data to simplify using <see cref="AvxDivRem(int, AvxDivRemData32)"/> - Accurate up to 2^24.
            /// <see cref="AvxDivRemData32.Divisors"/>.Length should be a multiple of 4 for best performance.
            /// Up to 50% faster than <see cref="AvxDivRem(int, AvxDivRemData32)"/>.
            /// </summary>
            public class AvxDivRemData32
            {
                public int[] Divisors;
                public float[] quotients;
                public float[] remainders;
                public float[] d_divisors;
                public float[] d_reciprocals;
                public int MaxDivisor;

                private GCHandle divisorsHandle;
                private GCHandle ddivisorsHandle;
                private GCHandle reciprocalsHandle;
                private GCHandle quotientsHandle;
                private GCHandle remaindersHandle;
                public AvxDivRemData32(int[] divisors)
                {
                    this.Divisors = divisors.ToArray();
                    MaxDivisor = divisors.Max();

                    quotients = new float[divisors.Length];
                    remainders = new float[divisors.Length];
                    d_divisors = divisors.Select(x => (float)x).ToArray();

                    // get the reciprocals of our divisors so we can divide faster using multiplication
                    d_reciprocals = divisors.Select(x => 1.0f / x).ToArray();


                    divisorsHandle = MathLib.AlignPinnedArray(ref Divisors);
                    ddivisorsHandle = MathLib.AlignPinnedArray(ref d_divisors);
                    reciprocalsHandle = MathLib.AlignPinnedArray(ref d_reciprocals);
                    quotientsHandle = MathLib.AlignPinnedArray(ref quotients);
                    remaindersHandle = MathLib.AlignPinnedArray(ref remainders);

                }

                // Override the finalizer to release the pinned arrays
                ~AvxDivRemData32()
                {
                    divisorsHandle.Free();
                    ddivisorsHandle.Free();
                    reciprocalsHandle.Free();
                    quotientsHandle.Free();
                    remaindersHandle.Free();
                }
                public static AvxDivRemData32 Create(int[] divisors)
                    => new AvxDivRemData32(divisors);

                public static AvxDivRemData32_8_Divisors Create_8_Divisors(int[] divisors)
                    => AvxDivRemData32_8_Divisors.Create(divisors);

                public static AvxDivRemData32_8_Divisors CreateSmallPrimesData()
                    => Create_8_Divisors(new[] { 2, 3, 5, 7, 11, 13, 17, 19 });
            }


            /// <summary>
            /// Data to simplify using <see cref="AvxDivRem(int, AvxDivRemData32_8_Divisors)"/>
            /// 
            /// Accurate up to 2^24.
            /// 
            /// <see cref="AvxDivRemData32.Divisors"/>.Length must be exactly 8.
            /// Up to 50% faster than <see cref="AvxDivRem(int, AvxDivRemData64_4_Divisors)"/>.
            /// </summary>
            public class AvxDivRemData32_8_Divisors
                : AvxDivRemData32
            {
                public int DivisorMask;

                private AvxDivRemData32_8_Divisors(int[] divisors) : base(divisors)
                {
                    MaxDivisor = divisors.Max();
                }

                public static new AvxDivRemData32_8_Divisors Create(int[] divisors)

                {
                    if (divisors.Length != 8)
                    {
                        throw new ArgumentException("Divisors length must be 8");
                    }
                    return new AvxDivRemData32_8_Divisors(divisors);
                }
            }


            /// <summary>
            /// Data to simplify using<see cref="AvxDivRem(int, AvxDivRemData64)"/> - Accurate up to 2^52.
            /// <see cref="AvxDivRemData64.Divisors"/>.Length should be a multiple of 4 for best performance.
            /// </summary>

            public class AvxDivRemData64
            {
                public int[] Divisors;
                public double[] quotients;
                public double[] remainders;
                public double[] d_divisors;
                public double[] d_reciprocals;
                public int MaxDivisor;
                private GCHandle divisorsHandle;
                private GCHandle ddivisorsHandle;
                private GCHandle reciprocalsHandle;
                private GCHandle quotientsHandle;
                private GCHandle remaindersHandle;
                public AvxDivRemData64(int[] divisors)
                {
                    this.Divisors = divisors.ToArray();
                    MaxDivisor = divisors.Max();
                    quotients = new double[divisors.Length];
                    remainders = new double[divisors.Length];
                    d_divisors = divisors.Select(x => (double)x).ToArray();

                    // get the reciprocals of our divisors so we can divide faster using multiplication
                    d_reciprocals = divisors.Select(x => 1.0 / x).ToArray();

                    divisorsHandle = MathLib.AlignPinnedArray(ref Divisors);
                    ddivisorsHandle = MathLib.AlignPinnedArray(ref d_divisors);
                    reciprocalsHandle = MathLib.AlignPinnedArray(ref d_reciprocals);
                    quotientsHandle = MathLib.AlignPinnedArray(ref quotients);
                    remaindersHandle = MathLib.AlignPinnedArray(ref remainders);

                }

                // Override the finalizer to release the pinned arrays
                ~AvxDivRemData64()
                {
                    divisorsHandle.Free();
                    ddivisorsHandle.Free();
                    reciprocalsHandle.Free();
                    quotientsHandle.Free();
                    remaindersHandle.Free();
                }

                public static AvxDivRemData64 Create(int[] divisors)
                    => new AvxDivRemData64(divisors);
                public static AvxDivRemData64_4_Divisors Create_4_Divisors(int[] divisors)
                => AvxDivRemData64_4_Divisors.Create(divisors);
            }

            /// <summary>
            /// Data to simplify using <see cref="AvxDivRem(int, AvxDivRemData64_4_Divisors)"/>
            /// 
            /// Accurate up to 2^52.
            /// 
            /// <see cref="AvxDivRemData32.Divisors"/>.Length must be exactly 4.
            /// </summary>
            public class AvxDivRemData64_4_Divisors
            : AvxDivRemData64
            {
                public int DivisorMask;
                private AvxDivRemData64_4_Divisors(int[] divisors) : base(divisors)
                {
                }

                public static new AvxDivRemData64_4_Divisors Create(int[] divisors)

                {
                    if (divisors.Length != 4)
                    {
                        throw new ArgumentException("Divisors length must be 8");
                    }
                    return new AvxDivRemData64_4_Divisors(divisors);
                }
            }



            public static unsafe int AvxDivRem(int N, AvxDivRemData64 data)
            {
                return AvxDivRemBatched(N, data.d_divisors, data.d_reciprocals, ref data.quotients, ref data.remainders);
            }

            public static unsafe int AvxDivRem(long N, AvxDivRemData64 data)
            {
                return AvxDivRemBatched(N, data.d_divisors, data.d_reciprocals, ref data.quotients, ref data.remainders);
            }

            /// <summary>
            /// Specialized to overload to process just 4 divisors of a single Vector256<Double> without a for loop.
            /// </summary>
            /// <param name="N"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public static unsafe int AvxDivRem(int N, AvxDivRemData64_4_Divisors data)
            {
                return AvxDivRemBatched_4_Divisors(N, data.d_divisors, data.d_reciprocals, ref data.quotients, ref data.remainders);
            }

            public static unsafe int AvxDivRem(int N, int[] divisors,
                ref double[] quotients, ref double[] remainders)
            {
                double[] d_divisors = divisors.Select(x => (double)x).ToArray();

                // get the reciprocals of our divisors so we can divide faster using multiplication
                double[] d_reciprocals = divisors.Select(x => 1.0 / x).ToArray();

                //align the arrays to a 32 bit word boundary
                MathLib.Align(ref d_divisors);
                MathLib.Align(ref d_reciprocals);
                MathLib.Align(ref quotients);
                MathLib.Align(ref remainders);

                return AvxDivRemBatched(N, d_divisors, d_reciprocals, ref quotients, ref remainders);
            }

            public static unsafe int AvxDivRemBatched(int N, double[] d_divisors, double[] d_reciprocals,
                ref double[] quotients, ref double[] remainders)
            {
                const int VectorSize = 4;
                const int VectorSizeMask = VectorSize - 1;

                Vector256<double> errVector = Vector256.Create((double)err);
                Vector256<double> nVector = Vector256.Create((double)N);
                int dividesExactlyCount = 0;

                // Create vectors to store the quotient and remainder
                var quotient = Vector256<double>.Zero;
                var remainder = Vector256<double>.Zero;



                fixed (double* divPtr = &d_divisors[0])
                fixed (double* recipPtr = &d_reciprocals[0])
                fixed (double* quotientPtr = &quotients[0])
                fixed (double* remainderPtr = &remainders[0])
                {
                    for (int i = 0; i < d_divisors.Length; i += VectorSize)
                    {
                        // Load divisors and reciprocals in batches of 4
                        Vector256<double> divVect = Avx.LoadAlignedVector256(divPtr + i);
                        Vector256<double> recipVec = Avx.LoadAlignedVector256(recipPtr + i);
                        Vector256<double> quotientVect = Avx.LoadAlignedVector256(quotientPtr + i);
                        Vector256<double> remainderVect = Avx.LoadAlignedVector256(remainderPtr + i);
                        // Multiply: n * (1.0/d)
                        Vector256<double> mulVec = Avx.Multiply(recipVec, nVector);

                        // Add error: n * (1.0/d) + 0.00000001
                        Vector256<double> addErrVec = Avx.Add(mulVec, errVector);

                        // Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001)
                        quotient = Avx.Floor(addErrVec);

                        // Calculate the multiplicand (q * d)
                        Vector256<double> multiplicand = Avx.Multiply(quotient, divVect);

                        // Compare N == (q * d)
                        Vector256<double> eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);

                        // Reduce the comparison N == (q * d) into a bit mask
                        var dividesExactlyMask = Avx.MoveMask(eqVector);

                        // Get a count of N == (q * d) from the mask
                        dividesExactlyCount += MathLib.PopCount(dividesExactlyMask);

                        //        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
                        // Append the quotient and remainder vectors

                        remainder = Avx.Subtract(nVector, multiplicand);

                        // Store the quotient and remainder vectors in the arrays
                        Avx.Store(quotientPtr + i, quotient);
                        Avx.Store(remainderPtr + i, remainder);

                    }
                    // handle remianing divisors normally
                    if ((d_divisors.Length & VectorSizeMask) > 0)
                    {

                    }
                }

                return dividesExactlyCount;
            }

            public static unsafe int AvxDivRemBatched(long N, double[] d_divisors, double[] d_reciprocals,
               ref double[] quotients, ref double[] remainders)
            {
                const int VectorSize = 4;
                const int VectorSizeMask = VectorSize - 1;

                Vector256<double> errVector = Vector256.Create((double)err);
                Vector256<double> nVector = Vector256.Create((double)N);
                int dividesExactlyCount = 0;

                // Create vectors to store the quotient and remainder
                var quotient = Vector256<double>.Zero;
                var remainder = Vector256<double>.Zero;



                fixed (double* divPtr = &d_divisors[0])
                fixed (double* recipPtr = &d_reciprocals[0])
                fixed (double* quotientPtr = &quotients[0])
                fixed (double* remainderPtr = &remainders[0])
                {
                    for (int i = 0; i < d_divisors.Length; i += VectorSize)
                    {
                        // Load divisors and reciprocals in batches of 4
                        Vector256<double> divVect = Avx.LoadAlignedVector256(divPtr + i);
                        Vector256<double> recipVec = Avx.LoadAlignedVector256(recipPtr + i);
                        Vector256<double> quotientVect = Avx.LoadAlignedVector256(quotientPtr + i);
                        Vector256<double> remainderVect = Avx.LoadAlignedVector256(remainderPtr + i);
                        // Multiply: n * (1.0/d)
                        Vector256<double> mulVec = Avx.Multiply(recipVec, nVector);

                        // Add error: n * (1.0/d) + 0.00000001
                        Vector256<double> addErrVec = Avx.Add(mulVec, errVector);

                        // Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001)
                        quotient = Avx.Floor(addErrVec);

                        // Calculate the multiplicand (q * d)
                        Vector256<double> multiplicand = Avx.Multiply(quotient, divVect);

                        // Compare N == (q * d)
                        Vector256<double> eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);

                        // Reduce the comparison N == (q * d) into a bit mask
                        var dividesExactlyMask = Avx.MoveMask(eqVector);

                        // Get a count of N == (q * d) from the mask
                        dividesExactlyCount += MathLib.PopCount(dividesExactlyMask);

                        //        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
                        // Append the quotient and remainder vectors

                        remainder = Avx.Subtract(nVector, multiplicand);

                        // Store the quotient and remainder vectors in the arrays
                        Avx.Store(quotientPtr + i, quotient);
                        Avx.Store(remainderPtr + i, remainder);

                    }
                    // handle remianing divisors normally
                    if ((d_divisors.Length & VectorSizeMask) > 0)
                    {

                    }
                }

                return dividesExactlyCount;
            }

            public static unsafe int AvxDivRemBatched_4_Divisors(int N, double[] d_divisors, double[] d_reciprocals,
              ref double[] quotients, ref double[] remainders)
            {

                Vector256<double> errVector = Vector256.Create((double)err);
                Vector256<double> nVector = Vector256.Create((double)N);
                int dividesExactlyCount = 0;

                // Create vectors to store the quotient and remainder
                var quotient = Vector256<double>.Zero;
                var remainder = Vector256<double>.Zero;



                fixed (double* divPtr = &d_divisors[0])
                fixed (double* recipPtr = &d_reciprocals[0])
                fixed (double* quotientPtr = &quotients[0])
                fixed (double* remainderPtr = &remainders[0])
                {

                    // Load divisors and reciprocals in batches of 4
                    Vector256<double> divVect = Avx.LoadAlignedVector256(divPtr);
                    Vector256<double> recipVec = Avx.LoadAlignedVector256(recipPtr);
                    Vector256<double> quotientVect = Avx.LoadAlignedVector256(quotientPtr);
                    Vector256<double> remainderVect = Avx.LoadAlignedVector256(remainderPtr);
                    // Multiply: n * (1.0/d)
                    Vector256<double> mulVec = Avx.Multiply(recipVec, nVector);

                    // Add error: n * (1.0/d) + 0.00000001
                    Vector256<double> addErrVec = Avx.Add(mulVec, errVector);

                    // Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001)
                    quotient = Avx.Floor(addErrVec);

                    // Calculate the multiplicand (q * d)
                    Vector256<double> multiplicand = Avx.Multiply(quotient, divVect);

                    // Compare N == (q * d)
                    Vector256<double> eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);

                    // Reduce the comparison N == (q * d) into a bit mask
                    int dividesExactlyMask = Avx.MoveMask(eqVector);

                    // Get a count of N == (q * d) from the mask
                    dividesExactlyCount += MathLib.PopCount(dividesExactlyMask);

                    //        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
                    // Append the quotient and remainder vectors

                    remainder = Avx.Subtract(nVector, multiplicand);

                    // Store the quotient and remainder vectors in the arrays
                    Avx.Store(quotientPtr, quotient);
                    Avx.Store(remainderPtr, remainder);


                }

                return dividesExactlyCount;
            }


            public static unsafe int AvxDivRem(int N, AvxDivRemData32 data)
            {
                return AvxDivRemBatched(N, data.d_divisors, data.d_reciprocals, ref data.quotients, ref data.remainders);
            }


            /// Specialized to overload to process just 8 divisors of a single Vector256<Float> without a for loop.
            public static unsafe int AvxDivRem(int N, AvxDivRemData32_8_Divisors data)
            {
                return AvxDivRemBatched_8_Divisors(N, data.d_divisors, data.d_reciprocals, ref data.DivisorMask, ref data.quotients, ref data.remainders);
            }

            //bsmooth fails at 2151680 with errf 0.001
            public const float errf = 0.01f;
            public const double err = 0.00000001f;


            public static unsafe int AvxDivRemBatched(int N, float[] d_divisors, float[] d_reciprocals,
                 ref float[] quotients, ref float[] remainders)
            {
                const int VectorSize = 8;
                const int VectorSizeMask = VectorSize - 1;

                Vector256<float> errVector = Vector256.Create(errf);
                Vector256<float> nVector = Vector256.Create((float)N);
                int dividesExactlyCount = 0;

                // Create vectors to store the quotient and remainder
                var quotient = Vector256<float>.Zero;
                var remainder = Vector256<float>.Zero;


                int alignment = 0;
                fixed (float* divPtr = &d_divisors[0])
                fixed (float* recipPtr = &d_reciprocals[0])
                fixed (float* quotientPtr = &quotients[0])
                fixed (float* remainderPtr = &remainders[0])

                {
                    for (int i = 0; i < d_divisors.Length; i += VectorSize)
                    {
                        Vector256<float> divVect = Avx.LoadAlignedVector256(divPtr + i);
                        Vector256<float> recipVec = Avx.LoadAlignedVector256(recipPtr + i);
                        Vector256<float> quotientVect = Avx.LoadAlignedVector256(quotientPtr + i);
                        Vector256<float> remainderVect = Avx.LoadAlignedVector256(remainderPtr + i);


                        // Multiply: n * (1.0/d)
                        var mulVec = Avx.Multiply(recipVec, nVector);

                        // Add error: n * (1.0/d) + 0.00000001
                        Vector256<float> addErrVec = Avx.Add(mulVec, errVector);

                        // Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001)
                        quotient = Avx.Floor(addErrVec);

                        // Calculate the multiplicand (q * d)
                        Vector256<float> multiplicand = Avx.Multiply(quotient, divVect);

                        // Compare N == (q * d)
                        Vector256<float> eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);

                        // Reduce the comparison N == (q * d) into a bit mask
                        int dividesExactlyMask = Avx.MoveMask(eqVector);

                        // Get a count of N == (q * d) from the mask
                        dividesExactlyCount += MathLib.PopCount(dividesExactlyMask);

                        //        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
                        // Append the quotient and remainder vectors

                        remainder = Avx.Subtract(nVector, multiplicand);

                        // Store the quotient and remainder vectors in the arrays

                        Avx.Store(quotientPtr + i, quotient);
                        Avx.Store(remainderPtr + i, remainder);

                    }
                    // handle remianing divisors normally
                    if ((d_divisors.Length & VectorSizeMask) > 0)
                    {

                    }
                }

                return dividesExactlyCount;
            }

            /// <summary>
            /// Performs divrem operation on N using specified data, and returns as soon as a factor is found.
            /// </summary>
            /// <param name="N"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public static unsafe int AvxFindFirstFactor(int N, AvxDivRemData32 data)
                => AvxFindFirstFactor(N, data, out int _);

            public static unsafe int AvxFindFirstFactor(int N, AvxDivRemData32 data, out int opcount)
            {
                return AvxFindFirstFactor(N, data.d_divisors, data.d_reciprocals, ref data.quotients, ref data.remainders, out opcount);
            }

            public static unsafe int AvxFindFirstFactor(int N, float[] d_divisors, float[] d_reciprocals,
               ref float[] quotients, ref float[] remainders, out int opcount)
            {
                const int VectorSize = 8;
                const int VectorSizeMask = VectorSize - 1;
                opcount = 0;
                if (MathLib.IsPerfectSquare(N, out int root))
                {
                    quotients[0] = root;
                    remainders[0] = 0;
                    return 1;
                }

                //TODO
                Vector256<float> errVector = Vector256.Create(errf);
                Vector256<float> nVector = Vector256.Create((float)N);
                int dividesExactlyCount = 0;

                // Create vectors to store the quotient and remainder
                var quotient = Vector256<float>.Zero;
                var remainder = Vector256<float>.Zero;


                int alignment = 0;
                fixed (float* divPtr = &d_divisors[0])
                fixed (float* recipPtr = &d_reciprocals[0])
                fixed (float* quotientPtr = &quotients[0])
                fixed (float* remainderPtr = &remainders[0])

                {
                    for (int i = 0; dividesExactlyCount == 0 && d_divisors[i] <= root && i < d_divisors.Length; i += VectorSize)
                    {
                        opcount++;
                        Vector256<float> divVect = Avx.LoadAlignedVector256(divPtr + i);
                        Vector256<float> recipVec = Avx.LoadAlignedVector256(recipPtr + i);
                        Vector256<float> quotientVect = Avx.LoadAlignedVector256(quotientPtr + i);
                        Vector256<float> remainderVect = Avx.LoadAlignedVector256(remainderPtr + i);


                        // Multiply: n * (1.0/d)
                        var mulVec = Avx.Multiply(recipVec, nVector);

                        // Add error: n * (1.0/d) + 0.00000001
                        Vector256<float> addErrVec = Avx.Add(mulVec, errVector);

                        // Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001)
                        quotient = Avx.Floor(addErrVec);

                        // Calculate the multiplicand (q * d)
                        Vector256<float> multiplicand = Avx.Multiply(quotient, divVect);

                        // Compare N == (q * d)
                        Vector256<float> eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);

                        // Reduce the comparison N == (q * d) into a bit mask
                        int dividesExactlyMask = Avx.MoveMask(eqVector);

                        // Get a count of N == (q * d) from the mask
                        dividesExactlyCount += MathLib.PopCount(dividesExactlyMask);

                        //        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
                        // Append the quotient and remainder vectors

                        remainder = Avx.Subtract(nVector, multiplicand);

                        // Store the quotient and remainder vectors in the arrays

                        Avx.Store(quotientPtr + i, quotient);
                        Avx.Store(remainderPtr + i, remainder);

                    }
                    // handle remianing divisors normally
                    if ((d_divisors.Length & VectorSizeMask) > 0)
                    {

                    }
                }

                return dividesExactlyCount;
            }


            public static unsafe int AvxDivRemBatched_8_Divisors(int N, float[] d_divisors, float[] d_reciprocals, ref int dividesExactlyMask,
                   ref float[] quotients, ref float[] remainders)
            {

                Vector256<float> errVector = Vector256.Create(errf);
                Vector256<float> nVector = Vector256.Create((float)N);
                int dividesExactlyCount = 0;

                // Create vectors to store the quotient and remainder
                var quotient = Vector256<float>.Zero;
                var remainder = Vector256<float>.Zero;
                dividesExactlyMask = 0;


                fixed (float* divPtr = &d_divisors[0])
                fixed (float* recipPtr = &d_reciprocals[0])
                fixed (float* quotientPtr = &quotients[0])
                fixed (float* remainderPtr = &remainders[0])
                {

                    // Load divisors and reciprocals in batches of 4
                    Vector256<float> divVect = Avx.LoadAlignedVector256(divPtr);
                    Vector256<float> recipVec = Avx.LoadAlignedVector256(recipPtr);
                    Vector256<float> quotientVect = Avx.LoadAlignedVector256(quotientPtr);
                    Vector256<float> remainderVect = Avx.LoadAlignedVector256(remainderPtr);
                    // Multiply: n * (1.0/d)
                    var mulVec = Avx.Multiply(recipVec, nVector);

                    // Add error: n * (1.0/d) + 0.00000001
                    Vector256<float> addErrVec = Avx.Add(mulVec, errVector);

                    // Floor the quotient: q = (int)(n * (1.0/d) + 0.00000001)
                    quotient = Avx.Floor(addErrVec);

                    // Calculate the multiplicand (q * d)
                    Vector256<float> multiplicand = Avx.Multiply(quotient, divVect);

                    // Compare N == (q * d)
                    Vector256<float> eqVector = Avx.Compare(multiplicand, nVector, FloatComparisonMode.UnorderedEqualNonSignaling);

                    // Reduce the comparison N == (q * d) into a bit mask
                    dividesExactlyMask = Avx.MoveMask(eqVector);

                    // Get a count of N == (q * d) from the mask
                    dividesExactlyCount += MathLib.PopCount(dividesExactlyMask);

                    //        ///   VINSERTF128 ymm, ymm, xmm/m128, imm8
                    // Append the quotient and remainder vectors

                    remainder = Avx.Subtract(nVector, multiplicand);

                    // Store the quotient and remainder vectors in the arrays
                    Avx.Store(quotientPtr, quotient);
                    Avx.Store(remainderPtr, remainder);


                }

                return dividesExactlyCount;
            }


            /// <summary>
            /// Demo Method: Only processes first 4 divisors. Produces the quotient and remainder vectors
            /// produced by dividing <paramref name="N"/> by the <paramref name="d_divisors"/> to produce
            /// the quotient and the remainder of the specified numbers.
            /// 
            /// Essentially equivalent to <see cref="AvxDivRemBatched_4_Divisors(int, double[], double[], ref double[], ref double[])"/> 
            /// but fully documented with links to AVX instructions used.
            /// </summary>
            /// <param name="N"></param>
            /// <param name="d_divisors"></param>
            /// <param name="d_reciprocals"></param>
            /// <param name="quotient"></param>
            /// <param name="remainder"></param>
            /// <returns>The count of divisors that divided N exact and sets quotient and the remainder vectors for the specified numbers.</returns>


            public static unsafe bool IsBSmooth(int N, AvxDivRemData64 data)
            {
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;

                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var quotients = data.quotients;
                    var remainders = data.remainders;
                    int res;
                    int m;
                    var max = data.MaxDivisor;
                    for (; q > max && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            if (q == N)
                                // we can actually break here if q<= max but 
                                q = (int)quotients[i];

                            //shoul test in any q is greater than N before even attempting to trial divide n?
                            var r = recips[i];
                            while (true)
                            {
                                res = (int)((q * r) + errf);
                                m = (int)(res * divisors[i]);
                                if (m != q)
                                    break;
                                if (m == q)
                                {
                                    q = res;
                                    if (q <= max)
                                        break;
                                }
                            }
                        }
                    }
                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(long N, AvxDivRemData64 data)
            {
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;

                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var quotients = data.quotients;
                    var remainders = data.remainders;
                    int res;
                    int m;
                    var max = data.MaxDivisor;
                    for (; q > max && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            if (q == N)
                                // we can actually break here if q<= max but 
                                q = (int)quotients[i];

                            //shoul test in any q is greater than N before even attempting to trial divide n?
                            var r = recips[i];
                            while (true)
                            {
                                res = (int)((q * r) + errf);
                                m = (int)(res * divisors[i]);
                                if (m != q)
                                    break;
                                if (m == q)
                                {
                                    q = res;
                                    if (q <= max)
                                        break;
                                }
                            }
                        }
                    }
                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(int N, AvxDivRemData64 data, out int factorBaseMask, out int gf2Mask)
            {
                factorBaseMask = gf2Mask = 0;
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;
                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var remainders = data.remainders;
                    int res;
                    int mask = 1;
                    double r;

                    double d;
                    for (; q > 1 && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            r = recips[i];
                            d = divisors[i];
                            while (q == (int)((res = (int)((q * r) + errf)) * d))
                            {
                                factorBaseMask = factorBaseMask | mask;
                                gf2Mask = gf2Mask ^ mask;
                                if (q < 2)
                                    break;
                                q = res;
                            }
                            /* while (true)
                             {

                                 res = ((int)((q * r) + errf));
                                 m = (int)(res * d);
                                 if (m != q)
                                     break;
                                 else // (m == q)
                                 {
                                     gf2Mask = gf2Mask ^ mask;
                                     q = res;
                                     if (q < 2)
                                         break;
                                 }
                             }*/
                        }
                        mask <<= 1;
                    }


                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(long N, AvxDivRemData64 data, out long factorBaseMask, out long gf2Mask)
            {
                factorBaseMask = gf2Mask = 0;
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;
                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var remainders = data.remainders;
                    long res;
                    long mask = 1;
                    double r;

                    double d;
                    for (; q > 1 && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            r = recips[i];
                            d = divisors[i];
                            while (q == (long)((res = (long)((q * r) + errf)) * d))
                            {
                                factorBaseMask = factorBaseMask | mask;
                                gf2Mask = gf2Mask ^ mask;
                                if (q < 2)
                                    break;
                                q = res;
                            }
                        }
                        mask <<= 1;
                    }


                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(long N, AvxDivRemData64 data, BitArray factorBaseMask, BitArray gf2Mask)
            {
                gf2Mask.SetAll(false);
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;
                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var remainders = data.remainders;
                    long res;
                    int mask = 1;
                    double r;

                    double d;
                    for (; q > 1 && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            r = recips[i];
                            d = divisors[i];
                            while (q == (long)((res = (long)((q * r) + errf)) * d))
                            {
                                factorBaseMask[i] = true;
                                gf2Mask[i] = gf2Mask[i] ^ gf2Mask[i];
                                if (q < 2)
                                    break;
                                q = res;
                            }
                        }
                        mask <<= 1;
                    }


                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(int N, AvxDivRemData32 data)
            {
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;

                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var quotients = data.quotients;
                    var remainders = data.remainders;
                    int res;
                    int m;
                    var max = data.MaxDivisor;
                    for (; q > max && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            if (q == N)
                                // we can actually break here if q<= max but 
                                q = (int)quotients[i];

                            //shoul test in any q is greater than N before even attempting to trial divide n?
                            var r = recips[i];
                            while (true)
                            {
                                res = (int)((q * r) + errf);
                                m = (int)(res * divisors[i]);
                                if (m != q)
                                    break;
                                if (m == q)
                                {
                                    q = res;
                                    if (q <= max)
                                        break;
                                }
                            }
                        }
                    }
                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(int N, AvxDivRemData32_8_Divisors data)
            {
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;
                    var mask = data.DivisorMask;
                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var quotients = data.quotients;

                    int res;
                    int m;
                    var max = data.MaxDivisor;
                    for (; q > max && i < 8; i++)
                    {
                        if ((mask & (1 << i)) > 0)
                        {
                            if (q == N)
                                // we can actually break here if q<= max but 
                                q = (int)quotients[i];

                            //shoul test in any q is greater than N before even attempting to trial divide n?
                            var r = recips[i];
                            while (true)
                            {
                                res = (int)((q * r) + errf);
                                m = (int)(res * divisors[i]);
                                if (m != q)
                                    break;
                                if (m == q)
                                {
                                    q = res;
                                    if (q <= max)
                                        break;
                                }
                            }
                        }
                    }
                    result = q <= data.MaxDivisor;
                }
                return result;
            }

            public static unsafe bool IsBSmooth(int N, AvxDivRemData32_8_Divisors data, out int factorBaseMask, out int gf2Mask)
            {
                factorBaseMask = gf2Mask = 0;
                var count = AvxDivRem(N, data);
                bool result = count > 0;
                if (result)
                {
                    var q = N;
                    var mask = data.DivisorMask;
                    int i = 0;
                    var recips = data.d_reciprocals;
                    var divisors = data.d_divisors;
                    var remainders = data.remainders;
                    int res;
                    double r;

                    double d;
                    for (; q > 1 && i < remainders.Length; i++)
                    {
                        if (remainders[i] == 0)
                        {
                            r = recips[i];
                            d = divisors[i];
                            while (q == (int)((res = (int)((q * r) + errf)) * d))
                            {
                                factorBaseMask = factorBaseMask | mask;
                                gf2Mask = gf2Mask ^ mask;
                                if (q < 2)
                                    break;
                                q = res;
                            }
                        }
                        mask <<= 1;
                    }


                    result = q <= data.MaxDivisor;
                }
                return result;
            }
        }
    }
    public class LL
    {
        /* MIT License

Copyright (c) 2018 Nemanja Mijailovic

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
        */

        //

        //TODO: Autofix alignment: https://mijailovic.net/2018/07/20/alignment-and-pipelining/

        /*
		[Params(8, 32)]
		public int Alignment { get; set; }

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
         * */

        public void Run()
        {

        }

        private const ulong AlignmentMask = 31UL;
        private const int VectorSizeInInts = 8;
        private const int BlockSizeInInts = 32;

        public unsafe static int Sum(int[] source)
        {
            fixed (int* ptr = &source[0])
            {
                var pos = 0;
                var sum = Vector256<int>.Zero;// < int>.Zero;

                for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
                {
                    var current = Avx.LoadVector256(ptr + pos);
                    sum = Avx2.Add(current, sum);
                }

                var temp = stackalloc int[VectorSizeInInts];
                Avx.Store(temp, sum);

                var final = Sum(temp, VectorSizeInInts);
                final += Sum(ptr + pos, source.Length - pos);

                return final;
            }
        }

        public unsafe static int SumAligned(int[] source)
        {
            fixed (int* ptr = &source[0])
            {
                var aligned = (int*)(((ulong)ptr + AlignmentMask) & ~AlignmentMask);
                var pos = (int)(aligned - ptr);
                var sum = Vector256<int>.Zero;
                var final = Sum(ptr, pos);

                for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
                {
                    var current = Avx.LoadAlignedVector256(ptr + pos);
                    sum = Avx2.Add(current, sum);
                }

                var temp = stackalloc int[VectorSizeInInts];
                Avx.Store(temp, sum);

                final += Sum(temp, VectorSizeInInts);
                final += Sum(ptr + pos, source.Length - pos);

                return final;
            }
        }

        public unsafe static int SumPipelined(int[] source)
        {
            fixed (int* ptr = &source[0])
            {
                var pos = 0;
                var sum = Vector256<int>.Zero;

                for (; pos <= source.Length - BlockSizeInInts; pos += BlockSizeInInts)
                {
                    var block0 = Avx.LoadVector256(ptr + pos + 0 * VectorSizeInInts);
                    var block1 = Avx.LoadVector256(ptr + pos + 1 * VectorSizeInInts);
                    var block2 = Avx.LoadVector256(ptr + pos + 2 * VectorSizeInInts);
                    var block3 = Avx.LoadVector256(ptr + pos + 3 * VectorSizeInInts);

                    sum = Avx2.Add(block0, sum);
                    sum = Avx2.Add(block1, sum);
                    sum = Avx2.Add(block2, sum);
                    sum = Avx2.Add(block3, sum);
                }

                for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
                {
                    var current = Avx.LoadVector256(ptr + pos);
                    sum = Avx2.Add(current, sum);
                }

                var temp = stackalloc int[VectorSizeInInts];
                Avx.Store(temp, sum);

                var final = Sum(temp, VectorSizeInInts);
                final += Sum(ptr + pos, source.Length - pos);

                return final;
            }
        }

        public unsafe static int SumAlignedPipelined(int[] source)
        {
            fixed (int* ptr = &source[0])
            {
                var aligned = (int*)(((ulong)ptr + AlignmentMask) & ~AlignmentMask);
                var pos = (int)(aligned - ptr);
                var sum = Vector256<int>.Zero;
                var final = Sum(ptr, pos);

                for (; pos <= source.Length - BlockSizeInInts; pos += BlockSizeInInts)
                {
                    var block0 = Avx.LoadAlignedVector256(ptr + pos + 0 * VectorSizeInInts);
                    var block1 = Avx.LoadAlignedVector256(ptr + pos + 1 * VectorSizeInInts);
                    var block2 = Avx.LoadAlignedVector256(ptr + pos + 2 * VectorSizeInInts);
                    var block3 = Avx.LoadAlignedVector256(ptr + pos + 3 * VectorSizeInInts);

                    sum = Avx2.Add(block0, sum);
                    sum = Avx2.Add(block1, sum);
                    sum = Avx2.Add(block2, sum);
                    sum = Avx2.Add(block3, sum);
                }

                for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
                {
                    var current = Avx.LoadAlignedVector256(ptr + pos);
                    sum = Avx2.Add(current, sum);
                }

                var temp = stackalloc int[VectorSizeInInts];
                Avx.Store(temp, sum);

                final += Sum(temp, VectorSizeInInts);
                final += Sum(ptr + pos, source.Length - pos);

                return final;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static int Sum(int* source, int length)
        {
            int sum = 0;

            for (int i = 0; i < length; ++i)
            {
                sum += source[i];
            }

            return sum;
        }


    }
}