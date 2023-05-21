/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

namespace HigginsSoft.Math.Lib
{

    /// <summary>
    /// Prime data with Max Prime and Next Prime stored as <see cref="long"/> values. 
    /// </summary>
    public class PrimeData
    {
        /// <summary>
        /// Largest integer prime, the largest prime less than 2^31
        /// </summary>
        public const int MaxIntPrime = 2147483647;

        /// <summary>
        /// Largest unsigned integer prime, the largest prime less than 2^32
        /// </summary>
        public const uint MaxUintPrime = 4294967291;

        /// <summary>
        /// Largest long integer prime, the largest prime less than 2^63
        /// </summary>
        public const long MaxLongPrime = 9223372036854775783;

        /// <summary>
        /// Largest unsigned long integer prime, the largest prime less than 2^64
        /// </summary>
        public const ulong MaxULongPrime = 18446744073709551557;

        /// <summary>
        /// The largest prime needed to factor int values.
        /// </summary>
        public const int MaxIntFactorPrime = 46337;

        /// <summary>
        /// The largest prime needed to factor uint values.
        /// </summary>
        public const int MaxUintFactorPrime = 65521;

        /// <summary>
        /// The largest prime needed to factor long values.
        /// </summary>
        public const uint MaxLongFactorPrime = 3_037_000_493;


        /// <summary>
        /// The largest prime needed to factor unsigned long values.
        /// </summary>
        public const uint MaxUlongFactorPrime = 4_294_967_291;

        /// <summary>
        /// Primes needed to factor or test primality of integers.
        /// </summary>
        public static readonly int[] IntFactorPrimes = new PrimeGeneratorUnsafe(MaxIntFactorPrime).ToArray();



        /// <summary>
        /// Primes needed to factor or test primality of integers.
        /// </summary>
        public static readonly int[] UintFactorPrimes = new PrimeGeneratorUnsafe(MaxUintFactorPrime).ToArray();

        /// <summary>
        /// Prime data for values up to 2^32, with primes stored <see cref="long"/> values. 
        /// </summary>
        /// <remarks>Manually calculated using HigginsSoft.Math and verified using Yafu</remarks>
        public static Dictionary<int, PrimeData> Counts = (new PrimeData[]
        {
            new(1,1, 2,3),
            new(2,2, 3,5),
            new(3,4, 7,11),
            new(4,6, 13,17),
            new(5,11, 31,37),
            new(6,18, 61,67),
            new(7,31, 127,131),
            new(8,54, 251,257),
            new(9,97, 509,521),
            new(10,172, 1021,1031),
            new(11,309, 2039,2053),
            new(12,564, 4093,4099),
            new(13,1028, 8191,8209),
            new(14,1900, 16381,16411),
            new(15,3512, 32749,32771),
            new(16,6542, 65521,65537),
            new(17,12251, 131071,131101),
            new(18,23000, 262139,262147),
            new(19,43390, 524287,524309),
            new(20,82025, 1048573,1048583),
            new(21,155611, 2097143,2097169),
            new(22,295947, 4194301,4194319),
            new(23,564163, 8388593,8388617),
            new(24,1077871, 16777213,16777259),
            new(25,2063689, 33554393,33554467),
            new(26,3957809, 67108859,67108879),
            new(27,7603553, 134217689,134217757),
            new(28,14630843, 268435399,268435459),
            new(29,28192750, 536870909,536870923),
            new(30,54400028, 1073741789,1073741827),
            new(31,105097565, 2147483647,2147483659),
            new(32,203280221, 4294967291,4294967311),


        }).ToDictionary(x => x.Bits, x => x);

