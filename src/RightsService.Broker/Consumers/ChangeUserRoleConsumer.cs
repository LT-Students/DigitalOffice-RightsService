using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class ChangeUserRoleConsumer : IConsumer<IChangeUserRoleRequest>
  {
    private readonly IUserRepository _userRepository;

    private async Task<bool> ChangeRoleAsync(IChangeUserRoleRequest request)
    {
      await _userRepository.AssignRoleAsync(request.UserId, request.RoleId, request.ChangedBy);

      return true;
    }

    public ChangeUserRoleConsumer(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<IChangeUserRoleRequest> context)
    {
      var result = OperationResultWrapper.CreateResponse(ChangeRoleAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(result);
    }
  }
}
