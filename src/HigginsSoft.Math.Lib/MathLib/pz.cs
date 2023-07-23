using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HigginsSoft.Math.Lib
{
    public class PzBuffer
    {
        private GCHandle handle;
        public ulong[] data;
        public unsafe ulong* bits;
        public int Size;
        public unsafe PzBuffer(ulong[] buffer)
        {
            this.data = buffer;
            handle = MathLib.AlignPinnedArray(ref data);
            fixed (ulong* ptr = data)
            {
                this.bits = ptr;
            }
            this.Size = buffer.Length;
        }
        ~PzBuffer()
        {
            handle.Free();
        }
    }
    public class pz
    {
        // TODO: create pzbuffer class so memory can be allocated and aligned at once for mulitple pz instances to use.
        //          This will eliminate allocations and enable detection of the result of an operation resulting in an index out of range.

        // another option is to use two arrays, 1 for pz_data and one for pz_metdata
        //private int m_value = 0;
        public unsafe ulong* bits;
        public ulong[] data;
        private PzBuffer buffer;
        const int size_mask = 0x7FFF_FFFF;
        const int sign_shift = 31;
        public int Size { get; set; } // { get => m_value & size_mask; private set => m_value = value & size_mask; }
        public int Sign { get; set; } //{ get => m_value >> sign_shift; private set => m_value = (value & 1) << sign_shift; }

        public static readonly pz One = new pz(1);
        public static readonly pz Two = new pz(2);
        public static readonly pz Three = new pz(3);
        public static readonly pz Four = new pz(4);
        public static readonly pz Five = new pz(5);
        public static readonly pz Ten = new pz(2);
        public static readonly pz Minus1 = new pz(-1);
        public static readonly pz Minus2 = new pz(-2);

        public static readonly pz IntMinValue = new pz(int.MinValue);
        public static readonly pz IntMaxValue = new pz(int.MaxValue);
        public static readonly pz UintMinValue = new pz(uint.MinValue);
        public static readonly pz UintMaxValue = new pz(uint.MaxValue);

        public static readonly pz LongMinValue = new pz(long.MinValue);
        public static readonly pz LongMaxValue = new pz(long.MaxValue);

        public static readonly pz ULongMinValue = new pz(ulong.MinValue);
        public static readonly pz ULongMaxValue = new pz(ulong.MaxValue);

        /*
         
        Ptr Length Options:
            Pass the length as a separate parameter: 
                If you have the length information available at the calling site,
                explicitly pass the length of the array to the constructor. 
                This ensures that the length is known and can be stored within the pz struct.
                Can optionaly then pack the length.
            Use a sentinel value: 
                If the array p has a known sentinel value at the end
                (e.g., a value that indicates the end of the array), 
                you can iterate through the array inside the constructor encountering
                    the sentinel value and determine the length accordingly.
            Pack the sign:
                Pack a size prefix: The first element denotes the length of the array,
                    Access the first element inside the constructor to obtain the length.
                Pack a size suffix: The last element denotes the length of the array.
                    With a ptr, we don' know the size to access the last element.
                    We also may have to resize the ptr to add a word to store the size
        
        Sign options:
            Pass the sign as a seperate parameter
            Pack the sign in the msb. 
        Choice:
            Since the plan is to use the class in CUDA, AVX to avoid memory allocations
                it will be on the caller to pass the length as a seperate parameter.
         */
        public unsafe pz(ulong* p, int size, int sign = 0)
        {
            this.bits = p;
            this.Size = size;
            this.Sign = sign;
        }

        public unsafe pz(int value)
            : this((ulong)value, value < 0 ? 1 : 0)
        {

        }

        public unsafe pz(long value)
            : this((ulong)value, value < 0 ? 1 : 0)
        {

        }

        public unsafe pz(uint value)
            : this((ulong)value, 0)
        {

        }

        public unsafe pz(ulong value, int sign = 0)
        {
            Sign = sign;
            Size = 1;
            this.buffer = new PzBuffer(new ulong[] { value });
            this.bits = buffer.bits;
            this.data = buffer.data;
        }


        public unsafe pz(ulong[] p, int sign = 0)
        {
            this.buffer = new PzBuffer(p);
            this.bits = buffer.bits;
            this.data = buffer.data;
            this.Size = p.Length;
            this.Sign = sign;
        }


        //follow signature of buffer.blockcopy, add elements of this to other
        public unsafe void AddU(pz other, int srcOffset, pz dest, int destOffset, int size)
        {
            //todo: implement signed addition

            ulong carry = 0;
            for (int i = 0; i < size; i++)
            {
                ulong* srcPtr = bits + srcOffset + i;
                ulong* otherPtr = other.bits + srcOffset + i;
                ulong* destPtr = dest.bits + destOffset + i;

                ulong sum = *srcPtr + *otherPtr + carry;
                carry = (sum < *srcPtr) ? 1UL : 0UL;
                *destPtr = sum;
            }

            if (carry != 0)
            {
                ulong* destPtr = dest.bits + destOffset + size;
                *destPtr += carry;
            }
        }

        public bool IsNegative() => Sign == 1;


        public unsafe void AddOneUnsigned(bool overflowCheck = false)
        {
            // Add 1 to the negated value
            ulong carry = 1;
            for (int i = 0; carry > 0 && i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                if (*elementPtr < ulong.MaxValue)
                {
                    *elementPtr += carry;
                    carry = 0;
                }
                else
                {
                    *elementPtr = 0;
                }
            }
            if (overflowCheck && carry > 0)
                throw new OverflowException();
        }

        public unsafe void SubtractOneUnsigned(bool underflowCheck = false)
        {
            ulong borrow = 1;
            for (int i = 0; borrow > 0 && i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                if (*elementPtr > 0)
                {
                    *elementPtr -= borrow;
                    borrow = 0;
                }
                else
                {
                    *elementPtr = ulong.MaxValue;
                }
            }

            if (underflowCheck && borrow > 0)
                throw new OverflowException();
        }

        public unsafe void AddOneSigned(bool overflowCheck = false)
        {
            // Add 1 to the negated value
            ulong carry = 1;
            for (int i = 0; carry > 0 && i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                if (*elementPtr < ulong.MaxValue)
                {
                    *elementPtr += carry;
                    carry = 0;
                }
                else
                {
                    *elementPtr = 0;
                }
            }
            if (carry > 0)
            {
                if (IsNegative())
                    SetSign(false);
                else if (overflowCheck)
                    throw new OverflowException();
                else if (!IsZero())
                {
                    SetSign(true);
                }
            }
        }

        public unsafe void SubtractOneSigned(bool underflowCheck = false)
        {
            ulong borrow = 1;
            for (int i = 0; borrow > 0 && i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                if (*elementPtr > 0)
                {
                    *elementPtr -= borrow;
                    borrow = 0;
                }
                else
                {
                    *elementPtr = ulong.MaxValue;
                }
            }

            if (borrow > 0)
            {
                if (!IsNegative())
                    SetSign(true);
                else if (!underflowCheck)
                    SetSign(false);
                else
                    throw new OverflowException();

            }
        }




        public unsafe bool IsZero()
        {
            for (int i = 0; i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                if (*elementPtr > 0) return false;

            }
            return true;
        }
        public unsafe bool IsOne()
        {
            if (Size == 0) return false;
            ulong* elementPtr = bits;
            if (*elementPtr != 1u)
                return false;
            else
            {
                for (int i = 2; i < Size; i++)
                {
                    elementPtr = bits + i;
                    if (*elementPtr > 0) return false;
                }
            }
            return true;
        }


        public unsafe void AddUnsigned(ulong value, bool overflowCheck = false)
        {
            ulong carry = value;
            for (int i = 0; carry > 0 && i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                ulong sum = *elementPtr + carry;

                // Check for overflow
                if (sum < *elementPtr)
                {
                    // Overflow occurred, set carry to 1
                    carry = 1UL;
                }
                else
                {
                    // No overflow, set carry to 0
                    carry = 0UL;
                }

                *elementPtr = sum;
            }

            if (overflowCheck && carry > 0)
                throw new OverflowException();
        }

        public unsafe void AddSigned(uint value, bool overflowCheck = false)
        {
            ulong carry = value;
            for (int i = 0; carry > 0 && i < Size; i++)
            {
                ulong* elementPtr = bits + i;
                ulong sum = *elementPtr + carry;

                // Check for overflow
                if (sum < *elementPtr)
                {
                    // Overflow occurred, set carry to 1
                    carry = 1UL;
                }
                else
                {
                    // No overflow, set carry to 0
                    carry = 0UL;
                }

                *elementPtr = sum;
            }

            if (carry > 0)
            {
                if (IsNegative())
                    SetSign(false);
                else if (!overflowCheck)
                    SetSign(true);
                else
                    throw new OverflowException();
            }
        }

        public unsafe void Add(pz other, int srcOffset, pz dest, int destOffset, int size)
        {
            bool resultIsNegative = false;

            // Check the signs of the operands
            bool thisIsNegative = IsNegative();
            bool otherIsNegative = other.IsNegative();

            if (thisIsNegative == otherIsNegative)
            {
                // Signs are the same, perform unsigned addition
                ulong carry = 0;
                for (int i = 0; i < size; i++)
                {
                    ulong* srcPtr = bits + srcOffset + i;
                    ulong* otherPtr = other.bits + srcOffset + i;
                    ulong* destPtr = dest.bits + destOffset + i;

                    ulong sum = *srcPtr + *otherPtr + carry;
                    carry = (sum < *srcPtr) ? 1UL : 0UL;
                    *destPtr = sum;
                }

                if (carry != 0)
                {
                    ulong* destPtr = dest.bits + destOffset + size;
                    *destPtr += carry;
                }

                // Result sign is the same as the operands
                resultIsNegative = thisIsNegative;
            }
            else
            {
                // Signs are different, perform unsigned subtraction
                // Get the absolute values of the operands

                if (thisIsNegative)
                    Abs();
                if (otherIsNegative)
                    other.Abs();


                // Subtract the absolute values
                Subtract(other, srcOffset, dest, destOffset, size);

                if (thisIsNegative)
                    Negate();
                if (otherIsNegative)
                    other.Negate();


                // Check the sign of the result
                int compareResult = CompareTo(other);
                if (compareResult >= 0)
                {
                    // Result sign is the same as `this`
                    resultIsNegative = thisIsNegative;
                }
                else
                {
                    // Result sign is the opposite of `this`
                    resultIsNegative = !thisIsNegative;
                }
            }

            // Normalize the result and adjust the sign
            dest.Normalize();
            dest.SetSign(resultIsNegative);
        }


        public void SetSign(bool isNegative) => Sign = (isNegative ? 1 : 0);

        public unsafe void Subtract(pz other, int srcOffset, pz dest, int destOffset, int size)
        {
            // Subtract the absolute values
            for (int i = 0; i < size; i++)
            {
                ulong* srcPtr = bits + srcOffset + i;
                ulong* otherPtr = other.bits + srcOffset + i;
                ulong* destPtr = dest.bits + destOffset + i;

                *destPtr = *srcPtr - *otherPtr;
            }

            // Normalize the result
            dest.Normalize();
        }


        public unsafe int CompareTo(pz other)
        {
            int minSize = MathLib.Min(Size, other.Size);

            for (int i = minSize - 1; i >= 0; i--)
            {
                var thisWord = bits[i];
                var otherWord = other.bits[i];
                if (thisWord < otherWord)
                    return -1;
                if (bits[i] < other.bits[i])
                    return -1;
                if (bits[i] > other.bits[i])
                    return 1;
            }
            if (minSize == Size)
            {
                if (minSize == other.Size)
                    return 0;
                for (var i = minSize; i < other.Size; i++)
                    if (other.bits[i] > 0) return 1;
                return 0;
            }
            else
            {
                for (var i = minSize+1; i < Size; i++)
                    if (bits[i] > 0) return 1;
                return 0;
            }

        }



        public static unsafe void DangerousMakeTwosComplement(pz p)
        {
            // ported from BigInteger src, but unsure the logic in tracking a carry and resizing.
            /*
            int i = 0;
            ulong v = 0;
            for (; i < p.Size; i++)
            {
                v = ~(*(p.bits + i)) + 1;
                *(p.bits + i) = v;
                if (v != 0)
                {
                    i++;
                    break;
                }
            }

            if (v != 0)
            {
                for (; i < p.Size; i++)
                {
                    *(p.bits + i) = ~(*(p.bits + i));
                }
            }
            else
            {
                // Resize the array to accommodate the additional element
                ResizePz(ref p, p.Size + 1);
                *(p.bits + p.Size - 1) = 1;
            }*/

            // since pz uses pointers to fixed size arrays for mp arithemitic we will not allow resizing
            for (int i = 0; i < p.Size; i++)
            {
                ulong* elementPtr = p.bits + i;
                *elementPtr = ~(*elementPtr);
            }
        }


        public static unsafe pz Abs(pz src)
        {
            pz result = new pz(src.data, src.Sign);

            // Check if the value is negative (the most significant bit is set)
            bool isNegative = result.IsNegative();

            // If the value is negative, apply two's complement
            if (isNegative)
            {
                // Apply two's complement to each element
                for (int i = 0; i < src.Size; i++)
                {
                    ulong* elementPtr = result.bits + i;
                    *elementPtr = ~(*elementPtr);
                }
                result.SetSign(false);
                // Add 1 to complete the two's complement
                result.Add(new pz(1), 0, result, 0, 1);
            }

            return result;
        }

        public unsafe void Abs()
        {

            // Check if the value is negative (the most significant bit is set)
            bool isNegative = IsNegative();

            // If the value is negative, apply two's complement
            if (isNegative)
            {
                // Apply two's complement to each element
                for (int i = 0; i < Size; i++)
                {
                    ulong* elementPtr = bits + i;
                    *elementPtr = ~(*elementPtr);
                }
                SetSign(false);
                // Add 1 to complete the two's complement
                AddOneUnsigned();
            }
        }

        public unsafe void Negate()
        {

            // Check if the value is negative (the most significant bit is set)
            bool isNegative = IsNegative();

            // If the value is negative, apply two's complement
            if (isNegative)
            {
                Abs();
            }
            else
            {
                for (int i = 0; i < Size; i++)
                {
                    ulong* elementPtr = bits + i;
                    *elementPtr = ~(*elementPtr);
                }

                AddOneUnsigned();
                SetSign(true);
            }
        }


        /// <summary>
        /// pz uses pointers to fixed size arrays for mp arithemitic we will not allow actual resizing.
        /// TODO: Decide if we will allow the caller (public or internal) to "resize" specifying a new length and/or new soruce ptr.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="newSize"></param>
        /// <exception cref="NotImplementedException"></exception>

        private static unsafe void ResizePz(ref pz p, int newSize)
        {
            p.Size = newSize;
            //throw new NotImplementedException();
            /*
            ulong* newP_a = stackalloc ulong[newSize];
            Buffer.MemoryCopy(p.bits, newP_a, newSize * sizeof(ulong), p.Size * sizeof(ulong));
            p.bits = newP_a;
            p.Size = newSize;*/
        }

        public unsafe ulong Normalize()
        {
            int len = Size;
            ulong result = 0;
            if (len > 0)
            {

                result = *bits;
                *bits = (uint)result;
                result >>= 32;

                if (len > 1)
                {
                    // can only normalize up to len>>1, if any bits higher than len/2 are set it is an overflow.
                    for (var i = 1; i < len; i++)
                    {
                        ulong* word = bits + i;
                        ulong low = *word & uint.MaxValue;
                        result += low;
                        result += *word >> 32;
                        *word = (uint)result;
                        var a = this[0];
                        var b = this[1];
                        result >>= 32;
                    }
                }
            }
            return result;
        }

        public unsafe ulong this[int index]
        {
            get
            {
                // Perform bounds checking and return the element at the specified index
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }

                // Calculate the pointer to the desired element
                ulong* elementPtr = bits + index;

                // Return the value at the specified index
                return *elementPtr;
            }

            set
            {
                // Perform bounds checking and set the element at the specified index
                if (index < 0 || index >= Size)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }

                // Calculate the pointer to the desired element
                ulong* elementPtr = bits + index;

                // Set the value at the specified index
                *elementPtr = value;
            }
        }


        public unsafe override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = Size - 1; i >= 0; i--)
            {
                var hex = bits[i].ToString("X8").PadLeft(16, '0');
                sb.Append(hex.Substring(0,8));
                sb.Append(' ');
                sb.Append(hex.Substring(8));
                if (i != 0)
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }



        public static unsafe explicit operator int(pz p)
        {
            if (p.Size == 0)
            {
                return 0;
            }
            else //if (p.Size==1)
            {
                return (int)*p.bits;
            }
            //// Assuming the most significant bits are stored at the higher index positions
            //int result = 0;
            //int shift = 0;

            //for (int i = 0; i < p.Size; i++)
            //{
            //    ulong value = *(p.p_a + i);
            //    result |= (int)(value << shift);
            //    shift += 64; // Assuming ulong is 64 bits
            //}

            //return result;
        }


        public static unsafe explicit operator uint(pz p)
        {
            if (p.Size == 0)
            {
                return 0;
            }
            else //if (p.Size==1)
            {
                return (uint)*p.bits;
            }
        }


        public static unsafe explicit operator long(pz p)
        {
            if (p.Size == 0)
            {
                return 0;
            }
            else //if (p.Size==1)
            {
                return (long)*p.bits;
            }
        }



        public static unsafe explicit operator ulong(pz p)
        {
            if (p.Size == 0)
            {
                return 0;
            }
            else
            {
                return (ulong)*p.bits;
            }
        }

    }



}
