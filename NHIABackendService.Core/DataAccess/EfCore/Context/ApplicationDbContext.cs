using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NHIABackendService.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NHIABackendService.Core.DataAccess.EfCore.Context
{
    [ExcludeFromCodeCoverage]
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<FileUpload> FileUploads { get; set; }

        //Custom DbSets Added Below
        #region Add DbSets
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

    [ExcludeFromCodeCoverage]
    public class AppDbContextMigrationFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public static readonly IConfigurationRoot ConfigBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile("appsettings.Development.json", false).Build();

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConfigBuilder.GetConnectionString("Default"),
                 b => b.MigrationsAssembly(GetType().Assembly.FullName).EnableRetryOnFailure()
                    ).Options);
        }
    }
}
