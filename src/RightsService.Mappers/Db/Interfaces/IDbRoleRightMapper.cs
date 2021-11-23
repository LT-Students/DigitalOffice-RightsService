using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;

namespace LT.DigitalOffice.RightsService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbRoleRightMapper
  {
    List<DbRoleRight> Map(Guid roleId, List<int> rightsIds);
  }
}
