using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.RightsService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IPatchDbRoleLocalizationMapper
  {
    JsonPatchDocument<DbRoleLocalization> Map(JsonPatchDocument<EditRoleLocalizationRequest> request);
  }
}
