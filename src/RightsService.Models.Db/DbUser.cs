using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Db
{
    public class DbUser
    {
        public const string TableName = "Users";

        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DbUserRight> Rights { get; set; }
        public DbRole Role { get; set; }

        public DbUser()
        {
            Rights = new HashSet<DbUserRight>();
        }
    }

    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder
                .ToTable(DbUser.TableName);

            builder
                .HasKey(u => u.Id);

            builder
                .HasMany(u => u.Rights)
                .WithOne(r => r.User);

            builder
                .HasOne(u => u.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.RoleId);
        }
    }
}
