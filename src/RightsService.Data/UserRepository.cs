using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Models.Db;
using System;
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
    }
}
