using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Models.Dto;

namespace LT.DigitalOffice.CheckRightsService.Validation
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