using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using System;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
    public class DbRoleMapper : IDbRoleMapper
    {
        public DbRole Map(CreateRoleRequest value, Guid userId)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var roleId = Guid.NewGuid();

            return new DbRole
            {
                Id = roleId,
                Name = value.Name,
                Description = value.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                Rights = value.Rights?.Select(x => new DbRoleRight
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    RightId = x,
                    RoleId = roleId
                }).ToList()
            };
        }
    }
}