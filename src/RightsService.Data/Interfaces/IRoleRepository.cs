using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of RightsService.
    /// </summary>
    [AutoInject]
    public interface IRoleRepository
    {
        /// <summary>
        /// Adds new role to the database. Returns whether it was successful to add.
        /// </summary>
        Guid Create(DbRole dbRole);

        /// <summary>
        /// Returns role by id or throws not found exception.
        /// </summary>
        /// <returns>List of all added roles.</returns>
        DbRole Get(Guid roleId);

        /// <summary>
        /// Returns a list of all added roles to the database.
        /// </summary>
        /// <returns>List of all added roles.</returns>
        IEnumerable<DbRole> Find(int skipCount, int takeCount, out int totalCount);
    }
}