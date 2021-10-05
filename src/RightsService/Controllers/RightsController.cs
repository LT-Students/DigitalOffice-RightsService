using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Right.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace LT.DigitalOffice.RightsService.Controllers
{
  [Route("[controller]")]
    [ApiController]
    public class RightsController : ControllerBase
    {
        private readonly IHttpContextAccessor _context;

        public RightsController(IHttpContextAccessor context)
        {
            _context = context;
        }

        [HttpGet("getRightsList")]
        public List<RightInfo> GetRightsList(
            [FromQuery] string locale,
            [FromServices] IGetRightsListCommand command)
        {
            return command.Execute(locale);
        }

        [HttpPost("addRightsForUser")]
        public OperationResultResponse<bool> AddRightsForUser(
            [FromServices] IAddRightsForUserCommand command,
            [FromQuery] Guid userId,
            [FromQuery] IEnumerable<int> rightsIds)
        {
            var result = command.Execute(userId, rightsIds);

            if (result.Status != Kernel.Enums.OperationResultStatusType.Failed)
            {
                _context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            }

            return result;
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