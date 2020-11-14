using LT.DigitalOffice.Kernel.Broker;

namespace LT.DigitalOffice.CheckRightsService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqOptions
    {
        public string AuthenticationServiceValidationUrl { get; set; }
    }
}