        public Dictionary<int, GmpInt> PrimesBelow2pN = new Dictionary<int, GmpInt>()
        {
                {1,"1"},
                {2,"2"},
                {3,"4"},
                {4,"6"},
                {5,"11"},
                {6,"18"},
                {7,"31"},
                {8,"54"},
                {9,"97"},
                {10,"172"},
                {11,"309"},
                {12,"564"},
                {13,"1028"},
                {14,"1900"},
                {15,"3512"},
                {16,"6542"},
                {17,"12251"},
                {18,"23000"},
                {19,"43390"},
                {20,"82025"},
                {21,"155611"},
                {22,"295947"},
                {23,"564163"},
                {24,"1077871"},
                {25,"2063689"},
                {26,"3957809"},
                {27,"7603553"},
                {28,"14630843"},
                {29,"28192750"},
                {30,"54400028"},
                {31,"105097565"},
                {32,"203280221"},
                {33,"393615806"},
                {34,"762939111"},
                {35,"1480206279"},
                {36,"2874398515"},
                {37,"5586502348"},
                {38,"10866266172"},
                {39,"21151907950"},
                {40,"41203088796"},
                {41,"80316571436"},
                {42,"156661034233"},
                {43,"305761713237"},
                {44,"597116381732"},
                {45,"1166746786182"},
                {46,"2280998753949"},
                {47,"4461632979717"},
                {48,"8731188863470"},
                {49,"17094432576778"},
                {50,"33483379603407"},
                {51,"65612899915304"},
                {52,"128625503610475"},
                {53,"252252704148404"},
                {54,"494890204904784"},
                {55,"971269945245201"},
                {56,"1906879381028850"},
                {57,"3745011184713964"},
                {58,"7357400267843990"},
                {59,"14458792895301660"},
                {60,"28423094496953330"},
                {61,"55890484045084135"},
                {62,"109932807585469973"},
                {63,"216289611853439384"},
                {64,"425656284035217743"},
                {65,"837903145466607212"},
                {66,"1649819700464785589"},
                {67,"3249254387052557215"},
                {68,"6400771597544937806"},
                {69,"12611864618760352880"},
                {70,"24855455363362685793"},
                {71,"48995571600129458363"},
                {72,"96601075195075186855"},
                {73,"190499823401327905601"},
                {74,"375744164937699609596"},
                {75,"741263521140740113483"},
                {76,"1462626667154509638735"},
                {77,"2886507381056867953916"},
                {78,"5697549648954257752872"},
                {79,"11248065615133675809379"},
                {80,"22209558889635384205844"},
                {81,"43860397052947409356492"},
                {82,"86631124695994360074872"},
                {83,"171136408646923240987028"},
                {84,"338124238545210097236684"},
                {85,"668150111666935905701562"},
                {86,"1320486952377516565496055"},
                {87,"2610087356951889016077639"},
                {88,"5159830247726102115466054"},
                {89,"10201730804263125133012340"},
                {90,"20172933541156002700963336"},

        };

        //const long primecount36 = 2874398515;
        //const long primecount37 = primecount36 + 2712103833;
        //const long primecount38 = primecount37 + 5279763824;
        //const long primecount39 = primecount38 + 10285641778;
        //const long primecount40 = primecount39 + 20051180846;
        //const long primecount41 = primecount40 + 39113482640;
        //const long primecount42 = primecount41 + 76344462797;
        //const long primecount43 = primecount42 + 0;
        //const long primecount44 = primecount43 + 0;

