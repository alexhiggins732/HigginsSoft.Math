#define INGORE_MULTIPLY
using HigginsSoft.Math.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HigginsSoft.Math.Demos.UnpackedTests
{

    [TestClass()]
    public class UnpackedDoubleTests
    {

        [TestMethod()]
        public void TestConstants()
        {
   

            double expected;
            UnpackedDouble unpacked;
            UnpackedDouble unpackedExpected;
            double actual;

            expected = -1;
            unpacked = UnpackedDouble.NegativeOne;
            actual = unpacked.ToDouble();
            Assert.AreEqual(expected, actual);

            unpackedExpected = expected;
            Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
            Assert.AreEqual(unpackedExpected.Exponent, unpacked.Exponent);
            Assert.AreEqual(unpackedExpected.Fraction, unpacked.Fraction);



            expected = 0;
            unpacked = UnpackedDouble.Zero;
            actual = unpacked.ToDouble();
            Assert.AreEqual(expected, actual);

            unpackedExpected = expected;
            Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
            Assert.AreEqual(unpackedExpected.Exponent, unpacked.Exponent);
            Assert.AreEqual(unpackedExpected.Fraction, unpacked.Fraction);



            expected = 1;
            unpacked = UnpackedDouble.One;
            actual = unpacked.ToDouble();
            Assert.AreEqual(expected, actual);


            unpackedExpected = expected;
            Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
            Assert.AreEqual(unpackedExpected.Exponent, unpacked.Exponent);
            Assert.AreEqual(unpackedExpected.Fraction, unpacked.Fraction);





        }

        [TestMethod()]
        public void UnpackedDoubleTest()
        {
            for (var i = 0; i < 256; i++)
            {
                double expected = i;
                UnpackedDouble fromDouble = expected;
                var actual = fromDouble.ToDouble();
                Assert.AreEqual(expected, actual);

                ulong u = BitConverter.DoubleToUInt64Bits(expected);
                UnpackedDouble fromUlong = new UnpackedDouble(u, true);
                var uActual = fromUlong.ToDouble();
                Assert.AreEqual(expected, uActual);

                var unpackedInt = new UnpackedDouble(i);
                var doubleFromI = unpackedInt.ToDouble();
                Assert.AreEqual(expected, doubleFromI);
            }
        }

        [TestMethod()]
        public void UnpackedDoubleTest1()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void UnpackedDoubleTest2()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            double actualDouble = 12131212.113212313;
            var expected = actualDouble.ToString();
            UnpackedDouble packed = actualDouble;
            var actual = packed.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void UnpackedDoubleTest3()
        {
            //
        }

        [TestMethod()]
        public void ToUlongTest()
        {
            double actualDouble = 12131212.113212313;
            var expected = BitConverter.DoubleToUInt64Bits(actualDouble);
            UnpackedDouble packed = actualDouble;
            var actual = packed.ToPackedUlong();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToDoubleTest()
        {

            double expected;
            UnpackedDouble unpacked;
            UnpackedDouble unpackedExpected;
            double actual;
            for (var i = -2; i <= 2; i++)
            {
                expected = i;
                unpacked = new UnpackedDouble(expected);
                actual = unpacked.ToDouble();

                Assert.AreEqual(expected, actual);


                unpackedExpected = expected;

                Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
                Assert.AreEqual(unpackedExpected.Exponent, unpacked.Exponent);
                Assert.AreEqual(unpackedExpected.Fraction, unpacked.Fraction);
            }
        }


        [TestMethod()]
        public void ToDoubleStringTest()
        {
            var maxError = 0.000000001;
            var t = new UnpackedDouble(.75);
            var ts = t.ToDoubleString();
            for (var @base = 10; @base >= -10; @base--)
            {
                for (var dec = 0; dec < 10; dec++)
                {
                    //var lg = System.Math.Log2(1.0/4);
                    var pow = 1 << dec;
                    var fp = 2.0 / pow;
                    double d = @base + fp;
                    var unpacked = new UnpackedDouble(d);
                    var actual = unpacked.ToDoubleString();
                    var parsed = double.Parse(actual);
                    var err = MathLib.Abs(d - parsed);
                    if (err > maxError)
                    {
                        var expected = d.ToString();
                        Assert.AreEqual(expected, actual, $"Base={@base}, decimal = {dec}, value = {d}");
                    }

                }
            }

            for (var @base = 10; @base >= -10; @base--)
            {
                for (var dec = 0; dec < 10; dec++)
                {
                    var fp = (double)dec * .1;
                    double d = @base + fp;
                    var unpacked = new UnpackedDouble(d);
                    var actual = unpacked.ToDoubleString();
                    var parsed = double.Parse(actual);
                    var err = MathLib.Abs(d - parsed);
                    if (err > maxError)
                    {
                        var expected = d.ToString();
                        Assert.AreEqual(expected, actual, $"Base={@base}, decimal = {dec}, value = {d}");
                    }

                }
            }
        }
#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod()]
        public void MultiplyTest()
        {
            for (var i = 0; i < 256; i++)
            {
                UnpackedDouble ui = i;
                for (var j = 0; j < 256; j++)
                {
                    UnpackedDouble uj = j;
                    var expected = (double)i * j;

                    UnpackedDouble expectedUnpacked = expected;

                    var actualUnpacked = ui * uj;
                    var actual = actualUnpacked.ToDouble();


                    if (expected != actual)
                    {
                        string bp = "";
                    }
                    Assert.AreEqual(expectedUnpacked.Sign, actualUnpacked.Sign);
                    Assert.AreEqual(expectedUnpacked.Exponent, actualUnpacked.Exponent);
                    Assert.AreEqual(expectedUnpacked.Fraction, actualUnpacked.Fraction);
                    Assert.AreEqual(expected, actual);

                }
            }
        }


        IEnumerable<(UnpackedDouble expected, UnpackedDouble a, UnpackedDouble b)>
            GetComboTests()
        {
            var exponentsA = new[] { -5, -7, 5, 7 };
            var exponentsB = new[] { -11, -13, 11, 13 };
            var fracts = Enumerable.Range(1, 16).Select(x => UnpackedDouble.FractionMask & ((1ul << (52 - MathLib.BitLength(x))) + 1))
                .ToList();
            var signs = new[] { 0, 1 };
            foreach (var aExp in exponentsA)
            {
                foreach (var bExp in exponentsB)
                {
                    foreach (var aSign in signs)
                    {
                        foreach (var bSign in signs)
                        {
                            foreach (var aFract in fracts)
                            {
                                foreach (var bFract in fracts)
                                {
                                    var a = new UnpackedDouble(aSign, aExp, aFract);
                                    var b = new UnpackedDouble(bSign, bExp, bFract);
                                    var dblA = a.ToDouble();
                                    var dblB = b.ToDouble();
                                    var expected = dblA * dblB;
                                    yield return (expected, a, b);

                                }
                            }
                        }
                    }
                }
            }
        }

#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod()]
        public void MultiplyExpCombos()
        {
            var tests = GetComboTests().ToList();
            foreach (var test in tests)
            {
                var actual = test.a * test.b;
                var expected = test.expected;
                if (actual.ToDouble() != expected.ToDouble())
                {
                    string message = "Mutliply failed";
                    Assert.AreEqual(actual.ToDouble(), expected.ToDouble(), message);
                }
            }


        }

#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod()]
        public void MultiplyInverseTest()
        {
            for (var i = 0; i < 256; i++)
            {
                UnpackedDouble ui = i;
                for (var j = 1; j < 256; j++)
                {
                    double recip = 1.0 / j;
                    UnpackedDouble uj = recip;
                    var expected = (double)i * recip;

                    UnpackedDouble expectedUnpacked = expected;

                    var actualUnpacked = ui * uj;
                    var actual = actualUnpacked.ToDouble();

                    if (expected != actual)
                    {
                        if (actualUnpacked.Exponent != expectedUnpacked.Exponent)
                        {
                            var test = new UnpackedDouble(actualUnpacked.AsPackedUlong, true);
                            test.Exponent = expectedUnpacked.Exponent;
                            if (test.ToDouble() == expected)
                            {
                                string bps = "Wrong exponent";
                                Assert.Fail($"Cacluated wrong exponent for {i} * {recip}");
                            }
                            else
                            {
                                Assert.Fail($"Cacluated wrong exponent and value for {i} * {recip}");
                            }
                        }
                        string bp = "";
                    }
                    Assert.AreEqual(expectedUnpacked.Sign, actualUnpacked.Sign);
                    Assert.AreEqual(expectedUnpacked.Exponent, actualUnpacked.Exponent);
                    Assert.AreEqual(expectedUnpacked.Fraction, actualUnpacked.Fraction);
                    Assert.AreEqual(expected, actual);

                }
            }
        }

    }
}
namespace HigginsSoft.Math.Demos.UnpackedFloatTests
{
}
