using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role.Interfaces
{
  [AutoInject]
  public interface IChangeRoleStatusCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid roleId, bool isActive);
  }
}
