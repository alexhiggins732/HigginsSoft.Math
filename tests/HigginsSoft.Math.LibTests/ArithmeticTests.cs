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

namespace HigginsSoft.Math.Lib.Tests.GmpIntTests
{
    public abstract class ArithmeticTests<T> : TestRunner<T>
    {
        //Define Tests

        [TestMethod]
        public void GmpInt_Add()
        {
            var tests = ConvertTests(Add());
            RunTests(tests, TestNames.Add, op.Add);
        }

        [TestMethod]
        public void GmpInt_Subtract()
        {
            var tests = ConvertTests(Subtract());
            RunTests(tests, TestNames.Subtract, op.Subtract);
        }

        [TestMethod]
        public void GmpInt_Multiply()
        {
            var tests = ConvertTests(Multiply());
            RunTests(tests, TestNames.Multiply, op.Multiply);
        }

        [TestMethod]
        public void GmpInt_Divide()
        {
            var tests = ConvertTests(Divide());
            RunTests(tests, TestNames.Divide, op.Divide);
        }

        [TestMethod]
        public void GmpInt_Modulo()
        {
            var tests = ConvertTests(Modulo());
            RunTests(tests, TestNames.Modulo, op.Mod);
        }

        [TestMethod]
        public void GmpInt_And()
        {
            var tests = ConvertTests(And());
            RunTests(tests, TestNames.And, op.And);
        }

        [TestMethod]
        public void GmpInt_Or()
        {
            var tests = ConvertTests(Or());
            RunTests(tests, TestNames.Or, op.Or);
        }

        [TestMethod]
        public void GmpInt_Xor()
        {
            var tests = ConvertTests(Xor());
            RunTests(tests, TestNames.Xor, op.Xor);
        }

        [TestMethod]
        public void GmpInt_RightShift()
        {
            var tests = ConvertTests(RightShift());
            RunTests(tests, TestNames.RightShift, op.RightShift);
        }

        [TestMethod]
        public void GmpInt_LeftShift()
        {
            var tests = ConvertTests(LeftShift());
            RunTests(tests, TestNames.LeftShift, op.LeftShift);
        }
        
        //Define Test Data
        protected abstract TestData<int, int, int>[] Add();
        protected abstract TestData<int, int, int>[] Subtract();
        protected abstract TestData<int, int, int>[] Multiply();
        protected abstract TestData<int, int, int>[] Divide();
        protected abstract TestData<int, int, int>[] Modulo();
        protected abstract TestData<int, int, int>[] And();
        protected abstract TestData<int, int, int>[] Or();
        protected abstract TestData<int, int, int>[] Xor();
        protected abstract TestData<int, int, int>[] LeftShift();
        protected abstract TestData<int, int, int>[] RightShift();
    }


    public class ArithmeticSignedTests<T> : ArithmeticTests<T>
    {
        protected override TestData<int, int, int>[] Add()
            => ArithmeticTestCases.Signed.Add;

        protected override TestData<int, int, int>[] Divide()
            => ArithmeticTestCases.Signed.Divide;

        protected override TestData<int, int, int>[] Modulo()
            => ArithmeticTestCases.Signed.Modulo;

        protected override TestData<int, int, int>[] Multiply()
            => ArithmeticTestCases.Signed.Multiply;

        protected override TestData<int, int, int>[] Subtract()
            => ArithmeticTestCases.Signed.Subtract;

        protected override TestData<int, int, int>[] And()
            => ArithmeticTestCases.Signed.And;

        protected override TestData<int, int, int>[] Or()
            => ArithmeticTestCases.Signed.Or;

        protected override TestData<int, int, int>[] Xor()
            => ArithmeticTestCases.Signed.Xor;

        protected override TestData<int, int, int>[] RightShift()
           => ArithmeticTestCases.Signed.RightShift;

        protected override TestData<int, int, int>[] LeftShift()
            => ArithmeticTestCases.Signed.LeftShift;
    }

    public class ArithmeticUnsignedTests<T> : ArithmeticTests<T>
    {
        protected override TestData<int, int, int>[] Add()
            => ArithmeticTestCases.Unsigned.Add;

        protected override TestData<int, int, int>[] Divide()
            => ArithmeticTestCases.Unsigned.Divide;

        protected override TestData<int, int, int>[] Modulo()
            => ArithmeticTestCases.Unsigned.Modulo;

        protected override TestData<int, int, int>[] Multiply()
            => ArithmeticTestCases.Unsigned.Multiply;

        protected override TestData<int, int, int>[] Subtract()
            => ArithmeticTestCases.Unsigned.Subtract;

        protected override TestData<int, int, int>[] And()
            => ArithmeticTestCases.Unsigned.And;

        protected override TestData<int, int, int>[] Or()
            => ArithmeticTestCases.Unsigned.Or;

        protected override TestData<int, int, int>[] Xor()
            => ArithmeticTestCases.Unsigned.Xor;
        protected override TestData<int, int, int>[] RightShift()
           => ArithmeticTestCases.Unsigned.RightShift;

        protected override TestData<int, int, int>[] LeftShift()
            => ArithmeticTestCases.Unsigned.LeftShift;

    }

    [TestClass]
    public class ArithmeticIntTests : ArithmeticSignedTests<int>
    {
        
    }

    [TestClass]
    public class ArithmeticLongTests : ArithmeticSignedTests<long>
    {

    }

    [TestClass]
    public class ArithmeticBigIntegerTests : ArithmeticSignedTests<BigInteger>
    {

    }

    [TestClass]
    public class GmpIntArithmeticTests : ArithmeticSignedTests<GmpInt>
    {

    }

    [TestClass]
    public class ArithmeticUintTests : ArithmeticUnsignedTests<uint>
    {

    }

    [TestClass]
    public class ArithmeticUlongTests : ArithmeticUnsignedTests<ulong>
    {

    }
}