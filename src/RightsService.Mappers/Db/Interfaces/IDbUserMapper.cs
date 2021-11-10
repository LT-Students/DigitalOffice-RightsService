using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;

namespace LT.DigitalOffice.RightsService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserMapper
  {
    DbUser Map(Guid userId, Guid? roleId, Guid createdBy);
    DbUser Map(Guid userId, Guid? roleId);
  }
}
