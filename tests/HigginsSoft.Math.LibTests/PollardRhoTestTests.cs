using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib.Tests
{
    namespace PollardRho
    {
        [TestClass()]
        public class PollardRhoTests
        {
            [TestMethod()]
            public void RunTestsTest()
            {
                for (var i = 0; i < 65536; i++)
                {
                    var result = MathLib.PollardRho(i);
                    var isPrime = MathLib.IsPrime(result);
                }
            }
        }
    }
}