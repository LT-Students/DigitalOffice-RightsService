using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Models.Dto.Models
{
    public class RoleInfo
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
