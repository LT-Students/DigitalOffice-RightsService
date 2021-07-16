using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Db
{
    public class DbRight
    {
        public const string TableName = "Rights";

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<DbUserRight> Users { get; set; }
        public ICollection<DbRoleRight> Roles { get; set; }

        public DbRight()
        {
            Users = new HashSet<DbUserRight>();
            Roles = new HashSet<DbRoleRight>();
        }
    }

    public class DbRightConfiguration : IEntityTypeConfiguration<DbRight>
    {
        public void Configure(EntityTypeBuilder<DbRight> builder)
        {
            builder
                .ToTable(DbRight.TableName);

            builder
                .HasKey(r => r.Id);

            builder
                .Property(r => r.Name)
                .IsRequired();

            builder
                .HasMany(r => r.Users)
                .WithOne(u => u.Right);

            builder
                .HasMany(r => r.Roles)
                .WithOne(u => u.Right);
        }
    }
}
