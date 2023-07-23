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
using MathGmp.Native;
using System.Security.Cryptography;

namespace HigginsSoft.Math.Lib.Tests.GmpIntTests
{


    [TestClass()]
    public class GmpIntTests
    {

        [TestMethod()]
        public void GmpMinMaxCastTests()
        {


            GmpInt a = 0;

            Assert.IsTrue((string)a == "0");
            a = "0";
            Assert.IsTrue((sbyte)a == 0);
            Assert.IsTrue((byte)a == 0);
            Assert.IsTrue((short)a == 0);
            Assert.IsTrue((ushort)a == 0);
            Assert.IsTrue((int)a == 0);
            Assert.IsTrue((long)a == 0L);
            Assert.IsTrue((ulong)a == 0UL);
            Assert.IsTrue((float)a == 0f);
            Assert.IsTrue((double)a == 0.0);
            Assert.IsTrue((decimal)a == (decimal)0);
            Assert.IsTrue((BigInteger)a == BigInteger.Zero);


            a = sbyte.MinValue;
            Assert.IsTrue(a == sbyte.MinValue);
            Assert.IsTrue((sbyte)a == sbyte.MinValue);

            a = sbyte.MaxValue;
            Assert.IsTrue(a == sbyte.MaxValue);
            Assert.IsTrue((sbyte)a == sbyte.MaxValue);



            a = byte.MinValue;
            Assert.IsTrue(a == byte.MinValue);
            Assert.IsTrue((byte)a == byte.MinValue);

            a = byte.MaxValue;
            Assert.IsTrue(a == byte.MaxValue);
            Assert.IsTrue((byte)a == byte.MaxValue);



            a = short.MinValue;
            Assert.IsTrue(a == short.MinValue);
            Assert.IsTrue((short)a == short.MinValue);

            a = short.MaxValue;
            Assert.IsTrue(a == short.MaxValue);
            Assert.IsTrue((short)a == short.MaxValue);



            a = ushort.MinValue;
            Assert.IsTrue(a == ushort.MinValue);
            Assert.IsTrue((ushort)a == ushort.MinValue);

            a = ushort.MaxValue;
            Assert.IsTrue(a == ushort.MaxValue);
            Assert.IsTrue((ushort)a == ushort.MaxValue);


            a = int.MinValue;
            Assert.IsTrue(a == int.MinValue);
            Assert.IsTrue((int)a == int.MinValue);

            a = int.MaxValue;
            Assert.IsTrue(a == int.MaxValue);
            Assert.IsTrue((int)a == int.MaxValue);



            a = uint.MinValue;
            Assert.IsTrue(a == uint.MinValue);
            Assert.IsTrue((uint)a == uint.MinValue);

            a = uint.MaxValue;
            Assert.IsTrue(a == uint.MaxValue);
            Assert.IsTrue((uint)a == uint.MaxValue);


            a = long.MinValue;
            Assert.IsTrue(a == long.MinValue);
            Assert.IsTrue((long)a == long.MinValue);

            a = long.MaxValue;
            Assert.IsTrue(a == long.MaxValue);
            Assert.IsTrue((long)a == long.MaxValue);


            a = ulong.MinValue;
            Assert.IsTrue(a == ulong.MinValue);
            Assert.IsTrue((ulong)a == ulong.MinValue);

            a = ulong.MaxValue;
            Assert.IsTrue(a == ulong.MaxValue);
            Assert.IsTrue((ulong)a == ulong.MaxValue);

            a = BigInteger.One;
            Assert.IsTrue(a == BigInteger.One);
            Assert.IsTrue((BigInteger)a == BigInteger.One);

            a = (GmpInt)1.0f;
            Assert.IsTrue(a == 1.0f);
            Assert.IsTrue((float)a == 1.0f);

            a = (GmpInt)1.0;
            Assert.IsTrue(a == 1.0);
            Assert.IsTrue((double)a == 1.0);

            a = (GmpInt)1.0m;
            Assert.IsTrue(a == 1.0m);
            Assert.IsTrue((decimal)a == 1.0m);

            var b = new GmpInt(a);
            Assert.IsTrue(a == b);

            b = new GmpInt(a.Data);
            Assert.IsTrue(a == b);

            b = new GmpInt((mpz_t)a);
            Assert.IsTrue(a == b);

            b += 1;
            Assert.IsTrue(a != b);

        }
        [TestMethod()]
        public void GmpIntTest()
        {

            Assert.IsTrue(GmpInt.MinusOne == -1);


            Assert.IsTrue(GmpInt.MinusOne == (long)-1);

            Assert.IsTrue(GmpInt.MinusOne == (float)-1);
            Assert.IsTrue(GmpInt.MinusOne == (double)-1);
            Assert.IsTrue(GmpInt.MinusOne == (decimal)-1);

            Assert.IsTrue(GmpInt.Zero == 0);
            Assert.IsTrue(GmpInt.Zero == 0);
            Assert.IsTrue(GmpInt.Zero == (uint)0);
            Assert.IsTrue(GmpInt.Zero == (long)0);
            Assert.IsTrue(GmpInt.Zero == (ulong)0);
            Assert.IsTrue(GmpInt.Zero == (float)0);
            Assert.IsTrue(GmpInt.Zero == (double)0);
            Assert.IsTrue(GmpInt.Zero == (decimal)0);
            Assert.IsTrue(GmpInt.Zero == BigInteger.Zero);


            Assert.IsTrue(GmpInt.One == 1);
            Assert.IsTrue(GmpInt.One == (uint)1);
            Assert.IsTrue(GmpInt.One == (long)1);
            Assert.IsTrue(GmpInt.One == (ulong)1);
            Assert.IsTrue(GmpInt.One == (float)1);
            Assert.IsTrue(GmpInt.One == (double)1);
            Assert.IsTrue(GmpInt.One == (decimal)1);
            Assert.IsTrue(GmpInt.One == (BigInteger)1);

        }

