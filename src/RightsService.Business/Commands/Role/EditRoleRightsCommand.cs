using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
  public class EditRoleRightsCommand : IEditRoleRightsCommand
  {
    private readonly IRoleRepository _roleRepository;
    private readonly IDbRoleRightMapper _dbRoleRightMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IChangeRoleRightsRequestValidator _requestValidator;
    private readonly IMemoryCache _cache;
    private readonly IResponseCreater _responseCreator;

    private async Task UpdateCache(IEnumerable<int> addedRights, Guid roleId)
    {
      List<(Guid roleId, IEnumerable<int> rights)> rights = _cache.Get<List<(Guid, IEnumerable<int>)>>(CacheKeys.RolesRights);

      if (rights == null)
      {
        List<DbRole> roles = await _roleRepository.GetAllWithRightsAsync();

        rights = roles.Select(x => (x.Id, x.RoleRights.Select(x => x.RightId))).ToList();
      }
      else
      {
        rights.Remove(rights.Where(x => x.roleId == roleId).FirstOrDefault());
        rights.Add((roleId, addedRights));
      }

      _cache.Set(CacheKeys.RolesRights, rights);
    }

    public EditRoleRightsCommand(
      IRoleRepository roleRepository,
      IDbRoleRightMapper dbRoleRightMapper,
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IChangeRoleRightsRequestValidator requestValidator,
      IMemoryCache cache,
      IResponseCreater responseCreator)
    {
      _roleRepository = roleRepository;
      _dbRoleRightMapper = dbRoleRightMapper;
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _cache = cache;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditRoleRightsRequest request)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _roleRepository.EditRoleRightsAsync(
        request.RoleId,
        _dbRoleRightMapper.Map(request.RoleId, request.Rights));

      await UpdateCache(request.Rights, request.RoleId);

      return response;
    }
  }
}
