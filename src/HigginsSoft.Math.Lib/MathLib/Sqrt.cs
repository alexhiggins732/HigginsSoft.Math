using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(double d) => System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(int d) => System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(uint d) => System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(long d) => (long)System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(ulong d) => (ulong)System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static BigInteger Sqrt(BigInteger d) => (BigInteger)Sqrt((GmpInt)d);
        public static GmpInt Sqrt(GmpInt d) => d.Sqrt();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqrt(int n, out bool isPerfect)
        {
            isPerfect = IsPerfectSquare(n, out int result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqrt(int n, out int value)
        {
            IsPerfectSquare(n, out value);
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqrt(uint n, out int value)
        {
            IsPerfectSquare(n, out value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Sqrt(long n, out uint value)
        {
            IsPerfectSquare(n, out value);
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Sqrt(ulong n, out uint value)
        {
            IsPerfectSquare(n, out value);
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(int n)
            => IsPerfectSquare(n, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(int n, out int sqrt)
        {

            sqrt = (int)System.Math.Sqrt(n);
            if (n < 4) return false;
            return sqrt * sqrt == n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(uint n)
            => IsPerfectSquare(n, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(uint n, out int sqrt)
        {
            sqrt = (int)System.Math.Sqrt(n);
            return sqrt * sqrt == n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(long n)
            => IsPerfectSquare(n, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(long n, out uint sqrt)
        {
            sqrt = (uint)System.Math.Sqrt(n);
            return sqrt * sqrt == n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(ulong n)
            => IsPerfectSquare(n, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(ulong n, out uint sqrt)
        {
            sqrt = (uint)System.Math.Sqrt(n);
            return sqrt * sqrt == n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(BigInteger n)
            => IsPerfectSquare((GmpInt)n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(GmpInt n)
            => n.IsPerfectSquare();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPerfectSquare(GmpInt n, out GmpInt sqrt)
            => n.IsPerfectSquare(out sqrt);
    }

    public static partial class MathLib
    {
        public const int KaraCutoff = 1000;

        //https://stackoverflow.com/questions/2187123/optimizing-karatsuba-implementation
        public static BigInteger Karatsuba(BigInteger x, BigInteger y, int cuttoff = KaraCutoff)
        {
            int n = (int)MathLib.Max(x.GetBitLength(), y.GetBitLength());
            if (n <= cuttoff) return x * y;

            n = ((n + 1) / 2);

            BigInteger b = x >> n;			            //low(x) 	// right(x) // lsb_half(x)
            BigInteger a = x - (b << n);	            //high(x) 	// left(x)	// msb_half(x)
            BigInteger d = y >> n;			            //low(y)	// right(y)	// lsb_half(y)
            BigInteger c = y - (d << n);                //high(y)	// left(y)	// msb_half(y)


            BigInteger ac = Karatsuba(a, c);	        // high(x) * high(y) // msb_half(x) * msb_half(y)
            BigInteger bd = Karatsuba(b, d);	        // low(x) * low(y) 	 // lsb_half(x) * lsb_half(y)
            BigInteger abcd = Karatsuba(a + b, c + d);	// (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));

            var mid = (abcd - ac - bd);
            return ac + (mid << n) + (bd << (2 * n));
        }


        //https://stackoverflow.com/questions/2187123/optimizing-karatsuba-implementation
        // This eliminates 2 shifts, 2 additions and 3 subtractions from each recursion of the Karatsuba algorithm.
        //TODO: optimize to use shared array for recursive calls. Then adapt single array to CUDA.
        public static BigInteger KaratsubaSquare(BigInteger x, int cuttoff = KaraCutoff)
        {

            var n = (int)x.GetBitLength();
            if (n < cuttoff) return x * x;

            n = (n + 1) >> 1;

            var b = x >> n;
            var a = x - (b << n); // a = x & !(b<<n); // replaces and+not with a single subtraction. With word aligned array, take the high n words
            var ac = KaratsubaSquare(a, cuttoff);
            var bd = KaratsubaSquare(b, cuttoff);
            var c = Karatsuba(a, b, cuttoff);
            return ac + (c << (n + 1)) + (bd << (2 * n)); // a correct word aligned implementation would have results in place and eliminate need to shift.
        }


        /// <summary>
        /// Perform karatsuba after aligning x and y on 32 word boundary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cuttoff"></param>
        /// <returns></returns>
        public static BigInteger KaratsubaWordAligned(BigInteger x, BigInteger y, int cuttoff = KaraCutoff)
        {
            bool debugHex = true;
            int n = (int)MathLib.Max(x.GetBitLength(), y.GetBitLength());
            if (n <= cuttoff)
            {
                if (debugHex)
                {
                    var res = x * y;
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"x = {FormatHex(x)}");
                    Console.WriteLine($"y = {FormatHex(y)}");
                    Console.WriteLine($"x * y = result = {FormatHex(res)}");
                    Console.WriteLine();
                    return res;
                }
                else
                {
                    return x * y;
                }

            }

            var pow = 32;
            while ((pow << 1) <= n)
                pow <<= 1;
            n = pow;
            /*
                  //if (!manualAlign)
                  //{

                  //}
                  //else
                  //{
                  //    var bytesX = x.ToByteArray();
                  //    var bytesY = y.ToByteArray();
                  //    var max = bytesX.Length > bytesY.Length ? bytesX.Length : bytesY.Length;
                  //    var maxWords = 1 + (max >> 32);
                  //    var pow = 1;
                  //    while (pow < maxWords)
                  //        pow <<= 1;
                  //    maxWords = pow;
                  //    n = maxWords * 32;


                  //    uint[] wordsX = new uint[pow];
                  //    Buffer.BlockCopy(bytesX, 0, wordsX, 0, bytesX.Length);
                  //    uint[] wordsY = new uint[pow];
                  //    Buffer.BlockCopy(bytesY, 0, wordsY, 0, bytesY.Length);

                  //    var t = new uint[pow >> 1];
                  //    var byteLen = t.Length * 4;


                  //    //BigInteger b = x >> n;			            //low(x) 	// right(x) // lsb_half(x)	
                  //    Buffer.BlockCopy(wordsX, 0, t, 0, byteLen);
                  //    b = MathLib.ToBigInteger(t);

                  //    //BigInteger a = x - (b << n);	            //high(x) 	// left(x)	// msb_half(x)
                  //    Buffer.BlockCopy(wordsX, byteLen, t, 0, byteLen);
                  //    a = MathLib.ToBigInteger(t);

                  //    //BigInteger d = y >> n;			            //low(y)	// right(y)	// lsb_half(y)
                  //    Buffer.BlockCopy(wordsY, 0, t, 0, byteLen);
                  //    d = MathLib.ToBigInteger(t);

                  //    //BigInteger c = y - (d << n);                //high(y)	// left(y)	// msb_half(y)
                  //    Buffer.BlockCopy(wordsY, byteLen, t, 0, byteLen);
                  //    c = MathLib.ToBigInteger(t);
                  //}
            */

            BigInteger b = x >> n;			            //high(x) 	// left(x)  // msb_half(x)
            BigInteger a = x - (b << n);	            //low(x) 	// right(x)	// lsb_half(x)
            BigInteger d = y >> n;			            //high(y)	// left(y)	// msb_half(y)
            BigInteger c = y - (d << n);                //low(y)	// right(y)	// lsb_half(y)


            BigInteger ac = KaratsubaWordAligned(a, c, cuttoff);            // low(x) * low(y) 	 // lsb_half(x) * lsb_half(y)
            BigInteger bd = KaratsubaWordAligned(b, d, cuttoff);	        // high(x) * high(y) // msb_half(x) * msb_half(y)
            BigInteger abcd;
            BigInteger ab = a + b;
            BigInteger cd = c + d;
            if (n > 128)
            {
                abcd = KaratsubaWordAligned(ab, cd, cuttoff);	// (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));
            }
            else
            {
                abcd = (ab) * (cd);
            }

            var mid = (abcd - ac - bd);
            var result = ac + (mid << n) + (bd << (2 * n));


            if (debugHex)
            {

                Console.WriteLine("---------------------------------");
                Console.WriteLine($"x = {FormatHex(x)}");
                Console.WriteLine($"y = {FormatHex(y)}");
                Console.WriteLine($"msb_half(x) = b = {FormatHex(b)}");
                Console.WriteLine($"lsb_half(x) = a = {FormatHex(a)}");

                Console.WriteLine($"msb_half(y) = d = {FormatHex(d)}");
                Console.WriteLine($"lsb_half(y) = c = {FormatHex(c)}");

                Console.WriteLine($"msb_half(x) * msb_half(y) = bd = {FormatHex(bd)}");
                Console.WriteLine($"lsb_half(x) * lsb_half(y) = ac = {FormatHex(ac)}");


                Console.WriteLine($"a+b = ab = {FormatHex(ab)}");
                Console.WriteLine($"c+d = cd = {FormatHex(cd)}");
                Console.WriteLine($"(lsb(x) + msb(x)) * (lsb(x) + msb(x)) = abcd = {FormatHex(abcd)}");

                Console.WriteLine($"(abcd - ac - bd) = mid = {FormatHex(mid)}");
                Console.WriteLine($" ac + (mid << n) + (bd << (2 * n)) = result = {FormatHex(result)}");
                Console.WriteLine("");
            }
            return result;

        }


        /// <summary>
        /// Perform karatsuba after aligning x and y on 32 word boundary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cuttoff"></param>
        /// <returns></returns>
        public static BigInteger KaratsubaWordAligned2(BigInteger x, BigInteger y, out BigInteger prevac, out BigInteger prevbd, int cuttoff = KaraCutoff)
        {
            bool debugHex = true;
            int n = (int)MathLib.Max(x.GetBitLength(), y.GetBitLength());
            if (n <= cuttoff)
            {
                prevac = prevbd = 0;
                if (debugHex)
                {
                    var res = x * y;
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"x = {FormatHex(x)}");
                    Console.WriteLine($"y = {FormatHex(y)}");
                    Console.WriteLine($"x * y = result = {FormatHex(res)}");
                    Console.WriteLine();
                    return res;
                }
                else
                {
                    return x * y;
                }

            }

            var pow = 32;
            while ((pow << 1) <= n)
                pow <<= 1;
            n = pow;


            BigInteger b = x >> n;			            //high(x) 	// left(x)  // msb_half(x)
            BigInteger a = x - (b << n);	            //low(x) 	// right(x)	// lsb_half(x)
            BigInteger d = y >> n;			            //high(y)	// left(y)	// msb_half(y)
            BigInteger c = y - (d << n);                //low(y)	// right(y)	// lsb_half(y)


            BigInteger ab = a + b;
            BigInteger cd = c + d;
            BigInteger abcd;
            BigInteger PrevAc;
            BigInteger PrevBd;
            BigInteger abcd2;
            if (n > 128)
            {
                abcd2 = abcd = KaratsubaWordAligned2(ab, cd, out PrevAc, out PrevBd, cuttoff); // (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));

            }
            else
            {
                if (n > 64)
                {
                    abcd = KaratsubaWordAligned2(ab, cd, out PrevAc, out PrevBd, cuttoff); // (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));
                    uint[] abWords = MathLib.ToUintArray(ab);
                    var abWordSum = abWords.Select(x => (BigInteger)x).Aggregate((a, b) => a + b);
                    var cdWordSum = MathLib.ToUintArray(cd).Select(x => (BigInteger)x).Aggregate((a, b) => a + b);
                    abcd2 = (BigInteger)abWordSum * (BigInteger)cdWordSum;

                    //(lsb(x) + msb(x)) * (lsb(x) + msb(x)) = abcd = 00000510 00000900 00000BE0 00000DC0 00000910 00000540 00000240
                    //(abcd2) = abcd2 = 00000510 000008FF FFFFFBDF FFFFF1C0 00003510 00000540 00000240

                    var t0 = abcd2 - PrevAc - PrevBd;
                    var t2 = (PrevBd << n) + (t0 << (n / 2)) + PrevAc;
                    abcd2 = t2;

                }
                else
                {
                    abcd = abcd2 = ab * cd;

                }
            }








            BigInteger ac = KaratsubaWordAligned2(a, c, out _, out _, cuttoff);            // low(x) * low(y) 	 // lsb_half(x) * lsb_half(y)
            BigInteger bd = KaratsubaWordAligned2(b, d, out _, out _, cuttoff);           // high(x) * high(y) // msb_half(x) * msb_half(y)

            prevac = ac;
            prevbd = bd;

            var mid = (abcd - ac - bd);
            var result = ac + (mid << n) + (bd << (2 * n));


            if (debugHex)
            {

                Console.WriteLine("---------------------------------");
                Console.WriteLine($"x = {FormatHex(x)}");
                Console.WriteLine($"y = {FormatHex(y)}");
                Console.WriteLine($"msb_half(x) = b = {FormatHex(b)}");
                Console.WriteLine($"lsb_half(x) = a = {FormatHex(a)}");

                Console.WriteLine($"msb_half(y) = d = {FormatHex(d)}");
                Console.WriteLine($"lsb_half(y) = c = {FormatHex(c)}");

                Console.WriteLine($"msb_half(x) * msb_half(y) = bd = {FormatHex(bd)}");
                Console.WriteLine($"lsb_half(x) * lsb_half(y) = ac = {FormatHex(ac)}");


                Console.WriteLine($"a+b = ab = {FormatHex(ab)}");
                Console.WriteLine($"c+d = cd = {FormatHex(cd)}");
                Console.WriteLine($"(lsb(x) + msb(x)) * (lsb(x) + msb(x)) = abcd = {FormatHex(abcd)}");
                Console.WriteLine($"(abcd2) = abcd2 = {FormatHex(abcd2)}");
                Console.WriteLine($"(abcd - ac - bd) = mid = {FormatHex(mid)}");

                BigInteger mid2 = (abcd2 - ac - bd);
                Console.WriteLine($"(abcd2 - ac - bd) = mid2 = {FormatHex(mid2)}");
                BigInteger mid3 = ((bd << (2 * n)) + (mid2 << n) + ac);
                Console.WriteLine($" ((bd << (2 * n)) + (mid2 <<n) + ac) = mid3 = {FormatHex(mid3)}");
                Console.WriteLine($" ac + (mid << n) + (bd << (2 * n)) = result = {FormatHex(result)}");



                Console.WriteLine("");
            }
            return result;

        }

        /// <summary>
        /// Perform karatsuba after aligning x and y on 32 word boundary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cuttoff"></param>
        /// <returns></returns>
        public static BigInteger KaratsubaWordAligned3(BigInteger x, BigInteger y, int cuttoff = KaraCutoff)
        {
            bool debugHex = true;
            int n = (int)MathLib.Max(x.GetBitLength(), y.GetBitLength());
            var pow = 32;
            while (pow < n)
                pow <<= 1;
            n = pow >> 1;

            if (n <= cuttoff)
            {
                if (debugHex)
                {
                    var res = x * y;
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"x = {FormatHex(x)}");
                    Console.WriteLine($"y = {FormatHex(y)}");
                    Console.WriteLine($"x * y = result = {FormatHex(res)}");
                    Console.WriteLine();
                    return res;
                }
                else
                {
                    return x * y;
                }

            }




            BigInteger b = x >> n;			            //high(x) 	// left(x)  // msb_half(x)
            BigInteger a = x - (b << n);	            //low(x) 	// right(x)	// lsb_half(x)
            BigInteger d = y >> n;			            //high(y)	// left(y)	// msb_half(y)
            BigInteger c = y - (d << n);                //low(y)	// right(y)	// lsb_half(y)


            BigInteger ab = a + b;
            BigInteger cd = c + d;
            BigInteger abcd;

            BigInteger abcd2;
            if (n > 64)
            {
                abcd2 = abcd = KaratsubaWordAligned3(ab, cd, cuttoff); // (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));

            }
            else
            {
                if (n == 64)
                {
                    System.Diagnostics.Debug.Assert(n == 64);



                    abcd = KaratsubaWordAligned2(ab, cd, out BigInteger pa, out BigInteger pb, cuttoff); // (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));
                    var n2 = n >> 1; // n2=32;
                    // overkill, but implementing for BigO calculation
                    BigInteger abh = ab >> n2;                     //high(x) 	// left(x)  // msb_half(x)
                    BigInteger abl = ab - (abh << n2);                //low(x) 	// right(x)	// lsb_half(x)
                    BigInteger cdh = cd >> n2;                      //high(y)	// left(y)	// msb_half(y)
                    BigInteger cdl = cd - (cdh << n2);                //low(y) 	// right(y)	// lsb_half(y)


                    BigInteger PrevAc = KaratsubaWordAligned3(abl, cdl, cuttoff);
                    BigInteger PrevBd = KaratsubaWordAligned3(abh, cdh, cuttoff);

                    if (PrevAc != pa)
                    {
                        string bp = "PrevAc calc failed";
                    }

                    if (PrevBd != pb)
                    {
                        string bp = "PrevBd calc failed";
                    }
                    System.Diagnostics.Debug.Assert(PrevAc == pa);
                    System.Diagnostics.Debug.Assert(PrevBd == pb);


                    // for debug purposes only. Can simply calc abWordSum = awordsum = a>>wordsize + (word)ab
                    uint[] abWords = MathLib.ToUintArray(ab);
                    var abWordSum = abWords.Select(x => (BigInteger)x).Aggregate((a, b) => a + b);
                    var cdWordSum = MathLib.ToUintArray(cd).Select(x => (BigInteger)x).Aggregate((a, b) => a + b);
                    // get word s
                    // can directly multiply, but pass recusively to count in BigO caclulation
                    //  abcd2 = (BigInteger)abWordSum * (BigInteger)cdWordSum;
                    abcd2 = KaratsubaWordAligned3(abWordSum, cdWordSum);


                    var t0 = abcd2 - PrevAc - PrevBd;
                    var t2 = (PrevBd << n) + (t0 << (n / 2)) + PrevAc;
                    abcd2 = t2;
                    if (abcd != abcd2)
                    {
                        var hexa = FormatHex(abcd);
                        var hex2 = FormatHex(abcd2);
                        string message = $"abcd2 calc failed - Expected: {hexa} Actual: {hex2}";
                        string bp = message;
                    }
                    System.Diagnostics.Debug.Assert(abcd == abcd2);

                }
                else
                {
                    // can directly multiply, but pass recusively to count in BigO caclulation
                    // abcd = abcd2 = ab * cd;
                    abcd = abcd2 = KaratsubaWordAligned3(ab, cd);
                }
            }

            BigInteger ac = KaratsubaWordAligned3(a, c, cuttoff);            // low(x) * low(y) 	 // lsb_half(x) * lsb_half(y)
            BigInteger bd = KaratsubaWordAligned3(b, d, cuttoff);           // high(x) * high(y) // msb_half(x) * msb_half(y)


            var mid = (abcd - ac - bd);
            var result = ac + (mid << n) + (bd << (2 * n));


            if (debugHex)
            {

                Console.WriteLine("---------------------------------");
                Console.WriteLine($"x = {FormatHex(x)}");
                Console.WriteLine($"y = {FormatHex(y)}");
                Console.WriteLine($"msb_half(x) = b = {FormatHex(b)}");
                Console.WriteLine($"lsb_half(x) = a = {FormatHex(a)}");

                Console.WriteLine($"msb_half(y) = d = {FormatHex(d)}");
                Console.WriteLine($"lsb_half(y) = c = {FormatHex(c)}");

                Console.WriteLine($"msb_half(x) * msb_half(y) = bd = {FormatHex(bd)}");
                Console.WriteLine($"lsb_half(x) * lsb_half(y) = ac = {FormatHex(ac)}");


                Console.WriteLine($"a+b = ab = {FormatHex(ab)}");
                Console.WriteLine($"c+d = cd = {FormatHex(cd)}");
                Console.WriteLine($"(lsb(x) + msb(x)) * (lsb(x) + msb(x)) = abcd = {FormatHex(abcd)}");
                Console.WriteLine($"(abcd2) = abcd2 = {FormatHex(abcd2)}");
                Console.WriteLine($"(abcd - ac - bd) = mid = {FormatHex(mid)}");

                BigInteger mid2 = (abcd2 - ac - bd);
                Console.WriteLine($"(abcd2 - ac - bd) = mid2 = {FormatHex(mid2)}");
                BigInteger mid3 = ((bd << (2 * n)) + (mid2 << n) + ac);
                Console.WriteLine($" ((bd << (2 * n)) + (mid2 <<n) + ac) = mid3 = {FormatHex(mid3)}");
                Console.WriteLine($" ac + (mid << n) + (bd << (2 * n)) = result = {FormatHex(result)}");



                Console.WriteLine("");
            }
            return result;

        }


        public static KaratsubaData KaratsubaOO(BigInteger x, BigInteger y, int cuttoff = KaraCutoff)
        {
            var data = new KaratsubaData { x = x, y = y };
            bool debugHex = true;
            int n = (int)MathLib.Max(x.GetBitLength(), y.GetBitLength());
            if (n <= cuttoff)
            {
                data.Result = x * y;
                data.ac = new KaratsubaData { Result = x };
                data.bd = new KaratsubaData { Result = y };
                if (debugHex)
                {
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"x = {FormatHex(x)}");
                    Console.WriteLine($"y = {FormatHex(y)}");
                    Console.WriteLine($"x * y = result = {FormatHex(data.Result)}");
                    Console.WriteLine();

                }
                return data;
            }

            var pow = 32;
            while ((pow << 1) <= n)
                pow <<= 1;
            n = pow;

            data.N = n;

            data.b = x >> n;			            //high(x) 	// left(x)  // msb_half(x)
            data.a = x - (data.b << n);	            //low(x) 	// right(x)	// lsb_half(x)
            data.d = y >> n;			            //high(y)	// left(y)	// msb_half(y)
            data.c = y - (data.d << n);                //low(y)	// right(y)	// lsb_half(y)


            data.ab = data.a + data.b;
            data.cd = data.c + data.d;
            data.abWords = MathLib.ToUintArray(data.ab);
            data.abWordSum = data.abWords.Select(x => (BigInteger)x).Aggregate((a, b) => a + b);
            data.cdWords = MathLib.ToUintArray(data.cd);
            data.cdWordSum = data.cdWords.Select(x => (BigInteger)x).Aggregate((a, b) => a + b);




            data.abcd = KaratsubaOO(data.ab, data.cd, cuttoff); // (lsb_half(x) + msb_half(x)) * (lsb_half(x) + msb_half(x));

            data.wordSum = new KaratsubaData { x = data.abWordSum, y = data.cdWordSum, Result = data.abWordSum * data.cdWordSum };
            data.shiftedWordSum = new KaratsubaData { x = data.abWordSum, y = data.cdWordSum, Result = data.abWordSum * data.cdWordSum };
            data.abcd2 = data.wordSum.Result;
            if (data.abcd != data.abcd2)
            {
                //(lsb(x) + msb(x)) * (lsb(x) + msb(x)) = abcd = 00000510 00000900 00000BE0 00000DC0 00000910 00000540 00000240
                //(abcd2) = abcd2 = 00000510 000008FF FFFFFBDF FFFFF1C0 00003510 00000540 00000240


                var t0 = data.abcd2 - data.abcd.ac - data.abcd.bd;
                var t2 = (data.abcd.bd.Result << n) + (t0 << (n / 2)) + data.abcd.ac;
                data.shiftedWordSum.Result = data.abcd2 = t2;
                if (t2 != data.abcd)
                {
                    string bp = "";
                }
            }


            data.ac = KaratsubaOO(data.a, data.c, cuttoff);            // low(x) * low(y) 	 // lsb_half(x) * lsb_half(y)
            data.bd = KaratsubaOO(data.b, data.d, cuttoff);           // high(x) * high(y) // msb_half(x) * msb_half(y)
            data.mid2 = data.abcd2 - data.ac - data.bd;


            data.mid = (BigInteger)data.abcd - data.ac - data.bd;
            data.Result = data.ac + (data.mid << n) + ((BigInteger)data.bd << (2 * n));


            if (debugHex)
            {
                data.PrintDebugHex();

            }
            return data;

        }

        public class KaratsubaData
        {
            public BigInteger x;
            public BigInteger y;
            public BigInteger Result;

            public int N;
            internal BigInteger b;
            internal BigInteger a;
            internal BigInteger d;
            internal BigInteger c;
            internal BigInteger ab;
            internal BigInteger cd;
            internal uint[] abWords = new uint[] { };
            internal BigInteger abWordSum;
            internal uint[] cdWords = new uint[] { };
            internal BigInteger cdWordSum;
            internal KaratsubaData abcd = null!;
            internal KaratsubaData wordSum = null!;
            internal KaratsubaData ac = null!;
            internal KaratsubaData bd = null!;
            internal BigInteger mid;
            internal BigInteger abcd2;
            internal KaratsubaData shiftedWordSum;
            internal BigInteger mid2;

            internal void PrintDebugHex()
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine($"x = {FormatHex(x)}");
                Console.WriteLine($"y = {FormatHex(y)}");
                Console.WriteLine($"msb_half(x) = b = {FormatHex(b)}");
                Console.WriteLine($"lsb_half(x) = a = {FormatHex(a)}");

                Console.WriteLine($"msb_half(y) = d = {FormatHex(d)}");
                Console.WriteLine($"lsb_half(y) = c = {FormatHex(c)}");

                Console.WriteLine($"msb_half(x) * msb_half(y) = bd = {FormatHex(bd)}");
                Console.WriteLine($"lsb_half(x) * lsb_half(y) = ac = {FormatHex(ac)}");


                Console.WriteLine($"a+b = ab = {FormatHex(ab)}");
                Console.WriteLine($"c+d = cd = {FormatHex(cd)}");
                Console.WriteLine($"(lsb(x) + msb(x)) * (lsb(x) + msb(x)) = abcd = {FormatHex(abcd)}");
                Console.WriteLine($"wordsum = mid2 = {FormatHex(wordSum)}");

                Console.WriteLine($"(abcd - ac - bd) = mid = {FormatHex(mid)}");
                Console.WriteLine($"(abcd2) = abcd2 = {FormatHex(abcd2)}");
                Console.WriteLine($"(abcd2 - ac - bd) = mid2 = {FormatHex(mid2)}");
                BigInteger mid3 = ((bd.Result << (2 * N)) + (mid2 << N) + ac);
                Console.WriteLine($" ((bd << (2 * n)) + (mid2 <<n) + ac) = mid3 = {FormatHex(mid3)}");
                Console.WriteLine($" ac + (mid << n) + (bd << (2 * n)) = result = {FormatHex(Result)}");



                Console.WriteLine("");
            }

            public static implicit operator BigInteger(KaratsubaData a) { return a.Result; }

            public override string ToString()
            {
                return FormatHex(Result);
            }
        }
        /// <summary>
        /// Perform karatsuba after aligning x and y on 32 word boundary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cuttoff"></param>
        /// <returns></returns>
        public static BigInteger KaratsubaSquareWordAligned(BigInteger x, int cuttoff = KaraCutoff)
        {
            bool debugHex = true;
            int n = (int)x.GetBitLength();
            if (n <= cuttoff)
            {
                if (debugHex)
                {
                    var res = x * x;
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"x = {FormatHex(x)}");
                    Console.WriteLine($"x * x = result = {FormatHex(res)}");
                    Console.WriteLine();
                    return res;
                }
                else
                {
                    return x * x;
                }

            }

            var pow = 32;
            while ((pow << 1) <= n)
                pow <<= 1;
            n = pow;

            var b = x >> n;
            var a = x - (b << n); // a = x & !(b<<n); // replaces and+not with a single subtraction. With word aligned array, take the high n words
            var ac = KaratsubaSquareWordAligned(a, cuttoff);
            var bd = KaratsubaSquareWordAligned(b, cuttoff);
            var c = KaratsubaWordAligned(a, b, cuttoff);

            var mid = c << (n + 1);

            var result = ac + mid + (bd << (2 * n));

            if (debugHex)
            {

                Console.WriteLine("---------------------------------");
                Console.WriteLine($"x = {FormatHex(x)}");

                Console.WriteLine($"msb_half(x) = b = {FormatHex(b)}");
                Console.WriteLine($"lsb_half(x) = a = {FormatHex(a)}");


                Console.WriteLine($"msb_half(x) * msb_half(x) = bd = {FormatHex(bd)}");
                Console.WriteLine($"lsb_half(x) * lsb_half(x) = ac = {FormatHex(ac)}");

                Console.WriteLine($"(lsb(x) + msb(x)) = c = {FormatHex(c)}");
                Console.WriteLine($"(lsb(x) + msb(x)) * 2 = mid = {FormatHex(mid)}");
                Console.WriteLine($" ac + mid + (bd << (2 * n)) = result = {FormatHex(result)}");
                Console.WriteLine("");
            }
            return result;

        }

        public static string FormatHex(BigInteger n)
        {
            var s = string.Format("{0:X8}", n);

            var result = FormatHex(s);
            return result;
            //return FormatHex(n.ToString("{0:X8}"));
        }
        public static string FormatHex(string hexString)
        {
            var padSize = (hexString.Length % 8);
            if (padSize > 0)
                hexString = hexString.PadLeft(hexString.Length + (8 - padSize), '0');

            string formattedHexString = string.Join(" ", Enumerable.Range(0, hexString.Length / 8)
                .Select(i => hexString.Substring(i * 8, 8)));
            return formattedHexString;
        }


        public static BigInteger KaratsubaInline(BigInteger a, BigInteger b)
        {
            //calculate wordsize
            BigInteger result = 0;
            return result;
        }


        public static BigInteger KaratsubaSquareInline(BigInteger x)
        {
            //calculate wordsize
            BigInteger result = 0;
            var xWordSize = (int)x.GetBitLength() >> 5;


            // make word length a power of 2 so we can karatsuba on a 32 bit boundary down to 1 element.
            int wordSize = 1;
            while (wordSize < xWordSize)
            {
                wordSize <<= 1;
            }
            wordSize <<= 1;// double the length to allow for the full product.


            //initilize ulong[] src with 32 bit values of x, and perform the first single element square operation
            //initilize mid with 
            var src = new ulong[wordSize];
            {
                var words32 = new uint[xWordSize];
                {
                    var bytes = x.ToByteArray();
                    Buffer.BlockCopy(bytes, 0, words32, 0, bytes.Length);
                }
                for (var i = 0; i < words32.Length; i++)
                {
                    ref ulong s = ref src[i];
                    s = (ulong)words32[i];
                    s *= s;
                }
            }

            // we have a[0]^2 .. a[n]^2
            KaratsubaSquareInline(src, 1);

            return result;
        }

        private static void KaratsubaSquareInline(ulong[] src, int elementSize)
        {
            if (!MathLib.IsPow2(src.Length))
            {
                throw new NotImplementedException($"{nameof(KaratsubaInline)} only implemented for arrays with a length that are power of two.");
            }

            var temp = new ulong[src.Length];
            while (elementSize < src.Length)
            {
                var outerSize = elementSize << 1;
                for (var i = 0; i < src.Length; i += outerSize)
                {

                    var a = src[i];
                    var b = src[i + elementSize];

                    // ac already squared from last iteration
                    //var ac = KaratsubaSquare(a, cuttoff);

                    // b3 already squared from last iteration
                    //var bd = KaratsubaSquare(b, cuttoff);

                    //var c = Karatsuba(a, b, cuttoff);
                    //KaratsubaInline(i, i + elementSize, elementSize, outerSize, src, temp);

                    //return ac + (c << (n + 1)) + (bd << (2 * n)); 
                    //inlineAdd(i, i + elementSize, src, temp);
                }

                elementSize <<= 1;
            }

            //var b = x >> n;
            //var a = x - (b << n); // a = x & !(b<<n); // replaces and+not with a single subtraction. With word aligned array, take the high n words
            //var ac = KaratsubaSquare(a, cuttoff);
            //var bd = KaratsubaSquare(b, cuttoff);
            //var c = Karatsuba(a, b, cuttoff);
            //return ac + (c << (n + 1)) + (bd << (2 * n)); // a correct word aligned implementation would have results in place and eliminate need to shift.
        }

        //var c = Karatsuba(a, b, cuttoff);
        private static void KaratsubaInline(int offsetX, int offsetY, int elementSize, int outerSize,
            ulong[] src,
            ulong[] result,
            ulong[] addBuffer,
            ulong[] mulBuffer)
        {
            if (elementSize == 1)
            {
                ulong res = src[offsetX] * src[offsetY];
                result[offsetX] = res & uint.MaxValue;
                result[offsetX + 1] = res >> 32;

            }
            else
            {
                //BigInteger b = x >> n;
                //BigInteger a = x - (b << n);
                var innerElementSize = elementSize >> 1;
                int offsetB = offsetX;
                int offsetA = offsetX + innerElementSize;


                //BigInteger d = y >> n;
                //BigInteger c = y - (d << n);
                int offsetD = offsetY;
                int offsetC = offsetY + innerElementSize;


                //BigInteger ac = Karatsuba(a, c);
                KaratsubaInline(offsetA, offsetC, innerElementSize, elementSize, src, result, addBuffer, mulBuffer);

                //BigInteger bd = Karatsuba(b, d);
                KaratsubaInline(offsetB, offsetD, innerElementSize, elementSize, src, result, addBuffer, mulBuffer);

                //BigInteger abcd = Karatsuba(a + b, c + d);
                InlineAdd(innerElementSize, elementSize, offsetA, offsetB, offsetC, offsetD, src, addBuffer);

                KaratsubaInline(offsetB, offsetD, innerElementSize, elementSize, src, result, addBuffer, mulBuffer);


                //return ac + ((abcd - ac - bd) << n) + (bd << (2 * n));
            }

        }

        private static void InlineAdd(int halfSize, int elementSize, int offsetA, int offsetB, int offsetC, int offsetD, ulong[] src, ulong[] addBuffer)
        {
            //BigInteger abcd = Karatsuba(a + b, c + d);
            //clear the temp buffer.

            //offset b is the low index of the left
            //Assert(offsetB + outerSize == offsetC+innerSize)
            for (var i = offsetB; i < offsetC + halfSize; i++)
            {
                addBuffer[i] = 0;
            }
            ulong res = 0;
            for (var i = 0; i < halfSize; i++)
            {
                var a = src[offsetA + i]; //offset b is the low index of the left
                var b = src[offsetB + i];
                res = a + b;
                addBuffer[offsetB + i] += (res & uint.MaxValue);
                addBuffer[offsetB + i + 1] += (res >> 32);

                var c = src[offsetC + i]; //offset d is the low index of the right (high)
                var d = src[offsetD + i];

                res = a + b;
                addBuffer[offsetD + i] += (res & uint.MaxValue);
                addBuffer[offsetD + i + 1] += (res >> 32);
            }

            //normalize as int32 and carry;
            for (var i = 0; i < elementSize; i++)
            {
                ref ulong s = ref addBuffer[offsetB + i];
                if (s > ulong.MaxValue)
                {
                    // can get index out of bounds here, but if we do something went wrong because we overflowed while carrying
                    addBuffer[offsetB + i + 1] += (s >> 32);
                    s = s & uint.MaxValue;
                }
                ref ulong t = ref addBuffer[offsetD + i];
                if (t > ulong.MaxValue)
                {
                    // can get index out of bounds here, but if we do something went wrong because we overflowed while carrying
                    addBuffer[offsetD + i + 1] += (t >> 32);
                    t = t & uint.MaxValue;
                }
            }

        }


    }
}
