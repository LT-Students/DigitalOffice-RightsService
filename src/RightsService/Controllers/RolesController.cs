using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Role.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace LT.DigitalOffice.RightsService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IHttpContextAccessor _context;

        public RolesController(IHttpContextAccessor context)
        {
            _context = context;
        }

        [HttpGet("find")]
        public FindResponse Find(
            [FromServices] IFindRolesCommand command,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount)
        {
            return command.Execute(skipCount, takeCount);
        }

        [HttpPost("create")]
        public OperationResultResponse<Guid> Create(
            [FromServices] ICreateRoleCommand command,
            [FromBody] CreateRoleRequest role)
        {
            var result = command.Execute(role);

            if (result.Status != Kernel.Enums.OperationResultStatusType.Failed)
            {
                _context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            }

            return result;
        }

        [HttpGet("get")]
        public RoleResponse Get(
            [FromServices] IGetRoleCommand command,
            [FromQuery] Guid roleId)
        {
            return command.Execute(roleId);
        }
    }
}