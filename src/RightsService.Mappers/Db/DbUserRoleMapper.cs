using System;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Right;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
  public class DbUserRoleMapper : IDbUserRoleMapper
  {
    public DbUserRole Map(ICreateUserRolePublish request)
    {
      return new DbUserRole
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        RoleId = request.RoleId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = request.ChangedBy,
        IsActive = true
      };
    }
  }
}
