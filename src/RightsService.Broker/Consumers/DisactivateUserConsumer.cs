using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IUserRepository _repository;
    private readonly IMemoryCache _cache;

    private async Task UpdateCacheAsync(Guid userId)
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

        if (user != default)
        { 
          users.Add((user.userId, false, user.roleId, user.userRights));
        }
      }

      _cache.Set(CacheKeys.Users, users);
    }

    public DisactivateUserConsumer(
      IUserRepository userRepository,
      IMemoryCache cache)
    {
      _repository = userRepository;
      _cache = cache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await _repository.RemoveAsync(context.Message.UserId);

      await UpdateCacheAsync(context.Message.UserId);
    }
  }
}
