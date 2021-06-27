using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.RightsService.Mappers
{
    public class RightMapper : IRightMapper
    {
        public RightResponse Map(DbRight value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new RightResponse
            {
                Id = value.Id,
                Name = value.Name,
                Description = value.Description
            };
        }
    }
}