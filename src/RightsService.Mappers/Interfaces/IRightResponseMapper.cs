using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;

namespace LT.DigitalOffice.RightsService.Mappers.Interfaces
{
    [AutoInject]
    public interface IRightResponseMapper
    {
        RightResponse Map(DbRight value);
    }
}