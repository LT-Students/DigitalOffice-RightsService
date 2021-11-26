using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
  public class DbRole
  {
    public const string TableName = "Roles";
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }

    public ICollection<DbRoleLocalization> RoleLocalizations { get; set; }
    public ICollection<DbRoleRight> RoleRights { get; set; }
    public ICollection<DbUser> Users { get; set; }

    public DbRole()
    {
      RoleLocalizations = new HashSet<DbRoleLocalization>();
      RoleRights = new HashSet<DbRoleRight>();
      Users = new HashSet<DbUser>();
    }
  }

  public class DbRoleConfiguration : IEntityTypeConfiguration<DbRole>
  {
    public void Configure(EntityTypeBuilder<DbRole> builder)
    {
      builder
        .ToTable(DbRole.TableName);

      builder
        .HasKey(x => x.Id);

      builder
        .HasMany(r => r.RoleLocalizations)
        .WithOne(rl => rl.Role);

      builder
        .HasMany(r => r.RoleRights)
        .WithOne(rl => rl.Role);

      builder
        .HasMany(r => r.Users)
        .WithOne(u => u.Role);
    }
  }
}
