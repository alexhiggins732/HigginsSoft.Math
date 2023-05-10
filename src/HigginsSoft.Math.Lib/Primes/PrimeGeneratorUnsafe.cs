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
    public class PrimeGeneratorPreSieve
    {
        //11 - length:385
        public static uint[] Wheel11U => new uint[] { 0x96ED7B78, 0xCDEB361F, 0xBB56DF23, 0x7ADCA5E5, 0xC797CCBB, 0xB769F16E, 0xAD733EDC, 0xDA6E5BB5, 0x7CCFB3A5, 0x9F966D7A, 0x33EDEB56, 0xE4B95EDB, 0xDB7A9D27, 0x6AD7B7CC, 0x5EB669F9, 0xA5CDF33E, 0xADDA765B, 0x3B7CCEB5, 0x768F96ED, 0xDD33E9E3, 0xA7A53B5E, 0xC4FB6ADD, 0xF86CD7B3, 0x1EDEB769, 0x5BB5EDE3, 0x37ACD87E, 0xED5B7C8F, 0xEB729796, 0x1E5F32ED, 0xDDA7C5BB, 0xB5CCFB72, 0x69B96ED6, 0xF33ECE97, 0x7E59B5E9, 0xCFB7AD5A, 0x92E57B6C, 0x6DEA749F, 0xAB5EDF33, 0x7AD9A7E5, 0x9737CCF9, 0xB769D96E, 0xEDF33AD6, 0xDA3E5BB4, 0x74CFA78D, 0x9E94ED7B, 0x13EDAB76, 0xE1B35ECF, 0x7B7ADDA7, 0x6ED7B78C, 0xDEB361F9, 0xB56DF23C, 0xADCA5E5B, 0x797CCBB7, 0x769F16EC, 0xD733EDCB, 0xA6E5BB5A, 0xCCFB3A5D, 0xF966D7A7, 0x3EDEB569, 0x4B95EDB3, 0xB7A9D27E, 0xAD7B7CCD, 0xEB669F96, 0x5CDF33E5, 0xDDA765BA, 0xB7CCEB5A, 0x68F96ED3, 0xD33E9E37, 0x7A53B5ED, 0x4FB6ADDA, 0x86CD7B3C, 0xEDEB769F, 0xBB5EDE31, 0x7ACD87E5, 0xD5B7C8F3, 0xB729796E, 0xE5F32EDE, 0xDA7C5BB1, 0x5CCFB72D, 0x9B96ED6B, 0x33ECE976, 0xE59B5E9F, 0xFB7AD5A7, 0x2E57B6CC, 0xDEA749F9, 0xB5EDF336, 0xAD9A7E5A, 0x737CCF97, 0x769D96E9, 0xDF33AD6B, 0xA3E5BB4E, 0x4CFA78DD, 0xE94ED7B7, 0x3EDAB769, 0x1B35ECF1, 0xB7ADDA7E, 0xED7B78C7, 0xEB361F96, 0x56DF23CD, 0xDCA5E5BB, 0x97CCBB7A, 0x69F16EC7, 0x733EDCB7, 0x6E5BB5AD, 0xCFB3A5DA, 0x966D7A7C, 0xEDEB569F, 0xB95EDB33, 0x7A9D27E4, 0xD7B7CCDB, 0xB669F96A, 0xCDF33E5E, 0xDA765BA5, 0x7CCEB5AD, 0x8F96ED3B, 0x33E9E376, 0xA53B5EDD, 0xFB6ADDA7, 0x6CD7B3C4, 0xDEB769F8, 0xB5EDE31E, 0xACD87E5B, 0x5B7C8F37, 0x729796ED, 0x5F32EDEB, 0xA7C5BB1E, 0xCCFB72DD, 0xB96ED6B5, 0x3ECE9769, 0x59B5E9F3, 0xB7AD5A7E, 0xE57B6CCF, 0xEA749F92, 0x5EDF336D, 0xD9A7E5AB, 0x37CCF97A, 0x69D96E97, 0xF33AD6B7, 0x3E5BB4ED, 0xCFA78DDA, 0x94ED7B74, 0xEDAB769E, 0xB35ECF13, 0x7ADDA7E1, 0xD7B78C7B, 0xB361F96E, 0x6DF23CDE, 0xCA5E5BB5, 0x7CCBB7AD, 0x9F16EC79, 0x33EDCB76, 0xE5BB5AD7, 0xFB3A5DA6, 0x66D7A7CC, 0xDEB569F9, 0x95EDB33E, 0xA9D27E4B, 0x7B7CCDB7, 0x669F96AD, 0xDF33E5EB, 0xA765BA5C, 0xCCEB5ADD, 0xF96ED3B7, 0x3E9E3768, 0x53B5EDD3, 0xB6ADDA7A, 0xCD7B3C4F, 0xEB769F86, 0x5EDE31ED, 0xCD87E5BB, 0xB7C8F37A, 0x29796ED5, 0xF32EDEB7, 0x7C5BB1E5, 0xCFB72DDA, 0x96ED6B5C, 0xECE9769B, 0x9B5E9F33, 0x7AD5A7E5, 0x57B6CCFB, 0xA749F92E, 0xEDF336DE, 0x9A7E5AB5, 0x7CCF97AD, 0x9D96E973, 0x33AD6B76, 0xE5BB4EDF, 0xFA78DDA3, 0x4ED7B74C, 0xDAB769E9, 0x35ECF13E, 0xADDA7E1B, 0x7B78C7B7, 0x361F96ED, 0xDF23CDEB, 0xA5E5BB56, 0xCCBB7ADC, 0xF16EC797, 0x3EDCB769, 0x5BB5AD73, 0xB3A5DA6E, 0x6D7A7CCF, 0xEB569F96, 0x5EDB33ED, 0x9D27E4B9, 0xB7CCDB7A, 0x69F96AD7, 0xF33E5EB6, 0x765BA5CD, 0xCEB5ADDA, 0x96ED3B7C, 0xE9E3768F, 0x3B5EDD33, 0x6ADDA7A5, 0xD7B3C4FB, 0xB769F86C, 0xEDE31EDE, 0xD87E5BB5, 0x7C8F37AC, 0x9796ED5B, 0x32EDEB72, 0xC5BB1E5F, 0xFB72DDA7, 0x6ED6B5CC, 0xCE9769B9, 0xB5E9F33E, 0xAD5A7E59, 0x7B6CCFB7, 0x749F92E5, 0xDF336DEA, 0xA7E5AB5E, 0xCCF97AD9, 0xD96E9737, 0x3AD6B769, 0x5BB4EDF3, 0xA78DDA3E, 0xED7B74CF, 0xAB769E94, 0x5ECF13ED, 0xDDA7E1B3, 0xB78C7B7A, 0x61F96ED7, 0xF23CDEB3, 0x5E5BB56D, 0xCBB7ADCA, 0x16EC797C, 0xEDCB769F, 0xBB5AD733, 0x3A5DA6E5, 0xD7A7CCFB, 0xB569F966, 0xEDB33EDE, 0xD27E4B95, 0x7CCDB7A9, 0x9F96AD7B, 0x33E5EB66, 0x65BA5CDF, 0xEB5ADDA7, 0x6ED3B7CC, 0x9E3768F9, 0xB5EDD33E, 0xADDA7A53, 0x7B3C4FB6, 0x769F86CD, 0xDE31EDEB, 0x87E5BB5E, 0xC8F37ACD, 0x796ED5B7, 0x2EDEB729, 0x5BB1E5F3, 0xB72DDA7C, 0xED6B5CCF, 0xE9769B96, 0x5E9F33EC, 0xD5A7E59B, 0xB6CCFB7A, 0x49F92E57, 0xF336DEA7, 0x7E5AB5ED, 0xCF97AD9A, 0x96E9737C, 0xAD6B769D, 0xBB4EDF33, 0x78DDA3E5, 0xD7B74CFA, 0xB769E94E, 0xECF13EDA, 0xDA7E1B35, 0x78C7B7AD, 0x1F96ED7B, 0x23CDEB36, 0xE5BB56DF, 0xBB7ADCA5, 0x6EC797CC, 0xDCB769F1, 0xB5AD733E, 0xA5DA6E5B, 0x7A7CCFB3, 0x569F966D, 0xDB33EDEB, 0x27E4B95E, 0xCCDB7A9D, 0xF96AD7B7, 0x3E5EB669, 0x5BA5CDF3, 0xB5ADDA76, 0xED3B7CCE, 0xE3768F96, 0x5EDD33E9, 0xDDA7A53B, 0xB3C4FB6A, 0x69F86CD7, 0xE31EDEB7, 0x7E5BB5ED, 0x8F37ACD8, 0x96ED5B7C, 0xEDEB7297, 0xBB1E5F32, 0x72DDA7C5, 0xD6B5CCFB, 0x9769B96E, 0xE9F33ECE, 0x5A7E59B5, 0x6CCFB7AD, 0x9F92E57B, 0x336DEA74, 0xE5AB5EDF, 0xF97AD9A7, 0x6E9737CC, 0xD6B769D9, 0xB4EDF33A, 0x8DDA3E5B, 0x7B74CFA7, 0x769E94ED, 0xCF13EDAB, 0xA7E1B35E, 0x8C7B7ADD, 0xF96ED7B7, 0x3CDEB361, 0x5BB56DF2, 0xB7ADCA5E, 0xEC797CCB, 0xCB769F16, 0x5AD733ED, 0x5DA6E5BB, 0xA7CCFB3A, 0x69F966D7, 0xB33EDEB5, 0x7E4B95ED, 0xCDB7A9D2, 0x96AD7B7C, 0xE5EB669F, 0xBA5CDF33, 0x5ADDA765, 0xD3B7CCEB, 0x3768F96E, 0xEDD33E9E, 0xDA7A53B5, 0x3C4FB6AD, 0x9F86CD7B, 0x31EDEB76, 0xE5BB5EDE, 0xF37ACD87, 0x6ED5B7C8, 0xDEB72979, 0xB1E5F32E, 0x2DDA7C5B, 0x6B5CCFB7, 0x769B96ED, 0x9F33ECE9, 0xA7E59B5E, 0xCCFB7AD5, 0xF92E57B6, 0x36DEA749, 0x5AB5EDF3, 0x97AD9A7E, 0xE9737CCF, 0x6B769D96, 0x4EDF33AD, 0xDDA3E5BB, 0xB74CFA78, 0x69E94ED7, 0xF13EDAB7, 0x7E1B35EC, 0xC7B7ADDA };
        public static int[] Wheel11 => Wheel11U.Select(x => (int)x).Concat(Wheel11U.Select(x => (int)x)).ToArray();

        public static int[] expectedLow = new int[] { 49305, 49299, 49303, 49321, 49329, 49319, 49303, 49353, 49287, 49285, 49291, 49391, 49323, 49397, 49379, 49361, 49353, 49317, 49327, 49299, 49471, 49381, 49491, 49497, 49431, 49309, 49425, 49353, 49381, 49495, 49469, 49385, 49497, 49345, 49469, 49591, 49557, 49591, 49341, 49517, 49561, 49631, 49585, 49367, 49449, 49549, 49539, 49493, 49445, 49385, 49683, 49299, 49495, 49449, 49713, 49825, 49599, 49515, 49371, 49905, 49689, 49837, 49831, 49839, 49531, 49581, 49945, 49829, 50017, 49375, 49915, 49907, 50049, 49675, 49633, 49625, 49771, 49559, 49669, 49967, 50139, 49459, 49659, 49727, 49753, 49789, 49769, 49649, 49557, 49309, 50159, 49897, 50271, 50145, 49853, 49311, 49415, 49883, 50327, 50089, 49343, 50341, 49491, 49791, 49485, 49749, 49989, 49313, 50205, 49303, 49937, 49543, 50033, 50105, 50153, 49723, 52415, 54215, 56935, 60599, 65239, 68999, 73759, 77615, 80535, 83479, 85455, 89439, 92455, 94479, 98559, 100615, 107895, 113175, 119599, 120679, 126119, 127215, 129415, 130519, 136079, 143975, 146255, 147399, 149695, 157815, 160159, 161335, 163695, 175655, 178079, 182959, 189119, 194095, 196599, 200375, 204175, 213135, 215719, 219615, 223535, 228799, 232775, 240799, 243495, 247559 };

        public static int[] expectedHigh = new int[] { 49296, 49310, 49290, 49290, 49290, 49298, 49352, 49298, 49344, 49316, 49326, 49312, 49282, 49352, 49284, 49312, 49300, 49372, 49386, 49428, 49336, 49312, 49348, 49424, 49280, 49478, 49512, 49444, 49288, 49296, 49368, 49280, 49388, 49456, 49584, 49352, 49436, 49336, 49598, 49648, 49428, 49490, 49436, 49518, 49296, 49704, 49698, 49332, 49612, 49556, 49332, 49478, 49314, 49818, 49338, 49636, 49794, 49310, 49578, 49696, 49900, 49616, 49606, 49376, 49298, 49816, 49466, 49584, 49768, 49880, 49404, 49388, 49784, 49942, 49360, 49904, 49490, 49846, 49380, 49674, 49548, 49758, 49354, 50034, 49444, 50100, 50088, 49324, 49884, 49974, 49488, 50236, 49576, 49796, 49492, 50040, 49786, 50258, 49568, 49708, 50112, 49558, 49886, 50190, 50286, 49344, 49580, 49724, 49792, 50144, 50364, 50400, 50464, 50540, 50592, 49282, 53312, 54666, 57390, 61520, 65706, 69944, 74238, 78584, 81512, 84464, 85950, 90440, 93464, 94986, 99584, 101130, 108944, 113706, 120138, 121760, 126666, 128312, 129966, 131624, 136638, 145112, 146826, 148544, 150270, 158984, 160746, 162512, 164286, 176864, 178686, 184184, 189738, 195344, 197226, 201006, 204810, 214424, 216366, 220266, 224190, 230120, 234104, 242144, 244170, 248238 };
    }
    public class PrimeGeneratorUnsafe : IPrimeGenerator
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

        public int CurrentWindow => Window;
        private int Window { get => window; set => windowBits = (window = value) * BitLength; }
        public int BitIndex => bitIndex;

        private bool _sievedFactorBase;


        Action incNext = null!;

        public PrimeGeneratorUnsafe() : this(PrimeData.MaxIntPrime) { }


        bool logOffsets = bool.Parse(bool.TrueString);
        /// <summary>
        /// Construct a generator for primes in the range of <paramref name="startValue"/> to <paramref name="maxPrime"/>
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="maxPrime"></param>
        public PrimeGeneratorUnsafe(int startValue, int endValue)
            : this(endValue)
        {

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

            if (startValue <= 2) return;
            if (startValue == 3) {incNext(); return; }
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
            var c = _current;
            var p = _previous;
            if (startWindow > 0 && windowBitIndex > 1)
            {
                bitIndex = windowBitIndex - 1;
                //incNext();
                return;
            }
            
            while (bitIndex < windowBitIndex-1)
            {
                incNext();
            }

        }
        public PrimeGeneratorUnsafe(int maxPrime = 2147483647)
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



            //index++;

            //int wordIndex = index >> 5; // index / 32
            //int wordOffset = index & 31; // index % 32

            //uint word = (uint)sieve[wordIndex];
            //word >>= wordOffset;

            //if (word == 0)
            //{
            //    index += 32-wordOffset;
            //    if (index == bitLength)
            //    {
            //        sieveNextWindow();
            //    }
            //    word = (uint)sieve[++wordIndex];
            //    while (word == 0)
            //    {
            //        index += 32;
            //        if (index==bitLength)
            //        {
            //            sieveNextWindow();
            //        }
            //        word = (uint)sieve[++wordIndex];
            //    }
            //}
            //while ((word & 1) == 0)
            //{
            //    index++;
            //    word >>= 1;

            //}
            /* TODO: optimze read by elimating word look and mask on each step
            index++;
            
            
            int word = sieve[bitMask];
           
            int bitCount = index & 31;
            word >>= index & 31;
            while (index < bitLength)
            {
     
                for (; word != 0 && (bitMask = (word & 1)) == 0; bitCount++)
                {
                    word >>= 1;
                    index++;
                }
                if (bitMask == 1)
                {
                    break;
                }
                index += (32-bitCount);
                if (index == bitLength)
                {
                    sieveNextWindow();
                }
                bitCount = 0;
                bitMask = index >> 5;
                word = sieve[bitMask];

            }
           */

            value = GetBitWindowValue(index);
            //value = 5 + ((index >> 1) * 6) + (2 * (index & 1)) + windowBits;

            _previous = _current;
            _current = value;


        }

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
        int calcAbsFromOffset(int offsetValue, int step, int windowSize)
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
                string bp = "";
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
            unsafe
            {
                //fixed (int* lowindices = this.lowindices)
                //fixed (int* highindices = this.lowindices)
                fixed (int* sievePrimes = this.sievePrimes)
                {
                    int* sievePrime = sievePrimes;
                    //int* low = lowindices;
                    //int* high = highindices;
                    int* lastPrime = sievePrime + primecount;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetBitIndex(int value) => (value / 3) - 1;
        /*{
            int index = (value / 6) << 1;
            var res = (value % 6);
            if (res == 1)
                index -= 1;
            var test = (value / 3) - 1;
            return index;
        }*/

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
            //index = 5 + ((index >> 1) * 6) + (2 * (index & 1));
            //index = (((index + 2) * 3) + -1) - (index & 1);
            var m = index & 1;
            index += 2;
            index *= 3;
            index -= 1;
            index -= m;
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
            private PrimeGeneratorUnsafe sieve;
            public SievePrime(int current, int bitIndex, PrimeGeneratorUnsafe sieve)
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
                //if (low > high)
                //{
                //    sq = low;
                //    low = high;
                //    high = sq;
                //}

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