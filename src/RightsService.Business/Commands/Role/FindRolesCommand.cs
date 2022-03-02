using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Business.Role
{
  /// <inheritdoc/>
  public class FindRolesCommand : IFindRolesCommand
  {
    private readonly ILogger<FindRolesCommand> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleInfoMapper _roleInfoMapper;
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IRightInfoMapper _rightMapper;
    private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;
    private readonly IBaseFindFilterValidator _findFilterValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private async Task<List<UserData>> GetUsersAsync(List<Guid> usersIds, List<string> errors)
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

    public FindRolesCommand(
      ILogger<FindRolesCommand> logger,
      IRoleRepository roleRepository,
      IUserInfoMapper userInfoMapper,
      IRoleInfoMapper roleInfoMapper,
      IRightInfoMapper rightMapper,
      IRequestClient<IGetUsersDataRequest> usersDataRequestClient,
      IBaseFindFilterValidator findFilterValidator,
      IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _roleRepository = roleRepository;
      _roleInfoMapper = roleInfoMapper;
      _userInfoMapper = userInfoMapper;
      _rightMapper = rightMapper;
      _usersDataRequestClient = usersDataRequestClient;
      _findFilterValidator = findFilterValidator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FindResultResponse<RoleInfo>> ExecuteAsync(FindRolesFilter filter)
    {
      if (!_findFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new FindResultResponse<RoleInfo>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      FindResultResponse<RoleInfo> result = new();

      (List<(DbRole role, List<DbRightLocalization> rights)> roles, int totalCount) = filter.IncludeDeactivated
        ? await _roleRepository.FindAllAsync(filter)
        : await _roleRepository.FindActiveAsync(filter);

      result.TotalCount = totalCount;

      List<Guid> usersIds = new();

      foreach((DbRole role, List<DbRightLocalization> rights) in roles)
      {
        usersIds.Add(role.CreatedBy);

        if (role.ModifiedBy.HasValue)
        {
          usersIds.Add(role.ModifiedBy.Value);
        }
      }

      List<UserInfo> usersInfos = (await GetUsersAsync(usersIds.Distinct().ToList(), errors))?
        .Select(_userInfoMapper.Map)
        .ToList();

      result.Body = roles.Select(
        pair => _roleInfoMapper.Map(pair.role, pair.rights.Select(_rightMapper.Map).ToList(), usersInfos)).ToList();

      result.Errors = errors;

      result.Status = errors.Any() ?
        OperationResultStatusType.PartialSuccess :
        OperationResultStatusType.FullSuccess;

      return result;
    }
  }
}
