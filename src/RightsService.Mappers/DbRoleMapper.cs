using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
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

            // TODO add add rights and users

            return new DbRole
            {
                Id = Guid.NewGuid(),
                Name = value.Name,
                Description = value.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                Rights = null,
                Users = null
            };
        }
    }
}