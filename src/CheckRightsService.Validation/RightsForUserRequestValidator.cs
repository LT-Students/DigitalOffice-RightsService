using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Models.Dto;

namespace LT.DigitalOffice.CheckRightsService.Validation
{
    public class RightsForUserValidator : AbstractValidator<RightsForUserRequest>
    {
        public RightsForUserValidator()
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