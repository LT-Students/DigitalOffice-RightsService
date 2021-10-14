using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Business.Role.Interfaces
{
  [AutoInject]
    public interface IGetRoleCommand
    {
      Task<OperationResultResponse<RoleResponse>> ExecuteAsync(GetRoleFilter filter);
    }
}
