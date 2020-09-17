using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.CheckRightsService.Database.Entities
{
    public class DbRightUser
    {
        public Guid UserId { get; set; }
        public int RightId { get; set; }
        public DbRight Right { get; set; }
    }

    public class CompanyUserConfiguration : IEntityTypeConfiguration<DbRightUser>
    {
        public void Configure(EntityTypeBuilder<DbRightUser> builder)
        {
            builder.HasKey(right => new { right.UserId, right.RightId });

            builder.HasOne(userRight => userRight.Right)
                .WithMany(right => right.RightUsers)
                .HasForeignKey(user => user.RightId);
        }
    }
}