        [TestMethod]
        public void RsaTest()
        {
            GmpInt rsa;
            bool isPrp;
            rsa= MathLib.Rsa(65);
            isPrp = rsa.IsProbablePrime();
            Assert.AreEqual(false, isPrp);

            rsa = MathLib.Rsa(40);
            isPrp = rsa.IsProbablePrime();
            Assert.AreEqual(false, isPrp);

            rsa = MathLib.Rsa(36);
            isPrp = rsa.IsProbablePrime();
            Assert.AreEqual(false, isPrp);

        }


        [TestMethod()]
        public void GmpIntUlongTest()
        {
            GmpInt gmp = ulong.MaxValue;
            Assert.AreEqual<GmpInt>(ulong.MaxValue, gmp);
            Assert.IsTrue(gmp.ToString() == ulong.MaxValue.ToString());


            BigInteger big = ulong.MaxValue;
            Assert.IsTrue(gmp == big);

            GmpInt gmpSq = gmp * gmp;
            var bigSq = big * big;
            Assert.IsTrue(gmpSq == bigSq);
        }

        [TestMethod()]
        public void GmpIntLongTest()
        {
            GmpInt gmp = long.MaxValue;
            Assert.AreEqual<GmpInt>(long.MaxValue, gmp);
            Assert.IsTrue(gmp.ToString() == long.MaxValue.ToString());


            BigInteger big = long.MaxValue;
            Assert.IsTrue(gmp == big);

            GmpInt gmpSq = gmp * gmp;
            var bigSq = big * big;
            Assert.IsTrue(gmpSq == bigSq);

            GmpInt gmpNeg = long.MinValue;
            Assert.AreEqual<GmpInt>(long.MinValue, gmpNeg);
            Assert.IsTrue(gmpNeg.ToString() == long.MinValue.ToString());

            BigInteger bigMNeg = long.MinValue;
            Assert.AreEqual<GmpInt>(bigMNeg, gmpNeg);

            var gmpNegSq = gmpNeg * gmpNeg;
            var bigMNegSq = bigMNeg * bigMNeg;
            Assert.AreEqual<GmpInt>(bigMNegSq, gmpNegSq);
            Assert.IsTrue(bigMNegSq.ToString() == gmpNegSq.ToString());
        }


