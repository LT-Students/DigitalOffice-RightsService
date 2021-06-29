using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Mappers.Interfaces
{
    [AutoInject]
    public interface IRoleInfoMapper
    {
        RoleInfo Map(DbRole value, IEnumerable<RightResponse> rights, IEnumerable<UserInfo> users);
    }
}