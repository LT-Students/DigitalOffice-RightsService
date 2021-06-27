using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Mappers
{
    public class RoleInfoMapper : IRoleInfoMapper
    {
        public RoleInfo Map(DbRole value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new RoleInfo
            {
                Id = value.Id,
                Name = value.Name,
                Description = value.Description,
                CreatedAt = value.CreatedAt,
                CreatedBy = value.CreatedBy,
                Rights = value.Rights.Select(x => x.Id).ToList(),
                Users = value.Users.Select(x => x.Id).ToList()
            };
        }
    }
}