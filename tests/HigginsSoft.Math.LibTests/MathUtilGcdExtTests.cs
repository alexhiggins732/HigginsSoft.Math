/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using HigginsSoft.Math.Lib.Tests.GmpIntTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HigginsSoft.Math.Lib.Tests.MathUtilTests
{
    public class GcdTestData
    {
        public int A;
        public int B;
        public int S;
        public int T;
        public int Expected;


        public GcdTestData(int a, int b, int s, int t, int expected)
        {
            A = a;
            B = b;
            S = s;
            T = t;
            Expected = expected;
        }

        public static GcdTestData Create(int a, int b, int s, int t, int expected)
            => new GcdTestData(a, b, s, t, expected);
        public static GcdTestData<TData> Create<TData>(int a, int b, int s, int t, int expected, Func<int, TData> converter)
            => new GcdTestData<TData>(converter(a), converter(b), converter(s), converter(t), converter(expected));
    }

    public class GcdTestData<TData>
    {
        public TData A;
        public TData B;
        public TData S;
        public TData T;
        public TData Expected;


        public GcdTestData(TData a, TData b, TData s, TData t, TData expected)
        {
            A = a;
            B = b;
            S = s;
            T = t;
            Expected = expected;
        }


    }

    public abstract class MathUtil_GcdExtGenericTests<T> : TestRunner<T>
    {



        [TestMethod()]
        public void GcdExt_ReturnsGcd_WhenAEqualsB()
        {
            var data = GcdTestData.Create(12, 12, 0, 1, 12);




            // Arrange
            int a = 12;
            int b = 12;

            // Act
            var result = MathUtil.GcdExtInt(a, b);

            // Assert
            Assert.AreEqual(data.Expected, result.Gcd);
            Assert.AreEqual(data.S, result.S);
            Assert.AreEqual(data.T, result.T);
        }



        [TestMethod()]
        public void GcdExt_GmpInt_ReturnsGcd_WhenAEqualsB()
        {
            var data = GcdTestData.Create(12, 12, 0, 1, 12, x => (GmpInt)x);
            // Arrange
            GmpInt a = 12;
            GmpInt b = 12;
            GmpInt s = 0;
            GmpInt t = 0;
            GmpInt gcd = 0;
            // Act

            var result = MathUtil.GcdExt(gcd, a, b, s, t);

            // Assert
            Assert.AreEqual(gcd, result.Gcd);
            Assert.AreEqual(data.S, result.S);
            Assert.AreEqual(data.T, result.T);

            //verify |s × a + t × b|
            var sum = +(s * a + t * b);
            Assert.AreEqual(result.Gcd, sum);

        }

        [TestMethod()]
        public void GcdExt_T_ReturnsGcd_WhenAEqualsB()
        {
            var data = GcdTestData.Create(12, 12, 0, 1, 12, x => op.ConvertFromInt(x));
            // Arrange

            // Act

            var result = MathUtil.GcdExt(data.A, data.B);

            // Assert
            Assert.AreEqual(data.Expected, result.Gcd);
            Assert.AreEqual(data.S, result.S);
            Assert.AreEqual(data.T, result.T);

            //verify |s × a + t × b|
            var sum = op.AbsT(op.AddT(op.MultiplyT(result.S, result.A), op.MultiplyT(result.T, result.B)));
            Assert.AreEqual(result.Gcd, sum);

        }


        [TestMethod()]
        public void GcdExt_GmpInt_ReturnsGcd_WhenAGreaterThanB()
        {
            // Arrange
            GmpInt a = 24;
            GmpInt b = 12;
            GmpInt s = 0;
            GmpInt t = 0;
            GmpInt result = 0;
            // Act

            MathUtil.GcdExt(result, a, b, s, t);

            // Assert
            Assert.AreEqual(12, (int)result);
            Assert.AreEqual(0, (int)s);
            Assert.AreEqual(1, (int)t);

            //verify |s × a + t × b|
            var sum = +(s * a + t * b);
            Assert.AreEqual(12, (int)result);

        }

        [TestMethod()]
        public void GcdExt_GmpInt_ReturnsGcd_WhenALessThanB()
        {
            // Arrange
            GmpInt a = 12;
            GmpInt b = 24;
            GmpInt s = 0;
            GmpInt t = 0;
            GmpInt result = 0;
            // Act

            MathUtil.GcdExt(result, a, b, s, t);

            // Assert
            Assert.AreEqual(12, (int)result);
            Assert.AreEqual(1, (int)s);
            Assert.AreEqual(0, (int)t);

            //verify |s × a + t × b|
            var sum = +(s * a + t * b);
            Assert.AreEqual(12, (int)result);

        }



        [TestMethod()]
        public void GcdExt_ReturnsGcd_WhenAAndBGcdIs1()
        {
            // Arrange
            GmpInt a = 15;
            GmpInt b = 32;
            GmpInt s = 0;
            GmpInt t = 0;

            // Act
            GmpInt result = 0;
            MathUtil.GcdExt(result, a, b, s, t);

            // Assert
            Assert.AreEqual(1, result);

            Assert.AreEqual(15, (int)s);
            Assert.AreEqual(-7, (int)t);

            //verify |s × a + t × b|
            var sum = +(s * a + t * b);
            Assert.AreEqual(1, (int)result);
        }

        [TestMethod()]
        public void GcdExt_ReturnsGcdWithModInv_WhenAAndBGcdIs1()
        {
            // Arrange
            GmpInt a = 11;
            GmpInt b = 26;
            GmpInt s = 0;
            GmpInt t = 0;

            // Act
            GmpInt result = 0;
            MathUtil.GcdExt(result, a, b, s, t);

            // Assert
            Assert.AreEqual(1, result);

            Assert.AreEqual(-7, (int)s);
            Assert.AreEqual(3, (int)t);

            //verify |s × a + t × b|
            var sum = +(s * a + t * b);
            Assert.AreEqual(1, (int)result);
        }



        [TestMethod]
        public void GcdExt_TestAsT()
        {
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal))
                return;

            var tests = ConvertTests(GetAllTests());
            tests.ForEach(x =>
            {
                var result = op.GcdExtT(x.A, x.B);
                var expectedFromInt = op.ConvertFromInt(x.Expected);

                Assert.AreEqual(expectedFromInt, result.Gcd, $"Test failed for GCDExt({x.A}, {x.B})");
            });

        }


        TestData<int, int, int>[] GetAllTests()
        {
            var result = new List<TestData<int, int, int>[]>
            {
                AEqualsB(),
                NoCommonFactors(),
                ADividesB(),
                BDividesA(),
                CommonSmallerFactor(),
            };
            return result.SelectMany(x => x).ToArray();
        }

        //Define Data

        protected abstract TestData<int, int, int>[] AEqualsB();
        protected abstract TestData<int, int, int>[] NoCommonFactors();
        protected abstract TestData<int, int, int>[] ADividesB();
        protected abstract TestData<int, int, int>[] BDividesA();
        protected abstract TestData<int, int, int>[] CommonSmallerFactor();

    }


    public class MathUtil_GcdExt_Tests<T> : MathUtil_GcdExtGenericTests<T>
    {
        protected override TestData<int, int, int>[] AEqualsB() => new[] {
            TestData.Create(12,12,12)
        };

        protected override TestData<int, int, int>[] NoCommonFactors() => new[] {
            TestData.Create(5,7,1),
            TestData.Create(11,13,1),
        };

        protected override TestData<int, int, int>[] ADividesB() => new[] {
            TestData.Create(3,12,3),
            TestData.Create(5,15,5)
        };

        protected override TestData<int, int, int>[] BDividesA() => new[] {
            TestData.Create(12,3,3),
            TestData.Create(15,5,5)
        };
        protected override TestData<int, int, int>[] CommonSmallerFactor() => new[] {
            TestData.Create(4,6,2),
            TestData.Create(15,21,3)
        };

    }


    // Define implementations

    [TestClass] public class MathUtil_GcdExt_IntTests : MathUtil_GcdExt_Tests<int> { }

    [TestClass] public class MathUtil_GcdExt_LongTests : MathUtil_GcdExt_Tests<long> { }

    [TestClass] public class MathUtil_GcdExt_UintTests : MathUtil_GcdExt_Tests<uint> { }

    [TestClass] public class MathUtil_GcdExt_UlongTests : MathUtil_GcdExt_Tests<ulong> { }

    [TestClass] public class MathUtil_GcdExt_BigIntegerTests : MathUtil_GcdExt_Tests<BigInteger> { }

    [TestClass] public class MathUtil_GcdExt_GmpIntTests : MathUtil_GcdExt_Tests<GmpInt> { }

    [TestClass] public class MathUtil_GcdExt_FloatTests : MathUtil_GcdExt_Tests<float> { }
    [TestClass] public class MathUtil_GcdExt_DoubleTests : MathUtil_GcdExt_Tests<double> { }
    [TestClass] public class MathUtil_GcdExt_DecimalTests : MathUtil_GcdExt_Tests<decimal> { }

}


