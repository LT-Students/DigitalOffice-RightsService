using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;

namespace LT.DigitalOffice.RightsService.Business.Commands.User.Interfaces
{
  [AutoInject]
  public interface IEditUserRoleCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(EditUserRoleRequest request);
  }
}
