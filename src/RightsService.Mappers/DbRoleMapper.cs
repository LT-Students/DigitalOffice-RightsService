using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
    public class DbRoleMapper : IDbRoleMapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbRoleMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbRole Map(CreateRoleRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var roleId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            Guid creatorId = _httpContextAccessor.HttpContext.GetUserId();

            return new DbRole
            {
                Id = roleId,
                Name = request.Name,
                Description = request.Description,
                CreatedBy = creatorId,
                CreatedAt = createdAt,
                IsActive = true,
                Rights = request.Rights?.Select(x => new DbRoleRight
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    CreatedBy = creatorId,
                    CreatedAt = createdAt,
                    RightId = x,
                }).ToList()
            };
        }
    }
}