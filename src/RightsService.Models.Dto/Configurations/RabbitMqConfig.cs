using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.RightsService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        [AutoInjectRequest(typeof(IGetUserDataRequest))]
        public string GetUserInfoEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDataRequest))]
        public string GetUsersInfoEndpoint { get; set; }
    }
}
