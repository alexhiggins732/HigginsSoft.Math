using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsSoft.Math.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Demos.Tests
{
    [TestClass()]
    public class LehmanTests
    {
        [TestMethod()]
        public void RunTest()
        {



            // These test number were too hard for previous versions:
            long[] testNumbers = new long[] {
                5640012124823L,
                7336014366011L,
                19699548984827L,
                52199161732031L,
                73891306919159L,
                112454098638991L,

                32427229648727L,
                87008511088033L,
                92295512906873L,
                338719143795073L,
                346425669865991L,
                1058244082458461L,
                1773019201473077L,
                6150742154616377L,

                44843649362329L,
                67954151927287L,
                134170056884573L,
                198589283218993L,
                737091621253457L,
                1112268234497993L,
                2986396307326613L,

                26275638086419L,
                62246008190941L,
                209195243701823L,
                290236682491211L,
                485069046631849L,
                1239671094365611L,
                2815471543494793L,
                5682546780292609L,
            };


            ulong n;
            long m;
            var lehman = new Lehman();

            long LehmanFactor(ulong N, double Tune, double HartOLF, bool DoTrial, double CutFrac)
                => (long)lehman.LehmanFactor(N, Tune, HartOLF, DoTrial, CutFrac);

            //Here are some typical calls to LehmanFactor.
            //  LehmanFactor(N, (tune from 0.1 to 9.6), (tune from 0 to 5.0),
            //            (TRUE unless want to skip trial factoring which would be unusual), 
            //            (TRUE if want to try OLF speculative speedup FALSE if skip it) );
            n = 3141592651;
            m = LehmanFactor(n, 2.5, 0.0, true, 0.4);
            Console.WriteLine($"A factor of {n} is {m}.", n, m);

            n = 3141592661; //prime
            m = LehmanFactor(n, 2.5, 0.0, true, 0.5);
            Console.WriteLine($"A factor of {n} is {m}.", n, m);

            n = 7919; n *= 10861;
            m = LehmanFactor(n, 1.0, 0.0, true, 0.1);
            Console.WriteLine($"A factor of {n} is {m}.", n, m);

            n = 1299709; n *= 2750159;
            m = LehmanFactor(n, 1.0, 0.0, true, 0.1);
            Console.WriteLine($"A factor of {n} is {m}.", n, m);
            Console.WriteLine("All done.");


            foreach (var test in testNumbers)
            {
                long factor = lehman.LehmanFactor((ulong)test, .3, false, 1);
                m = LehmanFactor((ulong)test, 1.0, 0.0, true, 0.1);
                if (m == test || m==1)
                    m = 0;
                if (factor != m)
                {
                    Console.WriteLine($"A factor of {test} is {m}.");
                    Console.WriteLine($"N={test} has factor {factor}");
                }
            }
        }
    }
}