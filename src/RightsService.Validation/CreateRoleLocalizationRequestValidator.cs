using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class CreateRoleLocalizationRequestValidator : AbstractValidator<CreateRoleLocalizationRequest>, ICreateRoleLocalizationRequestValidator
  {
    public CreateRoleLocalizationRequestValidator(
      IRoleRepository roleRepository,
      IRoleLocalizationRepository localizationRepository)
    {
      RuleFor(x => x.Locale)
        .NotEmpty()
        .Length(2);

      RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(100);

      When(x => x.Name != null && x.Locale != null, () =>
      {
        RuleFor(x => x)
          .MustAsync(async (x, _) => !await localizationRepository.DoesNameExistAsync(x.Locale, x.Name))
          .WithMessage("Role name should be unique.");
      });

      When(x => x.RoleId.HasValue, () =>
      {
        RuleFor(x => x)
          .MustAsync(async (x, _) => await roleRepository.DoesExistAsync(x.RoleId.Value))
          .WithMessage("Role must exist.")
          .MustAsync(async (x, _) => !await localizationRepository.DoesLocaleExistAsync(x.RoleId.Value, x.Locale))
          .WithMessage("Role must have only one localization per locale.");
      });
    }
  }
}
