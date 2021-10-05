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
    /// <inheritdoc cref="IRightLocalizationRepository"/>
    public class RightLocalizationRepository : IRightLocalizationRepository
    {
        private readonly IDataProvider _provider;
        private readonly IRequestClient<IGetUserDataRequest> _client;

        public RightLocalizationRepository(
            IDataProvider provider,
            IRequestClient<IGetUserDataRequest> client)
        {
            _provider = provider;
            _client = client;
        }

        public List<DbRightsLocalization> GetRightsList()
        {
            return _provider.RightsLocalizations.ToList();
        }

        public List<DbRightsLocalization> GetRightsList(string locale)
        {
            return _provider.RightsLocalizations.Where(r => r.Locale == locale).OrderBy(r => r.RightId).ToList();
        }

        //TODO rework
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
                //TODO rework
                var dbRight = _provider.RightsLocalizations.FirstOrDefault(right => right.RightId == rightId);

                if (dbRight == null)
                {
                    continue;
                }

                var dbRightUser = _provider.UserRights.FirstOrDefault(rightUser =>
                    rightUser.RightId == rightId && rightUser.UserId == userId);

                if (dbRightUser == null)
                {
                    _provider.UserRights.Add(new DbUserRight
                    {
                        UserId = userId,
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
    }
}
