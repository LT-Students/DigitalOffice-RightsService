using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.RightsService.Models.Dto.Configurations
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    public string GetUserRolesEndpoint { get; set; }
    public string CreateUserRoleEndpoint { get; set; }
    public string DisactivateUserRoleEndpoint { get; set; }
    public string ActivateUserRoleEndpoint { get; set; }
    public string FilterRolesEndpoint { get; set; }

    // users

    [AutoInjectRequest(typeof(IGetUsersDataRequest))]
    public string GetUsersDataEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICheckUsersExistence))]
    public string CheckUsersExistenceEndpoint { get; set; }
  }
}
