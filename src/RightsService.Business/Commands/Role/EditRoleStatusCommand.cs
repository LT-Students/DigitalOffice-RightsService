using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
  public class EditRoleStatusCommand : IEditRoleStatusCommand
  {
    private readonly IRoleRepository _roleRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IEditRoleStatusRequestValidator _validator;
    private readonly IResponseCreator _responseCreator;
    private readonly IMemoryCache _cache;

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
      IEditRoleStatusRequestValidator validator,
      IResponseCreator responseCreator,
      IMemoryCache cache)
    {
      _roleRepository = roleRepository;
      _accessValidator = accessValidator;
      _validator = validator;
      _responseCreator = responseCreator;
      _cache = cache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid roleId, bool isActive)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult result = await _validator.ValidateAsync((roleId, isActive));
      if (!result.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          result.Errors.Select(x => x.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _roleRepository.EditStatusAsync(roleId, isActive);

      await UpdateCacheAsync(roleId, isActive);

      return response;
    }
  }
}
