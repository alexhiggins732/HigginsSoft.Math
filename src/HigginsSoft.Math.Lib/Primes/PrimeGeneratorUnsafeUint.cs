/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

#define LOG_OFFSETS
#undef LOG_OFFSETS
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public class PrimeGeneratorUnsafeUint : IPrimeGenerator<uint>
    {
        /// todo: calculate initial sieve with small primes already crossed of for each window.
        //static readonly int[] arr = Enumerable.Repeat(-1, 342 * 2).ToArray();
        //static readonly int[] arr = Enumerable.Repeat(-1, 1000 * 2).ToArray();
        //static readonly int[] arr = Enumerable.Repeat(-1, 5 * 2 * 7 * 2 * 11 * 2).ToArray();

        static readonly int[] arr = PrimeGeneratorPreSieve.Wheel11.Concat(PrimeGeneratorPreSieve.Wheel11).ToArray();

        private int[] sieve = null!;
        private int[] sievePrimes = new int[6542];
        private int[] lowindices = new int[6542];
        private int[] highindices = new int[6542];

        internal int BitLength;
        private uint _current = 0;
        private uint _previous = 0;
        int bitIndex = -1;
        uint value;

        public uint Current => _current;
        public uint Previous => _previous;

        private int window;
        private int windowBits;
        private uint maxPrime;
        private uint startValue;
        private uint endValue;
        private int maxSievePrime;

        public uint MaxPrime => maxPrime;

        public int CurrentWindow => Window;
        private int Window { get => window; set => windowBits = (window = value) * BitLength; }
        public int BitIndex => bitIndex;

        private bool _sievedFactorBase;


        Action incNext = null!;

        public PrimeGeneratorUnsafeUint() : this(PrimeData.MaxUintPrime) { }


        bool logOffsets = bool.Parse(bool.TrueString);
        /// <summary>
        /// Construct a generator for primes in the range of <paramref name="startValue"/> to <paramref name="maxPrime"/>
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="maxPrime"></param>
        public PrimeGeneratorUnsafeUint(uint startValue, uint endValue)
            : this(endValue)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            if (endValue < startValue)
            {
                throw new ArgumentException($"End value ({endValue}) must be greater than start value ({startValue})");
            }

            //Make sure maxPrime is an actual prime to prevent over enumerating and arithemtic overlflows with long.maxvalue
            if (!MathUtil.IsProbablePrime(endValue))
            {
                maxPrime = MathUtil.GetPreviousPrime(endValue);
                maxSievePrime = (int)System.Math.Ceiling(System.Math.Sqrt(maxPrime));
            }
            // calculate window and bit index to sieve from
            var startPrime = startValue < 2 ? 2 : startValue;
            if (MathUtil.IsProbablePrime(startPrime))
            {
                startPrime = MathUtil.GetPreviousPrime(startPrime);
            }
            else
            {
                startPrime = MathUtil.GetNextPrime(startPrime);
            }
            this.startValue = startValue;
            if (startValue <= 2) return;
            if (startValue == 3) { incNext(); return; }
            if (startValue == 5) { incNext(); incNext(); return; }
            // sieve the factor base
            while (!_sievedFactorBase)
            {
                incNext();
            }





            var absoluteStartBitIndex = GetBitIndex(startPrime);
            var startWindow = absoluteStartBitIndex / BitLength;
            var windowBitIndex = absoluteStartBitIndex % BitLength;


            //move sieve to correct window.
            if (startWindow > 0)
            {
                //TODO:
                if (startWindow - 1 > window) skipToWindow(startWindow - 1);

                while (startWindow - 1 > window)
                {
                    skipToWindow(window + 1);
                    //skipToNextWindow();
                }


                sieveNextWindow();
            }

            // move to start prime bit index
            //TODO: handle case where windowBitIndex=0
            if (windowBitIndex > 1)
            {
                bitIndex = windowBitIndex - 1;
                return;
            }



        }
        public PrimeGeneratorUnsafeUint(uint maxPrime = PrimeData.MaxUintPrime)
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
            int bitLength = this.BitLength;
            var sieve = this.sieve;
            //for (index++; (index < bitLength || sieveNextWindow()) && (sieve[index >> 5] & (1 << index)) == 0; index++) ;


            index++;
            var notPrime = true;
            int word;
            int i;
            int shift;
            int mask = index & 31;


            while (notPrime && (index < bitLength || sieveNextWindow()))
            {
                //if (index == bitLength) sieveNextWindow();
                word = sieve[index >> 5];
                i = mask;
                shift = 1 << i;
                for (; i < 32 && (notPrime = (mask = (word & shift)) == 0); i++, index++, shift <<= 1)
                {

                }
                mask = 0;
            }

            value = GetBitWindowValue(index);
            //value = 5 + ((index >> 1) * 6) + (2 * (index & 1)) + windowBits;

            _previous = _current;
            _current = value;


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void readSieve()
        {
            readNextPrime();
        }

        private bool skipToWindow(int n)
        {
            int bitLength = BitLength;
            var windowOffset = n - window;
            int windowSize = bitLength * windowOffset;
            //int overflowWindowSize = windowSize + bitLength;
#if LOG_OFFSETS
            var windowStart = windowBits;
            var windowEnd = windowBits + bitLength;


            var values = Enumerable.Range(0, primecount + 1)

                .Select(i => new
                {
                    Prime = sievePrimes[i],
                    Step = sievePrimes[i] << 2,
                    Low = GetBitWindowValue(lowindices[i]),
                    LowBit = lowindices[i],
                    High = GetBitWindowValue(highindices[i]),
                    HighBit = highindices[i],

                }).ToArray();
#endif
            if (windowOffset < 1)
            {
                return false;
            }

            Window += windowOffset;
#if LOG_OFFSETS
            var nextWindowStart = windowBits;
            var nextWindowEnd = windowBits + bitLength;



            //Window++;
            //var bitLength = BitLength;

            if (logOffsets)
            {
                var logPath = Path.GetFullPath($"offset-{maxPrime}.csv");
                Console.WriteLine($"Logging offsets to {logPath}");
                if (File.Exists($"offset-{maxPrime}.csv")) File.Delete($"offset-{maxPrime}.csv");
                var windowStartValue = windowStart * 3;// (windowStart >> 1) * 6;
                var windowEndValue = (windowStart + bitLength) * 3; ////((windowStart +bitLength) >> 1 ) * 6;

                var nextWindowStartValue = windowBits * 3;
                var nextWindowEndValue = (windowBits + bitLength) * 3;

                var windowSizeValue = (windowSize) * 3;
                File.AppendAllText($"offset-{maxPrime}.csv", $"windowStart,windowEnd,nextWindowStart,nextWindowEnd,windowStartBits,windowEndBits,nextWindowStartBits,nextWindowEndBits,bitLength,windowSizeValue,windowSize,numSteps\n");
                File.AppendAllText($"offset-{maxPrime}.csv", $"{windowStartValue},{windowEndValue},{nextWindowStartValue},{nextWindowEndValue},{windowStart},{windowEnd},{nextWindowStart},{nextWindowEnd},{bitLength},{windowSizeValue},{windowSize},{offset}\n\n");

                File.AppendAllText($"offset-{maxPrime}.csv", $"prime,type,step,lowValue,lowBitRelative,lowBitAbsolute,nextLowValue,nextLowBitRelative,nextLowBitAbsolute\n");

            }
#endif
            int step;
            int prime;
            int offsetBits = (n + 1) * bitLength;
            int p6;
            //var windowBits = this.windowBits;
            for (var i = 0;
                i <= primecount;
                i++)
            {


#if LOG_OFFSETS
                ref int low0 = ref lowindices[i];
                ref int high0 = ref highindices[i];
                prime = sievePrimes[i];
                step = prime << 1;
                var low = low0;
                var high = high0;
                var lowValue = GetBitValue(low0);
                var highValue = GetBitValue(high0);


                int calcLow = low - windowSize;
                if (calcLow < bitLength)
                {
                    //to window 1, needs 2?

                    calcLow = this.calcAbsFromOffset(prime * 6 - prime, step, (n + 1) * bitLength);
                    calcLow = calcLow - windowBits;
                }


                int calcHigh = high - windowSize;
                if (calcHigh < bitLength)
                {
                    calcHigh = this.calcAbsFromOffset(prime * 6 + prime, step, (n + 1) * bitLength);
                    calcHigh = calcHigh - windowBits;
                }

                if (logOffsets)
                {
                    var calcLowCheck = this.CalcStepsWithLoop(low, step, bitLength, offset);
                    var calcHighCheck = this.CalcStepsWithLoop(high, step, bitLength, offset);
                    var calcLowCheckValue = calcLowCheck.Last().Value;
                    var calcHighCheckValue = calcHighCheck.Last().Value;
                    var primeData = values[i];
                    {

                        if (calcLowCheckValue != calcLow)
                        {
                            throw new Exception("invalid low calc");
                        }
                        var nextLowAbs = GetBitWindowValue(calcLow);
                        //prime,step,lowValue,lowBitRelative,lowBitAbsolute,nextLowValue,nextLowBitRelative,nextLowBitAbsolute
                        var nextLowBitRelative = calcLow;
                        var nextLowBitAbsolute = nextLowBitRelative + windowBits;
                        var nextLowValue = GetBitWindowValue(calcLow);
                        if (nextLowBitAbsolute == primeData.LowBit)
                        {
                            //string bp = "how?";
                        }
                        File.AppendAllText($"offset-{maxPrime}.csv", $"{step >> 1},{nameof(low)},{step},{primeData.Low},{primeData.LowBit},{primeData.LowBit + windowStart},{nextLowValue},{nextLowBitRelative},{nextLowBitAbsolute}\n");
                    }

                    {

                        if (calcHighCheckValue != calcHigh)
                        {
                            throw new Exception("invalid low high");
                        }
                        var nextHighAbs = GetBitWindowValue(calcHigh);
                        //prime,step,highValue,highBitRelative,highBitAbsolute,nextHighValue,nextHighBitRelative,nextHighBitAbsolute
                        var nextHighBitRelative = calcHigh;
                        var nextHighBitAbsolute = nextHighBitRelative + windowBits;
                        var nextHighValue = GetBitWindowValue(calcHigh);
                        File.AppendAllText($"offset-{maxPrime}.csv", $"{step >> 1},{nameof(high)},{step},{primeData.High},{primeData.HighBit},{primeData.HighBit + windowStart},{nextHighValue},{nextHighBitRelative},{nextHighBitAbsolute}\n");
                    }

                }
                bool runTests = bool.Parse(bool.FalseString);
                if (runTests)
                {
                    var calcLowWithLoop = CalcStepsWithLoop(low, step, bitLength, offset);
                    var calcHighWithLoop = CalcStepsWithLoop(high, step, bitLength, offset);

                    var finalLow = calcLowWithLoop.Last();
                    if (calcLow != finalLow.Value)
                    {
                        string bp = "Invalid low calculation";
                    }

                    var finalHigh = calcHighWithLoop.Last();
                    if (calcHigh != finalHigh.Value)
                    {
                        string bp = "Invalid high calculation";
                    }
                }

                low0 = calcLow;
                high0 = calcHigh;
#else
                ref int low = ref lowindices[i];
                ref int high = ref highindices[i];

                low -= windowSize;
                high -= windowSize;

                if (low < bitLength || high < bitLength)
                {
                    prime = sievePrimes[i];
                    step = prime << 1;
                    p6 = prime * 6;

                    if (low < bitLength)
                    {

                        // working with method call
                        //low = this.calcAbsFromOffset(prime * 6 - prime, step, (n + 1) * bitLength);
                        //low -= windowBits;

                        //low = GetBitIndex(prime * 6 - prime);
                        low = p6 - prime;
                        low /= 3;
                        low -= 1;
                        low += (((int)System.Math.Ceiling((offsetBits - (low % step)) / (double)step)) - (low / step)) * step
                            - windowBits;
                    }
                    if (high < bitLength)
                    {
                        // working with method call
                        //high = this.calcAbsFromOffset(prime * 6 + prime, step, offsetBits);
                        //high -= windowBits;

                        //high = GetBitIndex(prime * 6 + prime);
                        high = p6 + prime;
                        high /= 3;
                        high -= 1;
                        high += (((int)System.Math.Ceiling((offsetBits - (high % step)) / (double)step)) - (high / step)) * step
                            - windowBits;
                    }
                }

                /* simple loop:
                 * for (var j = 0; j < offset; j++)
                    {
                        low -= bitLength;
                        high -= bitLength;
                        if (low < bitLength)
                        {
                            low += step * (int)System.Math.Ceiling((double)(bitLength - low) / step);
                        }
                        if (high < bitLength)
                        {
                            high += step * (int)System.Math.Ceiling((double)(bitLength - high) / step);
                        }
                    }
                */

                /*optimized loop

                if (low >= overflowWindowSize)
                {
                    low -= windowSize;
                }
                else if  (low >= windowSize)
                {
                    low -= windowSize;
                    if (low < bitLength)
                        low += step * (int)System.Math.Ceiling((double)(bitLength - low) / step);
                }
                else
                {
                    for (var j = 0; j < offset; j++)
                    {
                        low -= bitLength;
                        if (low < bitLength)
                        {
                            low += step * (int)System.Math.Ceiling((double)(bitLength - low) / step);
                        }
                    }
                }
                if (high >= overflowWindowSize)
                {
                    high -= windowSize;
                }
                else if (high >= windowSize)
                {
                    high -= windowSize;
                    if (high < bitLength)
                        high += step * (int)System.Math.Ceiling((double)(bitLength - high) / step);
                }
                else
                {
                    for (var j = 0; j < offset; j++)
                    {
                        high -= bitLength;
                        if (high < bitLength)
                        {
                            high += step * (int)System.Math.Ceiling((double)(bitLength - high) / step);
                        }
                    }
                }
                */
#endif




            }
            return true;

        }
        int calcAbsFromOffset(uint offsetValue, int step, int windowSize)
        {
            int offset = GetBitIndex(offsetValue);

            //var oneliner = GetBitIndex(offsetValue) + (((int)System.Math.Ceiling((windowSize - (offset % step)) / (double)step)) - (offset / step))*step;

            int stepsPerWindow = (int)System.Math.Ceiling((windowSize - (offset % step)) / (double)step);
            var stepsTaken = offset / step;
            var stepsNeeded = (stepsPerWindow - stepsTaken);
            var stepSize = stepsNeeded * step;
            var result = offset + stepSize;
            return result;

            //=[@offset]+([@StepsPerBitLength]-FLOOR.MATH([@offset]/[@step]))*[@step]
        }
        int CalcWindowOffset(int value, int step, int bitLength, int offset, int windowSize, int overflowWindowSize)
        {
            if (value >= overflowWindowSize) return value -= windowSize; ;
            var startValue = value;

            if (bool.Parse(bool.FalseString))
            {
                value = startValue;
            }
            if (value >= windowSize)
            {
                value -= windowSize;
            }
            else if (value >= bitLength)
            {
#if LOG_OFFSETS
                var distance = windowSize - value;
                var stepsForDistance = (int)(System.Math.Ceiling((double)distance / step));
                var calcValue = value + stepsForDistance * step;
                calcValue -= bitLength;
                var stepsPerWindow = windowSize / step;
                var stepsTaken = (int)System.Math.Floor((double)value / step);
                //calcValue = value + (stepsPerWindow - stepsTaken) * step;
                //=[@offset]+([@StepsPerBitLength]-FLOOR.MATH([@offset]/[@step]))*[@step]

                calcValue = value + (stepsPerWindow - stepsTaken) * step;
                calcValue -= bitLength;
                if (value > windowSize + bitLength)
                {
                    calcValue = value - windowSize;
                }
                var totalBits = bitLength * offset;
                var doubleSteps = (double)totalBits / step;
                var testSteps = (int)System.Math.Ceiling(doubleSteps);
                var actualSteps = 0;
#endif
                for (int windowCount = offset; windowCount > 0 && value >= bitLength; windowCount--)
                {
                    value -= bitLength;
                    if (value < bitLength)
                    {
                        value += step * (int)System.Math.Ceiling((double)(bitLength - value) / step);
                    }
                }
#if LOG_OFFSETS
                if (calcValue > 0 && calcValue != value)
                {
                    string bp = "invalid calc value";
                }
#endif 

            }
            if (value < bitLength)
                value += step * (int)System.Math.Ceiling((double)(bitLength - value) / step);
            return value;

        }

        int CalcWindowOffset1(int value, int step, int bitLength, int offset)
        {

            if (value >= bitLength)
            {
                for (int windowCount = offset; windowCount > 0 && value >= bitLength; windowCount--)
                {
                    value -= bitLength;
                    if (value < bitLength)
                    {
                        value += step * (int)System.Math.Ceiling((double)(bitLength - value) / step);
                    }
                }
            }
            if (value < bitLength)
                value += step * (int)System.Math.Ceiling((double)(bitLength - value) / step);
            return value;

        }
        private class Step
        {
            private int _subtractValue;
            private int _initialValue;
            public Step(int value, int stepSize, int windowSize, int stepNumber)
            {
                InitialValue = value;
                StepSize = stepSize;
                WindowSize = windowSize;
                StepNumber = stepNumber;
            }

            public int InitialValue { get => _initialValue; set => Value = SubtractValue = _initialValue = value; }

            public int SubtractValue { get => _subtractValue; set => Value = _subtractValue = value; }
            public int Value { get; set; }

            public int StepNumber { get; set; }
            public int StepSize { get; set; }
            public int WindowSize { get; set; }
            public override string ToString()
            {
                return $"Step: {StepNumber} - {InitialValue} => {SubtractValue} => {Value}";
            }
        }
        private List<Step> CalcStepsWithLoop(int value, int stepSize, int windowSize, int numSteps)
        {
            List<Step> result = new();
            var data = new Step(value, stepSize, windowSize, -1);
            result.Add(data);

            for (var i = 0; i < numSteps; i++)
            {
                data = new Step(value, stepSize, windowSize, i);
                result.Add(data);
                data.SubtractValue -= windowSize;
                if (data.SubtractValue < windowSize)
                {
                    data.Value += stepSize * (int)System.Math.Ceiling((double)(windowSize - data.SubtractValue) / stepSize);
                }
                value = data.Value;
            }

            return result;
        }

        private bool skipToNextWindow()
        {
            Window += 1;
            int bitLength = BitLength;
            int step;
            for (var i = 0;
                i <= primecount;
                i++)
            {
                ref int low = ref lowindices[i];
                ref int high = ref highindices[i];
                low -= bitLength;
                high -= bitLength;


                if (low < bitLength || high < bitLength)
                {
                    step = sievePrimes[i];
                    step <<= 1;
                    if (low < bitLength)
                    {
                        //if (low + step > bitLength)
                        //{
                        //    low += step;
                        //}
                        //else
                        {

                            /* debugging */
                            //var offset = low;
                            //int actual = 0;
                            //for (; low < bitLength; low += step, actual++) ;

                            //int calcSteps = (int)System.Math.Ceiling((double)(bitLength - offset) / step);
                            //if (calcSteps != actual)
                            //{
                            //    string bp = "";
                            //}

                            low += step * (int)System.Math.Ceiling((double)(bitLength - low) / step);
                        }
                    }
                    if (high < bitLength)
                    {
                        //if (high + step > bitLength)
                        //{
                        //    high += step;
                        //}
                        //else
                        {
                            high += step * (int)System.Math.Ceiling((double)(bitLength - high) / step);
                        }

                    }


                    //for (; low < bitLength; low += step) ;
                    //for (; high < bitLength; high += step) ;

                    //TODO: Calculate index for next window without loop.


                    /*
                    if (low < bitLength)
                    {
                        int calcLowSteps = (bitLength - low) / step;
                        low += calcLowSteps * step;
                        if (low < bitLength) low += step;
                    }
                    if (high< bitLength)
                    {
                        int calcHigh = (bitLength - high) / step;
                        high += calcHigh * step;
                        if (high < bitLength) high += step;
                    }
                    */
                }

            }
            return true;
        }
        private bool sieveNextWindow()
        {
            clearSieve();
            bitIndex = 0;
            Window += 1;

            int bitLength = BitLength;              //massive performance hit calling property in loop.
            int bitMask;
            int step;
            //var lowindices = this.lowindices;     //local variable causes 10% slowdown
            //var highindices=this.highindices;     //local variable causes 10% slowdown
            ref int segment = ref sieve[0];         //does seem to effect time, but better not to allocate
            ref int value = ref lowindices[0];      //does seem to effect time, but better not to allocate
            //var sievePrimes = this.sievePrimes;     //local variable without ref gives sligh performance increase
            int primecount = this.primecount;
            if (primecount == -1)
            {
                return false;

            }
            unsafe
            {
                //fixed (int* lowindices = this.lowindices)
                //fixed (int* highindices = this.lowindices)
                fixed (int* sievePrimes = this.sievePrimes)
                {
                    int* sievePrime = sievePrimes;
                    //int* low = lowindices;
                    //int* high = highindices;
                    for (var i = 0; i <= primecount; i++, sievePrime++)
                    {
                        value = ref lowindices[i];
                        value -= bitLength;
                        step = *sievePrime;// sievePrimes[i];
                        step <<= 1;
                        //int test = *sievePrime;
                        //int test2 = sievePrimes[i];
                        if (value < bitLength)
                        {
                            //for (; value < bitLength; value += step)
                            while (value < bitLength)
                            {

                                //ref int segment = ref sieve[low >> 5];
                                bitMask = value >> 5;
                                segment = ref sieve[bitMask];
                                bitMask = 1 << value;
                                segment &= ~bitMask;
                                value += step;
                            }
                        }

                        value = ref highindices[i];
                        value -= bitLength;
                        if (value < bitLength)
                        {
                            step = sievePrimes[i];
                            step <<= 1;
                            //for (; value < bitLength; value += step)
                            while (value < bitLength)
                            {

                                //ref int segment = ref sieve[low >> 5];
                                bitMask = value >> 5;
                                segment = ref sieve[bitMask];
                                bitMask = 1 << value;
                                segment &= ~bitMask;
                                value += step;
                            }
                        }
                    }
                }
            }
#if useManagedArrays

            for (var i = 0;
                //i < lowindices.Length &&
                //i < highindices.Length &&
                //i < sievePrimes.Length &&
                i <= primecount;

                i++)
            {
                value = ref lowindices[i];
                value -= bitLength;
                step = sievePrimes[i];
                step <<= 1;
                if (value < bitLength)
                {
                    for (; value < bitLength; value += step)
                    {

                        //ref int segment = ref sieve[low >> 5];
                        bitMask = value >> 5;
                        segment = ref sieve[bitMask];
                        bitMask = 1 << value;
                        segment &= ~bitMask;
                    }
                }
                value = ref highindices[i];
                value -= bitLength;
                if (value < bitLength)
                {
                    step = sievePrimes[i];
                    step <<= 1;
                    for (; value < bitLength; value += step)
                    {

                        //ref int segment = ref sieve[low >> 5];
                        bitMask = value >> 5;
                        segment = ref sieve[bitMask];
                        bitMask = 1 << value;
                        segment &= ~bitMask;
                    }
                }

                //ref int low = ref lowindices[i];
                //ref int high = ref highindices[i];
                //;
                //low -= bitLength;
                //high -= bitLength;
                //if (low < bitLength || high < bitLength)
                //{
                //    step = sievePrimes[i];
                //    step <<= 1;
                //    for (; low < bitLength; low += step)
                //    {

                //        //ref int segment = ref sieve[low >> 5];
                //        bitMask = low >> 5;
                //        segment = ref sieve[bitMask];
                //        bitMask = 1 << low;
                //        segment &= ~bitMask;
                //    }
                //    for (; high < bitLength; high += step)
                //    {
                //        //ref int segment = ref sieve[high >> 5];
                //        bitMask = high >> 5;
                //        segment = ref sieve[bitMask]; 
                //        bitMask = 1 << high;
                //        segment &= ~bitMask;
                //    }
                //}
            }
#endif
            return true;
        }


        private void readAndSieve()
        {
            readNextPrime();


            if (value <= maxSievePrime)
            {
                int prime = (int)_current;
                SievePrime x = null!;
                x = new SievePrime(prime, bitIndex, this);
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

                // initial bit indexes are absolute. Offset index to current window bits.
                low -= windowBits;
                high -= windowBits;

                int bitLength = BitLength;
                if (low < bitLength)
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
                _sievedFactorBase = true;
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

        public IEnumerator<uint> GetEnumerator()
            => GetEnumerator(false);
        public IEnumerator<uint> GetEnumerator(bool useIntGenerator)
        {
            if (useIntGenerator && maxPrime <= PrimeData.MaxIntPrime)
            {
                var gen = new PrimeGeneratorUnsafe((int)this.startValue, (int)maxPrime);
                var iter = gen.GetEnumerator();
                while (iter.MoveNext())
                {
                    yield return (uint)iter.Current;
                }
            }
            else
            {
                while (value < maxPrime)
                {
                    incNext();
                    yield return value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Return the bit array index of the value n,

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetBitIndex(uint value) => (int)(value / 3) - 1;
        /*{
            int index = (value / 6) << 1;
            var res = (value % 6);
            if (res == 1)
                index -= 1;
            var test = (value / 3) - 1;
            return index;
        }*/

        public int GetBitIndexForCurrentWindow(uint value)
        {
            int index = GetBitIndex(value);
            index = index - windowBits;
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //Return the value represented by bitindex
        public uint GetBitWindowValue(int index)
            => GetBitValue(index + windowBits);


        //Return the value represented by bitindex
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetBitValue(int index)
        {
            //index = 5 + ((index >> 1) * 6) + (2 * (index & 1));
            //index = (((index + 2) * 3) + -1) - (index & 1);
            var m = index & 1;
            long result = index;
            result += 2;
            result *= 3;
            result -= 1;
            result -= m;
            return (uint)result;
        }

        private class SievePrime
        {
            internal int lowBitIndex;
            internal int highBitIndex;
            private int step;
            private PrimeGeneratorUnsafeUint sieve;

            public int Value { get; }
            public int LowBitIndex { get => lowBitIndex; set => lowBitIndex = value; }
            public int HighBitIndex { get => highBitIndex; set => highBitIndex = value; }
            public int Step => step;

            public SievePrime(int current, int bitIndex, PrimeGeneratorUnsafeUint sieve)
            {
                Value = current;
                this.sieve = sieve;
                step = current << 1;

                var intersect = current * 6;
                var low = intersect - current;
                var high = intersect + current;
                var sq = (long)current * current;


                //while (low < sq)
                //{
                //    low += intersect;
                //}
                uint lowIntersect = calculate(low, intersect, sq);

                //while (high < sq)
                //{
                //    high += intersect;
                //}
                uint highIntersect = calculate(high, intersect, sq);
                //if (low > high)
                //{
                //    sq = low;
                //    low = high;
                //    high = sq;
                //}

                lowBitIndex = sieve.GetBitIndexForCurrentWindow(lowIntersect);
                highBitIndex = sieve.GetBitIndexForCurrentWindow(highIntersect);

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

            public uint calculate(int value, int intersect, long sq)
            {
                uint delta = (uint)intersect;

                // calculate the number of steps to reach the first value >= sq
                int steps = (int)System.Math.Ceiling((double)(sq - value) / delta);

                // calculate the next value
                long nextValue = value + (steps * delta);
                if (nextValue > uint.MaxValue)
                {
                    throw new OverflowException("Cannot set max value to greater than uint.MaxValue");
                }
                return (uint)nextValue;
            }


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