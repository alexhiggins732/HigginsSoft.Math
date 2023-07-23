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

namespace HigginsSoft.Math.Lib.Tests
{

    namespace ExtensionTests
    {
        [TestClass]
        public class IlogTests
        {
            [TestMethod]
            public void Extensions_TestHexConversion()
            {
                var rng = Enumerable.Range(0, 16);
                var hexValues = rng.Select(x => x.ToString("X")).ToArray();
                var bytes = hexValues.Select(x => x.HexToByteArray().First());
                var rngBytes = rng.Select(x => (byte)x).ToArray();
                var equal = bytes.SequenceEqual(rngBytes);
                Assert.IsTrue(equal);



            }

            [TestMethod]
            public void Extensions_TestAlaises()
            {
                Assert.IsTrue(typeof(byte).Alias() == "byte");
                Assert.IsTrue(typeof(byte).Alias() == "byte");
                Assert.IsTrue(typeof(sbyte).Alias() == "sbyte");
                Assert.IsTrue(typeof(short).Alias() == "short");
                Assert.IsTrue(typeof(ushort).Alias() == "ushort");
                Assert.IsTrue(typeof(int).Alias() == "int");
                Assert.IsTrue(typeof(uint).Alias() == "uint");
                Assert.IsTrue(typeof(long).Alias() == "long");
                Assert.IsTrue(typeof(ulong).Alias() == "ulong");
                Assert.IsTrue(typeof(float).Alias() == "float");
                Assert.IsTrue(typeof(double).Alias() == "double");
                Assert.IsTrue(typeof(decimal).Alias() == "decimal");
                Assert.IsTrue(typeof(object).Alias() == "object");
                Assert.IsTrue(typeof(bool).Alias() == "bool");
                Assert.IsTrue(typeof(char).Alias() == "char");
                Assert.IsTrue(typeof(string).Alias() == "string");
                Assert.IsTrue(typeof(void).Alias() == "void");
                Assert.IsTrue(typeof(byte?).Alias() == "byte?");
                Assert.IsTrue(typeof(sbyte?).Alias() == "sbyte?");
                Assert.IsTrue(typeof(short?).Alias() == "short?");
                Assert.IsTrue(typeof(ushort?).Alias() == "ushort?");
                Assert.IsTrue(typeof(int?).Alias() == "int?");
                Assert.IsTrue(typeof(uint?).Alias() == "uint?");
                Assert.IsTrue(typeof(long?).Alias() == "long?");
                Assert.IsTrue(typeof(ulong?).Alias() == "ulong?");
                Assert.IsTrue(typeof(float?).Alias() == "float?");
                Assert.IsTrue(typeof(double?).Alias() == "double?");
                Assert.IsTrue(typeof(decimal?).Alias() == "decimal?");
                Assert.IsTrue(typeof(bool?).Alias() == "bool?");
                Assert.IsTrue(typeof(char?).Alias() == "char?");

                Assert.IsTrue(typeof(GmpInt).Alias() == "GmpInt");
            }
    }

}
}