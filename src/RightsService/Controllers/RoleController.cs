using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpGet("find")]
        public RolesResponse Find([FromServices] IFindRolesCommand command,
            [FromQuery] int skipCount,
            [FromQuery] int takeCount)
        {
            return command.Execute(skipCount, takeCount);
        }

        [HttpPost("create")]
        public void Create(
            [FromServices] ICreateRoleCommand command,
            [FromQuery] CreateRoleRequest role)
        {
            command.Execute(role);
        }
    }
}