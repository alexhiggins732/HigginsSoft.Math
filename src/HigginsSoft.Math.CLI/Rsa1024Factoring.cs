/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using HigginsSoft.Math.Lib;
using HigginsSoft.Math.Lib.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static HigginsSoft.Math.Lib.MathLib;
using PrimalityType = HigginsSoft.Math.Lib.Database.PrimalityType;

namespace HigginsSoft.Math.CLI
{
    public class Rsa1024Factoring
    {
        public static void Run(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<FactorDbContext>(options => options.UseSqlServer("Server=localhost;Database=Factors;AttachDbFilename=E:\\sql\\Factors.mdf;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;"));

            var provider = services.BuildServiceProvider();


            var arguments = RsaFactoringArgs.ParseRsaFactoringArgs(args);

        
            var ecm = new GmpEcm();
            Func<GmpInt, Factorization> factorize = null!;
            switch (arguments.Algo)
            {
                case FactorAlgorithm.TDiv:
                    factorize = (factor) => Factorization.FactorTrialDivide(factor, arguments.B1.HasValue ? (int)arguments.B1.Value : 99999);
                    break;
                case FactorAlgorithm.PM1:
                    factorize = (factor) => ecm.PM1(factor, arguments.TDiv.HasValue ? arguments.TDiv.Value : 20, arguments.B1, arguments.B2);
                    break;
                case FactorAlgorithm.PP1:
                    factorize = (factor) => ecm.PP1(factor, arguments.TDiv.HasValue ? arguments.TDiv.Value : 20, arguments.B1, arguments.B2);
                    break;
                case FactorAlgorithm.Ecm:
                    factorize = (factor) => ecm.ECM(factor, arguments.TDiv.HasValue ? arguments.TDiv.Value : 20, arguments.B1, arguments.B2, arguments.NumCurves, arguments.UseGpu);
                    break;
                default:
                    throw new Exception("Invalid arguments");
            }

            while (true)
            {
                using var app = provider.CreateScope();
                using var dbContext = app.ServiceProvider.GetRequiredService<FactorDbContext>();

                var dbFact = dbContext.Factorizations.Include(x => x.Factors)
                    .FirstOrDefault(x => x.TDiv < 5 && (x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite));
                if (dbFact == null)
                    break;

                var unfactored = dbFact.Factors.Where(x => x.Type == PrimalityType.Unknown || x.Type == PrimalityType.Composite).ToList();

                foreach (var unfactor in unfactored)
                {
                    if ((int)unfactor.Type > 0)
                        continue;

                    var factor = new GmpInt(unfactor.P);
                    var factorization = factorize(factor);
                    if (factorization.Factors.Count == 1)
                    {
                        dbFact.TDiv = 6;
                        dbContext.SaveChanges();
                        continue;
                    }
                    else
                    {
                        dbFact.TDiv = 6;
                        dbFact.Factors.Remove(unfactor);
                        dbFact.Type = factorization.Factors.All(x => (int)x.P.Primality() > 0) ? PrimalityType.ProbablePrime : PrimalityType.Composite;
                        dbFact.Factors.AddRange(factorization.Factors.Select(f =>
                                    new DbFactor
                                    {
                                        P = f.P.ToString(),
                                        Power = f.Power,
                                        Type =  (PrimalityType)f.P.Primality(),
                                        Digits = f.P.ToString().Length,
                                        Bits = MathLib.BitLength(f.P)
                                    }
                            ));

                        dbContext.SaveChanges();

                        var t = factorization.Factors.Select(x => x.P.Primality());
                    }

                }
                System.Diagnostics.Debug.WriteLine($"Factored {dbFact.Id}");


            }
        }
    }

    public class RsaFactoringArgs
    {
        public FactorAlgorithm Algo { get; set; }
        public int? TDiv { get; set; }
        public long? B1 { get; set; }
        public long? B2 { get; set; }
        public int NumCurves { get; set; }
        public bool UseGpu { get; set; }

        internal static RsaFactoringArgs ParseRsaFactoringArgs(string[] args)
        {
            var result = new RsaFactoringArgs();
            return result;
        }
    }

    public enum FactorAlgorithm
    {
        Unknown = 0,
        TDiv = 1,
        PM1 = 2,
        PP1 = 3,
        Ecm = 4

    }
}