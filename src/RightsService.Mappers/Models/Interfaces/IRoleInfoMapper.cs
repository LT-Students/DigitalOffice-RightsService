using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IRoleInfoMapper
  {
    RoleInfo Map(
      DbRole dbRole,
      List<RightInfo> rights,
      List<UserInfo> userInfos);
  }
}
