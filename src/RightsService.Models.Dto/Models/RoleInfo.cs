using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Models
{
  public record RoleInfo
  {
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public UserInfo CreatedBy { get; set; }
    public IEnumerable<RightInfo> Rights { get; set; }
    public IEnumerable<RoleLocalizationInfo> Localizations { get; set; }
  }
}
