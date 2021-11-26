using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
  public class DbRoleRight
  {
    public const string TableName = "RoleRights";

    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int RightId { get; set; }

    public DbRole Role { get; set; }
  }

  public class DbRoleRightConfiguration : IEntityTypeConfiguration<DbRoleRight>
  {
    public void Configure(EntityTypeBuilder<DbRoleRight> builder)
    {
      builder
        .HasKey(r => r.Id);

      builder
        .HasOne(ru => ru.Role)
        .WithMany(u => u.RoleRights);
    }
  }
}
