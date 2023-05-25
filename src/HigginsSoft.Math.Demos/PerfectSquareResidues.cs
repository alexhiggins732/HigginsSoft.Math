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

namespace HigginsSoft.Math.Demos
{
    public class PerfectSquareResidues
    {
        public static int Count(sbyte value) => Count((int)value);
        public static int Count(byte value) => Count((int)value);
        public static int Count(short value) => Count((int)value);
        public static int Count(ushort value) => Count((int)value);
   
        public static int Count(int value)
        {
            var root = MathLib.Sqrt(value, out bool isexact);
            int start = isexact ? root : root + 1;
            //if (isexact) { return value < 1 ? 0 : 2; }
            int end = (int)value - 1;

            int count = 0;
            for (var i = start; i <= end; i++)
            {
                var square = i * i;
                var res = square % value;
                var isSquare = res == 0 ? true : MathLib.IsPerfectSquare(res);
                if (isSquare)
                    count++;
            }

            return count; 
        }
     
        public static List<int> GetPerfectSquareResidues(int value)
        {
            var root = MathLib.Sqrt(value, out bool isexact);
            int start = isexact ? root : root + 1;
            //if (isexact) { return value < 1 ? 0 : 2; }
            int end = (int)value - 1;
            List<int> result = new();

            for (var i = start; i <= end; i++)
            {
                var square = i * i;
                var res = square % value;
                var isSquare = res == 0 ? true : MathLib.IsPerfectSquare(res);
                if(isSquare)
                    result.Add(res);
            }

            return result;
        }

        public static List<int> GetPerfectSquareResidueDistributions(int value)
        {
            var root = MathLib.Sqrt(value, out bool isexact);
            int start = isexact ? root : root + 1;
            //if (isexact) { return value < 1 ? 0 : 2; }
            int end = (int)value -1 - root;
            List<int> result = new();

            for (var i = start; i <= end; i++)
            {
                var square = i * i;
                var res = square % value;
                var isSquare = res == 0 ? true : MathLib.IsPerfectSquare(res);
                if (isSquare)
                    result.Add(i);
            }

            return result;
        }

    }
}