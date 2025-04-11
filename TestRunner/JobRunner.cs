using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    public class JobRunner : IDisposable
    {
        public void Dispose()
        {
            // Dispose of any resources if necessary
            foreach (var process in Processes)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
                process.Dispose();
            }
        }

        public List<Process> Processes { get; } = new List<Process>();

        internal void RunJob(string[] args)
        {


            var config = FactorConfig.GetFromCommandArgs(args.ToList());
            string? algo = config.FactorAlgorithm;
            int? digits = config.Digits;
            var startDigit = config.StartDigit;
            var endDigit = config.EndDigit;
            var batchSize = config.BatchSize;
            int totalJobs = config.TotalJobs;
            if (algo == null)
            {
                throw new ArgumentNullException("Algorithm cannot be null.");
            }

            if (startDigit < 0 || startDigit > endDigit)
            {
                throw new ArgumentOutOfRangeException("Start and end digits must be between 0 and 9, and start must be less than or equal to end.");
            }
            if (totalJobs < 1)
            {
                throw new ArgumentOutOfRangeException("Total jobs must be at least 1.");
            }
            if (config.JobName == null)
            {
                throw new ArgumentNullException("Job name, type, description, and parameters cannot be null.");
            }
            if (config.JobName.Length > 50)
            {
                throw new ArgumentException("Job name cannot exceed 50 characters.");
            }
            var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            for (var i = 0; i < totalJobs; i++)
            {
                var jobDirectoryPath = Path.Combine(AppContext.BaseDirectory, "Jobs", $"{config.JobName}_{i}");
                var jobDirectory = new DirectoryInfo(jobDirectoryPath);
                jobDirectory.Create();
            }

            var jobProcesses = Processes;

            var exeName = Path.Combine(AppContext.BaseDirectory, Process.GetCurrentProcess().ProcessName);
            if (!exeName.EndsWith(".exe"))
            {
                exeName += ".exe";
            }


            var optionalArguments = new List<string>();


            if (config.FactorAlgorithm != null)
            {
                optionalArguments.Add($"{config.FactorAlgorithm}");
            }

            if (config.B1 != null)
            {
                optionalArguments.Add($"{config.B1}");
            }
            if (config.B2 != null)
            {
                optionalArguments.Add($"{config.B2}");
            }
            if (config.Curves != null)
            {
                optionalArguments.Add($"{config.Curves}");
            }

            //if (config.ProcessorIndex != null)
            //{
            //optionalArguments.Add($"thread {config.ProcessorIndex}");
            optionalArguments.Add($"t {{i}}");
            //}
            if (config.EnableGpu != null)
            {
                optionalArguments.Add($"gpu {config.EnableGpu}");
            }

            if (config.Digits != null)
            {
                optionalArguments.Add($"digits {config.Digits}");
            }

            bool runProcesses = bool.Parse(bool.TrueString);
            for (int i = 0; i < totalJobs; i++)
            {

                var threadStartDigit = startDigit + (i * (endDigit - startDigit + 1) / totalJobs);
                var threadEndDigit = startDigit + ((i + 1) * (endDigit - startDigit + 1) / totalJobs) - 1;
                //p.StartInfo.Arguments = $"{startDigit} {endDigit} {batchSize} {algo}";
                var optionalArgs = string.Join(" ", optionalArguments.Select(x => x.Replace("{i}", $"{i}")));
                var jobCommandArguments = $"{threadStartDigit} {threadEndDigit} {batchSize} {optionalArgs} test";

                var jobDirectory = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Jobs", $"{config.JobName}_{i}"));

                Console.WriteLine($"[{DateTime.Now}] Starting job {i + 1}/{totalJobs} with arguments: {exeName} {jobCommandArguments} in {jobDirectory.FullName}");

                if (runProcesses)
                {
                    var p = new Process();
                    p.StartInfo.FileName = exeName;
                    p.StartInfo.WorkingDirectory = jobDirectory.FullName;
                    p.StartInfo.Arguments = jobCommandArguments;
                    jobProcesses.Add(p);
                    p.Start();
                    Console.WriteLine($"[{DateTime.Now}] Started job {i + 1}/{totalJobs} in {p.StartInfo.WorkingDirectory} with id {p.Id}");
                }


            }

            // Wait for all processes to finish
            while (Processes.Count > 0)
            {
                var completedProcess = Processes.FirstOrDefault(p => p.HasExited);
                if (completedProcess != null)
                {
                    Processes.Remove(completedProcess);
                    Console.WriteLine($"Process {completedProcess.Id} has exited. - {Processes.Count} jobs remaining");
                }
                else
                {
                    // Wait for a short period before checking again
                    System.Threading.Thread.Sleep(1000);
                }
            }

            // Copy the results to the main directory
        }


    }

    public static class IoExtensions
    {
        public static void CopyTo(this FileInfo source, string destFileName, bool overwrite)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destFileName == null) throw new ArgumentNullException(nameof(destFileName));
            using (var sourceStream = source.OpenRead())
            using (var destStream = File.Create(destFileName))
            {
                sourceStream.CopyTo(destStream);
            }
        }

        public static void CopyTo(this DirectoryInfo source, string destDirName, bool overwrite)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destDirName == null) throw new ArgumentNullException(nameof(destDirName));
            var destDir = new DirectoryInfo(destDirName);
            if (!destDir.Exists)
            {
                destDir.Create();
            }
            foreach (var file in source.GetFiles())
            {
                var destFile = Path.Combine(destDir.FullName, file.Name);
                file.CopyTo(destFile, overwrite);
            }
            foreach (var dir in source.GetDirectories())
            {
                var destSubDir = Path.Combine(destDir.FullName, dir.Name);
                dir.CopyTo(destSubDir, overwrite);
            }
        }
    }

}
