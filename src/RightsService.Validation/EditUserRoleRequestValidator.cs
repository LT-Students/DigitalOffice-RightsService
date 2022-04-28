using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Dto.Requests;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Validation
{
  public class EditUserRoleRequestValidator : AbstractValidator<EditUserRoleRequest>, IEditUserRoleRequestValidator
  {
    IRequestClient<ICheckUsersExistence> _rcCheckUsersExistence;
    private readonly ILogger<EditUserRoleRequestValidator> _logger;

    public EditUserRoleRequestValidator(
      IRequestClient<ICheckUsersExistence> rcCheckUsersExistence,
      ILogger<EditUserRoleRequestValidator> logger,
      IRoleRepository repository)
    {
      _rcCheckUsersExistence = rcCheckUsersExistence;
      _logger = logger;

      RuleFor(request => request)
        .MustAsync(async (request, _) =>
          await CheckUsersExistenceAsync(new List<Guid> { request.UserId }))
        .WithMessage("This user's role cannot be changed.");

      When(request =>
          request.RoleId.HasValue,
        () =>
          RuleFor(request => request.RoleId.Value)
            .MustAsync(async (id, _) => await repository.DoesExistAsync(id))
            .WithMessage("Role must exist."));
    }

    private async Task<bool> CheckUsersExistenceAsync(List<Guid> usersIds)
    {
      try
      {
        Response<IOperationResult<ICheckUsersExistence>> response =
          await _rcCheckUsersExistence.GetResponse<IOperationResult<ICheckUsersExistence>>(
            ICheckUsersExistence.CreateObj(usersIds));

        if (response.Message.IsSuccess)
        {
          return usersIds.Count == response.Message.Body.UserIds.Count;
        }

        _logger.LogWarning(
          "Error while find users Ids: {UsersIds}.\n{Errors}:",
          string.Join('\n', usersIds),
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot check existing users ids: {UsersIds}",
          string.Join('\n', usersIds));
      }

      return false;
    }
  }
}
