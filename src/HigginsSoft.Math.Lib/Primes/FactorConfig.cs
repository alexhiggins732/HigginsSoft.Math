/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System;
using static HigginsSoft.Math.Lib.FactorizationBigInteger;

namespace HigginsSoft.Math.Lib
{
    public class FactorConfig
    {
        public bool checkPrimality = true;
        public bool skipTrialDivide = true;
        public bool skipFermat = true;
        public bool skipRho = true;
        public bool skipRhoP2 = true;
        public bool skipRhoP3 = true;
        public bool skipRhoZ = true;
        public bool skipPP1 = true;
        public bool skipPM1 = true;
        public bool skipECM = true;
        public bool skipQS = true;
        public bool skipFact = true;
        public int? B1 = null;
        public int? B2 = null;
        public int? Curves = null;
        public int? Digits = null;
        public bool? EnableGpu = null;
        public string? JobName = null;
        public int TotalJobs = 1;
        public int? ProcessorIndex { get; internal set; }
        public string? FactorAlgorithm { get; private set; }
        public int? StartDigit { get; private set; }
        public int? EndDigit { get; private set; }
        public int? BatchSize { get; private set; }
        public int TotalThreads { get; private set; }

        static FactorConfig? commandLineConfig = null;
        public static FactorConfig GetFromCommandArgs(List<string> args)
        {
            // copy the arguments to a new list
            var argCopy = args.ToList();

            var process = System.Diagnostics.Process.GetCurrentProcess();
            //check if the first argument is the current process name and if it is remove it.
            if (argCopy.Count > 0 && argCopy[0].Equals(process.ProcessName, StringComparison.OrdinalIgnoreCase))
            {
                argCopy.RemoveAt(0);
            }
            return ParseCommandLineArguments(argCopy);
        }

