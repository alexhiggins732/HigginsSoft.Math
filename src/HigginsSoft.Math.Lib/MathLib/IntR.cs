using MathGmp.Native;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MathGmp.Native.gmp_lib;
namespace HigginsSoft.Math.Lib
{
    public class Limb32 : Limb
    {


        public Limb32(int value, int radix)
        {
            m_value = value;
            Radix = radix;
        }

        public new int Value
        {
            get => (int)m_value;
            set => m_value = value;
        }
    }
    public class Limb64 : Limb
    {
        public Limb64(long value, int radix)
        {
            m_value = value;
            Radix = radix;
        }
        public new long Value
        {
            get => (long)m_value;
            set => m_value = value;
        }
    }
    public class LimbMp : Limb
    {
        public LimbMp(GmpInt value, int radix)
        {
            m_value = value;
            Radix = radix;
        }

        public new GmpInt Value
        {
            get => (GmpInt)m_value;
            set => m_value = value;
        }
    }
    public class LimbBig : Limb
    {
        public LimbBig(BigInteger value, int radix)
        {
            m_value = value;
            Radix = radix;
        }

        public new BigInteger Value
        {
            get => (BigInteger)m_value;
            set => m_value = value;
        }
    }
    public interface ILimb
    {
        public object m_value { get; set; }
    }
    public abstract class Limb : ILimb
    {
        public int Radix;
        public object m_value { get; set; } = null!;
        public dynamic Value { get => m_value; set => m_value = value; }
        public override string ToString()
        {
            return m_value?.ToString() ?? "0";
        }
    }
    /// <summary>
    /// Integer class capable of representing a radix of any arbitrary size.
    /// </summary>
    public class IntR
    {
        public Limb[] Words;
        public int Radix;
        public int BitLength;
        public int Sign;
        public IntR(GmpInt value, int radix)
        {
            if (radix < 2)
                throw new ArgumentOutOfRangeException("Base must be 2 or greater", nameof(radix));
            this.Radix = radix;
            this.BitLength = value.BitLength;
            this.Sign = value.Sign;



            var bitsPerWord = radix - 1;
            GmpInt power = 1;


            int wordLen = 0;
            while (true)
            {
                wordLen++;
                power *= radix;
                if (power > value)
                    break;
            }

            if (radix <= 32)
            {
                Words = new Limb32[wordLen];
                for (var i = 0; i < Words.Length; i++)
                {
                    Words[i] = new Limb32((int)(value % radix), radix);
                    value = value / radix;
                }
            }

            else if (radix <= 64)
            {
                Words = new Limb64[wordLen];
                for (var i = 0; i < Words.Length; i++)
                {
                    Words[i] = new Limb64((long)(value % radix), radix);
                    value = value / radix;
                }
            }
            else
            {
                Words = new LimbMp[wordLen];
                for (var i = 0; i < Words.Length; i++)
                {
                    Words[i] = new LimbMp((value % radix), radix);
                    value = value / radix;
                }
            }




            //var wordLen = (int)MathLib.Ceiling((double)BitLength / bitsPerWord);
            //if (radix <= 32)
            //{
            //    Words = new Limb32[wordLen];
            //    var bitSize = radix - 1;
            //    for (int i = 0; i < wordLen; i++)
            //    {
            //        int startIndex = i * bitSize; // Calculate the starting index for the current word
            //        int endIndex = startIndex + bitSize; // Calculate the ending index for the current word
            //        int wordValue = ExtractWordValue(value, startIndex, endIndex, radix); // Extract the value for the current word
            //        Words[i] = new Limb32(wordValue, radix); // Create a new Limb32 instance with the calculated value and radix
            //    }
            //}
            //else if (radix <= 64)
            //{
            //    Words = new Limb64[wordLen];
            //}
            //else
            //{
            //    Words = new LimbMp[wordLen];
            //}

        }


        public GmpInt DecimalValue
        {
            get
            {
                GmpInt result = 0;
                GmpInt power = 1;
                for (var i = 0; i < Words.Length; i++)
                {
                    result += GmpInt.Convert(Words[i].m_value) * power;
                    if (i < Words.Length - 1)
                        power *= Radix;
                }
                return result;
            }
        }
        private int ExtractWordValue(GmpInt value, int startIndex, int endIndex, int radix)
        {
            int wordValue = 0;
            int wordLength = endIndex - startIndex;

            // Get the bits representing the value


            // Extract the bits for the current word and convert to an integer
            for (int i = 0; i < wordLength; i++)
            {
                int bitIndex = startIndex + i;
                int bitValue = value.GetBit(bitIndex);

                // Convert the bit value to an integer using bitwise operations
                wordValue |= bitValue << i;
            }

            // Adjust the value based on the radix
            //wordValue *= radix;
            return wordValue;
        }









        public override string ToString()
        {
            // return string.Join(" ", Words.Reverse().Select(x => x.ToString()));

            // [[15] [14] [13] [12]]  [[11] [10] [9] [8]] - [[7] [6] [5] [4]]  [[3] [2] [1] [0]]



            var chunks = Words.Chunk(32).Select(x => x.ToArray()).ToList();
            //format chunks aligned to 32

            var sb = new StringBuilder();


            for (var j = chunks.Count - 1; j > -1; j--)
            {
                Limb[] chunk = chunks[j];

                if (j == chunks.Count - 1)
                    sb.Append("[");
                else
                    sb.Append(" -- [");
                for (var i = chunk.Length - 1; i > -1; i--)
                {
                    sb.Append(chunk[i].ToString());
                    if (i % 4 == 0)
                    {
                        if (i == 0)
                        {
                            sb.Append("]");
                        }
                        else
                        {
                            if (i % 8 == 0)
                            {
                                sb.Append("] - [");
                            }
                            else
                            {
                                sb.Append("] [");
                            }
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }

            }

            var result = sb.ToString();
            return result;


        }



        //private unsafe static Boolean HexNumberToBigInteger(ref BigNumberBuffer number, ref BigInteger value)
        //{
        //    if (number.digits == null || number.digits.Length == 0)
        //        return false;

        //    int len = number.digits.Length - 1; // ignore trailing '\0'
        //    byte[] bits = new byte[(len / 2) + (len % 2)];

        //    bool shift = false;
        //    bool isNegative = false;
        //    int bitIndex = 0;

        //    // parse the string into a little-endian two's complement byte array
        //    // string value     : O F E B 7 \0
        //    // string index (i) : 0 1 2 3 4 5 <--
        //    // byte[] (bitIndex): 2 1 1 0 0 <--
        //    //
        //    for (int i = len - 1; i > -1; i--)
        //    {
        //        char c = number.digits[i];

        //        byte b;
        //        if (c >= '0' && c <= '9')
        //        {
        //            b = (byte)(c - '0');
        //        }
        //        else if (c >= 'A' && c <= 'F')
        //        {
        //            b = (byte)((c - 'A') + 10);
        //        }
        //        else
        //        {
        //            Contract.Assert(c >= 'a' && c <= 'f');
        //            b = (byte)((c - 'a') + 10);
        //        }
        //        if (i == 0 && (b & 0x08) == 0x08)
        //            isNegative = true;

        //        if (shift)
        //        {
        //            bits[bitIndex] = (byte)(bits[bitIndex] | (b << 4));
        //            bitIndex++;
        //        }
        //        else
        //        {
        //            bits[bitIndex] = isNegative ? (byte)(b | 0xF0) : (b);
        //        }
        //        shift = !shift;
        //    }

        //    value = new BigInteger(bits);
        //    return true;
    }

}
