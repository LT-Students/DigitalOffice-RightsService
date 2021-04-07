using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Dto;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Business.Interfaces
{
    /// <summary>
    /// Represents the command pattern.
    /// Provides a method for getting all added rights.
    /// </summary>
    [AutoInject]
    public interface IGetRightsListCommand
    {
        /// <summary>
        /// Returns all added rights.
        /// </summary>
        /// <returns>All added rights.</returns>
        List<Right> Execute();
    }
}