        static FactorConfig ParseCommandLineArguments(List<string> argCopy)
        {
            var commandLineConfig = new FactorConfig();
            int idx = -1;
            // todo remove non-positional args
            if (argCopy.Contains("digits", StringComparer.OrdinalIgnoreCase))
            {
                idx = argCopy.Select(x => x.ToLower()).ToList().IndexOf("digits");
                if (idx < argCopy.Count - 1 && int.TryParse(argCopy[idx + 1], out var digits))
                {
                    commandLineConfig.Digits = digits;
                    // remove the args from the list
                    argCopy.RemoveAt(idx + 1);
                    argCopy.RemoveAt(idx);

                }
            }

            if (argCopy.Contains("threads", StringComparer.OrdinalIgnoreCase) )
            {
                idx = argCopy.Select(x => x.ToLower()).ToList().IndexOf("threads");
                if (idx < argCopy.Count - 1 && int.TryParse(argCopy[idx + 1], out var totalThreads))
                {
                    commandLineConfig.TotalThreads = totalThreads;
                }
                // remove the args from the list
                argCopy.RemoveAt(idx + 1);
                argCopy.RemoveAt(idx);

            }

            if (argCopy.Contains("thread", StringComparer.OrdinalIgnoreCase) || argCopy.Contains("t", StringComparer.OrdinalIgnoreCase))
            {
                idx = argCopy.Select(x => x.ToLower()).ToList().IndexOf("thread");
                if (idx == -1) idx = argCopy.IndexOf("t");
                if (idx < argCopy.Count - 1 && int.TryParse(argCopy[idx + 1], out var threadNumber))
                {
                    commandLineConfig.ProcessorIndex = threadNumber;
                }
                // remove the args from the list
                argCopy.RemoveAt(idx + 1);
                argCopy.RemoveAt(idx);

            }
            if (argCopy.Contains("job", StringComparer.OrdinalIgnoreCase))
            {
                idx = argCopy.Select(x => x.ToLower()).ToList().IndexOf("job");
                commandLineConfig.JobName = argCopy[idx + 1];
                if (idx < argCopy.Count - 2 && int.TryParse(argCopy[idx + 2], out var jobCount))
                {
                    commandLineConfig.TotalJobs = jobCount;

                    // remove the args from the list
                    argCopy.RemoveAt(idx + 2);
                }
                else
                {
                    commandLineConfig.TotalJobs = 1;
                }
                // remove the args from the list
                argCopy.RemoveAt(idx + 1);
                argCopy.RemoveAt(idx);

            }


            if (argCopy.Contains("gpu", StringComparer.OrdinalIgnoreCase))
            {
                idx = argCopy.Select(x => x.ToLower()).ToList().IndexOf("gpu");
                commandLineConfig.EnableGpu = true;
                // remove the args from the list
                argCopy.RemoveAt(idx);
            }

            if (argCopy.Count >= 3 && int.TryParse(argCopy[0], out int minDigits) && int.TryParse(argCopy[1], out int maxDigits) && int.TryParse(argCopy[2], out int batchSize))
            {
                commandLineConfig.StartDigit = minDigits;
                commandLineConfig.EndDigit = maxDigits;
                commandLineConfig.BatchSize = batchSize;

                // remove the args from the list
                argCopy.RemoveAt(0);
                argCopy.RemoveAt(0);
                argCopy.RemoveAt(0);

            }
            else if (argCopy.Count >= 2 && int.TryParse(argCopy[0], out minDigits) && int.TryParse(argCopy[1], out maxDigits))
            {
                commandLineConfig.StartDigit = minDigits;
                commandLineConfig.EndDigit = maxDigits;

                // remove the args from the list
                argCopy.RemoveAt(0);
                argCopy.RemoveAt(0);
            }
            else if (argCopy.Count >= 1 && int.TryParse(argCopy[0], out maxDigits))
            {
                commandLineConfig.EndDigit = 0;
                // remove the args from the list
                argCopy.RemoveAt(0);

            }



            var algos = (new[] { "fermat", "rho", "rhop2", "rhop3", "rhoz", "pp1", "pm1", "ecm", "qs", "siqs", "tdiv","trialdivide", "fact", "factor" })
                .Select(x => x.ToLower()) // insensitive incase another developer uses a different case
                .ToList();

            // check for the algorithm name and arguments and remove them.
            if (argCopy.Count > 0)
            {

                if (algos.Contains(argCopy[0], StringComparer.CurrentCultureIgnoreCase));
                var arg = argCopy[0].ToLower();
                idx = 0;
                // De-alias the algorithm names to the short name
                if (arg == "factor") arg = "fact"; // factor is the same as fact
                else if (arg == "siqs") arg = "qs"; // siqs is the same as qs
                else if (arg == "trialdivide") arg = "tdiv"; // tdiv is the same as trialdivide

                switch (arg)
                {
                    case FactorizationSwitches.fermat:
                        commandLineConfig.skipFermat = false; break;
                    case FactorizationSwitches.rho:
                        commandLineConfig.skipRho = false; break;
                    case FactorizationSwitches.rhop2:
                        commandLineConfig.skipRhoP2 = false; break;
                    case FactorizationSwitches.rhop3:
                        commandLineConfig.skipRhoP3 = false; break;
                    case FactorizationSwitches.rhoz:
                        commandLineConfig.skipRhoZ = false; break;
                    case FactorizationSwitches.pp1:
                        commandLineConfig.skipPP1 = false; break;
                    case FactorizationSwitches.pm1:
                        commandLineConfig.skipPM1 = false; break;
                    case FactorizationSwitches.ecm:
                        commandLineConfig.skipECM = false; break;
                    case FactorizationSwitches.qs:
                    case FactorizationSwitches.siqs:
                        commandLineConfig.skipQS = false; break;
                    case FactorizationSwitches.tdiv:
                    case FactorizationSwitches.trialdivide:
                        commandLineConfig.skipTrialDivide = false; break;
                    case FactorizationSwitches.fact:
                    case FactorizationSwitches.factor:
                        commandLineConfig.skipFact = false; break;
                    default:
                        break;
                }

                commandLineConfig.FactorAlgorithm = arg;

                if (idx < argCopy.Count - 3 && arg == "ecm"
                       && int.TryParse(argCopy[idx + 1], out int b1)
                       && int.TryParse(argCopy[idx + 2], out int b2)
                       && int.TryParse(argCopy[idx + 3], out int curves)
                       )
                {
                    commandLineConfig.B1 = b1;
                    commandLineConfig.B2 = b2;
                    commandLineConfig.Curves = curves;

                    // remove the args from the list
                    argCopy.RemoveAt(idx + 3);  // remove curves
                    argCopy.RemoveAt(idx + 2);  // remove b2
                    argCopy.RemoveAt(idx + 1);  // remove b1
                    argCopy.RemoveAt(idx);      // remove ecm
                }

                // test for b1 and b2 argument bounds following the arg for fermat, rho[x], pp1, pm1, ecm, or tdiv. qs/siqs don't have bounds argument
                else if (idx < argCopy.Count - 2 && int.TryParse(argCopy[idx + 1], out b1) && int.TryParse(argCopy[idx + 2], out b2))
                {
                    commandLineConfig.B1 = b1;
                    commandLineConfig.B2 = b2;

                    // remove the args from the list
                    argCopy.RemoveAt(idx + 2);  // remove b2
                    argCopy.RemoveAt(idx + 1);  // remove b1
                    argCopy.RemoveAt(idx);      // remove the algo

                }

                else if (idx < argCopy.Count - 1 && int.TryParse(argCopy[idx + 1], out b1))
                {
                    commandLineConfig.B1 = b1;

                    // remove the args from the list
                    argCopy.RemoveAt(idx + 1);  // remove b1
                    argCopy.RemoveAt(idx);      // remove the algo

                }

            }

            return commandLineConfig;
        }
        public static FactorConfig GetCommandLineConfig()
        {
            if (commandLineConfig is null)
            {
                commandLineConfig = new FactorConfig();
                var args = CommandLine.Arguments.Select(x => x.Trim().ToLower()).ToList();
                commandLineConfig = GetFromCommandArgs(args);

            }
            return commandLineConfig;
        }

    }
    public class CommandLine
    {
        public static string[] Arguments { get; private set; } = Array.Empty<string>();

        public static FactorConfig GetFactorArguments() => FactorConfig.GetCommandLineConfig();

        public static void SetArguments(string[] args)
        {
            Arguments = args;
        }
    }
}