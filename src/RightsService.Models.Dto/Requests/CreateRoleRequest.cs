using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Models.Dto.Requests
{
  public record CreateRoleRequest
  {
    public List<CreateRoleLocalizationRequest> Localizations { get; set; }
    public List<int> Rights { get; set; }
  }
}
