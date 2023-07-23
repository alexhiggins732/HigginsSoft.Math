/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace HigginsSoft.Math.Demos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // run benchmarks:  dotnet run -c Release [argument]
            if (args.Length == 1)
            {
                Summary summary;
                //args[0] = "primerecip";
                switch (args[0])
                {
                    case "loop":
                         summary = BenchmarkRunner.Run<LoopBenchmark>();
                        break;
                    case "prints":
                        PrimeClasses.PrintClasses(); break;
                    case "factorial":
                        PrimeClasses.ShowProductFactorial(); break;
                    case "factorialfactor":
                        PrimeClasses.ProductFactorialFactor();
                        break;
                    case "primeinline":
                        //var summary2 = BenchmarkRunner.Run<InlinePrimeCheck>();
                        break;
                    case "timeprimechecker":
                        summary = BenchmarkRunner.Run<PrimeCheckerBenchmarks>();
                        //PrimeChecker.TestDiv();
                        break;
                    case "primerecip":
                        RecipTDiv.PrintRecipricols();
                        break;
                    case "primechecker":
                        //var summary1 = BenchmarkRunner.Run<PrimeChecker>();
                        PrimeCheckerBenchmarks.TestLoopDiv();
                        break;

                    //PrimeChecker.TestDiv();
                    case "avx":
                        //var summary1 = BenchmarkRunner.Run<PrimeChecker>();
                        summary = BenchmarkRunner.Run<AvxBenchmark>();
                        break;
                    case "avxsq":
                        //var summary1 = BenchmarkRunner.Run<PrimeChecker>();
                        summary = BenchmarkRunner.Run<AvxSquareGenBenchmark>();
                        break;
                    case "tdivcount":
                        TDivCount.PrintCountIncremental();
                        break;
                    case "tdivcountavx":
                        TDivCount.PrintCountIncrementalAvx();
                        break;
                }

            }
        }
    }
}