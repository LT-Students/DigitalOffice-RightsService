using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.RightsService.Models.Db
{
    public class DbRightsLocalization
    {
        public const string TableName = "RightsLocalization";

        public Guid Id { get; set; }
        public int RightId { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DbRightLocalizationConfiguration : IEntityTypeConfiguration<DbRightsLocalization>
    {
        public void Configure(EntityTypeBuilder<DbRightsLocalization> builder)
        {
            builder
                .ToTable(DbRightsLocalization.TableName);

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
                .Property(x => x.Description)
                .IsRequired();
        }
    }
}
