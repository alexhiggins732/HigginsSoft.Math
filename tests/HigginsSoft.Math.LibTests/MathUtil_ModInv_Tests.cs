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

namespace HigginsSoft.Math.Lib.Tests
{

    namespace MathUtilTests
    {
        using static MathUtil;
        [TestClass]
        public class MathUtil_ModInv_Tests
        {
            [TestMethod]
            public void ModInv_WhenHasInv()
            {

                var a = 11;
                var b = 26;
                var expected = 19;
                var result = ModInverse(a, b);
                Assert.AreEqual(expected, result);  
                  
            }

            [TestMethod]
            public void ModInv_WhenHasNoInv()
            {
                var a = 8;
                var b = 10;
                var result = ModInverse(a, b);
                Assert.AreEqual(a, result);
            }
        }
    }
}