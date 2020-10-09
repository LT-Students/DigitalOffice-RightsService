using LT.DigitalOffice.CheckRightsService.Models.Dto;

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
        /// <param name="request">Request with rights and user id.</param>
        /// <exception cref="Kernel.Exceptions.BadRequestException">Thrown when user data is incorrect.</exception>
        /// <exception cref="Kernel.Exceptions.ForbiddenException">Thrown when user does not have the necessary rights.</exception>
        void Execute(RemoveRightsFromUserRequest request);
    }
}