using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;

namespace LT.DigitalOffice.RightsService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IRoleResponseMapper
  {
    RoleResponse Map(DbRole role, List<DbRightsLocalization> rights, List<UserData> users);
  }
}
