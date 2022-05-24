using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
  public class DbUserRole
  {
    public const string TableName = "UsersRoles";
    public const string HistoryTableName = "UsersRolesHistory";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid CreatedBy { get; set; }
    public bool IsActive { get; set; }

    public DbRole Role { get; set; }
  }

  public class DbUserConfiguration : IEntityTypeConfiguration<DbUserRole>
  {
    public void Configure(EntityTypeBuilder<DbUserRole> builder)
    {
      builder
        .ToTable(
          DbUserRole.TableName,
          ur => ur.IsTemporal(builder =>
            builder.UseHistoryTable(DbUserRole.HistoryTableName)));

      builder
        .HasKey(u => u.Id);

      builder
        .HasOne(u => u.Role)
        .WithMany(r => r.Users);
    }
  }
}
