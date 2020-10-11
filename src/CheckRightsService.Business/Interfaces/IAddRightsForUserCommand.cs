using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using LT.DigitalOffice.Kernel.Exceptions;

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
        /// <param name="request">Request with rights and user id.</param>
        /// <exception cref="ValidationException">Thrown when user data is incorrect.</exception>
        /// <exception cref="ForbiddenException">Thrown when user does not have the necessary rights.</exception>
        void Execute(AddRightsForUserRequest request);
    }
}