using LT.DigitalOffice.Kernel.Configurations;

namespace LT.DigitalOffice.RightsService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string GetUserInfoEndpoint { get; set; }
    }
}
