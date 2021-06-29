using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.RightsService.Business.Role.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for get role model.
    /// </summary>
    [AutoInject]
    public interface IGetRoleCommand
    {
        /// <summary>
        /// Returns role by id.
        /// </summary>
        RoleResponse Execute(Guid roleId);
    }
}
