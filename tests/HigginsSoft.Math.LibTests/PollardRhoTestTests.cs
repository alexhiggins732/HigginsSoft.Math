#define SKIP_LONG_TESTS

using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace HigginsSoft.Math.Lib.Tests
{
    namespace PollardRho
    {
        [TestClass()]
        public class PollardRhoTests
        {
            void RunI63Tests(int start, int limit, int num_witnesses = 20)
            {

                var gen = Composites.GenerateTo(start, limit).ToList();
                int fails = 0;
                int minFail = 0;
                int factored = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < gen.Count; i++)
                {
                    var c = gen[i];
                    var result = MathLib.PollardRho63(c, num_witnesses);
                    if (result == 1 || result == c)
                    {
                        if (minFail == 0) { minFail = c; }
                        fails++;
                    }
                    else { factored++; }
                }
                sw.Stop();

                Console.WriteLine($"Rho factored {factored} of {gen.Count} composites between {start} and {limit} in {sw.Elapsed}.");
                Console.WriteLine($" -> {fails} failures starting at {minFail} using {num_witnesses} witnesses");
            }

            void RunZOldTests(int start, int limit, int num_witnesses = 20)
            {

                var gen = Composites.GenerateTo(start, limit).ToList();
                int fails = 0;
                int minFail = 0;
                int factored = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < gen.Count; i++)
                {
                    var c = gen[i];
                    var result = MathLib.PollardRhoZOld(c, num_witnesses);
                    if (result == 1 || result == c)
                    {
                        if (minFail == 0) { minFail = c; }
                        fails++;
                    }
                    else { factored++; }
                }
                sw.Stop();

                Console.WriteLine($"Rho factored {factored} of {gen.Count} composites between {start} and {limit} in {sw.Elapsed}.");
                Console.WriteLine($" -> {fails} failures starting at {minFail} using {num_witnesses} witnesses");
            }

            void RunZTests(int start, int limit, int num_witnesses = 20)
            {

                var gen = Composites.GenerateTo(start, limit).ToList();
                int fails = 0;
                int minFail = 0;
                int factored = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < gen.Count; i++)
                {
                    var c = gen[i];
                    var result = MathLib.PollardRhoZ(c, num_witnesses);
                    if (result == 1 || result == c)
                    {
                        if (minFail == 0)
                        {
                            minFail = c;
#if VERIFY_WITH_RHO63
                            var res63 = MathLib.PollardRho63(c, num_witnesses);
                            if (res63 > 1 && res63 < c)
                            {
                                string bp = "63 found z failure";
                                Console.WriteLine($" -> {bp}");
                            }
#endif
                        }
                        fails++;
                    }
                    else { factored++; }
                }
                sw.Stop();

                Console.WriteLine($"Rho factored {factored} of {gen.Count} composites between {start} and {limit} in {sw.Elapsed}.");
                Console.WriteLine($" -> {fails} failures starting at {minFail} using {num_witnesses} witnesses");
            }


            void RunCTests(int start, int limit, int poly, int num_witnesses = 20)
            {
                var gen = Composites.GenerateTo(start, limit).ToList();

                int fails = 0;
                int minFail = 0;
                int factored = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < gen.Count; i++)
                {
                    var c = gen[i];
                    var result = MathLib.PollardRhoC(c, poly, num_witnesses);
                    if (result == 1 || result == c)
                    {
                        if (minFail == 0) { minFail = c; }
                        fails++;
                    }
                    else { factored++; }
                }
                sw.Stop();

                Console.WriteLine($"Rho(x^2+{poly}) factored {factored} of {gen.Count} composites between {start} and {limit} in {sw.Elapsed}.");
                Console.WriteLine($" -> {fails} failures starting at {minFail} using {num_witnesses} witnesses");

            }

            void RunAvxTests(int start, int limit, int num_witnesses = 20)
            {
                var gen = Composites.GenerateTo(start, limit).ToList();



                int fails = 0;
                int minFail = 0;
                int factored = 0;
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < gen.Count; i++)
                {
                    var c = gen[i];
                    var result = MathLib.PollardRho31Avx(c, num_witnesses);
                    if (result == 1 || result == c)
                    {
                        result = MathLib.PollardRho31(c);
                        if (minFail == 0) { minFail = c; }
                        fails++;
                    }
                    else { factored++; }
                }
                sw.Stop();

                Console.WriteLine($"RhoAvx factored {factored} of {gen.Count} composites between {start} and {limit} in {sw.Elapsed}.");
                Console.WriteLine($" -> {fails} failures starting at {minFail} using {num_witnesses} witnesses");

            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsRsa65BigIntegerTest()
            {
                BigInteger rsa65 = BigInteger.Parse("25063116026386232513");
                int num_witnesses = 20;
                var result = MathLib.PollardRho(rsa65, num_witnesses);

                if (result == 1 || result == rsa65)
                {
                    string message = $"Rho failed for {nameof(rsa65)} with {num_witnesses} witnesses ";
                    Assert.AreNotEqual(rsa65, result, message);
                }


                /*    
                Rho factored 58992 of 58992 composites between 4 and 65536 in 00:00:00.0083748.
                -> 0 failures starting at 0 using 20 witnesses
                */
            }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
            [TestMethod()]
            public void RunTestsRsa126igIntegerTest()
            {


                // var rsa126 = BigInteger.Parse("46935327687222996017896600397399584183");
                var rsa65 = BigInteger.Parse("25063116026386232513");
                var rsa70 = BigInteger.Parse("871275967667700283483");
                var rsa75 = BigInteger.Parse("23790073920099416851127");
                var rsa80 = BigInteger.Parse("857524652453087263265749");
                var rsa85 = BigInteger.Parse("21380067156139824770407831");
                var rsa90 = BigInteger.Parse("1049015384649796327872365419");
                var tests = new Dictionary<int, BigInteger>
                {
                    {65,rsa65 }, {70,rsa70},{75,rsa75 }, {80,rsa80},  {85,rsa85}, {90, rsa90}
                };

                int bitLimit = 100;
         
                foreach (var t in tests)
                {
                    var bits = t.Key;
                    if (bits > bitLimit) break;

                    var sw = Stopwatch.StartNew();
                    var c = t.Value;
     
                    int num_witnesses = (int)bits * bits;

                    var result = MathLib.PollardRho(c, num_witnesses);

                    if (result == 1 || result == c)
                    {
                        string message = $"Rho failed for {nameof(rsa65)} with {num_witnesses} witnesses ";
                        Assert.AreNotEqual(rsa65, result, message);
                    }

                    Console.WriteLine($"Rho({bits}) took {sw.Elapsed}");

                }

                /*    
                Rho factored 58992 of 58992 composites between 4 and 65536 in 00:00:00.0083748.
                -> 0 failures starting at 0 using 20 witnesses
                */
            }




            [TestMethod()]
            public void RunTestsI63TestTo2P16()
            {
                RunI63Tests(4, 1 << 16);
                /*    
                Rho factored 58992 of 58992 composites between 4 and 65536 in 00:00:00.0083748.
                -> 0 failures starting at 0 using 20 witnesses
                */
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsI63TestTo2P16To2P24()
            {
                RunI63Tests(1 << 16, 1 << 24);
                /*    
                Rho factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.1685273.
                -> 0 failures starting at 0 using 20 witnesses
                */
            }


            [TestMethod()]
            public void RunTestsI63TestTo2P16WithWitnesses()
            {
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunI63Tests(4, 1 << 16, count);
                }
                /* 
                 Rho factored 58800 of 58992 composites between 4 and 65536 in 00:00:00.0102138.
                  -> 192 failures starting at 35 using 5 witnesses
                 Rho factored 58801 of 58992 composites between 4 and 65536 in 00:00:00.0104929.
                  -> 191 failures starting at 9 using 10 witnesses
                 Rho factored 58778 of 58992 composites between 4 and 65536 in 00:00:00.0098652.
                  -> 214 failures starting at 51 using 20 witnesses
                 Rho factored 58779 of 58992 composites between 4 and 65536 in 00:00:00.0098765.
                  -> 213 failures starting at 9 using 40 witnesses
                */
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsI63Test2p16To2P24WithWitnesses()
            {
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunI63Tests(1 << 16, 1 << 24, count);
                }
                /* 
                Rho factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.1523129.
                 -> 0 failures starting at 0 using 5 witnesses
                Rho factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.0572052.
                 -> 0 failures starting at 0 using 10 witnesses
                Rho factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.0752197.
                 -> 0 failures starting at 0 using 20 witnesses
                Rho factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.0841897.
                 -> 0 failures starting at 0 using 40 witnesses
                */
            }

            [TestMethod()]
            public void RunTestsAvxTestTo2P16WithWitnesses()
            {
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunAvxTests(4, 1 << 16, count);
                }
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsAvxTest2P16To2P24WithWitnesses()
            {
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunAvxTests(1 << 16, 1 << 24, count);
                }
                /*
                    RhoAvx factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.2250266.
                        -> 0 failures starting at 0 using 5 witnesses
                    RhoAvx factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.1286132.
                        -> 0 failures starting at 0 using 10 witnesses
                    RhoAvx factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.1197578.
                        -> 0 failures starting at 0 using 20 witnesses
                    RhoAvx factored 15640351 of 15640351 composites between 65536 and 16777216 in 00:00:03.1142953.
                        -> 0 failures starting at 0 using 40 witnesses
                 */
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsZOldTestTo2P16WithWitnesses()
            {
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunZOldTests(4, 1 << 16, count);
                }
                /* - GMP int much slower. This method is not optimized (using default operators)
                    Rho factored 58778 of 58992 composites between 4 and 65536 in 00:00:01.1073423.
                     -> 214 failures starting at 9 using 5 witnesses
                    Rho factored 58763 of 58992 composites between 4 and 65536 in 00:00:01.0616953.
                     -> 229 failures starting at 9 using 10 witnesses
                    Rho factored 58770 of 58992 composites between 4 and 65536 in 00:00:01.0765440.
                     -> 222 failures starting at 9 using 20 witnesses
                    Rho factored 58786 of 58992 composites between 4 and 65536 in 00:00:01.1210127.
                     -> 206 failures starting at 9 using 40 witnesses
                */
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsZTestTo2P16WithWitnesses()
            {
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunZTests(4, 1 << 16, count);
                }
                /* - GMP int much slower. This method IS optimized using low-level gmp_lib. 2-4x speed up.
                 * Still 10x slower than using long, which is of course slower than int.
                Rho factored 58775 of 58992 composites between 4 and 65536 in 00:00:00.3179230.
                 -> 217 failures starting at 9 using 5 witnesses
                Rho factored 58757 of 58992 composites between 4 and 65536 in 00:00:00.2786337.
                 -> 235 failures starting at 9 using 10 witnesses
                Rho factored 58757 of 58992 composites between 4 and 65536 in 00:00:00.2263579.
                 -> 235 failures starting at 9 using 20 witnesses
                Rho factored 58770 of 58992 composites between 4 and 65536 in 00:00:00.2569497.
                 -> 222 failures starting at 55 using 40 witnesses
                */
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsCPlusOneTestTo2P16WithWitnesses()
            {
                var poly = 1;
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunCTests(4, 1 << 16, poly, count);
                }
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsCPlusTwoTestTo2P16WithWitnesses()
            {
                var poly = 2;
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunCTests(4, 1 << 16, poly, count);
                }
            }

#if SKIP_LONG_TESTS
            [Ignore]
#endif
            [TestMethod()]
            public void RunTestsCPlusThreeTestTo2P16WithWitnesses()
            {
                var poly = 3;
                for (int count = 5; count <= 40; count <<= 1)
                {
                    //if (count == 20) continue;// don't repeat default test
                    RunCTests(4, 1 << 16, poly, count);
                }
            }


        }
    }
}