        [TestMethod()]
        public void GmpIntTest1()
        {
            GmpInt a = new();
            Assert.IsTrue(a.IsZero);

            Assert.IsTrue(GmpInt.NewMpz() == a);

            a = 1;
            BigInteger big = 1;
            Assert.IsTrue(a == big);
            var aFromBig = new GmpInt(big);
            Assert.IsTrue(a == aFromBig);

            Assert.IsTrue(a.IsOne);
            Assert.IsTrue(a.Sign == 1);
            Assert.IsTrue(a.IsOdd);

            Assert.IsFalse(a.IsZero);
            Assert.IsFalse(a.IsEven);


            a = 0;
            Assert.IsTrue(a.IsZero);
            Assert.IsTrue(a.IsEven);
            Assert.IsTrue(a.Sign == 0);

            Assert.IsFalse(a.IsOne);
            Assert.IsFalse(a.IsOdd);

            a = -1;

            Assert.IsFalse(a.IsOne);
            Assert.IsTrue(a.Sign == -1);
            Assert.IsTrue(a.IsOdd);

            Assert.IsFalse(a.IsZero);
            Assert.IsFalse(a.IsEven);

            a = 1;
            Assert.IsTrue(a << -1 == a >> 1);

            a = 1;
            Assert.IsTrue(a >> -1 == a << 1);

            var product = GmpInt.MinusOne * GmpInt.MinusThree * GmpInt.MinusFive * GmpInt.MinusTen;

            Assert.IsTrue(product == 150);

        }


        [TestMethod()]
        public void ToStringTest()
        {
            GmpInt a = 1;
            var s = a.ToString();

            Assert.IsTrue(s == "1");

            //Assert.IsTrue(GmpInt.Zero.ToString() == "{}");
            Assert.IsTrue(GmpInt.One.ToString() == "1");
            Assert.IsTrue(GmpInt.MinusOne.ToString() == "-1");
        }

        [TestMethod()]
        public void ToStringTest1()
        {
            GmpInt a = 15;
            var s = a.ToString(16);

            Assert.IsTrue(s == "f", $"expected: f, actual: {s}");

            a = 255;
            s = a.ToString(16);

            Assert.IsTrue(s == "ff", $"expected: ff, actual: {s}");

            a = (1 << 16) - 1;
            s = a.ToString(16);

            Assert.IsTrue(s == "ffff", $"expected: ffff, actual: {s}");



            a = GmpInt.MinusOne;
            s = a.ToString(16);

            Assert.IsTrue(s == "-1", $"expected: -1, actual: {s}");

            a = -15;
            s = a.ToString(16);

            Assert.IsTrue(s == "-f", $"expected: -f, actual: {s}");

            a = -((1 << 16) - 1);
            s = a.ToString(16);

            Assert.IsTrue(s == "-ffff", $"expected: -ffff, actual: {s}");
        }

