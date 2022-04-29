using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.RightsService.Broker.Requests.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class EditUserRoleRequestValidator : AbstractValidator<EditUserRoleRequest>, IEditUserRoleRequestValidator
  {
    public EditUserRoleRequestValidator(
      IRoleRepository repository,
      IUserService userService)
    {
      RuleFor(x => x.UserId)
        .MustAsync(async (request, _) => (await userService.CheckUsersExistence(new List<Guid> { request }, new List<string>())).Count == 1)
        .WithMessage("User does not exist.");

      When(request =>
          request.RoleId.HasValue,
        () =>
          RuleFor(request => request.RoleId.Value)
            .MustAsync(async (id, _) => await repository.DoesExistAsync(id))
            .WithMessage("Role must exist."));
    }
  }
}
