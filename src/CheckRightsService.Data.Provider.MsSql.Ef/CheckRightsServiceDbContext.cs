using LT.DigitalOffice.CheckRightsService.Data.Provider;
using LT.DigitalOffice.CheckRightsService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LT.DigitalOffice.CheckRightsService.Database
{
    /// <summary>
    /// A class that defines the tables and its properties in the database of CheckRightsService.
    /// </summary>
    public class CheckRightsServiceDbContext : DbContext, IDataProvider
    {
        public DbSet<DbRight> Rights { get; set; }
        public DbSet<DbRightUser> RightUsers { get; set; }

        public CheckRightsServiceDbContext(DbContextOptions<CheckRightsServiceDbContext> options) : base(options) { }

        // Fluent API is written here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        void IDataProvider.SaveChanges()
        {
            SaveChanges();
        }

        public object MakeEntityDetached(object obj)
        {
            Entry(obj).State = EntityState.Detached;

            return Entry(obj).State;
        }

        public void EnsureDeleted()
        {
            Database.EnsureDeleted();
        }

        public bool IsInMemory()
        {
            return Database.IsInMemory();
        }
    }
}