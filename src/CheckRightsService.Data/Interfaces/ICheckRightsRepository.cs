using LT.DigitalOffice.CheckRightsService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of CheckRightsService.
    /// </summary>
    public interface ICheckRightsRepository
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
        /// <param name="userId">ID of the user.</param>
        /// <param name="rightId">ID of the right.</param>
        /// <returns>True, if there's a UserId-RightId pair. False otherwise.</returns>
        bool CheckIfUserHasRight(Guid userId, int rightId);
    }
}