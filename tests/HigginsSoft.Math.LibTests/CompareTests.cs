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
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests.GmpIntTests
{

    public abstract class CompareTests<T> : TestRunner<T>
    {
        [TestMethod]
        public void GmpInt_Compare_Greater()
        {
            var tests = ConvertTests(CompareGreaterData());
            RunTests(tests, TestNames.CompareGreater);
        }

        [TestMethod]
        public void GmpInt_Compare_Equal()
        {
            var tests = ConvertTests(CompareEqualData());
            RunTests(tests, TestNames.CompareEqual);
        }

        [TestMethod]
        public void GmpInt_Compare_Less()
        {
            var tests = ConvertTests(CompareLessData());
            RunTests(tests, TestNames.CompareLessThan);
        }

        [TestMethod]
        public void GmpInt_Greater()
        {
            var tests = ConvertTests(GreaterData());
            RunTests(tests, TestNames.Greater, op.Greater);
        }

        [TestMethod]
        public void GmpInt_GreaterOrEqual()
        {
            var tests = ConvertTests(GreaterOrEqualData());
            RunTests(tests, TestNames.GreaterOrEqual, op.GreaterOrEqual);
        }


        [TestMethod]
        public void GmpInt_Equal()
        {
            var tests = ConvertTests(EqualData());
            RunTests(tests, TestNames.Equal, op.Equal);
        }

        [TestMethod]
        public void GmpInt_NotEqual()
        {
            var tests = ConvertTests(NotEqualData());
            RunTests(tests, TestNames.Equal, op.NotEqual);
        }

        [TestMethod]
        public void GmpInt_LessOrEqual()
        {
            var tests = ConvertTests(LessOrEqualData());
            RunTests(tests, TestNames.LessOrEqual, op.LessOrEqual);
        }

        [TestMethod]
        public void GmpInt_Less()
        {
            var tests = ConvertTests(LessData());
            RunTests(tests, TestNames.Less, op.Less);
        }





        protected abstract TestData<int, int, int>[] CompareGreaterData();
        protected abstract TestData<int, int, int>[] CompareEqualData();
        protected abstract TestData<int, int, int>[] CompareLessData();
        protected abstract TestData<int, int, bool>[] GreaterData();
        protected abstract TestData<int, int, bool>[] GreaterOrEqualData();
        protected abstract TestData<int, int, bool>[] EqualData();
        protected abstract TestData<int, int, bool>[] NotEqualData();
        protected abstract TestData<int, int, bool>[] LessOrEqualData();
        protected abstract TestData<int, int, bool>[] LessData();
    }

    public abstract class CompareSignedTests<T> : CompareTests<T>
    {

        protected override TestData<int, int, int>[] CompareGreaterData()
            => CompareTestCases.Signed.CompareGreater;

        protected override TestData<int, int, int>[] CompareEqualData()
            => CompareTestCases.Signed.CompareEqual;

        protected override TestData<int, int, int>[] CompareLessData()
            => CompareTestCases.Signed.CompareLess;

        protected override TestData<int, int, bool>[] GreaterData()
            => CompareTestCases.Signed.Greater;

        protected override TestData<int, int, bool>[] GreaterOrEqualData()
             => CompareTestCases.Signed.GreaterOrEqual;

        protected override TestData<int, int, bool>[] EqualData()
            => CompareTestCases.Signed.Equal;

        protected override TestData<int, int, bool>[] NotEqualData()
            => CompareTestCases.Signed.NotEqual;

        protected override TestData<int, int, bool>[] LessOrEqualData()
            => CompareTestCases.Signed.LessOrEqual;

        protected override TestData<int, int, bool>[] LessData()
            => CompareTestCases.Signed.Less;
    }

    public abstract class CompareUnsignedTests<T> : CompareTests<T>
    {

        protected override TestData<int, int, int>[] CompareGreaterData()
            => CompareTestCases.Unsigned.CompareGreater;

        protected override TestData<int, int, int>[] CompareEqualData()
            => CompareTestCases.Unsigned.CompareEqual;

        protected override TestData<int, int, int>[] CompareLessData()
            => CompareTestCases.Unsigned.CompareLess;

        protected override TestData<int, int, bool>[] GreaterData()
            => CompareTestCases.Unsigned.Greater;

        protected override TestData<int, int, bool>[] GreaterOrEqualData()
             => CompareTestCases.Unsigned.GreaterOrEqual;

        protected override TestData<int, int, bool>[] EqualData()
            => CompareTestCases.Unsigned.Equal;

        protected override TestData<int, int, bool>[] NotEqualData()
            => CompareTestCases.Unsigned.NotEqual;

        protected override TestData<int, int, bool>[] LessOrEqualData()
            => CompareTestCases.Unsigned.LessOrEqual;

        protected override TestData<int, int, bool>[] LessData()
            => CompareTestCases.Unsigned.Less;
    }



    [TestClass()]
    public class CompareIntTestsGeneric : CompareSignedTests<int>
    {
        public CompareIntTestsGeneric()
        {
        }
    }

    [TestClass()]
    public class CompareUintTestsGeneric : CompareUnsignedTests<uint>
    {
        public CompareUintTestsGeneric()
        {
        }
    }

    [TestClass()]
    public class CompareLongTestsGeneric : CompareSignedTests<long>
    {
        public CompareLongTestsGeneric()
        {
        }
    }

    [TestClass()]
    public class CompareUlongTestsGeneric : CompareUnsignedTests<ulong>
    {
        public CompareUlongTestsGeneric()
        {
        }
    }

    [TestClass()]
    public class CompareBigIntTestsGeneric : CompareSignedTests<BigInteger>
    {
        public CompareBigIntTestsGeneric()
        {
        }
    }

    [TestClass()]
    public class CompareGmpIntTestsGeneric : CompareSignedTests<GmpInt>
    {
        public CompareGmpIntTestsGeneric()
        {

        }
    }


    [TestClass()]
    public class CompareLongTests
    {

        TestData<long, long, int> CreateTest(long a, long b, int expected)
            => TestData.Create(a, b, expected);

        TestData<long, long, bool> CreateTest(long a, long b, bool expected)
         => TestData.Create(a, b, expected);

        [TestMethod]
        public void GmpInt_Compare_Greater_Returns_1()
        {
            var tests = new[]{
                CreateTest(2L, 1L, 1),
                CreateTest(3L, 1L, 1),
                CreateTest(-1L, -2L, 1),
            };

            RunTests(tests, TestNames.CompareGreater);
        }

        [TestMethod]
        public void GmpInt_Compare_Equal_Returns_0()
        {
            var tests = new[]{
                CreateTest(1, 1,0 ),
                CreateTest(0, 0, 0),
                CreateTest(-1, -1, 0),
            };

            RunTests(tests, TestNames.CompareEqual);
        }


        [TestMethod]
        public void GmpInt_Compare_LessThan_Returns_0()
        {
            var tests = new[]{
                CreateTest(1, 2, -1),
                CreateTest(0, 1, -1),
                CreateTest(-1, 0, -1),
            };

            RunTests(tests, TestNames.CompareLessThan);
        }


        [TestMethod]
        public void GmpInt_Op_Greater()
        {
            var tests = new[]{
                CreateTest(2, 1, true),
                CreateTest(1, 0, true),
                CreateTest(0, -1, true),
                CreateTest(-1, -2, true),

                CreateTest(1, 1, false),
                CreateTest(0, 0, false),
                CreateTest(-1, -1, false),

                CreateTest(1, 2, false),
                CreateTest(0, 1, false),
                CreateTest(-1, 0, false),
                CreateTest(-2, -1, false),
            };

            RunTests(tests, TestNames.Greater, (a, b) => a > b);
        }

        [TestMethod]
        public void GmpInt_Op_GreaterOrEqual_Returns_True()
        {
            var tests = new[]{
                CreateTest(2, 1, true),
                CreateTest(1, 0, true),
                CreateTest(0, -1, true),
                CreateTest(-1, -2, true),

                CreateTest(1, 1, true),
                CreateTest(0, 0, true),
                CreateTest(-1, -1, true),

                CreateTest(1, 2, false),
                CreateTest(0, 1, false),
                CreateTest(-1, 0, false),
                CreateTest(-2, -1, false),
            };

            RunTests(tests, TestNames.GreaterOrEqual, (a, b) => a >= b);
        }

        [TestMethod]
        public void GmpInt_Op_Equal_Returns_True()
        {
            var tests = new[]{
                CreateTest(2, 1, false),
                CreateTest(1, 0, false),
                CreateTest(0, -1, false),
                CreateTest(-1, -2, false),

                CreateTest(1, 1, true),
                CreateTest(0, 0, true),
                CreateTest(-1, -1, true),

                CreateTest(1, 2, false),
                CreateTest(0, 1, false),
                CreateTest(-1, 0, false),
                CreateTest(-2, -1, false),
            };

            RunTests(tests, TestNames.Equal, (a, b) => a == b);
        }

        [TestMethod]
        public void GmpInt_Op_NotEqual_Returns_True()
        {
            var tests = new[]{
                CreateTest(2, 1, true),
                CreateTest(1, 0, true),
                CreateTest(0, -1, true),
                CreateTest(-1, -2, true),

                CreateTest(1, 1, false),
                CreateTest(0, 0, false),
                CreateTest(-1, -1, false),

                CreateTest(1, 2, true),
                CreateTest(0, 1, true),
                CreateTest(-1, 0, true),
                CreateTest(-2, -1, true),
            };

            RunTests(tests, TestNames.Equal, (a, b) => a != b);
        }

        [TestMethod]
        public void GmpInt_Op_LessOrEqual_Returns_True()
        {
            var tests = new[]{
                CreateTest(2, 1, false),
                CreateTest(1, 0, false),
                CreateTest(0, -1, false),
                CreateTest(-1, -2, false),

                CreateTest(1, 1, true),
                CreateTest(0, 0, true),
                CreateTest(-1, -1, true),

                CreateTest(1, 2, true),
                CreateTest(0, 1, true),
                CreateTest(-1, 0, true),
                CreateTest(-2, -1, true),
            };

            RunTests(tests, TestNames.LessOrEqual, (a, b) => a <= b);
        }


        [TestMethod]
        public void GmpInt_Op_Less_Returns_True()
        {
            var tests = new[]{
                CreateTest(2, 1, false),
                CreateTest(1, 0, false),
                CreateTest(0, -1, false),
                CreateTest(-1, -2, false),

                CreateTest(1, 1, false),
                CreateTest(0, 0, false),
                CreateTest(-1, -1, false),

                CreateTest(1, 2, true),
                CreateTest(0, 1, true),
                CreateTest(-1, 0, true),
                CreateTest(-2, -1, true),
            };

            RunTests(tests, TestNames.Less, (a, b) => a < b);
        }



        private static void RunTests(TestData<long, long, int>[] tests, string testName)
        {
            foreach (var data in tests)
            {
                GmpInt a = data.A;
                long b = data.B;
                int result = a.CompareTo(b);
                Assert.AreEqual(data.Expected, result, $"Compare<int> {testName} failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }
        private static void RunTests(TestData<long, long, bool>[] tests, string testName, Func<GmpInt, long, bool> comparer)
        {
            foreach (var data in tests)
            {
                GmpInt a = data.A;
                long b = data.B;
                bool result = comparer(a, b);
                Assert.AreEqual(data.Expected, result, $"Compare<int> {testName} failed for a:{a} b:{b} ({(GmpInt)b})");
            }
        }



    }
}