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
using System.Reflection.Metadata;

namespace HigginsSoft.Math.Lib.Tests
{



    [TestClass]
    public class IlogTests
    {
        public const int limit = 2 << 16;
        [TestMethod]
        public void ILogB_Int_Tests()
        {
            for (var i = -8; i < limit; i++)
            {
                double d = i;
                int dLog2 = MathLib.ILogB(d);
                int iLog2 = MathLib.ILogB(i);

                if (dLog2 != iLog2)
                {
                    string message = $"IlogB failed for {i}";
                    Assert.AreEqual(dLog2, iLog2, message);
                }
            }
        }


        [TestMethod]
        public void ILogB_GmpInt_Tests()
        {
            for (var i = -8; i < limit; i++)
            {
                double d = i;
                int dLog2 = MathLib.ILogB(d);
                int iLog2 = MathLib.ILogB((GmpInt)i);

                if (dLog2 != iLog2)
                {
                    string message = $"IlogB failed for {i}";
                    Assert.AreEqual(dLog2, iLog2, message);
                }
            }
        }

    }

    [TestClass]
    public class Log2Tests
    {
        public const int limit = 2 << 16;

        [TestMethod]
        public void Log2_Int_Tests()
        {
            for (var i = -8; i < limit; i++)
            {
                double d = i;
                double dLog2 = MathLib.Log2(d);
                double iLog2 = MathLib.ILogB(i);

                if ((int)dLog2 != iLog2)
                {
                    string message = $"IlogB failed for {i}";
                    Assert.AreEqual(dLog2, iLog2, message);
                }
            }
        }


        [TestMethod]
        public void Log2_GmpInt_Tests()
        {
            double maxError = 0.000001;
            for (var i = -8; i < limit>>2; i++)
            {
                double d = i;
                var dLog2 = MathLib.Log2(d);
                var iLog2 = MathLib.Log2((GmpInt)i);

                if (dLog2 != iLog2 && MathLib.Abs(dLog2 - iLog2) > maxError)
                {
                    string message = $"Log2 failed for {i}";

                    Assert.AreEqual(dLog2, iLog2, message);
                }
            }
        }


        [TestMethod]
        public void Log_GmpInt_Tests()
        {
            for (var i = -8; i < limit>>2; i++)
            {
                double d = i;
                var dLog = MathLib.Log(d);
                var iLog = MathLib.Log((GmpInt)i);

                if (dLog != iLog)
                {
                    string message = $"Log failed for {i}";

                    Assert.AreEqual(dLog, iLog, message);
                }
            }
        }



        [TestMethod]
        public void Log2_Rsa_ApproxRoot_Tests()
        {
            int bits = 1024;
            var rsa = MathLib.Rsa(bits);

            for (var i = 1; i < 10; i++)
            {
                var root = MathLib.Sqrt(rsa * i);
                var dLog = MathLib.Log2(i * root);
                var expected = (i>>1) + (bits>>1);
                double maxError = 1.5;
                var error = MathLib.Abs(expected - dLog);
                if (expected != dLog && error > maxError)
                {
                    string message = $"Log2 failed for {i} - error: {error}";

                    Assert.AreEqual(expected, dLog, message);
                }
            }
        }

    }
}