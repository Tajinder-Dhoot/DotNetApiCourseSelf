using _1_HelloWorld_Database.Models;
using Microsoft.EntityFrameworkCore;

namespace _1_HelloWorld_Database
{
    public class DataContextEF: DbContext
    {
        public DbSet<Computer>? Computer {get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                string connectionString = "Server=localhost;Database=DotNetCourseDatabase;Trusted_Connection=false;TrustServerCertificate=True;User Id=;Password=;";
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