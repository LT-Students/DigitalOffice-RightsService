using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Models
{
    public class RemoveRightsFromUserRequest
    {
        public Guid UserId { get; set; }
        public List<int> RightIds { get; set; }
    }
}