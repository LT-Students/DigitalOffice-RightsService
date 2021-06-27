using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.RightsService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting list of role models with pagination.
    /// </summary>
    [AutoInject]
    public interface IFindRolesCommand
    {
        /// <summary>
        /// Returns the list of role models using pagination.
        /// </summary>
        RolesResponse Execute(int skipCount, int takeCount);
    }
}
