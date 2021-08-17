using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
    [AutoInject]
    public interface IUserRepository
    {
        void Add(DbUser user);

        DbUser Get(Guid userId);

        void AssignRole(Guid userId, Guid roleId, Guid assignedBy);

        bool CheckRights(Guid userId, params int[] rightIds);

        List<DbUser> Get(List<Guid> userId);

        void Remove(Guid userId);
    }
}
