using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Right;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;

namespace LT.DigitalOffice.RightsService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserRoleMapper
  {
    DbUserRole Map(ICreateUserRolePublish request);

    DbUserRole Map(EditUserRoleRequest request);
  }
}
