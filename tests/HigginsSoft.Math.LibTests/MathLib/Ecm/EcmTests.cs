#define HAVE_ECM_AVX
#undef HAVE_ECM_AVX
#define SKIP_LONG_TESTS
//#undef SKIP_LONG_TESTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace HigginsSoft.Math.Lib.Tests
{
    [TestClass()]
    public class EcmTests
    {

        [TestMethod()]
        public void FactorPm1TestCompositsTo216()
        {
            Factorization f = new();
            int bits = 16;
            var comp = Composites.GenerateTo(1 << bits).ToArray();
            var pm1 = new Pm1();
            int count = 0;
            int factoredCount = 0;
            int firstFail = 0;

            int[] filterPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 21, 29 };
            Dictionary<int, int> fails = new();
            var sw = Stopwatch.StartNew();
            foreach (var c in comp)
            {
                count++;

                bool hasSmallFactor = false;
                foreach (var p in filterPrimes)
                {
                    if (c % p == 0)
                    {
                        hasSmallFactor = true;
                        break;
                    }

                }
                if (hasSmallFactor)
                {
                    factoredCount++;
                    continue;
                }

                f = pm1.FactorPm1((ulong)c);


                if (f.Factors.Count > 1)
                    factoredCount++;
                else
                {
                    if (firstFail == 0)
                        firstFail = c;
                    var f2 = FactorizationInt.FactorTrialDivide(c);
                    var min = f2.Factors.Min(x => x.P);
                    if (!fails.ContainsKey(min))
                        fails.Add(min, 1);
                    else
                        fails[min]++;
                }


            }
            sw.Stop();
            Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail.ToString("N0")}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");
            if (fails.Count > 0)
            {
                Console.WriteLine($"{fails.Count} b1 failures.");
                foreach (var kvp in fails.OrderBy(x => x.Key))
                {
                    Console.WriteLine($" => {kvp.Key} = {kvp.Value}");
                }
            }
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod()]
        public void FactorEcmTestCompositsTo216()
        {
            ulong rsa;
            Stopwatch sw;
            Factorization f = new();
            FactorizationInt f2;
            Ecm ecm = new();
            int bits = 16;
            var random = (ulong)MathLib.Random63();

            rsa = (ulong)MathLib.Rsa(bits);
            var comp = Composites.GenerateTo(1 << bits).ToArray();
            int factoredCount = 0;
            int count = 0;
            sw = Stopwatch.StartNew();
            int firstFail = 0;
            foreach (var c in comp)
            {
                count++;
                f = ecm.FactorEcm((ulong)c, 0, ref random);
                if (f.Factors.Count == 1)
                {
                    if (firstFail == 0)
                        firstFail = c;
                    f2 = FactorizationInt.FactorTrialDivide(c);
                    Console.WriteLine($"Factorization failed for {f2}");
                }
                else
                {
                    factoredCount++;
                }
            }

            sw.Stop();
            Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail.ToString("N0")}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");
        }

        public static ulong default_randomstate = 0xdeadbeef0badcafe;


        void ecmtestn(ulong value)
        {

            ulong rsa = value;
            Stopwatch sw;
            Factorization f = new();
            FactorizationInt f2;
            Ecm ecm = new();
            int bits = MathLib.BitLength(value);
            var random = default_randomstate;


            int factoredCount = 0;
            int count = 1;
            sw = Stopwatch.StartNew();
            GmpInt firstFail = 0;


            f = ecm.FactorEcm((ulong)rsa, 1, ref random);
            if (f.Factors.Count == 1)
            {
                if (firstFail == 0)
                    firstFail = rsa;
                f2 = FactorizationInt.FactorTrialDivide(rsa);
                Console.WriteLine($"Factorization failed for {f2}");
            }
            else
            {
                factoredCount++;
            }


            sw.Stop();
            if (firstFail > 0)
                Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");
        }
