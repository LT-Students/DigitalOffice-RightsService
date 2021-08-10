using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
    public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
    {
        private readonly IUserRepository _repository;

        public DisactivateUserConsumer(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        public Task Consume(ConsumeContext<IDisactivateUserRequest> context)
        {
            _repository.Remove(context.Message.UserId);

            return Task.FromResult(0);
        }
    }
}
