using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IUserRepository _repository;

    public DisactivateUserConsumer(IUserRepository userRepository)
    {
      _repository = userRepository;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await _repository.RemoveAsync(context.Message.UserId);
    }
  }
}
