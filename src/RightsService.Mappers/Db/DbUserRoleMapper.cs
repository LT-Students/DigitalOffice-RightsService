using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Right;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
  public class DbUserRoleMapper : IDbUserRoleMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserRoleMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

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

    public DbUserRole Map(EditUserRoleRequest request)
    {
      return new DbUserRole
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        RoleId = request.RoleId.Value,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }
  }
}
