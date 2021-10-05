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
        List<DbRightsLocalization> GetRightsList();

        /// <summary>
        /// Returns a list of all added rights with a certain localization to the database.
        /// </summary>
        /// <param name="locale">Requested localization.</param>
        /// <returns>List of all added rights with a certain localization</returns>
        List<DbRightsLocalization> GetRightsList(string locale);

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
    }
}