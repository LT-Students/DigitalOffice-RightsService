using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Dto;
using System.Collections.Generic;

namespace LT.DigitalOffice.RightsService.Validation.Interfaces
{
    [AutoInject]
    public interface ICreateRoleRequestValidator : IValidator<CreateRoleRequest>
    {
    }
}
