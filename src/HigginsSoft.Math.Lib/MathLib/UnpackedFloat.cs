/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/


using System.Text;

namespace HigginsSoft.Math.Lib
{

    public class UnpackedFloat
    {
        /// <summary>
        /// The sign of the float. Bit 1 (bits[0])
        /// </summary>
        public uint Sign;

        /// <summary>
        /// The exponent of the float. 7 bits in length - Bits 2-9 (bits[1]-bits[8])
        /// </summary>
        public int RawExponent;

        /// <summary>
        /// The fraction of the float. 24 bits in length: Bits 9-32 (bits[8]-bits[31])
        /// </summary>
        public uint RawFraction;

        #region constants

        public const uint SignMask = 0x8000_0000;
        public const int SignShift = 31;

        public const uint ExponentMask = 0x7F80_0000;
        public const int ExponentShiftSize = 23;
        public const uint ShiftedExponentMask = ExponentMask >> ExponentShiftSize;

        public const uint FractionMask = 0x007F_FFFF;

        public const byte MinSign = 0;
        public const byte MaxSign = 1;

        public const byte MinExponent = 0x00;
        public const byte MaxExponent = 0xFF;

        public const uint MinSignificand = 0x0000_0000;
        public const uint MaxSignificand = 0x007F_FFFF;
        public const int ExponentBitLength = 7;
        public const int FloatBits = 32;
        public const int Bias = (1 << ExponentBitLength) - 1;

        public static readonly UnpackedFloat Zero = new UnpackedFloat();
        public static readonly UnpackedFloat One = new UnpackedFloat(1);
        public static readonly UnpackedFloat NegativeOne = new UnpackedFloat(-1);
        #endregion
        public float ToFloat()
        {
            var packed = ToPackedUint();
            var result = BitConverter.UInt32BitsToSingle(packed);
            return result;
        }

        public uint ToPackedUint()
             => RawFraction | ((uint)RawExponent << ExponentShiftSize) | ((uint)Sign) << SignShift;

        public string FractionBits => ((GmpInt)RawFraction).ToString(2).PadLeft(ExponentShiftSize, '0');
        public string ExponentBits => ((GmpInt)RawExponent).ToString(2).PadLeft(ExponentBitLength, '0');
        public string Bits => $"{Sign} {ExponentBits} {FractionBits}";



        public uint ToUint => (1u << NormalizedExponent) + (RawFraction >> (ExponentShiftSize - NormalizedExponent));

        public int NormalizedExponent => RawExponent - Bias;

        public int AbsExponent => MathLib.Abs(NormalizedExponent);

        public UnpackedFloat()
        {
            Sign = 0; RawExponent = 0; RawFraction = 0;
        }


        public UnpackedFloat(int value, bool packed)
            : this((uint)value, packed)
        {

        }

        public UnpackedFloat(uint value, bool packed)
        {
            if (packed)
            {
                Sign = ExtractSign(value);
                RawExponent = ExtractExponentFromBits(value);
                RawFraction = ExtractFraction(value);
            }
            else
            {
                pack(value);
            }

        }
        public UnpackedFloat(uint sign, int exponent, uint fraction)
        {

            Sign = sign;
            RawExponent = Bias + exponent;
            RawFraction = fraction;
        }


        public UnpackedFloat(float value)
            : this(BitConverter.SingleToInt32Bits(value), true)
        {

        }

        public override string ToString() => ToFloat().ToString();

        private void pack(uint value)
        {
            Sign = value > 0u ? 0u : 1u;
            RawExponent = 0; // todo calculate exponent
            if (value == 0)
                return;

            var absValue = (uint)value;
            var msbPosition = MathLib.ILogB(absValue);


            RawExponent = Bias + msbPosition;
            var fractionMask = (1u << msbPosition) - 1;
            var fraction = absValue & fractionMask;

            var shiftedValue = fraction << (ExponentShiftSize - msbPosition);

            RawFraction = shiftedValue & FractionMask;
        }

        #region extraction
        internal static uint ExtractSign(uint bits)
        {
            return (uint)((bits & SignMask) >> SignShift);
        }

        internal static int ExtractExponentFromBits(uint bits)
        {
            return (int)(bits >> ExponentShiftSize) & (int)ShiftedExponentMask;
        }

