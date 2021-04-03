using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using System;

namespace LT.DigitalOffice.RightsService.Mappers
{
    public class RightsMapper : IMapper<DbRight, Right>
    {
        public Right Map(DbRight value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new Right
            {
                Id = value.Id,
                Name = value.Name,
                Description = value.Description
            };
        }
    }
}