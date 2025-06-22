using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using static HigginsSoft.Math.Lib.FactorizationBigInteger;
using static HigginsSoft.Math.Lib.NumericsYafu;

namespace HigginsSoft.Math.Lib
{
    public class GmpEcm
    {
        /// <summary>
        /// Runs the GMP-ECM P+1 facotrization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="curves"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Factorization PP1(GmpInt n, int targetDigits = 20, long? B1 = null, long? B2 = null)
        {
            if (targetDigits <= 0)
            {
                throw new ArgumentException("targetDigits must be specified and greater than 0.");
            }

            if (!DefaultParameters.ContainsKey(targetDigits))
            {
                throw new ArgumentException($"No default parameters for targetDigits = {targetDigits}");
            }

            var defaults = DefaultParameters[targetDigits];

            long effectiveB1 = B1 ?? defaults.B1;
            long effectiveB2 = B2 ?? defaults.B2;


            // Debug output to show the effective parameters used.
            Console.WriteLine($"Running PP1 with parameters: B1 = {effectiveB1}, B2 = {effectiveB2}");

            // Implementation of the ECM algorithm would go here.

            var result = RunGmpEcm(n, effectiveB1, effectiveB2, 0, false, algo: "-pp1");
            //var di = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "EcmTests"));
            //var output = Path.Combine(di.FullName, $"{n}-ecm-test.log");
            //File.WriteAllText(output, result.Output);

            Factorization f = new();
            try
            {
                f = ParseFactors(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
                Console.WriteLine($"Output: {result.Output}");
            }
            return f;
        }

        /// <summary>
        /// Runs the GMP-ECM P-1 facotrization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="curves"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Factorization PM1(GmpInt n, int targetDigits = 20, long? B1 = null, long? B2 = null)
        {
            if (targetDigits <= 0)
            {
                throw new ArgumentException("targetDigits must be specified and greater than 0.");
            }

            if (!DefaultParameters.ContainsKey(targetDigits))
            {
                throw new ArgumentException($"No default parameters for targetDigits = {targetDigits}");
            }

            var defaults = DefaultParameters[targetDigits];

            long effectiveB1 = B1 ?? defaults.B1;
            long effectiveB2 = B2 ?? defaults.B2;


            // Debug output to show the effective parameters used.
            Console.WriteLine($"Running PM1 with parameters: B1 = {effectiveB1}, B2 = {effectiveB2}");

            // Implementation of the ECM algorithm would go here.

            var result = RunGmpEcm(n, effectiveB1, effectiveB2, 0, false, algo: "-pm1");
            //var di = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "EcmTests"));
            //var output = Path.Combine(di.FullName, $"{n}-ecm-test.log");
            //File.WriteAllText(output, result.Output);

            Factorization f = new();
            try
            {
                f = ParseFactors(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
                Console.WriteLine($"Output: {result.Output}");
            }
            return f;
        }

        private static readonly Dictionary<int, (long B1, long B2, int Curves)> DefaultParameters = DefaultParams.Defaults;


        /// <summary>
        /// Runs the GMP-ECM factorization algorithm.
        /// If any of the optional parameters (B1, B2, or curves) are not specified,
        /// they are set to default values based on the expected digit count of the target factor.
        /// </summary>
        /// <param name="n">The number to factorize.</param>
        /// <param name="targetDigits">
        /// The expected number of digits of the factor.
        /// (Must be one of the keys defined in the DefaultParameters dictionary.)
        /// </param>
        /// <param name="B1">Optional optimal B1 parameter.</param>
        /// <param name="B2">Optional default B2 parameter.</param>
        /// <param name="curves">Optional expected number of curves.</param>
        /// <returns>A list of factors (implementation not provided).</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when targetDigits is not positive or when no defaults exist for the specified targetDigits.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Thrown because the actual ECM algorithm is not implemented.
        /// </exception>
        public Factorization ECM(GmpInt n, int targetDigits = 20, long? B1 = null, long? B2 = null, int? curves = null, bool? enableGpu = false)
        {
            if (enableGpu is null)
            {
                enableGpu = false;
            }
            if (targetDigits <= 0)
            {
                throw new ArgumentException("targetDigits must be specified and greater than 0.");
            }

            if (!DefaultParameters.ContainsKey(targetDigits))
            {
                throw new ArgumentException($"No default parameters for targetDigits = {targetDigits}");
            }

            var defaults = DefaultParameters[targetDigits];

            long effectiveB1 = B1 ?? defaults.B1;
            long effectiveB2 = B2 ?? defaults.B2;
            int effectiveCurves = curves ?? defaults.Curves;

            // Debug output to show the effective parameters used.
            //Console.WriteLine($"Running ECM with parameters: B1 = {effectiveB1}, B2 = {effectiveB2}, Curves = {effectiveCurves}");

            // Implementation of the ECM algorithm would go here.

            var result = RunGmpEcm(n, effectiveB1, effectiveB2, effectiveCurves, enableGpu.Value);
            //var di = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "EcmTests"));
            //var output = Path.Combine(di.FullName, $"{n}-ecm-test.log");
            //File.WriteAllText(output, result.Output);

            Factorization f = new();
            try
            {
                f = ParseFactors(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
                Console.WriteLine($"Output: {result.Output}");
            }
            return f;
        }


        // TODO: Need to dedupe factors and return a list of unique actual factors.
        public static Factorization ParseFactors(ProcessResult result)
        {
            return ParseFactors(result.Output);

        }
        public static Factorization ParseFactors(string ecmOutput)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.


            var factorParts = ecmOutput.Split("********** Factor found");
            var f = new Factorization();

            if (factorParts.Length > 1)
            {
                for (var p = 1; p < factorParts.Length; p++)
                {
                    using var partFactors = ParseFactorPart(factorParts[p]);
                    f.Add(partFactors);
                    if (p == factorParts.Length - 1)
                    {
                        using var coFactor = ParseCoFactor(factorParts[p]);
                        f.Add(coFactor);
                    }
                }
            }
            return f;
        }

        static Factorization ParseCoFactor(string factorPart)
        {
            var result = new Factorization();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf("cofactor");
                if (idx > -1)
                {
                    var str = line.Substring(idx + "cofactor".Length + 1).Trim();
                    var header = line.Substring(0, idx).ToLower().Trim();


                    str = str.Split(" ")[0];
                    var f = new GmpInt(str);
                    var factor = new Factor(f, 1);
                    result.Add(f, 1);
                    switch (header)
                    {
                        case "composite":
                            factor.FactorType = MathLib.PrimalityType.Composite;
                            break;
                        case "prime":
                            factor.FactorType = MathLib.PrimalityType.Prime;
                            break;
                        case "probable prime":
                            factor.FactorType = MathLib.PrimalityType.ProbablePrime;
                            break;

                        default:
                            throw new Exception($"Unexpected header in factor output - {header}");
                    }
                }
            }
            return result;
        }

        static Factorization ParseFactorPart(string factorPart)
        {
            Factorization result = new Factorization();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf(":");
                if (idx > -1)
                {
                    var str = line.Substring(idx + 1).Trim();
                    GmpInt f = new GmpInt(str);
                    result.Add(f, 1);
                    break;
                }
            }
            return result;
        }


