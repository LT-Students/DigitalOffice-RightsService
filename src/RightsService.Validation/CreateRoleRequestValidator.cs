using FluentValidation;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Validation
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>, ICreateRoleRequestValidator
    {
        public CreateRoleRequestValidator(
            IRightsIdsValidator rightsIdsValidator)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Role name must not be empty.")
                .MaximumLength(100);

            RuleFor(x => x.Rights)
                .SetValidator(rightsIdsValidator);
        }
    }
}
