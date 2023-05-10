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
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests.MathUtilTests
{
    public abstract class MathUtil_Lcm_Tests<T> : TestRunner<T>
    {

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsLessThanB()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(3);
            T b = Ops<T>.ConvertFromInt(5);
            T expected = Ops<T>.ConvertFromInt(15);
            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsGreaterThanB()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(6);
            T b = Ops<T>.ConvertFromInt(4);
            T expected = Ops<T>.ConvertFromInt(12);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAEqualsB()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(4);
            T b = Ops<T>.ConvertFromInt(4);
            T expected = Ops<T>.ConvertFromInt(4);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsZero()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(0);
            T b = Ops<T>.ConvertFromInt(3);
            T expected = Ops<T>.ConvertFromInt(0);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenBIsZero()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(6);
            T b = Ops<T>.ConvertFromInt(0);
            T expected = Ops<T>.ConvertFromInt(0);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_SharedMultiples_ReturnsCorrectResult()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(6);
            T b = Ops<T>.ConvertFromInt(15);
            T expected = Ops<T>.ConvertFromInt(30);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_SharedMultiples_ReverseOrder_ReturnsCorrectResult()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(15);
            T b = Ops<T>.ConvertFromInt(6);
            T expected = Ops<T>.ConvertFromInt(30);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_LargeSharedMultiples_ReturnsCorrectResult()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(12);
            T b = Ops<T>.ConvertFromInt(18);
            T expected = Ops<T>.ConvertFromInt(36);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_LargeSharedMultiples_ReverseOrder_ReturnsCorrectResult()
        {
            // Arrange
            T a = Ops<T>.ConvertFromInt(18);
            T b = Ops<T>.ConvertFromInt(12);
            T expected = Ops<T>.ConvertFromInt(36);

            // Act
            T result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenAGreaterThanBAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            T a = Ops<T>.ConvertFromInt(24);
            T b = Ops<T>.ConvertFromInt(18);
            T expected = Ops<T>.ConvertFromInt(72);

            T result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenBGreaterThanAAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            T a = Ops<T>.ConvertFromInt(18);
            T b = Ops<T>.ConvertFromInt(24);
            T expected = Ops<T>.ConvertFromInt(72);

            T result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenAEqualsBAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            T a = Ops<T>.ConvertFromInt(30);
            T b = Ops<T>.ConvertFromInt(30);
            T expected = Ops<T>.ConvertFromInt(30);
            T result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }


    }






    [TestClass] public class MathUtil_Lcm_Int_GenericTests : MathUtil_Lcm_Tests<int> { }
    [TestClass] public class MathUtil_Lcm_Uint_GenericTests : MathUtil_Lcm_Tests<uint> { }
    [TestClass] public class MathUtil_Lcm_Long_GenericTests : MathUtil_Lcm_Tests<long> { }
    [TestClass] public class MathUtil_Lcm_Ulong_GenericTests : MathUtil_Lcm_Tests<ulong> { }
    [TestClass] public class MathUtil_Lcm_Float_GenericTests : MathUtil_Lcm_Tests<float> { }
    [TestClass] public class MathUtil_Lcm_Double_GenericTests : MathUtil_Lcm_Tests<double> { }
    [TestClass] public class MathUtil_Lcm_Decimal_GenericTests : MathUtil_Lcm_Tests<decimal> { }
    [TestClass] public class MathUtil_Lcm_BigInteger_GenericTests : MathUtil_Lcm_Tests<BigInteger> { }
    [TestClass] public class MathUtil_Lcm_GmpInt_GenericTests : MathUtil_Lcm_Tests<GmpInt> { }


    [TestClass]
    public class MathUtil_Lcm_AllTypeTests
    {
        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsLessThanB()
        {
            int a = 3;
            int b = 5;
            int expected = 15;


            // Assert
            Assert.AreEqual(expected, MathUtil.Lcm(a, b));
            Assert.AreEqual((uint)expected, MathUtil.Lcm((uint)a, (uint)b));
            Assert.AreEqual((long)expected, MathUtil.Lcm((long)a, (long)b));
            Assert.AreEqual((ulong)expected, MathUtil.Lcm((ulong)a, (ulong)b));
            Assert.AreEqual((float)expected, MathUtil.Lcm((float)a, (float)b));
            Assert.AreEqual((double)expected, MathUtil.Lcm((double)a, (double)b));
            Assert.AreEqual((decimal)expected, MathUtil.Lcm((decimal)a, (decimal)b));
            Assert.AreEqual((BigInteger)expected, MathUtil.Lcm((BigInteger)a, (BigInteger)b));
            Assert.AreEqual((GmpInt)expected, MathUtil.Lcm((GmpInt)a, (GmpInt)b));

            GmpInt aGmp = a;
            GmpInt aExpected = expected;

            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (uint)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (long)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (ulong)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (float)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (double)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (decimal)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (BigInteger)b));
            Assert.AreEqual(aExpected, MathUtil.Lcm(aGmp, (GmpInt)b));

            GmpInt bGmp = b;


            Assert.AreEqual(aExpected, MathUtil.Lcm(a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((uint)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((long)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((ulong)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((float)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((double)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((decimal)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((BigInteger)a, bGmp));
            Assert.AreEqual(aExpected, MathUtil.Lcm((GmpInt)a, bGmp));


        }
    }

    [TestClass]
    public class MathUtil_Lcm_IntTests
    {
        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsLessThanB()
        {
            // Arrange
            int a = 3;
            int b = 5;

            // Act
            int result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsGreaterThanB()
        {
            // Arrange
            int a = 6;
            int b = 4;

            // Act
            int result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAEqualsB()
        {
            // Arrange
            int a = 4;
            int b = 4;

            // Act
            int result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsZero()
        {
            // Arrange
            int a = 0;
            int b = 3;

            // Act
            int result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenBIsZero()
        {
            // Arrange
            int a = 6;
            int b = 0;

            // Act
            int result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Lcm_SharedMultiples_ReturnsCorrectResult()
        {
            // Arrange
            int a = 6;
            int b = 15;

            // Act
            int lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(30, lcm);
        }

        [TestMethod]
        public void Lcm_SharedMultiples_ReverseOrder_ReturnsCorrectResult()
        {
            // Arrange
            int a = 15;
            int b = 6;

            // Act
            int lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(30, lcm);
        }

        [TestMethod]
        public void Lcm_LargeSharedMultiples_ReturnsCorrectResult()
        {
            // Arrange
            int a = 12;
            int b = 18;

            // Act
            int lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(36, lcm);
        }

        [TestMethod]
        public void Lcm_LargeSharedMultiples_ReverseOrder_ReturnsCorrectResult()
        {
            // Arrange
            int a = 18;
            int b = 12;

            // Act
            int lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(36, lcm);
        }

        [TestMethod]
        public void Lcm_WhenAGreaterThanBAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            int a = 24;
            int b = 18;
            int expected = 72;
            int result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenBGreaterThanAAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            int a = 18;
            int b = 24;
            int expected = 72;
            int result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenAEqualsBAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            int a = 30;
            int b = 30;
            int expected = 30;
            int result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }


    }

    [TestClass]
    public class MathUtil_Lcm_GmpIntTests
    {
        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsLessThanB()
        {
            // Arrange
            GmpInt a = 3;
            GmpInt b = 5;

            // Act
            GmpInt result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsGreaterThanB()
        {
            // Arrange
            GmpInt a = 6;
            GmpInt b = 4;

            // Act
            GmpInt result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAEqualsB()
        {
            // Arrange
            GmpInt a = 4;
            GmpInt b = 4;

            // Act
            GmpInt result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenAIsZero()
        {
            // Arrange
            GmpInt a = 0;
            GmpInt b = 3;

            // Act
            GmpInt result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Lcm_ReturnsCorrectResult_WhenBIsZero()
        {
            // Arrange
            GmpInt a = 6;
            GmpInt b = 0;

            // Act
            GmpInt result = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Lcm_SharedMultiples_ReturnsCorrectResult()
        {
            // Arrange
            GmpInt a = 6;
            GmpInt b = 15;

            // Act
            GmpInt lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(30, lcm);
        }

        [TestMethod]
        public void Lcm_SharedMultiples_ReverseOrder_ReturnsCorrectResult()
        {
            // Arrange
            GmpInt a = 15;
            GmpInt b = 6;

            // Act
            GmpInt lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(30, lcm);
        }

        [TestMethod]
        public void Lcm_LargeSharedMultiples_ReturnsCorrectResult()
        {
            // Arrange
            GmpInt a = 12;
            GmpInt b = 18;

            // Act
            GmpInt lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(36, lcm);
        }

        [TestMethod]
        public void Lcm_LargeSharedMultiples_ReverseOrder_ReturnsCorrectResult()
        {
            // Arrange
            GmpInt a = 18;
            GmpInt b = 12;

            // Act
            GmpInt lcm = MathUtil.Lcm(a, b);

            // Assert
            Assert.AreEqual(36, lcm);
        }

        [TestMethod]
        public void Lcm_WhenAGreaterThanBAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            GmpInt a = 24;
            GmpInt b = 18;
            GmpInt expected = 72;
            GmpInt result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenBGreaterThanAAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            GmpInt a = 18;
            GmpInt b = 24;
            GmpInt expected = 72;
            GmpInt result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Lcm_WhenAEqualsBAndTheyHaveCommonGcd_ReturnsCorrectResult()
        {
            GmpInt a = 30;
            GmpInt b = 30;
            GmpInt expected = 30;
            GmpInt result = MathUtil.Lcm(a, b);
            Assert.AreEqual(expected, result);
        }


    }

}