        internal static uint ExtractFraction(uint bits)
        {
            return bits & FractionMask;
        }

        public uint ExtractWholePart()
        {
            var exp = NormalizedExponent;
            if (exp < 0) return 0;
            var wholeShift = ExponentShiftSize - exp;
            var @base = 1u << exp;
            uint fraction = RawFraction;
            uint wholeBitMask = (@base - 1) << wholeShift;
            uint wholePart = @base + ((fraction & wholeBitMask) >> wholeShift);
            return wholePart;

        }

        public float ExtractDecimalPart()
        {
            var exp = NormalizedExponent;
            if (RawExponent == 0) return 0;

            var absExp = MathLib.Abs(NormalizedExponent);
            var wholeShift = ExponentShiftSize - absExp;
            var @base = 1u << absExp;
            var dbase = exp > 0 ? @base : 1.0f / @base;

            uint fraction = RawFraction;
            uint wholeBitMask = ~FractionMask;
            if (exp > -1)
            {
                wholeBitMask = (@base - 1) << wholeShift;
            }

            var decimalMask = ~wholeBitMask;
            var decimalBits = fraction & decimalMask;

            if (decimalBits == 0)
            {
                if (exp > -1)
                    return 0; //$"{sign}{wholePart}";
                else
                    return dbase;// $"{sign}{dbase}";
            }
            else
            {
                var decimalShift = ExponentShiftSize - exp;
                var decimalBaseTwoValue = 1u << (decimalShift);
                var decimalBase10 = (float)decimalBits / decimalBaseTwoValue;

                if (exp < 0)
                    decimalBase10 += dbase;

                return decimalBase10;
            }

        }

        #endregion


        public static implicit operator UnpackedFloat(float x)
        {
            return new UnpackedFloat(x);
        }
        public static explicit operator UnpackedFloat(uint x)
        {
            return new UnpackedFloat(x);
        }

        public static explicit operator UnpackedFloat(int x)
        {
            return new UnpackedFloat(x);
        }


        public static UnpackedFloat operator *(UnpackedFloat left, UnpackedFloat right)
            => Multiply(left, right);


        public int SignificantBits
        {
            get
            {
                if (NormalizedExponent < 0)
                {
                    return (NormalizedExponent == -1) ? 0 : MathLib.Abs(NormalizedExponent);
                }
                else
                {
                    var result = 0;
                    result = ExponentShiftSize - NormalizedExponent;
                    return result;
                }
            }
        }
        public string BinaryDecimal
        {
            get
            {
                var result = string.Empty;
                if (NormalizedExponent < 0)
                {
                    var fractionBits = RawFraction | (1 << ExponentShiftSize);

                    var leadingZeros = (NormalizedExponent == -1) ? "" : "".PadLeft(MathLib.Abs(NormalizedExponent) - 1, '0');
                    var fractionBinary = ((GmpInt)fractionBits).ToString(2);
                    result = $"0.{leadingZeros}{fractionBinary}";
                }
                else
                {
                    var fractionBits = RawFraction & (FractionMask >> NormalizedExponent);
                    var fractionBinary = ((GmpInt)fractionBits).ToString(2);
                    var wordBits = (RawFraction | (1ul << ExponentShiftSize)) >> (ExponentShiftSize - NormalizedExponent);
                    var wordBinary = ((GmpInt)wordBits).ToString(2);
                    result = $"{wordBinary}.{fractionBinary}";

                }
                return result;
            }
        }




