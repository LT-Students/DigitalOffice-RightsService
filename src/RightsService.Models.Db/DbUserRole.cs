using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
  public class DbUserRole
  {
    public const string TableName = "UsersRoles";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public bool IsActive { get; set; }

    public ICollection<DbUserRight> Rights { get; set; }
    public DbRole Role { get; set; }

    public DbUserRole()
    {
      Rights = new HashSet<DbUserRight>();
    }
  }

  public class DbUserConfiguration : IEntityTypeConfiguration<DbUserRole>
  {
    public void Configure(EntityTypeBuilder<DbUserRole> builder)
    {
      builder
        .ToTable(DbUserRole.TableName);

      builder
        .HasKey(u => u.Id);

      builder
        .HasMany(u => u.Rights)
        .WithOne(r => r.User);

      builder
        .HasOne(u => u.Role)
        .WithMany(u => u.Users);
    }
  }
}
