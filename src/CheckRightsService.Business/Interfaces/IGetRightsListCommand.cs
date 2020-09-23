using LT.DigitalOffice.CheckRightsService.Models.Dto;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting all added rights.
    /// </summary>
    public interface IGetRightsListCommand
    {
        /// <summary>
        /// Returns all added rights.
        /// </summary>
        /// <returns>All added rights.</returns>
        List<Right> Execute();
    }
}