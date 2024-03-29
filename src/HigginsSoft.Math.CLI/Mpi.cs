﻿/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HigginsSoft.Math.CLI.Mpi;

namespace HigginsSoft.Math.CLI
{
    public class Logger
    {
#if RELEASE
        internal static bool EnableLogging = false;
        internal static bool EnableDebug = false;
#else
        internal static bool EnableLogging = true;
        internal static bool EnableDebug = true;
#endif
        private string id;
        private bool _enabled;
        private bool _debugEnabled;

        public Logger(string id)
        {
            this.id = id;
            Enabled = EnableLogging;
            DebugEnabled = EnableDebug;
        }

        public string Id { get => id; set => id = value; }
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public bool DebugEnabled { get => _debugEnabled; set => _debugEnabled = value; }


        public void WriteLine(string message)
        {
            if (Enabled)
                Console.WriteLine($"[{DateTime.Now}] {id} {message}");
        }

        public void DebugLine(object? message)
            => DebugLine(message?.ToString() ?? string.Empty);

        public void DebugLine(string message)
        {
            if (DebugEnabled)
                Console.WriteLine($"[{DateTime.Now}] {id} {message}");
        }
    }

    public class InProcessEmitConfig : ManualConfig
    {
        public InProcessEmitConfig()
        {
            AddJob(Job.MediumRun
                .WithToolchain(InProcessEmitToolchain.Instance));
        }
    }

    public class InProcessNoEmit : ManualConfig
    {
        public InProcessNoEmit()
        {
            AddJob(Job.MediumRun
                .WithToolchain(InProcessNoEmitToolchain.Instance));
        }
    }

    [Config(typeof(InProcessNoEmit))]
    public class MpiBenchmark
    {
        //[Params(8, 16)]
        [Params(16)]
        public int threads { get; set; } = 8;

        //[Params(1<<16,1<<20)]
        [Params(1ul << 30)]
        public ulong end { get; set; } = 65536;

        internal static void Run()
        {
            var b = new MpiBenchmark();
            b.end = 1ul << 32;
            b.threads = 16;
            b.TestMpiUint32();
        }

        [Benchmark]
        public void TestMpiUint32()
        {
            if (end > uint.MaxValue) end = uint.MaxValue;
            var args = $"threads {threads} range 1 {end}";
            Console.WriteLine($"Running args: {args}");
            MpiRunner.RunThreads(args.Split(' '));
        }
    }

   



    public class Mpi
    {
        public static void RunThread()
        {

            MpiRunner.RunRangeThread(ArgHelper.CommandLineArgs);
        }

        internal static void RunThreads()
        {
            MpiRunner.RunThreads(ArgHelper.CommandLineArgs);
        }

        internal class MpiRunner
        {
            public static Logger Logger = new Logger(id);


            /// <summary>
            /// args => $"threads {numThreads} command [commandArgs...]
            /// </summary>
            /// <param name="args"></param>
            public static void RunThreads(string[] args)
            {
                pipeName = "Master";


                /// threads {numThreads} command [commandArgs...]
                if (args.Length >= 3 && int.TryParse(args[1], out int numThreads))
                {
                    numThreads = numThreads < 1 ? 1 : numThreads;
                    var command = (args[2] ?? "").ToLower();
                    var commandArgs = args.Skip(3).ToArray();
                    switch (command)
                    {
                        case "range":

                            commandArgs = commandArgs.Select(x => x.Replace(",", "").Trim()).ToArray();
                            if (args.Length == 5
                                && uint.TryParse(commandArgs[0], out uint start)
                                 &&
                                    (uint.TryParse(commandArgs[1], out uint end)
                                    || TryParseExpression(commandArgs[1], out end)
                                    )
                                )
                            {
                                RunRangeThreads(numThreads, start, end);
                                return;
                            }
                            else
                            {
                                goto InvalidArgs;
                            }
                        default:
                            break;


                    }

                }

            InvalidArgs:

                Logger.WriteLine($"Invalid arguments {string.Join(", ", args)}");


            }

            private static bool TryParseExpression(string expression, out uint end)
            {
                end = 0;
                var result = false;
                if (expression != null)
                {
                    var idx = expression.IndexOf("^");
                    if (idx > 0 && idx < expression.Length)
                    {
                        var parts = expression.Split("^");
                        if (int.TryParse(parts[0].Trim(), out int @base) && int.TryParse(parts[1], out int power))
                        {
                            var product = (ulong)MathLib.Pow(@base, @power);
                            end = product <= uint.MaxValue ? (uint)product : uint.MaxValue;

                            return true;
                        }
                    }
                    idx = expression.IndexOf("<<");
                    if (idx > 0 && idx < expression.Length)
                    {
                        var parts = expression.Split("<<");
                        if (int.TryParse(parts[0].Trim(), out int @base) && int.TryParse(parts[1], out int power))
                        {
                            var product = 1ul << power;
                            end = product <= uint.MaxValue ? (uint)product : uint.MaxValue;
                        }
                    }
                }

                return result;
            }

