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

    public RoleInfo Map(DbRole value, List<RightInfo> rights, List<UserInfo> userInfos)
    {
      if (value == null)
      {
        return null;
      }

      return new RoleInfo
      {
        Id = value.Id,
        CreatedAtUtc = value.CreatedAtUtc,
        CreatedBy = userInfos?.FirstOrDefault(x => x.Id == value.CreatedBy),
        ModifiedAtUtc = value.ModifiedAtUtc,
        ModifiedBy = userInfos?.FirstOrDefault(x => x.Id == value.ModifiedBy),
        Rights = rights,
        Localizations = value.RoleLocalizations.Select(_roleLocalizationInfoMapper.Map).ToList()
      };
    }
  }
}
