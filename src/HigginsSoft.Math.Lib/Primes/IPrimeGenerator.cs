/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

namespace HigginsSoft.Math.Lib
{
    public interface IPrimeGenerator : IEnumerable<int>
    {
        int Previous { get; }
        int Current { get; }
        int BitIndex { get; }

        int GetBitIndex(int v);
        int GetBitValue(int v);
  
    }

    public interface IPrimeGenerator<T> : IEnumerable<T>
    {
        T Previous { get; }
        T Current { get; }
        int BitIndex { get; }

        int GetBitIndex(T v);
        T GetBitValue(int v);

    }
}