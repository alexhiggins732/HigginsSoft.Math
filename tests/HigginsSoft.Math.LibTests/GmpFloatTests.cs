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

namespace HigginsSoft.Math.Lib.Tests.GmpFloatTests
{

    [TestClass()]
    public class GmpFloatTests
    {
        [TestMethod()]
        public void GmpFloat_CtorTest()
        {
            var a = new GmpFloat();
            var f = a.Data;
            var s = a.ToString();
            Assert.IsTrue(a.Sign == 0);
            Assert.IsTrue(a.IsZero);
            Assert.IsFalse(a.IsOne);
            Assert.IsTrue(s == "0");
            a.Dispose();
            a.Data.Dispose();
            s = a.ToString();
            Assert.IsTrue(s == "uninitialized");

            a = 1;
            Assert.IsTrue(a.Sign == 1);
            Assert.IsFalse(a.IsZero);
            Assert.IsTrue(a.IsOne);
            s = a.ToString();
            Assert.IsTrue(s == "1");

            var b = new GmpFloat(a.Data);
            Assert.IsTrue((string)b == "1");

            Assert.IsTrue(((mpf_t)b).ToString() == b.Data.ToString());

        }

        [TestMethod()]
        public void GmpMinMaxCastTests()
        {
            GmpFloat a = 0;
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
            var aint = (int)a;
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

            a = (GmpFloat)1.0f;
            Assert.IsTrue(a == 1.0f);
            Assert.IsTrue((float)a == 1.0f);

            a = (GmpFloat)1.0;
            Assert.IsTrue(a == 1.0);
            Assert.IsTrue((double)a == 1.0);

            a = (GmpFloat)1.0m;
            Assert.IsTrue(a == 1.0m);
            Assert.IsTrue((decimal)a == 1.0m);

            var b = new GmpFloat(a);
            Assert.IsTrue(a == b);

            b = new GmpFloat(a.Data);
            Assert.IsTrue(a == b);

            b = new GmpFloat((mpf_t)a);
            Assert.IsTrue(a == b);

            b += 1;
            Assert.IsTrue(a != b);

            a = b;
            a += 0;
            b = 0 + a;
            Assert.IsTrue(a == b);

            a = new GmpFloat(b.Data);
            Assert.IsTrue(a == b);

            a = 10;
            var aS = a.Data.ToString();
            Assert.IsTrue(aS == "0.1e2");
            aS = a.ToString();
            Assert.IsTrue(aS == "10");

            b = a / 5;
            Assert.IsTrue(b == 2);


            // division is exact with a float, no remainder apparently.
            b = a % 3;
            Assert.IsTrue(b == 0);

        }
        [TestMethod()]
        public void GmpFloatTest()
        {

            Assert.IsTrue(GmpFloat.MinusOne == -1);


            Assert.IsTrue(GmpFloat.MinusOne == (long)-1);

            Assert.IsTrue(GmpFloat.MinusOne == (float)-1);
            Assert.IsTrue(GmpFloat.MinusOne == (double)-1);
            Assert.IsTrue(GmpFloat.MinusOne == (decimal)-1);

            Assert.IsTrue(GmpFloat.Zero == 0);
            Assert.IsTrue(GmpFloat.Zero == 0);
            Assert.IsTrue(GmpFloat.Zero == (uint)0);
            Assert.IsTrue(GmpFloat.Zero == (long)0);
            Assert.IsTrue(GmpFloat.Zero == (ulong)0);
            Assert.IsTrue(GmpFloat.Zero == (float)0);
            Assert.IsTrue(GmpFloat.Zero == (double)0);
            Assert.IsTrue(GmpFloat.Zero == (decimal)0);
            Assert.IsTrue(GmpFloat.Zero == BigInteger.Zero);


            Assert.IsTrue(GmpFloat.One == 1);
            Assert.IsTrue(GmpFloat.One == (uint)1);
            Assert.IsTrue(GmpFloat.One == (long)1);
            Assert.IsTrue(GmpFloat.One == (ulong)1);
            Assert.IsTrue(GmpFloat.One == (float)1);
            Assert.IsTrue(GmpFloat.One == (double)1);
            Assert.IsTrue(GmpFloat.One == (decimal)1);
            Assert.IsTrue(GmpFloat.One == (BigInteger)1);
            ;
        }

