using System;

namespace LT.DigitalOffice.RightsService.Models.Dto.Models
{
  public record RoleLocalizationInfo
  {
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
  }
}
