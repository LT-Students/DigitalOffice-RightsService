using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Helpers;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Validation
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>, ICreateRoleRequestValidator
    {
        public CreateRoleRequestValidator(
            IRightRepository repository,
            IMemoryCache cache)
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name must not be empty.");

            RuleFor(x => x.Rights)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(rightsIds => rightsIds.All(r => r > 0))
                .WithMessage("Right number can not be less than zero.")
                .Must(rightsIds => CheckRightsHelper.DoesExist(rightsIds, cache, repository))
                .WithMessage("Some rights does not exist.");
        }
    }
}
