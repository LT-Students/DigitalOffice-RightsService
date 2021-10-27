using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class AccessValidatorConsumer : IConsumer<ICheckUserRightsRequest>
  {
    private readonly IUserRepository _repository;

    private async Task<object> HasRightAsync(ICheckUserRightsRequest request)
    {
      return await _repository.CheckRightsAsync(request.UserId, request.RightIds);
    }

    public AccessValidatorConsumer([FromServices] IUserRepository repository)
    {
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<ICheckUserRightsRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(HasRightAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
