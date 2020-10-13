using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService.Validation
{
    public class RightsIdsValidator : AbstractValidator<IEnumerable<int>>
    {
        public RightsIdsValidator([FromServices] ICheckRightsRepository repository)
        {
            RuleFor(rightsIds => rightsIds)
                .NotEmpty().WithMessage("Rights list can not be empty");

            RuleForEach(rightsIds => rightsIds)
                .GreaterThan(-1).WithMessage("Right number can not be less than zero.")
                .Must(right => DoesRightExist(repository, right)).WithMessage("Some rights does not exist.");
        }

        private bool DoesRightExist(ICheckRightsRepository repository, int rightId)
        {
            return repository.DoesRightExist(rightId);
        }
    }
}