        private static UnpackedFloat MultiplyWholePositiveInt(UnpackedFloat a, UnpackedFloat b)
        {
            int decimalA = (int)a.RawFraction;
            int decimalB = (int)b.RawFraction;
            decimalA = decimalA | (1 << ExponentShiftSize);
            decimalB = decimalB | (1 << ExponentShiftSize);

            long decimalC;
            int exp;

            decimalA >>= (ExponentShiftSize - a.NormalizedExponent);
            decimalB >>= (ExponentShiftSize - b.NormalizedExponent);
            //decimalA <<= a.NormalizedExponent; 
            //decimalB <<= b.NormalizedExponent;
            //decimalA >>= ExponentShiftSize; 
            //decimalB >>= ExponentShiftSize;
            decimalC = decimalA * decimalB;

            //todo can we take whole part while normalized?
            //otherwise (decimalC & ExponentMask) fails if c<<taking mask fails if c < int.maxvalue
            //fraction will fail too, but we will have no fraction if we already normalized to whole numbers.
            var wholePart = decimalC;

            exp = MathLib.ILogB(wholePart);
            var wholeBits = wholePart & ~(1 << exp);
            uint fract = (uint)wholeBits;
            fract <<= (ExponentShiftSize - exp);
            //need to clear bit and set whole part;

            var sign = a.Sign ^ b.Sign;

            var result = new UnpackedFloat(sign, exp, fract);

            bool checkResult = bool.Parse(bool.FalseString);
            if (checkResult)
            {
                var expectedResult = new UnpackedFloat(a.ToFloat() * b.ToFloat());
                var expectedFloat = expectedResult.ToFloat();
                var actualFloat = result.ToFloat();
                if (expectedFloat != actualFloat)
                {
                    string bp = $"Wrong calculation: Actual: {actualFloat} - Expected: {expectedFloat}";
                }
            }

            return result;
        }


        private static UnpackedFloat MultiplyWholePositiveLong(UnpackedFloat a, UnpackedFloat b)
        {
            long decimalA = (long)a.RawFraction;
            long decimalB = (long)b.RawFraction;

            decimalA = decimalA | (1L << ExponentShiftSize);
            decimalB = decimalB | (1L << ExponentShiftSize);

            long decimalC;
            int exp;

            //right shift by the normalized exponent so we can work within a machine size word.
            //      Note: The fraction can be truncated here, so this can be wrong ith non-whole numbers.
            decimalA >>= (ExponentShiftSize - a.NormalizedExponent);
            decimalB >>= (ExponentShiftSize - b.NormalizedExponent);

            decimalC = decimalA * decimalB;

            //todo can we take whole part while normalized?
            //otherwise (decimalC & ExponentMask) fails if c<<taking mask fails if c < int.maxvalue
            //fraction will fail too, but we will have no fraction if we already normalized to whole numbers.
            var wholePart = decimalC;


            //int log 2;
            exp = MathLib.ILogB(wholePart);

            var expDouble = System.Math.Log2(exp);
            var baseFloat = (float)(1 << exp);


            var low = wholePart & (~(1L << exp));
            var baseFract = low / baseFloat;
            // calculate mask to get all bits except MSB exponent bit.


            // extract all bits except MSB. Could also do whole part & (~(1l<<exp));
            //  var fractmask = (1l << exp) - 1;
            //  var low = wholepart & fract mask
            // more efficient just to clear the exponent bit.



            var overflow = exp - 23;
            var doubleOverflow = expDouble - 23;
            //determin rounding by checking if msb of truncated parts is set.
            //wrong rounding logic ot check msb of truncated bit

            bool roundTrunc = false, roundMathLog = false, roundNone = false, roundBase = false;

            if (bool.Parse(bool.FalseString))
            {
                // gets all the way through 4096 * (4096-8,192) and fails at 4097*4097
                roundTrunc = true;
            }
            else if (bool.Parse(bool.TrueString))
            {
                roundMathLog = true;
            }
            else if (bool.Parse(bool.TrueString))
            {
                roundBase = true;
            }
            else
            {
                roundNone = true;
            }
;
            int round = 0;
            if (roundBase)
            {
                if (overflow > 0)
                {
                    round = (baseFract > .5F) ? 1 : 0;
                    low >>= overflow;
                    if (round > 0)
                    {
                        low++;
                    }
                }
            }
            else if (roundNone)
            {
                var truncMask = (1L << overflow) - 1;
                var truncPart = (low & truncMask);
                if (overflow > 0)
                {
                    low >>= (overflow);
                }
            }
            else if (roundMathLog)
            {
                if (doubleOverflow > .5)
                {
                    round = (expDouble - (int)expDouble) >= -.5 ? 1 : 0;
                    low >>= (int)MathLib.Floor(doubleOverflow);
                    if (round > 0)
                        low++;
                }
            }
            else if (roundTrunc)
            {

                if (overflow > 0)
                {
                    var truncMask = (1L << overflow) - 1;
                    round = (low & truncMask) > 0 ? 1 : 0;
                    low >>= (overflow - 1);
                    if (round > 0)
                        low += 1;
                    low >>= 1;
                }


            }
            else
            {

                if (overflow > 0)
                {
                    if (overflow > 1)
                    {
                        low >>= exp - 1;

                    }
                    round = (int)low & 1;
                    low >>= 1;
                }
            }



            uint fract = (uint)low;

            // bad rounding logic;
            //if (round > 0) fract++;


            //var wholeBits = wholePart & ~(1l << exp);
            //long fract = (uint)wholeBits;
            //fract <<= (ExponentShiftSize - overflow);
            ////need to clear bit and set whole part;

            var sign = a.Sign ^ b.Sign;

            var result = new UnpackedFloat(sign, exp, (uint)fract);

            bool checkResult = bool.Parse(bool.TrueString);
            if (checkResult)
            {
                var expectedInt = ((int)a.ToFloat()) * ((int)b.ToFloat());
                var unpackedInt = new UnpackedDouble(expectedInt);
                var expectedResult = new UnpackedFloat(a.ToFloat() * b.ToFloat());
                var expectedFloat = expectedResult.ToFloat();
                var actualFloat = result.ToFloat();
                if (expectedFloat != actualFloat)
                {
                    string bp = $"Wrong calculation: Actual: {actualFloat} - Expected: {expectedFloat}";
                }
            }

            return result;
        }


