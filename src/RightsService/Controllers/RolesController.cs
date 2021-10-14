using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.RightsService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    [HttpGet("find")]
    public async Task<FindResultResponse<RoleInfo>> FindAsync(
      [FromServices] IFindRolesCommand command,
      [FromQuery] FindRolesFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }

    [HttpPost("create")]
    public OperationResultResponse<Guid> Create(
      [FromServices] ICreateRoleCommand command,
      [FromBody] CreateRoleRequest role)
    {
      return command.Execute(role);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<RoleResponse>> GetAsync(
      [FromServices] IGetRoleCommand command,
      [FromQuery] GetRoleFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }
  }
}