        [TestMethod()]
        public void CloneTest()
        {
            GmpInt a = 1;
            var b = a.Clone();
            a = 2;
            Assert.IsTrue(b == 1);

            ICloneable cloneable = a;

            Assert.IsTrue((GmpInt)cloneable.Clone() == 2);

            a = 0;
            b = a & b;
            Assert.IsTrue(b == 0);

            a = 1;
            b = a & b;
            Assert.IsTrue(b == 0);


            b = a | b;
            Assert.IsTrue(b == a);

            a = 0;
            b = a | b;
            a = 1;
            Assert.IsTrue(b == a);

            b = a << 0;
            a = 2;
            Assert.IsTrue(a != b);

            a = 1;
            b = a >> 0;
            a = 2;
            Assert.IsTrue(a != b);

            a = 1;
            a = ~a;
            Assert.IsTrue(a == -2);

            a = +a;
            Assert.IsTrue(a == 2);

            a--;
            Assert.IsTrue(a == 1);

            a = -a;
            Assert.IsTrue(a == -1);

            a++;
            Assert.IsTrue(a == 0);

        }

        [TestMethod()]
        public void CompareToTest()
        {
            GmpInt a = 1;

            Assert.IsTrue(0 < a);
            Assert.IsTrue(0 <= a);
            Assert.IsFalse(0 == a);
            Assert.IsTrue(0 != a);
            Assert.IsFalse(0 > a);
            Assert.IsFalse(0 >= a);

            Assert.IsTrue(a > 0);
            Assert.IsTrue(a >= 0);
            Assert.IsFalse(a == 0);
            Assert.IsTrue(a != 0);
            Assert.IsFalse(a <= 0);
            Assert.IsFalse(a < 0);



            Assert.IsTrue(0U < a);
            Assert.IsTrue(0U <= a);
            Assert.IsFalse(0U == a);
            Assert.IsTrue(0U != a);
            Assert.IsFalse(0U > a);
            Assert.IsFalse(0U >= a);

            Assert.IsTrue(a > 0U);
            Assert.IsTrue(a >= 0U);
            Assert.IsFalse(a == 0U);
            Assert.IsTrue(a != 0U);
            Assert.IsFalse(a <= 0U);
            Assert.IsFalse(a < 0U);



            Assert.IsTrue(0L < a);
            Assert.IsTrue(0L <= a);
            Assert.IsFalse(0L == a);
            Assert.IsTrue(0L != a);
            Assert.IsFalse(0L > a);
            Assert.IsFalse(0L >= a);

            Assert.IsTrue(a > 0L);
            Assert.IsTrue(a >= 0L);
            Assert.IsFalse(a == 0L);
            Assert.IsTrue(a != 0L);
            Assert.IsFalse(a <= 0L);
            Assert.IsFalse(a < 0L);



            Assert.IsTrue(0UL < a);
            Assert.IsTrue(0UL <= a);
            Assert.IsFalse(0UL == a);
            Assert.IsTrue(0UL != a);
            Assert.IsFalse(0UL > a);
            Assert.IsFalse(0UL >= a);

            Assert.IsTrue(a > 0UL);
            Assert.IsTrue(a >= 0UL);
            Assert.IsFalse(a == 0UL);
            Assert.IsTrue(a != 0UL);
            Assert.IsFalse(a <= 0UL);
            Assert.IsFalse(a < 0UL);



            Assert.IsTrue(0.0 < a);
            Assert.IsTrue(0.0 <= a);
            Assert.IsFalse(0.0 == a);
            Assert.IsTrue(0.0 != a);
            Assert.IsFalse(0.0 > a);
            Assert.IsFalse(0.0 >= a);

            Assert.IsTrue(a > 0.0);
            Assert.IsTrue(a >= 0.0);
            Assert.IsFalse(a == 0.0);
            Assert.IsTrue(a != 0.0);
            Assert.IsFalse(a <= 0.0);
            Assert.IsFalse(a < 0.0);


            Assert.IsTrue(0.0M < a);
            Assert.IsTrue(0.0M <= a);
            Assert.IsFalse(0.0M == a);
            Assert.IsTrue(0.0M != a);
            Assert.IsFalse(0.0M > a);
            Assert.IsFalse(0.0M >= a);

            Assert.IsTrue(a > 0.0M);
            Assert.IsTrue(a >= 0.0M);
            Assert.IsFalse(a == 0.0M);
            Assert.IsTrue(a != 0.0M);
            Assert.IsFalse(a <= 0.0M);
            Assert.IsFalse(a < 0.0M);

            Assert.IsTrue(BigInteger.Zero < a);
            Assert.IsTrue(BigInteger.Zero <= a);
            Assert.IsFalse(BigInteger.Zero == a);
            Assert.IsTrue(BigInteger.Zero != a);
            Assert.IsFalse(BigInteger.Zero > a);
            Assert.IsFalse(BigInteger.Zero >= a);

            Assert.IsTrue(a > BigInteger.Zero);
            Assert.IsTrue(a >= BigInteger.Zero);
            Assert.IsFalse(a == BigInteger.Zero);
            Assert.IsTrue(a != BigInteger.Zero);
            Assert.IsFalse(a <= BigInteger.Zero);
            Assert.IsFalse(a < BigInteger.Zero);

            Assert.IsTrue(GmpInt.Zero < a);
            Assert.IsTrue(GmpInt.Zero <= a);
            Assert.IsFalse(GmpInt.Zero == a);
            Assert.IsTrue(GmpInt.Zero != a);
            Assert.IsFalse(GmpInt.Zero > a);
            Assert.IsFalse(GmpInt.Zero >= a);

            Assert.IsTrue(a > GmpInt.Zero);
            Assert.IsTrue(a >= GmpInt.Zero);
            Assert.IsFalse(a == GmpInt.Zero);
            Assert.IsTrue(a != GmpInt.Zero);
            Assert.IsFalse(a <= GmpInt.Zero);
            Assert.IsFalse(a < GmpInt.Zero);

        }



