using LT.DigitalOffice.CheckRightsService.Models.Dto;

namespace LT.DigitalOffice.CheckRightsService.Business.Interfaces
{
    /// <summary>
    /// Add rights for user.
    /// </summary>
    public interface IAddRightsForUserCommand
    {
        /// <summary>
        /// Add rights for user.
        /// </summary>
        /// <param name="request">Request with rights and user id.</param>
        void Execute(AddRightsForUserRequest request);
    }
}