using System;

namespace LT.DigitalOffice.RightsService.Models.Dto.Models
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
