using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;
using ManagedCuda.NVRTC;
using System.Diagnostics;

namespace vectorAdd_nvrtc
{

    class Program
    {

        public static void Fill<T>(T[] array, T value)
        {

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }

        }

        static void TimeMulxX(int x, string[] args)
        {
            Console.WriteLine($"==== Begin test x{x} ====");

            const int nElements = 1_000_000;
            int numElements = nElements * x;


            ulong[] h_A = new ulong[numElements];
            ulong[] h_B = new ulong[numElements];
            ulong[] h_C = new ulong[numElements];
            Fill(h_A, 1ul);
            Fill(h_B, 1ul);
            string filename = $"vectorMul_kernel64x{x}.cu"; //we assume the file is in the same folder...
            string fileToCompile = File.ReadAllText(filename);


            var sw = Stopwatch.StartNew();
            var opWatch = Stopwatch.StartNew();
            CudaRuntimeCompiler rtc = new CudaRuntimeCompiler(fileToCompile, "vectorMul_kernel");

            rtc.Compile(args);
            opWatch.Stop();
            sw.Stop();


            Console.WriteLine($"Compiling x{nElements} took {opWatch.Elapsed}");
            string log = rtc.GetLogAsString();

            Console.WriteLine(log);
            sw.Start();
            opWatch.Restart();
            byte[] ptx = rtc.GetPTX();
            opWatch.Stop();
            sw.Stop();

            Console.WriteLine($"GetPtx x{nElements} took {opWatch.Elapsed}");
            sw.Start();
            rtc.Dispose();
            sw.Stop();

            sw.Start();
            CudaContext ctx = new CudaContext(0);

            opWatch.Restart();
            CudaKernel vectorMul = ctx.LoadKernelPTX(ptx, "vectorMul");
            opWatch.Stop();


            // Launch the Vector Mul CUDA Kernel
            int threadsPerBlock = 1024;// 256;
            int blocksPerGrid = (numElements + threadsPerBlock - 1) / threadsPerBlock;
            Console.WriteLine("CUDA kernel launch with {0} blocks of {1} threads\n", blocksPerGrid, threadsPerBlock);

            sw.Start();
            vectorMul.BlockDimensions = new dim3(threadsPerBlock, 1, 1);
            vectorMul.GridDimensions = new dim3(blocksPerGrid, 1, 1);

            opWatch.Restart();
            // Allocate the device input vector A and copy to device
            CudaDeviceVariable<ulong> d_A = h_A;

            // Allocate the device input vector B and copy to device
            CudaDeviceVariable<ulong> d_B = h_B;

            // Allocate the device output vector C
            CudaDeviceVariable<ulong> d_C = new CudaDeviceVariable<ulong>(numElements);
            opWatch.Stop();
            sw.Stop();

            string numElementsStr = numElements.ToString("N0");
            Console.WriteLine($"Copy and allocate {numElementsStr} took {opWatch.Elapsed}");

            sw.Start();
            opWatch.Restart();
            //for(var j=0; j<1_000_000;j++)
            vectorMul.Run(d_A.DevicePointer, d_B.DevicePointer, d_C.DevicePointer, numElements);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"vectorMul.Run for {numElementsStr} took {opWatch.Elapsed}");

            // Copy the device result vector in device memory to the host result vector
            // in host memory.
            Console.WriteLine("Copy output data from the CUDA device to the host memory\n");