        public static UnpackedFloat Multiply(UnpackedFloat a, UnpackedFloat b)
        {

            if (a.RawExponent == 0)
                return Zero;
            if (b.RawExponent == 0)
                return Zero;

            //TODO: when does this fail due to precision? works for a & b up to 1<<12, with float max at 1<<12;
            // also, to what point can we use int instead of long?
            if (a.NormalizedExponent > -1 && b.NormalizedExponent > -1)
            {
                bool useInt = true;
                bool useLong = true;
                if (useInt)
                {
                    if (a.NormalizedExponent + b.NormalizedExponent < 24)
                        return MultiplyWholePositiveInt(a, b);
                    else
                        return MultiplyWholePositiveLong(a, b);
                }
                else // if (useLong)
                {
                    long decimalA = a.RawFraction;
                    long decimalB = b.RawFraction;
                    decimalA = decimalA | (1 << ExponentShiftSize);
                    decimalB = decimalB | (1 << ExponentShiftSize);

                    long decimalC;
                    int exp;


                    decimalA <<= a.NormalizedExponent; decimalB <<= b.NormalizedExponent;
                    decimalA >>= ExponentShiftSize; decimalB >>= ExponentShiftSize;
                    decimalC = decimalA * decimalB;

                    //todo can we take whole part while normalized?
                    //otherwise (decimalC & ExponentMask) fails if c<<taking mask fails if c < int.maxvalue
                    //fraction will fail too, but we will have no fraction if we already normalized to whole numbers.
                    var wholePart = decimalC;

                    exp = MathLib.ILogB(wholePart);
                    var wholeBits = wholePart & ~(1 << exp);
                    uint fract = (uint)wholeBits;
                    fract <<= ExponentShiftSize - exp;
                    //need to clear bit and set whole part;

                    var sign = a.Sign ^ b.Sign;

                    var result = new UnpackedFloat(sign, exp, fract);


                    return result;
                }
            }
            else
            {
                if (a.NormalizedExponent < 0 && b.NormalizedExponent < 0)
                {
                    bool useint = bool.Parse(bool.TrueString);
                    if (useint)
                    {
                        int decimalA = (int)a.RawFraction;
                        int decimalB = (int)b.RawFraction;
                        decimalA = decimalA | (1 << ExponentShiftSize);
                        decimalB = decimalB | (1 << ExponentShiftSize);

                        long decimalC;
                        int exp;

                        decimalA >>= (ExponentShiftSize - a.NormalizedExponent);
                        decimalB >>= (ExponentShiftSize - b.NormalizedExponent);
                        //decimalA <<= a.NormalizedExponent; 
                        //decimalB <<= b.NormalizedExponent;
                        //decimalA >>= ExponentShiftSize; 
                        //decimalB >>= ExponentShiftSize;
                        decimalC = decimalA * decimalB;

                        //todo can we take whole part while normalized?
                        //otherwise (decimalC & ExponentMask) fails if c<<taking mask fails if c < int.maxvalue
                        //fraction will fail too, but we will have no fraction if we already normalized to whole numbers.
                        var wholePart = decimalC;

                        exp = MathLib.ILogB(wholePart);
                        var wholeBits = wholePart & ~(1 << exp);
                        uint fract = (uint)wholeBits;
                        fract <<= ExponentShiftSize - exp;
                        //need to clear bit and set whole part;

                        var sign = a.Sign ^ b.Sign;

                        var result = new UnpackedFloat(sign, exp, fract);


                        return result;
                    }
                    else //use long
                    {
                        var result = new UnpackedFloat();


                        return result;
                    }
                }
                else
                {

                    GmpInt decimalA = a.RawFraction;
                    GmpInt decimalB = b.RawFraction;
                    decimalA = decimalA | (1 << ExponentShiftSize);
                    decimalB = decimalB | (1 << ExponentShiftSize);

                    GmpInt decimalC;
                    bool round;
                    UnpackedFloat result;
                    int exp;

                    if (a.RawExponent > -1 && b.RawExponent > -1)
                    {
                        decimalA <<= a.NormalizedExponent; decimalB <<= b.NormalizedExponent;
                        decimalA >>= ExponentShiftSize; decimalB >>= ExponentShiftSize;
                        decimalC = decimalA * decimalB;

                        //todo can we take whole part while normalized?
                        //otherwise (decimalC & ExponentMask) fails if c<<taking mask fails if c < int.maxvalue
                        //fraction will fail too, but we will have no fraction if we already normalized to whole numbers.
                        var wholePart = decimalC;

                        exp = MathLib.ILogB(wholePart);
                        var wholeBits = wholePart & ~(1 << exp);
                        uint fract = (uint)wholeBits;
                        fract <<= ExponentShiftSize - exp;
                        //need to clear bit and set whole part;

                        var sign = a.Sign ^ b.Sign;

                        result = new UnpackedFloat(sign, exp, fract);

                        var expected = a.ToFloat() * b.ToFloat();
                        if (result.ToFloat() != expected)
                        {
                            string message = $"{a} * {b} = {result} <> {expected}";
                            Console.WriteLine($"Warning: {message}");
                        }

                        return result;
                    }
                    else
                    {
                        decimalC = decimalA * decimalB;

                        decimalC >>= ExponentShiftSize;
                        round = (decimalC & (1 << ExponentShiftSize - 1)) > 0;

                        exp = a.NormalizedExponent + b.NormalizedExponent;
                        var fract = (uint)decimalC & FractionMask;

                        round = true;
                        if (round)
                        {
                            exp += 1;
                            fract += 1;
                            fract >>= 1;
                        }


                        //var packedResult = (uint)(decimalC >> ExponentShiftSize);
                        //packedResult += ExponentMask;
                        result = new UnpackedFloat(a.Sign ^ b.Sign, exp, fract);
                        return result;
                    }
                }
            }


        }

