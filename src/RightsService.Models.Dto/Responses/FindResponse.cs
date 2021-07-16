using LT.DigitalOffice.RightsService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Responses
{
    public record FindResponse
    {
        public int TotalCount { get; set; }
        public List<RoleInfo> Roles { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
