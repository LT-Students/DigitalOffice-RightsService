using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Provider;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LT.DigitalOffice.CheckRightsService.Data
{
    /// <inheritdoc cref="ICheckRightsRepository"/>
    public class CheckRightsRepository : ICheckRightsRepository
    {
        private readonly IDataProvider provider;
        private readonly IRequestClient<IGetUserRequest> client;

        public CheckRightsRepository(IDataProvider _provider, IRequestClient<IGetUserRequest> _client)
        {
            provider = _provider;
            client = _client;
        }

        public List<DbRight> GetRightsList()
        {
            return provider.Rights.ToList();
        }

        private bool SentRequestInUserService(Guid userId)
        {
            var brokerResponse = client.GetResponse<IOperationResult<IGetUserResponse>>(new
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
                var dbRight = provider.Rights.FirstOrDefault(right => right.Id == rightId);

                if (dbRight == null)
                {
                    throw new BadRequestException("Right doesn't exist.");
                }

                var dbRightUser = provider.RightUsers.FirstOrDefault(rightUser =>
                    rightUser.RightId == rightId && rightUser.UserId == userId);

                if (dbRightUser == null)
                {
                    provider.RightUsers.Add(new DbRightUser
                    {
                        UserId = userId,
                        Right = dbRight,
                        RightId = rightId,
                    });
                }
            }
            provider.SaveChanges();
        }

        public void RemoveRightsFromUser(Guid userId, IEnumerable<int> rightsIds)
        {
            var userRights = provider.RightUsers.Where(ru =>
                ru.UserId == userId && rightsIds.Contains(ru.RightId));

            provider.RightUsers.RemoveRange(userRights);

            provider.SaveChanges();
        }

        public bool CheckIfUserHasRight(Guid userId, int rightId)
        {
            var rights = provider.Rights
                .AsNoTracking()
                .Include(r => r.RightUsers);

            if (rights.Any(r => r.RightUsers.Select(ru => ru.UserId).Contains(userId)))
            {
                return true;
            }

            throw new Exception("Such user doesn't exist or does not have this right.");
        }
    }
}