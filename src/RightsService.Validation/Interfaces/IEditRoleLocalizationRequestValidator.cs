using System;
using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.RightsService.Validation.Interfaces
{
  [AutoInject]
  public interface IEditRoleLocalizationRequestValidator : IValidator<(Guid, JsonPatchDocument<EditRoleLocalizationRequest>)>
  {
  }
}
