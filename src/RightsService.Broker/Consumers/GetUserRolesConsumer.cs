using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using MassTransit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Broker.Consumers
{
    public class GetUserRolesConsumer : IConsumer<IGetUserRolesRequest>
    {
        private readonly IUserRepository _repository;

        private object GetRoles(IGetUserRolesRequest request)
        {
            var users = _repository.Get(request.UserIds);

            List<DbRole> roles = new();

            foreach(var user in users)
            {
                if (!roles.Contains(user.Role))
                {
                    roles.Add(user.Role);
                }
            }

            return IGetUserRolesResponse.CreateObj(
                roles.Select(r =>
                    new RoleData(
                        r.Id,
                        r.Name,
                        r.Description,
                        users.Where(u => u.RoleId == r.Id).Select(u => u.UserId).ToList()))
                .ToList());
        }

        public GetUserRolesConsumer(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        public async Task Consume(ConsumeContext<IGetUserRolesRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetRoles, context.Message);

            await context.RespondAsync<IOperationResult<IGetUserRolesResponse>>(response);
        }


    }
}
