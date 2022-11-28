using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
  public class UpdateRoleRightsCommand : IUpdateRoleRightsCommand
  {
    private readonly IRoleRepository _roleRepository;
    private readonly IDbRoleRightMapper _dbRoleRightMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IUpdateRoleRightsRequestValidator _requestValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IMemoryCacheHelper _memoryCacheHelper;

    public UpdateRoleRightsCommand(
      IRoleRepository roleRepository,
      IDbRoleRightMapper dbRoleRightMapper,
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IUpdateRoleRightsRequestValidator requestValidator,
      IResponseCreator responseCreator,
      IMemoryCacheHelper memoryCacheHelper)
    {
      _roleRepository = roleRepository;
      _dbRoleRightMapper = dbRoleRightMapper;
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _responseCreator = responseCreator;
      _memoryCacheHelper = memoryCacheHelper;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(UpdateRoleRightsRequest request)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
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

      _memoryCacheHelper.Clear(CacheKeys.RolesRights);

      return response;
    }
  }
}
