﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class AccessValidatorConsumer : IConsumer<ICheckUserRightsRequest>
  {
    private readonly IUserRepository _repository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;

    private async Task<object> HasRightAsync(ICheckUserRightsRequest request)
    {
      var users = _cache.Get<List<(Guid userId, bool isActive, Guid? roleId, IEnumerable<int> userRights)>>(CacheKeys.Users);
      var roles = _cache.Get<List<(Guid roleId, bool isActive, IEnumerable<int> rights)>>(CacheKeys.RolesRights);

      if (users == null)
      {
        List<DbUser> dbUsers = await _repository.GetWithRightsAsync();
        users = dbUsers.Select(x => (x.UserId, x.IsActive, x.RoleId, x.Rights.Select(x => x.RightId))).ToList();
        _cache.Set(CacheKeys.Users, users);
      }
      if (roles == null)
      {
        List<DbRole> dbRoles = await _roleRepository.GetAllWithRightsAsync();
        roles = dbRoles.Select(x => (x.Id, x.IsActive, x.RoleRights.Select(x => x.RightId))).ToList();
        _cache.Set(CacheKeys.RolesRights, roles);
      }

      (Guid userId, bool isActive, Guid? roleId, IEnumerable<int> userRights) user = users.FirstOrDefault(x => x.userId == request.UserId);
      (Guid roleId, bool isActive, IEnumerable<int> rights) role = roles.FirstOrDefault(x => x.roleId == user.roleId && x.isActive);

      foreach (int rightId in request.RightIds)
      {
        if (user.userRights.Contains(rightId) || role.rights?.Contains(rightId) == true)
        {
          continue;
        }

        return false;
      }

      return true;
    }

    public AccessValidatorConsumer(
      [FromServices] IUserRepository repository,
      [FromServices] IRoleRepository roleRepository,
      [FromServices] IMemoryCache cache)
    {
      _repository = repository;
      _roleRepository = roleRepository;
      _cache = cache;
    }

    public async Task Consume(ConsumeContext<ICheckUserRightsRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(HasRightAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
