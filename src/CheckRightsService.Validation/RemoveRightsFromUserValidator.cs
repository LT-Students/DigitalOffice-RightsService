using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Models.Dto;

namespace LT.DigitalOffice.CheckRightsService.Validation
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