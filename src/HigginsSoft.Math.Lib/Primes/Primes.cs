/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using MathGmp.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace HigginsSoft.Math.Lib
{
    public partial class Primes
    {


        public static readonly int[] IntFactorPrimes = PrimeData.IntFactorPrimes;
        public static readonly int[] UintFactorPrimes = PrimeData.UintFactorPrimes;

        /// <summary>
        /// Returns the number of primes less than max value
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Count(int max)
        {
            // TODO: Implement range: Range(start, nextPower2(start) + (counts[previousPowerOf2(end)] - counts[nextPower2(start)]) + range(previousPowerOf2(end), end);
            if (max < 2) return 0;
            GmpInt a = max;
            var bits = a.BitLength;
            var data = PrimeData.Counts[bits - 1];
            var count = data.Count;
            if (!a.IsPowerOfTwo)
            {
                if (max <= data.NextPrime)
                {
                    if (max == data.NextPrime)
                        count += 1;
                }
                else
                {
                    var nextData = PrimeData.Counts[bits];
                    if (max >= nextData.MaxPrime)
                    {
                        count = nextData.Count;
                    }
                    else
                    {
                        var start = (int)data.NextPrime;
                        //TODO: Determine number of windows and launch parallel count.
                        //var sieveSize = max - start;
                        //var windowSize = PrimeGeneratorUnsafe.WindowSize;
                        //var windows = Math.Ceiling(sieveSize / windowSize);


                        //if (windows > 1)
                        //{
                        //    // sieve start window on 1 thread.
                        //    // sieve end window on another thread
                        //    // sieve windows 1..n-1 using parallel.
                        //    or just luanch parallel 
                        //}
                        var rangeCount = new PrimeGeneratorUnsafe(start, max).Count();
                        count += rangeCount;
                    }
                }
            }
            return count;
        }

        public static int Count(int start, int end)
        {
            if (end <= start)
            {
                throw new ArgumentOutOfRangeException(nameof(end), "End must be greater than start");
            }
            if (start <= 2) return Count(end);
            if (end < 2) return 0;
            if (end < 2) end = 2;



            var startBits = MathLib.BitLength(start);
            // start between 2^startBits and 2^(startBits+1)
            var endBits = MathLib.BitLength(end);
            // end between 2^endBits and 2^(endBits+1)

            var startPowerOfTwo = MathLib.IsPowerOfTwo(start, out int startExponent);
            var startData = PrimeData.Counts[startBits - 1];
            //numprimes less than this bitrange = startData.Count;
            // count from startData.N= to Start-1;

            var nextPowerOf2 = startData.N << 1;
            var limit = end < nextPowerOf2 ? end : nextPowerOf2;
            int count = 0;
            int n = 0;
            PrimeData next = next = PrimeData.Counts[startBits];
            n = next.N;
            if (startPowerOfTwo && nextPowerOf2 <= end)
            {


                count = next.Count - startData.Count;
                startBits++;
            }
            else
            {
                count = new PrimeGeneratorUnsafe(start, limit).Count();
                n = limit;
                startBits++;
            }

            // limit==end means all primes read.
            if (limit != end)
            {
                var endData = PrimeData.Counts[endBits - 1];
                while (n < endData.N)
                {
                    var temp = PrimeData.Counts[startBits++];
                    var range = temp.Count - next.Count;
                    count += range;
                    next = temp;
                    n = next.N;

                }
                if (n < end)
                {

                    if (end == endData.NextPrime)
                    {
                        count++;
                    }
                    else
                    {
                        var nextData = PrimeData.Counts[endBits];
                        if (end == nextData.MaxPrime)
                        {
                            count += nextData.Count - endData.Count;
                        }
                        else
                        {
                            var endCount = new PrimeGeneratorUnsafe(endData.N, end).Count();
                            count += endCount;
                        }
                    }
                }

            }

            return count;


        }

        public static int[] Primes16 => new[] { 2, 3, 5, 7, 11, 13 };
        public static int[] Primes256 =>
            Primes16.Concat(
                Enumerable.Range(0, (256 >> 1) - (16 >> 1))
                .Select(x => 16 + (x << 1) + 1)
                .Where(x =>
                    IsCoPrime(x, Primes16)
                )
            )
            .ToArray();

        public static bool IsCoPrime(int x, int[] primes)
        {
            bool result = primes.All(prime => x % prime != 0);
            return result;
        }


        public static int[] Primes65536 =>
            Primes256.Concat(
                Enumerable.Range(0, (65536 >> 1) - (256 >> 1))
                .Select(x => 256 + (x << 1) + 1)
                .Where(x =>
                    IsCoPrime(x, Primes256))
                )
            .ToArray();




    }
}