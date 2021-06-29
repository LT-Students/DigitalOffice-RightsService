using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Mappers
{
    public class RoleInfoMapper : IRoleInfoMapper
    {
        public RoleInfo Map(DbRole value, List<RightResponse> rights, List<UserInfo> users)
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
                Rights = rights,
                Users = users
            };
        }
    }
}