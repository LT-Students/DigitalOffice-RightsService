using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;

namespace LT.DigitalOffice.RightsService.Mappers.Interfaces
{
    [AutoInject]
    public interface IRightsMapper
    {
        Right Map(DbRight value);
    }
}