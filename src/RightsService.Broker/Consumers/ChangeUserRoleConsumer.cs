using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class ChangeUserRoleConsumer : IConsumer<IChangeUserRoleRequest>
  {
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;

    private async Task UpdateCacheAsync(Guid userId, Guid? roleId)
    {
      List<(Guid userId, bool isActive, Guid? roleId, IEnumerable<int> userRights)> users =
        _cache.Get<List<(Guid, bool, Guid?, IEnumerable<int>)>>(CacheKeys.Users);

      if (users == null)
      {
        List<DbUser> dbUsers = await _userRepository.GetWithRightsAsync();

        users = dbUsers.Select(x => (x.UserId, x.IsActive, x.RoleId, x.Rights.Select(x => x.RightId))).ToList();
      }
      else
      {
        (Guid userId, bool isActive, Guid? roleId, IEnumerable<int> userRights) user = users.FirstOrDefault(x => x.userId == userId);
        users.Remove(user);
        users.Add((userId, user.isActive, roleId, user.userRights));
      }

      _cache.Set(CacheKeys.Users, users);
    }

    private async Task<bool> ChangeRoleAsync(IChangeUserRoleRequest request)
    {
      await _userRepository.AssignRoleAsync(request.UserId, request.RoleId, request.ChangedBy);

      await UpdateCacheAsync(request.UserId, request.RoleId);

      return true;
    }

    public ChangeUserRoleConsumer(
      IUserRepository userRepository,
      IMemoryCache cache)
    {
      _userRepository = userRepository;
      _cache = cache;
    }

    public async Task Consume(ConsumeContext<IChangeUserRoleRequest> context)
    {
      var result = OperationResultWrapper.CreateResponse(ChangeRoleAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(result);
    }
  }
}
