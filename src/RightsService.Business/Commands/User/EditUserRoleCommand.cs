using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.RightsService.Business.Commands.User
{
  public class EditUserRoleCommand : IEditUserRoleCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IEditUserRoleRequestValidator _validator;
    private readonly IDbUserRoleMapper _mapper;
    private readonly IUserRoleRepository _repository;

    public EditUserRoleCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IEditUserRoleRequestValidator validator,
      IDbUserRoleMapper mapper,
      IUserRoleRepository repository)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(EditUserRoleRequest request)
    {
      // TODO: Check proper right
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();
      await _repository.RemoveAsync(request.UserId);
      response.Body = request.RoleId.HasValue
        ? (await _repository.CreateAsync(_mapper.Map(
          request,
          _httpContextAccessor.HttpContext.GetUserId())))
        : null;
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
