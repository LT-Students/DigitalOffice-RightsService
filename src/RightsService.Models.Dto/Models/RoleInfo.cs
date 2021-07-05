using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Models
{
    public record RoleInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public IEnumerable<RightResponse> Rights { get; set; }
        public IEnumerable<UserInfo> Users { get; set; }
    }
}
