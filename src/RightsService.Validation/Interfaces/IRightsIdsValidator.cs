using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.Interfaces
{
    [AutoInject]
    public interface IRightsIdsValidator : IValidator<IEnumerable<int>>
    {
    }
}
