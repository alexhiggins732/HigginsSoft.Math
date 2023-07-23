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
    public partial class MathUtil
    {
        public static GmpInt ModInv(GmpIntConvertible a, GmpIntConvertible b)
        {
            var res = GcdExt(a.Value, b.Value);
            var gcd = res.Gcd;
            if (gcd.IsOne) return a;
            return res.S % b;
        }

        public static int ModInv(int a, int b)
        {
            var res = GcdExtInt(a, b);
            var gcd = res.Gcd;
            if (gcd == 1) return a;
            return res.S % b;
        }


    }
}
