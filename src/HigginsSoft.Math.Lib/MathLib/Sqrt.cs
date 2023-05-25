using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqrt(double d) => System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqrt(int d) => (int)System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Sqrt(uint d) => (uint)System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sqrt(long d) => (long)System.Math.Sqrt(d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Sqrt(ulong d) => (ulong)System.Math.Sqrt(d);
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
}
