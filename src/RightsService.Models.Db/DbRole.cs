using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Db
{
    public class DbRole
    {
        public const string TableName = "Roles";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DbRoleRight> Rights { get; set; }
        public ICollection<DbUser> Users { get; set; }

        public DbRole()
        {
            Users = new HashSet<DbUser>();
            Rights = new HashSet<DbRoleRight>();
        }
    }

    public class DbRoleConfiguration : IEntityTypeConfiguration<DbRole>
    {
        public void Configure(EntityTypeBuilder<DbRole> builder)
        {
            builder
                .ToTable(DbRole.TableName);

            builder
                .HasKey(r => r.Id);

            builder
                .Property(r => r.Name)
                .IsRequired();

            builder
                .HasMany(r => r.Rights)
                .WithOne(u => u.Role);

            builder
                .HasMany(r => r.Users)
                .WithOne(u => u.Role);
        }
    }
}
