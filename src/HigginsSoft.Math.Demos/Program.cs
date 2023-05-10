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
using System.Reflection;

namespace HigginsSoft.Math.Demos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public class Qs16
    {
        public Factorization Factor(ushort value)
        {
            GmpInt z = value;
            Factorization result = new();

            var root = GmpInt.Sqrt(value, out bool isexact);
            if (isexact)
            {
                result.Add(root, 2);
            }
            else
            {

            }
            return result;
        }
    }
}