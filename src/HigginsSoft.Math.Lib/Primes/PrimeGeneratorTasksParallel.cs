/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Collections;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public interface ISieveOfHiggins
    {
        int GetBitIndexForCurrentWindow(int low);
    }

    public enum ProcessorCount
    {
        None = 0,
        P_0 = 1 << 0,
        P_1 = 1 << 1,
        P_2 = 1 << 2,
        P_3 = 1 << 3,
        P_4 = 1 << 4,
        P_5 = 1 << 5,
        P_6 = 1 << 6,
        P_7 = 1 << 7,
        P_8 = 1 << 8,
        P_9 = 1 << 9,
        P_10 = 1 << 10,
        P_11 = 1 << 11,
        P_12 = 1 << 12,
        P_13 = 1 << 13,
        P_14 = 1 << 14,
        P_15 = 1 << 15,
        P_16 = 1 << 16,
        P_17 = 1 << 17,
        P_18 = 1 << 18,
        P_19 = 1 << 19,
        P_20 = 1 << 20,
        P_21 = 1 << 21,
        P_22 = 1 << 22,
        P_23 = 1 << 23,
        P_24 = 1 << 24,
        P_25 = 1 << 25,
        P_26 = 1 << 26,
        P_27 = 1 << 27,
        P_28 = 1 << 28,
        P_29 = 1 << 29,
        P_30 = 1 << 30,
        //P_31 = 1 << 31,
    }

    public class PrimeGeneratorTasksParallel : IEnumerable<int>, ISieveOfHiggins
    {
        /// todo: calculate initial sieve with small primes already crossed of for each window.
        static readonly int[] arr = Enumerable.Repeat(-1, 342 * 2).ToArray();
        private int[] sieve = null!;
        private int[][] threadSieveData = null!;

        internal int BitLength;
        private int _current = -1;
        private int _previous = -1;
        int bitIndex = -1;
        int value;
        int threadCount;
        public int Current => _current;
        public int Previous => _previous;

        private int window;
        private int windowBits;
        private int maxPrime;
        private int maxSievePrime;
        public int MaxPrime => maxPrime;

        private int Window { get => window; set => windowBits = (window = value) * BitLength; }
        public int BitIndex => bitIndex;


        private List<Mod6SievePrime>[] sievePrimes = null!;

        public Action incNext = null!;

        readonly int threadMask;
        readonly int threadModMask;
        public PrimeGeneratorTasksParallel(int maxPrime = 2147483647, ProcessorCount numThreads = ProcessorCount.P_0)
        {

            this.maxPrime = maxPrime;
            this.threadCount = (int)numThreads;
            maxSievePrime = (int)System.Math.Ceiling(System.Math.Sqrt(maxPrime));
            value = 0;
            bitIndex = -3;
            sieve = new int[arr.Length];
            threadSieveData = new int[threadCount][];
            sievePrimes = new List<Mod6SievePrime>[threadCount];

            int mask = 1;
            while (mask > 0 && mask < this.threadCount)
            {
                mask <<= 1;
            }
            this.threadMask = mask;
            threadModMask = mask - 1;

            ///

            Parallel.For(0, threadCount, i =>
            {
                threadSieveData[i] = new int[arr.Length];
                sievePrimes[i] = new();
            });

            clearSieve();

            BitLength = arr.Length << 5;

            Action<Action, Action> _incNoSieve = (a, b) =>
            {
                bitIndex++;
                _previous = _current;
                a();
                _current = value;
                incNext = b;
            };

            sieveAction = readAndSieve;
            Action<Action, Action> _inc = (incrementValue, nextIncrement) =>
            {
                bitIndex++;
                if (bitIndex >= BitLength)
                {
                    incSievePrimes();
                }
                incrementValue();
                incNext = nextIncrement;
                sieveAction();
            };

            //increment the sieve +4, and set the increment function to +2
            Action incP4 = null!;
            //increment the sieve +2, and set the increment function to +4
            Action incP2 = () => _inc(() => value += 2, incP4);
            incP4 = () => _inc(() => value += 4, incP2);

            //increment the sieve to 7 and set the increment function to +4
            Action inc7 = () => _inc(() => value += 2, incP4);
            //increment the sieve to 5 and set the increment function to incrememt to 7
            Action inc5 = () => _inc(() => value += 2, inc7);
            //increment the sieve to 3 and set the increment function to incrememt to 5
            Action inc3 = () => _incNoSieve(() => value += 1, inc5);
            //increment the sieve to 2 and set the increment function to incrememt to 3
            Action inc2 = () => _incNoSieve(() => value += 2, inc3);

            incNext = inc2;
        }

        Action sieveAction = null!;

        private void clearSieve()
        {
            Buffer.BlockCopy(arr, 0, sieve, 0, arr.Length << 2);
            Action<int> clearSieveData = (i) => Buffer.BlockCopy(arr, 0, threadSieveData[i], 0, arr.Length << 2);
            Parallel.For(0, threadCount, clearSieveData);
        }


        private void mergeSieveData()
        {
            int mask = threadMask;
            ///
            while (mask > 0)
            {
                // perform binary tree merge

                /* for example:
                 
                    4 threads:
                    first loop iteration
                    threadSieveData[0][] = threadSieveData[0][] & threadSieveData[2][]
                    threadSieveData[1][] = threadSieveData[1][] & threadSieveData[3][]
                
                    // second loop iteration
                    threadSieveData[0][] = threadSieveData[0][] & threadSieveData[1][]

                    8 threads
                    first loop iteration
                    threadSieveData[0][] = threadSieveData[0][] & threadSieveData[4][]
                    threadSieveData[1][] = threadSieveData[1][] & threadSieveData[5][]
                    threadSieveData[2][] = threadSieveData[2][] & threadSieveData[6][]
                    threadSieveData[3][] = threadSieveData[3][] & threadSieveData[7][]


                    // second loop iteration
                    threadSieveData[0][] = threadSieveData[0][] & threadSieveData[2][]
                    threadSieveData[1][] = threadSieveData[1][] & threadSieveData[3][]

                    // second loop iteration
                    threadSieveData[0][] = threadSieveData[0][] & threadSieveData[1][]
                */
                Parallel.For(0, mask, (i) =>
                {

                    int leftIndex = i;
                    int rightIndex = i + mask;


                    if (rightIndex < threadCount)
                    {
                        var left = threadSieveData[leftIndex];
                        var right = threadSieveData[rightIndex];
                        for (int j = 0; j < left.Length; j++)
                        {
                            ref int segment = ref left[j];
                            segment &= right[j];
                        }
                    }
                });
                mask >>= 1;
            }

            //The binary merge will leave the final result in threadSieveData[0]
            // Copy to our sieve array
            Buffer.BlockCopy(threadSieveData[0], 0, sieve, 0, arr.Length * sizeof(int));
        }

        private void readSieve()
        {
            var bitValue = this[BitIndex];
            if (!bitValue)
            {
                incNext();
            }
            else
            {
                _previous = _current;
                _current = value;
            }
        }

        int sievePrimeCount = 0;
        private void readAndSieve()
        {
            var bitValue = this[BitIndex];
            if (!bitValue)
            {
                incNext();
            }
            else
            {
                _previous = _current;
                _current = value;

                if (value <= maxSievePrime)
                {
                    var x = new Mod6SievePrime(_current, bitIndex, this);
                    //x.SieveTo(BitLength);
                    sievePrimes[(sievePrimeCount++) & threadModMask].Add(x);

                    //sieveUnsafe(x.LowBitIndex, x.HighBitIndex, x.Step, BitLength, out low, out high);
                    int low = 0;
                    int high = 0;
                    int lowBitIndex = x.LowBitIndex;
                    int highBitIndex = x.HighBitIndex;
                    int bitLength = BitLength;
                    int step = x.Step;

                    int bitMask = 0;
                    unsafe
                    {
                        fixed (int* pSieve = sieve)
                        {
                            for (low = lowBitIndex; low < bitLength; low += step)
                            {
                                int* segment = pSieve + (low >> 5); // equivalent to index / 32
                                                                    //*segment &= notMasks[indexLow & 31];
                                bitMask = 1 << low;
                                *segment &= ~bitMask;
                            }


                            for (high = highBitIndex; high < bitLength; high += step)
                            {
                                int* segment = pSieve + (high >> 5); // equivalent to index / 32
                                                                     //*segment &= notMasks[indexHigh & 31];
                                bitMask = 1 << high;
                                *segment &= ~bitMask;
                            }

                        }
                    }


                    x.lowBitIndex = low;
                    x.highBitIndex = high;
                }
                else
                {
                    sieveAction = readSieve;
                }
            }
        }


        //unsafe sieve is 15-20% faster  (2^31 16 secs vs 19 secs  in release and 3 secs faster in debug (56 vs 59)
        private void incSievePrimes()
        {
            clearSieve();
            bitIndex = 0;
            Window += 1;

            Parallel.For(0, threadCount, incSievePrimes);
            mergeSieveData();
        }

        private void incSievePrimesSafe(int threadIndex)
        {

            int bitLength = BitLength;
            int low = 0;
            int high = 0;
            int bitMask = 0;

            int lowBitIndex;
            int highBitIndex;
            int step;
            var sievePrimes = this.sievePrimes[threadIndex];
            var data = threadSieveData[threadIndex];

            for (var i = 0; i < sievePrimes.Count; i++)
            {
                var x = sievePrimes[i];
                lowBitIndex = x.LowBitIndex - BitLength;
                highBitIndex = x.highBitIndex - BitLength;
                step = x.Step;
                for (low = lowBitIndex; low < bitLength; low += step)
                {
                    ref int segment = ref data[low >> 5];
                    bitMask = 1 << low;
                    segment &= ~bitMask;
                }
                for (high = highBitIndex; high < bitLength; high += step)
                {
                    ref int segment = ref data[high >> 5];
                    bitMask = 1 << high;
                    segment &= ~bitMask;
                }

                x.lowBitIndex = low;
                x.highBitIndex = high;

            }
        }

        private void incSievePrimes(int threadIndex)
        {

            int bitLength = BitLength;
            int low = 0;
            int high = 0;
            int bitMask = 0;

            int lowBitIndex;
            int highBitIndex;
            int step;
            var sievePrimes = this.sievePrimes[threadIndex];
            for (var i = 0; i < sievePrimes.Count; i++)
            {
                var x = sievePrimes[i];
                lowBitIndex = x.LowBitIndex - BitLength;
                highBitIndex = x.highBitIndex - BitLength;
                step = x.Step;
                //sieveUnsafe(x.LowBitIndex - BitLength, x.HighBitIndex - BitLength, x.Step, bitLength, out low, out high);
                unsafe
                {
                    fixed (int* pSieve = threadSieveData[threadIndex])
                    {

                        for (low = lowBitIndex; low < bitLength; low += step)
                        {
                           
                            int * segment = pSieve + (low >> 5); // equivalent to index / 32
                                                                //*segment &= notMasks[indexLow & 31];
                            bitMask = 1 << low;
                            *segment &= ~bitMask;
                        }


                        for (high = highBitIndex; high < bitLength; high += step)
                        {
                            int* segment = pSieve + (high >> 5); // equivalent to index / 32
                                                                 //*segment &= notMasks[indexHigh & 31];
                            bitMask = 1 << high;
                            *segment &= ~bitMask;
                        }

                    }
                }
                x.lowBitIndex = low;
                x.highBitIndex = high;
            }
          
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void sieveUnsafe(int lowBitIndex, int highBitIndex, int step, int bitLength, out int low, out int high)
        {
            int bitMask = 0;
            unsafe
            {
                fixed (int* pSieve = sieve)
                {
                    for (low = lowBitIndex; low < bitLength; low += step)
                    {
                        int* segment = pSieve + (low >> 5); // equivalent to index / 32
                                                            //*segment &= notMasks[indexLow & 31];
                        bitMask = 1 << low;
                        *segment &= ~bitMask;
                    }


                    for (high = highBitIndex; high < bitLength; high += step)
                    {
                        int* segment = pSieve + (high >> 5); // equivalent to index / 32
                                                             //*segment &= notMasks[indexHigh & 31];
                        bitMask = 1 << high;
                        *segment &= ~bitMask;
                    }

                }
            }

        }


        internal bool this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Get(int index)
        {
            return (sieve[index >> 5] & (1 << index)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ClearBit(int index)
        {
            int bitMask = 1 << index;
            ref int segment = ref sieve[index >> 5];
            segment &= ~bitMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Set(int index, bool value)
        {
            int bitMask = 1 << index;
            ref int segment = ref sieve[index >> 5];
            //var val = GetBitWindowValue(index);
            if (value)
            {
                segment |= bitMask;
            }
            else
            {
                segment &= ~bitMask;
            }
        }


        public IEnumerator<int> GetEnumerator()
        {
            while (value < maxPrime)
            {
                incNext();
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Return the bit array index of the value n,
        public int GetBitIndex(int value)
        {
            int index = (value / 6) << 1;
            var res = (value % 6);
            if (res == 1)
                index -= 1;
            return index;
        }

        public int GetBitIndexForCurrentWindow(int value)
        {
            int index = GetBitIndex(value);
            index = index - windowBits;
            return index;
        }

        //Return the value represented by bitindex
        public int GetBitWindowValue(int index)
        {
            index += windowBits;
            int value = ((index >> 1) + 1) * 6;
            int sign = ((index & 1) == 0) ? -1 : 1;
            return value + sign;
        }

        //Return the value represented by bitindex

        public int GetBitValue(int index)
        {
            int value = ((index >> 1) + 1) * 6;
            int sign = ((index & 1) == 0) ? -1 : 1;
            return value + sign;
        }

        private class Mod6SievePrime
        {
            internal int lowBitIndex;
            internal int highBitIndex;
            private int step;

            public int LowBitIndex { get => lowBitIndex; set => lowBitIndex = value; }
            public int HighBitIndex { get => highBitIndex; set => highBitIndex = value; }
            public int Step => step;

            public Mod6SievePrime(int current, int bitIndex, ISieveOfHiggins sieve)
            {
                Value = current;
                step = current << 1;

                //TODO: Calculate direct indexes, to remove need for highlever arithmetic.
                var intersect = current * 6;
                var low = intersect - current;
                var high = intersect + current;
                var sq = current * current;


                low = calculate(low, intersect, sq);
                high = calculate(high, intersect, sq);


                lowBitIndex = sieve.GetBitIndexForCurrentWindow(low);
                highBitIndex = sieve.GetBitIndexForCurrentWindow(high);
            }

            public int calculate(int value, int intersect, int sq)
            {
                int delta = intersect;

                // calculate the number of steps to reach the first value >= sq
                int steps = (int)System.Math.Ceiling((double)(sq - value) / delta);

                // calculate the next value
                int nextValue = value + (steps * delta);

                return nextValue;
            }

            public int Value { get; }

        }
    }
}