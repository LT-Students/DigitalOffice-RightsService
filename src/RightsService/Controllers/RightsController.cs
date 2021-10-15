using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.RightsService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class RightsController : ControllerBase
  {
    [HttpGet("get")]
    public async Task<OperationResultResponse<List<RightInfo>>> Get(
      [FromQuery] string locale,
      [FromServices] IGetRightsListCommand command)
    {
      return await command.ExecuteAsync(locale);
    }
  }
}
