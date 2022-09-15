using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class DisactivateUserRoleConsumer : IConsumer<IDisactivateUserPublish>
  {
    private readonly IUserRoleRepository _repository;
    private readonly IMemoryCache _cache;

    private async Task UpdateCacheAsync(Guid userId)
    {
      List<(Guid userId, Guid roleId)> users = _cache.Get<List<(Guid, Guid)>>(CacheKeys.Users);

      if (users == null)
      {
        List<DbUserRole> dbUsers = await _repository.GetWithRightsAsync();

        users = dbUsers.Select(x => (x.UserId, x.RoleId)).ToList();
      }
      else
      {
        (Guid userId, Guid roleId) user = users.FirstOrDefault(x => x.userId == userId);

        if (user != default)
        {
          users.Remove(user);
        }
      }

      _cache.Set(CacheKeys.Users, users);
    }

    public DisactivateUserRoleConsumer(
      IUserRoleRepository userRepository,
      IMemoryCache cache)
    {
      _repository = userRepository;
      _cache = cache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserPublish> context)
    {
      if (await _repository.RemoveAsync(context.Message.UserId, removedBy: context.Message.ModifiedBy))
      {
        await UpdateCacheAsync(context.Message.UserId);
      }
    }
  }
}
