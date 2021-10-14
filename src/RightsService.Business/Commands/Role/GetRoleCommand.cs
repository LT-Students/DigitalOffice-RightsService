using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Business.Role
{
  /// <inheritdoc/>
  public class GetRoleCommand : IGetRoleCommand
  {
    private readonly ILogger<GetRoleCommand> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleResponseMapper _roleResponseMapper;
    private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private async Task<List<UserData>> GetUsers(List<Guid> usersIds, List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      try
      {
        var usersDataResponse = await _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
          IGetUsersDataRequest.CreateObj(usersIds));

        if (usersDataResponse.Message.IsSuccess)
        {
          return usersDataResponse.Message.Body.UsersData;
        }

        _logger.LogWarning(
            $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Message.Errors)}.");
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Exception on get users information.");
      }
      errors.Add("Can not get users info. Please try again later.");

      return null;
    }

    public GetRoleCommand(
      ILogger<GetRoleCommand> logger,
      IRoleRepository roleRepository,
      IRoleResponseMapper roleResponseMapper,
      IRequestClient<IGetUsersDataRequest> usersDataRequestClient,
      IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _roleRepository = roleRepository;
      _roleResponseMapper = roleResponseMapper;
      _usersDataRequestClient = usersDataRequestClient;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<RoleResponse>> Execute(GetRoleFilter filter)
    {
      OperationResultResponse<RoleResponse> result = new();

      (DbRole role, List<DbUser> users, List<DbRightsLocalization> rights) = _roleRepository.Get(filter);

      if (role == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

        return result;
      }

      List<Guid> usersIds = users?.Select(u => u.UserId).ToList();

      usersIds.Add(role.CreatedBy);

      if (role.ModifiedBy.HasValue)
      {
        usersIds.Add(role.ModifiedBy.Value);
      }

      List<UserData> usersDatas = await GetUsers(usersIds, result.Errors);

      result.Body = _roleResponseMapper.Map(role, rights, usersDatas);

      return result;
    }
  }
}
