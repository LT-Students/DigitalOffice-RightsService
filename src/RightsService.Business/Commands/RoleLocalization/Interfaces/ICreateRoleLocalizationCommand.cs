using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;

namespace LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization.Interfaces
{
  [AutoInject]
  public interface ICreateRoleLocalizationCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateRoleLocalizationRequest request);
  }
}
