using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.RoleLocalization.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.RightsService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class RoleLocalizationController
  {
    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditRoleLocalizationCommand command,
      [FromQuery] Guid roleLocalizationId,
      [FromBody] JsonPatchDocument<EditRoleLocalizationRequest> request)
    {
      return await command.ExecuteAsync(roleLocalizationId, request);
    }
  }
}