#if HAVE_ECM_AVX
        void ecmtestnavx(ulong value)
        {

            ulong rsa = value;
            Stopwatch sw;
            Factorization[] factorizations;
            FactorizationInt f2;
            Ecm ecm = new();
            int bits = MathLib.BitLength(value);
            var random = randomstate;
 
            var rsaArray = Enumerable.Range(0, Ecm.VectorSize256)
                .Select(i => rsa).ToArray();


            int factoredCount = 0;
            int count = 1;
            sw = Stopwatch.StartNew();
            GmpInt firstFail = 0;


            factorizations = ecm.FactorEcmAvx(rsaArray, 0, ref random);
            for (var i = 0; i < factorizations.Length; i++)
            {
                var f = factorizations[i];
                if (f.Factors.Count == 1)
                {
                    if (firstFail == 0)
                        firstFail = rsa;
                    f2 = FactorizationInt.FactorTrialDivide(rsa);
                    Console.WriteLine($"Factorization failed for {f2}");
                }
                else
                {
                    factoredCount++;
                }
            }



            sw.Stop();
            if (firstFail > 0)
                Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");
        }
#endif



#if HAVE_ECM_AVX
        [TestMethod]
        public void EcmTestAvxDefault()
        {
            var rsa = 2013536689;
            ecmtestnavx((ulong)rsa);
        }

#endif
        static void ecmtest(int bits)
        {
            if (bits > 63)
            {
                ecmtest((uint)bits);
                return;
            }
            ulong rsa;
            Stopwatch sw;
            Factorization f = new();
            Factorization f2;
            Ecm ecm = new();

            var random = default_randomstate;// (ulong)MathLib.Random63();

            rsa = (ulong)MathLib.Rsa(bits);

            int factoredCount = 0;
            int count = 1;
            sw = Stopwatch.StartNew();
            GmpInt firstFail = 0;


            f = ecm.FactorEcm((ulong)rsa, 0, ref random);
            if (f.Factors.Count == 1)
            {
                if (firstFail == 0)
                    firstFail = rsa;
                f2 = Factorization.Factor(rsa, false);
                Console.WriteLine($"Factorization failed for {f2}");
            }
            else
            {
                factoredCount++;
            }


            sw.Stop();
            if (firstFail > 0)
                Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");

        }

        static void ecmtestu(int bits)
        {
            if (bits > 63)
            {
                ecmtest((uint)bits);
                return;
            }
            ulong rsa;
            Stopwatch sw;
            Factorization f = new();
            FactorizationInt f2;
            EcmU ecm = new();

            var random = default_randomstate;// (ulong)MathLib.Random63();

            rsa = (ulong)MathLib.Rsa(bits);

            int factoredCount = 0;
            int count = 1;
            sw = Stopwatch.StartNew();
            GmpInt firstFail = 0;
            //243013

            f = ecm.FactorEcm((ulong)rsa, 1, ref random);
            if (f.Factors.Count == 1)
            {
                if (firstFail == 0)
                    firstFail = rsa;
                f2 = FactorizationInt.FactorTrialDivide(rsa);
                Console.WriteLine($"Factorization failed for {f2}");
            }
            else
            {
                factoredCount++;
            }


            sw.Stop();
            if (firstFail > 0)
                Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");

        }


#if HAVE_ECM_AVX
        static void ecmtestavx(int bits)
        {
            if (bits > 63)
            {
                ecmtest((uint)bits);
                return;
            }

            Stopwatch sw;
            Factorization[] factorizations;
            FactorizationInt f2;
            Ecm ecm = new();

            var random = (ulong)MathLib.Random63();
            var rsa = (ulong)MathLib.Rsa(bits);
            var rsaArray = Enumerable.Range(0, Ecm.VectorSize256)
                .Select(i => rsa).ToArray();


            int factoredCount = 0;
            int count = 1;
            sw = Stopwatch.StartNew();
            GmpInt firstFail = 0;


            factorizations = ecm.FactorEcmAvx(rsaArray, 0, ref random);
            for (var i = 0; i < factorizations.Length; i++)
            {
                var f = factorizations[i];
                if (f.Factors.Count == 1)
                {
                    if (firstFail == 0)
                        firstFail = rsa;
                    f2 = FactorizationInt.FactorTrialDivide(rsa);
                    Console.WriteLine($"Factorization failed for {f2}");
                }
                else
                {
                    factoredCount++;
                }
            }

            sw.Stop();
            if (firstFail > 0)
                Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");

        }
