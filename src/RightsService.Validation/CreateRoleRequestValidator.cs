using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Validation
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>, ICreateRoleRequestValidator
    {
        // TODO add check exists rights

        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty()
                .MinimumLength(1).WithMessage("Role name is too short.")
                .MaximumLength(100).WithMessage("Role name is too long.");

            RuleForEach(x => x.Rights)
                .NotEmpty();
        }
    }
}
