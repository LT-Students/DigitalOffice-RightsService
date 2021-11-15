using FluentValidation;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class ChangeRoleRightsRequestValidator : AbstractValidator<EditRoleRightsRequest>, IChangeRoleRightsRequestValidator
  {
    public ChangeRoleRightsRequestValidator(
      IRoleRepository roleRepository,
      IRightsIdsValidator rightsIdsValidator)
    {
      RuleFor(request => request.RoleId)
        .MustAsync(async (roleId, _) => await roleRepository.DoesRoleExistAsync(roleId))
        .WithMessage("Role doesn't exist.");

      RuleFor(request => request.Rights)
        .SetValidator(rightsIdsValidator);
    }
  }
}
