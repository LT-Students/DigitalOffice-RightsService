using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Validation.Helpers
{
  public class MemoryCacheHelper : IMemoryCacheHelper
  {
    private readonly IMemoryCache _cache;
    private readonly IRoleRepository _roleRepository;
    private readonly IRightLocalizationRepository _rightLocalizationRepository;

    public MemoryCacheHelper(
      IMemoryCache cache,
      IRoleRepository roleRepository,
      IRightLocalizationRepository rightLocalizationRepository)
    {
      _cache = cache;
      _roleRepository = roleRepository;
      _rightLocalizationRepository = rightLocalizationRepository;
    }

    public async Task<List<int>> GetRightIdsAsync()
    {
      List<int> rights = _cache.Get<List<int>>(CacheKeys.RightsIds);

      if (rights == null)
      {
        rights = (await _rightLocalizationRepository.GetRightsListAsync()).Select(r => r.RightId).ToList();
        _cache.Set(CacheKeys.RightsIds, rights);
      }

      return rights;
    }

    public async Task<List<(Guid roleId, bool isActive, IEnumerable<int> rights)>> GetRoleRightsListAsync()
    {
      List<(Guid roleId, bool isActive, IEnumerable<int> rights)> rights = _cache.Get<List<(Guid, bool, IEnumerable<int>)>>(CacheKeys.RolesRights);

      if (rights == null)
      {
        List<DbRole> roles = await _roleRepository.GetAllWithRightsAsync();

        rights = roles.Select(x => (x.Id, x.IsActive, x.RoleRights.Select(x => x.RightId))).ToList();
        _cache.Set(CacheKeys.RolesRights, rights);
      }

      return rights;
    }
  }
}
