using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Validation.Helpers.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class EditRoleStatusRequestValidator : AbstractValidator<(Guid roleId, bool isActive)>, IEditRoleStatusRequestValidator
  {
    private readonly IRoleRepository _roleRepository;
    private readonly ICheckRightsUniquenessHelper _checkRightsUniquenessHelper;

    private async Task CheckEnablePossibilityAsync(
      (Guid roleId, bool isActive) tuple,
      CustomContext context,
      CancellationToken _)
    {
      DbRole role = await _roleRepository.GetAsync(tuple.roleId);

      if (role is null)
      {
        context.AddFailure("Can't find role with this ID.");
        return;
      }

      if (role.IsActive == tuple.isActive)
      {
        context.AddFailure("Role already has this status.");
        return;
      }

      if (tuple.isActive && !await _checkRightsUniquenessHelper.IsRightsSetUniqueAsync(role.RoleRights.Select(x => x.RightId)))
      {
        context.AddFailure("Role's set of rights are not unique.");
        return;
      }
    }

    public EditRoleStatusRequestValidator(
      IRoleRepository roleRepository,
      ICheckRightsUniquenessHelper checkRightsUniquenessHelper)
    {
      _roleRepository = roleRepository;
      _checkRightsUniquenessHelper = checkRightsUniquenessHelper;

      RuleFor(x => x)
        .CustomAsync(CheckEnablePossibilityAsync);
    }
  }
}
