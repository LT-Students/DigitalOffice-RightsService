using LT.DigitalOffice.RightsService.Mappers.Models.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Models;

namespace LT.DigitalOffice.RightsService.Mappers.Models
{
  public class RightInfoMapper : IRightInfoMapper
  {
    public RightInfo Map(DbRightLocalization value)
    {
      if (value == null)
      {
        return null;
      }

      return new RightInfo
      {
        RightId = value.RightId,
        Locale = value.Locale,
        Name = value.Name,
        Description = value.Description
      };
    }
  }
}
