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
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> Rights { get; set; }
        public List<Guid> Users { get; set; }
    }
}
