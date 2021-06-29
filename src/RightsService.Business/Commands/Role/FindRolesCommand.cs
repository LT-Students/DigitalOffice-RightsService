﻿using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Business.Role
{
    /// <inheritdoc/>
    public class FindRolesCommand : IFindRolesCommand
    {
        private readonly ILogger<FindRolesCommand> _logger;
        private readonly IRoleRepository _roleRepository;
        private readonly IRightRepository _rightRepository;
        private readonly IRoleInfoMapper _roleInfoMapper;
        private readonly IUserInfoMapper _userInfoMapper;
        private readonly IRightMapper _rightMapper;
        private readonly IRequestClient<IGetUsersDataRequest> _usersDataRequestClient;

        private List<UserInfo> GetUsers(List<Guid> userIds, List<string> errors)
        {
            string errorMessage = null;

            List<UserInfo> usersInfo = new();

            try
            {
                errorMessage = $"Can not get users info for UserIds {string.Join('\n', userIds)}. Please try again later.";

                var usersDataResponse = _usersDataRequestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
                    IGetUsersDataRequest.CreateObj(userIds)).Result;

                if (usersDataResponse.Message.IsSuccess)
                {
                    var usersData = usersDataResponse.Message.Body.UsersData;

                    usersInfo = userIds
                        .Select(x => _userInfoMapper.Map(usersData.First(ud => ud.Id == x)))
                        .ToList();
                }
                else
                {
                    _logger.LogWarning(
                        $"Can not get users. Reason:{Environment.NewLine}{string.Join('\n', usersDataResponse.Message.Errors)}.");
                }
            }
            catch (Exception exc)
            {
                errors.Add(errorMessage);

                _logger.LogError(exc, "Exception on get user information.");
            }

            return usersInfo;
        }

        public FindRolesCommand(
            ILogger<FindRolesCommand> logger,
            IRoleRepository roleRepository,
            IRightRepository rightRepository,
            IUserInfoMapper userInfoMapper,
            IRoleInfoMapper roleInfoMapper,
            IRightMapper rightMapper,
            IRequestClient<IGetUsersDataRequest> usersDataRequestClient)
        {
            _logger = logger;
            _roleRepository = roleRepository;
            _rightRepository = rightRepository;
            _roleInfoMapper = roleInfoMapper;
            _userInfoMapper = userInfoMapper;
            _rightMapper = rightMapper;
            _usersDataRequestClient = usersDataRequestClient;
        }

        public RolesResponse Execute(int skipCount, int takeCount)
        {
            RolesResponse result = new();

            var dbRoles = _roleRepository.Find(skipCount, takeCount, out int totalCount);

            result.TotalCount = totalCount;

            var allRights = _rightRepository.GetRightsList();

            foreach (var dbRole in dbRoles)
            {
                var rights = allRights
                    .Where(x => dbRole.Rights.Select(x => x.RightId).ToList().Contains(x.Id))
                    .Select(_rightMapper.Map)
                    .ToList();

                var users = GetUsers(dbRole.Users.Select(x => x.UserId).ToList(), result.Errors);

                result.Roles.Add(_roleInfoMapper.Map(dbRole, rights, users));
            }

            return result;
        }
    }
}
