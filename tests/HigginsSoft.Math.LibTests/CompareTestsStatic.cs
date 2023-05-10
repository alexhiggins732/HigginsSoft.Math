/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HigginsSoft.Math.Lib.Tests.GmpIntTests
{

    [TestClass()]
    public class CompareIntTests
    {

        [TestMethod]
        public void GmpInt_Compare_Greater_Returns_1()
        {
            var tests = new[]{
                TestData.Create(2, 1, 1),
                TestData.Create(3, 1, 1),
                TestData.Create(-1, -2, 1),
            };

            RunTests(tests, TestNames.CompareGreater);
        }

        [TestMethod]
        public void GmpInt_Compare_Equal_Returns_0()
        {
            var tests = new[]{
                TestData.Create(1, 1,0 ),
                TestData.Create(0, 0, 0),
                TestData.Create(-1, -1, 0),
            };

            RunTests(tests, TestNames.CompareEqual);
        }

        [TestMethod]
        public void GmpInt_Compare_LessThan_Returns_0()
        {
            var tests = new[]{
                TestData.Create(1, 2, -1),
                TestData.Create(0, 1, -1),
                TestData.Create(-1, 0, -1),
            };

            RunTests(tests, TestNames.CompareLessThan);
        }

        [TestMethod]
        public void GmpInt_Op_Greater()
        {
            var tests = new[]{
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

            RunTests(tests, TestNames.Greater, (a, b) => a > b);
        }

        [TestMethod]
        public void GmpInt_Op_GreaterOrEqual_Returns_True()
        {
            var tests = new[]{
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

            RunTests(tests, TestNames.GreaterOrEqual, (a, b) => a >= b);
        }

        [TestMethod]
        public void GmpInt_Op_Equal_Returns_True()
        {
            var tests = new[]{
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

            RunTests(tests, TestNames.Equal, (a, b) => a == b);
        }

        [TestMethod]
        public void GmpInt_Op_NotEqual_Returns_True()
        {
            var tests = new[]{
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

            RunTests(tests, TestNames.Equal, (a, b) => a != b);
        }

        [TestMethod]
        public void GmpInt_Op_LessOrEqual_Returns_True()
        {
            var tests = new[]{
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

            RunTests(tests, TestNames.LessOrEqual, (a, b) => a <= b);
        }


        [TestMethod]
        public void GmpInt_Op_Less_Returns_True()
        {
            var tests = new[]{
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

            RunTests(tests, TestNames.Less, (a, b) => a < b);
        }



        private static void RunTests(TestData<int, int, int>[] tests, string testName)
        {
            foreach (var data in tests)
            {
                GmpInt a = data.A;
                int b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare<int> {testName} failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }
        private static void RunTests(TestData<int, int, bool>[] tests, string testName, Func<GmpInt, int, bool> comparer)
        {
            foreach (var data in tests)
            {
                GmpInt a = data.A;
                int b = data.B;
                bool result = comparer(a, b);
                Assert.AreEqual(data.Expected, result, $"Compare<int> {testName} failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }


    }

    [TestClass()]
    public class CompareUIntTests
    {
        [TestMethod]
        public void GmpInt_UintWhenGreater_Returns_1()
        {
            var tests = new[]{
                TestData.Create(2u, 1u, 1),
                TestData.Create(3u, 1u, 1),

            };

            foreach (var data in tests)
            {
                GmpInt a = data.A;
                uint b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare Greater failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }

    }

    [TestClass()]
    public class CompareUlongTests
    {
        [TestMethod]
        public void GmpInt_UlongWhenGreater_Returns_1()
        {
            var tests = new[]{
                TestData.Create(2ul, 1ul, 1),
                TestData.Create(3ul, 1ul, 1),
                TestData.Create(ulong.MaxValue, ulong.MaxValue-1, 1),
            };

            foreach (var data in tests)
            {
                GmpInt a = data.A;
                ulong b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare Greater failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }

    }

    [TestClass()]
    public class CompareFloatTests
    {
        const float precisions_delta = .9999999f;

        [TestMethod]
        public void GmpInt_FloatWhenGreater_Returns_1()
        {

            var tests = new[]{
                TestData.Create(2.0f, 1.0f, 1),
                TestData.Create(3.0f, 2.0f, 1),
                TestData.Create(float.MaxValue, float.MaxValue *precisions_delta, 1),
                TestData.Create(float.MinValue + (float.MaxValue-( float.MaxValue*precisions_delta)), float.MinValue, 1),
            };

            foreach (var data in tests)
            {
                GmpInt a = (GmpInt)data.A;
                float b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare Greater failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }
    }

    [TestClass()]
    public class CompareDoubleTests
    {
        const double precisions_delta = .99999999999999;

        [TestMethod]
        public void GmpInt_DoubleWhenGreater_Returns_1()
        {

            var tests = new[]{
                TestData.Create(2.0, 1.0, 1),
                TestData.Create(3.0, 2.0, 1),
                TestData.Create(double.MaxValue, double.MaxValue * precisions_delta,1),
                TestData.Create(double.MinValue + (double.MaxValue-( double.MaxValue*precisions_delta)), double.MinValue, 1),
            };

            foreach (var data in tests)
            {
                GmpInt a = (GmpInt)data.A;
                double b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare Greater failed for a:{a} b:{b} ({(GmpInt)b})"); ;
            }
        }
    }

    [TestClass()]
    public class CompareDecimalTests
    {
        const decimal precisions_delta = .999999999999999999999m;
        [TestMethod]
        public void GmpInt_DecimalWhenGreater_Returns_1()
        {

            var tests = new[]{
                TestData.Create(2.0m, 1.0m, 1),
                TestData.Create(3.0m, 2.0m, 1),
                TestData.Create(decimal.MaxValue, decimal.MaxValue * precisions_delta,1),
                TestData.Create(decimal.MinValue + (decimal.MaxValue-( decimal.MaxValue*precisions_delta)), decimal.MinValue, 1),
            };

            foreach (var data in tests)
            {
                GmpInt a = (GmpInt)data.A;
                decimal b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare Greater failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }

    }

}