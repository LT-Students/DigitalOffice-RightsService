using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.UserRights.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.RightsService.Controllers
{
  [ApiController]
  public class UserRightsController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<bool>> Create(
      [FromServices] ICreateUserRightsCommand command,
      [FromQuery] Guid userId,
      [FromQuery] IEnumerable<int> rightsIds)
    {
      return await command.ExecuteAsync(userId, rightsIds);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> Remove(
      [FromServices] IRemoveRightsFromUserCommand command,
      [FromQuery] Guid userId,
      [FromQuery] IEnumerable<int> rightsIds)
    {
      return await command.ExecuteAsync(userId, rightsIds);
    }
  }
}