        static string debugString(GmpInt value, bool padLeft = false)
        {
            var r2 = value.ToString(2);//.PadLeft(FloatBits, '0');
            if (r2.Length == FloatBits)
            {
                r2 = $"{r2.Substring(0, 1)} {r2.Substring(1, ExponentBitLength)} {r2.Substring(ExponentBitLength + 1)}";
            }
            if (padLeft)
            {
                int padLeftSize = (ExponentShiftSize << 2) + (ExponentBitLength << 1);
                r2 = r2.PadLeft(padLeftSize, '0');
            }
            return r2;
        }


        static string debugString2(GmpInt value, int decimalPlace)
        {

            var r2 = value.ToString(2).PadLeft(FloatBits + decimalPlace, '0');
            if (r2.Length >= FloatBits)
            {
                r2 = r2.Substring(r2.Length - FloatBits);
                r2 = $"{r2.Substring(0, 1)} {r2.Substring(1, 11)} {r2.Substring(12)}";
            }
            else
            {
                return debugString(value);
            }
            return r2;
        }

        public static void DebugMul(UnpackedFloat a, UnpackedFloat b)
        {
            var cFloat = a.ToFloat() * b.ToFloat();
            var c = new UnpackedFloat(cFloat);
            Console.WriteLine("========================================");
            Console.WriteLine($"{a.ToFloat()} * {b.ToFloat()} = {cFloat}");
            Console.WriteLine();
            Console.WriteLine(a.DebugString(nameof(a)));
            Console.WriteLine();
            Console.WriteLine(b.DebugString(nameof(b)));
            Console.WriteLine();
            Console.WriteLine(c.DebugString(nameof(c)));

            GmpInt decimalA = a.RawFraction;
            GmpInt decimalB = b.RawFraction;
            decimalA = decimalA | (1 << ExponentShiftSize);
            decimalB = decimalB | (1 << ExponentShiftSize);
            //decimalA <<= ExponentShiftSize;
            //decimalB <<= ExponentShiftSize;
            if (a.NormalizedExponent < 0)
            {
                //decimalA >>= a.AbsExponent;
            }
            else
            {
                //decimalA <<= a.NormalizedExponent;
            }

            if (b.NormalizedExponent < 0)
            {
                //decimalB >>= b.AbsExponent;
            }
            else
            {
                //decimalB <<= b.NormalizedExponent;
            }

            GmpInt decimalC = decimalA * decimalB;


            //var maskstring = $"m = dddddddfffffffffffffffffffffffrrrrrrrrrrrrrrrrrrrrrrrdddddddfffffffffffffffffffffffrrrrrrrrrrrrrrrrrrrrrrr";
            Console.WriteLine();

            var debugA = debugString(decimalA, false);
            var debugB = debugString(decimalB, false);
            var debugC = debugString(decimalC, false);
            var padLeftSize = debugA.Length < debugB.Length ? debugB.Length : debugA.Length;
            padLeftSize *= 2;
            debugA = debugA.PadLeft(padLeftSize, '0');
            debugB = debugB.PadLeft(padLeftSize, '0');
            debugC = debugC.PadLeft(padLeftSize, '0');

            Console.WriteLine();
            Console.WriteLine("      ------------------            ");
            Console.WriteLine($"{nameof(a)} = {debugA}");
            Console.WriteLine();
            Console.WriteLine($"{nameof(b)} = {debugB}");
            Console.WriteLine();
            Console.WriteLine($"{nameof(c)} = {debugC}"); ;


            var decimalPlace = a.NormalizedExponent + b.NormalizedExponent;
            var round = (decimalC & (1 << ExponentShiftSize - 1)) > 0;


            Console.WriteLine($"Decimal Place: {decimalPlace}");
            Console.WriteLine($"Round: {round}");
            decimalC >>= ExponentShiftSize;
            debugC = debugString(decimalC, false);
            debugC = debugC.PadLeft(padLeftSize, '0');
            Console.WriteLine();
            Console.WriteLine($"{nameof(c)} = {debugC}");



            var fract = (uint)decimalC & FractionMask;
            var exp = (int)((decimalC & ExponentMask) >> ExponentShiftSize);

            Console.WriteLine($"{nameof(fract)} = {((GmpInt)fract).ToString(2).PadLeft(ExponentShiftSize, '0')}");
            Console.WriteLine($"{nameof(exp)} = {((GmpInt)exp).ToString(2) + "".PadLeft(ExponentShiftSize, '0')}");
            exp = decimalPlace;
            if (exp > 0)
            {
                exp = MathLib.ILogB(exp);
            }
            var test = new UnpackedFloat(a.Sign ^ b.Sign, exp, fract);

            Console.WriteLine(test.DebugString("packed"));

        }


        public string ToDebugString => DebugString(nameof(ToFloat));
        public string DebugString(string name)
        {

            var sb = new StringBuilder();
            sb.AppendLine($"{name}: {ToFloat()}");
            sb.AppendLine($"\t{nameof(Bits)}: {Bits}");
            sb.AppendLine($"\t{nameof(Sign)}: {Sign}");
            sb.AppendLine($"\t{nameof(ExponentBits)}: {ExponentBits}");
            sb.AppendLine($"\t{nameof(RawExponent)}: {RawExponent}");
            sb.AppendLine($"\t{nameof(NormalizedExponent)}: {NormalizedExponent}");
            sb.AppendLine($"\t{nameof(RawFraction)}: {RawFraction}");
            sb.AppendLine($"\t{nameof(FractionBits)}: {FractionBits}");
            sb.AppendLine($"\t{nameof(BinaryDecimal)}: {BinaryDecimal}");
            var result = sb.ToString();
            return result;

        }
    }
}
