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
    public class UnpackedDouble
    {

        /// <summary>
        /// The sign of the double. Bit 1 (bits[0])
        /// </summary>
        public int Sign;


        /// <summary>
        /// The sign of the double. 11 bits in length - Bits 2-12 (bits[1]-bits[11])
        /// </summary>
        public int Exponent;

        /// <summary>
        /// The fraction of the double. 52 bits in length: Bits 13-64 (bits[12]-bits[63])
        /// </summary>
        public ulong Fraction;


        public int RawExponent => Exponent - Bias;
        public ulong AsPackedUlong => ToPackedUlong();
        public double AsDouble => ToDouble();

        #region constants

        public const int Bias = 1023;

        internal const ulong SignMask = 0x8000_0000_0000_0000;
        internal const int SignShift = 63;

        internal const ulong ExponentMask = 0x7FF0_0000_0000_0000;
        internal const int ExponentShift = 52;
        internal const uint ShiftedExponentMask = (uint)(ExponentMask >> ExponentShift);


        public const ulong ExponentZero = 1ul << 62;
        public const ulong ExponentOne = 0x3FF_00000_0000_0000;
        public const ulong FractionMask = 0x000F_FFFF_FFFF_FFFF;

        internal const byte MinSign = 0;
        internal const byte MaxSign = 1;

        internal const ushort MinExponent = 0x0000;
        internal const ushort MaxExponent = 0x07FF;

        internal const ulong MinSignificand = 0x0000_0000_0000_0000;
        internal const ulong MaxSignificand = 0x000F_FFFF_FFFF_FFFF;

        public static readonly UnpackedDouble Zero = new UnpackedDouble();
        public static readonly UnpackedDouble One = new UnpackedDouble(1);
        public static readonly UnpackedDouble NegativeOne = new UnpackedDouble(-1);


        #endregion


        public UnpackedDouble()
        {
            Sign = 0; Exponent = 0; Fraction = 0;
        }

        public UnpackedDouble(ulong value, bool packed)
        {
            if (packed)
            {
                Sign = ExtractSign(value);
                Exponent = ExtractExponentFromBits(value);
                Fraction = ExtractFraction(value);
            }
            else
            {
                pack(value);
            }

        }

        public UnpackedDouble(double value)
            : this(BitConverter.DoubleToUInt64Bits(value), true)
        {

        }

        public override string ToString() => ToDouble().ToString();

        public UnpackedDouble(int value)
        {
            pack(value);
        }

        public UnpackedDouble(int sign, int productExponent, ulong productFract)
        {
            Sign = sign; Exponent = productExponent + 1023; Fraction = productFract;
        }

        private void pack(ulong value)
        {
            Sign = value > 0 ? 0 : 1;
            Exponent = 0; // todo calculate exponent
            if (value == 0)
                return;

            ulong absValue = value;
            var msbPosition = MathLib.ILogB(absValue);


            Exponent = Bias + msbPosition;
            var fractionMask = (1ul << msbPosition) - 1;
            var fraction = absValue & fractionMask;

            ulong shiftedValue = fraction << (52 - msbPosition);

            Fraction = shiftedValue & FractionMask;

        }

        private void pack(int value)
        {
            Sign = value > 0 ? 0 : 1;
            Exponent = 0; // todo calculate exponent
            if (value == 0)
                return;

            int absValue = MathLib.Abs(value);
            var msbPosition = MathLib.ILogB(absValue);




            Exponent = Bias + msbPosition;
            var fractionMask = (1 << msbPosition) - 1;
            var fraction = absValue & fractionMask;

            ulong shiftedValue = (ulong)fraction << (52 - msbPosition);

            //fraction for 15.
            //1110000000000000000000000000000000000000000000000000
            //1110000000000000000000000000000000000000000000000000
            // Mask the fraction bits
            Fraction = shiftedValue & FractionMask;

            //var thisBits = this.Bits;

            //ulong packed = ExponentOne + ((ulong)value << 52);
            //var expOneBits = ((GmpInt)ExponentOne).ToString(2).PadLeft(64, '0');
            //var packedBits = ((GmpInt)packed).ToString(2).PadLeft(64, '0');

            //var thisUlong = this.ToUlong();
            ////Debug.Assert(thisUlong == packed);

            //var packedToDouble = BitConverter.UInt64BitsToDouble(packed);
            //var thisDouble = this.ToDouble();
            //Debug.Assert(thisDouble == packedToDouble, "Failed to unpack value");
        }

        public string FractionBits => ((GmpInt)Fraction).ToString(2).PadLeft(52, '0');
        public string ExponentBits => ((GmpInt)Exponent).ToString(2).PadLeft(11, '0');
        public string Bits => $"{Sign} {ExponentBits} {FractionBits}";

        public ulong ToPackedUlong()
            => Fraction | ((ulong)Exponent << ExponentShift) | ((ulong)Sign) << SignShift;

        public ulong ToUlong => (1ul << RawExponent) + (Fraction >> (52 - RawExponent));

        public double ToDouble()
        {
            var packed = ToPackedUlong();
            var result = BitConverter.UInt64BitsToDouble(packed);
            return result;
        }


        public ulong ExtractWholePart1(int exp, ulong @base, int wholeShift, ulong fraction, out ulong wholeBitMask)
        {
            ulong wholePart = 0;
            wholeBitMask = ~FractionMask;
            if (exp > -1)
            {
                var wholeMask = @base - 1;
                wholeBitMask = wholeMask << wholeShift;
                var wholeBits = fraction & wholeBitMask;
                var wholeBitsNorm = wholeBits >> wholeShift;
                wholePart = @base + wholeBitsNorm;
            }
            return wholePart;
        }

        public string ToDoubleString()
        {

            var exp = RawExponent;
            if (Exponent == 0) return "0";

            var absExp = MathLib.Abs(RawExponent);
            var wholeShift = 52 - absExp;
            var @base = 1ul << absExp;
            var dbase = exp > 0 ? @base : 1.0 / @base;
            var sign = Sign > 0 ? "-" : "";
            ulong fraction = Fraction;

            //ulong wholePart = ExtractWholePart1(exp, @base, wholeShift, fraction, out ulong wholeBitMask);
            ulong wholePart = 0;
            ulong wholeBitMask = ~FractionMask;
            //ulong wholeBitMask = (@base - 1) << wholeShift;
            if (exp > -1)
            {
                // for debugging
                //var wholeMask = @base - 1;
                //wholeBitMask = wholeMask << wholeShift;
                //var wholeBits = fraction & wholeBitMask;
                //var wholeBitsNorm = wholeBits >> wholeShift;
                //wholePart = @base + wholeBitsNorm;

                wholeBitMask = (@base - 1) << wholeShift;
                wholePart = @base + ((fraction & wholeBitMask) >> wholeShift);

            }

            var decimalMask = ~wholeBitMask;
            var decimalBits = fraction & decimalMask;

            if (decimalBits == 0)
            {

                if (exp > -1)
                {
                    return $"{sign}{wholePart}";
                }
                else
                {
                    //var decimalBase = 1.0 / @base;
                    return $"{sign}{dbase}";
                }
            }
            else
            {
                /*            

                for binary values:
                log2(100)=2 log2(10)=1  log2(1)=0   log2(1/10)=-1 log2(1/100)=-2    log2(1/1000)=3

                for decimal values
                log2(4)=2   log2(2)=1   log2(1)=0   log2(1.0/2)=-1 log2(1.0/4)=-2   log2(1.0/8)=-3

                */


                // expected: .25, decimalPart=140737488355328, calculate n/decimalPart=25,
                //      16 bits in 0000, each char is 4 bits: 0-15.
                //      decimal part= 8000_0000_0000 ==> 48 bits
                //      and decimal part * 4 (inverse(.25)) = 140737488355328*4 = 562,949,953,421,312
                //      bits: decimal part*4 = 0x2_0000_0000_0000 = decimalPart*4= 50 bits;
                //      exp=3, 52-exponent=49;
                //      to get .25 base = 140737488355328/562,949,953,421,312=.25;

                // expected: .75, decimalpart= 2251799813685248
                //      decimal part= 8_0000_0000_0000 (same as .25)
                //      exponent=-1, absexp=1;
                //      

                var decimalShift = 52 - exp;
                var decimalBaseTwoValue = 1ul << (decimalShift);
                var decimalBase10 = (double)decimalBits / decimalBaseTwoValue;

                if (exp < 0)
                {
                    //var decimalBase = 1.0 / @base;
                    //decimalBase10 = decimalBase + decimalBase10;
                    decimalBase10 += dbase;
                }
                var normDecimalPart = $"{decimalBase10.ToString().TrimStart('0').TrimStart('.').TrimEnd('0')}";

                return $"{sign}{wholePart}.{normDecimalPart}";


            }

        }


        public ulong ExtractWholePart()
        {
            var exp = RawExponent;
            if (exp < 0) return 0;
            var wholeShift = 52 - exp;
            var @base = 1ul << exp;
            ulong fraction = Fraction;
            ulong wholeBitMask = (@base - 1) << wholeShift;
            ulong wholePart = @base + ((fraction & wholeBitMask) >> wholeShift);
            return wholePart;

        }
        public double ExtractDecimalPart()
        {
            var exp = RawExponent;
            if (Exponent == 0) return 0;

            var absExp = MathLib.Abs(RawExponent);
            var wholeShift = 52 - absExp;
            var @base = 1ul << absExp;
            var dbase = exp > 0 ? @base : 1.0 / @base;

            ulong fraction = Fraction;
            ulong wholeBitMask = ~FractionMask;
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
                var decimalShift = 52 - exp;
                var decimalBaseTwoValue = 1ul << (decimalShift);
                var decimalBase10 = (double)decimalBits / decimalBaseTwoValue;

                if (exp < 0)
                    decimalBase10 += dbase;

                return decimalBase10;
            }

        }





        internal static int ExtractSign(ulong bits)
        {
            return (int)((bits & SignMask) >> SignShift);
        }

        internal static int ExtractExponentFromBits(ulong bits)
        {
            return (int)(bits >> ExponentShift) & (int)ShiftedExponentMask;
        }

        internal static ulong ExtractFraction(ulong bits)
        {
            return bits & FractionMask;
        }

        public static UnpackedDouble operator *(UnpackedDouble left, UnpackedDouble right)
            => Multiply(left, right);



        public string ToBinaryDecimal
        {
            get
            {
                var result = string.Empty;
                if (RawExponent < 0)
                {
                    var fractionBits = Fraction;
                    var leadingZeros = (RawExponent == -1) ? "" : "".PadLeft(MathLib.Abs(RawExponent) - 1, '0');
                    var fractionBinary = ((GmpInt)Fraction).ToString(2);
                    result = $"0.{leadingZeros}{fractionBinary}";
                }
                else
                {
                    var fractionBits = Fraction & (FractionMask >> RawExponent);
                    var fractionBinary = ((GmpInt)fractionBits).ToString(2);
                    var wordBits = (Fraction | (1ul << 52)) >> (52 - RawExponent);
                    var wordBinary = ((GmpInt)wordBits).ToString(2);
                    result = $"{wordBinary}.{fractionBinary}";

                }
                return result;
            }
        }

        public static UnpackedDouble Multiply(UnpackedDouble a, UnpackedDouble b)
        {

            if (a.Exponent == 0)
                return Zero;
            if (b.Exponent == 0)
                return Zero;

            string debugString(GmpInt value)
            {
                var r2 = value.ToString(2).PadLeft(64, '0');
                if (r2.Length == 64)
                {
                    r2 = $"{r2.Substring(0, 1)} {r2.Substring(1, 11)} {r2.Substring(12)}";
                }
                return r2;
            }


            string debugString2(GmpInt value, int decimalPlace)
            {

                var r2 = value.ToString(2).PadLeft(64 + decimalPlace, '0');
                if (r2.Length >= 64)
                {
                    r2 = r2.Substring(r2.Length - 64);
                    r2 = $"{r2.Substring(0, 1)} {r2.Substring(1, 11)} {r2.Substring(12)}";
                }
                else
                {
                    return debugString(value);
                }
                return r2;
            }

            var wholea = a.ExtractWholePart();
            var deca = a.ExtractDecimalPart();
            var wholeb = b.ExtractWholePart();
            var decb = b.ExtractDecimalPart();

            {

                var decimalBitsA = a.Fraction;
                var decimalBitsB = b.Fraction;
                if (a.RawExponent > -1 && b.RawExponent > -1)
                {
                    string debugA, debugB, debugProduct;
                    GmpInt decimalA = decimalBitsA;
                    GmpInt decimalB = decimalBitsB;
                    // denormaliza a

                    decimalA = decimalA | 1ul << 52;
                    debugA = debugString(decimalA);

                    // align to decimal place to 52 bits;
                    var decimalAShifted = decimalA << a.RawExponent;
                    debugA = debugString2(decimalAShifted, a.RawExponent);


                    //denormalize b
                    decimalB = decimalB | 1ul << 52;
                    debugB = debugString(decimalB);


                    // align to decimal place to 52 bits;
                    var decimalBShifted = decimalB <<= b.RawExponent;
                    debugB = debugString2(decimalBShifted, b.RawExponent);


                    // calculate the product
                    var denormalizedProduct = decimalAShifted * decimalBShifted;
                    denormalizedProduct >>= 52;
                    debugProduct = debugString(denormalizedProduct);

                    // get the integer part of the product;
                    var wholeProduct = denormalizedProduct & ExponentMask;
                    debugProduct = debugString(wholeProduct);

                    // get the expontnet
                    var wholeProductNormalized = (ulong)wholeProduct >> 52;
                    //var productExponentLen = MathLib.BitLength(wholeProductNormalized) - 1;
                    // debug use log2, same operations but bitlength prefers asm lzcount first. but some double tests fail with bitlength
                    var productExponent = MathLib.ILogB(wholeProductNormalized);
                    //if (productExponent != productExponentLen)
                    //{
                    //    string bp = $"BitLen failed for {wholeProductNormalized} ";
                    //}

                    // Get the decimal part
                    var productFract = denormalizedProduct & ~wholeProduct;

                    var fullProductFact = productFract | (wholeProduct >> (productExponent));
                    debugProduct = debugString(fullProductFact);

                    fullProductFact = fullProductFact & FractionMask;
                    debugProduct = debugString(fullProductFact);

                    // Get the Sign;
                    var productSign = a.Sign ^ b.Sign;

                    var packedResult = new UnpackedDouble(productSign, productExponent, (ulong)fullProductFact);
                    debugProduct = debugString(packedResult.AsPackedUlong);
                    if (packedResult.ToDouble() != a.ToDouble() * b.ToDouble())
                    {
                        UnpackedDouble expected = a.ToDouble() * b.ToDouble();
                        var packedExpected = expected.ToPackedUlong();
                        var packedProduct = debugString(packedExpected);

                        var packedA = a.ToPackedUlong();
                        var aUnpacked = BitConverter.UInt64BitsToDouble(packedA);
                        debugA = debugString(packedA);

                        var packed = b.ToPackedUlong();
                        debugB = debugString(packed);


                        UnpackedDouble actualPacked = a.ToDouble() * b.ToDouble();
                        string bp = "";
                    }

                    return packedResult;



                }
                else
                {
                    if (decimalBitsA == 0)
                    {
                        var noBitsResult = new UnpackedDouble(b.AsPackedUlong, true);
                        noBitsResult.Exponent = a.RawExponent + b.RawExponent + 1023;
                        return noBitsResult;
                    }
                    if (decimalBitsB == 0)
                    {
                        var noBitsResult = new UnpackedDouble(a.AsPackedUlong, true);
                        noBitsResult.Exponent = a.RawExponent + b.RawExponent + 1023;
                        return noBitsResult;

                    }
                    string debugA, debugB, debugProduct;


                    GmpInt decimalA = decimalBitsA;
                    GmpInt decimalB = decimalBitsB;



                    debugA = debugString(decimalA);
                    debugB = debugString(decimalB);

                    int absShift = 0;

                    if (a.RawExponent < 0)
                    {
                        var abs = MathLib.Abs(a.RawExponent);
                        absShift += abs;
                        decimalA <<= abs;
                    }
                    else
                    {
                        decimalA = decimalA | 1ul << 52;
                        absShift += a.RawExponent;
                    }

                    if (b.RawExponent < 0)
                    {
                        var abs = MathLib.Abs(b.RawExponent);
                        absShift += abs;
                        decimalB <<= abs;
                    }
                    else
                    {
                        decimalB = decimalB | 1ul << 52;
                        absShift += b.RawExponent;
                    }

                    var test = decimalA * decimalB;

                    debugProduct = debugString(test);

                    test >>= (52 + absShift - 1);
                    if ((test & 1) == 1)
                        test += 1;
                    test >>= 1;

                    debugProduct = debugString(test);

                    var exponent = (test & ExponentMask);
                    if (exponent == 0)
                    {
                        var fract = (ulong)test & FractionMask;

                        var lz = MathLib.LeadingZeroCount(fract << (64 - 52));

                        fract = (fract << (1 << lz)) & FractionMask;
                        fract >>= (1 << lz);
                        var res = new UnpackedDouble(a.Sign ^ b.Sign, -lz, fract);

                        var resd = res.ToDouble();
                        var expectedToDouble = a.ToDouble() * b.ToDouble();
                        if (resd != expectedToDouble)
                        {
                            var exp = (UnpackedDouble)expectedToDouble;
                            string bp = "";
                        }
                        return res;

                    }
                    else
                    {
                        //TODO: extract the exponent from 
                    }

                    //0 and -1 are already 52 bit aligned.
                    // denormaliza a

                    if (a.RawExponent > -1)
                    {
                        decimalA = decimalA | 1ul << 52;
                        debugA = debugString(decimalA);
                        decimalA <<= a.RawExponent;
                        debugA = debugString(decimalA);
                        if (b.RawExponent < -1)
                        {
                            decimalA <<= (MathLib.Abs(b.RawExponent) - 1);
                            debugA = debugString(decimalA);

                            var result = decimalA * decimalB;
                            debugProduct = debugString(result);
                            result >>= 51;
                            debugProduct = debugString(result);
                            if (1 == (result & 1))
                            {
                                result += 1;
                                debugProduct = debugString(result);
                            }
                            result >>= 1;
                            debugProduct = debugString(result);

                            //normalize back to 52bits.
                            result >>= (MathLib.Abs(b.RawExponent) - 1);
                            debugProduct = debugString(result);
                        }
                    }


                    // denormalize b
                    if (b.RawExponent > -1)
                    {
                        decimalB = decimalB | 1ul << 52;
                        debugB = debugString(decimalB);
                        decimalB <<= b.RawExponent;

                    }


                    debugB = debugString(decimalB);




                    test = decimalA * decimalB;
                    var testBits = debugString(test);

                    var normTest = test >> 51;
                    //(test & FractionMask) >= (FractionMask>>1); is same as >> 51 &1;
                    //var roundUp = (test & FractionMask) >= (FractionMask>>1);
                    var roundup = (normTest & 1) == 1;
                    if (roundup)
                    {
                        normTest += 1;
                    }
                    normTest >>= 1;
                    var normTestBits = debugString(normTest);
                    var normTestBitsRound = debugString(normTest + 1);
                    var act = (UnpackedDouble)(a.ToDouble() * b.ToDouble());
                    //actual
                    //0 01111111110 0011001100110011001100110011001100110011001100110100
                    //normtest bits:
                    //0 00000000000 1110011001100110011001100110011001100110011001100111

                    //actual
                    //0 01111111110 0011001100110011001100110011001100110011001100110100
                    //0 00000000000 1 110011001100110011001100110011001100110011001100111                     //norm test


                    //actual: if add 1. Get exponent. Shift for exponent,
                    //0 01111111110 0011001100110011001100110011001100110011001100110100
                    //              1110011001100110011001100110011001100110011001100100000 test, no adjustm

                    //0 01111111110 0011001100110011001100110011001100110011001100110100
                    //            1   110011001100110011001100110011001100110011001100100000 test, shift 2
                    //            1   110011001100110011001100110011001100110011001101000000 test, shift 2 +1 last bit
                    //0 01111111110 00110011001100110011001100110011001100110011001101000000 test, shift 2 +1 last bit, set exp=0-1-1
                    //0 01111111110 0011001100110011001100110011001100110011001100110100 << exact answer.
                    //               11100110011001100110011001100110011001100110011001<<if while at >>=51, we get the lsb.

                    //0 01111111110 0011001100110011001100110011001100110011001100110100 << exact answer.
                    //0 00000000000 0001110011001100110011001100110011001100110011001101 +1 at 51

                    //0 01111111110 0011001100110011001100110011001100110011001100110100 << exact answer.
                    //0 00000000000  1110011001100110011001100110011001100110011001101 + 1 at 51 <<2
                    //Did we not shift inputs correctly in first place?

                    //full product
                    //11100110011001100110011001100110011001100110011001000000000000000000000000000000000000000000000000000
                    //                                   0 01111111110 0011001100110011001100110011001100110011001100110100

                    var len = testBits.Length;
                    //rounding debug:
                    var t = (test >> 52) << 52;
                    var keep = test & t;
                    var drop = test & (~keep);


                    test >>= 512;
                    testBits = debugString(test);
                    debugA = debugString(decimalA);

                    // align to decimal place to 52 bits;
                    var decimalAShifted = decimalA << a.RawExponent;
                    debugA = debugString2(decimalAShifted, a.RawExponent);


                    //denormalize b
                    if (b.RawExponent > -1)
                        decimalB = decimalB | 1ul << 52;

                    debugB = debugString(decimalB);



                    // align to decimal place to 52 bits;
                    var decimalBShifted = decimalB <<= b.RawExponent;
                    debugB = debugString2(decimalBShifted, b.RawExponent);


                    // calculate the product
                    var denormalizedProduct = decimalAShifted * decimalBShifted;
                    denormalizedProduct >>= 52;
                    debugProduct = debugString(denormalizedProduct);

                    if (a.RawExponent < 0)
                        denormalizedProduct >>= a.Exponent;
                    if (b.RawExponent < 0)
                        denormalizedProduct >>= b.Exponent;


                    // get the integer part of the product;
                    var wholeProduct = denormalizedProduct & ExponentMask;
                    debugProduct = debugString(wholeProduct);

                    // get the expontnet
                    var wholeProductNormalized = (ulong)wholeProduct >> 52;
                    //var productExponentLen = MathLib.BitLength(wholeProductNormalized) - 1;
                    // debug use log2, same operations but bitlength prefers asm lzcount first. but some double tests fail with bitlength
                    var productExponent = MathLib.ILogB(wholeProductNormalized);
                    //if (productExponent != productExponentLen)
                    //{
                    //    string bp = $"BitLen failed for {wholeProductNormalized} ";
                    //}

                    // Get the decimal part
                    var productFract = denormalizedProduct & ~wholeProduct;

                    var fullProductFact = productFract | (wholeProduct >> (productExponent));
                    debugProduct = debugString(fullProductFact);

                    fullProductFact = fullProductFact & FractionMask;
                    debugProduct = debugString(fullProductFact);

                    // Get the Sign;
                    var productSign = a.Sign ^ b.Sign;

                    var packedResult = new UnpackedDouble(productSign, productExponent, (ulong)fullProductFact);
                    debugProduct = debugString(packedResult.AsPackedUlong);
                    if (packedResult.ToDouble() != a.ToDouble() * b.ToDouble())
                    {
                        var aBin = a.ToBinaryDecimal;
                        var bBin = b.ToBinaryDecimal;
                        UnpackedDouble expected = a.ToDouble() * b.ToDouble();
                        var unpackedBin = expected.ToBinaryDecimal;


                        var packedExpected = expected.ToPackedUlong();
                        var packedProduct = debugString(packedExpected);

                        var packedA = a.ToPackedUlong();
                        var aUnpacked = BitConverter.UInt64BitsToDouble(packedA);
                        debugA = debugString(packedA);

                        var packed = b.ToPackedUlong();
                        debugB = debugString(packed);


                        UnpackedDouble actualPacked = a.ToDouble() * b.ToDouble();
                        string bp = "";
                    }

                    return packedResult;


                }
            }






            //var expA = a.RawExponent;
            //var absExpA = MathLib.Abs(expA);
            //var expB = b.RawExponent;
            //var absExpB = MathLib.Abs(expB);
            //var normalizedShift = absExpA > absExpB ? absExpA : absExpB;
            //var baseA = 1ul << absExpA;
            //var normFracA = a.Fraction >> (52 - absExpA);
            //// Truncate fraction completely
            ////var bigA = (1ul << absExpA) + (a.Fraction >> (52 - expA));
            //// Truncate fraction to bits matching exponent
            //var bigA = baseA + normFracA;

            //var bigADenorm = (baseA << 52 - absExpA) + a.Fraction;



            //var baseB = 1ul << absExpB;
            //var normFracB = b.Fraction >> (52 - absExpB);
            //// Truncate fraction completely
            ////var bigB = (1ul << absExpB) + (b.Fraction >> (52 - expB));
            //// Truncate fraction to bits matching exponent
            //var bigB = baseB + normFracB;
            //var bigBDenorm = (baseB << 52 - absExpB) + b.Fraction;

            ///*  5.0
            //    0.5
            //    --

            //denormalize:
            //    a= 5.0
            //    b= 5.0 bshift<<=1
            //mul:
            //    c=25.0
            //normalize
            //    c=2.5
            //    c>> bshift
            //*/
            //var product = bigA * bigB;
            //var result = new UnpackedDouble(product, false);
            //var productDenorm = (GmpInt)bigADenorm * bigBDenorm;
            //var rs = productDenorm >> 52;
            //var rsUnpacked = new UnpackedDouble((ulong)rs, true);

            //if (expA < 0)
            //{
            //    product >>= absExpA;
            //    result.Exponent -= absExpB;
            //    //todo normalize fraction
            //    //product >>= absExpB;
            //}
            //if (expB < 0)
            //{
            //    var carry = MathLib.Log2(result.RawExponent);
            //    result.Exponent -= absExpB * 2;
            //    //todo normalize fraction
            //    //product >>= absExpB;
            //}





            //var resultBits = ((GmpInt)result.AsPackedUlong).ToString(2);
            //var bits = result.Bits;
            //if (a.RawExponent < 0 || b.RawExponent < 0)
            //{
            //    var resultExpMsb = MathLib.Log2(result.RawExponent);
            //    var expShift = resultExpMsb - a.RawExponent - b.RawExponent;
            //    var calcExp = result.RawExponent >> resultExpMsb;

            //    //0 1000 0000 0000 x 000 0000 0000 0000 0000 -  0000 0000 0000 0000 0000 0000 0000 0000
            //    var calcFrac = (ulong)result.RawExponent & (1ul << (resultExpMsb - 1));
            //    string bp = "";
            //    //result.Exponent = calcExp + Bias;
            //    //result.Sign = a.Sign * b.Sign;

            //}
            //return result;

        }

        // efficient representation is simply n<<52.
        // recipPBits * n<<52;

        // Helper method to normalize the result
        private static void Normalize(UnpackedDouble result)
        {
            // TODO: Implement normalization logic
        }

        internal static int ExtractSignificandFromBits(ulong bits)
        {
            return (int)ExtractFraction(bits);
        }

        public static implicit operator UnpackedDouble(double x)
        {
            return new UnpackedDouble(x);
        }
        public static explicit operator UnpackedDouble(ulong x)
        {
            return new UnpackedDouble(x);
        }


        /*

            log(100)=2,log(10)=1,log(1)=0,log(.1)=-1, log(.01)=-2, log(.001)=-3

            *for binary values:
            log2(100)=2 log2(10)=1  log2(1)=0   log2(1/10)=-1 log2(1/100)=-2    log2(1/1000)=3
            *for decimal values
            log2(4)=2   log2(2)=1   log2(1)=0   log2(1.0/2)=-1 log2(1.0/4)=-2   log2(1.0/8)=3

            Exponent: -10  = -3.511119404027961E+305  = 0 000000-1010 0000000000000000000000000000000000000000000000000000
            Exponent: -9   = -7.022238808055922E+305  = 0 000000-1001 0000000000000000000000000000000000000000000000000000
            Exponent: -8   = -1.4044477616111843E+306 = 0 000000-1000 0000000000000000000000000000000000000000000000000000
            Exponent: -7   = -2.8088955232223686E+306 = 0 0000000-111 0000000000000000000000000000000000000000000000000000
            Exponent: -6   = -5.617791046444737E+306  = 0 0000000-110 0000000000000000000000000000000000000000000000000000
            Exponent: -5   = -1.1235582092889474E+307 = 0 0000000-101 0000000000000000000000000000000000000000000000000000
            Exponent: -4   = -2.247116418577895E+307  = 0 0000000-100 0000000000000000000000000000000000000000000000000000
            Exponent: -3   = -4.49423283715579E+307   = 0 00000000-11 0000000000000000000000000000000000000000000000000000
            Exponent: -2   = -8.98846567431158E+307   = 0 00000000-10 0000000000000000000000000000000000000000000000000000
            Exponent: -1   = -∞                       = 0 000000000-1 0000000000000000000000000000000000000000000000000000
            Exponent: 0    = 0                        = 0 00000000000 0000000000000000000000000000000000000000000000000000
            Exponent: 1    = 2.2250738585072014E-308  = 0 00000000001 0000000000000000000000000000000000000000000000000000
            Exponent: 2    = 4.450147717014403E-308   = 0 00000000010 0000000000000000000000000000000000000000000000000000
            Exponent: 3    = 8.900295434028806E-308   = 0 00000000011 0000000000000000000000000000000000000000000000000000
            Exponent: 4    = 1.7800590868057611E-307  = 0 00000000100 0000000000000000000000000000000000000000000000000000
            Exponent: 5    = 3.5601181736115222E-307  = 0 00000000101 0000000000000000000000000000000000000000000000000000
            Exponent: 6    = 7.120236347223045E-307   = 0 00000000110 0000000000000000000000000000000000000000000000000000
            Exponent: 7    = 1.424047269444609E-306   = 0 00000000111 0000000000000000000000000000000000000000000000000000
            Exponent: 8    = 2.848094538889218E-306   = 0 00000001000 0000000000000000000000000000000000000000000000000000
            Exponent: 9    = 5.696189077778436E-306   = 0 00000001001 0000000000000000000000000000000000000000000000000000
            Exponent: 10   = 1.1392378155556871E-305  = 0 00000001010 0000000000000000000000000000000000000000000000000000
            Exponent: 1013 = 0.0009765625             = 0 01111110101 0000000000000000000000000000000000000000000000000000
            Exponent: 1014 = 0.001953125              = 0 01111110110 0000000000000000000000000000000000000000000000000000
            Exponent: 1015 = 0.00390625               = 0 01111110111 0000000000000000000000000000000000000000000000000000
            Exponent: 1016 = 0.0078125                = 0 01111111000 0000000000000000000000000000000000000000000000000000
            Exponent: 1017 = 0.015625                 = 0 01111111001 0000000000000000000000000000000000000000000000000000
            Exponent: 1018 = 0.03125                  = 0 01111111010 0000000000000000000000000000000000000000000000000000
            Exponent: 1019 = 0.0625                   = 0 01111111011 0000000000000000000000000000000000000000000000000000
            Exponent: 1020 = 0.125                    = 0 01111111100 0000000000000000000000000000000000000000000000000000
            Exponent: 1021 = 0.25                     = 0 01111111101 0000000000000000000000000000000000000000000000000000
            Exponent: 1022 = 0.5                      = 0 01111111110 0000000000000000000000000000000000000000000000000000
            Exponent: 1023 = 1                        = 0 01111111111 0000000000000000000000000000000000000000000000000000
            Exponent: 1024 = 2                        = 0 10000000000 0000000000000000000000000000000000000000000000000000
            Exponent: 1025 = 4                        = 0 10000000001 0000000000000000000000000000000000000000000000000000
            Exponent: 1026 = 8                        = 0 10000000010 0000000000000000000000000000000000000000000000000000
            Exponent: 1027 = 16                       = 0 10000000011 0000000000000000000000000000000000000000000000000000
            Exponent: 1028 = 32                       = 0 10000000100 0000000000000000000000000000000000000000000000000000
            Exponent: 1029 = 64                       = 0 10000000101 0000000000000000000000000000000000000000000000000000
            Exponent: 1030 = 128                      = 0 10000000110 0000000000000000000000000000000000000000000000000000
            Exponent: 1031 = 256                      = 0 10000000111 0000000000000000000000000000000000000000000000000000
            Exponent: 1032 = 512                      = 0 10000001000 0000000000000000000000000000000000000000000000000000
            Exponent: 1033 = 1024                     = 0 10000001001 0000000000000000000000000000000000000000000000000000
         */
    }
}
