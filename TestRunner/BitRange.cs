using System.Numerics;


namespace TestRunner
{
    public class BitRange
    {
        public readonly BigInteger StartValue;
        public readonly BigInteger EndValue;
        public readonly int StartBit;
        public readonly int EndBit;
        public BitRange(int bits)
            : this(bits, bits)
        {

        }
        public BitRange(int startBit, int endBit)
        {
            StartBit = startBit;
            EndBit = endBit;
            var one = BigInteger.One;
            StartValue = one << (startBit - 1);
            EndValue = (one << endBit) - 1;
        }

        internal void ValidateBounds(BigInteger rangeMinValue, BigInteger rangeMaxValue)
        {
            // validate range start and end don't overflow as uint.max.Value
            if (StartValue < rangeMinValue || EndValue > rangeMaxValue || StartValue >= EndValue)
            {
                throw new ArgumentOutOfRangeException($"Range {StartValue} - {EndValue} is out of bounds of {rangeMinValue} - {rangeMaxValue}");
            }
        }
    }
}
