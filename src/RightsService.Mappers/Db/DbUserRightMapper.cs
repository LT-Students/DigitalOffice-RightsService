using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
  public class DbUserRightMapper : IDbUserRightMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbUserRightMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbUserRight Map(Guid userId, int rightId)
    {
      return new DbUserRight
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        RightId = rightId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId()
      };
    }
  }
}