        /// <summary>
        /// Prime data for values up to 2^64, with primes stored <see cref="GmpInt"/> values. 
        /// </summary>
        /// <remarks>Manually calculated using HigginsSoft.Math up to 2^45 and verified using Yafu. 
        /// Values 2^46 to 2^64 retrieved from https://oeis.org/A185192</remarks>
        public static Dictionary<int, PrimeDataMp64> Counts64 =
            (new PrimeDataMp64[]
            {
                new(33, 393615806, 8589934583, 8589934609 ),
                new(34, 762939111, 17179869143, 17179869209 ),
                new(35, 1480206279, 34359738337, 34359738421 ),
                new(36, 2874398515, 68719476731, 68719476767 ),
                new(37, 5586502348, 137438953447, 137438953481 ),
                new(38,10866266172, 274877906899, 274877906951 ),
                new(39,21151907950, 549755813881, 549755813911 ),
                new(40,41203088796, 1099511627689, 1099511627791 ),
                new(41,80316571436, 2199023255531, 2199023255579 ),
                new(42,156661034233, 4398046511093, 4398046511119 ),
                new(43,305761713237, 8796093022151, 8796093022237 ),
                new(44,597116381732, 17592186044399, 17592186044423 ),
                new(45,1166746786182, 35184372088777, 35184372088891 ),
                new(46,2280998753949, 70368744177643, 70368744177679 ),
                new(47,4461632979717, 140737488355213, 140737488355333 ),
                new(48,8731188863470, 281474976710597, 281474976710677 ),
                new(49,17094432576778, 562949953421231, 562949953421381 ),
                new(50,33483379603407, 1125899906842597, 1125899906842679 ),
                new(51,65612899915304, 2251799813685119, 2251799813685269 ),
                new(52,128625503610475, 4503599627370449, 4503599627370517 ),
                new(53,252252704148404, 9007199254740881, 9007199254740997 ),
                new(54,494890204904784, 18014398509481951, 18014398509482143 ),
                new(55,971269945245201, 36028797018963913, 36028797018963971 ),
                new(56,1906879381028850, 72057594037927931, 72057594037928017 ),
                new(57,3745011184713964, 144115188075855859, 144115188075855881 ),
                new(58,7357400267843990, 288230376151711717, 288230376151711813 ),
                new(59,14458792895301660, 576460752303423433, 576460752303423619 ),
                new(60,28423094496953330, 1152921504606846883, 1152921504606847009 ),
                new(61,55890484045084135, 2305843009213693951, 2305843009213693967 ),
                new(62,109932807585469973, 4611686018427387847, 4611686018427388039 ),
                new(63,216289611853439384, 9223372036854775783,9223372036854775837),
                new(64,425656284035217743, 18446744073709551557,"18446744073709551629"),
            }
            ).Concat(Counts.Select(x => new PrimeDataMp64(x.Value.Bits, x.Value.Count, x.Value.MaxPrime, x.Value.NextPrime)))
            .ToDictionary(x => x.Bits, x => x);

        /// <summary>
        /// Prime data for values between 2^65 and 2^90, with primes stored <see cref="GmpInt"/> values. 
        /// </summary>
        /// <remarks>Values retrieved from https://oeis.org/A185192. Based on prime density 2^90 is likely an under count.</remarks>
        public static Dictionary<int, PrimeDataMp90> Counts90 =
            (new PrimeDataMp90[]
            {
                new(65, "837903145466607212","36893488147419103183","36893488147419103363"),
                new(66, "1649819700464785589","73786976294838206459","73786976294838206473"),
                new(67, "3249254387052557215","147573952589676412909","147573952589676412931"),
                new(68, "6400771597544937806","295147905179352825833","295147905179352825889"),
                new(69, "12611864618760352880","590295810358705651693","590295810358705651741"),
                new(70, "24855455363362685793","1180591620717411303389","1180591620717411303449"),
                new(71, "48995571600129458363","2361183241434822606617","2361183241434822606859"),
                new(72, "96601075195075186855","4722366482869645213603","4722366482869645213711"),
                new(73, "190499823401327905601","9444732965739290427323","9444732965739290427421"),
                new(74, "375744164937699609596","18889465931478580854749","18889465931478580854821"),
                new(75, "741263521140740113483","37778931862957161709471","37778931862957161709601"),
                new(76, "1462626667154509638735","75557863725914323419121","75557863725914323419151"),
                new(77, "2886507381056867953916","151115727451828646838239","151115727451828646838283"),
                new(78, "5697549648954257752872","302231454903657293676533","302231454903657293676551"),
                new(79, "11248065615133675809379","604462909807314587353021","604462909807314587353111"),
                new(80, "22209558889635384205844","1208925819614629174706111","1208925819614629174706189"),
                new(81, "43860397052947409356492","2417851639229258349412301","2417851639229258349412369"),
                new(82, "86631124695994360074872","4835703278458516698824647","4835703278458516698824713"),
                new(83, "171136408646923240987028","9671406556917033397649353","9671406556917033397649483"),
                new(84, "338124238545210097236684","19342813113834066795298781","19342813113834066795298819"),
                new(85, "668150111666935905701562","38685626227668133590597613","38685626227668133590597803"),
                new(86, "1320486952377516565496055","77371252455336267181195229","77371252455336267181195291"),
                new(87, "2610087356951889016077639","154742504910672534362390461","154742504910672534362390567"),
                new(88, "5159830247726102115466054","309485009821345068724780757","309485009821345068724781063"),
                new(89, "10201730804263125133012340","618970019642690137449562111","618970019642690137449562141"),
                new(90, "20172933541156002700963336","1237940039285380274899124191","1237940039285380274899124357"),
            }
            ).Concat(Counts64.Select(x => new PrimeDataMp90(x.Value.Bits, x.Value.Count, x.Value.MaxPrime, x.Value.NextPrime)))
            .ToDictionary(x => x.Bits, x => x);
        /// <summary>
        /// Absolute value of N. Equals 2^Bits (or 1 &lt;&lt; Bits).
        /// </summary>
        public int N { get; }

