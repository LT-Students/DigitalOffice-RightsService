using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Provider;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using LT.DigitalOffice.Kernel.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.CheckRightsService.Data
{
    /// <inheritdoc cref="ICheckRightsRepository"/>
    public class CheckRightsRepository : ICheckRightsRepository
    {
        private readonly IDataProvider provider;

        public CheckRightsRepository(IDataProvider _provider)
        {
            provider = _provider;
        }

        public List<DbRight> GetRightsList()
        {
            return provider.Rights.ToList();
        }

        public void AddRightsToUser(Guid userId, IEnumerable<int> rightsIds)
        {
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
            provider.Save();
        }

        public void RemoveRightsFromUser(Guid userId, IEnumerable<int> rightsIds)
        {
            var userRights = provider.RightUsers.Where(ru =>
                ru.UserId == userId && rightsIds.Contains(ru.RightId));

            provider.RightUsers.RemoveRange(userRights);

            provider.Save();
        }

        public bool IsUserHasRight(Guid userId, int rightId)
        {
            return provider.Rights
                .AsNoTracking()
                .Include(r => r.RightUsers)
                .Select(r => r.RightUsers)
                .ToList()
                .Any(r => r.Select(r => (r.UserId, r.RightId))
                .Contains((userId, rightId)));
        }
    }
}