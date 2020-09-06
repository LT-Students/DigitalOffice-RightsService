using LT.DigitalOffice.CheckRightsService.Commands.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckRightsController : ControllerBase
    {
        [HttpGet("getRightsList")]
        public List<Right> GetRightsList([FromServices] IGetRightsListCommand command)
        {
            return command.Execute();
        }

        [HttpPost("addRightsForUser")]
        public void AddRightsForUser(
            [FromServices] IAddRightsForUserCommand command,
            [FromBody] AddRightsForUserRequest request)
        {
            command.Execute(request);
        }

        [HttpDelete("removeRightsFromUser")]
        public void RemoveRightsFromUser(
            [FromServices] IRemoveRightsFromUserCommand command,
            [FromBody] RemoveRightsFromUserRequest request)
        {
            command.Execute(request);
        }
    }
}