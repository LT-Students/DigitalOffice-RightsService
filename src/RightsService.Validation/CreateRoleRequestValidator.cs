﻿using FluentValidation;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>, ICreateRoleRequestValidator
  {
    public CreateRoleRequestValidator(
      IRightsIdsValidator rightsIdsValidator,
      ICreateRoleLocalizationRequestValidator localizationRequestValidator)
    {
      RuleFor(x => x.Localizations)
        .Cascade(CascadeMode.Stop)
        .NotNull().WithMessage("Localizations can't be empty.")
        .Must(x => x.Any()).WithMessage("Localizations can't be empty.")
        .Must(x => !x.GroupBy(rl => rl.Locale).Any(group => group.Count() > 1))
        .WithMessage("Role must have only one localization per locale.");

      RuleForEach(x => x.Localizations)
        .SetValidator(localizationRequestValidator);        

      RuleFor(x => x.Rights)
        .NotEmpty()
        .SetValidator(rightsIdsValidator);
    }
  }
}
