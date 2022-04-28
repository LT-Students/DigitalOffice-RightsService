using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.RightsService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    [HttpPut("edit")]
    public async Task<OperationResultResponse<Guid?>> EditAsync(
      [FromServices] IEditUserRoleCommand command,
      [FromBody] EditUserRoleRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
