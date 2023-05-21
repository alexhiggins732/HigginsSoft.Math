using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HigginsSoft.Math.Lib.Tests.PrimalityCheckTests;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;

namespace HigginsSoft.Math.Lib.Tests.FermatTests
{
    [TestClass()]
    public class FermatTest
    {

        [TestMethod()]
        public void TestResumableFermatTest()
        {
            var testData = PrimeTestDataGenerator.GetSmallPrimes();
            var testStates = new Fermat.FactorizationState[testData.Count];
            for ( var i = 0; i < testData.Count; i++)
            {
                var test = testData[i];
                var state = Fermat.StartResumable(test.N, 100);
                testStates[i] = state;
            }

            testStates.ForEach(x => Fermat.Resume(x, 7400));
            int minFail = 0;
            int failCount = 0;
            for (var i = 0; i < testStates.Length; i++)
            {
                var test = testData[i];
                var state = testStates[i];
                var result = state.HasFactor;
                var p = state.P;
                var q = state.Q;
                if (result)
                {
                    Assert.AreEqual(test.N, p * q, $"Fermat returned the wrong factors for {test.N}");
                }
                else
                {
                    if (p > 0 || q > 0)
                    {
                        Assert.IsFalse(test.IsPrime, $"Fermat returned a false result but had factors set to {p}, {q} for {test.N} after {state.Iterations} iterations");
                    }
                    else
                    {
                        if (!test.IsPrime)
                        {
                            failCount++;
                            if (minFail == 0)
                            {
                                minFail = test.N;
                            }
                        }

                        Assert.IsTrue(test.IsPrime, $"Fermat failed to find factor for {(test.IsPrime ? "prime" : "composite")} {test.N}");
                    }
                }
            }
        }


        [TestMethod()]
        public void TestFermatTest()
        {
            var testData = PrimeTestDataGenerator.GetSmallPrimes();
            var sb = new StringBuilder();
            sb.AppendLine($"iterations\tfailCount\ttestData.Count\tpct\tminfail");
            for (var iterations = 7500; iterations <= 7500; iterations += 100)
            {
                int failCount = 0;
                int minFail = 0;
                for (var i = 2; i < testData.Count; i++)
                {
                    var test = testData[i];
                    int candidate = test.N;
                    var result = Fermat.FermatFactorization(candidate, out int p, out int q, iterations);
                    if (result)
                    {
                        Assert.AreEqual(test.N, p * q, $"Fermat returned the wrong factors for {test.N}");
                    }
                    else
                    {
                        if (p > 0 || q > 0)
                        {
                            Assert.IsFalse(test.IsPrime, $"Fermat returned a false for is prime with factors {p}, {q} for {test.N}");
                        }
                        else
                        {
                            if (!test.IsPrime)
                            {
                                failCount++;
                                if (minFail == 0)
                                {
                                    minFail = candidate;
                                }
                            }


                            //Assert.IsTrue(test.IsPrime, $"Fermat failed to find factor for {(test.IsPrime ? "prime" : "composite")} {candidate}");
                        }
                    }
                }
                var pct = (double)failCount / testData.Count;

                Console.WriteLine($"{iterations} Fermat iterations failed to find factor for {failCount} candidates of {testData.Count} ({pct.ToString("P")}) starting at {minFail}");
                sb.AppendLine($"{iterations}\t{failCount}\t{testData.Count}\t{pct.ToString("P")}\t{minFail}");

            }
            Console.WriteLine(sb.ToString());

        }
    }
}