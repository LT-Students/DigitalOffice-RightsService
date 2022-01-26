using System;
using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.RightsService.Validation.Interfaces
{
  [AutoInject]
  public interface IEditRoleStatusRequestValidator : IValidator<(Guid roleId, bool isActive)>
  {
  }
}
