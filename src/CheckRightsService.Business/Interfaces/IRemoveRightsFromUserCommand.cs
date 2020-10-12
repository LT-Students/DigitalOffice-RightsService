using FluentValidation;
using LT.DigitalOffice.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business.Interfaces
{
    /// <summary>
    /// Represents the command pattern.
    /// Provides method for removing rights from user.
    /// </summary>
    public interface IRemoveRightsFromUserCommand
    {
        /// <summary>
        /// Remove rights from user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="rightsIds">List of rights.</param>
        /// <exception cref="ValidationException">Thrown when user data is incorrect.</exception>
        /// <exception cref="ForbiddenException">Thrown when user does not have the necessary rights.</exception>
        void Execute(Guid userId, IEnumerable<int> rightsIds);
    }
}