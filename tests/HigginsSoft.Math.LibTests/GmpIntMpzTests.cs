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
using MathGmp.Native;
using System.Numerics;
using HigginsSoft.Math.Lib.Tests.MathUtilTests;

namespace HigginsSoft.Math.Lib.Tests
{
    namespace GmpIntTests
    {
        [TestClass()]
        public class GmpIntMpzTests
        {
            [TestMethod()]
            public void AbsTest()
            {
                GmpInt negA = -1;
                GmpInt a = negA.Abs();

                Assert.IsTrue(a == GmpInt.One);
            }

            [TestMethod()]
            public void NegateTest()
            {

                Assert.IsTrue(GmpInt.One == GmpInt.MinusOne.Negate());
                Assert.IsTrue(GmpInt.MinusOne == GmpInt.One.Negate());
            }

            [TestMethod()]
            public void NegateTest1()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.Negate(GmpInt.MinusOne));
                Assert.IsTrue(GmpInt.MinusOne == GmpInt.Negate(GmpInt.One));
            }




            [TestMethod()]
            public void ComplementTest()
            {
                var result = GmpInt.One.Complement();
                Assert.IsTrue(GmpInt.MinusTwo == GmpInt.One.Complement());
            }

            [TestMethod()]
            public void AddTest()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.Zero.Add(GmpInt.One));
            }

            [TestMethod()]
            public void AddTest1()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.Zero.Add(1));
            }

            [TestMethod()]
            public void AddTest2()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.Zero.Add(1U));
            }

            [TestMethod()]
            public void SubtractTest()
            {
                Assert.IsTrue(GmpInt.Zero == GmpInt.One.Subtract(GmpInt.One));
            }

            [TestMethod()]
            public void SubtractTest1()
            {
                Assert.IsTrue(GmpInt.Zero == GmpInt.One.Subtract(1));
                Assert.IsTrue(GmpInt.Zero == GmpInt.One.Subtract(1U));
            }

            [TestMethod()]
            public void SubtractTest2()
            {
                Assert.IsTrue(GmpInt.Two == GmpInt.One.Subtract(-1));
            }

            [TestMethod()]
            public void MultiplyTest()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.One.Multiply(GmpInt.One));
            }

            [TestMethod()]
            public void MultiplyTest1()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.One.Multiply(1));
            }

            [TestMethod()]
            public void MultiplyTest2()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.One.Multiply(1U));
            }

            [TestMethod()]
            public void SquareTest()
            {
                GmpInt expected = 4;
                GmpInt a = 2;
                Assert.IsTrue(expected == a.Square());
            }

            [TestMethod()]
            public void DivideTest()
            {
                GmpInt expected = 2;
                GmpInt a = 4;
                Assert.IsTrue(expected == a.Divide(expected));
            }

            [TestMethod()]
            public void DivideTest1()
            {
                GmpInt expected = 2;
                GmpInt a = 4;
                Assert.IsTrue(expected == a.Divide(2));
            }

            [TestMethod()]
            public void DivideTest2()
            {
                GmpInt expected = 2;
                GmpInt a = 4;
                Assert.IsTrue(expected == a.Divide(2u));
            }

            [TestMethod()]
            public void DivideTest3()
            {
                GmpInt result = new();
                GmpInt a = 5;
                GmpInt divisor = 2;
                mpz_t mod = result.Data;
                Assert.IsTrue(divisor == a.Divide(divisor, out mod));
                Assert.IsTrue(mod == GmpInt.One);
            }

            [TestMethod()]
            public void DivideTest4()
            {
                GmpInt result = new();
                GmpInt a = 5;
                GmpInt divisor = 2;

                Assert.IsTrue(divisor == a.Divide(2, out mpz_t mod));
                Assert.IsTrue(mod == GmpInt.One);


                Assert.IsTrue(-divisor == a.Divide(-2, out mod));
                Assert.IsTrue(mod == GmpInt.One);
            }

            [TestMethod()]
            public void DivideTest5()
            {

                GmpInt a = 5;
                GmpInt quotient = a.Divide(2, out int remainder);
                Assert.IsTrue(quotient == 2);
                Assert.IsTrue(remainder == 1);

                a = 5;
                quotient = a.Divide(-2, out remainder);
                Assert.IsTrue(quotient == -2);
                Assert.IsTrue(remainder == -1);

            }

            [TestMethod()]
            public void DivideTest6()
            {
                GmpInt a = 5;
                GmpInt quotient = a.Divide(2u, out int remainder);
                Assert.IsTrue(quotient == 2);
                Assert.IsTrue(remainder == 1);




                a = -5;
                quotient = a.Divide(2u, out remainder);
                Assert.IsTrue(quotient == -2);
                Assert.IsTrue(remainder == -1);

                a = GmpInt.Two.Power(127);
                a -= 1;

                Assert.ThrowsException<OverflowException>(() => a.Divide(3221225585u, out remainder));
                //b = 3221225585u;

            }

            [TestMethod()]
            public void DivideTest7()
            {
                GmpInt a = 5;
                GmpInt quotient = a.Divide(2u, out ulong remainder);
                Assert.IsTrue(quotient == 2);
                Assert.IsTrue(remainder == 1);


                quotient = a.Divide(2u, out mpz_t mp_remainder);
                Assert.IsTrue(quotient == 2);
                Assert.IsTrue((GmpInt)mp_remainder == 1);
            }

            [TestMethod()]
            public void DivideTest8()
            {
                GmpInt a = 5;
                GmpInt quotient = a.Divide(2u, out int remainder);
                Assert.IsTrue(quotient == 2);
                Assert.IsTrue(remainder == 1);
            }

            [TestMethod()]
            public void RemainderTest()
            {
                GmpInt a = 5;
                GmpInt remainder = a.Remainder(GmpInt.Two);
                Assert.IsTrue(remainder == 1);
            }

            [TestMethod()]
            public void IsDivisibleByTest()
            {
                Assert.IsTrue(GmpInt.Ten.IsDivisibleBy(GmpInt.Two));
                Assert.IsFalse(GmpInt.Three.IsDivisibleBy(GmpInt.Two));
            }

            [TestMethod()]
            public void IsDivisibleByTest1()
            {
                Assert.IsTrue(GmpInt.Ten.IsDivisibleBy(2));
                Assert.IsFalse(GmpInt.Three.IsDivisibleBy(2));

                Assert.IsTrue(GmpInt.Ten.IsDivisibleBy(-2));
                Assert.IsFalse(GmpInt.Three.IsDivisibleBy(-2));
            }

            [TestMethod()]
            public void IsDivisibleByTest2()
            {
                Assert.IsTrue(GmpInt.Ten.IsDivisibleBy(2U));
                Assert.IsFalse(GmpInt.Three.IsDivisibleBy(2U));
            }

            [TestMethod()]
            public void DivideExactlyTest()
            {
                var result = GmpInt.Ten.DivideExactly(GmpInt.Five);
                Assert.IsTrue(result == GmpInt.Two);

                result = GmpInt.Ten.DivideExactly(GmpInt.Three);
                Assert.IsTrue(result != GmpInt.Two);
            }

            [TestMethod()]
            public void DivideExactlyTest1()
            {
                var result = GmpInt.Ten.DivideExactly(5);
                Assert.IsTrue(result == GmpInt.Two);

                result = GmpInt.Ten.DivideExactly(GmpInt.Three);
                Assert.IsTrue(result != GmpInt.Two);

                result = GmpInt.Ten.DivideExactly(-5);
                Assert.IsTrue(result == GmpInt.MinusTwo);

                result = GmpInt.Ten.DivideExactly(GmpInt.Three);
                Assert.IsTrue(result != GmpInt.MinusTwo);
            }

            [TestMethod()]
            public void DivideExactlyTest2()
            {
                var result = GmpInt.Ten.DivideExactly(5u);
                Assert.IsTrue(result == GmpInt.Two);

                result = GmpInt.Ten.DivideExactly(3u);
                Assert.IsTrue(result != GmpInt.Three);
            }

            [TestMethod()]
            public void DivideModTest()
            {

                GmpInt a = 5;
                GmpInt b = 2;
                GmpInt mod = 1;
                GmpInt expected = 0;


                GmpInt actual = a.DivideMod(b, mod);
                Assert.AreEqual(expected, actual);


                // Test case 1: Small numbers
                a = 10;
                b = 3;
                mod = 7;
                expected = 1;
                actual = a.DivideMod(b, mod);
                Assert.AreEqual(expected, actual);

                // Test case 2: Large numbers
                a = "123456789012345678901234567890";
                b = "987654321098765432109876543210";
                mod = "987654323";


                expected = "432098767";
                actual = a.DivideMod(b, mod);
                Assert.AreEqual(expected, actual);

                // Test case 3: Divisor is larger than dividend
                a = "123456789012345678901234567890";
                b = "987654321098765432109876543210";
                mod = "246813583";


                expected = "15053266";
                actual = a.DivideMod(b, mod);
                Assert.AreEqual(expected, actual);

                // Test case 4: Divisor is 1
                a = 123456;
                b = 1;
                mod = 123;

                expected = 87;
                actual = a.DivideMod(b, mod);
                Assert.AreEqual(expected, actual);

            }

            [TestMethod()]
            public void AndTest()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.One.And(GmpInt.One));
            }

            [TestMethod()]
            public void OrTest()
            {
                Assert.IsTrue(GmpInt.One == GmpInt.One.Or(GmpInt.One));
            }

            [TestMethod()]
            public void XorTest()
            {
                Assert.IsTrue(GmpInt.Zero == GmpInt.One.Xor(GmpInt.One));
            }

            [TestMethod()]
            public void ModTest()
            {
                Assert.IsTrue(GmpInt.Ten.Mod(GmpInt.Five) == GmpInt.Zero);
            }

            [TestMethod()]
            public void ModTest1()
            {
                Assert.IsTrue(GmpInt.Ten.Mod(5) == GmpInt.Zero);
            }

            [TestMethod()]
            public void ModTest2()
            {
                Assert.IsTrue(GmpInt.Ten.Mod(5u) == GmpInt.Zero);
            }

            [TestMethod()]
            public void ModAsInt32Test()
            {
                Assert.IsTrue(GmpInt.Ten.ModAsInt32(5) == 0);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Ten.ModAsInt32(-5));
            }

            [TestMethod()]
            public void ModAsUInt32Test()
            {
                Assert.IsTrue(GmpInt.Ten.ModAsUInt32(5) == 0);
            }

            [TestMethod()]
            public void ShiftLeftTest()
            {
                Assert.IsTrue(GmpInt.One.ShiftLeft(1) == GmpInt.Two);
            }

            [TestMethod()]
            public void ShiftRightTest()
            {
                Assert.IsTrue(GmpInt.One.ShiftRight(1) == GmpInt.Zero);
            }

            [TestMethod()]
            public void PowerModTest()
            {
                Assert.IsTrue(GmpInt.Two.PowerMod(GmpInt.Five, GmpInt.Three) == GmpInt.Two);
            }

            [TestMethod()]
            public void PowerModTest1()
            {
                Assert.IsTrue(GmpInt.Two.PowerMod(5, GmpInt.Three) == GmpInt.Two);

                Assert.IsTrue(GmpInt.Two.PowerMod(-2, GmpInt.Three) == GmpInt.One);
            }

            [TestMethod()]
            public void PowerModTest2()
            {
                Assert.IsTrue(GmpInt.Two.PowerMod(5U, GmpInt.Three) == GmpInt.Two);
            }

            [TestMethod()]
            public void PowerTest()
            {
                Assert.IsTrue(GmpInt.Two.Power(3) == (GmpInt)8);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Two.Power(-2));
            }

            [TestMethod()]
            public void PowerTest1()
            {
                Assert.IsTrue(GmpInt.Two.Power(3u) == (GmpInt)8);
            }

            [TestMethod()]
            public void PowerTest2()
            {
                Assert.IsTrue(GmpInt.Power(2, 3) == (GmpInt)8);
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Power(2u, -3));
            }

            [TestMethod()]
            public void PowerTest3()
            {
                Assert.IsTrue(GmpInt.Power(2u, 3u) == (GmpInt)8);

            }

            [TestMethod()]
            public void InvertModTest()
            {
                Assert.IsTrue(GmpInt.Ten.InvertMod(GmpInt.Three) == GmpInt.One);
                Assert.ThrowsException<ArithmeticException>(() => GmpInt.Ten.InvertMod(GmpInt.Two) == GmpInt.Two);

            }

            [TestMethod()]
            public void InvertModTest1()
            {
                Assert.IsTrue(GmpInt.InvertMod(GmpInt.Ten, GmpInt.Three) == GmpInt.One);
                Assert.ThrowsException<ArithmeticException>(() => GmpInt.InvertMod(GmpInt.Ten, GmpInt.Two) == GmpInt.Two);
            }

            [TestMethod()]
            public void TryInvertModTest()
            {
                Assert.IsTrue(GmpInt.Ten.TryInvertMod(GmpInt.Three, out mpz_t result));
                Assert.IsTrue(result == GmpInt.One);
                Assert.IsFalse(GmpInt.Ten.TryInvertMod(GmpInt.Two, out result));
                Assert.IsTrue(result == GmpInt.Zero);

            }

            [TestMethod()]
            public void InverseModExistsTest()
            {
                Assert.IsTrue(GmpInt.Ten.InverseModExists(GmpInt.Three));
                Assert.IsFalse(GmpInt.Ten.InverseModExists(GmpInt.Two));
            }

            [TestMethod()]
            public void SqrtTest()
            {
                Assert.IsTrue(GmpInt.Ten.Sqrt() == (GmpInt)3);
            }

            [TestMethod()]
            public void SqrtTest1()
            {
                Assert.IsTrue(GmpInt.Ten.Sqrt(out mpz_t remainder) == (GmpInt)3);
                Assert.IsTrue(remainder == GmpInt.One);
            }

            [TestMethod()]
            public void SqrtTest2()
            {
                Assert.IsTrue(GmpInt.Ten.Sqrt(out bool isExact) == (GmpInt)3);
                Assert.IsFalse(isExact);
            }

            [TestMethod()]
            public void RootTest()
            {
                GmpInt power = GmpInt.Ten.Root(2);
                Assert.IsTrue(power == GmpInt.Three);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Three.Root(-2));

            }

            [TestMethod()]
            public void RootTest1()
            {
                GmpInt power = GmpInt.Three.Power(3);
                Assert.IsTrue(power.Root(3u) == GmpInt.Three);
            }

            [TestMethod()]
            public void RootTest2()
            {
                GmpInt power = GmpInt.Three.Power(3);
                Assert.IsTrue(power.Root(3, out bool isExact) == GmpInt.Three);
                Assert.IsTrue(isExact);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Three.Root(-2, out isExact));
            }

            [TestMethod()]
            public void RootTest3()
            {
                GmpInt power = GmpInt.Three.Power(3);
                Assert.IsTrue(power.Root(3u, out bool isExact) == GmpInt.Three);
                Assert.IsTrue(isExact);


            }

            [TestMethod()]
            public void RootTest4()
            {
                GmpInt power = GmpInt.Three.Power(3);
                Assert.IsTrue(power.Root(3, out mpz_t remainder) == GmpInt.Three);
                Assert.IsTrue(remainder == GmpInt.Zero);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Three.Root(-2, out remainder));
            }

            [TestMethod()]
            public void RootTest5()
            {
                GmpInt power = GmpInt.Three.Power(3);
                Assert.IsTrue(power.Root(3u, out mpz_t remainder) == GmpInt.Three);
                Assert.IsTrue(remainder == GmpInt.Zero);
            }

            [TestMethod()]
            public void IsPerfectSquareTest()
            {
                Assert.IsFalse(GmpInt.Ten.IsPerfectSquare());
                GmpInt z = 1 << 4;
                Assert.IsTrue(z.IsPerfectSquare());

            }

            [TestMethod()]
            public void IsPerfectPowerTest()
            {
                GmpInt z = 1 << 4;
                Assert.IsTrue(z.IsPerfectPower());
                z >>= 1;
                Assert.IsTrue(z.IsPerfectPower());
                Assert.IsFalse(z.IsPerfectSquare());
            }

            [TestMethod()]
            public void IsProbablyPrimeRabinMillerTest()
            {
                GmpInt n = int.MaxValue;
                Assert.IsTrue(n.IsProbablyPrimeRabinMiller(20));
                n -= 4;
                Assert.IsFalse(n.IsProbablyPrimeRabinMiller(20));

                n = 7919; // a prime number
                Assert.IsTrue(n.IsProbablyPrimeRabinMiller(20));
                n -= 1;
                Assert.IsFalse(n.IsProbablyPrimeRabinMiller(20));
            }

            [TestMethod()]
            public void NextPrimeGMPTest()
            {
                GmpInt n = 2;
                n = n.NextPrimeGMP();
                Assert.IsTrue(n == 3);
                n = n.NextPrimeGMP();
                Assert.IsTrue(n == 5);
                n = int.MaxValue;
                n = n.NextPrimeGMP();
                Assert.IsTrue(n == "2147483659"); ;
            }

            [TestMethod()]
            public void GcdTest()
            {
                GmpInt result = GmpInt.Gcd(GmpInt.Three, GmpInt.Two);
                Assert.IsTrue(result == 1);

                result = GmpInt.Gcd(GmpInt.Ten, GmpInt.Five);
                Assert.IsTrue(result == 5);
            }

            [TestMethod()]
            public void GcdTest1()
            {
                GmpInt result = GmpInt.Gcd(3, GmpInt.Two);
                Assert.IsTrue(result == 1);

                result = GmpInt.Gcd(10, GmpInt.Five);
                Assert.IsTrue(result == 5);

                result = GmpInt.Gcd(10, GmpInt.MinusFive);
                Assert.IsTrue(result == GmpInt.Five);

                result = GmpInt.Gcd(-10, GmpInt.Five);
                Assert.IsTrue(result == GmpInt.Five);
            }

            [TestMethod()]
            public void GcdTest2()
            {
                GmpInt result = GmpInt.Gcd(GmpInt.Three, 2);
                Assert.IsTrue(result == 1);

                result = GmpInt.Gcd(GmpInt.Ten, 5);
                Assert.IsTrue(result == 5);

                result = GmpInt.Gcd(GmpInt.Ten, -5);
                Assert.IsTrue(result == GmpInt.Five);
            }

            [TestMethod()]
            public void GcdTest3()
            {
                GmpInt result = GmpInt.Gcd(3u, GmpInt.Two);
                Assert.IsTrue(result == 1);

                result = GmpInt.Gcd(10u, GmpInt.Five);
                Assert.IsTrue(result == 5);
            }

            [TestMethod()]
            public void GcdTest4()
            {
                GmpInt result = GmpInt.Gcd(GmpInt.Three, 2u);
                Assert.IsTrue(result == 1);

                result = GmpInt.Gcd(GmpInt.Ten, 5u);
                Assert.IsTrue(result == 5);
            }

            [TestMethod()]
            public void GcdTest5()
            {

                var data = GcdTestData.Create(12, 12, 0, 1, 12, x => (GmpInt)x);

                var result = GmpInt.Gcd(data.A, data.B, out mpz_t s, out mpz_t t);

                // Assert
                Assert.IsTrue(data.Expected == result);
                Assert.IsTrue(data.S == s);
                Assert.IsTrue(data.T == t);


            }

            [TestMethod()]
            public void GcdTest6()
            {
                var data = GcdTestData.Create(12, 12, 0, 1, 12, x => (GmpInt)x);

                var result = GmpInt.Gcd(data.A, data.B, out mpz_t s);

                // Assert
                Assert.IsTrue(data.Expected == result);
                Assert.IsTrue(data.S == s);


            }

            [TestMethod()]
            public void LcmTest()
            {
                GmpInt result = GmpInt.Lcm(GmpInt.Five, GmpInt.Three);
                Assert.IsTrue(result == 15);
            }

            [TestMethod()]
            public void LcmTest1()
            {
                GmpInt result = GmpInt.Lcm(GmpInt.Five, 3);
                Assert.IsTrue(result == 15);

                result = GmpInt.Lcm(GmpInt.Five, -3);
                Assert.IsTrue(result == 15);
            }

            [TestMethod()]
            public void LcmTest2()
            {
                GmpInt result = GmpInt.Lcm(3, GmpInt.Five);
                Assert.IsTrue(result == 15);

                result = GmpInt.Lcm(-3, GmpInt.Five);
                Assert.IsTrue(result == 15);
            }

            [TestMethod()]
            public void LcmTest3()
            {
                GmpInt result = GmpInt.Lcm(GmpInt.Five, 3u);
                Assert.IsTrue(result == 15);
            }

            [TestMethod()]
            public void LcmTest4()
            {
                GmpInt result = GmpInt.Lcm(3u, GmpInt.Five);
                Assert.IsTrue(result == 15);
            }

            [TestMethod()]
            public void LegendreSymbolTest()
            {
                var tests = new Dictionary<int, int[]>
            {
                {2, new[] {0}},
                {3, new[] {0, 1, -1}},
                {5, new[] {0, 1, -1, -1, 1}},
                {7, new[] {0, 1, 1, -1, 1, -1, -1}},
                {11, new[] {0, 1, -1, 1, 1, 1, -1, -1, -1, 1, -1}},
            };


                tests.ToList().ForEach(x =>
                {
                    GmpInt prime = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.LegendreSymbol(k, prime);
                        Assert.IsTrue(result == expected, $"Test failed for prime:{prime}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });

            }


            Dictionary<int, int[]> getJacobiTests()
            {
                var tests = new Dictionary<int, int[]>()
            {

                {1, new[]{1} },
                {3, new[]{0,1,-1} },
                {5, new[]{0,1,-1,-1,1} },
                {7, new[]{0,1,1,-1,1,-1,-1} },
                {9, new[]{0,1,1,0,1,1,0,1,1} },
                {11,new[]{0,1,-1,1,1,1,-1,-1,-1,1,-1} },
                {13,new[]{0,1,-1,1,1,-1,-1,-1,-1,1,1,-1,1} },
                {15,new[]{0,1,1,0,1,0,0,-1,1,0,0,-1,0,-1,-1} },
                {17,new[]{0,1,1,-1,1,-1,-1,-1,1,1,-1,-1,-1,1,-1,1,1} },
            };
                return tests;
            }

            [TestMethod()]
            public void JacobiSymbolTest()
            {

                var tests = getJacobiTests();

                tests.ToList().ForEach(x =>
                {
                    GmpInt n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.JacobiSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });

            }

            [TestMethod()]
            public void JacobiSymbolTest1()
            {
                var tests = getJacobiTests();

                tests.ToList().ForEach(x =>
                {
                    GmpInt n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        int k = i;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.JacobiSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void JacobiSymbolTest2()
            {
                var tests = getJacobiTests();

                tests.ToList().ForEach(x =>
                {
                    int n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.JacobiSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void JacobiSymbolTest3()
            {
                var tests = getJacobiTests();

                tests.ToList().ForEach(x =>
                {
                    GmpInt n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        uint k = (uint)i;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.JacobiSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void JacobiSymbolTest4()
            {
                var tests = getJacobiTests();

                tests.ToList().ForEach(x =>
                {
                    uint n = (uint)x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.JacobiSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            Dictionary<int, int[]> getKroneckerTests()
            {
                var tests = new Dictionary<int, int[]>()
            {
                {1, new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
                {2, new[] {1, 0, -1, 0, -1, 0, 1, 0, 1, 0, -1, 0, -1, 0, 1, 0, 1, 0, -1, 0, -1, 0, 1, 0, 1, 0, -1, 0, -1, 0}},
                {3, new[] {1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0}},
                {4, new[] {1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0}},
                {5, new[] {1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0}},
            };
                return tests;
            }

            [TestMethod()]
            public void KroneckerSymbolTest()
            {
                var tests = getKroneckerTests();

                tests.ToList().ForEach(x =>
                {
                    GmpInt n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i + 1;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.KroneckerSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void KroneckerSymbolTest1()
            {
                var tests = getKroneckerTests();

                tests.ToList().ForEach(x =>
                {
                    int n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i + 1;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.KroneckerSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void KroneckerSymbolTest2()
            {
                var tests = getKroneckerTests();

                tests.ToList().ForEach(x =>
                {
                    GmpInt n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        int k = i + 1;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.KroneckerSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void KroneckerSymbolTest3()
            {
                var tests = getKroneckerTests();

                tests.ToList().ForEach(x =>
                {
                    uint n = (uint)x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        GmpInt k = i + 1;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.KroneckerSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void KroneckerSymbolTest4()
            {
                var tests = getKroneckerTests();

                tests.ToList().ForEach(x =>
                {
                    GmpInt n = x.Key;
                    for (var i = 0; i < x.Value.Length; i++)
                    {
                        uint k = (uint)i + 1;
                        GmpInt expected = x.Value[i];
                        GmpInt result = GmpInt.KroneckerSymbol(k, n);
                        Assert.IsTrue(result == expected, $"Test failed for n:{n}, k:{k}, expected:{expected}, actual:{result}");
                    }

                });
            }

            [TestMethod()]
            public void RemoveFactorTest()
            {
                var a = GmpInt.Ten;
                var actual = a.RemoveFactor(GmpInt.Two);
                Assert.IsTrue(actual == GmpInt.Five);
            }

            [TestMethod()]
            public void RemoveFactorTest1()
            {
                var a = GmpInt.Ten;
                var actual = a.RemoveFactor(GmpInt.Two, out int count);
                Assert.IsTrue(actual == GmpInt.Five);
                Assert.IsTrue(count == 1);
            }

            [TestMethod()]
            public void FactorialTest()
            {
                var a = GmpInt.Factorial(3);
                Assert.IsTrue(a == (GmpInt)6);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Factorial(-1));
            }

            [TestMethod()]
            public void FactorialTest1()
            {
                var a = GmpInt.Factorial(3u);
                Assert.IsTrue(a == (GmpInt)6);
            }

            [TestMethod()]
            public void BinomialTest()
            {
                var n = GmpInt.Five;

                // Act
                GmpInt result = GmpInt.Binomial(n, 2);

                // Assert
                Assert.IsTrue(result == 10);
                Assert.IsTrue(result == GmpInt.Binomial(n, 2u));

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Binomial(n, -2));
            }

            [TestMethod()]
            public void BinomialTest1()
            {
                var n = 5;

                // Act
                GmpInt result = GmpInt.Binomial(n, 2);

                // Assert
                Assert.IsTrue(result == 10);


                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Binomial(n, -2));

                // Assert
                result = GmpInt.Binomial(-5, 2);
                Assert.IsTrue(result == 15);

                result = GmpInt.Binomial(-7, 5);
                Assert.IsTrue(result == -462);

            }

            [TestMethod()]
            public void BinomialTest2()
            {
                var n = 5u;

                // Act
                GmpInt result = GmpInt.Binomial(n, 2);

                // Assert
                Assert.IsTrue(result == 10);
            }

            [TestMethod()]
            public void BinomialTest3()
            {
                var n = 5u;

                // Act
                GmpInt result = GmpInt.Binomial(n, 2u);

                // Assert
                Assert.IsTrue(result == 10);
            }

            [TestMethod()]
            public void FibonacciTest()
            {
                GmpInt result = GmpInt.Fibonacci(3);
                Assert.IsTrue(result == 2);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Fibonacci(-1));
            }

            [TestMethod()]
            public void FibonacciTest1()
            {
                GmpInt result = GmpInt.Fibonacci(4u);
                Assert.IsTrue(result == 3);
            }

            [TestMethod()]
            public void FibonacciTest2()
            {
                GmpInt result = GmpInt.Fibonacci(3, out mpz_t previous);
                Assert.IsTrue(result == GmpInt.Two);
                Assert.IsTrue(previous == GmpInt.One);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Fibonacci(-1, out previous));
            }

            [TestMethod()]
            public void FibonacciTest3()
            {
                GmpInt result = GmpInt.Fibonacci(3u, out mpz_t previous);
                Assert.IsTrue(result == GmpInt.Two);
                Assert.IsTrue(previous == GmpInt.One);
            }

            [TestMethod()]
            public void LucasTest()
            {
                GmpInt result = GmpInt.Lucas(3);
                Assert.IsTrue(result == 4);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Lucas(-1));
            }

            [TestMethod()]
            public void LucasTest1()
            {
                GmpInt result = GmpInt.Lucas(3u);
                Assert.IsTrue(result == 4);
            }

            [TestMethod()]
            public void LucasTest2()
            {
                GmpInt result = GmpInt.Lucas(3, out mpz_t previous);
                Assert.IsTrue(result == 4);
                Assert.IsTrue(previous == GmpInt.Three);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.Lucas(-1, out previous));
            }

            [TestMethod()]
            public void LucasTest3()
            {
                GmpInt result = GmpInt.Lucas(3u, out mpz_t previous);
                Assert.IsTrue(result == 4);
                Assert.IsTrue(previous == GmpInt.Three);
            }

            [TestMethod()]
            public void CountOnesTest()
            {
                GmpInt result = GmpInt.Five.CountOnes();
                Assert.IsTrue(result == 2);
                Assert.IsTrue(result.BitLength == 2);

            }

            [TestMethod()]
            public void HammingDistanceTest()
            {
                var result = GmpInt.HammingDistance(GmpInt.Five, GmpInt.Two);
                Assert.IsTrue(result == 3);
            }

            [TestMethod()]
            public void IndexOfZeroTest()
            {
                var result = (GmpInt.Five + 1).IndexOfOne(0);
                Assert.IsTrue(result == 1);


                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.One.IndexOfOne(-1));
            }

            [TestMethod()]
            public void IndexOfOneTest()
            {
                var result = GmpInt.Five.IndexOfZero(0);
                Assert.IsTrue(result == 1);

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => GmpInt.One.IndexOfZero(-1));
            }


            [TestMethod()]
            public void GetBitTest()
            {
                Assert.IsTrue(GmpInt.Five.GetBit(0) == 1);
                Assert.IsTrue(GmpInt.Five.GetBit(1) == 0);
                Assert.IsTrue(GmpInt.Five.GetBit(2) == 1);
            }

            [TestMethod()]
            public void SetBitTest()
            {
                Assert.IsTrue(GmpInt.Zero.SetBit(0, 1) == 1);
                Assert.IsTrue(GmpInt.Zero.SetBit(1, 1) == 2);
                Assert.IsTrue(GmpInt.Zero.SetBit(2, 1) == 4);

                Assert.IsTrue(GmpInt.Five.SetBit(0, 0) == 4);
                Assert.IsTrue(GmpInt.Five.SetBit(2, 0) == 1);
            }

            [TestMethod()]
            public void EqualsModTest()
            {
                Assert.IsTrue(GmpInt.One.EqualsMod(GmpInt.Five, GmpInt.Two));
                Assert.IsFalse(GmpInt.One.EqualsMod(GmpInt.Five, GmpInt.Three));
            }

            [TestMethod()]
            public void EqualsModTests1()
            {
                Assert.IsTrue(GmpInt.One.EqualsMod(5, 2));

                Assert.IsTrue(GmpInt.MinusOne.EqualsMod(-5, 2));
                Assert.IsFalse(GmpInt.One.EqualsMod(5,3));

                Assert.ThrowsException<ArgumentOutOfRangeException>(() => (GmpInt.MinusOne.EqualsMod(5, -2)));
            }

            [TestMethod()]
            public void EqualsModTest2()
            {
                Assert.IsTrue(GmpInt.One.EqualsMod(5u, 2u));
                Assert.IsFalse(GmpInt.One.EqualsMod(5u, 3u));
            }

            [TestMethod()]
            public void CompareAbsToTest()
            {
                var a = GmpInt.MinusOne;

                Assert.IsTrue(a.CompareAbsTo(1) == 0);
                Assert.IsTrue(a.CompareAbsTo(1u) == 0);
                Assert.IsTrue(a.CompareAbsTo(1L) == 0);
                Assert.IsTrue(a.CompareAbsTo(1UL) == 0);
                Assert.IsTrue(a.CompareAbsTo(1f) == 0);
                Assert.IsTrue(a.CompareAbsTo(1.0) == 0);
                Assert.IsTrue(a.CompareAbsTo(1M) == 0);
                Assert.IsTrue(a.CompareAbsTo(BigInteger.One) == 0);
                Assert.IsTrue(a.CompareAbsTo(GmpInt.One) == 0);
                Assert.IsTrue(a.CompareAbsTo("1") == 0);


                Assert.IsTrue(a.CompareAbsTo((object)(sbyte)1) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)(byte)1) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)(short)1) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)(ushort)1) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)GmpInt.One.Data) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1u) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1L) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1UL) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1f) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1.0) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)1M) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)BigInteger.One) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)GmpInt.One) == 0);
                Assert.IsTrue(a.CompareAbsTo((object)"1") == 0);


                Assert.ThrowsException<ArgumentException>(()=>a.CompareAbsTo((object)DateTime.MinValue) == 0);

            }
        }
    }
}