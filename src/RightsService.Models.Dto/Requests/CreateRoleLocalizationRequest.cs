using System;

namespace LT.DigitalOffice.RightsService.Models.Dto.Requests
{
  public record CreateRoleLocalizationRequest
  {
    public Guid? RoleId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
