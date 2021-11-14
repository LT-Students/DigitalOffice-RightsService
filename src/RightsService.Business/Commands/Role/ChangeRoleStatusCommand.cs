using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.RightsService.Business.Commands.Role.Interfaces;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Mappers.Db.Interfaces;
using LT.DigitalOffice.RightsService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.RightsService.Business.Commands.Role
{
  public class ChangeRoleStatusCommand : IChangeRoleStatusCommand
  {
    private readonly IRoleRepository _roleRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreator;

    public ChangeRoleStatusCommand(
      IRoleRepository roleRepository,
      IAccessValidator accessValidator,
      IResponseCreater responseCreator)
    {
      _roleRepository = roleRepository;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid roleId, bool isActive)
    {
      if (!await _accessValidator.IsAdminAsync())
      {return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.Forbidden,
          new List<string> { "Not enough rights" });
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _roleRepository.ChangeStatusAsync(roleId, isActive);

      if (response.Body == false)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.NotFound,
          new List<string> { "Role doesn't exist." });
      }

      return response;
    }
  }
}
