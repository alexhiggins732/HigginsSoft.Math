/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

namespace HigginsSoft.Math.Lib.Tests.GmpIntTests
{
    public class ArithmeticTestCases
    {
        public class Signed
        {
            public static TestData<int, int, int>[] Add => new[]{
                TestData.Create(1, 0, 1),
                TestData.Create(1, 1, 2),
                TestData.Create(-1, 1, 0),
                TestData.Create(-1, 0, -1),
                TestData.Create(0, -1, -1),
                TestData.Create(-1, -1, -2),
            };

            public static TestData<int, int, int>[] Subtract => new[]{
                TestData.Create(2, 1, 1),
                TestData.Create(1, 2, -1),
                TestData.Create(1, 0, 1),
                TestData.Create(1, 1, 0),
                TestData.Create(-1, 1, -2),
                TestData.Create(-1, 0, -1),
                TestData.Create(0, -1, 1),
                TestData.Create(-1, -1, 0),
            };

            public static TestData<int, int, int>[] Multiply => new[]{
                TestData.Create(0, 1, 0),
                TestData.Create(1, 0, 0),
                TestData.Create(0, -1, 0),
                TestData.Create(-1, 0, 0),
                TestData.Create(-1, 1, -1),
                TestData.Create(-1, -1, 1),
                TestData.Create(2, -1, -2),
                TestData.Create(-1, 2, -2),
                TestData.Create(2, 2, 4),
                TestData.Create(5, 11, 55),
                TestData.Create(-2, 2, -4),
                TestData.Create(-2, -2, 4),
                TestData.Create(2, -2, -4),
                TestData.Create(5, -11, -55),
                TestData.Create(-5, 11, -55),
                TestData.Create(-5, -11, 55),
            };

            public static TestData<int, int, int>[] Divide => new[]{
                TestData.Create(0, 1, 0),
                TestData.Create(0, 2, 0),
                TestData.Create(0, -1, 0),
                TestData.Create(0, -2, 0),
                TestData.Create(1, 1, 1),
                TestData.Create(-1, -1, 1),
                TestData.Create(-1, 1, -1),
                TestData.Create(1, -1, -1),
                TestData.Create(2, 1, 2),
                TestData.Create(4, 2, 2),
                TestData.Create(4, -2, -2),
                TestData.Create(-4, 2, -2),

            };


            public static TestData<int, int, int>[] Modulo => new[]{
                
                TestData.Create(4, 2, 0),
                TestData.Create(3, 2, 1),
                TestData.Create(1, 1, 0),
                TestData.Create(0, 1, 0),
                //TestData.Create(1, 0, 1),
                TestData.Create(0, -1, 0),
                //TestData.Create(-1, 0, 0),
                TestData.Create(-3, 2, 1),
                
            };

            static IEnumerable<int> range = Enumerable.Range(-3, 6);
            public static TestData<int, int, int>[] And
                => range.SelectMany(a => range.Select(b => TestData.Create(a, b, a & b))).ToArray();

            public static TestData<int, int, int>[] Or
                => range.SelectMany(a => range.Select(b => TestData.Create(a, b, a | b))).ToArray();

            public static TestData<int, int, int>[] Xor
                => range.SelectMany(a => range.Select(b => TestData.Create(a, b, a ^ b))).ToArray();

            static IEnumerable<int> shiftRange = Enumerable.Range(0, 3);
            public static TestData<int, int, int>[] RightShift
                => shiftRange.SelectMany(a => shiftRange.Select(b => TestData.Create(a, b, a >> b))).ToArray();

            public static TestData<int, int, int>[] LeftShift
                => shiftRange.SelectMany(a => shiftRange.Select(b => TestData.Create(a, b, a << b))).ToArray();
        }

        public class Unsigned
        {
            private static bool UnsignedFilter(TestData<int, int, int> x)
                => x.A >= 0 && x.B >= 0 && x.Expected >= 0;

            public static TestData<int, int, int>[] Add
                => Signed.Add.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] Subtract
                => Signed.Subtract.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] Multiply
                => Signed.Multiply.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] Divide
                => Signed.Divide.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] Modulo
                => Signed.Modulo.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] And
            {
                get
                {
                    var data = Signed.And;
                    var result= data.Where(UnsignedFilter).ToArray();
                    return result;
                }
            }

            public static TestData<int, int, int>[] Or
                => Signed.Or.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] Xor
                => Signed.Xor.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] RightShift
                => Signed.RightShift.Where(UnsignedFilter).ToArray();

            public static TestData<int, int, int>[] LeftShift
                => Signed.LeftShift.Where(UnsignedFilter).ToArray();
        }
    }
}