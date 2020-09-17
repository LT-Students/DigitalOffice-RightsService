using LT.DigitalOffice.CheckRightsService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LT.DigitalOffice.CheckRightsService.Database
{
    /// <summary>
    /// A class that defines the tables and its properties in the database of CheckRightsService.
    /// </summary>
    public class CheckRightsServiceDbContext : DbContext
    {
        public DbSet<DbRight> Rights { get; set; }
        public DbSet<DbRightUser> RightUsers { get; set; }

        public CheckRightsServiceDbContext(DbContextOptions<CheckRightsServiceDbContext> options) : base(options) { }

        // Fluent API is written here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}