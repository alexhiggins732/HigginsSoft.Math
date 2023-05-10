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

    public class PrimeGeneratorRef : IPrimeGenerator
    {
        /// todo: calculate initial sieve with small primes already crossed of for each window.
        static readonly int[] arr = Enumerable.Repeat(-1, 342 * 2).ToArray();
        private int[] sieve = null!;

        internal int BitLength;
        private int _current = -1;
        private int _previous = -1;
        int bitIndex = -1;
        int value;

        public int Current => _current;
        public int Previous => _previous;

        private int window;
        private int windowBits;
        private int maxPrime;
        private int maxSievePrime;
        public int MaxPrime => maxPrime;

        private int Window { get => window; set => windowBits = (window = value) * BitLength; }
        public int BitIndex => bitIndex;


        private List<SievePrime> sievePrimes = new();

        public Action incNext = null!;

        public PrimeGeneratorRef() : this(PrimeData.MaxIntPrime) { }
        public PrimeGeneratorRef(int maxPrime)
        {
            this.maxPrime = maxPrime;
            maxSievePrime = (int)System.Math.Ceiling(System.Math.Sqrt(maxPrime));
            value = 0;
            bitIndex = -3;
            sieve = new int[arr.Length];
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

        static readonly int[] notMasks = {
            ~(1<<0),
            ~(1<<1),
            ~(1<<2),
            ~(1<<3),
            ~(1<<4),
            ~(1<<5),
            ~(1<<6),
            ~(1<<7),
            ~(1<<8),
            ~(1<<9),
            ~(1<<10),
            ~(1<<11),
            ~(1<<12),
            ~(1<<13),
            ~(1<<14),
            ~(1<<15),
            ~(1<<16),
            ~(1<<17),
            ~(1<<18),
            ~(1<<19),
            ~(1<<20),
            ~(1<<21),
            ~(1<<22),
            ~(1<<23),
            ~(1<<24),
            ~(1<<25),
            ~(1<<26),
            ~(1<<27),
            ~(1<<28),
            ~(1<<29),
            ~(1<<30),
            ~(1<<31),
            };

        //unsafe sieve is 15-20% faster  (2^31 16 secs vs 19 secs  in release and 3 secs faster in debug (56 vs 59)
        private void incSievePrimes()
        {
            clearSieve();
            bitIndex = 0;
            Window += 1;

            int bitLength = BitLength;

            int bitMask = 0;


            int step;
            for (var i = 0; i < sievePrimes.Count; i++)
            {
                var x = sievePrimes[i];
                int low = x.lowBitIndex;
                int high = x.highBitIndex;
                low -= bitLength;
                high -= bitLength;


                step = x.Step;
                //sieveUnsafe(x.LowBitIndex - BitLength, x.HighBitIndex - BitLength, x.Step, bitLength, out low, out high);

                //ref int low = ref x.lowBitIndex;
                //ref int high = ref x.highBitIndex;




                for (; low < bitLength; low += step)
                {
                    ref int segment = ref sieve[(low >> 5)];
                    bitMask = 1 << low;
                    segment &= ~bitMask;
                }
                for (; high < bitLength; high += step)
                {
                    ref int segment = ref sieve[(high >> 5)];
                    bitMask = 1 << high;
                    segment &= ~bitMask;
                }

                x.lowBitIndex = low;
                x.highBitIndex = high;

                //unsafe
                //{
                //    fixed (int* pSieve = sieve)
                //    {
                //        for (low = lowBitIndex; low < bitLength; low += step)
                //        {
                //            int* segment = pSieve + (low >> 5); // equivalent to index / 32
                //                                                //*segment &= notMasks[indexLow & 31];
                //            bitMask = 1 << low;
                //            *segment &= ~bitMask;
                //        }


                //        for (high = highBitIndex; high < bitLength; high += step)
                //        {
                //            int* segment = pSieve + (high >> 5); // equivalent to index / 32
                //                                                 //*segment &= notMasks[indexHigh & 31];
                //            bitMask = 1 << high;
                //            *segment &= ~bitMask;
                //        }

                //    }
                //}


                //x.lowBitIndex = low;
                //x.highBitIndex = high;
            }
        }


        private void clearSieve()
        {
            Buffer.BlockCopy(arr, 0, sieve, 0, arr.Length << 2);
        }


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
                    var x = new SievePrime(_current, bitIndex, this);
                    sievePrimes.Add(x);

                    //takes 2 seconds longer for 2^31
                    //ref int low = ref x.lowBitIndex;
                    //ref int high = ref x.highBitIndex;
                    int low = x.lowBitIndex;
                    int bitLength = BitLength;
                    if (low < windowBits + bitLength)
                    {
                        int high = x.highBitIndex;
                        int step = x.Step;
                        int bitMask = 0;
                        for (; low < bitLength; low += step)
                        {
                            ref int segment = ref sieve[(low >> 5)];
                            bitMask = 1 << low;
                            segment &= ~bitMask;
                        }
                        for (; high < bitLength; high += step)
                        {
                            ref int segment = ref sieve[(high >> 5)];
                            bitMask = 1 << high;
                            segment &= ~bitMask;
                        }
                        x.lowBitIndex = low;
                        x.highBitIndex = high;

                        /*
                         *  int index = x.lowBitIndex;
                        for (; index < bitLength; index += step)
                        {
                            ref int segment = ref sieve[(index >> 5)];
                            bitMask = 1 << index;
                            segment &= ~bitMask;
                        }
                        x.lowBitIndex = index;
                        index = x.highBitIndex;
                        for (; index < bitLength; index += step)
                        {
                            ref int segment = ref sieve[(index >> 5)];
                            bitMask = 1 << index;
                            segment &= ~bitMask;
                        }
                        x.highBitIndex = index;*/
                    }
                }
                else
                {
                    sieveAction = readSieve;
                }
            }
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

        private class SievePrime
        {
            internal int lowBitIndex;
            internal int highBitIndex;
            private int step;

            public int LowBitIndex { get => lowBitIndex; set => lowBitIndex = value; }
            public int HighBitIndex { get => highBitIndex; set => highBitIndex = value; }
            public int Step => step;
            private PrimeGeneratorRef sieve;
            public SievePrime(int current, int bitIndex, PrimeGeneratorRef sieve)
            {
                Value = current;
                this.sieve = sieve;
                step = current << 1;

                var intersect = current * 6;
                var low = intersect - current;
                var high = intersect + current;
                var sq = current * current;


                //while (low < sq)
                //{
                //    low += intersect;
                //}
                low = calculate(low, intersect, sq);


                high = calculate(high, intersect, sq);


                lowBitIndex = sieve.GetBitIndexForCurrentWindow(low);
                highBitIndex = sieve.GetBitIndexForCurrentWindow(high);

                //var lowTest = calculate(low, intersect, sq);
                //if (lowTest != low)
                //{
                //    string bp = "";
                //}

                //var highTest = calculate(high, intersect, sq);
                //if (highTest != high)
                //{
                //    string bp = "";
                //}




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
            internal void SieveTo(int maxBit)
            {
                //var initialValue = sieve.GetBitWindowValue(lowBitIndex);
                //if (initialValue % Value != 0)
                //{
                //    string bp = "";
                //}

                //TODO: optimization step 2*Value;
                for (; lowBitIndex < maxBit; lowBitIndex += step)
                {
                    //var val = sieve.GetBitWindowValue(lowBitIndex);
                    sieve.ClearBit(lowBitIndex);
                }

                //initialValue = sieve.GetBitWindowValue(highBitIndex);
                //if (initialValue % Value != 0)
                //{
                //    string bp = "";
                //}

                for (; highBitIndex < maxBit; highBitIndex += step)
                {
                    //var val = sieve.GetBitWindowValue(highBitIndex);
                    sieve.ClearBit(highBitIndex);
                }
            }
        }
    }
}