        [TestMethod()]
        public void EqualsTest()
        {
            GmpInt a = 1;
            Assert.IsFalse(a.Equals((object?)null));
            Assert.IsFalse(a.Equals(DateTime.MinValue));
            Assert.IsFalse(a.Equals(GmpInt.MinusOne));
            Assert.IsFalse(a.Equals((GmpInt)2));

            Assert.IsTrue(a.Equals(1));
            Assert.IsTrue(a.Equals(1u));
            Assert.IsTrue(a.Equals(1L));
            Assert.IsTrue(a.Equals(1UL));
            Assert.IsTrue(a.Equals(1.0));
        }


        [TestMethod()]
        public void BytesTest()
        {
            GmpInt a = 1;
            var bytes = a.Bytes();
            Assert.IsTrue(bytes.Length == 1);
            Assert.IsTrue(bytes[0] == 1);

            var data = a.RawData();
            Assert.IsTrue(data.Length == 1);
            Assert.IsTrue(data[0] == 1);

        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            GmpInt a = 1010101;
            var hashCode = a.GetHashCode();
            Assert.AreEqual(704770, hashCode);
        }



        [TestMethod()]
        public void DisposeTest()
        {
            GmpInt a = 1;
            a.Data.Dispose();
            a.Dispose();
            var s = a.ToString();
            //Assert.IsTrue(s == "uninitialized");
        }


        [TestMethod()]
        public void PopCountPowOfTwoTest()
        {
            for (var i = 1; i < 8; i++)
            {
                var mask = (1 << i);
                GmpInt a = mask;
                Assert.IsTrue((int)a.BitCount == i + 1);
                Assert.IsTrue(a.IsPowerOfTwo);
                Assert.IsTrue(a.PopCount == 1);
                a -= 1;
                Assert.IsTrue((int)a.BitCount == i);
                if (i > 1)
                    Assert.IsFalse(a.IsPowerOfTwo);
                Assert.IsTrue(a.PopCount == i);
            }


        }

        [TestMethod()]
        public void DigitCountTest()
        {
            for (var i = 1; i < 5; i++)
            {
                var pow = System.Math.Pow(10, i);

                GmpInt a = (int)pow;
                Assert.IsTrue((int)a.DigitCount == i + 1);

            }
        }


    }

}