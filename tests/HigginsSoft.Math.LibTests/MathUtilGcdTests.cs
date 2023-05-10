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



    public abstract class MathUtil_GcdGenericTests<T> : TestRunner<T>
    {
        //Define Tests

        [TestMethod]
        public void Gcd_WhenAEqualsB()
        {
            var tests = ConvertTests(AEqualsB());
            RunTests(tests, nameof(Gcd_WhenAEqualsB), op.Gcd);
            RunTests2(tests, nameof(Gcd_WhenAEqualsB), op.Gcd2);
        }

        [TestMethod]
        public void Gcd_NoCommonFactors()
        {
            var tests = ConvertTests(NoCommonFactors());
            RunTests(tests, nameof(Gcd_NoCommonFactors), op.Gcd);
            RunTests2(tests, nameof(Gcd_WhenAEqualsB), op.Gcd2);
        }


        [TestMethod]
        public void Gcd_ADividesB()
        {
            var tests = ConvertTests(ADividesB());
            RunTests(tests, nameof(Gcd_ADividesB), op.Gcd);
            RunTests2(tests, nameof(Gcd_WhenAEqualsB), op.Gcd2);
        }

        [TestMethod]
        public void Gcd_BDividesA()
        {
            var tests = ConvertTests(BDividesA());
            RunTests(tests, nameof(BDividesA), op.Gcd);
            RunTests2(tests, nameof(Gcd_WhenAEqualsB), op.Gcd2);
        }

        [TestMethod]
        public void Gcd_CommonSmallerFactor()
        {
            var tests = ConvertTests(CommonSmallerFactor());
            RunTests(tests, nameof(CommonSmallerFactor), op.Gcd);
            RunTests2(tests, nameof(Gcd_WhenAEqualsB), op.Gcd2);
        }

        [TestMethod]
        public void Gcd_TestAsT()
        {
            var tests = ConvertTests(GetAllTests());
            tests.ForEach(x =>
            {
                var result = op.GcdT(x.A, x.B);
                var expectedFromInt = op.ConvertFromInt(x.Expected);
                Assert.AreEqual(expectedFromInt, result);
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


    public class MathUtil_Gcd_Tests<T> : MathUtil_GcdGenericTests<T>
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


    [TestClass] public class MathUtil_Gcd_IntTests : MathUtil_Gcd_Tests<int> { }

    [TestClass] public class MathUtil_Gcd_LongTests : MathUtil_Gcd_Tests<long> { }

    [TestClass] public class MathUtil_Gcd_UintTests : MathUtil_Gcd_Tests<uint> { }

    [TestClass] public class MathUtil_Gcd_UlongTests : MathUtil_Gcd_Tests<ulong> { }

    [TestClass] public class MathUtil_Gcd_BigIntegerTests : MathUtil_Gcd_Tests<BigInteger> { }

    [TestClass] public class MathUtil_Gcd_GmpIntTests : MathUtil_Gcd_Tests<GmpInt> { }

    [TestClass] public class MathUtil_Gcd_FloatTests : MathUtil_Gcd_Tests<float> { }
    [TestClass] public class MathUtil_Gcd_DoubleTests : MathUtil_Gcd_Tests<double> { }
    [TestClass] public class MathUtil_Gcd_DecimalTests : MathUtil_Gcd_Tests<decimal> { }

}
