using LT.DigitalOffice.CheckRightsService.Models;

namespace LT.DigitalOffice.CheckRightsService.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for removing rights from user.
    /// </summary>
    public interface IRemoveRightsFromUserCommand
    {
        /// <summary>
        /// Remove rights from user.
        /// </summary>
        /// <param name="request">Request with rights and user id.</param>
        void Execute(RemoveRightsFromUserRequest request);
    }
}