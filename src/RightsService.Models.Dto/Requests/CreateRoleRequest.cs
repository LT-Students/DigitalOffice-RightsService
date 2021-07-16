using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto
{
    public record CreateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Rights { get; set; }
    }
}