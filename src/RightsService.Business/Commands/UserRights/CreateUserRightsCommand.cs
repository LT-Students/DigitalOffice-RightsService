using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Business.Commands.UserRights
{
  /// <inheritdoc cref="ICreateUserRightsCommand"/>
  public class CreateUserRightsCommand : ICreateUserRightsCommand
  {
    private readonly IUserRepository _repository;
    private readonly IRightsIdsValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUser;
    private readonly ILogger<CreateUserRightsCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;

    private async Task UpdateCacheAsync(Guid userId, IEnumerable<int> rights)
    {
      List<(Guid userId, bool isActive, Guid? roleId, IEnumerable<int> userRights)> users =
        _cache.Get<List<(Guid, bool, Guid?, IEnumerable<int>)>>(CacheKeys.Users);

      if (users == null)
      {
        List<DbUser> dbUsers = await _repository.GetWithRightsAsync();

        users = dbUsers.Select(x => (x.UserId, x.IsActive, x.RoleId, x.Rights.Select(x => x.RightId))).ToList();
      }
      else
      {
        (Guid userId, bool isActive, Guid? roleId, IEnumerable<int> userRights) user = users.FirstOrDefault(x => x.userId == userId);
        users.Remove(user);

        if (user == default)
        {
          DbUser dbUser = await _repository.GetAsync(userId);
          user = (dbUser.UserId, dbUser.IsActive, dbUser.RoleId, dbUser.Rights.Select(x => x.RightId));
        }

        var newRights = from right in user.userRights.Union(rights) select right;

        users.Add((userId, user.isActive, user.roleId, newRights));
      }

      _cache.Set(CacheKeys.Users, users);
    }

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
      IResponseCreator responseCreator,
      IRequestClient<ICheckUsersExistence> rcCheckUser,
      ILogger<CreateUserRightsCommand> logger,
      IHttpContextAccessor httpContextAccessor,
      IMemoryCache cache)
    {
      _repository = repository;
      _validator = validator;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _rcCheckUser = rcCheckUser;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _cache = cache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, IEnumerable<int> rightsIds)
    {
      if (!await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(rightsIds);

      List<string> errors = validationResult.Errors.Select(vf => vf.ErrorMessage).ToList();

      if (!validationResult.IsValid && !await CheckUserExistenceAsync(userId, errors))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      await _repository.AddUserRightsAsync(userId, rightsIds);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      await UpdateCacheAsync(userId, rightsIds);

      return new OperationResultResponse<bool>
      {
        Body = true,
        Status = OperationResultStatusType.FullSuccess
      };
    }
  }
}
