using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
  public class DbRoleLocalization
  {
    public const string TableName = "RolesLocalizations";

    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public bool IsActive { get; set; }

    public DbRole Role { get; set; }
  }

  public class DbRoleLocalizationConfiguration : IEntityTypeConfiguration<DbRoleLocalization>
  {
    public void Configure(EntityTypeBuilder<DbRoleLocalization> builder)
    {
      builder
        .ToTable(DbRoleLocalization.TableName);

      builder
        .HasKey(r => r.Id);

      builder
        .Property(r => r.Locale)
        .HasMaxLength(2)
        .IsFixedLength()
        .IsRequired();

      builder
        .Property(r => r.Name)
        .IsRequired();

      builder
        .HasOne(r => r.Role)
        .WithMany(u => u.RoleLocalizations);
    }
  }
}
