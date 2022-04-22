﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Right;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class CreateUserRoleConsumer : IConsumer<ICreateUserRolePublish>
  {
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDbUserRoleMapper _mapper;
    private readonly IMemoryCache _cache;

    private async Task UpdateCacheAsync(Guid userId, Guid roleId)
    {
      List<(Guid userId, Guid roleId)> users =
        _cache.Get<List<(Guid, Guid)>>(CacheKeys.Users);

      if (users is null)
      {
        List<DbUserRole> dbUsersRoles = await _userRoleRepository.GetWithRightsAsync();

        users = dbUsersRoles.Select(x => (x.UserId, x.RoleId)).ToList();
      }
      else
      {
        (Guid userId, Guid roleId) user = users.FirstOrDefault(x => x.userId == userId);

        if (user != default)
        {
          users.Remove(user);
          users.Add((userId, roleId));
        }
      }

      _cache.Set(CacheKeys.Users, users);
    }

    public CreateUserRoleConsumer(
      IUserRoleRepository userRoleRepository,
      IRoleRepository roleRepository,
      IMemoryCache cache,
      IDbUserRoleMapper mapper)
    {
      _userRoleRepository = userRoleRepository;
      _roleRepository = roleRepository;
      _cache = cache;
      _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ICreateUserRolePublish> context)
    {
      if (await _roleRepository.DoesExistAsync(context.Message.RoleId))
      {
        await _userRoleRepository.CreateAsync(_mapper.Map(context.Message));
        await UpdateCacheAsync(context.Message.UserId, context.Message.RoleId);
      }
    }
  }
}
