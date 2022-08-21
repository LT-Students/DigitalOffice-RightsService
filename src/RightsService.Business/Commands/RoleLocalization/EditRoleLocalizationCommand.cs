using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization
{
  public class EditRoleLocalizationCommand : IEditRoleLocalizationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IRoleLocalizationRepository _roleLocalizationRepository;
    private readonly IPatchDbRoleLocalizationMapper _roleLocalizationMapper;
    private readonly IEditRoleLocalizationRequestValidator _validator;

    public EditRoleLocalizationCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
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

      ValidationResult result = await _validator.ValidateAsync((roleLocalizationId, request));

      if (!result.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          result.Errors.Select(x => x.ErrorMessage).ToList());
      }

      return new OperationResultResponse<bool>()
      {
        Body = await _roleLocalizationRepository.EditRoleLocalizationAsync(roleLocalizationId, _roleLocalizationMapper.Map(request))
      };
    }
  }
}
