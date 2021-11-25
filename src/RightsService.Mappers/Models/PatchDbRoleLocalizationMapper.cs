using System;
using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.RightsService.Mappers.Models
{
  public class PatchDbRoleLocalizationMapper : IPatchDbRoleLocalizationMapper
  {
    public JsonPatchDocument<DbRoleLocalization> Map(JsonPatchDocument<EditRoleLocalizationRequest> request)
    {
      if (request == null)
      {
        return null;
      }

      JsonPatchDocument<DbRoleLocalization> result = new JsonPatchDocument<DbRoleLocalization>();

      Func<Operation<EditRoleLocalizationRequest>, string> value = item =>
        !string.IsNullOrEmpty(item.value?.ToString().Trim())
          ? item.value.ToString().Trim() 
          : null;

      foreach (Operation<EditRoleLocalizationRequest> item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditRoleLocalizationRequest.IsActive), StringComparison.OrdinalIgnoreCase))
        {
          result.Operations.Add(new Operation<DbRoleLocalization>(item.op, item.path, item.from, bool.Parse(value(item))));
          
          continue;
        }

        result.Operations.Add(new Operation<DbRoleLocalization>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}