#endif  
        static void ecmtest(uint bits)
        {
            ulong rsa;
            Stopwatch sw;
            Factorization f = new();
            FactorizationInt f2;
            Ecm ecm = new();

            var random = (ulong)MathLib.Random63();

            rsa = (ulong)MathLib.Rsa((int)bits);

            int factoredCount = 0;
            int count = 1;
            sw = Stopwatch.StartNew();
            GmpInt firstFail = 0;


            f = ecm.FactorEcm((ulong)rsa, 0, ref random);
            if (f.Factors.Count == 1)
            {
                if (firstFail == 0)
                    firstFail = rsa;
                f2 = FactorizationInt.FactorTrialDivide(rsa);
                Console.WriteLine($"Factorization failed for {f2}");
            }
            else
            {
                factoredCount++;
            }


            sw.Stop();
            if (firstFail > 0)
                Console.WriteLine($"Ecm(rsa({bits})) first fail {firstFail}");
            Console.WriteLine($"Ecm(rsa({bits})) found {factoredCount.ToString("N0")} factors in {count.ToString("N0")} composites in {sw.Elapsed}");

        }

        [TestMethod]
        public void EcmTestUDefault()
        {
            //tinyecm(327496601) passes in yafu
            var rsa = 327496601;
            ecmtestn((ulong)rsa);
        }

        [TestMethod]
        public void TestEcmN2p1To2p32()
        {
            ulong[] tests = { 75151, 138689, 292499, 723301, 1428397, 2231401, 5717429, 10008871, 22910221,
                35619469, 76550623, 147375653, 294423761, 619766629, 1420171451};
            /*
                Factorization failed for 75151 = 223 ^ 1 * 337 ^ 1
                Factorization failed for 138689 = 331 ^ 1 * 419 ^ 1
                Factorization failed for 292499 = 367 ^ 1 * 797 ^ 1
                Factorization failed for 723301 = 821 ^ 1 * 881 ^ 1  
                Factorization failed for 1428397 = 761 ^ 1 * 1877 ^ 1         
                Factorization failed for 2231401 = 1123 ^ 1 * 1987 ^ 1         
                Factorization failed for 5717429 = 1429 ^ 1 * 4001 ^ 1             
                Factorization failed for 10008871 = 3119 ^ 1 * 3209 ^ 1
                Factorization failed for 22910221 = 3109 ^ 1 * 7369 ^ 1
                Factorization failed for 35619469 = 4481 ^ 1 * 7949 ^ 1
                Factorization failed for 76550623 = 6163 ^ 1 * 12421 ^ 1
                Factorization failed for 147375653 = 10657 ^ 1 * 13829 ^ 1
                Factorization failed for 294423761 = 12071 ^ 1 * 24391 ^ 1
                Factorization failed for 619766629 = 19751 ^ 1 * 31379 ^ 1
                Factorization failed for 1420171451 = 21683 ^ 1 * 65497 ^ 1
            */


            for (var i = 0; i < tests.Length; i++)
            {
                var test = tests[i];
                ecmtestn(test);
            }
        }

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod]
        public void TestEcmN2p32To2P62()
        {
            /*    
                Factorization failed for 3136124801 = 60251^1 * 52051^1
                Factorization failed for 5167274689 = 45659^1 * 113171^1
                Factorization failed for 12103265033 = 94793^1 * 127681^1
                Factorization failed for 21434202893 = 107339^1 * 199687^1
                Factorization failed for 39837156257 = 250301^1 * 159157^1
                Factorization failed for 107970692857 = 232709^1 * 463973^1
                Factorization failed for 149017567507 = 503317^1 * 296071^1
                Factorization failed for 332049835543 = 364373^1 * 911291^1
                Factorization failed for 934123623583 = 976109^1 * 956987^1
                Factorization failed for 1738138456193 = 2068811^1 * 840163^1
                Factorization failed for 3761484487351 = 1810597^1 * 2077483^1
                Factorization failed for 4510340639659 = 3829459^1 * 1177801^1
                Factorization failed for 16019832413987 = 3918139^1 * 4088633^1
                Factorization failed for 30033664368679 = 3840157^1 * 7820947^1
                Factorization failed for 44573349428147 = 5707397^1 * 7809751^1
                Factorization failed for 81447377220647 = 7714739^1 * 10557373^1
                Factorization failed for 250632391927963 = 15542509^1 * 16125607^1
                Factorization failed for 494179371521047 = 15241309^1 * 32423683^1
                Factorization failed for 791880097709549 = 27563863^1 * 28728923^1
                Factorization failed for 1363465386495533 = 24419401^1 * 55835333^1
                Factorization failed for 3362255880315763 = 55637563^1 * 60431401^1
                Factorization failed for 4569520691494763 = 50444269^1 * 90585527^1
                Factorization failed for 11634812465422717 = 95088229^1 * 122358073^1
                Factorization failed for 27745799608586429 = 130684271^1 * 212311699^1
                Factorization failed for 48733710664695769 = 199593179^1 * 244165211^1
                Factorization failed for 111981360983916631 = 505687979^1 * 221443589^1
                Factorization failed for 174573020431781029 = 534285221^1 * 326741249^1
                Factorization failed for 294838456761712723 = 277052249^1 * 1064198027^1
                Factorization failed for 929918276046718417 = 895529857^1 * 1038400081^1
                Factorization failed for 1444807327371595571 = 795175291^1 * 1816967081^1

            */
            ulong[] tests = { 3136124801, // =  60251^1 * 52051^1
                 5167274689, // =  45659^1 * 113171^1
                 12103265033, // =  94793^1 * 127681^1
                 21434202893, // =  107339^1 * 199687^1
                 39837156257, // =  250301^1 * 159157^1
                 107970692857, // =  232709^1 * 463973^1
                 149017567507, // =  503317^1 * 296071^1
                 332049835543, // =  364373^1 * 911291^1
                 934123623583, // =  976109^1 * 956987^1
                 1738138456193, // =  2068811^1 * 840163^1
                 3761484487351, // =  1810597^1 * 2077483^1
                 4510340639659, // =  3829459^1 * 1177801^1
                 16019832413987, // =  3918139^1 * 4088633^1
                 30033664368679, // =  3840157^1 * 7820947^1
                 44573349428147, // =  5707397^1 * 7809751^1
                 81447377220647, // =  7714739^1 * 10557373^1
                 250632391927963, // =  15542509^1 * 16125607^1
                 494179371521047, // =  15241309^1 * 32423683^1
                 791880097709549, // =  27563863^1 * 28728923^1
                 1363465386495533, // =  24419401^1 * 55835333^1
                 3362255880315763, // =  55637563^1 * 60431401^1
                 4569520691494763, // =  50444269^1 * 90585527^1
                 11634812465422717, // =  95088229^1 * 122358073^1
                 27745799608586429, // =  130684271^1 * 212311699^1
                 48733710664695769, // =  199593179^1 * 244165211^1
                 111981360983916631, // =  505687979^1 * 221443589^1
                 174573020431781029, // =  534285221^1 * 326741249^1
                 294838456761712723, // =  277052249^1 * 1064198027^1
                 929918276046718417, // =  895529857^1 * 1038400081^1
                 1444807327371595571, // =  795175291^1 * 1816967081^1
                };

            for (var i = 0; i < tests.Length; i++)
            {
                var test = tests[i];
                ecmtestn(test);
            }
        }

        [TestMethod()]
        public void FactorEcmTest2p17To2p32()
        {
            for (var i = 17; i < 32; i++)
            {
                ecmtest(i);
            }
        }

        [TestMethod()]
        public void FactorEcmUTest2p17To2p32()
        {
            for (var i = 17; i < 32; i++)
            {
                ecmtestu(i);
            }
        }

#if HAVE_ECM_AVX
        [TestMethod()]
        public void FactorEcmAvxTest2p17To2p32()
        {
            for (var i = 17; i < 32; i++)
            {
                ecmtestavx(i);
            }
        }
#endif

#if SKIP_LONG_TESTS
        [Ignore]
#endif
        [TestMethod()]
        public void FactorEcm2p32To2p62()
        {
            for (var i = 32; i < 62; i++)
            {
                ecmtest(i);
            }
        }

        [TestMethod()]
        public void FactorEcm2p63To2P64()
        {
            for (var i = 63; i <= 64; i++)
            {
                ecmtest(i);
            }
        }

    }
}