        [TestMethod()]
        public void GmpFloatUlongTest()
        {
            GmpFloat gmp = ulong.MaxValue;
            Assert.AreEqual<GmpFloat>(ulong.MaxValue, gmp);
            Assert.IsTrue(gmp.ToString() == ulong.MaxValue.ToString());


            BigInteger big = ulong.MaxValue;
            Assert.IsTrue(gmp == big);

            GmpFloat gmpSq = gmp * gmp;
            var bigSq = big * big;
            Assert.IsTrue(gmpSq == bigSq);

            gmpSq = 0 * gmp;
            Assert.IsTrue(gmpSq.IsZero);
            gmpSq = 1;
            gmpSq = gmpSq * 0;
            Assert.IsTrue(gmpSq.IsZero);
        }

        [TestMethod()]
        public void GmpFloatLongTest()
        {

            GmpFloat gmp = new GmpFloat(long.MaxValue, 128);

            Assert.AreEqual<GmpFloat>(long.MaxValue, gmp);
            Assert.IsTrue(gmp.ToString() == long.MaxValue.ToString());
            var a = GmpFloat.One;

            BigInteger big = long.MaxValue;
            Assert.IsTrue(gmp == big);

            GmpFloat gmpSq = gmp * gmp;
            var bigSq = big * big;
            Assert.IsTrue(gmpSq == bigSq);

            GmpFloat gmpNeg = long.MinValue;
            Assert.AreEqual<GmpFloat>(long.MinValue, gmpNeg);
            Assert.IsTrue(gmpNeg.ToString() == long.MinValue.ToString());

            BigInteger bigMNeg = long.MinValue;
            Assert.AreEqual<GmpFloat>(bigMNeg, gmpNeg);

            var gmpNegSq = gmpNeg * gmpNeg;


            var bigMNegSq = bigMNeg * bigMNeg;
            Assert.AreEqual<GmpFloat>(bigMNegSq, gmpNegSq);
            Assert.IsTrue(bigMNegSq.ToString() == ((GmpInt)gmpNegSq).ToString());

            gmp = 1;
            Assert.IsTrue((gmp ^ gmp) == GmpFloat.Zero);
        }


        [TestMethod()]
        public void GmpFloatTest1()
        {
            GmpFloat a = 1;
            BigInteger big = 1;
            Assert.IsTrue(a == big);
            var aFromBig = new GmpFloat(big);
            Assert.IsTrue(a == aFromBig);

            a = "1";
            Assert.IsTrue(a.IsOne);

            var aFromMpf = new GmpFloat(a.Data);
            Assert.IsTrue(a == aFromMpf);

            aFromMpf = a.Data;
            Assert.IsTrue(a == aFromMpf);

            Assert.IsTrue(a.IsOne);
            Assert.IsTrue(a.Sign == 1);
            Assert.IsTrue(((GmpInt)a).IsOdd);

            Assert.IsFalse(a.IsZero);
            Assert.IsFalse(((GmpInt)a).IsEven);


            a = 0;
            Assert.IsTrue(a.IsZero);
            Assert.IsTrue(((GmpInt)a).IsEven);
            Assert.IsTrue(a.Sign == 0);

            Assert.IsFalse(a.IsOne);
            Assert.IsFalse(((GmpInt)a).IsOdd);

            a = -1;

            Assert.IsFalse(a.IsOne);
            Assert.IsTrue(a.Sign == -1);
            Assert.IsTrue(((GmpInt)a).IsOdd);

            Assert.IsFalse(a.IsZero);
            Assert.IsFalse(((GmpInt)a).IsEven);

            a = 1;
            Assert.IsTrue(a << -1 == a >> 1);

            a = 1;
            Assert.IsTrue(a >> -1 == a << 1);
        }


        [TestMethod()]
        public void ToStringTest()
        {


            GmpFloat a = 1;
            var s = a.ToString();

            Assert.IsTrue(s == "1");

            //Assert.IsTrue(GmpFloat.Zero.ToString() == "{}");
            Assert.IsTrue(GmpFloat.One.ToString() == "1");
            Assert.IsTrue(GmpFloat.MinusOne.ToString() == "-1");

        }

