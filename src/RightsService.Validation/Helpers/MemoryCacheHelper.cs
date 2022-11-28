using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Configurations;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.RightsService.Validation.Helpers
{
  public class MemoryCacheHelper : IMemoryCacheHelper
  {
    private readonly IMemoryCache _cache;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly IRoleRepository _roleRepository;
    private readonly IRightLocalizationRepository _rightLocalizationRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public MemoryCacheHelper(
      IMemoryCache cache,
      IRoleRepository roleRepository,
      IRightLocalizationRepository rightLocalizationRepository,
      IUserRoleRepository userRoleRepository)
    {
      _cache = cache;
      _roleRepository = roleRepository;
      _rightLocalizationRepository = rightLocalizationRepository;
      _userRoleRepository = userRoleRepository;
    }

    public async Task<List<int>> GetRightIdsAsync()
    {
      List<int> rights = _cache.Get<List<int>>(CacheKeys.RightsIds);

      if (rights is null)
      {
        rights = (await _rightLocalizationRepository.GetRightsListAsync()).Select(r => r.RightId).ToList();
        _cache.Set(CacheKeys.RightsIds, rights, TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes));
      }

      return rights;
    }

    public async Task<List<(Guid roleId, bool isActive, IEnumerable<int> rights)>> GetRoleRightsListAsync()
    {
      List<(Guid roleId, bool isActive, IEnumerable<int> rights)> rolesRights = _cache.Get<List<(Guid, bool, IEnumerable<int>)>>(CacheKeys.RolesRights);

      if (rolesRights is null)
      {
        List<DbRole> roles = await _roleRepository.GetAllWithRightsAsync();

        rolesRights = roles.Select(x => (x.Id, x.IsActive, x.RolesRights.Select(x => x.RightId))).ToList();
        
        _cache.Set(CacheKeys.RolesRights, rolesRights, TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes));
      }

      return rolesRights;
    }

    public async Task<List<(Guid userId, Guid roleId)>> GetUsersRolesAsync()
    {
      if (!_cache.TryGetValue(CacheKeys.Users, out List<(Guid userId, Guid roleId)> users))
      {
        users = (await _userRoleRepository.GetWithoutRightsAsync()).Select(x => (x.UserId, x.RoleId)).ToList();
      }

      return users;
    }

    public void Clear(params string[] keys)
    {
      foreach (string key in keys)
      {
        _cache.Remove(key);
      }
    }
  }
}
