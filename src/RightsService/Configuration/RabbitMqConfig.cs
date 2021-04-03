using LT.DigitalOffice.Kernel.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.RightsService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqOptions
    {
        public string GetUserInfoEndpoint { get; set; }
    }
}
