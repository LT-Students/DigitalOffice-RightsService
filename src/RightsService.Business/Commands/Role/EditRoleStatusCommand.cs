using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
  public class EditRoleStatusCommand : IEditRoleStatusCommand
  {
    private readonly IRoleRepository _roleRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreator;
    private readonly IMemoryCache _cache;

    private async Task<List<(Guid, bool, IEnumerable<int>)>> GetRoleRightsListAsync()
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

    private async Task<bool> CheckRightsUniquenessAsync(IEnumerable<int> rightsIds)
    {
      HashSet<int> addedRights = new(rightsIds);

      IEnumerable<(Guid roleId, bool isActive, IEnumerable<int> rights)> roles = await GetRoleRightsListAsync();

      foreach ((Guid roleId, bool isActive, IEnumerable<int> rights) role in roles)
      {
        if (role.isActive && addedRights.SetEquals(role.rights))
        {
          return false;
        }
      }

      return true;
    }

    private async Task<(bool isSuccess, string error)> CheckEnablePossibilityAsync(Guid roleId, bool isActive)
    {
      DbRole role = await _roleRepository.GetAsync(roleId);

      if (role == null)
      {
        return (false, "Role doesn't exist.");
      }

      if (role.IsActive == isActive)
      {
        return (false, "Role already has this status.");
      }

      if (isActive && !(await CheckRightsUniquenessAsync(role.RoleRights.Select(x => x.RightId))))
      {
        return (false, "Role's set of rights are not unique.");
      }

      return (true, null);
    }

    private async Task UpdateCacheAsync(Guid roleId, bool isActive)
    {
      List<(Guid roleId, bool isActive, IEnumerable<int> rights)> rights = _cache.Get<List<(Guid, bool, IEnumerable<int>)>>(CacheKeys.RolesRights);

      if (rights == null)
      {
        List<DbRole> roles = await _roleRepository.GetAllWithRightsAsync();

        rights = roles.Select(x => (x.Id, x.IsActive, x.RoleRights.Select(x => x.RightId))).ToList();
      }
      else
      {
        (Guid roleId, bool isActive, IEnumerable<int> rights) oldRole = rights.FirstOrDefault(x => x.roleId == roleId);
        rights.Remove(oldRole);
        rights.Add((roleId, isActive, oldRole.rights));
      }

      _cache.Set(CacheKeys.RolesRights, rights);
    }

    public EditRoleStatusCommand(
      IRoleRepository roleRepository,
      IAccessValidator accessValidator,
      IResponseCreater responseCreator,
      IMemoryCache cache)
    {
      _roleRepository = roleRepository;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _cache = cache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid roleId, bool isActive)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      (bool isSuccess, string error) check = await CheckEnablePossibilityAsync(roleId, isActive);
      if (!check.isSuccess)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.Conflict,
          new List<string> { check.error });
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _roleRepository.EditStatusAsync(roleId, isActive);

      await UpdateCacheAsync(roleId, isActive);

      return response;
    }
  }
}
