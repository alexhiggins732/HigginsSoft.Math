﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib.Tests.CompositesTests
{
    [TestClass()]
    public class CompositesTest
    {
        [TestMethod()]
        public void GenerateTest()
        {
            var composites = Composites.GenerateWithUniqueFactors(20);
            foreach (var composite in composites)
            {
                Console.WriteLine(composite);
            }
        }

        [TestMethod()]
        public void GenerateAllTest()
        {
            var composites = Composites.GenerateAll(20);
            foreach (var composite in composites)
            {
                Console.WriteLine(composite);
            }
        }

        [TestMethod()]
        public void GenerateAllCountTest()
        {
            var expected = 20;
            var actual = Composites.GenerateAll(20).Count();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GenerateToCountTest()
        {
            for (var i = 1; i < 8; i++)
            {
                var count = PrimeData.Counts[i].Count;
                var n = 1 << i;
                var limit = n - count;
                var actual = Composites.GenerateTo(n).Count();
                var expected = n - count - 1; // 1 to exclude 1 not being generated by the generator.
                Assert.AreEqual(expected, actual, $"There are {count} primes and {limit} composites below {n}");
            }

        }
    }
}