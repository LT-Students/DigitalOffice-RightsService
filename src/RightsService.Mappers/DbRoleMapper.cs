using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using System;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
    public class DbRoleMapper : IDbRoleMapper
    {
        public DbRole Map(CreateRoleRequest request, Guid userId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var roleId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            return new DbRole
            {
                Id = roleId,
                Name = request.Name,
                Description = request.Description,
                CreatedBy = userId,
                CreatedAt = createdAt,
                IsActive = true,
                Rights = request.Rights?.Select(x => new DbRoleRight
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    CreatedBy = userId,
                    CreatedAt = createdAt,
                    RightId = x,
                }).ToList()
            };
        }
    }
}