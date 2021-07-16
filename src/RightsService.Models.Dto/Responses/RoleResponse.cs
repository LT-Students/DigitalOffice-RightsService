using LT.DigitalOffice.RightsService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Responses
{
    public record RoleResponse
    {
        public RoleInfo Role { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
