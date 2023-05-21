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
    public static partial class MathLib
    {
        static Random rand = new Random();

        public static long PollardRho(long n, int maxAttempts = 20)
        {

            if (n == 1) return 1;
            if ((n & 1) == 0) return 2;

            long x;
            long y;
            long c;
            long abs;
            long gcd = 1;

            for (var i = 0; gcd == 1 && i < maxAttempts; i++)
            {
                x = rand.Next(0, (int)(n - 2)) + 2;
                y = x;
                c = rand.Next(0, (int)(n - 1)) + 1;
                while (gcd == 1)
                {
                    x = (x * x + c) % n;
                    y = (y * y + c) % n;
                    y = (y * y + c) % n;
                    abs = MathLib.Abs(x - y);
                    gcd = MathUtil.Gcd(abs, n);
                }
            }
            if (gcd== 1)
            {
                gcd = n;
            }
            return gcd;
        }

    }
}