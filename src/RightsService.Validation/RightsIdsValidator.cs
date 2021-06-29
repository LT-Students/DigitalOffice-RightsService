using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Validation.Helpers;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.RightsService.Validation
{
    public class RightsIdsValidator : AbstractValidator<IEnumerable<int>>, IRightsIdsValidator
    {
        public RightsIdsValidator(
            IRightRepository repository,
            IMemoryCache cache)
        {
            RuleFor(rightsIds => rightsIds)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rights list can not be empty")
                .Must(rightsIds =>
                {
                    return rightsIds.All(r => r > 0);
                }).WithMessage("Right number can not be less than zero.")
                .Must(rightsIds => CheckRightsHelper.DoesExist(rightsIds, cache, repository)).WithMessage("Some rights does not exist.");
        }
    }
}