            sw.Start();
            opWatch.Restart();
            d_C.CopyToHost(h_C);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"CopyToHost for {numElementsStr} took {opWatch.Elapsed}");


            Console.WriteLine($"Total time for {numElementsStr} took {sw.Elapsed}");

            Console.WriteLine($"==== End test x{x} ====");
            Console.WriteLine();

        }

    
        static void TimeAddxX(int x, string[] args)
        {
            Console.WriteLine($"==== Begin test x{x} ====");

            const int nElements = 1_000_000;
            int numElements = nElements * x;

        
            ulong[] h_A = new ulong[numElements];
            ulong[] h_B = new ulong[numElements];
            ulong[] h_C = new ulong[numElements];
            Fill(h_A, 1ul);
            Fill(h_B, 1ul);
            string filename = $"vectorAdd_kernel64x{x}.cu"; //we assume the file is in the same folder...
            string fileToCompile = File.ReadAllText(filename);


            var sw = Stopwatch.StartNew();
            var opWatch = Stopwatch.StartNew();
            CudaRuntimeCompiler rtc = new CudaRuntimeCompiler(fileToCompile, "vectorAdd_kernel");

            rtc.Compile(args);
            opWatch.Stop();
            sw.Stop();


            Console.WriteLine($"Compiling x{nElements} took {opWatch.Elapsed}");
            string log = rtc.GetLogAsString();

            Console.WriteLine(log);
            sw.Start();
            opWatch.Restart();
            byte[] ptx = rtc.GetPTX();
            opWatch.Stop();
            sw.Stop();

            Console.WriteLine($"GetPtx x{nElements} took {opWatch.Elapsed}");
            sw.Start();
            rtc.Dispose();
            sw.Stop();

            sw.Start();
            CudaContext ctx = new CudaContext(0);

            opWatch.Restart();
            CudaKernel vectorAdd = ctx.LoadKernelPTX(ptx, "vectorAdd");
            opWatch.Stop();


            // Launch the Vector Add CUDA Kernel
            int threadsPerBlock = 1024;// 256;
            int blocksPerGrid = (numElements + threadsPerBlock - 1) / threadsPerBlock;
            Console.WriteLine("CUDA kernel launch with {0} blocks of {1} threads\n", blocksPerGrid, threadsPerBlock);

            sw.Start();
            vectorAdd.BlockDimensions = new dim3(threadsPerBlock, 1, 1);
            vectorAdd.GridDimensions = new dim3(blocksPerGrid, 1, 1);

            opWatch.Restart();
            // Allocate the device input vector A and copy to device
            CudaDeviceVariable<ulong> d_A = h_A;

            // Allocate the device input vector B and copy to device
            CudaDeviceVariable<ulong> d_B = h_B;

            // Allocate the device output vector C
            CudaDeviceVariable<ulong> d_C = new CudaDeviceVariable<ulong>(numElements);
            opWatch.Stop();
            sw.Stop();

            string numElementsStr = numElements.ToString("N0");
            Console.WriteLine($"Copy and allocate {numElementsStr} took {opWatch.Elapsed}");

            sw.Start();
            opWatch.Restart();
            //for(var j=0; j<1_000_000;j++)
            vectorAdd.Run(d_A.DevicePointer, d_B.DevicePointer, d_C.DevicePointer, numElements);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"vectorAdd.Run for {numElementsStr} took {opWatch.Elapsed}");

            // Copy the device result vector in device memory to the host result vector
            // in host memory.
            Console.WriteLine("Copy output data from the CUDA device to the host memory\n");

            sw.Start();
            opWatch.Restart();
            d_C.CopyToHost(h_C);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"CopyToHost for {numElementsStr} took {opWatch.Elapsed}");


            Console.WriteLine($"Total time for {numElementsStr} took {sw.Elapsed}");

            Console.WriteLine($"==== End test x{x} ====");
            Console.WriteLine();

        }

        static void TimeAddx8(string[] args)
        {
            TimeAddxX(8, args);
        }

        static void TimeAddx4(string[] args)
        {
            TimeAddxX(4, args);
        }

        static void TimeAddx2(string[] args)
        {
            TimeAddxX(2, args);
        }

        static void TimeAddx1(string[] args)
        {
            TimeAddxX(1, args);
        }

        static void TimeMulx8(string[] args)
        {
            TimeMulxX(8, args);
        }

        static void TimeMulx4(string[] args)
        {
            TimeMulxX(4, args);
        }

        static void TimeMulx2(string[] args)
        {
            TimeMulxX(2, args);
        }

        static void TimeMulx1(string[] args)
        {
            TimeMulxX(1, args);
        }

        static void TimeAddOrig(string[] args)
        {
            const int numElements = 1_000_000;
            ulong[] h_A = new ulong[numElements];
            ulong[] h_B = new ulong[numElements];
            ulong[] h_C = new ulong[numElements];
            Fill(h_A, 1ul);
            Fill(h_B, 1ul);
            string filename = "vectorAdd_kernel64.cu"; //we assume the file is in the same folder...
            string fileToCompile = File.ReadAllText(filename);


            var sw = Stopwatch.StartNew();
            var opWatch = Stopwatch.StartNew();
            CudaRuntimeCompiler rtc = new CudaRuntimeCompiler(fileToCompile, "vectorAdd_kernel");

            rtc.Compile(args);
            opWatch.Stop();
            sw.Stop();


            Console.WriteLine($"Compiling took {opWatch.Elapsed}");
            string log = rtc.GetLogAsString();

            Console.WriteLine(log);
            sw.Start();
            opWatch.Restart();
            byte[] ptx = rtc.GetPTX();
            opWatch.Stop();
            sw.Stop();

            Console.WriteLine($"GetPtx took {opWatch.Elapsed}");
            sw.Start();
            rtc.Dispose();
            sw.Stop();

            sw.Start();
            CudaContext ctx = new CudaContext(0);

            opWatch.Restart();
            CudaKernel vectorAdd = ctx.LoadKernelPTX(ptx, "vectorAdd");
            opWatch.Stop();


            // Launch the Vector Add CUDA Kernel
            int threadsPerBlock = 256;
            int blocksPerGrid = (numElements + threadsPerBlock - 1) / threadsPerBlock;
            Console.WriteLine("CUDA kernel launch with {0} blocks of {1} threads\n", blocksPerGrid, threadsPerBlock);

            sw.Start();
            vectorAdd.BlockDimensions = new dim3(threadsPerBlock, 1, 1);
            vectorAdd.GridDimensions = new dim3(blocksPerGrid, 1, 1);

            opWatch.Restart();
            // Allocate the device input vector A and copy to device
            CudaDeviceVariable<ulong> d_A = h_A;

            // Allocate the device input vector B and copy to device
            CudaDeviceVariable<ulong> d_B = h_B;

            // Allocate the device output vector C
            CudaDeviceVariable<ulong> d_C = new CudaDeviceVariable<ulong>(numElements);
            opWatch.Stop();
            sw.Stop();

            string numElementsStr = numElements.ToString("N0");
            Console.WriteLine($"Copy and allocate {numElementsStr} took {opWatch.Elapsed}");

            sw.Start();
            opWatch.Restart();
            //for(var j=0; j<1_000_000;j++)
            vectorAdd.Run(d_A.DevicePointer, d_B.DevicePointer, d_C.DevicePointer, numElements);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"vectorAdd.Run for {numElementsStr} took {opWatch.Elapsed}");

            // Copy the device result vector in device memory to the host result vector
            // in host memory.
            Console.WriteLine("Copy output data from the CUDA device to the host memory\n");

            sw.Start();
            opWatch.Restart();
            d_C.CopyToHost(h_C);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"CopyToHost for {numElementsStr} took {opWatch.Elapsed}");


            Console.WriteLine($"Total time for {numElementsStr} took {sw.Elapsed}");
        }
        static void Main(string[] args)
        {
            Console.Clear();

            TimeMulx1(args);
            TimeMulx2(args);
            TimeMulx4(args);
            TimeMulx8(args);

            TimeAddx1(args);
            TimeAddx2(args);
            TimeAddx4(args);
            TimeAddx8(args);
            //Console.Clear();

            string filename = "vectorAdd_kernel.cu"; //we assume the file is in the same folder...
            string fileToCompile = File.ReadAllText(filename);


            var sw = Stopwatch.StartNew();
            var opWatch = Stopwatch.StartNew();
            CudaRuntimeCompiler rtc = new CudaRuntimeCompiler(fileToCompile, "vectorAdd_kernel");

            rtc.Compile(args);
            opWatch.Stop();
            sw.Stop();


            Console.WriteLine($"Compiling took {opWatch.Elapsed}");
            string log = rtc.GetLogAsString();

            Console.WriteLine(log);
            sw.Start();
            opWatch.Restart();
            byte[] ptx = rtc.GetPTX();
            opWatch.Stop();
            sw.Stop();

            Console.WriteLine($"GetPtx took {opWatch.Elapsed}");
            sw.Start();
            rtc.Dispose();
            sw.Stop();

            // Print the vector length to be used, and compute its size
            int numElements = 50000;
            SizeT size = numElements * sizeof(float);
            sw.Stop();


            Console.WriteLine("[Vector addition of {0} elements]", numElements);


            // Allocate the host input vector A
            float[] h_A = new float[numElements];
            // Allocate the host input vector B
            float[] h_B = new float[numElements];
            // Allocate the host output vector C
            float[] h_C = new float[numElements];

            Random rand = new Random(0);

            // Initialize the host input vectors
            for (int i = 0; i < numElements; ++i)
            {
                h_A[i] = (float)rand.NextDouble();
                h_B[i] = (float)rand.NextDouble();
            }

            Console.WriteLine("Allocate and copy input data from the host memory to the CUDA device\n");


            // Launch the Vector Add CUDA Kernel
            int threadsPerBlock = 256;
            int blocksPerGrid = (numElements + threadsPerBlock - 1) / threadsPerBlock;
            Console.WriteLine("CUDA kernel launch with {0} blocks of {1} threads\n", blocksPerGrid, threadsPerBlock);

            sw.Start();
            CudaContext ctx = new CudaContext(0);

            opWatch.Restart();
            CudaKernel vectorAdd = ctx.LoadKernelPTX(ptx, "vectorAdd");
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"LoadKernelPTX took {opWatch.Elapsed}");

            sw.Start();
            vectorAdd.BlockDimensions = new dim3(threadsPerBlock, 1, 1);
            vectorAdd.GridDimensions = new dim3(blocksPerGrid, 1, 1);

            opWatch.Restart();
            // Allocate the device input vector A and copy to device
            CudaDeviceVariable<float> d_A = h_A;

            // Allocate the device input vector B and copy to device
            CudaDeviceVariable<float> d_B = h_B;

            // Allocate the device output vector C
            CudaDeviceVariable<float> d_C = new CudaDeviceVariable<float>(numElements);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"Copy and allocate {numElements} took {opWatch.Elapsed}");

            sw.Start();
            opWatch.Restart();
            vectorAdd.Run(d_A.DevicePointer, d_B.DevicePointer, d_C.DevicePointer, numElements);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"vectorAdd.Run for {numElements} took {opWatch.Elapsed}");

            // Copy the device result vector in device memory to the host result vector
            // in host memory.
            Console.WriteLine("Copy output data from the CUDA device to the host memory\n");

            sw.Start();
            opWatch.Restart();
            d_C.CopyToHost(h_C);
            opWatch.Stop();
            sw.Stop();
            Console.WriteLine($"CopyToHost for {numElements} took {opWatch.Elapsed}");


            Console.WriteLine($"Total time for {numElements} took {sw.Elapsed}");
            // Verify that the result vector is correct
            for (int i = 0; i < numElements; ++i)
            {
                if (Math.Abs(h_A[i] + h_B[i] - h_C[i]) > 1e-5)
                {
                    Console.WriteLine("Result verification failed at element {0}!\n", i);
                    return;
                }
            }

            Console.WriteLine("Test PASSED\n");

            // Free device global memory
            d_A.Dispose();
            d_B.Dispose();
            d_C.Dispose();

            ctx.Dispose();
            Console.WriteLine("Done\n");
        }
    }
}
