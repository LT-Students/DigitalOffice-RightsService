using System.Collections.Generic;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Models.Dto.Responses
{
  public record RoleResponse
  {
    public RoleInfo Role { get; set; }
    public List<UserInfo> Users { get; set; }
  }
}
