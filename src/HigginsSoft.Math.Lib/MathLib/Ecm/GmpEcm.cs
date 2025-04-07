using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        private static readonly Dictionary<int, (long B1, long B2, int Curves)> DefaultParameters =
             new Dictionary<int, (long B1, long B2, int Curves)>
             {
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
        private Factorization ParseFactors(ProcessResult result)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.


            var factorParts = result.Output.Split("********** Factor found");
            var f = new Factorization();

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

        private Factorization ParseCoFactor(string factorPart)
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

        private Factorization ParseFactorPart(string factorPart)
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

            string gpuSwitch = enableGpu ? "-gpu" : "";
            string b2Switch = enableGpu ? "-gpu" : "";
            var exeName = "ecm_gpu.exe";
            if (!string.IsNullOrEmpty(algo))
            {
                algo = $" {algo}";
            }

            //TODO: stage exe in stand-alone directory to allow multiple instances to run
            var workingDirectory = @"E:\Source\Repos\NumTheory\msieve\HigginsSoft\gmp-ecm-alexhiggins732\bin\x64\Release";
            var cmd = $"echo \"{n}\" | {exeName} {gpuSwitch}{algo} -c {effectiveCurves} {effectiveB1} {effectiveB2}";

            //todo us DotMpi, for now use process helper
            var result = ProcessHelper.RunProcess(cmd, workingDirectory);
            return result;

        }

    }

    public class NumericsEcm
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
        public FactorizationBigInteger PP1(BigInteger n, int targetDigits = 20, long? B1 = null, long? B2 = null)
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
        public FactorizationBigInteger PM1(BigInteger n, int targetDigits = 20, long? B1 = null, long? B2 = null)
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
        private static readonly Dictionary<int, (long B1, long B2, int Curves)> DefaultParameters =
             new Dictionary<int, (long B1, long B2, int Curves)>
             {
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
        public FactorizationBigInteger ECM(BigInteger n, int targetDigits = 20, long? B1 = null, long? B2 = null, int? curves = null, bool? enableGpu = false)
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

            var result = RunBigIntegerEcm(n, effectiveB1, effectiveB2, effectiveCurves, enableGpu.Value);
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


        // TODO: Need to dedupe factors and return a list of unique actual factors.
        private FactorizationBigInteger ParseFactors(ProcessResult result)
        {
            var factorization = new Factorization();
            // Parse the output of the GMP-ECM process to extract the factors.


            var factorParts = result.Output.Split("********** Factor found");
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
                    var f =BigInteger.Parse(str);
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

        // TODO: Need to dedupe factors and return a list of unique actual factors.
        private List<Factor<BigInteger>> ParseFactors1(ProcessResult result)
        {
            var factors = new List<Factor<BigInteger>>();
            // Parse the output of the GMP-ECM process to extract the factors.

            Action<string> AddFactor = (str) =>
            {
                var f = BigInteger.Parse(str);
                var factor = factors.FirstOrDefault(x => x.P == f);
                if (factor != null)
                    factor.Power++;
                else
                {
                    factor = new Factor<BigInteger>(f, 1);
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

            string gpuSwitch = enableGpu ? "-gpu" : "";
           // string b2Switch = effectiveB2 >0 ?  enableGpu ? "-gpu" : "";
            var exeName = "ecm_gpu.exe";
            if (!string.IsNullOrEmpty(algo))
            {
                algo = $" {algo}";
            }
            string curveSwitch =  effectiveCurves> 0 ? $"-c {effectiveCurves}" : "";

            //TODO: stage exe in stand-alone directory to allow multiple instances to run
            //var workingDirectory = @"E:\Source\Repos\NumTheory\msieve\HigginsSoft\gmp-ecm-alexhiggins732\bin\x64\Release";
            var workingDirectory = Path.Combine(AppContext.BaseDirectory, "binaries");
            var cmd = $"echo \"{n}\" | {exeName} {gpuSwitch}{algo} {curveSwitch} {effectiveB1} {effectiveB2}";

            //Console.WriteLine(cmd);
            //todo us DotMpi, for now use process helper
            var result = ProcessHelper.RunProcess(cmd, workingDirectory);
            return result;

        }

    }

    public class ProcessHelper
    {
        public static ProcessResult RunProcess(string cmd, string workingDirectory)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmd}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            var result = new ProcessResult
            {
                Output = output,
                Error = error,
                ExitCode = process.ExitCode
            };
            return result;


        }
    }
    public class ProcessResult
    {
        public string Output { get; set; }
        public string Error { get; set; }
        public int ExitCode { get; set; }
    }
}
