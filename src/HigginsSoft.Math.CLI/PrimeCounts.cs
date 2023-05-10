/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.CLI
{
    public class PrimeCounts
    {
        public TimeSpan TimeCountsParallel(int powerOfTwo)
        {
            var sw = Stopwatch.StartNew();
            TestPrimeGeneratorParallel(powerOfTwo, 8);
            sw.Stop();

            Console.WriteLine($"Completed {nameof(TimeCountsParallel)}(2^{powerOfTwo}) in {sw.Elapsed}");
            return sw.Elapsed;
        }

        public TimeSpan TimeCountsGenerator(int powerOfTwo)
        {
            var sw = Stopwatch.StartNew();

            TestPrimeGenerator(powerOfTwo);
            sw.Stop();

            Console.WriteLine($"Completed {nameof(TimeCountsGenerator)}(2^{powerOfTwo}) in {sw.Elapsed}");
            return sw.Elapsed;
        }

        public TimeSpan TimeCountsGeneratorRef(int powerOfTwo)
        {
            var sw = Stopwatch.StartNew();

            TestPrimeGeneratorRef(powerOfTwo);
            sw.Stop();

            Console.WriteLine($"Completed {nameof(TimeCountsGeneratorRef)}(2^{powerOfTwo}) in {sw.Elapsed}");
            return sw.Elapsed;
        }

        public TimeSpan TimeCountsUnsafe(int powerOfTwo)
        {
            var sw = Stopwatch.StartNew();

            TestPrimeGeneratorUnsafe(powerOfTwo);
            sw.Stop();

            Console.WriteLine($"Completed {nameof(TimeCountsUnsafe)}(2^{powerOfTwo}) in {sw.Elapsed}");
            return sw.Elapsed;
        }

        public TimeSpan TimeRangeCountsUnsafe(int powerOfTwo)
        {
            var sw = Stopwatch.StartNew();


            TestPrimeGeneratorUnsafeRange(powerOfTwo);
            sw.Stop();

            Console.WriteLine($"Completed {nameof(TimeRangeCountsUnsafe)}(2^{(powerOfTwo - 1)}-2^{powerOfTwo}) in {sw.Elapsed}");
            return sw.Elapsed;
        }





        private void TestPrimeGeneratorParallel(int bits, long max, ProcessorCount threadCount, out int count, out int previousPrime, out int currentPrime)
        {

            var gen = new PrimeGeneratorParallel((int)max);
            count = 0;
            var iter = gen.GetEnumerator();
            while (iter.MoveNext())
            {
                count++;
                if (gen.Current >= max)
                {
                    break;
                }
            }
            previousPrime = gen.Previous;
            currentPrime = gen.Current;


        }

        private void TestPrimeGeneratorParallel(int powerOfTwo, int threadCount = 4)
        {
            ProcessorCount processorCount = (ProcessorCount)threadCount;
            var expected = PrimeData.Counts[powerOfTwo];
            TestPrimeGeneratorParallel(powerOfTwo, expected.MaxPrime, processorCount, out int count, out int previousPrime, out int currentPrime);
        }

        private void TestPrimeGenerator(int bits, long max, ProcessorCount threadCount, out int count, out int previousPrime, out int currentPrime)
        {

            var gen = new PrimeGenerator((int)max);
            count = 0;
            var iter = gen.GetEnumerator();
            while (iter.MoveNext())
            {
                count++;
                if (gen.Current >= max)
                {
                    break;
                }
            }
            previousPrime = gen.Previous;
            currentPrime = gen.Current;


        }

        private void TestPrimeGenerator(int powerOfTwo, int threadCount = 4)
        {
            ProcessorCount processorCount = (ProcessorCount)threadCount;
            var expected = PrimeData.Counts[powerOfTwo];
            TestPrimeGenerator(powerOfTwo, expected.MaxPrime, processorCount, out int count, out int previousPrime, out int currentPrime);
        }


        private void TestPrimeGeneratorRef(int bits, long max, ProcessorCount threadCount, out int count, out int previousPrime, out int currentPrime)
        {

            var gen = new PrimeGeneratorRef((int)max);
            count = 0;
            var iter = gen.GetEnumerator();
            while (iter.MoveNext())
            {
                count++;
                if (gen.Current >= max)
                {
                    break;
                }
            }
            previousPrime = gen.Previous;
            currentPrime = gen.Current;


        }

        private void TestPrimeGeneratorRef(int powerOfTwo, int threadCount = 4)
        {
            ProcessorCount processorCount = (ProcessorCount)threadCount;
            var expected = PrimeData.Counts[powerOfTwo];
            TestPrimeGeneratorRef(powerOfTwo, expected.MaxPrime, processorCount, out int count, out int previousPrime, out int currentPrime);
        }

        private void TestPrimeGeneratorUnsafe(int bits, long max, ProcessorCount threadCount, out int count, out int previousPrime, out int currentPrime)
        {

            var gen = new PrimeGeneratorUnsafe((int)max);
            count = 0;
            var iter = gen.GetEnumerator();
            while (iter.MoveNext())
            {
                count++;
                if (gen.Current >= max)
                {
                    break;
                }
            }
            previousPrime = gen.Previous;
            currentPrime = gen.Current;


        }

        private void TestPrimeGeneratorUnsafe(int powerOfTwo, int threadCount = 4)
        {
            ProcessorCount processorCount = (ProcessorCount)threadCount;
            var expected = PrimeData.Counts[powerOfTwo];
            TestPrimeGeneratorUnsafe(powerOfTwo, expected.MaxPrime, processorCount, out int count, out int previousPrime, out int currentPrime);
        }

        private void TestPrimeGeneratorUnsafeRange(int start, int end, out int count, out int previousPrime, out int currentPrime)
        {

            var gen = new PrimeGeneratorUnsafe(start, end);
            count = 0;
            var iter = gen.GetEnumerator();
            while (iter.MoveNext())
            {
                count++;
            }
            previousPrime = gen.Previous;
            currentPrime = gen.Current;


        }

        private void TestPrimeGeneratorUnsafeRange(int powerOfTwo)
        {

            var lowData = PrimeData.Counts[powerOfTwo];
            var highData = PrimeData.Counts[powerOfTwo + 1];

            var start = 1 << (powerOfTwo -1);
            var end = (int)((1u << (powerOfTwo)) - 1);
            var expectedCount = highData.Count - lowData.Count;

            TestPrimeGeneratorUnsafeRange(start, end, out int count, out int previousPrime, out int currentPrime);
        }
    }
}
