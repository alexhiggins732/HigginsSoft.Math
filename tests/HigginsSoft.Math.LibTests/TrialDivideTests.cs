using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib.Tests.PrimeTests
{
    [TestClass()]
    public class TrialDivideTests
    {

        int[] candidates_32 = new[] { 2, 3, 5 };
        uint[] candidates_64 = new[] { 2U, 3U, 5U };

        [DataRow(1, false, 1)]
        [DataRow(2, true, 2)]
        [DataRow(3, true, 3)]
        [DataRow(4, false, 2)]
        [DataRow(5, true, 5)]
        [DataRow(6, false, 2)]
        [DataRow(7, true, 7)]
        [DataRow(9, false, 3)]
        [TestMethod()]
        public void DataTests_Int(int n, bool expected, int factor)
        {
            var isprime = Primes.TrialDivide(n, out int actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [DataRow(1, false, 1)]
        [DataRow(2, true, 2)]
        [DataRow(3, true, 3)]
        [DataRow(4, false, 2)]
        [DataRow(5, true, 5)]
        [DataRow(6, false, 2)]
        [DataRow(7, true, 7)]
        [DataRow(9, false, 3)]
        [TestMethod()]
        public void DataTests_Int_Enumerable(int n, bool expected, int factor)
        {
            var isprime = Primes.TrialDivide(n, candidates_32, out int actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }


        [DataRow(1U, false, 1U)]
        [DataRow(2U, true, 2U)]
        [DataRow(3U, true, 3U)]
        [DataRow(4U, false, 2U)]
        [DataRow(5U, true, 5U)]
        [DataRow(6U, false, 2U)]
        [DataRow(7U, true, 7U)]
        [DataRow(9U, false, 3U)]
        [TestMethod()]
        public void DataTests_UInt(uint n, bool expected, uint factor)
        {
            var isprime = Primes.TrialDivide(n, out uint actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [DataRow(1U, false, 1U)]
        [DataRow(2U, true, 2U)]
        [DataRow(3U, true, 3U)]
        [DataRow(4U, false, 2U)]
        [DataRow(5U, true, 5U)]
        [DataRow(6U, false, 2U)]
        [DataRow(7U, true, 7U)]
        [DataRow(9U, false, 3U)]
        [TestMethod()]
        public void DataTests_UInt_Enumerable(uint n, bool expected, uint factor)
        {
            var isprime = Primes.TrialDivide(n, candidates_32, out uint actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }


        [DataRow(1L, false, 1L)]
        [DataRow(2L, true, 2L)]
        [DataRow(3L, true, 3L)]
        [DataRow(4L, false, 2L)]
        [DataRow(5L, true, 5L)]
        [DataRow(6L, false, 2L)]
        [DataRow(7L, true, 7L)]
        [DataRow(9L, false, 3L)]
        [TestMethod()]
        public void DataTests_Long(long n, bool expected, long factor)
        {
            var isprime = Primes.TrialDivide(n, out long actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [DataRow(1L, false, 1L)]
        [DataRow(2L, true, 2L)]
        [DataRow(3L, true, 3L)]
        [DataRow(4L, false, 2L)]
        [DataRow(5L, true, 5L)]
        [DataRow(6L, false, 2L)]
        [DataRow(7L, true, 7L)]
        [DataRow(9L, false, 3L)]
        [TestMethod()]
        public void DataTests_Long_Enumerable(long n, bool expected, long factor)
        {
            var isprime = Primes.TrialDivide(n, candidates_64, out long actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }




        [DataRow(1UL, false, 1UL)]
        [DataRow(2UL, true, 2UL)]
        [DataRow(3UL, true, 3UL)]
        [DataRow(4UL, false, 2UL)]
        [DataRow(5UL, true, 5UL)]
        [DataRow(6UL, false, 2UL)]
        [DataRow(7UL, true, 7UL)]
        [DataRow(9UL, false, 3UL)]
        [TestMethod()]
        public void DataTests_ULong(ulong n, bool expected, ulong factor)
        {
            var isprime = Primes.TrialDivide(n, out ulong actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [DataRow(1UL, false, 1UL)]
        [DataRow(2UL, true, 2UL)]
        [DataRow(3UL, true, 3UL)]
        [DataRow(4UL, false, 2UL)]
        [DataRow(5UL, true, 5UL)]
        [DataRow(6UL, false, 2UL)]
        [DataRow(7UL, true, 7UL)]
        [DataRow(9UL, false, 3UL)]
        [TestMethod()]
        public void DataTests_ULong_Enumerabled(ulong n, bool expected, ulong factor)
        {
            var isprime = Primes.TrialDivide(n, candidates_64, out ulong actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }




        public void DataTest_GmpInt(GmpInt n, bool expected, GmpInt factor)
        {
            var isprime = Primes.TrialDivide(n, out GmpInt actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [TestMethod()]
        public void DataTests_GmpInt()
        {
            DataTest_GmpInt(1, false, 1);
            DataTest_GmpInt(2, true, 2);
            DataTest_GmpInt(3, true, 3);
            DataTest_GmpInt(4, false, 2);
            DataTest_GmpInt(5, true, 5);
            DataTest_GmpInt(6, false, 2);
            DataTest_GmpInt(7, true, 7);
            DataTest_GmpInt(9, false, 3);
        }

        public void DataTest_GmpInt_IntEnumerable(GmpInt n, bool expected, GmpInt factor)
        {
            var isprime = Primes.TrialDivide(n, candidates_32, out GmpInt actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [TestMethod()]
        public void DataTests_GmpInt_IntEnumerable()
        {
            DataTest_GmpInt_IntEnumerable(1, false, 1);
            DataTest_GmpInt_IntEnumerable(2, true, 2);
            DataTest_GmpInt_IntEnumerable(3, true, 3);
            DataTest_GmpInt_IntEnumerable(4, false, 2);
            DataTest_GmpInt_IntEnumerable(5, true, 5);
            DataTest_GmpInt_IntEnumerable(6, false, 2);
            DataTest_GmpInt_IntEnumerable(7, true, 7);
            DataTest_GmpInt_IntEnumerable(9, false, 3);
        }

        public void DataTest_GmpInt_UIntEnumerable(GmpInt n, bool expected, GmpInt factor)
        {

            var isprime = Primes.TrialDivide(n, candidates_64, out GmpInt actual);
            Assert.AreEqual(expected, isprime, $"IsPrime Trial divide failed for {n}");
            Assert.AreEqual(factor, actual, $"Factor Trial divide failed for {n}");
        }

        [TestMethod()]
        public void DataTests_GmpInt_UIntEnumerable()
        {
            DataTest_GmpInt_IntEnumerable(1, false, 1);
            DataTest_GmpInt_IntEnumerable(2, true, 2);
            DataTest_GmpInt_IntEnumerable(3, true, 3);
            DataTest_GmpInt_IntEnumerable(4, false, 2);
            DataTest_GmpInt_IntEnumerable(5, true, 5);
            DataTest_GmpInt_IntEnumerable(6, false, 2);
            DataTest_GmpInt_IntEnumerable(7, true, 7);
            DataTest_GmpInt_IntEnumerable(9, false, 3);
        }


        [TestMethod()]
        public void RunIntTests()
        {
            var primes = Primes.IntFactorPrimes;
            var max = primes.Last();
            int actual;
            int j = 0;
            for (var i = 0; i <= max; i++)
            {
                var isprime = Primes.TrialDivide(i, out actual);
                var expected = i == primes[j];
                if (expected != isprime)
                {
                    string message = $"Trial divide failed for {i}";
                    Console.WriteLine(message);
                }

                Assert.AreEqual(expected, isprime, $"Trial divide failed for {i}");
                if (isprime)
                {
                    Assert.AreEqual(i, actual, $"Trial divide factor failed for {i}");
                }
                else if (i > 1)
                {
                    Assert.AreNotEqual(i, actual, $"Trial divide factor failed for {i}");
                    Assert.AreEqual(0, i % actual, $"Trial divide factor does not divide {i}");
                }
                else
                {
                    Assert.AreEqual(1, actual, $"Trial divide factor failed for {i}");
                }
                if (i == primes[j])
                {
                    j++;
                }

            }
        }


        [TestMethod()]
        public void RunIntEnumerableTests()
        {
            var primes = Primes.IntFactorPrimes;
            var max = primes.Last();
            int actual;
            int j = 0;
            for (var i = 0; i <= max; i++)
            {
                var isprime = Primes.TrialDivide(i, primes, out actual);
                var expected = i == primes[j];
                if (expected != isprime)
                {
                    string message = $"Trial divide failed for {i}";
                    Console.WriteLine(message);
                }

                Assert.AreEqual(expected, isprime, $"Trial divide failed for {i}");
                if (isprime)
                {
                    Assert.AreEqual(i, actual, $"Trial divide factor failed for {i}");
                }
                else if (i > 1)
                {
                    Assert.AreNotEqual(i, actual, $"Trial divide factor failed for {i}");
                    Assert.AreEqual(0, i % actual, $"Trial divide factor does not divide {i}");
                }
                else
                {
                    Assert.AreEqual(1, actual, $"Trial divide factor failed for {i}");
                }
                if (i == primes[j])
                {
                    j++;
                }

            }
        }


    }
}