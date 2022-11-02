using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class FilterRolesUsersConsumer : IConsumer<IFilterRolesRequest>
  {
    private readonly IRoleRepository _repository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<List<RoleFilteredData>> GetRolesDataAsync(IFilterRolesRequest request)
    {
      List<DbRole> roles = await _repository.GetAsync(request.RolesIds);

      return roles.Select(r =>
        new RoleFilteredData(
          r.Id,
          r.RoleLocalizations.Where(rl => rl.RoleId == r.Id).Select(rl => rl.Name).FirstOrDefault(),
          r.Users.Select(u => u.UserId).ToList()))
      .ToList();
    }

    public FilterRolesUsersConsumer(
      IRoleRepository repository,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IFilterRolesRequest> context)
    {
      List<RoleFilteredData> rolesFilteredData = await GetRolesDataAsync(context.Message);

      await context.RespondAsync<IOperationResult<IFilterRolesResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilterRolesResponse.CreateObj(rolesFilteredData), context));

      if (rolesFilteredData is not null)
      {
        await _globalCache.CreateAsync(
          Cache.Rights,
          context.Message.RolesIds.GetRedisCacheKey(nameof(IFilterRolesRequest), context.Message.GetBasicProperties()),
          rolesFilteredData,
          context.Message.RolesIds,
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}
