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
    public class CompareTestCases
    {
        public class Signed
        {
            public static TestData<int, int, int>[] CompareGreater => new[]{
                TestData.Create(2, 1, 1),
                TestData.Create(3, 1, 1),
                TestData.Create(-1, -2, 1),
            };

            public static TestData<int, int, int>[] CompareEqual => new[]{
                TestData.Create(1, 1,0 ),
                TestData.Create(0, 0, 0),
                TestData.Create(-1, -1, 0),
            };

            public static TestData<int, int, int>[] CompareLess => new[]{
                TestData.Create(1, 2, -1),
                TestData.Create(0, 1, -1),
                TestData.Create(-1, 0, -1),
            };

            public static TestData<int, int, bool>[] Greater => new[]{
                TestData.Create(2, 1, true),
                TestData.Create(1, 0, true),
                TestData.Create(0, -1, true),
                TestData.Create(-1, -2, true),

                TestData.Create(1, 1, false),
                TestData.Create(0, 0, false),
                TestData.Create(-1, -1, false),

                TestData.Create(1, 2, false),
                TestData.Create(0, 1, false),
                TestData.Create(-1, 0, false),
                TestData.Create(-2, -1, false),
            };

            public static TestData<int, int, bool>[] GreaterOrEqual => new[]{
                TestData.Create(2, 1, true),
                TestData.Create(1, 0, true),
                TestData.Create(0, -1, true),
                TestData.Create(-1, -2, true),

                TestData.Create(1, 1, true),
                TestData.Create(0, 0, true),
                TestData.Create(-1, -1, true),

                TestData.Create(1, 2, false),
                TestData.Create(0, 1, false),
                TestData.Create(-1, 0, false),
                TestData.Create(-2, -1, false),
            };

            public static TestData<int, int, bool>[] Equal => new[]{
                TestData.Create(2, 1, false),
                TestData.Create(1, 0, false),
                TestData.Create(0, -1, false),
                TestData.Create(-1, -2, false),

                TestData.Create(1, 1, true),
                TestData.Create(0, 0, true),
                TestData.Create(-1, -1, true),

                TestData.Create(1, 2, false),
                TestData.Create(0, 1, false),
                TestData.Create(-1, 0, false),
                TestData.Create(-2, -1, false),
            };

            public static TestData<int, int, bool>[] NotEqual => new[]{
                TestData.Create(2, 1, true),
                TestData.Create(1, 0, true),
                TestData.Create(0, -1, true),
                TestData.Create(-1, -2, true),

                TestData.Create(1, 1, false),
                TestData.Create(0, 0, false),
                TestData.Create(-1, -1, false),

                TestData.Create(1, 2, true),
                TestData.Create(0, 1, true),
                TestData.Create(-1, 0, true),
                TestData.Create(-2, -1, true),
            };

            public static TestData<int, int, bool>[] LessOrEqual => new[]{
                TestData.Create(2, 1, false),
                TestData.Create(1, 0, false),
                TestData.Create(0, -1, false),
                TestData.Create(-1, -2, false),

                TestData.Create(1, 1, true),
                TestData.Create(0, 0, true),
                TestData.Create(-1, -1, true),

                TestData.Create(1, 2, true),
                TestData.Create(0, 1, true),
                TestData.Create(-1, 0, true),
                TestData.Create(-2, -1, true),
            };

            public static TestData<int, int, bool>[] Less => new[]{
                TestData.Create(2, 1, false),
                TestData.Create(1, 0, false),
                TestData.Create(0, -1, false),
                TestData.Create(-1, -2, false),

                TestData.Create(1, 1, false),
                TestData.Create(0, 0, false),
                TestData.Create(-1, -1, false),

                TestData.Create(1, 2, true),
                TestData.Create(0, 1, true),
                TestData.Create(-1, 0, true),
                TestData.Create(-2, -1, true),
            };

        }

        public class Unsigned
        {
            public static TestData<int, int, int>[] CompareGreater => new[]{
                TestData.Create(2, 1, 1),
                TestData.Create(3, 1, 1),
                TestData.Create(1,0, 1),
            };

            public static TestData<int, int, int>[] CompareEqual => new[]{
                TestData.Create(1, 1,0 ),
                TestData.Create(0, 0, 0),

            };

            public static TestData<int, int, int>[] CompareLess => new[]{
                TestData.Create(1, 2, -1),
                TestData.Create(0, 1, -1),
            };

            public static TestData<int, int, bool>[] Greater => new[]{
                TestData.Create(2, 1, true),
                TestData.Create(1, 0, true),


                TestData.Create(1, 1, false),
                TestData.Create(0, 0, false),

                TestData.Create(1, 2, false),
                TestData.Create(0, 1, false),
            };

            public static TestData<int, int, bool>[] GreaterOrEqual => new[]{
                TestData.Create(2, 1, true),
                TestData.Create(1, 0, true),

                TestData.Create(1, 1, true),
                TestData.Create(0, 0, true),

                TestData.Create(1, 2, false),
                TestData.Create(0, 1, false),
            };

            public static TestData<int, int, bool>[] Equal => new[]{
                TestData.Create(2, 1, false),
                TestData.Create(1, 0, false),

                TestData.Create(1, 1, true),
                TestData.Create(0, 0, true),

                TestData.Create(1, 2, false),
                TestData.Create(0, 1, false),
            };

            public static TestData<int, int, bool>[] NotEqual => new[]{
                TestData.Create(2, 1, true),
                TestData.Create(1, 0, true),

                TestData.Create(1, 1, false),
                TestData.Create(0, 0, false),

                TestData.Create(1, 2, true),
                TestData.Create(0, 1, true),
            };

            public static TestData<int, int, bool>[] LessOrEqual => new[]{
                TestData.Create(2, 1, false),
                TestData.Create(1, 0, false),


                TestData.Create(1, 1, true),
                TestData.Create(0, 0, true),

                TestData.Create(1, 2, true),
                TestData.Create(0, 1, true),
            };

            public static TestData<int, int, bool>[] Less => new[]{
                TestData.Create(2, 1, false),
                TestData.Create(1, 0, false),


                TestData.Create(1, 1, false),
                TestData.Create(0, 0, false),

                TestData.Create(1, 2, true),
                TestData.Create(0, 1, true),
            };

        }
    }
}