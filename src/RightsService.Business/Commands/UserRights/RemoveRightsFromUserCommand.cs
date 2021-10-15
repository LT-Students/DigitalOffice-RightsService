using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Business.Commands.UserRights
{
  /// <inheritdoc cref="IRemoveRightsFromUserCommand"/>
  public class RemoveRightsFromUserCommand : IRemoveRightsFromUserCommand
  {
    private readonly IUserRepository _repository;
    private readonly IRightsIdsValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreater;

    public RemoveRightsFromUserCommand(
      IUserRepository repository,
      IRightsIdsValidator validator,
      IAccessValidator accessValidator,
      IResponseCreater responseCreater)
    {
      _repository = repository;
      _validator = validator;
      _accessValidator = accessValidator;
      _responseCreater = responseCreater;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, IEnumerable<int> rightsIds)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(rightsIds);

      if (!validationResult.IsValid)
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      bool result = await _repository.RemoveUserRightsAsync(userId, rightsIds);

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
