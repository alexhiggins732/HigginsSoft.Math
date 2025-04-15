using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HigginsSoft.Math.Lib.Database
{
    using static DbConstants;
    public class FactorDbContext : DbContext
    {


        public FactorDbContext() { }

        public FactorDbContext(DbContextOptions<FactorDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            //base.ConfigureConventions(configurationBuilder);
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
        }

        public static string DbConnectionString { get; set; } =
            "Server=localhost;Database=Factors;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Command Timeout=300";


        public DbSet<DbFactorization> Factorizations { get; set; }

        public DbSet<DbFactor> Factors { get; set; }
        public DbSet<FactorQueue> FactorQueue { get; set; }
    }

    public class BaseEntity
    {
        public int Id { get; set; }
    }

    public class DbFactorization : BaseEntity
    {
        public int Offset { get; set; }
        public string N { get; set; }
        public int Digits { get; set; }
        public int Bits { get; set; }
        public int TDiv { get; set; } = 0;
        public PrimalityType Type { get; set; } = PrimalityType.Unknown;
        public List<DbFactor> Factors { get; set; } = new();
    }

    public class DbFactor : BaseEntity
    {
        public string P { get; set; }
        public int Power { get; set; }
        public PrimalityType Type { get; set; }
        public int Digits { get; set; }
        public int Bits { get; set; }
    }

    public class DbConstants
    {
        public const int PRP_ERROR = -1;
        public const int PRP_COMPOSITE = 0;
        public const int PRP_PRP = 1;
        public const int PRP_PRIME = 2;
    }

    public enum PrimalityType
    {
        Error = PRP_ERROR,
        Unknown = -2,
        Composite = PRP_COMPOSITE,
        ProbablePrime = PRP_PRP,
        Prime = PRP_PRIME,
    }

    public class FactorQueue
    {
        public int Id { get; set; }
        public int FactorizationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; } = false;

        public string Prime { get; set; } = null!;
    }
}
