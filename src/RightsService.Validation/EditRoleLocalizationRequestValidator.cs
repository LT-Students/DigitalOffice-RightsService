using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class EditRoleLocalizationRequestValidator : BaseEditRequestValidator<EditRoleLocalizationRequest>, IEditRoleLocalizationRequestValidator
  {
    private void HandleInternalPropertyValidation(
      Operation<EditRoleLocalizationRequest> operation,
      CustomContext context)
    {
      Context = context;
      RequestedOperation = operation;

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditRoleLocalizationRequest.Name),
          nameof(EditRoleLocalizationRequest.Description),
          nameof(EditRoleLocalizationRequest.IsActive)
        });

      AddСorrectOperations(nameof(EditRoleLocalizationRequest.Name), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditRoleLocalizationRequest.Description), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditRoleLocalizationRequest.IsActive), new List<OperationType> { OperationType.Replace });

      AddFailureForPropertyIf(
        nameof(EditRoleLocalizationRequest.Name),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditRoleLocalizationRequest>, bool>, string>
        {
          {
            x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Name can't be empty."
          },
          {
            x => x.value.ToString().Trim().Length <= 100, "Name is too long."
          }
        }, 
        CascadeMode.Stop);

      AddFailureForPropertyIf(
        nameof(EditRoleLocalizationRequest.IsActive),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditRoleLocalizationRequest>, bool>, string>
        {
          {
            x => bool.TryParse(x.value?.ToString(), out _), "Incorrect isActive format."
          }
        });
    }

    public EditRoleLocalizationRequestValidator()
    {
      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }
  }
}
