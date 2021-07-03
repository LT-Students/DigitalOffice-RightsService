using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.Kernel.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef
{
    public class RightsServiceDbContext : DbContext, IDataProvider
    {
        public DbSet<DbRight> Rights { get; set; }
        public DbSet<DbUserRight> RightUsers { get; set; }
        public DbSet<DbRole> Roles { get; set; }
        public DbSet<DbUserRole> UserRoles { get; set; }
        public DbSet<DbRoleRight> RoleRights { get; set; }
        public DbSet<DbUser> Users { get; set; }

        public RightsServiceDbContext(DbContextOptions<RightsServiceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("LT.DigitalOffice.RightsService.Models.Db"));
        }

        public object MakeEntityDetached(object obj)
        {
            Entry(obj).State = EntityState.Detached;

            return Entry(obj).State;
        }

        void IBaseDataProvider.Save()
        {
            SaveChanges();
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