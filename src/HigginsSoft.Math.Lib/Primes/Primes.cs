/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using MathGmp.Native;
using System;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace HigginsSoft.Math.Lib
{
    public class Primes
    {
        public static int[] Primes16 => new[] { 2, 3, 5, 7, 11, 13 };
        public static int[] Primes256 =>
            Primes16.Concat(
                Enumerable.Range(0, (256 >> 1) - (16 >> 1))
                .Select(x => 16 + (x << 1) + 1)
                .Where(x =>
                    IsCoPrime(x, Primes16)
                )
            )
            .ToArray();

        public static bool IsCoPrime(int x, int[] primes)
        {
            bool result = primes.All(prime => x % prime != 0);
            return result;
        }


        public static int[] Primes65536 =>
            Primes256.Concat(
                Enumerable.Range(0, (65536 >> 1) - (256 >> 1))
                .Select(x => 256 + (x << 1) + 1)
                .Where(x =>
                    IsCoPrime(x, Primes256))
                )
            .ToArray();

        public static bool IsPrime(int n)
        {
            if (n <= 1) return false;
            if ((n & 1) == 0) return n == 2;
            if ((n % 3) == 0) return n == 3;
            int max = (int)System.Math.Sqrt(n);
            for (int i = 5; i <= max; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0) return false;
            }

            return true;
        }
    }

    public class Factorization
    {
        public List<Factor> Factors = new();

        public void Add(mpz_t root, int count)
        {
            Factors.Add(new(root, count));
        }
    }
    public class Factor
    {
        public Factor(mpz_t value, int count)
        {
            Value = value;
            Count = count;
        }

        public GmpInt Value { get; private set; }
        public int Count { get; set; }
    }

    public class PrimeData
    {
        public const int MaxIntPrime = 2147483647;
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

        public PrimeData(int bit, int count, long maxPrime, long nextPrime)
        {
            Bits = bit;
            N = 1 << bit;
            Count = count;
            MaxPrime = maxPrime;
            NextPrime = nextPrime;
        }

        public int N { get; }
        public int Bits { get; }
        public int Count { get; }
        public long MaxPrime { get; }
        public long NextPrime { get; }
    }
}