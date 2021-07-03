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
    public interface IRightRepository
    {
        /// <summary>
        /// Returns a list of all added rights to the database.
        /// </summary>
        /// <returns>List of all added rights.</returns>
        List<DbRight> GetRightsList();

        /// <summary>
        /// Adds rights for user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="rightIds">List of rights.</param>
        void AddRightsToUser(Guid userId, IEnumerable<int> rightIds);

        /// <summary>
        /// Remove rights for user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="rightIds">List of rights.</param>
        void RemoveRightsFromUser(Guid userId, IEnumerable<int> rightIds);

        /// <summary>
        /// Checks whether the user has the specific right.
        /// </summary>
        bool CheckUserHasRights(Guid userId, params int[] rightIds);
    }
}