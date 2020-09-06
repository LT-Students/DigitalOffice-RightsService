using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Models
{
    public class AddRightsForUserRequest
    {
        public Guid UserId { get; set; }
        public IEnumerable<int> RightsIds { get; set; }
    }
}
