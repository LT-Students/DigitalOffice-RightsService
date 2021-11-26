using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.RightsService.Business.Role.Interfaces
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
        Task<FindResultResponse<RoleInfo>> ExecuteAsync(FindRolesFilter filter);
    }
}
