/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{

    public class PrimeGeneratorUnsafeLong : IPrimeGenerator
    {
        /// todo: calculate initial sieve with small primes already crossed of for each window.
        //static readonly int[] arr = Enumerable.Repeat(-1, 342 * 2).ToArray();
        //static readonly int[] arr = Enumerable.Repeat(-1, 1000 * 2).ToArray();
        //static readonly int[] arr = Enumerable.Repeat(-1, 5 * 2 * 7 * 2 * 11 * 2).ToArray();
        static readonly int[] arr = PrimeGeneratorPreSieve.Wheel11.Concat(PrimeGeneratorPreSieve.Wheel11).Select(x => (int)x).ToArray();

        private int[] sieve = null!;
        private int[] sievePrimes = new int[6542];
        private int[] lowindices = new int[6542];
        private int[] highindices = new int[6542];

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




        Action incNext = null!;

        public PrimeGeneratorUnsafeLong() : this(PrimeData.MaxIntPrime) { }
        public PrimeGeneratorUnsafeLong(int maxPrime = 2147483647)
        {
            this.maxPrime = maxPrime;
            maxSievePrime = (int)System.Math.Ceiling(System.Math.Sqrt(maxPrime));
            value = 0;
            bitIndex = -3;
            sieve = new int[arr.Length];
            clearSieve();
            sieve = Enumerable.Repeat(-1, arr.Length).ToArray();


            BitLength = arr.Length << 5;


            sieveAction = readAndSieve;


            Action valueIncrementer4 = null!;
            Action valueIncrementer2 = () => { value += 2; valueIncrementer = valueIncrementer4; };
            valueIncrementer4 = () => { value += 4; valueIncrementer = valueIncrementer2; };

            Action valueIncremen5to7 = () => { value += 2; valueIncrementer = valueIncrementer4; };
            Action valueIncrement3To5 = () => { value += 2; valueIncrementer = valueIncremen5to7; };
            Action valueIncrement2To3 = () => { value += 2; valueIncrementer = valueIncrement3To5; };
            Action valueIncrementTo3 = () => { value += 2; valueIncrementer = valueIncrement2To3; };

            valueIncrementer = valueIncrement3To5;

            Action<Action, Action> _incNoSieve = (a, b) =>
            {
                bitIndex++;
                _previous = _current;
                a();
                _current = value;
                incNext = b;
            };


            Action<Action> _inc = (nextIncrement) =>
            {
                //bitIndex++;
                //if (bitIndex >= BitLength)
                //{
                //    sieveNextWindow();
                //}
                //valueIncrementer();
                //incNext = nextIncrement;
                sieveAction();
            };

            //increment the sieve +4, and set the increment function to +2
            Action incP4 = null!;
            //increment the sieve +2, and set the increment function to +4
            Action incP2 = () => _inc(incP4);
            incP4 = () => _inc(incP2);


            Action incTo11 = () =>
            {
                ref int head = ref arr[0];
                head &= ~7;
                _incNoSieve(() => value += 4, sieveAction);
            };
            //increment the sieve to 7 and set the increment function to +4
            Action incTo7 = () => _incNoSieve(() => value += 2, incTo11);
            //increment the sieve to 5 and set the increment function to incrememt to 7
            Action incTo5 = () => _incNoSieve(() => value += 2, incTo7);
            //increment the sieve to 3 and set the increment function to incrememt to 5
            Action incTo3 = () => _incNoSieve(() => value += 1, sieveAction);
            //increment the sieve to 2 and set the increment function to incrememt to 3
            Action incTo2 = () => _incNoSieve(() => value += 2, incTo3);

            incNext = incTo2;
        }

        Action valueIncrementer = null!;
        Action sieveAction = null!;
        void findPeriod()
        {
            int i = 1;

            var seive = sieve;
            var max = sieve.Length;
            var allValues = sieve.Select(x => $"0x{x.ToString("X2")}").ToArray();
            var allValuesDec = $"var period = new[] {{\n{string.Join("\n,", allValues)} }};";
            for (; (i << 1) + i < max; i++)
            {
                var offsetLow = i;
                var offsetHigh = i << 1;
                var low = sieve.Skip(i).Take(i).ToArray();
                var high = sieve.Skip(i << 1).Take(i).ToArray();

                bool eq = true;
                for (var j = 0; j < low.Length && eq; j++)
                {
                    var a = low[j];
                    var b = high[j];
                    eq = a == b;
                }
                if (eq)
                {
                    break;
                }
            }
            if (i > max) i = -1;
            Console.WriteLine($"Period of {Current} = {i}");
            if (i > -1)
            {
                var values = sieve.Take(i).Select(x => $"0x{x.ToString("X2")}").ToArray();
                values = values.Concat(values).ToArray();
                var dec = $"//{_current} - length:{values.Length}\nvar period = new[] {{ {string.Join(",", values)} }};";
                Console.WriteLine(dec);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void clearSieve()
        {
            Buffer.BlockCopy(arr, 0, sieve, 0, arr.Length << 2);
        }

        private int primecount = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void readNextPrime()
        {
            ref int index = ref bitIndex;
            for (index++; (index < BitLength || sieveNextWindow()) && (sieve[index >> 5] & (1 << index)) == 0; index++) ;

            value = GetBitWindowValue(index);
            //value = 5 + ((index >> 1) * 6) + (2 * (index & 1)) + windowBits;

            _previous = _current;
            _current = value;

        }

        private void readSieve()
        {
            readNextPrime();

        }

        private bool sieveNextWindow()
        {
            clearSieve();
            bitIndex = 0;
            Window += 1;
            int prime;
            int bitLength = BitLength;
            int bitMask;
            for (var i = 0;
                //i < lowindices.Length &&
                //i < highindices.Length &&
                //i < sievePrimes.Length &&
                i <= primecount;

                i++)
            {
                prime = sievePrimes[i];
                ref int low = ref lowindices[i];
                ref int high = ref highindices[i];
                low -= bitLength;
                high -= bitLength;
                if (low < bitLength + windowBits)
                {
                    int step = prime << 1;
                    for (; low < bitLength; low += step)
                    {
                        ref int segment = ref sieve[low >> 5];
                        bitMask = 1 << low;
                        segment &= ~bitMask;
                    }
                    for (; high < bitLength; high += step)
                    {
                        ref int segment = ref sieve[high >> 5];
                        bitMask = 1 << high;
                        segment &= ~bitMask;
                    }
                }
            }
            return true;
        }


        private void readAndSieve()
        {
            readNextPrime();


            if (value <= maxSievePrime)
            {
                int prime = _current;
                var x = new SievePrime(_current, bitIndex, this);
                if (value < 13)
                {
                    x.SieveTo(BitLength);
                    return;
                }
                primecount++;
                this.sievePrimes[primecount] = prime;
                ref int low = ref lowindices[primecount];
                ref int high = ref highindices[primecount];

                low = x.lowBitIndex;
                high = x.highBitIndex;
                int bitLength = BitLength;
                if (low < windowBits + BitLength)
                {

                    int bitMask;


                    int step = prime << 1;
                    for (; low < bitLength; low += step)
                    {
                        ref int segment = ref sieve[low >> 5];
                        bitMask = 1 << low;
                        segment &= ~bitMask;
                    }
                    for (; high < bitLength; high += step)
                    {
                        ref int segment = ref sieve[high >> 5];
                        bitMask = 1 << high;
                        segment &= ~bitMask;
                    }
                }

            }
            else
            {
                incNext = readSieve;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //Return the value represented by bitindex
        public int GetBitWindowValue(int index)
            => GetBitValue(index + windowBits);


        //Return the value represented by bitindex
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetBitValue(int index)
        {
            index = 5 + ((index >> 1) * 6) + (2 * (index & 1));
            return index;
        }

        private class SievePrime
        {
            internal int lowBitIndex;
            internal int highBitIndex;
            private int step;

            public int LowBitIndex { get => lowBitIndex; set => lowBitIndex = value; }
            public int HighBitIndex { get => highBitIndex; set => highBitIndex = value; }
            public int Step => step;
            private PrimeGeneratorUnsafeLong sieve;
            public SievePrime(int current, int bitIndex, PrimeGeneratorUnsafeLong sieve)
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

                //while (high < sq)
                //{
                //    high += intersect;
                //}
                high = calculate(high, intersect, sq);
                if (low > high)
                {
                    sq = low;
                    low = high;
                    high = sq;
                }

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