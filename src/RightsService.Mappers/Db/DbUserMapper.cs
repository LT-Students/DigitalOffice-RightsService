using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
  public class DbUserMapper : IDbUserMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbUser Map(Guid userId, Guid? roleId, Guid createdBy)
    {
      return new DbUser
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        RoleId = roleId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = createdBy,
        IsActive = true
      };
    }

    public DbUser Map(Guid userId, Guid? roleId)
    {
      return new DbUser
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        RoleId = roleId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }
  }
}
