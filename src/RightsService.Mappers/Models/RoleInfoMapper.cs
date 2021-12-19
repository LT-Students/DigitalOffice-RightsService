using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Mappers.Models
{
  public class RoleInfoMapper : IRoleInfoMapper
  {
    private readonly IRoleLocalizationInfoMapper _roleLocalizationInfoMapper;

    public RoleInfoMapper(IRoleLocalizationInfoMapper roleLocalizationInfoMapper)
    {
      _roleLocalizationInfoMapper = roleLocalizationInfoMapper;
    }

    public RoleInfo Map(DbRole dbRole, List<RightInfo> rights, List<UserInfo> userInfos)
    {
      if (dbRole == null)
      {
        return null;
      }

      return new RoleInfo
      {
        Id = dbRole.Id,
        IsActive = dbRole.IsActive,
        CreatedAtUtc = dbRole.CreatedAtUtc,
        CreatedBy = userInfos?.FirstOrDefault(x => x.Id == dbRole.CreatedBy),
        ModifiedAtUtc = dbRole.ModifiedAtUtc,
        ModifiedBy = userInfos?.FirstOrDefault(x => x.Id == dbRole.ModifiedBy),
        Rights = rights,
        Localizations = dbRole.RoleLocalizations.Select(_roleLocalizationInfoMapper.Map).ToList()
      };
    }
  }
}
