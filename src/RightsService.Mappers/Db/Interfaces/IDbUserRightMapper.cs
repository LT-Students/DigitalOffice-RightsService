using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;

namespace LT.DigitalOffice.RightsService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserRightMapper
  {
    DbUserRight Map(Guid userId, int rightId);
  }
}