            private static void RunRangeThreads(int numThreads, uint start, uint end)
            {
                var sieveSize = end - start;

                var exeName = Process.GetCurrentProcess().ProcessName;
                var exeFileName = $"{exeName}.exe";

                uint rangeSize = (uint)((end - start) / numThreads);
                uint[] threadStarts = Enumerable.Range(0, numThreads)
                                               .Select(i => (uint)i * rangeSize + start)
                                               .ToArray();
                // Each thread starts at threadStarts[i] and ends at threadStarts[i+1]-1 (except for the last thread)
                for (int i = 0; i < numThreads; i++)
                {
                    uint threadStart = threadStarts[i];
                    uint threadEnd = (i < numThreads - 1) ? threadStarts[i + 1] - 1 : end;
                    // TODO: Launch thread with start=threadStart and end=threadEnd
                }

                var processData = Enumerable.Range(0, threadStarts.Length)
                    .Select(i =>
                    new ThreadArgument(

                        index: i,
                        start: threadStarts[i],
                        end: (i < numThreads - 1) ? threadStarts[i + 1] - 1 : end,
                        name: Guid.NewGuid().ToString().Substring(0, 8),
                        exe: exeFileName,
                        command: "range"
                   )).ToArray();

                runCommandArgs(processData);

            }

            private static void runCommandArgs(ThreadArgument[] startInfos)
            {
                int numThreads = startInfos.Length;
                var tasks = new List<Task>();
                var procs = new List<Process>();
                int[][] processResults = new int[numThreads][];
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < numThreads; i++)
                {
                    var x = startInfos[i];
                    var p = new Process();
                    p.StartInfo.FileName = x.Exe;
                    p.StartInfo.Arguments = x.Args;
                    p.Start();

                    procs.Add(p);
                    // Wait for the sub-process to complete and capture the results
                    NamedPipeServerStream pipeServer = new NamedPipeServerStream(x.Name);
                    var monitorTask = Task.Run(() =>
                    {
                        if (!p.HasExited)
                        {
                            pipeServer.WaitForConnection();
                            Logger.WriteLine($"Client connected {pipeName}");

                            ReadResultsFromSubProcess(processResults, x.Index, p, pipeName, pipeServer);
                        }
                        else
                        {
                            Logger.WriteLine($"Client failed to connect {pipeName}");
                        }

                        p.WaitForExit();
                    });
                    tasks.Add(monitorTask);
                }
                while (tasks.Count > 0)
                {
                    System.Threading.Thread.Sleep(100);
                    tasks = tasks.Where(x => x.IsCompleted == false).ToList();
                }
                sw.Stop();
                long sum = 0;
                for (var i = 0; i < processResults.Length; i++)
                {
                    var resultCount = processResults[i][0];
                    sum += resultCount;
                    Logger.WriteLine($"Result {i} {string.Join(", ", resultCount)}");
                }
                Console.WriteLine($"[{DateTime.Now}] Completed {startInfos.Length} processes in {sw.Elapsed}");
                Console.WriteLine();
                Console.WriteLine("".PadLeft(Console.BufferWidth, '='));
                Console.WriteLine($"Prime Count: {sum} - Range {startInfos.First().Start.ToString("N0")} to {startInfos.Last().End.ToString("N0")} ");
                Console.WriteLine();
                Console.WriteLine("".PadLeft(Console.BufferWidth, '='));
                Console.WriteLine();

            }

            /// <summary>
            /// args => $"thread {Index} {Name} {Command} {Start} {End}"
            /// </summary>
            /// <param name="args"></param>
            public static void RunRangeThread(string[] args)
            {

                //Args => $"thread {Index} {Name} {Command} {Start} {End}";
                pipeName = "invalid";
                if (args.Length == 6)
                {
                    if (
                        int.TryParse(args[1], out var clientIndex)
                        && uint.TryParse(args[4], out var startIndex)
                        && uint.TryParse(args[5], out var endIndex)
                        )
                    {
                        pipeName = args[2];
                        var command = args[3];
                        //Logger.WriteLine($"Setting Processor affinity to {affinityMask} for process {p.Id}");
                        SetProcessor(clientIndex);

                        Logger.WriteLine($"Running client {string.Join(" ", args)}");
                        var sw = Stopwatch.StartNew();
                        LaunchSieve(startIndex, endIndex, pipeName);
                        Logger.WriteLine($"Completed {startIndex} - {endIndex} in {sw.Elapsed}");
                    }

                }
                else
                {
                    Logger.WriteLine($"Invalid arguments {string.Join(", ", args)}");
                }

            }



