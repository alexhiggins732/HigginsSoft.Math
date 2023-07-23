#define INGORE_MULTIPLY

using HigginsSoft.Math.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace HigginsSoft.Math.Demos.UnpackedTests
{
    [TestClass()]
    public class UnpackedFloatTests
    {

        [TestMethod()]
        public void TestConstants()
        {

            float expected;
            UnpackedFloat unpacked;
            UnpackedFloat unpackedExpected;
            float actual;

            expected = -1;
            unpacked = UnpackedFloat.NegativeOne;
            actual = unpacked.ToFloat();
            Assert.AreEqual(expected, actual);

            unpackedExpected = expected;
            Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
            Assert.AreEqual(unpackedExpected.RawExponent, unpacked.RawExponent);
            Assert.AreEqual(unpackedExpected.RawFraction, unpacked.RawFraction);



            expected = 0;
            unpacked = UnpackedFloat.Zero;
            actual = unpacked.ToFloat();
            Assert.AreEqual(expected, actual);

            unpackedExpected = expected;
            Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
            Assert.AreEqual(unpackedExpected.RawExponent, unpacked.RawExponent);
            Assert.AreEqual(unpackedExpected.RawFraction, unpacked.RawFraction);



            expected = 1;
            unpacked = UnpackedFloat.One;
            actual = unpacked.ToFloat();
            Assert.AreEqual(expected, actual);


            unpackedExpected = expected;
            Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
            Assert.AreEqual(unpackedExpected.RawExponent, unpacked.RawExponent);
            Assert.AreEqual(unpackedExpected.RawFraction, unpacked.RawFraction);





        }

        [TestMethod()]
        public void UnpackedFloatTest()
        {
            for (var i = 0; i < 256; i++)
            {
                float expected = i;
                UnpackedFloat fromDouble = expected;
                var actual = fromDouble.ToFloat();
                Assert.AreEqual(expected, actual);

                uint u = BitConverter.SingleToUInt32Bits(expected);
                UnpackedFloat fromUlong = new UnpackedFloat(u, true);
                var uActual = fromUlong.ToFloat();
                Assert.AreEqual(expected, uActual);

                var unpackedInt = new UnpackedFloat(i);
                var floatFromI = unpackedInt.ToFloat();
                Assert.AreEqual(expected, floatFromI);
            }
        }

        [TestMethod()]
        public void UnpackedFloatTest1()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void UnpackedFloatTest2()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            float actualDouble = 12131212.113212313f;
            var expected = actualDouble.ToString();
            UnpackedFloat packed = actualDouble;
            var actual = packed.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void UnpackedFloatTest3()
        {
            //
        }

        [TestMethod()]
        public void ToUlongTest()
        {
            float actualDouble = 12131212.113212313f;
            var expected = BitConverter.SingleToUInt32Bits(actualDouble);
            UnpackedFloat packed = actualDouble;
            var actual = packed.ToPackedUint();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToFloatTest()
        {

            float expected;
            UnpackedFloat unpacked;
            UnpackedFloat unpackedExpected;
            float actual;
            for (var i = -2; i <= 2; i++)
            {
                expected = i;
                unpacked = new UnpackedFloat(expected);
                actual = unpacked.ToFloat();

                Assert.AreEqual(expected, actual);


                unpackedExpected = expected;

                Assert.AreEqual(unpackedExpected.Sign, unpacked.Sign);
                Assert.AreEqual(unpackedExpected.RawExponent, unpacked.RawExponent);
                Assert.AreEqual(unpackedExpected.RawFraction, unpacked.RawFraction);
            }
        }

        [TestMethod()]
        public void ToFloatStringTest()
        {
            var maxError = 0.000000001;
            var t = new UnpackedFloat(.75f);
            var ts = t.ToString();
            for (var @base = 10; @base >= -10; @base--)
            {
                for (var dec = 0; dec < 10; dec++)
                {
                    //var lg = System.Math.Log2(1.0/4);
                    var pow = 1 << dec;
                    var fp = 2.0f / pow;
                    float d = @base + fp;
                    var unpacked = new UnpackedFloat(d);
                    var actual = unpacked.ToString();
                    var parsed = float.Parse(actual);
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
                    var fp = (float)dec * .1f;
                    float d = @base + fp;
                    var unpacked = new UnpackedFloat(d);
                    var actual = unpacked.ToString();
                    var parsed = float.Parse(actual);
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
        [TestMethod]
        public void MultiplyWholeNumbers()
        {
            // float max precision is 2^24;
            var max = 1 << 12;
            Console.Write($"Testing 0 < a < {max}  * 0 < b < {max}");
            var numbers = Enumerable.Range(0, max);
            foreach (var a in numbers)
            {
                foreach (var b in numbers)
                {
                    float floatA = a;
                    float floatB = b;
                    float floatC = floatA * floatB;
                    var expectedUnpacked = new UnpackedFloat(floatC);
                    UnpackedFloat unpackedA = a;
                    UnpackedFloat unpackedB = b;
                    UnpackedFloat unpackedC = unpackedA * unpackedB;
                    float actualC = unpackedC.ToFloat();
                    if (floatC != actualC)
                    {
                        string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                        Assert.AreEqual(floatC, actualC, message);
                    }

                }
            }
        }

#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod()]
        public void MultiplyWholeNumbersNegativeExpTo2p08()
        {
            //zero covered by multiply by zero
            for (var a = 1; a < 256; a++)
            {
                float floatA = 1f / a;

                UnpackedFloat unpackedA = new UnpackedFloat(0, -a, 0);
            
                for (var b = 1; b < 256; b++)
                {
                    UnpackedFloat unpackedB = new UnpackedFloat(0, -b, 0);
                    float floatB = unpackedB.ToFloat();
                    float floatC = floatA * floatB;
                    UnpackedFloat unpackedC = unpackedA * unpackedB;

                    float actualC = unpackedC.ToFloat();
                    if (floatC != actualC)
                    {
                        var expectedUnpacked = new UnpackedFloat(floatC);
                        string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                        Assert.AreEqual(floatC, actualC, message);
                    }
                }
            }
        }


        [TestMethod()]
        public void MultiplyWholeFrom2p0to08()
        {
            //zero covered by multiply by zero
            for (var a = 1; a < 256; a++)
            {

                float floatA = a;
                UnpackedFloat unpackedA = a;

                for (var b = 1; b < 256; b++)
                {
                    float floatB = b;
                    float floatC = floatA * floatB;


                    UnpackedFloat unpackedB = b;
                    UnpackedFloat unpackedC = unpackedA * unpackedB;

                    float actualC = unpackedC.ToFloat();
                    if (floatC != actualC)
                    {
                        var expectedUnpacked = new UnpackedFloat(floatC);
                        string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                        Assert.AreEqual(floatC, actualC, message);
                    }
                }
            }
        }


        [Ignore("FixMe")]
        [TestMethod()]
        public void MultiplyWholeFrom2p09To2p24()
        {
            //zero covered by multiply by zero
            for (var a = 256; a < 4096; a++)
            {
                float floatA = a;
                UnpackedFloat unpackedA = a;

                for (var b = 256; b < 4096; b++)
                {
                    float floatB = b;
                    float floatC = floatA * floatB;


                    UnpackedFloat unpackedB = b;
                    UnpackedFloat unpackedC = unpackedA * unpackedB;

                    float actualC = unpackedC.ToFloat();
                    if (floatC != actualC)
                    {
                        var expectedUnpacked = new UnpackedFloat(floatC);
                        string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                        Assert.AreEqual(floatC, actualC, message);
                    }
                }
            }
        }

#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod()]
        public void MultiplyWholeFrom2p24To26()
        {
            //zero covered by multiply by zero
            for (var a = 4096; a < 4096 << 1; a++)
            {
                float floatA = a;
                UnpackedFloat unpackedA = a;

                for (var b = 4096; b < 4096 << 1; b++)
                {
                    float floatB = b;
                    float floatC = floatA * floatB;


                    UnpackedFloat unpackedB = b;
                    UnpackedFloat unpackedC = unpackedA * unpackedB;

                    float actualC = unpackedC.ToFloat();
                    if (floatC != actualC)
                    {
                        var intExpected = a * b;
                        var unpackedIntExpected = new UnpackedFloat(intExpected);
                        var expectedUnpacked = new UnpackedFloat(floatC);
                        string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                        Assert.AreEqual(floatC, actualC, message);
                    }
                }
            }
        }


#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod]
        public void DebugMul()
        {
            // UnpackedFloat.DebugMul(.25f, 7);
            //UnpackedFloat.DebugMul(.5f, 7);
            //UnpackedFloat.DebugMul(1, 7);
            //UnpackedFloat.DebugMul(1.5f, 7);
            //UnpackedFloat.DebugMul(2, 7);
            //UnpackedFloat.DebugMul(2.5f, 7);
            //UnpackedFloat.DebugMul(3f, 7);
            //UnpackedFloat.DebugMul(6f, 7);

            var floatA = 0.046875004f;
            var floatB = 0.0006103516f;

            UnpackedFloat a, b, c;


            if (true)
            {
                UnpackedFloat.DebugMul(floatA, floatB);

                a = floatA;
                b = floatB;
                c = a * b;
                Console.WriteLine(a.DebugString(nameof(a)));
                Console.WriteLine(b.DebugString(nameof(b)));
                Console.WriteLine(c.DebugString(nameof(c)));

            }

            if (false)
            {
                UnpackedFloat.DebugMul(.33f, .25f);

                Console.WriteLine("==============================");
                Console.WriteLine("testing mul");
                a = new UnpackedFloat(.33f);
                b = new UnpackedFloat(.25f);
                c = a * b;

                Console.WriteLine(a.DebugString(nameof(a)));
                Console.WriteLine(b.DebugString(nameof(b)));
                Console.WriteLine(c.DebugString(nameof(c)));

            }


            if (false)
            {
                UnpackedFloat.DebugMul(floatA, floatB);

                a = floatA;
                b = floatB;
                c = a * b;
                Console.WriteLine(a.DebugString(nameof(a)));
                Console.WriteLine(b.DebugString(nameof(b)));
                Console.WriteLine(c.DebugString(nameof(c)));

            }
        }

#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod]
        public void MultiplyTest()
        {
            var x = 1;
            var high = 1u << (UnpackedFloat.ExponentShiftSize - MathLib.BitLength(x));
            var low = 1u;
            uint fract = high + low;

            var a = new UnpackedFloat(0, -5, fract);
            var b = new UnpackedFloat(0, -7, fract);

            var aFloat = a.ToFloat();
            var bFloat = b.ToFloat();
            var expected = aFloat * bFloat;
            var c = new UnpackedFloat(expected);
            var actualUnpacked = a * b;
            var actual = actualUnpacked.ToFloat();

            if (expected != actual)
            {
                var sb = new StringBuilder();
                sb.AppendLine(a.DebugString(nameof(a)));
                sb.AppendLine();
                sb.AppendLine(b.DebugString(nameof(b)));
                sb.AppendLine();
                sb.AppendLine(c.DebugString(nameof(c)));

                var debugString = sb.ToString();

                var message = $"Mult failed for {a} * {b} = \n{debugString}";
                Assert.AreEqual(expected, actual, message);
            }



        }
        IEnumerable<(UnpackedFloat expected, UnpackedFloat a, UnpackedFloat b)>
            GetComboTests()
        {
            var exponentsA = new[] { -5, -7, 5, 7 };
            var exponentsB = new[] { -11, -13, 11, 13 };
            var mask = UnpackedFloat.FractionMask;
            var fracts = Enumerable.Range(1, 16)
                .Select(x =>
                    mask & ((1u << (UnpackedFloat.ExponentShiftSize - MathLib.BitLength(x))) + 1)
                )
                .ToList();
            var signs = new[] { 0u, 1u };
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
                                    var a = new UnpackedFloat(aSign, aExp, aFract);
                                    var b = new UnpackedFloat(bSign, bExp, bFract);
                                    var dblA = a.ToFloat();
                                    var dblB = b.ToFloat();
                                    var expected = dblA * dblB;
                                    yield return (expected, a, b);

                                }
                            }
                        }
                    }
                }
            }
        }
        [TestMethod]
        public void MultiplyByZeroTest()
        {
            // float max precision is 2^24;
            var max = 1 << 12;
            var numbers = Enumerable.Range(0, max);

            foreach (var b in numbers)
            {
                var a = 0;
                float floatA = a;
                float floatB = b;
                float floatC = floatA * floatB;
                var expectedUnpacked = new UnpackedFloat(floatC);
                UnpackedFloat unpackedA = a;
                UnpackedFloat unpackedB = b;
                UnpackedFloat unpackeC = unpackedA * unpackedB;
                float actualC = unpackeC.ToFloat();
                if (floatC != actualC)
                {
                    string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                    Assert.AreEqual(floatC, actualC, message);
                }

            }
            foreach (var a in numbers)
            {
                var b = 0;
                float floatA = a;
                float floatB = b;
                float floatC = floatA * floatB;
                var expectedUnpacked = new UnpackedFloat(floatC);
                UnpackedFloat unpackedA = a;
                UnpackedFloat unpackedB = b;
                UnpackedFloat unpackeC = unpackedA * unpackedB;
                float actualC = unpackeC.ToFloat();
                if (floatC != actualC)
                {
                    string message = $"Whole number test failed for {floatA} * {floatB} = {floatC}";
                    Assert.AreEqual(floatC, actualC, message);
                }

            }

        }


