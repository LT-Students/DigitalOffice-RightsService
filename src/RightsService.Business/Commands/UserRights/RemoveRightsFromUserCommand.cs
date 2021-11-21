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
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Business.Commands.UserRights
{
  /// <inheritdoc cref="IRemoveRightsFromUserCommand"/>
  public class RemoveRightsFromUserCommand : IRemoveRightsFromUserCommand
  {
    private readonly IUserRepository _repository;
    private readonly IRightsIdsValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreater;
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

        IEnumerable<int> newRights = from right in user.userRights.Except(rights) select right;

        users.Add((userId, user.isActive, user.roleId, newRights));
      }

      _cache.Set(CacheKeys.Users, users);
    }

    public RemoveRightsFromUserCommand(
      IUserRepository repository,
      IRightsIdsValidator validator,
      IAccessValidator accessValidator,
      IResponseCreater responseCreater,
      IMemoryCache cache)
    {
      _repository = repository;
      _validator = validator;
      _accessValidator = accessValidator;
      _responseCreater = responseCreater;
      _cache = cache;
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

      if (result)
      {
        await UpdateCacheAsync(userId, rightsIds);
      }

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
