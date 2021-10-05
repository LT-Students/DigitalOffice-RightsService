using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Mappers.Db
{
  public class DbRoleMapper : IDbRoleMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbRoleLocalizationMapper _localizationMapper;

    public DbRoleMapper(
      IHttpContextAccessor httpContextAccessor,
      IDbRoleLocalizationMapper localizationMapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _localizationMapper = localizationMapper;
    }

    public DbRole Map(CreateRoleRequest request)
    {
      if (request == null)
      {
        return null;
      }

      var roleId = Guid.NewGuid();
      var createdAtUtc = DateTime.UtcNow;
      var creatorId = _httpContextAccessor.HttpContext.GetUserId();

      return new DbRole
      {
        Id = roleId,
        CreatedBy = creatorId,
        CreatedAtUtc = createdAtUtc,
        IsActive = true,
        RoleLocalizations = request.Localizations.Select(rl => _localizationMapper.Map(rl, roleId)).ToList(),
        RoleRights = request.Rights?.Select(x => new DbRoleRight
        {
          Id = Guid.NewGuid(),
          RoleId = roleId,
          CreatedBy = creatorId,
          CreatedAtUtc = createdAtUtc,
          RightId = x,
        }).ToList()
      };
    }
  }
}