#if INGORE_MULTIPLY
        [Ignore]
#endif
        [TestMethod()]
        public void MultiplyExpCombos()
        {
            int count = 0;
            var tests = GetComboTests().ToList();
            foreach (var test in tests)
            {
                var actual = test.a * test.b;
                var expected = test.expected;
                count++;
                if (actual.ToFloat() != expected.ToFloat())
                {
                    string message = $"Mutliply failed for {test.a} * {test.b} after {count} tests";
                    Assert.AreEqual(actual.ToFloat(), expected.ToFloat(), message);
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
                UnpackedFloat ui = i;
                for (var j = 1; j < 256; j++)
                {
                    float recip = 1.0f / j;
                    UnpackedFloat uj = recip;
                    var expected = (float)i * recip;

                    UnpackedFloat expectedUnpacked = expected;

                    var actualUnpacked = ui * uj;
                    var actual = actualUnpacked.ToFloat();

                    if (expected != actual)
                    {
                        if (actualUnpacked.RawExponent != expectedUnpacked.RawExponent)
                        {
                            var test = new UnpackedFloat(actualUnpacked.ToPackedUint(), true);
                            test.RawExponent = expectedUnpacked.RawExponent;
                            if (test.ToFloat() == expected)
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
                    Assert.AreEqual(expectedUnpacked.RawExponent, actualUnpacked.RawExponent);
                    Assert.AreEqual(expectedUnpacked.RawFraction, actualUnpacked.RawFraction);
                    Assert.AreEqual(expected, actual);

                }
            }
        }

    }
}
