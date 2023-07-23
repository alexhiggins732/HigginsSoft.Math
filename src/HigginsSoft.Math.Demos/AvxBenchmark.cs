#define INLINEPRIMECHECK
#undef INLINEPRIMECHECK
namespace HigginsSoft.Math.Demos
{
    using HigginsSoft.Math.Lib;
    using BenchmarkDotNet.Attributes;


    public class AvxSquareGenBenchmark
    {
        [Benchmark]
        public int AvxSquareGen()
        {
            var count = 0;
            var gen = new MathLib.Vect.SquareGeneratorAvx();
            while (gen.MoveNext())
            {
                count += MathLib.Vect.SquareGeneratorAvx.Count;
            }
            return count;

        }

        [Benchmark]
        public int MulSquareGen()
        {
            var count = 0;
            for (var i = 0; i <= 46336; i++)
            {
                var sq = i * i;
                if (sq > 0)
                    count++;
            }
            return count;
        }

        [Benchmark]
        public int AddSquareGen()
        {
            var current = 1;
            var delta = 1;
            int count = 0;
            for (var i = 0; i <= 46336; i++)
            {
                count++;
                current += delta;
                delta += 2;
            }
            return count;
        }

    }



    public class AvxBenchmark
    {
        private const int Length = 32 * 1024;
        private int[] data = new int[] { };

        [Params(8, 32)]
        public int Alignment { get; set; }

        [GlobalSetup]
        public unsafe void GlobalSetup()
        {
            for (; ; )
            {
                data = Enumerable.Range(0, Length).ToArray();

                fixed (int* ptr = data)
                {
                    if ((Alignment == 32 && (uint)ptr % 32 == 0) || (Alignment == 8 && (uint)ptr % 16 != 0))
                    {
                        break;
                    }
                }
            }
        }

        [Benchmark(Baseline = true)]
        public int Sum() => LL.Sum(data);

        [Benchmark]
        public int SumAligned() => LL.SumAligned(data);

        [Benchmark]
        public int SumPipelined() => LL.SumPipelined(data);

        [Benchmark]
        public int SumAlignedPipelined() => LL.SumAlignedPipelined(data);
    }

}
