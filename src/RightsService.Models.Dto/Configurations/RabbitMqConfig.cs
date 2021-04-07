using LT.DigitalOffice.Kernel.Configurations;

namespace LT.DigitalOffice.RightsService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string GetUserInfoEndpoint { get; set; }
    }
}
