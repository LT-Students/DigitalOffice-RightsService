using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Models;

namespace LT.DigitalOffice.CheckRightsService.Validator
{
    public class RemoveRightsFromUserValidator : AbstractValidator<RemoveRightsFromUserRequest>
    {
        public RemoveRightsFromUserValidator()
        {
            RuleFor(rights => rights.UserId)
                .NotEmpty()
                .WithName("User Id");

            RuleFor(rights => rights.RightIds)
                .NotEmpty()
                .WithName("Right Id");
        }
    }
}