using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
    public class AccessCollectionValidatorConsumer : IConsumer<ICheckUserRightsRequest>
    {
        private readonly IRightRepository _repository;

        public AccessCollectionValidatorConsumer(IRightRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<ICheckUserRightsRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(HasRights, context.Message);

            await context.RespondAsync<IOperationResult<bool>>(response);
        }

        private object HasRights(ICheckUserRightsRequest request)
        {
            return _repository.CheckUserHasRights(request.UserId, request.RightIds);
        }
    }
}
