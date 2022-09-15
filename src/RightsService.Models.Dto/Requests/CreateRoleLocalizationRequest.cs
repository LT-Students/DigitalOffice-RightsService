using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.RightsService.Models.Dto.Requests
{
  public record CreateRoleLocalizationRequest
  {
    public Guid? RoleId { get; set; }
    [Required]
    public string Locale { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
