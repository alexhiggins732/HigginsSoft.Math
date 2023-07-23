using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace HigginsSoft.Math.Lib.Tests
{

    [TestClass()]
    public class pzTests
    {

        [TestMethod]
        public unsafe void pzCtorTest()
        {
            var pza = new pz(1);
            var actual = pza[0];
            var expected = 1ul;
            Assert.AreEqual(expected, actual);

            pza = new pz(-1);
            expected = ulong.MaxValue;
            actual = pza[0];
            Assert.AreEqual(expected, actual);

            pza.Abs();
            actual = pza[0];
            expected = 1ul;
            Assert.AreEqual(expected, actual);

            pza.Negate();
            actual = pza[0];
            expected = ulong.MaxValue;
            Assert.AreEqual(expected, actual);


        }

        [TestMethod]
        public unsafe void AddPositivePzBufferTest()
        {
            var dataA = new ulong[] { 0, 1, 2, 3, };
            var dataB = new ulong[] { 0, 1, 2, 3, };
            var dataC = new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, };
            var pza = new pz(dataA);
            var pzb = new pz(dataB);
            var pzc = new pz(dataC);

            pza.Add(pzb, 0, pzc, 0, 1);

            var expected = 0;
            var zActual = (int)pzc;
            Assert.AreEqual(expected, zActual);


            pza.Add(pzb, 0, pzc, 0, 2);

            ulong[] expectedData = { 0, 2 };
            var pzExpected = new pz(expectedData);
            int cmp = pzc.CompareTo(pzExpected);

            Assert.AreEqual(expected, cmp);

            pza.Add(pzb, 2, pzc, 2, 2);
            expectedData = new ulong[] { 0, 2, 4, 6 };
            pzExpected = new pz(expectedData);
            cmp = pzc.CompareTo(pzExpected);
            Assert.AreEqual(expected, cmp);

        }

        [TestMethod]
        public unsafe void AddSubOverflowTests()
        {
            var dataA = new ulong[] { ulong.MaxValue };

            var pza = new pz(dataA);

            // add 1+ulong.maxvalue, unsigned and assert sign=0
            pza.AddOneUnsigned(false);
            var expected = 0ul;
            var actual = pza[0];
            Assert.AreEqual(expected, actual);
            var expectedSign = 0;
            var actualSign = pza.Sign;
            Assert.AreEqual(expectedSign, actualSign);

            // subtract 0-1, unsigned and assert sign=0
            pza.SubtractOneUnsigned(false);
            expected = ulong.MaxValue;
            actual = pza[0];
            Assert.AreEqual(expected, actual);
            expectedSign = 0;
            actualSign = pza.Sign;
            Assert.AreEqual(expectedSign, actualSign);


            // add 1+ulong.maxvalue, signed and assert sign=1
            pza.AddOneSigned(false);
            expected = 0ul;
            actual = pza[0];
            Assert.AreEqual(expected, actual);
            expectedSign = 0;
            actualSign = pza.Sign;
            Assert.AreEqual(expectedSign, actualSign);


            // subtract 0-1, unsigned and assert sign=1
            pza.SubtractOneSigned(false);
            expected = ulong.MaxValue;
            actual = pza[0];
            Assert.AreEqual(expected, actual);
            expectedSign = 1;
            actualSign = pza.Sign;
            Assert.AreEqual(expectedSign, actualSign);

        }


        [TestMethod]
        public unsafe void AddPositiveWithOverflowPzBufferTest()
        {
            var dataA = new ulong[] { ulong.MaxValue, ulong.MaxValue, 2, 3, };
            var dataB = new ulong[] { ulong.MaxValue, ulong.MaxValue, 2, 3, };
            var dataC = new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, };
            var pza = new pz(dataA);
            var pzb = new pz(dataB);
            var pzc = new pz(dataC);

            pza.Add(pzb, 0, pzc, 0, 1);


            var zActual = pzc[0];
            var expected = 4294967294;
            Assert.AreEqual(expected, zActual);
            zActual = pzc[1];
            expected = 0;
            Assert.AreEqual(expected, zActual);

            zActual = pzc[2];
            expected = 1;
            Assert.AreEqual(expected, zActual);

            pza.Add(pzb, 0, pzc, 0, 2);

            //00000000 00000003 00000000 FFFFFFFD 00000000 FFFFFFFE
            ulong[] expectedData = {0xFFFFFFFE, 0xFFFFFFFD, 3 };
            var pzExpected = new pz(expectedData);
            int cmp = pzc.CompareTo(pzExpected);

            expected = 0;// should be same regardless of leading zeros
            Assert.AreEqual((int)expected, cmp);

            pza.Add(pzb, 2, pzc, 2, 2);
            // 00000004 00000000 FFFFFFFD 00000000 FFFFFFFE

            expectedData = new ulong[] { 0xFFFFFFFE, 0xFFFFFFFD, 4, 6 };
            pzExpected = new pz(expectedData);
            cmp = pzc.CompareTo(pzExpected);
            Assert.AreEqual((int)expected, cmp);

        }



        [TestMethod]
        public void FFuckMySon()
        {
            var primes = Primes.IntFactorPrimes;
            var ajsPrimes = primes.Where(x => x < 52);



        }



        [TestMethod()]
        public unsafe void AddPositiveTest()
        {

            var dataA = new ulong[] { 0, 1, 2, 3, };
            var dataB = new ulong[] { 0, 1, 2, 3, };
            var dataC = new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, };
            var pzalign = new pz(dataA);
            fixed (ulong* p_a = dataA)
            fixed (ulong* p_b = dataB)
            fixed (ulong* p_c = dataC)
            {
                var pza = new pz(p_a, dataA.Length);
                var pzb = new pz(p_b, dataB.Length);
                var pzc = new pz(p_c, dataC.Length);


                pza.Add(pzb, 0, pzc, 0, 1);

                var expected = 0;
                var zActual = (int)pzc;
                Assert.AreEqual(expected, zActual);


                pza.Add(pzb, 0, pzc, 0, 2);

                ulong[] expectedData = { 0, 2 };
                var pzExpected = new pz(expectedData);
                int cmp = pzc.CompareTo(pzExpected);

                Assert.AreEqual(expected, cmp);

                pza.Add(pzb, 2, pzc, 2, 2);
                expectedData = new ulong[] { 0, 2, 4, 6 };
                pzExpected = new pz(expectedData);
                cmp = pzc.CompareTo(pzExpected);
                Assert.AreEqual(expected, cmp);



            }

        }

        [TestMethod()]
        public unsafe void NormalizeTests()
        {

            var dataA = new ulong[] { ulong.MaxValue, 0 };
            fixed (ulong* p_a = dataA)
            {
                var pza = new pz(p_a, dataA.Length);

                var carry = pza.Normalize();


                var actual = pza[0];
                var expected = (ulong)uint.MaxValue;
                Assert.AreEqual(expected, actual);


                actual = pza[1];
                Assert.AreEqual(expected, actual);

                expected = 0;
                Assert.AreEqual(expected, carry);

                Assert.ThrowsException<IndexOutOfRangeException>(() => pza[3]);
                Assert.ThrowsException<IndexOutOfRangeException>(() => pza[-1] == 1);
                Assert.ThrowsException<IndexOutOfRangeException>(() => pza[3] == 1);

                pza[1] = ulong.MaxValue;
                actual = pza[1];
                expected = ulong.MaxValue;
                Assert.AreEqual(expected, actual);

                carry = pza.Normalize();
                expected = 1ul;
                Assert.AreEqual(expected, carry);

            }

        }

    }
}