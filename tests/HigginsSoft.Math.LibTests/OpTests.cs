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
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests
{

    public class OpTests<T>
    {
        private OpFactory<T> op;

        public OpTests()
        {
            op = OpFactory.GetOpFactory<T>();
        }

        [TestMethod()]
        public void Ops_AllTests()
        {

            var a = op.ConvertFromInt(1);
            var aZero = Ops<T>.ConvertFromInt(0);

            var aGmp = op.ToGmpInt(a);
            var aGmpMinusOne = GmpInt.MinusOne;
            Assert.IsTrue(op.Equal(aGmp, a));

            var aFromGmp = op.FromGmpInt(aGmp);
            Assert.IsTrue(op.Equal(aGmp, aFromGmp));

            var aInt = op.ConvertToInt(a);
            var aFromInt = op.ConvertFromInt(aInt);
            Assert.AreEqual(a, aFromInt);

            Assert.IsTrue(op.Compare(aGmp, a) == 0);

            var aGmpPlusOne = aGmp + 1;
            var aGmpMinus1 = aGmp - 1;

            Assert.IsTrue(op.Greater(aGmpPlusOne, a));
            Assert.IsFalse(op.Greater(aGmp, a));

            Assert.IsFalse(op.Greater(aGmpMinus1, a));

            Assert.IsTrue(op.GreaterOrEqual(aGmpPlusOne, a));
            Assert.IsTrue(op.GreaterOrEqual(aGmp, a));
            Assert.IsFalse(op.GreaterOrEqual(aGmpMinus1, a));

            Assert.IsFalse(op.Equal(aGmpPlusOne, a));
            Assert.IsTrue(op.Equal(aGmp, a));
            Assert.IsFalse(op.Equal(aGmpMinus1, a));

            Assert.IsTrue(op.NotEqual(aGmpPlusOne, a));
            Assert.IsFalse(op.NotEqual(aGmp, a));
            Assert.IsTrue(op.NotEqual(aGmpMinus1, a));

            Assert.IsFalse(op.LessOrEqual(aGmpPlusOne, a));
            Assert.IsTrue(op.LessOrEqual(aGmp, a));
            Assert.IsTrue(op.LessOrEqual(aGmpMinus1, a));

            Assert.IsFalse(op.Less(aGmpPlusOne, a));
            Assert.IsFalse(op.Less(aGmp, a));
            Assert.IsTrue(op.Less(aGmpMinus1, a));

            Assert.IsTrue(op.Add(aGmp, a) == aGmpPlusOne);
            Assert.IsTrue(op.Subtract(aGmp, a) == aGmpMinus1);
            Assert.IsTrue(op.Divide(aGmp, a) == aGmp / 1);
            Assert.IsTrue(op.Multiply(aGmp, a) == (aGmp * 1));
            Assert.IsTrue(op.Mod(aGmp, a) == (aGmp % 1));

            Assert.IsTrue(op.LeftShift(aGmp, a) == (aGmp << 1));

            Assert.IsTrue(op.RightShift(aGmp, a) == (aGmp >> 1));

            Assert.IsTrue(op.And(aGmp, a) == (aGmp & aGmp));
            Assert.IsTrue(op.Or(aGmp, a) == (aGmp | aGmp));
            Assert.IsTrue(op.Xor(aGmp, a) == (aGmp ^ aGmp));

            Assert.IsTrue(op.Gcd(aGmp, a) == MathUtil.Gcd(aGmp, aGmp));
            Assert.IsTrue(op.Gcd2(a, aGmp) == MathUtil.Gcd(aGmp, aGmp));

            Assert.IsTrue(op.ToGmpInt(op.GcdT(a, a)) == MathUtil.Gcd(aGmp, aGmp));

            Assert.IsTrue(op.GcdExt(aGmp, a).Gcd == MathUtil.GcdExt(aGmp, aGmp).Gcd);
            Assert.IsTrue(op.GcdExt2(a, aGmp).Gcd == MathUtil.GcdExt(aGmp, aGmp).Gcd);
            Assert.IsTrue(op.EqualT(op.GcdExtT(a, a).Gcd, MathUtil.GcdExt(a, a).Gcd));

            Assert.IsTrue(op.Abs(a) == op.ToGmpInt(op.AbsT(a)));


            Assert.IsTrue(op.Neg(a) == aGmpMinusOne);
            var right = op.ToGmpInt(op.NegT(a));
            Assert.IsTrue(right == aGmp || right == aGmpMinusOne);
            Assert.IsTrue(op.ToGmpInt(op.AbsT(a)) == +aGmp);

            Assert.IsTrue(op.EqualT(a, a));

            Assert.IsTrue(op.CompareT(a, a) == 0);

            var aPlusOne = op.AddT(a, op.ConvertFromInt(1));
            var aMinusOne = op.SubtractT(a, op.ConvertFromInt(1));

            Assert.IsTrue(op.GreaterT(aPlusOne, a));
            Assert.IsFalse(op.GreaterT(a, a));
            Assert.IsFalse(op.GreaterT(aMinusOne, a));

            Assert.IsTrue(op.GreaterOrEqualT(aPlusOne, a));
            Assert.IsTrue(op.GreaterOrEqualT(a, a));
            Assert.IsFalse(op.GreaterOrEqualT(aMinusOne, a));

            Assert.IsFalse(op.EqualT(aPlusOne, a));
            Assert.IsTrue(op.EqualT(a, a));
            Assert.IsFalse(op.EqualT(aMinusOne, a));

            Assert.IsTrue(op.NotEqualT(aPlusOne, a));
            Assert.IsFalse(op.NotEqualT(a, a));
            Assert.IsTrue(op.NotEqualT(aMinusOne, a));

            Assert.IsFalse(op.LessOrEqualT(aPlusOne, a));
            Assert.IsTrue(op.LessOrEqualT(a, a));
            Assert.IsTrue(op.LessOrEqualT(aMinusOne, a));

            Assert.IsFalse(op.LessT(aPlusOne, a));
            Assert.IsFalse(op.LessT(a, a));
            Assert.IsTrue(op.LessT(aMinusOne, a));


            Assert.IsTrue(op.EqualT(op.AddT(a, a), aPlusOne));
            Assert.IsTrue(op.EqualT(op.SubtractT(a, a), aMinusOne));
            Assert.IsTrue(op.EqualT(op.DivideT(a, a), a));
            Assert.IsTrue(op.EqualT(op.MultiplyT(a, a), a));
            Assert.IsTrue(op.EqualT(op.ModT(a, aPlusOne), a));


            Assert.IsTrue(op.EqualT(op.AndT(a, a), a));
            Assert.IsTrue(op.EqualT(op.OrT(a, a), a));
            Assert.IsTrue(op.EqualT(op.XorT(a, a), aZero));

        }

        [TestMethod()]
        public void Ops_T_AllTests()
        {

            var a = Ops<T>.ConvertFromInt(1);
            var aZero = Ops<T>.ConvertFromInt(0);

            var aGmp = Ops<T>.ToGmpInt(a);
            var aGmpMinusOne = GmpInt.MinusOne;
            Assert.IsTrue(Ops<T>.Equal(aGmp, a));

            var aFromGmp = Ops<T>.FromGmpInt(aGmp);
            Assert.IsTrue(Ops<T>.Equal(aGmp, aFromGmp));

            var aInt = Ops<T>.ConvertToInt(a);
            var aFromInt = Ops<T>.ConvertFromInt(aInt);
            Assert.AreEqual(a, aFromInt);

            Assert.IsTrue(Ops<T>.Compare(aGmp, a) == 0);

            var aGmpPlusOne = aGmp + 1;
            var aGmpMinus1 = aGmp - 1;

            Assert.IsTrue(Ops<T>.Greater(aGmpPlusOne, a));
            Assert.IsFalse(Ops<T>.Greater(aGmp, a));

            Assert.IsFalse(Ops<T>.Greater(aGmpMinus1, a));

            Assert.IsTrue(Ops<T>.GreaterOrEqual(aGmpPlusOne, a));
            Assert.IsTrue(Ops<T>.GreaterOrEqual(aGmp, a));
            Assert.IsFalse(Ops<T>.GreaterOrEqual(aGmpMinus1, a));

            Assert.IsFalse(Ops<T>.Equal(aGmpPlusOne, a));
            Assert.IsTrue(Ops<T>.Equal(aGmp, a));
            Assert.IsFalse(Ops<T>.Equal(aGmpMinus1, a));

            Assert.IsTrue(Ops<T>.NotEqual(aGmpPlusOne, a));
            Assert.IsFalse(Ops<T>.NotEqual(aGmp, a));
            Assert.IsTrue(Ops<T>.NotEqual(aGmpMinus1, a));

            Assert.IsFalse(Ops<T>.LessOrEqual(aGmpPlusOne, a));
            Assert.IsTrue(Ops<T>.LessOrEqual(aGmp, a));
            Assert.IsTrue(Ops<T>.LessOrEqual(aGmpMinus1, a));

            Assert.IsFalse(Ops<T>.Less(aGmpPlusOne, a));
            Assert.IsFalse(Ops<T>.Less(aGmp, a));
            Assert.IsTrue(Ops<T>.Less(aGmpMinus1, a));

            Assert.IsTrue(Ops<T>.Add(aGmp, a) == aGmpPlusOne);
            Assert.IsTrue(Ops<T>.Subtract(aGmp, a) == aGmpMinus1);
            Assert.IsTrue(Ops<T>.Divide(aGmp, a) == aGmp / 1);
            Assert.IsTrue(Ops<T>.Multiply(aGmp, a) == (aGmp * 1));
            Assert.IsTrue(Ops<T>.Mod(aGmp, a) == (aGmp % 1));

            Assert.IsTrue(Ops<T>.LeftShift(aGmp, a) == (aGmp << 1));
   
            Assert.IsTrue(Ops<T>.RightShift(aGmp, a) == (aGmp >> 1));

            Assert.IsTrue(Ops<T>.And(aGmp, a) == (aGmp & aGmp));
            Assert.IsTrue(Ops<T>.Or(aGmp, a) == (aGmp | aGmp));
            Assert.IsTrue(Ops<T>.Xor(aGmp, a) == (aGmp ^ aGmp));

            Assert.IsTrue(Ops<T>.Gcd(aGmp, a) == MathUtil.Gcd(aGmp, aGmp));
            Assert.IsTrue(Ops<T>.Gcd2(a, aGmp) == MathUtil.Gcd(aGmp, aGmp));
            Assert.IsTrue(Ops<T>.ToGmpInt(Ops<T>.GcdT(a, a)) == MathUtil.Gcd(aGmp, aGmp));

            Assert.IsTrue(Ops<T>.GcdExt(aGmp, a).Gcd == MathUtil.GcdExt(aGmp, aGmp).Gcd);
            Assert.IsTrue(Ops<T>.GcdExt2(a, aGmp).Gcd == MathUtil.GcdExt(aGmp, aGmp).Gcd);
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.GcdExtT(a, a).Gcd, MathUtil.GcdExt(a, a).Gcd));

            Assert.IsTrue(Ops<T>.Abs(a) == Ops<T>.ToGmpInt(Ops<T>.AbsT(a)));


            Assert.IsTrue(Ops<T>.Neg(a) == aGmpMinusOne);
            var right = Ops<T>.ToGmpInt(Ops<T>.NegT(a));
            Assert.IsTrue(right == aGmp || right == aGmpMinusOne);
            Assert.IsTrue(Ops<T>.ToGmpInt(Ops<T>.AbsT(a)) == +aGmp);

            Assert.IsTrue(Ops<T>.EqualT(a, a));

            Assert.IsTrue(Ops<T>.CompareT(a, a) == 0);

            var aPlusOne = Ops<T>.AddT(a, Ops<T>.ConvertFromInt(1));
            var aMinusOne = Ops<T>.SubtractT(a, Ops<T>.ConvertFromInt(1));

            Assert.IsTrue(Ops<T>.GreaterT(aPlusOne, a));
            Assert.IsFalse(Ops<T>.GreaterT(a, a));
            Assert.IsFalse(Ops<T>.GreaterT(aMinusOne, a));

            Assert.IsTrue(Ops<T>.GreaterOrEqualT(aPlusOne, a));
            Assert.IsTrue(Ops<T>.GreaterOrEqualT(a, a));
            Assert.IsFalse(Ops<T>.GreaterOrEqualT(aMinusOne, a));

            Assert.IsFalse(Ops<T>.EqualT(aPlusOne, a));
            Assert.IsTrue(Ops<T>.EqualT(a, a));
            Assert.IsFalse(Ops<T>.EqualT(aMinusOne, a));

            Assert.IsTrue(Ops<T>.NotEqualT(aPlusOne, a));
            Assert.IsFalse(Ops<T>.NotEqualT(a, a));
            Assert.IsTrue(Ops<T>.NotEqualT(aMinusOne, a));

            Assert.IsFalse(Ops<T>.LessOrEqualT(aPlusOne, a));
            Assert.IsTrue(Ops<T>.LessOrEqualT(a, a));
            Assert.IsTrue(Ops<T>.LessOrEqualT(aMinusOne, a));

            Assert.IsFalse(Ops<T>.LessT(aPlusOne, a));
            Assert.IsFalse(Ops<T>.LessT(a, a));
            Assert.IsTrue(Ops<T>.LessT(aMinusOne, a));


            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.AddT(a, a), aPlusOne));
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.SubtractT(a, a), aMinusOne));
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.DivideT(a, a), a));
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.MultiplyT(a, a), a));
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.ModT(a, aPlusOne), a));

            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.AndT(a, a), a));
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.OrT(a, a), a));
            Assert.IsTrue(Ops<T>.EqualT(Ops<T>.XorT(a, a), aZero));




        }

    }

    namespace a_OpTests
    {
        [TestClass()]
        public class OpFactory_Tests
        {
            [TestMethod]
            public void OpFactory_UnregisteredTypeTest()
            {
                Assert.ThrowsException<NotImplementedException>(() => OpFactory.GetOpFactory<DateTime>());

            }
        }
        [TestClass()] public class Ops_Int_Tests : OpTests<int> { }
        [TestClass()] public class Ops_Unt_Tests : OpTests<uint> { }
        [TestClass()] public class Ops_Long_Tests : OpTests<long> { }
        [TestClass()] public class Ops_Ulong_Tests : OpTests<ulong> { }
        [TestClass()] public class Ops_BigInteger_Tests : OpTests<BigInteger> { }
        [TestClass()] public class Ops_GmpInt_Tests : OpTests<GmpInt> { }
        [TestClass()] public class Ops_Float_Tests : OpTests<float> { }
        [TestClass()] public class Ops_Double_Tests : OpTests<double> { }
        [TestClass()] public class Ops_Decimal_Tests : OpTests<decimal> { }
    }
}