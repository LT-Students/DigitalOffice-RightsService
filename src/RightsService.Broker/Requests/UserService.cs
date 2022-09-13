using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Broker.Requests
{
  public class UserService : IUserService
  {
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUserExistence;
    private readonly IRequestClient<IGetUsersDataRequest> _rcGetUsersDataRequest;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly ILogger<UserService> _logger;

    public UserService(
      IRequestClient<ICheckUsersExistence> rcCheckUserExistence,
      IRequestClient<IGetUsersDataRequest> rcGetUsersDataRequest,
      IGlobalCacheRepository globalCache,
      ILogger<UserService> logger)
    {
      _rcCheckUserExistence = rcCheckUserExistence;
      _rcGetUsersDataRequest = rcGetUsersDataRequest;
      _globalCache = globalCache;
      _logger = logger;
    }

    public async Task<List<Guid>> CheckUsersExistence(List<Guid> usersIds, List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<ICheckUsersExistence, ICheckUsersExistence>(
          _rcCheckUserExistence,
          ICheckUsersExistence.CreateObj(usersIds),
          errors,
          _logger))
        ?.UserIds;
    }

    public async Task<List<UserData>> GetUsersAsync(List<Guid> usersIds, List<string> errors)
    {
      if (usersIds is null || !usersIds.Any())
      {
        return null;
      }

      object request = IGetUsersDataRequest.CreateObj(usersIds);

      List<UserData> usersData = await _globalCache.GetAsync<List<UserData>>(Cache.Users, usersIds.GetRedisCacheKey(request.GetBasicProperties()));

      if (usersData is not null)
      {
        _logger.LogInformation(
          "UsersDatas were taken from the cache. Users ids: {usersIds}", string.Join(", ", usersIds));
      }
      else
      {
        usersData = (await _rcGetUsersDataRequest.ProcessRequest<IGetUsersDataRequest, IGetUsersDataResponse>(
          request,
          errors,
          _logger))?.UsersData;
      }

      return usersData;
    }
  }
}