            [SupportedOSPlatformGuard("windows")]  // The platform guard attributes used
            [SupportedOSPlatformGuard("linux")]
            private static readonly bool _isWindowsOrLinux = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

            private static void SetProcessor(int clientIndex)
            {
                int processorAffinity = clientIndex % Environment.ProcessorCount;
                var affinityMask = 1 << processorAffinity;
                var p = Process.GetCurrentProcess();


                p.PriorityClass = ProcessPriorityClass.BelowNormal;
                if (_isWindowsOrLinux)
                {
                    Console.WriteLine($"[{DateTime.Now}] {id} Setting Processor affinity to {affinityMask} for process {p.Id}");
                    p.ProcessorAffinity = (IntPtr)(affinityMask);
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now}] {id} Setting Processor affinity nonly supported on Windows and Linux");
                }

            }

            private static string _pipeName = string.Empty;
            private static string pipeName
            {
                get => _pipeName; set { Logger.Id = _pipeName = value; }
            }
            private static string id
            {
                get => $"[{pipeName}]"; set { Logger.Id = pipeName = value; }
            }
            private static void LaunchSieve(uint startIndex, uint endIndex, string pipeName)
            {


                var count = 0;

                var gen = new PrimeGeneratorUnsafeUint(startIndex, endIndex);
                Logger.DebugLine($"Created generator({startIndex}, {endIndex})");
                var iter = gen.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (Logger.DebugEnabled)
                    {
                        Logger.DebugLine(iter.Current);
                    }

                    count++;
                }

                var result = new[] { count };
                //TODO: share result with main process.
                //Logger.WriteLine($"Calling WriteResultsToPipe for {result.Length} primes.");
                WriteResultsToPipe(pipeName, result);
                //Logger.WriteLine($"Wrote results to pipe: {pipeName}");
            }




            private static void WriteResultsToPipe(string pipeName, int[] result)
            {
                var bytes = new byte[result.Length << 2];
                Buffer.BlockCopy(result, 0, bytes, 0, bytes.Length);
                //Logger.WriteLine($"Copied {result.Length} primes to byte array.");
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName))
                {
                    //Logger.WriteLine($"Connecting to pipe {result.Length}.");
                    pipeClient.Connect();

                    //Logger.WriteLine($"Writing {result.Length} primes to pipe: Buffer {pipeClient.OutBufferSize}");


                    using (var br = new BinaryWriter(pipeClient))
                    {
                        //Logger.WriteLine($"Writing Length {result.Length} to pipe.");
                        br.Write(result.Length);
                        br.Write(bytes);
                        //Logger.WriteLine($"Sent length of {result.Length} to pipe.");
                        //br.Write(bytes);
                        //Logger.WriteLine($"Send {bytes.Length} bytes");
                        br.Flush();
                        //pipeClient.WaitForPipeDrain();
                    }

                    // pipeClient.Close();
                    //Logger.WriteLine($"Finished writing {result.Length - 1} primes to pipe");
                }

            }


            static void ReadResultsFromSubProcess(int[][] processResults, int index, Process subProcess, string pipeName, NamedPipeServerStream pipeServer)
            {
                // TODO: Implement this method to read the results from a sub-process
                //       and return them as an int array.

                try
                {
                    using (var reader = new BinaryReader(pipeServer))
                    {
                        int count = reader.ReadInt32();
                        Logger.WriteLine($"Read prime count {count} from client {pipeName}");
                        var result = new int[count];

                        if (count > 0)
                        {
                            var buffer = reader.ReadBytes(count << 2);
                            Buffer.BlockCopy(buffer, 0, result, 0, buffer.Length);
                        }
                        processResults[index] = result;

                    }
                    pipeServer.Close();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Error reading primes from pipe  {pipeName}: {ex}");
                }

            }

            static int[] CombineResults(int[][] processResults)
            {
                // TODO: Implement this method to combine the results from all
                //       sub-processes into a single int array.
                //throw new NotImplementedException();
                return processResults.SelectMany(x => x).ToArray();
            }

            private class ThreadArgument
            {
                public int Index { get; set; }
                public uint Start { get; set; }
                public uint End { get; set; }
                public string Name { get; set; }
                public string Exe { get; set; }
                public string Command { get; set; }
                public string Args => $"thread {Index} {Name} {Command} {Start} {End}";
                public ThreadArgument(int index, uint start, uint end, string name, string exe, string command)
                {
                    Index = index;
                    Start = start;
                    End = end;
                    Name = name;
                    Exe = exe;
                    Command = command;
                }
            }
        }

        internal class PrimeSieve
        {
            internal static void Seive(int startIndex, int endIndex, int[] result)
            {
                //throw new NotImplementedException();
            }
        }
    }


}
