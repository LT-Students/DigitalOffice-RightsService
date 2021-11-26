using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization
{
  public class CreateRoleLocalizationCommand : ICreateRoleLocalizationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IRoleLocalizationRepository _roleLocalizationRepository;
    private readonly IDbRoleLocalizationMapper _roleLocalizationMapper;
    private readonly ICreateRoleLocalizationRequestValidator _requestValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateRoleLocalizationCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IRoleLocalizationRepository roleLocalizationRepository,
      IDbRoleLocalizationMapper roleLocalizationMapper,
      ICreateRoleLocalizationRequestValidator requestValidator,
      IHttpContextAccessor httpContextAccessor)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _roleLocalizationRepository = roleLocalizationRepository;
      _roleLocalizationMapper = roleLocalizationMapper;
      _requestValidator = requestValidator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateRoleLocalizationRequest request)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      if (!request.RoleId.HasValue)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          new List<string> { "RoleId can't be empty." });
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      response.Body = await _roleLocalizationRepository.CreateAsync(_roleLocalizationMapper.Map(request));
      response.Status = response.Body != null
        ? OperationResultStatusType.FullSuccess
        : OperationResultStatusType.Failed;

      _httpContextAccessor.HttpContext.Response.StatusCode = response.Body != null
        ? (int)HttpStatusCode.Created
        : (int)HttpStatusCode.BadRequest;

      return response;
    }
  }
}
