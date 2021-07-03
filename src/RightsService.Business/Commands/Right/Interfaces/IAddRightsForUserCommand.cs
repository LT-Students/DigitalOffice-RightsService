using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces
{
    /// <summary>
    /// Represents the command pattern.
    /// Provides a method to add rights for user.
    /// </summary>
    [AutoInject]
    public interface IAddRightsForUserCommand
    {
        /// <summary>
        /// Add rights for user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="rightsIds">List of rights.</param>
        /// <exception cref="ForbiddenException">Thrown when user does not have the necessary rights.</exception>
        OperationResultResponse<bool> Execute(Guid userId, IEnumerable<int> rightsIds);
    }
}