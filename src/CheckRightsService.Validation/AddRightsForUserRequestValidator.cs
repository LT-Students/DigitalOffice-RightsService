using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Models;

namespace LT.DigitalOffice.CheckRightsService.Validators
{
    public class AddRightsForUserValidator : AbstractValidator<AddRightsForUserRequest>
    {
        public AddRightsForUserValidator()
        {
            RuleFor(rights => rights.UserId)
                .NotEmpty()
                .WithName("User Id");

            RuleFor(rights => rights.RightsIds)
                .NotEmpty()
                .WithName("Right Id");
        }
    }
}