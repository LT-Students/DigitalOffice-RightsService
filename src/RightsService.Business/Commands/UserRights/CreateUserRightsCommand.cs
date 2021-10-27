using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Business.Commands.UserRights
{
  /// <inheritdoc cref="ICreateUserRightsCommand"/>
  public class CreateUserRightsCommand : ICreateUserRightsCommand
  {
    private readonly IUserRepository _repository;
    private readonly IRightsIdsValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreater;
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUser;
    private readonly ILogger<CreateUserRightsCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private async Task<bool> CheckUserExistenceAsync(Guid userId, List<string> erros)
    {
      string logMessage = "User existence check is not successful";

      try
      {
        Response<IOperationResult<ICheckUsersExistence>> brokerResponse =
          await _rcCheckUser.GetResponse<IOperationResult<ICheckUsersExistence>>(
            ICheckUsersExistence.CreateObj(new() { userId }));

        if (brokerResponse.Message.IsSuccess && brokerResponse.Message.Body.UserIds.Any())
        {
          return true;
        }

        _logger.LogWarning(logMessage);
      }
      catch(Exception exc)
      {
        _logger.LogError(exc, logMessage);
      }

      erros.Add("User must exist.");

      return false;
    }
    public CreateUserRightsCommand(
      IUserRepository repository,
      IRightsIdsValidator validator,
      IAccessValidator accessValidator,
      IResponseCreater responseCreater,
      IRequestClient<ICheckUsersExistence> rcCheckUser,
      ILogger<CreateUserRightsCommand> logger,
      IHttpContextAccessor httpContextAccessor)
    {
      _repository = repository;
      _validator = validator;
      _accessValidator = accessValidator;
      _responseCreater = responseCreater;
      _rcCheckUser = rcCheckUser;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, IEnumerable<int> rightsIds)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(rightsIds);

      List<string> erros = validationResult.Errors.Select(vf => vf.ErrorMessage).ToList();

      if (!validationResult.IsValid && !await CheckUserExistenceAsync(userId, erros))
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, erros);
      }

      await _repository.AddUserRightsAsync(userId, rightsIds);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<bool>
      {
        Body = true,
        Status = OperationResultStatusType.FullSuccess
      };
    }
  }
}