        public static FactorizationBigInteger ParseFactorsNumeric(string ecmOutput)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.


            var factorParts = ecmOutput.Split("********** Factor found");
            var f = new FactorizationBigInteger();

            if (factorParts.Length > 1)
            {
                for (var p = 1; p < factorParts.Length; p++)
                {
                    using var partFactors = ParseFactorPartNumeric(factorParts[p]);
                    f.Add(partFactors);
                    if (p == factorParts.Length - 1)
                    {
                        using var coFactor = ParseCoFactorNumeric(factorParts[p]);
                        f.Add(coFactor);
                    }
                }
            }
            return f;
        }

        static FactorizationBigInteger ParseCoFactorNumeric(string factorPart)
        {
            var result = new FactorizationBigInteger();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf("cofactor");
                if (idx > -1)
                {
                    var str = line.Substring(idx + "cofactor".Length + 1).Trim();
                    var header = line.Substring(0, idx).ToLower().Trim();


                    str = str.Split(" ")[0];
                    var f = BigInteger.Parse(str);
                    var factor = new Factor<BigInteger>(f, 1);
                    result.Add(f, 1);
                    switch (header)
                    {
                        case "composite":
                            factor.FactorType = MathLib.PrimalityType.Composite;
                            break;
                        case "prime":
                            factor.FactorType = MathLib.PrimalityType.Prime;
                            break;
                        case "probable prime":
                            factor.FactorType = MathLib.PrimalityType.ProbablePrime;
                            break;

                        default:
                            throw new Exception($"Unexpected header in factor output - {header}");
                    }
                }
            }
            return result;
        }

        static FactorizationBigInteger ParseFactorPartNumeric(string factorPart)
        {
            FactorizationBigInteger result = new();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf(":");
                if (idx > -1)
                {
                    var str = line.Substring(idx + 1).Trim();
                    BigInteger f = BigInteger.Parse(str);
                    result.Add(f, 1);
                    break;
                }
            }
            return result;
        }

        // T

        // TODO: Need to dedupe factors and return a list of unique actual factors.
        private List<Factor> ParseFactors1(ProcessResult result)
        {
            var factors = new List<Factor>();
            // Parse the output of the GMP-ECM process to extract the factors.

            Action<string> AddFactor = (str) =>
            {
                GmpInt f = new GmpInt(str);
                var factor = factors.FirstOrDefault(x => x.P == f);
                if (factor != null)
                    factor.Power++;
                else
                {
                    factor = new Factor(f, 1);
                    factors.Add(factor);
                }
            };
            var factorParts = result.Output.Split("********** Factor found");
            if (factorParts.Length > 1)
            {

                for (var p = 1; p < factorParts.Length; p++)
                {
                    var factorPart = factorParts[p];
                    var lines = factorPart.Split('\n');

                    bool hasMoreFactors = p < factorParts.Length - 1;
                    for (var i = 0; i < lines.Length;)
                    {
                        i++;
                        var line = lines[i].Trim();
                        while (line.IndexOf(":") > -1)
                        {
                            var factorStr = line.Substring(line.IndexOf(":") + 1).Trim();
                            AddFactor(factorStr);
                            continue;
                        }


                        var cfIndex = line.IndexOf("cofactor");
                        var header = line.Substring(0, cfIndex).ToLower().Trim();
                        switch (header)
                        {
                            case "composite":
                                if (!hasMoreFactors)
                                {
                                    var cfString = line.Substring(cfIndex + "cofactor".Length).Trim();
                                    cfString = cfString.Split(" ")[0];
                                    AddFactor(cfString);
                                }
                                break;
                            case "prime":
                            case "probably prime":
                                {
                                    var cfString = line.Substring(cfIndex + "cofactor".Length).Trim();
                                    cfString = cfString.Split(" ")[0];
                                    AddFactor(cfString);
                                    continue;
                                }
                                break;

                            default:
                                throw new Exception("Unexpected header in factor output");
                        }

                        //if (hasMoreFactors)
                        //{
                        //    break;
                        //}


                        //GmpInt cf = new GmpInt(cfString);
                        //var cofactor = new Factor(cf, 1);
                        //factors.Add(cofactor);
                    }




                }
            }






            return factors;
        }

        private ProcessResult RunGmpEcm(GmpInt n, long effectiveB1, long effectiveB2, int effectiveCurves, bool enableGpu, string algo = "")
        {
            // -I f increment B1 by f*sqrt(B1) on each run
            // docs suggest f=10 for B1 < 10^6, f=5 for B1 < 10^9, f=2 for B1 < 10^12
            /*
                pm1(n, b1*10);
                pp1(n, b1 * 5) 3 times
                ecm(n, b1);
                TLevel += 5 digits;
            */

            string gpuSwitch = enableGpu ? " -gpu" : "";
            string b2Switch = enableGpu ? " -gpu" : "";
            var exeName = "ecm_gpu.exe";
            if (!string.IsNullOrEmpty(algo))
            {
                algo = $" {algo}";
            }

            //TODO: stage exe in stand-alone directory to allow multiple instances to run
            //var workingDirectory = @"E:\Source\Repos\NumTheory\msieve\HigginsSoft\gmp-ecm-alexhiggins732\bin\x64\Release";
            var exeFullPath = Path.Combine(AppContext.BaseDirectory, "binaries", exeName);
            var workingDirectory = Path.GetFullPath(".");
            var cmd = $"echo \"{n}\" | \"{exeFullPath}\"{gpuSwitch}{algo} -c {effectiveCurves} {effectiveB1} {effectiveB2}";

            //todo us DotMpi, for now use process helper
            var result = ProcessHelper.RunProcess(cmd, workingDirectory);
            return result;

        }

    }


    public class DefaultParams
    {
        // Mapping from the target factor digit count to default ECM parameters.
        // The table below is based on:
        //   digits   optimal B1   default B2       expected curves
        //   20       11e3         1.9e6            74
        //   25        5e4         1.3e7           221
        //   30       25e4         1.3e8           453
        //   35        1e6         1.0e9           984
        //   40        3e6         5.7e9          2541
        //   45       11e6        3.5e10          4949
        //   50       43e6        2.4e11          8266
        //   55       11e7        7.8e11         20158
        //   60       26e7        3.2e12         47173
        //   65       85e7        1.6e13         77666
        public static readonly Dictionary<int, (long B1, long B2, int Curves)> Defaults =
             new Dictionary<int, (long B1, long B2, int Curves)>
             {
                { 10, (B1: 2000,      B2: 147396,        Curves: 74) },
                { 15, (B1: 5000,      B2: 600786,       Curves: 74) },
                { 20, (B1: 11000,     B2: 1900000,     Curves: 74) },
                { 25, (B1: 50000,     B2: 13000000,    Curves: 221) },
                { 30, (B1: 250000,    B2: 130000000,   Curves: 453) },
                { 35, (B1: 1000000,   B2: 1000000000,  Curves: 984) },
                { 40, (B1: 3000000,   B2: 5700000000,  Curves: 2541) },
                { 45, (B1: 11000000,  B2: 35000000000, Curves: 4949) },
                { 50, (B1: 43000000,  B2: 240000000000,Curves: 8266) },
                { 55, (B1: 110000000, B2: 780000000000,Curves: 20158) },
                { 60, (B1: 260000000, B2: 3200000000000,Curves: 47173) },
                { 65, (B1: 850000000, B2: 16000000000000,Curves: 77666) }
             };
    }

    public class NumericsEcm
    {

        static Dictionary<int, (long B1, long B2, int Curves)> DefaultParameters = DefaultParams.Defaults;
        /// <summary>
        /// Runs the GMP-ECM P+1 facotrization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="curves"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public FactorizationBigInteger PP1(BigInteger n, int? targetDigits = null, long? B1 = null, long? B2 = null)
        {
            bool? enableGpu = false;
            (long B1, long B2, int Curves) defaults = GetDefaults(targetDigits, B1, ref enableGpu);

            int effectiveDigits = targetDigits ?? 20;
            if (effectiveDigits <= 0)
            {
                throw new ArgumentException("targetDigits must be specified and greater than 0.");
            }

            //if (!DefaultParameters.ContainsKey(effectiveDigits))
            //{
            //    throw new ArgumentException($"No default parameters for targetDigits = {effectiveDigits}");
            //}



            long effectiveB1 = B1 ?? defaults.B1;
            long effectiveB2 = B2 ?? defaults.B2;


            // Debug output to show the effective parameters used.
            //Console.WriteLine($"Running PP1 with parameters: B1 = {effectiveB1}, B2 = {effectiveB2}");

            // Implementation of the ECM algorithm would go here.

            var result = RunBigIntegerEcm(n, effectiveB1, effectiveB2, 0, false, algo: "-pp1");
            //var di = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "EcmTests"));
            //var output = Path.Combine(di.FullName, $"{n}-ecm-test.log");
            //File.WriteAllText(output, result.Output);

            FactorizationBigInteger f = new();
            try
            {
                f = ParseFactors(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
                Console.WriteLine($"Output: {result.Output}");
            }
            return f;
        }

        /// <summary>
        /// Runs the GMP-ECM P-1 facotrization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="curves"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public FactorizationBigInteger PM1(BigInteger n, int? targetDigits = null, long? B1 = null, long? B2 = null)
        {

            bool? enableGpu = false;
            (long B1, long B2, int Curves) defaults = GetDefaults(targetDigits, B1, ref enableGpu);

            int effectiveDigits = targetDigits ?? 20;
            if (effectiveDigits <= 0)
            {
                throw new ArgumentException("targetDigits must be specified and greater than 0.");
            }

            //if (!DefaultParameters.ContainsKey(effectiveDigits))
            //{
            //    throw new ArgumentException($"No default parameters for targetDigits = {effectiveDigits}");
            //}


            long effectiveB1 = B1 ?? defaults.B1;
            long effectiveB2 = B2 ?? defaults.B2;


            // Debug output to show the effective parameters used.
            //Console.WriteLine($"Running PM1 with parameters: B1 = {effectiveB1}, B2 = {effectiveB2}");

            // Implementation of the ECM algorithm would go here.

            var result = RunBigIntegerEcm(n, effectiveB1, effectiveB2, 0, false, algo: "-pm1");
            //var di = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "EcmTests"));
            //var output = Path.Combine(di.FullName, $"{n}-ecm-test.log");
            //File.WriteAllText(output, result.Output);

            FactorizationBigInteger f = new();
            try
            {
                f = ParseFactors(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
                Console.WriteLine($"Output: {result.Output}");
            }
            return f;
        }


        /// <summary>
        /// Runs the GMP-ECM factorization algorithm.
        /// If any of the optional parameters (B1, B2, or curves) are not specified,
        /// they are set to default values based on the expected digit count of the target factor.
        /// </summary>
        /// <param name="n">The number to factorize.</param>
        /// <param name="targetDigits">
        /// The expected number of digits of the factor.
        /// (Must be one of the keys defined in the DefaultParameters dictionary.)
        /// </param>
        /// <param name="B1">Optional optimal B1 parameter.</param>
        /// <param name="B2">Optional default B2 parameter.</param>
        /// <param name="curves">Optional expected number of curves.</param>
        /// <returns>A list of factors (implementation not provided).</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when targetDigits is not positive or when no defaults exist for the specified targetDigits.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Thrown because the actual ECM algorithm is not implemented.
        /// </exception>
        public FactorizationBigInteger ECM(BigInteger n, int? targetDigits = null, long? B1 = null, long? B2 = null, int? curves = null, bool? enableGpu = false)
        {
            (long B1, long B2, int Curves) defaults = GetDefaults(targetDigits, B1, ref enableGpu);

            long effectiveB1 = B1 ?? defaults.B1;
            long effectiveB2 = B2 ?? defaults.B2;
            int effectiveCurves = curves ?? defaults.Curves;

            // Debug output to show the effective parameters used.
            //Console.WriteLine($"Running ECM with parameters: B1 = {effectiveB1}, B2 = {effectiveB2}, Curves = {effectiveCurves}");

            // Implementation of the ECM algorithm would go here.

            var result = RunBigIntegerEcm(n, effectiveB1, effectiveB2, effectiveCurves, enableGpu.Value);
            //var di = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "EcmTests"));
            //var output = Path.Combine(di.FullName, $"{n}-ecm-test.log");
            //File.WriteAllText(output, result.Output);

            FactorizationBigInteger f = new();
            try
            {
                f = enableGpu.Value ? ParseGpuFactors(result) : ParseFactors(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
                Console.WriteLine($"Output: {result.Output}");
            }
            return f;
        }

        private static (long B1, long B2, int Curves) GetDefaults(int? targetDigits, long? B1, ref bool? enableGpu)
        {
            int effectiveDigits = targetDigits ?? 20;
            if (enableGpu is null)
            {
                enableGpu = false;
            }
            if (effectiveDigits <= 0)
            {
                throw new ArgumentException("targetDigits must be specified and greater than 0.");
            }


            var defaults = DefaultParameters.First().Value;



            if (B1.HasValue)
            {
                foreach (var kvp in DefaultParameters)
                {
                    if (kvp.Value.B1 == B1)
                    {
                        if (kvp.Key != effectiveDigits)
                        {
                            defaults = kvp.Value;
                        }
                        break;
                    }
                }
            }
            else
            {
                if (!DefaultParameters.ContainsKey(effectiveDigits))
                {
                    throw new ArgumentException($"No default parameters for targetDigits = {effectiveDigits}");
                }
                defaults = DefaultParameters[effectiveDigits];

            }

            return defaults;
        }


        // TODO: Need to dedupe factors and return a list of unique actual factors.
        private FactorizationBigInteger ParseFactors(ProcessResult result)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.

            var f = new FactorizationBigInteger();
            var factorParts = result.Output.Split("********** Factor found");
            if (factorParts.Length > 1)
            {
                for (var p = 1; p < factorParts.Length; p++)
                {
                    var partFactors = ParseFactorPart(factorParts[p]);
                    f.Add(partFactors);
                    if (p == factorParts.Length - 1)
                    {
                        var coFactor = ParseCoFactor(factorParts[p]);
                        f.Add(coFactor);
                    }
                }
            }
            return f;
        }

        private FactorizationBigInteger ParseGpuFactors(ProcessResult result)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.

            var f = new FactorizationBigInteger();

            var lines = result.Output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("GPU: factor ")).ToList();
            if (lines.Count > 0)
            {
                var uniqueFactors = new List<BigInteger>();
                var factors = lines.Select(x => x.Substring("GPU: factor ".Length).Split(' ').First());
                foreach (var factor in factors)
                {
                    if (BigInteger.TryParse(factor, out BigInteger b))
                    {
                        if (!uniqueFactors.Contains(b))
                        {
                            uniqueFactors.Add(b);
                        }
                    }
                }
                foreach (var item in uniqueFactors)
                {
                    f.Add(item, 1);
                }
            }



            return f;
        }


        private FactorizationBigInteger ParseCoFactor(string factorPart)
        {
            var result = new FactorizationBigInteger();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf("cofactor");
                if (idx > -1)
                {
                    var str = line.Substring(idx + "cofactor".Length + 1).Trim();
                    var header = line.Substring(0, idx).ToLower().Trim();


                    str = str.Split(" ")[0];
                    var f = BigInteger.Parse(str);
                    var factor = new Factor<BigInteger>(f, 1);
                    result.Add(f, 1);
                    switch (header)
                    {
                        case "composite":
                            factor.FactorType = MathLib.PrimalityType.Composite;
                            break;
                        case "prime":
                            factor.FactorType = MathLib.PrimalityType.Prime;
                            break;
                        case "probable prime":
                            factor.FactorType = MathLib.PrimalityType.ProbablePrime;
                            break;

                        default:
                            throw new Exception($"Unexpected header in factor output - {header}");
                    }
                }
            }
            return result;
        }

        private FactorizationBigInteger ParseFactorPart(string factorPart)
        {
            var result = new FactorizationBigInteger();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf(":");
                if (idx > -1)
                {
                    var str = line.Substring(idx + 1).Trim();
                    var f = BigInteger.Parse(str);
                    result.Add(f, 1);
                    break;
                }
            }
            return result;
        }


        private ProcessResult RunBigIntegerEcm(BigInteger n, long effectiveB1, long effectiveB2, int effectiveCurves, bool enableGpu, string algo = "")
        {
            // -I f increment B1 by f*sqrt(B1) on each run
            // docs suggest f=10 for B1 < 10^6, f=5 for B1 < 10^9, f=2 for B1 < 10^12
            /*
                pm1(n, b1*10);
                pp1(n, b1 * 5) 3 times
                ecm(n, b1);
                TLevel += 5 digits;
            */

            string gpuSwitch = enableGpu ? " -gpu" : "";
            // string b2Switch = effectiveB2 >0 ?  enableGpu ? "-gpu" : "";
            var exeName = !string.IsNullOrEmpty(gpuSwitch) ? "ecm_gpu.exe" : "ecm.exe";
            if (!string.IsNullOrEmpty(algo))
            {
                algo = $" {algo}";
            }
            string curveSwitch = enableGpu == false && effectiveCurves > 0 ? $" -c {effectiveCurves}" : "";

            //TODO: stage exe in stand-alone directory to allow multiple instances to run
            //var workingDirectory = @"E:\Source\Repos\NumTheory\msieve\HigginsSoft\gmp-ecm-alexhiggins732\bin\x64\Release";
            var exeFullName = Path.Combine(Path.GetFullPath("."), "binaries", exeName);
            var workingDirectory = Path.GetFullPath(".");
            var cmd = $"echo \"{n}\" | \"{exeFullName}\"{gpuSwitch}{algo}{curveSwitch} {effectiveB1} {effectiveB2}";

            //Console.WriteLine(cmd);
            //todo us DotMpi, for now use process helper

            bool waitForExit = enableGpu ? false : true;
            var result = ProcessHelper.RunProcess(cmd, workingDirectory, WaitForExit: waitForExit);
            return result;

        }

    }

    public class NumericsYafu : IDisposable
    {

        Process? factorProcess = null;
        FactorizationBigInteger GetFactorization(BigInteger n, FactorResult factorResult)
        {

            FactorizationBigInteger result = new();

            try
            {
                var factors = new List<string>();
                if (factorResult.factorsprime != null && factorResult.factorsprime.Length > 0)
                    factors.AddRange(factorResult.factorsprime);

                if (factorResult.factorscomposite != null && factorResult.factorscomposite.Length > 0)
                    factors.AddRange(factorResult.factorscomposite);

                if (factors.Count > 0)
                {
                    var bigN = n;

                    foreach (var factor in factors)
                    {
                        var bigFactor = BigInteger.Parse(factor);
                        if (bigFactor == n)
                            continue;
                        var f = new Factor<BigInteger>(bigFactor, 0);
                        using var fact = new FactorizationBigInteger();
                        while (bigN % f.P == 0)
                        {
                            bigN /= f.P;
                            f.Power++;
                        }
                        if (f.Power > 0)
                        {
                            fact.Factors.Add(f);
                        }

                        result.Add(fact);
                    }
                    if (bigN > 1)
                    {
                        result.Add(bigN, 0);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Run the Yafu QS factorization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public FactorizationBigInteger QS(BigInteger n)
        {
            FactorizationBigInteger f = new();
            try
            {
                var factorResult = Run(n, 0, 0, 0, false, 30, algo: FactorizationMethod.QS);
                f = GetFactorization(n, factorResult);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
            }
            if (f.Factors.Count < 1)
            {
                Console.WriteLine($"QS error: No factors found for {n}");
            }
            return f;
        }

        /// <summary>
        /// Run the Yafu QS factorization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public FactorizationBigInteger Factor(BigInteger n, int? tdiv)
        {
            if (tdiv == null)
                tdiv = 30;
            FactorizationBigInteger f = new();
            try
            {
                var factorResult = Run(n, 0, 0, 0, false, effectiveDigits: tdiv.Value, algo: FactorizationMethod.Fact);
                f = GetFactorization(n, factorResult);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing factors: {ex.Message}");
            }
            if (f.Factors.Count < 1)
            {
                Console.WriteLine($"QS error: No factors found for {n}");
            }
            return f;
        }


        /// <summary>
        /// Runs the GMP-ECM P+1 facotrization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="curves"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public FactorizationBigInteger PP1(BigInteger n, int? targetDigits = null, long? B1 = null, long? B2 = null)
        {
            var ecm = new NumericsEcm();
            return ecm.PP1(n, targetDigits, B1, B2);

        }

        /// <summary>
        /// Runs the GMP-ECM P-1 facotrization algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="curves"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public FactorizationBigInteger PM1(BigInteger n, int? targetDigits = null, long? B1 = null, long? B2 = null)
        {
            var ecm = new NumericsEcm();
            return ecm.PM1(n, targetDigits, B1, B2);
        }


        /// <summary>
        /// Runs the GMP-ECM factorization algorithm.
        /// If any of the optional parameters (B1, B2, or curves) are not specified,
        /// they are set to default values based on the expected digit count of the target factor.
        /// </summary>
        /// <param name="n">The number to factorize.</param>
        /// <param name="targetDigits">
        /// The expected number of digits of the factor.
        /// (Must be one of the keys defined in the DefaultParameters dictionary.)
        /// </param>
        /// <param name="B1">Optional optimal B1 parameter.</param>
        /// <param name="B2">Optional default B2 parameter.</param>
        /// <param name="curves">Optional expected number of curves.</param>
        /// <returns>A list of factors (implementation not provided).</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when targetDigits is not positive or when no defaults exist for the specified targetDigits.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Thrown because the actual ECM algorithm is not implemented.
        /// </exception>
        public FactorizationBigInteger ECM(BigInteger n, int? targetDigits = null, long? B1 = null, long? B2 = null, int? curves = null, bool? enableGpu = false)
        {
            var ecm = new NumericsEcm();
            return ecm.ECM(n, targetDigits, B1, B2, curves, enableGpu);

        }


        // TODO: Need to dedupe factors and return a list of unique actual factors.
        private FactorizationBigInteger ParseFactors(ProcessResult result)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.


            var factorParts = result.Output.Split("***factors found***");
            var f = new FactorizationBigInteger();

            if (factorParts.Length > 1)
            {
                for (var p = 1; p < factorParts.Length; p++)
                {
                    var partFactors = ParseFactorPart(factorParts[p]);
                    f.Add(partFactors);
                    if (p == factorParts.Length - 1)
                    {
                        var coFactor = ParseCoFactor(factorParts[p]);
                        f.Add(coFactor);
                    }
                }
            }
            return f;
        }

        private FactorizationBigInteger ParseCoFactor(string factorPart)
        {
            var result = new FactorizationBigInteger();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf("cofactor");
                if (idx > -1)
                {
                    var str = line.Substring(idx + "cofactor".Length + 1).Trim();
                    var header = line.Substring(0, idx).ToLower().Trim();


                    str = str.Split(" ")[0];
                    var f = BigInteger.Parse(str);
                    var factor = new Factor<BigInteger>(f, 1);
                    result.Add(f, 1);
                    switch (header)
                    {
                        case "composite":
                            factor.FactorType = MathLib.PrimalityType.Composite;
                            break;
                        case "prime":
                            factor.FactorType = MathLib.PrimalityType.Prime;
                            break;
                        case "probable prime":
                            factor.FactorType = MathLib.PrimalityType.ProbablePrime;
                            break;

                        default:
                            throw new Exception($"Unexpected header in factor output - {header}");
                    }
                }
            }
            return result;
        }

        private FactorizationBigInteger ParseFactorPart(string factorPart)
        {
            var result = new FactorizationBigInteger();
            var lines = factorPart.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var idx = line.IndexOf(" = ");
                if (idx > -1)
                {
                    var key = line.Substring(0, idx).Trim();
                    var str = line.Substring(idx + 3);
                    if (key == "ans" && str == "1")
                    {
                        break;
                    }


                    var f = BigInteger.Parse(str);
                    result.Add(f, 1);
                    //break;
                }
            }
            return result;
        }


        static FactorConfig config = FactorConfig.GetCommandLineConfig();
        private FactorResult Run(BigInteger n, long effectiveB1, long effectiveB2, int effectiveCurves, bool enableGpu, int effectiveDigits, string algo = "")
        {
            // -I f increment B1 by f*sqrt(B1) on each run
            // docs suggest f=10 for B1 < 10^6, f=5 for B1 < 10^9, f=2 for B1 < 10^12
            /*
                pm1(n, b1*10);
                pp1(n, b1 * 5) 3 times
                ecm(n, b1);
                TLevel += 5 digits;
            */

            string gpuSwitch = enableGpu ? " -gpu" : "";
            // string b2Switch = effectiveB2 >0 ?  enableGpu ? "-gpu" : "";

            string curveSwitch = effectiveCurves > 0 ? $"-c {effectiveCurves}" : "";

            //TODO: stage exe in stand-alone directory to allow multiple instances to run
            //var workingDirectory = @"E:\Source\Repos\NumTheory\msieve\HigginsSoft\gmp-ecm-alexhiggins732\bin\x64\Release";
            //var workingDirectory = Path.Combine(AppContext.BaseDirectory, "binaries");


            var arguments = "";
            if (algo == FactorizationMethod.QS)
            {
                arguments = $"siqs({n})";
            }
            else if (algo == FactorizationMethod.Fact)
            {
                arguments = $"factor({n}) -pretest {effectiveDigits}";
            }
            else
            {
                arguments = $"{algo}{gpuSwitch} {curveSwitch} {effectiveB1} {effectiveB2}";
            }
            var exeName = "yafu-x64.exe";
            var exeFullPath = Path.Combine(AppContext.BaseDirectory, "binaries", exeName);
            var cmd = $"\"{exeFullPath}\" {arguments}";
            //Console.WriteLine(cmd);
            //todo us DotMpi, for now use process helper
            var cwd = Path.GetFullPath(".");
            var factorJsonPath = Path.Combine(cwd, "factor.json");
            var factorLogPath = Path.Combine(cwd, "factor.log");
            if (algo == FactorizationMethod.Fact)
            {
                if (File.Exists(factorJsonPath))
                    File.Delete(factorJsonPath);

            }
            else if (algo == FactorizationMethod.QS)
            {
                if (File.Exists(factorLogPath))
                    File.Delete(factorLogPath);
            }

            //Console.WriteLine($"Starting process {cmd} in {cwd}");
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmd}",
                    WorkingDirectory = cwd
                }
            };
            factorProcess = process;



            var startTime = DateTime.Now;
            var sw = Stopwatch.StartNew();



            process.Start();
            ProcessHelper.SetProcessAffinity(process);
            process.WaitForExit();
            System.Threading.Thread.Sleep(1);
            sw.Stop();
            var endTime = DateTime.Now;
            if (process.ExitCode == 0)
            {
                if (algo == FactorizationMethod.Fact)
                {



                    var stringN = n.ToString();
                    using (var sr = new StreamReader(factorJsonPath))
                    {
                        var line = sr.ReadLine() ?? "";
                        try
                        {
                            var cleanedLine = line.Replace("{,", "{");
                            var factorResult = JsonSerializer.Deserialize<FactorResult>(cleanedLine);

                            if (factorResult.inputdecimal == stringN)
                                return factorResult;
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"Error parsing {factorJsonPath}\r\n{line}\r\n");
                            File.AppendAllText(factorJsonPath + ".error.txt", $"{n}\r\n{line}\r\n\r\n");
                        }

                        //Console.WriteLine(json);
                    }
                }
                else if (algo == FactorizationMethod.QS)
                {

                    var factorResult = new FactorResult();
                    var stringN = n.ToString();
                    var output = string.Empty;
                    using (var sr = new StreamReader(factorLogPath))
                    {


                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (line.Contains("starting", StringComparison.CurrentCultureIgnoreCase)
                               && line.Contains(stringN, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var sb = new StringBuilder();
                                while (!sr.EndOfStream)
                                {
                                    line = sr.ReadLine();
                                    sb.AppendLine(line);
                                    if (line.Contains("****************************"))
                                    {
                                        break;
                                    }
                                }
                                output = sb.ToString();
                                break;
                            }
                        }
                        var outputLines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        var linesWithoutTimestamps = outputLines.Select(x => x.Split(',')).Where(x => x.Length > 1).Select(x => x[1].Trim().Replace(" cofactor", "")).ToArray();
                        var factorLines = linesWithoutTimestamps.Where(x => (x.StartsWith("c") || x.StartsWith("p")) &&
                                x.Contains(" = ", StringComparison.CurrentCultureIgnoreCase) && !x.Contains(stringN));
                        var factorStrings = factorLines.Select(x => x.Substring(x.IndexOf(" = ") + 3).Trim()).ToArray();
                        List<string> factors = new();
                        foreach (var f in factorStrings)
                        {
                            if (BigInteger.TryParse(f, out var bigFactor))
                            {
                                factors.Add(f);
                            }
                        }

                        factorResult.inputdecimal = stringN;
                        factorResult.inputexpression = arguments;
                        factorResult.inputargumentstring = arguments;
                        factorResult.factorsprime = factors.ToArray();
                        factorResult.runtime = new Runtime { total = float.Parse(sw.Elapsed.TotalSeconds.ToString("N3")) };
                        factorResult.timestart = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                        factorResult.timeend = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                        factorResult.info = new Info
                        {
                            compiler = "MSVC 1934",
                            ECMversion = "7.0.6-dev",
                            MPIRversion = "3.0.0",
                            yafuversion = "2.11"
                        };
                        ;

                        //Console.WriteLine(json);
                    }
                    return factorResult;
                }
            }
            return new FactorResult { inputdecimal = n.ToString() }; // TODO: return the actual result from the process

        }

        public void Dispose()
        {
            if (factorProcess != null)
            {
                try
                {
                    if (!factorProcess.HasExited) factorProcess.Kill();
                }
                catch { }
                try
                {
                    factorProcess.Dispose();
                }
                catch { }
                factorProcess = null;

            }
        }
        /*
        {
          "input-expression": "factor(4577487232847279791)",
          "input-decimal": "4577487232847279791",
          "input-argument-string": "factor(4577487232847279791) ",
          "factors-prime": [
            "1393262831",
            "3285444161"
          ],
          "runtime": {
            "total": 0.029
          },
          "time-start": "2025-04-08  02:33:33",
          "time-end": "2025-04-08  02:33:33",
          "info": {
            "compiler": "MSVC 1934",
            "ECM-version": "7.0.6-dev",
            "MPIR-version": "3.0.0",
            "yafu-version": "2.11"
          }
        }  
        * */
        public class FactorResult
        {
            [JsonPropertyName("input-expression")]
            public string inputexpression { get; set; }
            [JsonPropertyName("input-decimal")]
            public string inputdecimal { get; set; }
            [JsonPropertyName("input-argument-string")]
            public string inputargumentstring { get; set; }
            [JsonPropertyName("factors-prime")]
            public string[] factorsprime { get; set; }
            [JsonPropertyName("factors-composite")]
            public string[] factorscomposite { get; set; }
            [JsonPropertyName("runtime")]
            public Runtime runtime { get; set; }
            [JsonPropertyName("time-start")]
            public string timestart { get; set; }
            [JsonPropertyName("time-end")]
            public string timeend { get; set; }
            [JsonPropertyName("info")]
            public Info info { get; set; }
        }

        public class Runtime
        {
            [JsonPropertyName("total")]
            public float total { get; set; }
        }

        public class Info
        {
            [JsonPropertyName("compiler")]
            public string compiler { get; set; }
            [JsonPropertyName("ECM-version")]
            public string ECMversion { get; set; }
            [JsonPropertyName("MPIR-version")]
            public string MPIRversion { get; set; }
            [JsonPropertyName("yafu-version")]
            public string yafuversion { get; set; }
        }

    }

    [SupportedOSPlatform("windows")]
    public class ProcessHelper
    {
        static FactorConfig config = FactorConfig.GetCommandLineConfig();
        public static ProcessResult RunProcess(string cmd, string workingDirectory,
          bool RedirectStandardOutput = true,
          bool RedirectStandardError = true,
          bool UseShellExecute = false,
          bool CreateNoWindow = true,
          bool WaitForExit = true)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmd}",
                    RedirectStandardOutput = RedirectStandardOutput,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }
            };
            process.StartInfo.CreateNoWindow = false;


            process.Start();

            SetProcessAffinity(process);
            if (WaitForExit) process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();



            var result = new ProcessResult
            {
                Output = output,
                Error = error,
                ExitCode = process.ExitCode
            };
            return result;


        }

        public static void SetProcessAffinity(Process process, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            if (config.ProcessorIndex != null)
            {
                var idx = config.ProcessorIndex.Value % Environment.ProcessorCount;
                if (idx < 0) // || idx > Environment.ProcessorCount) <-- bug some cpus are returning 0 processors
                {
                    throw new ArgumentOutOfRangeException($"ProcessorIndex {idx} is out of range. Must be between >0)");
                }
                Console.WriteLine("Setting Processor Affinity to " + idx + " from " + config.ProcessorIndex);
                process.ProcessorAffinity = (IntPtr)(1L << idx);
                process.PriorityClass = priority;
                //Console.WriteLine($"Set process {process.Id} affinity to processor " + config.ProcessorIndex.Value);
            }
            else
            {
                //Console.WriteLine("Skipping Set Process Affinity - Process affinity not set.");
            }

        }
    }

    public class ProcessResult
    {
        public string Output { get; set; }
        public string Error { get; set; }
        public int ExitCode { get; set; }
    }
}
