using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
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