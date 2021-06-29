using LT.DigitalOffice.RightsService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Responses
{
    public class RoleResponse
    {
        public int TotalCount { get; set; }
        public RoleInfo Role { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
