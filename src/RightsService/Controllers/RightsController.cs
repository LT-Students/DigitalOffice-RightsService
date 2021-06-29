using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RightsController : ControllerBase
    {
        [HttpGet("getRightsList")]
        public List<RightResponse> GetRightsList([FromServices] IGetRightsListCommand command)
        {
            return command.Execute();
        }

        [HttpPost("addRightsForUser")]
        public void AddRightsForUser(
            [FromServices] IAddRightsForUserCommand command,
            [FromQuery] Guid userId,
            [FromQuery] IEnumerable<int> rightsIds)
        {
            command.Execute(userId, rightsIds);
        }

        [HttpDelete("removeRightsFromUser")]
        public void RemoveRightsFromUser(
            [FromServices] IRemoveRightsFromUserCommand command,
            [FromQuery] Guid userId,
            [FromQuery] IEnumerable<int> rightsIds)
        {
            command.Execute(userId, rightsIds);
        }
    }
}