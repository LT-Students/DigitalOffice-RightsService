using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization
{
  public class EditRoleLocalizationCommand : IEditRoleLocalizationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreator;
    private readonly IRoleLocalizationRepository _roleLocalizationRepository;
    private readonly IPatchDbRoleLocalizationMapper _roleLocalizationMapper;
    private readonly IEditRoleLocalizationRequestValidator _validator;

    public EditRoleLocalizationCommand(
      IAccessValidator accessValidator,
      IResponseCreater responseCreator,
      IRoleLocalizationRepository roleLocalizationRepository,
      IPatchDbRoleLocalizationMapper roleLocalizationMapper,
      IEditRoleLocalizationRequestValidator validator)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _roleLocalizationRepository = roleLocalizationRepository;
      _roleLocalizationMapper = roleLocalizationMapper;
      _validator = validator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid roleLocalizationId, JsonPatchDocument<EditRoleLocalizationRequest> request)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          errors);
      }

      Operation<EditRoleLocalizationRequest> isActiveOperation = request.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditRoleLocalizationRequest.IsActive), StringComparison.OrdinalIgnoreCase));
      Operation<EditRoleLocalizationRequest> nameOperation = request.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditRoleLocalizationRequest.Name), StringComparison.OrdinalIgnoreCase));

      DbRoleLocalization roleLocalization = await _roleLocalizationRepository.GetAsync(roleLocalizationId);

      if (isActiveOperation != default)
      {
        bool isActive = bool.Parse(isActiveOperation.value.ToString().Trim());

        if (roleLocalization.IsActive == isActive)
        {
          return _responseCreator.CreateFailureResponse<bool>(
            HttpStatusCode.BadRequest,
            new List<string> { "Role localization already has this status." });
        }

        if (isActive
          && await _roleLocalizationRepository.DoesLocaleExistAsync(roleLocalization.RoleId, roleLocalization.Locale))
        {
          return _responseCreator.CreateFailureResponse<bool>(
            HttpStatusCode.BadRequest,
            new List<string> { "Role must have only one localization per locale." });
        }
      }

      if (nameOperation != default
        && await _roleLocalizationRepository.DoesNameExistAsync(roleLocalization.Locale, nameOperation.value.ToString().Trim()))
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          new List<string> { "Name already exists." });
      }

      return new OperationResultResponse<bool>()
      {
        Body = await _roleLocalizationRepository.EditRoleLocalizationAsync(roleLocalizationId, _roleLocalizationMapper.Map(request))
      };
    }
  }
}
