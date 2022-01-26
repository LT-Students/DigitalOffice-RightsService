using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;

namespace LT.DigitalOffice.RightsService.Validation.Helpers
{
  public class CheckRightsUniquenessHelper : ICheckRightsUniquenessHelper
  {
    private readonly IMemoryCacheHelper _memoryCacheHelper;

    public CheckRightsUniquenessHelper(
      IMemoryCacheHelper memoryCacheHelper)
    {
      _memoryCacheHelper = memoryCacheHelper;
    }

    public async Task<bool> IsRightsSetUniqueAsync(IEnumerable<int> rightsIds)
    {
      HashSet<int> addedRights = new(rightsIds);

      IEnumerable<(Guid roleId, bool isActive, IEnumerable<int> rights)> roles = await _memoryCacheHelper.GetRoleRightsListAsync();

      foreach ((Guid roleId, bool isActive, IEnumerable<int> rights) role in roles)
      {
        if (role.isActive && addedRights.SetEquals(role.rights))
        {
          return false;
        }
      }

      return true;
    }
  }
}
