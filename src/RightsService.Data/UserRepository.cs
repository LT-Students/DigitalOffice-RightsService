using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataProvider _provider;

        public UserRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public void Add(DbUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _provider.Users.Add(user);
            _provider.Save();
        }

        public DbUser Get(Guid userId)
        {
            return _provider.Users.FirstOrDefault(x => x.UserId == userId)
                ?? throw new NotFoundException($"No user with id '{userId}'");
        }

        public void AssignRole(Guid userId, Guid roleId, Guid assignedBy)
        {
            var editedUser = _provider.Users.FirstOrDefault(x => x.UserId == userId);

            if (editedUser != null)
            {
                editedUser.RoleId = roleId;
                _provider.Save();

                return;
            }

            _provider.Users.Add(
                new DbUser
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = assignedBy,
                    IsActive = true
                });

            _provider.Save();
        }

        public bool CheckRights(Guid userId, params int[] rightIds)
        {
            if (rightIds == null)
            {
                throw new ArgumentNullException(nameof(rightIds));
            }

            DbUser user = _provider.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Rights)
                .Include(u => u.Rights).FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                throw new NotFoundException($"User with ID '{userId}' does not have any rights.");
            }

            foreach(var rightId in rightIds)
            {
                if (user.Rights.Any(r => r.RightId == rightId) || user.Role.Rights.Any(r => r.RightId == rightId))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public List<DbUser> Get(List<Guid> userId)
        {
            return _provider.Users.Where(u => userId.Contains(u.UserId)).Include(u => u.Role).ToList();
        }

        public void Remove(Guid userId)
        {
            DbUser user = _provider.Users.FirstOrDefault(u => u.UserId == userId)
                ?? throw new NotFoundException($"No user with id {userId}.");

            user.IsActive = false;
            _provider.Save();
        }
    }
}
