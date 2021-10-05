using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
  public class DbRoleLocalizationMapper : IDbRoleLocalizationMapper
  {
    private IHttpContextAccessor _httpContextAccessor;

    public DbRoleLocalizationMapper(
      IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbRoleLocalization Map(CreateRoleLocalizationRequest request)
    {
      if (request == null)
      {
        return null;
      }

      return new DbRoleLocalization
      {
        Id = Guid.NewGuid(),
        RoleId = request.RoleId.Value,
        Locale = request.Locale,
        Name = request.Name,
        Description = request.Description,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
        IsActive = true
      };
    }

    public DbRoleLocalization Map(CreateRoleLocalizationRequest request, Guid roleId)
    {
      if (request == null)
      {
        return null;
      }

      return new DbRoleLocalization
      {
        Id = Guid.NewGuid(),
        RoleId = roleId,
        Locale = request.Locale,
        Name = request.Name,
        Description = request.Description,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow,
        IsActive = true
      };
    }
  }
}