        [TestMethod()]
        public void ToStringTest1()
        {
            GmpFloat a = 15;
            var s = a.ToString(16);

            Assert.IsTrue(s == "f", $"expected: f, actual: {s}");

            a = 255;
            s = a.ToString(16);

            Assert.IsTrue(s == "ff", $"expected: ff, actual: {s}");

            a = (1 << 16) - 1;
            s = a.ToString(16);

            Assert.IsTrue(s == "ffff", $"expected: ffff, actual: {s}");



            a = GmpFloat.MinusOne;
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
        public void ToStringTest2()
        {
            double maxError = 0.001;
            double[] expectedValues = { 0.01, 0.1, 1, 1.1, 10.01, 100.001 };
            expectedValues = expectedValues.Concat(expectedValues.Select(x => -x)).ToArray();
            foreach (var expected in expectedValues)
            {
                GmpFloat value = (GmpFloat)expected;
                var valueAsD = (double)value;
                Assert.AreEqual(expected, valueAsD, $"Comp failed for {expected} - Actual: {valueAsD}");

                var expectedStr = expected.ToString();
                var actualStr = valueAsD.ToString();
                Assert.AreEqual(expectedStr, actualStr, $"Comp failed for {expectedStr} - Actual: {actualStr}");
                var actualToStrFull = value.ToString();
                var actualToStr = actualToStrFull.Substring(0, expectedStr.Length);
               
                
                if (expectedStr != actualToStr)
                {
                    var parsed = double.Parse(actualToStrFull);
                    var diff = MathLib.Abs(valueAsD - parsed);
                    if (diff> maxError)
                     Assert.AreEqual(expectedStr, actualToStr, $"Comp failed for {expectedStr} - Actual: {actualToStr}");
                }
            

            }
        }

        [TestMethod()]
        public void CloneTest()
        {
            GmpFloat a = 1;
            var b = a.Clone();
            a = 2;
            Assert.IsTrue(b == 1);



            a = 0;
            b = 0;
            b = a & b;
            Assert.IsTrue(b == 0);

            a = 0;
            b = 1;
            b = a & b;
            Assert.IsTrue(b == 0);

            a = 1;
            b = 0;
            b = a & b;
            Assert.IsTrue(b == 0);

            a = 1;
            b = 1;
            b = a & b;
            Assert.IsTrue(b == 1);


            a = 0;
            b = 0;
            b = a | b;
            Assert.IsTrue(b == 0);

            a = 0;
            b = 1;
            b = a | b;
            Assert.IsTrue(b == 1);

            a = 1;
            b = 0;
            b = a | b;
            Assert.IsTrue(b == 1);

            a = 1;
            b = 1;
            b = a | b;
            Assert.IsTrue(b == 1);



            a = 0;
            b = 0;
            b = a | b;
            Assert.IsTrue(b == 0);

            a = 0;
            b = 1;
            b = a | b;
            Assert.IsTrue(b == 1);

            a = 1;
            b = 0;
            b = a | b;
            Assert.IsTrue(b == 1);

            a = 1;
            b = 1;
            b = a | b;
            Assert.IsTrue(b == 1);



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

            b = a + a;
            Assert.IsTrue(b == 4);

            a--;
            Assert.IsTrue(a == 1);

            b = a - a;
            Assert.IsTrue(b == 0);


            a = -a;
            Assert.IsTrue(a == -1);

            a++;
            Assert.IsTrue(a == 0);

            a = 1;
            b = a;
            b -= 0;
            a = 0 - a;
            Assert.IsTrue(b == 1);
            Assert.IsTrue(a == -1);

            GmpFloat.SetDefaultPrecision((uint)a.Data._mp_prec);
        }

        [TestMethod()]
        public void CompareToTest()
        {
            GmpFloat a = 1;

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

            Assert.IsTrue(GmpFloat.Zero < a);
            Assert.IsTrue(GmpFloat.Zero <= a);
            Assert.IsFalse(GmpFloat.Zero == a);
            Assert.IsTrue(GmpFloat.Zero != a);
            Assert.IsFalse(GmpFloat.Zero > a);
            Assert.IsFalse(GmpFloat.Zero >= a);

            Assert.IsTrue(a > GmpFloat.Zero);
            Assert.IsTrue(a >= GmpFloat.Zero);
            Assert.IsFalse(a == GmpFloat.Zero);
            Assert.IsTrue(a != GmpFloat.Zero);
            Assert.IsFalse(a <= GmpFloat.Zero);
            Assert.IsFalse(a < GmpFloat.Zero);

        }



        [TestMethod()]
        public void EqualsTest()
        {
            GmpFloat a = 1;
            Assert.IsFalse(a.Equals((object?)null));
            Assert.IsFalse(a.Equals(DateTime.MinValue));
            Assert.IsFalse(a.Equals(GmpFloat.MinusOne));
            Assert.IsFalse(a.Equals((GmpFloat)2));

            Assert.IsTrue(a.Equals(1));
            Assert.IsTrue(a.Equals(1u));
            Assert.IsTrue(a.Equals(1L));
            Assert.IsTrue(a.Equals(1UL));
            Assert.IsTrue(a.Equals(1.0));
        }


        [TestMethod()]
        public void BytesTest()
        {
            GmpFloat a = 1;
            var bytes = a.Bytes();
            Assert.IsTrue(bytes.Length == 1);
            Assert.IsTrue(bytes[0] == 1);

            var data = a.RawData();
            Assert.IsTrue(data.Length == 1);
            Assert.IsTrue(data[0] == 1ul);

        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            GmpFloat a = 1010101;
            var hashCode = a.GetHashCode();
            Assert.AreEqual(704770, hashCode);
        }



        [TestMethod()]
        public void DisposeTest()
        {
            GmpFloat a = 1;
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
                GmpFloat aa = mask;
                GmpInt a = (GmpInt)aa;
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

                GmpFloat a = (int)pow;
                Assert.IsTrue((int)((GmpInt)a).DigitCount == i + 1);

            }
        }

