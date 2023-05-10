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

namespace HigginsSoft.Math.Lib.Tests
{
    public class TestRunner<T>
    {
        protected OpFactory<T> op => OpFactory.GetOpFactory<T>();



        protected TestData<T, T, int> CreateTest(T a, T b, int expected)
           => TestData.Create(a, b, expected);

        protected TestData<T, T, bool> CreateTest(T a, T b, bool expected)
            => TestData.Create(a, b, expected);

        protected TestData<T, T, int> CreateTest(int a, int b, int expected)
          => CreateTest(op.ConvertFromInt(a), op.ConvertFromInt(b), expected);

        protected TestData<T, T, bool> CreateTest(int a, int b, bool expected)
            => CreateTest(op.ConvertFromInt(a), op.ConvertFromInt(b), expected);



        protected void RunTests(TestData<int, int, int>[] tests, string testName)
            => RunTests(ConvertTests(tests), testName);


        protected void RunTests(TestData<int, int, bool>[] tests, string testName, Func<GmpInt, T, bool> comparer)
            => RunTests(ConvertTests(tests), testName, comparer);

        protected void RunTests(TestData<int, int, int>[] tests, string testName, Func<GmpInt, T, GmpInt> binaryFunc)
        {
            var testData = ConvertTests(tests);
            RunTests(testData, testName, binaryFunc);
        }


        protected void RunTests(TestData<T, T, int>[] tests, string testName)
        {
            foreach (var data in tests)
            {
                GmpInt a = op.ToGmpInt(data.A);
                int result = op.Compare(a, data.B);

                Assert.AreEqual(data.Expected, result,
                    $"Compare<int> {testName} failed for a:{a} b:{data.B} ({op.ToGmpInt(data.B)})");
            }
        }


        protected void RunTestsAsT(TestData<T, T, T>[] tests, string testName, Func<T, T, T> binaryFunc)
        {
            foreach (var data in tests)
            {
                var result = binaryFunc(data.A, data.B);

                Assert.AreEqual(data.Expected, result,
                    $"Compare<int> {testName} failed for a:{data.A} b:{data.B} expected: ({data.Expected})");
            }
        }

        protected void RunTests(TestData<T, T, int>[] tests, string testName, Func<GmpInt, T, GmpInt> binaryFunc)
        {
            foreach (var data in tests)
            {
                GmpInt a = op.ToGmpInt(data.A);
                Assert.IsTrue(op.Equal(a, op.FromGmpInt(a)));

                var result = (int)binaryFunc(a, data.B);

                Assert.AreEqual(data.Expected, result,
                    $"Compare<int> {testName} failed for a:{data.A} b:{data.B} expected: ({data.Expected})");
            }
        }

        protected void RunTests2(TestData<T, T, int>[] tests, string testName, Func<T, GmpInt, GmpInt> binaryFunc)
        {
            foreach (var data in tests)
            {
                GmpInt a = op.ToGmpInt(data.A);
                Assert.IsTrue(op.Equal(a, op.FromGmpInt(a)));

                var result = (int)binaryFunc(data.B, a);

                Assert.AreEqual(data.Expected, result,
                    $"Compare<int> {testName} failed for a:{data.A} b:{data.B} expected: ({data.Expected})");
            }
        }

        protected void RunTests(TestData<T, T, bool>[] tests, string testName, Func<GmpInt, T, bool> comparer)
        {
            foreach (var data in tests)
            {
                GmpInt a = op.ToGmpInt(data.A);

                bool result = comparer(a, data.B);
                Assert.AreEqual(data.Expected, result,
                    $"Compare<int> {testName} failed for a:{a} b:{data.B} ({op.ToGmpInt(data.B)})");
            }
        }




        protected TestData<T, T, int>[] ConvertTests(TestData<int, int, int>[] testData)
        {
            var result = testData.ToList()
                .Select(x => CreateTest(op.ConvertFromInt(x.A), op.ConvertFromInt(x.B), x.Expected))
                .ToArray();
            return result;
        }

        protected TestData<T, T, bool>[] ConvertTests(TestData<int, int, bool>[] testData)
        {
            var result = testData.ToList()
                .Select(x => CreateTest(op.ConvertFromInt(x.A), op.ConvertFromInt(x.B), x.Expected))
                .ToArray();
            return result;
        }
    }
}