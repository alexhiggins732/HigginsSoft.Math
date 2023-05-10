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
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests.GmpFloatTests
{
    [TestClass()]
    public class GmpConvertibleTests
    {
        [TestMethod()]
        public void GmpFloat_TestConverters()
        {
            var converted = new GmpIntConvertible[]
            {
                1,
                1U,
                1L,
                1UL,
                1f,
                1.0,
                1M,
                BigInteger.One,
                (GmpIntConvertible)GmpInt.One,
                (GmpIntConvertible)GmpFloat.One
            };
            converted.ForEach(x =>
                Assert.IsTrue(x.Value.IsOne)
            );
          

        }
    }

}