using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Mappers.Models
{
  public class RoleLocalizationInfoMapper : IRoleLocalizationInfoMapper
  {
    public RoleLocalizationInfo Map(DbRoleLocalization dbRoleLocalization)
    {
      if (dbRoleLocalization == null)
      {
        return null;
      }

      return new RoleLocalizationInfo
      {
        Id = dbRoleLocalization.Id,
        Locale = dbRoleLocalization.Locale,
        Name = dbRoleLocalization.Name,
        Description = dbRoleLocalization.Description,
        IsActive = dbRoleLocalization.IsActive,
        RoleId = dbRoleLocalization.RoleId
      };
    }
  }
}
