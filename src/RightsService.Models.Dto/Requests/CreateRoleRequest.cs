using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.RightsService.Models.Dto.Requests
{
  public record CreateRoleRequest
  {
    [Required]
    public List<CreateRoleLocalizationRequest> Localizations { get; set; }
    public List<int> Rights { get; set; }
  }
}