        /// <summary>
        /// The number of bits in N.
        /// </summary>
        public int Bits { get; }

        /// <summary>
        /// The number of primes less than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The largest prime less than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public long MaxPrime { get; }

        /// <summary>
        /// The next prime greater than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public long NextPrime { get; }


        /// <summary>
        /// The density ratio of number of primes below N to N.
        /// </summary>
        public double Density => (double)Count / N;

        public PrimeData(int bit, int count, long maxPrime, long nextPrime)
        {
            Bits = bit;
            N = 1 << bit;
            Count = count;
            MaxPrime = maxPrime;
            NextPrime = nextPrime;
        }
    }

    /// <summary>
    /// Prime data with Max Prime and Next Prime stored as <see cref="GmpInt"/> values. 
    /// </summary>
    public class PrimeDataMp64
    {
        /// <summary>
        /// Absolute value of N. Equals 2^Bits (or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt N { get; }

        /// <summary>
        /// The number of bits in N.
        /// </summary>
        public int Bits { get; }

        /// <summary>
        /// The number of primes less than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// The largest prime less than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt MaxPrime { get; }

        /// <summary>
        /// The next prime greater than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt NextPrime { get; }

        /// <summary>
        /// The density ratio of number of primes below N to N.
        /// </summary>
        public double Density => (double)((GmpFloat)Count / (GmpFloat)N);

        public PrimeDataMp64(int bit, long count, GmpInt maxPrime, GmpInt nextPrime)
        {
            Bits = bit;
            N = GmpInt.One << bit;
            Count = count;
            MaxPrime = maxPrime;
            NextPrime = nextPrime;
        }
    }

    /// <summary>
    /// Prime data with Count, Max Prime and Next Prime stored as <see cref="GmpInt"/> values. 
    /// </summary>
    public class PrimeDataMp90
    {
        /// <summary>
        /// Absolute value of N. Equals 2^Bits (or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt N { get; }

        /// <summary>
        /// The number of bits in N.
        /// </summary>
        public int Bits { get; }

        /// <summary>
        /// The number of primes less than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt Count { get; }

        /// <summary>
        /// The largest prime less than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt MaxPrime { get; }

        /// <summary>
        /// The next prime greater than N (2^Bits or 1 &lt;&lt; Bits).
        /// </summary>
        public GmpInt NextPrime { get; }

        public PrimeDataMp90(int bit, GmpInt count, GmpInt maxPrime, GmpInt nextPrime)
        {
            Bits = bit;
            N = GmpInt.One << bit;
            Count = count;
            MaxPrime = maxPrime;
            NextPrime = nextPrime;
        }
    }
}