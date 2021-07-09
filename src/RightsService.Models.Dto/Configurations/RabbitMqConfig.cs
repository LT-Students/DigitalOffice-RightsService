using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.RightsService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string GetUserRolesEndpoint { get; set; }
        public string ChangeUserRoleEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserDataRequest))]
        public string GetUserInfoEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDataRequest))]
        public string GetUsersDataEndpoint { get; set; }
    }
}
