using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Constants;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class AccessValidatorConsumer : IConsumer<ICheckUserRightsRequest>
  {
    private readonly IUserRoleRepository _repository;
    private readonly IRoleRepository _roleRepository;

    private async Task<object> HasRightAsync(ICheckUserRightsRequest request)
    {
      DbUserRole dbUser = await _repository.GetAsync(request.UserId);
      
      if (dbUser is null)
      {
        return false;
      }

      DbRole dbRole = await _roleRepository.GetAsync(dbUser.RoleId);

      return dbRole?.RolesRights?.IntersectBy(request.RightIds, x => x.RightId)?.Any()
        ?? false;
    }

    public AccessValidatorConsumer(
      IUserRoleRepository repository,
      IRoleRepository roleRepository)
    {
      _repository = repository;
      _roleRepository = roleRepository;
    }

    public async Task Consume(ConsumeContext<ICheckUserRightsRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(HasRightAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
