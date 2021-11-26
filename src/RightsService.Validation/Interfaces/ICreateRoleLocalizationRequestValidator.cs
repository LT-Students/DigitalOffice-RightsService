using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;

namespace LT.DigitalOffice.RightsService.Validation.Interfaces
{
  [AutoInject]
  public interface ICreateRoleLocalizationRequestValidator : IValidator<CreateRoleLocalizationRequest>
  {
  }
}
