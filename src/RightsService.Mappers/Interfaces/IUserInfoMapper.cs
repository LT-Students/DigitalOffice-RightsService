using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Mappers.Interfaces
{
    [AutoInject]
    public interface IUserInfoMapper
    {
        UserInfo Map(UserData userData);
    }
}