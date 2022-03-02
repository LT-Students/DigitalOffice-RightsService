using System.Reflection;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef
{
  public class RightsServiceDbContext : DbContext, IDataProvider
  {
    public DbSet<DbRightLocalization> RightsLocalizations { get; set; }
    public DbSet<DbRole> Roles { get; set; }
    public DbSet<DbRoleLocalization> RolesLocalizations { get; set; }
    public DbSet<DbRoleRight> RoleRights { get; set; }
    public DbSet<DbUserRole> Users { get; set; }
    public DbSet<DbUserRight> UsersRights { get; set; }

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

    public async Task SaveAsync()
    {
      await SaveChangesAsync();
    }
  }
}
