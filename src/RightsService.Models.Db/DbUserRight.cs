using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
  public class DbUserRight
  {
    public const string TableName = "UserRights";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int RightId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public DbUser User { get; set; }
  }

  public class DbRightUserConfiguration : IEntityTypeConfiguration<DbUserRight>
  {
    public void Configure(EntityTypeBuilder<DbUserRight> builder)
    {
      builder
        .ToTable(DbUserRight.TableName);

      builder
        .HasKey(r => r.Id);

      builder
        .HasOne(ru => ru.User)
        .WithMany(u => u.Rights)
        .HasForeignKey(ur => ur.UserId);
    }
  }
}
