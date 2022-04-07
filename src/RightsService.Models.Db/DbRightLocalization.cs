using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.RightsService.Models.Db
{
    public class DbRightLocalization
    {
        public const string TableName = "RightsLocalizations";

        public Guid Id { get; set; }
        public int RightId { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DbRightLocalizationConfiguration : IEntityTypeConfiguration<DbRightLocalization>
    {
        public void Configure(EntityTypeBuilder<DbRightLocalization> builder)
        {
            builder
                .ToTable(DbRightLocalization.TableName);

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
