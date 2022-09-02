using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Business.Role
{
  /// <inheritdoc/>
  public class CreateRoleCommand : ICreateRoleCommand
  {
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICreateRoleRequestValidator _validator;
    private readonly IDbRoleMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IMemoryCache _cache;
    private readonly IResponseCreator _responseCreator;

    private async Task UpdateCacheAsync(IEnumerable<int> addedRights, Guid roleId)
    {
      List<(Guid roleId, bool isActive, IEnumerable<int> rights)> rolesRights = _cache.Get<List<(Guid, bool, IEnumerable<int>)>>(CacheKeys.RolesRights);

      if (rolesRights == null)
      {
        List<DbRole> roles = await _roleRepository.GetAllWithRightsAsync();

        rolesRights = roles.Select(x => (x.Id, x.IsActive, x.RolesRights.Select(x => x.RightId))).ToList();
      }
      else
      {
        rolesRights.Add((roleId, true, addedRights));
      }

      _cache.Set(CacheKeys.RolesRights, rolesRights);
    }

    public CreateRoleCommand(
      IHttpContextAccessor httpContextAccessor,
      IRoleRepository roleRepository,
      ICreateRoleRequestValidator validator,
      IDbRoleMapper mapper,
      IAccessValidator accessValidator,
      IMemoryCache cache,
      IResponseCreator responseCreator)
    {
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _roleRepository = roleRepository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _cache = cache;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateRoleRequest request)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<Guid>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      DbRole dbRole = _mapper.Map(request);
      await _roleRepository.CreateAsync(dbRole);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      await UpdateCacheAsync(request.Rights, dbRole.Id);

      return new OperationResultResponse<Guid>(body: dbRole.Id);
    }
  }
}