        [TestMethod]
        public void AlgebraicPolyEvalTest()
        {
            decimal maxError = 0.00000001m;
            BigInteger a = 1;
            BigInteger b = 3;
            int[] poly = { 62, 10, 1, 2 };

            //Algebraic(BigInteger a, BigInteger b, Polynomial poly)


            decimal aD = (decimal)a;
            decimal bD = (decimal)b;
            decimal ab = (-aD) / bD;


            var aG = (GmpFloat)a;
            var bG = (GmpFloat)b;
            var abG = (-aG) / bG;

            int num = poly.Length - 1;
            decimal num2 = (decimal)poly[num];
            var num2g = (GmpFloat)poly[num];
            while (--num >= 0)
            {
                num2 *= (decimal)ab;
                num2g *= abG;

                var num2gAsD= (decimal)num2g;
                if (num2gAsD != num2)
                {
                    var diff = MathLib.Abs(num2 - num2gAsD);
                    if (diff > maxError)
                    {
                        Assert.AreEqual(num2, num2gAsD);
                    }
                }


                num2 += (decimal)poly[num];
                num2g += (GmpFloat)poly[num];
                num2gAsD = (decimal)num2g;
                if (num2gAsD != num2)
                {
                    var diff = MathLib.Abs(num2 - num2gAsD);
                    if (diff > maxError)
                    {
                        Assert.AreEqual(num2, num2gAsD);
                    }
                }

            }

            decimal leftd = num2;
            BigInteger rightd = BigInteger.Pow(BigInteger.Negate(b), poly.Length);
            var productB = leftd * (decimal)rightd;
            var productBRounded = MathLib.Round(productB);
            BigInteger resultb = (BigInteger)productBRounded;



            var leftg = num2g;
            var rightg = GmpInt.Power(GmpInt.Negate(b), poly.Length);
            var productg = leftg * (GmpFloat)rightg;
            var resultg = MathLib.Round(productg);

            var gAsD = (decimal)resultg;
            Assert.AreEqual(productBRounded, gAsD);

            var resultz = (GmpInt)resultg;
            var zAsB = (BigInteger)resultz;

            Assert.AreEqual(resultb, zAsB);



        }
        [TestMethod]
        public void AlgebraicPolyEval2Test()
        {
            //{9487735613*X^3 + 4481689*X^2 + 2117*X + 253201175967347219055173370331573192760}
            BigInteger a = 1;
            BigInteger b = 3;
            BigInteger[] poly = {
                BigInteger.Parse("253201175967347219055173370331573192760"),
                2117,
                4481689,
                9487735613
            };

            //Algebraic(BigInteger a, BigInteger b, Polynomial poly)


            decimal aD = (decimal)a;
            decimal bD = (decimal)b;
            decimal ab = (-aD) / bD;


            var aG = (GmpFloat)a;
            var bG = (GmpFloat)b;
            var abG = (-aG) / bG;

            int num = poly.Length - 1;
            decimal num2 = (decimal)poly[num];
            var num2g = (GmpFloat)poly[num];
            while (--num >= 0)
            {
                num2 *= (decimal)ab;
                num2g *= abG;

                var num2AsG = (decimal)num2g;
                Assert.AreEqual(num2, num2AsG);

                num2 += (decimal)poly[num];

                num2g += (GmpFloat)poly[num];

                var num2gAsD = (decimal)num2g;
                Assert.AreEqual(num2, num2gAsD);
            }

            decimal leftd = num2;
            BigInteger rightd = BigInteger.Pow(BigInteger.Negate(b), poly.Length);
            var productB = leftd * leftd;
            var productBRounded = MathLib.Round(productB);
            BigInteger resultb = (BigInteger)productBRounded;



            var leftg = num2;
            var rightg = GmpInt.Power(GmpInt.Negate(b), poly.Length);
            var productg = leftd * leftd;
            var resultg = MathLib.Round(productg);

            var gAsD = (decimal)resultg;
            Assert.AreEqual(productBRounded, gAsD);

            var resultz = (GmpInt)resultg;
            var zAsB = (BigInteger)resultz;

            Assert.AreEqual(resultb, zAsB);



        }


    }

}