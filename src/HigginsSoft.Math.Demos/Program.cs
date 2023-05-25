/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using BenchmarkDotNet.Running;
using System.Reflection;

namespace HigginsSoft.Math.Demos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // run benchmarks:  dotnet run -c Release [argument]
            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "loop":
                        var summary = BenchmarkRunner.Run<LoopBenchmark>();
                        break;
                    case "printresidues":
                        PrimeResidueClasses.PrintResidueClasses(); break;
                    case "factorial":
                        PrimeResidueClasses.ShowProductFactorial(); break;
                    case "factorialfactor":
                        PrimeResidueClasses.ProductFactorialFactor();
                        break;
                    case "primeinline":
                        //var summary2 = BenchmarkRunner.Run<InlinePrimeCheck>();
                        break;
                    case "timeprimechecker":
                        var summary1 = BenchmarkRunner.Run<PrimeCheckerBenchmarks>();
                        //PrimeChecker.TestDiv();
                        break;
                    case "primechecker":
                        //var summary1 = BenchmarkRunner.Run<PrimeChecker>();
                        PrimeCheckerBenchmarks.TestLoopDiv();
                        break;
                }

            }
        }
    }
}