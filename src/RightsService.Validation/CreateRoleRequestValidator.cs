using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Helpers;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Validation
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>, ICreateRoleRequestValidator
    {
        // TODO add check exists rights

        public CreateRoleRequestValidator(
            IRightRepository repository,
            IMemoryCache cache)
        {
            RuleFor(x => x.Name)
            .NotEmpty()
                .MinimumLength(1).WithMessage("Role name is too short.")
                .MaximumLength(100).WithMessage("Role name is too long.");

            RuleFor(x => x.Rights)
                .Cascade(CascadeMode.Stop)
                .Must(rightsIds =>
                {
                    if (rightsIds == null)
                    {
                        return true;
                    }

                    return rightsIds.All(r => r > 0);
                }).WithMessage("Right number can not be less than zero.")
                .Must(rightsIds => CheckRightsHelper.DoesExist(rightsIds, cache, repository)).WithMessage("Some rights does not exist.");
        }
    }
}
