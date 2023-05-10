/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    public class OpFactory
    {
        public static Dictionary<Type, object> data = new();
        static OpFactory()
        {
            data = new()
            {
                { typeof(int), new IntOps() },
                { typeof(uint), new UintOps() },
                { typeof(long), new LongOps() },
                { typeof(ulong), new UlongOps() },
                { typeof(float), new FloatOps() },
                { typeof(double), new DoubleOps() },
                { typeof(decimal), new DecimalOps() },
                { typeof(BigInteger), new BigIntegerOps() },
                { typeof(GmpInt), new GmpIntOps() },
            };
        }

        public static OpFactory<T> GetOpFactory<T>()
        {
            var typeOfT = typeof(T);
            if (data.ContainsKey(typeOfT))
                return (OpFactory<T>)data[typeOfT];

            throw new NotImplementedException($"OpFactory not registered for type {typeOfT}");
        }


    }

    public static class Ops<T>
    {
        static Ops()
        {
            var op = OpFactory.GetOpFactory<T>();

            ConvertFromInt = op.ConvertFromInt;
            ConvertToInt = op.ConvertToInt;

            ToGmpInt = op.ToGmpInt;
            FromGmpInt = op.FromGmpInt;

            Compare = op.Compare;
            Greater = op.Greater;
            GreaterOrEqual = op.GreaterOrEqual;
            Equal = op.Equal;
            NotEqual = op.NotEqual;
            LessOrEqual = op.LessOrEqual;
            Less = op.Less;

            Add = op.Add;
            Subtract = op.Subtract;
            Divide = op.Divide;
            Multiply = op.Multiply;
            Mod = op.Mod;

            LeftShift = op.LeftShift;
            RightShift = op.RightShift;

            And = op.And;
            Or = op.Or;
            Xor = op.Xor;

            Gcd = op.Gcd;
            Gcd2 = op.Gcd2;
            GcdT = op.GcdT;

            GcdExt = op.GcdExt;
            GcdExt2 = op.GcdExt2;
            GcdExtT = op.GcdExtT;

            Abs = op.Abs;
            AbsT = op.AbsT;

            Neg = op.Neg;
            NegT = op.NegT;

            CompareT = op.CompareT;
            GreaterT = op.GreaterT;
            GreaterOrEqualT = op.GreaterOrEqualT;
            EqualT = op.EqualT;
            NotEqualT = op.NotEqualT;
            LessOrEqualT = op.LessOrEqualT;
            LessT = op.LessT;

            AddT = op.AddT;
            SubtractT = op.SubtractT;
            DivideT = op.DivideT;
            MultiplyT = op.MultiplyT;
            ModT = op.ModT;

            AndT = op.AndT;
            OrT = op.OrT;
            XorT = op.XorT;
        }

        public static Func<int, T> ConvertFromInt { get; } = null!;
        public static Func<T, int> ConvertToInt { get; } = null!;

        public static Func<T, GmpInt> ToGmpInt { get; } = null!;
        public static Func<GmpInt, T> FromGmpInt { get; } = null!;

        public static Func<GmpInt, T, int> Compare { get; } = null!;
        public static Func<T, T, int> CompareT { get; } = null!;

        public static Func<GmpInt, T, bool> Greater { get; } = null!;
        public static Func<T, T, bool> GreaterT { get; } = null!;
        public static Func<GmpInt, T, bool> GreaterOrEqual { get; } = null!;
        public static Func<T, T, bool> GreaterOrEqualT { get; } = null!;
        public static Func<GmpInt, T, bool> Equal { get; } = null!;
        public static Func<T, T, bool> EqualT { get; } = null!;
        public static Func<GmpInt, T, bool> NotEqual { get; } = null!;
        public static Func<T, T, bool> NotEqualT { get; } = null!;
        public static Func<GmpInt, T, bool> LessOrEqual { get; } = null!;
        public static Func<T, T, bool> LessOrEqualT { get; } = null!;
        public static Func<GmpInt, T, bool> Less { get; } = null!;
        public static Func<T, T, bool> LessT { get; } = null!;

        public static Func<GmpInt, T, GmpInt> Add { get; } = null!;
        public static Func<T, T, T> AddT { get; } = null!;
        public static Func<GmpInt, T, GmpInt> Subtract { get; } = null!;
        public static Func<T, T, T> SubtractT { get; } = null!;
        public static Func<GmpInt, T, GmpInt> Divide { get; } = null!;
        public static Func<T, T, T> DivideT { get; } = null!;
        public static Func<GmpInt, T, GmpInt> Multiply { get; } = null!;
        public static Func<T, T, T> MultiplyT { get; } = null!;
        public static Func<GmpInt, T, GmpInt> Mod { get; } = null!;
        public static Func<T, T, T> ModT { get; } = null!;

        public static Func<GmpInt, T, GmpInt> LeftShift { get; } = null!;
        public static Func<GmpInt, T, GmpInt> RightShift { get; } = null!;

        public static Func<GmpInt, T, GmpInt> And { get; } = null!;
        public static Func<T, T, T> AndT { get; } = null!;
        public static Func<GmpInt, T, GmpInt> Or { get; } = null!;
        public static Func<T, T, T> OrT { get; } = null!;
        public static Func<GmpInt, T, GmpInt> Xor { get; } = null!;
        public static Func<T, T, T> XorT { get; } = null!;

        public static Func<GmpInt, T, GmpInt> Gcd { get; } = null!;
        public static Func<T, GmpInt, GmpInt> Gcd2 { get; } = null!;
        public static Func<T, T, T> GcdT { get; } = null!;
        public static Func<GmpInt, T, GcdExtResult<GmpInt>> GcdExt { get; } = null!;
        public static Func<T, GmpInt, GcdExtResult<GmpInt>> GcdExt2 { get; } = null!;
        public static Func<T, T, GcdExtResult<T>> GcdExtT { get; } = null!;

        public static Func<T, GmpInt> Abs { get; } = null!;
        public static Func<T, T> AbsT { get; } = null!;

        public static Func<T, GmpInt> Neg { get; } = null!;
        public static Func<T, T> NegT { get; } = null!;
    }

    public abstract class OpFactory<T>
    {

        public Func<int, T> ConvertFromInt { get; protected set; } = null!;
        public Func<T, int> ConvertToInt { get; protected set; } = null!;

        public Func<T, GmpInt> ToGmpInt { get; protected set; } = null!;
        public Func<GmpInt, T> FromGmpInt { get; protected set; } = null!;

        public Func<GmpInt, T, int> Compare { get; protected set; } = null!;
        public Func<T, T, int> CompareT { get; protected set; } = null!;

        public Func<GmpInt, T, bool> Greater { get; protected set; } = null!;
        public Func<T, T, bool> GreaterT { get; protected set; } = null!;
        public Func<GmpInt, T, bool> GreaterOrEqual { get; protected set; } = null!;
        public Func<T, T, bool> GreaterOrEqualT { get; protected set; } = null!;
        public Func<GmpInt, T, bool> Equal { get; protected set; } = null!;
        public Func<T, T, bool> EqualT { get; protected set; } = null!;
        public Func<GmpInt, T, bool> NotEqual { get; protected set; } = null!;
        public Func<T, T, bool> NotEqualT { get; protected set; } = null!;
        public Func<GmpInt, T, bool> LessOrEqual { get; protected set; } = null!;
        public Func<T, T, bool> LessOrEqualT { get; protected set; } = null!;
        public Func<GmpInt, T, bool> Less { get; protected set; } = null!;
        public Func<T, T, bool> LessT { get; protected set; } = null!;

        public Func<GmpInt, T, GmpInt> Add { get; protected set; } = null!;
        public Func<T, T, T> AddT { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> Subtract { get; protected set; } = null!;
        public Func<T, T, T> SubtractT { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> Divide { get; protected set; } = null!;
        public Func<T, T, T> DivideT { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> Multiply { get; protected set; } = null!;
        public Func<T, T, T> MultiplyT { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> Mod { get; protected set; } = null!;
        public Func<T, T, T> ModT { get; protected set; } = null!;

        public Func<GmpInt, T, GmpInt> LeftShift { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> RightShift { get; protected set; } = null!;

        public Func<GmpInt, T, GmpInt> And { get; protected set; } = null!;
        public Func<T, T, T> AndT { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> Or { get; protected set; } = null!;
        public Func<T, T, T> OrT { get; protected set; } = null!;
        public Func<GmpInt, T, GmpInt> Xor { get; protected set; } = null!;
        public Func<T, T, T> XorT { get; protected set; } = null!;

        public Func<GmpInt, T, GmpInt> Gcd { get; protected set; } = null!;
        public Func<T, GmpInt, GmpInt> Gcd2 { get; protected set; } = null!;
        public Func<T, T, T> GcdT { get; protected set; } = null!;
        public Func<GmpInt, T, GcdExtResult<GmpInt>> GcdExt { get; protected set; } = null!;
        public Func<T, GmpInt, GcdExtResult<GmpInt>> GcdExt2 { get; protected set; } = null!;
        public Func<T, T, GcdExtResult<T>> GcdExtT { get; protected set; } = null!;

        public Func<T, GmpInt> Abs { get; protected set; } = null!;
        public Func<T, T> AbsT { get; protected set; } = null!;

        public Func<T, GmpInt> Neg { get; protected set; } = null!;
        public Func<T, T> NegT { get; protected set; } = null!;
    }

    public class IntOps : OpFactory<int>
    {
        public IntOps()
        {
            ConvertFromInt = (a) => (int)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (int)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + b;
            Subtract = (a, b) => a - b;
            Divide = (a, b) => a / b;
            Multiply = (a, b) => a * b;
            Mod = (a, b) => a % b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & b;
            Or = (a, b) => a | b;
            Xor = (a, b) => a ^ b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);

            GcdExt = (a, b) => MathUtil.GcdExt(a, b);
            GcdExt2 = (a, b) => MathUtil.GcdExt(a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +a;
            AbsT = (a) => +a;

            Neg = (a) => -a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;

            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => a & b;
            OrT = (a, b) => a | b;
            XorT = (a, b) => a ^ b;
        }


    }

    public class UintOps : OpFactory<uint>
    {
        public UintOps()
        {
            ConvertFromInt = (a) => (uint)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (uint)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + b;
            Subtract = (a, b) => a - b;
            Divide = (a, b) => a / b;
            Multiply = (a, b) => a * b;
            Mod = (a, b) => a % b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & b;
            Or = (a, b) => a | b;
            Xor = (a, b) => a ^ b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);


            GcdExt = (a, b) => MathUtil.GcdExt(a, b);
            GcdExt2 = (a, b) => MathUtil.GcdExt(a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +a;
            AbsT = (a) => +a;

            Neg = (a) => -a;
            NegT = (a) => a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;

            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;


            AndT = (a, b) => a & b;
            OrT = (a, b) => a | b;
            XorT = (a, b) => a ^ b;
        }


    }

    public class LongOps : OpFactory<long>
    {
        public LongOps()
        {
            ConvertFromInt = (a) => (long)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (int)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + b;
            Subtract = (a, b) => a - b;
            Divide = (a, b) => a / b;
            Multiply = (a, b) => a * b;
            Mod = (a, b) => a % b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & b;
            Or = (a, b) => a | b;
            Xor = (a, b) => a ^ b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);

            GcdExt = (a, b) => MathUtil.GcdExt(a, b);
            GcdExt2 = (a, b) => MathUtil.GcdExt(a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +a;
            AbsT = (a) => +a;

            Neg = (a) => -a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;

            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => a & b;
            OrT = (a, b) => a | b;
            XorT = (a, b) => a ^ b;
        }


    }

    public class UlongOps : OpFactory<ulong>
    {
        public UlongOps()
        {
            ConvertFromInt = (a) => (ulong)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (ulong)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + b;
            Subtract = (a, b) => a - b;
            Divide = (a, b) => a / b;
            Multiply = (a, b) => a * b;
            Mod = (a, b) => a % b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & b;
            Or = (a, b) => a | b;
            Xor = (a, b) => a ^ b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);

            GcdExt = (a, b) => MathUtil.GcdExt(a, b);
            GcdExt2 = (a, b) => MathUtil.GcdExt(a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +a;
            AbsT = (a) => +a;

            Neg = (a) => -(GmpInt)a;
            NegT = (a) => a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;

            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => a & b;
            OrT = (a, b) => a | b;
            XorT = (a, b) => a ^ b;
        }


    }

    public class FloatOps : OpFactory<float>
    {
        public FloatOps()
        {
            ConvertFromInt = (a) => (float)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (float)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + (GmpInt)b;
            Subtract = (a, b) => a - (GmpInt)b;
            Divide = (a, b) => a / (GmpInt)b;
            Multiply = (a, b) => a * (GmpInt)b;
            Mod = (a, b) => a % (GmpInt)b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & (GmpInt)b;
            Or = (a, b) => a | (GmpInt)b;
            Xor = (a, b) => a ^ (GmpInt)b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);


            GcdExt = (a, b) => MathUtil.GcdExt(a, (GmpInt)b);
            GcdExt2 = (a, b) => MathUtil.GcdExt((GmpInt)a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +(GmpInt)a;
            AbsT = (a) => +a;

            Neg = (a) => -(GmpInt)a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;

            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => FromGmpInt((GmpInt)a & (GmpInt)b);
            OrT = (a, b) => FromGmpInt((GmpInt)a |(GmpInt)b);
            XorT = (a, b) => FromGmpInt((GmpInt)a ^ (GmpInt)b);
        }


    }

    public class DoubleOps : OpFactory<double>
    {
        public DoubleOps()
        {
            ConvertFromInt = (a) => (double)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (double)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + (GmpInt)b;
            Subtract = (a, b) => a - (GmpInt)b;
            Divide = (a, b) => a / (GmpInt)b;
            Multiply = (a, b) => a * (GmpInt)b;
            Mod = (a, b) => a % (GmpInt)b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & (GmpInt)b;
            Or = (a, b) => a | (GmpInt)b;
            Xor = (a, b) => a ^ (GmpInt)b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);

            GcdExt = (a, b) => MathUtil.GcdExt(a, (GmpInt)b);
            GcdExt2 = (a, b) => MathUtil.GcdExt((GmpInt)a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +(GmpInt)a;
            AbsT = (a) => +a;

            Neg = (a) => -(GmpInt)a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;

            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => FromGmpInt((GmpInt)a & (GmpInt)b);
            OrT = (a, b) => FromGmpInt((GmpInt)a | (GmpInt)b);
            XorT = (a, b) => FromGmpInt((GmpInt)a ^ (GmpInt)b);
        }
    }

    public class DecimalOps : OpFactory<decimal>
    {
        public DecimalOps()
        {
            ConvertFromInt = (a) => (decimal)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (decimal)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + (GmpInt)b;
            Subtract = (a, b) => a - (GmpInt)b;
            Divide = (a, b) => a / (GmpInt)b;
            Multiply = (a, b) => a * (GmpInt)b;
            Mod = (a, b) => a % (GmpInt)b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & (GmpInt)b;
            Or = (a, b) => a | (GmpInt)b;
            Xor = (a, b) => a ^ (GmpInt)b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);


            GcdExt = (a, b) => MathUtil.GcdExt(a, (GmpInt)b);
            GcdExt2 = (a, b) => MathUtil.GcdExt((GmpInt)a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +(GmpInt)a;
            AbsT = (a) => +a;

            Neg = (a) => -(GmpInt)a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;


            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => FromGmpInt((GmpInt)a & (GmpInt)b);
            OrT = (a, b) => FromGmpInt((GmpInt)a | (GmpInt)b);
            XorT = (a, b) => FromGmpInt((GmpInt)a ^ (GmpInt)b);
        }
    }

    public class BigIntegerOps : OpFactory<BigInteger>
    {
        public BigIntegerOps()
        {
            ConvertFromInt = (a) => (BigInteger)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => (GmpInt)a;
            FromGmpInt = (a) => (BigInteger)a;

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + b;
            Subtract = (a, b) => a - b;
            Divide = (a, b) => a / b;
            Multiply = (a, b) => a * b;
            Mod = (a, b) => a % b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & b;
            Or = (a, b) => a | b;
            Xor = (a, b) => a ^ b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);

            GcdExt = (a, b) => MathUtil.GcdExt(a, b);
            GcdExt2 = (a, b) => MathUtil.GcdExt(a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +a;
            AbsT = (a) => +a;

            Neg = (a) => -a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;


            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => a & b;
            OrT = (a, b) => a | b;
            XorT = (a, b) => a ^ b;


        }
    }

    public class GmpIntOps : OpFactory<GmpInt>
    {
        public GmpIntOps()
        {
            ConvertFromInt = (a) => (GmpInt)a;
            ConvertToInt = (a) => (int)a;

            ToGmpInt = (a) => a.Clone();
            FromGmpInt = (a) => a.Clone();

            Compare = (a, b) => a.CompareTo(b);
            Greater = (a, b) => a > b;
            GreaterOrEqual = (a, b) => a >= b;
            Equal = (a, b) => a == b;
            NotEqual = (a, b) => a != b;
            LessOrEqual = (a, b) => a <= b;
            Less = (a, b) => a < b;

            Add = (a, b) => a + b;
            Subtract = (a, b) => a - b;
            Divide = (a, b) => a / b;
            Multiply = (a, b) => a * b;
            Mod = (a, b) => a % b;

            LeftShift = (a, b) => a << ConvertToInt(b);
            RightShift = (a, b) => a >> ConvertToInt(b);

            And = (a, b) => a & b;
            Or = (a, b) => a | b;
            Xor = (a, b) => a ^ b;

            Gcd = (a, b) => MathUtil.Gcd(a, b);
            Gcd2 = (a, b) => MathUtil.Gcd(a, b);
            GcdT = (a, b) => MathUtil.Gcd(a, b);

            GcdExt = (a, b) => MathUtil.GcdExt(a, b);
            GcdExt2 = (a, b) => MathUtil.GcdExt(a, b);
            GcdExtT = (a, b) => MathUtil.GcdExt(a, b);

            Abs = (a) => +a;
            AbsT = (a) => +a;

            Neg = (a) => -a;
            NegT = (a) => -a;

            CompareT = (a, b) => a.CompareTo(b);
            GreaterT = (a, b) => a > b;
            GreaterOrEqualT = (a, b) => a >= b;
            EqualT = (a, b) => a == b;
            NotEqualT = (a, b) => a != b;
            LessOrEqualT = (a, b) => a <= b;
            LessT = (a, b) => a < b;


            AddT = (a, b) => a + b;
            SubtractT = (a, b) => a - b;
            DivideT = (a, b) => a / b;
            MultiplyT = (a, b) => a * b;
            ModT = (a, b) => a % b;

            AndT = (a, b) => a & b;
            OrT = (a, b) => a | b;
            XorT = (a, b) => a ^ b;
        }
    }

}



