using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
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
    private readonly ILogger<UserService> _logger;

    public UserService(
      IRequestClient<ICheckUsersExistence> rcCheckUserExistence,
      IRequestClient<IGetUsersDataRequest> rcGetUsersDataRequest,
      ILogger<UserService> logger)
    {
      _rcCheckUserExistence = rcCheckUserExistence;
      _rcGetUsersDataRequest = rcGetUsersDataRequest;
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
      return (await RequestHandler.ProcessRequest<IGetUsersDataRequest, IGetUsersDataResponse>(
          _rcGetUsersDataRequest,
          IGetUsersDataRequest.CreateObj(usersIds),
          errors,
          _logger))
        ?.UsersData;
    }
  }
}
