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

namespace HigginsSoft.Math.Lib
{
    public class PrimeGeneratorTrialDivide : IPrimeGenerator
    {
        private int _value;
        private int _current;
        private int _previous;
        private int maxPrime;
        public int Current => _current;
        public int Previous => _previous;
        //private List<int> _primes;

        public PrimeGeneratorTrialDivide() : this(PrimeData.MaxIntPrime) { }
        public PrimeGeneratorTrialDivide(int maxPrime = 2147483647)
        {
            _value = 1;
            this.maxPrime = maxPrime;

            Action incP2 = null!;
            Action incP4 = null!;
            Action inc2 = null!;
            Action inc3 = null!;
            Action inc5 = null!;
            incP2 = () =>
            {
                _value += 2;
                inc = incP4;
            };
            incP4 = () =>
            {
                _value += 4;
                inc = incP2;
            };
            inc5 = () =>
            {
                _value = 5;
                inc = incP2;
            };
            inc3 = () =>
            {
                _value = 3;
                inc = inc5;
            };
            inc2 = () =>
            {
                _value = 2;
                inc = inc3;
            };

            this.inc = inc2;
        }

        Action inc = null!;
        public IEnumerator<int> GetEnumerator()
        {
            while (_value<maxPrime)
            {
                inc();

                if (IsPrime(_value))
                {
                    _previous = _current;
                    _current = _value;
 
                    yield return _value;
                    if (_value == 2147483647)
                        break;
                }
            }
        }

        private IEnumerator<int> iter => GetEnumerator();

        public int BitIndex => GetBitIndex(Current);
     
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool IsPrime(int n)
            => Primes.IsPrime(n);

        public int GetBitIndex(int value)
        {
            if (value < 5) return value == 2 ? -2 : -1;
            int index = (value / 6) << 1;
            var res = (value % 6);
            if (res == 1)
                index -= 1;
            return index;
        }


        public int GetBitValue(int index)
        {
            index = 5 + ((index >> 1) * 6) + (2 * (index & 1));
            return index;
        }
    }
}