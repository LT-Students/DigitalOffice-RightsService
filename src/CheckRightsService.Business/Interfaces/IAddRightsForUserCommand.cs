using LT.DigitalOffice.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business.Interfaces
{
    /// <summary>
    /// Represents the command pattern.
    /// Provides a method to add rights for user.
    /// </summary>
    public interface IAddRightsForUserCommand
    {
        /// <summary>
        /// Add rights for user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="rightsIds">List of rights.</param>
        /// <exception cref="ForbiddenException">Thrown when user does not have the necessary rights.</exception>
        void Execute(Guid userId, IEnumerable<int> rightsIds);
    }
}