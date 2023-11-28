using _1_HelloWorld_Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace _1_HelloWorld_Database
{
    public class DataContextEF: DbContext
    {
        private IConfiguration _connectionString;

        public DbSet<Computer>? Computer {get; set; }

        public DataContextEF(IConfiguration config)
        {
            _connectionString = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                string? connectionString = _connectionString.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString,
                    options => options.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");
            modelBuilder.Entity<Computer>()
                .HasKey(c => c.ComputerId);

            // This can also be done as
            // modelBuilder.Entity<Computer>()
            //     .ToTable("Computer", "TutorialAppSchema");
        }
    }
}