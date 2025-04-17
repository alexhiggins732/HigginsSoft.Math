using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static HigginsSoft.Math.Lib.MathLib;


namespace HigginsSoft.Math.Lib
{
    class U128
    {
    }
    public static class LongRabinMiller
    {
        public static PrimalityType RabinMillerLong(long n, int iterations)
        {
            if (n <= 4)
            {
                if (n == 2 || n == 3) return PrimalityType.ProbablePrime;
                return PrimalityType.Error;
            }
            if ((n & 1) == 0) return PrimalityType.Composite;

            long nm1 = n - 1;
            long d = nm1;
            int s = 0;
            while ((d & 1) == 0)
            {
                d >>= 1;
                s++;
            }

            int rangeMax = (n - 2) > int.MaxValue ? int.MaxValue : (int)(n - 2);

            for (int k = 0; k < iterations; k++)
            {
                long a = (k < deterministicRabinMillerBases32.Length) ? deterministicRabinMillerBases32[k] : Random.Shared.Next(2, rangeMax);
                long x = (long)PowerMod((ulong)a, (ulong)d, (ulong)n);
                long y = 0;

                for (int r = 0; r < s; r++)
                {
                    y = (long)MultiplyMod((ulong)x, (ulong)x, (ulong)n);
                    if (y == 1 && x != 1 && x != nm1)
                        return PrimalityType.Composite;
                    x = y;
                }

                if (y != 1)
                    return PrimalityType.Composite;
            }

            return PrimalityType.ProbablePrime;
        }

        public static ulong PowerMod(ulong a, ulong e, ulong mod)
        {
            ulong result = 1;
            a %= mod;

            while (e > 0)
            {
                if ((e & 1) != 0)
                    result = MultiplyMod(result, a, mod);
                a = MultiplyMod(a, a, mod);
                e >>= 1;
            }

            return result;
        }

        public static ulong MultiplyMod(ulong a, ulong b, ulong mod)
        {
            ulong aLow = (uint)a, aHigh = a >> 32;
            ulong bLow = (uint)b, bHigh = b >> 32;

            ulong lowLow = aLow * bLow;
            ulong lowHigh = aLow * bHigh;
            ulong highLow = aHigh * bLow;
            ulong highHigh = aHigh * bHigh;

            ulong mid = (lowHigh & 0xFFFFFFFF) + (highLow & 0xFFFFFFFF) + (lowLow >> 32);
            ulong high = highHigh + (lowHigh >> 32) + (highLow >> 32) + (mid >> 32);
            ulong low = lowLow + ((lowHigh + highLow) << 32);

            // approximate reduction
            //return ((BigInteger)high << 64 | low) % mod;
            return (high << 64 | low) % mod;
        }

        private static readonly int[] deterministicRabinMillerBases32 = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 };
    }
}
