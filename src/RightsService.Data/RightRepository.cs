using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Data
{
    /// <inheritdoc cref="IRightRepository"/>
    public class RightRepository : IRightRepository
    {
        private readonly IDataProvider _provider;
        private readonly IRequestClient<IGetUserDataRequest> _client;

        public RightRepository(
            IDataProvider provider,
            IRequestClient<IGetUserDataRequest> client)
        {
            _provider = provider;
            _client = client;
        }

        public List<DbRight> GetRightsList()
        {
            return _provider.Rights.ToList();
        }

        private bool SentRequestInUserService(Guid userId)
        {
            var brokerResponse = _client.GetResponse<IOperationResult<IGetUserDataResponse>>(new
            {
                UserId = userId
            }).Result;

            return brokerResponse.Message.IsSuccess;
        }

        public void AddRightsToUser(Guid userId, IEnumerable<int> rightsIds)
        {
            if (!SentRequestInUserService(userId))
            {
                throw new NotFoundException("User not found.");
            }

            foreach (var rightId in rightsIds)
            {
                var dbRight = _provider.Rights.FirstOrDefault(right => right.Id == rightId);

                if (dbRight == null)
                {
                    throw new BadRequestException("Right doesn't exist.");
                }

                var dbRightUser = _provider.UserRights.FirstOrDefault(rightUser =>
                    rightUser.RightId == rightId && rightUser.UserId == userId);

                if (dbRightUser == null)
                {
                    _provider.UserRights.Add(new DbUserRight
                    {
                        UserId = userId,
                        Right = dbRight,
                        RightId = rightId,
                    });
                }
            }
            _provider.Save();
        }

        public void RemoveRightsFromUser(Guid userId, IEnumerable<int> rightsIds)
        {
            var userRights = _provider.UserRights.Where(ru =>
                ru.UserId == userId && rightsIds.Contains(ru.RightId));

            _provider.UserRights.RemoveRange(userRights);

            _provider.Save();
        }

        public bool CheckUserHasRights(Guid userId, params int[] rightIds)
        {
            if (rightIds == null)
            {
                throw new ArgumentNullException(nameof(rightIds));
            }

            bool result = rightIds.Any();

            DbUser dbRoleUser = _provider.Users.FirstOrDefault(u => u.UserId == userId);
            if (dbRoleUser == null)
            {
                throw new NotFoundException($"User with ID '{userId}' does not have any rights.");
            }

            return dbRoleUser.Rights.All(r => rightIds.Contains(r.RightId));
        }
    }
}