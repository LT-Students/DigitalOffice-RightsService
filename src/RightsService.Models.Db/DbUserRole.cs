using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.RightsService.Models.Db
{
    public class DbUserRole
    {
        public const string TableName = "UserRoles";

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? RemovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public Guid RoleId { get; set; }
        public DbRole Role { get; set; }
        public DbUser User { get; set; }
    }

    public class DbRoleUserConfiguration : IEntityTypeConfiguration<DbUserRole>
    {
        public void Configure(EntityTypeBuilder<DbUserRole> builder)
        {
            builder
                .HasKey(r => r.Id);

            builder
                .HasOne(ru => ru.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(ru => ru.RoleId);

            builder
                .HasOne(ru => ru.User)
                .WithOne(r => r.Role);
        }
    }
}