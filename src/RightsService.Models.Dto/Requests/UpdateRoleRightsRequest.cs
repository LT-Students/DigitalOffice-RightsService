using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Requests
{
  public class UpdateRoleRightsRequest
  {
    public Guid RoleId { get; set; }
    public List<int> Rights { get; set; }
  }
}
